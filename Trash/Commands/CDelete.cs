namespace Trash.Commands
{
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CDelete
    {
        public void Help()
        {
            System.Console.WriteLine(@"delete <string>
Delete nodes in the parsed file at the top of stack specified by the XPath expression string.

Example:
    delete ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.DeleteContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                var results = LanguageServer.Transform.Delete(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
