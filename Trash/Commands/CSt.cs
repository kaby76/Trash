using Algorithms;

namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Text;

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
            var pair = repl.tree_stack.Pop();
            var trees = pair.Item1;
            var parser = pair.Item2;
            StringBuilder sb = new StringBuilder();
            foreach (var t in trees)
            {
                sb.AppendLine(t.ToStringTree(parser));
            }
            repl.tree_stack.Push(new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, sb.ToString()));
        }
    }
}
