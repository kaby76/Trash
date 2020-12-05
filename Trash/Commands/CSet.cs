namespace Trash.Commands
{
    class CSet
    {
        public void Help()
        {
            System.Console.WriteLine(@"set <id> = <value>
Set a value.

Example:
    set quietafter=10
    set times=true
");
        }

        public void Execute(Repl repl, ReplParser.SetContext tree, bool piped)
        {
            var ids = tree.NonWsArgMode();
            var v1 = tree.StringLiteral()?.GetText();
            var v2 = tree.INTArgMode()?.GetText();
            if (ids[0]?.GetText().ToLower() == "quietafter")
            {
                var v = int.Parse(v2);
                repl.QuietAfter = v;
            }
            else if (ids[0]?.GetText().ToLower() == "times")
            {
                var v = bool.Parse(ids[1].GetText());
                repl.Times = v;
            }
        }
    }
}
