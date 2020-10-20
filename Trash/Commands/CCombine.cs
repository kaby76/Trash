namespace Trash.Commands
{
    class CCombine
    {
        public void Help()
        {
            System.Console.WriteLine(@"combine
Combine two grammars on top of stack into one grammar.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant.

Example:
    (top of stack contains a lexer file and a parser file, both parsed.)
    combine
");
        }

        public void Execute(Repl repl, ReplParser.CombineContext tree)
        {
            var doc1 = repl.stack.PeekTop(0);
            var doc2 = repl.stack.PeekTop(1);
            var results = LanguageServer.Transform.CombineGrammars(doc1, doc2);
            repl.EnactEdits(results);
        }
    }
}
