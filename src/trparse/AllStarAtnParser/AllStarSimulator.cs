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
    private readonly HashSet<(int stateNum, int alt, int ctxHash)> _closureBusy = new();

    public AllStarSimulator(MyATN atn) => _atn = atn;

    // Returns the predicted alternative (1-indexed) for the given decision.
    // tokenTypes: on-channel token types starting at startPos.
    // callerCtx:  the PredictionContext of the rule that contains this decision (for LL fallback).
    public int AdaptivePredict(int decision, int[] tokenTypes, int startPos, PredictionContext callerCtx)
    {
        var decisionState = _atn.decisionToState[decision];
        int n = decisionState.transitions.Count;
        if (n == 1) return 1; // trivial

        bool isLoop = decisionState.stateType == MyStateType.StarLoopEntry ||
                      decisionState.stateType == MyStateType.PlusLoopBack;

        // Always use full-LL prediction with the actual caller context.
        // SLL (EMPTY context) cannot follow rule-return transitions and therefore
        // misidentifies the viable alternative in grammars with complex rules like uid /
        // simpleId — it incorrectly returns the epsilon alt when a non-epsilon rule
        // alternative is the right choice.  Full-LL is slower but always correct.
        int llAlt = ExecATN(decisionState, tokenTypes, startPos, callerCtx, fullCtx: true);
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
            def = LoopBodyCanMatchToken(decisionState, tokenTypes, startPos, callerCtx) ? 1 : n;
        else
            def = 1;
        if (AllStarParser.Trace)
            Console.Error.WriteLine($"[SIM] dec={decision} default={def} (isLoop={isLoop})");
        return def;
    }

    private int ExecATN(MyATNState decisionState, int[] tokenTypes, int startPos,
                        PredictionContext baseCtx, bool fullCtx)
    {
        _closureBusy.Clear();
        var initial = new ATNConfigSet();

        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            var cfg = new ATNConfig(target, i + 1, baseCtx);
            Closure(cfg, initial, fullCtx);
        }

        int alt = initial.GetUniqueAlt();
        if (alt != -1) return alt;

        var current = initial;
        int pos = startPos;

        while (pos < tokenTypes.Length)
        {
            int tokenType = tokenTypes[pos++];
            var reach = ComputeReachSet(current, tokenType, fullCtx);
            if (reach.IsEmpty) break;

            alt = reach.GetUniqueAlt();
            if (alt != -1) return alt;

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

    private ATNConfigSet ComputeReachSet(ATNConfigSet configs, int tokenType, bool fullCtx)
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
            Closure(c, closed, fullCtx);
        return closed;
    }

    private void Closure(ATNConfig seed, ATNConfigSet configs, bool fullCtx)
    {
        _closureStack.Clear();
        _closureStack.Push(seed);

        while (_closureStack.Count > 0)
        {
            var config = _closureStack.Pop();

            var key = (config.State.stateNumber, config.Alt, config.Context.GetHashCode());
            if (!_closureBusy.Add(key)) continue;

            configs.Add(config);

            if (config.State.stateType == MyStateType.RuleStop)
            {
                if (!fullCtx) continue; // SLL: can't pop — leave config here

                // LL: pop the context stack
                if (config.Context.IsEmpty) continue; // at outermost level — done

                int returnStateNum = config.Context.ReturnState;
                PredictionContext parent = config.Context.Parent;
                if (returnStateNum == PredictionContext.EMPTY_RETURN_STATE) continue;

                var returnState = _atn.allStates[returnStateNum];
                _closureStack.Push(new ATNConfig(returnState, config.Alt, parent));
                continue;
            }

            foreach (var tr in config.State.transitions)
            {
                ATNConfig next = null;
                switch (tr)
                {
                    case MyEpsilonTransition:
                    case MyActionTransition:
                    case MyPredicateTransition:
                    case MyPrecedencePredicateTransition:
                        next = config.WithState(tr.target);
                        break;

                    case MyRuleTransition rt:
                        // In SLL: don't push context (treat all stacks as EMPTY).
                        // In LL: push the follow state so we can pop on RuleStop,
                        // unless this is a tail call � the callee will return directly
                        // to whatever is already on the context stack, so no new frame
                        // is needed.
                        PredictionContext newCtx = (fullCtx && !rt.isTailCall)
                            ? new SingletonPredictionContext(config.Context, rt.target.stateNumber)
                            : config.Context;
                        next = new ATNConfig(_atn.start[rt.ruleIndex], config.Alt, newCtx);
                        break;
                    // Terminal transitions are not followed during closure.
                }
                if (next != null) _closureStack.Push(next);
            }
        }
    }

    // Returns true when any alt=1 (loop-body) config in the initial LL closure has a
    // terminal transition that matches the token at startPos.  Used to implement greedy
    // loop semantics: continue the loop iff the body can fire on the next token.
    private bool LoopBodyCanMatchToken(MyATNState decisionState, int[] tokenTypes,
                                       int startPos, PredictionContext callerCtx)
    {
        if (startPos >= tokenTypes.Length) return false;
        int tok = tokenTypes[startPos];

        _closureBusy.Clear();
        var initial = new ATNConfigSet();
        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            Closure(new ATNConfig(target, i + 1, callerCtx), initial, fullCtx: true);
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

    // Number of frames in the context chain (0 for EMPTY).
    private static int ContextDepth(PredictionContext ctx)
    {
        int d = 0;
        while (!ctx.IsEmpty) { ctx = ctx.Parent; d++; }
        return d;
    }
}
