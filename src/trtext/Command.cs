namespace Trash
{
    using AntlrTreeEditing.AntlrDOM;
    using org.w3c.dom;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

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
            int last = -1;
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is AntlrAttr a)
                {
                    sb.Append(a.StringValue);
                }
                else if (n is AntlrText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is AntlrElement e)
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
                for (; ; )
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
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            bool more_than_one_fn = data.Count() > 1;
            bool files_with_matches = config.FilesWithMatches;
            bool files_without_match = config.FilesWithoutMatch;
            bool line_number = config.LineNumber;
            bool count = config.Count;
            foreach (var obj1 in data)
            {
                var fn = obj1.FileName;
                var nodes = obj1.Nodes;
                var parser = obj1.Parser;
                var lexer = obj1.Lexer;
                if (files_with_matches)
                {
                    if (nodes.Any()) System.Console.WriteLine(fn);
                    continue;
                }
                if (files_without_match)
                {
                    if (! nodes.Any()) System.Console.WriteLine(fn);
                    continue;
                }
                if (count)
                {
                    if (more_than_one_fn)
                        System.Console.Write(fn + ":");
                    System.Console.WriteLine(nodes.Count());
                    continue;
                }
                {
                    foreach (var node in nodes)
                    {
                        if (more_than_one_fn)
                            System.Console.Write(fn + ":");
                        //if (line_number)
                        //{
                        //    var source_interval = node.SourceInterval;
                        //    int a = source_interval.a;
                        //    int b = source_interval.b;
                        //    IToken ta = parser.TokenStream.Get(a);
                        //    IToken tb = parser.TokenStream.Get(b);
                        //    var start = ta.StartIndex;
                        //    var stop = tb.StopIndex + 1;
                        //    var (line_a, col_a) = new LanguageServer.Module().GetLineColumn(start, doc);
                        //    var (line_b, col_b) = new LanguageServer.Module().GetLineColumn(stop, doc);
                        //    System.Console.Write(line_a + "," + col_a
                        //            + "-" + line_b + "," + col_b + ":");
                        //}
                        System.Console.WriteLine(this.Reconstruct(node));
                    }
                }
            }
        }
    }
}
