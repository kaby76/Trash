namespace Trash
{
    using ParseTreeEditing.UnvParseTreeDOM;
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

        class JsonWalk : MyParseTreeListener
        {
            int INDENT = 4;
            int level = 0;

            public JsonWalk()
            {
            }

            public void EnterEveryRule(UnvParseTreeNode ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "{");
                System.Console.WriteLine(
                    indent()
                    + "\"" + ctx.LocalName
                    + "\":");
                System.Console.WriteLine(
                    indent()
                    + "[");
                ++level;
            }

            public void ExitEveryRule(UnvParseTreeNode ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "]");
                System.Console.WriteLine(
                    indent()
                    + "}");
            }

            public void VisitErrorNode(UnvParseTreeNode node)
            {
                throw new NotImplementedException();
            }

            public void VisitTerminal(UnvParseTreeNode node)
            {
                var text_node = (UnvParseTreeText)node;
                string value = text_node.Data;
                {
                    System.Console.WriteLine(
                        indent()
                        + "{");
                    System.Console.WriteLine(
                        indent()
                        + "\"Text\":"
                        + "\""
                        + text_node.Data
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
                lines = lines.Trim();
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
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var fn = parse_info.FileName;
                foreach (var node in nodes)
                {
                    MyParseTreeWalker.Default.Walk(new JsonWalk(), node);
                }
            }
        }
    }
}
