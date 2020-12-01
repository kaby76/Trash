using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Algorithms;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

namespace Trash.Commands
{
    class CEcho
    {
        public void Help()
        {
            System.Console.WriteLine(@"Echo <string>*
Echo string literals and write to stdout.

Example:
    echo ""1 + 2""
");
        }

        public void Execute(Repl repl, ReplParser.EchoContext tree, bool piped)
        {
            var args = tree.arg();
            var list = args?.Select(t => repl.GetArg(t)).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var input in list)
            {
                sb.Append(input);
            }
            repl.tree_stack.Push(new MyTuple<IParseTree[], Parser, Workspaces.Document, string>(null, null, null, sb.ToString()));
        }
    }
}
