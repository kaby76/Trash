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

    class CKleene
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trkleene.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
            //          System.Console.Error.WriteLine("Expr = '" + expr + "'");
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
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var f = doc.FullPath;
                doc.ParseTree = null;
                doc.Changed = true;
                ParsingResults pr = ParsingResultsFactory.Create(doc);
                pr.ParseTree = null;
                _ = new Module().GetQuickInfo(0, doc);
                var aparser = pr.Parser;
                var atree = pr.ParseTree;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                {
                    List<IParseTree> nodes = null;
                    if (expr != null)
                    {
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    }
                    var res = LanguageServer.Transform.ConvertRecursionToKleeneOperator(doc, nodes);
                    Docs.Class1.EnactEdits(res);
                    pr = ParsingResultsFactory.Create(doc);
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
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
