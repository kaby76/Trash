namespace Trash.Commands
{
    using System;
    using System.IO;
    using System.Linq;

    class CCd
    {
        public void Help()
        {
            System.Console.WriteLine(@"cd <string>
Change current directory. If string is not given, change to the user's home directory.
'cd' accepts wildcards.

Example:
    cd
    cd *foo
");
        }

        public void Execute(Repl repl, ReplParser.CdContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            if (expr != null)
            {
                var dirs = Directory.EnumerateDirectories(Directory.GetCurrentDirectory(), expr);
                if (dirs.Count() == 1)
                    Directory.SetCurrentDirectory(dirs.First());
                else
                    throw new Exception("No matching directory for '" + expr + "'.");
            }
            else
            {
                expr = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                Directory.SetCurrentDirectory(expr);
            }
        }
    }
}
