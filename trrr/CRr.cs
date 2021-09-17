namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CRr
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trrr.readme.md"))
				    using (StreamReader reader = new StreamReader(stream))
		    {
			    return reader.ReadToEnd();
		    }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr;
            //System.Console.Error.WriteLine("Expr = '" + expr + "'");
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
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
            foreach (var doc in docs)
            {
                var pr = LanguageServer.ParsingResultsFactory.Create(doc);
                var workspace = doc.Workspace;
                _ = new LanguageServer.Module().Compile(workspace);
                var text = doc.Code;
                var fn = doc.FullPath;
                var tree = pr.ParseTree;
                var parser = pr.Parser;
                var lexer = pr.Lexer;
                List<IParseTree> nodes = null;
                if (expr != null)
                {
                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                    {
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    }
                }
                var res = LanguageServer.Transform.ToRightRecursion(nodes, doc);
                Docs.Class1.EnactEdits(res);
                IParseTree pt = pr.ParseTree;
                var tuple = new ParsingResultSet()
                {
                    Text = doc.Code,
                    FileName = doc.FullPath,
                    Stream = pr.TokStream,
                    Nodes = new IParseTree[] { pt },
                    Lexer = pr.Lexer,
                    Parser = pr.Parser
                };
                results.Add(tuple);
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
