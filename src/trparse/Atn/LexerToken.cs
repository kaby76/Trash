namespace Atn;

/// <summary>
/// Lightweight token value. Values returned by <see cref="TokenStore"/> are
/// indexed views over compact storage; independently constructed values remain
/// useful for tests and context-aware lexing.
/// </summary>
public struct LexerToken
{
    public const int SKIP_CHANNEL = -2;

    private TokenStore _store;
    private int _storeIndex;
    private string _source;
    private string _text;
    private int _type;
    private int _channel;
    private int _startIndex;
    private int _stopIndex;
    private int _line;
    private int _column;
    private int _tokenIndex;

    internal LexerToken(TokenStore store, int index)
    {
        _store = store;
        _storeIndex = index;
        _source = null;
        _text = null;
        _type = _channel = _startIndex = _stopIndex = _line = _column = 0;
        _tokenIndex = index;
    }

    internal LexerToken(string source)
    {
        _store = null;
        _storeIndex = -1;
        _source = source;
        _text = null;
        _type = _channel = _startIndex = _stopIndex = _line = _column = 0;
        _tokenIndex = 0;
    }

    public int Type
    {
        readonly get => _store?.GetType(_storeIndex) ?? _type;
        set { if (_store != null) _store.SetType(_storeIndex, value); else _type = value; }
    }

    public int Channel
    {
        readonly get => _store?.GetChannel(_storeIndex) ?? _channel;
        set { if (_store != null) _store.SetChannel(_storeIndex, value); else _channel = value; }
    }

    public int StartIndex
    {
        readonly get => _store?.GetStart(_storeIndex) ?? _startIndex;
        set => _startIndex = value;
    }

    public int StopIndex
    {
        readonly get => _store?.GetStop(_storeIndex) ?? _stopIndex;
        set => _stopIndex = value;
    }

    public int Line
    {
        readonly get => _store?.GetLine(_storeIndex) ?? _line;
        set => _line = value;
    }

    public int Column
    {
        readonly get => _store?.GetColumn(_storeIndex) ?? _column;
        set => _column = value;
    }

    public int TokenIndex
    {
        readonly get => _store != null ? _storeIndex : _tokenIndex;
        set => _tokenIndex = value;
    }

    public string Text
    {
        readonly get
        {
            if (_store != null) return _store.GetText(_storeIndex);
            if (_text != null) return _text;
            if (_source == null || _stopIndex < _startIndex) return "";
            return _source.Substring(_startIndex, _stopIndex - _startIndex + 1);
        }
        set
        {
            if (_store != null) _store.SetExplicitText(_storeIndex, value);
            else _text = value;
        }
    }

    internal readonly bool IsTextMaterialized =>
        _store?.HasExplicitText(_storeIndex) ?? _text != null;
    internal readonly string ExplicitText => _text;
}
