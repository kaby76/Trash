namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
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

        public void Execute(Repl repl, ReplParser.StContext tree, bool piped)
        {
            var lines = repl.input_output_stack.Pop();
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var parse_info = JsonSerializer.Deserialize<AntlrJson.ParseInfo>(lines, serializeOptions);
            var lexer = parse_info.Lexer;
            var parser = parse_info.Parser;
            var nodes = parse_info.Nodes;
            StringBuilder sb = new StringBuilder();
            foreach (var t in nodes)
            {
                sb.AppendLine(t.ToStringTree(parser));
            }
            repl.input_output_stack.Push(sb.ToString());
        }
    }
}
