namespace XQuery.DataModel;

/// <summary>
/// Represents a document node in the XDM 4.0 data model.
/// </summary>
public class XdmDocument : XdmNode
{
    private readonly List<XdmNode> _children = new();
    private Uri? _documentUri;

    public override XdmNodeKind NodeKind => XdmNodeKind.Document;

    public override XdmQName? NodeName => null;

    public override IReadOnlyList<XdmNode> Children => _children;

    public override Uri? DocumentUri
    {
        get => _documentUri;
    }

    public void SetDocumentUri(Uri? uri) => _documentUri = uri;

    public override string StringValue
    {
        get
        {
            var sb = new System.Text.StringBuilder();
            foreach (var child in _children)
            {
                if (child is XdmText || child is XdmElement)
                    sb.Append(child.StringValue);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the document element (root element).
    /// </summary>
    public XdmElement? DocumentElement =>
        _children.OfType<XdmElement>().FirstOrDefault();

    /// <summary>
    /// Appends a child node to this document.
    /// </summary>
    public void AppendChild(XdmNode child)
    {
        if (child == null) throw new ArgumentNullException(nameof(child));

        // Documents can only contain one element
        if (child is XdmElement && _children.OfType<XdmElement>().Any())
            throw new InvalidOperationException("Document can only have one element child");

        // Documents can only contain elements, processing instructions, and comments
        if (child is not (XdmElement or XdmProcessingInstruction or XdmComment or XdmText))
            throw new InvalidOperationException($"Cannot add {child.NodeKind} as child of document");

        if (child.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        child.Parent = this;
        _children.Add(child);
    }

    /// <summary>
    /// Inserts a child before the reference node.
    /// </summary>
    public void InsertBefore(XdmNode newChild, XdmNode? refChild)
    {
        if (newChild == null) throw new ArgumentNullException(nameof(newChild));

        if (refChild == null)
        {
            AppendChild(newChild);
            return;
        }

        int index = _children.IndexOf(refChild);
        if (index < 0)
            throw new ArgumentException("Reference node is not a child of this document");

        if (newChild is XdmElement && _children.OfType<XdmElement>().Any())
            throw new InvalidOperationException("Document can only have one element child");

        if (newChild.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        newChild.Parent = this;
        _children.Insert(index, newChild);
    }

    /// <summary>
    /// Inserts a child at the specified index.
    /// </summary>
    public void InsertChildAt(int index, XdmNode child)
    {
        if (child == null) throw new ArgumentNullException(nameof(child));

        // Documents can only contain one element
        if (child is XdmElement && _children.OfType<XdmElement>().Any())
            throw new InvalidOperationException("Document can only have one element child");

        // Documents can only contain elements, processing instructions, and comments
        if (child is not (XdmElement or XdmProcessingInstruction or XdmComment or XdmText))
            throw new InvalidOperationException($"Cannot add {child.NodeKind} as child of document");

        if (child.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        if (index < 0 || index > _children.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        child.Parent = this;
        _children.Insert(index, child);
    }

    /// <summary>
    /// Removes a child node.
    /// </summary>
    public void RemoveChild(XdmNode child)
    {
        if (child == null) throw new ArgumentNullException(nameof(child));

        if (_children.Remove(child))
            child.Parent = null;
    }

    /// <summary>
    /// Replaces a child node.
    /// </summary>
    public void ReplaceChild(XdmNode newChild, XdmNode oldChild)
    {
        if (newChild == null) throw new ArgumentNullException(nameof(newChild));
        if (oldChild == null) throw new ArgumentNullException(nameof(oldChild));

        int index = _children.IndexOf(oldChild);
        if (index < 0)
            throw new ArgumentException("Old node is not a child of this document");

        if (newChild is XdmElement && oldChild is not XdmElement && _children.OfType<XdmElement>().Any())
            throw new InvalidOperationException("Document can only have one element child");

        if (newChild.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        oldChild.Parent = null;
        newChild.Parent = this;
        _children[index] = newChild;
    }

    public override XdmNode DeepCopy()
    {
        var copy = new XdmDocument();
        copy._documentUri = _documentUri;
        copy.BaseUri = BaseUri;

        foreach (var child in _children)
        {
            var childCopy = child.DeepCopy();
            childCopy.Parent = copy;
            copy._children.Add(childCopy);
        }

        return copy;
    }

    public override string ToString() => $"document {{ {DocumentElement?.NodeName?.LocalName ?? "(empty)"} }}";
}
