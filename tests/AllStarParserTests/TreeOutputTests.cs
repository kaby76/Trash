using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public class TreeOutputTests
{
    [Fact]
    public void BlockTreeUsesNumberForUnnamedTokenChannel()
    {
        var lexer = new MyLexer(CharStreams.fromString(string.Empty))
        {
            _channelNames = ["DEFAULT_TOKEN_CHANNEL", "HIDDEN", null]
        };
        var root = new UnvParseTreeElement { LocalName = "root" };
        root.ChildNodes.Add(new UnvParseTreeAttr
        {
            Name = "COMMENT",
            StringValue = "// comment",
            Channel = 2,
            TokenType = 1
        });

        var output = new TreeOutput(lexer, new EditableAntlrTree.MyParser())
            .OutputTreeBlockStyle(root).ToString();

        Assert.Contains("chnl:2", output);
    }
}
