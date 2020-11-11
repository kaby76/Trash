namespace Trash.Commands
{
    using System.Linq;

    class CRun
    {
        public void Help()
        {
            System.Console.WriteLine(@"run arg?
Run a parser that was generated and built using ""generate"" with input that is read from
stdin. The output of the command is the resulting parse tree.

Examples:
    cat input.txt | run | json
    echo ""1 + 2"" | run
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
