namespace Trash
{
    using System.Text;
    using System.Text.Json;

    class CSt
    {
        public void Help()
        {
            System.Console.WriteLine(@"st
Output tree using the Antlr runtime ToStringTree().

Examples:
    . | st
");
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
            var lexer = parse_info.Lexer;
            var parser = parse_info.Parser;
            var nodes = parse_info.Nodes;
            StringBuilder sb = new StringBuilder();
            foreach (var t in nodes)
            {
                sb.AppendLine(t.ToStringTree(parser));
            }
            System.Console.WriteLine(sb.ToString());
        }
    }
}
