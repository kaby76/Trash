namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CRename
    {
        public void Help()
        {
            System.Console.WriteLine(@"rename <string> <string>
Rename a symbol, the first parameter as specified by the xpath expression string,
to a new name, the second parameter as a string. The result may place all changed
grammars that use the symbol on the stack.

Example:
    rename ""//parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']"" ""xxx""
");
        }

        public void Execute(Repl repl, ReplParser.RenameContext tree)
        {
            var to_sym = tree.StringLiteral()[1].GetText();
            to_sym = to_sym.Substring(1, to_sym.Length - 2);
            var doc = repl.stack.Peek();
            var expr = tree.StringLiteral()[0].GetText();
            expr = expr.Substring(1, expr.Length - 2);
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
            {
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                var results = LanguageServer.Transform.Rename(nodes, to_sym, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
