namespace AllStarAtnParser;
using EarleyAtnParser;

// A graph-structured parser call stack. Array contexts merge stacks which
// reach the same ATN state/alternative without discarding return paths.

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

public abstract class PredictionContext
{
    public const int EMPTY_RETURN_STATE = int.MaxValue;
    public static readonly PredictionContext EMPTY = EmptyPredictionContext.Instance;

    public abstract bool IsEmpty { get; }
    public abstract bool HasEmptyPath { get; }
    // Return state number to go to after the current rule finishes.
    public abstract int ReturnState { get; }
    // Parent context (the frame below this one).
    public abstract PredictionContext Parent { get; }
    public abstract int Size { get; }
    public abstract int GetReturnState(int index);
    public abstract PredictionContext GetParent(int index);

    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();
}

public sealed class EmptyPredictionContext : PredictionContext
{
    public static readonly EmptyPredictionContext Instance = new();
    private EmptyPredictionContext() { }

    public override bool IsEmpty => true;
    public override bool HasEmptyPath => true;
    public override int ReturnState => EMPTY_RETURN_STATE;
    public override PredictionContext Parent => null;
    public override int Size => 1;
    public override int GetReturnState(int index) => index == 0
        ? EMPTY_RETURN_STATE : throw new ArgumentOutOfRangeException(nameof(index));
    public override PredictionContext GetParent(int index) => index == 0
        ? null : throw new ArgumentOutOfRangeException(nameof(index));

    public override bool Equals(object obj) => obj is EmptyPredictionContext;
    public override int GetHashCode() => 1;
}

// One stack frame: return to state ReturnState, with Parent below.
public sealed class SingletonPredictionContext : PredictionContext
{
    private readonly int _hashCode;

    public override PredictionContext Parent { get; }
    public override int ReturnState { get; }
    public override bool IsEmpty => false;
    public override bool HasEmptyPath => false;
    public override int Size => 1;

    public SingletonPredictionContext(PredictionContext parent, int returnState)
    {
        Parent = parent;
        ReturnState = returnState;
        _hashCode = HashCode.Combine(returnState, parent.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is SingletonPredictionContext s)
            return _hashCode == s._hashCode &&
                   ReturnState == s.ReturnState &&
                   Parent.Equals(s.Parent);
        return false;
    }

    public override int GetHashCode() => _hashCode;
    public override int GetReturnState(int index) => index == 0
        ? ReturnState : throw new ArgumentOutOfRangeException(nameof(index));
    public override PredictionContext GetParent(int index) => index == 0
        ? Parent : throw new ArgumentOutOfRangeException(nameof(index));
}

public sealed class ArrayPredictionContext : PredictionContext
{
    private readonly PredictionContext[] _parents;
    private readonly int[] _returnStates;
    private readonly int _hashCode;

    public ArrayPredictionContext(PredictionContext[] parents, int[] returnStates)
    {
        _parents = parents;
        _returnStates = returnStates;
        var hash = new HashCode();
        foreach (int state in returnStates) hash.Add(state);
        foreach (var parent in parents) hash.Add(parent);
        _hashCode = hash.ToHashCode();
    }

    public override bool IsEmpty => false;
    public override bool HasEmptyPath =>
        _returnStates[^1] == PredictionContext.EMPTY_RETURN_STATE;
    public override int ReturnState => _returnStates[0];
    public override PredictionContext Parent => _parents[0];
    public override int Size => _returnStates.Length;
    public override int GetReturnState(int index) => _returnStates[index];
    public override PredictionContext GetParent(int index) => _parents[index];

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is ArrayPredictionContext other &&
               _hashCode == other._hashCode &&
               _returnStates.AsSpan().SequenceEqual(other._returnStates) &&
               _parents.AsSpan().SequenceEqual(other._parents);
    }

    public override int GetHashCode() => _hashCode;
}

public static class PredictionContextMerger
{
    public static PredictionContext Merge(PredictionContext left, PredictionContext right)
    {
        if (left.Equals(right)) return left;

        var entries = new SortedDictionary<int, PredictionContext>();
        AddEntries(entries, left);
        AddEntries(entries, right);
        if (entries.Count == 1)
        {
            var only = entries.First();
            return only.Key == PredictionContext.EMPTY_RETURN_STATE
                ? PredictionContext.EMPTY
                : new SingletonPredictionContext(only.Value, only.Key);
        }

        var states = new int[entries.Count];
        var parents = new PredictionContext[entries.Count];
        int i = 0;
        foreach (var entry in entries)
        {
            states[i] = entry.Key;
            parents[i] = entry.Value;
            i++;
        }
        return new ArrayPredictionContext(parents, states);
    }

    private static void AddEntries(SortedDictionary<int, PredictionContext> result,
                                   PredictionContext context)
    {
        for (int i = 0; i < context.Size; i++)
        {
            int state = context.GetReturnState(i);
            PredictionContext parent = context.GetParent(i);
            if (result.TryGetValue(state, out var existing) &&
                state != PredictionContext.EMPTY_RETURN_STATE)
                result[state] = Merge(existing, parent);
            else
                result[state] = parent;
        }
    }
}
