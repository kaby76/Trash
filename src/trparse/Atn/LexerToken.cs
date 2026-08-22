namespace Atn;

public struct LexerToken
{
    /// <summary>Sentinel channel value for tokens matched by a 'skip' lexer action.</summary>
	public const int SKIP_CHANNEL = -2;

	public int Type;
	public int Channel;
	public string Text;
	public int StartIndex;
	public int StopIndex; // inclusive
	public int Line;
	public int Column;
	public int TokenIndex; // index into full token list (all channels)
}

