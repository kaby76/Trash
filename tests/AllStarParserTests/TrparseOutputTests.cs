using System.Diagnostics;
using System.Text.RegularExpressions;
using Xunit;

namespace AllStarParserTests;

public sealed class TrparseOutputTests
{
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
