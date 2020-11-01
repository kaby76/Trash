using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trash.Commands
{
    class CAnything
    {
        internal void Execute(Repl repl, ReplParser.AnythingContext tree, bool piped, bool redo_aliases = false)
        {
            if (repl.Aliases.ContainsKey(tree.id().GetText()))
            {
                var cmd = repl.Aliases[tree.id().GetText()];
                var stuff = tree.children.ToList().Skip(1);
                var rest = stuff.Select(s => s.GetText()).ToList();
                var rs = rest != null ? String.Join(" ", rest) : "";
                cmd = cmd + " " + rs;
                if (redo_aliases)
                    repl.ExecuteAlias(cmd);
                else
                    repl.Execute(cmd);
            }
            else
            {
                throw new Exception("Unknown command");
            }
        }
    }
}
