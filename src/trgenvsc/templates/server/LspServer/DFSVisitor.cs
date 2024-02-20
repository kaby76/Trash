using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;

namespace Server
{

    internal class DFSVisitor
    {
        public static IEnumerable<UnvParseTreeElement> DFS(UnvParseTreeElement root)
        {
            if (root == null) yield break;
            Stack<UnvParseTreeElement> toVisit = new Stack<UnvParseTreeElement>();
            Stack<UnvParseTreeElement> visitedAncestors = new Stack<UnvParseTreeElement>();
            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                UnvParseTreeElement node = toVisit.Peek();
                if (node.ChildNodes.Length > 0)
                {
                    if (visitedAncestors.PeekOrDefault() != node)
                    {
                        visitedAncestors.Push(node);

                        int child_count = node.ChildNodes.Length;
                        for (int i = child_count - 1; i >= 0; --i)
                        {
                            Node o = node.ChildNodes.item(i);
                            var n = o as UnvParseTreeElement;
                            if (n == null) continue;
                            toVisit.Push(n);
                        }
                        continue;
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
        public static UnvParseTreeElement PeekOrDefault(this Stack<UnvParseTreeElement> s)
        {
            return s.Count == 0 ? null : s.Peek();
        }
    }
}
