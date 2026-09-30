using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class AbnfFrontendTests
{
    private static GrammarModel Model(string source)
    {
        var lexer = new AbnfLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new AbnfParser(tokens);
        var tree = parser.rulelist();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens), "Sample.abnf");
    }

    private static string Parse(string grammar, string input)
    {
        var model = Model(grammar);
        var models = new[] { model, model.ImplicitLexer };
        GrammarBinding.Bind(models);
        var directory = Directory.CreateTempSubdirectory("AbnfFrontend-").FullName;
        try
        {
            foreach (var m in models)
            {
                ParserAtnFactory factory = m.IsLexer ? new LexerAtnFactory(m) : new ParserAtnFactory(m);
                var atn = factory.CreateATN();
                var content = InterpFormatter.FormatInterp(m, atn, false);
                if (!m.IsLexer)
                {
                    var start = Assert.Single(trinterp.Command.FindEofTerminatedRules(m));
                    content += "\nstart-rule:\n" + atn.ruleToStartState[m.GetRule(start).Index].stateNumber + "\n";
                }
                File.WriteAllText(Path.Combine(directory, m.Name + ".interp"), content);
            }
            var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, model.Name + ".interp"),
                Path.Combine(directory, model.ImplicitLexer.Name + ".interp"), input, "input", false);
            return new TreeOutput(result.Lexer, result.Parser).OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData("a5")]
    [InlineData("A57")]
    public void CaseInsensitiveStringsAndCoreDigit(string input)
    {
        var tree = Parse("start = \"a\" 1*2DIGIT\n", input);
        Assert.Contains("(start", tree);
        Assert.Contains("(start " + input[0], tree);
        Assert.Contains("(digit " + input[1] + ")", tree);
        Assert.Throws<InvalidOperationException>(() => Parse("start = \"a\" 1*2DIGIT\n", "a"));
    }

    [Fact]
    public void IncrementalAlternativesAreCaseInsensitive()
    {
        const string grammar = "Start = \"red\"\nstart =/ \"blue\"\n";
        Assert.Contains("(start", Parse(grammar, "RED"));
        Assert.Contains("(start", Parse(grammar, "Blue"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "green"));
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("C")]
    [InlineData("D")]
    public void BinaryDecimalHexAndNumericSequences(string input)
    {
        const string grammar = "start = %x41.42 / %d67 / %b01000100\n";
        Assert.Contains("(start", Parse(grammar, input));
    }

    [Fact]
    public void GroupsOptionsAndRanges()
    {
        const string grammar = "start = (\"a\" / \"b\") [\"-\"] %x30-39\n";
        Assert.Contains("(start", Parse(grammar, "B-7"));
        Assert.Contains("(start", Parse(grammar, "a3"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "a-z"));
    }

    [Theory]
    [InlineData("start = missing\n", "Undefined")]
    [InlineData("start = \"a\"\nstart = \"b\"\n", "Duplicate")]
    [InlineData("start =/ \"a\"\n", "undefined")]
    [InlineData("start = <prose>\n", "prose")]
    [InlineData("start = 3*2\"a\"\n", "maximum")]
    [InlineData("start = 257\"a\"\n", "256")]
    [InlineData("start = %x1F600\n", "BMP")]
    public void InvalidOrUnsupportedAbnfHasDiagnostics(string grammar, string expected)
    {
        Assert.Contains(expected, Assert.ThrowsAny<Exception>(() => Model(grammar)).Message,
            StringComparison.OrdinalIgnoreCase);
    }
}
