using Antlr4.Runtime;
using System;
using System.IO;
using System.Text;

namespace ParseTreeEditing.UnvParseTreeDOM
{
    public class TokenOutput
    {
        private static int TokenIndex = -1;

        public static StringBuilder OutputTokens(UnvParseTreeNode tree, Lexer lexer, Parser parser)
        {
            TokenIndex = 0;
            var sb = new StringBuilder();
            ParenthesizedAST(tree, sb, lexer, parser);
            return sb;
        }

        private static void ParenthesizedAST(UnvParseTreeNode tree, StringBuilder sb, Lexer lexer, Parser parser, int level = 0)
        {
            if (tree is UnvParseTreeText t)
            {
                sb.Append(
                    "[@" + TokenIndex++
                    + ","
                    // skip start stop.
                    + "='" + PerformEscapes(t.Data) + "',<" + t.TokenType + ">" + lexer.ChannelNames[t.Channel] + ","
                    + "]"
                    );
                sb.AppendLine();
            }
            else if (tree is UnvParseTreeAttr a)
            {
                sb.Append(
                    "[@" + TokenIndex++
                    + ","
                    // skip start stop.
                    + "='" + PerformEscapes(a.StringValue) + "',<" + a.TokenType + ">" + ((a.Channel >= 0) ? lexer.ChannelNames[a.Channel] : "") + ","
                    + "]"
                    );
                sb.AppendLine();
            }
            for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
            {
                var c = tree.ChildNodes.item(i);
                ParenthesizedAST(c as UnvParseTreeNode, sb, lexer, parser, level + 1);
            }
            if (level == 0)
            {
                sb.AppendLine();
            }
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
