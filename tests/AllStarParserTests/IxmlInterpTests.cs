using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class IxmlInterpTests
{
    private static GrammarModel Model(string source)
    {
        var lexer = new ixmlLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new ixmlParser(tokens);
        var tree = parser.ixml();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens), "Sample.ixml");
    }
    private static string Parse(string grammar, string input)
    {
        var model = Model(grammar);
        var models = new[] { model, model.ImplicitLexer };
        GrammarBinding.Bind(models);
        var directory = Directory.CreateTempSubdirectory("IxmlInterp-").FullName;
        try
        {
            foreach (var m in models)
            {
                ParserAtnFactory factory = m.IsLexer ? new LexerAtnFactory(m) : new ParserAtnFactory(m);
                var atn = factory.CreateATN();
                var content = InterpFormatter.FormatInterp(m, atn, false);
                if (!m.IsLexer)
                    content += "\nstart-rule:\n" + atn.ruleToStartState[m.GetRule(Assert.Single(trinterp.Command.FindEofTerminatedRules(m))).Index].stateNumber + "\n";
                File.WriteAllText(Path.Combine(directory, m.Name + ".interp"), content);
            }
            var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, model.Name + ".interp"),
                Path.Combine(directory, model.ImplicitLexer.Name + ".interp"), input, "input", false);
            return new TreeOutput(result.Lexer, result.Parser).OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void RecursiveFirstRuleAndFullConsumption()
    {
        const string grammar = "s: '(', s, ')'; 'x'.";
        Assert.Equal("(ixml_start (s ( (s x) )) <EOF>)", Parse(grammar, "(x)"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "xx"));
    }
    [Theory]
    [InlineData("", "(ixml_start s <EOF>)")]
    [InlineData("a,b", "(ixml_start (s a , b) <EOF>)")]
    public void SeparatedRepetition(string input, string expected)
    {
        Assert.Equal(expected, Parse("s: ['a'-'z'] ** ','.", input));
        Assert.Throws<InvalidOperationException>(() => Parse("s: ['a'-'z'] ++ ','.", ""));
        Assert.Throws<InvalidOperationException>(() => Parse("s: ['a'-'z'] ** ','.", "a,"));
    }
    [Fact]
    public void OverlappingStringsAndSetsStayScannerless()
    {
        Assert.Equal("(ixml_start (s a b) <EOF>)", Parse("s: 'a', ['a'-'z']; 'abc'.", "ab"));
        Assert.Equal("(ixml_start (s a b c) <EOF>)", Parse("s: 'a', ['a'-'z']; 'abc'.", "abc"));
    }
    [Fact]
    public void QuotesCodesNegationAndOptionalGroups()
    {
        Assert.Equal("(ixml_start (s ' A x) <EOF>)", Parse("s: '''', #41, (~['z'])?.", "'Ax"));
        Assert.Throws<InvalidOperationException>(() => Parse("s: ~['z'].", "z"));
    }
    [Fact]
    public void EmptyAlternativesGroupsAndExplicitWhitespace()
    {
        const string grammar = "ixml version '1.0'. s: ('a'; 'b'), gap, 'c'. gap: ' '; .";
        Assert.Equal("(ixml_start (s a gap c) <EOF>)", Parse(grammar, "ac"));
        Assert.Contains("(gap", Parse(grammar, "b c"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "b  c"));
    }
    [Fact]
    public void SyntheticNamesDoNotCollideWithUserRules()
    {
        Assert.Equal("(ixml_start1 (ixml_start (IXML_CHAR_0 x)) <EOF>)",
            Parse("ixml_start: IXML_CHAR_0. IXML_CHAR_0: 'x'.", "x"));
    }
    [Fact]
    public void DirectLeftRecursion()
    {
        Assert.Equal("(ixml_start (s (s 1) + 2) <EOF>)", Parse("s: s, '+', ['0'-'9']; ['0'-'9'].", "1+2"));
    }
    [Theory]
    [InlineData("s: missing.", "Undefined")]
    [InlineData("s: 'a'. s: 'b'.", "Duplicate")]
    [InlineData("-s: 'a'.", "mark")]
    [InlineData("s: + 'a'.", "insertion")]
    [InlineData("s: [L].", "categories")]
    [InlineData("s: #1F600.", "BMP")]
    [InlineData("s: ['z'-'a'].", "Reversed")]
    [InlineData("s: [].", "Empty")]
    public void UnsupportedSyntaxHasDiagnostics(string grammar, string message)
    {
        Assert.Contains(message, Assert.ThrowsAny<Exception>(() => Model(grammar)).Message);
    }
}
