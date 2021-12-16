namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.IO;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trjson.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
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
            if (!(config.File != null && config.File != ""))
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
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
}
