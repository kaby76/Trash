namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using Workspaces;

    class CText
    {
        public Workspace _workspace { get; set; } = new Workspace();

        public Document ReadDoc(string path)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                throw new Exception("File does not exist.");
            }
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file_name))
                {
                    // Read the stream to a string, and write the string to the console.
                    string str = sr.ReadToEnd();
                    document.Code = str;
                }
            }
            catch (IOException)
            {
                throw;
            }
            return document;
        }

        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trtext.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public string Reconstruct(IParseTree tree)
        {
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            int last = -1;
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is TerminalNodeImpl term)
                {
                    var s = term.Symbol.InputStream;
                    var c = term.Payload.StartIndex;
                    var d = term.Payload.StopIndex;
                    if (last != -1 && c != -1 && c > last)
                    {
                        if (s != null)
                        {
                            var txt = s.GetText(new Antlr4.Runtime.Misc.Interval(last, c - 1));
                            sb.Append(txt);
                        }
                    }
                    if (c != -1 && d != -1)
                    {
                        if (s != null)
                        {
                            string txt = s.GetText(new Antlr4.Runtime.Misc.Interval(c, d));
                            sb.Append(txt);
                        }
                        else
                        {
                            string txt = term.Symbol.Text;
                            sb.Append(txt);
                        }
                    }
                    last = d + 1;
                }
                else
                    for (int i = n.ChildCount - 1; i >= 0; i--)
                    {
                        stack.Push(n.GetChild(i));
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
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }
                lines = File.ReadAllText(config.File);
            }
            var line_number = config.LineNumber != null ? (bool)config.LineNumber : false;
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var obj1 in data)
            {
                var nodes = obj1.Nodes;
                var parser = obj1.Parser;
                var lexer = obj1.Lexer;
                var fn = obj1.FileName;
                Document doc = null;
                if (!(fn == null || fn == "stdin"))
                {
                    doc = _workspace.ReadDocument(fn);
                }
                foreach (var node in nodes)
                {
                    if (line_number && doc != null)
                    {
                        var source_interval = node.SourceInterval;
                        int a = source_interval.a;
                        int b = source_interval.b;
                        IToken ta = parser.TokenStream.Get(a);
                        IToken tb = parser.TokenStream.Get(b);
                        var start = ta.StartIndex;
                        var stop = tb.StopIndex + 1;
                        var (line_a, col_a) = new LanguageServer.Module().GetLineColumn(start, doc);
                        var (line_b, col_b) = new LanguageServer.Module().GetLineColumn(stop, doc);
                        System.Console.Write(System.IO.Path.GetFileName(doc.FullPath)
                                             + ":" + line_a + "," + col_a
                                + "-" + line_b + "," + col_b
                                + "\t");
                    }
                    System.Console.WriteLine(this.Reconstruct(node));
                }
            }
        }
    }
}
