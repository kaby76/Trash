
namespace Trash.Commands
{
    using System.Text.Json;

    class CStrip
    {
        public void Help()
        {
        }

        public void Execute(Repl repl, ReplParser.StripContext tree, bool piped)
        {
            var doc = repl.stack.Peek();
            var results = LanguageServer.Transform.Strip(doc);
            repl.EnactEdits(results);
        }
    }
}
