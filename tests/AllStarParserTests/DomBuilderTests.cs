using Atn;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Text.Json;
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CompactTreeSerializesExactlyLikeMutableDom(bool lineNumbers)
    {
        var tokens = new[]
        {
            Token(3, LexerToken.SKIP_CHANNEL, " ", 0),
            Token(1, 0, "x", 1)
        };
        var events = new[]
        {
            ParseEvent.EnterRule(0),
            ParseEvent.EnterRule(1),
            ParseEvent.Consume(1),
            ParseEvent.ExitRule(1),
            ParseEvent.ExitRule(0)
        };
        string[] symbolic = [null!, "VISIBLE", "COMMENT", "WS"];
        string[] lexerRules = [null!, "VISIBLE", "COMMENT", "WS"];
        var dom = DomBuilder.Build(events, tokens, ["root", "item"],
            symbolic, [], lexerRules, lineNumbers);
        var compact = CompactTreeBuilder.Build(events, tokens, ["root", "item"],
            symbolic, [], lexerRules, lineNumbers);

        Assert.Equal(Serialize(new ParsingResultSet { Nodes = [dom] }),
            Serialize(new ParsingResultSet { NodeProvider = compact }));
        Assert.Equal(1, compact.Count);
        Assert.True(compact.NodeCount > 0);
        Assert.True(compact.EdgeCount > 0);
        if (!lineNumbers)
        {
            Assert.Equal(4, compact.NodeCount);
            Assert.Equal(3, compact.EdgeCount);
        }
    }

    [Fact]
    public void CompactTreeMaterializesMutableDomOnlyOnDemand()
    {
        var compact = CompactTreeBuilder.Build(
            [ParseEvent.EnterRule(0), ParseEvent.Consume(0), ParseEvent.ExitRule(0)],
            [Token(1, 0, "x", 0)], ["root"], [null!, "VISIBLE"], [],
            [null!, "VISIBLE"], lineNumbers: false);
        var result = new ParsingResultSet { NodeProvider = compact };

        Assert.False(result.HasMaterializedNodes);
        _ = Serialize(result);
        Assert.False(result.HasMaterializedNodes);
        var root = Assert.IsType<UnvParseTreeElement>(Assert.Single(result.Nodes));
        Assert.True(result.HasMaterializedNodes);
        Assert.Equal("root", root.LocalName);
    }

    private static string Serialize(ParsingResultSet result)
    {
        var options = new JsonSerializerOptions { MaxDepth = 10000 };
        options.Converters.Add(new ParsingResultSetSerializer());
        return JsonSerializer.Serialize(new[] { result }, options);
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
