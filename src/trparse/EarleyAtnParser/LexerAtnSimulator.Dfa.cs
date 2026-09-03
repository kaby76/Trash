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
    internal long DfaEdgeCacheHits { get; private set; }
    internal long DfaEdgeCacheMisses { get; private set; }

    private DfaState GetModeStartState(int mode)
    {
        if (!_enableDfa)
        {
            var uncachedConfigs = new HashSet<LexerConfig>(LexerConfigEq.Instance)
            {
                new LexerConfig(_atn.modeToStartState[mode], LexStack.Empty,
                    0, -1, -1, false, -1)
            };
            EpsClosure(uncachedConfigs);
            return new DfaState(uncachedConfigs);
        }
        var cached = _modeStartStates[mode];
        if (cached != null) return cached;

        var configs = NewDfaConfigSet();
        configs.Add(new LexerConfig(
            _atn.modeToStartState[mode], LexStack.Empty,
            0, -1, -1, false, -1));
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
        HashSet<(int Rule, int Decision)> accepts = null;
        foreach (var config in configs)
        {
            if (config.CompletedNonGreedyDecision >= 0)
            {
                accepts ??= new();
                accepts.Add((config.OuterRule, config.CompletedNonGreedyDecision));
            }
            if (config.State.stateType == Atn.MyStateType.RuleStop &&
                config.Stack.IsEmpty && config.NonGreedyDecision >= 0)
            {
                accepts ??= new();
                accepts.Add((config.State.ruleIndex, config.NonGreedyDecision));
            }
        }
        if (accepts == null) return;

        configs.RemoveWhere(config =>
            accepts.Contains((config.OuterRule, config.NonGreedyDecision)) &&
            config.State.stateType != Atn.MyStateType.RuleStop &&
            !config.CompletedInnerRule);
    }

    private sealed class DfaLexerConfigEq : IEqualityComparer<LexerConfig>
    {
        public static readonly DfaLexerConfigEq Instance = new();

        public bool Equals(LexerConfig x, LexerConfig y) =>
            ReferenceEquals(x.State, y.State) &&
            LexStack.StructuralSame(x.Stack, y.Stack) &&
            x.Actions == y.Actions &&
            x.OuterRule == y.OuterRule &&
            x.NonGreedyDecision == y.NonGreedyDecision &&
            x.CompletedInnerRule == y.CompletedInnerRule &&
            x.CompletedNonGreedyDecision == y.CompletedNonGreedyDecision;

        public int GetHashCode(LexerConfig config)
        {
            unchecked
            {
                int hash = config.State.stateNumber * 31 +
                    config.Stack.StructuralHashCode();
                hash = hash * 31 + config.Actions;
                hash = hash * 31 + config.OuterRule;
                hash = hash * 31 + config.NonGreedyDecision;
                hash = hash * 31 + (config.CompletedInnerRule ? 1 : 0);
                return hash * 31 + config.CompletedNonGreedyDecision;
            }
        }
    }
}
