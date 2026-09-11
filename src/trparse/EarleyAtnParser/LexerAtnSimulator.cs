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
    private readonly LexContextCache _contextCache;
    private readonly Stack<LexerConfig> _closureWork = new();
    private readonly Dictionary<int, IMyLexerAction[]> _actionSets;
    private string _input;
    private const int DEFAULT_CHANNEL = 0;
    private const int EOF = -1;

    public LexerStatistics Statistics { get; }
    public bool RecordStatistics { get; set; } = true;

    public LexerAtnSimulator(MyATN lexerAtn, LexerStatistics statistics = null)
        : this(lexerAtn, statistics, true, null)
    {
    }

    public LexerAtnSimulator(MyATN lexerAtn, LexerStatistics statistics,
        LexerDfaCache dfaCache)
        : this(lexerAtn, statistics, true, dfaCache)
    {
    }

    internal LexerAtnSimulator(
        MyATN lexerAtn, LexerStatistics statistics, bool enableDfa,
        LexerDfaCache dfaCache = null)
    {
        _atn = lexerAtn;
        _enableDfa = enableDfa;
        Statistics = statistics;
        if (enableDfa && dfaCache != null)
        {
            dfaCache.Bind(lexerAtn);
            _modeStartStates = (DfaState[])dfaCache.ModeStartStates;
            _dfaStates = (List<DfaState>)dfaCache.DfaStates;
            _contextCache = (LexContextCache)dfaCache.ContextCache;
            _actionSets =
                (Dictionary<int, IMyLexerAction[]>)dfaCache.ActionSets;
        }
        else
        {
            _modeStartStates = new DfaState[lexerAtn.modeToStartState.Length];
            _dfaStates = new List<DfaState>();
            _contextCache = new LexContextCache();
            _actionSets = new Dictionary<int, IMyLexerAction[]>();
        }
        _dfaStatesAtStart = _dfaStates.Count;
        _dfaTransitionsAtStart = CountDfaTransitions();
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

    public TokenStore Tokenize(string input)
    {
        SetInput(input);
        var tokens = new TokenStore(input);
        var cursor = new Cursor();
        while (cursor.Position <= _input.Length)
        {
            int index = NextToken(tokens, cursor);
            if (tokens[index].Type == EOF) break;
        }
        return tokens;
    }

    public void SetInput(string input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    /// <summary>
    /// Lex one token at the cursor. When expectedTokenTypes is supplied, matches
    /// producing one of those types are preferred before applying longest-match
    /// and rule-order priority. Skip/off-channel matches remain eligible. If no
    /// eligible match exists, ordinary ANTLR lexer selection is used.
    /// </summary>
    public LexerToken NextToken(
        Cursor cursor, IReadOnlySet<int> expectedTokenTypes = null)
    {
        EnsureInput();
        int tokenIndex = cursor?.TokenIndex ?? 0;
        var tokens = new TokenStore(_input, 1);
        int index = NextToken(tokens, cursor, expectedTokenTypes);
        var stored = tokens[index];
        var token = new LexerToken(_input)
        {
            Type = stored.Type,
            Channel = stored.Channel,
            StartIndex = stored.StartIndex,
            StopIndex = stored.StopIndex,
            Line = stored.Line,
            Column = stored.Column,
            TokenIndex = tokenIndex
        };
        if (stored.Type == EOF) token.Text = "<EOF>";
        return token;
    }

    /// <summary>
    /// Lex the next token directly into compact storage and return its index.
    /// This is the allocation-free production path used by complete and
    /// context-aware tokenization.
    /// </summary>
    public int NextToken(
        TokenStore tokens, Cursor cursor,
        IReadOnlySet<int> expectedTokenTypes = null)
    {
        EnsureInput();
        if (tokens == null) throw new ArgumentNullException(nameof(tokens));
        if (cursor == null) throw new ArgumentNullException(nameof(cursor));
        if (cursor.Position == _input.Length)
        {
            int index = tokens.Add(
                EOF, DEFAULT_CHANNEL, cursor.Position, cursor.Position - 1,
                cursor.Line, cursor.Column, "<EOF>");
            cursor.TokenIndex++;
            // Move beyond the sentinel so accidental repeated calls are visible.
            cursor.Position++;
            return index;
        }
        if (cursor.Position > _input.Length)
            throw new InvalidOperationException("The lexer cursor is past EOF.");

        int start = cursor.Position;
        int startLine = cursor.Line;
        int startColumn = cursor.Column;
        int startMode = cursor.Mode;
        bool collectDiagnostics = RecordStatistics && Statistics != null;
        var diagnostics = collectDiagnostics ? new MatchDiagnostics() : null;
        var match = MatchNextToken(
            start, cursor.Mode, expectedTokenTypes, diagnostics);
        int matchedRule = match.RuleIndex;
        int matchEnd = match.EndPosition;

        if (matchedRule < 0)
            throw new InvalidOperationException(
                $"Lexer error at line {startLine}:{startColumn}: no rule matches '{_input[start]}' (U+{(int)_input[start]:X4}).");

        int tokenType = _atn.ruleToTokenType[matchedRule];
        int channel = DEFAULT_CHANNEL;
        bool skip = false;

        // Most lexer rules have no commands. Avoid resolving and enumerating an
        // empty action collection on the production path.
        var actions = match.ActionBits == 0
            ? null
            : ResolveActions(match.ActionBits);
        if (actions != null)
        {
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
        }

        int tokenIndex = tokens.Add(
            tokenType, skip ? LexerToken.SKIP_CHANNEL : channel,
            start, matchEnd - 1, startLine, startColumn);
        cursor.TokenIndex++;
        int line = cursor.Line, column = cursor.Column;
        UpdateLineCol(start, matchEnd, ref line, ref column);
        cursor.Line = line;
        cursor.Column = column;
        cursor.Position = matchEnd;
        if (diagnostics != null)
            Statistics.Record(
                start, startLine, startColumn, startMode,
                diagnostics.Candidates, diagnostics.OrdinaryWinner,
                diagnostics.SelectedWinner, expectedTokenTypes,
                diagnostics.UsedContextFallback);
        return tokenIndex;
    }

    [Obsolete("Bind input with SetInput and call NextToken without repeated session arguments.")]
    public LexerToken NextToken(
        string input, Cursor cursor, IReadOnlySet<int> expectedTokenTypes = null,
        bool recordStatistics = true)
    {
        SetInput(input);
        bool previous = RecordStatistics;
        RecordStatistics = recordStatistics;
        try { return NextToken(cursor, expectedTokenTypes); }
        finally { RecordStatistics = previous; }
    }

    private void EnsureInput()
    {
        if (_input == null)
            throw new InvalidOperationException(
                "No lexer input is bound. Call SetInput or Tokenize first.");
    }

    /// <summary>
    /// The production lexer returns only the values needed to emit a token.
    /// Candidate lists and winner objects are populated separately, and only
    /// when lexer statistics have explicitly been requested.
    /// </summary>
    private readonly record struct TokenMatch(
        int RuleIndex, int EndPosition, int ActionBits);

    private sealed class MatchDiagnostics
    {
        public IReadOnlyList<LexerCandidate> Candidates;
        public LexerCandidate OrdinaryWinner;
        public LexerCandidate SelectedWinner;
        public bool UsedContextFallback;
    }

    private TokenMatch MatchNextToken(
        int startPos, int mode,
        IReadOnlySet<int> expectedTokenTypes = null,
        MatchDiagnostics diagnostics = null)
    {
        if (mode < 0 || mode >= _atn.modeToStartState.Length)
            return EmptyMatch(startPos);
        var current = GetModeStartState(mode);
        int pos = startPos;
        int bestRule = -1, bestEnd = -1, bestActions = 0;
        int expectedRule = -1, expectedEnd = -1, expectedActions = 0;
        Dictionary<int, (int End, int Actions)> acceptedRules = diagnostics != null
            ? new()
            : null;

        CheckAccepts(current, pos, expectedTokenTypes,
            ref bestRule, ref bestEnd, ref bestActions,
            ref expectedRule, ref expectedEnd, ref expectedActions,
            acceptedRules);

        while (pos < _input.Length)
        {
            int ch = _input[pos];
            var next = GetTargetState(current, ch);
            if (next == null) break;
            pos++;
            current = next;
            pos = ConsumeKnownAsciiSelfLoop(current, pos);
            CheckAccepts(current, pos, expectedTokenTypes,
                ref bestRule, ref bestEnd, ref bestActions,
                ref expectedRule, ref expectedEnd, ref expectedActions,
                acceptedRules);
        }

        // EOF is a real lexer-ATN symbol. It does not consume a character, but
        // rules such as line comments commonly use (... | EOF) to terminate at
        // the end of a file that has no trailing newline.
        if (pos == _input.Length)
        {
            var eof = GetTargetState(current, EOF);
            if (eof != null)
            {
                CheckAccepts(eof, pos, expectedTokenTypes,
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

        if (diagnostics != null)
        {
            diagnostics.Candidates = acceptedRules
                .Where(entry => entry.Value.End > startPos)
                .Select(entry => CreateCandidate(
                    startPos, entry.Key,
                    entry.Value.End, entry.Value.Actions))
                .OrderBy(candidate => candidate.RuleIndex)
                .ToArray();
            diagnostics.OrdinaryWinner = CreateCandidate(
                startPos, ordinaryRule, ordinaryEnd, ordinaryActions);
            diagnostics.SelectedWinner = CreateCandidate(
                startPos, bestRule, bestEnd, bestActions);
            diagnostics.UsedContextFallback = usedContextFallback;
        }
        return new TokenMatch(bestRule, bestEnd, bestActions);
    }

    private static TokenMatch EmptyMatch(int startPosition)
        => new(-1, startPosition, 0);

    private void CheckAccepts(
        DfaState state, int pos,
        IReadOnlySet<int> expectedTokenTypes,
        ref int bestRule, ref int bestEnd, ref int bestActions,
        ref int expectedRule, ref int expectedEnd, ref int expectedActions,
        Dictionary<int, (int End, int Actions)> acceptedRules)
    {
        foreach (var acceptConfig in state.Accepts)
        {
            int ri = acceptConfig.Rule;
            int actions = acceptConfig.Actions;
            if (acceptedRules != null &&
                (!acceptedRules.TryGetValue(ri, out var accepted) || pos > accepted.End))
                acceptedRules[ri] = (pos, actions);
            // Longer match wins; tie-break by earlier rule index.
            if (pos > bestEnd || (pos == bestEnd && ri < bestRule))
            {
                bestRule = ri;
                bestEnd = pos;
                bestActions = actions;
            }
            if (expectedTokenTypes != null &&
                IsContextEligible(ri, actions, expectedTokenTypes) &&
                (pos > expectedEnd || (pos == expectedEnd && ri < expectedRule)))
            {
                expectedRule = ri;
                expectedEnd = pos;
                expectedActions = actions;
            }
        }
    }

    private LexerCandidate CreateCandidate(
        int startPosition, int ruleIndex,
        int endPosition, int actionBits)
    {
        var (tokenType, channel, skip) = ResolveDisposition(ruleIndex, actionBits);
        return new LexerCandidate(
            ruleIndex, tokenType, endPosition, channel, skip,
            endPosition >= startPosition
                ? _input.Substring(startPosition, endPosition - startPosition)
                : "");
    }

    private IReadOnlyList<IMyLexerAction> ResolveActions(int actionBits)
    {
        if (actionBits == 0) return Array.Empty<IMyLexerAction>();
        if (_actionSets.TryGetValue(actionBits, out var cached)) return cached;
        var actions = new List<IMyLexerAction>();
        for (int i = 0; i < _atn.lexerActions.Length && i < 32; i++)
            if ((actionBits & (1 << i)) != 0)
                actions.Add(_atn.lexerActions[i]);
        cached = actions.ToArray();
        _actionSets.Add(actionBits, cached);
        return cached;
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
        var next = _enableDfa
            ? NewDfaConfigSet()
            : new HashSet<LexerConfig>(LexerConfigEq.Instance);
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
                        NextNonGreedyBranch(c, transitionIndex),
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
        var work = _closureWork;
        work.Clear();
        foreach (var c in configs) work.Push(c);

        while (work.Count > 0)
        {
            var c = work.Pop();

            // Completion: at RuleStop with non-empty stack → pop and continue
            if (c.State.stateType == MyStateType.RuleStop && !c.Stack.IsEmpty)
            {
                var (ret, rest) = c.Stack.Pop();
                bool exitedNonGreedyOwner = c.CompletedInnerRule &&
                    c.NonGreedyDecision >= 0 &&
                    c.Stack.Id != c.NonGreedyContext.Id;
                var next = new LexerConfig(
                    ret, rest, c.Actions, c.OuterRule,
                    exitedNonGreedyOwner ? -1 : c.NonGreedyDecision,
                    exitedNonGreedyOwner ? LexStack.Empty : c.NonGreedyContext,
                    exitedNonGreedyOwner ? -1 : c.NonGreedyBranch,
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
                            NextNonGreedyBranch(c, transitionIndex),
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
                            NextNonGreedyBranch(c, transitionIndex),
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
                            NextNonGreedyBranch(c, transitionIndex),
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

    private static int NextNonGreedyBranch(
        LexerConfig config, int transitionIndex) =>
        config.State.transitions.Count > 1 && config.State.nonGreedy
            ? transitionIndex
            : config.NonGreedyBranch;

    private void UpdateLineCol(int from, int to, ref int line, ref int col)
    {
        for (int i = from; i < to; i++)
        {
            if (_input[i] == '\n') { line++; col = 0; }
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
        public readonly int NonGreedyBranch;
        // True when the most recently consumed character completed any
        // referenced rule. This preserves a higher-priority fragment path
        // when a wildcard path can accept at the same position.
        public readonly bool CompletedInnerRule;
        public LexerConfig(MyATNState state, LexStack stack, int actions,
                           int outerRule, int nonGreedyDecision,
                           LexStack nonGreedyContext,
                           int nonGreedyBranch,
                           bool completedInnerRule)
        {
            State = state;
            Stack = stack;
            Actions = actions;
            OuterRule = outerRule;
            NonGreedyDecision = nonGreedyDecision;
            NonGreedyContext = nonGreedyContext;
            NonGreedyBranch = nonGreedyBranch;
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
               x.NonGreedyBranch == y.NonGreedyBranch &&
               x.CompletedInnerRule == y.CompletedInnerRule;

        public int GetHashCode(LexerConfig c)
        {
            unchecked
            {
                int hash = c.State.stateNumber * 31 + c.Stack.GetHashCode();
                hash = hash * 31 + c.OuterRule;
                hash = hash * 31 + c.NonGreedyDecision;
                hash = hash * 31 + c.NonGreedyContext.GetHashCode();
                hash = hash * 31 + c.NonGreedyBranch;
                hash = hash * 31 + (c.CompletedInnerRule ? 1 : 0);
                return hash;
            }
        }
    }

    private readonly record struct NonGreedyAccept(
        int Rule, int Decision, LexStack Context, int Branch);

    private sealed class NonGreedyAcceptEq : IEqualityComparer<NonGreedyAccept>
    {
        public static readonly NonGreedyAcceptEq Instance = new();

        public bool Equals(NonGreedyAccept x, NonGreedyAccept y) =>
            x.Rule == y.Rule && x.Decision == y.Decision &&
            x.Context.Id == y.Context.Id && x.Branch == y.Branch;

        public int GetHashCode(NonGreedyAccept value) => HashCode.Combine(
            value.Rule, value.Decision, value.Context.Id, value.Branch);
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
