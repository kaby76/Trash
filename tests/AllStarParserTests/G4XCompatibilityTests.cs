using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Xunit;

namespace AllStarParserTests;

public sealed class G4XCompatibilityTests
{
    // These are unchanged ANTLR4 grammars, copied to test output with their
    // directory structure intact. New grammars-v4 fixtures join the test
    // automatically, including combined, lexer, and parser grammars.
    public static IEnumerable<object[]> ExistingGrammars()
    {
        var corpus = Path.Combine(AppContext.BaseDirectory, "TestData", "g4x");
        return Directory.EnumerateFiles(corpus, "*.g4", SearchOption.AllDirectories)
            .Where(path => !Path.GetRelativePath(corpus, path)
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part.StartsWith("Generated-", StringComparison.OrdinalIgnoreCase)))
            .Select(path => new object[] { Path.GetRelativePath(corpus, path) });
    }

    [Theory]
    [MemberData(nameof(ExistingGrammars))]
    public void UnlabeledAntlr4GrammarParsesWithG4X(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "g4x", relativePath);
        Assert.True(File.Exists(path), $"Missing grammar: {path}");
        var source = File.ReadAllText(path);

        // G4X deliberately rejects ANTLR4's alternative and element labels.
        var nativeLexer = new ANTLRv4Lexer(new AntlrInputStream(source));
        var nativeParser = new ANTLRv4Parser(new CommonTokenStream(nativeLexer));
        var nativeTree = nativeParser.grammarSpec();
        var hasLabels = Descendants(nativeTree).Any(node =>
            node is ANTLRv4Parser.LabeledAltContext alt && alt.POUND() != null ||
            node is ANTLRv4Parser.LabeledElementContext);

        var lexer = new G4XLexer(new AntlrInputStream(source));
        var lexerErrors = new LexerErrors();
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(lexerErrors);
        var parser = new G4XParser(new CommonTokenStream(lexer));
        var tree = parser.grammarSpec();

        if (hasLabels)
            Assert.True(lexerErrors.Messages.Count != 0 || parser.NumberOfSyntaxErrors != 0,
                $"{relativePath}: G4X accepted ANTLR4 labels.");
        else
        {
            Assert.True(lexerErrors.Messages.Count == 0,
                $"{relativePath}: {string.Join(Environment.NewLine, lexerErrors.Messages)}");
            Assert.Equal(0, parser.NumberOfSyntaxErrors);
            Assert.NotNull(tree.Eof());
        }
    }

    private static IEnumerable<IParseTree> Descendants(IParseTree tree)
    {
        yield return tree;
        for (var i = 0; i < tree.ChildCount; i++)
            foreach (var child in Descendants(tree.GetChild(i)))
                yield return child;
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
