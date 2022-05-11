namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trinsert.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
            var str = config.Expr.Skip(1).First();
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
                System.Console.Error.WriteLine("str = >>>" + str + "<<<");
            }
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
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var trees = parse_info.Nodes;
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                var before_tokens = tokstream.GetTokens();
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

                    foreach (var node in nodes)
                    {
                        TerminalNodeImpl leaf = TreeEdits.Frontier(node).First();
                        // There are two ways to insert text: as a token/tree node,
                        // or in the intertoken character range between tokens in the
                        // token stream. Both have issues, but
                        // there are differences.

                        if (config.AsTree)
                        {
                            throw new System.NotImplementedException();
                        }
                        else
                        {
                            // Insert in the char stream and adjust tokens.
                            var t = node.Payload as AltAntlr.MyToken;
                            var cs = t.InputStream as AltAntlr.MyCharStream;
                            var old_buffer = cs.Text;
                            var index = LanguageServer.Util.GetIndex(t.Line, t.Column, old_buffer);
                            if (config.After) index += t.Text.Length;
                            var add = str.Length;
                            var new_buffer = old_buffer.Insert(index, str);
                            var start = leaf.Payload.StartIndex;
                            if (config.After) start += +1;
                            Dictionary<int,int> old_indices = new Dictionary<int,int>();
                            var i = start;
                            tokstream.Seek(i);
                            for (; ; )
                            {
                                if (i >= tokstream.Size) break;
                                var tt = tokstream.Get(i);
                                if (tt.Type == -1) break;
                                var tok = tt as AltAntlr.MyToken;
                                var line = tok.Line;
                                var col = tok.Column;
                                var i2 = LanguageServer.Util.GetIndex(line, col, old_buffer);
                                old_indices[i] = i2;
                                ++i;
                            }
                            i = start;
                            tokstream.Seek(i);
                            cs.Text = new_buffer;
                            text = new_buffer;
                            tokstream.Text = new_buffer;
                            for (; ; )
                            {
                                if (i >= tokstream.Size) break;
                                var tt = tokstream.Get(i);
                                if (tt.Type == -1) break;
                                var tok = tt as AltAntlr.MyToken;
                                var (line, col) = LanguageServer.Util.GetLineColumn(old_indices[i], text);
                                tok.Line = line;
                                tok.Column = col;
                                tok.StartIndex += add;
                                tok.StopIndex += add;
                                ++i;
                            }
                        }
                    }
                    var tuple = new ParsingResultSet()
                    {
                        Text = text,
                        FileName = fn,
                        Stream = tokstream,
                        Nodes = trees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }

        private string OutputTokens(IParseTree tree)
        {
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is TerminalNodeImpl term)
                {
                    sb.AppendLine(term.Symbol.ToString());
                }
                else
                    for (int i = n.ChildCount - 1; i >= 0; i--)
                    {
                        stack.Push(n.GetChild(i));
                    }
            }
            return sb.ToString();
        }

    }
}
