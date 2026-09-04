namespace EarleyAtnParser;

/// <summary>Learned, per-mode DFA support for <see cref="LexerAtnSimulator"/>.</summary>
public partial class LexerAtnSimulator
{
    private sealed class DfaState
    {
        public readonly HashSet<LexerConfig> Configs;
        public readonly Dictionary<int, DfaState> Edges = new();
        public readonly HashSet<int> DeadEdges = new();

        public DfaState(HashSet<LexerConfig> configs) => Configs = configs;
    }

    private readonly DfaState[] _modeStartStates;
    private readonly List<DfaState> _dfaStates = new();

    internal int DfaStateCount => _dfaStates.Count;
    internal int LexerContextCount => _contextCache.Count;
    internal long DfaEdgeCacheHits { get; private set; }
    internal long DfaEdgeCacheMisses { get; private set; }

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
        if (source.Edges.TryGetValue(symbol, out var cached))
        {
            DfaEdgeCacheHits++;
            return cached;
        }
        if (source.DeadEdges.Contains(symbol))
        {
            DfaEdgeCacheHits++;
            return null;
        }

        DfaEdgeCacheMisses++;
        var targetConfigs = Scan(source.Configs, symbol);
        if (targetConfigs.Count == 0)
        {
            source.DeadEdges.Add(symbol);
            return null;
        }

        EpsClosure(targetConfigs);
        targetConfigs = Canonicalize(targetConfigs);
        PruneAfterNonGreedyAccept(targetConfigs);
        if (targetConfigs.Count == 0)
        {
            source.DeadEdges.Add(symbol);
            return null;
        }

        var target = InternDfaState(targetConfigs);
        source.Edges[symbol] = target;
        return target;
    }

    private HashSet<LexerConfig> NewDfaConfigSet() =>
        new(DfaLexerConfigEq.Instance);

    private HashSet<LexerConfig> Canonicalize(IEnumerable<LexerConfig> configs) =>
        new(configs, DfaLexerConfigEq.Instance);

    private DfaState InternDfaState(HashSet<LexerConfig> configs)
    {
        foreach (var state in _dfaStates)
            if (state.Configs.SetEquals(configs))
                return state;

        var created = new DfaState(configs);
        _dfaStates.Add(created);
        return created;
    }

    private static void PruneAfterNonGreedyAccept(HashSet<LexerConfig> configs)
    {
        HashSet<NonGreedyAccept> accepts = null;
        foreach (var config in configs)
        {
            if (config.State.stateType == Atn.MyStateType.RuleStop &&
                config.Stack.IsEmpty && config.NonGreedyDecision >= 0)
            {
                accepts ??= new(NonGreedyAcceptEq.Instance);
                accepts.Add(new NonGreedyAccept(
                    config.State.ruleIndex, config.NonGreedyDecision,
                    config.NonGreedyContext));
            }
        }
        if (accepts == null) return;
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
