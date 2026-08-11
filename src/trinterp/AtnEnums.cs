namespace trinterp;

public enum ATNType { Lexer = 0, Parser = 1 }

public enum StateType
{
    InvalidType = 0,
    Basic = 1,
    RuleStart = 2,
    BlockStart = 3,
    PlusBlockStart = 4,
    StarBlockStart = 5,
    TokenStart = 6,
    RuleStop = 7,
    BlockEnd = 8,
    StarLoopBack = 9,
    StarLoopEntry = 10,
    PlusLoopBack = 11,
    LoopEnd = 12
}

public enum TransitionType
{
    EPSILON = 1,
    RANGE = 2,
    RULE = 3,
    PREDICATE = 4,
    ATOM = 5,
    ACTION = 6,
    SET = 7,
    NOT_SET = 8,
    WILDCARD = 9,
    PRECEDENCE = 10
}

public enum LexerActionType
{
    Channel = 0,
    Custom = 1,
    Mode = 2,
    More = 3,
    PopMode = 4,
    PushMode = 5,
    Skip = 6,
    Type = 7
}
