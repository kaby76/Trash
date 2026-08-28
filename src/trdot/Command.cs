using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trash
{
    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trdot.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var input = ParsingResultIO.Read(config.File);
            if (config.Bundle)
            {
                using var output = System.Console.OpenStandardOutput();
                ParsingResultIO.WriteFilteredBundle(
                    output, input, ".dot",
                    result => ParsingResultIO.Utf8(Render(result)));
                return;
            }

            foreach (var result in input.Results)
                System.Console.Write(Render(result));
        }

        private static string Render(ParsingResultSet in_tuple)
        {
            var nodes = in_tuple.Nodes;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            foreach (var node in nodes)
            {
                Stack<UnvParseTreeElement> stack = new Stack<UnvParseTreeElement>();
                if (!(node is UnvParseTreeElement nn)) continue;
                stack.Push(nn);
                while (stack.Any())
                {
                    var t = stack.Pop();
                    if (t.IsTerminal())
                    {
                        if (t.NodeType == Antlr4.Runtime.TokenConstants.EOF)
                            sb.AppendLine("Node" + t.GetHashCode().ToString() + " [label=\"EOF\"];");
                        else
                            sb.AppendLine("Node" + t.GetHashCode().ToString()
                                                 + " [label=\""
                                                 + t.LocalName
                                                 + " "
                                                 + TokenOutput.PerformEscapes(t.GetText())
                                                 + "\"];");
                    }
                    else
                    {
                        sb.AppendLine("Node" + t.GetHashCode().ToString()
                                             + " [label=\""
                                             + t.LocalName
                                             + "\"];");
                        for (int i = t.ChildNodes.Length - 1; i >= 0; --i)
                        {
                            var c = t.ChildNodes.item(i);
                            if (!(c is UnvParseTreeElement cc)) continue;
                            stack.Push(cc);
                        }
                    }
                }

                stack.Push(nn);
                while (stack.Any())
                {
                    var t = stack.Pop();
                    for (int i = 0; i < t.ChildNodes.Length; ++i)
                    {
                        var c = t.ChildNodes.item(i);
                        if (!(c is UnvParseTreeElement cc)) continue;
                        sb.AppendLine("Node" + t.GetHashCode().ToString()
                                             + " -> "
                                             + "Node" + c.GetHashCode().ToString()
                                             + ";");
                        stack.Push(cc);
                    }
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
