using AllStarAtnParser;
using Atn;
using AntlrJson;
using Antlr4.Runtime;
using ParseTreeEditing.UnvParseTreeDOM;
using trinterp;
using Xunit;

namespace AllStarParserTests;

public sealed class AstMappingTests
{
    [Fact]
    public void GeneratedAstParseTreeCompilesAndRoundTripsThroughInterp()
    {
        const string source = """
            ast Sample {
                default single-child -> child;
                default otherwise -> error;
                source(prefix:ITEM*, last:ITEM, tail:END?)
                    -> ^(SourceNode ...prefix last ...tail);
            }
            """;
        var lexer = new AstMappingLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new AstMappingParser(tokens);
        var tree = parser.mapping();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        var dom = new ConvertToDOM().BottomUpConvert(
            tree, null, parser, lexer, tokens);
        Assert.True(AstMappingCompiler.IsMapping(dom));
        var table = AstMappingCompiler.Compile(dom);
        Assert.Equal("Sample", table.Name);
        Assert.True(table.SingleChild);
        Assert.True(table.ErrorOtherwise);
        var rule = Assert.Single(table.Rules);
        Assert.Equal("source", rule.RuleName);
        Assert.Equal(0, rule.Pattern[0].Min);
        Assert.Equal(int.MaxValue, rule.Pattern[0].Max);
        Assert.Equal("node", rule.Result.Op);
        Assert.Equal("splice", rule.Result.Children[0].Op);

        var encoded = Convert.ToBase64String(
            System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(table));
        var interp = "token literal names:\nnull\n\n" +
            "token symbolic names:\nnull\n\n" +
            "rule names:\nsource\n\n" +
            $"atn:\n[1]\n\nast-mapping:\n{encoded}\n\n";
        var loaded = InterpFileReader.Read(interp).AstMapping;
        Assert.Equal(table.Name, loaded.Name);
        Assert.Equal(table.SingleChild, loaded.SingleChild);
        Assert.Equal(table.ErrorOtherwise, loaded.ErrorOtherwise);
        Assert.Equal(rule.RuleName, Assert.Single(loaded.Rules).RuleName);
        var mapping = MappingParser.FromTable(loaded);
        Assert.Equal("SourceNode", Assert.IsType<NodeTemplate>(
            Assert.Single(mapping.Rules).Result).Kind);
    }

    [Fact]
    public void MappingInputIsRecognizedByTreeShapeRegardlessOfFileName()
    {
        const string source = "ast Sample { default single-child -> child; }";
        var lexer = new AstMappingLexer(new AntlrInputStream(source));
        var tokens = new CommonTokenStream(lexer);
        var parser = new AstMappingParser(tokens);
        var tree = parser.mapping();
        Assert.Equal(0, parser.NumberOfSyntaxErrors);
        var dom = new ConvertToDOM().BottomUpConvert(
            tree, null, parser, lexer, tokens);
        var result = new ParsingResultSet
        {
            FileName = "stdin",
            Nodes = [dom],
            Lexer = lexer,
            Parser = parser
        };
        var directory = Directory.CreateTempSubdirectory("AstMappingInput-").FullName;
        try
        {
            var input = Path.Combine(directory, "input.tar");
            using (var stream = File.Create(input))
                ArtifactBundle.Write(stream, [new Artifact("stdin.pt",
                    ArtifactBundle.SerializeParsingResult(result))]);
            var error = Assert.Throws<InvalidOperationException>(() =>
                new trinterp.Command().Execute(new trinterp.Config
                {
                    File = input,
                    OutputDirectory = Path.Combine(directory, "out")
                }));
            Assert.Contains("An .ast mapping requires exactly one parser grammar",
                error.Message);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void EarleyRejectsEmbeddedMappingWithoutPoisoningAllStarRuntimeCache()
    {
        var sourceDirectory = Path.Combine(AppContext.BaseDirectory,
            "TestData", "interp");
        var directory = Directory.CreateTempSubdirectory("AstMappingCache-").FullName;
        try
        {
            var parserPath = Path.Combine(directory, "Abnf.interp");
            var lexerPath = Path.Combine(directory, "AbnfLexer.interp");
            var table = new AstMappingTable("Abnf", false, false,
                [new AstMappingRuleTable("rulelist",
                    [new AstPatternPartTable(null, "_", null, null,
                        0, int.MaxValue)],
                    new AstTemplateTable("node", "MappedRoot", null, []))]);
            var encoded = Convert.ToBase64String(
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(table));
            File.WriteAllText(parserPath,
                File.ReadAllText(Path.Combine(sourceDirectory, "Abnf.interp")) +
                $"\nast-mapping:\n{encoded}\n\n");
            File.Copy(Path.Combine(sourceDirectory, "AbnfLexer.interp"), lexerPath);

            var cache = new InterpRuntimeCache();
            const string input = "ALPHA = %x41-5A\n";
            Assert.Throws<NotSupportedException>(() =>
                EarleyAtnParser.InterpRunner.Run(parserPath, lexerPath,
                    input, "input.abnf", false, runtimeCache: cache));
            Assert.Equal(0, cache.Count);

            var (result, _) = AllStarAtnParser.InterpRunner.Run(
                parserPath, lexerPath, input, "input.abnf", false,
                runtimeCache: cache);
            Assert.Equal("MappedRoot",
                Assert.IsType<UnvParseTreeElement>(Assert.Single(result.Nodes)).LocalName);
            Assert.Equal(1, cache.Count);
        }
        finally { Directory.Delete(directory, true); }
    }

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

    [Fact]
    public void RepeatedAndOptionalCapturesBuildAFlatList()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                source(items:ITEM+, tail:END?) -> ^(SourceNode ...items ...tail);
            }
            """);
        var builder = new AstTreeBuilder(mapping, ["source"],
            ["", "ITEM", "END"],
            Tokens("ab!", (1, 0, 0), (1, 1, 1), (2, 2, 2)));

        builder.OnEvent(ParseEvent.EnterRule(0));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.Consume(1));
        builder.OnEvent(ParseEvent.Consume(2));
        builder.OnEvent(ParseEvent.ExitRule(0));

        var root = Assert.IsType<UnvParseTreeElement>(builder.Root);
        Assert.Equal("SourceNode", root.LocalName);
        Assert.Equal(3, root.ChildNodes.Length);
        Assert.Equal("ITEM", Assert.IsType<UnvParseTreeElement>(
            root.ChildNodes.item(0)).LocalName);
        Assert.Equal("END", Assert.IsType<UnvParseTreeElement>(
            root.ChildNodes.item(2)).LocalName);
    }

    [Fact]
    public void RepetitionBacktracksAndAbsentOptionalSplicesNothing()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                source(prefix:ITEM*, last:ITEM, tail:END?)
                    -> ^(SourceNode ...prefix last ...tail);
            }
            """);
        var builder = new AstTreeBuilder(mapping, ["source"], ["", "ITEM", "END"],
            Tokens("ab", (1, 0, 0), (1, 1, 1)));

        builder.OnEvent(ParseEvent.EnterRule(0));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.Consume(1));
        builder.OnEvent(ParseEvent.ExitRule(0));

        Assert.Equal(2, Assert.IsType<UnvParseTreeElement>(builder.Root).ChildNodes.Length);
    }

    [Fact]
    public void TextCaptureJoinsARepeatedSequence()
    {
        var mapping = MappingParser.Parse("""
            ast A {
                default otherwise -> error;
                name(parts:_+) -> ^(IdNode[parts]);
            }
            """);
        var builder = new AstTreeBuilder(mapping, ["name"],
            ["", "IDENTIFIER", "DOT"],
            Tokens("a.b", (1, 0, 0), (2, 1, 1), (1, 2, 2)));

        builder.OnEvent(ParseEvent.EnterRule(0));
        builder.OnEvent(ParseEvent.Consume(0));
        builder.OnEvent(ParseEvent.Consume(1));
        builder.OnEvent(ParseEvent.Consume(2));
        builder.OnEvent(ParseEvent.ExitRule(0));

        Assert.Equal("a.b", UnvParseTreeElement.Reconstruct(builder.Root));
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
