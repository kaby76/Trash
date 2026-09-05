namespace Atn;

public sealed class LexerToken
{
    /// <summary>Sentinel channel value for tokens matched by a 'skip' lexer action.</summary>
	public const int SKIP_CHANNEL = -2;

	public int Type;
	public int Channel;
	private readonly string _source;
	private string _text;

	public LexerToken()
	{
	}

	internal LexerToken(string source)
	{
		_source = source;
	}

	/// <summary>
	/// Token text, materialized from the shared source only when requested.
	/// Explicitly assigned text (including EOF) takes precedence.
	/// </summary>
	public string Text
	{
		get
		{
			if (_text != null) return _text;
			if (_source == null || StopIndex < StartIndex) return _text = "";
			return _text = _source.Substring(StartIndex, StopIndex - StartIndex + 1);
		}
		set => _text = value;
	}

	internal bool IsTextMaterialized => _text != null;
	public int StartIndex;
	public int StopIndex; // inclusive
	public int Line;
	public int Column;
	public int TokenIndex; // index into full token list (all channels)
}
