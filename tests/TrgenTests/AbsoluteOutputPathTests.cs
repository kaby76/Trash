using System.Diagnostics;
using Xunit;

namespace TrgenTests;

public class AbsoluteOutputPathTests
{
    [Fact]
    public async Task CppSourcesRemainRelativeWithAbsoluteOutputDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), "trgen-cpp-output-" + Guid.NewGuid().ToString("N"));
        var grammarDirectory = Path.Combine(root, "grammar");
        var outputDirectory = Path.Combine(root, "output");
        Directory.CreateDirectory(Path.Combine(grammarDirectory, "Cpp"));

        try
        {
            File.WriteAllText(Path.Combine(grammarDirectory, "G.g4"),
                "grammar G; root : ID EOF; ID : [a-z]+; WS : [ \\t\\r\\n]+ -> skip;");
            File.WriteAllText(Path.Combine(grammarDirectory, "Cpp", "Extra.cpp"),
                "int extra() { return 1; }");

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = grammarDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(typeof(Trash.Program).Assembly.Location);
            startInfo.ArgumentList.Add("-t");
            startInfo.ArgumentList.Add("Cpp");
            startInfo.ArgumentList.Add("-o");
            startInfo.ArgumentList.Add(outputDirectory + Path.DirectorySeparatorChar);

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(90));
            _ = await stdoutTask;
            var stderr = await stderrTask;
            Assert.True(process.ExitCode == 0, stderr);

            var generatedDirectory = Path.Combine(outputDirectory, "Generated-Cpp");
            Assert.True(File.Exists(Path.Combine(generatedDirectory, "Extra.cpp")));
            var cmake = File.ReadAllText(Path.Combine(generatedDirectory, "CMakeLists.txt"));
            Assert.Contains("    Extra.cpp", cmake);
            Assert.DoesNotContain(outputDirectory.Replace('\\', '/'), cmake);
            var runtimeCmake = File.ReadAllText(Path.Combine(
                generatedDirectory, "cmake", "ExternalAntlr4Cpp.cmake"));
            Assert.Contains("${CMAKE_CURRENT_BINARY_DIR}/antlr4_runtime", runtimeCmake);
            Assert.Contains("-DANTLR_BUILD_CPP_TESTS:BOOL=OFF", runtimeCmake);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
