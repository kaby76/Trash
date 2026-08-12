namespace Trash.EarleyAtn;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

/// <summary>
/// Earley parser over a MyATN (parser ATN). Epsilon transitions (including
/// action/predicate/precedence) are followed silently during closure.
/// Produces a single-derivation ParserRuleContext parse tree.
///
/// Key design: each Item includes its chart position so that the global
/// backpointer dictionary can store separate backs for the same logical
/// (state, origin, stack) that appears at different chart positions.
/// </summary>
public static class EarleyParser
{
    /// <summary>
    /// Parse <paramref name="allTokens"/> (all channels, including HIDDEN; EOF at end)
    /// and return an IParseTree, or null if the input is rejected.
    /// Off-channel tokens are skipped during scanning but kept in the token stream
    /// so that predicates and tree consumers can access them.
    /// </summary>
    public static IParseTree Parse(MyATN atn, IReadOnlyList<IToken> allTokens, int startRuleIndex)
    {
        if (atn == null) throw new ArgumentNullException(nameof(atn));
        if (startRuleIndex < 0 || startRuleIndex >= atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        // Build on-channel index map: chart position k → index in allTokens.
        // This mirrors CommonTokenStream's behaviour: the parser sees only
        // DEFAULT_CHANNEL + EOF, but off-channel tokens remain accessible.
        var onIdx = new List<int>();
        for (int i = 0; i < allTokens.Count; i++)
        {
            var t = allTokens[i];
            if (t.Channel == TokenConstants.DefaultChannel || t.Type == TokenConstants.EOF)
                onIdx.Add(i);
        }

        int n = onIdx.Count; // chart size; last entry is EOF

        var chart = new List<HashSet<Item>>(n + 1);
        // backs: keyed on positioned Item (state + origin + stack + position)
        var backs = new Dictionary<Item, Back>(Item.EqComparer);
        for (int k = 0; k <= n; k++) chart.Add(new HashSet<Item>(Item.EqComparer));

        // Seed
        var startState = atn.start[startRuleIndex];
        var seed = new Item(startState, 0, CallStack.Empty, 0);
        chart[0].Add(seed);
        backs[seed] = Back.Seed();
        Closure(atn, chart[0], backs, 0);

        // Earley scan loop — chart positions index into onIdx
        for (int k = 0; k < n; k++)
        {
            var next = chart[k + 1];
            var tok = allTokens[onIdx[k]]; // on-channel token at chart position k

            foreach (var it in chart[k])
            {
                foreach (var tr in it.State.transitions)
                {
                    if (IsTerminal(tr) && TerminalMatches(tr, tok.Type))
                    {
                        var adv = new Item(tr.target, it.Origin, it.CallStack, k + 1);
                        int allTokIdx = onIdx[k]; // store all-token index for BuildTree
                        if (next.Add(adv))
                            backs[adv] = Back.Scan(it, allTokIdx);
                        else if (!backs.ContainsKey(adv))
                            backs[adv] = Back.Scan(it, allTokIdx);
                    }
                }
            }
            Closure(atn, next, backs, k + 1);
        }

        // Find accept: stop state with empty callstack at position n
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

        var events = ReconstructEvents(accept.Value, backs);
        if (events == null) return null;

        return BuildTree(events, allTokens, onIdx, startRuleIndex);
    }

    // =========================================================================
    // Closure
    // =========================================================================

    private static void Closure(MyATN atn, HashSet<Item> set, Dictionary<Item, Back> backs, int pos)
    {
        var work = new Stack<Item>(set);
        // visited: use the set itself (items added there are already visited)

        while (work.Count > 0)
        {
            var it = work.Pop();

            // Completion: at RuleStop with non-empty callstack → pop
            if (atn.stop.Contains(it.State) && !it.CallStack.IsEmpty)
            {
                var (follow, rest) = it.CallStack.Pop();
                var cont = new Item(follow, it.Origin, rest, pos);
                if (set.Add(cont))
                {
                    work.Push(cont);
                    backs.TryAdd(cont, Back.Complete(it, it.State.ruleIndex));
                }
                else if (!backs.ContainsKey(cont))
                    backs[cont] = Back.Complete(it, it.State.ruleIndex);
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
                        if (set.Add(next)) { work.Push(next); backs.TryAdd(next, Back.Epsilon(it)); }
                        else if (!backs.ContainsKey(next)) backs[next] = Back.Epsilon(it);
                        break;

                    case MyRuleTransition rt:
                        // rt.target = followState; push it, then jump to the rule's start state
                        var pushed = it.CallStack.Push(rt.target);
                        var ruleStart = atn.start[rt.ruleIndex];
                        var enter = new Item(ruleStart, it.Origin, pushed, pos);
                        if (set.Add(enter)) { work.Push(enter); backs.TryAdd(enter, Back.Predict(it, rt.ruleIndex)); }
                        else if (!backs.ContainsKey(enter)) backs[enter] = Back.Predict(it, rt.ruleIndex);
                        break;
                }
            }
        }
    }

    // =========================================================================
    // Backpointer reconstruction
    // =========================================================================

    private static List<Event> ReconstructEvents(Item accept, Dictionary<Item, Back> backs)
    {
        var ev = new List<Event>();
        var cur = accept;

        while (true)
        {
            if (!backs.TryGetValue(cur, out var b)) return null;
            switch (b.Kind)
            {
                case BackKind.Seed:
                    goto done;
                case BackKind.Scan:
                    ev.Add(Event.Consume(b.TokenIndex));
                    cur = b.Prev;
                    break;
                case BackKind.Epsilon:
                    cur = b.Prev;
                    break;
                case BackKind.Predict:
                    ev.Add(Event.EnterRule(b.RuleIndex));
                    cur = b.Prev;
                    break;
                case BackKind.Complete:
                    ev.Add(Event.ExitRule(b.RuleIndex));
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
    // Tree construction
    // =========================================================================

    // allTokens: full token stream (all channels).
    // onIdx:     chart-position k → index into allTokens for the k-th on-channel token.
    // Consume events carry an all-token index; cursor tracks on-channel position.
    private static IParseTree BuildTree(List<Event> events, IReadOnlyList<IToken> allTokens,
        IReadOnlyList<int> onIdx, int startRuleIndex)
    {
        var root = new GenericRuleContext(null, -1, startRuleIndex);
        if (onIdx.Count > 0) root.Start = allTokens[onIdx[0]];
        var stack = new Stack<GenericRuleContext>();
        stack.Push(root);

        int cursor = 0; // on-channel position (index into onIdx)
        foreach (var e in events)
        {
            switch (e.Kind)
            {
                case EventKind.EnterRule:
                {
                    var ctx = new GenericRuleContext(stack.Peek(), 0, e.RuleIndex);
                    if (cursor < onIdx.Count) ctx.Start = allTokens[onIdx[cursor]];
                    stack.Peek().AddChild(ctx);
                    stack.Push(ctx);
                    break;
                }
                case EventKind.ExitRule:
                {
                    var done = stack.Pop();
                    int stopPos = cursor > 0 ? Math.Min(cursor - 1, onIdx.Count - 1) : 0;
                    if (onIdx.Count > 0) done.Stop = allTokens[onIdx[stopPos]];
                    break;
                }
                case EventKind.Consume:
                {
                    int i = e.TokenIndex; // index into allTokens
                    if (i >= 0 && i < allTokens.Count)
                    {
                        var term = new TerminalNodeImpl(allTokens[i]);
                        stack.Peek().AddChild(term);
                        stack.Peek().Stop = allTokens[i];
                        cursor++; // advance on-channel position
                    }
                    break;
                }
            }
        }

        // Close any rules left open
        while (stack.Count > 1)
        {
            var done = stack.Pop();
            int stopPos = cursor > 0 ? Math.Min(cursor - 1, onIdx.Count - 1) : 0;
            if (onIdx.Count > 0) done.Stop = allTokens[onIdx[stopPos]];
        }

        if (root.Stop == null && onIdx.Count > 0)
            root.Stop = allTokens[onIdx[Math.Max(0, Math.Min(cursor, onIdx.Count - 1))]];

        return root;
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
        MyNotSetTransition ns => !ns.set.Contains(tokenType) && tokenType != TokenConstants.EOF,
        MyWildcardTransition  => tokenType != TokenConstants.EOF,
        MyRangeTransition r   => tokenType >= r.from && tokenType <= r.to,
        _ => false
    };

    // =========================================================================
    // Item: (state, origin, callStack, position)
    // Position is the chart index where this item lives.
    // Including position prevents backpointer collisions for the same logical
    // item appearing in different chart sets.
    // =========================================================================

    private readonly struct Item
    {
        public MyATNState State { get; }
        public int Origin { get; }
        public CallStack CallStack { get; }
        public int Position { get; }

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
            public readonly Node Tail;
            public Node(MyATNState h, Node t) { Head = h; Tail = t; }
        }

        private readonly Node _node;
        public static CallStack Empty => default;
        public bool IsEmpty => _node == null;
        private CallStack(Node n) => _node = n;

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

    private enum BackKind { Seed, Epsilon, Predict, Complete, Scan }

    private sealed class Back
    {
        public BackKind Kind { get; }
        public Item Prev { get; }
        public int RuleIndex { get; }
        public int TokenIndex { get; }

        private Back(BackKind k, Item p, int r, int t) { Kind = k; Prev = p; RuleIndex = r; TokenIndex = t; }

        public static Back Seed() => new(BackKind.Seed, default, -1, -1);
        public static Back Epsilon(Item p) => new(BackKind.Epsilon, p, -1, -1);
        public static Back Predict(Item p, int r) => new(BackKind.Predict, p, r, -1);
        public static Back Complete(Item p, int r) => new(BackKind.Complete, p, r, -1);
        public static Back Scan(Item p, int t) => new(BackKind.Scan, p, -1, t);
    }

    // =========================================================================
    // Events
    // =========================================================================

    private enum EventKind { EnterRule, ExitRule, Consume }

    private sealed class Event
    {
        public EventKind Kind { get; }
        public int RuleIndex { get; }
        public int TokenIndex { get; }

        private Event(EventKind k, int r, int t) { Kind = k; RuleIndex = r; TokenIndex = t; }

        public static Event EnterRule(int r) => new(EventKind.EnterRule, r, -1);
        public static Event ExitRule(int r)  => new(EventKind.ExitRule, r, -1);
        public static Event Consume(int t)   => new(EventKind.Consume, -1, t);
    }

    // =========================================================================
    // Generic parse-tree node
    // =========================================================================

    private sealed class GenericRuleContext : ParserRuleContext
    {
        private readonly int _ruleIndex;
        public override int RuleIndex => _ruleIndex;

        public GenericRuleContext(ParserRuleContext parent, int invokingState, int ruleIndex)
            : base(parent, invokingState) => _ruleIndex = ruleIndex;
    }
}
