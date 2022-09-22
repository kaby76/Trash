namespace AltAntlr
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    public class MyParserRuleContext : ParserRuleContext
    {
        public MyParserRuleContext(ParserRuleContext parent, int invokingStateNumber) : base(parent, invokingStateNumber)
        {
            //_sourceInterval = new Interval(0, 0);
        }

        public int _ruleIndex;
        public override int RuleIndex
        {
            get { return _ruleIndex; }
        }

        private IParseTree LeftMost(IParseTree node)
        {
            var n = node as MyParserRuleContext;
            if (n != null)
            {
                if (n.children != null && n.children.Count > 0)
                    return LeftMost(n.children[0]);
                else
                    return n;
            }
            var l = node as MyTerminalNodeImpl;
            if (l != null)
            {
                return l;
            }
            return null;
        }

        private IParseTree RightMost(IParseTree node)
        {
            var n = node as MyParserRuleContext;
            if (n != null)
            {
                if (n.children != null && n.children.Count > 0)
                    return RightMost(n.children[n.ChildCount-1]);
                else
                    return n;
            }
            var l = node as MyTerminalNodeImpl;
            if (l != null)
            {
                return l;
            }
            return null;
        }

        public override Interval SourceInterval
        {
            get
            {
                // This is always just a computed value.
                var lm = LeftMost(this);
                int lmi = lm is AltAntlr.MyParserRuleContext ? -1 : (lm as AltAntlr.MyTerminalNodeImpl).Payload.TokenIndex;
                var rm = RightMost(this);
                int rmi = rm is AltAntlr.MyParserRuleContext ? -2 : (rm as AltAntlr.MyTerminalNodeImpl).Payload.TokenIndex;
                return new Interval(lmi, rmi);
                //return _sourceInterval;
            }
        }

        public MyCharStream InputStream { get; set; }
        public MyLexer Lexer { get; set; }
        public MyParser Parser { get; set; }
        public MyTokenStream TokenStream { get; set; }
    }
}
