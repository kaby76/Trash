using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class CongoChoiceAnalyzerTests
{
    [Fact]
    public void DelegatedCommentChoiceUsesShortTokenPrefix()
    {
        const string grammar = """
            grammar Lines;
            start: line+ EOF;
            line: commentLine | instructionLine;
            commentLine: ws? comment EOL;
            instructionLine: ws? WORD EOL;
            ws: WS;
            comment: COMMENT;
            WS: [ \t]+;
            COMMENT: '#' ~[\r\n]*;
            WORD: [a-z]+;
            EOL: '\n';
            """;
        var lexer = new ANTLRv4Lexer(new AntlrInputStream(grammar));
        var tokens = new CommonTokenStream(lexer);
        var parser = new ANTLRv4Parser(tokens);
        var tree = parser.grammarSpec();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        var dom = new ConvertToDOM().BottomUpConvert(tree, null, parser, lexer, tokens);

        var facts = CongoChoiceAnalyzer.Analyze(GrammarNode.FromDom(dom)).ToArray();

        Assert.Contains("line#P#1#(<WS>)? <COMMENT>", facts);
        Assert.DoesNotContain(CongoChoiceAnalyzer.Diagnostics(GrammarNode.FromDom(dom)),
            diagnostic => diagnostic.Contains("rule 'line'"));
    }
}
