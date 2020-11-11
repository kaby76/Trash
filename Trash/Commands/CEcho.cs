using System;
using System.Collections.Generic;
using System.Text;

namespace Trash.Commands
{
    class CEcho
    {
        public void Help()
        {
            System.Console.WriteLine(@"Echo <string>
Echo a string literal and write to stdout.

Example:
    cat input.txt
");
        }

        public void Execute(Repl repl, ReplParser.EchoContext tree, bool piped)
        {

        }
    }
}
