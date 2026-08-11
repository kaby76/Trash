namespace Trash.EarleyAtn;

public enum MyStateType
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
