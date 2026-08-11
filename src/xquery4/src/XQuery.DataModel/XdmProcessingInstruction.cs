namespace XQuery.DataModel;

/// <summary>
/// Represents a processing instruction node in the XDM 4.0 data model.
/// </summary>
public class XdmProcessingInstruction : XdmNode
{
    private string _target;
    private string _data;

    public XdmProcessingInstruction(string target, string data)
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _data = data ?? string.Empty;
    }

    public override XdmNodeKind NodeKind => XdmNodeKind.ProcessingInstruction;

    public override XdmQName? NodeName => new XdmQName(_target);

    /// <summary>
    /// Gets or sets the target of the processing instruction.
    /// </summary>
    public string Target
    {
        get => _target;
        set => _target = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the data of the processing instruction.
    /// </summary>
    public string Data
    {
        get => _data;
        set => _data = value ?? string.Empty;
    }

    public override string StringValue => _data;

    public override XdmSequence TypedValue =>
        new XdmSequence(new XdmAtomicValue(_data));

    public override XdmNode DeepCopy()
    {
        return new XdmProcessingInstruction(_target, _data);
    }

    public override bool DeepEquals(XdmNode other)
    {
        if (other is not XdmProcessingInstruction pi) return false;
        return _target == pi._target && _data == pi._data;
    }

    public override string ToString() => $"<?{_target} {_data}?>";
}
