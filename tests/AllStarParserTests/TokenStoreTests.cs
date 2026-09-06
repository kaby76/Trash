using Atn;
using Xunit;

namespace AllStarParserTests;

public class TokenStoreTests
{
    [Fact]
    public void RetainsSourceOnceAndReturnsIndexedTokenViews()
    {
        const string source = "alpha beta";
        var store = new TokenStore(source);
        store.Add(new LexerToken(source)
        {
            Type = 1, Channel = 0, StartIndex = 0, StopIndex = 4,
            Line = 1, Column = 0, TokenIndex = 99
        });
        store.Add(new LexerToken(source)
        {
            Type = 2, Channel = 1, StartIndex = 6, StopIndex = 9,
            Line = 1, Column = 6
        });

        Assert.Equal(2, store.Count);
        Assert.Equal(0, store[0].TokenIndex);
        Assert.Equal("alpha", store[0].Text);
        Assert.Equal("beta", store[1].Text);
        Assert.False(store[0].IsTextMaterialized);
    }

    [Fact]
    public void ViewMutationUpdatesCanonicalTokenMetadata()
    {
        var store = new TokenStore("x");
        store.Add(new LexerToken("x")
        {
            Type = 1, Channel = 0, StartIndex = 0, StopIndex = 0
        });

        var token = store[0];
        token.Type = 7;
        token.Channel = 3;
        token.Text = "replacement";

        Assert.Equal(7, store[0].Type);
        Assert.Equal(3, store[0].Channel);
        Assert.Equal("replacement", store[0].Text);
        Assert.True(store[0].IsTextMaterialized);
    }
}
