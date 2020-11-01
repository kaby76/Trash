using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LanguageServer;
using org.eclipse.wst.xml.xpath2.processor.util;

namespace Trash.Commands
{
    class CFold
    {
        public void Help()
        {
            System.Console.WriteLine(@"fold <string>
Replace a sequence of symbols on the RHS of a rule
with the rule LHS symbol.

Example:
    fold ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.FoldContext tree, bool piped)
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
                var results = LanguageServer.Transform.Fold(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
