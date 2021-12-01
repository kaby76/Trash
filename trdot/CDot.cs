namespace Trash
{
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Linq;

    class CDot
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
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
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
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var in_tuple in data)
            {
                var nodes = in_tuple.Nodes;
                var lexer = in_tuple.Lexer;
                var parser = in_tuple.Parser;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("digraph G {");
                foreach (var node in nodes)
                {
                    Stack<IParseTree> stack = new Stack<IParseTree>();
                    stack.Push(node);
                    while (stack.Any())
                    {
                        var t = stack.Pop();
                        if (t is TerminalNodeImpl tni)
                        {
                            if (tni.Symbol.Type == Antlr4.Runtime.TokenConstants.EOF)
                                sb.AppendLine("Node" + t.GetHashCode().ToString() + " [label=\"EOF\"];");
                            else
                                sb.AppendLine("Node" + t.GetHashCode().ToString()
                                    + " [label=\""
                                    + LanguageServer.TreeOutput.PerformEscapes(Trees.GetNodeText(t, parser.RuleNames))
                                    + "\"];");
                        }
                        else
                        {
                            sb.AppendLine("Node" + t.GetHashCode().ToString()
                                                    + " [label=\""
                                                    + LanguageServer.TreeOutput.PerformEscapes(Trees.GetNodeText(t, parser.RuleNames))
                                                    + "\"];");
                            for (int i = t.ChildCount - 1; i >= 0; --i)
                            {
                                var c = t.GetChild(i);
                                stack.Push(c);
                            }
                        }
                    }
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
                System.Console.WriteLine(sb.ToString());
            }
        }
    }
}
