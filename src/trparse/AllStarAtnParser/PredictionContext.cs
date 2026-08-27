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
    public abstract int GetPrecedence(int index);

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
    public override int GetPrecedence(int index) => index == 0
        ? 0 : throw new ArgumentOutOfRangeException(nameof(index));

    public override bool Equals(object obj) => obj is EmptyPredictionContext;
    public override int GetHashCode() => 1;
}

// One stack frame: return to state ReturnState, with Parent below.
public sealed class SingletonPredictionContext : PredictionContext
{
    private readonly int _hashCode;

    public override PredictionContext Parent { get; }
    public override int ReturnState { get; }
    public int Precedence { get; }
    public override bool IsEmpty => false;
    public override bool HasEmptyPath => false;
    public override int Size => 1;

    public SingletonPredictionContext(PredictionContext parent, int returnState,
                                      int precedence = 0)
    {
        Parent = parent;
        ReturnState = returnState;
        Precedence = precedence;
        _hashCode = HashCode.Combine(returnState, precedence, parent.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is SingletonPredictionContext s)
            return _hashCode == s._hashCode &&
                   ReturnState == s.ReturnState &&
                   Precedence == s.Precedence &&
                   Parent.Equals(s.Parent);
        return false;
    }

    public override int GetHashCode() => _hashCode;
    public override int GetReturnState(int index) => index == 0
        ? ReturnState : throw new ArgumentOutOfRangeException(nameof(index));
    public override PredictionContext GetParent(int index) => index == 0
        ? Parent : throw new ArgumentOutOfRangeException(nameof(index));
    public override int GetPrecedence(int index) => index == 0
        ? Precedence : throw new ArgumentOutOfRangeException(nameof(index));
}

public sealed class ArrayPredictionContext : PredictionContext
{
    private readonly PredictionContext[] _parents;
    private readonly int[] _returnStates;
    private readonly int[] _precedences;
    private readonly int _hashCode;

    public ArrayPredictionContext(PredictionContext[] parents, int[] returnStates,
                                  int[] precedences)
    {
        _parents = parents;
        _returnStates = returnStates;
        _precedences = precedences;
        var hash = new HashCode();
        foreach (int state in returnStates) hash.Add(state);
        foreach (int precedence in precedences) hash.Add(precedence);
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
    public override int GetPrecedence(int index) => _precedences[index];

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is ArrayPredictionContext other &&
               _hashCode == other._hashCode &&
               _returnStates.AsSpan().SequenceEqual(other._returnStates) &&
               _precedences.AsSpan().SequenceEqual(other._precedences) &&
               _parents.AsSpan().SequenceEqual(other._parents);
    }

    public override int GetHashCode() => _hashCode;
}

public static class PredictionContextMerger
{
    public static PredictionContext Merge(PredictionContext left, PredictionContext right)
    {
        if (left.Equals(right)) return left;

        var entries = new SortedDictionary<(int state, int precedence), PredictionContext>();
        AddEntries(entries, left);
        AddEntries(entries, right);
        if (entries.Count == 1)
        {
            var only = entries.First();
            return only.Key.state == PredictionContext.EMPTY_RETURN_STATE
                ? PredictionContext.EMPTY
                : new SingletonPredictionContext(only.Value, only.Key.state,
                                                 only.Key.precedence);
        }

        var states = new int[entries.Count];
        var parents = new PredictionContext[entries.Count];
        var precedences = new int[entries.Count];
        int i = 0;
        foreach (var entry in entries)
        {
            states[i] = entry.Key.state;
            precedences[i] = entry.Key.precedence;
            parents[i] = entry.Value;
            i++;
        }
        return new ArrayPredictionContext(parents, states, precedences);
    }

    private static void AddEntries(
                                   SortedDictionary<(int state, int precedence), PredictionContext> result,
                                   PredictionContext context)
    {
        for (int i = 0; i < context.Size; i++)
        {
            int state = context.GetReturnState(i);
            int precedence = context.GetPrecedence(i);
            PredictionContext parent = context.GetParent(i);
            var key = (state, precedence);
            if (result.TryGetValue(key, out var existing) &&
                state != PredictionContext.EMPTY_RETURN_STATE)
                result[key] = Merge(existing, parent);
            else
                result[key] = parent;
        }
    }
}
