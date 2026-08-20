namespace Trash.EarleyAtn;

/// <summary>
/// Converts the integer-array ATN format (as produced by trinterp's AtnSerializer)
/// into a MyATN structure. No Antlr4.Runtime.Standard deserialization code is used.
/// </summary>
public static class AtnDeserializer
{
    private enum TransitionType
    {
        EPSILON = 1, RANGE = 2, RULE = 3, PREDICATE = 4, ATOM = 5,
        ACTION = 6, SET = 7, NOT_SET = 8, WILDCARD = 9, PRECEDENCE = 10
    }

    public static MyATN Deserialize(int[] data)
    {
        int p = 0;

        // Preamble
        int version = data[p++]; // expected: 4
        var grammarType = (MyATNType)data[p++]; // 0=Lexer, 1=Parser
        int maxTokenType = data[p++];

        var atn = new MyATN { grammarType = grammarType, maxTokenType = maxTokenType };

        // --- States (two-pass) ---
        int stateCount = data[p++];
        var states = new MyATNState[stateCount];
        var loopEndRefs = new Dictionary<int, int>();
        var blockStartRefs = new Dictionary<int, int>();

        for (int i = 0; i < stateCount; i++)
        {
            var stateType = (MyStateType)data[p++];

            // InvalidType states are null placeholders — no ruleIndex word follows.
            // Mirrors ATNDeserializer.ReadStates in the ANTLR4 runtime.
            if (stateType == MyStateType.InvalidType)
            {
                states[i] = null;
                continue;
            }

            int ruleIndex = data[p++];
            states[i] = new MyATNState { stateNumber = i, ruleIndex = ruleIndex, stateType = stateType };

            if (stateType == MyStateType.RuleStop)
                atn.stop.Add(states[i]);

            switch (stateType)
            {
                case MyStateType.LoopEnd:
                    loopEndRefs[i] = data[p++];
                    break;
                case MyStateType.BlockStart:
                case MyStateType.PlusBlockStart:
                case MyStateType.StarBlockStart:
                    blockStartRefs[i] = data[p++];
                    break;
                // TokenStart (type 6): no extra word
            }
        }

        // Resolve cross-references now that all states exist
        foreach (var (i, j) in loopEndRefs) states[i].loopBackState = states[j];
        foreach (var (i, j) in blockStartRefs) states[i].endState = states[j];

        atn.allStates = states;

        // --- Non-greedy states (skip) ---
        int nonGreedyCount = data[p++];
        p += nonGreedyCount;

        // --- Precedence (left-recursive) rule start states ---
        int precedenceCount = data[p++];
        for (int i = 0; i < precedenceCount; i++)
            states[data[p++]].isPrecedenceRule = true;

        // --- Rules ---
        int ruleCount = data[p++];
        atn.start = new MyATNState[ruleCount];
        atn.ruleToStopState = new MyATNState[ruleCount];
        atn.ruleToTokenType = new int[ruleCount];

        for (int i = 0; i < ruleCount; i++)
        {
            int startIdx = data[p++];
            atn.start[i] = states[startIdx];
            if (grammarType == MyATNType.Lexer)
                atn.ruleToTokenType[i] = data[p++];
        }

        // Populate ruleToStopState from stop-state scan
        foreach (var s in atn.stop)
        {
            int ri = s.ruleIndex;
            if (ri >= 0 && ri < ruleCount)
                atn.ruleToStopState[ri] = s;
        }

        // --- Modes ---
        int modeCount = data[p++];
        atn.modeToStartState = new MyATNState[modeCount];
        for (int i = 0; i < modeCount; i++)
            atn.modeToStartState[i] = states[data[p++]];

        // --- Interval sets ---
        int setCount = data[p++];
        var sets = new MyIntervalSet[setCount];
        for (int i = 0; i < setCount; i++)
        {
            int intervalCount = data[p++];
            bool containsEof = data[p++] == 1;
            var set = new MyIntervalSet();
            if (containsEof) set.Add(-1, -1);
            for (int j = 0; j < intervalCount; j++)
            {
                int a = data[p++];
                int b = data[p++];
                set.Add(a, b);
            }
            sets[i] = set;
        }

        // --- Transitions ---
        int edgeCount = data[p++];
        for (int e = 0; e < edgeCount; e++)
        {
            int src  = data[p++];
            int trg  = data[p++];
            int type = data[p++];
            int arg1 = data[p++];
            int arg2 = data[p++];
            int arg3 = data[p++];

            MyTransition tr;
            switch ((TransitionType)type)
            {
                case TransitionType.EPSILON:
                    tr = new MyEpsilonTransition(states[trg], arg1 != 0 ? arg1 : -1);
                    break;

                case TransitionType.RANGE:
                    // arg3==1 means range starts at EOF (-1)
                    tr = new MyRangeTransition(states[trg], arg3 == 1 ? -1 : arg1, arg2);
                    break;

                case TransitionType.RULE:
                    // trg = followState index; arg1 = ruleStart (unused here – looked up via atn.start[arg2])
                    // target = followState; ruleIndex = arg2; precedence = arg3
                    tr = new MyRuleTransition(states[trg], arg2, arg3);
                    break;

                case TransitionType.PREDICATE:
                    tr = new MyPredicateTransition(states[trg], arg1, arg2, arg3 != 0);
                    break;

                case TransitionType.ATOM:
                    // arg3==1 means EOF atom
                    tr = new MyAtomTransition(states[trg], arg3 == 1 ? -1 : arg1);
                    break;

                case TransitionType.ACTION:
                    tr = new MyActionTransition(states[trg], arg1, arg2, arg3 != 0);
                    break;

                case TransitionType.SET:
                    tr = new MySetTransition(states[trg], sets[arg1]);
                    break;

                case TransitionType.NOT_SET:
                    tr = new MyNotSetTransition(states[trg], sets[arg1]);
                    break;

                case TransitionType.WILDCARD:
                    tr = new MyWildcardTransition(states[trg]);
                    break;

                case TransitionType.PRECEDENCE:
                    tr = new MyPrecedencePredicateTransition(states[trg], arg1);
                    break;

                default:
                    tr = new MyEpsilonTransition(states[trg]);
                    break;
            }

            states[src].AddTransition(tr);
        }

        // --- Decisions ---
        int decisionCount = data[p++];
        atn.decisionToState = new MyATNState[decisionCount];
        for (int i = 0; i < decisionCount; i++)
            atn.decisionToState[i] = states[data[p++]];

        // --- Lexer actions (lexer ATN only) ---
        if (grammarType == MyATNType.Lexer)
        {
            int actionCount = data[p++];
            atn.lexerActions = new IMyLexerAction[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                var actionType = (MyLexerActionType)data[p++];
                int a1 = data[p++];
                int a2 = data[p++];
                atn.lexerActions[i] = new MyLexerAction(actionType, a1, a2);
            }
        }

        // Mark tail-call rule transitions: when the follow state is a RuleStop there is
        // nothing left in the calling rule after the callee returns, so closure can skip
        // pushing a context frame.
        foreach (var s in states)
        {
            if (s == null) continue;
            foreach (var tr in s.transitions)
                if (tr is MyRuleTransition rt && rt.target.stateType == MyStateType.RuleStop)
                    rt.isTailCall = true;
        }

        // Mark StarLoopEntry states that are the precedence suffix loop of a left-recursive rule.
        // Mirrors ATNDeserializer.MarkPrecedenceDecisions in the ANTLR4 runtime.
        foreach (var s in states)
        {
            if (s == null) continue;
            if (s.stateType != MyStateType.StarLoopEntry) continue;
            if (s.ruleIndex < 0 || s.ruleIndex >= ruleCount || !atn.start[s.ruleIndex].isPrecedenceRule) continue;
            // Last transition must lead to a LoopEnd whose sole transition exits to RuleStop.
            var loopEnd = s.transitions[^1].target;
            if (loopEnd.stateType == MyStateType.LoopEnd &&
                loopEnd.transitions.Count == 1 &&
                loopEnd.transitions[0].target.stateType == MyStateType.RuleStop)
            {
                s.isPrecedenceDecision = true;
            }
        }

        return atn;
    }
}
