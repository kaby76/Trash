namespace LanguageServer
{
    using Antlr4.Runtime.Tree;
    using System.Text;

    internal class Reconstruct
    {
        public static void Doit(StringBuilder sb, IParseTree node)
        {
            if (node is ANTLRv4Parser.ActionBlockContext)
            {
                sb.Append(" { ... }");
            }
            else if (node is TerminalNodeImpl)
            {
                sb.Append(" " + node.GetText());
            }
            else
            {
                for (int i = 0; i < node.ChildCount; ++i)
                {
                    IParseTree c = node.GetChild(i);
                    Doit(sb, c);
                }
            }
        }
    }
}
