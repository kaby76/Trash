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
}
