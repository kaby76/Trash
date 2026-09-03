using System.IO;
using Atn;
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
    [Fact]
    public void LearnedLexerDfaMatchesUncachedTokenStream()
    {
        var lexerInterp = InterpFileReader.Read(File.ReadAllText(LexerInterp));
        var lexerAtn = Atn.AtnDeserializer.Deserialize(lexerInterp.AtnData);
        var inputPath = Path.Combine(ExamplesDir, "admin.sql");
        var input = File.ReadAllText(inputPath);

        var expected = new LexerAtnSimulator(
            lexerAtn, statistics: null, enableDfa: false).Tokenize(input);
        var actual = new LexerAtnSimulator(lexerAtn).Tokenize(input);

        Assert.Equal(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.True(
                expected[i].Type == actual[i].Type &&
                expected[i].Channel == actual[i].Channel &&
                expected[i].StartIndex == actual[i].StartIndex &&
                expected[i].StopIndex == actual[i].StopIndex &&
                expected[i].Line == actual[i].Line &&
                expected[i].Column == actual[i].Column &&
                expected[i].Text == actual[i].Text,
                $"Token {i} differs. Expected type {expected[i].Type}, " +
                $"channel {expected[i].Channel}, text '{expected[i].Text}'; " +
                $"actual type {actual[i].Type}, channel {actual[i].Channel}, " +
                $"text '{actual[i].Text}'.");
        }
    }
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

    [Fact]
    public void LineCommentAtEofRemainsHidden()
    {
        var lexerInterp = Atn.InterpFileReader.Read(File.ReadAllText(LexerInterp));
        var lexerAtn = Atn.AtnDeserializer.Deserialize(lexerInterp.AtnData);
        var tokens = new LexerAtnSimulator(lexerAtn).Tokenize("#end");

        var comment = Assert.Single(tokens, token => token.Type != -1);
        Assert.Equal("#end", comment.Text);
        Assert.NotEqual(0, comment.Channel);
        Assert.Equal(-1, tokens[^1].Type);
    }

    [Theory]
    [MemberData(nameof(SqlFiles))]
    public void AllStarParsesSuccessfully(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var (result, tokenCount) = AllStarAtnParser.InterpRunner.Run(
            ParserInterp, LexerInterp, text, Path.GetFileName(filePath), lineNumbers: false);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Nodes);
        Assert.True(tokenCount > 0, $"Expected at least one token in {Path.GetFileName(filePath)}");
    }

    [Theory]
    [MemberData(nameof(SqlFiles))]
    public void AllStarWorks(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var fileName = Path.GetFileName(filePath);
        var (allStarResult, _) = AllStarAtnParser.InterpRunner.Run(
            ParserInterp, LexerInterp, text, fileName, lineNumbers: false);
        var allStarTree = RenderAntlrStyle(allStarResult);
	// Any tree will do.
        Assert.NotNull(allStarTree);
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
