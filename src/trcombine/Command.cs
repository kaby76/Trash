using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XQuery.DataModel;
using XQuery.Parser;

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

        var name = parser.Name.EndsWith("Parser", StringComparison.Ordinal)
            ? parser.Name[..^"Parser".Length] : parser.Name;
        if (name.Length == 0) throw new InvalidDataException("Parser grammar has no usable combined name.");
        var context = new EvaluationContext().WithContextItem(AdapterDocument.Build(parser.Result.Nodes));
        context.SetVariable("combined-name", new XdmSequence(new XdmAtomicValue(name)));
        context.SetVariable("lexer-rules", new XdmSequence(new XdmAtomicValue(lexer.RulesAndModes)));
        using var script = new StreamReader(GetType().Assembly.GetManifestResourceStream("trcombine.combine.xq")!);
        new ParseTreeUpdateEvaluator(context).EvaluateModule(new XQueryParser(script.ReadToEnd()).ParseModule());

        var directory = input.IsBundle
            ? DirectoryOf(input.ResultArtifactNames[Array.IndexOf(input.Results, parser.Result)]) : "";
        var outputName = ArtifactBundle.ValidateMemberName(directory + name + ".g4.pt");
        var result = new ParsingResultSet
        {
            FileName = name + ".g4", Nodes = parser.Result.Nodes,
            Lexer = parser.Result.Lexer, Parser = parser.Result.Parser,
            StartSymbol = parser.Result.StartSymbol, MetaStartSymbol = parser.Result.MetaStartSymbol,
        };
        var removed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in input.ResultArtifactNames)
        {
            removed.Add(member);
            removed.Add(ArtifactBundle.ChangeExtension(member, ""));
            removed.Add(ArtifactBundle.ChangeExtension(member, ".errors"));
        }
        var output = input.Artifacts.Where(a => !removed.Contains(a.Name)).ToList();
        output.Add(new Artifact(outputName, ArtifactBundle.SerializeParsingResult(result, config.Format)));
        output.Add(new Artifact(directory + name + ".g4.errors", Array.Empty<byte>()));
        using var stdout = Console.OpenStandardOutput();
        ArtifactBundle.Write(stdout, output);
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
        public string RulesAndModes { get; }

        public Grammar(ParsingResultSet result)
        {
            Result = result;
            var root = result.Nodes.OfType<UnvParseTreeElement>()
                .SingleOrDefault(n => n.LocalName == "grammarSpec")
                ?? throw new InvalidDataException($"'{result.FileName}' is not an ANTLRv4 grammar tree.");
            var declaration = Child(root, "grammarDecl")!;
            Kind = Text(Child(declaration, "grammarType")!)
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)[0];
            Name = Text(Child(declaration, "identifier")!).Trim();
            var rules = Child(root, "rules")!;
            RulesAndModes = "\n" + Text(rules).Trim() + "\n"
                + string.Concat(Children(root, "modeSpec").Select(Text));
            var option = Children(root, "prequelConstruct")
                .SelectMany(p => Children(Child(p, "optionsSpec"), "option"))
                .FirstOrDefault(o => Text(Child(o, "identifier")!).Trim() == "tokenVocab");
            TokenVocabulary = option == null ? null : Text(Child(option, "optionValue")!).Trim();
        }

        private static UnvParseTreeElement? Child(UnvParseTreeElement? node, string name) =>
            Children(node, name).FirstOrDefault();

        private static IEnumerable<UnvParseTreeElement> Children(UnvParseTreeElement? node, string name) =>
            node?.AllChildren.OfType<UnvParseTreeElement>().Where(n => n.LocalName == name)
                ?? Enumerable.Empty<UnvParseTreeElement>();

        private static string Text(UnvParseTreeNode node)
        {
            var text = new StringBuilder();
            var stack = new Stack<UnvParseTreeNode>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current is UnvParseTreeText token) text.Append(token.Data);
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
