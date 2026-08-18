using AntlrJson;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;
using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Trash
{
    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trfoldlit.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public string Reconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeAttr a)
                {
                    sb.Append(a.StringValue);
                }
                else if (n is UnvParseTreeText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is UnvParseTreeElement e)
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        stack.Push(n.ChildNodes.item(i));
                    }
                }
            }
            return sb.ToString();
        }

        public string StrictReconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeAttr a)
                {
                }
                else if (n is UnvParseTreeText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is UnvParseTreeElement e)
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        stack.Push(n.ChildNodes.item(i));
                    }
                }
            }
            return sb.ToString();
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.FirstOrDefault();
            if (expr == null) expr = "//lexerRuleSpec[lexerRuleBlock/lexerAltList[count(*) = 1]/lexerAlt[count(*) = 1]/lexerElements[count(*) = 1]/lexerElement[count(*) = 1]/lexerAtom/terminalDef[count(*) = 1]/STRING_LITERAL]/TOKEN_REF";
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
            }
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
                lines = lines.Trim();
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            var evaluator = new XPathEvaluator();
            var exprNode = new XPathParser(expr).Parse();
            List<AdapterElement> nodes = new List<AdapterElement>();
            // First pass: gather all lexer rules.
            foreach (var parse_info in data)
            {
                var atrees = parse_info.Nodes;
                var adapterDoc = AdapterDocument.Build(atrees);
                var n = evaluator.Evaluate(exprNode, adapterDoc).OfType<AdapterElement>().ToList();
                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                nodes.AddRange(n);
            }
            // Second pass to replace.
            foreach (var parse_info in data)
            {
                var fn = parse_info.FileName;
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var adapterDoc = AdapterDocument.Build(atrees);
                {
                    // Go through lexer LHS symbols.
                    foreach (var node in nodes)
                    {
                        var lhs = this.StrictReconstruct(node.Source);
                        // Get RHS string literal of the lexer rule.
                        // Navigate ../lexerRuleBlock: parent is lexerRuleSpec, find child lexerRuleBlock.
                        var refs3 = ((UnvParseTreeElement)node.Source.ParentNode).AllChildren
                            .OfType<UnvParseTreeElement>()
                            .First(c => c.LocalName == "lexerRuleBlock");

                        var rhs = refs3;
                        var str = this.StrictReconstruct(rhs).Trim();
                        if (str == "") continue;
                        str = escape(str);
                        // Find string literals on RHS of parser rules that match lexer
                        // string literal.
                        var expr2 = "//parserRuleSpec/ruleBlock//STRING_LITERAL[text() = \"" + str + "\"]";
                        var refs = evaluator.Evaluate(new XPathParser(expr2).Parse(), adapterDoc)
                            .OfType<AdapterElement>().Select(ae => ae.Source).ToList();
                        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + refs.Count + " nodes.");
                        // Replace all occurrences of string literal with the lexer LHS symbol.
                        foreach (var r in refs)
                        {
                            TreeEdits.Replace(r, this.StrictReconstruct(node.Source));
                        }
                    }
                    var tuple = new ParsingResultSet()
                    {
                        FileName = fn,
                        Nodes = atrees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);
        }

        private string escape(string str)
        {
            return new Regex("\"").Replace(str, "\"\"");
        }
    }
}
