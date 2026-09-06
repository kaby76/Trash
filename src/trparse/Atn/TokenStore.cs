namespace Atn;

using System.Collections;

/// <summary>
/// Append-only, source-backed token storage. Token metadata lives in parallel
/// primitive arrays instead of one managed object per token.
/// </summary>
public sealed class TokenStore : IReadOnlyList<LexerToken>
{
    private const int MinimumCapacity = 16;
    private readonly string _source;
    private int[] _types;
    private int[] _channels;
    private int[] _starts;
    private int[] _stops;
    private int[] _lines;
    private int[] _columns;
    private Dictionary<int, string> _explicitTexts;

    public TokenStore(string source, int initialCapacity = 0)
    {
        _source = source ?? "";
        if (initialCapacity > 0) Allocate(initialCapacity);
    }

    public int Count { get; private set; }

    public LexerToken this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return new LexerToken(this, index);
        }
    }

    public int Add(LexerToken token)
    {
        EnsureCapacity(Count + 1);
        var index = Count++;
        _types[index] = token.Type;
        _channels[index] = token.Channel;
        _starts[index] = token.StartIndex;
        _stops[index] = token.StopIndex;
        _lines[index] = token.Line;
        _columns[index] = token.Column;
        if (token.ExplicitText != null)
            SetExplicitText(index, token.ExplicitText);
        return index;
    }

    internal int GetType(int index) => _types[index];
    internal int GetChannel(int index) => _channels[index];
    internal int GetStart(int index) => _starts[index];
    internal int GetStop(int index) => _stops[index];
    internal int GetLine(int index) => _lines[index];
    internal int GetColumn(int index) => _columns[index];
    internal void SetType(int index, int value) => _types[index] = value;
    internal void SetChannel(int index, int value) => _channels[index] = value;

    internal string GetText(int index)
    {
        if (_explicitTexts != null && _explicitTexts.TryGetValue(index, out var text))
            return text;
        return GetSourceSpan(_starts[index], _stops[index] - _starts[index] + 1).ToString();
    }

    internal ReadOnlySpan<char> GetTextSpan(int index)
    {
        if (_explicitTexts != null && _explicitTexts.TryGetValue(index, out var text))
            return text.AsSpan();
        return GetSourceSpan(_starts[index], _stops[index] - _starts[index] + 1);
    }

    internal ReadOnlySpan<char> GetSourceSpan(int start, int length) =>
        length <= 0 ? ReadOnlySpan<char>.Empty : _source.AsSpan(start, length);

    internal void SetExplicitText(int index, string value)
    {
        (_explicitTexts ??= new Dictionary<int, string>())[index] = value;
    }

    internal bool HasExplicitText(int index) =>
        _explicitTexts != null && _explicitTexts.ContainsKey(index);

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<LexerToken> IEnumerable<LexerToken>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<LexerToken>
    {
        private readonly TokenStore _store;
        private int _index;
        internal Enumerator(TokenStore store) { _store = store; _index = -1; }
        public readonly LexerToken Current => _store[_index];
        readonly object IEnumerator.Current => Current;
        public bool MoveNext() => ++_index < _store.Count;
        public void Reset() => _index = -1;
        public readonly void Dispose() { }
    }

    private void EnsureCapacity(int required)
    {
        if (_types != null && required <= _types.Length) return;
        var capacity = _types == null ? MinimumCapacity : _types.Length * 2;
        if (capacity < required) capacity = required;
        Resize(capacity);
    }

    private void Allocate(int capacity)
    {
        capacity = Math.Max(capacity, MinimumCapacity);
        _types = new int[capacity];
        _channels = new int[capacity];
        _starts = new int[capacity];
        _stops = new int[capacity];
        _lines = new int[capacity];
        _columns = new int[capacity];
    }

    private void Resize(int capacity)
    {
        if (_types == null) { Allocate(capacity); return; }
        Array.Resize(ref _types, capacity);
        Array.Resize(ref _channels, capacity);
        Array.Resize(ref _starts, capacity);
        Array.Resize(ref _stops, capacity);
        Array.Resize(ref _lines, capacity);
        Array.Resize(ref _columns, capacity);
    }
}
