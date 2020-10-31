namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Linq;
    using System;

    class CFind
    {
        public void Help()
        {
            System.Console.WriteLine(@"find <string>
Find all sub-trees in the parsed file at the top of stack using the given XPath expression string.

Example:
    find ""//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']""
");
        }

        public void Execute(Repl repl, ReplParser.FindContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            // var doc = repl.stack.Peek();
            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            //var pr = ParsingResultsFactory.Create(doc);
            //var aparser = pr.Parser;
            //var atree = pr.ParseTree;
            var pair = repl.tree_stack.Pop();
            var atrees = pair.Item1;
            var aparser = pair.Item2;
            IParseTree root = atrees.First().Root();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(root, aparser))
            {
                var l = atrees.Select(t => ate.FindDomNode(t));
                var nodes = engine.parseExpression(expr,
                        new StaticContextBuilder()).evaluate(dynamicContext, l.ToArray() )
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();
                repl.tree_stack.Push(new Tuple<IParseTree[], Parser>(nodes, aparser));
            }
        }
    }
}
