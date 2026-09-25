using Antlr4.Runtime;
using Xunit;

namespace AllStarParserTests;

public sealed class G4PlusCompatibilityTests
{
    // These are unchanged ANTLR4 grammars, copied to test output with their
    // directory structure intact. New grammars-v4 fixtures join the test
    // automatically, including combined, lexer, and parser grammars.
    public static IEnumerable<object[]> ExistingGrammars()
    {
        var corpus = Path.Combine(AppContext.BaseDirectory, "TestData", "g4plus");
        return Directory.EnumerateFiles(corpus, "*.g4", SearchOption.AllDirectories)
            .Where(path => !Path.GetRelativePath(corpus, path)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part.StartsWith("Generated-", StringComparison.OrdinalIgnoreCase)))
            .Select(path => new object[] { Path.GetRelativePath(corpus, path) });
    }

    [Theory]
    [MemberData(nameof(ExistingGrammars))]
    public void ExistingAntlr4GrammarParsesWithG4Plus(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "g4plus", relativePath);
        Assert.True(File.Exists(path), $"Missing grammar: {path}");

        var lexer = new G4PlusLexer(new AntlrInputStream(File.ReadAllText(path)));
        var lexerErrors = new LexerErrors();
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(lexerErrors);
        var parser = new G4PlusParser(new CommonTokenStream(lexer));
        var tree = parser.grammarSpec();

        Assert.True(lexerErrors.Messages.Count == 0,
            $"{relativePath}: {string.Join(Environment.NewLine, lexerErrors.Messages)}");
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        Assert.NotNull(tree.Eof());
    }

    private sealed class LexerErrors : IAntlrErrorListener<int>
    {
        public List<string> Messages { get; } = new();

        public void SyntaxError(TextWriter output, IRecognizer recognizer,
            int offendingSymbol, int line, int charPositionInLine,
            string msg, RecognitionException e)
        {
            Messages.Add($"{line}:{charPositionInLine} {msg}");
        }
    }
}
