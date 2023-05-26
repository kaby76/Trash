namespace Trash
{
    using ParseTreeEditing.UnvParseTreeDOM;
    using org.w3c.dom;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtext.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public string Reconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeAttr a)
                {
                    if (a.Name as string == "Before")
                        sb.Append(a.StringValue);
                }
                else if (n is UnvParseTreeText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is UnvParseTreeElement e)
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        stack.Push(n.ChildNodes.item(i));
                    }
                }
            }
            return sb.ToString();
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
            serializeOptions.WriteIndented = false;
            serializeOptions.MaxDepth = 10000;
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("deserialized");
            bool more_than_one_fn = data.Count() > 1;
            foreach (var obj1 in data)
            {
                var fn = obj1.FileName;
                var nodes = obj1.Nodes;
                var parser = obj1.Parser;
                var lexer = obj1.Lexer;
                var text = System.IO.File.ReadAllLines(fn);
                var current_line = 1;
                foreach (var node in nodes)
                {
                    int line = GetLine(node);
                    int col = GetColumn(node);
                    if (line <= 0) continue;
                    if (config.Prefix)
                    {
                        for (;;)
                        {
                            if (current_line < line)
                            {
                                System.Console.WriteLine(text[current_line - 1]);
                                current_line++;
                                break;
                            }
                        }
                    }
                    string file_name = fn + ":";
                    string line_number = "L" + line.ToString() + ": ";
                    if (more_than_one_fn)
                        System.Console.Write(file_name);
                    System.Console.Write(line_number);
                    System.Console.WriteLine(text[line - 1].Replace('\t',' '));
                    System.Console.WriteLine(new string(' ', (more_than_one_fn?file_name.Length:0) + line_number.Length + col) + "^");
                }
            }
        }

        private int GetColumn(UnvParseTreeNode node)
        {
            foreach (var c in node.ChildNodes.All())
            {
                if (c is UnvParseTreeAttr a && a.Name as string == "Column")
                {
                    return int.Parse(a.StringValue);
                }
            }

            // Go down a level.
            foreach (var c in node.ChildNodes.All())
            {
                return GetColumn(c as UnvParseTreeNode);
            }

            return 0;
        }

        private int GetLine(UnvParseTreeNode node)
        {
            foreach (var c in node.ChildNodes.All())
            {
                if (c is UnvParseTreeAttr a && a.Name as string == "Line")
                {
                    return int.Parse(a.StringValue);
                }
            }
            // Go down a level.
            foreach (var c in node.ChildNodes.All())
            {
                return GetLine(c as UnvParseTreeNode);
            }
            return 0;
        }
    }
}
