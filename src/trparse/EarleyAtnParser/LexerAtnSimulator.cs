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

    /// <summary>
    /// Mutable lexer position. Clone it when parser prediction needs speculative
    /// lookahead; only the cursor used by the selected parse path is committed.
    /// </summary>
    public sealed class Cursor
    {
        public int Position { get; set; }
        public int Line { get; set; } = 1;
        public int Column { get; set; }
        public int TokenIndex { get; set; }
        public int Mode { get; set; }
        internal Stack<int> ModeStack { get; set; } = new();

        public Cursor Clone() => new()
        {
            Position = Position,
            Line = Line,
            Column = Column,
            TokenIndex = TokenIndex,
            Mode = Mode,
            ModeStack = new Stack<int>(ModeStack.Reverse())
        };
    }

    public List<LexerToken> Tokenize(string input)
    {
        var tokens = new List<LexerToken>();
        var cursor = new Cursor();
        while (cursor.Position <= input.Length)
        {
            var token = NextToken(input, cursor);
            tokens.Add(token);
            if (token.Type == EOF) break;
        }
        return tokens;
    }

    /// <summary>
    /// Lex one token at the cursor. When expectedTokenTypes is supplied, matches
    /// producing one of those types are preferred before applying longest-match
    /// and rule-order priority. Skip/off-channel matches remain eligible. If no
    /// eligible match exists, ordinary ANTLR lexer selection is used.
    /// </summary>
    public LexerToken NextToken(
        string input, Cursor cursor, IReadOnlySet<int> expectedTokenTypes = null)
    {
        if (cursor.Position == input.Length)
        {
            var eof = new LexerToken
            {
                Type = EOF, Channel = DEFAULT_CHANNEL, Text = "<EOF>",
                StartIndex = cursor.Position, StopIndex = cursor.Position - 1,
                Line = cursor.Line, Column = cursor.Column,
                TokenIndex = cursor.TokenIndex++
            };
            // Move beyond the sentinel so accidental repeated calls are visible.
            cursor.Position++;
            return eof;
        }
        if (cursor.Position > input.Length)
            throw new InvalidOperationException("The lexer cursor is past EOF.");

        int start = cursor.Position;
        int startLine = cursor.Line;
        int startColumn = cursor.Column;
        var (matchedRule, matchEnd, actions) = MatchNextToken(
            input, start, cursor.Mode, expectedTokenTypes);

        if (matchedRule < 0)
            throw new InvalidOperationException(
                $"Lexer error at line {startLine}:{startColumn}: no rule matches '{input[start]}' (U+{(int)input[start]:X4}).");

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
                case MyLexerActionType.Mode:    cursor.Mode = action.Arg1; break;
                case MyLexerActionType.PushMode:
                    cursor.ModeStack.Push(cursor.Mode);
                    cursor.Mode = action.Arg1;
                    break;
                case MyLexerActionType.PopMode:
                    if (cursor.ModeStack.Count == 0)
                        throw new InvalidOperationException(
                            "Cannot pop the lexer mode because the mode stack is empty.");
                    cursor.Mode = cursor.ModeStack.Pop();
                    break;
                default:
                    throw new NotSupportedException(
                        $"Lexer action '{action.ActionType}' is not supported by the Earley ATN lexer.");
            }
        }

        var token = new LexerToken
        {
            Type = tokenType,
            Channel = skip ? LexerToken.SKIP_CHANNEL : channel,
            Text = input.Substring(start, matchEnd - start),
            StartIndex = start,
            StopIndex = matchEnd - 1,
            Line = startLine,
            Column = startColumn,
            TokenIndex = cursor.TokenIndex++
        };
        int line = cursor.Line, column = cursor.Column;
        UpdateLineCol(input, start, matchEnd, ref line, ref column);
        cursor.Line = line;
        cursor.Column = column;
        cursor.Position = matchEnd;
        return token;
    }

    // Returns (bestRuleIndex, endPosition, actions). bestRuleIndex < 0 on failure.
    private (int ruleIdx, int endPos, List<IMyLexerAction> actions) MatchNextToken(
        string input, int startPos, int mode,
        IReadOnlySet<int> expectedTokenTypes = null)
    {
        if (mode >= _atn.modeToStartState.Length) return (-1, startPos, new());
        var modeStart = _atn.modeToStartState[mode];

        var initConfigs = new HashSet<LexerConfig>(LexerConfigEq.Instance);
        initConfigs.Add(new LexerConfig(modeStart, LexStack.Empty, 0, -1, -1, false));
        EpsClosure(initConfigs);

        var current = initConfigs;
        int pos = startPos;
        int bestRule = -1, bestEnd = -1, bestActions = 0;
        int expectedRule = -1, expectedEnd = -1, expectedActions = 0;

        CheckAccepts(current, pos, expectedTokenTypes,
            ref bestRule, ref bestEnd, ref bestActions,
            ref expectedRule, ref expectedEnd, ref expectedActions);

        while (pos < input.Length)
        {
            int ch = input[pos];
            var next = Scan(current, ch);
            if (next.Count == 0) break;
            EpsClosure(next);
            pos++;
            current = next;
            var nonGreedyAccepts = CheckAccepts(current, pos, expectedTokenTypes,
                ref bestRule, ref bestEnd, ref bestActions,
                ref expectedRule, ref expectedEnd, ref expectedActions);
            if (nonGreedyAccepts.Count != 0)
                current.RemoveWhere(c =>
                    nonGreedyAccepts.Contains((c.OuterRule, c.NonGreedyDecision)) &&
                    !c.CompletedInnerRule);
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
                CheckAccepts(eof, pos, expectedTokenTypes,
                    ref bestRule, ref bestEnd, ref bestActions,
                    ref expectedRule, ref expectedEnd, ref expectedActions);
            }
        }

        if (expectedRule >= 0)
        {
            bestRule = expectedRule;
            bestEnd = expectedEnd;
            bestActions = expectedActions;
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
        IReadOnlySet<int> expectedTokenTypes,
        ref int bestRule, ref int bestEnd, ref int bestActions,
        ref int expectedRule, ref int expectedEnd, ref int expectedActions)
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
                if (expectedTokenTypes != null &&
                    IsContextEligible(ri, c.Actions, expectedTokenTypes) &&
                    (pos > expectedEnd || (pos == expectedEnd && ri < expectedRule)))
                {
                    expectedRule = ri;
                    expectedEnd = pos;
                    expectedActions = c.Actions;
                }
                if (c.NonGreedyDecision >= 0)
                    nonGreedyAccepts.Add((ri, c.NonGreedyDecision));
            }
        }
        return nonGreedyAccepts;
    }

    private bool IsContextEligible(
        int ruleIndex, int actionBits, IReadOnlySet<int> expectedTokenTypes)
    {
        int tokenType = _atn.ruleToTokenType[ruleIndex];
        int channel = DEFAULT_CHANNEL;
        bool skip = false;
        for (int i = 0; i < _atn.lexerActions.Length && i < 32; i++)
        {
            if ((actionBits & (1 << i)) == 0) continue;
            var action = _atn.lexerActions[i];
            if (action.ActionType == MyLexerActionType.Type) tokenType = action.Arg1;
            else if (action.ActionType == MyLexerActionType.Channel) channel = action.Arg1;
            else if (action.ActionType == MyLexerActionType.Skip) skip = true;
        }
        return skip || channel != DEFAULT_CHANNEL || expectedTokenTypes.Contains(tokenType);
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
                        NextNonGreedyDecision(c),
                        false));
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
                    ret, rest, c.Actions, c.OuterRule, c.NonGreedyDecision,
                    true);
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
                            NextNonGreedyDecision(c),
                            c.CompletedInnerRule);
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
                            NextNonGreedyDecision(c),
                            c.CompletedInnerRule);
                        if (configs.Add(next)) work.Push(next);
                        break;

                    case MyRuleTransition rt:
                        // rt.target = followState; push it, then move to rule start
                        var pushed = c.Stack.Push(rt.target);
                        var ruleStart = _atn.start[rt.ruleIndex];
                        next = new LexerConfig(
                            ruleStart, pushed, c.Actions, c.OuterRule,
                            NextNonGreedyDecision(c),
                            false);
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
        return config.State.nonGreedy
            ? config.State.stateNumber
            : config.NonGreedyDecision;
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
        // True when the most recently consumed character completed a
        // referenced lexer rule. Such a path has priority over a competing
        // wildcard path which accepts the outer rule at the same position.
        public readonly bool CompletedInnerRule;

        public LexerConfig(MyATNState state, LexStack stack, int actions,
                           int outerRule, int nonGreedyDecision,
                           bool completedInnerRule)
        {
            State = state;
            Stack = stack;
            Actions = actions;
            OuterRule = outerRule;
            NonGreedyDecision = nonGreedyDecision;
            CompletedInnerRule = completedInnerRule;
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
               x.NonGreedyDecision == y.NonGreedyDecision &&
               x.CompletedInnerRule == y.CompletedInnerRule;

        public int GetHashCode(LexerConfig c)
        {
            unchecked
            {
                int hash = c.State.stateNumber * 31 + c.Stack.GetHashCode();
                hash = hash * 31 + c.OuterRule;
                hash = hash * 31 + c.NonGreedyDecision;
                return hash * 31 + (c.CompletedInnerRule ? 1 : 0);
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
