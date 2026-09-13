using Atn;
using EarleyAtnParser;
using Xunit;

namespace AllStarParserTests;

public class YiniNativeTreeTests
{
    private static NativeTreeGrammar Grammar => NativeTreeTestSupport.Yini;

    [Fact]
    public void LearnedLexerDfaMatchesUncachedTokenStream()
    {
        var lexerInterp = InterpFileReader.Read(File.ReadAllText(Grammar.LexerInterp));
        var lexerAtn = AtnDeserializer.Deserialize(lexerInterp.AtnData);
        var inputPath = Assert.Single(Grammar.InputFiles());
        var input = File.ReadAllText(inputPath);

        var expected = new LexerAtnSimulator(
            lexerAtn, statistics: null, enableDfa: false).Tokenize(input);
        var actual = new LexerAtnSimulator(lexerAtn).Tokenize(input);

        Assert.Equal(expected.Count, actual.Count);
        for (var index = 0; index < expected.Count; index++)
        {
            Assert.True(
                expected[index].Type == actual[index].Type &&
                expected[index].Channel == actual[index].Channel &&
                expected[index].StartIndex == actual[index].StartIndex &&
                expected[index].StopIndex == actual[index].StopIndex,
                $"Token {index} differs. Expected type {expected[index].Type}, " +
                $"channel {expected[index].Channel}, text '{expected[index].Text}'; " +
                $"actual type {actual[index].Type}, channel {actual[index].Channel}, " +
                $"text '{actual[index].Text}'.");
        }
    }

    [Fact]
    public void AllStarTreeMatchesNativeJava()
    {
        var inputPath = Assert.Single(Grammar.InputFiles());
        NativeTreeTestSupport.AssertMatchesNativeTree(Grammar, inputPath);
    }
}
