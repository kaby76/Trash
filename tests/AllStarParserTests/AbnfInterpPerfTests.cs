using System.Diagnostics;
using System.IO;
using EarleyAtnParser;
using AllStarAtnParser;
using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

/// <summary>
/// Parse-time comparison between the Earley and ALL(*) interpreters.
/// Timings are written to the xunit test output (visible in the IDE "Tests" panel
/// or via `dotnet test --logger "console;verbosity=detailed"`).
///
/// Each file is run through a short warm-up pass (to let the JIT settle) and then
/// a measured pass; the results are reported per-file and as an aggregate.
/// </summary>
public class AbnfInterpPerfTests(ITestOutputHelper output)
{
    private const int WarmupRuns   = 2;
    private const int MeasuredRuns = 5;

    private static readonly string InterpDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "interp");

    private static readonly string ExamplesDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "examples");

    private static readonly string ParserInterp =
        Path.Combine(InterpDir, "Abnf.interp");

    private static readonly string LexerInterp =
        Path.Combine(InterpDir, "AbnfLexer.interp");

    public static IEnumerable<object[]> ExampleFiles() =>
        Directory.EnumerateFiles(ExamplesDir, "*.*bnf", SearchOption.AllDirectories)
                 .OrderBy(f => f)
                 .Select(f => new object[] { f });

    [Theory]
    [MemberData(nameof(ExampleFiles))]
    public void CompareParseTime(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var text     = File.ReadAllText(filePath);

        double earleyMs  = MeasureMs(() => InterpRunner.Run(ParserInterp, LexerInterp, text, fileName, false));
        double allStarMs = MeasureMs(() => AllStarRunner.Run(ParserInterp, LexerInterp, text, fileName, false));

        double ratio = allStarMs > 0 ? earleyMs / allStarMs : double.PositiveInfinity;

        output.WriteLine($"{fileName,-40}  Earley: {earleyMs,7:F2} ms   AllStar: {allStarMs,7:F2} ms   Earley/AllStar: {ratio:F2}x");
    }

    /// <summary>
    /// Parses every example file with both parsers and prints a single aggregate
    /// summary line showing total and per-file-average times.
    /// </summary>
    [Fact]
    public void AggregateTiming()
    {
        var files = Directory
            .EnumerateFiles(ExamplesDir, "*.*bnf", SearchOption.AllDirectories)
            .OrderBy(f => f)
            .ToList();

        double totalEarleyMs  = 0;
        double totalAllStarMs = 0;

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            var text     = File.ReadAllText(filePath);

            double earleyMs  = MeasureMs(() => InterpRunner.Run(ParserInterp, LexerInterp, text, fileName, false));
            double allStarMs = MeasureMs(() => AllStarRunner.Run(ParserInterp, LexerInterp, text, fileName, false));

            totalEarleyMs  += earleyMs;
            totalAllStarMs += allStarMs;

            double ratio = allStarMs > 0 ? earleyMs / allStarMs : double.PositiveInfinity;
            output.WriteLine($"  {fileName,-40}  Earley: {earleyMs,7:F2} ms   AllStar: {allStarMs,7:F2} ms   ratio: {ratio:F2}x");
        }

        double avgEarley  = totalEarleyMs  / files.Count;
        double avgAllStar = totalAllStarMs / files.Count;
        double totalRatio = totalAllStarMs > 0 ? totalEarleyMs / totalAllStarMs : double.PositiveInfinity;

        output.WriteLine("");
        output.WriteLine($"{"TOTAL",-40}  Earley: {totalEarleyMs,7:F2} ms   AllStar: {totalAllStarMs,7:F2} ms   Earley/AllStar: {totalRatio:F2}x");
        output.WriteLine($"{"AVERAGE per file",-40}  Earley: {avgEarley,7:F2} ms   AllStar: {avgAllStar,7:F2} ms");
    }

    // -------------------------------------------------------------------------

    private double MeasureMs(Action action)
    {
        // Warm-up: let the JIT and interp-file caches settle.
        for (int i = 0; i < WarmupRuns; i++)
            action();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < MeasuredRuns; i++)
            action();
        sw.Stop();

        return sw.Elapsed.TotalMilliseconds / MeasuredRuns;
    }
}
