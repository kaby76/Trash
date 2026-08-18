using System.Collections.Generic;
using System.Linq;
using org.w3c.dom;
using XQuery.DataModel;

namespace ParseTreeEditing.UnvParseTreeDOM;

/// <summary>
/// Wraps a set of UnvParseTreeNode roots as an XdmDocument so the xquery4
/// engine can navigate the parse tree using standard XPath axes.
/// </summary>
public sealed class AdapterDocument : XdmDocument
{
    private readonly List<XdmNode> _kids = new();

    public override IReadOnlyList<XdmNode> Children => _kids;

    public override string StringValue
    {
        get
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in _kids) sb.Append(c.StringValue);
            return sb.ToString();
        }
    }

    public override string ToString()
    {
        var first = _kids.OfType<AdapterElement>().FirstOrDefault();
        return $"document {{ {first?.NodeName?.LocalName ?? "(empty)"} }}";
    }

    public static AdapterDocument Build(IEnumerable<UnvParseTreeNode> roots)
    {
        var doc = new AdapterDocument();
        foreach (var root in roots)
        {
            if (root is UnvParseTreeElement elem)
                doc._kids.Add(AdapterElement.Build(elem, doc));
        }
        return doc;
    }
}

/// <summary>
/// Wraps a UnvParseTreeElement as an XdmElement. Carries a back-reference
/// to the original Source node so results can be re-serialised.
/// </summary>
public sealed class AdapterElement : XdmElement
{
    public UnvParseTreeElement Source { get; }
    private readonly List<XdmNode> _kids = new();
    private readonly List<XdmAttribute> _attrs = new();

    AdapterElement(UnvParseTreeElement source) : base(source.LocalName ?? "")
    {
        Source = source;
    }

    public override IReadOnlyList<XdmNode> Children => _kids;
    public override IReadOnlyList<XdmAttribute> Attributes => _attrs;

    public override string StringValue
    {
        get
        {
            var sb = new System.Text.StringBuilder();
            CollectText(sb);
            return sb.ToString();
        }
    }

    private void CollectText(System.Text.StringBuilder sb)
    {
        foreach (var child in _kids)
        {
            if (child is AdapterText t) sb.Append(t.StringValue);
            else if (child is AdapterElement e) e.CollectText(sb);
        }
    }

    public static AdapterElement Build(UnvParseTreeElement source, XdmNode parent)
    {
        var elem = new AdapterElement(source);
        elem.Parent = parent;

        foreach (Node child in source.AllChildren)
        {
            switch (child)
            {
                case UnvParseTreeElement childElem:
                    elem._kids.Add(AdapterElement.Build(childElem, elem));
                    break;
                case UnvParseTreeText childText:
                    var t = new AdapterText(childText);
                    t.Parent = elem;
                    elem._kids.Add(t);
                    break;
                case UnvParseTreeAttr childAttr:
                    var a = new AdapterAttribute(childAttr);
                    a.Parent = elem;
                    elem._attrs.Add(a);
                    break;
            }
        }

        return elem;
    }

    // Override DeepCopy so element constructors get a proper XdmElement with
    // all children — the base XdmElement.DeepCopy() copies its own private
    // _children list (always empty for adapters which store in _kids).
    public override XdmNode DeepCopy()
    {
        var copy = new XdmElement(NodeName!);
        foreach (var attr in _attrs)
            copy.AddAttribute((XdmAttribute)attr.DeepCopy());
        foreach (var kid in _kids)
            copy.AppendChild(kid.DeepCopy());
        return copy;
    }
}

/// <summary>
/// Wraps a UnvParseTreeAttr as an XdmAttribute.
/// </summary>
public sealed class AdapterAttribute : XdmAttribute
{
    public UnvParseTreeAttr Source { get; }

    public AdapterAttribute(UnvParseTreeAttr source)
        : base(source.LocalName ?? source.Name?.ToString() ?? "", source.StringValue ?? "")
    {
        Source = source;
    }
}

/// <summary>
/// Wraps a UnvParseTreeText as an XdmText.
/// </summary>
public sealed class AdapterText : XdmText
{
    public UnvParseTreeText Source { get; }

    public AdapterText(UnvParseTreeText source) : base(source.Data ?? "")
    {
        Source = source;
    }
}
