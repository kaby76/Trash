using Antlr4.Runtime.Misc;

namespace AltAntlr
{
    interface IMyParseTree
    {
        void ComputeSourceInterval(ref int start);
        void Reset();
        Interval SourceInterval { get; }
    }
}
