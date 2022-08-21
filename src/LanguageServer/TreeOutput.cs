namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TreeOutput
    {
        private static int changed = 0;
        private static bool first_time = true;

        public static StringBuilder OutputTree(IParseTree tree, Lexer lexer, Parser parser, CommonTokenStream stream)
        {
            changed = 0;
            first_time = true;
            var sb = new StringBuilder();
            ParenthesizedAST(tree, sb, lexer, parser, stream);
            return sb;
        }

        private static void ParenthesizedAST(IParseTree tree, StringBuilder sb, Lexer lexer, Parser parser, CommonTokenStream stream, int level = 0)
        {
            // Antlr always names a non-terminal with first letter lowercase,
            // but renames it when creating the type in C#. So, remove the prefix,
            // lowercase the first letter, and remove the trailing "Context" part of
            // the name. Saves big time on output!
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl leaf = tree as TerminalNodeImpl;
                IList<IToken> inter = null;
                if (leaf.Symbol.TokenIndex >= 0)
                    inter = stream?.GetHiddenTokensToLeft(leaf.Symbol.TokenIndex);
                if (inter != null)
                    foreach (var t in inter)
                    {
                        StartLine(sb, level);
                        sb.Append(
                            "( interleaf"
                                + " text:" + PerformEscapes(t.Text)
                                + " chnl:" + lexer.ChannelNames[t.Channel]
                                + " l:" + t.Line
                                + " c:" + t.Column
                                + " si:" + t.StartIndex
                                + " ei:" + t.StopIndex
                                + " ti:" + t.TokenIndex
                                );
                        if (lexer is AltAntlr.MyLexer mylexer)
                        {
                            var ss = mylexer._inputstream;
                            var s = ss as AltAntlr.MyCharStream;
                            var st = s.Text.Substring(t.StartIndex, t.StopIndex - t.StartIndex + 1);
                            sb.Append(" tstext:" + PerformEscapes(st));
                        }
                        sb.AppendLine();
                    }
                {
                    var ty = leaf.Symbol.Type;
                    var t = leaf.Symbol;
                    var name = lexer.Vocabulary.GetSymbolicName(ty);
                    StartLine(sb, level);
                    sb.Append(
                        "( " + name
                        + " int:" + tree.SourceInterval
                        + " text:" + PerformEscapes(leaf.GetText())
                        + " tt:" + leaf.Symbol.Type
                        + " chnl:" + lexer.ChannelNames[leaf.Symbol.Channel]
                        + " text:" + PerformEscapes(t.Text)
                        + " chnl:" + lexer.ChannelNames[t.Channel]
                        + " l:" + t.Line
                        + " c:" + t.Column
                        + " si:" + t.StartIndex
                        + " ei:" + t.StopIndex
                        + " ti:" + t.TokenIndex
                        );
                    if (lexer is AltAntlr.MyLexer mylexer)
                    {
                        var ss = mylexer._inputstream;
                        var s = ss as AltAntlr.MyCharStream;
                        var st = s.Text.Substring(t.StartIndex, t.StopIndex - t.StartIndex + 1);
                        sb.Append(" tstext:" + PerformEscapes(st));
                    }
                    sb.AppendLine();
                }
            }
            else
            {
                var x = tree as RuleContext;
		        var ri = x.RuleIndex;
                var name = parser.RuleNames.Length <= ri ? "unknown" : parser.RuleNames[ri];
                StartLine(sb, level);
                sb.Append(
                    "( " + name
                    + " int:" + tree.SourceInterval
                    );
                sb.AppendLine();
            }
            for (int i = 0; i < tree.ChildCount; ++i)
            {
                var c = tree.GetChild(i);
                ParenthesizedAST(c, sb, lexer, parser, stream, level + 1);
            }
            if (level == 0)
            {
                for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                sb.AppendLine();
                changed = 0;
            }
        }

        private static void StartLine(StringBuilder sb, int level = 0)
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

        private static string ToLiteral(string input)
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

        public static string PerformEscapes(string s)
        {
            StringBuilder new_s = new StringBuilder();
            new_s.Append(ToLiteral(s));
            return new_s.ToString();
        }
    }
}
