using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace Trash;

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
            var trees = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            if (config.Verbose)
            {
                foreach (var n in trees)
                    System.Console.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
            }

            AnalyzeDoc();
        }
    }


    public void AnalyzeDoc()
    {
        // Find p : a q* r b where q =>* r or
        //      p : a q+ r b where q =>* r



        // Find non-terminals p that:
        //  FIRST(p) contains empty;
        //  FIRST(p) and FOLLOW(p) is not an empty set.


    }
}

public class Grammar
{
    public List<string> NonTerminals { get; set; }
    public List<string> Terminals { get; set; }
    public List<Production> Productions { get; set; }
    public string StartSymbol { get; set; }

    public Grammar(List<string> nonTerminals, List<string> terminals, List<Production> productions, string startSymbol)
    {
        NonTerminals = nonTerminals;
        Terminals = terminals;
        Productions = productions;
        StartSymbol = startSymbol;
    }
}

public class Production
{
    public string LeftHandSide { get; set; }
    public List<string> RightHandSide { get; set; }

    public Production(string leftHandSide, List<string> rightHandSide)
    {
        LeftHandSide = leftHandSide;
        RightHandSide = rightHandSide;
    }
}

public class LR0Item
{
    public Production Production { get; set; }
    public int DotPosition { get; set; }

    public LR0Item(Production production, int dotPosition)
    {
        Production = production;
        DotPosition = dotPosition;
    }

    public override string ToString()
    {
        var rhs = Production.RightHandSide.ToList();
        rhs.Insert(DotPosition, ".");
        return $"{Production.LeftHandSide} -> {string.Join(" ", rhs)}";
    }
}

public class LR0Parser
{
    private Grammar _grammar;
    private Dictionary<string, HashSet<string>> _firstSets;
    private Dictionary<string, HashSet<string>> _followSets;

    public LR0Parser(Grammar grammar)
    {
        _grammar = grammar;
        _firstSets = new Dictionary<string, HashSet<string>>();
        _followSets = new Dictionary<string, HashSet<string>>();
        ComputeFirstSets();
        ComputeFollowSets();
    }

    public List<HashSet<LR0Item>> ComputeLR0Items()
    {
        var states = new List<HashSet<LR0Item>>();

        var startItem = new LR0Item(new Production("S'", new List<string> { _grammar.StartSymbol }), 0);
        var startState = Closure(new HashSet<LR0Item> { startItem });
        states.Add(startState);

        var unmarkedStates = new Queue<HashSet<LR0Item>>();
        unmarkedStates.Enqueue(startState);

        while (unmarkedStates.Any())
        {
            var currentState = unmarkedStates.Dequeue();

            foreach (var symbol in _grammar.NonTerminals.Concat(_grammar.Terminals))
            {
                var gotoState = Goto(currentState, symbol);
                if (gotoState.Any() && !states.Any(s => s.SetEquals(gotoState)))
                {
                    states.Add(gotoState);
                    unmarkedStates.Enqueue(gotoState);
                }
            }
        }

        return states;
    }

    private HashSet<LR0Item> Closure(HashSet<LR0Item> items)
    {
        var closure = new HashSet<LR0Item>(items);

        bool added;
        do
        {
            added = false;
            var newItems = new HashSet<LR0Item>(closure);

            foreach (var item in closure)
            {
                if (item.DotPosition < item.Production.RightHandSide.Count)
                {
                    var symbolAfterDot = item.Production.RightHandSide[item.DotPosition];
                    if (_grammar.NonTerminals.Contains(symbolAfterDot))
                    {
                        foreach (var production in _grammar.Productions.Where(p => p.LeftHandSide == symbolAfterDot))
                        {
                            var newItem = new LR0Item(production, 0);
                            if (newItems.Add(newItem))
                            {
                                added = true;
                            }
                        }
                    }
                }
            }

            closure = newItems;
        } while (added);

        return closure;
    }

    private HashSet<LR0Item> Goto(HashSet<LR0Item> items, string symbol)
    {
        var gotoItems = new HashSet<LR0Item>();

        foreach (var item in items)
        {
            if (item.DotPosition < item.Production.RightHandSide.Count && item.Production.RightHandSide[item.DotPosition] == symbol)
            {
                gotoItems.Add(new LR0Item(item.Production, item.DotPosition + 1));
            }
        }

        return Closure(gotoItems);
    }

    private void ComputeFirstSets()
    {
        foreach (var nonTerminal in _grammar.NonTerminals)
        {
            _firstSets[nonTerminal] = ComputeFirst(nonTerminal);
        }
    }

    private HashSet<string> ComputeFirst(string symbol)
    {
        var firstSet = new HashSet<string>();

        if (_grammar.Terminals.Contains(symbol))
        {
            firstSet.Add(symbol);
            return firstSet;
        }

        foreach (var production in _grammar.Productions.Where(p => p.LeftHandSide == symbol))
        {
            foreach (var s in production.RightHandSide)
            {
                var firstOfS = ComputeFirst(s);
                firstSet.UnionWith(firstOfS);

                if (!firstOfS.Contains("ε"))
                {
                    break;
                }
            }
        }

        return firstSet;
    }

    private void ComputeFollowSets()
    {
        foreach (var nonTerminal in _grammar.NonTerminals)
        {
            _followSets[nonTerminal] = new HashSet<string>();
        }

        _followSets[_grammar.StartSymbol].Add("$");  // Add end-of-input marker to the start symbol's FOLLOW set

        bool added;
        do
        {
            added = false;

            foreach (var production in _grammar.Productions)
            {
                for (int i = 0; i < production.RightHandSide.Count; i++)
                {
                    var symbol = production.RightHandSide[i];
                    if (_grammar.NonTerminals.Contains(symbol))
                    {
                        var nextSymbols = production.RightHandSide.Skip(i + 1).ToList();

                        var firstOfNext = new HashSet<string>();

                        if (nextSymbols.Any())
                        {
                            firstOfNext = ComputeFirst(string.Join("", nextSymbols));
                            if (firstOfNext.Contains("ε"))
                            {
                                firstOfNext.Remove("ε");
                                firstOfNext.UnionWith(_followSets[production.LeftHandSide]);
                            }
                        }
                        else
                        {
                            firstOfNext.UnionWith(_followSets[production.LeftHandSide]);
                        }

                        _followSets[symbol].UnionWith(firstOfNext);

                        {
                            added = true;
                        }
                    }
                }
            }
        } while (added);
    }

    public HashSet<string> GetFirstSet(string nonTerminal)
    {
        return _firstSets.ContainsKey(nonTerminal) ? _firstSets[nonTerminal] : new HashSet<string>();
    }

    public HashSet<string> GetFollowSet(string nonTerminal)
    {
        return _followSets.ContainsKey(nonTerminal) ? _followSets[nonTerminal] : new HashSet<string>();
    }
}

public class P
{
    public static void foo()
    {
        var nonTerminals = new List<string> { "S", "A" };
        var terminals = new List<string> { "a", "b" };
        var productions = new List<Production>
        {
            new Production("S", new List<string> { "A", "b" }),
            new Production("A", new List<string> { "a" })
        };
        var startSymbol = "S";

        var grammar = new Grammar(nonTerminals, terminals, productions, startSymbol);
        var parser = new LR0Parser(grammar);

        // Compute LR(0) Items
        var lr0Items = parser.ComputeLR0Items();

        for (int i = 0; i < lr0Items.Count; i++)
        {
            Console.WriteLine($"State {i}:");
            foreach (var item in lr0Items[i])
            {
                Console.WriteLine($"  {item}");
            }
        }

        // Compute and display FIRST and FOLLOW sets
        Console.WriteLine("\nFIRST and FOLLOW sets:");
        foreach (var nonTerminal in nonTerminals)
        {
            var firstSet = parser.GetFirstSet(nonTerminal);
            var followSet = parser.GetFollowSet(nonTerminal);

            Console.WriteLine($"FIRST({nonTerminal}) = {{ {string.Join(", ", firstSet)} }}");
            Console.WriteLine($"FOLLOW({nonTerminal}) = {{ {string.Join(", ", followSet)} }}");
        }
    }
}

