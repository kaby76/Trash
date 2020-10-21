using System;
using System.Collections.Generic;
using System.Text;

namespace Trash.Commands
{
    class CParse
    {
        public void Help()
        {
            System.Console.WriteLine(@"parse (antlr2 | antlr3 | antlr4 | bison | ebnf)?
Parse the flie at the top of stack with the given parser type (antlr2, _antlr3, antlr4, bison, etc).

Example:
    parse
    parse antlr2
");
        }

        public void Execute(Repl repl, ReplParser.ParseContext tree)
        {
            var doc = repl.stack.Peek();
            repl._docs.ParseDoc(doc, repl.QuietAfter, tree.type()?.GetText());
        }
    }
}
