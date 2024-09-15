using System;
using Antlr4.Runtime;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace tranalyze;

class Command
{
    public string Help()
    {
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("tranalyze.readme.md"))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public void Execute(Config config)
    {
        string lines = null;
        if (!(config.File != null && config.File != ""))
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from stdin");
            }

            for (;;)
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }

            lines = lines.Trim();
        }
        else
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
            }

            lines = File.ReadAllText(config.File);
        }

        var serializeOptions = new JsonSerializerOptions();
        serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
        serializeOptions.WriteIndented = config.Format;
        serializeOptions.MaxDepth = 10000;
        AntlrJson.ParsingResultSet[] data =
            JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
        foreach (AntlrJson.ParsingResultSet parse_info in data)
        {
            if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
            UnvParseTreeNode[] trees = parse_info.Nodes;
            foreach (var t in trees) t.Validate(true);
            Antlr4.Runtime.Parser parser = parse_info.Parser;
            Lexer lexer = parse_info.Lexer;
            if (config.Verbose)
            {
                foreach (var n in trees)
                    System.Console.WriteLine(new TreeOutput(lexer, parser).OutputTree(n).ToString());
            }
            AnalyzeDoc(trees);
        }
    }

    public void AnalyzeDoc(UnvParseTreeNode[] trees)
    {
        for (int t = 0; t < trees.Length; t++)
        {
            var g = new Grammar(trees[t]);
            
            // Find p : a q* r b where q =>* r or
            //      p : a q+ r b where q =>* r

            // Find non-terminals p that:
            //  FIRST(p) contains empty;
            //  FIRST(p) and FOLLOW(p) is not an empty set.

        }
    }
}

public class Grammar
{
    public SortedDictionary<string, UnvParseTreeNode> NonTerminals { get; set; }
    public SortedDictionary<string, UnvParseTreeNode> Terminals { get; set; }
    public List<UnvParseTreeNode> Productions { get; set; }
    public string StartSymbol { get; set; }
    public UnvParseTreeNode StartProduction { get; set; }

    public Grammar(UnvParseTreeNode root)
    {
        NonTerminals = new SortedDictionary<string, UnvParseTreeNode>();
        Terminals = new SortedDictionary<string, UnvParseTreeNode>();
        Productions = new List<UnvParseTreeNode>();
        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
        var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
        using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dynamicContext = ate.Try(root))
        {
            // Initialize to do list.
            var prods = engine.parseExpression(
                    "//(parserRuleSpec | lexerRuleSpec)", new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
            foreach (var prod in prods)
            {
                Productions.Add(prod);
                var nt = engine.parseExpression(
                        "./RULE_REF", new StaticContextBuilder())
                    .evaluate(dynamicContext, new object[] { prod })
                    .ToList();
                foreach (var n in nt)
                {
                    NonTerminals.Add(n.StringValue, n.NativeValue as UnvParseTreeNode);
                }
                var t = engine.parseExpression(
                        "./TOKEN_REF", new StaticContextBuilder())
                    .evaluate(dynamicContext, new object[] { prod })
                    .ToList();
                foreach (var n in t)
                {
                    Terminals.Add(n.StringValue, n.NativeValue as UnvParseTreeNode);
                }
                if (t.Count > 0) continue;
                var rhs_start = engine.parseExpression(
                        "./ruleBlock", new StaticContextBuilder())
                    .evaluate(dynamicContext, new object[] { prod })
                    .Select(x => x.NativeValue as UnvParseTreeElement)
                    .FirstOrDefault();
                string name = null;
                prod.Validate(true);
                using (ParseTreeEditing.UnvParseTreeDOM.AntlrDynamicContext dc2 = ate.Try(prod))
                {
                    // rewrite prod to replace whitespaces with a single space.
                    var bc = engine.parseExpression(
                            "//@BLOCK_COMMENT", new StaticContextBuilder())
                        .evaluate(dc2, new object[] { dc2.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .ToList();
                    TreeEdits.Delete(bc);
                    var lc = engine.parseExpression(
                            "//@LINE_COMMENT", new StaticContextBuilder())
                        .evaluate(dc2, new object[] { dc2.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .ToList();
                    TreeEdits.Delete(lc);
                    var dc = engine.parseExpression(
                            "//@DOC_COMMENT", new StaticContextBuilder())
                        .evaluate(dc2, new object[] { dc2.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .ToList();
                    TreeEdits.Delete(dc);
                    prod.Validate(true);
                    var ws = engine.parseExpression(
                            "//@WS[position()>1]", new StaticContextBuilder())
                        .evaluate(dc2, new object[] { dc2.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .ToList();
                    TreeEdits.Delete(ws);
                    var ws2 = engine.parseExpression(
                            "//@WS", new StaticContextBuilder())
                        .evaluate(dc2, new object[] { dc2.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeNode))
                        .ToList();
                    prod.Validate(true);
                    foreach (var w in ws2) w.Validate(true);
                    TreeEdits.Replace(ws2, " ");
                }
                //LR0Item startItem = new LR0Item(prod, rhs_start);
                //System.Console.WriteLine(startItem);
            }
            var start = engine.parseExpression(
                    "//grammarSpec/grammarDecl[not(grammarType/LEXER)]//parserRuleSpec//alternative/element[.//TOKEN_REF/text()=\"EOF\"]/following-sibling::element",
                    new StaticContextBuilder())
                .evaluate(dynamicContext, new object[] { dynamicContext.Document })
                .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
            if (start.Count == 0 || start.Count > 1)
            {
                throw new Exception("Grammar must have exactly one start rule.");
            }
            StartProduction = start[0];
        }
    }
}

public class LR0Item
{
    public UnvParseTreeNode DotPosition { get; set; }
    public UnvParseTreeNode Production { get; set; }
    public LR0Item(UnvParseTreeNode production, UnvParseTreeNode dotPosition)
    {
        // "production" must be a parserRuleSpec
        // and dotPosition must be a node in the right hand side of the production.
        Production = production;
        production.Validate(true);
        DotPosition = dotPosition;
    }

    public override string ToString()
    {
        return TreeEdits.ReconstructItem(this.Production, this.DotPosition);
    }
}

//public class LR0Parser
//{
//    private Grammar _grammar;
//    private Dictionary<string, HashSet<string>> _firstSets;
//    private Dictionary<string, HashSet<string>> _followSets;

//    public LR0Parser(Grammar grammar)
//    {
//        _grammar = grammar;
//        _firstSets = new Dictionary<string, HashSet<string>>();
//        _followSets = new Dictionary<string, HashSet<string>>();
//        ComputeFirstSets();
//        ComputeFollowSets();
//    }

//    public List<HashSet<LR0Item>> ComputeLR0Items()
//    {
//        var states = new List<HashSet<LR0Item>>();
//        // Find start rule.

//        var startItem = new LR0Item(new Production("S'", new List<string> { _grammar.StartSymbol }), 0);
//        var startState = Closure(new HashSet<LR0Item> { startItem });
//        states.Add(startState);

//        var unmarkedStates = new Queue<HashSet<LR0Item>>();
//        unmarkedStates.Enqueue(startState);

//        while (unmarkedStates.Any())
//        {
//            var currentState = unmarkedStates.Dequeue();

//            foreach (var symbol in _grammar.NonTerminals.Concat(_grammar.Terminals))
//            {
//                var gotoState = Goto(currentState, symbol);
//                if (gotoState.Any() && !states.Any(s => s.SetEquals(gotoState)))
//                {
//                    states.Add(gotoState);
//                    unmarkedStates.Enqueue(gotoState);
//                }
//            }
//        }

//        return states;
//    }

//    private HashSet<LR0Item> Closure(HashSet<LR0Item> items)
//    {
//        var closure = new HashSet<LR0Item>(items);

//        bool added;
//        do
//        {
//            added = false;
//            var newItems = new HashSet<LR0Item>(closure);

//            foreach (var item in closure)
//            {
//                if (item.DotPosition < item.Production.RightHandSide.Count)
//                {
//                    var symbolAfterDot = item.Production.RightHandSide[item.DotPosition];
//                    if (_grammar.NonTerminals.Contains(symbolAfterDot))
//                    {
//                        foreach (var production in
//                                 _grammar.Productions.Where(p => p.LeftHandSide == symbolAfterDot))
//                        {
//                            var newItem = new LR0Item(production, 0);
//                            if (newItems.Add(newItem))
//                            {
//                                added = true;
//                            }
//                        }
//                    }
//                }
//            }

//            closure = newItems;
//        } while (added);

//        return closure;
//    }

//    private HashSet<LR0Item> Goto(HashSet<LR0Item> items, string symbol)
//    {
//        var gotoItems = new HashSet<LR0Item>();

//        foreach (var item in items)
//        {
//            if (item.DotPosition < item.Production.RightHandSide.Count &&
//                item.Production.RightHandSide[item.DotPosition] == symbol)
//            {
//                gotoItems.Add(new LR0Item(item.Production, item.DotPosition + 1));
//            }
//        }

//        return Closure(gotoItems);
//    }

//    private void ComputeFirstSets()
//    {
//        foreach (var nonTerminal in _grammar.NonTerminals)
//        {
//            _firstSets[nonTerminal] = ComputeFirst(nonTerminal);
//        }
//    }

//    private HashSet<string> ComputeFirst(string symbol)
//    {
//        var firstSet = new HashSet<string>();

//        if (_grammar.Terminals.Contains(symbol))
//        {
//            firstSet.Add(symbol);
//            return firstSet;
//        }

//        foreach (var production in _grammar.Productions.Where(p => p.LeftHandSide == symbol))
//        {
//            foreach (var s in production.RightHandSide)
//            {
//                var firstOfS = ComputeFirst(s);
//                firstSet.UnionWith(firstOfS);

//                if (!firstOfS.Contains("ε"))
//                {
//                    break;
//                }
//            }
//        }

//        return firstSet;
//    }

//    private void ComputeFollowSets()
//    {
//        foreach (var nonTerminal in _grammar.NonTerminals)
//        {
//            _followSets[nonTerminal] = new HashSet<string>();
//        }

//        _followSets[_grammar.StartSymbol].Add("$"); // Add end-of-input marker to the start symbol's FOLLOW set

//        bool added;
//        do
//        {
//            added = false;

//            foreach (var production in _grammar.Productions)
//            {
//                for (int i = 0; i < production.RightHandSide.Count; i++)
//                {
//                    var symbol = production.RightHandSide[i];
//                    if (_grammar.NonTerminals.Contains(symbol))
//                    {
//                        var nextSymbols = production.RightHandSide.Skip(i + 1).ToList();

//                        var firstOfNext = new HashSet<string>();

//                        if (nextSymbols.Any())
//                        {
//                            firstOfNext = ComputeFirst(string.Join("", nextSymbols));
//                            if (firstOfNext.Contains("ε"))
//                            {
//                                firstOfNext.Remove("ε");
//                                firstOfNext.UnionWith(_followSets[production.LeftHandSide]);
//                            }
//                        }
//                        else
//                        {
//                            firstOfNext.UnionWith(_followSets[production.LeftHandSide]);
//                        }

//                        _followSets[symbol].UnionWith(firstOfNext);

//                        {
//                            added = true;
//                        }
//                    }
//                }
//            }
//        } while (added);
//    }

//    public HashSet<string> GetFirstSet(string nonTerminal)
//    {
//        return _firstSets.ContainsKey(nonTerminal) ? _firstSets[nonTerminal] : new HashSet<string>();
//    }

//    public HashSet<string> GetFollowSet(string nonTerminal)
//    {
//        return _followSets.ContainsKey(nonTerminal) ? _followSets[nonTerminal] : new HashSet<string>();
//    }
//}