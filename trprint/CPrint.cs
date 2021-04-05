namespace Trash.Commands
{
    class CPrint
    {
        public void Help()
        {
            System.Console.WriteLine(@"print
Print out text file at the top of stack.

Example:
    print
");
        }

        public void Execute(Repl repl, ReplParser.PrintContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            System.Console.Error.WriteLine();
            System.Console.Error.WriteLine(doc.FullPath);
            System.Console.WriteLine(doc.Code);
        }
    }
}
