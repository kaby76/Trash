using System.Collections;
using XQuery.DataModel;

namespace XQuery.Engine;

/// <summary>
/// An identity-based collection of XDM nodes with lazy document-order normalization.
/// </summary>
internal sealed class NodeSet : IEnumerable<XdmNode>
{
    private readonly List<XdmNode> _nodes = new();
    private readonly HashSet<XdmNode> _membership = new();
    private readonly HashSet<XdmNode> _roots = new();
    private bool _isDocumentOrdered = true;

    public int Count => _nodes.Count;
    public int RootCount => _roots.Count;

    public bool Add(XdmNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!_membership.Add(node))
            return false;

        _roots.Add(node.Root);

        if (_isDocumentOrdered && _nodes.Count > 0 &&
            _nodes[^1].CompareDocumentOrder(node) > 0)
        {
            _isDocumentOrdered = false;
        }

        _nodes.Add(node);
        return true;
    }

    public bool Contains(XdmNode node) => _membership.Contains(node);

    public void Normalize()
    {
        if (_isDocumentOrdered || _nodes.Count < 2)
            return;

        _nodes.Sort(static (left, right) => left.CompareDocumentOrder(right));
        _isDocumentOrdered = true;
    }

    public XdmSequence ToXdmSequence()
    {
        Normalize();
        return new XdmSequence(_nodes.Cast<XdmItem>());
    }

    public static NodeSet FromSequence(XdmSequence sequence, string operation)
    {
        var result = new NodeSet();
        foreach (var item in sequence)
        {
            if (item is not XdmNode node)
                throw XdmException.TypeError($"{operation} requires node sequences");
            result.Add(node);
        }
        return result;
    }

    public IEnumerator<XdmNode> GetEnumerator() => _nodes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
