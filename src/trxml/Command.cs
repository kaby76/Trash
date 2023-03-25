namespace Trash
{
    using ParseTreeEditing.ParseTreeDOM;
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

        class XmlWalk : MyParseTreeListener
        {
            int INDENT = 4;
            int level = 0;

            public XmlWalk()
            {
            }

            public void EnterEveryRule(UnvParseTreeNode ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "<" + ctx.LocalName
                    + ">");
                ++level;
            }

            public void ExitEveryRule(UnvParseTreeNode ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "</" + ctx.LocalName
                    + ">");
            }

            public void VisitErrorNode(UnvParseTreeNode node)
            {
                throw new NotImplementedException();
            }

            //public void VisitErrorNode(IErrorNode node)
            //{
            //    throw new NotImplementedException();
            //}

            public void VisitTerminal(UnvParseTreeNode node)
            {
                string value = (node as UnvParseTreeText).Data;
                {
                    System.Console.WriteLine(
                       indent()
                       + "<t>"
                       + value
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
            serializeOptions.MaxDepth = 10000;
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var fn = parse_info.FileName;
                foreach (var node in parse_info.Nodes)
                {
                    if (node is UnvParseTreeElement e) MyParseTreeWalker.Default.Walk(new XmlWalk(), e);
                }
            }
        }
    }
}
