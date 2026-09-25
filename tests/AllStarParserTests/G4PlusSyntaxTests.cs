using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Xunit;

namespace AllStarParserTests;

public sealed class G4PlusSyntaxTests
{
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
