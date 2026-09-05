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

    public abstract int Id { get; }
    internal abstract PredictionContextArena Arena { get; }

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
    public override int Id => 0;
    internal override PredictionContextArena Arena => null;
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
    public override int Id { get; }
    internal override PredictionContextArena Arena { get; }
    public override bool IsEmpty => false;
    public override bool HasEmptyPath => false;
    public override int Size => 1;

    internal SingletonPredictionContext(PredictionContextArena arena, int id,
        PredictionContext parent, int returnState, int precedence = 0)
    {
        Arena = arena;
        Id = id;
        Parent = parent;
        ReturnState = returnState;
        Precedence = precedence;
        _hashCode = HashCode.Combine(returnState, precedence, parent.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) ||
            obj is SingletonPredictionContext s &&
            ReferenceEquals(Arena, s.Arena) && Id == s.Id;
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
    public override int Id { get; }
    internal override PredictionContextArena Arena { get; }

    internal ArrayPredictionContext(PredictionContextArena arena, int id,
        PredictionContext[] parents, int[] returnStates, int[] precedences)
    {
        Arena = arena;
        Id = id;
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
        return ReferenceEquals(this, obj) ||
            obj is ArrayPredictionContext other &&
            ReferenceEquals(Arena, other.Arena) && Id == other.Id;
    }

    public override int GetHashCode() => _hashCode;
}

public static class PredictionContextMerger
{
    public static PredictionContext Merge(PredictionContextArena arena,
        PredictionContext left, PredictionContext right)
    {
        if (ReferenceEquals(left, right)) return left;

        var entries = new SortedDictionary<(int state, int precedence), PredictionContext>();
        AddEntries(arena, entries, left);
        AddEntries(arena, entries, right);
        if (entries.Count == 1)
        {
            var only = entries.First();
            return only.Key.state == PredictionContext.EMPTY_RETURN_STATE
                ? PredictionContext.EMPTY
                : arena.GetChild(only.Value, only.Key.state, only.Key.precedence);
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
        return arena.GetArray(parents, states, precedences);
    }

    private static void AddEntries(PredictionContextArena arena,
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
                result[key] = Merge(arena, existing, parent);
            else
                result[key] = parent;
        }
    }
}

/// <summary>
/// Canonical storage for every prediction-context node used by one parser ATN.
/// Context IDs are compact and stable for the lifetime of the arena.
/// </summary>
public sealed class PredictionContextArena
{
    private readonly Dictionary<(int parent, int state, int precedence),
        SingletonPredictionContext> _singletons = new();
    private readonly SingletonPredictionContext[] _singletonHot =
        new SingletonPredictionContext[2048];
    private readonly Dictionary<int, List<ArrayPredictionContext>> _arrays = new();
    private int _nextId = 1;
    private readonly Action _onCreate;

    public PredictionContextArena(Action onCreate = null) => _onCreate = onCreate;

    public int Count => _nextId;
    public long Creations { get; private set; }
    public long Hits { get; private set; }

    public SingletonPredictionContext GetChild(PredictionContext parent,
        int returnState, int precedence = 0)
    {
        ArgumentNullException.ThrowIfNull(parent);
        if (!parent.IsEmpty && !ReferenceEquals(parent.Arena, this))
            throw new ArgumentException(
                "The parent context belongs to a different arena.", nameof(parent));
        int hotIndex = HashCode.Combine(parent.Id, returnState, precedence) &
            (_singletonHot.Length - 1);
        var hot = _singletonHot[hotIndex];
        if (hot != null && hot.Parent.Id == parent.Id &&
            hot.ReturnState == returnState && hot.Precedence == precedence)
        {
            Hits++;
            return hot;
        }
        var key = (parent.Id, returnState, precedence);
        if (_singletons.TryGetValue(key, out var result))
        {
            _singletonHot[hotIndex] = result;
            Hits++;
            return result;
        }
        result = new SingletonPredictionContext(
            this, _nextId++, parent, returnState, precedence);
        _singletons.Add(key, result);
        _singletonHot[hotIndex] = result;
        Creations++;
        _onCreate?.Invoke();
        return result;
    }

    public PredictionContext GetArray(PredictionContext[] parents,
        int[] returnStates, int[] precedences)
    {
        ArgumentNullException.ThrowIfNull(parents);
        ArgumentNullException.ThrowIfNull(returnStates);
        ArgumentNullException.ThrowIfNull(precedences);
        if (parents.Length != returnStates.Length ||
            parents.Length != precedences.Length || parents.Length == 0)
            throw new ArgumentException(
                "Context arrays must be non-empty and have equal lengths.");
        for (int i = 0; i < parents.Length; i++)
            if (parents[i] != null && !parents[i].IsEmpty &&
                !ReferenceEquals(parents[i].Arena, this))
                throw new ArgumentException(
                    "A parent context belongs to a different arena.", nameof(parents));
        int hash = ArrayHash(parents, returnStates, precedences);
        if (_arrays.TryGetValue(hash, out var bucket))
        {
            foreach (var candidate in bucket)
            {
                bool equal = candidate.Size == returnStates.Length;
                for (int i = 0; equal && i < returnStates.Length; i++)
                    equal = candidate.GetReturnState(i) == returnStates[i] &&
                        candidate.GetPrecedence(i) == precedences[i] &&
                        (candidate.GetParent(i)?.Id ?? 0) ==
                        (parents[i]?.Id ?? 0);
                if (equal)
                {
                    Hits++;
                    return candidate;
                }
            }
        }
        else
        {
            bucket = new List<ArrayPredictionContext>();
            _arrays.Add(hash, bucket);
        }
        var result = new ArrayPredictionContext(this, _nextId++, parents,
            returnStates, precedences);
        bucket.Add(result);
        Creations++;
        _onCreate?.Invoke();
        return result;
    }

    public void Clear()
    {
        _singletons.Clear();
        Array.Clear(_singletonHot);
        _arrays.Clear();
        _nextId = 1;
        Creations = 0;
        Hits = 0;
    }

    private static int ArrayHash(PredictionContext[] parents, int[] states,
        int[] precedences)
    {
        var hash = new HashCode();
        for (int i = 0; i < states.Length; i++)
        {
            hash.Add(states[i]);
            hash.Add(precedences[i]);
            hash.Add(parents[i]?.Id ?? 0);
        }
        return hash.ToHashCode();
    }
}
