namespace Trash.Commands
{
    class CUnalias
    {
        public void Help()
        {
            System.Console.WriteLine(@"unalias <id>
Remove an aliased command.

Example:
    unalias h
");
        }

        public void Execute(Repl repl, ReplParser.UnaliasContext tree)
        {
            var id = tree.id();
            repl.Aliases.Remove(id.GetText());
        }
    }
}
