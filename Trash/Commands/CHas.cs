using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using LanguageServer;
using org.eclipse.wst.xml.xpath2.processor.util;

namespace Trash.Commands
{
    class CHas
    {
        public void Help()
        {
            System.Console.WriteLine(@"has (dr | ir) (left | right) <string>
Print out whether the rule specified by the xpath expression pointing to the LHS symbol
of a parser or lexer rule has left or right recursion.

Example:
");
        }

        public void Execute(Repl repl, ReplParser.HasContext tree)
        {
            var graph = tree.GRAPH() != null;
            var expr = repl.GetArg(tree.arg());
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
            {
                List<IParseTree> nodes = null;
                if (expr != null)
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                }
                if (tree.DR() != null)
                {
                    var result = LanguageServer.Transform.HasDirectRec(doc, nodes);
                    foreach (var r in result)
                    {
                        System.Console.WriteLine(r);
                    }
                }
                else if (tree.IR() != null)
                {
                    var result = LanguageServer.Transform.HasIndirectRec(nodes, graph, doc);
                    foreach (var r in result)
                    {
                        System.Console.WriteLine(r);
                    }
                }
                else throw new Exception("unknown check");
            }
        }
    }
}
