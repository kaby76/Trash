using System.Diagnostics;
using System.Text.RegularExpressions;
using AntlrJson;
using Xunit;

namespace AllStarParserTests;

public sealed class TrparseOutputTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OutsideGeneratedDirectoryDefaultsToPipedAllStarDespiteSibling(bool selectBundlePair)
    {
        string directory = Directory.CreateTempSubdirectory("TrparseDefault-").FullName;
        try
        {
            string generated = Path.Combine(directory, "Generated-CSharp", "bin", "Debug", "net10.0");
            Directory.CreateDirectory(generated);
            await File.WriteAllBytesAsync(Path.Combine(generated, "Test.dll"), [0]);

            string input = Path.Combine(directory, "input.txt");
            await File.WriteAllTextAsync(input, "rule = %x41\r\n");
            string interp = Path.Combine(AppContext.BaseDirectory, "TestData", "interp");
            using var bundle = new MemoryStream();
            ArtifactBundle.Write(bundle,
            [
                new Artifact("Abnf.interp", File.ReadAllBytes(Path.Combine(interp, "Abnf.interp"))),
                new Artifact("AbnfLexer.interp", File.ReadAllBytes(Path.Combine(interp, "AbnfLexer.interp")))
            ]);

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = directory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("--no-output");
            startInfo.ArgumentList.Add("--per-file");
            if (selectBundlePair)
            {
                startInfo.ArgumentList.Add("--pinterp");
                startInfo.ArgumentList.Add("Abnf.interp");
                startInfo.ArgumentList.Add("--linterp");
                startInfo.ArgumentList.Add("AbnfLexer.interp");
            }
            startInfo.ArgumentList.Add(input);

            using var process = Process.Start(startInfo)!;
            await process.StandardInput.BaseStream.WriteAsync(bundle.ToArray());
            process.StandardInput.Close();
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            Assert.Equal(string.Empty, await stdout);
            string diagnostics = await stderr;
            Assert.True(process.ExitCode == 0, diagnostics);
            Assert.Contains("ALL(*) 0 ", diagnostics);
            Assert.Contains(" success ", diagnostics);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData(".g4x", false)]
    [InlineData(".g4", true)]
    public void G4XParsesGrammarFiles(string extension, bool specifyParserType)
    {
        var file = Path.Combine(Path.GetTempPath(),
            "G4XSmoke-" + Guid.NewGuid().ToString("N") + extension);
        try
        {
            File.WriteAllText(file,
                "grammar G4XSmoke; root : 'x' EOF;\n");
            var startInfo = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("--no-output");
            if (specifyParserType)
            {
                startInfo.ArgumentList.Add("-t");
                startInfo.ArgumentList.Add("G4X");
            }
            startInfo.ArgumentList.Add(file);

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            Assert.Equal(string.Empty, process.StandardOutput.ReadToEnd());
            var stderr = process.StandardError.ReadToEnd();
            Assert.True(process.WaitForExit(30_000));
            Assert.True(process.ExitCode == 0, stderr);
            Assert.Contains("TT:", stderr);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NamedStartRuleOverridesInterpDefault(bool allstar)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        startInfo.ArgumentList.Add(allstar ? "--allstar" : "--earley");
        startInfo.ArgumentList.Add("--no-output");
        startInfo.ArgumentList.Add("--per-file");
        startInfo.ArgumentList.Add("--start-rule");
        startInfo.ArgumentList.Add("elements");
        startInfo.ArgumentList.Add("-L");
        startInfo.ArgumentList.Add(Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp"));
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("x");

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        Assert.Equal(string.Empty, process.StandardOutput.ReadToEnd());
        var stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));
        Assert.True(process.ExitCode == 0, stderr);
        Assert.Contains(allstar ? "ALL(*) 0 " : "Earley 0 ", stderr);
        Assert.Contains("TT:", stderr);
    }

    [Fact]
    public void EarleyRejectsConflictingAllStarOption()
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        startInfo.ArgumentList.Add("--earley");
        startInfo.ArgumentList.Add("--allstar");
        startInfo.ArgumentList.Add("-L");
        startInfo.ArgumentList.Add(Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp"));
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("rule = %x41\r\n");

        using var process = Process.Start(startInfo)!;
        _ = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));
        Assert.NotEqual(0, process.ExitCode);
        Assert.Contains("--earley cannot be combined with ALL(*)-only options", stderr);
    }

    [Fact]
    public void EarleyAcceptsExplicitInterpPairWithoutLibOrStdinBundle()
    {
        string interp = Path.Combine(AppContext.BaseDirectory, "TestData", "interp");
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        startInfo.ArgumentList.Add("--earley");
        startInfo.ArgumentList.Add("--no-output");
        startInfo.ArgumentList.Add("--per-file");
        startInfo.ArgumentList.Add("--pinterp");
        startInfo.ArgumentList.Add(Path.Combine(interp, "Abnf.interp"));
        startInfo.ArgumentList.Add("--linterp");
        startInfo.ArgumentList.Add(Path.Combine(interp, "AbnfLexer.interp"));
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("rule = %x41\r\n");

        using var process = Process.Start(startInfo)!;
        process.StandardInput.Close();
        Assert.Equal(string.Empty, process.StandardOutput.ReadToEnd());
        string stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));
        Assert.True(process.ExitCode == 0, stderr);
        Assert.Contains("Earley 0 ", stderr);
    }

    [Fact]
    public void UnknownStartRuleReportsAvailableNames()
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        startInfo.ArgumentList.Add("--allstar");
        startInfo.ArgumentList.Add("--no-output");
        startInfo.ArgumentList.Add("--start-rule");
        startInfo.ArgumentList.Add("missing");
        startInfo.ArgumentList.Add("-L");
        startInfo.ArgumentList.Add(Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp"));
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("x");

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        _ = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));
        Assert.NotEqual(0, process.ExitCode);
        Assert.Contains("Start rule 'missing' was not found", stderr);
        Assert.Contains("rulelist, rule_, elements", stderr);
    }

    [Fact]
    public void NoOutputParsesAndReportsDiagnosticsWithoutWritingStdout()
    {
        var interpDirectory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp");
        var trparse = typeof(Trash.Program).Assembly.Location;
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(trparse);
        startInfo.ArgumentList.Add("--allstar");
        startInfo.ArgumentList.Add("--no-output");
        startInfo.ArgumentList.Add("--parser-stats");
        startInfo.ArgumentList.Add("--perf");
        startInfo.ArgumentList.Add("--per-file");
        startInfo.ArgumentList.Add("-L");
        startInfo.ArgumentList.Add(interpDirectory);
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("rule = %x41\r\n");

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));

        Assert.Equal(0, process.ExitCode);
        Assert.Equal(string.Empty, stdout);
        Assert.Contains("ALL(*)", stderr);
        Assert.Contains("Parser statistics:", stderr);
        Assert.Contains("DFA edges:", stderr);
        Assert.Contains("committed parser:", stderr);
        Assert.Contains("PT:", stderr);
        Assert.Contains("PR:", stderr);
        Assert.Contains(" tokens ", stderr);
        Assert.Contains(" pr", stderr);
    }

    [Fact]
    public void DefaultPerformanceOutputContainsOnlyTotalTime()
    {
        var interpDirectory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp");
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        startInfo.ArgumentList.Add("--allstar");
        startInfo.ArgumentList.Add("--no-output");
        startInfo.ArgumentList.Add("-L");
        startInfo.ArgumentList.Add(interpDirectory);
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("rule = %x41\r\n");

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000));

        Assert.Equal(0, process.ExitCode);
        Assert.Equal(string.Empty, stdout);
        Assert.Contains("TT:", stderr);
        Assert.DoesNotContain("PT:", stderr);
        Assert.DoesNotContain("PR:", stderr);
        Assert.DoesNotContain("ALL(*) 0", stderr);
    }

    [Fact]
    public void MultipleInputsReuseTheCommandScopedParserDfa()
    {
        var interpDirectory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "interp");
        var first = Path.GetTempFileName();
        var second = Path.GetTempFileName();
        try
        {
            File.WriteAllText(first, "first = %x41\r\n");
            File.WriteAllText(second, "second = %x42\r\n");
            var startInfo = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("--allstar");
            startInfo.ArgumentList.Add("--no-output");
            startInfo.ArgumentList.Add("--parser-stats");
            startInfo.ArgumentList.Add("--interp-timings");
            startInfo.ArgumentList.Add("-L");
            startInfo.ArgumentList.Add(interpDirectory);
            startInfo.ArgumentList.Add(first);
            startInfo.ArgumentList.Add(second);

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            Assert.True(process.WaitForExit(30_000));

            Assert.Equal(0, process.ExitCode);
            Assert.Equal(string.Empty, stdout);
            Assert.Equal(2, Regex.Matches(
                stderr, "Parser statistics:").Count);
            Assert.Matches(
                @"shared DFA at parse start: [1-9][0-9,]* states",
                stderr);
            Assert.Matches(
                @"Lexer DFA \(last file\) at input start: [1-9][0-9,]* states, " +
                @"[1-9][0-9,]* transitions",
                stderr);
            Assert.Matches(
                @"ALL\(\*\) stage timings \(2 files\):", stderr);
        }
        finally
        {
            File.Delete(first);
            File.Delete(second);
        }
    }
}
