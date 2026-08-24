using Xunit;

namespace AllStarParserTests;

public class AcmeNativeTreeTests
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.Acme);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(NativeTreeTestSupport.Acme, inputPath);
}
