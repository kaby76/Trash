namespace AllStarAtnParser;

using Atn;
using System.Security.Cryptography;

/// <summary>
/// Grammar-scoped learned SLL prediction data which can be shared by parser
/// instances. A cache binds permanently to the first immutable parser ATN it
/// sees. Access is synchronized so separate parser instances may use it safely.
/// </summary>
public sealed class ParserPredictionCache
{
    internal readonly object SyncRoot = new();
    internal readonly Dictionary<(int decision, int precedence),
        AllStarSimulator.DecisionDfa> DecisionDfas = new();
    internal readonly PredictionContextArena ContextArena;

    private MyATN _atn;
    private string _serializedAtnFingerprint;
    private int _retainedStates;
    private int _retainedTransitions;
    private long _estimatedBytes;

    public ParserPredictionCache(
        int maximumStates = 100_000,
        long maximumEstimatedBytes = 64L * 1024 * 1024)
    {
        if (maximumStates < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumStates));
        if (maximumEstimatedBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumEstimatedBytes));
        MaximumStates = maximumStates;
        MaximumEstimatedBytes = maximumEstimatedBytes;
        ContextArena = new PredictionContextArena(() => TryRetainContext());
    }

    public int MaximumStates { get; }
    public long MaximumEstimatedBytes { get; }
    public int RetainedStates => _retainedStates;
    public int RetainedTransitions => _retainedTransitions;
    public long EstimatedRetainedBytes => _estimatedBytes;
    public bool IsSaturated { get; private set; }
    public bool SharingEnabled { get; private set; } = true;
    public string SharingDisabledReason { get; private set; }

    internal MyATN BoundAtn
    {
        get
        {
            lock (SyncRoot) return _atn;
        }
    }

    internal MyATN GetBoundAtn(int[] serializedAtn)
    {
        string fingerprint = Fingerprint(serializedAtn);
        lock (SyncRoot)
        {
            if (_atn != null && _serializedAtnFingerprint != fingerprint)
                throw new InvalidOperationException(
                    "A parser prediction cache cannot be shared by different parser ATNs.");
            return _atn;
        }
    }

    internal MyATN Bind(MyATN atn)
    {
        ArgumentNullException.ThrowIfNull(atn);
        lock (SyncRoot)
        {
            if (_atn == null)
            {
                _atn = atn;
                if (ContainsSemanticPredicate(atn))
                {
                    SharingEnabled = false;
                    SharingDisabledReason =
                        "parser ATN contains semantic predicate transitions";
                }
            }
            else if (!ReferenceEquals(_atn, atn))
            {
                throw new InvalidOperationException(
                    "A parser prediction cache cannot be shared by different ATN instances.");
            }
            return _atn;
        }
    }

    internal MyATN Bind(MyATN atn, int[] serializedAtn)
    {
        string fingerprint = Fingerprint(serializedAtn);
        lock (SyncRoot)
        {
            if (_serializedAtnFingerprint != null &&
                _serializedAtnFingerprint != fingerprint)
                throw new InvalidOperationException(
                    "A parser prediction cache cannot be shared by different parser ATNs.");
            _serializedAtnFingerprint = fingerprint;
            return Bind(atn);
        }
    }

    internal bool TryRetainState(int configurations)
    {
        const long stateBytes = 96;
        long bytes = stateBytes + configurations * 64L;
        if (_retainedStates >= MaximumStates ||
            _estimatedBytes + bytes > MaximumEstimatedBytes)
        {
            IsSaturated = true;
            return false;
        }
        _retainedStates++;
        _estimatedBytes += bytes;
        return true;
    }

    internal bool TryRetainTransition()
    {
        const long transitionBytes = 32;
        if (_estimatedBytes + transitionBytes > MaximumEstimatedBytes)
        {
            IsSaturated = true;
            return false;
        }
        _retainedTransitions++;
        _estimatedBytes += transitionBytes;
        return true;
    }

    internal bool TryRetainContext()
    {
        const long contextBytes = 48;
        if (_estimatedBytes + contextBytes > MaximumEstimatedBytes)
        {
            IsSaturated = true;
            return false;
        }
        _estimatedBytes += contextBytes;
        return true;
    }

    /// <summary>Discard all learned DFA and prediction-context data.</summary>
    public void Clear()
    {
        lock (SyncRoot)
        {
            DecisionDfas.Clear();
            ContextArena.Clear();
            _retainedStates = 0;
            _retainedTransitions = 0;
            _estimatedBytes = 0;
            IsSaturated = false;
        }
    }

    private static bool ContainsSemanticPredicate(MyATN atn) =>
        atn.allStates.Any(state => state != null &&
            state.transitions.Any(transition => transition is MyPredicateTransition));

    private static string Fingerprint(int[] serializedAtn)
    {
        ArgumentNullException.ThrowIfNull(serializedAtn);
        var bytes = new byte[serializedAtn.Length * sizeof(int)];
        Buffer.BlockCopy(serializedAtn, 0, bytes, 0, bytes.Length);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
