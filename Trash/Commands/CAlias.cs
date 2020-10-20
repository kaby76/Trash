using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;

namespace Trash.Commands
{
    public class CAlias
    {
        public void Help()
        {
            System.Console.WriteLine(@"alias <id> = <string>
Set up an alias that assigns <string> to <id>. The command <string> is executed with <id>.

Example:
    alias h=history
    h
");
        }

        public void Execute(Repl repl, ReplParser.AliasContext tree)
        {
            var id = tree.ID();
            var sl = tree.StringLiteral();
            if (sl != null)
            {
                repl.Aliases[id.GetText()] = sl.GetText().Substring(1, sl.GetText().Length - 2);
            }
            else if (tree.id_keyword() != null)
            {
                var id_keyword = tree.id_keyword();
                repl.Aliases[id.GetText()] = id_keyword.GetText();
            }
            else if (tree.ID() == null)
            {
                System.Console.WriteLine();
                foreach (var p in repl.Aliases)
                {
                    System.Console.WriteLine(p.Key + " = " + p.Value);
                }
            }
        }
    }
}
