namespace Trash
{
    using System.Text.Json;

    class CPrint
    {
        public void Help()
        {
            System.Console.WriteLine(@"print
Print out text file at the top of stack.

Example:
    print
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
            var nodes = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var fn = parse_info.FileName;
            var code = parse_info.Text;
            System.Console.Error.WriteLine();
            System.Console.Error.WriteLine(fn);
            System.Console.WriteLine(code);
        }
    }
}
