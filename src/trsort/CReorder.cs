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

    // Sort reachable parser rules in BFS order starting from the named start rule.
    // Pass null to auto-detect the start rule via the EOF-alternative heuristic.
    // Rules unreachable from the start rule are dropped from the grammar.
    public static void BfsSort(UnvParseTreeNode[] trees, Parser parser, string startRule)
        => ReachabilitySort(trees, parser, startRule, bfs: true);

    // Sort reachable parser rules in DFS preorder starting from the named start rule.
    // Pass null to auto-detect the start rule via the EOF-alternative heuristic.
    // Rules unreachable from the start rule are dropped from the grammar.
    public static void DfsSort(UnvParseTreeNode[] trees, Parser parser, string startRule)
        => ReachabilitySort(trees, parser, startRule, bfs: false);

    static void ReachabilitySort(UnvParseTreeNode[] trees, Parser parser, string startRule, bool bfs)
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

        // Resolve the set of start rule names.
        List<string> startNames;
        if (string.IsNullOrEmpty(startRule))
        {
            // Auto-detect: find parser rule(s) in non-lexer grammars whose alternatives
            // contain an EOF token.  The first descendant RULE_REF of each matched
            // parserRuleSpec is its rule name.
            const string autoXPath =
                "//grammarSpec/grammarDecl[not(grammarType/LEXER)]" +
                "//parserRuleSpec[.//alternative/element[.//TOKEN_REF/text()=\"EOF\"]]";

            var detected = _engine.parseExpression(autoXPath, new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                .Select(x => x.NativeValue as UnvParseTreeElement)
                .ToList();

            if (detected.Count == 0)
            {
                System.Console.Error.WriteLine(
                    "trsort: cannot auto-detect start rule: " +
                    "no parser rule with an EOF alternative found. " +
                    "Provide an XPath expression explicitly.");
                throw new System.Exception("Auto-detect: no start rule found.");
            }
            if (detected.Count > 1)
            {
                System.Console.Error.WriteLine(
                    "trsort: cannot auto-detect start rule: " +
                    "multiple parser rules contain an EOF alternative:");
                foreach (var d in detected)
                {
                    var n = _engine.parseExpression("./RULE_REF/text()", new StaticContextBuilder())
                        .evaluate(dynamicContext, new object[] { d })
                        .Select(x => x.NativeValue as UnvParseTreeText)
                        .FirstOrDefault()?.NodeValue as string;
                    System.Console.Error.WriteLine("  " + n);
                }
                System.Console.Error.WriteLine(
                    "Provide an XPath expression explicitly to choose a start rule.");
                throw new System.Exception("Auto-detect: multiple start rules found.");
            }

            var detectedName = _engine.parseExpression("./RULE_REF/text()", new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { detected[0] })
                .Select(x => x.NativeValue as UnvParseTreeText)
                .First().NodeValue as string;
            startNames = new List<string> { detectedName };
        }
        else
        {
            // The caller supplied a rule name directly.  Look it up in the grammar.
            if (!nameToNode.ContainsKey(startRule))
            {
                System.Console.Error.WriteLine(
                    $"trsort: start rule '{startRule}' not found in grammar.");
                throw new System.Exception($"Start rule '{startRule}' not found.");
            }
            startNames = new List<string> { startRule };
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
