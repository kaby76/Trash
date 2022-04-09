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
                var atrees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var tokstream = parse_info.Stream as AltAntlr.MyTokenStream;
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                IParseTree[] root = atrees.ToArray();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(atrees, parser))
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
                        int sub = 0;
                        int index = 0;
                        var payload = node.Payload;
                        if (payload is AltAntlr.MyParserRuleContext prc)
                        {
                            var xi = prc.SourceInterval;
                            var s = xi.a;
                            var e = xi.b;
                            sub = 1 + e - s;
                            index = s;
                        }
                        else if (payload is AltAntlr.MyToken t)
                        {
                            var s = t.StartIndex;
                            var e = t.StopIndex;
                            sub = 1 + e - s;
                            index = s;
                        }
                        ts = first_token.InputStream as AltAntlr.MyCharStream;
                        var old_buffer = ts.Text;
                        var new_buffer = old_buffer.Remove(index, sub);
                        var last = leaves.Last() as AltAntlr.MyTerminalNodeImpl;
                        var start = last.Payload.TokenIndex + 1;
                        Dictionary<int, int> old_indices = new Dictionary<int, int>();
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
                        ts.Text = new_buffer;
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
                            tok.StartIndex -= sub;
                            tok.StopIndex -= sub;
                            ++i;
                        }
                        TreeEdits.Delete(node);
                        // Adjust intervals up the tree.
                    }

                    var tuple = new ParsingResultSet()
                    {
                        Text = text,
                        FileName = fn,
                        Stream = tokstream,
                        Nodes = atrees,
                        Lexer = lexer,
                        Parser = parser
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
