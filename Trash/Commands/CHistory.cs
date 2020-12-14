using System;

namespace Trash.Commands
{
    class CHistory
    {
        public void Help()
        {
            System.Console.WriteLine(@"history
Print out the shell command history.

Example:
    history
");
        }

        public void Execute(Repl repl, ReplParser.HistoryContext tree, bool piped)
        {
            System.Console.WriteLine();
            int start = Math.Max(0, repl.History.Count - repl.HistoryLimit);
            for (int i = start; i < repl.History.Count; ++i)
            {
                var h = repl.History[i];
                System.Console.WriteLine(i + " " + h);
            }
        }
    }
}
