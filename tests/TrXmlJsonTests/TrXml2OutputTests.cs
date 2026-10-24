extern alias trxml2;

using System.Diagnostics;
using Xunit;

namespace TrXmlJsonTests;

public sealed class TrXml2OutputTests
{
    private static readonly string App = typeof(trxml2::Trash.Config).Assembly.Location;

    [Fact]
    public async Task FileNameOptionPrefixesEachInputWithoutChangingDefaultOutput()
    {
        var directory = Path.Combine(Path.GetTempPath(), "trxml2-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var first = Path.Combine(directory, "first.xml");
        var second = Path.Combine(directory, "second.xml");
        try
        {
            await File.WriteAllTextAsync(first, "<root><one>1</one></root>");
            await File.WriteAllTextAsync(second, "<root><two>2</two></root>");

            var defaultOutput = await Run(null, first);
            var namedOutput = await Run(null, "--file-name", first, second);

            Assert.Equal(0, defaultOutput.Exit);
            Assert.Contains("/root/one=1", defaultOutput.Stdout);
            Assert.DoesNotContain("first.xml:", defaultOutput.Stdout);
            Assert.Equal(0, namedOutput.Exit);
            Assert.Contains(first + ":/root/one=1", namedOutput.Stdout);
            Assert.Contains(second + ":/root/two=2", namedOutput.Stdout);
        }
        finally
        {
            File.Delete(first);
            File.Delete(second);
            Directory.Delete(directory);
        }
    }

    [Fact]
    public async Task FileNameOptionLabelsStandardInput()
    {
        var result = await Run("<root><value>x</value></root>", "--file-name");
        Assert.Equal(0, result.Exit);
        Assert.Contains("<stdin>:/root/value=x", result.Stdout);
    }

    private static async Task<(int Exit, string Stdout)> Run(string? input, params string[] args)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        start.ArgumentList.Add(App);
        foreach (var arg in args) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        try
        {
            if (input != null) await process.StandardInput.WriteAsync(input);
            process.StandardInput.Close();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            var output = await stdout;
            var errors = await stderr;
            Assert.True(process.ExitCode == 0, errors);
            return (process.ExitCode, output);
        }
        finally
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
        }
    }
}
