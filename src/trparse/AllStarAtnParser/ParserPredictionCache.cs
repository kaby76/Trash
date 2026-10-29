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
        long maximumEstimatedBytes = 64L * 1024 * 1024,
        bool synchronizeAccess = true)
    {
        if (maximumStates < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumStates));
        if (maximumEstimatedBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumEstimatedBytes));
        MaximumStates = maximumStates;
        MaximumEstimatedBytes = maximumEstimatedBytes;
        SynchronizeAccess = synchronizeAccess;
        ContextArena = new PredictionContextArena(() => TryRetainContext());
    }

    public int MaximumStates { get; }
    public long MaximumEstimatedBytes { get; }
    public bool SynchronizeAccess { get; }
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

    internal void Write(BinaryWriter writer)
    {
        lock (SyncRoot)
        {
            // Contexts are numbered by dependency order so they can be rebuilt
            // in a new arena without retaining process-local object identities.
            var contextIds = new Dictionary<PredictionContext, int>(
                ReferenceEqualityComparer.Instance)
            {
                [PredictionContext.EMPTY] = 0
            };
            var contexts = new List<PredictionContext>();
            int Register(PredictionContext context)
            {
                if (contextIds.TryGetValue(context, out int id)) return id;
                for (int i = 0; i < context.Size; i++)
                    if (context.GetParent(i) != null)
                        Register(context.GetParent(i));
                id = contexts.Count + 1;
                contexts.Add(context);
                contextIds.Add(context, id);
                return id;
            }
            foreach (var dfa in DecisionDfas.Values)
                foreach (var state in dfa.States.Values)
                    foreach (var config in state.Configs.Configs)
                        Register(config.Context);
            writer.Write(contexts.Count);
            foreach (var context in contexts)
            {
                writer.Write(context.Size);
                for (int i = 0; i < context.Size; i++)
                {
                    writer.Write(context.GetParent(i) == null ? -1 :
                        contextIds[context.GetParent(i)]);
                    writer.Write(context.GetReturnState(i));
                    writer.Write(context.GetPrecedence(i));
                }
            }
            writer.Write(DecisionDfas.Count);
            foreach (var ((decision, precedence), dfa) in DecisionDfas)
            {
                writer.Write(decision);
                writer.Write(precedence);
                var states = new List<AllStarSimulator.DfaState>();
                var indices = new Dictionary<AllStarSimulator.DfaState, int>(
                    ReferenceEqualityComparer.Instance);
                void Add(AllStarSimulator.DfaState state)
                {
                    if (state == null || indices.ContainsKey(state)) return;
                    indices.Add(state, states.Count);
                    states.Add(state);
                }
                foreach (var state in dfa.States.Values) Add(state);
                Add(dfa.Start);
                for (int i = 0; i < states.Count; i++)
                    foreach (var target in states[i].Edges.Values) Add(target);
                writer.Write(states.Count);
                writer.Write(dfa.Start == null ? -1 : indices[dfa.Start]);
                foreach (var state in states)
                {
                    writer.Write(state.IsError);
                    writer.Write(state.Prediction);
                    writer.Write(state.CompletedPrediction);
                    writer.Write(state.RequiresFullContext);
                    writer.Write(state.Configs != null);
                    if (state.Configs != null)
                    {
                        writer.Write(state.Configs.Configs.Count);
                        foreach (var config in state.Configs.Configs)
                        {
                            writer.Write(config.State.stateNumber);
                            writer.Write(config.Alt);
                            writer.Write(contextIds[config.Context]);
                            writer.Write(config.Precedence);
                        }
                    }
                    writer.Write(state.Edges.Count);
                    foreach (var (symbol, target) in state.Edges)
                    {
                        writer.Write(symbol);
                        writer.Write(indices[target]);
                    }
                    writer.Write(state.StopLiveEdges.Count);
                    foreach (int symbol in state.StopLiveEdges) writer.Write(symbol);
                }
            }
        }
    }

    internal void Read(BinaryReader reader, MyATN atn, int[] serializedAtn)
    {
        lock (SyncRoot)
        {
            Bind(atn, serializedAtn);
            if (DecisionDfas.Count != 0 || ContextArena.Count != 1)
                throw new InvalidOperationException("The parser DFA cache is already populated.");
            int contextCount = ReadCount(reader, 2_000_000);
            var contexts = new List<PredictionContext>(contextCount + 1)
            {
                PredictionContext.EMPTY
            };
            for (int id = 1; id <= contextCount; id++)
            {
                int size = ReadCount(reader, 100_000);
                if (size == 0) throw new InvalidDataException("Empty parser context array.");
                var parents = new PredictionContext[size];
                var states = new int[size];
                var precedences = new int[size];
                for (int i = 0; i < size; i++)
                {
                    int parent = reader.ReadInt32();
                    states[i] = reader.ReadInt32();
                    precedences[i] = reader.ReadInt32();
                    if (parent < -1 || parent >= id ||
                        (parent == -1 && states[i] != PredictionContext.EMPTY_RETURN_STATE))
                        throw new InvalidDataException("Invalid parser prediction context.");
                    parents[i] = parent < 0 ? null : contexts[parent];
                }
                var context = size == 1 && parents[0] != null
                    ? ContextArena.GetChild(parents[0], states[0], precedences[0])
                    : ContextArena.GetArray(parents, states, precedences);
                if (IsSaturated)
                    throw new InvalidDataException("Parser DFA cache exceeds the configured memory budget.");
                contexts.Add(context);
            }
            int dfaCount = ReadCount(reader, 1_000_000);
            for (int d = 0; d < dfaCount; d++)
            {
                int decision = reader.ReadInt32();
                int precedence = reader.ReadInt32();
                if ((uint)decision >= (uint)atn.decisionToState.Length)
                    throw new InvalidDataException("Invalid parser DFA decision.");
                int stateCount = ReadCount(reader, 2_000_000);
                int startIndex = reader.ReadInt32();
                if (startIndex < -1 || startIndex >= stateCount)
                    throw new InvalidDataException("Invalid parser DFA start state.");
                var states = new AllStarSimulator.DfaState[stateCount];
                var edges = new List<(int Symbol, int Target)>[stateCount];
                var stopLive = new List<int>[stateCount];
                var dfa = new AllStarSimulator.DecisionDfa();
                for (int i = 0; i < stateCount; i++)
                {
                    bool isError = reader.ReadBoolean();
                    int prediction = reader.ReadInt32();
                    int completed = reader.ReadInt32();
                    bool fullContext = reader.ReadBoolean();
                    AllStarSimulator.DfaState state;
                    if (reader.ReadBoolean())
                    {
                        var configs = new ATNConfigSet(contextArena: ContextArena);
                        int configCount = ReadCount(reader, 1_000_000);
                        for (int j = 0; j < configCount; j++)
                        {
                            int number = reader.ReadInt32();
                            int alt = reader.ReadInt32();
                            int context = reader.ReadInt32();
                            int priority = reader.ReadInt32();
                            if ((uint)number >= (uint)atn.allStates.Length ||
                                atn.allStates[number] == null ||
                                (uint)context >= (uint)contexts.Count || alt <= 0)
                                throw new InvalidDataException("Invalid parser DFA configuration.");
                            configs.Add(new ATNConfig(atn.allStates[number], alt,
                                contexts[context], priority));
                        }
                        if (!TryRetainState(configs.Configs.Count))
                            throw new InvalidDataException("Parser DFA cache exceeds the configured memory budget.");
                        state = new AllStarSimulator.DfaState(configs);
                        dfa.States.Add(new AllStarSimulator.ConfigSetKey(configs), state);
                    }
                    else state = isError ? AllStarSimulator.DfaState.Error :
                        AllStarSimulator.DfaState.Accept(prediction);
                    state.Prediction = prediction;
                    state.CompletedPrediction = completed;
                    state.RequiresFullContext = fullContext;
                    state.IsRetained = true;
                    states[i] = state;
                    int edgeCount = ReadCount(reader, 2_000_000);
                    edges[i] = new List<(int, int)>(edgeCount);
                    for (int j = 0; j < edgeCount; j++)
                        edges[i].Add((reader.ReadInt32(), reader.ReadInt32()));
                    int stopCount = ReadCount(reader, 2_000_000);
                    stopLive[i] = new List<int>(stopCount);
                    for (int j = 0; j < stopCount; j++)
                        stopLive[i].Add(reader.ReadInt32());
                }
                for (int i = 0; i < stateCount; i++)
                {
                    foreach (var (symbol, target) in edges[i])
                    {
                        if ((uint)target >= (uint)stateCount ||
                            !TryRetainTransition())
                            throw new InvalidDataException("Invalid or oversized parser DFA edge.");
                        states[i].Edges.Add(symbol, states[target]);
                    }
                    foreach (int symbol in stopLive[i])
                        states[i].StopLiveEdges.Add(symbol);
                }
                dfa.Start = startIndex < 0 ? null : states[startIndex];
                DecisionDfas.Add((decision, precedence), dfa);
            }
        }
    }

    private static int ReadCount(BinaryReader reader, int maximum)
    {
        int count = reader.ReadInt32();
        if (count < 0 || count > maximum)
            throw new InvalidDataException("Invalid parser DFA cache count.");
        return count;
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
