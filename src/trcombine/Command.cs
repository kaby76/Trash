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
        var grammars = input.Results.Select(result => new Grammar(result)).ToArray();
        var byName = new Dictionary<string, Grammar>(StringComparer.Ordinal);
        foreach (var grammar in grammars)
            if (!byName.TryAdd(grammar.Name, grammar))
                throw new InvalidDataException($"Duplicate grammar name '{grammar.Name}'.");
        foreach (var grammar in grammars)
        foreach (var import in grammar.Imports)
            if (!byName.ContainsKey(import))
                throw new InvalidDataException(
                    $"Imported grammar '{import}' for '{grammar.Name}' was not supplied.");
        var roots = TopLevelGrammars(grammars, byName);
        var parsers = roots.Where(g => g.Kind == "parser").ToArray();
        var lexers = roots.Where(g => g.Kind == "lexer").ToArray();
        if (parsers.Length != 1 || lexers.Length != 1 || roots.Length != 2)
            throw new InvalidDataException(
                "combine requires one top-level parser grammar and one top-level lexer grammar; " +
                "found: " + (roots.Length == 0 ? "none" :
                    string.Join(", ", roots.Select(g => $"{g.Name} ({g.Kind})"))) + ".");
        var parser = parsers[0];
        var lexer = lexers[0];
        if (parser.TokenVocabulary != null && parser.TokenVocabulary != lexer.Name)
            throw new InvalidDataException($"Parser tokenVocab '{parser.TokenVocabulary}' does not name lexer '{lexer.Name}'.");
        var parserOrder = ImportOrder(parser, byName);
        var lexerOrder = ImportOrder(lexer, byName);
        var selected = new HashSet<Grammar>(parserOrder.Concat(lexerOrder));
        if (selected.Count != grammars.Length)
            throw new InvalidDataException("Some supplied grammars are not reachable from the top-level parser or lexer.");

        var name = parser.Name.EndsWith("Parser", StringComparison.Ordinal)
            ? parser.Name[..^"Parser".Length] : parser.Name;
        if (name.Length == 0) throw new InvalidDataException("Parser grammar has no usable combined name.");
        var source = Combine(parserOrder, lexerOrder, name);

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

    // Topologically sort importer -> dependency edges. Zero-indegree nodes
    // are the top-level grammars; an incomplete sort reports import cycles.
    private static Grammar[] TopLevelGrammars(
        IReadOnlyList<Grammar> grammars, IReadOnlyDictionary<string, Grammar> byName)
    {
        var indegree = grammars.ToDictionary(g => g.Name, _ => 0, StringComparer.Ordinal);
        foreach (var grammar in grammars)
        foreach (var name in grammar.Imports)
            indegree[name]++;
        var roots = grammars.Where(g => indegree[g.Name] == 0).ToArray();
        var ready = new Queue<Grammar>(roots);
        var count = 0;
        while (ready.Count > 0)
        {
            var current = ready.Dequeue();
            count++;
            foreach (var name in current.Imports)
                if (--indegree[name] == 0)
                    ready.Enqueue(byName[name]);
        }
        if (count != grammars.Count)
            throw new InvalidDataException("Cyclic grammar imports prevent topological ordering.");
        return roots;
    }

    // ANTLR gives an importing grammar's rules precedence over its delegates.
    // Visit each importer before its dependencies, in declared import order.
    private static IReadOnlyList<Grammar> ImportOrder(
        Grammar root, IReadOnlyDictionary<string, Grammar> byName)
    {
        var order = new List<Grammar>();
        var active = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        void Visit(Grammar grammar)
        {
            if (active.Contains(grammar.Name))
                throw new InvalidDataException($"Cyclic grammar import involving '{grammar.Name}'.");
            if (!visited.Add(grammar.Name)) return;
            active.Add(grammar.Name);
            order.Add(grammar);
            foreach (var name in grammar.Imports)
            {
                var dependency = byName[name];
                if (dependency.Kind != grammar.Kind)
                    throw new InvalidDataException(
                        $"Grammar '{grammar.Name}' cannot import {dependency.Kind} grammar '{name}'.");
                Visit(dependency);
            }
            active.Remove(grammar.Name);
        }
        Visit(root);
        return order;
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

    private static string Combine(
        IReadOnlyList<Grammar> parserOrder, IReadOnlyList<Grammar> lexerOrder, string name)
    {
        var parser = parserOrder[0];
        var lexer = lexerOrder[0];
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        // ANTLR ignores options declared by imported grammars.
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
        foreach (var grammar in parserOrder.Concat(lexerOrder))
        foreach (var prequel in grammar.Prequels)
            if (Grammar.Child(prequel, "optionsSpec") == null &&
                Grammar.Child(prequel, "delegateGrammars") == null)
                combined.Append(Grammar.Text(prequel));
        var ruleNames = new HashSet<string>(StringComparer.Ordinal);
        string RulesText(IEnumerable<Grammar> order)
        {
            var text = new StringBuilder();
            foreach (var grammar in order)
            foreach (var rule in Grammar.Children(grammar.Rules, "ruleSpec"))
            {
                if (!ruleNames.Add(Grammar.RuleName(rule))) continue;
                text.Append(Grammar.Text(rule));
            }
            return text.ToString();
        }
        combined.Append(RulesText(parserOrder).TrimEnd());
        combined.Append('\n').Append(RulesText(lexerOrder).TrimStart('\r', '\n'));
        // Modes are retained in the editable .pt result. ANTLR's code generator
        // requires a lexer grammar for them; an XQuery pass can transform them.
        foreach (var grammar in lexerOrder)
        foreach (var mode in grammar.Modes)
            combined.Append(Grammar.Text(mode));
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
        public string[] Imports { get; }

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
            Imports = Prequels.SelectMany(p => Children(Child(p, "delegateGrammars"), "delegateGrammar"))
                .Select(g => Children(g, "identifier").LastOrDefault())
                .Where(n => n != null).Select(n => Text(n!).Trim()).ToArray();
            var option = Options.FirstOrDefault(o => Text(Child(o, "identifier")!).Trim() == "tokenVocab");
            TokenVocabulary = option == null ? null : Text(Child(option, "optionValue")!).Trim();
        }

        public static UnvParseTreeElement? Child(UnvParseTreeElement? node, string name) =>
            Children(node, name).FirstOrDefault();

        public static IEnumerable<UnvParseTreeElement> Children(UnvParseTreeElement? node, string name) =>
            node?.AllChildren.OfType<UnvParseTreeElement>().Where(n => n.LocalName == name)
                ?? Enumerable.Empty<UnvParseTreeElement>();

        public static string RuleName(UnvParseTreeElement rule)
        {
            var definition = Child(rule, "parserRuleSpec") ?? Child(rule, "lexerRuleSpec")
                ?? throw new InvalidDataException("Invalid grammar rule.");
            var name = Child(definition, "RULE_REF") ?? Child(definition, "TOKEN_REF")
                ?? throw new InvalidDataException("Grammar rule has no name.");
            return Text(name).Trim();
        }

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
