namespace Trash.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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

        public void Execute(Repl repl, ReplParser.LsContext tree)
        {
            var expr = repl.GetArg(tree.arg());
            var cwd = Directory.GetCurrentDirectory();
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(cwd));
            foreach (var dir in dirs)
            {
                Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}/");
            }
            if (expr == null) expr = "*";
            string[] files = Directory.GetFiles(cwd, expr);
            foreach (var f in files)
            {
                Console.WriteLine($"{f.Substring(f.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
            }
        }
    }
}
