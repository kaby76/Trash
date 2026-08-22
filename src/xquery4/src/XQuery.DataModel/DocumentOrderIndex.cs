namespace XQuery.DataModel;

/// <summary>
/// Immutable preorder positions for one version of an XDM tree.
/// </summary>
public sealed class DocumentOrderIndex
{
    private readonly Dictionary<XdmNode, int> _positions = new();

    internal DocumentOrderIndex(XdmNode root, long structuralVersion)
    {
        Root = root;
        StructuralVersion = structuralVersion;
        var position = 0;
        AddSubtree(root, ref position);
    }

    public XdmNode Root { get; }
    public long StructuralVersion { get; }
    public int Count => _positions.Count;

    public int GetPosition(XdmNode node)
    {
        if (!ReferenceEquals(node.Root, Root) || !_positions.TryGetValue(node, out var position))
            throw new ArgumentException("Node does not belong to this document-order index.", nameof(node));
        return position;
    }

    private void AddSubtree(XdmNode node, ref int position)
    {
        _positions.Add(node, position++);

        // Namespace nodes are not represented in this XDM implementation.
        // Attributes precede children in XPath document order.
        foreach (var attribute in node.Attributes)
            _positions.Add(attribute, position++);

        foreach (var child in node.Children)
            AddSubtree(child, ref position);
    }
}
