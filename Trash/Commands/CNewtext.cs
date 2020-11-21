namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using AntlrJson;
    using System;
    using LanguageServer;
    using Algorithms;

    class CNewtext
    {
        public void Help()
        {
            System.Console.WriteLine(@"text line-number?
Reads a tree from stdin and prints the source text. If 'line-number' is
specified, the line number range for the tree is printed.

Example:
    find //lexerRuleSpec | text
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

        public void Execute(Repl repl, ReplParser.NewtextContext tree, bool piped)
        {
            var arg = tree.arg()?.GetText();
            var line_number = (arg == "line-number");
            var pair = repl.tree_stack.Pop();
            //var nodes = pair.Item1;
            //var parser = pair.Item2;
            //var doc = pair.Item3;
            var lines = pair.Item4;
            var doc = repl.stack.Peek();
            var pr = ParsingResultsFactory.Create(doc);
            var lexer = pr.Lexer;
            var parser = pr.Parser;
            var serializeOptions = new JsonSerializerOptions();
            if (tree.arg()?.GetText() == "2")
            {
                serializeOptions.Converters.Add(new AntlrJson.Impl2.ParseTreeConverter(null, null, lexer, parser));
                serializeOptions.WriteIndented = true;
                var obj1 = JsonSerializer.Deserialize<IParseTree>(lines, serializeOptions);
                if (obj1 == null) return;
                var nodes = new IParseTree[] { obj1 };
                foreach (var node in nodes)
                {
                    if (line_number)
                    {
                        var source_interval = node.SourceInterval;
                        int a = source_interval.a;
                        int b = source_interval.b;
                        IToken ta = parser.TokenStream.Get(a);
                        IToken tb = parser.TokenStream.Get(b);
                        var start = ta.StartIndex;
                        var stop = tb.StopIndex + 1;
                        if (doc != null)
                        {
                            var (line_a, col_a) = new LanguageServer.Module().GetLineColumn(start, doc);
                            var (line_b, col_b) = new LanguageServer.Module().GetLineColumn(stop, doc);
                            System.Console.Write(System.IO.Path.GetFileName(doc.FullPath)
                                + ":" + line_a + "," + col_a
                                + "-" + line_b + "," + col_b
                                + "\t");
                        }
                    }
                    System.Console.WriteLine(this.Reconstruct(node));
                }
            }
            else
            {
                serializeOptions.Converters.Add(new AntlrJson.Impl1.ParseTreeConverter(parser));
                serializeOptions.Converters.Add(new AntlrJson.Impl1.TokenConverter());
                serializeOptions.Converters.Add(new AntlrJson.Impl1.TokenStreamConverter());
                serializeOptions.WriteIndented = true;
                var obj1 = JsonSerializer.Deserialize<MyTuple<IParseTree, ITokenStream>>(lines, serializeOptions);
                var nodes = new IParseTree[] { obj1.Item1 };
                foreach (var node in nodes)
                {
                    if (line_number)
                    {
                        var source_interval = node.SourceInterval;
                        int a = source_interval.a;
                        int b = source_interval.b;
                        IToken ta = parser.TokenStream.Get(a);
                        IToken tb = parser.TokenStream.Get(b);
                        var start = ta.StartIndex;
                        var stop = tb.StopIndex + 1;
                        if (doc != null)
                        {
                            var (line_a, col_a) = new LanguageServer.Module().GetLineColumn(start, doc);
                            var (line_b, col_b) = new LanguageServer.Module().GetLineColumn(stop, doc);
                            System.Console.Write(System.IO.Path.GetFileName(doc.FullPath)
                                + ":" + line_a + "," + col_a
                                + "-" + line_b + "," + col_b
                                + "\t");
                        }
                    }
                    System.Console.WriteLine(this.Reconstruct(node));
                }
            }
        }
    }
}
