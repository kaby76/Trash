using Xunit;
using XQuery.Parser;
using XQuery.Parser.Ast;

namespace XQuery.Tests;

public class ParserTests
{
    #region Lexer Tests

    [Fact]
    public void Lexer_IntegerLiteral()
    {
        var lexer = new Lexer("42");
        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
    }

    [Fact]
    public void Lexer_DecimalLiteral()
    {
        var lexer = new Lexer("3.14");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.DecimalLiteral, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Value);
    }

    [Fact]
    public void Lexer_DoubleLiteral()
    {
        var lexer = new Lexer("1.5e10");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.DoubleLiteral, tokens[0].Type);
    }

    [Fact]
    public void Lexer_StringLiteral_DoubleQuotes()
    {
        var lexer = new Lexer("\"hello world\"");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal("hello world", tokens[0].Value);
    }

    [Fact]
    public void Lexer_StringLiteral_SingleQuotes()
    {
        var lexer = new Lexer("'hello world'");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal("hello world", tokens[0].Value);
    }

    [Fact]
    public void Lexer_StringLiteral_EscapedQuotes()
    {
        var lexer = new Lexer("\"he said \"\"hello\"\"\"");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal("he said \"hello\"", tokens[0].Value);
    }

    [Fact]
    public void Lexer_Operators()
    {
        var lexer = new Lexer("+ - * div mod");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Asterisk, tokens[2].Type);
        Assert.Equal(TokenType.Div, tokens[3].Type);
        Assert.Equal(TokenType.Mod, tokens[4].Type);
    }

    [Fact]
    public void Lexer_ComparisonOperators()
    {
        var lexer = new Lexer("= != < <= > >= eq ne lt le gt ge");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Equal, tokens[0].Type);
        Assert.Equal(TokenType.NotEqual, tokens[1].Type);
        Assert.Equal(TokenType.LessThan, tokens[2].Type);
        Assert.Equal(TokenType.LessOrEqual, tokens[3].Type);
        Assert.Equal(TokenType.GreaterThan, tokens[4].Type);
        Assert.Equal(TokenType.GreaterOrEqual, tokens[5].Type);
        Assert.Equal(TokenType.Eq, tokens[6].Type);
    }

    [Fact]
    public void Lexer_PathOperators()
    {
        var lexer = new Lexer("/ // . .. @ ::");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Slash, tokens[0].Type);
        Assert.Equal(TokenType.SlashSlash, tokens[1].Type);
        Assert.Equal(TokenType.Dot, tokens[2].Type);
        Assert.Equal(TokenType.DotDot, tokens[3].Type);
        Assert.Equal(TokenType.At, tokens[4].Type);
        Assert.Equal(TokenType.ColonColon, tokens[5].Type);
    }

    [Fact]
    public void Lexer_Keywords()
    {
        var lexer = new Lexer("for let where return if then else");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.For, tokens[0].Type);
        Assert.Equal(TokenType.Let, tokens[1].Type);
        Assert.Equal(TokenType.Where, tokens[2].Type);
        Assert.Equal(TokenType.Return, tokens[3].Type);
        Assert.Equal(TokenType.If, tokens[4].Type);
        Assert.Equal(TokenType.Then, tokens[5].Type);
        Assert.Equal(TokenType.Else, tokens[6].Type);
    }

    [Fact]
    public void Lexer_VariableRef()
    {
        var lexer = new Lexer("$x $my-var");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Dollar, tokens[0].Type);
        Assert.Equal(TokenType.NCName, tokens[1].Type);
        Assert.Equal("x", tokens[1].Value);
    }

    [Fact]
    public void Lexer_QName()
    {
        var lexer = new Lexer("fn:concat xs:string");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.QName, tokens[0].Type);
        Assert.Equal("fn:concat", tokens[0].Value);
    }

    [Fact]
    public void Lexer_Comment()
    {
        var lexer = new Lexer("1 (: this is a comment :) + 2");
        var tokens = lexer.Tokenize();

        Assert.Equal(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.IntegerLiteral, tokens[2].Type);
    }

    [Fact]
    public void Lexer_NestedComment()
    {
        var lexer = new Lexer("1 (: outer (: inner :) outer :) + 2");
        var tokens = lexer.Tokenize();

        Assert.Equal(3, tokens.Count - 1); // Exclude EOF
    }

    #endregion

    #region Parser Tests - Literals

    [Fact]
    public void Parse_IntegerLiteral()
    {
        var parser = new XPathParser("42");
        var ast = parser.Parse();

        Assert.IsType<IntegerLiteralExpr>(ast);
        Assert.Equal(42, ((IntegerLiteralExpr)ast).Value);
    }

    [Fact]
    public void Parse_StringLiteral()
    {
        var parser = new XPathParser("'hello'");
        var ast = parser.Parse();

        Assert.IsType<StringLiteralExpr>(ast);
        Assert.Equal("hello", ((StringLiteralExpr)ast).Value);
    }

    [Fact]
    public void Parse_DoubleLiteral()
    {
        var parser = new XPathParser("3.14e2");
        var ast = parser.Parse();

        Assert.IsType<DoubleLiteralExpr>(ast);
    }

    #endregion

    #region Parser Tests - Arithmetic

    [Fact]
    public void Parse_Addition()
    {
        var parser = new XPathParser("1 + 2");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Add, binary.Operator);
    }

    [Fact]
    public void Parse_Subtraction()
    {
        var parser = new XPathParser("5 - 3");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Subtract, binary.Operator);
    }

    [Fact]
    public void Parse_Multiplication()
    {
        var parser = new XPathParser("2 * 3");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Multiply, binary.Operator);
    }

    [Fact]
    public void Parse_Division()
    {
        var parser = new XPathParser("10 div 2");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Divide, binary.Operator);
    }

    [Fact]
    public void Parse_Precedence()
    {
        var parser = new XPathParser("1 + 2 * 3");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Add, binary.Operator);
        Assert.IsType<BinaryExpr>(binary.Right);
    }

    [Fact]
    public void Parse_UnaryMinus()
    {
        var parser = new XPathParser("-5");
        var ast = parser.Parse();

        Assert.IsType<UnaryExpr>(ast);
        var unary = (UnaryExpr)ast;
        Assert.Equal(UnaryOperator.Minus, unary.Operator);
    }

    #endregion

    #region Parser Tests - Comparisons

    [Fact]
    public void Parse_ValueComparison()
    {
        var parser = new XPathParser("$x eq 5");
        var ast = parser.Parse();

        Assert.IsType<ComparisonExpr>(ast);
        var comp = (ComparisonExpr)ast;
        Assert.Equal(ComparisonOperator.Eq, comp.Operator);
    }

    [Fact]
    public void Parse_GeneralComparison()
    {
        var parser = new XPathParser("$x = 5");
        var ast = parser.Parse();

        Assert.IsType<ComparisonExpr>(ast);
        var comp = (ComparisonExpr)ast;
        Assert.Equal(ComparisonOperator.Equal, comp.Operator);
    }

    [Fact]
    public void Parse_And()
    {
        var parser = new XPathParser("$x and $y");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.And, binary.Operator);
    }

    [Fact]
    public void Parse_Or()
    {
        var parser = new XPathParser("$x or $y");
        var ast = parser.Parse();

        Assert.IsType<BinaryExpr>(ast);
        var binary = (BinaryExpr)ast;
        Assert.Equal(BinaryOperator.Or, binary.Operator);
    }

    #endregion

    #region Parser Tests - Sequences

    [Fact]
    public void Parse_EmptySequence()
    {
        var parser = new XPathParser("()");
        var ast = parser.Parse();

        Assert.IsType<SequenceExpr>(ast);
        Assert.Empty(((SequenceExpr)ast).Items);
    }

    [Fact]
    public void Parse_Sequence()
    {
        var parser = new XPathParser("(1, 2, 3)");
        var ast = parser.Parse();

        Assert.IsType<ParenthesizedExpr>(ast);
        var paren = (ParenthesizedExpr)ast;
        Assert.IsType<SequenceExpr>(paren.Inner);
    }

    [Fact]
    public void Parse_Range()
    {
        var parser = new XPathParser("1 to 10");
        var ast = parser.Parse();

        Assert.IsType<RangeExpr>(ast);
    }

    #endregion

    #region Parser Tests - Path Expressions

    [Fact]
    public void Parse_RootPath()
    {
        var parser = new XPathParser("/");
        var ast = parser.Parse();

        Assert.IsType<PathExpr>(ast);
        var path = (PathExpr)ast;
        Assert.True(path.IsAbsolute);
        Assert.True(path.IsRootOnly);
    }

    [Fact]
    public void Parse_SimpleChildPath()
    {
        var parser = new XPathParser("/root/child");
        var ast = parser.Parse();

        Assert.IsType<PathExpr>(ast);
        var path = (PathExpr)ast;
        Assert.True(path.IsAbsolute);
        Assert.Equal(2, path.Steps.Count);
    }

    [Fact]
    public void Parse_DescendantPath()
    {
        var parser = new XPathParser("//item");
        var ast = parser.Parse();

        Assert.IsType<PathExpr>(ast);
        var path = (PathExpr)ast;
        Assert.True(path.IsAbsolute);
    }

    [Fact]
    public void Parse_AttributeAxis()
    {
        var parser = new XPathParser("@id");
        var ast = parser.Parse();

        Assert.IsType<AxisStepExpr>(ast);
        var step = (AxisStepExpr)ast;
        Assert.Equal(Axis.Attribute, step.Axis);
    }

    [Fact]
    public void Parse_Predicate()
    {
        // "item[1]" is an axis step with name test "item" and predicate [1]
        var parser = new XPathParser("item[1]");
        var ast = parser.Parse();

        Assert.IsType<AxisStepExpr>(ast);
        var step = (AxisStepExpr)ast;
        Assert.Single(step.Predicates);
    }

    [Fact]
    public void Parse_FilterExpr()
    {
        // FilterExpr applies predicates to primary expressions like variable references
        var parser = new XPathParser("$x[1]");
        var ast = parser.Parse();

        Assert.IsType<FilterExpr>(ast);
        var filter = (FilterExpr)ast;
        Assert.Single(filter.Predicates);
    }

    [Fact]
    public void Parse_ContextItem()
    {
        var parser = new XPathParser(".");
        var ast = parser.Parse();

        Assert.IsType<ContextItemExpr>(ast);
    }

    [Fact]
    public void Parse_ParentAxis()
    {
        var parser = new XPathParser("..");
        var ast = parser.Parse();

        Assert.IsType<AxisStepExpr>(ast);
        var step = (AxisStepExpr)ast;
        Assert.Equal(Axis.Parent, step.Axis);
    }

    #endregion

    #region Parser Tests - Functions

    [Fact]
    public void Parse_FunctionCall()
    {
        var parser = new XPathParser("concat('a', 'b')");
        var ast = parser.Parse();

        Assert.IsType<FunctionCallExpr>(ast);
        var call = (FunctionCallExpr)ast;
        Assert.Equal("concat", call.Name);
        Assert.Equal(2, call.Arguments.Count);
    }

    [Fact]
    public void Parse_FunctionCallWithQName()
    {
        var parser = new XPathParser("fn:concat('a', 'b')");
        var ast = parser.Parse();

        Assert.IsType<FunctionCallExpr>(ast);
        var call = (FunctionCallExpr)ast;
        Assert.Equal("concat", call.Name);
        Assert.Equal("fn", call.Prefix);
    }

    [Fact]
    public void Parse_VariableRef()
    {
        var parser = new XPathParser("$myVar");
        var ast = parser.Parse();

        Assert.IsType<VariableRefExpr>(ast);
        Assert.Equal("myVar", ((VariableRefExpr)ast).Name);
    }

    #endregion

    #region Parser Tests - Conditionals

    [Fact]
    public void Parse_IfExpression()
    {
        var parser = new XPathParser("if ($x > 0) then 'positive' else 'non-positive'");
        var ast = parser.Parse();

        Assert.IsType<IfExpr>(ast);
        var ifExpr = (IfExpr)ast;
        Assert.IsType<ComparisonExpr>(ifExpr.Condition);
    }

    #endregion

    #region Parser Tests - FLWOR

    [Fact]
    public void Parse_ForExpression()
    {
        var parser = new XPathParser("for $x in (1, 2, 3) return $x * 2");
        var ast = parser.Parse();

        Assert.IsType<FlworExpr>(ast);
        var flwor = (FlworExpr)ast;
        Assert.Single(flwor.Clauses);
        Assert.IsType<ForClause>(flwor.Clauses[0]);
    }

    [Fact]
    public void Parse_LetExpression()
    {
        var parser = new XPathParser("let $x := 5 return $x + 1");
        var ast = parser.Parse();

        Assert.IsType<FlworExpr>(ast);
        var flwor = (FlworExpr)ast;
        Assert.IsType<LetClause>(flwor.Clauses[0]);
    }

    [Fact]
    public void Parse_ForLetWhere()
    {
        var parser = new XPathParser("for $x in (1, 2, 3) let $y := $x * 2 where $y > 2 return $y");
        var ast = parser.Parse();

        Assert.IsType<FlworExpr>(ast);
        var flwor = (FlworExpr)ast;
        Assert.Equal(3, flwor.Clauses.Count);
    }

    #endregion

    #region Parser Tests - Quantified

    [Fact]
    public void Parse_Some()
    {
        var parser = new XPathParser("some $x in (1, 2, 3) satisfies $x > 2");
        var ast = parser.Parse();

        Assert.IsType<QuantifiedExpr>(ast);
        var quant = (QuantifiedExpr)ast;
        Assert.True(quant.IsSome);
    }

    [Fact]
    public void Parse_Every()
    {
        var parser = new XPathParser("every $x in (1, 2, 3) satisfies $x > 0");
        var ast = parser.Parse();

        Assert.IsType<QuantifiedExpr>(ast);
        var quant = (QuantifiedExpr)ast;
        Assert.False(quant.IsSome);
    }

    #endregion

    #region Parser Tests - Maps and Arrays

    [Fact]
    public void Parse_MapConstructor()
    {
        var parser = new XPathParser("map { 'key': 'value' }");
        var ast = parser.Parse();

        Assert.IsType<MapConstructorExpr>(ast);
        var map = (MapConstructorExpr)ast;
        Assert.Single(map.Entries);
    }

    [Fact]
    public void Parse_SquareArrayConstructor()
    {
        var parser = new XPathParser("[1, 2, 3]");
        var ast = parser.Parse();

        Assert.IsType<ArrayConstructorExpr>(ast);
        var array = (ArrayConstructorExpr)ast;
        Assert.False(array.IsCurly);
        Assert.Equal(3, array.Members.Count);
    }

    [Fact]
    public void Parse_CurlyArrayConstructor()
    {
        var parser = new XPathParser("array { 1, 2, 3 }");
        var ast = parser.Parse();

        Assert.IsType<ArrayConstructorExpr>(ast);
        var array = (ArrayConstructorExpr)ast;
        Assert.True(array.IsCurly);
    }

    [Fact]
    public void Parse_Lookup()
    {
        var parser = new XPathParser("$map?key");
        var ast = parser.Parse();

        Assert.IsType<PostfixLookupExpr>(ast);
    }

    #endregion

    #region Parser Tests - Arrow Expression

    [Fact]
    public void Parse_ArrowExpression()
    {
        var parser = new XPathParser("'hello' => upper-case()");
        var ast = parser.Parse();

        Assert.IsType<ArrowExpr>(ast);
    }

    #endregion

    #region Parser Tests - String Concatenation

    [Fact]
    public void Parse_StringConcat()
    {
        var parser = new XPathParser("'hello' || ' ' || 'world'");
        var ast = parser.Parse();

        Assert.IsType<ConcatExpr>(ast);
    }

    #endregion
}
