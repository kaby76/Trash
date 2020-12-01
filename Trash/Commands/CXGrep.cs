using Algorithms;

namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;
    using System;

    class CXGrep
    {
        public void Help()
        {
            System.Console.WriteLine(@"xgrep <string>
Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:
    xgrep ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.XgrepContext tree, bool piped)
        {
            var expr = repl.GetArg(tree.arg());
            IParseTree[] atrees;
            Parser aparser;
            Workspaces.Document doc;
            if (piped)
            {
                var pair = repl.tree_stack.Pop();
                atrees = pair.Item1;
                aparser = pair.Item2;
                doc = pair.Item3;
            }
            else
            {
                doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                aparser = pr.Parser;
                IParseTree atree = pr.ParseTree;
                atrees = new IParseTree[] { atree };
            }
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, aparser))
            {
                var l = atrees.Select(t => ate.FindDomNode(t));
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, l.ToArray() )
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();
                repl.tree_stack.Push(new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(nodes, aparser, doc, null));
            }
        }
    }
}
