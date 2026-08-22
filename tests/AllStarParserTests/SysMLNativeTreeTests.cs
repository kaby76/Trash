using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

public class SysMLNativeTreeTests(ITestOutputHelper output)
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.SysML);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(NativeTreeTestSupport.SysML, inputPath);

    [Fact]
    [Trait("Category", "Performance")]
    public void AllStarPerformanceDoesNotRegress() =>
        NativeTreeTestSupport.ParseCorpus(NativeTreeTestSupport.SysML, output);
}
