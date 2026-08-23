namespace EarleyAtnParser;

using Atn;

/// <summary>
/// Character-level ATN-based lexer. Implements a longest-match NFA simulation
/// from scratch, with no Antlr4 runtime dependencies in the core algorithm.
/// </summary>
public class LexerAtnSimulator
{
    private readonly MyATN _atn;
    private const int DEFAULT_CHANNEL = 0;
    private const int EOF = -1;

    public LexerAtnSimulator(MyATN lexerAtn) => _atn = lexerAtn;

    public List<LexerToken> Tokenize(string input)
    {
        var tokens = new List<LexerToken>();
        int pos = 0;
        int line = 1, col = 0;
        int tokenIndex = 0;
        int mode = 0;
        var modeStack = new Stack<int>();

        while (pos <= input.Length)
        {
            if (pos == input.Length)
            {
                tokens.Add(new LexerToken
                {
                    Type = EOF, Channel = DEFAULT_CHANNEL, Text = "<EOF>",
                    StartIndex = pos, StopIndex = pos - 1,
                    Line = line, Column = col, TokenIndex = tokenIndex++
                });
                break;
            }

            var (matchedRule, matchEnd, actions) = MatchNextToken(input, pos, mode);

            if (matchedRule < 0)
                throw new InvalidOperationException(
                    $"Lexer error at line {line}:{col}: no rule matches '{input[pos]}' (U+{(int)input[pos]:X4}).");

            int tokenType = _atn.ruleToTokenType[matchedRule];
            int channel = DEFAULT_CHANNEL;
            bool skip = false;

            foreach (var action in actions)
            {
                switch (action.ActionType)
                {
                    case MyLexerActionType.Skip:    skip = true; break;
                    case MyLexerActionType.Channel: channel = action.Arg1; break;
                    case MyLexerActionType.Type:    tokenType = action.Arg1; break;
                    case MyLexerActionType.Mode:    mode = action.Arg1; break;
                    case MyLexerActionType.PushMode:
                        modeStack.Push(mode);
                        mode = action.Arg1;
                        break;
                    case MyLexerActionType.PopMode:
                        if (modeStack.Count == 0)
                            throw new InvalidOperationException(
                                "Cannot pop the lexer mode because the mode stack is empty.");
                        mode = modeStack.Pop();
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Lexer action '{action.ActionType}' is not supported by the Earley ATN lexer.");
                }
            }

            if (tokenType != 0)
            {
                tokens.Add(new LexerToken
                {
                    Type = tokenType,
                    Channel = skip ? LexerToken.SKIP_CHANNEL : channel,
                    Text = input.Substring(pos, matchEnd - pos),
                    StartIndex = pos,
                    StopIndex = matchEnd - 1,
                    Line = line,
                    Column = col,
                    TokenIndex = tokenIndex++
                });
            }

            UpdateLineCol(input, pos, matchEnd, ref line, ref col);
            pos = matchEnd;
        }

        return tokens;
    }

    // Returns (bestRuleIndex, endPosition, actions). bestRuleIndex < 0 on failure.
    private (int ruleIdx, int endPos, List<IMyLexerAction> actions) MatchNextToken(string input, int startPos, int mode)
    {
        if (mode >= _atn.modeToStartState.Length) return (-1, startPos, new());
        var modeStart = _atn.modeToStartState[mode];

        var initConfigs = new HashSet<LexerConfig>(LexerConfigEq.Instance);
        initConfigs.Add(new LexerConfig(modeStart, LexStack.Empty, 0, -1, -1));
        EpsClosure(initConfigs);

        var current = initConfigs;
        int pos = startPos;
        int bestRule = -1, bestEnd = -1, bestActions = 0;

        CheckAccepts(current, pos, ref bestRule, ref bestEnd, ref bestActions);

        while (pos < input.Length)
        {
            int ch = input[pos];
            var next = Scan(current, ch);
            if (next.Count == 0) break;
            EpsClosure(next);
            pos++;
            current = next;
            var nonGreedyAccepts = CheckAccepts(
                current, pos, ref bestRule, ref bestEnd, ref bestActions);
            if (nonGreedyAccepts.Count != 0)
                current.RemoveWhere(c =>
                    nonGreedyAccepts.Contains((c.OuterRule, c.NonGreedyDecision)));
        }

        // EOF is a real lexer-ATN symbol. It does not consume a character, but
        // rules such as line comments commonly use (... | EOF) to terminate at
        // the end of a file that has no trailing newline.
        if (pos == input.Length)
        {
            var eof = Scan(current, EOF);
            if (eof.Count != 0)
            {
                EpsClosure(eof);
                CheckAccepts(eof, pos, ref bestRule, ref bestEnd, ref bestActions);
            }
        }

        if (bestRule < 0) return (-1, startPos, new());

        var resolvedActions = new List<IMyLexerAction>();
        for (int i = 0; i < _atn.lexerActions.Length && i < 32; i++)
            if ((bestActions & (1 << i)) != 0)
                resolvedActions.Add(_atn.lexerActions[i]);

        return (bestRule, bestEnd, resolvedActions);
    }

    private HashSet<(int Rule, int Decision)> CheckAccepts(
        HashSet<LexerConfig> configs, int pos,
        ref int bestRule, ref int bestEnd, ref int bestActions)
    {
        var nonGreedyAccepts = new HashSet<(int, int)>();
        foreach (var c in configs)
        {
            if (c.State.stateType == MyStateType.RuleStop && c.Stack.IsEmpty)
            {
                int ri = c.State.ruleIndex;
                // Longer match wins; tie-break by earlier rule index
                if (pos > bestEnd || (pos == bestEnd && ri < bestRule))
                {
                    bestRule = ri;
                    bestEnd = pos;
                    bestActions = c.Actions;
                }
                if (c.NonGreedyDecision >= 0)
                    nonGreedyAccepts.Add((ri, c.NonGreedyDecision));
            }
        }
        return nonGreedyAccepts;
    }

    private HashSet<LexerConfig> Scan(HashSet<LexerConfig> configs, int ch)
    {
        var next = new HashSet<LexerConfig>(LexerConfigEq.Instance);
        foreach (var c in configs)
        {
            foreach (var tr in c.State.transitions)
            {
                if (CharMatches(tr, ch))
                    next.Add(new LexerConfig(
                        tr.target, c.Stack, c.Actions, c.OuterRule,
                        NextNonGreedyDecision(c)));
            }
        }
        return next;
    }

    private static bool CharMatches(MyTransition tr, int ch) => tr switch
    {
        MyAtomTransition a  => a.label == ch,
        MyRangeTransition r => ch >= r.from && ch <= r.to,
        MySetTransition s   => s.set.Contains(ch),
        MyNotSetTransition ns => !ns.set.Contains(ch) && ch >= 0,
        MyWildcardTransition  => ch >= 0,
        _ => false
    };

    private void EpsClosure(HashSet<LexerConfig> configs)
    {
        var work = new Stack<LexerConfig>();
        foreach (var c in configs) work.Push(c);

        while (work.Count > 0)
        {
            var c = work.Pop();

            // Completion: at RuleStop with non-empty stack → pop and continue
            if (c.State.stateType == MyStateType.RuleStop && !c.Stack.IsEmpty)
            {
                var (ret, rest) = c.Stack.Pop();
                var next = new LexerConfig(
                    ret, rest, c.Actions, c.OuterRule, c.NonGreedyDecision);
                if (configs.Add(next)) work.Push(next);
                continue;
            }

            foreach (var tr in c.State.transitions)
            {
                LexerConfig next;
                switch (tr)
                {
                    case MyEpsilonTransition:
                    case MyPredicateTransition:
                    case MyPrecedencePredicateTransition:
                        next = new LexerConfig(
                            tr.target, c.Stack, c.Actions,
                            c.OuterRule < 0 ? tr.target.ruleIndex : c.OuterRule,
                            NextNonGreedyDecision(c));
                        if (configs.Add(next)) work.Push(next);
                        break;

                    case MyActionTransition at:
                        int acts = c.Actions;
                        // ANTLR executes actions only in the outermost token rule.
                        // Actions reached inside a referenced lexer rule/fragment
                        // must not affect the token being assembled by its caller.
                        if (c.Stack.IsEmpty && at.actionIndex >= 0 && at.actionIndex < 32)
                            acts |= 1 << at.actionIndex;
                        next = new LexerConfig(
                            tr.target, c.Stack, acts, c.OuterRule,
                            NextNonGreedyDecision(c));
                        if (configs.Add(next)) work.Push(next);
                        break;

                    case MyRuleTransition rt:
                        // rt.target = followState; push it, then move to rule start
                        var pushed = c.Stack.Push(rt.target);
                        var ruleStart = _atn.start[rt.ruleIndex];
                        next = new LexerConfig(
                            ruleStart, pushed, c.Actions, c.OuterRule,
                            NextNonGreedyDecision(c));
                        if (configs.Add(next)) work.Push(next);
                        break;
                }
            }
        }
    }

    private static int NextNonGreedyDecision(LexerConfig config)
    {
        if (config.State.transitions.Count <= 1)
            return config.NonGreedyDecision;
        return config.State.nonGreedy ? config.State.stateNumber : -1;
    }

    private static void UpdateLineCol(string input, int from, int to, ref int line, ref int col)
    {
        for (int i = from; i < to; i++)
        {
            if (input[i] == '\n') { line++; col = 0; }
            else col++;
        }
    }

    // NFA configuration: state + persistent return-address stack + accumulated action bitfield
    private readonly struct LexerConfig
    {
        public readonly MyATNState State;
        public readonly LexStack Stack;
        public readonly int Actions; // bitfield indexing into atn.lexerActions
        public readonly int OuterRule;
        public readonly int NonGreedyDecision;

        public LexerConfig(MyATNState state, LexStack stack, int actions,
                           int outerRule, int nonGreedyDecision)
        {
            State = state;
            Stack = stack;
            Actions = actions;
            OuterRule = outerRule;
            NonGreedyDecision = nonGreedyDecision;
        }
    }

    // Equality ignores action bits, but preserves fields which affect matching.
    private sealed class LexerConfigEq : IEqualityComparer<LexerConfig>
    {
        public static readonly LexerConfigEq Instance = new();

        public bool Equals(LexerConfig x, LexerConfig y)
            => ReferenceEquals(x.State, y.State) &&
               LexStack.Same(x.Stack, y.Stack) &&
               x.OuterRule == y.OuterRule &&
               x.NonGreedyDecision == y.NonGreedyDecision;

        public int GetHashCode(LexerConfig c)
        {
            unchecked
            {
                int hash = c.State.stateNumber * 31 + c.Stack.GetHashCode();
                hash = hash * 31 + c.OuterRule;
                return hash * 31 + c.NonGreedyDecision;
            }
        }
    }

    // Persistent linked-list stack for return states (fragment rule calls)
    private readonly struct LexStack
    {
        private sealed class Node
        {
            public readonly MyATNState Head;
            public readonly Node Tail;
            public Node(MyATNState h, Node t) { Head = h; Tail = t; }
        }

        private readonly Node _node;
        public static LexStack Empty => default;
        public bool IsEmpty => _node == null;
        private LexStack(Node n) => _node = n;

        public LexStack Push(MyATNState s) => new(new Node(s, _node));

        public (MyATNState head, LexStack rest) Pop()
            => (_node.Head, new LexStack(_node.Tail));

        public static bool Same(LexStack a, LexStack b)
            => ReferenceEquals(a._node, b._node);

        public override int GetHashCode() => _node?.GetHashCode() ?? 0;
    }
}
