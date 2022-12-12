namespace Trash
{
    using Antlr4.Runtime;
    using AntlrTreeEditing.AntlrDOM;
    using System;
    using System.IO;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trxml.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        class XmlWalk : IMyParseTreeListener
        {
            int INDENT = 4;
            int level = 0;
            Parser parser;

            public XmlWalk(Parser p)
            {
                parser = p;
            }

            public void EnterEveryRule(AntlrElement ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "<" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
                ++level;
            }

            public void ExitEveryRule(AntlrElement ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "</" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
            }

            public void VisitErrorNode(AntlrElement node)
            {
                throw new NotImplementedException();
            }

            //public void VisitErrorNode(IErrorNode node)
            //{
            //    throw new NotImplementedException();
            //}

            public void VisitTerminal(AntlrElement node)
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
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var fn = parse_info.FileName;
                foreach (var node in parse_info.Nodes)
                {
                    if (node is AntlrElement e) MyParseTreeWalker.Default.Walk(new XmlWalk(parser), e);
                }
            }
        }
    }
}
