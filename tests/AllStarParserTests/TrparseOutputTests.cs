using System.Diagnostics;
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
        Assert.Contains("PT:", stderr);
    }
}
