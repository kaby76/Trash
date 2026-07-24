using Xunit;
using XQuery.DataModel;
using XQuery.Parser;
using XQuery.Engine;
using XQuery.IO;

namespace XQuery.Tests;

public class EvaluatorTests
{
    private XdmSequence Eval(string expr, XdmItem? contextItem = null)
    {
        var parser = new XPathParser(expr);
        var ast = parser.Parse();
        var context = EvaluationContext.CreateDefault();
        var evaluator = new XPathEvaluator(context);

        if (contextItem != null)
            return evaluator.Evaluate(ast, contextItem);
        return evaluator.Evaluate(ast);
    }

    private XdmSequence EvalWithVars(string expr, Dictionary<string, XdmSequence> vars)
    {
        var parser = new XPathParser(expr);
        var ast = parser.Parse();
        var context = EvaluationContext.CreateDefault();

        foreach (var (name, value) in vars)
        {
            context.SetVariable(name, value);
        }

        var evaluator = new XPathEvaluator(context);
        return evaluator.Evaluate(ast);
    }

    #region Literal Evaluation

    [Fact]
    public void Eval_IntegerLiteral()
    {
        var result = Eval("42");
        Assert.Equal(42, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_StringLiteral()
    {
        var result = Eval("'hello'");
        Assert.Equal("hello", result.StringValue);
    }

    [Fact]
    public void Eval_DecimalLiteral()
    {
        var result = Eval("3.14");
        Assert.Equal(3.14m, (result.Single() as XdmAtomicValue)!.AsDecimal());
    }

    #endregion

    #region Arithmetic

    [Fact]
    public void Eval_Addition()
    {
        var result = Eval("2 + 3");
        Assert.Equal(5, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Subtraction()
    {
        var result = Eval("10 - 3");
        Assert.Equal(7, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Multiplication()
    {
        var result = Eval("4 * 5");
        Assert.Equal(20, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Division()
    {
        var result = Eval("15 div 3");
        Assert.Equal(5, (result.Single() as XdmAtomicValue)!.AsDouble(), 0.001);
    }

    [Fact]
    public void Eval_IntegerDivision()
    {
        var result = Eval("17 idiv 5");
        Assert.Equal(3, (result.Single() as XdmAtomicValue)!.AsDouble(), 0.001);
    }

    [Fact]
    public void Eval_Modulo()
    {
        var result = Eval("17 mod 5");
        Assert.Equal(2, (result.Single() as XdmAtomicValue)!.AsDouble(), 0.001);
    }

    [Fact]
    public void Eval_ComplexArithmetic()
    {
        var result = Eval("(2 + 3) * 4");
        Assert.Equal(20, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_UnaryMinus()
    {
        var result = Eval("-5");
        Assert.Equal(-5, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    #endregion

    #region Comparisons

    [Fact]
    public void Eval_ValueComparison_Eq()
    {
        Assert.True(Eval("5 eq 5").EffectiveBooleanValue);
        Assert.False(Eval("5 eq 6").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_ValueComparison_Ne()
    {
        Assert.True(Eval("5 ne 6").EffectiveBooleanValue);
        Assert.False(Eval("5 ne 5").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_ValueComparison_Lt()
    {
        Assert.True(Eval("3 lt 5").EffectiveBooleanValue);
        Assert.False(Eval("5 lt 3").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_ValueComparison_Le()
    {
        Assert.True(Eval("3 le 5").EffectiveBooleanValue);
        Assert.True(Eval("5 le 5").EffectiveBooleanValue);
        Assert.False(Eval("6 le 5").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_ValueComparison_Gt()
    {
        Assert.True(Eval("5 gt 3").EffectiveBooleanValue);
        Assert.False(Eval("3 gt 5").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_ValueComparison_Ge()
    {
        Assert.True(Eval("5 ge 3").EffectiveBooleanValue);
        Assert.True(Eval("5 ge 5").EffectiveBooleanValue);
        Assert.False(Eval("3 ge 5").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_GeneralComparison_Equal()
    {
        Assert.True(Eval("(1, 2, 3) = 2").EffectiveBooleanValue);
        Assert.False(Eval("(1, 2, 3) = 5").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_LogicalAnd()
    {
        Assert.True(Eval("true() and true()").EffectiveBooleanValue);
        Assert.False(Eval("true() and false()").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_LogicalOr()
    {
        Assert.True(Eval("true() or false()").EffectiveBooleanValue);
        Assert.False(Eval("false() or false()").EffectiveBooleanValue);
    }

    #endregion

    #region Sequences

    [Fact]
    public void Eval_EmptySequence()
    {
        var result = Eval("()");
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void Eval_Sequence()
    {
        var result = Eval("(1, 2, 3)");
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Eval_Range()
    {
        var result = Eval("1 to 5");
        Assert.Equal(5, result.Count);
        Assert.Equal(1, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(5, (result[4] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_StringConcat()
    {
        var result = Eval("'hello' || ' ' || 'world'");
        Assert.Equal("hello world", result.StringValue);
    }

    #endregion

    #region Variables

    [Fact]
    public void Eval_VariableRef()
    {
        var result = EvalWithVars("$x", new Dictionary<string, XdmSequence>
        {
            ["x"] = new XdmSequence(new XdmAtomicValue(42L))
        });
        Assert.Equal(42, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_VariableInExpression()
    {
        var result = EvalWithVars("$x + $y", new Dictionary<string, XdmSequence>
        {
            ["x"] = new XdmSequence(new XdmAtomicValue(10L)),
            ["y"] = new XdmSequence(new XdmAtomicValue(20L))
        });
        Assert.Equal(30, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    #endregion

    #region Conditionals

    [Fact]
    public void Eval_If_ThenBranch()
    {
        var result = Eval("if (true()) then 'yes' else 'no'");
        Assert.Equal("yes", result.StringValue);
    }

    [Fact]
    public void Eval_If_ElseBranch()
    {
        var result = Eval("if (false()) then 'yes' else 'no'");
        Assert.Equal("no", result.StringValue);
    }

    [Fact]
    public void Eval_If_WithComparison()
    {
        var result = EvalWithVars("if ($x > 0) then 'positive' else 'non-positive'",
            new Dictionary<string, XdmSequence>
            {
                ["x"] = new XdmSequence(new XdmAtomicValue(5L))
            });
        Assert.Equal("positive", result.StringValue);
    }

    #endregion

    #region FLWOR

    [Fact]
    public void Eval_For()
    {
        var result = Eval("for $x in (1, 2, 3) return $x * 2");
        Assert.Equal(3, result.Count);
        Assert.Equal(2, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(4, (result[1] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(6, (result[2] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Let()
    {
        var result = Eval("let $x := 5 return $x * 2");
        Assert.Equal(10, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_ForLet()
    {
        var result = Eval("for $x in (1, 2) let $y := $x * 10 return $y");
        Assert.Equal(2, result.Count);
        Assert.Equal(10, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(20, (result[1] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_ForWhere()
    {
        var result = Eval("for $x in (1, 2, 3, 4, 5) where $x > 3 return $x");
        Assert.Equal(2, result.Count);
        Assert.Equal(4, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(5, (result[1] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_ForPositional()
    {
        var result = Eval("for $x at $i in ('a', 'b', 'c') return $i");
        Assert.Equal(3, result.Count);
        Assert.Equal(1, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(2, (result[1] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(3, (result[2] as XdmAtomicValue)!.AsInteger());
    }

    #endregion

    #region Quantified

    [Fact]
    public void Eval_Some_True()
    {
        var result = Eval("some $x in (1, 2, 3) satisfies $x > 2");
        Assert.True(result.EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Some_False()
    {
        var result = Eval("some $x in (1, 2, 3) satisfies $x > 10");
        Assert.False(result.EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Every_True()
    {
        var result = Eval("every $x in (1, 2, 3) satisfies $x > 0");
        Assert.True(result.EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Every_False()
    {
        var result = Eval("every $x in (1, 2, 3) satisfies $x > 2");
        Assert.False(result.EffectiveBooleanValue);
    }

    #endregion

    #region Functions

    [Fact]
    public void Eval_Concat()
    {
        var result = Eval("concat('hello', ' ', 'world')");
        Assert.Equal("hello world", result.StringValue);
    }

    [Fact]
    public void Eval_StringLength()
    {
        var result = Eval("string-length('hello')");
        Assert.Equal(5, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Substring()
    {
        var result = Eval("substring('hello', 2, 3)");
        Assert.Equal("ell", result.StringValue);
    }

    [Fact]
    public void Eval_UpperCase()
    {
        var result = Eval("upper-case('hello')");
        Assert.Equal("HELLO", result.StringValue);
    }

    [Fact]
    public void Eval_LowerCase()
    {
        var result = Eval("lower-case('HELLO')");
        Assert.Equal("hello", result.StringValue);
    }

    [Fact]
    public void Eval_Contains()
    {
        Assert.True(Eval("contains('hello world', 'world')").EffectiveBooleanValue);
        Assert.False(Eval("contains('hello world', 'xyz')").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_StartsWith()
    {
        Assert.True(Eval("starts-with('hello', 'hel')").EffectiveBooleanValue);
        Assert.False(Eval("starts-with('hello', 'ell')").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_EndsWith()
    {
        Assert.True(Eval("ends-with('hello', 'llo')").EffectiveBooleanValue);
        Assert.False(Eval("ends-with('hello', 'hel')").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Abs()
    {
        Assert.Equal(5, (Eval("abs(-5)").Single() as XdmAtomicValue)!.AsInteger());
        Assert.Equal(5, (Eval("abs(5)").Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Floor()
    {
        Assert.Equal(3.0, (Eval("floor(3.7)").Single() as XdmAtomicValue)!.AsDouble());
    }

    [Fact]
    public void Eval_Ceiling()
    {
        Assert.Equal(4.0, (Eval("ceiling(3.2)").Single() as XdmAtomicValue)!.AsDouble());
    }

    [Fact]
    public void Eval_Round()
    {
        Assert.Equal(4.0, (Eval("round(3.5)").Single() as XdmAtomicValue)!.AsDouble());
        Assert.Equal(3.0, (Eval("round(3.4)").Single() as XdmAtomicValue)!.AsDouble());
    }

    [Fact]
    public void Eval_Sum()
    {
        var result = Eval("sum((1, 2, 3, 4, 5))");
        Assert.Equal(15.0, (result.Single() as XdmAtomicValue)!.AsDouble());
    }

    [Fact]
    public void Eval_Avg()
    {
        var result = Eval("avg((1, 2, 3, 4, 5))");
        Assert.Equal(3.0, (result.Single() as XdmAtomicValue)!.AsDouble());
    }

    [Fact]
    public void Eval_Min()
    {
        var result = Eval("min((5, 2, 8, 1, 9))");
        Assert.Equal(1, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Max()
    {
        var result = Eval("max((5, 2, 8, 1, 9))");
        Assert.Equal(9, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Count()
    {
        var result = Eval("count((1, 2, 3, 4, 5))");
        Assert.Equal(5, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Empty()
    {
        Assert.True(Eval("empty(())").EffectiveBooleanValue);
        Assert.False(Eval("empty((1))").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Exists()
    {
        Assert.True(Eval("exists((1))").EffectiveBooleanValue);
        Assert.False(Eval("exists(())").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Head()
    {
        var result = Eval("head((1, 2, 3))");
        Assert.Equal(1, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Tail()
    {
        var result = Eval("tail((1, 2, 3))");
        Assert.Equal(2, result.Count);
        Assert.Equal(2, (result[0] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Reverse()
    {
        var result = Eval("reverse((1, 2, 3))");
        Assert.Equal(3, (result[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(1, (result[2] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Not()
    {
        Assert.True(Eval("not(false())").EffectiveBooleanValue);
        Assert.False(Eval("not(true())").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Boolean()
    {
        Assert.True(Eval("boolean(1)").EffectiveBooleanValue);
        Assert.False(Eval("boolean(0)").EffectiveBooleanValue);
        Assert.True(Eval("boolean('text')").EffectiveBooleanValue);
        Assert.False(Eval("boolean('')").EffectiveBooleanValue);
    }

    #endregion

    #region Maps and Arrays

    [Fact]
    public void Eval_MapConstructor()
    {
        var result = Eval("map { 'a': 1, 'b': 2 }");
        Assert.True(result.Single() is XdmMap);
        var map = (XdmMap)result.Single();
        Assert.Equal(2, map.Count);
    }

    [Fact]
    public void Eval_MapLookup()
    {
        var result = Eval("map { 'name': 'John', 'age': 30 }?name");
        Assert.Equal("John", result.StringValue);
    }

    [Fact]
    public void Eval_ArrayConstructor()
    {
        var result = Eval("[1, 2, 3]");
        Assert.True(result.Single() is XdmArray);
        var array = (XdmArray)result.Single();
        Assert.Equal(3, array.Count);
    }

    [Fact]
    public void Eval_ArrayLookup()
    {
        var result = Eval("[10, 20, 30]?2");
        Assert.Equal(20, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_MapSize()
    {
        var result = Eval("map:size(map { 'a': 1, 'b': 2, 'c': 3 })");
        Assert.Equal(3, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_ArraySize()
    {
        var result = Eval("array:size([1, 2, 3, 4])");
        Assert.Equal(4, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    #endregion

    #region Path Expressions with XML

    [Fact]
    public void Eval_XPath_RootElement()
    {
        var doc = XmlDocumentReader.Parse("<root><child/></root>");
        var result = Eval("/root", doc);

        Assert.Single(result);
        Assert.IsType<XdmElement>(result.First);
        Assert.Equal("root", ((XdmElement)result.First!).LocalName);
    }

    [Fact]
    public void Eval_XPath_ChildElements()
    {
        var doc = XmlDocumentReader.Parse("<root><a/><b/><c/></root>");
        var result = Eval("/root/*", doc);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Eval_XPath_Descendants()
    {
        var doc = XmlDocumentReader.Parse("<root><a><b/></a><c><d/></c></root>");
        var result = Eval("//b", doc);

        Assert.Single(result);
    }

    [Fact]
    public void Eval_XPath_Attribute()
    {
        var doc = XmlDocumentReader.Parse("<root id='123'/>");
        var result = Eval("/root/@id", doc);

        Assert.Single(result);
        Assert.Equal("123", result.StringValue);
    }

    [Fact]
    public void Eval_XPath_Predicate()
    {
        var doc = XmlDocumentReader.Parse("<root><item n='1'/><item n='2'/><item n='3'/></root>");
        var result = Eval("/root/item[@n='2']", doc);

        Assert.Single(result);
    }

    [Fact]
    public void Eval_XPath_TextContent()
    {
        var doc = XmlDocumentReader.Parse("<root>Hello World</root>");
        var result = Eval("/root/text()", doc);

        Assert.Equal("Hello World", result.StringValue);
    }

    [Fact]
    public void Eval_XPath_Position()
    {
        var doc = XmlDocumentReader.Parse("<root><a/><b/><c/></root>");
        var result = Eval("/root/*[2]", doc);

        Assert.Single(result);
        Assert.Equal("b", ((XdmElement)result.First!).LocalName);
    }

    #endregion

    #region Type Expressions

    [Fact]
    public void Eval_InstanceOf_True()
    {
        Assert.True(Eval("42 instance of xs:integer").EffectiveBooleanValue);
        Assert.True(Eval("'hello' instance of xs:string").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_InstanceOf_False()
    {
        Assert.False(Eval("'hello' instance of xs:integer").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Cast()
    {
        var result = Eval("'42' cast as xs:integer");
        Assert.Equal(42, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Castable_True()
    {
        Assert.True(Eval("'42' castable as xs:integer").EffectiveBooleanValue);
    }

    [Fact]
    public void Eval_Castable_False()
    {
        Assert.False(Eval("'hello' castable as xs:integer").EffectiveBooleanValue);
    }

    #endregion

    #region Otherwise Expression (XPath 4.0)

    [Fact]
    public void Eval_Otherwise_LeftNotEmpty()
    {
        var result = Eval("42 otherwise 0");
        Assert.Equal(42, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Eval_Otherwise_LeftEmpty()
    {
        var result = Eval("() otherwise 0");
        Assert.Equal(0, (result.Single() as XdmAtomicValue)!.AsInteger());
    }

    #endregion

    #region Arrow Expression

    [Fact]
    public void Eval_Arrow()
    {
        var result = Eval("'hello' => upper-case()");
        Assert.Equal("HELLO", result.StringValue);
    }

    [Fact]
    public void Eval_Arrow_Chain()
    {
        var result = Eval("'  hello  ' => normalize-space() => upper-case()");
        Assert.Equal("HELLO", result.StringValue);
    }

    #endregion
}
