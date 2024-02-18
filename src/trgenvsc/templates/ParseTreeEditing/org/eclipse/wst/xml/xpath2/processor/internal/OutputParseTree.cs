namespace xpath.org.eclipse.wst.xml.xpath2.processor.@internal
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    public class OutputParseTree
    {
        private int changed = 0;
        private bool first_time = true;

        public StringBuilder OutputTokens(CommonTokenStream stream)
        {
            var sb = new StringBuilder();
            foreach (var token in stream.GetTokens())
            {
                sb.AppendLine("Token " + token.TokenIndex + " " + token.Type + " " + "channel " + ((Lexer)stream.TokenSource).ChannelNames[token.Channel]
                              + " " + new OutputParseTree().PerformEscapes(token.Text));
            }
            return sb;
        }

        public StringBuilder OutputTree(IParseTree tree, CommonTokenStream stream, Parser parser, bool do_hidden_tokens)
        {
            var sb = new StringBuilder();
            changed = 0;
            first_time = true;
            ParenthesizedAST(tree, parser, sb, stream, do_hidden_tokens);
            return sb;
        }

        private void ParenthesizedAST(IParseTree tree, Parser parser, StringBuilder sb, CommonTokenStream stream, bool do_hidden_tokens, int level = 0)
        {
            // Antlr always names a non-terminal with first letter lowercase,
            // but renames it when creating the type in C#. So, remove the prefix,
            // lowercase the first letter, and remove the trailing "Context" part of
            // the name. Saves big time on output!
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                Interval interval = tok.SourceInterval;
                if (do_hidden_tokens)
                {
                    IList<IToken> inter = null;
                    if (tok.Symbol.TokenIndex >= 0)
                        inter = stream.GetHiddenTokensToLeft(tok.Symbol.TokenIndex);
                    if (inter != null)
                        foreach (var t in inter)
                        {
                            StartLine(sb, tree, stream, level);
                            sb.AppendLine("( " + ((Lexer) stream.TokenSource).ChannelNames[t.Channel] + " text=" +
                                          PerformEscapes(t.Text));
                        }
                }
                StartLine(sb, tree, stream, level);
                sb.AppendLine("( " + ((Lexer)stream.TokenSource).ChannelNames[tok.Symbol.Channel] + " i=" + tree.SourceInterval.a
                    + " txt=" + PerformEscapes(tree.GetText())
                    + " tt=" + tok.Symbol.Type);
            }
            else
            {
                var t = tree as RuleContext;
                var name = parser.RuleNames[t.RuleIndex];
                StartLine(sb, tree, stream, level);
                sb.Append("( " + name);
                sb.AppendLine();
            }
            for (int i = 0; i < tree.ChildCount; ++i)
            {
                var c = tree.GetChild(i);
                ParenthesizedAST(c, parser, sb, stream, do_hidden_tokens, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                sb.AppendLine();
                changed = 0;
            }
        }

        private void StartLine(StringBuilder sb, IParseTree tree, CommonTokenStream stream, int level = 0)
        {
            if (changed - level >= 0)
            {
                if (!first_time)
                {
                    for (int j = 0; j < level; ++j) sb.Append("  ");
                    for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                    sb.AppendLine();
                }
                changed = 0;
                first_time = false;
            }
            changed = level;
            for (int j = 0; j < level; ++j) sb.Append("  ");
        }

        private string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                var literal = input;
                literal = literal.Replace("\\", "\\\\");
                literal = literal.Replace("\b", "\\b");
                literal = literal.Replace("\n", "\\n");
                literal = literal.Replace("\t", "\\t");
                literal = literal.Replace("\r", "\\r");
                literal = literal.Replace("\f", "\\f");
                literal = literal.Replace("\"", "\\\"");
                literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                return literal;
            }
        }

        public string PerformEscapes(string s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.Append(ToLiteral(s));
            return new_s.ToString();
        }
    }
}
