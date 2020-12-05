using System;
using Algorithms;
using LanguageServer;
using Workspaces;

namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class CText
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

        public void Execute(Repl repl, ReplParser.TextContext tree, bool piped)
        {
            var args = tree.arg();
            var arg = args?.GetText();
            var line_number = (arg == "line-number");
            var lines = repl.input_output_stack.Pop();
			var serializeOptions = new JsonSerializerOptions();
			serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
			serializeOptions.WriteIndented = false;
			var obj1 = JsonSerializer.Deserialize<AntlrJson.ParseInfo>(lines, serializeOptions);
			var nodes = obj1.Nodes;
            var parser = obj1.Parser;
            var lexer = obj1.Lexer;
            var fn = obj1.FileName;
            Document doc = null;
            if (!(fn == null || fn == "stdin"))
            {
                doc = repl._docs.ReadDoc(fn);
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
