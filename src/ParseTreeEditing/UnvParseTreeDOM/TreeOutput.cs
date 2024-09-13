using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace ParseTreeEditing.UnvParseTreeDOM;

public class TreeOutput
{
    private static int changed = 0;
    private static bool first_time = true;

    public static StringBuilder OutputTree(UnvParseTreeNode tree, Lexer lexer, Parser parser)
    {
        changed = 0;
        first_time = true;
        var sb = new StringBuilder();
        ParenthesizedAST(tree, sb, lexer, parser);
        return sb;
    }

    private static void ParenthesizedAST(UnvParseTreeNode tree, StringBuilder sb, Lexer lexer, Parser parser, int level = 0)
    {
        // Antlr always names a non-terminal with first letter lowercase,
        // but renames it when creating the type in C#. So, remove the prefix,
        // lowercase the first letter, and remove the trailing "Context" part of
        // the name. Saves big time on output!
        if (tree is UnvParseTreeText t)
        {
            StartLine(sb, level);
            sb.Append(
                "( "
                + " text:'" + PerformEscapes(t.Data) + "'"
                + " tt:" + t.TokenType
                + " chnl:" + lexer.ChannelNames[t.Channel]
                //+ " l:" + t.Line
                //+ " c:" + t.Column
                //+ " si:" + t.StartIndex
                //+ " ei:" + t.StopIndex
                //+ " ti:" + t.TokenIndex
                );
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeAttr a)
        {
            StartLine(sb, level);
            sb.Append("( Attribute " + a.Name as string);
            sb.Append(" Value '");
            sb.Append(PerformEscapes(a.StringValue));
            sb.Append("'");
            if (a.Channel >= 0 && a.Channel < lexer.ChannelNames.Length)
            {
                sb.Append(" chnl:");
                sb.Append(lexer.ChannelNames[a.Channel].ToString());
            }
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeElement e)
        {
            var x = e;
            var name = e.LocalName;
            StartLine(sb, level);
            sb.Append(
                "( " + name
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

    public static StringBuilder OutputTreeAntlrStyle(UnvParseTreeNode tree, Lexer lexer, Parser parser)
    {
        changed = 0;
        first_time = true;
        var sb = new StringBuilder();
        AntlrParenthesizedAST(tree, sb, lexer, parser);
        return sb;
    }

    private static void AntlrParenthesizedAST(UnvParseTreeNode tree, StringBuilder sb, Lexer lexer, Parser parser, int level = 0)
    {
        if (tree is UnvParseTreeText t)
        {
            sb.Append((!first_time ? " ": "") + "\"" + PerformEscapes(t.Data) + "\"");
            return;
        }
        else if (tree is UnvParseTreeAttr a)
        {
            return;
        }
        else if (tree is UnvParseTreeElement e)
        {
            var x = e;
            var name = e.LocalName;
            sb.Append((!first_time ? " " : "") + "(" + name);
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            AntlrParenthesizedAST(c as UnvParseTreeNode, sb, lexer, parser, level + 1);
        }
        sb.Append(")");
    }

    public static StringBuilder OutputTreeIndentStyle(UnvParseTreeNode tree, Lexer lexer, Parser parser)
    {
        changed = 0;
        first_time = true;
        var sb = new StringBuilder();
        IndentAST(tree, sb, lexer, parser);
        return sb;
    }

    public static void IndentAST(UnvParseTreeNode tree, StringBuilder sb, Lexer lexer, Parser parser, int level = 0)
    {
        if (tree is UnvParseTreeText t)
        {
            IndentStartLine(sb, level);
			sb.Append("\"" + PerformEscapes(t.Data) + "\"");
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeAttr a)
        {
            IndentStartLine(sb, level);
            sb.Append("Attribute " + a.Name as string);
            sb.Append(" Value '");
            sb.Append(PerformEscapes(a.StringValue));
            sb.Append("'");
            if (a.Channel >= 0 && a.Channel < lexer.ChannelNames.Length)
            {
                sb.Append(" chnl:");
                sb.Append(lexer.ChannelNames[a.Channel].ToString());
            }
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeElement e)
        {
            var x = e;
            var name = e.LocalName;
            IndentStartLine(sb, level);
            sb.Append(name);
            sb.AppendLine();
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            IndentAST(c as UnvParseTreeNode, sb, lexer, parser, level + 1);
        }
        if (level == 0)
        {
//            sb.AppendLine();
            changed = 0;
        }
    }

    private static void IndentStartLine(StringBuilder sb, int level = 0)
    {
        if (changed - level >= 0)
        {
            changed = 0;
            first_time = false;
        }
        changed = level;
        for (int j = 0; j < level; ++j) sb.Append(" ");
    }

}
