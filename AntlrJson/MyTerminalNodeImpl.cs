namespace AntlrJson
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    public class MyTerminalNodeImpl : TerminalNodeImpl
    {
        public MyTerminalNodeImpl(IToken symbol) : base(symbol)
        {
        }

        public Interval _sourceInterval;
        public override Interval SourceInterval
        {
            get { return _sourceInterval; }
        }
    }
}
