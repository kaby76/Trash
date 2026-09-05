namespace AllStarAtnParser;

/// <summary>Opt-in counters for interpreted ALL(*) prediction and parsing.</summary>
public sealed class ParserStatistics
{
    private readonly Dictionary<int, DecisionStatistics> _decisions = new();
    private readonly long[] _lookaheadHistogram = new long[6];

    public long AdaptivePredictionCalls { get; private set; }
    public long Ll1Bypasses { get; internal set; }
    public long DfaEdgeHits { get; internal set; }
    public long DfaEdgeMisses { get; internal set; }
    public long DfaStatesCreated { get; internal set; }
    public long DfaStatesDeduplicated { get; internal set; }
    public long PredictionLookaheadTokens { get; internal set; }
    public int MaximumPredictionLookahead { get; private set; }
    public long ClosureConfigurationsVisited { get; internal set; }
    public long ReachConfigurationsExamined { get; internal set; }
    public long PredictionContextCreations { get; internal set; }
    public long PredictionContextCacheHits { get; internal set; }
    public long PredictionContextMerges { get; internal set; }
    public long PredictionContextMergeCacheHits { get; internal set; }
    public long SllConflicts { get; internal set; }
    public long FullContextFallbacks { get; internal set; }
    public long FullContextLookaheadTokens { get; internal set; }
    public long FullContextMemoHits { get; internal set; }
    public long FullContextMemoMisses { get; internal set; }
    public int MaximumConfigurationsPerSet { get; internal set; }
    public int RetainedDfaStates { get; internal set; }
    public int RetainedDfaTransitions { get; internal set; }
    public int RetainedPredictionContexts { get; internal set; }
    public long EstimatedRetainedBytes { get; internal set; }
    public long CommittedAtnStatesVisited { get; internal set; }
    public long RuleCalls { get; internal set; }
    public long ParseEventsCreated { get; internal set; }

    public IReadOnlyDictionary<int, DecisionStatistics> Decisions => _decisions;
    public IReadOnlyList<long> LookaheadHistogram => _lookaheadHistogram;
    public double MeanPredictionLookahead => AdaptivePredictionCalls == 0
        ? 0 : (double)PredictionLookaheadTokens / AdaptivePredictionCalls;

    internal long BeginPrediction(int decision)
    {
        AdaptivePredictionCalls++;
        GetDecision(decision).Calls++;
        return PredictionLookaheadTokens;
    }

    internal void EndPrediction(int decision, long lookaheadAtStart)
    {
        var lookahead = checked((int)(PredictionLookaheadTokens - lookaheadAtStart));
        MaximumPredictionLookahead = Math.Max(MaximumPredictionLookahead, lookahead);
        _lookaheadHistogram[LookaheadBucket(lookahead)]++;
        var item = GetDecision(decision);
        item.LookaheadTokens += lookahead;
        item.MaximumLookahead = Math.Max(item.MaximumLookahead, lookahead);
    }

    internal void RecordDfaHit(int decision)
    {
        DfaEdgeHits++;
        GetDecision(decision).DfaEdgeHits++;
    }

    internal void RecordDfaMiss(int decision)
    {
        DfaEdgeMisses++;
        GetDecision(decision).DfaEdgeMisses++;
    }

    internal void RecordFullContextFallback(int decision)
    {
        FullContextFallbacks++;
        GetDecision(decision).FullContextFallbacks++;
    }

    internal void ObserveConfigurations(int count) =>
        MaximumConfigurationsPerSet = Math.Max(MaximumConfigurationsPerSet, count);

    private DecisionStatistics GetDecision(int decision)
    {
        if (!_decisions.TryGetValue(decision, out var result))
        {
            result = new DecisionStatistics();
            _decisions.Add(decision, result);
        }
        return result;
    }

    private static int LookaheadBucket(int count) => count switch
    {
        0 => 0,
        1 => 1,
        <= 4 => 2,
        <= 16 => 3,
        <= 64 => 4,
        _ => 5
    };

    public string Format(string prefix = "")
    {
        var lines = new List<string>
        {
            $"{prefix}Parser statistics:",
            $"{prefix}adaptive predictions: {AdaptivePredictionCalls:N0}",
            $"{prefix}LL(1) bypasses: {Ll1Bypasses:N0}",
            $"{prefix}DFA edges: {DfaEdgeHits:N0} hits, {DfaEdgeMisses:N0} misses",
            $"{prefix}DFA learning: {DfaStatesCreated:N0} states created, " +
                $"{DfaStatesDeduplicated:N0} deduplicated",
            $"{prefix}lookahead: mean {MeanPredictionLookahead:F3}, " +
                $"maximum {MaximumPredictionLookahead:N0}, total {PredictionLookaheadTokens:N0}",
            $"{prefix}lookahead histogram (0/1/2-4/5-16/17-64/65+): " +
                string.Join("/", _lookaheadHistogram.Select(value => value.ToString("N0"))),
            $"{prefix}ATN work: {ClosureConfigurationsVisited:N0} closure configurations, " +
                $"{ReachConfigurationsExamined:N0} reach configurations",
            $"{prefix}prediction contexts: {PredictionContextCreations:N0} created, " +
                $"{PredictionContextCacheHits:N0} cache hits, " +
                $"{PredictionContextMerges:N0} merges, " +
                $"{PredictionContextMergeCacheHits:N0} merge-cache hits",
            $"{prefix}full context: {SllConflicts:N0} SLL conflicts, " +
                $"{FullContextFallbacks:N0} fallbacks, " +
                $"{FullContextLookaheadTokens:N0} lookahead tokens",
            $"{prefix}full-context memo: {FullContextMemoHits:N0} hits, " +
                $"{FullContextMemoMisses:N0} misses",
            $"{prefix}configuration maximum: {MaximumConfigurationsPerSet:N0}",
            $"{prefix}retained parser DFA: {RetainedDfaStates:N0} states, " +
                $"{RetainedDfaTransitions:N0} transitions, " +
                $"{RetainedPredictionContexts:N0} contexts, " +
                $"~{EstimatedRetainedBytes:N0} bytes",
            $"{prefix}committed parser: {CommittedAtnStatesVisited:N0} ATN states, " +
                $"{RuleCalls:N0} rule calls, {ParseEventsCreated:N0} parse events"
        };

        var busiest = _decisions
            .OrderByDescending(pair => pair.Value.Calls)
            .ThenBy(pair => pair.Key)
            .Take(10)
            .ToList();
        if (busiest.Count > 0)
        {
            lines.Add($"{prefix}busiest decisions " +
                "(decision:calls,hits,misses,LL-fallbacks,mean/max-lookahead):");
            lines.AddRange(busiest.Select(pair =>
                $"{prefix}  {pair.Key}:{pair.Value.Calls:N0}," +
                $"{pair.Value.DfaEdgeHits:N0},{pair.Value.DfaEdgeMisses:N0}," +
                $"{pair.Value.FullContextFallbacks:N0}," +
                $"{pair.Value.MeanLookahead:F2}/{pair.Value.MaximumLookahead:N0}"));
        }
        return string.Join(Environment.NewLine, lines);
    }
}

public sealed class DecisionStatistics
{
    public long Calls { get; internal set; }
    public long DfaEdgeHits { get; internal set; }
    public long DfaEdgeMisses { get; internal set; }
    public long FullContextFallbacks { get; internal set; }
    public long LookaheadTokens { get; internal set; }
    public int MaximumLookahead { get; internal set; }
    public double MeanLookahead => Calls == 0 ? 0 : (double)LookaheadTokens / Calls;
}
