using System.Diagnostics;
using AllStarAtnParser;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public sealed class XQueryHooksTests
{
    [Fact]
    public void JavaSourceHookTranslatesEscapesAndRetainsOriginalOffsets()
    {
        string query = Path.Combine(AppContext.BaseDirectory,
            "TestData", "java27-g4x", "java-unicode.xq");
        string raw = "\"-\\u005c\\-\"";
        var transformed = SourceTransformHook.Apply(raw, query);
        Assert.Equal("\"-\\\\-\"", transformed.Text);
        Assert.Equal(2, transformed.OriginalOffset(2));
        Assert.Equal(8, transformed.OriginalOffset(3));
        Assert.Equal(raw.Length, transformed.OriginalOffset(transformed.Text.Length));

        string ineligible = "\\\\u005c";
        Assert.Equal(ineligible,
            SourceTransformHook.Apply(ineligible, query).Text);
        Assert.Equal(@"\u005a",
            SourceTransformHook.Apply(@"\u005cu005a", query).Text);
        Assert.Equal(@"\\",
            SourceTransformHook.Apply(@"\u005c\u005c", query).Text);
        Assert.Equal(@"\\n",
            SourceTransformHook.Apply(@"\\\u006e", query).Text);
        Assert.Throws<InvalidDataException>(() =>
            SourceTransformHook.Apply("\\u00xz", query));
    }

    [Fact]
    public void SourceOnlyHookWorksWithIndirectLeftRecursion()
    {
        string fixture = Path.Combine(AppContext.BaseDirectory,
            "TestData", "indirect-left-recursion");
        string manifest = Path.Combine(AppContext.BaseDirectory,
            "TestData", "java27-g4x", "hooks.json");
        var (result, _) = InterpRunner.Run(
            Path.Combine(fixture, "IndirectLeftRecursion.interp"),
            Path.Combine(fixture, "IndirectLeftRecursionLexer.interp"),
            "\\u0031+\\u0032+3", "unicode.txt", false,
            indirectLeftRecursion: true, xqueryHooksPath: manifest);
        string tree = new TreeOutput(result.Lexer, result.Parser)
            .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        Assert.Contains("1", tree);
        Assert.Contains("2", tree);
        Assert.Contains("3", tree);
    }

    [Fact]
    public void SourceOnlyHookReportsOriginalPositionsForLexerAndParserFailures()
    {
        string fixture = Path.Combine(AppContext.BaseDirectory,
            "TestData", "indirect-left-recursion");
        string manifest = Path.Combine(AppContext.BaseDirectory,
            "TestData", "java27-g4x", "hooks.json");
        string parser = Path.Combine(fixture, "IndirectLeftRecursion.interp");
        string lexer = Path.Combine(fixture, "IndirectLeftRecursionLexer.interp");

        var lexerError = Assert.Throws<InvalidOperationException>(() =>
            InterpRunner.Run(parser, lexer, "\\u0031+?", "lexer.txt", false,
                indirectLeftRecursion: true, xqueryHooksPath: manifest));
        Assert.Contains("lexer.txt: line 1:7", lexerError.Message);
        Assert.Contains("Lexer error", lexerError.Message);

        var contextError = Assert.Throws<InvalidOperationException>(() =>
            InterpRunner.Run(parser, lexer, "\\u0031+?", "context.txt", false,
                contextAwareLexing: true, indirectLeftRecursion: true,
                xqueryHooksPath: manifest));
        Assert.Contains("context.txt: line 1:7", contextError.Message);

        var parserError = Assert.Throws<InvalidOperationException>(() =>
            InterpRunner.Run(parser, lexer, "\\u0031+", "parser.txt", false,
                indirectLeftRecursion: true, xqueryHooksPath: manifest));
        Assert.Contains("parser.txt: line 1:7", parserError.Message);
        Assert.Contains("input rejected by grammar", parserError.Message);
    }

    [Fact]
    public async Task SourceOnlyHookReportsOriginalTokenOffsets()
    {
        string fixture = Path.Combine(AppContext.BaseDirectory,
            "TestData", "indirect-left-recursion");
        string manifest = Path.Combine(AppContext.BaseDirectory,
            "TestData", "java27-g4x", "hooks.json");
        var directory = Directory.CreateTempSubdirectory("SourceTokenOffsets-").FullName;
        try
        {
            string file = Path.Combine(directory, "unicode.txt");
            File.WriteAllText(file, "\\u0031+2");
            var run = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, "--allstar", "-L", fixture, "--indirect-left-recursion",
                "--xquery-hooks", manifest, "--no-output", "--tokens", file);
            Assert.Contains("[@0,0:5='1'", run.Error);
            Assert.Contains("[@1,6:6='+'", run.Error);
            Assert.Contains("[@2,7:7='2'", run.Error);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task InterpretedLexerPredicateUsesCommittedDeclaration()
    {
        var directory = Directory.CreateTempSubdirectory("XQueryHooks-").FullName;
        try
        {
            var source = Path.Combine(AppContext.BaseDirectory,
                "TestData", "xquery-contextual-lexing");
            foreach (var name in new[] { "FooLexer.g4", "FooParser.g4", "hooks.json",
                         "hyphen.xq", "on-decl-enter.xq", "on-decl-exit.xq", "input.txt" })
                File.Copy(Path.Combine(source, name), Path.Combine(directory, name));
            var lexer = Path.Combine(directory, "FooLexer.g4");
            var parser = Path.Combine(directory, "FooParser.g4");

            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, "-t", "ANTLRv4", lexer, parser);
            Assert.Equal(0, parsed.Exit);
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.Equal(0, generated.Exit);

            var (result, _) = InterpRunner.Run(
                Path.Combine(directory, "FooParser.interp"),
                Path.Combine(directory, "FooLexer.interp"),
                await File.ReadAllTextAsync(Path.Combine(directory, "input.txt")),
                "input.txt", false,
                xqueryHooksPath: Path.Combine(directory, "hooks.json"));
            var tree = new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
            Assert.Contains("(decl var c-4)", tree);
            Assert.Contains("c-4-6", tree);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public async Task JavaParserPredicatesUseLookaheadAndPartialTree()
    {
        var directory = Directory.CreateTempSubdirectory("JavaXQueryHooks-").FullName;
        try
        {
            var source = Path.Combine(AppContext.BaseDirectory,
                "TestData", "java-allstar-xquery");
            foreach (var name in new[] { "JavaLexer.g4", "JavaParser.g4", "hooks.json",
                         "is-not-identifier-assign.xq", "last-record-component.xq",
                         "input.java", "invalid-record.java" })
                File.Copy(Path.Combine(source, name), Path.Combine(directory, name));

            var parsed = await RunCli(typeof(Trash.Program).Assembly.Location,
                null, "-t", "ANTLRv4", Path.Combine(directory, "JavaLexer.g4"),
                Path.Combine(directory, "JavaParser.g4"));
            var generated = await RunCli(typeof(trinterp.Program).Assembly.Location,
                parsed.Output, "-o", directory);
            Assert.Equal(0, generated.Exit);

            string parser = Path.Combine(directory, "JavaParser.interp");
            string lexer = Path.Combine(directory, "JavaLexer.interp");
            string manifest = Path.Combine(directory, "hooks.json");
            var valid = await File.ReadAllTextAsync(Path.Combine(directory, "input.java"));
            var invalid = await File.ReadAllTextAsync(
                Path.Combine(directory, "invalid-record.java"));

            var runtimeCache = new InterpRuntimeCache();
            var statistics = new ParserStatistics();
            var (result, _) = InterpRunner.Run(parser, lexer, valid,
                "input.java", false, parserStatistics: statistics,
                runtimeCache: runtimeCache,
                xqueryHooksPath: manifest);
            Assert.Single(result.Nodes);
            Assert.True(statistics.DfaStatesCreated > 0);
            Assert.True(statistics.FullContextFallbacks <
                statistics.AdaptivePredictionCalls);
            Assert.Throws<InvalidOperationException>(() =>
                InterpRunner.Run(parser, lexer, invalid, "invalid-record.java",
                    false, runtimeCache: runtimeCache,
                    xqueryHooksPath: manifest));
            var (withoutHooks, _) = InterpRunner.Run(parser, lexer, invalid,
                "invalid-record.java", false, runtimeCache: runtimeCache);
            Assert.Single(withoutHooks.Nodes);
        }
        finally { Directory.Delete(directory, true); }
    }

    private static async Task<(int Exit, byte[] Output, string Error)> RunCli(
        string assembly, byte[]? input, params string[] args)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        start.ArgumentList.Add(assembly);
        foreach (var arg in args) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        using var output = new MemoryStream();
        var read = process.StandardOutput.BaseStream.CopyToAsync(output);
        var error = process.StandardError.ReadToEndAsync();
        try
        {
            if (input != null) await process.StandardInput.BaseStream.WriteAsync(input);
            process.StandardInput.Close();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
            await read;
            string stderr = await error;
            Assert.True(process.ExitCode == 0, stderr);
            return (process.ExitCode, output.ToArray(), stderr);
        }
        finally { if (!process.HasExited) process.Kill(entireProcessTree: true); }
    }
}
