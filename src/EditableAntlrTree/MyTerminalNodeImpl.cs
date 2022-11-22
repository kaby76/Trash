namespace EditableAntlrTree
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    public class MyTerminalNodeImpl : TerminalNodeImpl, IMyParseTree
    {
        public MyTerminalNodeImpl(IToken symbol) : base(symbol)
        {
        }

        public void Reset() { }

        public void ComputeSourceInterval(ref int start)
        {
            start = this.Payload.TokenIndex + 1;
            if (this.TokenStream is MyTokenStream ts)
            {
                for (; ; )
                {
                    if (start >= ts._tokens.Count) break;
                    var tok = ts.Get(start);
                    if (tok == null) break;
                    if (tok.Type == TokenConstants.EOF) break;
                    if (tok.Channel == TokenConstants.DefaultChannel)
                    {
                        break;
                    }
                    start++;
                }
            }
        }

        public override Interval SourceInterval
        {
            get
            {
                var t = this.Payload.TokenIndex;
                return new Interval(t, t);
            }
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
