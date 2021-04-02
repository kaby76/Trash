namespace Trash.Commands
{
    using System.Linq;

    class CGenerate
    {
        public void Help()
        {
            System.Console.WriteLine(@"generate <arg>* 
Generate a parser using the Antlr tool on the grammar specified by the current
workspace. The generated parser is placed in the directory <current-directory>/Generated/.
The given arg is the name of the start rule. After generating the parser and driver, it
is compiled for use with the ""run"" command.

Examples:
    generate expression
");
        }

        public void Execute(Repl repl, ReplParser.GenerateContext tree, bool piped)
        {
            var g = new Generate(repl);
            var p = tree.arg();
            var parameters = p.Select(a => repl.GetArg(a)).ToArray();
            g.Run(repl, parameters);
        }
    }
}
