namespace Trash;

using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

class CReorder
{
    static readonly org.eclipse.wst.xml.xpath2.processor.Engine _engine =
        new org.eclipse.wst.xml.xpath2.processor.Engine();

    // Sort all parser rules alphabetically by rule name, keeping all rules.
    public static void AlphaSort(UnvParseTreeNode[] trees, Parser parser)
    {
        var ate = new ConvertToDOM();
        using var dynamicContext = ate.Try(trees, parser);

        var ruleNodes = _engine.parseExpression("//ruleSpec[parserRuleSpec]",
                new StaticContextBuilder())
            .evaluate(dynamicContext, new object[] { dynamicContext.Document })
            .Select(x => x.NativeValue as UnvParseTreeElement)
            .ToList();

        if (ruleNodes.Count == 0) return;

        var list = new List<(string name, UnvParseTreeNode node)>();
        foreach (var node in ruleNodes)
        {
            var name = _engine.parseExpression("./parserRuleSpec/RULE_REF/text()",
                    new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { node })
                .Select(x => x.NativeValue as UnvParseTreeText)
                .First().NodeValue as string;
            list.Add((name, node));
        }

        // Sort descending so that inserting each at position 0 yields ascending order.
        list.Sort((x, y) => string.Compare(y.name, x.name, System.StringComparison.Ordinal));

        var rulesContainer = list[0].node.ParentNode as UnvParseTreeNode;
        foreach (var (_, node) in list)
            TreeEdits.MoveToFirstChild(node, rulesContainer);
    }

    // Sort reachable parser rules in BFS order starting from rules matched by startExpr.
    // Rules unreachable from the start set are dropped from the grammar.
    public static void BfsSort(UnvParseTreeNode[] trees, Parser parser, string startExpr)
        => ReachabilitySort(trees, parser, startExpr, bfs: true);

    // Sort reachable parser rules in DFS preorder starting from rules matched by startExpr.
    // Rules unreachable from the start set are dropped from the grammar.
    public static void DfsSort(UnvParseTreeNode[] trees, Parser parser, string startExpr)
        => ReachabilitySort(trees, parser, startExpr, bfs: false);

    static void ReachabilitySort(UnvParseTreeNode[] trees, Parser parser, string startExpr, bool bfs)
    {
        var ate = new ConvertToDOM();
        using var dynamicContext = ate.Try(trees, parser);

        // Build map: rule name -> ruleSpec node.
        var ruleNodes = _engine.parseExpression("//ruleSpec[parserRuleSpec]",
                new StaticContextBuilder())
            .evaluate(dynamicContext, new object[] { dynamicContext.Document })
            .Select(x => x.NativeValue as UnvParseTreeElement)
            .ToList();

        if (ruleNodes.Count == 0) return;

        var nameToNode = new Dictionary<string, UnvParseTreeNode>();
        foreach (var node in ruleNodes)
        {
            var name = _engine.parseExpression("./parserRuleSpec/RULE_REF/text()",
                    new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { node })
                .Select(x => x.NativeValue as UnvParseTreeText)
                .First().NodeValue as string;
            nameToNode[name] = node;
        }

        // Build adjacency: rule name -> list of parser rule names referenced in body.
        var adjacency = new Dictionary<string, List<string>>();
        foreach (var kvp in nameToNode)
        {
            var refs = _engine.parseExpression("./parserRuleSpec/ruleBlock//RULE_REF/text()",
                    new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { kvp.Value })
                .Select(x => x.NativeValue as UnvParseTreeText)
                .Select(t => t.NodeValue as string)
                .Where(n => nameToNode.ContainsKey(n))
                .Distinct()
                .ToList();
            adjacency[kvp.Key] = refs;
        }

        // Evaluate the user-supplied XPath to find start rules.
        var startItems = _engine.parseExpression(startExpr,
                new StaticContextBuilder())
            .evaluate(dynamicContext, new object[] { dynamicContext.Document })
            .ToList();

        var startNames = new List<string>();
        foreach (var item in startItems)
        {
            string name = null;
            var native = item.NativeValue;
            if (native is UnvParseTreeText txt)
            {
                name = txt.NodeValue as string;
            }
            else if (native is UnvParseTreeElement elem)
            {
                if (elem.LocalName == "RULE_REF")
                {
                    // XPath selected the RULE_REF token element itself.
                    name = elem.GetChildrenText().FirstOrDefault();
                }
                else
                {
                    // For parserRuleSpec, ruleSpec, or any other container: the first
                    // descendant RULE_REF in document order is the rule name.
                    name = _engine.parseExpression(".//RULE_REF/text()",
                            new StaticContextBuilder())
                        .evaluate(dynamicContext, new object[] { elem })
                        .Select(x => x.NativeValue as UnvParseTreeText)
                        .FirstOrDefault()?.NodeValue as string;
                }
            }
            if (name != null && nameToNode.ContainsKey(name))
                startNames.Add(name);
        }

        if (startNames.Count == 0) return;

        // Traverse graph to get ordered reachable rule names.
        var ordered = bfs ? BfsOrder(startNames, adjacency) : DfsOrder(startNames, adjacency);
        if (ordered.Count == 0) return;

        // Capture the rules container before any deletions.
        var rulesContainer = ruleNodes[0].ParentNode as UnvParseTreeNode;

        // Drop rules that are unreachable from the start set.
        var reachable = new HashSet<string>(ordered);
        foreach (var kvp in nameToNode.ToList())
        {
            if (!reachable.Contains(kvp.Key))
                TreeEdits.Delete(kvp.Value);
        }

        // Reorder remaining rules: insert in reverse order so position-0 insertions
        // produce the desired final order.
        for (int i = ordered.Count - 1; i >= 0; i--)
            TreeEdits.MoveToFirstChild(nameToNode[ordered[i]], rulesContainer);
    }

    static List<string> BfsOrder(List<string> startNames, Dictionary<string, List<string>> adjacency)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        var result = new List<string>();

        foreach (var name in startNames)
        {
            if (adjacency.ContainsKey(name) && visited.Add(name))
                queue.Enqueue(name);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);
            if (adjacency.TryGetValue(current, out var refs))
            {
                foreach (var next in refs)
                {
                    if (visited.Add(next))
                        queue.Enqueue(next);
                }
            }
        }

        return result;
    }

    static List<string> DfsOrder(List<string> startNames, Dictionary<string, List<string>> adjacency)
    {
        var visited = new HashSet<string>();
        var result = new List<string>();

        void Dfs(string name)
        {
            if (!visited.Add(name)) return;
            result.Add(name);
            if (adjacency.TryGetValue(name, out var refs))
            {
                foreach (var next in refs)
                    Dfs(next);
            }
        }

        foreach (var name in startNames)
            Dfs(name);

        return result;
    }
}
