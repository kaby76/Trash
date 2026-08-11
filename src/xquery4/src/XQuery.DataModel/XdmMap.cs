using System.Collections;

namespace XQuery.DataModel;

/// <summary>
/// Represents a map item in the XDM 4.0 data model.
/// Maps are ordered collections of key-value pairs with unique keys.
/// In XDM 4.0, maps preserve insertion order.
/// </summary>
public class XdmMap : XdmItem, IEnumerable<KeyValuePair<XdmAtomicValue, XdmSequence>>
{
    // Use a list to maintain insertion order (XDM 4.0 feature)
    private readonly List<KeyValuePair<XdmAtomicValue, XdmSequence>> _entries = new();

    public override bool IsMap => true;
    public override bool IsFunction => true; // Maps are functions

    public override XdmQName TypeName => new XdmQName(XdmQName.MapNamespace, "map", "map");

    /// <summary>
    /// Gets the number of entries in the map.
    /// </summary>
    public int Count => _entries.Count;

    /// <summary>
    /// Returns true if the map is empty.
    /// </summary>
    public bool IsEmpty => _entries.Count == 0;

    /// <summary>
    /// Gets all keys in the map.
    /// </summary>
    public IEnumerable<XdmAtomicValue> Keys => _entries.Select(e => e.Key);

    /// <summary>
    /// Gets all values in the map.
    /// </summary>
    public IEnumerable<XdmSequence> Values => _entries.Select(e => e.Value);

    /// <summary>
    /// Gets the value associated with the given key.
    /// </summary>
    public XdmSequence? Get(XdmAtomicValue key)
    {
        foreach (var entry in _entries)
        {
            if (entry.Key.ValueEquals(key))
                return entry.Value;
        }
        return null;
    }

    /// <summary>
    /// Gets the value at the given key, or the empty sequence if not found.
    /// </summary>
    public XdmSequence this[XdmAtomicValue key] => Get(key) ?? XdmSequence.Empty;

    /// <summary>
    /// Returns true if the map contains the given key.
    /// </summary>
    public bool ContainsKey(XdmAtomicValue key)
    {
        return _entries.Any(e => e.Key.ValueEquals(key));
    }

    /// <summary>
    /// Adds or replaces an entry in the map. Returns a new map.
    /// </summary>
    public XdmMap Put(XdmAtomicValue key, XdmSequence value)
    {
        var newMap = new XdmMap();
        bool replaced = false;

        foreach (var entry in _entries)
        {
            if (entry.Key.ValueEquals(key))
            {
                newMap._entries.Add(new KeyValuePair<XdmAtomicValue, XdmSequence>(key, value));
                replaced = true;
            }
            else
            {
                newMap._entries.Add(entry);
            }
        }

        if (!replaced)
            newMap._entries.Add(new KeyValuePair<XdmAtomicValue, XdmSequence>(key, value));

        return newMap;
    }

    /// <summary>
    /// Removes an entry from the map. Returns a new map.
    /// </summary>
    public XdmMap Remove(XdmAtomicValue key)
    {
        var newMap = new XdmMap();
        foreach (var entry in _entries)
        {
            if (!entry.Key.ValueEquals(key))
                newMap._entries.Add(entry);
        }
        return newMap;
    }

    /// <summary>
    /// Merges this map with another map. Entries from the other map override entries in this map.
    /// Returns a new map.
    /// </summary>
    public XdmMap Merge(XdmMap other)
    {
        var newMap = new XdmMap();

        // Add all entries from this map that aren't in other
        foreach (var entry in _entries)
        {
            if (!other.ContainsKey(entry.Key))
                newMap._entries.Add(entry);
        }

        // Add all entries from other
        foreach (var entry in other._entries)
        {
            newMap._entries.Add(entry);
        }

        return newMap;
    }

    /// <summary>
    /// Returns an empty map.
    /// </summary>
    public static XdmMap Empty => new();

    public override string StringValue =>
        throw new XdmException("FOTY0014", "Maps do not have a string value");

    public override XdmSequence TypedValue =>
        throw new XdmException("FOTY0014", "Maps do not have a typed value");

    public override bool EffectiveBooleanValue =>
        throw new XdmException("FORG0006", "Cannot compute effective boolean value of a map");

    public override bool Equals(XdmItem? other)
    {
        if (other is not XdmMap otherMap) return false;
        return DeepEquals(otherMap);
    }

    /// <summary>
    /// Deep equality comparison.
    /// </summary>
    public bool DeepEquals(XdmMap other)
    {
        if (other is null) return false;
        if (_entries.Count != other._entries.Count) return false;

        foreach (var entry in _entries)
        {
            var otherValue = other.Get(entry.Key);
            if (otherValue is null) return false;
            if (!entry.Value.Equals(otherValue)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var entry in _entries)
        {
            hash.Add(entry.Key);
            hash.Add(entry.Value);
        }
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        if (_entries.Count == 0)
            return "map{}";

        var entries = _entries.Select(e => $"{e.Key}: {e.Value}");
        return $"map{{{string.Join(", ", entries)}}}";
    }

    public IEnumerator<KeyValuePair<XdmAtomicValue, XdmSequence>> GetEnumerator() =>
        _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Creates a map from key-value pairs.
    /// </summary>
    public static XdmMap Of(params (XdmAtomicValue key, XdmSequence value)[] entries)
    {
        var map = new XdmMap();
        foreach (var (key, value) in entries)
        {
            if (map.ContainsKey(key))
                throw new XdmException("XQDY0137", $"Duplicate key in map constructor: {key}");
            map._entries.Add(new KeyValuePair<XdmAtomicValue, XdmSequence>(key, value));
        }
        return map;
    }

    /// <summary>
    /// Creates a mutable map builder.
    /// </summary>
    public static MapBuilder Builder() => new();

    public class MapBuilder
    {
        private readonly XdmMap _map = new();

        public MapBuilder Add(XdmAtomicValue key, XdmSequence value)
        {
            if (_map.ContainsKey(key))
                throw new XdmException("XQDY0137", $"Duplicate key in map: {key}");
            _map._entries.Add(new KeyValuePair<XdmAtomicValue, XdmSequence>(key, value));
            return this;
        }

        public MapBuilder Add(string key, XdmSequence value) =>
            Add(new XdmAtomicValue(key), value);

        public MapBuilder Add(string key, string value) =>
            Add(new XdmAtomicValue(key), new XdmSequence(new XdmAtomicValue(value)));

        public MapBuilder Add(string key, long value) =>
            Add(new XdmAtomicValue(key), new XdmSequence(new XdmAtomicValue(value)));

        public MapBuilder Add(string key, bool value) =>
            Add(new XdmAtomicValue(key), new XdmSequence(new XdmAtomicValue(value)));

        public XdmMap Build() => _map;
    }
}
