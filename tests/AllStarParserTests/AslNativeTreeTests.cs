using Xunit;

namespace AllStarParserTests;

public class AslNativeTreeTests
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.Asl);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(NativeTreeTestSupport.Asl, inputPath);
}
