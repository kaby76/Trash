using System.Runtime.CompilerServices;

namespace XQuery.DataModel;

/// <summary>
/// Represents a function item in the XDM 4.0 data model.
/// </summary>
public class XdmFunction : XdmItem
{
    private readonly XdmQName? _name;
    private readonly int _arity;
    private readonly Func<XdmSequence[], XdmSequence> _implementation;
    private readonly XdmSequenceType[] _parameterTypes;
    private readonly XdmSequenceType _returnType;

    public XdmFunction(
        XdmQName? name,
        int arity,
        Func<XdmSequence[], XdmSequence> implementation,
        XdmSequenceType[]? parameterTypes = null,
        XdmSequenceType? returnType = null)
    {
        _name = name;
        _arity = arity;
        _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        _parameterTypes = parameterTypes ?? new XdmSequenceType[arity];
        _returnType = returnType ?? XdmSequenceType.ItemStar;
    }

    public override bool IsFunction => true;

    public override XdmQName TypeName => new XdmQName(XdmQName.FnNamespace, "function", "fn");

    /// <summary>
    /// Gets the name of this function, or null for anonymous functions.
    /// </summary>
    public XdmQName? Name => _name;

    /// <summary>
    /// Gets the arity (number of parameters) of this function.
    /// </summary>
    public int Arity => _arity;

    /// <summary>
    /// Gets the parameter types of this function.
    /// </summary>
    public IReadOnlyList<XdmSequenceType> ParameterTypes => _parameterTypes;

    /// <summary>
    /// Gets the return type of this function.
    /// </summary>
    public XdmSequenceType ReturnType => _returnType;

    /// <summary>
    /// Invokes this function with the given arguments.
    /// </summary>
    public XdmSequence Invoke(params XdmSequence[] arguments)
    {
        if (arguments.Length != _arity)
            throw new XdmException("XPTY0004", $"Function expects {_arity} arguments but received {arguments.Length}");

        return _implementation(arguments);
    }

    public override string StringValue =>
        throw new XdmException("FOTY0014", "Functions do not have a string value");

    public override XdmSequence TypedValue =>
        throw new XdmException("FOTY0014", "Functions do not have a typed value");

    public override bool EffectiveBooleanValue =>
        throw new XdmException("FORG0006", "Cannot compute effective boolean value of a function");

    public override bool Equals(XdmItem? other)
    {
        // Functions are compared by identity
        return ReferenceEquals(this, other);
    }

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public override string ToString()
    {
        var nameStr = _name?.ToString() ?? "(anonymous)";
        return $"function {nameStr}#{_arity}";
    }

    /// <summary>
    /// Creates a partial application of this function.
    /// </summary>
    public XdmFunction Partial(XdmSequence?[] arguments)
    {
        // Count placeholders
        int newArity = arguments.Count(a => a is null);

        return new XdmFunction(
            null,
            newArity,
            args =>
            {
                var fullArgs = new XdmSequence[_arity];
                int argIndex = 0;

                for (int i = 0; i < _arity; i++)
                {
                    fullArgs[i] = arguments[i] ?? args[argIndex++];
                }

                return Invoke(fullArgs);
            });
    }
}

/// <summary>
/// Represents a sequence type (item type + occurrence indicator).
/// </summary>
public class XdmSequenceType
{
    public XdmItemType ItemType { get; }
    public OccurrenceIndicator Occurrence { get; }

    public XdmSequenceType(XdmItemType itemType, OccurrenceIndicator occurrence)
    {
        ItemType = itemType;
        Occurrence = occurrence;
    }

    public bool AllowsEmpty => Occurrence is OccurrenceIndicator.ZeroOrMore or OccurrenceIndicator.ZeroOrOne;
    public bool AllowsMany => Occurrence is OccurrenceIndicator.ZeroOrMore or OccurrenceIndicator.OneOrMore;

    public override string ToString()
    {
        var suffix = Occurrence switch
        {
            OccurrenceIndicator.ZeroOrOne => "?",
            OccurrenceIndicator.ZeroOrMore => "*",
            OccurrenceIndicator.OneOrMore => "+",
            _ => ""
        };
        return ItemType.ToString() + suffix;
    }

    public static readonly XdmSequenceType ItemStar = new(XdmItemType.Item, OccurrenceIndicator.ZeroOrMore);
    public static readonly XdmSequenceType ItemPlus = new(XdmItemType.Item, OccurrenceIndicator.OneOrMore);
    public static readonly XdmSequenceType ItemOne = new(XdmItemType.Item, OccurrenceIndicator.ExactlyOne);
    public static readonly XdmSequenceType ItemOpt = new(XdmItemType.Item, OccurrenceIndicator.ZeroOrOne);
    public static readonly XdmSequenceType Empty = new(XdmItemType.Empty, OccurrenceIndicator.ZeroOrMore);
}

public enum OccurrenceIndicator
{
    ExactlyOne,
    ZeroOrOne,
    ZeroOrMore,
    OneOrMore
}

/// <summary>
/// Represents an item type in a sequence type.
/// </summary>
public class XdmItemType
{
    public ItemTypeKind Kind { get; }
    public XdmQName? TypeName { get; }
    public XdmQName? ElementName { get; }
    public XdmNodeKind? NodeKind { get; }

    private XdmItemType(ItemTypeKind kind, XdmQName? typeName = null, XdmQName? elementName = null, XdmNodeKind? nodeKind = null)
    {
        Kind = kind;
        TypeName = typeName;
        ElementName = elementName;
        NodeKind = nodeKind;
    }

    public override string ToString()
    {
        return Kind switch
        {
            ItemTypeKind.Item => "item()",
            ItemTypeKind.Empty => "empty-sequence()",
            ItemTypeKind.AtomicType => TypeName?.ToString() ?? "xs:anyAtomicType",
            ItemTypeKind.Node => "node()",
            ItemTypeKind.Element => ElementName != null ? $"element({ElementName})" : "element()",
            ItemTypeKind.Attribute => "attribute()",
            ItemTypeKind.Document => "document-node()",
            ItemTypeKind.Text => "text()",
            ItemTypeKind.Comment => "comment()",
            ItemTypeKind.ProcessingInstruction => "processing-instruction()",
            ItemTypeKind.Function => "function(*)",
            ItemTypeKind.Map => "map(*)",
            ItemTypeKind.Array => "array(*)",
            _ => "item()"
        };
    }

    public static readonly XdmItemType Item = new(ItemTypeKind.Item);
    public static readonly XdmItemType Empty = new(ItemTypeKind.Empty);
    public static readonly XdmItemType Node = new(ItemTypeKind.Node);
    public static readonly XdmItemType Element = new(ItemTypeKind.Element);
    public static readonly XdmItemType Attribute = new(ItemTypeKind.Attribute);
    public static readonly XdmItemType Document = new(ItemTypeKind.Document);
    public static readonly XdmItemType Text = new(ItemTypeKind.Text);
    public static readonly XdmItemType Comment = new(ItemTypeKind.Comment);
    public static readonly XdmItemType ProcessingInstruction = new(ItemTypeKind.ProcessingInstruction);
    public static readonly XdmItemType Function = new(ItemTypeKind.Function);
    public static readonly XdmItemType Map = new(ItemTypeKind.Map);
    public static readonly XdmItemType Array = new(ItemTypeKind.Array);

    public static XdmItemType AtomicType(XdmQName typeName) =>
        new(ItemTypeKind.AtomicType, typeName);

    public static XdmItemType ElementType(XdmQName? name = null, XdmQName? typeName = null) =>
        new(ItemTypeKind.Element, typeName, name);
}

public enum ItemTypeKind
{
    Item,
    Empty,
    AtomicType,
    Node,
    Element,
    Attribute,
    Document,
    Text,
    Comment,
    ProcessingInstruction,
    Namespace,
    Function,
    Map,
    Array
}
