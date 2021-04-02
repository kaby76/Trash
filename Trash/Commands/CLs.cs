namespace Trash.Commands
{
    using System;
    using System.IO;
    using System.Linq;

    class CLs
    {
        public void Help()
        {
            System.Console.WriteLine(@"ls <string>?
List directory contents. If string is not given, list the current directory contents.
ls accepts wildcards.

Example:
    ls
    ls foobar*
");
        }

        public void Execute(Repl repl, ReplParser.LsContext tree, bool piped)
        {
            var args = tree.arg();
            if (args.Length == 0) args = null;
            var list = new Globbing().Contents(args?.Select(t => repl.GetArg(t)).ToList());
            foreach (var f in list)
            {
                if (f is DirectoryInfo d)
                    Console.WriteLine($"{d.Name}/");
                else
                    Console.WriteLine($"{f.Name}");
            }
        }
    }
}
