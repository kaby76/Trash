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
    private static readonly string DotDirectory = Path.Combine(
        AppContext.BaseDirectory, "TestData", "dot");

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

    [Fact]
    public void InterpreterBundleRestoresLexerAndParserDfasWithoutChangingTree()
    {
        const string source = "digraph G { a -> b; }";
        var parserPath = Path.Combine(DotDirectory, "DOTParser.interp");
        var lexerPath = Path.Combine(DotDirectory, "DOTLexer.interp");
        var parserCache = new AllStarAtnParser.ParserPredictionCache();
        var lexerCache = new EarleyAtnParser.LexerAtnSimulator.LexerDfaCache();
        AllStarAtnParser.InterpRunner.Run(parserPath, lexerPath,
            source, "graph.dot", lineNumbers: false,
            predictionCache: parserCache,
            lexerDfaCache: lexerCache);
        Assert.True(parserCache.RetainedStates > 0);
        var cache = Trash.DfaCacheFile.Write(
            parserPath, lexerPath, parserCache, lexerCache);
        var bundle = new[]
        {
            new Artifact("DOTParser.interp", File.ReadAllBytes(parserPath)),
            new Artifact("DOTLexer.interp", File.ReadAllBytes(lexerPath)),
            new Artifact("graph.dfacache", cache)
        };

        var cold = Run(Array.Empty<byte>(), "--allstar", "-L",
            DotDirectory, "-i", source);
        var warm = Run(bundle, "--allstar", "-i", source,
            "--parser-stats", "--interp-timings");
        Assert.True(cold.ExitCode == 0, cold.Stderr);
        Assert.True(warm.ExitCode == 0, warm.Stderr);
        var coldTree = ArtifactBundle.Read(new MemoryStream(cold.Output))
            .Single(artifact => artifact.Name.EndsWith(".pt")).Data;
        var warmTree = ArtifactBundle.Read(new MemoryStream(warm.Output))
            .Single(artifact => artifact.Name.EndsWith(".pt")).Data;
        Assert.Equal(coldTree, warmTree);
        Assert.Matches("shared DFA at parse start: [1-9]", warm.Stderr);
        Assert.Matches("Lexer DFA at input start: [1-9]", warm.Stderr);
        Assert.Contains("DFA cache loading:", warm.Stderr);

        var lexerOnly = Run(bundle, "--allstar", "-i", source,
            "--no-shared-parser-dfa", "--interp-timings", "--parser-stats");
        Assert.True(lexerOnly.ExitCode == 0, lexerOnly.Stderr);
        Assert.Contains("shared DFA at parse start: 0 states", lexerOnly.Stderr);
        Assert.Matches("Lexer DFA at input start: [1-9]", lexerOnly.Stderr);
    }

    [Fact]
    public void InterpreterBundleRejectsCacheForDifferentTables()
    {
        const string source = "digraph G { a -> b; }";
        var parserPath = Path.Combine(DotDirectory, "DOTParser.interp");
        var lexerPath = Path.Combine(DotDirectory, "DOTLexer.interp");
        var parserCache = new AllStarAtnParser.ParserPredictionCache();
        var lexerCache = new EarleyAtnParser.LexerAtnSimulator.LexerDfaCache();
        AllStarAtnParser.InterpRunner.Run(parserPath, lexerPath,
            source, "graph.dot", lineNumbers: false,
            predictionCache: parserCache,
            lexerDfaCache: lexerCache);
        var cache = Trash.DfaCacheFile.Write(
            parserPath, lexerPath, parserCache, lexerCache);
        var changedParser = File.ReadAllBytes(parserPath)
            .Concat(new byte[] { (byte)'\n' }).ToArray();
        var bundle = new[]
        {
            new Artifact("DOTParser.interp", changedParser),
            new Artifact("DOTLexer.interp", File.ReadAllBytes(lexerPath)),
            new Artifact("graph.dfacache", cache)
        };

        var result = Run(bundle, "--allstar", "-i", source);
        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("does not match", result.Stderr);
    }

    [Fact]
    public void SourceBundleLoadsLeadingDfaCacheBeforeMatchingMembers()
    {
        const string source = "digraph G { a -> b; }";
        var parserPath = Path.Combine(DotDirectory, "DOTParser.interp");
        var lexerPath = Path.Combine(DotDirectory, "DOTLexer.interp");
        var parserCache = new AllStarAtnParser.ParserPredictionCache();
        var lexerCache = new EarleyAtnParser.LexerAtnSimulator.LexerDfaCache();
        AllStarAtnParser.InterpRunner.Run(parserPath, lexerPath,
            source, "graph.dot", lineNumbers: false,
            predictionCache: parserCache, lexerDfaCache: lexerCache);
        var cache = Trash.DfaCacheFile.Write(
            parserPath, lexerPath, parserCache, lexerCache);
        var inputs = new[]
        {
            new Artifact("graph.dfacache", cache),
            new Artifact("src/Graph.dot", Encoding.UTF8.GetBytes(source))
        };

        var result = Run(inputs, "--allstar", "-L", DotDirectory,
            "--bundle-glob", "src/**/*.dot", "--interp-timings");
        Assert.True(result.ExitCode == 0, result.Stderr);
        var output = ArtifactBundle.Read(new MemoryStream(result.Output));
        Assert.Contains(output, artifact => artifact.Name == "src/Graph.dot.pt");
        Assert.Contains(output, artifact => artifact.Name == "graph.dfacache" &&
            artifact.Data.SequenceEqual(cache));
        Assert.Matches("Lexer DFA at input start: [1-9]", result.Stderr);

        var noOutput = Run(inputs, "--allstar", "-L", DotDirectory,
            "--bundle-glob", "src/**/*.dot", "--interp-timings",
            "--no-output");
        Assert.True(noOutput.ExitCode == 0, noOutput.Stderr);
        Assert.Empty(noOutput.Output);
        Assert.Matches("Lexer DFA at input start: [1-9]", noOutput.Stderr);

        var refreshed = Run(inputs, "--allstar", "-L", DotDirectory,
            "--bundle-glob", "src/**/*.dot", "--write-dfa-cache");
        Assert.True(refreshed.ExitCode == 0, refreshed.Stderr);
        var refreshedArtifacts = ArtifactBundle.Read(
            new MemoryStream(refreshed.Output));
        Assert.Equal("trparse.dfacache", refreshedArtifacts[0].Name);
        Assert.DoesNotContain(refreshedArtifacts,
            artifact => artifact.Name == "graph.dfacache");
    }

    [Fact]
    public void WriteDfaCacheProducesSelfContainedReusableInterpreterBundle()
    {
        const string source = "digraph G { a -> b; }";
        var trained = Run(Array.Empty<byte>(), "--allstar", "-L",
            DotDirectory, "-i", source, "--write-dfa-cache");
        Assert.True(trained.ExitCode == 0, trained.Stderr);
        var artifacts = ArtifactBundle.Read(new MemoryStream(trained.Output));
        Assert.Contains(artifacts, artifact =>
            artifact.Name == "DOTParser.interp");
        Assert.Contains(artifacts, artifact =>
            artifact.Name == "DOTLexer.interp");
        Assert.Contains(artifacts, artifact =>
            artifact.Name == "trparse.dfacache" && artifact.Data.Length > 0);
        var originalTree = artifacts.Single(artifact =>
            artifact.Name.EndsWith(".pt")).Data;

        var reused = Run(trained.Output, "--allstar", "-i", source,
            "--interp-timings", "--parser-stats");
        Assert.True(reused.ExitCode == 0, reused.Stderr);
        var reusedTree = ArtifactBundle.Read(new MemoryStream(reused.Output))
            .Single(artifact => artifact.Name.EndsWith(".pt")).Data;
        Assert.Equal(originalTree, reusedTree);
        Assert.Matches("shared DFA at parse start: [1-9]", reused.Stderr);
    }

    [Fact]
    public void WriteDfaCacheWithNoOutputProducesCacheOnlyBundle()
    {
        const string source = "digraph G { a -> b; }";
        var trained = Run(Array.Empty<byte>(), "--allstar", "-L",
            DotDirectory, "-i", source, "--write-dfa-cache", "--no-output");
        Assert.True(trained.ExitCode == 0, trained.Stderr);
        var artifacts = ArtifactBundle.Read(new MemoryStream(trained.Output));
        Assert.Contains(artifacts, artifact => artifact.Name == "DOTParser.interp");
        Assert.Contains(artifacts, artifact => artifact.Name == "DOTLexer.interp");
        Assert.Contains(artifacts, artifact => artifact.Name == "trparse.dfacache");
        Assert.DoesNotContain(artifacts, artifact =>
            artifact.Name.EndsWith(".pt") || artifact.Name.EndsWith(".errors"));

        var reused = Run(trained.Output, "--allstar", "-i", source,
            "--parser-stats");
        Assert.True(reused.ExitCode == 0, reused.Stderr);
        Assert.Single(ArtifactBundle.Read(new MemoryStream(reused.Output)),
            artifact => artifact.Name.EndsWith(".pt"));
        Assert.Matches("shared DFA at parse start: [1-9]", reused.Stderr);
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
