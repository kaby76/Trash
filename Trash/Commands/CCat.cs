using Algorithms;

namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    class CCat
    {
        public void Help()
        {
            System.Console.WriteLine(@"Cat <arg>
Read a file and write to stdout.

Example:
    cat input.txt
");
        }

        public void Execute(Repl repl, ReplParser.CatContext tree, bool piped)
        {
            var args = tree.arg();
            if (args.Length == 0) args = null;
            var list = new Globbing().Contents(args?.Select(t => repl.GetArg(t)).ToList());
            StringBuilder sb = new StringBuilder();
            foreach (var f in list)
            {
                if (f is DirectoryInfo d)
                    throw new Exception("Cannot cat a directory.");
                else
                {
                    string input = System.IO.File.ReadAllText(f.FullName);
                    sb.Append(input);
                }
            }
            repl.tree_stack.Push(new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, sb.ToString()));
        }
    }
}
