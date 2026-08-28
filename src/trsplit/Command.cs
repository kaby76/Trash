using AntlrJson;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;
using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trash;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsplit.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        var input = ParsingResultIO.Read(config.File);
        var data = input.Results;
        var results = new List<ParsingResultSet>();
        foreach (var parse_info in data)
        {
            var fn = parse_info.FileName;
            var atrees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var evaluator = new XPathEvaluator();
            var adapterDoc = AdapterDocument.Build(atrees);
            // Collect all parser rules.
            var id = evaluator.Evaluate(new XPathParser("/grammarSpec/grammarDecl/identifier").Parse(), adapterDoc)
                .OfType<AdapterElement>().Select(ae => ae.Source).ToList();
            var parser_rules = evaluator.Evaluate(new XPathParser("//parserRuleSpec").Parse(), adapterDoc)
                .OfType<AdapterElement>().Select(ae => ae.Source).ToList();
            if (config.Verbose)
                LoggerNs.TimedStderrOutput.WriteLine("Found " + parser_rules.Count + " parser rules.");
            var lexer_rules = evaluator.Evaluate(new XPathParser("//lexerRuleSpec").Parse(), adapterDoc)
                .OfType<AdapterElement>().Select(ae => ae.Source).ToList();
            if (config.Verbose)
                LoggerNs.TimedStderrOutput.WriteLine("Found " + lexer_rules.Count + " lexer rules.");
            var z = evaluator.Evaluate(new XPathParser("/grammarSpec").Parse(), adapterDoc)
                .OfType<AdapterElement>().Select(ae => ae.Source).First();

            // Create text files.
            StringBuilder sb_parser = new StringBuilder();
            StringBuilder sb_lexer = new StringBuilder();
            sb_parser.Append("parser grammar " + id.First().GetText() + "Parser;" + Environment.NewLine);
            sb_parser.AppendLine("options { tokenVocab=" + id.First().GetText() + "Lexer; }");
            sb_lexer.Append("lexer grammar " + id.First().GetText() + "Lexer;" + Environment.NewLine);
            foreach (var r in parser_rules)
            {
                sb_parser.Append(Reconstruct(r));
            }

            foreach (var r in lexer_rules)
            {
                sb_lexer.Append(Reconstruct(r));
            }

            string parser_g4_file_path = (id.First().GetText() + "Parser.g4").Trim();
            string lexer_g4_file_path = (id.First().GetText() + "Lexer.g4").Trim();
            var r1 = new UnvParseTreeElement(z);
            var r2 = new UnvParseTreeElement(z);
            TreeEdits.AddChildren(r1, new UnvParseTreeText() { Data = sb_parser.ToString() });
            TreeEdits.AddChildren(r2, new UnvParseTreeText() { Data = sb_lexer.ToString() });
            var tuple1 = new ParsingResultSet()
            {
                FileName = parser_g4_file_path,
                Nodes = new UnvParseTreeNode[] { r1 },
                Lexer = lexer,
                Parser = parser
            };
            results.Add(tuple1);
            var tuple2 = new ParsingResultSet()
            {
                FileName = lexer_g4_file_path,
                Nodes = new UnvParseTreeNode[] { r2 },
                Lexer = lexer,
                Parser = parser
            };
            results.Add(tuple2);
        }

        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
        using var output = System.Console.OpenStandardOutput();
        ParsingResultIO.WriteBundle(output, input, results, config.Format);
        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
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

}
