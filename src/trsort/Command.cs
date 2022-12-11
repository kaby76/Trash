namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsort.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var exprs = config.Exprs;
            if (config.Verbose)
            {
                //System.Console.Error.WriteLine("from = >>>" + from + "<<<");
                //System.Console.Error.WriteLine("to = >>>" + to + "<<<");
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
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var trees = parse_info.Nodes;
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as EditableAntlrTree.MyTokenStream;
                var before_tokens = tokstream.GetTokens();
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                var (text_before, other) = TreeEdits.TextToLeftOfLeaves(tokstream, trees);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    // Types of sorts.
                    if (config.Alphabetic)
                    {
                        var first = engine.parseExpression(
                            "(//ruleSpec)[1]",
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree
                                as EditableAntlrTree.MyParserTreeNode).First();
                        var nodes = engine.parseExpression(
                            "//ruleSpec[parserRuleSpec]",
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree
                                as EditableAntlrTree.MyParserTreeNode).ToList();
                        if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                        // HACK FIX BELOW GetChild(---> 0)
                        var sorted = nodes.OrderBy(x => x.GetChild(0).GetText()).ToList();
                        sorted.Reverse();
                        var tb = new Dictionary<TerminalNodeImpl, string>();
                        foreach (var node in sorted)
                        {
                            // Output the tree.
                            //System.Console.Error.WriteLine(TreeOutput.OutputTree(
                            //    trees[0],
                            //    lexer,
                            //    parser,
                            //    null).ToString());
                            //tokstream.Seek(0);
                            //for (var i = 0; i < tokstream.Size; i++)
                            //{
                            //    var token = tokstream.Get(i);
                            //    System.Console.Error.WriteLine(token.ToString());
                            //}
                            if (node == first) continue;
                            TreeEdits.MoveBefore(new List<IParseTree>() { node }, first);
                            first = node;
                            // Output the tree.
                            //System.Console.Error.WriteLine(TreeOutput.OutputTree(
                            //    trees[0],
                            //    lexer,
                            //    parser,
                            //    null).ToString());
                            //tokstream.Seek(0);
                            //for (var i = 0; i < tokstream.Size; i++)
                            //{
                            //    var token = tokstream.Get(i);
                            //    System.Console.Error.WriteLine(token.ToString());
                            //}
                        }
                    }
                    var tuple = new ParsingResultSet()
                    {
                        Text = tokstream.Text,
                        FileName = fn,
                        Stream = tokstream,
                        Nodes = trees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
