namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.IO;
    using System.Text;
    using System.Text.Json;

    class CTree
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtree.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
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
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var in_tuple in data)
            {
                var nodes = in_tuple.Nodes;
                var lexer = in_tuple.Lexer;
                var parser = in_tuple.Parser;
                StringBuilder sb = new StringBuilder();
                foreach (var node in nodes)
                {
                    sb.AppendLine();
                    sb.AppendLine(
                        TreeOutput.OutputTree(
                            node,
                            lexer,
                            parser,
                            null).ToString());
                }
                System.Console.WriteLine(sb.ToString());
            }
        }
    }
}
