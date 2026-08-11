using System.Collections;

namespace XQuery.DataModel;

/// <summary>
/// Represents a sequence in the XDM 4.0 data model.
/// A sequence is an ordered collection of zero or more items.
/// Sequences are always flat - they never contain other sequences.
/// </summary>
public sealed class XdmSequence : IEnumerable<XdmItem>, IEquatable<XdmSequence>
{
    private readonly List<XdmItem> _items;

    /// <summary>
    /// Gets the empty sequence.
    /// </summary>
    public static readonly XdmSequence Empty = new();

    public XdmSequence()
    {
        _items = new List<XdmItem>();
    }

    public XdmSequence(XdmItem item)
    {
        _items = item is null ? new List<XdmItem>() : new List<XdmItem> { item };
    }

    public XdmSequence(IEnumerable<XdmItem> items)
    {
        _items = items?.ToList() ?? new List<XdmItem>();
    }

    /// <summary>
    /// Gets the number of items in the sequence.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Returns true if the sequence is empty.
    /// </summary>
    public bool IsEmpty => _items.Count == 0;

    /// <summary>
    /// Returns true if the sequence contains exactly one item.
    /// </summary>
    public bool IsSingleton => _items.Count == 1;

    /// <summary>
    /// Gets the item at the specified index (0-based).
    /// </summary>
    public XdmItem this[int index] => _items[index];

    /// <summary>
    /// Gets the first item in the sequence, or null if empty.
    /// </summary>
    public XdmItem? First => _items.Count > 0 ? _items[0] : null;

    /// <summary>
    /// Gets the last item in the sequence, or null if empty.
    /// </summary>
    public XdmItem? Last => _items.Count > 0 ? _items[^1] : null;

    /// <summary>
    /// Returns the single item if sequence has exactly one item.
    /// Throws if sequence is empty or has more than one item.
    /// </summary>
    public XdmItem Single()
    {
        return _items.Count switch
        {
            0 => throw new InvalidOperationException("Sequence is empty"),
            1 => _items[0],
            _ => throw new InvalidOperationException($"Sequence has {_items.Count} items, expected 1")
        };
    }

    /// <summary>
    /// Returns the single item if sequence has exactly one item, or null if empty.
    /// Throws if sequence has more than one item.
    /// </summary>
    public XdmItem? SingleOrDefault()
    {
        return _items.Count switch
        {
            0 => null,
            1 => _items[0],
            _ => throw new InvalidOperationException($"Sequence has {_items.Count} items, expected 0 or 1")
        };
    }

    /// <summary>
    /// Gets the effective boolean value of this sequence.
    /// </summary>
    public bool EffectiveBooleanValue
    {
        get
        {
            if (_items.Count == 0)
                return false;

            var first = _items[0];

            if (first.IsNode)
                return true;

            if (_items.Count > 1)
                throw new XdmException("FORG0006", "Effective boolean value not defined for sequence of more than one item unless first item is a node");

            return first.EffectiveBooleanValue;
        }
    }

    /// <summary>
    /// Atomizes all items in the sequence.
    /// </summary>
    public XdmSequence Atomize()
    {
        var atomized = new List<XdmItem>();
        foreach (var item in _items)
        {
            var itemAtomized = item.Atomize();
            atomized.AddRange(itemAtomized);
        }
        return new XdmSequence(atomized);
    }

    /// <summary>
    /// Gets the string value of the sequence.
    /// </summary>
    public string StringValue
    {
        get
        {
            if (_items.Count == 0)
                return string.Empty;
            if (_items.Count == 1)
                return _items[0].StringValue;
            throw new XdmException("XPTY0004", "Cannot get string value of sequence with more than one item");
        }
    }

    /// <summary>
    /// Concatenates this sequence with another sequence.
    /// </summary>
    public XdmSequence Concat(XdmSequence other)
    {
        if (other is null || other.IsEmpty)
            return this;
        if (IsEmpty)
            return other;

        var combined = new List<XdmItem>(_items.Count + other._items.Count);
        combined.AddRange(_items);
        combined.AddRange(other._items);
        return new XdmSequence(combined);
    }

    /// <summary>
    /// Returns a subsequence starting at the given position (1-based).
    /// </summary>
    public XdmSequence Subsequence(int startingLoc)
    {
        if (startingLoc < 1)
            startingLoc = 1;
        if (startingLoc > _items.Count)
            return Empty;

        return new XdmSequence(_items.Skip(startingLoc - 1));
    }

    /// <summary>
    /// Returns a subsequence starting at the given position (1-based) with the given length.
    /// </summary>
    public XdmSequence Subsequence(int startingLoc, int length)
    {
        if (startingLoc < 1)
        {
            length += startingLoc - 1;
            startingLoc = 1;
        }
        if (length <= 0 || startingLoc > _items.Count)
            return Empty;

        return new XdmSequence(_items.Skip(startingLoc - 1).Take(length));
    }

    /// <summary>
    /// Reverses the sequence.
    /// </summary>
    public XdmSequence Reverse()
    {
        if (_items.Count <= 1)
            return this;

        var reversed = new List<XdmItem>(_items);
        reversed.Reverse();
        return new XdmSequence(reversed);
    }

    /// <summary>
    /// Returns the distinct values in the sequence.
    /// </summary>
    public XdmSequence Distinct()
    {
        if (_items.Count <= 1)
            return this;

        var distinct = new List<XdmItem>();
        var seen = new HashSet<XdmItem>();

        foreach (var item in _items)
        {
            if (seen.Add(item))
                distinct.Add(item);
        }

        return new XdmSequence(distinct);
    }

    /// <summary>
    /// Inserts items at the specified position (1-based).
    /// </summary>
    public XdmSequence InsertBefore(int position, XdmSequence inserts)
    {
        if (inserts is null || inserts.IsEmpty)
            return this;

        var result = new List<XdmItem>(_items.Count + inserts.Count);

        int insertIndex = Math.Max(0, Math.Min(position - 1, _items.Count));

        result.AddRange(_items.Take(insertIndex));
        result.AddRange(inserts);
        result.AddRange(_items.Skip(insertIndex));

        return new XdmSequence(result);
    }

    /// <summary>
    /// Removes the item at the specified position (1-based).
    /// </summary>
    public XdmSequence Remove(int position)
    {
        if (position < 1 || position > _items.Count)
            return this;

        var result = new List<XdmItem>(_items.Count - 1);
        result.AddRange(_items.Take(position - 1));
        result.AddRange(_items.Skip(position));
        return new XdmSequence(result);
    }

    /// <summary>
    /// Gets all items as a specific type.
    /// </summary>
    public IEnumerable<T> ItemsAs<T>() where T : XdmItem
    {
        return _items.OfType<T>();
    }

    public IEnumerator<XdmItem> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(XdmSequence? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_items.Count != other._items.Count) return false;

        for (int i = 0; i < _items.Count; i++)
        {
            if (!_items[i].Equals(other._items[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is XdmSequence seq && Equals(seq);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var item in _items)
            hash.Add(item);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        if (_items.Count == 0)
            return "()";
        if (_items.Count == 1)
            return _items[0].ToString() ?? string.Empty;
        return $"({string.Join(", ", _items)})";
    }

    public static XdmSequence operator +(XdmSequence left, XdmSequence right) =>
        left.Concat(right);

    /// <summary>
    /// Creates a sequence from a single item.
    /// </summary>
    public static implicit operator XdmSequence(XdmItem item) => new(item);

    /// <summary>
    /// Creates a sequence from multiple items.
    /// </summary>
    public static XdmSequence Of(params XdmItem[] items) => new(items);

    /// <summary>
    /// Creates a sequence of integers in a range.
    /// </summary>
    public static XdmSequence Range(long start, long end)
    {
        if (start > end)
            return Empty;

        var items = new List<XdmItem>();
        for (long i = start; i <= end; i++)
            items.Add(new XdmAtomicValue(i));

        return new XdmSequence(items);
    }
}
