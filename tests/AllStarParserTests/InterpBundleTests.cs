using AntlrJson;
using Trash;
using Xunit;

namespace AllStarParserTests;

public sealed class InterpBundleTests
{
    private static MemoryStream Bundle(params Artifact[] artifacts)
    {
        var stream = new MemoryStream();
        ArtifactBundle.Write(stream, artifacts);
        stream.Position = 0;
        return stream;
    }

    [Fact]
    public void StagesOnlyInterpreterTablesAndCleansUp()
    {
        using var input = Bundle(
            new Artifact("Example.interp", [1, 2]),
            new Artifact("ExampleLexer.interp", [3]),
            new Artifact("Example.tokens", [4]),
            new Artifact("Example.atn.dot", [5]));
        string directory;
        using (var staged = InterpBundle.Open(input))
        {
            directory = staged.DirectoryPath;
            Assert.Equal([1, 2], File.ReadAllBytes(Path.Combine(directory, "Example.interp")));
            Assert.Equal([3], File.ReadAllBytes(Path.Combine(directory, "ExampleLexer.interp")));
            Assert.Equal([4], File.ReadAllBytes(Path.Combine(directory, "Example.tokens")));
            Assert.False(File.Exists(Path.Combine(directory, "Example.atn.dot")));
        }
        Assert.False(Directory.Exists(directory));
    }

    [Fact]
    public void RejectsBundleWithoutInterpFiles()
    {
        using var input = Bundle(new Artifact("Example.tokens", [1]));
        Assert.Contains(".interp", Assert.Throws<InvalidDataException>(() => InterpBundle.Open(input)).Message);
    }

    [Theory]
    [InlineData("nested/Example.interp")]
    [InlineData("CON.interp")]
    [InlineData("name:stream.interp")]
    public void RejectsUnsafeTableNames(string name)
    {
        using var input = Bundle(new Artifact(name, [1]));
        Assert.Throws<InvalidDataException>(() => InterpBundle.Open(input));
    }
}
