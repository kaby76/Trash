namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System;
    using System.Text.Json;

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
            string lines = repl.input_output_stack.Pop();
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
                ParseTreeWalker.Default.Walk(new XmlWalk(parser), node);
            }
        }
    }
}
