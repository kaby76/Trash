
namespace Trash.Commands
{
    using System.Text.Json;

    class CStrip
    {
        public void Help()
        {
            System.Console.WriteLine(@"strip
Replaces the grammar at the top of stack with one that has all comments, labels, and
action blocks removed. The resulting grammar is a basic CFG. Once completed, you can write
the grammar out using 'write'.

Example:
    strip
");
        }

        public void Execute(Repl repl, ReplParser.StripContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            var results = LanguageServer.Transform.Strip(doc);
            repl.EnactEdits(results);
        }
    }
}
