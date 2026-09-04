using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Atn;
using EarleyAtnParser;
using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

/// <summary>
/// Repeatable lexer-only baseline for issue #712. The input is generated so a
/// multi-megabyte grammars-v4 fixture need not be checked into the repository.
/// Tokenize retains every token, matching the work required by trparse.
/// </summary>
public sealed class DotLexerPerformanceTests(ITestOutputHelper output)
{
    private const int SampleSize = 5;
    private static readonly string LexerInterp = Path.Combine(
        AppContext.BaseDirectory, "TestData", "dot", "DOTLexer.interp");

    [Fact]
    public void LearnedDfaReusesTransitionsAcrossTokens()
    {
        var interp = InterpFileReader.Read(File.ReadAllText(LexerInterp));
        var simulator = new LexerAtnSimulator(
            AtnDeserializer.Deserialize(interp.AtnData));

        var tokens = simulator.Tokenize(GenerateDotInput(100));

        Assert.Equal(-1, tokens[^1].Type);
        Assert.All(tokens.Where(token => token.Type != -1),
            token => Assert.False(token.IsTextMaterialized));
        var firstToken = tokens.First(token => token.Type != -1);
        Assert.Equal("digraph", firstToken.Text);
        Assert.True(firstToken.IsTextMaterialized);
        Assert.True(simulator.DfaStateCount > 0);
        Assert.True(simulator.DfaEdgeCacheMisses > 0);
        Assert.True(simulator.LexerContextCount > 0);
        var dfa = simulator.GetDfaStatistics();
        Assert.True(dfa.DenseAsciiRows > 0);
        Assert.True(dfa.LiveTransitions > 0);
        Assert.True(dfa.DeadTransitions > 0);
        Assert.True(dfa.MaximumConfigurations > 0);
        Assert.True(dfa.EstimatedRetainedBytes > 0);
        Assert.True(simulator.DfaEdgeCacheHits > simulator.DfaEdgeCacheMisses,
            $"Expected learned edges to dominate: {simulator.DfaEdgeCacheHits} hits, " +
            $"{simulator.DfaEdgeCacheMisses} misses.");
        output.WriteLine($"Learned lexer DFA: {simulator.DfaStateCount} states, " +
            $"{simulator.DfaEdgeCacheHits} edge hits, " +
            $"{simulator.DfaEdgeCacheMisses} edge misses");

        var hitsBeforeSecondInput = simulator.DfaEdgeCacheHits;
        var contextsBeforeSecondInput = simulator.LexerContextCount;
        _ = simulator.Tokenize("digraph second { alpha -> beta; }");
        Assert.True(simulator.DfaEdgeCacheHits > hitsBeforeSecondInput,
            "Expected the simulator to reuse its learned DFA on another input.");
        Assert.Equal(contextsBeforeSecondInput, simulator.LexerContextCount);
    }

    [Fact]
    public void PerformanceStatisticsUseSampleStandardDeviationAndSem()
    {
        var statistics = Statistics([1, 2, 3, 4, 5]);

        Assert.Equal(3, statistics.Mean);
        Assert.Equal(Math.Sqrt(2.5), statistics.StandardDeviation, 12);
        Assert.Equal(Math.Sqrt(0.5), statistics.Sem, 12);
        Assert.Equal(1, statistics.Minimum);
        Assert.Equal(5, statistics.Maximum);
    }

    [Fact]
    [Trait("Category", "Performance")]
    public void LargeGeneratedDotInputMaintainsLexerThroughput()
    {
        var input = GenerateDotInput(8_000);

        var stopwatch = Stopwatch.StartNew();
        var interpText = File.ReadAllText(LexerInterp);
        stopwatch.Stop();
        var fileRead = stopwatch.Elapsed;

        stopwatch.Restart();
        var interp = InterpFileReader.Read(interpText);
        stopwatch.Stop();
        var interpParse = stopwatch.Elapsed;

        stopwatch.Restart();
        var atn = AtnDeserializer.Deserialize(interp.AtnData);
        stopwatch.Stop();
        var deserialize = stopwatch.Elapsed;

        WarmUp(atn, input);
        var samples = Measure(atn, input, expectedTokenCount: null);
        var tokenCount = samples[0].TokenCount;
        Assert.True(tokenCount >= 100_000,
            $"Expected a representative token volume, but produced {tokenCount} tokens.");

        var throughput = Statistics(samples.Select(s => s.TokensPerSecond));
        var bytesPerToken = Statistics(samples.Select(s => s.BytesPerToken));
        Report("DOT lexer benchmark", tokenCount, input.Length, samples);
        output.WriteLine($"Setup: file read {fileRead.TotalMilliseconds:F3} ms; " +
            $"interp parse {interpParse.TotalMilliseconds:F3} ms; " +
            $"ATN deserialize {deserialize.TotalMilliseconds:F3} ms");

        // Deliberately conservative: this catches catastrophic regressions while
        // remaining stable on slower CI hosts. Reported numbers provide the
        // baseline for evaluating the DFA work in subsequent #712 subtasks.
        Assert.True(throughput.Mean >= 100_000,
            $"DOT lexer mean throughput {throughput.Mean:N0} tokens/s is below the " +
            "100,000 tokens/s learned-DFA regression floor.");
        Assert.True(bytesPerToken.Mean < 5_000,
            $"DOT lexer allocated a mean {bytesPerToken.Mean:F1} bytes/token; expected less " +
            "than 5,000 with learned DFA reuse.");
    }

    [Fact]
    [Trait("Category", "Performance")]
    [Trait("Size", "Large")]
    public void Graphviz1864DotMaintainsLexerThroughput()
    {
        var compressedPath = Path.Combine(
            AppContext.BaseDirectory, "TestData", "dot", "1864.dot.gz");
        var stopwatch = Stopwatch.StartNew();
        using var compressed = File.OpenRead(compressedPath);
        using var gzip = new GZipStream(compressed, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        var input = reader.ReadToEnd();
        stopwatch.Stop();
        var loadTime = stopwatch.Elapsed;

        var interp = InterpFileReader.Read(File.ReadAllText(LexerInterp));
        var atn = AtnDeserializer.Deserialize(interp.AtnData);
        WarmUp(atn, input);
        var samples = Measure(atn, input, expectedTokenCount: 1_109_453);
        var throughput = Statistics(samples.Select(s => s.TokensPerSecond));
        var bytesPerToken = Statistics(samples.Select(s => s.BytesPerToken));
        Report("1864.dot", samples[0].TokenCount, input.Length, samples);
        output.WriteLine($"Fixture decompression/read: {loadTime.TotalMilliseconds:F3} ms " +
            "(excluded from lexer time)");

        Assert.True(throughput.Mean >= 100_000,
            $"1864.dot lexer mean throughput {throughput.Mean:N0} tokens/s is below " +
            "the 100,000 tokens/s learned-DFA regression floor.");
        Assert.True(bytesPerToken.Mean < 5_000,
            $"1864.dot allocated a mean {bytesPerToken.Mean:F1} bytes/token; expected less " +
            "than 5,000 with learned DFA reuse.");
    }

    [Fact]
    public void AllStarRunReportsSeparateInterpPhases()
    {
        var timings = new AllStarAtnParser.InterpRunTimings();
        var interpDir = Path.Combine(AppContext.BaseDirectory, "TestData", "interp");
        var (result, tokenCount) = AllStarAtnParser.InterpRunner.Run(
            Path.Combine(interpDir, "Abnf.interp"),
            Path.Combine(interpDir, "AbnfLexer.interp"),
            "rule = %x41\r\n", "timing.abnf", false,
            timings: timings);

        Assert.NotNull(result);
        Assert.True(tokenCount > 0);
        Assert.True(timings.InterpFileReading >= TimeSpan.Zero);
        Assert.True(timings.InterpParsing >= TimeSpan.Zero);
        Assert.True(timings.AtnDeserialization >= TimeSpan.Zero);
        Assert.True(timings.Initialization >= TimeSpan.Zero);
        Assert.True(timings.Tokenization >= TimeSpan.Zero);
        Assert.True(timings.Parsing >= TimeSpan.Zero);
        Assert.True(timings.TreeBuilding >= TimeSpan.Zero);
        Assert.Contains("Tokenization:", timings.Format());
        Assert.NotNull(timings.LexerDfa);
        Assert.Contains("Lexer DFA storage:", timings.Format());
    }

    private static string GenerateDotInput(int edgeCount)
    {
        var text = new StringBuilder(edgeCount * 50);
        text.AppendLine("digraph benchmark {");
        for (var i = 0; i < edgeCount; i++)
            text.Append("n").Append(i).Append(" -> n").Append(i + 1)
                .Append(" [label=\"edge").Append(i).AppendLine("\"];");
        return text.AppendLine("}").ToString();
    }

    private static void WarmUp(MyATN atn, string input)
    {
        var tokens = new LexerAtnSimulator(atn).Tokenize(input);
        Assert.Equal(-1, tokens[^1].Type);
        GC.KeepAlive(tokens);
    }

    private static LexerSample[] Measure(
        MyATN atn, string input, int? expectedTokenCount)
    {
        var samples = new LexerSample[SampleSize];
        for (var i = 0; i < samples.Length; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            var tokens = new LexerAtnSimulator(atn).Tokenize(input);
            stopwatch.Stop();
            var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

            Assert.Equal(-1, tokens[^1].Type);
            if (expectedTokenCount.HasValue)
                Assert.Equal(expectedTokenCount.Value, tokens.Count);
            samples[i] = new LexerSample(
                stopwatch.Elapsed.TotalSeconds, tokens.Count, allocated);
            GC.KeepAlive(tokens);
        }
        return samples;
    }

    private void Report(
        string name, int tokenCount, int characterCount, LexerSample[] samples)
    {
        var elapsed = Statistics(samples.Select(s => s.ElapsedSeconds));
        var throughput = Statistics(samples.Select(s => s.TokensPerSecond));
        var bytesPerToken = Statistics(samples.Select(s => s.BytesPerToken));
        output.WriteLine($"{name}: {tokenCount:N0} retained tokens, " +
            $"{characterCount:N0} chars, n={samples.Length}");
        output.WriteLine($"Elapsed: {elapsed.Mean:F6} +/- {elapsed.Sem:F6} s SEM " +
            $"(SD {elapsed.StandardDeviation:F6}, range " +
            $"{elapsed.Minimum:F6}-{elapsed.Maximum:F6})");
        output.WriteLine($"Throughput: {throughput.Mean:N0} +/- {throughput.Sem:N0} " +
            $"tokens/s SEM (SD {throughput.StandardDeviation:N0}, range " +
            $"{throughput.Minimum:N0}-{throughput.Maximum:N0})");
        output.WriteLine($"Allocation: {bytesPerToken.Mean:F1} +/- " +
            $"{bytesPerToken.Sem:F1} bytes/token SEM (SD " +
            $"{bytesPerToken.StandardDeviation:F1}, range " +
            $"{bytesPerToken.Minimum:F1}-{bytesPerToken.Maximum:F1}; " +
            "current-thread measurement)");
    }

    private static SampleStatistics Statistics(IEnumerable<double> values)
    {
        var sample = values.ToArray();
        var mean = sample.Average();
        var sumSquaredDeviations = sample.Sum(value =>
            (value - mean) * (value - mean));
        var standardDeviation = sample.Length > 1
            ? Math.Sqrt(sumSquaredDeviations / (sample.Length - 1))
            : 0;
        return new SampleStatistics(
            mean, standardDeviation, standardDeviation / Math.Sqrt(sample.Length),
            sample.Min(), sample.Max());
    }

    private readonly record struct LexerSample(
        double ElapsedSeconds, int TokenCount, long AllocatedBytes)
    {
        public double TokensPerSecond => TokenCount / ElapsedSeconds;
        public double BytesPerToken => (double)AllocatedBytes / TokenCount;
    }

    private readonly record struct SampleStatistics(
        double Mean, double StandardDeviation, double Sem,
        double Minimum, double Maximum);
}
