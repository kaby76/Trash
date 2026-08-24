using Atn;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public class DomBuilderTests
{
    [Fact]
    public void CoalescesContiguousSkippedTokensButPreservesHiddenTokens()
    {
        var tokens = new[]
        {
            Token(2, LexerToken.SKIP_CHANNEL, "// first", 0),
            Token(3, LexerToken.SKIP_CHANNEL, "\n  ", 8),
            Token(4, 1, "hidden", 11),
            Token(3, LexerToken.SKIP_CHANNEL, " ", 17),
            Token(2, LexerToken.SKIP_CHANNEL, "// second", 18),
            Token(1, 0, "x", 27)
        };
        var events = new[]
        {
            ParseEvent.EnterRule(0),
            ParseEvent.Consume(5),
            ParseEvent.ExitRule(0)
        };

        var root = DomBuilder.Build(events, tokens,
            ["root"],
            [null!, "VISIBLE", "LINE_COMMENT", "WS", "HIDDEN"],
            [],
            [null!, "VISIBLE", "LINE_COMMENT", "WS", "HIDDEN"],
            lineNumbers: false);
        var children = Enumerable.Range(0, root.ChildNodes.Length)
            .Select(index => root.ChildNodes.item(index))
            .ToArray();

        var firstSkip = Assert.IsType<UnvParseTreeAttr>(children[0]);
        Assert.Equal("Skip", firstSkip.Name);
        Assert.Equal("// first\n  ", firstSkip.StringValue);
        Assert.Equal(-1, firstSkip.TokenType);
        Assert.Equal(-1, firstSkip.Channel);

        var hidden = Assert.IsType<UnvParseTreeAttr>(children[1]);
        Assert.Equal("HIDDEN", hidden.Name);
        Assert.Equal("hidden", hidden.StringValue);

        var secondSkip = Assert.IsType<UnvParseTreeAttr>(children[2]);
        Assert.Equal("Skip", secondSkip.Name);
        Assert.Equal(" // second", secondSkip.StringValue);
        Assert.IsType<UnvParseTreeElement>(children[3]);
    }

    private static LexerToken Token(int type, int channel, string text, int start) => new()
    {
        Type = type,
        Channel = channel,
        Text = text,
        StartIndex = start,
        StopIndex = start + text.Length - 1,
        Line = 1,
        Column = start,
        TokenIndex = start
    };
}
