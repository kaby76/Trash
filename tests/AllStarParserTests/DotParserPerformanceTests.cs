using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using AllStarAtnParser;
using Atn;
using EarleyAtnParser;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

/// <summary>
/// Parser-only baseline for issue #713. Input loading, .interp parsing, ATN
/// deserialization, and lexing are completed before every measured region.
/// Each measured parse receives the same retained token list.
/// </summary>
public sealed class DotParserPerformanceTests(ITestOutputHelper output)
{
    private const int SampleSize = 5;
    private static readonly string DotData = Path.Combine(
        AppContext.BaseDirectory, "TestData", "dot");
    private static readonly string ParserInterpPath = Path.Combine(
        DotData, "DOTParser.interp");
    private static readonly string LexerInterpPath = Path.Combine(
        DotData, "DOTLexer.interp");

    [Fact]
    [Trait("Category", "Performance")]
    public void GeneratedDotSeparatesRecognitionEventsAndTreeBuilding()
    {
        var fixture = Prepare(GenerateDotInput(8_000));
        Assert.True(fixture.OnChannelTokenCount >= 50_000);

        ReportStatistics(fixture);

        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
        var recognition = Measure(() =>
        {
            Assert.True(AllStarParser.Recognize(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
            return null;
        }, fixture.OnChannelTokenCount);

        Assert.NotNull(AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
        var withEvents = Measure(() =>
            AllStarParser.Parse(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule),
            fixture.OnChannelTokenCount);

        var events = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule);
        Assert.NotNull(events);
        var treeBuilding = Measure(() => DomBuilder.Build(
            events, fixture.Tokens,
            fixture.ParserInterp.RuleNames,
            fixture.ParserInterp.SymbolicNames,
            fixture.ParserInterp.LiteralNames,
            fixture.LexerInterp.RuleNames,
            lineNumbers: false), fixture.OnChannelTokenCount);

        Report("Generated DOT recognition only", recognition);
        Report("Generated DOT parser + events", withEvents);
        Report("Generated DOT tree building only", treeBuilding);

        Assert.True(Statistics(recognition.Select(s => s.TokensPerSecond)).Mean >= 25_000,
            "Generated DOT recognition fell below the conservative parser baseline.");
    }

    [Fact]
    [Trait("Category", "Performance")]
    [Trait("Size", "Large")]
    public void Graphviz1864DotMaintainsParserThroughput()
    {
        var loadWatch = Stopwatch.StartNew();
        var compressedPath = Path.Combine(DotData, "1864.dot.gz");
        using var compressed = File.OpenRead(compressedPath);
        using var gzip = new GZipStream(compressed, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        var input = reader.ReadToEnd();
        loadWatch.Stop();

        var setupWatch = Stopwatch.StartNew();
        var fixture = Prepare(input);
        setupWatch.Stop();
        Assert.Equal(1_109_453, fixture.Tokens.Count);

        ReportStatistics(fixture);

        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
        var samples = Measure(() =>
        {
            Assert.True(AllStarParser.Recognize(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
            return null;
        }, fixture.OnChannelTokenCount);

        Report("1864.dot recognition only", samples);
        output.WriteLine(
            $"Fixture read/decompression: {loadWatch.Elapsed.TotalMilliseconds:F3} ms; " +
            $"interp/ATN/token setup: {setupWatch.Elapsed.TotalMilliseconds:F3} ms " +
            "(both excluded from parser samples)");

        var throughput = Statistics(samples.Select(s => s.TokensPerSecond));
        var bytesPerToken = Statistics(samples.Select(s => s.BytesPerToken));
        Assert.True(throughput.Mean >= 25_000,
            $"1864.dot parser mean throughput {throughput.Mean:N0} tokens/s is below " +
            "the conservative 25,000 tokens/s baseline.");
        Assert.True(bytesPerToken.Mean < 20_000,
            $"1864.dot parser allocated {bytesPerToken.Mean:F1} bytes/token; " +
            "expected less than the conservative 20,000-byte baseline.");
    }

    private static DotFixture Prepare(string input)
    {
        var parserInterp = InterpFileReader.Read(File.ReadAllText(ParserInterpPath));
        var lexerInterp = InterpFileReader.Read(File.ReadAllText(LexerInterpPath));
        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn = AtnDeserializer.Deserialize(lexerInterp.AtnData);
        var tokens = new LexerAtnSimulator(lexerAtn).Tokenize(input);
        var startRule = Array.FindIndex(parserAtn.start,
            state => state.stateNumber == parserInterp.StartStateNumber);
        Assert.True(startRule >= 0,
            $"DOT parser start state {parserInterp.StartStateNumber} was not found.");
        var onChannelTokenCount = tokens.Count(token =>
            token.Channel == 0 || token.Type == -1);
        return new DotFixture(
            parserAtn, parserInterp, lexerInterp, tokens,
            startRule, onChannelTokenCount);
    }

    [Fact]
    public void DotParserStatisticsDescribePredictionAndCommittedWork()
    {
        var fixture = Prepare(GenerateDotInput(100));
        var statistics = new ParserStatistics();

        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule, statistics));

        Assert.True(statistics.AdaptivePredictionCalls > 0);
        Assert.Equal(statistics.AdaptivePredictionCalls,
            statistics.LookaheadHistogram.Sum());
        Assert.True(statistics.DfaEdgeHits + statistics.DfaEdgeMisses > 0);
        Assert.True(statistics.DfaStatesCreated > 0);
        Assert.True(statistics.PredictionLookaheadTokens > 0);
        Assert.True(statistics.ClosureConfigurationsVisited > 0);
        Assert.True(statistics.ReachConfigurationsExamined > 0);
        Assert.True(statistics.MaximumConfigurationsPerSet > 0);
        Assert.True(statistics.RetainedDfaStates > 0);
        Assert.True(statistics.EstimatedRetainedBytes > 0);
        Assert.True(statistics.CommittedAtnStatesVisited > 0);
        Assert.True(statistics.RuleCalls > 0);
        Assert.Equal(0, statistics.ParseEventsCreated);
        Assert.NotEmpty(statistics.Decisions);
        Assert.Contains("busiest decisions", statistics.Format());
    }

    [Fact]
    public void DotParserStatisticsCountConstructedEvents()
    {
        var fixture = Prepare(GenerateDotInput(10));
        var statistics = new ParserStatistics();

        var events = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule, statistics);

        Assert.NotNull(events);
        Assert.Equal(events.Count, statistics.ParseEventsCreated);
    }

    [Fact]
    public void DotLl1BypassMatchesAllStarSimulation()
    {
        var fixture = Prepare(GenerateDotInput(100));
        var statistics = new ParserStatistics();

        var bypassEvents = AllStarParser.ParseWithLl1(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            statistics);
        var simulatorEvents = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule);

        Assert.NotNull(bypassEvents);
        Assert.Equal(simulatorEvents, bypassEvents);
        Assert.True(statistics.Ll1Bypasses > 0);
        Assert.True(statistics.AdaptivePredictionCalls > 0);
    }

    [Fact]
    [Trait("Category", "Performance")]
    public void GeneratedDotReportsLl1BypassPerformance()
    {
        var fixture = Prepare(GenerateDotInput(8_000));
        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
        Assert.True(AllStarParser.RecognizeWithLl1(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule));

        var simulator = Measure(() =>
        {
            Assert.True(AllStarParser.Recognize(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
            return null;
        }, fixture.OnChannelTokenCount);
        var bypass = Measure(() =>
        {
            Assert.True(AllStarParser.RecognizeWithLl1(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule));
            return null;
        }, fixture.OnChannelTokenCount);

        Report("Generated DOT without LL(1) bypass", simulator);
        Report("Generated DOT with LL(1) bypass", bypass);
    }

    [Fact]
    public void SharedDotDfaReusesStatesAndCanBeCleared()
    {
        var fixture = Prepare(GenerateDotInput(100));
        var cache = new ParserPredictionCache();
        var coldStatistics = new ParserStatistics();
        var coldEvents = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            coldStatistics, cache);

        var warmStatistics = new ParserStatistics();
        var warmEvents = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            warmStatistics, cache);

        Assert.NotNull(coldEvents);
        Assert.Equal(coldEvents, warmEvents);
        Assert.Equal(0, coldStatistics.SharedDfaStatesAtStart);
        Assert.True(warmStatistics.SharedDfaStatesAtStart > 0);
        Assert.True(coldStatistics.DfaEdgeMisses > 0);
        Assert.Equal(0, warmStatistics.DfaEdgeMisses);
        Assert.True(warmStatistics.DfaEdgeHits > 0);
        Assert.Equal(0, warmStatistics.DfaStatesCreated);
        Assert.True(warmStatistics.ClosureConfigurationsVisited <
            coldStatistics.ClosureConfigurationsVisited);

        cache.Clear();
        Assert.Equal(0, cache.RetainedStates);
        Assert.Equal(0, cache.RetainedTransitions);
        var clearedStatistics = new ParserStatistics();
        var clearedEvents = AllStarParser.Parse(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            clearedStatistics, cache);
        Assert.Equal(coldEvents, clearedEvents);
        Assert.Equal(0, clearedStatistics.SharedDfaStatesAtStart);
        Assert.True(clearedStatistics.DfaEdgeMisses > 0);
    }

    [Fact]
    public void SharedDotParserInternsCommittedCallerContexts()
    {
        var fixture = Prepare(GenerateDotInput(100));
        var cache = new ParserPredictionCache();
        var cold = new ParserStatistics();
        var warm = new ParserStatistics();

        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule, cold, cache));
        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule, warm, cache));

        Assert.True(cold.PredictionContextCreations > 0);
        Assert.True(warm.PredictionContextCacheHits > 0);
        Assert.Equal(0, warm.PredictionContextCreations);
        Assert.Equal(cold.RetainedPredictionContexts,
            warm.RetainedPredictionContexts);
    }

    [Fact]
    public void SharedDotDfaHonorsItsStateBudget()
    {
        var fixture = Prepare(GenerateDotInput(100));
        var cache = new ParserPredictionCache(
            maximumStates: 1, maximumEstimatedBytes: 1_000_000);
        var statistics = new ParserStatistics();

        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            statistics, cache));

        Assert.True(cache.IsSaturated);
        Assert.True(cache.RetainedStates <= 1);
        Assert.True(statistics.SharedDfaCacheSaturated);
    }

    [Fact]
    public void SharedDfaIsDisabledForSemanticPredicateAtn()
    {
        var source = new MyATNState { stateNumber = 0 };
        var target = new MyATNState { stateNumber = 1 };
        source.AddTransition(new MyPredicateTransition(
            target, ruleIndex: 0, predIndex: 0, isCtxDependent: true));
        var atn = new MyATN { allStates = new[] { source, target } };
        var cache = new ParserPredictionCache();

        _ = new AllStarSimulator(atn, predictionCache: cache);

        Assert.False(cache.SharingEnabled);
        Assert.Contains("semantic predicate", cache.SharingDisabledReason);
    }

    [Fact]
    [Trait("Category", "Performance")]
    public void GeneratedDotReportsColdAndWarmSharedDfaPerformance()
    {
        var fixture = Prepare(GenerateDotInput(8_000));
        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            predictionCache: new ParserPredictionCache()));
        var cold = Measure(() =>
        {
            Assert.True(AllStarParser.Recognize(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
                predictionCache: new ParserPredictionCache()));
            return null;
        }, fixture.OnChannelTokenCount);

        var shared = new ParserPredictionCache();
        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
            predictionCache: shared));
        var warm = Measure(() =>
        {
            Assert.True(AllStarParser.Recognize(
                fixture.ParserAtn, fixture.Tokens, fixture.StartRule,
                predictionCache: shared));
            return null;
        }, fixture.OnChannelTokenCount);

        Report("Generated DOT cold DFA", cold);
        Report("Generated DOT warm shared DFA", warm);
    }

    private void ReportStatistics(DotFixture fixture)
    {
        var statistics = new ParserStatistics();
        Assert.True(AllStarParser.Recognize(
            fixture.ParserAtn, fixture.Tokens, fixture.StartRule, statistics));
        output.WriteLine(statistics.Format());
        output.WriteLine(
            "Statistics collection is a separate warm-up run and is excluded " +
            "from the five timed samples.");
    }

    private static ParserSample[] Measure(
        Func<object?> operation, int tokenCount)
    {
        var samples = new ParserSample[SampleSize];
        for (var i = 0; i < samples.Length; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var gen0Before = GC.CollectionCount(0);
            var gen1Before = GC.CollectionCount(1);
            var gen2Before = GC.CollectionCount(2);
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            var result = operation();
            stopwatch.Stop();
            var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

            samples[i] = new ParserSample(
                stopwatch.Elapsed.TotalSeconds, tokenCount, allocated,
                GC.CollectionCount(0) - gen0Before,
                GC.CollectionCount(1) - gen1Before,
                GC.CollectionCount(2) - gen2Before);
            GC.KeepAlive(result);
        }
        return samples;
    }

    private void Report(string name, ParserSample[] samples)
    {
        var elapsed = Statistics(samples.Select(s => s.ElapsedSeconds));
        var throughput = Statistics(samples.Select(s => s.TokensPerSecond));
        var allocation = Statistics(samples.Select(s => s.BytesPerToken));
        output.WriteLine(
            $"{name}: {samples[0].TokenCount:N0} on-channel tokens, n={samples.Length}");
        output.WriteLine(
            $"Elapsed: {elapsed.Mean:F6} +/- {elapsed.Sem:F6} s SEM " +
            $"(SD {elapsed.StandardDeviation:F6}, range " +
            $"{elapsed.Minimum:F6}-{elapsed.Maximum:F6})");
        output.WriteLine(
            $"Throughput: {throughput.Mean:N0} +/- {throughput.Sem:N0} tokens/s SEM " +
            $"(SD {throughput.StandardDeviation:N0}, range " +
            $"{throughput.Minimum:N0}-{throughput.Maximum:N0})");
        output.WriteLine(
            $"Allocation: {allocation.Mean:F1} +/- {allocation.Sem:F1} bytes/token SEM " +
            $"(SD {allocation.StandardDeviation:F1}, range " +
            $"{allocation.Minimum:F1}-{allocation.Maximum:F1}; current thread)");
        output.WriteLine(
            $"GC collections per sample (Gen0/Gen1/Gen2): " +
            $"{samples.Average(s => s.Gen0Collections):F2}/" +
            $"{samples.Average(s => s.Gen1Collections):F2}/" +
            $"{samples.Average(s => s.Gen2Collections):F2}");
    }

    private static SampleStatistics Statistics(IEnumerable<double> values)
    {
        var sample = values.ToArray();
        var mean = sample.Average();
        var squaredDeviations = sample.Sum(value =>
            (value - mean) * (value - mean));
        var standardDeviation = sample.Length > 1
            ? Math.Sqrt(squaredDeviations / (sample.Length - 1))
            : 0;
        return new SampleStatistics(
            mean, standardDeviation, standardDeviation / Math.Sqrt(sample.Length),
            sample.Min(), sample.Max());
    }

    private static string GenerateDotInput(int edgeCount)
    {
        var text = new StringBuilder(edgeCount * 50);
        text.AppendLine("digraph benchmark {");
        for (var i = 0; i < edgeCount; i++)
            text.Append('n').Append(i).Append(" -> n").Append(i + 1)
                .Append(" [label=\"edge").Append(i).AppendLine("\"];");
        return text.AppendLine("}").ToString();
    }

    private sealed record DotFixture(
        MyATN ParserAtn,
        ParsedInterp ParserInterp,
        ParsedInterp LexerInterp,
        List<LexerToken> Tokens,
        int StartRule,
        int OnChannelTokenCount);

    private readonly record struct ParserSample(
        double ElapsedSeconds,
        int TokenCount,
        long AllocatedBytes,
        int Gen0Collections,
        int Gen1Collections,
        int Gen2Collections)
    {
        public double TokensPerSecond => TokenCount / ElapsedSeconds;
        public double BytesPerToken => (double)AllocatedBytes / TokenCount;
    }

    private readonly record struct SampleStatistics(
        double Mean,
        double StandardDeviation,
        double Sem,
        double Minimum,
        double Maximum);
}
