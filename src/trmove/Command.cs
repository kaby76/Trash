namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trmove.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var from = config.Expr.First();
            var to = config.Expr.Skip(1).First();
	        if (config.Verbose)
	        {
		        System.Console.Error.WriteLine("from = >>>" + from + "<<<");
		        System.Console.Error.WriteLine("to = >>>" + to + "<<<");
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
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            var docs = new List<Workspaces.Document>();
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream;
                var doc = Docs.Class1.CreateDoc(parse_info);
                docs.Add(doc);
            }
            foreach (var doc in docs)
            {
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                _ = new LanguageServer.Module().Compile(workspace);
                var text = doc.Code;
                var fn = doc.FullPath;
                var tree = doc.ParseTree;
                var parser = pr.Parser;
                var lexer = pr.Lexer;
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                IParseTree root = tree;
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, parser))
                {
                    var nodes = engine.parseExpression(from,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
				            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("Found " + nodes.Count + " nodes.");
                    var to_node = engine.parseExpression(to,
			            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
				            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList().FirstOrDefault();

                    Dictionary<string, string> res;
                    if (config.After)
                        res = LanguageServer.Transform.MoveAfter(nodes, to_node, doc);
                    else
                        res = LanguageServer.Transform.MoveBefore(nodes, to_node, doc);
                    Docs.Class1.EnactEdits(res);
                    var pr2 = ParsingResultsFactory.Create(doc);
                    IParseTree pt2 = pr2.ParseTree;
                    var tuple = new ParsingResultSet()
                    {
                        Text = doc.Code,
                        FileName = doc.FullPath,
                        Stream = pr2.TokStream,
                        Nodes = new IParseTree[] { pt2 },
                        Lexer = pr2.Lexer,
                        Parser = pr2.Parser
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
