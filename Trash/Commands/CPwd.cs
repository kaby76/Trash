namespace Trash.Commands
{
    using System.IO;

    class CPwd
    {
        public void Help()
        {
            System.Console.WriteLine(@"pwd
Print out the current working directory.

Example:
    pwd
");
        }

        public void Execute(Repl repl, ReplParser.PwdContext tree, bool piped)
        {
            var cwd = Directory.GetCurrentDirectory();
            System.Console.Error.WriteLine(cwd);
        }
    }
}
