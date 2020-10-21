namespace Trash.Commands
{
    class CQuit
    {
        public void Help()
        {
            System.Console.WriteLine(@"(quit | exit)
Exit the shell program.

Example:
    quit
");
        }

        public void Execute(Repl repl, ReplParser.QuitContext tree)
        {
        }
    }
}
