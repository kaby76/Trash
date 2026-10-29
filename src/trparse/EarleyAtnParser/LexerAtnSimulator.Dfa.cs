namespace EarleyAtnParser;

using Atn;

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
        long FastPathRuns,
        long FastPathCharacters,
        int StatesAtStart,
        int TransitionsAtStart,
        int MaximumConfigurations,
        long EstimatedRetainedBytes);

    /// <summary>
    /// Grammar-scoped learned lexer DFA data shared by lexer instances in one
    /// command. Input text, cursor state, and per-file counters are not shared.
    /// </summary>
    public sealed class LexerDfaCache
    {
        internal MyATN _atn;
        internal object ModeStartStates;
        internal object DfaStates;
        internal object ContextCache;
        internal object ActionSets;

        internal void Bind(MyATN atn)
        {
            ArgumentNullException.ThrowIfNull(atn);
            if (_atn == null)
            {
                _atn = atn;
                ModeStartStates = new DfaState[atn.modeToStartState.Length];
                DfaStates = new List<DfaState>();
                ContextCache = new LexContextCache();
                ActionSets = new Dictionary<int, IMyLexerAction[]>();
            }
            else if (!ReferenceEquals(_atn, atn))
            {
                throw new InvalidOperationException(
                    "A lexer DFA cache cannot be shared by different ATN instances.");
            }
        }

        internal void Write(BinaryWriter writer)
        {
            var contexts = (LexContextCache)ContextCache;
            var states = (List<DfaState>)DfaStates;
            var starts = (DfaState[])ModeStartStates;
            writer.Write(contexts.Count);
            for (int id = 1; id <= contexts.Count; id++)
            {
                var context = contexts.GetById(id);
                writer.Write(context.Parent?.Id ?? 0);
                writer.Write(context.ReturnState.stateNumber);
            }
            var indices = new Dictionary<DfaState, int>(ReferenceEqualityComparer.Instance);
            for (int i = 0; i < states.Count; i++) indices.Add(states[i], i);
            writer.Write(states.Count);
            foreach (var state in states)
            {
                writer.Write(state.Configs.Count);
                foreach (var config in state.Configs)
                {
                    writer.Write(config.State.stateNumber);
                    writer.Write(config.Stack.Id);
                    writer.Write(config.Actions);
                    writer.Write(config.OuterRule);
                    writer.Write(config.NonGreedyDecision);
                    writer.Write(config.NonGreedyContext.Id);
                    writer.Write(config.NonGreedyBranch);
                    writer.Write(config.CompletedRule);
                }
                var edges = state.Edges.Entries().ToArray();
                writer.Write(edges.Length);
                foreach (var (symbol, target) in edges)
                {
                    writer.Write(symbol);
                    writer.Write(target == null ? -1 : indices[target]);
                }
            }
            writer.Write(starts.Length);
            foreach (var start in starts)
                writer.Write(start == null ? -1 : indices[start]);
        }

        internal void Read(BinaryReader reader, MyATN atn)
        {
            Bind(atn);
            var contexts = (LexContextCache)ContextCache;
            var states = (List<DfaState>)DfaStates;
            if (contexts.Count != 0 || states.Count != 0)
                throw new InvalidOperationException("The lexer DFA cache is already populated.");
            int contextCount = ReadCount(reader, 2_000_000);
            for (int id = 1; id <= contextCount; id++)
            {
                int parent = reader.ReadInt32();
                int returnState = reader.ReadInt32();
                if (parent < 0 || parent >= id ||
                    (uint)returnState >= (uint)atn.allStates.Length ||
                    atn.allStates[returnState] == null)
                    throw new InvalidDataException("Invalid lexer DFA context.");
                if (contexts.Push(contexts.GetStack(parent),
                    atn.allStates[returnState]).Id != id)
                    throw new InvalidDataException("Duplicate lexer DFA context.");
            }
            int stateCount = ReadCount(reader, 2_000_000);
            var edges = new List<(int Symbol, int Target)>[stateCount];
            for (int i = 0; i < stateCount; i++)
            {
                var configs = new HashSet<LexerConfig>(DfaLexerConfigEq.Instance);
                int configCount = ReadCount(reader, 1_000_000);
                for (int j = 0; j < configCount; j++)
                {
                    int number = reader.ReadInt32();
                    int stack = reader.ReadInt32();
                    int actions = reader.ReadInt32();
                    int outer = reader.ReadInt32();
                    int decision = reader.ReadInt32();
                    int nonGreedyContext = reader.ReadInt32();
                    int branch = reader.ReadInt32();
                    int completed = reader.ReadInt32();
                    if ((uint)number >= (uint)atn.allStates.Length ||
                        atn.allStates[number] == null ||
                        (uint)stack > contextCount ||
                        (uint)nonGreedyContext > contextCount)
                        throw new InvalidDataException("Invalid lexer DFA configuration.");
                    configs.Add(new LexerConfig(atn.allStates[number],
                        contexts.GetStack(stack), actions, outer, decision,
                        contexts.GetStack(nonGreedyContext), branch, completed));
                }
                states.Add(new DfaState(configs));
                int edgeCount = ReadCount(reader, 2_000_000);
                edges[i] = new List<(int, int)>(edgeCount);
                for (int j = 0; j < edgeCount; j++)
                {
                    int symbol = reader.ReadInt32();
                    int target = reader.ReadInt32();
                    if (symbol < -1 || target < -1 || target >= stateCount)
                        throw new InvalidDataException("Invalid lexer DFA edge.");
                    edges[i].Add((symbol, target));
                }
            }
            for (int i = 0; i < stateCount; i++)
                foreach (var (symbol, target) in edges[i])
                    states[i].Edges.Set(symbol, target < 0 ? null : states[target]);
            var starts = (DfaState[])ModeStartStates;
            if (reader.ReadInt32() != starts.Length)
                throw new InvalidDataException("Lexer DFA mode count does not match the grammar.");
            for (int i = 0; i < starts.Length; i++)
            {
                int index = reader.ReadInt32();
                if (index < -1 || index >= stateCount)
                    throw new InvalidDataException("Invalid lexer DFA start state.");
                starts[i] = index < 0 ? null : states[index];
            }
        }

        private static int ReadCount(BinaryReader reader, int maximum)
        {
            int count = reader.ReadInt32();
            if (count < 0 || count > maximum)
                throw new InvalidDataException("Invalid lexer DFA cache count.");
            return count;
        }
    }

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

        public IEnumerable<(int Symbol, DfaState Target)> Entries()
        {
            if (_asciiKinds != null)
                for (int i = 0; i < AsciiLimit; i++)
                    if (_asciiKinds[i] != 0)
                        yield return (i, _asciiTargets[i]);
            if (_sparse != null)
                foreach (var (symbol, target) in _sparse)
                    yield return (symbol, target);
        }

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

        public int ConsumeKnownAsciiSelfLoop(
            DfaState owner, string input, int position)
        {
            if (_asciiKinds == null) return position;
            while (position < input.Length)
            {
                var symbol = input[position];
                if (symbol >= AsciiLimit ||
                    _asciiKinds[symbol] != 1 ||
                    !ReferenceEquals(_asciiTargets[symbol], owner))
                    break;
                position++;
            }
            return position;
        }
    }

    private sealed class DfaState
    {
        public readonly HashSet<LexerConfig> Configs;
        public readonly DfaAccept[] Accepts;
        public readonly DfaEdgeTable Edges = new();

        public DfaState(HashSet<LexerConfig> configs)
        {
            Configs = configs;
            Accepts = configs
                .Where(config =>
                    config.State.stateType == Atn.MyStateType.RuleStop &&
                    config.Stack.IsEmpty)
                .Select(config => new DfaAccept(
                    config.State.ruleIndex, config.Actions))
                .Distinct()
                .OrderBy(accept => accept.Rule)
                .ThenBy(accept => accept.Actions)
                .ToArray();
        }
    }

    private readonly record struct DfaAccept(int Rule, int Actions);

    private readonly DfaState[] _modeStartStates;
    private readonly List<DfaState> _dfaStates;
    private readonly HashSet<NonGreedyAccept> _nonGreedyAcceptWork =
        new(NonGreedyAcceptEq.Instance);

    internal int DfaStateCount => _dfaStates.Count;
    internal int LexerContextCount => _contextCache.Count;
    internal long DfaEdgeCacheHits { get; private set; }
    internal long DfaEdgeCacheMisses { get; private set; }
    internal long DfaFastPathRuns { get; private set; }
    internal long DfaFastPathCharacters { get; private set; }
    private readonly int _dfaStatesAtStart;
    private readonly int _dfaTransitionsAtStart;

    private int CountDfaTransitions() =>
        _dfaStates.Sum(state => state.Edges.LiveCount + state.Edges.DeadCount);

    public LexerDfaStatistics GetDfaStatistics()
    {
        var live = _dfaStates.Sum(state => state.Edges.LiveCount);
        var dead = _dfaStates.Sum(state => state.Edges.DeadCount);
        var denseRows = _dfaStates.Count(state => state.Edges.HasDenseRow);
        var sparseEntries = _dfaStates.Sum(state => state.Edges.SparseCount);
        var maximumConfigurations = _dfaStates.Count == 0
            ? 0 : _dfaStates.Max(state => state.Configs.Count);
        var estimatedBytes = _dfaStates.Sum(state =>
            88L + state.Configs.Count * 64L + state.Accepts.Length * 8L +
            state.Edges.EstimatedRetainedBytes) +
            _contextCache.Count * 32L;
        return new LexerDfaStatistics(
            _dfaStates.Count, live, dead, denseRows, sparseEntries,
            DfaEdgeCacheHits, DfaEdgeCacheMisses,
            DfaFastPathRuns, DfaFastPathCharacters,
            _dfaStatesAtStart, _dfaTransitionsAtStart, maximumConfigurations,
            estimatedBytes);
    }

    private int ConsumeKnownAsciiSelfLoop(
        DfaState state, int position)
    {
        if (!_enableDfa) return position;
        var end = state.Edges.ConsumeKnownAsciiSelfLoop(state, _input, position);
        var consumed = end - position;
        if (consumed != 0)
        {
            DfaFastPathRuns++;
            DfaFastPathCharacters += consumed;
            DfaEdgeCacheHits += consumed;
        }
        return end;
    }

    private DfaState GetModeStartState(int mode)
    {
        if (!_enableDfa)
        {
            var uncachedConfigs = new HashSet<LexerConfig>(LexerConfigEq.Instance)
            {
                new LexerConfig(_atn.modeToStartState[mode], LexStack.Empty,
                    0, -1, -1, LexStack.Empty, -1, -1)
            };
            EpsClosure(uncachedConfigs);
            return new DfaState(uncachedConfigs);
        }
        var cached = _modeStartStates[mode];
        if (cached != null) return cached;

        var configs = NewDfaConfigSet();
        configs.Add(new LexerConfig(
            _atn.modeToStartState[mode], LexStack.Empty,
            0, -1, -1, LexStack.Empty, -1, -1));
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
        var completedRecursiveRules = configs
            .Where(config => config.CompletedRule == config.OuterRule)
            .Select(config => config.OuterRule)
            .ToHashSet();
        if (completedRecursiveRules.Count != 0)
        {
            configs.RemoveWhere(config =>
                config.CompletedRule != config.OuterRule &&
                completedRecursiveRules.Contains(config.OuterRule));
        }

        var accepts = _nonGreedyAcceptWork;
        accepts.Clear();
        foreach (var config in configs)
        {
            if (config.State.stateType == Atn.MyStateType.RuleStop &&
                config.Stack.IsEmpty && config.NonGreedyDecision >= 0)
            {
                accepts.Add(new NonGreedyAccept(
                    config.State.ruleIndex, config.NonGreedyDecision,
                    config.NonGreedyContext, config.NonGreedyBranch));
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
        bool competingBranchAccepted = false;
        foreach (var accept in accepts)
            if (accept.Rule == config.OuterRule &&
                accept.Decision == config.NonGreedyDecision &&
                accept.Context.Id == config.NonGreedyContext.Id)
            {
                // Preserve the selected branch while it remains inside the
                // rule context which owns the non-greedy decision. Once that
                // fragment has returned, its successful accept must prune
                // outer continuations just like ANTLR's ordered closure does.
                if ((accept.Branch == config.NonGreedyBranch ||
                     config.CompletedInnerRule) &&
                    accept.Context.Id == config.Stack.Id)
                    return false;
                competingBranchAccepted = true;
            }
        return competingBranchAccepted;
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
            x.NonGreedyBranch == y.NonGreedyBranch &&
            x.CompletedRule == y.CompletedRule;

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
                hash = hash * 31 + config.NonGreedyBranch;
                hash = hash * 31 + config.CompletedRule;
                return hash;
            }
        }
    }
}
