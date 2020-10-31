namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.Linq;

    class CRup
    {
        public void Help()
        {
            System.Console.WriteLine(@"rup <string>?
Find all altLists as specified by the xpath expression in the parsed file at 
the top of stack. If the xpath expression is not given, the transform is applied
to the whole file. Rewrite the node with the parentheses removed, if the altList
satifies three constraints: (1) the expression must be a altList type in the Antlr4
grammar; (2) the altList node doesn't contain more than one child, or if it does,
then the containing altList/labeledAlt/alterative each does not contain more than
one child; (3) the ebnf parent of block must not contain a blockSuffix.

Example:
    rup
");
        }

        public void Execute(Repl repl, ReplParser.RupContext tree)
        {
            var expr = tree.StringLiteral()?.GetText();
            expr = expr?.Substring(1, expr.Length - 2);
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            List<IParseTree> nodes = null;
            if (expr != null)
            {
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                }
            }
            var results = LanguageServer.Transform.RemoveUselessParentheses(doc, nodes);
            repl.EnactEdits(results);
        }
    }
}
