using System.Text.Json;
using System.Text.Json.Serialization;
using AllStarAtnParser;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;

// Prototype CLI: dotnet run --project AstRewrite.csproj -- arithmetic.ast [input.pt]
// With no input file, reads a legacy .pt stream or PAX/tar bundle from stdin.
try
{
    if (args.Length is < 1 or > 2)
        throw new ArgumentException("Usage: AstRewrite <mapping.ast> [parse-results.pt|bundle.tar]");
    var mapping = MappingParser.Parse(File.ReadAllText(args[0]));
    var input = ParsingResultIO.Read(args.Length == 2 ? args[1] : null);
    var outputs = new List<(string file, AstNode ast)>();
    foreach (var result in input.Results)
    {
        var roots = result.Nodes.OfType<UnvParseTreeElement>().ToArray();
        if (roots.Length != 1)
            throw new InvalidOperationException($"{result.FileName}: expected one parse-tree root, found {roots.Length}.");
        var ast = Rewriter.Rewrite(roots[0], mapping);
        outputs.Add((result.FileName, ast));
    }
    var options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    // One input has a directly usable AST; multiple inputs retain filenames.
    if (outputs.Count == 1)
        Console.WriteLine(JsonSerializer.Serialize(outputs[0].ast, options));
    else
        Console.WriteLine(JsonSerializer.Serialize(outputs.Select(o => new { o.file, o.ast }), options));
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ast-rewrite: {ex.Message}");
    Environment.ExitCode = 1;
}

public sealed record AstNode(
    [property: JsonPropertyName("kind")] string Kind,
    [property: JsonPropertyName("text")] string? Text = null,
    [property: JsonPropertyName("children")] AstNode[]? Children = null);

public sealed record CstChild(string Name, string Text, AstNode Ast);

public static class Rewriter
{
    public static AstNode Rewrite(UnvParseTreeElement source, Mapping mapping)
    {
        var children = source.Children.ToArray(); // Attributes and text live under terminal wrappers.
        if (source.RuleIndex < 0)
        {
            var text = children.Length == 0
                ? source.GetChildrenText().FirstOrDefault() ?? (source.LocalName == "EOF" ? "<EOF>" : "")
                : source.GetText();
            return new AstNode(source.LocalName, text);
        }

        // Transform children before applying a parent rewrite. This makes captures
        // refer to ASTs, while patterns still inspect the original CST names/text.
        var transformed = children.Select(c =>
            new CstChild(c.LocalName, c.GetText(), Rewrite(c, mapping))).ToArray();
        return RewriteChildren(source.LocalName, transformed, mapping);
    }

    public static AstNode RewriteChildren(string ruleName,
        IReadOnlyList<CstChild> children, Mapping mapping)
    {
        foreach (var rule in mapping.Rules)
        {
            if (rule.RuleName != ruleName || rule.Pattern.Length != children.Count)
                continue;
            var captures = new Dictionary<string, AstNode>(StringComparer.Ordinal);
            var matched = true;
            for (var i = 0; i < children.Count; i++)
            {
                var part = rule.Pattern[i];
                var child = children[i];
                if (part.NodeName is not null && part.NodeName != child.Name ||
                    part.Literal is not null && part.Literal != child.Text)
                {
                    matched = false;
                    break;
                }
                if (part.Capture is not null)
                    captures.Add(part.Capture, child.Ast);
            }
            if (matched)
                return Instantiate(rule.Result, captures);
        }
        if (mapping.SingleChild && children.Count == 1)
            return children[0].Ast;
        if (mapping.ErrorOtherwise)
            throw new InvalidOperationException($"No AST rewrite for {ruleName}({string.Join(", ", children.Select(c => c.Name))}).");
        return new AstNode(ruleName, Children: children.Select(c => c.Ast).ToArray());
    }

    private static AstNode Instantiate(Template template, IReadOnlyDictionary<string, AstNode> captures) => template switch
    {
        CaptureTemplate capture => captures.TryGetValue(capture.Name, out var ast)
            ? ast : throw new InvalidOperationException($"Unknown capture '{capture.Name}'."),
        NodeTemplate node => new AstNode(node.Kind,
            node.TextCapture is null ? null : captures.TryGetValue(node.TextCapture, out var value)
                ? value.Text : throw new InvalidOperationException($"Unknown text capture '{node.TextCapture}'."),
            node.Children.Length == 0 ? null : node.Children.Select(child => Instantiate(child, captures)).ToArray()),
        _ => throw new InvalidOperationException("Unknown AST template.")
    };
}
