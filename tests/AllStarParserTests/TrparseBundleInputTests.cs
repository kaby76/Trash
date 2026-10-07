using System.Diagnostics;
using System.Formats.Tar;
using System.Text;
using AntlrJson;
using Xunit;

namespace AllStarParserTests;

public sealed class TrparseBundleInputTests
{
    private static readonly string InterpDirectory = Path.Combine(
        AppContext.BaseDirectory, "TestData", "interp");

    [Fact]
    public void BundleGlobParsesOnlySelectedMembersAndPreservesOriginalBundle()
    {
        var inputs = new[]
        {
            new Artifact("src/Top.java", Encoding.UTF8.GetBytes("top = %x41\r\n")),
            new Artifact("src/pkg/Nested.java", Encoding.UTF8.GetBytes("nested = %x42\r\n")),
            new Artifact("test/Other.java", Encoding.UTF8.GetBytes("not ABNF")),
            new Artifact("src/pkg/data.bin", [0, 1, 2, 255])
        };

        var (exitCode, output, stderr) = Run(inputs, "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java");

        Assert.True(exitCode == 0, stderr);
        using var archive = new MemoryStream(output);
        var artifacts = ArtifactBundle.Read(archive);
        Assert.Equal(new[]
        {
            "src/Top.java", "src/Top.java.pt", "src/Top.java.errors",
            "src/pkg/Nested.java", "src/pkg/Nested.java.pt",
            "src/pkg/Nested.java.errors", "test/Other.java", "src/pkg/data.bin"
        }, artifacts.Select(a => a.Name));
        foreach (var input in inputs)
            Assert.Equal(input.Data, artifacts.Single(a => a.Name == input.Name).Data);
        Assert.All(artifacts.Where(a => a.Name.EndsWith(".errors")),
            artifact => Assert.Empty(artifact.Data));
        Assert.Equal("src/Top.java", ArtifactBundle.DeserializeParsingResult(
            artifacts.Single(a => a.Name == "src/Top.java.pt").Data).FileName);
        Assert.Equal("src/pkg/Nested.java", ArtifactBundle.DeserializeParsingResult(
            artifacts.Single(a => a.Name == "src/pkg/Nested.java.pt").Data).FileName);
    }

    [Fact]
    public void BundleGlobWithoutMatchesPassesInputThrough()
    {
        var inputs = new[] { new Artifact("notes.txt", Encoding.UTF8.GetBytes("keep")) };
        var (exitCode, output, stderr) = Run(inputs, "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java");

        Assert.True(exitCode == 0, stderr);
        var artifacts = ArtifactBundle.Read(new MemoryStream(output));
        Assert.Equal(inputs.Select(a => a.Name), artifacts.Select(a => a.Name));
        Assert.Equal(inputs[0].Data, artifacts[0].Data);
    }

    [Fact]
    public void BundleGlobAcceptsDirectoryEntriesFromOrdinaryTar()
    {
        using var archive = new MemoryStream();
        using (var writer = new TarWriter(archive, TarEntryFormat.Pax, leaveOpen: true))
        {
            writer.WriteEntry(new PaxTarEntry(TarEntryType.Directory, "src/"));
            writer.WriteEntry(new PaxTarEntry(TarEntryType.RegularFile, "src/One.java")
            {
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("one = %x41\r\n"))
            });
        }
        var (exitCode, output, stderr) = Run(archive.ToArray(), "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java");

        Assert.True(exitCode == 0, stderr);
        using var reader = new TarReader(new MemoryStream(output));
        Assert.Equal(TarEntryType.Directory, reader.GetNextEntry()!.EntryType);
        Assert.Equal("src/One.java", reader.GetNextEntry()!.Name);
        Assert.Equal("src/One.java.pt", reader.GetNextEntry()!.Name);
        Assert.Equal("src/One.java.errors", reader.GetNextEntry()!.Name);
    }

    [Fact]
    public void FailedParseRetainsSourceAndAddsErrors()
    {
        var input = new Artifact("src/Bad.java", Encoding.UTF8.GetBytes("not ABNF"));
        var (exitCode, output, stderr) = Run([input], "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java");

        Assert.NotEqual(0, exitCode);
        var artifacts = ArtifactBundle.Read(new MemoryStream(output));
        Assert.Equal(new[] { "src/Bad.java", "src/Bad.java.errors" },
            artifacts.Select(a => a.Name));
        Assert.Equal(input.Data, artifacts[0].Data);
        Assert.NotEmpty(artifacts[1].Data);
        Assert.Contains("parse failed", stderr);
    }

    [Fact]
    public void BundleGlobAlsoUsesBuiltInParser()
    {
        var input = new Artifact("src/Small.g4", Encoding.UTF8.GetBytes(
            "grammar Small; root : 'x' EOF;\n"));
        var (exitCode, output, stderr) = Run([input], "-t", "ANTLRv4",
            "--bundle-glob", "src/**/*.g4");

        Assert.True(exitCode == 0, stderr);
        var artifacts = ArtifactBundle.Read(new MemoryStream(output));
        Assert.Equal(new[] { "src/Small.g4", "src/Small.g4.pt",
            "src/Small.g4.errors" }, artifacts.Select(a => a.Name));
        Assert.Equal("src/Small.g4", ArtifactBundle.DeserializeParsingResult(
            artifacts[1].Data).FileName);
    }

    [Fact]
    public void NoOutputStillParsesSelectedMembers()
    {
        var inputs = new[]
        {
            new Artifact("other/data.bin", [0, 1, 2]),
            new Artifact("src/One.java", Encoding.UTF8.GetBytes("one = %x41\r\n"))
        };
        var (exitCode, output, stderr) = Run(inputs, "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java", "--no-output");

        Assert.True(exitCode == 0, stderr);
        Assert.Empty(output);
        Assert.Contains("TT:", stderr);
    }

    [Fact]
    public void BundleGlobRejectsCollisionWithExistingParseArtifact()
    {
        var inputs = new[]
        {
            new Artifact("src/One.java", Encoding.UTF8.GetBytes("one = %x41\r\n")),
            new Artifact("src/One.java.pt", Encoding.UTF8.GetBytes("old"))
        };
        var (exitCode, output, stderr) = Run(inputs, "--allstar", "-L",
            InterpDirectory, "--bundle-glob", "src/**/*.java");

        Assert.True(exitCode != 0, $"Unexpected success. stderr: {stderr}; names: " +
            string.Join(", ", ArtifactBundle.Read(new MemoryStream(output)).Select(a => a.Name)));
        Assert.Empty(output);
        Assert.Contains("Duplicate bundle member 'src/One.java.pt'", stderr);
    }

    private static (int ExitCode, byte[] Output, string Stderr) Run(
        IReadOnlyList<Artifact> inputs, params string[] arguments)
    {
        using var input = new MemoryStream();
        ArtifactBundle.Write(input, inputs);
        return Run(input.ToArray(), arguments);
    }

    private static (int ExitCode, byte[] Output, string Stderr) Run(
        byte[] inputBytes, params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo)!;
        process.StandardInput.BaseStream.Write(inputBytes);
        process.StandardInput.Close();
        var outputTask = Task.Run(() =>
        {
            using var output = new MemoryStream();
            process.StandardOutput.BaseStream.CopyTo(output);
            return output.ToArray();
        });
        var stderrTask = process.StandardError.ReadToEndAsync();
        Assert.True(process.WaitForExit(30_000));
        return (process.ExitCode, outputTask.Result, stderrTask.Result);
    }
}
