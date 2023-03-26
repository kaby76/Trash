namespace Trash
{
    using AntlrJson;
    using ParseTreeEditing.UnvParseTreeDOM;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trcombine.readme.md"))
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
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
                // Collect all parser rules.
                List<ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement> parser_rules = null;
                List<ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement> lexer_rules = null;
                List<ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement> id = null;
                UnvParseTreeElement z;
                using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
                {
                    id = engine.parseExpression(
                        "/grammarSpec/grammarDecl/identifier",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    parser_rules = engine.parseExpression(
                        "//parserRuleSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + parser_rules.Count + " parser rules.");
                    lexer_rules = engine.parseExpression(
                        "//lexerRuleSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + lexer_rules.Count + " parser rules.");
                    z = engine.parseExpression(
                        "/grammarSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList().First();
                }
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
                    Text = sb_parser.ToString(),
                    FileName = parser_g4_file_path,
                    Nodes = new UnvParseTreeNode[] { r1 },
                    Lexer = lexer,
                    Parser = parser
                };
                results.Add(tuple1);
                var tuple2 = new ParsingResultSet()
                {
                    Text = sb_lexer.ToString(),
                    FileName = lexer_g4_file_path,
                    Nodes = new UnvParseTreeNode[] { r2 },
                    Lexer = lexer,
                    Parser = parser
                };
                results.Add(tuple2);
            }
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting serialization");
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("serialized");
            System.Console.WriteLine(js1);

            //var results = LanguageServer.Transform.CombineGrammars(doc1, doc2);
        }

        public string Reconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            int last = -1;
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
}
