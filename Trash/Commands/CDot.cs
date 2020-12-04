using System.Collections;
using System.Linq;

namespace Trash.Commands
{
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Text;
    using Antlr4.Runtime.Tree;


    class CDot
    {
        public void Help()
        {
            System.Console.WriteLine(@"dot
Print out the parse tree in Dot format.

Example:
    . | dot > output.dot
");
        }

        public void Execute(Repl repl, ReplParser.DotContext tree, bool piped)
        {
            string lines = repl.input_output_stack.Pop();
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var parse_info = JsonSerializer.Deserialize<AntlrJson.ParseInfo>(lines, serializeOptions);
            var nodes = parse_info.Nodes;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            Stack<IParseTree> stack = new Stack<IParseTree>();
            foreach (var node in nodes)
            {
                stack.Push(node);
                while (stack.Any())
                {
                    var t = stack.Pop();
                    sb.AppendLine("Node" + t.GetHashCode().ToString()
                                          + " [label=\""
                                          + Trees.GetNodeText(t, parse_info.Parser.RuleNames)
                                          + "\"];");
                    for (int i = t.ChildCount-1; i >= 0; --i)
                    {
                        var c = t.GetChild(i);
                        stack.Push(c);
                    }
                }
            }
            foreach (var node in nodes)
            {
                stack.Push(node);
                while (stack.Any())
                {
                    var t = stack.Pop();
                    for (int i = 0; i < t.ChildCount; ++i)
                    {
                        var c = t.GetChild(i);
                        sb.AppendLine("Node" + t.GetHashCode().ToString()
                                             + " -> "
                                             + "Node" + c.GetHashCode().ToString()
                                             + ";");
                        stack.Push(c);
                    }
                }
            }
            sb.AppendLine("}");
            repl.input_output_stack.Push(sb.ToString());
        }
    }
}
