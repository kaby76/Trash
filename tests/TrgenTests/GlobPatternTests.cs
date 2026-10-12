extern alias trglob;

using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class GlobPatternTests
{
    [Fact]
    public async Task BraceAlternativesMayContainDirectorySeparators()
    {
        var directory = Path.Combine(Path.GetTempPath(),
            "trglob-braces-" + Guid.NewGuid().ToString("N"));
        var examples = Path.Combine(directory, "examples");
        var nested = Path.Combine(examples, "cpp20", "nested");
        var generated = Path.Combine(directory, "Generated-CSharp-cpp20");
        Directory.CreateDirectory(nested);
        Directory.CreateDirectory(generated);
        try
        {
            File.WriteAllText(Path.Combine(examples, "root.cpp"), "");
            File.WriteAllText(Path.Combine(nested, "nested.cpp"), "");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = generated,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(trglob::Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("../examples/{*.cpp,cpp20/**/*.cpp}");

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            var output = await outputTask;
            Assert.True(process.ExitCode == 0, await errorTask);
            Assert.Equal(new[]
            {
                "../examples/cpp20/nested/nested.cpp",
                "../examples/root.cpp"
            }, output.Split('\n', StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task SemicolonSeparatedPatternsShareInclusionsAndExclusions()
    {
        var directory = Path.Combine(Path.GetTempPath(),
            "trglob-patterns-" + Guid.NewGuid().ToString("N"));
        var examples = Path.Combine(directory, "examples");
        var nested = Path.Combine(examples, "cpp20", "nested");
        var generated = Path.Combine(directory, "Generated-CSharp-cpp20");
        Directory.CreateDirectory(nested);
        Directory.CreateDirectory(generated);
        try
        {
            File.WriteAllText(Path.Combine(examples, "root.cpp"), "");
            File.WriteAllText(Path.Combine(nested, "nested.cpp"), "");
            File.WriteAllText(Path.Combine(nested, "ignored.cpp"), "");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = generated,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(
                typeof(trglob::Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add(
                "../examples/*.cpp; ../examples/cpp20/**/*.cpp;" +
                "!../examples/cpp20/**/ignored.cpp");

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            var output = await outputTask;
            Assert.True(process.ExitCode == 0, await errorTask);
            Assert.Equal(new[]
            {
                "../examples/cpp20/nested/nested.cpp",
                "../examples/root.cpp"
            }, output.Split('\n', StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
