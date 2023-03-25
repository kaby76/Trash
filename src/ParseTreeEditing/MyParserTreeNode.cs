namespace EditableAntlrTree
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;
    using System.Linq;

    public class MyParserTreeNode
    {
        public MyParserTreeNode(MyParserTreeNode parent)
        {
            Children = new List<MyParserTreeNode>();
        }

        public List<MyParserTreeNode> Children { get; }

        public int _ruleIndex;
        public int RuleIndex
        {
            get { return _ruleIndex; }
        }
        public List<MyToken> Before { get; set; }
        public List<MyToken> After { get; set; }
        public MyToken Terminal { get; set; }
        public MyParserTreeNode LeftMost(MyParserTreeNode node)
        {
            if (node != null)
            {
                if (node.Children.Count > 0)
                    return LeftMost(node.Children.First());
                else
                    return node;
            }
            return null;
        }
        public MyParserTreeNode RightMost(MyParserTreeNode node)
        {
            if (node != null)
            {
                if (node.Children.Count > 0)
                    return RightMost(node.Children.Last());
                else
                    return node;
            }
            return null;
        }
        public MyLexer Lexer { get; set; }
        public MyParser Parser { get; set; }
        public IEnumerable<MyToken> AllTokens { get; set; }
    }
}
