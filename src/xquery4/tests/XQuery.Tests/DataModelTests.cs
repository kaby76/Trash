using Xunit;
using XQuery.DataModel;

namespace XQuery.Tests;

public class DataModelTests
{
    #region Atomic Value Tests

    [Fact]
    public void StringValue_Creation()
    {
        var value = new XdmAtomicValue("hello");
        Assert.Equal("hello", value.StringValue);
        Assert.True(value.IsString);
        Assert.Equal(XdmQName.XsString, value.TypeName);
    }

    [Fact]
    public void IntegerValue_Creation()
    {
        var value = new XdmAtomicValue(42L);
        Assert.Equal(42, value.AsInteger());
        Assert.True(value.IsInteger);
        Assert.Equal("42", value.StringValue);
    }

    [Fact]
    public void DecimalValue_Creation()
    {
        var value = new XdmAtomicValue(3.14m);
        Assert.Equal(3.14m, value.AsDecimal());
        Assert.True(value.IsDecimal);
    }

    [Fact]
    public void DoubleValue_Creation()
    {
        var value = new XdmAtomicValue(2.718);
        Assert.Equal(2.718, value.AsDouble(), 3);
        Assert.True(value.IsDouble);
    }

    [Fact]
    public void BooleanValue_True()
    {
        var value = XdmAtomicValue.True;
        Assert.True(value.AsBoolean());
        Assert.Equal("true", value.StringValue);
    }

    [Fact]
    public void BooleanValue_False()
    {
        var value = XdmAtomicValue.False;
        Assert.False(value.AsBoolean());
        Assert.Equal("false", value.StringValue);
    }

    [Fact]
    public void AtomicValue_Equality()
    {
        var v1 = new XdmAtomicValue(42L);
        var v2 = new XdmAtomicValue(42L);
        var v3 = new XdmAtomicValue(43L);

        Assert.True(v1.ValueEquals(v2));
        Assert.False(v1.ValueEquals(v3));
    }

    [Fact]
    public void AtomicValue_Comparison()
    {
        var v1 = new XdmAtomicValue(10L);
        var v2 = new XdmAtomicValue(20L);

        Assert.True(v1.CompareTo(v2) < 0);
        Assert.True(v2.CompareTo(v1) > 0);
    }

    [Fact]
    public void AtomicValue_CastStringToInteger()
    {
        var str = new XdmAtomicValue("42");
        var integer = str.CastAs(XdmQName.XsInteger);
        Assert.Equal(42, integer.AsInteger());
    }

    [Fact]
    public void AtomicValue_EffectiveBooleanValue_String()
    {
        var empty = new XdmAtomicValue("");
        var nonEmpty = new XdmAtomicValue("hello");

        Assert.False(empty.EffectiveBooleanValue);
        Assert.True(nonEmpty.EffectiveBooleanValue);
    }

    [Fact]
    public void AtomicValue_EffectiveBooleanValue_Number()
    {
        var zero = new XdmAtomicValue(0L);
        var nonZero = new XdmAtomicValue(42L);
        var nan = new XdmAtomicValue(double.NaN);

        Assert.False(zero.EffectiveBooleanValue);
        Assert.True(nonZero.EffectiveBooleanValue);
        Assert.False(nan.EffectiveBooleanValue);
    }

    #endregion

    #region Sequence Tests

    [Fact]
    public void Sequence_Empty()
    {
        var seq = XdmSequence.Empty;
        Assert.True(seq.IsEmpty);
        Assert.Equal(0, seq.Count);
    }

    [Fact]
    public void Sequence_Singleton()
    {
        var seq = new XdmSequence(new XdmAtomicValue(42L));
        Assert.True(seq.IsSingleton);
        Assert.Equal(1, seq.Count);
    }

    [Fact]
    public void Sequence_Multiple()
    {
        var seq = XdmSequence.Of(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L)
        );
        Assert.Equal(3, seq.Count);
    }

    [Fact]
    public void Sequence_Concat()
    {
        var seq1 = XdmSequence.Of(new XdmAtomicValue(1L), new XdmAtomicValue(2L));
        var seq2 = XdmSequence.Of(new XdmAtomicValue(3L), new XdmAtomicValue(4L));
        var result = seq1.Concat(seq2);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Sequence_Subsequence()
    {
        var seq = XdmSequence.Of(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L),
            new XdmAtomicValue(4L)
        );

        var sub = seq.Subsequence(2, 2);
        Assert.Equal(2, sub.Count);
        Assert.Equal(2, (sub[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(3, (sub[1] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Sequence_Reverse()
    {
        var seq = XdmSequence.Of(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L)
        );

        var reversed = seq.Reverse();
        Assert.Equal(3, (reversed[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(1, (reversed[2] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Sequence_Range()
    {
        var seq = XdmSequence.Range(1, 5);
        Assert.Equal(5, seq.Count);
        Assert.Equal(1, (seq[0] as XdmAtomicValue)!.AsInteger());
        Assert.Equal(5, (seq[4] as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Sequence_EffectiveBooleanValue_Empty()
    {
        Assert.False(XdmSequence.Empty.EffectiveBooleanValue);
    }

    [Fact]
    public void Sequence_EffectiveBooleanValue_SingleBoolean()
    {
        var seq = new XdmSequence(XdmAtomicValue.True);
        Assert.True(seq.EffectiveBooleanValue);
    }

    #endregion

    #region Node Tests

    [Fact]
    public void Element_Creation()
    {
        var elem = new XdmElement("test");
        Assert.Equal("test", elem.LocalName);
        Assert.Equal(XdmNodeKind.Element, elem.NodeKind);
    }

    [Fact]
    public void Element_WithNamespace()
    {
        var elem = new XdmElement("http://example.org", "test", "ex");
        Assert.Equal("test", elem.LocalName);
        Assert.Equal("http://example.org", elem.NamespaceUri);
        Assert.Equal("ex", elem.Prefix);
    }

    [Fact]
    public void Element_Attributes()
    {
        var elem = new XdmElement("test");
        elem.SetAttribute("id", "123");
        elem.SetAttribute("name", "foo");

        Assert.Equal("123", elem.GetAttribute("id"));
        Assert.Equal("foo", elem.GetAttribute("name"));
        Assert.Equal(2, elem.Attributes.Count);
    }

    [Fact]
    public void Element_Children()
    {
        var parent = new XdmElement("parent");
        var child1 = new XdmElement("child");
        var child2 = new XdmElement("child");

        parent.AppendChild(child1);
        parent.AppendChild(child2);

        Assert.Equal(2, parent.Children.Count);
        Assert.Equal(parent, child1.Parent);
    }

    [Fact]
    public void Element_TextContent()
    {
        var elem = new XdmElement("test");
        elem.SetTextContent("Hello, World!");

        Assert.Equal("Hello, World!", elem.StringValue);
    }

    [Fact]
    public void Document_Creation()
    {
        var doc = new XdmDocument();
        var root = new XdmElement("root");
        doc.AppendChild(root);

        Assert.Equal(root, doc.DocumentElement);
    }

    [Fact]
    public void Node_Ancestors()
    {
        var doc = new XdmDocument();
        var root = new XdmElement("root");
        var child = new XdmElement("child");
        var grandchild = new XdmElement("grandchild");

        doc.AppendChild(root);
        root.AppendChild(child);
        child.AppendChild(grandchild);

        var ancestors = grandchild.Ancestors().ToList();
        Assert.Equal(3, ancestors.Count);
        Assert.Equal(child, ancestors[0]);
        Assert.Equal(root, ancestors[1]);
        Assert.Equal(doc, ancestors[2]);
    }

    [Fact]
    public void Node_Descendants()
    {
        var root = new XdmElement("root");
        var child1 = new XdmElement("child1");
        var child2 = new XdmElement("child2");
        var grandchild = new XdmElement("grandchild");

        root.AppendChild(child1);
        root.AppendChild(child2);
        child1.AppendChild(grandchild);

        var descendants = root.Descendants().ToList();
        Assert.Equal(3, descendants.Count);
    }

    [Fact]
    public void Node_DeepCopy()
    {
        var elem = new XdmElement("test");
        elem.SetAttribute("id", "123");
        elem.AppendChild(new XdmText("content"));

        var copy = elem.DeepCopy() as XdmElement;

        Assert.NotNull(copy);
        Assert.Equal("test", copy.LocalName);
        Assert.Equal("123", copy.GetAttribute("id"));
        Assert.Equal("content", copy.StringValue);
        Assert.NotSame(elem, copy);
    }

    #endregion

    #region Map Tests

    [Fact]
    public void Map_Empty()
    {
        var map = XdmMap.Empty;
        Assert.True(map.IsEmpty);
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void Map_Put()
    {
        var map = XdmMap.Empty;
        map = map.Put(new XdmAtomicValue("key"), new XdmSequence(new XdmAtomicValue("value")));

        Assert.Equal(1, map.Count);
        Assert.Equal("value", map[new XdmAtomicValue("key")].StringValue);
    }

    [Fact]
    public void Map_Builder()
    {
        var map = XdmMap.Builder()
            .Add("name", "John")
            .Add("age", 30L)
            .Build();

        Assert.Equal(2, map.Count);
        Assert.Equal("John", map[new XdmAtomicValue("name")].StringValue);
    }

    [Fact]
    public void Map_Remove()
    {
        var map = XdmMap.Builder()
            .Add("a", "1")
            .Add("b", "2")
            .Build();

        var newMap = map.Remove(new XdmAtomicValue("a"));
        Assert.Equal(1, newMap.Count);
        Assert.False(newMap.ContainsKey(new XdmAtomicValue("a")));
    }

    [Fact]
    public void Map_Merge()
    {
        var map1 = XdmMap.Builder().Add("a", "1").Build();
        var map2 = XdmMap.Builder().Add("b", "2").Build();

        var merged = map1.Merge(map2);
        Assert.Equal(2, merged.Count);
    }

    #endregion

    #region Array Tests

    [Fact]
    public void Array_Empty()
    {
        var array = XdmArray.Empty;
        Assert.True(array.IsEmpty);
        Assert.Equal(0, array.Count);
    }

    [Fact]
    public void Array_Append()
    {
        var array = XdmArray.Empty;
        array = array.Append(new XdmSequence(new XdmAtomicValue(1L)));
        array = array.Append(new XdmSequence(new XdmAtomicValue(2L)));

        Assert.Equal(2, array.Count);
    }

    [Fact]
    public void Array_Get()
    {
        var array = XdmArray.OfItems(
            new XdmAtomicValue(10L),
            new XdmAtomicValue(20L),
            new XdmAtomicValue(30L)
        );

        Assert.Equal(10, (array.Get(1).Single() as XdmAtomicValue)!.AsInteger());
        Assert.Equal(20, (array.Get(2).Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Array_Reverse()
    {
        var array = XdmArray.OfItems(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L)
        );

        var reversed = array.Reverse();
        Assert.Equal(3, (reversed.Get(1).Single() as XdmAtomicValue)!.AsInteger());
        Assert.Equal(1, (reversed.Get(3).Single() as XdmAtomicValue)!.AsInteger());
    }

    [Fact]
    public void Array_Flatten()
    {
        var array = XdmArray.OfItems(
            new XdmAtomicValue(1L),
            new XdmAtomicValue(2L),
            new XdmAtomicValue(3L)
        );

        var flat = array.Flatten();
        Assert.Equal(3, flat.Count);
    }

    #endregion

    #region QName Tests

    [Fact]
    public void QName_Simple()
    {
        var qname = new XdmQName("test");
        Assert.Equal("test", qname.LocalName);
        Assert.False(qname.HasNamespace);
        Assert.False(qname.HasPrefix);
    }

    [Fact]
    public void QName_WithNamespace()
    {
        var qname = new XdmQName("http://example.org", "test", "ex");
        Assert.Equal("test", qname.LocalName);
        Assert.Equal("http://example.org", qname.NamespaceUri);
        Assert.Equal("ex", qname.Prefix);
        Assert.Equal("ex:test", qname.PrefixedName);
        Assert.Equal("{http://example.org}test", qname.ClarkNotation);
    }

    [Fact]
    public void QName_Equality()
    {
        var q1 = new XdmQName("http://example.org", "test", "ex");
        var q2 = new XdmQName("http://example.org", "test", "different");

        Assert.True(q1.Equals(q2)); // Prefix doesn't matter for equality
    }

    #endregion
}
