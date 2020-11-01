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

        public void Execute(Repl repl, ReplParser.AliasContext tree, bool piped, bool silent = false)
        {
            var ids = tree.NonWsArgMode();
            var sl = tree.StringLiteral();
            if (sl != null)
            {
                repl.Aliases[ids[0].GetText()] = sl.GetText().Substring(1, sl.GetText().Length - 2);
            }
            else if (ids.Length > 1)
            {
                repl.Aliases[ids[0].GetText()] = ids[1].GetText();
            }
            else if (! silent)
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
