namespace Trash.Commands
{
    class CWrite
    {
        public void Help()
        {
            System.Console.WriteLine(@"write
Pop the stack, and write out the file specified.

Example:
    write
");
        }

        public void Execute(Repl repl, ReplParser.WriteContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            repl._docs.WriteDoc(doc);
        }
    }
}
