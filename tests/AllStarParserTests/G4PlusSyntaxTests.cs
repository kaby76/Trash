using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Xunit;

namespace AllStarParserTests;

public sealed class G4PlusSyntaxTests
{
    [Theory]
    [InlineData("lexer grammar L; fragment lowercase : [a-z]+; Uppercase : lowercase -> skip;", "lexergrammar", "lowercase", "Uppercase")]
    [InlineData("parser grammar P; fragment Uppercase : [A-Z]+; lowercase : Uppercase EOF;", "parsergrammar", "Uppercase", "lowercase")]
    [InlineData("grammar C; Uppercase : [A-Z]+; lowercase : Uppercase EOF;", "grammar", "Uppercase", "lowercase")]
    public void RuleNamesAndBodiesDoNotDependOnCase(
        string grammar, string grammarType, string firstRule, string secondRule)
    {
        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        Assert.Equal(grammarType, tree.grammarDecl().grammarType().GetText());
        Assert.Equal(new[] { firstRule, secondRule },
            tree.rules().ruleSpec().Select(rule => rule.identifier().GetText()).ToArray());
        Assert.Contains(Descendants<G4PlusParser.AtomContext>(tree),
            atom => atom.LEXER_CHAR_SET() != null);
    }

    [Fact]
    public void BracketsAreContextualRatherThanSelectedByGrammarKind()
    {
        const string grammar = """
            parser grammar P;
            Uppercase[int n] : [A-Z] lowercase[n];
            lowercase[int n] : [a-z];
            """;

        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        Assert.Equal(2, tree.rules().ruleSpec().Length);
        Assert.Equal(3, Descendants<G4PlusParser.ArgActionBlockContext>(tree).Count());
        Assert.Equal(2, Descendants<G4PlusParser.AtomContext>(tree)
            .Count(atom => atom.LEXER_CHAR_SET() != null));
    }

    [Fact]
    public void LexerModeRulesUseTheSameRuleSyntax()
    {
        const string grammar = """
            lexer grammar L;
            mode inside;
            lowercase : [a-z]+ -> skip;
            Uppercase : lowercase;
            """;

        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        Assert.Empty(tree.rules().ruleSpec());
        Assert.Equal(new[] { "lowercase", "Uppercase" },
            tree.modeSpec()[0].ruleSpec().Select(rule => rule.identifier().GetText()).ToArray());
    }

    [Fact]
    public void ReferencesAndExclusionsAreCaseNeutral()
    {
        const string grammar = """
            parser grammar P;
            Uppercase : lowercase - (Other | other);
            lowercase : ~(Uppercase | other);
            """;

        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        Assert.Equal(new[] { "Other", "other" },
            Descendants<G4PlusParser.ExclusionOperandContext>(tree)
                .Select(operand => operand.GetText()).ToArray());
        Assert.Equal(2, Descendants<G4PlusParser.SetElementContext>(tree).Count());
    }

    [Fact]
    public void JlsIdentifierExclusionsAreRepresentedInParseTree()
    {
        const string grammar = """
            lexer grammar JlsIdentifiers;
            fragment IdentifierChars : [a-z]+;
            ReservedKeyword : 'if';
            BooleanLiteral : 'true' | 'false';
            NullLiteral : 'null';
            Identifier : IdentifierChars - (ReservedKeyword | BooleanLiteral | NullLiteral);
            TypeIdentifier : Identifier - ('permits' | 'record' | 'sealed' | 'var' | 'yield');
            """;

        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        var exclusions = Descendants<G4PlusParser.ExclusionContext>(tree).ToArray();
        Assert.Equal(2, exclusions.Length);
        Assert.Equal(3, exclusions[0].exclusionOperand().Length);
        Assert.Equal(5, exclusions[1].exclusionOperand().Length);
        Assert.Equal("ReservedKeyword", exclusions[0].exclusionOperand()[0].GetText());
        Assert.Equal("'yield'", exclusions[1].exclusionOperand()[4].GetText());
    }

    [Fact]
    public void SingleExclusionAndAlternativePrecedenceAreRecognized()
    {
        const string grammar = """
            parser grammar Example;
            root : name | name - 'yield';
            name : NAME;
            """;

        var tree = Parse(grammar, out var syntaxErrors);

        Assert.Equal(0, syntaxErrors);
        var alternatives = Descendants<G4PlusParser.AlternativeContext>(tree).ToArray();
        Assert.Equal(3, alternatives.Length);
        Assert.Null(alternatives[0].exclusion());
        Assert.Equal("'yield'", alternatives[1].exclusion()!.exclusionOperand()[0].GetText());
    }

    [Fact]
    public void EmptyExclusionListIsRejected()
    {
        const string grammar = "lexer grammar Invalid; Name : [a-z]+ - ();";

        _ = Parse(grammar, out var syntaxErrors);

        Assert.True(syntaxErrors > 0);
    }

    private static G4PlusParser.GrammarSpecContext Parse(string source, out int syntaxErrors)
    {
        var lexer = new G4PlusLexer(new AntlrInputStream(source));
        var parser = new G4PlusParser(new CommonTokenStream(lexer));
        var tree = parser.grammarSpec();
        syntaxErrors = parser.NumberOfSyntaxErrors;
        return tree;
    }

    private static IEnumerable<T> Descendants<T>(IParseTree node) where T : class, IParseTree
    {
        if (node is T match) yield return match;
        for (var i = 0; i < node.ChildCount; ++i)
        {
            foreach (var descendant in Descendants<T>(node.GetChild(i)))
                yield return descendant;
        }
    }
}
