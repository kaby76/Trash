using System.Diagnostics;
using System.Formats.Tar;
using System.Text;
using System.Text.Json;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace TrTreeTests;

public sealed class TrTreeCliTests
{
    [Fact]
    public async Task DefaultOutputIsPaxWithOneTreePerNestedResultAndUnchangedSidecars()
    {
        var input = Bundle(
            new Artifact("pkg/one.g4.pt", ParseResult("one.g4", "hello")),
            new Artifact("pkg/one.g4.errors", Encoding.UTF8.GetBytes("diagnostic\n")),
            new Artifact("pkg/sub/two.rex.pt", ParseResult("two.rex", "world")),
            new Artifact("notes.bin", [0, 1, 255]));

        var output = await Run(input, "-a");

        Assert.Equal(0, output.Exit);
        using (var stream = new MemoryStream(output.Stdout))
        using (var reader = new TarReader(stream))
            Assert.Equal(TarEntryFormat.Pax, reader.GetNextEntry()!.Format);
        var artifacts = ReadBundle(output.Stdout);
        Assert.Equal(new[] { "pkg/one.g4.tree", "pkg/one.g4.errors",
            "pkg/sub/two.rex.tree", "notes.bin" }, artifacts.Select(a => a.Name));
        Assert.Equal("(root hello)", Encoding.UTF8.GetString(artifacts[0].Data));
        Assert.Equal("diagnostic\n", Encoding.UTF8.GetString(artifacts[1].Data));
        Assert.Equal("(root world)", Encoding.UTF8.GetString(artifacts[2].Data));
        Assert.Equal(new byte[] { 0, 1, 255 }, artifacts[3].Data);
    }

    [Fact]
    public async Task TextModePreservesAntlrStyleAndMultipleResultLabels()
    {
        var input = Bundle(
            new Artifact("one.pt", ParseResult("one.g4", "hello")),
            new Artifact("two.pt", ParseResult("two.rex", "world")));

        var output = await Run(input, "--text", "-a");

        Assert.Equal(0, output.Exit);
        Assert.Equal($"one.g4: (root hello){Environment.NewLine}two.rex: (root world)",
            Encoding.UTF8.GetString(output.Stdout));
    }

    [Fact]
    public async Task DefaultBlockStyleBundleMatchesPlainRenderingWithoutDisplayNewline()
    {
        var input = Bundle(new Artifact("one.pt", ParseResult("one.g4", "hello")));
        var bundled = await Run(input);
        var plain = await Run(input, "--text");

        Assert.Equal(0, bundled.Exit);
        Assert.Equal(0, plain.Exit);
        var tree = Assert.Single(ReadBundle(bundled.Stdout));
        Assert.Equal("one.tree", tree.Name);
        Assert.Equal(Encoding.UTF8.GetString(tree.Data) + Environment.NewLine,
            Encoding.UTF8.GetString(plain.Stdout));
    }

    [Fact]
    public async Task FileInputAndBundleAliasProduceBundles()
    {
        var file = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(file,
                Bundle(new Artifact("dir/input.pt", ParseResult("input.g4", "x"))));
            foreach (var args in new[] { new[] { "-f", file, "-a" },
                         new[] { "-f", file, "--bundle", "-a" } })
            {
                var output = await Run(null, args);
                Assert.Equal(0, output.Exit);
                var tree = Assert.Single(ReadBundle(output.Stdout));
                Assert.Equal("dir/input.tree", tree.Name);
                Assert.Equal("(root x)", Encoding.UTF8.GetString(tree.Data));
            }
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public async Task LegacyJsonInputIsAcceptedInBothModes()
    {
        var input = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            new[] { Result("legacy.g4", "legacy") }, ParsingResultIO.JsonOptions()));
        var bundled = await Run(input, "-a");
        var plain = await Run(input, "--text", "-a");

        Assert.Equal(0, bundled.Exit);
        var tree = Assert.Single(ReadBundle(bundled.Stdout));
        Assert.Equal("legacy.tree", tree.Name);
        Assert.Equal("(root legacy)", Encoding.UTF8.GetString(tree.Data));
        Assert.Equal(0, plain.Exit);
        Assert.Equal("(root legacy)", Encoding.UTF8.GetString(plain.Stdout));
    }

    [Fact]
    public async Task DisplaySourceRequiresTextMode()
    {
        var rejected = await Run(null, "-d");
        Assert.NotEqual(0, rejected.Exit);
        Assert.Empty(rejected.Stdout);
        Assert.Contains("requires --text", rejected.Stderr);

        var input = Bundle(new Artifact("one.pt", ParseResult("one.g4", "hello")));
        var displayed = await Run(input, "--text", "-d", "-a");
        Assert.Equal(0, displayed.Exit);
        Assert.Contains("one.g4: ", Encoding.UTF8.GetString(displayed.Stdout));
    }

    [Fact]
    public async Task TextAndBundleModesCannotBeCombined()
    {
        var output = await Run(null, "--text", "--bundle");
        Assert.NotEqual(0, output.Exit);
        Assert.Empty(output.Stdout);
        Assert.Contains("cannot be combined", output.Stderr);
    }

    private static ParsingResultSet Result(string name, string text)
    {
        var root = new UnvParseTreeElement { LocalName = "root", RuleIndex = 0 };
        var terminal = new UnvParseTreeElement { LocalName = "WORD", RuleIndex = -1 };
        terminal.ChildNodes.Add(new UnvParseTreeText { Data = text });
        root.ChildNodes.Add(terminal);
        return new ParsingResultSet { FileName = name, Nodes = [root] };
    }

    private static byte[] ParseResult(string name, string text) =>
        ArtifactBundle.SerializeParsingResult(Result(name, text));

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
