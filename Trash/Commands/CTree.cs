namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Text;
    using System.Text.Json;

    class CTree
    {
        public void Help()
        {
            System.Console.WriteLine(@"tree
Reads a tree from stdin and prints the tree as an indented node list.

Example:
    . | tree
");
        }

        public void Execute(Repl repl, ReplParser.CtreeContext tree, bool piped)
        {
            var lines = repl.input_output_stack.Pop();
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var in_tuple = JsonSerializer.Deserialize<AntlrJson.ParseInfo>(lines, serializeOptions);
            var nodes = in_tuple.Nodes;
            var lexer = in_tuple.Lexer;
            var parser = in_tuple.Parser;
            StringBuilder sb = new StringBuilder();
            foreach (var node in nodes)
            {
                TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                var ts = x.Payload.TokenSource;
                sb.AppendLine();
                sb.AppendLine(
                    TreeOutput.OutputTree(
                        node,
                        lexer,
                        parser,
                        null).ToString());
            }
            repl.input_output_stack.Push(sb.ToString());
        }
    }
}
