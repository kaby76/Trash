namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Linq;
    using System.Text.Json;
    using ZhangShashaCSharp;

    class CDiff
    {
        public void Help()
        {
            System.Console.WriteLine(@"diff <g1> <g2>
Compute the diff between grammars g1 and g2, which are serialize tree output files.
The start of each grammar is a string, representing the start rule.

Example:
    diff a.tree b.tree
");
        }

        private Tree ToTree(IParseTree t, Parser parser, Lexer lexer)
        {
            var result = new Tree(t, parser, lexer);
            return result;
        }

        public void Execute(Repl repl, ReplParser.DiffContext tree, bool piped)
        {
            var args = tree.arg();
            var expr = args.Select(t => repl.GetArg(t)).ToList();
            var list = new Globbing().Contents(args?.Select(t => repl.GetArg(t)).ToList());
            var lines = list.Select(f => System.IO.File.ReadAllText(f.FullName)).ToList();
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var tuples = lines.Select(t => JsonSerializer.Deserialize<AntlrJson.ParseInfo>(t, serializeOptions)).ToList();
            var lexer = tuples[0].Lexer;
            var parser = tuples[0].Parser;
            var trees = tuples.Select(t => new Tree(t.Nodes[0], parser, lexer)).ToList();
            var result = ZhangShashaCSharp.Tree.ZhangShasha(trees[0], trees[1]);
            foreach (var o in result.Item2)
            {
                System.Console.Write(o.O.ToString() + " " + o.N1 + " => " + o.N2 + " ");
                if (o.O == Operation.Op.Insert)
                {
                    System.Console.Write(trees[1].labels[o.N2]);
                    var zs_node = trees[1].all_nodes[o.N2];
                    var a_node = trees[1].antlr_nodes[zs_node];
                    if (a_node is TerminalNodeImpl term)
                    {
                        var name = term.Symbol.Text;
                        System.Console.Write(" " + name);
                    }
                }
                System.Console.WriteLine();
            }
        }
    }
}
