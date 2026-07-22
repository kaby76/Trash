using System.Collections.Generic;

namespace trinterp;

public abstract class ATNState
{
    public int stateNumber = -1;
    public int ruleIndex = -1;
    private readonly List<Transition> _transitions = new();

    public abstract StateType StateType { get; }
    public int NumberOfTransitions => _transitions.Count;
    public Transition Transition(int i) => _transitions[i];
    public void AddTransition(Transition t) => _transitions.Add(t);
    public void AddTransition(int index, Transition t) => _transitions.Insert(index, t);
}

public sealed class BasicState : ATNState
{
    public override StateType StateType => StateType.Basic;
}

public sealed class RuleStartState : ATNState
{
    public RuleStopState stopState;
    public override StateType StateType => StateType.RuleStart;
}

public sealed class RuleStopState : ATNState
{
    public override StateType StateType => StateType.RuleStop;
}

public abstract class DecisionState : ATNState
{
    public bool nonGreedy;
}

public abstract class BlockStartState : DecisionState
{
    public BlockEndState endState;
}

public sealed class BasicBlockStartState : BlockStartState
{
    public override StateType StateType => StateType.BlockStart;
}

public sealed class StarBlockStartState : BlockStartState
{
    public override StateType StateType => StateType.StarBlockStart;
}

public sealed class PlusBlockStartState : BlockStartState
{
    public PlusLoopbackState loopBackState;
    public override StateType StateType => StateType.PlusBlockStart;
}

public sealed class TokensStartState : BlockStartState
{
    public override StateType StateType => StateType.TokenStart;
}

public sealed class BlockEndState : ATNState
{
    public BlockStartState startState;
    public override StateType StateType => StateType.BlockEnd;
}

public sealed class StarLoopEntryState : DecisionState
{
    public StarLoopbackState loopBackState;
    public override StateType StateType => StateType.StarLoopEntry;
}

public sealed class StarLoopbackState : ATNState
{
    public override StateType StateType => StateType.StarLoopBack;
}

public sealed class PlusLoopbackState : DecisionState
{
    public override StateType StateType => StateType.PlusLoopBack;
}

public sealed class LoopEndState : ATNState
{
    public ATNState loopBackState;
    public override StateType StateType => StateType.LoopEnd;
}
