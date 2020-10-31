using System;
using System.Collections.Generic;
using System.Text;

namespace Trash.Commands
{
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

        public void Execute(Repl repl, ReplParser.StContext tree)
        {
            var pair = repl.tree_stack.Pop();
            var trees = pair.Item1;
            var parser = pair.Item2;
            foreach (var t in trees)
            {
                System.Console.WriteLine(t.ToStringTree(), parser);
            }
        }
    }
}
