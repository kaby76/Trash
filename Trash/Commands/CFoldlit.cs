namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CFoldlit
    {
        public void Help()
        {
            System.Console.WriteLine(@"foldlit <string>
Replace a literal on the RHS of a rule with the lexer rule LHS symbol.

Example:
    foldlit ""//lexerRuleSpec/TOKEN_REF""
");
        }

        public void Execute(Repl repl, ReplParser.FoldlitContext tree, bool piped)
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
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                var results = LanguageServer.Transform.Foldlit(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
