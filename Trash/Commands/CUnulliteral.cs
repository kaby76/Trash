namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CUnulliteral
    {
        public void Help()
        {
            System.Console.WriteLine(@"unulliteral (uc | lc) <string>?
The unulliteral command applies the inverse ""upper - and lower -case string literal"" transform
to a collection of terminal nodes in the parse tree, which is identified with the supplied
xpath expression. If the xpath expression is not given, the transform is applied to the
whole file. Prior to using this command, the document must have been parsed. The unulliteral
operation substitutes a string literal for the sequence of sets containing an upper and lower case characters.
The expression must point to the right-hand side which contains nothing other than a sequence
of one- or two-character sets of a
parser or lexer rule. The resulting code is parsed and placed on the top of stack.

Example:
    unulliteral //lexerRuleSpec[TOKEN_REF/text()='A']
");
        }

        public void Execute(Repl repl, ReplParser.UnulliteralContext tree)
        {
            var type = tree.uclc()?.GetText();
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
                    var results = LanguageServer.Transform.UnUpperLowerCaseLiteral(nodes, type == "uc", doc);
                    repl.EnactEdits(results);
                }
            }
            else
            {
                var results = LanguageServer.Transform.UnUpperLowerCaseLiteral(null, type == "uc", doc);
                repl.EnactEdits(results);
            }
        }
    }
}
