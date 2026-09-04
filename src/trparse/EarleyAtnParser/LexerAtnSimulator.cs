namespace EarleyAtnParser;

using Atn;

/// <summary>
/// Character-level ATN-based lexer. Implements a longest-match NFA simulation
/// from scratch, with no Antlr4 runtime dependencies in the core algorithm.
/// </summary>
public partial class LexerAtnSimulator
{
    private readonly MyATN _atn;
    private readonly bool _enableDfa;
    private readonly LexContextCache _contextCache = new();
    private const int DEFAULT_CHANNEL = 0;
    private const int EOF = -1;

    public LexerStatistics Statistics { get; }

    public LexerAtnSimulator(MyATN lexerAtn, LexerStatistics statistics = null)
        : this(lexerAtn, statistics, true)
    {
    }

    internal LexerAtnSimulator(
        MyATN lexerAtn, LexerStatistics statistics, bool enableDfa)
    {
        _atn = lexerAtn;
        _enableDfa = enableDfa;
        Statistics = statistics;
        _modeStartStates = new DfaState[lexerAtn.modeToStartState.Length];
    }

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
        string input, Cursor cursor, IReadOnlySet<int> expectedTokenTypes = null,
        bool recordStatistics = true)
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
        int startMode = cursor.Mode;
        var match = MatchNextToken(
            input, start, cursor.Mode, expectedTokenTypes,
            collectCandidates: recordStatistics && Statistics != null);
        int matchedRule = match.RuleIndex;
        int matchEnd = match.EndPosition;
        var actions = match.Actions;

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
        if (recordStatistics && Statistics != null)
            Statistics.Record(
                start, startLine, startColumn, startMode,
                match.Candidates, match.OrdinaryWinner, match.SelectedWinner,
                expectedTokenTypes, match.UsedContextFallback);
        return token;
    }

    private sealed record MatchResult(
        int RuleIndex,
        int EndPosition,
        List<IMyLexerAction> Actions,
        IReadOnlyList<LexerCandidate> Candidates,
        LexerCandidate OrdinaryWinner,
        LexerCandidate SelectedWinner,
        bool UsedContextFallback);

    private MatchResult MatchNextToken(
        string input, int startPos, int mode,
        IReadOnlySet<int> expectedTokenTypes = null,
        bool collectCandidates = false)
    {
        if (mode < 0 || mode >= _atn.modeToStartState.Length)
            return EmptyMatch(startPos);
        var current = GetModeStartState(mode);
        int pos = startPos;
        int bestRule = -1, bestEnd = -1, bestActions = 0;
        int expectedRule = -1, expectedEnd = -1, expectedActions = 0;
        Dictionary<int, (int End, int Actions)> acceptedRules = collectCandidates
            ? new()
            : null;

        CheckAccepts(current.Configs, pos, expectedTokenTypes,
            ref bestRule, ref bestEnd, ref bestActions,
            ref expectedRule, ref expectedEnd, ref expectedActions,
            acceptedRules);

        while (pos < input.Length)
        {
            int ch = input[pos];
            var next = GetTargetState(current, ch);
            if (next == null) break;
            pos++;
            current = next;
            CheckAccepts(current.Configs, pos, expectedTokenTypes,
                ref bestRule, ref bestEnd, ref bestActions,
                ref expectedRule, ref expectedEnd, ref expectedActions,
                acceptedRules);
        }

        // EOF is a real lexer-ATN symbol. It does not consume a character, but
        // rules such as line comments commonly use (... | EOF) to terminate at
        // the end of a file that has no trailing newline.
        if (pos == input.Length)
        {
            var eof = GetTargetState(current, EOF);
            if (eof != null)
            {
                CheckAccepts(eof.Configs, pos, expectedTokenTypes,
                    ref bestRule, ref bestEnd, ref bestActions,
                    ref expectedRule, ref expectedEnd, ref expectedActions,
                    acceptedRules);
            }
        }

        int ordinaryRule = bestRule;
        int ordinaryEnd = bestEnd;
        int ordinaryActions = bestActions;
        bool usedContextFallback = expectedTokenTypes != null && expectedRule < 0;
        if (expectedRule >= 0)
        {
            bestRule = expectedRule;
            bestEnd = expectedEnd;
            bestActions = expectedActions;
        }

        if (bestRule < 0) return EmptyMatch(startPos);

        var resolvedActions = new List<IMyLexerAction>();
        for (int i = 0; i < _atn.lexerActions.Length && i < 32; i++)
            if ((bestActions & (1 << i)) != 0)
                resolvedActions.Add(_atn.lexerActions[i]);

        var candidates = acceptedRules == null
            ? Array.Empty<LexerCandidate>()
            : acceptedRules
                .Where(entry => entry.Value.End > startPos)
                .Select(entry => CreateCandidate(
                    input, startPos, entry.Key,
                    entry.Value.End, entry.Value.Actions))
                .OrderBy(candidate => candidate.RuleIndex)
                .ToArray();
        var ordinaryWinner = CreateCandidate(
            input, startPos, ordinaryRule, ordinaryEnd, ordinaryActions);
        var selectedWinner = CreateCandidate(
            input, startPos, bestRule, bestEnd, bestActions);
        return new MatchResult(
            bestRule, bestEnd, resolvedActions, candidates,
            ordinaryWinner, selectedWinner, usedContextFallback);
    }

    private static MatchResult EmptyMatch(int startPosition)
    {
        var empty = new LexerCandidate(-1, -1, startPosition, 0, false, "");
        return new MatchResult(
            -1, startPosition, new List<IMyLexerAction>(),
            Array.Empty<LexerCandidate>(), empty, empty, false);
    }

    private HashSet<NonGreedyAccept> CheckAccepts(
        HashSet<LexerConfig> configs, int pos,
        IReadOnlySet<int> expectedTokenTypes,
        ref int bestRule, ref int bestEnd, ref int bestActions,
        ref int expectedRule, ref int expectedEnd, ref int expectedActions,
        Dictionary<int, (int End, int Actions)> acceptedRules)
    {
        var nonGreedyAccepts = new HashSet<NonGreedyAccept>(NonGreedyAcceptEq.Instance);
        foreach (var c in configs)
        {
            // A non-greedy decision can be inside a referenced lexer rule
            // (fragment), rather than directly in the outer token rule. Keep
            // its call context so lower-priority continuations are pruned only
            // after the complete outer token accepts. A locally valid fragment
            // exit may still fail a recursive caller suffix (as in CMake's
            // bracket arguments), in which case the continuation is required.
            if (c.State.stateType == MyStateType.RuleStop && c.Stack.IsEmpty)
            {
                int ri = c.State.ruleIndex;
                if (acceptedRules != null &&
                    (!acceptedRules.TryGetValue(ri, out var accepted) || pos > accepted.End))
                    acceptedRules[ri] = (pos, c.Actions);
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
                nonGreedyAccepts.Add(new NonGreedyAccept(
                        ri, c.NonGreedyDecision, c.NonGreedyContext));
            }
        }
        return nonGreedyAccepts;
    }

    private LexerCandidate CreateCandidate(
        string input, int startPosition, int ruleIndex,
        int endPosition, int actionBits)
    {
        var (tokenType, channel, skip) = ResolveDisposition(ruleIndex, actionBits);
        return new LexerCandidate(
            ruleIndex, tokenType, endPosition, channel, skip,
            endPosition >= startPosition
                ? input.Substring(startPosition, endPosition - startPosition)
                : "");
    }

    private (int TokenType, int Channel, bool Skip) ResolveDisposition(
        int ruleIndex, int actionBits)
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
        return (tokenType, channel, skip);
    }

    private bool IsContextEligible(
        int ruleIndex, int actionBits, IReadOnlySet<int> expectedTokenTypes)
    {
        var (tokenType, channel, skip) = ResolveDisposition(ruleIndex, actionBits);
        return skip || channel != DEFAULT_CHANNEL || expectedTokenTypes.Contains(tokenType);
    }

    private HashSet<LexerConfig> Scan(HashSet<LexerConfig> configs, int ch)
    {
        var next = new HashSet<LexerConfig>(LexerConfigEq.Instance);
        foreach (var c in configs)
        {
            for (int transitionIndex = 0;
                 transitionIndex < c.State.transitions.Count;
                 transitionIndex++)
            {
                var tr = c.State.transitions[transitionIndex];
                if (CharMatches(tr, ch))
                    next.Add(new LexerConfig(
                        tr.target, c.Stack, c.Actions, c.OuterRule,
                        NextNonGreedyDecision(c),
                        NextNonGreedyContext(c),
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
                    ret, rest, c.Actions, c.OuterRule,
                    c.NonGreedyDecision,
                    c.NonGreedyContext,
                    true);
                if (configs.Add(next)) work.Push(next);
                continue;
            }

            for (int transitionIndex = 0;
                 transitionIndex < c.State.transitions.Count;
                 transitionIndex++)
            {
                var tr = c.State.transitions[transitionIndex];
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
                            NextNonGreedyContext(c),
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
                            NextNonGreedyContext(c),
                            c.CompletedInnerRule);
                        if (configs.Add(next)) work.Push(next);
                        break;

                    case MyRuleTransition rt:
                        // rt.target = followState; push it, then move to rule start
                        var pushed = _contextCache.Push(c.Stack, rt.target);
                        var ruleStart = _atn.start[rt.ruleIndex];
                        next = new LexerConfig(
                            ruleStart, pushed, c.Actions, c.OuterRule,
                            NextNonGreedyDecision(c),
                            NextNonGreedyContext(c),
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

    private static LexStack NextNonGreedyContext(LexerConfig config) =>
        config.State.transitions.Count > 1 && config.State.nonGreedy
            ? config.Stack
            : config.NonGreedyContext;

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
        public readonly LexStack NonGreedyContext;
        // True when the most recently consumed character completed any
        // referenced rule. This preserves a higher-priority fragment path
        // when a wildcard path can accept at the same position.
        public readonly bool CompletedInnerRule;
        public LexerConfig(MyATNState state, LexStack stack, int actions,
                           int outerRule, int nonGreedyDecision,
                           LexStack nonGreedyContext,
                           bool completedInnerRule)
        {
            State = state;
            Stack = stack;
            Actions = actions;
            OuterRule = outerRule;
            NonGreedyDecision = nonGreedyDecision;
            NonGreedyContext = nonGreedyContext;
            CompletedInnerRule = completedInnerRule;
        }
    }

    // Equality ignores action bits, but preserves fields which affect matching.
    private sealed class LexerConfigEq : IEqualityComparer<LexerConfig>
    {
        public static readonly LexerConfigEq Instance = new();

        public bool Equals(LexerConfig x, LexerConfig y)
            => ReferenceEquals(x.State, y.State) &&
               x.Stack.Id == y.Stack.Id &&
               x.OuterRule == y.OuterRule &&
               x.NonGreedyDecision == y.NonGreedyDecision &&
               x.NonGreedyContext.Id == y.NonGreedyContext.Id &&
               x.CompletedInnerRule == y.CompletedInnerRule;

        public int GetHashCode(LexerConfig c)
        {
            unchecked
            {
                int hash = c.State.stateNumber * 31 + c.Stack.GetHashCode();
                hash = hash * 31 + c.OuterRule;
                hash = hash * 31 + c.NonGreedyDecision;
                hash = hash * 31 + c.NonGreedyContext.GetHashCode();
                hash = hash * 31 + (c.CompletedInnerRule ? 1 : 0);
                return hash;
            }
        }
    }

    private readonly record struct NonGreedyAccept(
        int Rule, int Decision, LexStack Context);

    private sealed class NonGreedyAcceptEq : IEqualityComparer<NonGreedyAccept>
    {
        public static readonly NonGreedyAcceptEq Instance = new();

        public bool Equals(NonGreedyAccept x, NonGreedyAccept y) =>
            x.Rule == y.Rule && x.Decision == y.Decision &&
            x.Context.Id == y.Context.Id;

        public int GetHashCode(NonGreedyAccept value) => HashCode.Combine(
            value.Rule, value.Decision, value.Context.Id);
    }

    // Canonical persistent stack for return states (fragment rule calls).
    // Equal (return-state, parent-context) pairs share one node and compact ID.
    private readonly struct LexStack
    {
        private readonly LexContext _context;
        public static LexStack Empty => default;
        public bool IsEmpty => _context == null;
        public int Id => _context?.Id ?? 0;
        public LexStack(LexContext context) => _context = context;

        public (MyATNState head, LexStack rest) Pop()
            => (_context.ReturnState, new LexStack(_context.Parent));

        public override int GetHashCode() => Id;

    }

    private sealed class LexContext
    {
        public readonly int Id;
        public readonly MyATNState ReturnState;
        public readonly LexContext Parent;

        public LexContext(int id, MyATNState returnState, LexContext parent)
        {
            Id = id;
            ReturnState = returnState;
            Parent = parent;
        }
    }

    private sealed class LexContextCache
    {
        private readonly Dictionary<(int ParentId, int ReturnState), LexContext>
            _contexts = new();
        private readonly List<LexContext> _byId = [null];
        private int _nextId = 1;

        public int Count => _contexts.Count;

        public LexStack Push(LexStack parent, MyATNState returnState)
        {
            var key = (parent.Id, returnState.stateNumber);
            if (!_contexts.TryGetValue(key, out var context))
            {
                var parentContext = _byId[parent.Id];
                context = new LexContext(_nextId++, returnState, parentContext);
                _contexts.Add(key, context);
                _byId.Add(context);
            }
            return new LexStack(context);
        }
    }
}
