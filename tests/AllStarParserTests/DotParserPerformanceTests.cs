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
