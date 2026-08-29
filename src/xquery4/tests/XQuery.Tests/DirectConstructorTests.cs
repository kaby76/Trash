using Xunit;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.IO;
using XQuery.Parser;

namespace XQuery.Tests;

public class DirectConstructorTests
{
    private static XdmSequence Evaluate(string query)
    {
        var ast = new XQueryParser(query).Parse();
        return new XQueryEvaluator(EvaluationContext.CreateDefault()).Evaluate(ast);
    }

    private static string Serialize(string query)
        => XmlSerializer.Serialize(Evaluate(query), new SerializationOptions { OmitXmlDeclaration = true, Indent = false });

    [Fact]
    public void EmptyElement_ParsesAndEvaluates()
    {
        Assert.Equal("<root/>", Serialize("<root/>"));
    }

    [Fact]
    public void NestedContent_AttributesAndEnclosedExpressions_Evaluate()
    {
        const string query = "<root answer=\"{ 1 + 1 }\" label='A &amp; B'><child>{ 'value' }</child></root>";
        Assert.Equal("<root answer=\"2\" label=\"A &amp; B\"><child>value</child></root>", Serialize(query));
    }

    [Fact]
    public void FlworVariables_AreVisibleInsideNestedConstructors()
    {
        const string query = "<root>{for $name in ('one', 'two') return <name>{string($name)}</name>}</root>";
        Assert.Equal("<root><name>one</name><name>two</name></root>", Serialize(query));
    }

    [Fact]
    public void TextEntitiesCharacterReferencesCDataAndEscapedBraces_Evaluate()
    {
        const string query = "<root>&lt;&#65;<![CDATA[<raw>]]>{{literal}}</root>";
        Assert.Equal("<root>&lt;A&lt;raw&gt;{literal}</root>", Serialize(query));
    }

    [Fact]
    public void DirectComment_ParsesAndEvaluates()
    {
        Assert.Equal("<!--hello-->", Serialize("<!--hello-->"));
    }

    [Fact]
    public void DirectProcessingInstruction_ParsesAndEvaluates()
    {
        Assert.Equal("<?target data?>", Serialize("<?target data?>"));
    }

    [Fact]
    public void MismatchedElementNames_AreRejected()
    {
        var ex = Assert.Throws<XPathParseException>(() => new XQueryParser("<one></two>").Parse());
        Assert.Contains("does not match", ex.Message);
    }
}
