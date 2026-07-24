using System;
using System.Collections.Generic;
using System.Text;

namespace trinterp;

/// <summary>
/// Serializes an <see cref="ATN"/> to the integer-array format consumed by the
/// ANTLR4 runtime's ATNDeserializer.  The output is formatted as "[n1, n2, ...]".
///
/// IMPORTANT: The deserializer assigns states sequential indices 0,1,2,… in the
/// order they appear in the state list (null slots are skipped).  All state
/// references in rules, modes, edges, etc. must use these sequential indices,
/// NOT the original stateNumber values (which may have gaps when states are
/// removed by the ElemList optimisation).
/// </summary>
public static class AtnSerializer
{
    public static string Serialize(ATN atn)
    {
        var data = SerializeToList(atn);
        var sb = new StringBuilder("[");
        for (int i = 0; i < data.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(data[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    public static List<int> SerializeToList(ATN atn)
    {
        // ---------------------------------------------------------------
        // Build stateNumber → serialised-index map.
        // Null slots (removed states) are skipped; remaining states are
        // numbered 0,1,2,… in the order they appear.
        // ---------------------------------------------------------------
        var stateIndex = new Dictionary<int, int>();
        int si = 0;
        foreach (var s in atn.states)
        {
            if (s == null) continue;
            stateIndex[s.stateNumber] = si++;
        }

        // Helper: resolve a stateNumber → serialised index.
        int Idx(int stateNumber) => stateIndex[stateNumber];
        int IdxS(ATNState s) => stateIndex[s.stateNumber];

        var result = new List<int>();

        // ---- preamble ----
        result.Add(4);                         // version
        result.Add((int)atn.grammarType);      // ATNType: Lexer=0, Parser=1
        result.Add(atn.maxTokenType);          // maxTokenType

        // ---- states ----
        int stateCount = stateIndex.Count;
        result.Add(stateCount);

        var nonGreedyStates = new List<int>();
        var precedenceStates = new List<int>();

        foreach (var s in atn.states)
        {
            if (s == null) continue;
            result.Add((int)s.StateType);
            result.Add(s.ruleIndex);   // ruleIndex: -1 stays -1 (metadata, not a reference)

            if (s is DecisionState ds && ds.nonGreedy)
                nonGreedyStates.Add(IdxS(s));

            // Extra words for states that reference another state.
            // Note: endState/loopBackState are only emitted when set (null → no word emitted),
            // matching antlr-ng's serialization where TokensStartState has no endState word.
            if (s is LoopEndState le2)
            {
                if (le2.loopBackState != null) result.Add(IdxS(le2.loopBackState));
            }
            else if (s is BlockStartState bs2)
            {
                if (bs2.endState != null) result.Add(IdxS(bs2.endState));
            }
        }

        // ---- non-greedy states ----
        result.Add(nonGreedyStates.Count);
        result.AddRange(nonGreedyStates);

        // ---- precedence states ----
        result.Add(precedenceStates.Count);
        result.AddRange(precedenceStates);

        // ---- rules ----
        int ruleCount = atn.ruleToStartState?.Length ?? 0;
        result.Add(ruleCount);
        for (int i = 0; i < ruleCount; i++)
        {
            result.Add(IdxS(atn.ruleToStartState[i]));
            if (atn.grammarType == ATNType.Lexer)
            {
                int tokenType = atn.ruleToTokenType != null && i < atn.ruleToTokenType.Length
                    ? atn.ruleToTokenType[i]
                    : 0;
                result.Add(tokenType);
            }
        }

        // ---- modes (lexer only) ----
        int modeCount = atn.modeToStartState?.Count ?? 0;
        result.Add(modeCount);
        for (int i = 0; i < modeCount; i++)
            result.Add(IdxS(atn.modeToStartState[i]));

        // ---- interval sets ----
        var setList = new List<IntervalSet>();
        var setKeyToIndex = new Dictionary<string, int>();

        int CollectSet(IntervalSet set)
        {
            var key = SetKey(set);
            if (setKeyToIndex.TryGetValue(key, out var idx)) return idx;
            idx = setList.Count;
            setList.Add(set);
            setKeyToIndex[key] = idx;
            return idx;
        }

        var transitionSetIndex = new Dictionary<Transition, int>();
        foreach (var s in atn.states)
        {
            if (s == null) continue;
            if (s.StateType == StateType.RuleStop) continue;
            for (int t = 0; t < s.NumberOfTransitions; t++)
            {
                var tr = s.Transition(t);
                if (tr is NotSetTransition nst)
                    transitionSetIndex[tr] = CollectSet(nst.set);
                else if (tr is SetTransition st)
                    transitionSetIndex[tr] = CollectSet(st.set);
            }
        }

        result.Add(setList.Count);
        foreach (var set in setList)
        {
            var intervals = set.GetIntervals();
            bool containsEof = set.Contains(TokenConstants.EOF);

            int intervalCount = 0;
            foreach (var iv in intervals)
                if (iv.b >= 0) intervalCount++;

            result.Add(intervalCount);
            result.Add(containsEof ? 1 : 0);

            foreach (var iv in intervals)
            {
                if (iv.b < 0) continue;
                result.Add(iv.a < 0 ? 0 : iv.a);
                result.Add(iv.b);
            }
        }

        // ---- edges (transitions) ----
        int edgeCount = 0;
        foreach (var s in atn.states)
        {
            if (s == null) continue;
            if (s.StateType == StateType.RuleStop) continue;
            edgeCount += s.NumberOfTransitions;
        }

        result.Add(edgeCount);
        foreach (var s in atn.states)
        {
            if (s == null) continue;
            if (s.StateType == StateType.RuleStop) continue;

            int src = IdxS(s);

            for (int t = 0; t < s.NumberOfTransitions; t++)
            {
                var tr = s.Transition(t);
                int trg, type, arg1, arg2, arg3;

                switch (tr)
                {
                    case EpsilonTransition et:
                        trg = Idx(et.target.stateNumber);
                        type = (int)TransitionType.EPSILON;
                        // OutermostPrecedenceReturn: -1 means "not set"; serialize as 0 to
                        // match antlr-ng convention (0 = no outermost precedence return).
                        arg1 = et.OutermostPrecedenceReturn < 0 ? 0 : et.OutermostPrecedenceReturn;
                        arg2 = 0; arg3 = 0;
                        break;

                    case RangeTransition rt:
                        trg = Idx(rt.target.stateNumber);
                        type = (int)TransitionType.RANGE;
                        bool isEofRange = rt.from == Antlr4.Runtime.TokenConstants.EOF;
                        arg1 = isEofRange ? 0 : rt.from;
                        arg2 = rt.to;
                        arg3 = isEofRange ? 1 : 0;
                        break;

                    case RuleTransition ruleT:
                        // trg = followState (serialised index), arg1 = ruleStart (serialised index)
                        trg = Idx(ruleT.followState.stateNumber);
                        type = (int)TransitionType.RULE;
                        arg1 = Idx(ruleT.target.stateNumber); // ruleStart
                        arg2 = ruleT.ruleIndex;
                        arg3 = ruleT.precedence;
                        break;

                    case PredicateTransition pred:
                        trg = Idx(pred.target.stateNumber);
                        type = (int)TransitionType.PREDICATE;
                        arg1 = pred.ruleIndex;
                        arg2 = pred.predIndex;
                        arg3 = pred.isCtxDependent ? 1 : 0;
                        break;

                    case PrecedencePredicateTransition ppred:
                        trg = Idx(ppred.target.stateNumber);
                        type = (int)TransitionType.PRECEDENCE;
                        arg1 = ppred.precedence;
                        arg2 = 0; arg3 = 0;
                        break;

                    case AtomTransition atom:
                        trg = Idx(atom.target.stateNumber);
                        type = (int)TransitionType.ATOM;
                        bool isEofAtom = atom.token == Antlr4.Runtime.TokenConstants.EOF;
                        arg1 = isEofAtom ? 0 : atom.token;
                        arg2 = 0;
                        arg3 = isEofAtom ? 1 : 0;
                        break;

                    case ActionTransition act:
                        trg = Idx(act.target.stateNumber);
                        type = (int)TransitionType.ACTION;
                        arg1 = act.ruleIndex;
                        arg2 = act.actionIndex;
                        arg3 = act.isCtxDependent ? 1 : 0;
                        break;

                    case NotSetTransition:
                        trg = Idx(tr.target.stateNumber);
                        type = (int)TransitionType.NOT_SET;
                        arg1 = transitionSetIndex[tr];
                        arg2 = 0; arg3 = 0;
                        break;

                    case SetTransition:
                        trg = Idx(tr.target.stateNumber);
                        type = (int)TransitionType.SET;
                        arg1 = transitionSetIndex[tr];
                        arg2 = 0; arg3 = 0;
                        break;

                    case WildcardTransition:
                        trg = Idx(tr.target.stateNumber);
                        type = (int)TransitionType.WILDCARD;
                        arg1 = 0; arg2 = 0; arg3 = 0;
                        break;

                    default:
                        trg = Idx(tr.target.stateNumber);
                        type = (int)TransitionType.EPSILON;
                        arg1 = 0; arg2 = 0; arg3 = 0;
                        break;
                }

                result.Add(src);
                result.Add(trg);
                result.Add(type);
                result.Add(arg1);
                result.Add(arg2);
                result.Add(arg3);
            }
        }

        // ---- decisions ----
        result.Add(atn.decisionToState.Count);
        foreach (var ds in atn.decisionToState)
            result.Add(IdxS(ds));

        // ---- lexer actions (lexer ATN only) ----
        if (atn.grammarType == ATNType.Lexer)
        {
            var actions = atn.lexerActions ?? Array.Empty<ILexerAction>();
            result.Add(actions.Length);
            foreach (var action in actions)
            {
                result.Add((int)action.ActionType);
                switch (action)
                {
                    case LexerChannelAction a:  result.Add(a.Channel);   result.Add(0); break;
                    case LexerCustomAction a:   result.Add(a.RuleIndex); result.Add(a.ActionIndex); break;
                    case LexerModeAction a:     result.Add(a.Mode);      result.Add(0); break;
                    case LexerPushModeAction a: result.Add(a.Mode);      result.Add(0); break;
                    case LexerTypeAction a:     result.Add(a.Type);      result.Add(0); break;
                    default:                    result.Add(0);           result.Add(0); break;
                }
            }
        }

        return result;
    }

    private static string SetKey(IntervalSet set)
    {
        var sb = new StringBuilder();
        foreach (var iv in set.GetIntervals())
        {
            sb.Append(iv.a); sb.Append('-'); sb.Append(iv.b); sb.Append(',');
        }
        return sb.ToString();
    }
}
