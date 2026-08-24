using System.Formats.Tar;
using System.Text;
using AntlrJson;
using Xunit;

namespace AllStarParserTests;

public class ArtifactBundleTests
{
    [Fact]
    public void BundleIsOrdinaryPaxAndPreservesBinaryArtifacts()
    {
        var expected = new[]
        {
            new Artifact("top.tree", Encoding.UTF8.GetBytes("(root child)\n")),
            new Artifact("nested/data.bin", new byte[] { 0, 1, 2, 255 })
        };
        using var archive = new MemoryStream();
        ArtifactBundle.Write(archive, expected);

        archive.Position = 0;
        using (var reader = new TarReader(archive, leaveOpen: true))
        {
            var first = reader.GetNextEntry();
            Assert.NotNull(first);
            Assert.Equal(TarEntryFormat.Pax, first!.Format);
            Assert.Equal("top.tree", first.Name);
        }

        archive.Position = 0;
        var actual = ArtifactBundle.Read(archive);
        Assert.Equal(expected.Select(a => a.Name), actual.Select(a => a.Name));
        Assert.Equal(expected[0].Data, actual[0].Data);
        Assert.Equal(expected[1].Data, actual[1].Data);
    }

    [Theory]
    [InlineData("../escape.tree")]
    [InlineData("nested/../../escape.tree")]
    [InlineData("/absolute.tree")]
    [InlineData("C:/drive.tree")]
    [InlineData("nested/./ambiguous.tree")]
    public void UnsafeMemberNamesAreRejected(string name)
    {
        Assert.Throws<InvalidDataException>(() => ArtifactBundle.ValidateMemberName(name));
    }

    [Fact]
    public void RelativeNamesStripCommonInputDirectoryAndPreserveHierarchy()
    {
        var baseDirectory = Path.Combine(Path.GetTempPath(), "trash-bundle-base");
        var inputs = new[]
        {
            Path.Combine(baseDirectory, "examples", "abnf.abnf"),
            Path.Combine(baseDirectory, "examples", "apg-java", "ABNFforSABNF.abnf")
        };

        var names = ArtifactBundle.RelativeInputNames(inputs);

        Assert.Equal("abnf.abnf", names[inputs[0]]);
        Assert.Equal("apg-java/ABNFforSABNF.abnf", names[inputs[1]]);
    }

    [Fact]
    public void ExplicitBaseRejectsOutsideInput()
    {
        var root = Path.Combine(Path.GetTempPath(), "trash-bundle-root");
        var outside = Path.Combine(Path.GetTempPath(), "outside.abnf");
        Assert.Throws<InvalidDataException>(() =>
            ArtifactBundle.RelativeInputNames(new[] { outside }, root));
    }

    [Fact]
    public void DuplicateArchiveNamesAreRejected()
    {
        using var archive = new MemoryStream();
        var entries = new[]
        {
            new Artifact("same.tree", Array.Empty<byte>()),
            new Artifact("same.tree", Array.Empty<byte>())
        };
        Assert.Throws<InvalidDataException>(() => ArtifactBundle.Write(archive, entries));
    }
}
