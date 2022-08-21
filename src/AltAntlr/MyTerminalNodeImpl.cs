namespace AltAntlr
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

        /// <summary>
        /// Start token stream index (not char stream index).
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// Stop token stream index (not char stream index).
        /// </summary>
        public int Stop { get; set; }

        public MyCharStream InputStream { get; set; }
        public MyLexer Lexer { get; set; }
        public MyParser Parser { get; set; }
        public MyTokenStream TokenStream { get; set; }
    }
}
