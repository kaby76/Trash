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
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }

                for (;;)
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }

                lines = lines.Trim();
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }

                lines = File.ReadAllText(config.File);
            }

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            AntlrJson.ParsingResultSet[] data =
                JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            List<ParsingResultSet> results = new List<ParsingResultSet>();
            foreach (var in_tuple in data)
            {
                var nodes = in_tuple.Nodes;
                var lexer = in_tuple.Lexer;
                var parser = in_tuple.Parser;
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
                                                     + ParseTreeEditing.UnvParseTreeDOM.TokenOutput.PerformEscapes(t.GetText())
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
                System.Console.WriteLine(sb.ToString());
            }
        }
    }
}
