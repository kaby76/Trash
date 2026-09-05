using Xunit;
using Atn;
using EarleyAtnParser;

namespace AllStarParserTests;

public class CMakeNativeTreeTests
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.CMake);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(
            NativeTreeTestSupport.CMake, inputPath);

    [Fact]
    public void RecursiveBracketArgumentsReuseCanonicalLexerContexts()
    {
        var grammar = NativeTreeTestSupport.CMake;
        var interp = InterpFileReader.Read(File.ReadAllText(grammar.LexerInterp));
        var simulator = new LexerAtnSimulator(
            AtnDeserializer.Deserialize(interp.AtnData));
        var input = File.ReadAllText(grammar.InputFiles().Single());

        var first = simulator.Tokenize(input);
        var contextsAfterFirstRun = simulator.LexerContextCount;
        var second = simulator.Tokenize(input);

        Assert.Equal(first.Select(TokenIdentity), second.Select(TokenIdentity));
        Assert.True(contextsAfterFirstRun > 0);
        Assert.Equal(contextsAfterFirstRun, simulator.LexerContextCount);
    }

    private static object TokenIdentity(LexerToken token) => new
    {
        token.Type,
        token.Channel,
        token.Text,
        token.StartIndex,
        token.StopIndex,
        token.Line,
        token.Column
    };
}
