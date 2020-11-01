using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageServer;
using org.eclipse.wst.xml.xpath2.processor.util;

namespace Trash.Commands
{
    class CGroup
    {
        public void Help()
        {
            System.Console.WriteLine(@"group <string>
Perform a recursive left- and right- factorization of alternatives for rules.
The nodes specified must be for ruleAltList, lexerAltList, or altList. A common
prefix and suffix is performed on the alternatives, and a new expression derived.
The process repeats for alternatives nested.
Example:
    group //parserRuleSpec[RULE_REF/text()='additiveExpression']//altList
");
        }

        public void Execute(Repl repl, ReplParser.GroupContext tree, bool piped)
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
                var results = LanguageServer.Transform.Group(nodes, doc);
                repl.EnactEdits(results);
            }
        }
    }
}
