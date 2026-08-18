namespace Trash.EarleyAtn;

// Core ALL(*) prediction engine.
// Implements SLL prediction (context-free) with full-LL fallback.
// No Antlr4.Runtime.Standard types used; token stream is int[] of token types.
public sealed class AllStarSimulator
{
    private readonly MyATN _atn;

    public AllStarSimulator(MyATN atn) => _atn = atn;

    // Returns the predicted alternative (1-indexed) for the given decision.
    // tokenTypes: on-channel token types starting at startPos.
    // callerCtx:  the PredictionContext of the rule that contains this decision (for LL fallback).
    public int AdaptivePredict(int decision, int[] tokenTypes, int startPos, PredictionContext callerCtx)
    {
        var decisionState = _atn.decisionToState[decision];
        int n = decisionState.transitions.Count;
        if (n == 1) return 1; // trivial

        // SLL phase: simulate with EMPTY context
        int sllAlt = ExecATN(decisionState, tokenTypes, startPos, PredictionContext.EMPTY, fullCtx: false);
        if (sllAlt > 0) return sllAlt;

        // LL fallback: simulate with actual caller context
        int llAlt = ExecATN(decisionState, tokenTypes, startPos, callerCtx, fullCtx: true);
        return llAlt > 0 ? llAlt : 1; // default to alt 1 on failure
    }

    private int ExecATN(MyATNState decisionState, int[] tokenTypes, int startPos,
                        PredictionContext baseCtx, bool fullCtx)
    {
        var busy = new HashSet<(int stateNum, int alt, int ctxHash)>();
        var initial = new ATNConfigSet();

        for (int i = 0; i < decisionState.transitions.Count; i++)
        {
            var target = decisionState.transitions[i].target;
            var cfg = new ATNConfig(target, i + 1, baseCtx);
            Closure(cfg, initial, busy, fullCtx);
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

        // Ambiguous or exhausted input: return lowest alt (greedy)
        return current.MinAlt();
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

        var closed = new ATNConfigSet();
        var busy = new HashSet<(int, int, int)>();
        foreach (var c in reach.Configs)
            Closure(c, closed, busy, fullCtx);
        return closed;
    }

    private void Closure(ATNConfig config, ATNConfigSet configs,
                         HashSet<(int stateNum, int alt, int ctxHash)> busy, bool fullCtx)
    {
        var key = (config.State.stateNumber, config.Alt, config.Context.GetHashCode());
        if (!busy.Add(key)) return;

        configs.Add(config);

        if (config.State.stateType == MyStateType.RuleStop)
        {
            if (!fullCtx)
                return; // SLL: can't pop — leave config here

            // LL: pop the context stack
            if (config.Context.IsEmpty)
                return; // at outermost level — done

            int returnStateNum = config.Context.ReturnState;
            PredictionContext parent = config.Context.Parent;
            if (returnStateNum == PredictionContext.EMPTY_RETURN_STATE)
                return;

            var returnState = _atn.allStates[returnStateNum];
            Closure(new ATNConfig(returnState, config.Alt, parent), configs, busy, fullCtx);
            return;
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
                    // In LL: push the follow state so we can pop on RuleStop.
                    PredictionContext newCtx = fullCtx
                        ? new SingletonPredictionContext(config.Context, rt.target.stateNumber)
                        : config.Context;
                    next = new ATNConfig(_atn.start[rt.ruleIndex], config.Alt, newCtx);
                    break;
                // Terminal transitions are not followed during closure.
            }
            if (next != null) Closure(next, configs, busy, fullCtx);
        }
    }

    private static bool IsTerminal(MyTransition t) =>
        t is MyAtomTransition || t is MySetTransition || t is MyNotSetTransition ||
        t is MyWildcardTransition || t is MyRangeTransition;
}
