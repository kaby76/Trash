namespace Trash.Commands
{
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CMvsr
    {
        public void Help()
        {
            System.Console.WriteLine(@"mvsr <string>
Move the rule, whose symbol is identified by the xpath string, to the top of the grammar.
");
        }

        public void Execute(Repl repl, ReplParser.MvsrContext tree)
        {
            var expr = tree.StringLiteral().GetText();
            expr = expr.Substring(1, expr.Length - 2);
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
                var results = LanguageServer.Transform.MoveStartRuleToTop(doc, nodes);
                repl.EnactEdits(results);
            }
        }
    }
}
