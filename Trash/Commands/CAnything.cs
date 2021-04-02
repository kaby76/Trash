namespace Trash.Commands
{
    using System;
    using System.Linq;

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
                repl.Execute(cmd, false);
            }
            else
            {
                throw new Exception("Unknown command '" + tree.id().GetText() + "'");
            }
        }
    }
}
