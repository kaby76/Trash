using AntlrJson;
using Antlr4.Runtime;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trash;

class Command
{
    public string Help()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("trcombine.readme.md")!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Execute(Config config)
    {
        if (config.Inputs.Any())
            throw new ArgumentException(
                "combine reads parsed grammars from stdin or -f FILE; pipe 'dotnet trash parse ...' into it.");
        var input = ParsingResultIO.Read(config.File);
        if (input.Results.Length != 2)
            throw new InvalidDataException("combine requires exactly one parser grammar and one lexer grammar.");
        var grammars = input.Results.Select(result => new Grammar(result)).ToArray();
        var parser = grammars.SingleOrDefault(g => g.Kind == "parser");
        var lexer = grammars.SingleOrDefault(g => g.Kind == "lexer");
        if (parser == null || lexer == null)
            throw new InvalidDataException("combine requires one parser grammar and one lexer grammar.");
        if (parser.TokenVocabulary != null && parser.TokenVocabulary != lexer.Name)
            throw new InvalidDataException($"Parser tokenVocab '{parser.TokenVocabulary}' does not name lexer '{lexer.Name}'.");
        if (lexer.Modes.Any())
            throw new InvalidDataException("ANTLR4 combined grammars do not support lexer modes.");

        var name = parser.Name.EndsWith("Parser", StringComparison.Ordinal)
            ? parser.Name[..^"Parser".Length] : parser.Name;
        if (name.Length == 0) throw new InvalidDataException("Parser grammar has no usable combined name.");
        var source = Combine(parser, lexer, name);

        var directory = input.IsBundle
            ? DirectoryOf(input.ResultArtifactNames[Array.IndexOf(input.Results, parser.Result)]) : "";
        var outputName = ArtifactBundle.ValidateMemberName(directory + name + ".g4.pt");
        var result = ParseCombined(source, name + ".g4");
        var removed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in input.ResultArtifactNames)
        {
            removed.Add(member);
            removed.Add(ArtifactBundle.ChangeExtension(member, ""));
            removed.Add(ArtifactBundle.ChangeExtension(member, ".errors"));
        }
        var output = input.Artifacts.Where(a => !removed.Contains(a.Name)).ToList();
        output.Add(new Artifact(outputName, ArtifactBundle.SerializeParsingResult(result, config.Format)));
        using var stdout = Console.OpenStandardOutput();
        ArtifactBundle.Write(stdout, output);
    }

    private static ParsingResultSet ParseCombined(string source, string fileName)
    {
        global::Program.SetupParse2(source, fileName, quiet: true);
        var parsed = global::Program.Parse2();
        if (global::Program.AnyErrors() || parsed.Count != 1)
            throw new InvalidDataException($"Combined grammar '{fileName}' is not valid ANTLRv4 syntax.");
        var tokenStream = global::Program.TokenStream as CommonTokenStream
            ?? throw new InvalidDataException("ANTLRv4 parser did not provide a common token stream.");
        var root = new ConvertToDOM().BottomUpConvert(parsed[0].Item2, null!,
            global::Program.Parser, global::Program.Lexer, tokenStream);
        return new ParsingResultSet
        {
            FileName = fileName,
            Nodes = new UnvParseTreeNode[] { root },
            Lexer = global::Program.Lexer,
            Parser = global::Program.Parser,
            StartSymbol = "grammarSpec",
        };
    }

    private static string Combine(Grammar parser, Grammar lexer, string name)
    {
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var grammar in new[] { parser, lexer })
        foreach (var option in grammar.Options)
        {
            var optionName = Grammar.Text(Grammar.Child(option, "identifier")!).Trim();
            if (optionName == "tokenVocab") continue;
            var value = Grammar.Text(Grammar.Child(option, "optionValue")!).Trim();
            if (options.TryGetValue(optionName, out var prior) && prior != value)
                throw new InvalidDataException($"Conflicting '{optionName}' options in parser and lexer grammars.");
            options[optionName] = value;
        }

        var replacements = new Dictionary<UnvParseTreeNode, string>
        {
            [Grammar.Child(parser.GrammarType, "PARSER")!] = "",
            [Grammar.Child(parser.Declaration, "identifier")!.AllChildren
                .OfType<UnvParseTreeElement>().First(n => n.LocalName is "TOKEN_REF" or "RULE_REF")] = name,
        };
        var typeChildren = parser.GrammarType.AllChildren.OfType<UnvParseTreeNode>().ToArray();
        var parserKeyword = Array.FindIndex(typeChildren, n => n is UnvParseTreeElement e && e.LocalName == "PARSER");
        if (parserKeyword >= 0 && parserKeyword + 1 < typeChildren.Length &&
            typeChildren[parserKeyword + 1] is UnvParseTreeAttr { Name: "WS" } whitespace)
            replacements[whitespace] = "";
        var combined = new StringBuilder(Grammar.Text(parser.Declaration, replacements));
        if (options.Count > 0)
        {
            combined.Append("\noptions {\n");
            foreach (var option in options)
                combined.Append("    ").Append(option.Key).Append(" = ").Append(option.Value).Append(";\n");
            combined.Append("}\n");
        }
        foreach (var grammar in new[] { parser, lexer })
        foreach (var prequel in grammar.Prequels)
            if (Grammar.Child(prequel, "optionsSpec") == null)
                combined.Append(Grammar.Text(prequel));
        combined.Append(Grammar.Text(parser.Rules).TrimEnd());
        combined.Append('\n').Append(Grammar.Text(lexer.Rules).TrimStart('\r', '\n'));
        return combined.ToString().TrimEnd() + "\n";
    }

    private static string DirectoryOf(string member)
    {
        var slash = member.LastIndexOf('/');
        return slash < 0 ? "" : member[..(slash + 1)];
    }

    private sealed class Grammar
    {
        public ParsingResultSet Result { get; }
        public string Kind { get; }
        public string Name { get; }
        public string? TokenVocabulary { get; }
        public UnvParseTreeElement Declaration { get; }
        public UnvParseTreeElement GrammarType { get; }
        public UnvParseTreeElement Rules { get; }
        public UnvParseTreeElement[] Prequels { get; }
        public UnvParseTreeElement[] Modes { get; }
        public UnvParseTreeElement[] Options { get; }

        public Grammar(ParsingResultSet result)
        {
            Result = result;
            var root = result.Nodes.OfType<UnvParseTreeElement>()
                .SingleOrDefault(n => n.LocalName == "grammarSpec")
                ?? throw new InvalidDataException($"'{result.FileName}' is not an ANTLRv4 grammar tree.");
            Declaration = Child(root, "grammarDecl")!;
            GrammarType = Child(Declaration, "grammarType")!;
            Kind = Child(GrammarType, "PARSER") != null ? "parser"
                : Child(GrammarType, "LEXER") != null ? "lexer" : "combined";
            Name = Text(Child(Declaration, "identifier")!).Trim();
            Rules = Child(root, "rules")!;
            Prequels = Children(root, "prequelConstruct").ToArray();
            Modes = Children(root, "modeSpec").ToArray();
            Options = Prequels.SelectMany(p => Children(Child(p, "optionsSpec"), "option")).ToArray();
            var option = Options.FirstOrDefault(o => Text(Child(o, "identifier")!).Trim() == "tokenVocab");
            TokenVocabulary = option == null ? null : Text(Child(option, "optionValue")!).Trim();
        }

        public static UnvParseTreeElement? Child(UnvParseTreeElement? node, string name) =>
            Children(node, name).FirstOrDefault();

        private static IEnumerable<UnvParseTreeElement> Children(UnvParseTreeElement? node, string name) =>
            node?.AllChildren.OfType<UnvParseTreeElement>().Where(n => n.LocalName == name)
                ?? Enumerable.Empty<UnvParseTreeElement>();

        public static string Text(UnvParseTreeNode node, IReadOnlyDictionary<UnvParseTreeNode, string>? replacements = null)
        {
            var text = new StringBuilder();
            var stack = new Stack<UnvParseTreeNode>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (replacements != null && replacements.TryGetValue(current, out var value))
                    text.Append(value);
                else if (current is UnvParseTreeText token) text.Append(token.Data);
                else if (current is UnvParseTreeAttr attribute &&
                         attribute.Name is not ("Line" or "Column" or "ChildCount"))
                    text.Append(attribute.StringValue);
                else if (current is UnvParseTreeElement element)
                {
                    var children = element.AllChildren.OfType<UnvParseTreeNode>().ToArray();
                    for (var index = children.Length - 1; index >= 0; index--)
                        stack.Push(children[index]);
                }
            }
            return text.ToString();
        }
    }
}
