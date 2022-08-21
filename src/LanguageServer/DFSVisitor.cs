namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;

    public class DFSVisitor
    {
        public static IEnumerable<IParseTree> DFS(IParseTree root)
        {
            if (root == null) yield break;
            Stack<IParseTree> toVisit = new Stack<IParseTree>();
            Stack<IParseTree> visitedAncestors = new Stack<IParseTree>();
            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                IParseTree node = toVisit.Peek();
                if (node.ChildCount > 0)
                {
                    if (visitedAncestors.PeekOrDefault() != node)
                    {
                        visitedAncestors.Push(node);

                        if (node as TerminalNodeImpl != null)
                        {
                        }
                        else
                        {
                            ParserRuleContext internal_node = node as ParserRuleContext;
                            int child_count = internal_node.children.Count;
                            for (int i = child_count - 1; i >= 0; --i)
                            {
                                IParseTree o = internal_node.children[i];
                                toVisit.Push(o);
                            }
                            continue;
                        }
                    }
                    visitedAncestors.Pop();
                }
                yield return node;
                toVisit.Pop();
            }
        }
    }

    internal static class StackHelper
    {
        public static IParseTree PeekOrDefault(this Stack<IParseTree> s)
        {
            return s.Count == 0 ? null : s.Peek();
        }
    }
}
