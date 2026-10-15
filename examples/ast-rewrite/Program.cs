using System.Text.Json;
using System.Text.Json.Serialization;
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

public sealed record PatternPart(string? Capture, string? NodeName, string? Literal);
public abstract record Template;
public sealed record CaptureTemplate(string Name) : Template;
public sealed record NodeTemplate(string Kind, string? TextCapture, Template[] Children) : Template;
public sealed record MappingRule(string RuleName, PatternPart[] Pattern, Template Result);
public sealed record Mapping(string Name, bool SingleChild, bool ErrorOtherwise, MappingRule[] Rules);

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
        var transformed = children.Select(c => Rewrite(c, mapping)).ToArray();
        foreach (var rule in mapping.Rules)
        {
            if (rule.RuleName != source.LocalName || rule.Pattern.Length != children.Length)
                continue;
            var captures = new Dictionary<string, AstNode>(StringComparer.Ordinal);
            var matched = true;
            for (var i = 0; i < children.Length; i++)
            {
                var part = rule.Pattern[i];
                var child = children[i];
                if (part.NodeName is not null && part.NodeName != child.LocalName ||
                    part.Literal is not null && part.Literal != child.GetText())
                {
                    matched = false;
                    break;
                }
                if (part.Capture is not null)
                    captures.Add(part.Capture, transformed[i]);
            }
            if (matched)
                return Instantiate(rule.Result, captures);
        }
        if (mapping.SingleChild && transformed.Length == 1)
            return transformed[0];
        if (mapping.ErrorOtherwise)
            throw new InvalidOperationException($"No AST rewrite for {source.LocalName}({string.Join(", ", children.Select(c => c.LocalName))}).");
        return new AstNode(source.LocalName, Children: transformed);
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

public sealed record Lexeme(string Text, int Offset);

public sealed class MappingParser
{
    private readonly List<Lexeme> _tokens;
    private int _position;

    private MappingParser(string input) => _tokens = Tokenize(input);
    public static Mapping Parse(string input) => new MappingParser(input).ReadMapping();

    private Mapping ReadMapping()
    {
        Expect("ast");
        var name = Identifier();
        Expect("{");
        var single = false;
        var error = false;
        var rules = new List<MappingRule>();
        while (!Take("}"))
        {
            if (Take("default"))
            {
                if (Take("single-child"))
                {
                    Expect("->"); Expect("child"); Expect(";");
                    single = true;
                }
                else
                {
                    Expect("otherwise"); Expect("->"); Expect("error"); Expect(";");
                    error = true;
                }
                continue;
            }
            var ruleName = Identifier();
            Expect("(");
            var parts = new List<PatternPart>();
            if (!Take(")"))
            {
                do { parts.Add(ReadPatternPart()); } while (Take(","));
                Expect(")");
            }
            Expect("->");
            var replacement = ReadTemplate();
            Expect(";");
            rules.Add(new MappingRule(ruleName, parts.ToArray(), replacement));
        }
        if (_position != _tokens.Count)
            throw Error("Unexpected text after mapping.");
        return new Mapping(name, single, error, rules.ToArray());
    }

    private PatternPart ReadPatternPart()
    {
        if (PeekQuoted())
            return new PatternPart(null, null, Unquote(Advance().Text));
        var first = Identifier();
        if (!Take(":")) return new PatternPart(null, first, null);
        if (PeekQuoted()) return new PatternPart(first, null, Unquote(Advance().Text));
        return new PatternPart(first, Identifier(), null);
    }

    private Template ReadTemplate()
    {
        if (!Take("^"))
            return new CaptureTemplate(Identifier());
        Expect("(");
        var kind = Identifier();
        string? textCapture = null;
        if (Take("["))
        {
            textCapture = Identifier();
            Expect("]");
        }
        var children = new List<Template>();
        while (!Take(")"))
            children.Add(ReadTemplate());
        return new NodeTemplate(kind, textCapture, children.ToArray());
    }

    private bool PeekQuoted() => _position < _tokens.Count && _tokens[_position].Text.StartsWith('\'');
    private static string Unquote(string token) => token[1..^1].Replace("\\'", "'").Replace("\\\\", "\\");
    private string Identifier()
    {
        var token = Advance().Text;
        if (token.Length == 0 || !char.IsLetter(token[0]) && token[0] != '_')
            throw Error($"Expected identifier, got '{token}'.");
        return token;
    }
    private void Expect(string value)
    {
        if (!Take(value)) throw Error($"Expected '{value}'.");
    }
    private bool Take(string value)
    {
        if (_position >= _tokens.Count || _tokens[_position].Text != value) return false;
        _position++;
        return true;
    }
    private Lexeme Advance() => _position < _tokens.Count
        ? _tokens[_position++] : throw Error("Unexpected end of mapping.");
    private FormatException Error(string message) => new($"{message} At offset {(_position < _tokens.Count ? _tokens[_position].Offset : -1)}.");

    private static List<Lexeme> Tokenize(string text)
    {
        var tokens = new List<Lexeme>();
        for (var i = 0; i < text.Length;)
        {
            if (char.IsWhiteSpace(text[i])) { i++; continue; }
            if (i + 1 < text.Length && text[i..(i + 2)] == "//")
            {
                while (i < text.Length && text[i] != '\n') i++;
                continue;
            }
            var start = i;
            if (i + 1 < text.Length && text[i..(i + 2)] == "->") i += 2;
            else if (text[i] == '\'')
            {
                i++;
                while (i < text.Length && text[i] != '\'')
                {
                    if (text[i] == '\\') i++;
                    if (i >= text.Length) break;
                    i++;
                }
                if (i >= text.Length) throw new FormatException($"Unterminated string at offset {start}.");
                i++;
            }
            else if (char.IsLetter(text[i]) || text[i] == '_')
            {
                do { i++; } while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] is '_' or '-'));
            }
            else if ("{}(),;:^[]".Contains(text[i])) i++;
            else throw new FormatException($"Unexpected character '{text[i]}' at offset {i}.");
            tokens.Add(new Lexeme(text[start..i], start));
        }
        return tokens;
    }
}
