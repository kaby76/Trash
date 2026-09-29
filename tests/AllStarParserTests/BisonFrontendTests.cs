using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class BisonFrontendTests
{
    private const string LexerGrammar = """
        lexer grammar Scan;
        NAME: [a-zA-Z]+;
        BANG: '!';
        WS: [ \t\r\n]+ -> skip;
        """;

    private static GrammarModel BisonModel(string source)
    {
        var lexer = new BisonLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new BisonParser(tokens);
        var tree = parser.input_();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens), "Sample.y");
    }

    private static GrammarModel LexerModel(bool g4plus = false)
    {
        Lexer lexer = g4plus ? new G4PlusLexer(new AntlrInputStream(LexerGrammar))
            : new ANTLRv4Lexer(new AntlrInputStream(LexerGrammar));
        var tokens = new CommonTokenStream(lexer);
        Parser parser = g4plus ? new G4PlusParser(tokens) : new ANTLRv4Parser(tokens);
        Antlr4.Runtime.Tree.IParseTree tree = g4plus ? ((G4PlusParser)parser).grammarSpec()
            : ((ANTLRv4Parser)parser).grammarSpec();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens),
            g4plus ? "Scan.g4p" : "Scan.g4");
    }

    private static string Parse(string grammar, string input, bool g4plus = false)
    {
        var parser = BisonModel(grammar);
        var lexer = LexerModel(g4plus);
        GrammarBinding.Bind([parser, lexer]);
        var directory = Directory.CreateTempSubdirectory("BisonFrontend-").FullName;
        try
        {
            foreach (var model in new[] { parser, lexer })
            {
                ParserAtnFactory factory = model.IsLexer ? new LexerAtnFactory(model) : new ParserAtnFactory(model);
                var atn = factory.CreateATN();
                var content = InterpFormatter.FormatInterp(model, atn, false);
                if (!model.IsLexer)
                {
                    string start = Assert.Single(trinterp.Command.FindEofTerminatedRules(model));
                    content += "\nstart-rule:\n" + atn.ruleToStartState[model.GetRule(start).Index].stateNumber + "\n";
                }
                File.WriteAllText(Path.Combine(directory, model.Name + ".interp"), content);
            }
            var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, parser.Name + ".interp"),
                Path.Combine(directory, lexer.Name + ".interp"), input, "input", false);
            return new TreeOutput(result.Lexer, result.Parser).OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void ExplicitStartAndTokenAliasUseSuppliedLexer()
    {
        const string grammar = "%token NAME \"name\"\n%start message\n%%\nunused: NAME ;\nmessage: \"name\" '!' ;\n%%\n";
        Assert.Equal("(bison_start (message Ada !) <EOF>)", Parse(grammar, "Ada!"));
        Assert.Throws<InvalidOperationException>(() => Parse(grammar, "Ada"));
    }

    [Fact]
    public void G4PlusLexerCanAlsoSupplyBisonTokens()
    {
        const string grammar = "%token NAME \"name\"\n%%\nmessage: \"name\" '!' ;\n%%\n";
        Assert.Equal("(bison_start (message Ada !) <EOF>)", Parse(grammar, "Ada!", g4plus: true));
    }

    [Theory]
    [InlineData("Ada", "(bison_start (message Ada tail) <EOF>)")]
    [InlineData("Ada!", "(bison_start (message Ada (tail !)) <EOF>)")]
    public void EmptyAlternativeAndDefaultFirstRule(string input, string expected)
    {
        const string grammar = "%token NAME\n%%\nmessage: NAME tail ;\ntail: %empty | '!' ;\n%%\n";
        Assert.Equal(expected, Parse(grammar, input));
    }

    [Fact]
    public void SemanticActionsDoNotChangeRecognizedLanguage()
    {
        const string grammar = "%token NAME\n%%\nmessage: NAME { do_something(); } '!' ;\n%%\n";
        Assert.Contains("(message Ada !)", Parse(grammar, "Ada!"));
    }

    [Fact]
    public void MissingLexerTokenIsReportedBeforeAtnConstruction()
    {
        var parser = BisonModel("%token UNKNOWN\n%%\nmessage: UNKNOWN ;\n%%\n");
        var error = Assert.Throws<InvalidOperationException>(() => GrammarBinding.Bind([parser, LexerModel()]));
        Assert.Contains("UNKNOWN", error.Message);
    }

    [Theory]
    [InlineData("%left '+'\n%%\nmessage: '!' ;\n%%\n", "precedence")]
    [InlineData("%glr-parser\n%%\nmessage: '!' ;\n%%\n", "%glr-parser")]
    [InlineData("%%\nmessage: '!' %prec NAME ;\n%%\n", "%prec")]
    [InlineData("%start absent\n%%\nmessage: missing ;\n%%\n", "start")]
    public void UnsupportedOrInvalidGrammarHasDiagnostics(string grammar, string expected)
    {
        Assert.Contains(expected, Assert.ThrowsAny<Exception>(() => BisonModel(grammar)).Message,
            StringComparison.OrdinalIgnoreCase);
    }
}
