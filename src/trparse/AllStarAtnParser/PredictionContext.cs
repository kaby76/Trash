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
        PredictionContext left, PredictionContext right) =>
        new PredictionContextMergeWorkspace(arena).Merge(left, right);
}

/// <summary>
/// Prediction-local context merger. Context entries are already sorted, so a
/// linear merge plus compact-ID pair memoization replaces recursive ordered-map
/// construction. Depth-indexed buffers are retained and reused by the workspace.
/// </summary>
public sealed class PredictionContextMergeWorkspace
{
    private readonly PredictionContextArena _arena;
    private readonly Dictionary<ulong, PredictionContext> _memo = new();
    private readonly List<MergeBuffer> _buffers = new();

    public PredictionContextMergeWorkspace(PredictionContextArena arena) =>
        _arena = arena ?? throw new ArgumentNullException(nameof(arena));

    public long CacheHits { get; private set; }

    public void Reset()
    {
        _memo.Clear();
        CacheHits = 0;
    }

    public PredictionContext Merge(PredictionContext left,
        PredictionContext right) => Merge(left, right, 0);

    private PredictionContext Merge(PredictionContext left,
        PredictionContext right, int depth)
    {
        if (ReferenceEquals(left, right)) return left;
        ulong key = PairKey(left.Id, right.Id);
        if (_memo.TryGetValue(key, out var cached))
        {
            CacheHits++;
            return cached;
        }

        var buffer = GetBuffer(depth, left.Size + right.Size);
        int li = 0, ri = 0, count = 0;
        while (li < left.Size || ri < right.Size)
        {
            bool takeLeft = ri >= right.Size || li < left.Size &&
                Compare(left, li, right, ri) < 0;
            bool takeRight = li >= left.Size || ri < right.Size &&
                Compare(left, li, right, ri) > 0;
            if (takeLeft)
            {
                buffer.Set(count++, left, li++);
            }
            else if (takeRight)
            {
                buffer.Set(count++, right, ri++);
            }
            else
            {
                int state = left.GetReturnState(li);
                int precedence = left.GetPrecedence(li);
                PredictionContext parent = state == PredictionContext.EMPTY_RETURN_STATE
                    ? null
                    : Merge(left.GetParent(li), right.GetParent(ri), depth + 1);
                buffer.Set(count++, parent, state, precedence);
                li++;
                ri++;
            }
        }

        PredictionContext result;
        if (count == 1)
        {
            result = buffer.States[0] == PredictionContext.EMPTY_RETURN_STATE
                ? PredictionContext.EMPTY
                : _arena.GetChild(buffer.Parents[0], buffer.States[0],
                    buffer.Precedences[0]);
        }
        else
        {
            result = _arena.GetArray(
                buffer.Parents.AsSpan(0, count),
                buffer.States.AsSpan(0, count),
                buffer.Precedences.AsSpan(0, count));
        }
        _memo[key] = result;
        return result;
    }

    private MergeBuffer GetBuffer(int depth, int capacity)
    {
        while (_buffers.Count <= depth) _buffers.Add(new MergeBuffer());
        var result = _buffers[depth];
        result.EnsureCapacity(capacity);
        return result;
    }

    private static int Compare(PredictionContext left, int li,
        PredictionContext right, int ri)
    {
        int result = left.GetReturnState(li).CompareTo(right.GetReturnState(ri));
        return result != 0 ? result :
            left.GetPrecedence(li).CompareTo(right.GetPrecedence(ri));
    }

    private static ulong PairKey(int left, int right)
    {
        uint low = (uint)Math.Min(left, right);
        uint high = (uint)Math.Max(left, right);
        return ((ulong)low << 32) | high;
    }

    private sealed class MergeBuffer
    {
        public PredictionContext[] Parents = Array.Empty<PredictionContext>();
        public int[] States = Array.Empty<int>();
        public int[] Precedences = Array.Empty<int>();

        public void EnsureCapacity(int capacity)
        {
            if (States.Length >= capacity) return;
            int size = Math.Max(capacity, States.Length == 0 ? 4 : States.Length * 2);
            Array.Resize(ref Parents, size);
            Array.Resize(ref States, size);
            Array.Resize(ref Precedences, size);
        }

        public void Set(int target, PredictionContext source, int sourceIndex) =>
            Set(target, source.GetParent(sourceIndex),
                source.GetReturnState(sourceIndex), source.GetPrecedence(sourceIndex));

        public void Set(int target, PredictionContext parent, int state,
            int precedence)
        {
            Parents[target] = parent;
            States[target] = state;
            Precedences[target] = precedence;
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
        return GetArray(parents.AsSpan(), returnStates.AsSpan(),
            precedences.AsSpan());
    }

    internal PredictionContext GetArray(ReadOnlySpan<PredictionContext> parents,
        ReadOnlySpan<int> returnStates, ReadOnlySpan<int> precedences)
    {
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
        var storedParents = parents.ToArray();
        var storedStates = returnStates.ToArray();
        var storedPrecedences = precedences.ToArray();
        var result = new ArrayPredictionContext(this, _nextId++, storedParents,
            storedStates, storedPrecedences);
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

    private static int ArrayHash(ReadOnlySpan<PredictionContext> parents,
        ReadOnlySpan<int> states, ReadOnlySpan<int> precedences)
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
