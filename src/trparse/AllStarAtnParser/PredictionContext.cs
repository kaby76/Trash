namespace Trash.EarleyAtn;

// Represents a parser call stack for ALL(*) prediction.
// Two concrete forms: Empty (bottom of stack) and Singleton (one frame).
// No Antlr4.Runtime.Standard types used.
public abstract class PredictionContext
{
    public const int EMPTY_RETURN_STATE = int.MaxValue;
    public static readonly PredictionContext EMPTY = EmptyPredictionContext.Instance;

    public abstract bool IsEmpty { get; }
    // Return state number to go to after the current rule finishes.
    public abstract int ReturnState { get; }
    // Parent context (the frame below this one).
    public abstract PredictionContext Parent { get; }

    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();
}

public sealed class EmptyPredictionContext : PredictionContext
{
    public static readonly EmptyPredictionContext Instance = new();
    private EmptyPredictionContext() { }

    public override bool IsEmpty => true;
    public override int ReturnState => EMPTY_RETURN_STATE;
    public override PredictionContext Parent => null;

    public override bool Equals(object obj) => obj is EmptyPredictionContext;
    public override int GetHashCode() => 1;
}

// One stack frame: return to state ReturnState, with Parent below.
public sealed class SingletonPredictionContext : PredictionContext
{
    public override PredictionContext Parent { get; }
    public override int ReturnState { get; }
    public override bool IsEmpty => false;

    public SingletonPredictionContext(PredictionContext parent, int returnState)
    {
        Parent = parent;
        ReturnState = returnState;
    }

    public override bool Equals(object obj)
    {
        if (obj is SingletonPredictionContext s)
            return ReturnState == s.ReturnState && Parent.Equals(s.Parent);
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(ReturnState, Parent.GetHashCode());
}
