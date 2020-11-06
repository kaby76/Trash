namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design.Serialization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Text.RegularExpressions;

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
                    var ais = s as AntlrInputStream;
                    var c = term.Payload.StartIndex;
                    var d = term.Payload.StopIndex;
                    if (last != -1 && c != -1 && c > last)
                    {
                        var txt = ais.GetText(new Antlr4.Runtime.Misc.Interval(last, c - 1));
                        sb.Append(txt);
                    }
                    if (c != -1 && d != -1)
                    {
                        var txt = ais.GetText(new Antlr4.Runtime.Misc.Interval(c, d));
                        sb.Append(txt);
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
            var pair = repl.tree_stack.Pop();
            var nodes = pair.Item1;
            var parser = pair.Item2;
            var doc = pair.Item3;
            var lines = pair.Item4;
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
