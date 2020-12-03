namespace Trash.Commands
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Text;
    using System.Text.Json;
    using Workspaces;

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
            MyTuple<IParseTree[], Parser, Document, string> tuple = repl.input_output_stack.Pop();
            var lines = tuple.Item4;
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var lexer = pr.Lexer;
            var parser = pr.Parser;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var nodes = JsonSerializer.Deserialize<IParseTree[]>(lines, serializeOptions);
            if (nodes == null) return;
            StringBuilder sb = new StringBuilder();
            foreach (var t in nodes)
            {
                sb.AppendLine(t.ToStringTree(parser));
            }
            repl.input_output_stack.Push(new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, sb.ToString()));
        }
    }
}
