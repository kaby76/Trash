using System.Text;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public class CorundumPrecedenceTests
{
    [Fact]
    public void AllStarMatchesEarleyForNestedIntegerAndFloatPrecedence()
    {
        var interpDirectory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "corundum", "interp");
        var parserInterp = Path.Combine(interpDirectory, "Corundum.interp");
        var lexerInterp = Path.Combine(interpDirectory, "CorundumLexer.interp");
        const string input =
            "exp1 = 100+2*3/(3+4*3)\n" +
            "exp2 = 100.5+2*3.1/(3+4*3)\n";

        var (earley, _) = EarleyAtnParser.InterpRunner.Run(
            parserInterp, lexerInterp, input, "precedence.rb", lineNumbers: false);
        var (allStar, _) = AllStarAtnParser.InterpRunner.Run(
            parserInterp, lexerInterp, input, "precedence.rb", lineNumbers: false);

        Assert.Equal(Render(earley), Render(allStar));
    }

    private static string Render(ParsingResultSet result)
    {
        var output = new StringBuilder();
        foreach (UnvParseTreeNode node in result.Nodes)
            output.AppendLine(new TreeOutput(result.Lexer, result.Parser)
                .OutputTreeBlockStyle(node).ToString());
        return output.ToString();
    }
}
