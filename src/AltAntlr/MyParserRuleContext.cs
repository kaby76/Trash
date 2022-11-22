namespace AltAntlr
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;

    public class MyParserRuleContext : ParserRuleContext, IMyParseTree
    {
        public MyParserRuleContext(ParserRuleContext parent, int invokingStateNumber) : base(parent, invokingStateNumber)
        {
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

        MyInterval _source_interval { get; set; }
        public override Interval SourceInterval
        {
            get
            {
                if (_source_interval == null)
                {
                    var start = (this.TokenStream as MyTokenStream)._tokens.Count > 0 ? 0 : -1;
                    this.ComputeSourceInterval(ref start);
                }
                return new Interval(this._source_interval.a, this._source_interval.b);
            }
        }

        public void Reset() { this._source_interval = null; }

        public void ComputeSourceInterval(ref int start)
        {
            if (_source_interval != null)
            {
                return;
            }
            var mi = new MyInterval();
            if (this.children == null || this.children.Count == 0)
            {
                mi.a = start;
                mi.b = start - 2;
            }
            else
            {
                for (int i = 0; i < this.children.Count; ++i)
                {
                    var child = this.children[i] as IMyParseTree;
                    child.ComputeSourceInterval(ref start);
                    if (i == 0)
                    {
                        if (child is MyTerminalNodeImpl tt)
                        {
                            mi.a = child.SourceInterval.a;
                        }
                        else if (child is MyParserRuleContext pp)
                        {
                            mi.a = pp._source_interval.a;
                        }
                    }
                    if (child is MyTerminalNodeImpl tt2)
                    {
                        mi.b = child.SourceInterval.b;
                    }
                    else if (child is MyParserRuleContext pp2)
                    {
                        mi.b = pp2._source_interval.b;
                    }
                }
            }
            _source_interval = mi;
        }

        public MyCharStream InputStream { get; set; }
        public MyLexer Lexer { get; set; }
        public MyParser Parser { get; set; }
        public MyTokenStream TokenStream { get; set; }
    }
}
