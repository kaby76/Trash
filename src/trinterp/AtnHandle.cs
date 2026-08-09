namespace trinterp;

/// <summary>
/// A fragment of the ATN graph: a (left, right) state pair.  Mirrors the
/// IStatePair / Handle used in antlr-ng's ParserATNFactory.
/// </summary>
public class AtnHandle
{
    public ATNState Left;
    public ATNState Right;

    public AtnHandle(ATNState left, ATNState right)
    {
        Left = left;
        Right = right;
    }
}
