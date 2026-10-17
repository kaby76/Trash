using AllStarAtnParser;
using Atn;
using ParseTreeEditing.UnvParseTreeDOM;
using Xunit;

namespace AllStarParserTests;

public sealed class AstMappingTests
{
    [Fact]
    public void DirectLeftRecursionReducesAtEachPush()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                expr(value:NUMBER) -> ^(LitExprNode[value]);
                expr(left:expr, op:_, right:NUMBER)
                    -> ^(ArithmExprNode left ^(ArithmOpNode[op]) ^(LitExprNode[right]));
            }
            """);
        var tokens = Tokens("1+2", (1, 0, 0), (2, 1, 1), (1, 2, 2));
        var builder = new AstTreeBuilder(mapping, ["expr"], ["", "NUMBER", "PLUS"], tokens);

        builder.OnEvent(ParseEvent.EnterRecursionRule(0));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.PushRecursionContext(0));
        builder.OnEvent(ParseEvent.Consume(1));
        builder.OnEvent(ParseEvent.Consume(2));
        builder.OnEvent(ParseEvent.ExitRecursionRule(0));

        var root = Assert.IsType<UnvParseTreeElement>(builder.Root);
        Assert.Equal("ArithmExprNode", root.LocalName);
        Assert.Equal("LitExprNode", Assert.IsType<UnvParseTreeElement>(
            root.ChildNodes.item(0)).LocalName);
        Assert.Equal("ArithmOpNode", Assert.IsType<UnvParseTreeElement>(
            root.ChildNodes.item(1)).LocalName);
    }

    [Fact]
    public void SpliceFlattensPreviouslyReducedList()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                items(value:NUMBER) -> ^(ExprListNode ^(LitExprNode[value]));
                items(left:items, ',', value:NUMBER)
                    -> ^(ExprListNode ...left ^(LitExprNode[value]));
            }
            """);
        var tokens = Tokens("1,2", (1, 0, 0), (2, 1, 1), (1, 2, 2));
        var builder = new AstTreeBuilder(mapping, ["items"], ["", "NUMBER", "COMMA"], tokens);

        builder.OnEvent(ParseEvent.EnterRecursionRule(0));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.PushRecursionContext(0));
        builder.OnEvent(ParseEvent.Consume(1));
        builder.OnEvent(ParseEvent.Consume(2));
        builder.OnEvent(ParseEvent.ExitRecursionRule(0));

        var root = Assert.IsType<UnvParseTreeElement>(builder.Root);
        Assert.Equal("ExprListNode", root.LocalName);
        Assert.Equal(2, root.ChildNodes.Length);
        Assert.All(new[] { root.ChildNodes.item(0), root.ChildNodes.item(1) },
            child => Assert.Equal("LitExprNode",
                Assert.IsType<UnvParseTreeElement>(child).LocalName));
    }

    [Fact]
    public void PatternCanSelectReducedChildKind()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                item(value:NUMBER) -> ^(ArrDeclNode[value]);
                wrapper(value:item[ArrDeclNode]) -> value;
                wrapper(value:item) -> ^(VarDeclNode value);
            }
            """);
        var builder = new AstTreeBuilder(mapping, ["wrapper", "item"],
            ["", "NUMBER"], Tokens("1", (1, 0, 0)));

        builder.OnEvent(ParseEvent.EnterRule(0));
        builder.OnEvent(ParseEvent.EnterRule(1));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.ExitRule(1));
        builder.OnEvent(ParseEvent.ExitRule(0));

        Assert.Equal("ArrDeclNode",
            Assert.IsType<UnvParseTreeElement>(builder.Root).LocalName);
    }

    private static TokenStore Tokens(string source,
        params (int Type, int Start, int Stop)[] values)
    {
        var tokens = new TokenStore(source);
        foreach (var value in values)
            tokens.Add(value.Type, 0, value.Start, value.Stop, 1, value.Start);
        return tokens;
    }
}
