using Xunit;

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
}
