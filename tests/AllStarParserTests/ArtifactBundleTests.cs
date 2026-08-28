using System.Formats.Tar;
using System.Text;
using System.Text.Json;
using AntlrJson;
using Xunit;

namespace AllStarParserTests;

public class ArtifactBundleTests
{
    [Fact]
    public void ParsingResultInputAutoDetectsLegacyJson()
    {
        var expected = new[] { new ParsingResultSet { FileName = "one.g4", Nodes = [] } };
        var json = JsonSerializer.Serialize(expected, ParsingResultIO.JsonOptions());
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var actual = ParsingResultIO.Read(input);

        Assert.False(actual.IsBundle);
        Assert.Single(actual.Results);
        Assert.Equal("one.g4", actual.Results[0].FileName);
    }

    [Fact]
    public void ParsingResultFilterPreservesBundleSidecars()
    {
        var result = new ParsingResultSet { FileName = "dir/one.g4", Nodes = [] };
        using var inputArchive = new MemoryStream();
        ArtifactBundle.Write(inputArchive, new[]
        {
            new Artifact("dir/one.pt", ArtifactBundle.SerializeParsingResult(result)),
            new Artifact("dir/one.errors", Encoding.UTF8.GetBytes("diagnostic"))
        });
        inputArchive.Position = 0;
        var input = ParsingResultIO.Read(inputArchive);
        using var outputArchive = new MemoryStream();

        ParsingResultIO.WriteFilteredBundle(
            outputArchive, input, ".txt", _ => ParsingResultIO.Utf8("rendered"));

        outputArchive.Position = 0;
        var artifacts = ArtifactBundle.Read(outputArchive);
        Assert.Equal(new[] { "dir/one.txt", "dir/one.errors" }, artifacts.Select(a => a.Name));
        Assert.Equal("rendered", Encoding.UTF8.GetString(artifacts[0].Data));
        Assert.Equal("diagnostic", Encoding.UTF8.GetString(artifacts[1].Data));
    }

    [Fact]
    public void SourceFilterRestoresOriginalSourceExtension()
    {
        Assert.Equal(
            "dir/Expression.g4",
            ParsingResultIO.SourceArtifactName("dir/Expression.pt", "Expression.g4"));
    }

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

    [Fact]
    public void CollidingInputStemsRetainTheirSourceExtensions()
    {
        var names = ArtifactBundle.ArtifactBaseNames(
            ["examples/pkg1.adb", "examples/pkg1.ads", "examples/other.adb"]);

        Assert.Equal("examples/pkg1.adb", names["examples/pkg1.adb"]);
        Assert.Equal("examples/pkg1.ads", names["examples/pkg1.ads"]);
        Assert.Equal("examples/other", names["examples/other.adb"]);
    }
}
