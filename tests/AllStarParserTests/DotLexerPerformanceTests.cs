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
    private static readonly string LexerInterp = Path.Combine(
        AppContext.BaseDirectory, "TestData", "dot", "DOTLexer.interp");

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

        // Warm the JIT without warming any future cross-token DFA cache with the
        // measured input. A new simulator is used for the measured run.
        _ = new LexerAtnSimulator(atn).Tokenize("digraph G { a -> b; }");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        stopwatch.Restart();
        var tokens = new LexerAtnSimulator(atn).Tokenize(input);
        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        Assert.Equal(-1, tokens[^1].Type);
        Assert.True(tokens.Count >= 100_000,
            $"Expected a representative token volume, but produced {tokens.Count} tokens.");

        var tokensPerSecond = tokens.Count / stopwatch.Elapsed.TotalSeconds;
        var bytesPerToken = (double)allocatedBytes / tokens.Count;
        output.WriteLine($"DOT lexer benchmark: {tokens.Count:N0} retained tokens, " +
            $"{input.Length:N0} chars, {stopwatch.Elapsed.TotalSeconds:F6} s, " +
            $"{tokensPerSecond:N0} tokens/s");
        output.WriteLine($"Allocated: {allocatedBytes:N0} bytes " +
            $"({bytesPerToken:F1} bytes/token, current-thread measurement)");
        output.WriteLine($"Setup: file read {fileRead.TotalMilliseconds:F3} ms; " +
            $"interp parse {interpParse.TotalMilliseconds:F3} ms; " +
            $"ATN deserialize {deserialize.TotalMilliseconds:F3} ms");

        // Deliberately conservative: this catches catastrophic regressions while
        // remaining stable on slower CI hosts. Reported numbers provide the
        // baseline for evaluating the DFA work in subsequent #712 subtasks.
        Assert.True(tokensPerSecond >= 5_000,
            $"DOT lexer throughput {tokensPerSecond:N0} tokens/s is below the " +
            "5,000 tokens/s regression floor.");

        GC.KeepAlive(tokens);
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
        _ = new LexerAtnSimulator(atn).Tokenize("digraph G { a -> b; }");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        stopwatch.Restart();
        var tokens = new LexerAtnSimulator(atn).Tokenize(input);
        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        Assert.Equal(1_109_453, tokens.Count);
        Assert.Equal(-1, tokens[^1].Type);
        var tokensPerSecond = tokens.Count / stopwatch.Elapsed.TotalSeconds;
        var bytesPerToken = (double)allocatedBytes / tokens.Count;
        output.WriteLine($"1864.dot: {tokens.Count:N0} retained tokens, " +
            $"{input.Length:N0} chars, {stopwatch.Elapsed.TotalSeconds:F6} s, " +
            $"{tokensPerSecond:N0} tokens/s");
        output.WriteLine($"Allocated: {allocatedBytes:N0} bytes " +
            $"({bytesPerToken:F1} bytes/token, current-thread measurement)");
        output.WriteLine($"Fixture decompression/read: {loadTime.TotalMilliseconds:F3} ms " +
            "(excluded from lexer time)");

        Assert.True(tokensPerSecond >= 5_000,
            $"1864.dot lexer throughput {tokensPerSecond:N0} tokens/s is below " +
            "the 5,000 tokens/s regression floor.");
        GC.KeepAlive(tokens);
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
}
