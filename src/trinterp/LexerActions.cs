namespace trinterp;

public interface ILexerAction
{
    LexerActionType ActionType { get; }
    bool IsPositionDependent { get; }
}

public sealed class LexerSkipAction : ILexerAction
{
    public static readonly LexerSkipAction Instance = new();
    private LexerSkipAction() { }
    public LexerActionType ActionType => LexerActionType.Skip;
    public bool IsPositionDependent => false;
}

public sealed class LexerMoreAction : ILexerAction
{
    public static readonly LexerMoreAction Instance = new();
    private LexerMoreAction() { }
    public LexerActionType ActionType => LexerActionType.More;
    public bool IsPositionDependent => false;
}

public sealed class LexerPopModeAction : ILexerAction
{
    public static readonly LexerPopModeAction Instance = new();
    private LexerPopModeAction() { }
    public LexerActionType ActionType => LexerActionType.PopMode;
    public bool IsPositionDependent => false;
}

public sealed class LexerModeAction : ILexerAction
{
    public readonly int Mode;
    public LexerModeAction(int mode) { Mode = mode; }
    public LexerActionType ActionType => LexerActionType.Mode;
    public bool IsPositionDependent => false;
}

public sealed class LexerPushModeAction : ILexerAction
{
    public readonly int Mode;
    public LexerPushModeAction(int mode) { Mode = mode; }
    public LexerActionType ActionType => LexerActionType.PushMode;
    public bool IsPositionDependent => false;
}

public sealed class LexerTypeAction : ILexerAction
{
    public readonly int Type;
    public LexerTypeAction(int type) { Type = type; }
    public LexerActionType ActionType => LexerActionType.Type;
    public bool IsPositionDependent => false;
}

public sealed class LexerChannelAction : ILexerAction
{
    public readonly int Channel;
    public LexerChannelAction(int channel) { Channel = channel; }
    public LexerActionType ActionType => LexerActionType.Channel;
    public bool IsPositionDependent => false;
}

public sealed class LexerCustomAction : ILexerAction
{
    public readonly int RuleIndex;
    public readonly int ActionIndex;
    public LexerCustomAction(int ruleIndex, int actionIndex) { RuleIndex = ruleIndex; ActionIndex = actionIndex; }
    public LexerActionType ActionType => LexerActionType.Custom;
    public bool IsPositionDependent => true;
}
