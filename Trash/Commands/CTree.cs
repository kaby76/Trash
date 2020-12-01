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
            MyTuple<IParseTree[], Parser, Document, string> tuple = repl.tree_stack.Pop();
            var lines = tuple.Item4;
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var lexer = pr.Lexer;
            var parser = pr.Parser;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.Impl2.ParseTreeConverter(null, null, lexer, parser));
            serializeOptions.WriteIndented = false;
            var obj1 = JsonSerializer.Deserialize<IParseTree>(lines, serializeOptions);
            if (obj1 == null) return;
            var nodes = new IParseTree[] { obj1 };
            StringBuilder sb = new StringBuilder();
            foreach (var node in nodes)
            {
                TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                var ts = x.Payload.TokenSource;
                sb.AppendLine();
                sb.AppendLine(
                    TreeOutput.OutputTree(
                        node,
                        ts as Lexer,
                        null).ToString());
            }
            tuple.Item4 = sb.ToString();
            repl.tree_stack.Push(tuple);
        }
    }
}
