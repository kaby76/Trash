namespace Trash.EarleyAtn;

// Lightweight event produced by Earley and ALL(*) parsers.
// No Antlr4.Runtime.Standard types used.
//
// Left-recursive rule events (mirrors ANTLR4 ParserInterpreter behaviour):
//   EnterRecursionRule  – enter a left-recursive rule; do NOT add to parent yet.
//   PushRecursionContext – start the next suffix-loop iteration: wrap the accumulated
//                          context as the first child of a fresh rule element.
//   ExitRecursionRule   – left-recursive rule finished; add accumulated element to parent.
public enum ParseEventKind
{
    EnterRule,
    ExitRule,
    Consume,
    EnterRecursionRule,
    ExitRecursionRule,
    PushRecursionContext
}

public readonly struct ParseEvent
{
    public ParseEventKind Kind { get; }
    // EnterRule/ExitRule/EnterRecursionRule/ExitRecursionRule/PushRecursionContext: rule index.
    // Consume: index into the all-channel LexerToken list.
    public int Index { get; }

    private ParseEvent(ParseEventKind kind, int index) { Kind = kind; Index = index; }

    public static ParseEvent EnterRule(int ruleIndex)           => new(ParseEventKind.EnterRule,           ruleIndex);
    public static ParseEvent ExitRule(int ruleIndex)            => new(ParseEventKind.ExitRule,            ruleIndex);
    public static ParseEvent Consume(int tokenIndex)            => new(ParseEventKind.Consume,             tokenIndex);
    public static ParseEvent EnterRecursionRule(int ruleIndex)  => new(ParseEventKind.EnterRecursionRule,  ruleIndex);
    public static ParseEvent ExitRecursionRule(int ruleIndex)   => new(ParseEventKind.ExitRecursionRule,   ruleIndex);
    public static ParseEvent PushRecursionContext(int ruleIndex)=> new(ParseEventKind.PushRecursionContext, ruleIndex);
}
