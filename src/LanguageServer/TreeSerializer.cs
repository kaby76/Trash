namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class TreeSerializer
    {
        private static int changed = 0;
        private static bool first_time = true;

        public static void ParenthesizedAST(this IParseTree tree, StringBuilder sb, string file_name, CommonTokenStream stream, int level = 0)
        {
            // Antlr always names a non-terminal with first letter lowercase,
            // but renames it when creating the type in C#. So, remove the prefix,
            // lowercase the first letter, and remove the trailing "Context" part of
            // the name. Saves big time on output!
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                Interval interval = tok.SourceInterval;

                System.Collections.Generic.IList<IToken> inter = tok.Symbol.TokenIndex >= 0 ? stream.GetHiddenTokensToLeft(tok.Symbol.TokenIndex) : null;
                if (inter != null)
                {
                    foreach (IToken t in inter)
                    {
                        StartLine(sb, file_name, tree, stream, level);
                        sb.AppendLine("( HIDDEN text=" + t.Text.PerformEscapes());
                    }
                }

                StartLine(sb, file_name, tree, stream, level);
                sb.AppendLine("( TOKEN i=" + tree.SourceInterval.a
                    + " t=" + tree.GetText().PerformEscapes());
            }
            else
            {
                string fixed_name = tree.GetType().ToString()
                    .Replace("Antlr4.Runtime.Tree.", "");
                fixed_name = Regex.Replace(fixed_name, "^.*[+]", "");
                fixed_name = fixed_name.Substring(0, fixed_name.Length - "Context".Length);
                fixed_name = fixed_name[0].ToString().ToLower()
                             + fixed_name.Substring(1);
                StartLine(sb, file_name, tree, stream, level);
                sb.Append("( " + fixed_name);
                if (level == 0)
                {
                    sb.Append(" File=\""
                    + file_name
                    + "\"");
                }

                sb.AppendLine();
            }
            for (int i = 0; i < tree.ChildCount; ++i)
            {
                IParseTree c = tree.GetChild(i);
                c.ParenthesizedAST(sb, file_name, stream, level + 1);
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

        private static void StartLine(StringBuilder sb, string file_name, IParseTree tree, CommonTokenStream stream, int level = 0)
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
                string literal = Regex.Escape(input);
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
