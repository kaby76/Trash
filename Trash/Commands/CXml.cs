namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;

    class CXml
    {
        public void Help()
        {
            System.Console.WriteLine(@"xml
Read a tree from stdin and write an XML represenation of it.

Example:
    . | xml
");
        }

        class XmlWalk : IParseTreeListener
        {
            int INDENT = 4;
            int level = 0;
            Parser parser;

            public XmlWalk(Parser p)
            {
                parser = p;
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "<" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
                ++level;
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "</" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
            }

            public void VisitErrorNode(IErrorNode node)
            {
                throw new NotImplementedException();
            }

            public void VisitTerminal(ITerminalNode node)
            {
                string value = node.GetText();
                {
                    System.Console.WriteLine(
                       indent()
                       + "<t>"
                       + node.GetText()
                       + "</t>");
                }
            }

            private String indent()
            {
                var result = new string(' ', level * INDENT);
                return result;
            }
        }

        public void Execute(Repl repl, ReplParser.XmlContext tree, bool piped)
        {
            var pair = repl.tree_stack.Pop();
            var nodes = pair.Item1;
            var parser = pair.Item2;
            var doc = pair.Item3;
            var lines = pair.Item4;
            foreach (var node in nodes)
            {
                ParseTreeWalker.Default.Walk(new XmlWalk(parser), node);
            }
        }
    }
}
