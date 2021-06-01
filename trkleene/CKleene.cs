namespace Trash
{
    using Antlr4.Runtime.Tree;
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
            var expr = config.Expr;
            string lines = null;
            for (; ; )
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var pr = ParsingResultsFactory.Create(doc);
                var aparser = pr.Parser;
                var atree = pr.ParseTree;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                {
                    List<IParseTree> nodes = null;
                    if (expr != null)
                    {
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        nodes = engine.parseExpression(expr.First(),
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    }
                    var res = LanguageServer.Transform.ConvertRecursionToKleeneOperator(doc, nodes);
                    Docs.Class1.EnactEdits(res);
                }
            }
        }
    }
}
