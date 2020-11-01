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
            for (int i = 0; i < repl.History.Count; ++i)
            {
                var h = repl.History[i];
                System.Console.WriteLine(i + " " + h);
            }
        }
    }
}
