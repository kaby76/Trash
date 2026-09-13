using Antlr4.Runtime;
using EditableAntlrTree;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public class TreeOutputTests
{
    [Fact]
    public void AntlrStyleOmitsTerminalTokenTypeElements()
    {
        var root = CreateTree();

        var output = new TreeOutput(null, null)
            .OutputTreeAntlrStyle(root).ToString();

        Assert.Equal("(root @keyframes emptyRule <EOF>)", output);
    }

    [Fact]
    public void AntlrStyleWithTokenTypesRetainsTerminalElements()
    {
        var root = CreateTree();

        var output = new TreeOutput(null, null)
            .OutputTreeAntlrStyleWithTokenTypes(root).ToString();

        Assert.Equal("(root (Keyframes \"@keyframes\") (emptyRule) (EOF \"\"))", output);
    }

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

    private static UnvParseTreeElement CreateTree()
    {
        var root = new UnvParseTreeElement { LocalName = "root", RuleIndex = 0 };
        var terminal = new UnvParseTreeElement { LocalName = "Keyframes", RuleIndex = -1 };
        terminal.ChildNodes.Add(new UnvParseTreeText { Data = "@keyframes" });
        root.ChildNodes.Add(terminal);
        root.ChildNodes.Add(new UnvParseTreeElement { LocalName = "emptyRule", RuleIndex = 1 });
        var eof = new UnvParseTreeElement { LocalName = "EOF", RuleIndex = -1 };
        eof.ChildNodes.Add(new UnvParseTreeText { Data = "" });
        root.ChildNodes.Add(eof);
        return root;
    }
}
