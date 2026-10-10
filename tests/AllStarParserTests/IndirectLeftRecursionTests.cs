using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Formats.Tar;
using Xunit;

namespace AllStarParserTests;

public class IndirectLeftRecursionTests
{
    [Fact(Timeout = 5000)]
    public async Task OrdinaryAllStarReportsIndirectCycleInsteadOfRecursingForever()
    {
        var directory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "indirect-left-recursion");
        var parserInterp = Path.Combine(
            directory, "IndirectLeftRecursion.interp");
        var lexerInterp = Path.Combine(
            directory, "IndirectLeftRecursionLexer.interp");
        var input = File.ReadAllText(Path.Combine(directory, "input.txt"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => Task.Run(() => AllStarAtnParser.InterpRunner.Run(
                parserInterp, lexerInterp, input, "input.txt",
                lineNumbers: false)));

        Assert.Contains("Indirect left recursion detected", exception.Message);
        Assert.Contains("--indirect-left-recursion", exception.Message);
    }

    [Fact]
    public void OptionParsesMutuallyRecursiveRulesAndBuildsLeftAssociatedTree()
    {
        var directory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "indirect-left-recursion");
        var parserInterp = Path.Combine(
            directory, "IndirectLeftRecursion.interp");
        var lexerInterp = Path.Combine(
            directory, "IndirectLeftRecursionLexer.interp");
        var input = File.ReadAllText(Path.Combine(directory, "input.txt"));

        var (result, _) = AllStarAtnParser.InterpRunner.Run(
            parserInterp, lexerInterp, input, "input.txt",
            lineNumbers: false, indirectLeftRecursion: true);

        var root = Assert.Single(result.Nodes);
        var tree = new TreeOutput(result.Lexer, result.Parser)
            .OutputTreeAntlrStyle(root).ToString();
        Assert.Equal(
            "(start (expression (additionExpression " +
            "(expression (additionExpression (expression 1) + 2)) + 3)) <EOF>)",
            tree);
    }

    [Fact]
    public void ContextAwareOptionPreservesLeftAssociatedTree()
    {
        var directory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "indirect-left-recursion");
        var parserInterp = Path.Combine(directory, "IndirectLeftRecursion.interp");
        var lexerInterp = Path.Combine(directory, "IndirectLeftRecursionLexer.interp");
        var input = File.ReadAllText(Path.Combine(directory, "input.txt"));
        var (result, _) = AllStarAtnParser.InterpRunner.Run(
            parserInterp, lexerInterp, input, "input.txt", lineNumbers: false,
            contextAwareLexing: true, indirectLeftRecursion: true);
        var tree = new TreeOutput(result.Lexer, result.Parser)
            .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        Assert.Equal(
            "(start (expression (additionExpression " +
            "(expression (additionExpression (expression 1) + 2)) + 3)) <EOF>)",
            tree);
    }

    [Fact]
    public async Task MultipleFilesContinueAfterAParseFailure()
    {
        var fixture = Path.Combine(
            AppContext.BaseDirectory, "TestData", "indirect-left-recursion");
        var directory = Directory.CreateTempSubdirectory("TrparseContinue-").FullName;
        try
        {
            var bad = Path.Combine(directory, "bad.txt");
            File.WriteAllText(bad, "1+");
            var start = new System.Diagnostics.ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            foreach (var argument in new[]
            {
                typeof(Trash.Program).Assembly.Location,
                "--allstar", "--indirect-left-recursion", "-L", fixture,
                "--no-output", "--per-file", bad,
                Path.Combine(fixture, "input.txt")
            }) start.ArgumentList.Add(argument);
            using var process = System.Diagnostics.Process.Start(start)!;
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            Assert.Equal(1, process.ExitCode);
            var diagnostic = await error;
            Assert.Contains($"ALL(*) 0 {bad} failed{Environment.NewLine}" +
                $"{bad}: line 1:2", diagnostic);
            Assert.Contains("bad.txt: line 1:2 near '<EOF>': ALL(*) parse failed: input rejected by grammar", diagnostic);
            Assert.DoesNotContain("at AllStarAtnParser", diagnostic);
            Assert.Contains("ALL(*) 1", diagnostic);
            Assert.Empty(await output);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RejectionReportsFarthestInputPosition(bool contextAwareLexing)
    {
        var fixture = Path.Combine(AppContext.BaseDirectory, "TestData",
            "indirect-left-recursion");
        var exception = Assert.Throws<InvalidOperationException>(
            () => AllStarAtnParser.InterpRunner.Run(
                Path.Combine(fixture, "IndirectLeftRecursion.interp"),
                Path.Combine(fixture, "IndirectLeftRecursionLexer.interp"),
                "1+", "bad.txt", lineNumbers: false,
                contextAwareLexing: contextAwareLexing,
                indirectLeftRecursion: true));
        Assert.Contains("bad.txt: line 1:2 near '<EOF>'", exception.Message);
    }

    [Fact(Timeout = 20000)]
    public async Task TimedOutFileIsKilledAndNextFileIsBundled()
    {
        var fixture = Path.Combine(AppContext.BaseDirectory, "TestData",
            "indirect-left-recursion");
        var directory = Directory.CreateTempSubdirectory("TrparseTimeout-").FullName;
        try
        {
            var slow = Path.Combine(directory, "slow.txt");
            var good = Path.Combine(directory, "good.txt");
            await File.WriteAllTextAsync(slow,
                string.Concat(Enumerable.Repeat("1+", 10000)) + "1");
            await File.WriteAllTextAsync(good,
                await File.ReadAllTextAsync(Path.Combine(fixture, "input.txt")));
            var start = new System.Diagnostics.ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            foreach (var argument in new[]
            {
                typeof(Trash.Program).Assembly.Location,
                "--allstar", "--indirect-left-recursion", "-L", fixture,
                "--timeout", "3", slow, good
            }) start.ArgumentList.Add(argument);
            using var process = System.Diagnostics.Process.Start(start)!;
            using var output = new MemoryStream();
            var read = process.StandardOutput.BaseStream.CopyToAsync(output);
            var error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
            await read;
            Assert.Equal(1, process.ExitCode);
            Assert.Contains("timed out after 3 seconds", await error);
            output.Position = 0;
            using var reader = new TarReader(output);
            var members = new Dictionary<string, string>();
            TarEntry? entry;
            while ((entry = reader.GetNextEntry()) != null)
            {
                if (!entry.Name.EndsWith(".pt", StringComparison.Ordinal) &&
                    !entry.Name.EndsWith(".errors", StringComparison.Ordinal))
                    continue;
                if (entry.DataStream == null)
                    members.Add(entry.Name, string.Empty);
                else
                {
                    using var stream = new StreamReader(entry.DataStream);
                    members.Add(entry.Name, await stream.ReadToEndAsync());
                }
            }
            Assert.Contains("slow.txt.errors", members.Keys);
            Assert.Contains("timed out", members["slow.txt.errors"]);
            Assert.Contains("good.txt.pt", members.Keys);
            Assert.Contains("good.txt.errors", members.Keys);
        }
        finally { Directory.Delete(directory, true); }
    }
}
