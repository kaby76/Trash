namespace XQuery.DataModel;

/// <summary>
/// Represents an attribute node in the XDM 4.0 data model.
/// </summary>
public class XdmAttribute : XdmNode
{
    private XdmQName _name;
    private string _value;

    public XdmAttribute(XdmQName name, string value)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _value = value ?? string.Empty;
    }

    public XdmAttribute(string name, string value)
        : this(new XdmQName(name), value)
    {
    }

    public XdmAttribute(string namespaceUri, string localName, string value)
        : this(new XdmQName(namespaceUri, localName), value)
    {
    }

    public XdmAttribute(string namespaceUri, string localName, string prefix, string value)
        : this(new XdmQName(namespaceUri, localName, prefix), value)
    {
    }

    public override XdmNodeKind NodeKind => XdmNodeKind.Attribute;

    public override XdmQName NodeName => _name;

    /// <summary>
    /// Gets or sets the name of this attribute.
    /// </summary>
    public XdmQName Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the local name of this attribute.
    /// </summary>
    public string LocalName => _name.LocalName;

    /// <summary>
    /// Gets the namespace URI of this attribute.
    /// </summary>
    public string NamespaceUri => _name.NamespaceUri;

    /// <summary>
    /// Gets the prefix of this attribute.
    /// </summary>
    public string Prefix => _name.Prefix;

    /// <summary>
    /// Gets or sets the value of this attribute.
    /// </summary>
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    public override string StringValue => _value;

    public override XdmSequence TypedValue =>
        new XdmSequence(XdmAtomicValue.UntypedAtomic(_value));

    /// <summary>
    /// Gets the owner element of this attribute.
    /// </summary>
    public XdmElement? OwnerElement => Parent as XdmElement;

    /// <summary>
    /// Renames this attribute.
    /// </summary>
    public void Rename(XdmQName newName)
    {
        _name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public override XdmNode DeepCopy()
    {
        return new XdmAttribute(_name, _value);
    }

    public override bool DeepEquals(XdmNode other)
    {
        if (other is not XdmAttribute attr) return false;
        return _name == attr._name && _value == attr._value;
    }

    public override string ToString() => $"@{_name.PrefixedName}=\"{_value}\"";
}
