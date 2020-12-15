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
//                System.Console.Write(o.O.ToString() + " " + o.N1 + " => " + o.N2 + " ");
                if (o.O == Operation.Op.Insert)
                {
                    var zs_node = trees[1].all_nodes[o.N2];
//                    System.Console.Write(zs_node.label);
                    var a_node = trees[1].antlr_nodes[zs_node];
                    if (a_node is TerminalNodeImpl term)
                    {
                        var name = term.Symbol.Text;
                        System.Console.WriteLine("+++ " + name);
                    }
                }
                else if (o.O == Operation.Op.Delete)
                {
                    var zs_node = trees[0].all_nodes[o.N1];
                    //                    System.Console.Write(zs_node.label);
                    var a_node = trees[0].antlr_nodes[zs_node];
                    if (a_node is TerminalNodeImpl term)
                    {
                        var name = term.Symbol.Text;
                        System.Console.WriteLine("--- " + name);
                    }
                }
                else if (o.O == Operation.Op.Change)
                {
                    var zs_node = trees[0].all_nodes[o.N1];
                    var a_node = trees[0].antlr_nodes[zs_node];
                    var zs_node2 = trees[1].all_nodes[o.N2];
                    var a_node2 = trees[1].antlr_nodes[zs_node2];
                    if (a_node is TerminalNodeImpl term)
                    {
                        var name = term.Symbol.Text;
                        System.Console.WriteLine("--- " + name);
                    }
                    if (a_node2 is TerminalNodeImpl term2)
                    {
                        var name = term2.Symbol.Text;
                        System.Console.WriteLine("+++ " + name);
                    }
                }
            }
            //System.Console.WriteLine("======");
            //foreach (var o in trees[0].all_nodes)
            //{
            //    System.Console.WriteLine(o.Key + " " + o.Value.label + " " + trees[0].antlr_nodes[o.Value]);
            //}
            //System.Console.WriteLine("======");
            //foreach (var o in trees[1].all_nodes)
            //{
            //    System.Console.WriteLine(o.Key + " " + o.Value.label + " " + trees[1].antlr_nodes[o.Value]);
            //}
        }
    }
}
