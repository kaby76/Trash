namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;

    class CDot
    {
        public void Help()
        {
            System.Console.WriteLine(@".
Print out the parse tree for the file at the top of stack.

Example:
    .
");
        }

        public void Execute(Repl repl, ReplParser.DotContext tree)
        {
            if (repl.stack.Any())
            {
                var doc = repl.stack.Peek();
                var pr = ParsingResultsFactory.Create(doc);
                var pt = pr.ParseTree;
                TerminalNodeImpl x = TreeEdits.LeftMostToken(pt);
                var ts = x.Payload.TokenSource;
                System.Console.WriteLine();
                System.Console.WriteLine(
                    TreeOutput.OutputTree(
                        pt,
                        ts as Lexer,
                        null).ToString());
            }
        }
    }
}
