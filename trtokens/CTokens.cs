namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class CTokens
    {
        public void Help()
        {
            System.Console.WriteLine(@"tokens
Print out the tokens for the parse tree.

Example:
    . | tokens
");
        }

        private string Reconstruct(IParseTree tree)
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
            for (; ; )
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var nodes = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var fn = parse_info.FileName;
            foreach (var node in nodes)
            {
            }
        }
    }
}
