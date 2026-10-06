using System.Diagnostics;
using System.Formats.Tar;
using System.Text;
using System.Text.Json;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace TrTextTests;

public sealed class TrTextCliTests
{
    [Fact]
    public async Task DefaultBundleRestoresSourcesAndPreservesSidecars()
    {
        const string firstSource = "first\r\nβ\n";
        const string secondSource = "second without final newline";
        var input = Bundle(
            new Artifact("pkg/one.g4.pt", ParseResult("one.g4", firstSource)),
            new Artifact("pkg/one.g4.errors", Encoding.UTF8.GetBytes("diagnostic\n")),
            new Artifact("pkg/sub/two.rex.pt", ParseResult("two.rex", secondSource)),
            new Artifact("pkg/sub/two.rex.errors", []),
            new Artifact("notes.bin", [0, 1, 255]));

        var output = await Run(input);

        Assert.Equal(0, output.Exit);
        using (var stream = new MemoryStream(output.Stdout))
        using (var reader = new TarReader(stream))
            Assert.Equal(TarEntryFormat.Pax, reader.GetNextEntry()!.Format);
        var artifacts = ReadBundle(output.Stdout);
        Assert.Equal(new[] { "pkg/one.g4", "pkg/one.g4.errors",
            "pkg/sub/two.rex", "pkg/sub/two.rex.errors", "notes.bin" },
            artifacts.Select(a => a.Name));
        Assert.Equal(Encoding.UTF8.GetBytes(firstSource), artifacts[0].Data);
        Assert.Equal(Encoding.UTF8.GetBytes("diagnostic\n"), artifacts[1].Data);
        Assert.Equal(Encoding.UTF8.GetBytes(secondSource), artifacts[2].Data);
        Assert.Empty(artifacts[3].Data);
        Assert.Equal(new byte[] { 0, 1, 255 }, artifacts[4].Data);
    }

    [Fact]
    public async Task FileInputAndBundleAliasStillProduceBundles()
    {
        var file = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(file,
                Bundle(new Artifact("dir/input.g4.pt", ParseResult("input.g4", "x"))));
            foreach (var args in new[] { new[] { "-f", file },
                         new[] { "-f", file, "--bundle" } })
            {
                var output = await Run(null, args);
                Assert.Equal(0, output.Exit);
                var source = Assert.Single(ReadBundle(output.Stdout));
                Assert.Equal("dir/input.g4", source.Name);
                Assert.Equal("x", Encoding.UTF8.GetString(source.Data));
            }
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public async Task TextModePreservesPlainOutputAndDisplayFlags()
    {
        var input = Bundle(
            new Artifact("one.pt", ParseResult("one.g4", "alpha")),
            new Artifact("two.pt", ParseResult("two.rex", "beta")),
            new Artifact("empty.pt", ParseResult("empty.g4")));
        var newline = Environment.NewLine;

        var text = await Run(input, "--text");
        Assert.Equal(0, text.Exit);
        Assert.Equal($"one.g4:alpha{newline}two.rex:beta{newline}",
            Encoding.UTF8.GetString(text.Stdout));
        var count = await Run(input, "--text", "-c");
        Assert.Equal($"one.g4:1{newline}two.rex:1{newline}empty.g4:0{newline}",
            Encoding.UTF8.GetString(count.Stdout));
        var withMatches = await Run(input, "--text", "-l");
        Assert.Equal($"one.g4{newline}two.rex{newline}",
            Encoding.UTF8.GetString(withMatches.Stdout));
        var withoutMatches = await Run(input, "--text", "-L");
        Assert.Equal($"empty.g4{newline}",
            Encoding.UTF8.GetString(withoutMatches.Stdout));
        var lineNumber = await Run(input, "--text", "-n");
        Assert.Equal(text.Stdout, lineNumber.Stdout);
    }

    [Theory]
    [InlineData("-l")]
    [InlineData("-L")]
    [InlineData("-c")]
    [InlineData("-n")]
    public async Task DisplayFlagsRequireTextMode(string flag)
    {
        var output = await Run(null, flag);
        Assert.NotEqual(0, output.Exit);
        Assert.Empty(output.Stdout);
        Assert.Contains("require --text", output.Stderr);
    }

    [Fact]
    public async Task TextAndBundleAreMutuallyExclusive()
    {
        var output = await Run(null, "--text", "--bundle");
        Assert.NotEqual(0, output.Exit);
        Assert.Empty(output.Stdout);
        Assert.Contains("cannot be combined", output.Stderr);
    }

    [Fact]
    public async Task LegacyJsonInputIsAutoDetectedInBothOutputModes()
    {
        var json = JsonSerializer.Serialize(new[] { Result("legacy.g4", "legacy") },
            ParsingResultIO.JsonOptions());
        var input = Encoding.UTF8.GetBytes(json);

        var bundled = await Run(input);
        Assert.Equal(0, bundled.Exit);
        var source = Assert.Single(ReadBundle(bundled.Stdout));
        Assert.Equal("legacy.g4", source.Name);
        Assert.Equal("legacy", Encoding.UTF8.GetString(source.Data));

        var plain = await Run(input, "--text");
        Assert.Equal(0, plain.Exit);
        Assert.Equal("legacy" + Environment.NewLine,
            Encoding.UTF8.GetString(plain.Stdout));
    }

    private static ParsingResultSet Result(string name, string source)
    {
        var root = new UnvParseTreeElement { LocalName = "root" };
        root.ChildNodes.Add(new UnvParseTreeText { Data = source });
        return new ParsingResultSet { FileName = name, Nodes = [root] };
    }

    private static ParsingResultSet Result(string name) =>
        new() { FileName = name, Nodes = [] };

    private static byte[] ParseResult(string name, string source) =>
        ArtifactBundle.SerializeParsingResult(Result(name, source));

    private static byte[] ParseResult(string name) =>
        ArtifactBundle.SerializeParsingResult(Result(name));

    private static byte[] Bundle(params Artifact[] artifacts)
    {
        using var stream = new MemoryStream();
        ArtifactBundle.Write(stream, artifacts);
        return stream.ToArray();
    }

    private static IReadOnlyList<Artifact> ReadBundle(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return ArtifactBundle.Read(stream);
    }

    private static async Task<(int Exit, byte[] Stdout, string Stderr)> Run(
        byte[]? input, params string[] args)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        start.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        foreach (var arg in args) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        using var stdout = new MemoryStream();
        var read = process.StandardOutput.BaseStream.CopyToAsync(stdout);
        var stderr = process.StandardError.ReadToEndAsync();
        try
        {
            if (input != null) await process.StandardInput.BaseStream.WriteAsync(input);
            process.StandardInput.Close();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            await read;
            return (process.ExitCode, stdout.ToArray(), await stderr);
        }
        finally { if (!process.HasExited) process.Kill(entireProcessTree: true); }
    }
}
