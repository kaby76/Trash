namespace trinterp;

public abstract class Transition
{
    public ATNState target;
    protected Transition(ATNState target) { this.target = target; }
}

public sealed class EpsilonTransition : Transition
{
    private readonly int _outermostPrecedenceReturn;
    public EpsilonTransition(ATNState to, int outermostPrecedenceReturn = -1) : base(to)
        => _outermostPrecedenceReturn = outermostPrecedenceReturn;
    public int OutermostPrecedenceReturn => _outermostPrecedenceReturn;
}

public sealed class AtomTransition : Transition
{
    public readonly int token;
    public AtomTransition(ATNState to, int token) : base(to) { this.token = token; }
}

public sealed class RangeTransition : Transition
{
    public readonly int from;
    public readonly int to;
    public RangeTransition(ATNState target, int from, int to) : base(target) { this.from = from; this.to = to; }
}

public sealed class RuleTransition : Transition
{
    public readonly int ruleIndex;
    public readonly int precedence;
    public ATNState followState;
    public RuleTransition(ATNState ruleStart, int ruleIndex, int precedence, ATNState followState)
        : base(ruleStart)
    {
        this.ruleIndex = ruleIndex;
        this.precedence = precedence;
        this.followState = followState;
    }
}

public class SetTransition : Transition
{
    public readonly IntervalSet set;
    public SetTransition(ATNState to, IntervalSet set) : base(to) { this.set = set ?? new IntervalSet(); }
}

public sealed class NotSetTransition : SetTransition
{
    public NotSetTransition(ATNState to, IntervalSet set) : base(to, set) { }
}

public sealed class WildcardTransition : Transition
{
    public WildcardTransition(ATNState to) : base(to) { }
}

public sealed class ActionTransition : Transition
{
    public readonly int ruleIndex;
    public readonly int actionIndex;
    public readonly bool isCtxDependent;
    public ActionTransition(ATNState to, int ruleIndex, int actionIndex, bool isCtxDependent)
        : base(to) { this.ruleIndex = ruleIndex; this.actionIndex = actionIndex; this.isCtxDependent = isCtxDependent; }
}

public sealed class PredicateTransition : Transition
{
    public readonly int ruleIndex;
    public readonly int predIndex;
    public readonly bool isCtxDependent;
    public PredicateTransition(ATNState to, int ruleIndex, int predIndex, bool isCtxDependent)
        : base(to) { this.ruleIndex = ruleIndex; this.predIndex = predIndex; this.isCtxDependent = isCtxDependent; }
}

public sealed class PrecedencePredicateTransition : Transition
{
    public readonly int precedence;
    public PrecedencePredicateTransition(ATNState to, int precedence) : base(to) { this.precedence = precedence; }
}
