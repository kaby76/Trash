using System.IO;
using ParseTreeEditing.UnvParseTreeDOM;
using Trash.EarleyAtn;
using Xunit;

namespace AllStarParserTests;

/// <summary>
/// End-to-end tests for AllStarParser using the checked-in ABNF example grammar
/// and its pre-built interp files.  Each example .abnf / .bnf file is parsed as
/// a separate theory case so failures are easy to pin down.
/// </summary>
public class AbnfInterpTests
{
    private static readonly string InterpDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "interp");

    private static readonly string ExamplesDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "examples");

    private static readonly string ParserInterp =
        Path.Combine(InterpDir, "Abnf.interp");

    private static readonly string LexerInterp =
        Path.Combine(InterpDir, "AbnfLexer.interp");

    /// <summary>
    /// Enumerate all *.abnf and *.bnf files copied to the test output directory.
    /// </summary>
    public static IEnumerable<object[]> ExampleFiles() =>
        Directory.EnumerateFiles(ExamplesDir, "*.*bnf", SearchOption.AllDirectories)
                 .OrderBy(f => f)
                 .Select(f => new object[] { f });

    [Theory]
    [MemberData(nameof(ExampleFiles))]
    public void AllStarParsesSuccessfully(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var (result, tokenCount) = AllStarRunner.Run(
            ParserInterp, LexerInterp, text, Path.GetFileName(filePath), lineNumbers: false);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Nodes);
        Assert.True(tokenCount > 0, $"Expected at least one token in {Path.GetFileName(filePath)}");
    }

    /// <summary>
    /// Parse each example file with both the Earley and ALL(*) parsers and assert
    /// that the resulting parse trees are identical (ANTLR-style parenthesised form).
    /// This is the primary regression guard: any change to AllStarSimulator that
    /// produces a different tree will be caught here.
    /// </summary>
    [Theory]
    [MemberData(nameof(ExampleFiles))]
    public void AllStarTreeMatchesEarley(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var fileName = Path.GetFileName(filePath);

        var (earleyResult, _) = InterpRunner.Run(
            ParserInterp, LexerInterp, text, fileName, lineNumbers: false);
        var (allStarResult, _) = AllStarRunner.Run(
            ParserInterp, LexerInterp, text, fileName, lineNumbers: false);

        var earleyTree = RenderAntlrStyle(earleyResult);
        var allStarTree = RenderAntlrStyle(allStarResult);

        Assert.Equal(earleyTree, allStarTree);
    }

    private static string RenderAntlrStyle(AntlrJson.ParsingResultSet result)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var node in result.Nodes)
        {
            sb.Append(new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeAntlrStyle(node));
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
