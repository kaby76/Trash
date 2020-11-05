namespace Trash.Commands
{
    using Antlr4.Runtime.Tree;
    using System.Linq;

    class CGenerate
    {
        public void Help()
        {
            System.Console.WriteLine(@"generate <arg>* 
Generate a parser using the Antlr tool on the grammar specified by the current
workspace. The generated parser is in the current directory, and each arg
is a command line option that you would pass to the Java Antlr tool.
See https://github.com/antlr/antlr4/blob/master/doc/tool-options.md for details.
The equal sign ('=') in an option must be escaped with a backslash, or the entire
arg placed in single or double quotes.

Examples:
    generate -Dlanguage\=Java
    generate ""-Dlanguage=csharp""
    generate -atn
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
