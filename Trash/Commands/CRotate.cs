namespace Trash.Commands
{
    using System.Linq;
    using Utils;

    class CRotate
    {
        public void Help()
        {
            System.Console.WriteLine(@"rotate
Rotate the stack once.

Example:
    rotate
");
        }

        public void Execute(Repl repl, ReplParser.RotateContext tree, bool piped)
        {
            var top = repl.stack.Pop();
            var docs = repl.stack.ToList();
            docs.Reverse();
            repl.stack = new StackQueue<Workspaces.Document>();
            repl.stack.Push(top);
            foreach (var doc in docs) repl.stack.Push(doc);
        }
    }
}
