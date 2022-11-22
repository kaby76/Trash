using Antlr4.Runtime.Misc;

namespace EditableAntlrTree
{
    public interface IMyParseTree
    {
        void ComputeSourceInterval(ref int start);
        void Reset();
        Interval SourceInterval { get; }
    }
}
