using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class GalopTargetTests
{
    [Theory]
    [InlineData("Galop")]
    [InlineData("*")]
    public async Task GeneratesNativeGalopDriverWithGoStyleOptions(string targets)
    {
        var directory = Directory.CreateTempSubdirectory("trgen-galop-").FullName;
        try
        {
            File.WriteAllText(Path.Combine(directory, "Expr.g4"),
                "grammar Expr; start: ID EOF; ID: [a-z]+; WS: [ \\t\\r\\n]+ -> skip;");
            File.WriteAllText(Path.Combine(directory, "desc.xml"),
                $"<desc><targets>{targets}</targets><entry-point>start</entry-point></desc>");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("-t");
            startInfo.ArgumentList.Add("Galop");
            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(90));
            Assert.True(process.ExitCode == 0, await error);
            _ = await output;

            var generated = Path.Combine(directory, "Generated-Galop");
            Assert.True(File.Exists(Path.Combine(generated, "Expr.g4")));
            var driver = File.ReadAllText(Path.Combine(generated, "Test.go"));
            Assert.Contains("ExprLex(src)", driver);
            Assert.Contains("ParseStart_(tokens)", driver);
            Assert.Contains("case \"-x\"", driver);
            Assert.Contains("case \"--per-file\"", driver);
            Assert.Contains("case \"--perf\"", driver);
            Assert.Contains("case \"-tree\"", driver);
            Assert.Contains("case \"-tokens\"", driver);
            Assert.Contains("-package main", File.ReadAllText(Path.Combine(generated, "build.sh")));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
}
