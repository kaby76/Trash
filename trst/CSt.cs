namespace Trash
{
    using System.IO;
    using System.Text;
    using System.Text.Json;

    class CSt
    {
        public string Help()
        {
			using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trst.readme.md"))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
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
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
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
}
