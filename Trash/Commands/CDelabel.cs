namespace Trash.Commands
{
    class CDelabel
    {
        public void Help()
        {
            System.Console.WriteLine(@"delabel
Remove all labels from an Antlr4 grammar that is on the top of stack, e.g.,
""expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....."" => ""expr : expr (PLUS | MINUS) expr .....""

Example:
    delabel
");
        }

        public void Execute(Repl repl, ReplParser.DelabelContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            var results = LanguageServer.Transform.Delabel(doc);
            repl.EnactEdits(results);
        }
    }
}
