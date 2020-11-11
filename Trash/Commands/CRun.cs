namespace Trash.Commands
{
    using System.Linq;

    class CRun
    {
        public void Help()
        {
            System.Console.WriteLine(@"run arg1 (arg2 arg3? )?
Generate a parser using the Antlr tool on the grammar specified by the current
workspace run the generated parser, output a tree or find elements in the tree.
");
        }

        public void Execute(Repl repl, ReplParser.RunContext tree, bool piped)
        {
            var g = new Grun(repl);
            var a = tree.arg();
            var input = repl.GetArg(a);
            g.Run(repl, input);
        }
    }
}
