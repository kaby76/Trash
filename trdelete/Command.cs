namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trdelete.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            var expr = config.Expr.First();
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("Expr = >>>" + expr + "<<<");
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
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var trees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                IParseTree[] root = trees.ToArray();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(trees, parser))
                {
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    foreach (var node in nodes)
                    {
                        AltAntlr.MyCharStream ts = null;
                        var leaves = TreeEdits.Frontier(node);
                        var first = leaves.First() as AltAntlr.MyTerminalNodeImpl;
                        var first_token = first.Payload as AltAntlr.MyToken;
                        var last = leaves.Last() as AltAntlr.MyTerminalNodeImpl;
                        var last_token = last.Payload as AltAntlr.MyToken;
                        int sub = 0;
                        int index = 0;
                        var s = first_token.StartIndex;
                        var e = last_token.StopIndex;
                        sub = 1 + e - s;
                        index = s;
                        ts = first_token.InputStream as AltAntlr.MyCharStream;
                        var old_buffer = ts.Text;
                        var new_buffer = old_buffer.Remove(index, sub);
                        var start = last.Payload.TokenIndex + 1;
                        Dictionary<int, int> old_indices = new Dictionary<int, int>();
                        var i = start;
                        tokstream.Seek(i);
                        for (; ; )
                        {
                            if (i >= tokstream.Size) break;
                            var tt = tokstream.Get(i);
                            var tok = tt as AltAntlr.MyToken;
                            var line = tok.Line;
                            var col = tok.Column;
                            var i2 = LanguageServer.Util.GetIndex(line, col, old_buffer);
                            old_indices[i] = i2;
                            if (tt.Type == -1) break;
                            ++i;
                        }
                        i = start;
                        tokstream.Seek(i);
                        ts.Text = new_buffer;
                        text = new_buffer;
                        tokstream.Text = new_buffer;
                        for (; ; )
                        {
                            if (i >= tokstream.Size) break;
                            var tt = tokstream.Get(i);
                            var tok = tt as AltAntlr.MyToken;
                            var new_index = old_indices[i] - sub;
                            if (new_index >= 0)
                            {
                                var (line, col) = LanguageServer.Util.GetLineColumn(new_index, text);
                                tok.Line = line;
                                tok.Column = col;
                            }
                            tok.StartIndex -= sub;
                            tok.StopIndex -= sub;
                            if (tt.Type == -1) break;
                            ++i;
                        }
                        TreeEdits.Delete(node);
                        // Nuke tokens in token stream.
                        tokstream.Seek(first_token.TokenIndex);
                        for (i = first_token.TokenIndex; i <= last_token.TokenIndex; ++i)
                            tokstream.Delete();

                        for (i = 0; i < tokstream.Size; ++i)
                        {
                            var t = tokstream.Get(i) as AltAntlr.MyToken;
                            t.TokenIndex = i;
                        }

                        // Adjust intervals up the tree.
                        foreach (var tree in trees)
                            Reset(tree);
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
        private string OutputTokens(AltAntlr.MyTokenStream tokstream, IParseTree tree)
        {
            var frontier = TreeEdits.Frontier(tree).ToList();
            var first = frontier.First();
            var last = frontier.Last();
            var first_index = first.Payload.TokenIndex;
            var last_index = last.Payload.TokenIndex;
            StringBuilder sb = new StringBuilder();
            for (var i = first_index; i <= last_index; i++)
            {
                var token = tokstream.Get(i);
                sb.AppendLine(token.ToString());
            }
            return sb.ToString();
        }

        private void Reset(IParseTree tree)
        {
            if (tree is AltAntlr.MyTerminalNodeImpl l)
            {
                var t = l.Payload as AltAntlr.MyToken;
                l.Start = t.TokenIndex;
                l.Stop = t.TokenIndex;
                l._sourceInterval = new Antlr4.Runtime.Misc.Interval(t.TokenIndex, t.TokenIndex);
            }
            else if (tree is AltAntlr.MyParserRuleContext p)
            {
                var res = p.SourceInterval;
                int min = int.MaxValue;
                int max = int.MinValue;
                for (int i = 0; i < tree.ChildCount; ++i)
                {
                    var c = tree.GetChild(i);
                    Reset(c);
                    min = Math.Min(min, c.SourceInterval.a);
                    max = Math.Max(max, c.SourceInterval.b);
                }
                p._sourceInterval = res;
            }
        }

        private void Adjust(IParseTree tree)
        {
            Reset(tree);
            var leaves = TreeEdits.Frontier(tree);
            Stack<IParseTree> stack = new Stack<IParseTree>();
            foreach (var leaf in leaves) stack.Push(leaf);
            while (stack.Count > 0)
            {
                var leaf = stack.Pop();
                if (leaf is AltAntlr.MyTerminalNodeImpl l)
                {
                    var t = l.Payload as AltAntlr.MyToken;
                    l._sourceInterval = new Antlr4.Runtime.Misc.Interval(t.TokenIndex, t.TokenIndex);
                }
                else if (leaf is AltAntlr.MyParserRuleContext p)
                {
                    var s = p.Start;
                    var e = p.Stop;
                    
                }
            }
        }
    }
}
