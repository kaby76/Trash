namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Collections.Generic;
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

        private string OutputTokens(EditableAntlrTree.MyTokenStream tokstream, IParseTree tree)
        {
            var frontier = TreeEdits.Frontier(tree);
            var first = frontier.First();
            var last = frontier.Last();
            var first_index = first.Payload.TokenIndex;
            var last_index = last.Payload.TokenIndex;
            StringBuilder sb = new StringBuilder();
            for (var i = first_index; i <= last_index; i++)
            {
                var token = tokstream.Get(i);
                sb.AppendLine(token.ToString());
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
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var fn = parse_info.FileName;
                var tokstream = parse_info.Stream as EditableAntlrTree.MyTokenStream;
                foreach (var node in nodes)
                {
                    System.Console.WriteLine(OutputTokens(tokstream, node));
                }
            }
        }
    }
}
