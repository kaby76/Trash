namespace Trash.Commands
{
    class CPop
    {
        public void Help()
        {
            System.Console.WriteLine(@"pop
Pop the top document from the stack. If the stack is empty, nothing is further popped.
There is no check as to whether the document has been written to disk. If you want to write
the file, use write.


Example:
    pop
");
        }

        public void Execute(Repl repl, ReplParser.PopContext tree, bool piped)
        {
            _ = repl.stack.Pop();
        }
    }
}
