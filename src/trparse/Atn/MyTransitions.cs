namespace Atn;

// Base transition class - no Antlr4 runtime dependencies.
public abstract class MyTransition
{
    public MyATNState target;

    protected MyTransition(MyATNState target) => this.target = target;

    public virtual MyIntervalSet Label => null;
    public abstract bool Matches(int symbol, int min, int max);
}

public class MyEpsilonTransition : MyTransition
{
    public int outermostPrecedenceReturn;
    public MyEpsilonTransition(MyATNState target, int opr = -1) : base(target)
        => outermostPrecedenceReturn = opr;
    public override bool Matches(int symbol, int min, int max) => false;
}

public class MyAtomTransition : MyTransition
{
    public int label;
    private readonly MyIntervalSet _labelSet;

    public MyAtomTransition(MyATNState target, int label) : base(target)
    {
        this.label = label;
        _labelSet = MyIntervalSet.Of(label);
    }

    public override MyIntervalSet Label => _labelSet;
    public override bool Matches(int symbol, int min, int max) => symbol == label;
}

public class MyRangeTransition : MyTransition
{
    public int from, to;
    private MyIntervalSet _label;

    public MyRangeTransition(MyATNState target, int from, int to) : base(target)
    {
        this.from = from; this.to = to;
        _label = MyIntervalSet.Of(from, to);
    }

    public override MyIntervalSet Label => _label;
    public override bool Matches(int symbol, int min, int max) => symbol >= from && symbol <= to;
}

// target = followState (return after rule completes); actual rule start = atn.start[ruleIndex]
public class MyRuleTransition : MyTransition
{
    public int ruleIndex;
    public int precedence;
    /// <summary>
    /// True when the follow state (target) is a RuleStop, meaning there is nothing
    /// left to do in the calling rule after the callee returns.  In that case the
    /// closure can inherit the caller's context rather than pushing a new frame,
    /// matching the tail-call optimisation in the ANTLR4 C# and Rust runtimes.
    /// </summary>
    public bool isTailCall;

    public MyRuleTransition(MyATNState followState, int ruleIndex, int precedence)
        : base(followState)
    {
        this.ruleIndex = ruleIndex;
        this.precedence = precedence;
    }

    public override bool Matches(int symbol, int min, int max) => false;
}

public class MySetTransition : MyTransition
{
    public MyIntervalSet set;

    public MySetTransition(MyATNState target, MyIntervalSet set) : base(target)
        => this.set = set;

    public override MyIntervalSet Label => set;
    public override bool Matches(int symbol, int min, int max) => set.Contains(symbol);
}

public class MyNotSetTransition : MyTransition
{
    public MyIntervalSet set;

    public MyNotSetTransition(MyATNState target, MyIntervalSet set) : base(target)
        => this.set = set;

    public override MyIntervalSet Label => set;
    public override bool Matches(int symbol, int min, int max)
        => !set.Contains(symbol) && symbol >= min && symbol <= max;
}

public class MyWildcardTransition : MyTransition
{
    public MyWildcardTransition(MyATNState target) : base(target) { }
    public override bool Matches(int symbol, int min, int max) => symbol >= min && symbol <= max;
}

public class MyActionTransition : MyTransition
{
    public int ruleIndex, actionIndex;
    public bool isCtxDependent;

    public MyActionTransition(MyATNState target, int ruleIndex, int actionIndex, bool isCtxDependent)
        : base(target)
    {
        this.ruleIndex = ruleIndex;
        this.actionIndex = actionIndex;
        this.isCtxDependent = isCtxDependent;
    }

    public override bool Matches(int symbol, int min, int max) => false;
}

public class MyPredicateTransition : MyTransition
{
    public int ruleIndex, predIndex;
    public bool isCtxDependent;

    public MyPredicateTransition(MyATNState target, int ruleIndex, int predIndex, bool isCtxDependent)
        : base(target)
    {
        this.ruleIndex = ruleIndex;
        this.predIndex = predIndex;
        this.isCtxDependent = isCtxDependent;
    }

    public override bool Matches(int symbol, int min, int max) => false;
}

public class MyPrecedencePredicateTransition : MyTransition
{
    public int precedence;

    public MyPrecedencePredicateTransition(MyATNState target, int precedence) : base(target)
        => this.precedence = precedence;

    public override bool Matches(int symbol, int min, int max) => false;
}
