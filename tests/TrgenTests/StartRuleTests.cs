using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class StartRuleTests
{
    [Fact]
    public async Task DescriptorEntryPointsApplyOnlyToTheirSelectedParserGrammars()
    {
        var directory = Path.Combine(Path.GetTempPath(),
            "trgen-multiple-grammars-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            File.WriteAllText(Path.Combine(directory, "OneLexer.g4"),
                "lexer grammar OneLexer; A : 'a';");
            File.WriteAllText(Path.Combine(directory, "OneParser.g4"),
                "parser grammar OneParser; options { tokenVocab=OneLexer; } " +
                "first : A EOF;");
            File.WriteAllText(Path.Combine(directory, "TwoLexer.g4"),
                "lexer grammar TwoLexer; B : 'b';");
            File.WriteAllText(Path.Combine(directory, "TwoParser.g4"),
                "parser grammar TwoParser; options { tokenVocab=TwoLexer; } " +
                "second : B EOF;");
            File.WriteAllText(Path.Combine(directory, "desc.xml"),
                "<desc><targets>CSharp</targets>" +
                "<test><name>one</name><grammar-name>One</grammar-name>" +
                "<entry-point>first</entry-point>" +
                "<inputs>examples/*.one; examples/nested/**/*.one; " +
                "!examples/ignored/**</inputs></test>" +
                "<test><name>two</name><grammar-name>Two</grammar-name>" +
                "<entry-point>second</entry-point>" +
                "<inputs>examples/*.two;examples/deep/**/*.two</inputs>" +
                "</test></desc>");

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

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(60));
            _ = await stdoutTask;
            var stderr = await stderrTask;
            Assert.True(process.ExitCode == 0, stderr);
            Assert.Contains("Start rule first", stderr);
            Assert.Contains("Start rule second", stderr);
            string oneScript = File.ReadAllText(Path.Combine(directory,
                "Generated-CSharp-one", "test.sh"));
            string twoScript = File.ReadAllText(Path.Combine(directory,
                "Generated-CSharp-two", "test.sh"));
            Assert.Contains("glob '../examples/*.one' " +
                "'../examples/nested/**/*.one' '!../examples/ignored/**'",
                oneScript);
            Assert.Contains("glob '../examples/*.two' " +
                "'../examples/deep/**/*.two'", twoScript);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

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
