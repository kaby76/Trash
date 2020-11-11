namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;

    class CJson
    {
        public void Help()
        {
            System.Console.WriteLine(@"json
Read a tree from stdin and write a JSON represenation of it.

Example:
    . | json
");
        }

        class JsonWalk : IParseTreeListener
        {
            int INDENT = 4;
            int level = 0;
            Parser parser;

            public JsonWalk(Parser p)
            {
                parser = p;
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "{");
                System.Console.WriteLine(
                    indent()
                    + "\"" + parser.RuleNames[ctx.RuleIndex]
                    + "\":");
                System.Console.WriteLine(
                    indent()
                    + "[");
                ++level;
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "]");
                System.Console.WriteLine(
                    indent()
                    + "}");
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
                        + "{");
                    System.Console.WriteLine(
                        indent()
                        + "\"Text\":"
                        + "\""
                        + node.GetText()
                        + "\""
                    );
                    System.Console.WriteLine(
                        indent()
                        + "}");
                }
            }

            private String indent()
            {
                var result = new string(' ', level * INDENT);
                return result;
            }
        }

        public void Execute(Repl repl, ReplParser.JsonContext tree, bool piped)
        {
            var pair = repl.tree_stack.Pop();
            var nodes = pair.Item1;
            var parser = pair.Item2;
            var doc = pair.Item3;
            var lines = pair.Item4;
            foreach (var node in nodes)
            {
                ParseTreeWalker.Default.Walk(new JsonWalk(parser), node);
            }
        }
    }
}
