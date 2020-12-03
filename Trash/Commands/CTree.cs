using Algorithms;
using Workspaces;

namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Collections.Generic;
    using System.Linq;
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
            MyTuple<IParseTree[], Parser, Document, string> stack_param = repl.input_output_stack.Pop();
            var lines = stack_param.Item4;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var in_tuple = JsonSerializer.Deserialize<MyTuple<string, ITokenStream, IParseTree[], Lexer, Parser>>(lines, serializeOptions);
            var nodes = in_tuple.Item3;
            var lexer = in_tuple.Item4;
            var parser = in_tuple.Item5;
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
            MyTuple<IParseTree[], Parser, Document, string> result = new MyTuple<IParseTree[], Parser, Document, string>()
            {
                Item1 = null, Item2 = null, Item3 = null, Item4 = sb.ToString()
            };
            repl.input_output_stack.Push(result);
        }
    }
}
