using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using XQuery.DataModel;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trunfoldlit.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
                System.Console.Error.WriteLine("reading from stdin");

            for (;;)
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }

            lines = lines.Trim();
        }
        else
        {
            if (config.Verbose)
                System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");

            lines = File.ReadAllText(config.File);
        }

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
        var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");

        // Build one combined AdapterDocument from all trees across all parse_infos.
        // This enables cross-document querying: token names used in parser rules in
        // one file can be matched against lexer rules defined in a separate file
        // (e.g. a lexer grammar with modes).
        var allTrees = data.SelectMany(pi => pi.Nodes).ToList();
        var adapterDoc = AdapterDocument.Build(allTrees);

        var evaluator = new XPathEvaluator();

        // ── Step 1: find every TOKEN_REF appearing inside a parser rule. ──────────
        var expr1 = "//parserRuleSpec//atom/terminalDef/TOKEN_REF";
        if (config.Verbose) System.Console.Error.WriteLine("Expr1 = >>>" + expr1 + "<<<");
        var exprNode1 = new XPathParser(expr1).Parse();
        var tokenRefItems = evaluator.Evaluate(exprNode1, adapterDoc).ToList();
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + tokenRefItems.Count + " TOKEN_REF nodes.");

        // ── Step 2: collect all non-fragment lexer rules whose body is exactly one
        //           string literal, then keep only those with a *unique* literal
        //           across all such rules (rules in different lexer modes can share
        //           the same literal, so those must be excluded). ──────────────────
        var expr2 =
            "//lexerRuleSpec[not(FRAGMENT)]" +
            "[lexerRuleBlock/lexerAltList[count(*) = 1]/lexerAlt[count(*) = 1]" +
            "/lexerElements[count(*) = 1]/lexerElement[count(*) = 1]" +
            "/lexerAtom/terminalDef[count(*) = 1]/STRING_LITERAL]";
        if (config.Verbose) System.Console.Error.WriteLine("Expr2 = >>>" + expr2 + "<<<");
        var exprNode2 = new XPathParser(expr2).Parse();
        var lexerSpecItems = evaluator.Evaluate(exprNode2, adapterDoc).ToList();
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + lexerSpecItems.Count + " candidate lexer rules.");

        // For each qualifying lexerRuleSpec, extract the token name, the string
        // literal text, and the lexerRuleBlock node that will be used for replacement.
        var candidates = new List<(string name, string lit, UnvParseTreeElement block)>();
        foreach (var specItem in lexerSpecItems)
        {
            if (specItem is not AdapterElement specAe) continue;

            var tokenRefAe = specAe.Children.OfType<AdapterElement>()
                .FirstOrDefault(c => c.NodeName?.LocalName == "TOKEN_REF");
            var blockAe = specAe.Children.OfType<AdapterElement>()
                .FirstOrDefault(c => c.NodeName?.LocalName == "lexerRuleBlock");
            if (tokenRefAe == null || blockAe == null) continue;

            var litAe = DescendChild(blockAe,
                "lexerAltList", "lexerAlt", "lexerElements",
                "lexerElement", "lexerAtom", "terminalDef", "STRING_LITERAL");
            if (litAe == null) continue;

            candidates.Add((tokenRefAe.StringValue, litAe.StringValue, blockAe.Source));
        }

        // Only inline tokens whose string literal is unique across all candidates.
        var uniqueRules = candidates
            .GroupBy(c => c.lit)
            .Where(g => g.Count() == 1)
            .ToDictionary(g => g.First().name, g => g.First().block);

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine(
            uniqueRules.Count + " lexer rules have a unique string literal and will be inlined.");

        // ── Step 3: replace each qualifying TOKEN_REF with the lexerRuleBlock. ───
        foreach (var item in tokenRefItems)
        {
            if (item is not AdapterElement ae) continue;
            var node = ae.Source;
            var name = node.GetText();

            if (!uniqueRules.TryGetValue(name, out var defBlock)) continue;

            if (config.Verbose)
                System.Console.Error.WriteLine("Inlining " + name);

            var copy = TreeEdits.CopyTreeRecursive(defBlock);
            TreeEdits.InsertBefore(node, copy);
            TreeEdits.Delete(node);
        }

        if (config.Verbose)
        {
            System.Console.Error.WriteLine("Final trees:");
            foreach (var parse_info in data)
            {
                var lexer = parse_info.Lexer;
                var parser = parse_info.Parser;
                foreach (var n in parse_info.Nodes)
                    System.Console.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
            }
        }

        var results = new List<ParsingResultSet>();
        foreach (var parse_info in data)
        {
            results.Add(new ParsingResultSet()
            {
                FileName = parse_info.FileName,
                Nodes = parse_info.Nodes,
                Lexer = parse_info.Lexer,
                Parser = parse_info.Parser
            });
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
        System.Console.WriteLine(js1);
    }

    /// <summary>
    /// Drills down through a chain of element names, returning the innermost match or null.
    /// </summary>
    private static AdapterElement? DescendChild(AdapterElement parent, params string[] names)
    {
        AdapterElement? current = parent;
        foreach (var name in names)
        {
            current = current?.Children.OfType<AdapterElement>()
                .FirstOrDefault(c => c.NodeName?.LocalName == name);
        }
        return current;
    }
}
