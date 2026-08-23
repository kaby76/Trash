using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

public class SystemVerilogNativeTreeTests(ITestOutputHelper output)
{
    public static IEnumerable<object[]> InputFiles() =>
        NativeTreeTestSupport.Cases(NativeTreeTestSupport.SystemVerilog);

    [Theory]
    [MemberData(nameof(InputFiles))]
    public void AllStarTreeMatchesNativeCSharp(string inputPath) =>
        NativeTreeTestSupport.AssertMatchesNativeTree(
            NativeTreeTestSupport.SystemVerilog, inputPath);

    [Fact]
    [Trait("Category", "Performance")]
    public void AllStarPerformanceDoesNotRegress() =>
        NativeTreeTestSupport.ParseCorpus(NativeTreeTestSupport.SystemVerilog, output);
}
