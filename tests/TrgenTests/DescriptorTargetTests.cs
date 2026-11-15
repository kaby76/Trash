using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class DescriptorTargetTests
{
    [Theory]
    [InlineData("CSharp;Java")]
    [InlineData(" CSharp ; ; Java ")]
    public async Task DefaultInvocationGeneratesEveryListedTarget(string targets)
    {
        var directory = Directory.CreateTempSubdirectory("trgen-targets-").FullName;
        try
        {
            WriteGrammarAndDescriptor(directory, targets);
            var (exitCode, error) = await RunGen(directory);

            Assert.True(exitCode == 0, error);
            Assert.Equal(new[] { "Generated-CSharp", "Generated-Java" },
                Directory.GetDirectories(directory, "Generated-*")
                    .Select(Path.GetFileName)
                    .OrderBy(name => name, StringComparer.Ordinal));
            Assert.True(File.Exists(Path.Combine(directory, "Generated-CSharp", "Mini.g4")));
            Assert.True(File.Exists(Path.Combine(directory, "Generated-Java", "Mini.g4")));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Fact]
    public async Task EmptyTargetsDoNotCreateUnnamedGeneratedDirectory()
    {
        var directory = Directory.CreateTempSubdirectory("trgen-empty-targets-").FullName;
        try
        {
            WriteGrammarAndDescriptor(directory, "");
            var (exitCode, error) = await RunGen(directory);

            Assert.True(exitCode == 0, error);
            Assert.Contains("no target languages", error);
            Assert.Empty(Directory.GetDirectories(directory, "Generated-*"));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    private static void WriteGrammarAndDescriptor(string directory, string targets)
    {
        File.WriteAllText(Path.Combine(directory, "Mini.g4"),
            "grammar Mini; start: 'a' EOF;");
        File.WriteAllText(Path.Combine(directory, "desc.xml"),
            $"<desc><targets>{targets}</targets><entry-point>start</entry-point></desc>");
    }

    private static async Task<(int ExitCode, string Error)> RunGen(string directory)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(90));
        _ = await stdout;
        return (process.ExitCode, await stderr);
    }
}
