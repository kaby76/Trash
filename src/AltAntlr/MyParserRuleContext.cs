namespace AltAntlr
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    public class MyParserRuleContext : ParserRuleContext
    {
        public MyParserRuleContext(ParserRuleContext parent, int invokingStateNumber) : base(parent, invokingStateNumber)
        {
            _sourceInterval = new Interval(0, 0);
        }

        public int _ruleIndex;
        public override int RuleIndex
        {
            get { return _ruleIndex; }
        }

        public Interval _sourceInterval;
        public override Interval SourceInterval
        {
            get { return _sourceInterval; }
        }

        public MyCharStream InputStream { get; set; }
        public MyLexer Lexer { get; set; }
        public MyParser Parser { get; set; }
        public MyTokenStream TokenStream { get; set; }
    }
}
