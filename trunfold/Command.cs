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
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                var doc = Docs.Class1.CreateDoc(parse_info);
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                _ = new LanguageServer.Module().Compile(workspace, false);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
                {
                    List<TerminalNodeImpl> nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                    var (text_before, other) = LanguageServer.TreeEdits.TextToLeftOfLeaves(tokstream, atrees[0]);
                    LanguageServer.Transform.Unfold(nodes, doc);
                    var t = LanguageServer.TreeOutput.OutputTree(atrees.First(), lexer, parser, null).ToString();
                    var xx = new System.Text.StringBuilder();
                    LanguageServer.TreeEdits.Reconstruct(xx, atrees[0], text_before);
                    var zz = xx.ToString();
                    var tuple = new ParsingResultSet()
                    {
                        Text = ((AltAntlr.MyTokenStream)tokstream).Text,
                        FileName = doc.FullPath,
                        Stream = pr.TokStream,
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
