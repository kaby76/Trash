namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CUlliteral
    {
        public void Help()
        {
            System.Console.WriteLine(@"ulliteral <string>?
The ulliteral command applies the ""upper - and lower -case string literal"" transform
to a collection of terminal nodes in the parse tree, which is identified with the supplied
xpath expression. If the xpath expression is not given, the transform is applied to the
whole file. Prior to using this command, the document must have been parsed. The ulliteral
operation substitutes a sequence of sets containing an upper and lower case characters for
a STRING_LITERAL. The expression must point to the right-hand side STRING_LITERAL of a
parser or lexer rule. The resulting code is parsed and placed on the top of stack.

Example:
    ulliteral ""//lexerRuleSpec[TOKEN_REF/text() = 'A']//STRING_LITERAL""
");
        }

        public void Execute(Repl repl, ReplParser.UlliteralContext tree)
        {
            var expr = tree.StringLiteral()?.GetText();
            expr = expr?.Substring(1, expr.Length - 2);
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            if (expr != null)
            {
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                    var results = LanguageServer.Transform.UpperLowerCaseLiteral(nodes, doc);
                    repl.EnactEdits(results);
                }
            }
            else
            {
                var results = LanguageServer.Transform.UpperLowerCaseLiteral(null, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
