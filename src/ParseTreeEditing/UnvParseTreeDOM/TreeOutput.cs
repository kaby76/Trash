using Antlr4.Runtime;
using System;
using System.IO;
using System.Text;

namespace ParseTreeEditing.UnvParseTreeDOM;

public class TreeOutput
{
    private int changed = 0;
    private bool first_time = true;
    private StringBuilder sb;
    private Lexer lexer;
    private Parser parser;
    private string prefix;

    public TreeOutput(Lexer lexer, Parser parser, string prefix = "")
    {
        changed = 0;
        first_time = true;
        sb = new StringBuilder();
        this.lexer = lexer;
        this.parser = parser;
        this.prefix = prefix;
    }

    public StringBuilder OutputTree(UnvParseTreeNode tree)
    {
        ParenthesizedAST(tree);
        return sb;
    }

    private void ParenthesizedAST(UnvParseTreeNode tree, int level = 0)
    {
        // Antlr always names a non-terminal with first letter lowercase,
        // but renames it when creating the type in C#. So, remove the prefix,
        // lowercase the first letter, and remove the trailing "Context" part of
        // the name. Saves big time on output!
        if (tree is UnvParseTreeText t)
        {
            StartLine(level);
            sb.Append(
                "( "
                + " text:'" + PerformEscapes(t.Data) + "'"
                + " tt:" + t.TokenType
                + " chnl:" + GetChannelName(t.Channel)
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
            StartLine(level);
            sb.Append("( Attribute " + a.Name as string);
            sb.Append(" Value '");
            sb.Append(PerformEscapes(a.StringValue));
            sb.Append("'");
            if (a.Channel >= 0)
            {
                sb.Append(" chnl:");
                sb.Append(GetChannelName(a.Channel));
            }
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeElement e)
        {
            var x = e;
            var name = e.LocalName;
            StartLine(level);
            sb.Append(
                "( " + name
                );
            sb.AppendLine();
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            ParenthesizedAST(c as UnvParseTreeNode, level + 1);
        }
        if (level == 0)
        {
            for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
            sb.AppendLine();
            changed = 0;
        }
    }

    private void StartLine(int level = 0)
    {
        if (changed - level >= 0)
        {
            if (!first_time)
            {
                sb.Append(prefix);
                for (int j = 0; j < level; ++j) sb.Append("  ");
                for (int k = 0; k < 1 + changed - level; ++k) sb.Append(") ");
                sb.AppendLine();
            }
            changed = 0;
            first_time = false;
        }
        changed = level;
        sb.Append(prefix);
        for (int j = 0; j < level; ++j) sb.Append("  ");
    }

    private static string ToLiteral(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (c == '&')
                sb.Append("&amp;");
            else if (c < 0x20 || c == 0x7F)
                sb.Append($"&#{(int)c};");
            else
                sb.Append(c);
        }
        return sb.ToString();
    }

    public static string PerformEscapes(string s)
    {
        StringBuilder new_s = new StringBuilder();
        new_s.Append(ToLiteral(s));
        return new_s.ToString();
    }

    public StringBuilder OutputTreeAntlrStyle(UnvParseTreeNode tree)
    {
        sb.Append(prefix);
        AntlrToStringTree(tree);
        return sb;
    }

    private void AntlrToStringTree(UnvParseTreeNode tree)
    {
        if (tree is not UnvParseTreeElement element)
            return;

        // The DOM represents a terminal as an element named for its token type
        // with a text child. ANTLR's Trees.ToStringTree() prints only the token
        // text, not that wrapper element.
        if (element.RuleIndex < 0)
        {
            var text = "<EOF>";
            if (element.LocalName != "EOF")
            {
                text = "";
                for (var i = 0; i < element.ChildNodes.Length; i++)
                {
                    if (element.ChildNodes.item(i) is UnvParseTreeText child)
                    {
                        text = child.Data;
                        break;
                    }
                }
            }
            sb.Append(EscapeAntlrWhitespace(text));
            return;
        }

        var childCount = 0;
        for (var i = 0; i < element.ChildNodes.Length; i++)
            if (element.ChildNodes.item(i) is UnvParseTreeElement)
                childCount++;

        if (childCount == 0)
        {
            sb.Append(element.LocalName);
            return;
        }

        sb.Append('(').Append(element.LocalName);
        for (var i = 0; i < element.ChildNodes.Length; i++)
        {
            if (element.ChildNodes.item(i) is not UnvParseTreeElement child)
                continue;
            sb.Append(' ');
            AntlrToStringTree(child);
        }
        sb.Append(')');
    }

    private static string EscapeAntlrWhitespace(string text) =>
        text.Replace("\t", "\\t", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal);

    public StringBuilder OutputTreeAntlrStyleWithTokenTypes(UnvParseTreeNode tree)
    {
        sb.Append(prefix);
        AntlrParenthesizedAST(tree);
        return sb;
    }

    private void AntlrParenthesizedAST(UnvParseTreeNode tree, int level = 0)
    {
        if (tree is UnvParseTreeText t)
        {
            sb.Append((!first_time ? " " : "") + "\"" + PerformEscapes(t.Data) + "\"");
            first_time = false;
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
            first_time = false;
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            AntlrParenthesizedAST(c as UnvParseTreeNode, level + 1);
        }
        sb.Append(")");
    }

    public StringBuilder OutputTreeIndentStyle(UnvParseTreeNode tree)
    {
        IndentAST(tree);
        return sb;
    }

    public void IndentAST(UnvParseTreeNode tree, int level = 0)
    {
        if (tree is UnvParseTreeText t)
        {
            IndentStartLine(level);
			sb.Append("\"" + PerformEscapes(t.Data) + "\"");
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeAttr a)
        {
            IndentStartLine(level);
            sb.Append("Attribute " + a.Name as string);
            sb.Append(" Value '");
            sb.Append(PerformEscapes(a.StringValue));
            sb.Append("'");
            if (a.Channel >= 0)
            {
                sb.Append(" chnl:");
                sb.Append(GetChannelName(a.Channel));
            }
            sb.AppendLine();
        }
        else if (tree is UnvParseTreeElement e)
        {
            var x = e;
            var name = e.LocalName;
            IndentStartLine(level);
            sb.Append(name);
            sb.AppendLine();
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            IndentAST(c as UnvParseTreeNode, level + 1);
        }
        if (level == 0)
        {
//            sb.AppendLine();
            changed = 0;
        }
    }

    private void IndentStartLine(int level = 0)
    {
        if (changed - level >= 0)
        {
            changed = 0;
            first_time = false;
        }
        changed = level;
        sb.Append(prefix);
        for (int j = 0; j < level; ++j) sb.Append(" ");
    }

    public StringBuilder OutputTreeBlockStyle(UnvParseTreeNode tree)
    {
        IndentBlockTree(tree, "", true);
        return sb;
    }

    public void IndentBlockTree(UnvParseTreeNode tree, string indent, bool isRoot)
    {
        if (isRoot)
        {
            // Print the current node.
            sb.AppendLine(indent + GetName(tree));
        }
        for (int i = 0; tree.ChildNodes != null && i < tree.ChildNodes.Length; ++i)
        {
            var c = tree.ChildNodes.item(i);
            if (c.ChildNodes == null || c.ChildNodes.Length == 0)
            {
                // "c" is a leaf node.
                bool isLastLeaf = i == tree.ChildNodes.Length - 1;
                sb.AppendLine(indent + (isLastLeaf ? @"└── " : @"├── ") + GetName(c as UnvParseTreeNode));
            }
            else
            {
                bool isLast = i == tree.ChildNodes.Length - 1;
                sb.AppendLine(indent + (isLast ? @"└── " : @"├── ") + GetName(c as UnvParseTreeNode));
                IndentBlockTree(c as UnvParseTreeNode, indent + (isLast ? "    " : "│   "), false);
            }
        }
    }

    string GetName(UnvParseTreeNode tree)
    {
        if (tree is UnvParseTreeText t)
        {
            return "\"" + PerformEscapes(t.Data) + "\"";
        }
        else if (tree is UnvParseTreeAttr a)
        {
            return "Attribute " + (a.Name as string) 
                 + " Value '"
                 + PerformEscapes(a.StringValue)
                 + "'"
                 + (a.Channel >= 0 ? " chnl:" + GetChannelName(a.Channel) : "");
        }
        else if (tree is UnvParseTreeElement e)
        {
            return e.LocalName;
        }
        else return "";
    }

    private string GetChannelName(int channel)
    {
        var channelNames = lexer?.ChannelNames;
        if (channelNames != null
            && channel >= 0
            && channel < channelNames.Length
            && !string.IsNullOrEmpty(channelNames[channel]))
        {
            return channelNames[channel];
        }

        return channel.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
