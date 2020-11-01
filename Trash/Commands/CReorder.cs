using System;
using System.Collections.Generic;

namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;

    class CReorder
    {
        public void Help()
        {
            System.Console.WriteLine(@"reorder alpha
reorder bfs <string>
reorder dfs <string>
Reorder the parser rules according to the specified type. For BFS and DFS, an XPath
expression must be supplied to specify all the start rule symbols. For alphabetic
reordering, all parser rules are retained, and simply reordered alphabetically. For BFS
and DFS, if the rule is unreachable from a start node, then the rule is dropped from
the grammar.

Example:
    reorder alpha
");
        }

        public void Execute(Repl repl, ReplParser.ReorderContext tree, bool piped)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            var doc = repl.stack.Peek();
            string expr = null;
            if (tree.modes() != null)
            {
                results = LanguageServer.Transform.SortModes(doc);
            }
            else
            {
                LspAntlr.ReorderType order = default;
                if (tree.alpha() != null)
                    order = LspAntlr.ReorderType.Alphabetically;
                else if (tree.bfs() != null)
                {
                    order = LspAntlr.ReorderType.BFS;
                    expr = tree.bfs().StringLiteral().GetText();
                }
                else if (tree.dfs() != null)
                {
                    order = LspAntlr.ReorderType.DFS;
                    expr = tree.dfs().StringLiteral().GetText();
                }
                else
                    throw new Exception("unknown sorting type");
                List<IParseTree> nodes = null;
                if (expr != null)
                {
                    expr = expr.Substring(1, expr.Length - 2);
                    var pr = ParsingResultsFactory.Create(doc);
                    var aparser = pr.Parser;
                    var atree = pr.ParseTree;
                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                    {
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        nodes = engine.parseExpression(expr,
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    }
                }
                results = LanguageServer.Transform.ReorderParserRules(doc, order, nodes);
            }
            repl.EnactEdits(results);
        }
    }
}
