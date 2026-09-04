namespace EarleyAtnParser;

/// <summary>Learned, per-mode DFA support for <see cref="LexerAtnSimulator"/>.</summary>
public partial class LexerAtnSimulator
{
    public readonly record struct LexerDfaStatistics(
        int States,
        int LiveTransitions,
        int DeadTransitions,
        int DenseAsciiRows,
        int SparseEntries,
        long CacheHits,
        long CacheMisses,
        int MaximumConfigurations,
        long EstimatedRetainedBytes);

    private sealed class DfaEdgeTable
    {
        private const int AsciiLimit = 128;
        private DfaState[] _asciiTargets;
        private byte[] _asciiKinds; // 0 = unknown, 1 = live, 2 = dead
        private Dictionary<int, DfaState> _sparse;

        public int LiveCount { get; private set; }
        public int DeadCount { get; private set; }
        public bool HasDenseRow => _asciiKinds != null;
        public int SparseCount => _sparse?.Count ?? 0;

        public bool TryGet(int symbol, out DfaState target)
        {
            if ((uint)symbol < AsciiLimit)
            {
                if (_asciiKinds == null || _asciiKinds[symbol] == 0)
                {
                    target = null;
                    return false;
                }
                target = _asciiTargets[symbol];
                return true;
            }

            if (_sparse != null && _sparse.TryGetValue(symbol, out target))
                return true;
            target = null;
            return false;
        }

        public void Set(int symbol, DfaState target)
        {
            if ((uint)symbol < AsciiLimit)
            {
                _asciiTargets ??= new DfaState[AsciiLimit];
                _asciiKinds ??= new byte[AsciiLimit];
                if (_asciiKinds[symbol] == 0)
                {
                    if (target == null) DeadCount++; else LiveCount++;
                }
                _asciiTargets[symbol] = target;
                _asciiKinds[symbol] = target == null ? (byte)2 : (byte)1;
                return;
            }

            _sparse ??= new Dictionary<int, DfaState>();
            if (!_sparse.ContainsKey(symbol))
            {
                if (target == null) DeadCount++; else LiveCount++;
            }
            _sparse[symbol] = target;
        }

        public long EstimatedRetainedBytes =>
            (HasDenseRow ? 2 * 24L + AsciiLimit * (IntPtr.Size + 1L) : 0) +
            (SparseCount == 0 ? 0 : 48L + SparseCount * 32L);
    }

    private sealed class DfaState
    {
        public readonly HashSet<LexerConfig> Configs;
        public readonly DfaEdgeTable Edges = new();

        public DfaState(HashSet<LexerConfig> configs) => Configs = configs;
    }

    private readonly DfaState[] _modeStartStates;
    private readonly List<DfaState> _dfaStates = new();
    private readonly HashSet<NonGreedyAccept> _nonGreedyAcceptWork =
        new(NonGreedyAcceptEq.Instance);

    internal int DfaStateCount => _dfaStates.Count;
    internal int LexerContextCount => _contextCache.Count;
    internal long DfaEdgeCacheHits { get; private set; }
    internal long DfaEdgeCacheMisses { get; private set; }

    public LexerDfaStatistics GetDfaStatistics()
    {
        var live = _dfaStates.Sum(state => state.Edges.LiveCount);
        var dead = _dfaStates.Sum(state => state.Edges.DeadCount);
        var denseRows = _dfaStates.Count(state => state.Edges.HasDenseRow);
        var sparseEntries = _dfaStates.Sum(state => state.Edges.SparseCount);
        var maximumConfigurations = _dfaStates.Count == 0
            ? 0 : _dfaStates.Max(state => state.Configs.Count);
        var estimatedBytes = _dfaStates.Sum(state =>
            64L + state.Configs.Count * 64L + state.Edges.EstimatedRetainedBytes) +
            _contextCache.Count * 32L;
        return new LexerDfaStatistics(
            _dfaStates.Count, live, dead, denseRows, sparseEntries,
            DfaEdgeCacheHits, DfaEdgeCacheMisses, maximumConfigurations,
            estimatedBytes);
    }

    private DfaState GetModeStartState(int mode)
    {
        if (!_enableDfa)
        {
            var uncachedConfigs = new HashSet<LexerConfig>(LexerConfigEq.Instance)
            {
                new LexerConfig(_atn.modeToStartState[mode], LexStack.Empty,
                    0, -1, -1, LexStack.Empty, false)
            };
            EpsClosure(uncachedConfigs);
            return new DfaState(uncachedConfigs);
        }
        var cached = _modeStartStates[mode];
        if (cached != null) return cached;

        var configs = NewDfaConfigSet();
        configs.Add(new LexerConfig(
            _atn.modeToStartState[mode], LexStack.Empty,
            0, -1, -1, LexStack.Empty, false));
        EpsClosure(configs);
        cached = InternDfaState(configs);
        _modeStartStates[mode] = cached;
        return cached;
    }

    private DfaState GetTargetState(DfaState source, int symbol)
    {
        if (!_enableDfa)
        {
            var uncachedConfigs = Scan(source.Configs, symbol);
            if (uncachedConfigs.Count == 0) return null;
            EpsClosure(uncachedConfigs);
            PruneAfterNonGreedyAccept(uncachedConfigs);
            return uncachedConfigs.Count == 0 ? null : new DfaState(uncachedConfigs);
        }
        if (source.Edges.TryGet(symbol, out var cached))
        {
            DfaEdgeCacheHits++;
            return cached;
        }

        DfaEdgeCacheMisses++;
        var targetConfigs = Scan(source.Configs, symbol);
        if (targetConfigs.Count == 0)
        {
            source.Edges.Set(symbol, null);
            return null;
        }

        EpsClosure(targetConfigs);
        PruneAfterNonGreedyAccept(targetConfigs);
        if (targetConfigs.Count == 0)
        {
            source.Edges.Set(symbol, null);
            return null;
        }

        var target = InternDfaState(targetConfigs);
        source.Edges.Set(symbol, target);
        return target;
    }

    private HashSet<LexerConfig> NewDfaConfigSet() =>
        new(DfaLexerConfigEq.Instance);

    private DfaState InternDfaState(HashSet<LexerConfig> configs)
    {
        foreach (var state in _dfaStates)
            if (state.Configs.SetEquals(configs))
                return state;

        var created = new DfaState(configs);
        _dfaStates.Add(created);
        return created;
    }

    private void PruneAfterNonGreedyAccept(HashSet<LexerConfig> configs)
    {
        var accepts = _nonGreedyAcceptWork;
        accepts.Clear();
        foreach (var config in configs)
        {
            if (config.State.stateType == Atn.MyStateType.RuleStop &&
                config.Stack.IsEmpty && config.NonGreedyDecision >= 0)
            {
                accepts.Add(new NonGreedyAccept(
                    config.State.ruleIndex, config.NonGreedyDecision,
                    config.NonGreedyContext));
            }
        }
        if (accepts.Count == 0) return;
        configs.RemoveWhere(config =>
            config.State.stateType != Atn.MyStateType.RuleStop &&
            IsLowerPriorityNonGreedyPath(config, accepts));
    }

    private static bool IsLowerPriorityNonGreedyPath(
        LexerConfig config, HashSet<NonGreedyAccept> accepts)
    {
        foreach (var accept in accepts)
            if (accept.Rule == config.OuterRule &&
                accept.Decision == config.NonGreedyDecision &&
                accept.Context.Id == config.NonGreedyContext.Id &&
                (!accept.Context.IsEmpty ||
                 (config.Stack.IsEmpty && !config.CompletedInnerRule)))
                return true;
        return false;
    }

    private sealed class DfaLexerConfigEq : IEqualityComparer<LexerConfig>
    {
        public static readonly DfaLexerConfigEq Instance = new();

        public bool Equals(LexerConfig x, LexerConfig y) =>
            ReferenceEquals(x.State, y.State) &&
            x.Stack.Id == y.Stack.Id &&
            x.Actions == y.Actions &&
            x.OuterRule == y.OuterRule &&
            x.NonGreedyDecision == y.NonGreedyDecision &&
            x.NonGreedyContext.Id == y.NonGreedyContext.Id &&
            x.CompletedInnerRule == y.CompletedInnerRule;

        public int GetHashCode(LexerConfig config)
        {
            unchecked
            {
                int hash = config.State.stateNumber * 31 +
                    config.Stack.Id;
                hash = hash * 31 + config.Actions;
                hash = hash * 31 + config.OuterRule;
                hash = hash * 31 + config.NonGreedyDecision;
                hash = hash * 31 + config.NonGreedyContext.Id;
                hash = hash * 31 + (config.CompletedInnerRule ? 1 : 0);
                return hash;
            }
        }
    }
}
