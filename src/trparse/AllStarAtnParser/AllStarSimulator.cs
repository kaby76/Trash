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

    // Reusable scratch buffers for Closure — cleared at the start of each use so
    // behaviour is identical to allocating fresh collections, but without the
    // per-call allocation cost on hot prediction paths.
    private readonly Stack<ATNConfig> _closureStack = new();
    private readonly HashSet<(int stateNum, int alt, PredictionContext context)> _closureBusy = new();
    private readonly Dictionary<(PredictionContext parent, int returnState), SingletonPredictionContext> _contextCache = new();
    private readonly Dictionary<(int decision, int precedence), DecisionDfa> _decisionDfas = new();

    public AllStarSimulator(MyATN atn)
    {
        _atn = atn;
    }

    // Returns the predicted alternative (1-indexed) for the given decision.
    // tokenTypes: on-channel token types starting at startPos.
    // callerCtx:  the PredictionContext of the rule that contains this decision (for LL fallback).
    public int AdaptivePredict(int decision, int[] tokenTypes, int startPos,
                               PredictionContext callerCtx, int precedence)
    {
        var decisionState = _atn.decisionToState[decision];
        int n = decisionState.transitions.Count;
        if (n == 1) return 1; // trivial

        bool isLoop = decisionState.stateType == MyStateType.StarLoopEntry ||
                      decisionState.stateType == MyStateType.PlusLoopBack;
        int precedenceRuleIndex = decisionState.isPrecedenceDecision
            ? decisionState.ruleIndex
            : -1;

        // Precedence belongs to the current rule invocation. Enforce it when
        // the parser actually reaches the left-recursive suffix decision,
        // rather than leaking it into speculative calls of the same rule in
        // subqueries or parentheses.
        if (decisionState.isPrecedenceDecision)
        {
            for (int i = 0; i < decisionState.transitions.Count; i++)
            {
                var target = decisionState.transitions[i].target;
                if (target.stateType != MyStateType.LoopEnd &&
                    !PrecedencePathAllowed(target, precedence))
                {
                    for (int exit = 0; exit < decisionState.transitions.Count; exit++)
                        if (decisionState.transitions[exit].target.stateType == MyStateType.LoopEnd)
                            return exit + 1;
                }
            }
        }

        // SLL uses a local prediction-context stack rooted at EMPTY. Conflicts
        // are not resolved here; they signal full-context LL fallback below.
        int sllAlt = ExecSllDfa(
            decision, decisionState, tokenTypes, startPos, callerCtx, precedence,
            precedenceRuleIndex);
        if (sllAlt > 0) return sllAlt;

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

    // Run context-independent prediction through a per-decision DFA. DFA
    // states are canonicalized SLL ATN configuration sets; edges memoize one
    // token of reach/closure work and are reusable at every input position.
    private int ExecSllDfa(int decision, MyATNState decisionState,
                           int[] tokenTypes, int startPos, PredictionContext callerCtx,
                           int precedence, int precedenceRuleIndex)
    {
        var dfaKey = (decision, precedence);
        if (!_decisionDfas.TryGetValue(dfaKey, out DecisionDfa dfa))
        {
            dfa = new DecisionDfa();
            _decisionDfas.Add(dfaKey, dfa);
        }
        if (dfa.Start == null)
        {
            _closureBusy.Clear();
            var initial = new ATNConfigSet();
            for (int i = 0; i < decisionState.transitions.Count; i++)
                Closure(new ATNConfig(decisionState.transitions[i].target, i + 1,
                                      PredictionContext.EMPTY),
                        initial, fullCtx: false, precedence, precedenceRuleIndex);
            dfa.Start = InternDfaState(dfa, initial);
        }

        DfaState state = dfa.Start;
        int pos = startPos;
        while (true)
        {
            if (state.Prediction > 0) return state.Prediction;
            if (state.RequiresFullContext || state.IsError)
            {
                if (AllStarParser.Trace && state.RequiresFullContext)
                    Console.Error.WriteLine($"[SLL] dec={decision} fallback=conflict");
                return -1;
            }
            if (pos >= tokenTypes.Length) return -1;

            int tokenType = tokenTypes[pos++];
            if (!state.Edges.TryGetValue(tokenType, out DfaState target))
            {
                var reach = ComputeReachSet(
                    state.Configs, tokenType, fullCtx: false, precedence,
                    precedenceRuleIndex);
                // A completed alternative only needs caller context when the
                // same lookahead also keeps a competing path alive. If no path
                // consumes it, the completed alternative is the SLL result.
                if (state.CompletedPrediction > 0)
                {
                    target = reach.IsEmpty
                        ? DfaState.Accept(state.CompletedPrediction)
                        : InternDfaState(dfa, reach);
                    if (!reach.IsEmpty)
                        state.StopLiveEdges.Add(tokenType);
                }
                else
                    target = reach.IsEmpty ? DfaState.Error : InternDfaState(dfa, reach);
                state.Edges[tokenType] = target;
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
        var continuation = new ATNConfigSet();
        Closure(new ATNConfig(_atn.allStates[returnState], alt, callerCtx.Parent),
                continuation, fullCtx: true, precedence,
                precedenceRuleIndex);
        foreach (var config in continuation.Configs)
            foreach (var transition in config.State.transitions)
                if (IsTerminal(transition) &&
                    transition.Matches(tokenType, 0, _atn.maxTokenType))
                    return true;
        return false;
    }

    private static DfaState InternDfaState(DecisionDfa dfa, ATNConfigSet configs)
    {
        var key = new ConfigSetKey(configs);
        if (dfa.States.TryGetValue(key, out DfaState existing))
            return existing;

        var state = new DfaState(configs);
        int unique = configs.GetUniqueAlt();
        if (unique > 0)
            state.Prediction = unique;
        else if (configs.GetAllSubsetsConflictAlt() > 0)
            state.RequiresFullContext = true;
        state.CompletedPrediction = configs.GetCompletedAlt();
        dfa.States.Add(key, state);
        return state;
    }

    private int ExecATN(MyATNState decisionState, int[] tokenTypes, int startPos,
                        PredictionContext baseCtx, bool fullCtx,
                        bool resolveConflicts = true, int precedence = 0,
                        int precedenceRuleIndex = -1)
    {
        _closureBusy.Clear();
        var initial = new ATNConfigSet();

        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            var cfg = new ATNConfig(target, i + 1, baseCtx);
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
        var reach = new ATNConfigSet();
        foreach (var cfg in configs.Configs)
        {
            foreach (var tr in cfg.State.transitions)
            {
                if (IsTerminal(tr) && tr.Matches(tokenType, 0, _atn.maxTokenType))
                    reach.Add(cfg.WithState(tr.target));
            }
        }

        _closureBusy.Clear();
        var closed = new ATNConfigSet();
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

            var key = (config.State.stateNumber, config.Alt, config.Context);
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
                        returnState, config.Alt, config.Context.GetParent(i)));
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
                            pt.precedence >= precedence)
                            next = config.WithState(pt.target);
                        break;

                    case MyRuleTransition rt:
                        // Both SLL and LL need local rule-return frames. SLL is
                        // rooted at EMPTY; LL is rooted in the caller context.
                        // Tail calls inherit the existing context.
                        PredictionContext newCtx = !rt.isTailCall
                            ? GetChildContext(config.Context, rt.target.stateNumber)
                            : config.Context;
                        next = new ATNConfig(_atn.start[rt.ruleIndex], config.Alt, newCtx);
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
        var initial = new ATNConfigSet();
        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            Closure(new ATNConfig(target, i + 1, callerCtx), initial,
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

    private static bool PrecedencePathAllowed(MyATNState start, int precedence)
    {
        var work = new Stack<MyATNState>();
        var seen = new HashSet<int>();
        work.Push(start);
        while (work.Count > 0)
        {
            var state = work.Pop();
            if (!seen.Add(state.stateNumber)) continue;
            foreach (var transition in state.transitions)
            {
                if (transition is MyPrecedencePredicateTransition predicate)
                    return predicate.precedence >= precedence;
                if (transition is MyEpsilonTransition or MyActionTransition or MyPredicateTransition)
                    work.Push(transition.target);
            }
        }
        return true;
    }

    private SingletonPredictionContext GetChildContext(PredictionContext parent, int returnState)
    {
        var key = (parent, returnState);
        if (!_contextCache.TryGetValue(key, out var context))
        {
            context = new SingletonPredictionContext(parent, returnState);
            _contextCache.Add(key, context);
        }
        return context;
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

    private sealed class DecisionDfa
    {
        public DfaState Start;
        public readonly Dictionary<ConfigSetKey, DfaState> States = new();
    }

    private sealed class DfaState
    {
        public static readonly DfaState Error = new(null) { IsError = true };
        public readonly ATNConfigSet Configs;
        public readonly Dictionary<int, DfaState> Edges = new();
        public readonly HashSet<int> StopLiveEdges = new();
        public int Prediction;
        public int CompletedPrediction;
        public bool RequiresFullContext;
        public bool IsError;

        public DfaState(ATNConfigSet configs) => Configs = configs;

        public static DfaState Accept(int prediction) =>
            new(null) { Prediction = prediction };
    }

    private sealed class ConfigSetKey : IEquatable<ConfigSetKey>
    {
        private readonly HashSet<(int state, int alt, PredictionContext context)> _items;
        private readonly int _hash;

        public ConfigSetKey(ATNConfigSet configs)
        {
            _items = new HashSet<(int, int, PredictionContext)>();
            int hash = 0;
            foreach (var c in configs.Configs)
            {
                var item = (c.State.stateNumber, c.Alt, c.Context);
                _items.Add(item);
                hash ^= HashCode.Combine(item.stateNumber, item.Alt, item.Context);
            }
            _hash = HashCode.Combine(hash, _items.Count);
        }

        public bool Equals(ConfigSetKey other) =>
            other != null && _hash == other._hash && _items.SetEquals(other._items);

        public override bool Equals(object obj) => obj is ConfigSetKey other && Equals(other);
        public override int GetHashCode() => _hash;
    }
}
