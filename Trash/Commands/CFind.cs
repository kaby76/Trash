namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CFind
    {
        public void Help()
        {
            System.Console.WriteLine(@"find <string>
Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:
    find ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.FindContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            var doc = repl.stack.Peek();
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();
                foreach (var node in nodes)
                {
                    TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                    var ts = x.Payload.TokenSource;
                    System.Console.WriteLine();
                    System.Console.WriteLine(
                        TreeOutput.OutputTree(
                            node,
                            ts as Lexer,
                            null).ToString());
                }
            }
        }
    }
}
