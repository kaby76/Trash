namespace Trash.Commands
{
    using System.Linq;

    class CStack
    {
        public void Help()
        {
            System.Console.WriteLine(@"stack
Print the stack of files.

Example:
    stack
");
        }

        public void Execute(Repl repl, ReplParser.StackContext tree, bool piped)
        {
            var docs = repl.stack.ToList();
            foreach (var doc in docs)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(doc.FullPath);
            }
        }
    }
}
