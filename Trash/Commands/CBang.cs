namespace Trash.Commands
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization.Formatters;

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
            if (tree.BANG().GetText() == "!!")
            {
                var recall = repl.History.Last();
                System.Console.Error.WriteLine(recall);
                repl.Execute(recall);
                return;
            }
            else if (int.TryParse(tree.BANG().GetText().Substring(1), out int num))
            {
                var recall = repl.History[num];
                System.Console.Error.WriteLine(recall);
                repl.Execute(recall);
                return;
            }
            else
            {
                var s = tree.BANG().GetText().Substring(1);
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
