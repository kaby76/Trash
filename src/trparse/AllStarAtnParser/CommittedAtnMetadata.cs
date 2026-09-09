namespace AllStarAtnParser;

using Atn;
using System.Runtime.CompilerServices;

internal enum CommittedStateKind : byte
{
    Invalid,
    Stop,
    Decision,
    Epsilon,
    Rule,
    Terminal
}

internal enum CommittedTerminalKind : byte
{
    None,
    Atom,
    Set,
    NotSet,
    Wildcard,
    Range
}

/// <summary>Immutable, index-addressed metadata for the committed ATN walker.</summary>
internal sealed class CommittedAtnMetadata
{
    private static readonly ConditionalWeakTable<MyATN, CommittedAtnMetadata>
        Cache = new();

    public readonly int[] Decision;
    public readonly CommittedStateKind[] Kind;
    public readonly MyTransition[] Transition;
    public readonly MyATNState[][] DecisionTargets;
    public readonly MyRuleTransition[] RuleTransition;
    public readonly CommittedTerminalKind[] TerminalKind;
    public readonly int[] TerminalArgument1;
    public readonly int[] TerminalArgument2;
    // Compiled destination after following a deterministic epsilon/action/
    // predicate chain.  The prediction engine continues to use the original
    // ATN; this table is only for the committed parser walk after an
    // alternative has been selected.
    public readonly MyATNState[] NextOperationState;
    public readonly int[] EpsilonPathLength;

    public static CommittedAtnMetadata For(MyATN atn) =>
        Cache.GetValue(atn, static value => new CommittedAtnMetadata(value));

    private CommittedAtnMetadata(MyATN atn)
    {
        Decision = new int[atn.allStates.Length];
        Array.Fill(Decision, -1);
        Kind = new CommittedStateKind[atn.allStates.Length];
        Transition = new MyTransition[atn.allStates.Length];
        DecisionTargets = new MyATNState[atn.allStates.Length][];
        RuleTransition = new MyRuleTransition[atn.allStates.Length];
        TerminalKind = new CommittedTerminalKind[atn.allStates.Length];
        TerminalArgument1 = new int[atn.allStates.Length];
        TerminalArgument2 = new int[atn.allStates.Length];
        NextOperationState = new MyATNState[atn.allStates.Length];
        EpsilonPathLength = new int[atn.allStates.Length];

        for (int i = 0; i < atn.decisionToState.Length; i++)
        {
            int stateNumber = atn.decisionToState[i].stateNumber;
            Decision[stateNumber] = i;
            Kind[stateNumber] = CommittedStateKind.Decision;
            DecisionTargets[stateNumber] = atn.decisionToState[i].transitions
                .Select(transition => transition.target).ToArray();
        }

        foreach (var state in atn.allStates)
        {
            if (state == null) continue;
            int number = state.stateNumber;
            if (state.stateType == MyStateType.RuleStop)
            {
                Kind[number] = CommittedStateKind.Stop;
                continue;
            }
            if (Decision[number] >= 0) continue;
            if (state.transitions.Count != 1) continue;
            var transition = state.transitions[0];
            Transition[number] = transition;
            Kind[number] = transition switch
            {
                MyEpsilonTransition or MyActionTransition or
                    MyPredicateTransition or MyPrecedencePredicateTransition =>
                    CommittedStateKind.Epsilon,
                MyRuleTransition => CommittedStateKind.Rule,
                _ => CommittedStateKind.Terminal
            };
            if (transition is MyRuleTransition ruleTransition)
                RuleTransition[number] = ruleTransition;
            TerminalKind[number] = transition switch
            {
                MyAtomTransition atom => SetArguments(
                    number, CommittedTerminalKind.Atom, atom.label),
                MySetTransition => CommittedTerminalKind.Set,
                MyNotSetTransition => CommittedTerminalKind.NotSet,
                MyWildcardTransition => CommittedTerminalKind.Wildcard,
                MyRangeTransition range => SetArguments(
                    number, CommittedTerminalKind.Range, range.from, range.to),
                _ => CommittedTerminalKind.None
            };
        }

        foreach (var state in atn.allStates)
        {
            if (state == null) continue;
            CompileEpsilonPath(state);
        }
    }

    private void CompileEpsilonPath(MyATNState start)
    {
        var state = start;
        int distance = 0;
        // A deterministic epsilon cycle would also make the old committed
        // walker loop forever.  Detect it here and retain the original state
        // so malformed ATNs do not hang metadata construction.
        var seen = new HashSet<int>();
        while (Kind[state.stateNumber] == CommittedStateKind.Epsilon)
        {
            if (!seen.Add(state.stateNumber))
            {
                state = start;
                distance = 0;
                break;
            }
            state = Transition[state.stateNumber].target;
            distance++;
        }
        NextOperationState[start.stateNumber] = state;
        EpsilonPathLength[start.stateNumber] = distance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MyATNState SkipEpsilon(MyATNState state) =>
        NextOperationState[state.stateNumber];

    private CommittedTerminalKind SetArguments(int state, CommittedTerminalKind kind,
        int argument1, int argument2 = 0)
    {
        TerminalArgument1[state] = argument1;
        TerminalArgument2[state] = argument2;
        return kind;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TerminalMatches(int state, int tokenType) => TerminalKind[state] switch
    {
        CommittedTerminalKind.Atom => TerminalArgument1[state] == tokenType,
        CommittedTerminalKind.Set =>
            ((MySetTransition)Transition[state]).set.Contains(tokenType),
        CommittedTerminalKind.NotSet => tokenType != -1 &&
            !((MyNotSetTransition)Transition[state]).set.Contains(tokenType),
        CommittedTerminalKind.Wildcard => tokenType != -1,
        CommittedTerminalKind.Range => tokenType >= TerminalArgument1[state] &&
            tokenType <= TerminalArgument2[state],
        _ => false
    };
}
