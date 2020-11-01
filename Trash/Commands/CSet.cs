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
");
        }

        public void Execute(Repl repl, ReplParser.SetContext tree, bool piped)
        {
            var id = tree.NonWsArgMode();
            var v1 = tree.StringLiteral()?.GetText();
            var v2 = tree.INT()?.GetText();
            if (id?.GetText().ToLower() == "quietafter")
            {
                var v = int.Parse(v2);
                repl.QuietAfter = v;
            }
        }
    }
}
