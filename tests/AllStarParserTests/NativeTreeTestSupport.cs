using System.Diagnostics;
using System.Text;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;
using Xunit.Abstractions;

namespace AllStarParserTests;

internal enum NativeTreeStyle
{
    Antlr,
    Block
}

internal sealed record NativeTreeGrammar(
    string Name,
    string DirectoryName,
    string ParserInterpName,
    string LexerInterpName,
    string InputPattern,
    int MinimumSuccessfulFiles,
    double MinimumTokensPerSecond,
    string? NativeTreeDirectoryName = null,
    NativeTreeStyle TreeStyle = NativeTreeStyle.Antlr)
{
    public string RootDirectory =>
        Path.Combine(AppContext.BaseDirectory, "TestData", DirectoryName);
    public string InterpDirectory => Path.Combine(RootDirectory, "interp");
    public string ExamplesDirectory => Path.Combine(RootDirectory, "examples");
    public string ParserInterp => Path.Combine(InterpDirectory, ParserInterpName);
    public string LexerInterp => Path.Combine(InterpDirectory, LexerInterpName);

    public string NativeTreePath(string inputPath) => NativeTreeDirectoryName == null
        ? inputPath + ".tree"
        : Path.Combine(RootDirectory, NativeTreeDirectoryName,
            Path.GetFileNameWithoutExtension(inputPath) + ".tree");

    public IEnumerable<string> InputFiles() =>
        Directory.EnumerateFiles(ExamplesDirectory, InputPattern, SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal);
}

internal static class NativeTreeTestSupport
{
    public static readonly NativeTreeGrammar SysML = new(
        "SysML v2", "sysml-v2", "SysMLv2Parser.interp", "SysMLv2Lexer.interp",
        "*.sysml", MinimumSuccessfulFiles: 1, MinimumTokensPerSecond: 50);

    public static readonly NativeTreeGrammar SystemVerilog = new(
        "SystemVerilog", "systemverilog", "SystemVerilogParser.interp",
        "SystemVerilogLexer.interp", "*.sv",
        MinimumSuccessfulFiles: 120, MinimumTokensPerSecond: 250);

    public static readonly NativeTreeGrammar Acme = new(
        "ACME", "acme", "acme.interp", "acmeLexer.interp", "*.acmetest",
        MinimumSuccessfulFiles: 12, MinimumTokensPerSecond: 50,
        NativeTreeDirectoryName: "native", TreeStyle: NativeTreeStyle.Block);

    public static readonly NativeTreeGrammar Aql = new(
        "AQL", "aql", "ArangoDbParser.interp", "ArangoDbLexer.interp", "*.aql",
        MinimumSuccessfulFiles: 6, MinimumTokensPerSecond: 50,
        NativeTreeDirectoryName: "native", TreeStyle: NativeTreeStyle.Block);

    public static readonly NativeTreeGrammar Asl = new(
        "ASL", "asl", "ASL.interp", "ASLLexer.interp", "loop*",
        MinimumSuccessfulFiles: 2, MinimumTokensPerSecond: 50,
        NativeTreeDirectoryName: "native", TreeStyle: NativeTreeStyle.Block);

    public static IEnumerable<object[]> Cases(NativeTreeGrammar grammar) =>
        grammar.InputFiles().Select(path => new object[] { path });

    public static (TimeSpan Elapsed, int TokenCount) ParseCorpus(
        NativeTreeGrammar grammar,
        ITestOutputHelper output)
    {
        var files = grammar.InputFiles().ToList();
        Assert.NotEmpty(files);

        var totalTokens = 0;
        var elapsed = TimeSpan.Zero;
        var successfulFiles = 0;
        var unsupportedFiles = 0;
        foreach (var inputPath in files)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var (_, tokenCount) = AllStarAtnParser.InterpRunner.Run(
                    grammar.ParserInterp,
                    grammar.LexerInterp,
                    File.ReadAllText(inputPath),
                    Path.GetFileName(inputPath),
                    lineNumbers: false);
                stopwatch.Stop();
                elapsed += stopwatch.Elapsed;
                totalTokens += tokenCount;
                successfulFiles++;
            }
            catch (InvalidOperationException)
            {
                unsupportedFiles++;
            }
            catch (NotSupportedException)
            {
                unsupportedFiles++;
            }
        }

        Assert.True(
            successfulFiles >= grammar.MinimumSuccessfulFiles,
            $"Only {successfulFiles} {grammar.Name} inputs parsed successfully; " +
            $"the regression floor is {grammar.MinimumSuccessfulFiles}.");

        var tokensPerSecond = totalTokens / elapsed.TotalSeconds;
        output.WriteLine(
            $"{grammar.Name}: {successfulFiles}/{files.Count} supported files " +
            $"({unsupportedFiles} currently unsupported), {totalTokens} tokens, " +
            $"{elapsed.TotalSeconds:F3} s, {tokensPerSecond:F1} tokens/s");

        Assert.True(
            tokensPerSecond >= grammar.MinimumTokensPerSecond,
            $"{grammar.Name} throughput {tokensPerSecond:F1} tokens/s was below " +
            $"the {grammar.MinimumTokensPerSecond:F1} tokens/s regression floor.");

        return (elapsed, totalTokens);
    }

    public static void AssertMatchesNativeTree(NativeTreeGrammar grammar, string inputPath)
    {
        ParsingResultSet result;
        int tokenCount;
        try
        {
            (result, tokenCount) = AllStarAtnParser.InterpRunner.Run(
                grammar.ParserInterp,
                grammar.LexerInterp,
                File.ReadAllText(inputPath),
                Path.GetFileName(inputPath),
                lineNumbers: false);
        }
        catch (NotSupportedException exception)
        {
            throw new NotSupportedException(
                $"{Path.GetFileName(inputPath)}: {exception.Message}", exception);
        }

        Assert.True(tokenCount > 0, $"Expected tokens in {Path.GetFileName(inputPath)}.");

        var expectedPath = grammar.NativeTreePath(inputPath);
        Assert.True(File.Exists(expectedPath), $"Native tree is missing: {expectedPath}");

        var expected = NormalizeTree(File.ReadAllText(expectedPath));
        var actual = NormalizeTree(grammar.TreeStyle == NativeTreeStyle.Block
            ? RenderBlockStyle(result)
            : RenderAntlrStyle(result));
        AssertEqualWithContext(expected, actual, Path.GetFileName(inputPath));
    }

    private static string RenderBlockStyle(ParsingResultSet result)
    {
        var output = new StringBuilder();
        foreach (var node in result.Nodes)
            output.AppendLine(new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeBlockStyle(node).ToString());
        return output.ToString();
    }

    private static string RenderAntlrStyle(ParsingResultSet result)
    {
        var output = new StringBuilder();
        foreach (var node in result.Nodes)
        {
            RenderNode(output, node);
            output.AppendLine();
        }
        return output.ToString();
    }

    // DomBuilder represents a terminal as an element named for its token type with
    // one text child. ANTLR's Trees.ToStringTree instead renders only the token text.
    // It also renders a childless rule as its rule name rather than "(rule)".
    private static void RenderNode(StringBuilder output, UnvParseTreeNode node)
    {
        if (node is not UnvParseTreeElement element)
            return;

        if (element.RuleIndex < 0)
        {
            var text = element.LocalName == "EOF"
                ? "<EOF>"
                : Enumerable.Range(0, element.ChildNodes.Length)
                    .Select(i => element.ChildNodes.item(i))
                    .OfType<UnvParseTreeText>()
                    .Select(child => child.Data)
                    .FirstOrDefault() ?? string.Empty;
            output.Append(EscapeWhitespace(text));
            return;
        }

        var children = Enumerable.Range(0, element.ChildNodes.Length)
            .Select(i => element.ChildNodes.item(i))
            .OfType<UnvParseTreeElement>()
            .ToList();
        if (children.Count == 0)
        {
            output.Append(element.LocalName);
            return;
        }

        output.Append('(').Append(element.LocalName);
        foreach (var child in children)
        {
            output.Append(' ');
            RenderNode(output, child);
        }
        output.Append(')');
    }

    private static string EscapeWhitespace(string text) =>
        text.Replace("\t", "\\t", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal);

    private static string NormalizeTree(string tree) =>
        tree.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd('\n', '\r');

    private static void AssertEqualWithContext(string expected, string actual, string fileName)
    {
        if (string.Equals(expected, actual, StringComparison.Ordinal))
            return;

        var commonLength = Math.Min(expected.Length, actual.Length);
        var mismatch = 0;
        while (mismatch < commonLength && expected[mismatch] == actual[mismatch])
            mismatch++;

        const int contextLength = 100;
        var start = Math.Max(0, mismatch - contextLength);
        var expectedContext = expected.Substring(start, Math.Min(contextLength * 2, expected.Length - start));
        var actualContext = actual.Substring(start, Math.Min(contextLength * 2, actual.Length - start));

        Assert.Fail(
            $"Native and AllStar trees differ for {fileName} at character {mismatch}. " +
            $"Expected length: {expected.Length}; actual length: {actual.Length}.\n" +
            $"Expected context: {expectedContext}\nActual context:   {actualContext}");
    }
}
