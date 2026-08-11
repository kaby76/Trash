using Xunit;
using XQuery.DataModel;
using XQuery.IO;

namespace XQuery.Tests;

public class IOTests
{
    #region XML Reader Tests

    [Fact]
    public void XmlReader_SimpleDocument()
    {
        var doc = XmlDocumentReader.Parse("<root/>");

        Assert.NotNull(doc.DocumentElement);
        Assert.Equal("root", doc.DocumentElement.LocalName);
    }

    [Fact]
    public void XmlReader_WithAttributes()
    {
        var doc = XmlDocumentReader.Parse("<root id='123' name='test'/>");

        Assert.Equal("123", doc.DocumentElement!.GetAttribute("id"));
        Assert.Equal("test", doc.DocumentElement.GetAttribute("name"));
    }

    [Fact]
    public void XmlReader_WithChildren()
    {
        var doc = XmlDocumentReader.Parse("<root><a/><b/><c/></root>");

        Assert.Equal(3, doc.DocumentElement!.Children.Count);
    }

    [Fact]
    public void XmlReader_WithText()
    {
        var doc = XmlDocumentReader.Parse("<root>Hello World</root>");

        Assert.Equal("Hello World", doc.DocumentElement!.StringValue);
    }

    [Fact]
    public void XmlReader_WithNamespace()
    {
        var doc = XmlDocumentReader.Parse("<root xmlns='http://example.org'/>");

        Assert.Equal("http://example.org", doc.DocumentElement!.NamespaceUri);
    }

    [Fact]
    public void XmlReader_WithPrefixedNamespace()
    {
        var doc = XmlDocumentReader.Parse("<ex:root xmlns:ex='http://example.org'/>");

        Assert.Equal("http://example.org", doc.DocumentElement!.NamespaceUri);
        Assert.Equal("ex", doc.DocumentElement.Prefix);
    }

    [Fact]
    public void XmlReader_WithComment()
    {
        var doc = XmlDocumentReader.Parse("<root><!-- comment --></root>");

        Assert.Single(doc.DocumentElement!.Children.OfType<XdmComment>());
    }

    [Fact]
    public void XmlReader_WithProcessingInstruction()
    {
        var doc = XmlDocumentReader.Parse("<?xml-stylesheet type='text/css' href='style.css'?><root/>");

        Assert.Single(doc.Children.OfType<XdmProcessingInstruction>());
    }

    [Fact]
    public void XmlReader_NestedElements()
    {
        var doc = XmlDocumentReader.Parse(@"
            <root>
                <level1>
                    <level2>
                        <level3>deep</level3>
                    </level2>
                </level1>
            </root>");

        var descendants = doc.DocumentElement!.Descendants().ToList();
        Assert.True(descendants.Count >= 3);
    }

    #endregion

    #region JSON Reader Tests

    [Fact]
    public void JsonReader_Object()
    {
        var item = JsonDocumentReader.Parse("{\"name\": \"John\", \"age\": 30}");

        Assert.IsType<XdmMap>(item);
        var map = (XdmMap)item;
        Assert.Equal(2, map.Count);
        Assert.Equal("John", map[new XdmAtomicValue("name")].StringValue);
    }

    [Fact]
    public void JsonReader_Array()
    {
        var item = JsonDocumentReader.Parse("[1, 2, 3]");

        Assert.IsType<XdmArray>(item);
        var array = (XdmArray)item;
        Assert.Equal(3, array.Count);
    }

    [Fact]
    public void JsonReader_String()
    {
        var item = JsonDocumentReader.Parse("\"hello\"");

        Assert.IsType<XdmAtomicValue>(item);
        Assert.Equal("hello", item.StringValue);
    }

    [Fact]
    public void JsonReader_Number()
    {
        var item = JsonDocumentReader.Parse("42");

        Assert.IsType<XdmAtomicValue>(item);
        Assert.Equal(42, ((XdmAtomicValue)item).AsInteger());
    }

    [Fact]
    public void JsonReader_Boolean()
    {
        var trueItem = JsonDocumentReader.Parse("true");
        var falseItem = JsonDocumentReader.Parse("false");

        Assert.True(((XdmAtomicValue)trueItem).AsBoolean());
        Assert.False(((XdmAtomicValue)falseItem).AsBoolean());
    }

    [Fact]
    public void JsonReader_Nested()
    {
        var item = JsonDocumentReader.Parse("{\"person\": {\"name\": \"John\", \"contacts\": [{\"type\": \"email\", \"value\": \"john@example.com\"}]}}");

        Assert.IsType<XdmMap>(item);
        var map = (XdmMap)item;
        var person = map[new XdmAtomicValue("person")].Single() as XdmMap;
        Assert.NotNull(person);
    }

    [Fact]
    public void JsonReader_AsXml()
    {
        var doc = JsonDocumentReader.ParseAsXml("{\"name\": \"John\", \"age\": 30}");

        Assert.NotNull(doc.DocumentElement);
        var nameElem = doc.DocumentElement.GetElementsByName("name").FirstOrDefault();
        Assert.NotNull(nameElem);
        Assert.Equal("John", nameElem.StringValue);
    }

    #endregion

    #region YAML Reader Tests

    [Fact]
    public void YamlReader_SimpleMapping()
    {
        var item = YamlDocumentReader.Parse(@"
name: John
age: 30");

        Assert.IsType<XdmMap>(item);
        var map = (XdmMap)item;
        Assert.Equal("John", map[new XdmAtomicValue("name")].StringValue);
    }

    [Fact]
    public void YamlReader_Sequence()
    {
        var item = YamlDocumentReader.Parse(@"
- apple
- banana
- cherry");

        Assert.IsType<XdmArray>(item);
        var array = (XdmArray)item;
        Assert.Equal(3, array.Count);
    }

    [Fact]
    public void YamlReader_Nested()
    {
        var item = YamlDocumentReader.Parse(@"
person:
  name: John
  contacts:
    - type: email
      value: john@example.com");

        Assert.IsType<XdmMap>(item);
    }

    [Fact]
    public void YamlReader_Boolean()
    {
        var item = YamlDocumentReader.Parse(@"
active: true
disabled: false");

        Assert.IsType<XdmMap>(item);
        var map = (XdmMap)item;
        Assert.True(((XdmAtomicValue)map[new XdmAtomicValue("active")].Single()).AsBoolean());
        Assert.False(((XdmAtomicValue)map[new XdmAtomicValue("disabled")].Single()).AsBoolean());
    }

    [Fact]
    public void YamlReader_Numbers()
    {
        var item = YamlDocumentReader.Parse(@"
integer: 42
float: 3.14");

        Assert.IsType<XdmMap>(item);
        var map = (XdmMap)item;
        Assert.Equal(42, ((XdmAtomicValue)map[new XdmAtomicValue("integer")].Single()).AsInteger());
    }

    [Fact]
    public void YamlReader_AsXml()
    {
        var doc = YamlDocumentReader.ParseAsXml(@"
name: John
age: 30");

        Assert.NotNull(doc.DocumentElement);
        var nameElem = doc.DocumentElement.GetElementsByName("name").FirstOrDefault();
        Assert.NotNull(nameElem);
        Assert.Equal("John", nameElem.StringValue);
    }

    #endregion

    #region XML Serializer Tests

    [Fact]
    public void XmlSerializer_SimpleElement()
    {
        var elem = new XdmElement("root");
        var xml = XmlSerializer.Serialize(new XdmSequence(elem), new SerializationOptions { OmitXmlDeclaration = true });

        Assert.Contains("<root/>", xml);
    }

    [Fact]
    public void XmlSerializer_WithAttributes()
    {
        var elem = new XdmElement("root");
        elem.SetAttribute("id", "123");
        var xml = XmlSerializer.Serialize(new XdmSequence(elem), new SerializationOptions { OmitXmlDeclaration = true });

        Assert.Contains("id=\"123\"", xml);
    }

    [Fact]
    public void XmlSerializer_WithChildren()
    {
        var root = new XdmElement("root");
        root.AppendChild(new XdmElement("child"));
        var xml = XmlSerializer.Serialize(new XdmSequence(root), new SerializationOptions { OmitXmlDeclaration = true });

        Assert.Contains("<child/>", xml);
    }

    [Fact]
    public void XmlSerializer_WithText()
    {
        var elem = new XdmElement("root");
        elem.AppendChild(new XdmText("Hello"));
        var xml = XmlSerializer.Serialize(new XdmSequence(elem), new SerializationOptions { OmitXmlDeclaration = true });

        Assert.Contains(">Hello<", xml);
    }

    #endregion

    #region JSON Serializer Tests

    [Fact]
    public void JsonSerializer_Map()
    {
        var map = XdmMap.Builder()
            .Add("name", "John")
            .Add("age", 30L)
            .Build();

        var json = JsonSerializer.Serialize(new XdmSequence(map));

        Assert.Contains("\"name\"", json);
        Assert.Contains("\"John\"", json);
        Assert.Contains("\"age\"", json);
        Assert.Contains("30", json);
    }

    [Fact]
    public void JsonSerializer_Array()
    {
        var array = XdmArray.OfItems(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L)
        );

        var json = JsonSerializer.Serialize(new XdmSequence(array));

        Assert.Contains("[", json);
        Assert.Contains("1", json);
        Assert.Contains("2", json);
        Assert.Contains("3", json);
        Assert.Contains("]", json);
    }

    [Fact]
    public void JsonSerializer_AtomicValues()
    {
        Assert.Equal("42", JsonSerializer.Serialize(new XdmAtomicValue(42L)).Trim());
        Assert.Equal("\"hello\"", JsonSerializer.Serialize(new XdmAtomicValue("hello")).Trim());
        Assert.Equal("true", JsonSerializer.Serialize(XdmAtomicValue.True).Trim());
        Assert.Equal("false", JsonSerializer.Serialize(XdmAtomicValue.False).Trim());
    }

    #endregion

    #region YAML Serializer Tests

    [Fact]
    public void YamlSerializer_Map()
    {
        var map = XdmMap.Builder()
            .Add("name", "John")
            .Add("age", 30L)
            .Build();

        var yaml = YamlSerializer.Serialize(new XdmSequence(map));

        Assert.Contains("name:", yaml);
        Assert.Contains("John", yaml);
        Assert.Contains("age:", yaml);
        Assert.Contains("30", yaml);
    }

    [Fact]
    public void YamlSerializer_Array()
    {
        var array = XdmArray.OfItems(
            new XdmAtomicValue("apple"),
            new XdmAtomicValue("banana"),
            new XdmAtomicValue("cherry")
        );

        var yaml = YamlSerializer.Serialize(new XdmSequence(array));

        Assert.Contains("- apple", yaml);
        Assert.Contains("- banana", yaml);
        Assert.Contains("- cherry", yaml);
    }

    #endregion
}
