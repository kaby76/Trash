namespace XQuery.DataModel;

/// <summary>
/// The kind of an XDM node.
/// </summary>
public enum XdmNodeKind
{
    Document,
    Element,
    Attribute,
    Text,
    Comment,
    ProcessingInstruction,
    Namespace
}

/// <summary>
/// Base class for all nodes in the XDM 4.0 data model.
/// </summary>
public abstract class XdmNode : XdmItem
{
    private static long _nextNodeId = 0;
    private readonly long _nodeId;

    protected XdmNode()
    {
        _nodeId = Interlocked.Increment(ref _nextNodeId);
    }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public abstract XdmNodeKind NodeKind { get; }

    /// <summary>
    /// Gets the name of this node, or null if unnamed.
    /// </summary>
    public abstract XdmQName? NodeName { get; }

    /// <summary>
    /// Gets or sets the parent of this node.
    /// </summary>
    public XdmNode? Parent { get; internal set; }

    /// <summary>
    /// Gets the children of this node.
    /// </summary>
    public virtual IReadOnlyList<XdmNode> Children => Array.Empty<XdmNode>();

    /// <summary>
    /// Gets the attributes of this node.
    /// </summary>
    public virtual IReadOnlyList<XdmAttribute> Attributes => Array.Empty<XdmAttribute>();

    /// <summary>
    /// Gets the namespace bindings of this node.
    /// </summary>
    public virtual IReadOnlyDictionary<string, string> NamespaceBindings =>
        new Dictionary<string, string>();

    /// <summary>
    /// Gets the base URI of this node.
    /// </summary>
    public virtual Uri? BaseUri { get; set; }

    /// <summary>
    /// Gets the document URI of this node.
    /// </summary>
    public virtual Uri? DocumentUri => null;

    /// <summary>
    /// Gets the root of the tree containing this node.
    /// </summary>
    public XdmNode Root
    {
        get
        {
            var node = this;
            while (node.Parent != null)
                node = node.Parent;
            return node;
        }
    }

    /// <summary>
    /// Gets the document containing this node, or null if not in a document.
    /// </summary>
    public XdmDocument? OwnerDocument
    {
        get
        {
            var root = Root;
            return root as XdmDocument;
        }
    }

    public override bool IsNode => true;

    public override XdmQName TypeName => NodeKind switch
    {
        XdmNodeKind.Document => XdmQName.XsUntyped,
        XdmNodeKind.Element => XdmQName.XsUntyped,
        XdmNodeKind.Attribute => XdmQName.XsUntypedAtomic,
        XdmNodeKind.Text => XdmQName.XsUntypedAtomic,
        XdmNodeKind.Comment => XdmQName.XsString,
        XdmNodeKind.ProcessingInstruction => XdmQName.XsString,
        XdmNodeKind.Namespace => XdmQName.XsString,
        _ => XdmQName.XsUntyped
    };

    public override XdmSequence TypedValue
    {
        get
        {
            var str = StringValue;
            return new XdmSequence(XdmAtomicValue.UntypedAtomic(str));
        }
    }

    public override bool EffectiveBooleanValue => true;

    /// <summary>
    /// Gets the unique identity of this node within the model.
    /// </summary>
    public long NodeId => _nodeId;

    /// <summary>
    /// Compares this node to another in document order.
    /// Returns negative if this precedes other, positive if other precedes this, zero if same.
    /// </summary>
    public int CompareDocumentOrder(XdmNode other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        // Get the paths from root to each node
        var path1 = GetPathFromRoot(this);
        var path2 = GetPathFromRoot(other);

        // Compare paths
        int minLen = Math.Min(path1.Count, path2.Count);
        for (int i = 0; i < minLen; i++)
        {
            if (!ReferenceEquals(path1[i], path2[i]))
            {
                // Find positions among siblings
                var parent = i > 0 ? path1[i - 1] : null;
                if (parent == null)
                    return path1[i].NodeId.CompareTo(path2[i].NodeId);

                return GetSiblingPosition(parent, path1[i]).CompareTo(
                    GetSiblingPosition(parent, path2[i]));
            }
        }

        // One is ancestor of the other
        return path1.Count.CompareTo(path2.Count);
    }

    private static List<XdmNode> GetPathFromRoot(XdmNode node)
    {
        var path = new List<XdmNode>();
        while (node != null)
        {
            path.Insert(0, node);
            node = node.Parent!;
        }
        return path;
    }

    private static int GetSiblingPosition(XdmNode parent, XdmNode child)
    {
        // Attributes come before children
        if (child is XdmAttribute attr)
        {
            var attrs = parent.Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                if (ReferenceEquals(attrs[i], child))
                    return i;
            }
            return -1;
        }

        var children = parent.Children;
        for (int i = 0; i < children.Count; i++)
        {
            if (ReferenceEquals(children[i], child))
                return parent.Attributes.Count + i;
        }
        return -1;
    }

    /// <summary>
    /// Gets all descendant nodes.
    /// </summary>
    public IEnumerable<XdmNode> Descendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.Descendants())
                yield return descendant;
        }
    }

    /// <summary>
    /// Gets all descendant nodes including this node.
    /// </summary>
    public IEnumerable<XdmNode> DescendantsAndSelf()
    {
        yield return this;
        foreach (var descendant in Descendants())
            yield return descendant;
    }

    /// <summary>
    /// Gets all ancestor nodes.
    /// </summary>
    public IEnumerable<XdmNode> Ancestors()
    {
        var node = Parent;
        while (node != null)
        {
            yield return node;
            node = node.Parent;
        }
    }

    /// <summary>
    /// Gets all ancestor nodes including this node.
    /// </summary>
    public IEnumerable<XdmNode> AncestorsAndSelf()
    {
        yield return this;
        foreach (var ancestor in Ancestors())
            yield return ancestor;
    }

    /// <summary>
    /// Gets all following sibling nodes.
    /// </summary>
    public IEnumerable<XdmNode> FollowingSiblings()
    {
        if (Parent == null) yield break;

        bool found = false;
        foreach (var sibling in Parent.Children)
        {
            if (found)
                yield return sibling;
            else if (ReferenceEquals(sibling, this))
                found = true;
        }
    }

    /// <summary>
    /// Gets all preceding sibling nodes.
    /// </summary>
    public IEnumerable<XdmNode> PrecedingSiblings()
    {
        if (Parent == null) return Enumerable.Empty<XdmNode>();

        var siblings = new List<XdmNode>();
        foreach (var sibling in Parent.Children)
        {
            if (ReferenceEquals(sibling, this))
                break;
            siblings.Add(sibling);
        }
        siblings.Reverse();
        return siblings;
    }

    /// <summary>
    /// Gets all following nodes in document order.
    /// </summary>
    public IEnumerable<XdmNode> Following()
    {
        // Following siblings and their descendants
        foreach (var sibling in FollowingSiblings())
        {
            yield return sibling;
            foreach (var desc in sibling.Descendants())
                yield return desc;
        }

        // Then parent's following
        if (Parent != null)
        {
            foreach (var node in Parent.Following())
                yield return node;
        }
    }

    /// <summary>
    /// Gets all preceding nodes in reverse document order.
    /// </summary>
    public IEnumerable<XdmNode> Preceding()
    {
        // Preceding siblings and their descendants (in reverse)
        foreach (var sibling in PrecedingSiblings())
        {
            foreach (var desc in sibling.Descendants().Reverse())
                yield return desc;
            yield return sibling;
        }

        // Then parent's preceding
        if (Parent != null)
        {
            foreach (var node in Parent.Preceding())
                yield return node;
        }
    }

    public override bool Equals(XdmItem? other)
    {
        // Node identity comparison
        if (other is XdmNode node)
            return ReferenceEquals(this, node);
        return false;
    }

    /// <summary>
    /// Deep equality comparison.
    /// </summary>
    public virtual bool DeepEquals(XdmNode other)
    {
        if (other == null) return false;
        if (NodeKind != other.NodeKind) return false;
        if (NodeName != other.NodeName) return false;
        return StringValue == other.StringValue;
    }

    public override int GetHashCode() => _nodeId.GetHashCode();

    /// <summary>
    /// Creates a deep copy of this node.
    /// </summary>
    public abstract XdmNode DeepCopy();

    /// <summary>
    /// Looks up a namespace URI for a prefix in the scope of this node.
    /// </summary>
    public virtual string? LookupNamespaceUri(string prefix)
    {
        if (NamespaceBindings.TryGetValue(prefix, out var uri))
            return uri;
        return Parent?.LookupNamespaceUri(prefix);
    }

    /// <summary>
    /// Looks up a prefix for a namespace URI in the scope of this node.
    /// </summary>
    public virtual string? LookupPrefix(string namespaceUri)
    {
        foreach (var kvp in NamespaceBindings)
        {
            if (kvp.Value == namespaceUri)
                return kvp.Key;
        }
        return Parent?.LookupPrefix(namespaceUri);
    }
}
