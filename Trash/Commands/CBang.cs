using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trash.Commands
{
    class CBang
    {
        public void Help()
        {
            System.Console.WriteLine(@"(!! | !<int> | !<id>)  -- (bang)
Execute the previous command;
Or, execute the command line int.
Or, execute the command that begins with string.

Example:
    !!
    !5
    !par
");
        }

        public void Execute(Repl repl, ReplParser.BangContext tree)
        {
            if (tree.@int() != null)
            {
                var snum = tree.@int().GetText();
                var num = Int32.Parse(snum);
                var recall = repl.History[num];
                System.Console.Error.WriteLine(recall);
                repl.Execute(recall);
                return;
            }
            else if (tree.BANG().Length > 1)
            {
                var recall = repl.History.Last();
                System.Console.Error.WriteLine(recall);
                repl.Execute(recall);
                return;
            }
            else if (tree.id_keyword() != null)
            {
                var s = tree.id_keyword().GetText();
                for (int i = repl.History.Count - 1; i >= 0; --i)
                {
                    if (repl.History[i].StartsWith(s))
                    {
                        var recall = repl.History[i];
                        System.Console.Error.WriteLine(recall);
                        repl.Execute(recall);
                        return;
                    }
                }
                System.Console.Error.WriteLine("No previous command starts with " + s);
            }
        }
    }
}
