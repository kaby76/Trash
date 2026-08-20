namespace Trash.EarleyAtn;

public enum MyATNType { Lexer = 0, Parser = 1 }

public enum MyLexerActionType
{
    Channel = 0, Custom = 1, Mode = 2, More = 3, PopMode = 4, PushMode = 5, Skip = 6, Type = 7
}

public interface IMyLexerAction
{
    MyLexerActionType ActionType { get; }
    int Arg1 { get; }
    int Arg2 { get; }
}

public class MyLexerAction : IMyLexerAction
{
    public MyLexerActionType ActionType { get; }
    public int Arg1 { get; }
    public int Arg2 { get; }

    public MyLexerAction(MyLexerActionType type, int arg1, int arg2)
    {
        ActionType = type; Arg1 = arg1; Arg2 = arg2;
    }
}

public class MyATN
{
    public MyATNType grammarType;
    public int maxTokenType;
    public MyATNState[] allStates = Array.Empty<MyATNState>();

    // start[ruleIndex] = rule start state (used by EarleyParser and LexerAtnSimulator)
    public MyATNState[] start = Array.Empty<MyATNState>();
    public MyATNState[] ruleToStopState = Array.Empty<MyATNState>();

    // Lexer-specific
    public int[] ruleToTokenType = Array.Empty<int>();
    public MyATNState[] modeToStartState = Array.Empty<MyATNState>();
    public IMyLexerAction[] lexerActions = Array.Empty<IMyLexerAction>();

    // Decisions
    public MyATNState[] decisionToState = Array.Empty<MyATNState>();

    // Set of all RuleStop states (used by EarleyParser for completion check)
    public HashSet<MyATNState> stop = new();
}
