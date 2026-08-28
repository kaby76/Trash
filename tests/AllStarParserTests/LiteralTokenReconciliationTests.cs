using AllStarAtnParser;
using Atn;
using Xunit;

namespace AllStarParserTests;

public class LiteralTokenReconciliationTests
{
    [Fact]
    public void RemapsKeywordsButNotSameNamedStringTokens()
    {
        var tokens = new List<LexerToken>
        {
            new() { Type = 1, Text = "table" },
            new() { Type = 3, Text = "`hello'" }
        };
        string[] lexerSymbolicNames = [null!, "TABLE", null!, "String"];
        string[] lexerLiteralNames = [null!, null!, null!, null!];
        string[] parserLiteralNames = [null!, null!, "'table'", null!, "'string'"];

        InterpRunner.ReconcileLiteralTokenTypes(
            tokens, lexerSymbolicNames, lexerLiteralNames, parserLiteralNames);

        Assert.Equal(2, tokens[0].Type);
        Assert.Equal(3, tokens[1].Type);
    }
}
