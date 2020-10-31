namespace Trash.Commands
{
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CUngroup
    {
        public void Help()
        {
            System.Console.WriteLine(@"ungroup <string>
Perform an ungroup transformation of the 'element' node(s) specified by the string.

Example:
    ungroup ""//parserRuleSpec[RULE_REF/text() = 'a']//ruleAltList""
");
        }

        public void Execute(Repl repl, ReplParser.UngroupContext tree)
        {
            var expr = repl.GetArg(tree.arg());
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
                var results = LanguageServer.Transform.Ungroup(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
