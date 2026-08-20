using System.IO;
using ParseTreeEditing.UnvParseTreeDOM;
using EarleyAtnParser;
using AllStarAtnParser;
using Xunit;

namespace AllStarParserTests;

/// <summary>
/// End-to-end tests for AllStarParser using the MySQL (Positive-Technologies)
/// grammar interp files and its pre-built SQL example inputs.
/// </summary>
public class MySqlInterpTests
{
    private static readonly string InterpDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "mysql-interp");

    private static readonly string ExamplesDir =
        Path.Combine(AppContext.BaseDirectory, "TestData", "mysql-examples");

    private static readonly string ParserInterp =
        Path.Combine(InterpDir, "MySqlParser.interp");

    private static readonly string LexerInterp =
        Path.Combine(InterpDir, "MySqlLexer.interp");

    public static IEnumerable<object[]> SqlFiles() =>
        Directory.EnumerateFiles(ExamplesDir, "*.sql", SearchOption.AllDirectories)
                 .OrderBy(f => f)
                 .Select(f => new object[] { f });

    [Theory]
    [MemberData(nameof(SqlFiles))]
    public void AllStarParsesSuccessfully(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var (result, tokenCount) = AllStarRunner.Run(
            ParserInterp, LexerInterp, text, Path.GetFileName(filePath), lineNumbers: false);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Nodes);
        Assert.True(tokenCount > 0, $"Expected at least one token in {Path.GetFileName(filePath)}");
    }

    [Theory]
    [MemberData(nameof(SqlFiles))]
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
