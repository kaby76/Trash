namespace XQuery.DataModel;

/// <summary>
/// Represents a comment node in the XDM 4.0 data model.
/// </summary>
public class XdmComment : XdmNode
{
    private string _value;

    public XdmComment(string value)
    {
        _value = value ?? string.Empty;
    }

    public override XdmNodeKind NodeKind => XdmNodeKind.Comment;

    public override XdmQName? NodeName => null;

    /// <summary>
    /// Gets or sets the comment content.
    /// </summary>
    public string Value
    {
        get => _value;
        set => _value = value ?? string.Empty;
    }

    public override string StringValue => _value;

    public override XdmSequence TypedValue =>
        new XdmSequence(new XdmAtomicValue(_value));

    public override XdmNode DeepCopy()
    {
        return new XdmComment(_value);
    }

    public override bool DeepEquals(XdmNode other)
    {
        if (other is not XdmComment comment) return false;
        return _value == comment._value;
    }

    public override string ToString() => $"<!--{_value}-->";
}
