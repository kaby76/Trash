namespace Trash
{
    using AntlrTreeEditing.AntlrDOM;
    using LanguageServer;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtokens.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string OutputTokens(AntlrElement tree)
        {
            var frontier = TreeEdits.Frontier(tree).ToList();
            if (frontier.Count == 0) return "";
            var first = frontier.First();
            var last = frontier.Last();
            StringBuilder sb = new StringBuilder();
            foreach (AntlrNode i in frontier)
            {
                var a = i as AntlrAttr;

                var e = i as AntlrElement;
                if (e == null) continue;

                sb.AppendLine("[@" + e.GetText());
            }
            return sb.ToString();
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
                    var s = TokenOutput.OutputTokens(node, lexer, parser).ToString();
                    System.Console.WriteLine(s);
                }
            }
        }
    }
}
