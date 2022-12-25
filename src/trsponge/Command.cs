namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using AntlrTreeEditing.AntlrDOM;
    using LanguageServer;
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
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trsponge.readme.md"))
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
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
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
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.MaxDepth = 10000;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var code = parse_info.Text;
                var fn = parse_info.FileName;
                if (config.OutputDirectory != null)
		{
                    Directory.CreateDirectory(config.OutputDirectory);
                    if (!(config.OutputDirectory.EndsWith("\\") || config.OutputDirectory.EndsWith("/")))
                        config.OutputDirectory = config.OutputDirectory + "/";
                    fn = config.OutputDirectory + Path.GetFileName(fn);
                }
                if (File.Exists(fn) && (!(bool)config.Clobber ))
                    throw new System.Exception("Attempting to overwrite '" + fn + "'. Use -c/--clobber option if it is intended.");
                System.Console.Error.WriteLine("Writing to " + fn);
                StringBuilder sb = new StringBuilder();
                foreach (var v in nodes)
                {
                    sb.Append(this.Reconstruct(v));
                }
                File.WriteAllText(fn, sb.ToString());
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
    }
}
