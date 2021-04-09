namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    class CKleene
    {
        public void Help()
        {
            System.Console.WriteLine(@"kleene <string>?
Replace a rule, whose symbol is identified by the xpath string, of the grammar
at the top of the grammar with an EBNF form if it contains direct left or direct
right recursion.

Examples:
    kleene
    kleene //parserRuleSpec/RULE_REF[text()='packageOrTypeName']
");
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
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
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
                var results = LanguageServer.Transform.ConvertRecursionToKleeneOperator(doc, nodes);
                Docs.Class1.EnactEdits(results);
            }
        }
    }
}
