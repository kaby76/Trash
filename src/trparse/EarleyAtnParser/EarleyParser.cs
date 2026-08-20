namespace EarleyAtnParser;

using Atn;

// No Antlr4.Runtime.Standard types used anywhere in this file.

/// <summary>
/// Earley parser over a MyATN (parser ATN). Produces a ParseEvent sequence
/// (EnterRule / ExitRule / Consume) from which the caller builds a DOM tree.
/// Off-channel tokens are skipped during scanning.
/// </summary>
public static class EarleyParser
{
    // DEFAULT_CHANNEL = 0, EOF = -1  (ANTLR4 conventions; no runtime import needed)
    private const int DEFAULT_CHANNEL = 0;
    private const int EOF_TYPE = -1;

    /// <summary>
    /// Parse allTokens (all channels, EOF at end) and return an ordered
    /// ParseEvent list, or null if the input is rejected by the grammar.
    /// </summary>
    public static List<ParseEvent> Parse(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex)
    {
        if (atn == null) throw new ArgumentNullException(nameof(atn));
        if (startRuleIndex < 0 || startRuleIndex >= atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        // Build on-channel index map: chart position k → index in allTokens.
        var onIdx = new List<int>();
        for (int i = 0; i < allTokens.Count; i++)
        {
            var t = allTokens[i];
            if (t.Channel == DEFAULT_CHANNEL || t.Type == EOF_TYPE)
                onIdx.Add(i);
        }

        int n = onIdx.Count; // chart size; last entry is EOF

        var chart = new List<HashSet<Item>>(n + 1);
        var backs = new Dictionary<Item, Back>(Item.EqComparer);
        for (int k = 0; k <= n; k++) chart.Add(new HashSet<Item>(Item.EqComparer));

        // Seed
        var startState = atn.start[startRuleIndex];
        var seed = new Item(startState, 0, CallStack.Empty, 0);
        chart[0].Add(seed);
        backs[seed] = Back.Seed();
        Closure(atn, chart[0], backs, 0);

        // Earley scan loop
        for (int k = 0; k < n; k++)
        {
            var next = chart[k + 1];
            var tok = allTokens[onIdx[k]];

            foreach (var it in chart[k])
            {
                foreach (var tr in it.State.transitions)
                {
                    if (IsTerminal(tr) && TerminalMatches(tr, tok.Type))
                    {
                        var adv = new Item(tr.target, it.Origin, it.CallStack, k + 1);
                        int allTokIdx = onIdx[k];
                        if (next.Add(adv))
                            backs[adv] = Back.Scan(it, allTokIdx);
                        else if (!backs.ContainsKey(adv))
                            backs[adv] = Back.Scan(it, allTokIdx);
                    }
                }
            }
            Closure(atn, next, backs, k + 1);
        }

        // Find accept: stop state with empty callstack at chart position n
        Item? accept = null;
        foreach (var it in chart[n])
        {
            if (it.CallStack.IsEmpty && atn.stop.Contains(it.State))
            {
                accept = it;
                break;
            }
        }

        if (accept == null) return null;

        var innerEvents = ReconstructEvents(accept.Value, backs);
        if (innerEvents == null) return null;

        // Wrap with enter/exit for the start rule (the seed has no Predict back-pointer).
        var events = new List<ParseEvent>(innerEvents.Count + 2);
        bool startIsRecursion = atn.start[startRuleIndex].isPrecedenceRule;
        events.Add(startIsRecursion
            ? ParseEvent.EnterRecursionRule(startRuleIndex)
            : ParseEvent.EnterRule(startRuleIndex));
        events.AddRange(innerEvents);
        events.Add(startIsRecursion
            ? ParseEvent.ExitRecursionRule(startRuleIndex)
            : ParseEvent.ExitRule(startRuleIndex));
        return events;
    }

    // =========================================================================
    // Closure
    // =========================================================================

    private static void Closure(
        MyATN atn, HashSet<Item> set, Dictionary<Item, Back> backs, int pos)
    {
        var work = new Stack<Item>(set);

        while (work.Count > 0)
        {
            var it = work.Pop();

            // Completion: at RuleStop with non-empty callstack → pop
            if (atn.stop.Contains(it.State) && !it.CallStack.IsEmpty)
            {
                var (follow, rest) = it.CallStack.Pop();
                var cont = new Item(follow, it.Origin, rest, pos);
                int ri = it.State.ruleIndex;
                Back completeBack = (ri >= 0 && ri < atn.start.Length && atn.start[ri].isPrecedenceRule)
                    ? Back.CompleteRecursion(it, ri)
                    : Back.Complete(it, ri);
                if (set.Add(cont))
                {
                    work.Push(cont);
                    backs.TryAdd(cont, completeBack);
                }
                else if (!backs.ContainsKey(cont))
                    backs[cont] = completeBack;
            }

            foreach (var tr in it.State.transitions)
            {
                Item next;
                switch (tr)
                {
                    case MyEpsilonTransition:
                    case MyActionTransition:
                    case MyPredicateTransition:
                    case MyPrecedencePredicateTransition:
                        next = new Item(tr.target, it.Origin, it.CallStack, pos);
                        // Detect the suffix-loop entry of a left-recursive rule:
                        // epsilon from StarLoopEntry.isPrecedenceDecision to a non-LoopEnd target.
                        Back epsilonBack =
                            it.State.stateType == MyStateType.StarLoopEntry &&
                            it.State.isPrecedenceDecision &&
                            tr.target.stateType != MyStateType.LoopEnd
                                ? Back.PushRecursion(it, it.State.ruleIndex)
                                : Back.Epsilon(it);
                        if (set.Add(next)) { work.Push(next); backs.TryAdd(next, epsilonBack); }
                        else if (!backs.ContainsKey(next)) backs[next] = epsilonBack;
                        break;

                    case MyRuleTransition rt:
                        var pushed    = it.CallStack.Push(rt.target);
                        var ruleStart = atn.start[rt.ruleIndex];
                        var enter     = new Item(ruleStart, it.Origin, pushed, pos);
                        // Left-recursive rule calls use Predict/Complete Recursion variants.
                        Back predictBack = ruleStart.isPrecedenceRule
                            ? Back.PredictRecursion(it, rt.ruleIndex)
                            : Back.Predict(it, rt.ruleIndex);
                        if (set.Add(enter)) { work.Push(enter); backs.TryAdd(enter, predictBack); }
                        else if (!backs.ContainsKey(enter)) backs[enter] = predictBack;
                        break;
                }
            }
        }
    }

    // =========================================================================
    // Backpointer reconstruction → ParseEvent list
    // =========================================================================

    private static List<ParseEvent> ReconstructEvents(Item accept, Dictionary<Item, Back> backs)
    {
        var ev  = new List<ParseEvent>();
        var cur = accept;

        while (true)
        {
            if (!backs.TryGetValue(cur, out var b)) return null;
            switch (b.Kind)
            {
                case BackKind.Seed:
                    goto done;
                case BackKind.Scan:
                    ev.Add(ParseEvent.Consume(b.TokenIndex));
                    cur = b.Prev;
                    break;
                case BackKind.Epsilon:
                    cur = b.Prev;
                    break;
                case BackKind.Predict:
                    ev.Add(ParseEvent.EnterRule(b.RuleIndex));
                    cur = b.Prev;
                    break;
                case BackKind.Complete:
                    ev.Add(ParseEvent.ExitRule(b.RuleIndex));
                    cur = b.Prev;
                    break;
                case BackKind.PredictRecursion:
                    ev.Add(ParseEvent.EnterRecursionRule(b.RuleIndex));
                    cur = b.Prev;
                    break;
                case BackKind.CompleteRecursion:
                    ev.Add(ParseEvent.ExitRecursionRule(b.RuleIndex));
                    cur = b.Prev;
                    break;
                case BackKind.PushRecursion:
                    ev.Add(ParseEvent.PushRecursionContext(b.RuleIndex));
                    cur = b.Prev;
                    break;
                default:
                    return null;
            }
        }
        done:
        ev.Reverse();
        return ev;
    }

    // =========================================================================
    // Terminal helpers
    // =========================================================================

    private static bool IsTerminal(MyTransition t) =>
        t is MyAtomTransition || t is MySetTransition || t is MyNotSetTransition ||
        t is MyWildcardTransition || t is MyRangeTransition;

    private static bool TerminalMatches(MyTransition t, int tokenType) => t switch
    {
        MyAtomTransition a    => a.label == tokenType,
        MySetTransition s     => s.set.Contains(tokenType),
        MyNotSetTransition ns => !ns.set.Contains(tokenType) && tokenType != EOF_TYPE,
        MyWildcardTransition  => tokenType != EOF_TYPE,
        MyRangeTransition r   => tokenType >= r.from && tokenType <= r.to,
        _ => false
    };

    // =========================================================================
    // Item: (state, origin, callStack, position)
    // =========================================================================

    private readonly struct Item
    {
        public MyATNState State     { get; }
        public int        Origin    { get; }
        public CallStack  CallStack { get; }
        public int        Position  { get; }

        public Item(MyATNState state, int origin, CallStack stack, int position)
        { State = state; Origin = origin; CallStack = stack; Position = position; }

        public static IEqualityComparer<Item> EqComparer { get; } = new ItemEq();

        private sealed class ItemEq : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
                => x.Position == y.Position &&
                   ReferenceEquals(x.State, y.State) &&
                   x.Origin == y.Origin &&
                   CallStack.Same(x.CallStack, y.CallStack);

            public int GetHashCode(Item o)
            {
                unchecked
                {
                    int h = o.Position;
                    h = h * 31 + o.State.stateNumber;
                    h = h * 31 + o.Origin;
                    h = h * 31 + o.CallStack.GetHashCode();
                    return h;
                }
            }
        }
    }

    private readonly struct CallStack
    {
        private sealed class Node
        {
            public readonly MyATNState Head;
            public readonly Node       Tail;
            public Node(MyATNState h, Node t) { Head = h; Tail = t; }
        }

        private readonly Node _node;
        public static CallStack Empty    => default;
        public bool             IsEmpty  => _node == null;
        private CallStack(Node n)        => _node = n;

        public CallStack Push(MyATNState s) => new(new Node(s, _node));

        public (MyATNState head, CallStack rest) Pop()
            => (_node.Head, new CallStack(_node.Tail));

        public static bool Same(CallStack a, CallStack b)
            => ReferenceEquals(a._node, b._node);

        public override int GetHashCode() => _node?.GetHashCode() ?? 0;
    }

    // =========================================================================
    // Backpointers
    // =========================================================================

    private enum BackKind { Seed, Epsilon, Predict, Complete, Scan, PredictRecursion, CompleteRecursion, PushRecursion }

    private sealed class Back
    {
        public BackKind Kind       { get; }
        public Item     Prev       { get; }
        public int      RuleIndex  { get; }
        public int      TokenIndex { get; }

        private Back(BackKind k, Item p, int r, int t)
        { Kind = k; Prev = p; RuleIndex = r; TokenIndex = t; }

        public static Back Seed()                          => new(BackKind.Seed,             default, -1, -1);
        public static Back Epsilon(Item p)                 => new(BackKind.Epsilon,          p,       -1, -1);
        public static Back Predict(Item p, int r)          => new(BackKind.Predict,          p,       r,  -1);
        public static Back Complete(Item p, int r)         => new(BackKind.Complete,         p,       r,  -1);
        public static Back Scan(Item p, int t)             => new(BackKind.Scan,             p,       -1, t );
        public static Back PredictRecursion(Item p, int r) => new(BackKind.PredictRecursion, p,       r,  -1);
        public static Back CompleteRecursion(Item p, int r)=> new(BackKind.CompleteRecursion,p,       r,  -1);
        public static Back PushRecursion(Item p, int r)    => new(BackKind.PushRecursion,    p,       r,  -1);
    }
}
