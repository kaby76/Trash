namespace Trash
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System;
    using System.Text.Json;
    using Workspaces;

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
            MyTuple<IParseTree[], Parser, Document, string> tuple = repl.input_output_stack.Pop();
            var lines = tuple.Item4;
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var lexer = pr.Lexer;
            var parser = pr.Parser;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var nodes = JsonSerializer.Deserialize<IParseTree[]>(lines, serializeOptions);
            if (nodes == null) return;
            
            foreach (var node in nodes)
            {
                ParseTreeWalker.Default.Walk(new JsonWalk(parser), node);
            }
        }
    }
}
