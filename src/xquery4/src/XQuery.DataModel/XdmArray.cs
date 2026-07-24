using System.Collections;

namespace XQuery.DataModel;

/// <summary>
/// Represents an array item in the XDM 4.0 data model.
/// Arrays are ordered lists of values (sequences).
/// </summary>
public class XdmArray : XdmItem, IEnumerable<XdmSequence>
{
    private readonly List<XdmSequence> _members = new();

    public override bool IsArray => true;
    public override bool IsFunction => true; // Arrays are functions

    public override XdmQName TypeName => new XdmQName(XdmQName.ArrayNamespace, "array", "array");

    /// <summary>
    /// Gets the number of members in the array.
    /// </summary>
    public int Count => _members.Count;

    /// <summary>
    /// Returns true if the array is empty.
    /// </summary>
    public bool IsEmpty => _members.Count == 0;

    /// <summary>
    /// Gets the member at the specified position (1-based, as per XPath).
    /// </summary>
    public XdmSequence Get(int position)
    {
        if (position < 1 || position > _members.Count)
            throw new XdmException("FOAY0001", $"Array index {position} out of bounds (1 to {_members.Count})");
        return _members[position - 1];
    }

    /// <summary>
    /// Gets the member at the specified 0-based index.
    /// </summary>
    public XdmSequence this[int index] => _members[index];

    /// <summary>
    /// Gets all members as a sequence of sequences.
    /// </summary>
    public IReadOnlyList<XdmSequence> Members => _members;

    /// <summary>
    /// Appends a member to the array. Returns a new array.
    /// </summary>
    public XdmArray Append(XdmSequence member)
    {
        var newArray = new XdmArray();
        newArray._members.AddRange(_members);
        newArray._members.Add(member);
        return newArray;
    }

    /// <summary>
    /// Inserts a member at the specified position (1-based). Returns a new array.
    /// </summary>
    public XdmArray InsertBefore(int position, XdmSequence member)
    {
        if (position < 1 || position > _members.Count + 1)
            throw new XdmException("FOAY0001", $"Array index {position} out of bounds");

        var newArray = new XdmArray();
        newArray._members.AddRange(_members.Take(position - 1));
        newArray._members.Add(member);
        newArray._members.AddRange(_members.Skip(position - 1));
        return newArray;
    }

    /// <summary>
    /// Removes the member at the specified position (1-based). Returns a new array.
    /// </summary>
    public XdmArray Remove(int position)
    {
        if (position < 1 || position > _members.Count)
            throw new XdmException("FOAY0001", $"Array index {position} out of bounds");

        var newArray = new XdmArray();
        newArray._members.AddRange(_members.Take(position - 1));
        newArray._members.AddRange(_members.Skip(position));
        return newArray;
    }

    /// <summary>
    /// Replaces the member at the specified position (1-based). Returns a new array.
    /// </summary>
    public XdmArray Put(int position, XdmSequence member)
    {
        if (position < 1 || position > _members.Count)
            throw new XdmException("FOAY0001", $"Array index {position} out of bounds");

        var newArray = new XdmArray();
        newArray._members.AddRange(_members);
        newArray._members[position - 1] = member;
        return newArray;
    }

    /// <summary>
    /// Returns a subarray. Positions are 1-based.
    /// </summary>
    public XdmArray Subarray(int start, int? length = null)
    {
        if (start < 1)
            throw new XdmException("FOAY0001", "Array start index must be at least 1");

        int startIndex = start - 1;
        int actualLength = length ?? (_members.Count - startIndex);

        if (startIndex > _members.Count)
            return Empty;

        actualLength = Math.Min(actualLength, _members.Count - startIndex);

        var newArray = new XdmArray();
        newArray._members.AddRange(_members.Skip(startIndex).Take(actualLength));
        return newArray;
    }

    /// <summary>
    /// Reverses the array. Returns a new array.
    /// </summary>
    public XdmArray Reverse()
    {
        var newArray = new XdmArray();
        for (int i = _members.Count - 1; i >= 0; i--)
            newArray._members.Add(_members[i]);
        return newArray;
    }

    /// <summary>
    /// Joins all members into a single sequence.
    /// </summary>
    public XdmSequence Flatten()
    {
        var items = new List<XdmItem>();
        foreach (var member in _members)
        {
            items.AddRange(member);
        }
        return new XdmSequence(items);
    }

    /// <summary>
    /// Returns the first member, or empty sequence if array is empty.
    /// </summary>
    public XdmSequence Head()
    {
        return _members.Count > 0 ? _members[0] : XdmSequence.Empty;
    }

    /// <summary>
    /// Returns all members except the first. Returns a new array.
    /// </summary>
    public XdmArray Tail()
    {
        if (_members.Count <= 1)
            return Empty;

        var newArray = new XdmArray();
        newArray._members.AddRange(_members.Skip(1));
        return newArray;
    }

    /// <summary>
    /// Concatenates two arrays. Returns a new array.
    /// </summary>
    public XdmArray Join(XdmArray other)
    {
        var newArray = new XdmArray();
        newArray._members.AddRange(_members);
        newArray._members.AddRange(other._members);
        return newArray;
    }

    /// <summary>
    /// Returns an empty array.
    /// </summary>
    public static XdmArray Empty => new();

    public override string StringValue =>
        throw new XdmException("FOTY0014", "Arrays do not have a string value");

    public override XdmSequence TypedValue =>
        throw new XdmException("FOTY0014", "Arrays do not have a typed value");

    public override bool EffectiveBooleanValue =>
        throw new XdmException("FORG0006", "Cannot compute effective boolean value of an array");

    public override bool Equals(XdmItem? other)
    {
        if (other is not XdmArray otherArray) return false;
        return DeepEquals(otherArray);
    }

    /// <summary>
    /// Deep equality comparison.
    /// </summary>
    public bool DeepEquals(XdmArray other)
    {
        if (other is null) return false;
        if (_members.Count != other._members.Count) return false;

        for (int i = 0; i < _members.Count; i++)
        {
            if (!_members[i].Equals(other._members[i]))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var member in _members)
            hash.Add(member);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        if (_members.Count == 0)
            return "[]";

        var members = _members.Select(m => m.Count == 1 ? m.First?.ToString() : $"({m})");
        return $"[{string.Join(", ", members)}]";
    }

    public IEnumerator<XdmSequence> GetEnumerator() => _members.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Creates an array from sequences.
    /// </summary>
    public static XdmArray Of(params XdmSequence[] members)
    {
        var array = new XdmArray();
        array._members.AddRange(members);
        return array;
    }

    /// <summary>
    /// Creates an array from items.
    /// </summary>
    public static XdmArray OfItems(params XdmItem[] items)
    {
        var array = new XdmArray();
        foreach (var item in items)
            array._members.Add(new XdmSequence(item));
        return array;
    }

    /// <summary>
    /// Creates an array from a sequence, where each item becomes a member.
    /// </summary>
    public static XdmArray FromSequence(XdmSequence sequence)
    {
        var array = new XdmArray();
        foreach (var item in sequence)
            array._members.Add(new XdmSequence(item));
        return array;
    }

    /// <summary>
    /// Creates a mutable array builder.
    /// </summary>
    public static ArrayBuilder Builder() => new();

    public class ArrayBuilder
    {
        private readonly XdmArray _array = new();

        public ArrayBuilder Add(XdmSequence member)
        {
            _array._members.Add(member);
            return this;
        }

        public ArrayBuilder Add(XdmItem item) =>
            Add(new XdmSequence(item));

        public ArrayBuilder Add(string value) =>
            Add(new XdmAtomicValue(value));

        public ArrayBuilder Add(long value) =>
            Add(new XdmAtomicValue(value));

        public ArrayBuilder Add(bool value) =>
            Add(new XdmAtomicValue(value));

        public XdmArray Build() => _array;
    }
}
