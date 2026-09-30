using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class W3CebnfFrontendTests
{
    private static GrammarModel Model(string source)
    {
        var lexer = new W3CebnfLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new W3CebnfParser(tokens);
        var tree = parser.grammar_();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens), "Sample.ebnf");
    }

    private static string Parse(string grammar, string input)
    {
        var model = Model(grammar);
        var models = new[] { model, model.ImplicitLexer };
        GrammarBinding.Bind(models);
        var directory = Directory.CreateTempSubdirectory("W3CebnfFrontend-").FullName;
        try
        {
            foreach (var grammarModel in models)
            {
                ParserAtnFactory factory = grammarModel.IsLexer
                    ? new LexerAtnFactory(grammarModel) : new ParserAtnFactory(grammarModel);
                var atn = factory.CreateATN();
                var content = InterpFormatter.FormatInterp(grammarModel, atn, false);
                if (!grammarModel.IsLexer)
                {
                    var start = Assert.Single(trinterp.Command.FindEofTerminatedRules(grammarModel));
                    content += "\nstart-rule:\n" + atn.ruleToStartState[grammarModel.GetRule(start).Index].stateNumber + "\n";
                }
                File.WriteAllText(Path.Combine(directory, grammarModel.Name + ".interp"), content);
            }
            var (result, _) = AllStarAtnParser.InterpRunner.Run(
                Path.Combine(directory, model.Name + ".interp"),
                Path.Combine(directory, model.ImplicitLexer.Name + ".interp"), input, "input", false);
            return new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData("hello Ada!\n")]
    [InlineData("hello Bob!\n")]
    [InlineData("hello Zoe!\n")]
    public void StringsHexSetsAlternativesAndRepetition(string input)
    {
        const string grammar = "message ::= \"hello\" #x20 name \"!\" #xA\n" +
                               "name ::= \"Ada\" | \"Bob\" | [A-Z] [a-z]+\n";
        var tree = Parse(grammar, input);
        Assert.Contains("(w3c_start", tree);
        Assert.Contains("(message", tree);
        Assert.Contains("(name", tree);
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "hello Bob?\n"));
    }

    [Fact]
    public void GroupsOptionalStarAndEmptyAlternative()
    {
        const string grammar = "start ::= (\"x\" | \"y\")? \"z\"* |\n";
        Assert.Contains("(start", Parse(grammar, "xzz"));
        Assert.Contains("start <EOF>", Parse(grammar, ""));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "q"));
    }

    [Fact]
    public void ComplementedSet()
    {
        const string grammar = "start ::= [^x]+\n";
        Assert.Contains("(start", Parse(grammar, "y"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "x"));
    }

    [Fact]
    public void HexadecimalSetRanges()
    {
        const string grammar = "start ::= [#x41-#x43]+\n";
        Assert.Contains("(start", Parse(grammar, "ABC"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "D"));
    }

    [Theory]
    [InlineData("start ::= missing\n", "Undefined")]
    [InlineData("start ::= \"a\"\nstart ::= \"b\"\n", "Duplicate")]
    [InlineData("start ::= [a-z] - [aeiou]\n", "difference")]
    [InlineData("start ::= #x1F600\n", "BMP")]
    public void InvalidOrUnsupportedW3CebnfHasDiagnostics(string grammar, string expected)
    {
        Assert.Contains(expected, Assert.ThrowsAny<Exception>(() => Model(grammar)).Message,
            StringComparison.OrdinalIgnoreCase);
    }
}
