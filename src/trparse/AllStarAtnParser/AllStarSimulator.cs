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

        bool isLoop = decisionState.stateType == MyStateType.StarLoopEntry ||
                      decisionState.stateType == MyStateType.PlusLoopBack;

        // SLL phase: only for non-loop decisions.
        // For loop decisions, SLL can't properly evaluate the exit alt because it stops
        // at RuleStop and cannot see that the exit alt may be viable via caller context.
        int sllAlt = -1;
        if (!isLoop)
        {
            sllAlt = ExecATN(decisionState, tokenTypes, startPos, PredictionContext.EMPTY, fullCtx: false);
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[SIM] dec={decision} state={decisionState.stateNumber} type={decisionState.stateType} sllAlt={sllAlt}");
            if (sllAlt > 0) return sllAlt;
        }

        // LL prediction using actual caller context
        int llAlt = ExecATN(decisionState, tokenTypes, startPos, callerCtx, fullCtx: true);
        if (AllStarParser.Trace)
            Console.Error.WriteLine($"[SIM] dec={decision} state={decisionState.stateNumber} type={decisionState.stateType} llAlt={llAlt}");
        if (llAlt > 0) return llAlt;

        // LL also couldn't determine: loop decisions exit (last alt), others take alt=1 (greedy).
        int def = isLoop ? n : 1;
        if (AllStarParser.Trace)
            Console.Error.WriteLine($"[SIM] dec={decision} default={def} (isLoop={isLoop})");
        return def;
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

        // Exhausted lookahead without finding a unique alt — signal caller.
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

        var closed = new ATNConfigSet();
        var busy = new HashSet<(int, int, int)>();
        foreach (var c in reach.Configs)
            Closure(c, closed, busy, fullCtx);
        return closed;
    }

    private void Closure(ATNConfig seed, ATNConfigSet configs,
                         HashSet<(int stateNum, int alt, int ctxHash)> busy, bool fullCtx)
    {
        var stack = new Stack<ATNConfig>();
        stack.Push(seed);

        while (stack.Count > 0)
        {
            var config = stack.Pop();

            var key = (config.State.stateNumber, config.Alt, config.Context.GetHashCode());
            if (!busy.Add(key)) continue;

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
                stack.Push(new ATNConfig(returnState, config.Alt, parent));
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
                        // In LL: push the follow state so we can pop on RuleStop.
                        PredictionContext newCtx = fullCtx
                            ? new SingletonPredictionContext(config.Context, rt.target.stateNumber)
                            : config.Context;
                        next = new ATNConfig(_atn.start[rt.ruleIndex], config.Alt, newCtx);
                        break;
                    // Terminal transitions are not followed during closure.
                }
                if (next != null) stack.Push(next);
            }
        }
    }

    private static bool IsTerminal(MyTransition t) =>
        t is MyAtomTransition || t is MySetTransition || t is MyNotSetTransition ||
        t is MyWildcardTransition || t is MyRangeTransition;
}
