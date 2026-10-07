extern alias trxml;
extern alias trjson;

using System.Diagnostics;
using System.Formats.Tar;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace TrXmlJsonTests;

public sealed class BundleOutputTests
{
    private static readonly string XmlApp = typeof(trxml::Trash.Program).Assembly.Location;
    private static readonly string JsonApp = typeof(trjson::Trash.Program).Assembly.Location;

    [Theory]
    [InlineData("xml")]
    [InlineData("json")]
    public async Task DefaultOutputReplacesNestedParseTreesAndPreservesOtherMembers(string format)
    {
        var input = Bundle(
            new Artifact("pkg/one.st.pt", ParseResult("one.st", "a&b\"")),
            new Artifact("pkg/one.st.errors", Encoding.UTF8.GetBytes("warning\n")),
            new Artifact("pkg/sub/two.st.pt", ParseResult("two.st", "other")),
            new Artifact("notes.bin", [0, 1, 255]));

        var output = await Run(App(format), input);

        Assert.Equal(0, output.Exit);
        using (var stream = new MemoryStream(output.Stdout))
        using (var reader = new TarReader(stream))
            Assert.Equal(TarEntryFormat.Pax, reader.GetNextEntry()!.Format);
        var artifacts = ReadBundle(output.Stdout);
        Assert.Equal(new[] { $"pkg/one.st.{format}", "pkg/one.st.errors",
            $"pkg/sub/two.st.{format}", "notes.bin" }, artifacts.Select(a => a.Name));
        Assert.Equal("warning\n", Encoding.UTF8.GetString(artifacts[1].Data));
        Assert.Equal(new byte[] { 0, 1, 255 }, artifacts[3].Data);
        Assert.Equal("a&b\"", ReadText(format, artifacts[0].Data));
        Assert.Equal("other", ReadText(format, artifacts[2].Data));
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("json")]
    public async Task TextModeAndLegacyJsonInputAreSupported(string format)
    {
        var input = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            new[] { Result("legacy.st", "legacy") }, ParsingResultIO.JsonOptions()));
        var bundled = await Run(App(format), input, "--bundle");
        var plain = await Run(App(format), input, "--text");

        Assert.Equal(0, bundled.Exit);
        var artifact = Assert.Single(ReadBundle(bundled.Stdout));
        Assert.Equal($"legacy.{format}", artifact.Name);
        Assert.Equal("legacy", ReadText(format, artifact.Data));
        Assert.Equal(0, plain.Exit);
        Assert.Equal("legacy", ReadText(format, plain.Stdout));
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("json")]
    public async Task FileInputWorksAndConflictingModesFail(string format)
    {
        var file = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(file,
                Bundle(new Artifact("nested/input.st.pt", ParseResult("input.st", "x"))));
            var output = await Run(App(format), null, "-f", file);
            Assert.Equal(0, output.Exit);
            Assert.Equal($"nested/input.st.{format}",
                Assert.Single(ReadBundle(output.Stdout)).Name);
        }
        finally { File.Delete(file); }

        var rejected = await Run(App(format), null, "--text", "--bundle");
        Assert.NotEqual(0, rejected.Exit);
        Assert.Contains("cannot be combined", rejected.Stderr);
    }

    private static string App(string format) => format == "xml" ? XmlApp : JsonApp;

    private static string ReadText(string format, byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes);
        if (format == "xml")
            return XDocument.Parse(text).Root!.Element("t")!.Value;
        using var document = JsonDocument.Parse(text);
        return document.RootElement.GetProperty("root")[0].GetProperty("Text").GetString()!;
    }

    private static ParsingResultSet Result(string name, string text)
    {
        var root = new UnvParseTreeElement { LocalName = "root", RuleIndex = 0 };
        root.ChildNodes.Add(new UnvParseTreeText { Data = text });
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
        string app, byte[]? input, params string[] args)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        start.ArgumentList.Add(app);
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
