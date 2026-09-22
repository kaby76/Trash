using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class StartRuleTests
{
    [Fact]
    public void RejectsMultipleEofTerminatedParserRulesAndNamesThem()
    {
        var directory = Path.Combine(Path.GetTempPath(),
            "trgen-start-rules-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            File.WriteAllText(Path.Combine(directory, "TwoStarts.g4"),
                "grammar TwoStarts; first : 'a' EOF; second : 'b' EOF;");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("-t");
            startInfo.ArgumentList.Add("CSharp");
            startInfo.ArgumentList.Add("-o");
            startInfo.ArgumentList.Add(
                Path.Combine(directory, "Generated").Replace('\\', '/') + "/");
            startInfo.ArgumentList.Add("TwoStarts.g4");

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            _ = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            Assert.True(process.WaitForExit(30_000));

            Assert.True(process.ExitCode != 0, stderr);
            Assert.Contains("multiple EOF-terminated parser start rules", stderr);
            Assert.Contains("first, second", stderr);
            Assert.False(Directory.Exists(Path.Combine(directory, "Generated-CSharp")));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void ExplicitStartRuleResolvesMultipleEofTerminatedRules()
    {
        var directory = Path.Combine(Path.GetTempPath(),
            "trgen-selected-start-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            File.WriteAllText(Path.Combine(directory, "TwoStarts.g4"),
                "grammar TwoStarts; first : 'a' EOF; second : 'b' EOF;");
            var outputDirectory = Path.Combine(directory, "Generated");
            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("-t");
            startInfo.ArgumentList.Add("CSharp");
            startInfo.ArgumentList.Add("--start-rule");
            startInfo.ArgumentList.Add("first");
            startInfo.ArgumentList.Add("-o");
            startInfo.ArgumentList.Add(outputDirectory.Replace('\\', '/') + "/");
            startInfo.ArgumentList.Add("TwoStarts.g4");

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            _ = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            Assert.True(process.WaitForExit(30_000));

            Assert.True(process.ExitCode == 0, stderr);
            Assert.Contains("Start rule first", stderr);
            var generatedDrivers = Directory.GetFiles(directory, "Test.cs",
                SearchOption.AllDirectories);
            Assert.True(generatedDrivers.Length > 0,
                "No generated Test.cs found. Files: " +
                string.Join(", ", Directory.GetFiles(directory, "*",
                    SearchOption.AllDirectories)));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
