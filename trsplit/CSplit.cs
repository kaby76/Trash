namespace Trash.Commands
{
    class CSplit
    {
        public void Help()
        {
            System.Console.WriteLine(@"split
The split command attempts to split a grammar at the top of the stack. The grammar
must be a combined lexer/parser grammar for the transformation to proceed. The
transformation creates a lexer grammar and a parser grammar and places them on the
stack. The original grammar is popped off the stack.

Example:
    split
");
        }

        public void Execute(Repl repl, ReplParser.SplitContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            var results = LanguageServer.Transform.SplitGrammar(doc);
            repl.EnactEdits(results);
        }
    }
}
