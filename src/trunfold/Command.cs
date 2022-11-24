namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trunfold.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
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
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            var docs = new List<Workspaces.Document>();
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream;
                var doc = Docs.Class1.CreateDoc(parse_info);
                docs.Add(doc);
            }
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as EditableAntlrTree.MyTokenStream;
                if (config.Verbose) System.Console.WriteLine(LanguageServer.TreeOutput.OutputTree(atrees.First(), lexer, parser, tokstream).ToString());

                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
                {
                    List<TerminalNodeImpl> nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                    var (text_before, other) = LanguageServer.TreeEdits.TextToLeftOfLeaves(tokstream, atrees[0]);
                    LanguageServer.Transform.Unfold(nodes, atrees.ToList(), parser, lexer, (EditableAntlrTree.MyTokenStream)tokstream);
                    if (config.Verbose)
                    {
                        System.Console.Error.WriteLine("Final tree:");
                        System.Console.WriteLine(LanguageServer.TreeOutput.OutputTree(atrees.First(), lexer, parser, tokstream).ToString());
                    }
                    var tuple = new ParsingResultSet()
                    {
                        Text = ((EditableAntlrTree.MyTokenStream)tokstream).Text,
                        FileName = fn,
                        Stream = tokstream,
                        Nodes = atrees,
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
