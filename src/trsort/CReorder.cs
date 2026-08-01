namespace Trash;

using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using XQuery.Engine;
using XPathParser = XQuery.Parser.XPathParser;

class CReorder
{
    static readonly XPathEvaluator _evaluator = new XPathEvaluator();

    // Sort all parser rules alphabetically by rule name, keeping all rules.
    public static void AlphaSort(UnvParseTreeNode[] trees, Parser parser)
    {
        var adapterDoc = AdapterDocument.Build(trees);

        var ruleSpecExpr = new XPathParser("//ruleSpec[parserRuleSpec]").Parse();
        var ruleAdapters = _evaluator.Evaluate(ruleSpecExpr, adapterDoc)
            .OfType<AdapterElement>().ToList();

        if (ruleAdapters.Count == 0) return;

        var nameExpr = new XPathParser("./parserRuleSpec/RULE_REF/text()").Parse();
        var list = new List<(string name, UnvParseTreeNode node)>();
        foreach (var ruleAdapter in ruleAdapters)
        {
            var name = _evaluator.Evaluate(nameExpr, ruleAdapter)
                .OfType<AdapterText>().First().Source.Data;
            list.Add((name, ruleAdapter.Source));
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
        => ReachabilitySort(trees, startRule, bfs: true);

    // Sort reachable parser rules in DFS preorder starting from the named start rule.
    // Pass null to auto-detect the start rule via the EOF-alternative heuristic.
    // Rules unreachable from the start rule are dropped from the grammar.
    public static void DfsSort(UnvParseTreeNode[] trees, Parser parser, string startRule)
        => ReachabilitySort(trees, startRule, bfs: false);

    static void ReachabilitySort(UnvParseTreeNode[] trees, string startRule, bool bfs)
    {
        var adapterDoc = AdapterDocument.Build(trees);

        // Build map: rule name -> ruleSpec node and AdapterElement.
        var ruleSpecExpr = new XPathParser("//ruleSpec[parserRuleSpec]").Parse();
        var ruleAdapters = _evaluator.Evaluate(ruleSpecExpr, adapterDoc)
            .OfType<AdapterElement>().ToList();

        if (ruleAdapters.Count == 0) return;

        var nameExpr = new XPathParser("./parserRuleSpec/RULE_REF/text()").Parse();
        var nameToNode = new Dictionary<string, UnvParseTreeNode>();
        var nameToAdapter = new Dictionary<string, AdapterElement>();
        foreach (var ruleAdapter in ruleAdapters)
        {
            var name = _evaluator.Evaluate(nameExpr, ruleAdapter)
                .OfType<AdapterText>().First().Source.Data;
            nameToNode[name] = ruleAdapter.Source;
            nameToAdapter[name] = ruleAdapter;
        }

        // Build adjacency: rule name -> list of parser rule names referenced in body.
        var refsExpr = new XPathParser("./parserRuleSpec/ruleBlock//RULE_REF/text()").Parse();
        var adjacency = new Dictionary<string, List<string>>();
        foreach (var kvp in nameToAdapter)
        {
            var refs = _evaluator.Evaluate(refsExpr, kvp.Value)
                .OfType<AdapterText>()
                .Select(t => t.Source.Data)
                .Where(n => nameToNode.ContainsKey(n))
                .Distinct()
                .ToList();
            adjacency[kvp.Key] = refs;
        }

        // Resolve the set of start rule names.
        List<string> startNames;
        if (string.IsNullOrEmpty(startRule))
        {
            // Auto-detect: find parser rule(s) whose rule body contains an EOF token.
            const string autoXPath = "//parserRuleSpec[./ruleBlock//TOKEN_REF/text()='EOF']";
            var autoExpr = new XPathParser(autoXPath).Parse();
            var detected = _evaluator.Evaluate(autoExpr, adapterDoc)
                .OfType<AdapterElement>().ToList();

            if (detected.Count == 0)
            {
                System.Console.Error.WriteLine(
                    "trsort: cannot auto-detect start rule: " +
                    "no parser rule with an EOF alternative found. " +
                    "Provide a start rule name explicitly.");
                throw new System.Exception("Auto-detect: no start rule found.");
            }

            var ruleRefExpr = new XPathParser("./RULE_REF/text()").Parse();
            if (detected.Count > 1)
            {
                System.Console.Error.WriteLine(
                    "trsort: cannot auto-detect start rule: " +
                    "multiple parser rules contain an EOF alternative:");
                foreach (var d in detected)
                {
                    var n = _evaluator.Evaluate(ruleRefExpr, d)
                        .OfType<AdapterText>().FirstOrDefault()?.Source.Data;
                    System.Console.Error.WriteLine("  " + n);
                }
                System.Console.Error.WriteLine(
                    "Provide a start rule name explicitly.");
                throw new System.Exception("Auto-detect: multiple start rules found.");
            }

            var detectedName = _evaluator.Evaluate(ruleRefExpr, detected[0])
                .OfType<AdapterText>().First().Source.Data;
            startNames = new List<string> { detectedName };
        }
        else
        {
            // The caller supplied a rule name directly. Look it up in the grammar.
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
        var rulesContainer = ruleAdapters[0].Source.ParentNode as UnvParseTreeNode;

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
