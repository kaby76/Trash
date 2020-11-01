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

        public void Execute(Repl repl, ReplParser.CdContext tree, bool piped)
        {
            var expr = repl.GetArg(tree.arg());
            var list = new Globbing().GetDirectory(expr);
            if (list.Count == 0)
                throw new Exception("directory does not exist.");
            else if (list.Count >= 2)
                throw new Exception("ambigous");
            Directory.SetCurrentDirectory(list.First().FullName);
        }
    }
}
