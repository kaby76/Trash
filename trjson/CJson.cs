namespace Trash
{
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

        public void Execute(Config config)
        {
            string lines = null;
            for (; ; )
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var nodes = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var fn = parse_info.FileName;
            foreach (var node in nodes)
            {
                ParseTreeWalker.Default.Walk(new JsonWalk(parser), node);
            }
        }
    }
}
