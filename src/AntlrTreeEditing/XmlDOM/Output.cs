// Template generated code from Antlr4BuildTasks.Template v 8.1
namespace XmlDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class Output
    {
        private static int changed = 0;
        private static bool first_time = true;

        public static StringBuilder OutputTokens(this CommonTokenStream stream, Lexer lexer)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IToken token in stream.GetTokens())
            {
                sb.AppendLine("Token " + token.TokenIndex
                              + " line " + token.Line
                              + " col " + token.Column
                              + " "
                              + GetName(lexer, token.Type)
                              + "(" + token.Type + ")"
                              + " channel " + ((Lexer)stream.TokenSource).ChannelNames[token.Channel] + " " + Output.PerformEscapes(token.Text));
            }
            return sb;
        }

        public static string GetName(Lexer lexer, int type)
        {
            if (type == -1) return "EOF";
            var vocab = lexer.Vocabulary;
            if (vocab.GetLiteralName(type) != null) return vocab.GetLiteralName(type);
            if (vocab.GetSymbolicName(type) != null) return vocab.GetSymbolicName(type);
            else return type.ToString();
        }
        public static StringBuilder OutputTree(this IParseTree tree, CommonTokenStream stream)
        {
            StringBuilder sb = new StringBuilder();
            changed = 0;
            first_time = true;
            tree.ParenthesizedAST(sb, stream);
            return sb;
        }

        private static void ParenthesizedAST(this IParseTree tree, StringBuilder sb, CommonTokenStream stream, int level = 0)
        {
            // Antlr always names a non-terminal with first letter lowercase,
            // but renames it when creating the type in C#. So, remove the prefix,
            // lowercase the first letter, and remove the trailing "Context" part of
            // the name. Saves big time on output!
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                Interval interval = tok.SourceInterval;
                IList<IToken> inter = null;
                if (tok.Symbol.TokenIndex >= 0)
                {
                    inter = stream.GetHiddenTokensToLeft(tok.Symbol.TokenIndex);
                }

                if (inter != null)
                {
                    foreach (IToken t in inter)
                    {
                        StartLine(sb, tree, stream, level);
                        sb.AppendLine("( " + ((Lexer)stream.TokenSource).ChannelNames[t.Channel] + " text=" + t.Text.PerformEscapes());
                    }
                }

                StartLine(sb, tree, stream, level);
                sb.AppendLine("( " + ((Lexer)stream.TokenSource).ChannelNames[tok.Symbol.Channel] + " i=" + tree.SourceInterval.a
                    + " txt=" + tree.GetText().PerformEscapes()
                    + " tt=" + tok.Symbol.Type);
            }
            else
            {
                string fixed_name = tree.GetType().ToString()
                    .Replace("Antlr4.Runtime.Tree.", "");
                fixed_name = Regex.Replace(fixed_name, "^.*[+]", "");
                fixed_name = fixed_name.Substring(0, fixed_name.Length - "Context".Length);
                fixed_name = fixed_name[0].ToString().ToLower()
                             + fixed_name.Substring(1);
                StartLine(sb, tree, stream, level);
                sb.Append("( " + fixed_name);
                sb.AppendLine();
            }
            for (int i = 0; i < tree.ChildCount; ++i)
            {
                IParseTree c = tree.GetChild(i);
                c.ParenthesizedAST(sb, stream, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k)
                {
                    sb.Append(") ");
                }

                sb.AppendLine();
                changed = 0;
            }
        }

        private static void StartLine(StringBuilder sb, IParseTree tree, CommonTokenStream stream, int level = 0)
        {
            if (changed - level >= 0)
            {
                if (!first_time)
                {
                    for (int j = 0; j < level; ++j)
                    {
                        sb.Append("  ");
                    }

                    for (int k = 0; k < 1 + changed - level; ++k)
                    {
                        sb.Append(") ");
                    }

                    sb.AppendLine();
                }
                changed = 0;
                first_time = false;
            }
            changed = level;
            for (int j = 0; j < level; ++j)
            {
                sb.Append("  ");
            }
        }

        private static string ToLiteral(this string input)
        {
            using (StringWriter writer = new StringWriter())
            {
                string literal = input;
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

        public static string PerformEscapes(this string s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.Append(s.ToLiteral());
            return new_s.ToString();
        }
    }
}
