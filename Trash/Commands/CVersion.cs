using System;
using System.Collections.Generic;
using System.Text;

namespace Trash.Commands
{
    class CVersion
    {
        public void Help()
        {
        }

        public void Execute(Repl repl, ReplParser.VersionContext tree, bool piped)
        {
            System.Console.WriteLine("Version " + Repl.Version);
        }
    }
}
