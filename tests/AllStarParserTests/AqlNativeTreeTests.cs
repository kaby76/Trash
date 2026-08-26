using Xunit;

namespace AllStarParserTests;

public class AqlNativeTreeTests
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.Aql);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(NativeTreeTestSupport.Aql, inputPath);
}
