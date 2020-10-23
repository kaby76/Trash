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
                DirectoryInfo di = new DirectoryInfo(expr);
                if (!di.Exists)
                    throw new Exception("directory does not exist.");
                Directory.SetCurrentDirectory(di.FullName);
            }
            else
            {
                expr = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                DirectoryInfo di = new DirectoryInfo(expr);
                if (!di.Exists)
                    throw new Exception("directory does not exist.");
                Directory.SetCurrentDirectory(di.FullName);
            }
        }
    }
}
