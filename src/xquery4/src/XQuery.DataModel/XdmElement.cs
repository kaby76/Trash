namespace XQuery.DataModel;

/// <summary>
/// Represents an element node in the XDM 4.0 data model.
/// </summary>
public class XdmElement : XdmNode
{
    private readonly List<XdmNode> _children = new();
    private readonly List<XdmAttribute> _attributes = new();
    private readonly Dictionary<string, string> _namespaceBindings = new();
    private XdmQName _name;

    public XdmElement(XdmQName name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public XdmElement(string localName) : this(new XdmQName(localName))
    {
    }

    public XdmElement(string namespaceUri, string localName)
        : this(new XdmQName(namespaceUri, localName))
    {
    }

    public XdmElement(string namespaceUri, string localName, string prefix)
        : this(new XdmQName(namespaceUri, localName, prefix))
    {
    }

    public override XdmNodeKind NodeKind => XdmNodeKind.Element;

    public override XdmQName NodeName => _name;

    /// <summary>
    /// Gets or sets the name of this element.
    /// </summary>
    public XdmQName Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the local name of this element.
    /// </summary>
    public string LocalName => _name.LocalName;

    /// <summary>
    /// Gets the namespace URI of this element.
    /// </summary>
    public string NamespaceUri => _name.NamespaceUri;

    /// <summary>
    /// Gets the prefix of this element.
    /// </summary>
    public string Prefix => _name.Prefix;

    public override IReadOnlyList<XdmNode> Children => _children;

    public override IReadOnlyList<XdmAttribute> Attributes => _attributes;

    public override IReadOnlyDictionary<string, string> NamespaceBindings => _namespaceBindings;

    public override string StringValue
    {
        get
        {
            var sb = new System.Text.StringBuilder();
            CollectStringValue(sb);
            return sb.ToString();
        }
    }

    private void CollectStringValue(System.Text.StringBuilder sb)
    {
        foreach (var child in _children)
        {
            if (child is XdmText text)
                sb.Append(text.Value);
            else if (child is XdmElement element)
                element.CollectStringValue(sb);
        }
    }

    /// <summary>
    /// Adds a namespace binding.
    /// </summary>
    public void AddNamespace(string prefix, string namespaceUri)
    {
        _namespaceBindings[prefix] = namespaceUri;
    }

    /// <summary>
    /// Sets an attribute value.
    /// </summary>
    public void SetAttribute(string name, string value)
    {
        SetAttribute(new XdmQName(name), value);
    }

    /// <summary>
    /// Sets an attribute value.
    /// </summary>
    public void SetAttribute(XdmQName name, string value)
    {
        var existing = _attributes.FirstOrDefault(a => a.NodeName == name);
        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            var attr = new XdmAttribute(name, value);
            attr.Parent = this;
            _attributes.Add(attr);
        }
    }

    /// <summary>
    /// Sets an attribute value with namespace and prefix.
    /// </summary>
    public void SetAttribute(string localName, string value, string? namespaceUri, string? prefix)
    {
        var name = string.IsNullOrEmpty(namespaceUri)
            ? new XdmQName(localName)
            : new XdmQName(namespaceUri, localName, prefix ?? string.Empty);
        SetAttribute(name, value);
    }

    /// <summary>
    /// Gets an attribute value.
    /// </summary>
    public string? GetAttribute(string name)
    {
        return GetAttribute(new XdmQName(name));
    }

    /// <summary>
    /// Gets an attribute value.
    /// </summary>
    public string? GetAttribute(XdmQName name)
    {
        return _attributes.FirstOrDefault(a => a.NodeName == name)?.Value;
    }

    /// <summary>
    /// Removes an attribute.
    /// </summary>
    public bool RemoveAttribute(string name)
    {
        return RemoveAttribute(new XdmQName(name));
    }

    /// <summary>
    /// Removes an attribute.
    /// </summary>
    public bool RemoveAttribute(XdmQName name)
    {
        var attr = _attributes.FirstOrDefault(a => a.NodeName == name);
        if (attr != null)
        {
            attr.Parent = null;
            return _attributes.Remove(attr);
        }
        return false;
    }

    /// <summary>
    /// Removes an attribute by local name and namespace URI.
    /// </summary>
    public bool RemoveAttribute(string localName, string? namespaceUri)
    {
        var attr = _attributes.FirstOrDefault(a =>
            a.LocalName == localName &&
            (string.IsNullOrEmpty(namespaceUri) ? string.IsNullOrEmpty(a.NamespaceUri) : a.NamespaceUri == namespaceUri));
        if (attr != null)
        {
            attr.Parent = null;
            return _attributes.Remove(attr);
        }
        return false;
    }

    /// <summary>
    /// Adds an attribute.
    /// </summary>
    public void AddAttribute(XdmAttribute attribute)
    {
        if (attribute == null) throw new ArgumentNullException(nameof(attribute));
        if (attribute.Parent != null)
            throw new InvalidOperationException("Attribute already has a parent");

        // Check for duplicate
        if (_attributes.Any(a => a.NodeName == attribute.NodeName))
            throw new InvalidOperationException($"Attribute {attribute.NodeName} already exists");

        attribute.Parent = this;
        _attributes.Add(attribute);
    }

    /// <summary>
    /// Appends a child node.
    /// </summary>
    public void AppendChild(XdmNode child)
    {
        if (child == null) throw new ArgumentNullException(nameof(child));

        if (child is XdmDocument or XdmAttribute)
            throw new InvalidOperationException($"Cannot add {child.NodeKind} as child of element");

        if (child.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        // Merge adjacent text nodes
        if (child is XdmText newText && _children.Count > 0 && _children[^1] is XdmText lastText)
        {
            lastText.Value += newText.Value;
            return;
        }

        child.Parent = this;
        _children.Add(child);
    }

    /// <summary>
    /// Inserts a child at the specified index.
    /// </summary>
    public void InsertChildAt(int index, XdmNode child)
    {
        if (child == null) throw new ArgumentNullException(nameof(child));

        if (child is XdmDocument or XdmAttribute)
            throw new InvalidOperationException($"Cannot add {child.NodeKind} as child of element");

        if (child.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        if (index < 0 || index > _children.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        child.Parent = this;
        _children.Insert(index, child);
    }

    /// <summary>
    /// Renames this element.
    /// </summary>
    public void Rename(XdmQName newName)
    {
        _name = newName ?? throw new ArgumentNullException(nameof(newName));
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
            throw new ArgumentException("Reference node is not a child of this element");

        if (newChild is XdmDocument or XdmAttribute)
            throw new InvalidOperationException($"Cannot add {newChild.NodeKind} as child of element");

        if (newChild.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        newChild.Parent = this;
        _children.Insert(index, newChild);
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
            throw new ArgumentException("Old node is not a child of this element");

        if (newChild is XdmDocument or XdmAttribute)
            throw new InvalidOperationException($"Cannot add {newChild.NodeKind} as child of element");

        if (newChild.Parent != null)
            throw new InvalidOperationException("Node already has a parent");

        oldChild.Parent = null;
        newChild.Parent = this;
        _children[index] = newChild;
    }

    /// <summary>
    /// Clears all children.
    /// </summary>
    public void ClearChildren()
    {
        foreach (var child in _children)
            child.Parent = null;
        _children.Clear();
    }

    /// <summary>
    /// Sets the text content, replacing all children with a single text node.
    /// </summary>
    public void SetTextContent(string text)
    {
        ClearChildren();
        if (!string.IsNullOrEmpty(text))
            AppendChild(new XdmText(text));
    }

    /// <summary>
    /// Gets child elements by name.
    /// </summary>
    public IEnumerable<XdmElement> GetElementsByName(string localName)
    {
        return _children.OfType<XdmElement>().Where(e => e.LocalName == localName);
    }

    /// <summary>
    /// Gets child elements by name.
    /// </summary>
    public IEnumerable<XdmElement> GetElementsByName(XdmQName name)
    {
        return _children.OfType<XdmElement>().Where(e => e.NodeName == name);
    }

    /// <summary>
    /// Gets all descendant elements by name.
    /// </summary>
    public IEnumerable<XdmElement> GetDescendantsByName(string localName)
    {
        return Descendants().OfType<XdmElement>().Where(e => e.LocalName == localName);
    }

    public override XdmNode DeepCopy()
    {
        var copy = new XdmElement(_name);
        copy.BaseUri = BaseUri;

        foreach (var kvp in _namespaceBindings)
            copy._namespaceBindings[kvp.Key] = kvp.Value;

        foreach (var attr in _attributes)
        {
            var attrCopy = (XdmAttribute)attr.DeepCopy();
            attrCopy.Parent = copy;
            copy._attributes.Add(attrCopy);
        }

        foreach (var child in _children)
        {
            var childCopy = child.DeepCopy();
            childCopy.Parent = copy;
            copy._children.Add(childCopy);
        }

        return copy;
    }

    public override bool DeepEquals(XdmNode other)
    {
        if (other is not XdmElement elem) return false;
        if (_name != elem._name) return false;

        // Compare attributes
        if (_attributes.Count != elem._attributes.Count) return false;
        foreach (var attr in _attributes)
        {
            var otherAttr = elem._attributes.FirstOrDefault(a => a.NodeName == attr.NodeName);
            if (otherAttr == null || attr.Value != otherAttr.Value)
                return false;
        }

        // Compare children
        if (_children.Count != elem._children.Count) return false;
        for (int i = 0; i < _children.Count; i++)
        {
            if (!_children[i].DeepEquals(elem._children[i]))
                return false;
        }

        return true;
    }

    public override string ToString() => $"<{_name.PrefixedName}>";
}
