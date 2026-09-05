namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

// Core ALL(*) prediction engine.
// Implements SLL prediction (context-free) with full-LL fallback.

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

public sealed class AllStarSimulator
{
    private readonly MyATN _atn;
    private readonly ParserStatistics _statistics;
    private readonly ParserPredictionCache _sharedCache;

    // Reusable scratch buffers for Closure — cleared at the start of each use so
    // behaviour is identical to allocating fresh collections, but without the
    // per-call allocation cost on hot prediction paths.
    private readonly Stack<ATNConfig> _closureStack = new();
    private readonly HashSet<(int stateNum, int alt, PredictionContext context, int precedence)> _closureBusy = new();
    private readonly Dictionary<(PredictionContext parent, int returnState, int precedence), SingletonPredictionContext> _contextCache;
    private readonly Dictionary<(int decision, int precedence), DecisionDfa> _decisionDfas;

    public AllStarSimulator(
        MyATN atn, ParserStatistics statistics = null,
        ParserPredictionCache predictionCache = null)
    {
        _atn = atn;
        _statistics = statistics;
        if (predictionCache != null)
        {
            predictionCache.Bind(atn);
            if (predictionCache.SharingEnabled)
                _sharedCache = predictionCache;
        }
        _decisionDfas = _sharedCache?.DecisionDfas ?? new();
        _contextCache = _sharedCache?.Contexts ?? new();
        if (_statistics != null && _sharedCache != null)
        {
            _statistics.SharedDfaStatesAtStart = _sharedCache.RetainedStates;
            _statistics.SharedDfaTransitionsAtStart =
                _sharedCache.RetainedTransitions;
        }
    }

    /// <summary>
    /// Compute the parser terminals reachable from a state without consuming a
    /// token. This is the valid-lookahead set supplied to context-aware lexing.
    /// </summary>
    public HashSet<int> GetExpectedTokenTypes(
        MyATNState state, PredictionContext callerCtx, int precedence)
    {
        if (_sharedCache != null)
        {
            lock (_sharedCache.SyncRoot)
                return GetExpectedTokenTypesCore(state, callerCtx, precedence);
        }
        return GetExpectedTokenTypesCore(state, callerCtx, precedence);
    }

    private HashSet<int> GetExpectedTokenTypesCore(
        MyATNState state, PredictionContext callerCtx, int precedence)
    {
        _closureBusy.Clear();
        var configs = new ATNConfigSet(_statistics);
        int precedenceRuleIndex = state.isPrecedenceDecision
            ? state.ruleIndex
            : -1;
        Closure(new ATNConfig(state, 1, callerCtx, precedence), configs,
            fullCtx: true, precedence, precedenceRuleIndex);

        var result = new HashSet<int>();
        foreach (var config in configs.Configs)
        {
            foreach (var transition in config.State.transitions)
            {
                if (!IsTerminal(transition)) continue;
                if (transition.Matches(-1, 0, _atn.maxTokenType))
                    result.Add(-1);
                for (int tokenType = 1; tokenType <= _atn.maxTokenType; tokenType++)
                    if (transition.Matches(tokenType, 1, _atn.maxTokenType))
                        result.Add(tokenType);
            }
        }
        return result;
    }

    // Returns the predicted alternative (1-indexed) for the given decision.
    // tokenTypes: on-channel token types starting at startPos.
    // callerCtx:  the PredictionContext of the rule that contains this decision (for LL fallback).
    public int AdaptivePredict(int decision, int[] tokenTypes, int startPos,
                               PredictionContext callerCtx, int precedence)
    {
        if (_sharedCache != null)
        {
            lock (_sharedCache.SyncRoot)
                return AdaptivePredictCore(
                    decision, tokenTypes, startPos, callerCtx, precedence);
        }
        return AdaptivePredictCore(
            decision, tokenTypes, startPos, callerCtx, precedence);
    }

    private int AdaptivePredictCore(
        int decision, int[] tokenTypes, int startPos,
        PredictionContext callerCtx, int precedence)
    {
        long lookaheadAtStart = _statistics?.BeginPrediction(decision) ?? 0;
        try
        {
            var decisionState = _atn.decisionToState[decision];
            int n = decisionState.transitions.Count;
            if (n == 1) return 1; // trivial

            bool isLoop = decisionState.stateType == MyStateType.StarLoopEntry ||
                          decisionState.stateType == MyStateType.PlusLoopBack;
            int precedenceRuleIndex = decisionState.isPrecedenceDecision
                ? decisionState.ruleIndex
                : -1;

            // SLL uses a local prediction-context stack rooted at EMPTY. Conflicts
            // are not resolved here; they signal full-context LL fallback below.
            int sllAlt = ExecSllDfa(
                decision, decisionState, tokenTypes, startPos, callerCtx, precedence,
                precedenceRuleIndex);
            if (sllAlt > 0) return sllAlt;

            _statistics?.RecordFullContextFallback(decision);

            int llAlt = ExecATN(
                decisionState, tokenTypes, startPos, callerCtx, fullCtx: true,
                precedence: precedence, precedenceRuleIndex: precedenceRuleIndex);
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[SIM] dec={decision} state={decisionState.stateNumber} type={decisionState.stateType} llAlt={llAlt}");
            if (llAlt > 0) return llAlt;

            // LL also couldn't determine.
            // For non-loop decisions, take alt=1 (greedy / first alternative).
            // For loop decisions, greedily continue (alt=1) when the loop body can still
            // match the next token; otherwise exit (alt=n).  This handles the common case
            // where LL prediction fails due to context-merge approximation but the loop
            // body is clearly viable from a single-token lookahead.
            int def;
            if (isLoop)
                def = LoopBodyCanMatchToken(
                    decisionState, tokenTypes, startPos, callerCtx, precedence,
                    precedenceRuleIndex) ? 1 : n;
            else
                def = 1;
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[SIM] dec={decision} default={def} (isLoop={isLoop})");
            return def;
        }
        finally
        {
            _statistics?.EndPrediction(decision, lookaheadAtStart);
        }
    }

    // Run context-independent prediction through a per-decision DFA. DFA
    // states are canonicalized SLL ATN configuration sets; edges memoize one
    // token of reach/closure work and are reusable at every input position.
    private int ExecSllDfa(int decision, MyATNState decisionState,
                           int[] tokenTypes, int startPos, PredictionContext callerCtx,
                           int precedence, int precedenceRuleIndex)
    {
        var dfaKey = (decision, precedence);
        bool sharedDfa = _sharedCache != null;
        if (!_decisionDfas.TryGetValue(dfaKey, out DecisionDfa dfa))
        {
            dfa = new DecisionDfa();
            if (_sharedCache == null || !_sharedCache.IsSaturated)
                _decisionDfas.Add(dfaKey, dfa);
            else
                sharedDfa = false;
        }
        if (dfa.Start == null)
        {
            _closureBusy.Clear();
            var initial = new ATNConfigSet(_statistics);
            for (int i = 0; i < decisionState.transitions.Count; i++)
                Closure(new ATNConfig(decisionState.transitions[i].target, i + 1,
                                      PredictionContext.EMPTY, precedence),
                        initial, fullCtx: false, precedence, precedenceRuleIndex);
            var start = InternDfaState(dfa, initial);
            if (sharedDfa && !start.IsRetained)
            {
                // The shared budget is exhausted. Keep prediction correct with
                // an ephemeral per-call DFA rather than retaining more data.
                dfa = new DecisionDfa { Start = start };
                sharedDfa = false;
            }
            else
            {
                dfa.Start = start;
            }
        }

        DfaState state = dfa.Start;
        int pos = startPos;
        while (true)
        {
            if (state.Prediction > 0) return state.Prediction;
            if (state.RequiresFullContext || state.IsError)
            {
                if (state.RequiresFullContext && _statistics != null)
                    _statistics.SllConflicts++;
                if (AllStarParser.Trace && state.RequiresFullContext)
                    Console.Error.WriteLine($"[SLL] dec={decision} fallback=conflict");
                return -1;
            }
            if (pos >= tokenTypes.Length) return -1;

            int tokenType = tokenTypes[pos++];
            if (_statistics != null)
                _statistics.PredictionLookaheadTokens++;
            if (!state.Edges.TryGetValue(tokenType, out DfaState target))
            {
                _statistics?.RecordDfaMiss(decision);
                var reach = ComputeReachSet(
                    state.Configs, tokenType, fullCtx: false, precedence,
                    precedenceRuleIndex);
                // A completed alternative only needs caller context when the
                // same lookahead also keeps a competing path alive. If no path
                // consumes it, the completed alternative is the SLL result.
                bool stopLive = false;
                if (state.CompletedPrediction > 0)
                {
                    target = reach.IsEmpty
                        ? DfaState.Accept(state.CompletedPrediction)
                        : InternDfaState(dfa, reach);
                    stopLive = !reach.IsEmpty;
                }
                else
                    target = reach.IsEmpty ? DfaState.Error : InternDfaState(dfa, reach);
                bool retainEdge = !sharedDfa ||
                    (target.IsRetained && _sharedCache.TryRetainTransition());
                if (retainEdge)
                {
                    state.Edges[tokenType] = target;
                    if (stopLive)
                        state.StopLiveEdges.Add(tokenType);
                }
            }
            else
            {
                _statistics?.RecordDfaHit(decision);
            }
            if (state.StopLiveEdges.Contains(tokenType) &&
                CallerCanMatchToken(
                    callerCtx, tokenType, state.CompletedPrediction, precedence,
                    precedenceRuleIndex))
            {
                if (AllStarParser.Trace)
                    Console.Error.WriteLine($"[SLL] dec={decision} fallback=stop-live tok={tokenType}");
                return -1;
            }
            state = target;
        }
    }

    private bool CallerCanMatchToken(PredictionContext callerCtx, int tokenType,
                                     int alt, int precedence,
                                     int precedenceRuleIndex)
    {
        if (callerCtx.IsEmpty) return false;
        int returnState = callerCtx.ReturnState;
        if (returnState == PredictionContext.EMPTY_RETURN_STATE) return false;

        _closureBusy.Clear();
        var continuation = new ATNConfigSet(_statistics);
        Closure(new ATNConfig(_atn.allStates[returnState], alt, callerCtx.Parent,
                              callerCtx.GetPrecedence(0)),
                continuation, fullCtx: true, precedence,
                precedenceRuleIndex);
        foreach (var config in continuation.Configs)
            foreach (var transition in config.State.transitions)
                if (IsTerminal(transition) &&
                    transition.Matches(tokenType, 0, _atn.maxTokenType))
                    return true;
        return false;
    }

    private DfaState InternDfaState(DecisionDfa dfa, ATNConfigSet configs)
    {
        var key = new ConfigSetKey(configs);
        if (dfa.States.TryGetValue(key, out DfaState existing))
        {
            if (_statistics != null)
                _statistics.DfaStatesDeduplicated++;
            return existing;
        }

        var state = new DfaState(configs);
        int unique = configs.GetUniqueAlt();
        if (unique > 0)
            state.Prediction = unique;
        else if (configs.GetAllSubsetsConflictAlt() > 0)
            state.RequiresFullContext = true;
        state.CompletedPrediction = configs.GetCompletedAlt();
        if (_sharedCache == null ||
            _sharedCache.TryRetainState(configs.Configs.Count))
        {
            state.IsRetained = true;
            dfa.States.Add(key, state);
        }
        if (_statistics != null)
            _statistics.DfaStatesCreated++;
        return state;
    }

    private int ExecATN(MyATNState decisionState, int[] tokenTypes, int startPos,
                        PredictionContext baseCtx, bool fullCtx,
                        bool resolveConflicts = true, int precedence = 0,
                        int precedenceRuleIndex = -1)
    {
        _closureBusy.Clear();
        var initial = new ATNConfigSet(_statistics);

        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            var cfg = new ATNConfig(target, i + 1, baseCtx, precedence);
            Closure(cfg, initial, fullCtx, precedence, precedenceRuleIndex);
        }

        int alt = initial.GetUniqueAlt();
        if (alt != -1) return alt;
        // A stop configuration competing with another live alternative is
        // context-sensitive in SLL: EMPTY cannot show whether the completed
        // path continues through the caller. Retry it with the real context.
        // Full LL must not accept the stop here; Closure has already followed
        // its return states and the normal conflict analysis decides the alt.
        if (!resolveConflicts && initial.GetCompletedAlt() != -1)
            return -1;
        if (resolveConflicts)
        {
            alt = initial.GetExactAmbiguityAlt();
            if (alt != -1) return alt;
        }
        else if (initial.GetAllSubsetsConflictAlt() != -1)
        {
            return -1;
        }

        var current = initial;
        int pos = startPos;

        while (pos < tokenTypes.Length)
        {
            int tokenType = tokenTypes[pos++];
            if (fullCtx && _statistics != null)
            {
                _statistics.PredictionLookaheadTokens++;
                _statistics.FullContextLookaheadTokens++;
            }
            var reach = ComputeReachSet(
                current, tokenType, fullCtx, precedence, precedenceRuleIndex);
            if (reach.IsEmpty) break;

            alt = reach.GetUniqueAlt();
            if (alt != -1) return alt;
            if (!resolveConflicts && reach.GetCompletedAlt() != -1)
                return -1;
            if (resolveConflicts)
            {
                alt = reach.GetExactAmbiguityAlt();
                if (alt != -1)
                    return alt;
            }
            else if (reach.GetAllSubsetsConflictAlt() != -1)
            {
                return -1;
            }

            current = reach;
        }

        // Exhausted lookahead without finding a unique alt.
        // In full-LL mode, check for alts that completed their path and entered the outer
        // prediction context (their context stack is shallower than baseCtx due to RuleStop
        // popping).  The minimum such alt has "accepted" — return it.
        if (fullCtx)
        {
            int baseDepth = ContextDepth(baseCtx);
            int minAccepted = -1;
            foreach (var c in current.Configs)
            {
                if (ContextDepth(c.Context) < baseDepth)
                {
                    if (minAccepted < 0 || c.Alt < minAccepted)
                        minAccepted = c.Alt;
                }
            }
            if (minAccepted >= 0) return minAccepted;

            // No config popped past the outer context.  Take the minimum alt that survived
            // the furthest in the lookahead — the standard ALL(*) / ANTLR4 ambiguity tiebreak.
            int minSurviving = -1;
            foreach (var c in current.Configs)
            {
                if (minSurviving < 0 || c.Alt < minSurviving)
                    minSurviving = c.Alt;
            }
            if (minSurviving >= 0) return minSurviving;
        }
        return -1;
    }

    private ATNConfigSet ComputeReachSet(ATNConfigSet configs, int tokenType,
                                         bool fullCtx, int precedence,
                                         int precedenceRuleIndex = -1)
    {
        var reach = new ATNConfigSet(_statistics);
        foreach (var cfg in configs.Configs)
        {
            if (_statistics != null)
                _statistics.ReachConfigurationsExamined++;
            foreach (var tr in cfg.State.transitions)
            {
                if (IsTerminal(tr) && tr.Matches(tokenType, 0, _atn.maxTokenType))
                    reach.Add(cfg.WithState(tr.target));
            }
        }

        _closureBusy.Clear();
        var closed = new ATNConfigSet(_statistics);
        foreach (var c in reach.Configs)
            Closure(c, closed, fullCtx, precedence, precedenceRuleIndex);
        return closed;
    }

    private void Closure(ATNConfig seed, ATNConfigSet configs, bool fullCtx,
                         int precedence, int precedenceRuleIndex)
    {
        _closureStack.Clear();
        _closureStack.Push(seed);

        while (_closureStack.Count > 0)
        {
            var config = _closureStack.Pop();
            if (_statistics != null)
                _statistics.ClosureConfigurationsVisited++;

            var key = (config.State.stateNumber, config.Alt, config.Context,
                       config.Precedence);
            if (!_closureBusy.Add(key)) continue;

            if (config.State.stateType == MyStateType.RuleStop)
            {
                configs.Add(config);
                if (config.Context.IsEmpty) continue; // prediction boundary — done

                // An array context represents several call stacks merged at
                // this configuration. Follow every return edge.
                for (int i = 0; i < config.Context.Size; i++)
                {
                    int returnStateNum = config.Context.GetReturnState(i);
                    if (returnStateNum == PredictionContext.EMPTY_RETURN_STATE) continue;
                    var returnState = _atn.allStates[returnStateNum];
                    _closureStack.Push(new ATNConfig(
                        returnState, config.Alt, config.Context.GetParent(i),
                        config.Context.GetPrecedence(i)));
                }
                continue;
            }

            bool hasTerminalTransition = false;
            foreach (var tr in config.State.transitions)
            {
                ATNConfig next = null;
                switch (tr)
                {
                    case MyEpsilonTransition:
                    case MyActionTransition:
                    case MyPredicateTransition:
                        next = config.WithState(tr.target);
                        break;

                    case MyPrecedencePredicateTransition pt:
                        if (config.State.ruleIndex != precedenceRuleIndex ||
                            pt.precedence >= config.Precedence)
                            next = config.WithState(pt.target);
                        break;

                    case MyRuleTransition rt:
                        // Both SLL and LL need local rule-return frames. SLL is
                        // rooted at EMPTY; LL is rooted in the caller context.
                        // Tail calls inherit the existing context.
                        PredictionContext newCtx = !rt.isTailCall
                            ? GetChildContext(config.Context, rt.target.stateNumber,
                                              config.Precedence)
                            : config.Context;
                        next = new ATNConfig(_atn.start[rt.ruleIndex], config.Alt,
                                             newCtx, rt.precedence);
                        break;
                    // Terminal transitions form the closure frontier and are
                    // followed later by ComputeReachSet.
                    default:
                        if (IsTerminal(tr))
                            hasTerminalTransition = true;
                        break;
                }
                if (next != null) _closureStack.Push(next);
            }

            if (hasTerminalTransition)
                configs.Add(config);
        }
    }

    // Returns true when any alt=1 (loop-body) config in the initial LL closure has a
    // terminal transition that matches the token at startPos.  Used to implement greedy
    // loop semantics: continue the loop iff the body can fire on the next token.
    private bool LoopBodyCanMatchToken(MyATNState decisionState, int[] tokenTypes,
                                       int startPos, PredictionContext callerCtx,
                                       int precedence, int precedenceRuleIndex)
    {
        if (startPos >= tokenTypes.Length) return false;
        int tok = tokenTypes[startPos];

        _closureBusy.Clear();
        var initial = new ATNConfigSet(_statistics);
        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            Closure(new ATNConfig(target, i + 1, callerCtx, precedence), initial,
                    fullCtx: true, precedence, precedenceRuleIndex);
        }

        foreach (var c in initial.Configs)
        {
            if (c.Alt != 1) continue;
            foreach (var tr in c.State.transitions)
                if (IsTerminal(tr) && tr.Matches(tok, 0, _atn.maxTokenType))
                    return true;
        }
        return false;
    }

    private static bool IsTerminal(MyTransition t) =>
        t is MyAtomTransition || t is MySetTransition || t is MyNotSetTransition ||
        t is MyWildcardTransition || t is MyRangeTransition;

    private SingletonPredictionContext GetChildContext(PredictionContext parent,
                                                        int returnState,
                                                        int precedence)
    {
        var key = (parent, returnState, precedence);
        if (!_contextCache.TryGetValue(key, out var context))
        {
            context = new SingletonPredictionContext(parent, returnState, precedence);
            if (_sharedCache == null || _sharedCache.TryRetainContext())
                _contextCache.Add(key, context);
            if (_statistics != null)
                _statistics.PredictionContextCreations++;
        }
        else if (_statistics != null)
            _statistics.PredictionContextCacheHits++;
        return context;
    }

    internal void CaptureRetainedStatistics()
    {
        if (_sharedCache != null)
        {
            lock (_sharedCache.SyncRoot)
                CaptureRetainedStatisticsCore();
            return;
        }
        CaptureRetainedStatisticsCore();
    }

    private void CaptureRetainedStatisticsCore()
    {
        if (_statistics == null) return;
        int states = 0;
        int transitions = 0;
        long configurationSlots = 0;
        foreach (var dfa in _decisionDfas.Values)
        {
            states += dfa.States.Count;
            foreach (var state in dfa.States.Values)
            {
                transitions += state.Edges.Count;
                configurationSlots += state.Configs?.Configs.Count ?? 0;
            }
        }
        _statistics.RetainedDfaStates = states;
        _statistics.RetainedDfaTransitions = transitions;
        _statistics.RetainedPredictionContexts = _contextCache.Count;
        _statistics.EstimatedRetainedBytes =
            _sharedCache?.EstimatedRetainedBytes ??
            states * 96L + transitions * 32L + configurationSlots * 64L +
            _contextCache.Count * 48L;
        _statistics.SharedDfaCacheSaturated =
            _sharedCache?.IsSaturated ?? false;
    }

    // Number of frames in the context chain (0 for EMPTY).
    private static int ContextDepth(PredictionContext ctx)
    {
        if (ctx.IsEmpty) return 0;
        int minimum = int.MaxValue;
        for (int i = 0; i < ctx.Size; i++)
        {
            if (ctx.GetReturnState(i) == PredictionContext.EMPTY_RETURN_STATE)
                return 0;
            minimum = Math.Min(minimum, 1 + ContextDepth(ctx.GetParent(i)));
        }
        return minimum;
    }

    internal sealed class DecisionDfa
    {
        public DfaState Start;
        public readonly Dictionary<ConfigSetKey, DfaState> States = new();
    }

    internal sealed class DfaState
    {
        public static readonly DfaState Error = new(null)
        {
            IsError = true,
            IsRetained = true
        };
        public readonly ATNConfigSet Configs;
        public readonly Dictionary<int, DfaState> Edges = new();
        public readonly HashSet<int> StopLiveEdges = new();
        public int Prediction;
        public int CompletedPrediction;
        public bool RequiresFullContext;
        public bool IsError;
        public bool IsRetained;

        public DfaState(ATNConfigSet configs) => Configs = configs;

        public static DfaState Accept(int prediction) =>
            new(null) { Prediction = prediction, IsRetained = true };
    }

    internal sealed class ConfigSetKey : IEquatable<ConfigSetKey>
    {
        private readonly HashSet<(int state, int alt, PredictionContext context, int precedence)> _items;
        private readonly int _hash;

        public ConfigSetKey(ATNConfigSet configs)
        {
            _items = new HashSet<(int, int, PredictionContext, int)>();
            int hash = 0;
            foreach (var c in configs.Configs)
            {
                var item = (c.State.stateNumber, c.Alt, c.Context, c.Precedence);
                _items.Add(item);
                hash ^= HashCode.Combine(item.stateNumber, item.Alt, item.Context,
                                         item.Precedence);
            }
            _hash = HashCode.Combine(hash, _items.Count);
        }

        public bool Equals(ConfigSetKey other) =>
            other != null && _hash == other._hash && _items.SetEquals(other._items);

        public override bool Equals(object obj) => obj is ConfigSetKey other && Equals(other);
        public override int GetHashCode() => _hash;
    }
}
