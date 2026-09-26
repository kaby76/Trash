using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class RexInterpTests
{
    private static GrammarModel Model(string source)
    {
        var lexer = new rexLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new rexParser(tokens);
        var tree = parser.grammar_();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        return new GrammarParser().Parse(new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens), "Sample.rex");
    }

    private static string Parse(string grammar, string input)
    {
        var model = Model(grammar);
        var models = new[] { model, model.ImplicitLexer };
        GrammarBinding.Bind(models);
        var directory = Directory.CreateTempSubdirectory("RexInterp-").FullName;
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
            var (result, _) = AllStarAtnParser.InterpRunner.Run(Path.Combine(directory, "Sample.interp"),
                Path.Combine(directory, "SampleLexer.interp"), input, "input", false);
            return new TreeOutput(result.Lexer, result.Parser).OutputTreeAntlrStyle(Assert.Single(result.Nodes)).ToString();
        }
        finally { Directory.Delete(directory, true); }
    }

    private const string Arithmetic = """
        Start ::= Expr End
        Expr ::= Expr '+' Term | Term
        Term ::= Number ('*' Number)*
        <?TOKENS?>
        Number ::= Digit+
        Digit ::= [0-9]
        End ::= $
        """;

    [Fact]
    public void ArithmeticPreservesLeftRecursiveTree()
    {
        Assert.Equal("(Start (Expr (Expr (Term 1)) + (Term 2 * 3)) <EOF>)", Parse(Arithmetic, "1+2*3"));
        Assert.Throws<InvalidOperationException>(() => Parse(Arithmetic, "1+"));
        Assert.True(Model(Arithmetic).ImplicitLexer.GetRule("Digit").IsFragment);
    }

    [Theory]
    [InlineData("[]", "(Start [ ] <EOF>)")]
    [InlineData("[red,blue]", "(Start [ red , blue ] <EOF>)")]
    public void ListsSupportOptionalGroupsAndRepeatedSequences(string input, string expected)
    {
        Assert.Equal(expected, Parse("""
            Start ::= '[' (Word (',' Word)*)? ']' End
            <?TOKENS?>
            Word ::= [a-z]+
            End ::= $
            """, input));
    }

    [Fact]
    public void LiteralQuotesCodesAndExplicitWhitespace()
    {
        const string grammar = """
            Start ::= "'" Space? X End
            <?TOKENS?>
            Space ::= ' '+
            X ::= #x78
            End ::= $
            """;
        Assert.Equal("(Start ' x <EOF>)", Parse(grammar, "'x"));
        Assert.Contains("x <EOF>", Parse(grammar, "' x"));
    }

    [Fact]
    public void HexadecimalClassRangesAreConverted()
    {
        Assert.Equal("(Start AZ <EOF>)", Parse("""
            Start ::= word End
            <?TOKENS?>
            word ::= [\u0041-\u005A]+
            End ::= $
            """, "AZ"));
    }

    [Theory]
    [InlineData("#x1F600")]
    [InlineData("\\u1F600")]
    [InlineData("[\\u1F600]")]
    [InlineData("[\\uFFFF-\\u10000]")]
    public void SupplementaryCodesFailDuringCompilation(string expression)
    {
        var error = Assert.Throws<NotSupportedException>(() => Model(
            "Start ::= X End\n<?TOKENS?>\nX ::= " + expression + "\nEnd ::= $"));
        Assert.Contains("outside the BMP", error.Message);
    }

    [Theory]
    [InlineData("Start ::= Missing", "Undefined")]
    [InlineData("Start ::= 'a' / 'b'", "operator")]
    [InlineData("Start ::= A\n<?TOKENS?>\nA ::= 'a' - 'b'", "operator")]
    [InlineData("Start ::= A\n<?TOKENS?>\nA ::= [^a]", "complemented")]
    public void UnsupportedOrUndefinedConstructsHaveDiagnostics(string grammar, string diagnostic)
    {
        Assert.Contains(diagnostic, Assert.ThrowsAny<Exception>(() => Model(grammar)).Message);
    }
}
