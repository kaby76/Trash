using Antlr4.Runtime.Tree;

namespace Trash.Commands
{
    public class Command
    {
        public Command()
        {
        }

        public virtual void Help()
        {
        }

        public virtual void Execute(Repl repl, IParseTree t)
        {
        }
        public virtual void Execute(Repl repl, ReplParser.HelpContext t)
        {
        }
    }
}
