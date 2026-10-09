using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
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
            Assert.Contains("input rejected by grammar", await error);
            Assert.Contains("ALL(*) 1", await error);
            Assert.Empty(await output);
        }
        finally { Directory.Delete(directory, true); }
    }
}
