namespace XQuery.DataModel;

/// <summary>
/// Represents a text node in the XDM 4.0 data model.
/// </summary>
public class XdmText : XdmNode
{
    private string _value;

    public XdmText(string value)
    {
        _value = value ?? string.Empty;
    }

    public override XdmNodeKind NodeKind => XdmNodeKind.Text;

    public override XdmQName? NodeName => null;

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    public override string StringValue => _value;

    public override XdmSequence TypedValue =>
        new XdmSequence(XdmAtomicValue.UntypedAtomic(_value));

    public override XdmNode DeepCopy()
    {
        return new XdmText(_value);
    }

    public override bool DeepEquals(XdmNode other)
    {
        if (other is not XdmText text) return false;
        return _value == text._value;
    }

    public override string ToString() => $"\"{_value}\"";
}
