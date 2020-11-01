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
            System.Console.WriteLine(@"text
Reads a tree from stdin and prints the source text.

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

        public void Execute(Repl repl, ReplParser.TextContext tree)
        {
            var pair = repl.tree_stack.Pop();
            var nodes = pair.Item1;
            foreach (var node in nodes)
            {
                System.Console.WriteLine(Reconstruct(node));
            }
        }
    }
}
