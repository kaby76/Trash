namespace Trash.Commands
{
    using System.Linq;
    using System.Text;

    class CEcho
    {
        public void Help()
        {
            System.Console.WriteLine(@"Echo <string>*
Echo string literals and write to stdout.

Example:
    echo ""1 + 2""
");
        }

        public void Execute(Repl repl, ReplParser.EchoContext tree, bool piped)
        {
            var args = tree.arg();
            var list = args?.Select(t => repl.GetArg(t)).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var input in list)
            {
                sb.Append(input);
            }
            repl.input_output_stack.Push(sb.ToString());
        }
    }
}
