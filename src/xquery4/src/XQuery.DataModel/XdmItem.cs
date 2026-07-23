namespace XQuery.DataModel;

/// <summary>
/// Base class for all items in the XDM 4.0 data model.
/// An item can be a node, an atomic value, a function, a map, or an array.
/// </summary>
public abstract class XdmItem : IEquatable<XdmItem>
{
    /// <summary>
    /// Gets the string value of this item.
    /// </summary>
    public abstract string StringValue { get; }

    /// <summary>
    /// Gets the typed value of this item.
    /// </summary>
    public abstract XdmSequence TypedValue { get; }

    /// <summary>
    /// Gets the type name of this item.
    /// </summary>
    public abstract XdmQName TypeName { get; }

    /// <summary>
    /// Returns true if this item is a node.
    /// </summary>
    public virtual bool IsNode => false;

    /// <summary>
    /// Returns true if this item is an atomic value.
    /// </summary>
    public virtual bool IsAtomicValue => false;

    /// <summary>
    /// Returns true if this item is a function.
    /// </summary>
    public virtual bool IsFunction => false;

    /// <summary>
    /// Returns true if this item is a map.
    /// </summary>
    public virtual bool IsMap => false;

    /// <summary>
    /// Returns true if this item is an array.
    /// </summary>
    public virtual bool IsArray => false;

    /// <summary>
    /// Performs atomization on this item.
    /// </summary>
    public virtual XdmSequence Atomize() => TypedValue;

    /// <summary>
    /// Gets the effective boolean value of this item.
    /// </summary>
    public abstract bool EffectiveBooleanValue { get; }

    public abstract bool Equals(XdmItem? other);

    public override bool Equals(object? obj) => obj is XdmItem item && Equals(item);

    public abstract override int GetHashCode();

    public override string ToString() => StringValue;

    /// <summary>
    /// Creates an XdmSequence containing just this item.
    /// </summary>
    public XdmSequence ToSequence() => new XdmSequence(this);
}
