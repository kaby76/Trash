namespace Trash.EarleyAtn;

// Lightweight event produced by Earley and ALL(*) parsers.
// No Antlr4.Runtime.Standard types used.
public enum ParseEventKind { EnterRule, ExitRule, Consume }

public readonly struct ParseEvent
{
    public ParseEventKind Kind { get; }
    // EnterRule/ExitRule: grammar rule index.
    // Consume: index into the all-channel LexerToken list (LexerToken.TokenIndex).
    public int Index { get; }

    private ParseEvent(ParseEventKind kind, int index) { Kind = kind; Index = index; }

    public static ParseEvent EnterRule(int ruleIndex) => new(ParseEventKind.EnterRule, ruleIndex);
    public static ParseEvent ExitRule(int ruleIndex)  => new(ParseEventKind.ExitRule,  ruleIndex);
    public static ParseEvent Consume(int tokenIndex)  => new(ParseEventKind.Consume,   tokenIndex);
}
