namespace Trash.Commands
{
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CRr
    {
        public void Help()
        {
            System.Console.WriteLine(@"rr <string>
Replace left indirect or direct recursion with right recursion.

Example:
    rr ...
");
        }

        public void Execute(Repl repl, ReplParser.RrContext tree, bool piped)
        {
            var expr = tree.StringLiteral().GetText();
            expr = expr.Substring(1, expr.Length - 2);
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                var results = LanguageServer.Transform.ToRightRecursion(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
