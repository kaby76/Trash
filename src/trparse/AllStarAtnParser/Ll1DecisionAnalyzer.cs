namespace AllStarAtnParser;

using Atn;
using System.Runtime.CompilerServices;

/// <summary>
/// Conservatively identifies parser decisions whose alternatives are uniquely
/// distinguishable by one token. Nullable and predicate-dependent decisions
/// remain on the normal ALL(*) path.
/// </summary>
internal sealed class Ll1DecisionAnalyzer
{
    private static readonly ConditionalWeakTable<MyATN, Ll1DecisionAnalyzer>
        Cache = new();

    private readonly ushort[][] _alternativeByToken;

    private Ll1DecisionAnalyzer(MyATN atn)
    {
        _alternativeByToken = Analyze(atn);
    }

    public static Ll1DecisionAnalyzer For(MyATN atn) =>
        Cache.GetValue(atn, static value => new Ll1DecisionAnalyzer(value));

    internal ushort[][] Tables => _alternativeByToken;

    public bool TryPredict(int decision, int tokenType, out int alternative)
    {
        alternative = 0;
        if ((uint)decision >= (uint)_alternativeByToken.Length)
            return false;
        var table = _alternativeByToken[decision];
        if (table == null) return false;
        int tokenIndex = tokenType + 1;
        if ((uint)tokenIndex >= (uint)table.Length) return false;
        alternative = table[tokenIndex];
        return alternative != 0;
    }

    public bool IsEligible(int decision) =>
        (uint)decision < (uint)_alternativeByToken.Length &&
        _alternativeByToken[decision] != null;

    private static ushort[][] Analyze(MyATN atn)
    {
        var summaries = new StateSummary[atn.allStates.Length];
        for (int i = 0; i < summaries.Length; i++)
            summaries[i] = new StateSummary();

        var dependents = new List<int>[atn.allStates.Length];
        for (int i = 0; i < dependents.Length; i++)
            dependents[i] = new List<int>();
        foreach (var state in atn.allStates)
        {
            if (state == null) continue;
            foreach (var transition in state.transitions)
            {
                dependents[transition.target.stateNumber].Add(state.stateNumber);
                if (transition is MyRuleTransition rule)
                    dependents[atn.start[rule.ruleIndex].stateNumber]
                        .Add(state.stateNumber);
            }
        }

        var queue = new Queue<int>();
        var queued = new bool[atn.allStates.Length];
        foreach (var state in atn.allStates)
        {
            if (state == null) continue;
            queue.Enqueue(state.stateNumber);
            queued[state.stateNumber] = true;
        }
        while (queue.Count > 0)
        {
            int stateNumber = queue.Dequeue();
            queued[stateNumber] = false;
            var state = atn.allStates[stateNumber];
            var summary = summaries[stateNumber];
            bool changed = false;
            if (state.stateType == MyStateType.RuleStop && !summary.Nullable)
            {
                summary.Nullable = true;
                changed = true;
            }
            foreach (var transition in state.transitions)
                changed |= AddTransitionSummary(
                    atn, summaries, summary, transition);
            if (!changed) continue;
            foreach (int dependent in dependents[stateNumber])
            {
                if (queued[dependent]) continue;
                queue.Enqueue(dependent);
                queued[dependent] = true;
            }
        }

        var result = new ushort[atn.decisionToState.Length][];
        for (int decision = 0; decision < result.Length; decision++)
        {
            var state = atn.decisionToState[decision];
            if (state.transitions.Count <= 1 ||
                state.transitions.Count > ushort.MaxValue) continue;
            var table = new ushort[atn.maxTokenType + 2];
            bool safe = true;
            for (int alt = 1; alt <= state.transitions.Count && safe; alt++)
            {
                var alternative = SummarizeTransition(
                    atn, summaries, state.transitions[alt - 1]);
                if (alternative.Unsafe || alternative.Nullable ||
                    alternative.First.Count == 0)
                {
                    safe = false;
                    break;
                }
                foreach (int tokenType in alternative.First)
                {
                    int index = tokenType + 1;
                    if ((uint)index >= (uint)table.Length || table[index] != 0)
                    {
                        safe = false;
                        break;
                    }
                    table[index] = (ushort)alt;
                }
            }
            if (safe) result[decision] = table;
        }
        return result;
    }

    private static bool AddTransitionSummary(
        MyATN atn, StateSummary[] summaries, StateSummary destination,
        MyTransition transition)
    {
        var source = SummarizeTransition(atn, summaries, transition);
        bool changed = destination.First.UnionWithChanged(source.First);
        if (source.Nullable && !destination.Nullable)
        {
            destination.Nullable = true;
            changed = true;
        }
        if (source.Unsafe && !destination.Unsafe)
        {
            destination.Unsafe = true;
            changed = true;
        }
        return changed;
    }

    private static StateSummary SummarizeTransition(
        MyATN atn, StateSummary[] summaries, MyTransition transition)
    {
        if (IsTerminal(transition))
        {
            var terminal = new StateSummary();
            if (transition.Matches(-1, 0, atn.maxTokenType))
                terminal.First.Add(-1);
            for (int tokenType = 1;
                 tokenType <= atn.maxTokenType; tokenType++)
                if (transition.Matches(tokenType, 1, atn.maxTokenType))
                    terminal.First.Add(tokenType);
            return terminal;
        }

        if (transition is MyPredicateTransition or
            MyPrecedencePredicateTransition)
            return StateSummary.PredicateDependent;

        if (transition is MyRuleTransition rule)
        {
            var called = summaries[atn.start[rule.ruleIndex].stateNumber];
            var result = new StateSummary(called);
            if (called.Nullable)
            {
                var follow = summaries[rule.target.stateNumber];
                result.First.UnionWith(follow.First);
                result.Nullable = follow.Nullable;
                result.Unsafe |= follow.Unsafe;
            }
            else
            {
                result.Nullable = false;
            }
            return result;
        }

        return new StateSummary(summaries[transition.target.stateNumber]);
    }

    private static bool IsTerminal(MyTransition transition) =>
        transition is MyAtomTransition or MySetTransition or
        MyNotSetTransition or MyWildcardTransition or MyRangeTransition;

    private sealed class StateSummary
    {
        public static StateSummary PredicateDependent => new()
        {
            Unsafe = true
        };

        public StateSummary()
        {
        }

        public StateSummary(StateSummary source)
        {
            First.UnionWith(source.First);
            Nullable = source.Nullable;
            Unsafe = source.Unsafe;
        }

        public HashSet<int> First { get; } = new();
        public bool Nullable { get; set; }
        public bool Unsafe { get; set; }
    }
}

internal static class HashSetExtensions
{
    public static bool UnionWithChanged<T>(
        this HashSet<T> destination, IEnumerable<T> source)
    {
        int count = destination.Count;
        destination.UnionWith(source);
        return destination.Count != count;
    }
}
