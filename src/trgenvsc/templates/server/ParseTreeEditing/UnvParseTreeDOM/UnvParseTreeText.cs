namespace ParseTreeEditing.UnvParseTreeDOM
{
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;

    public class UnvParseTreeText : UnvParseTreeNode, Text
    {
        public UnvParseTreeText() : base()
        {
            this.NodeType = NodeConstants.TEXT_NODE;
        }
        public string Data { get; set; }
        public override object NodeValue { get { return Data; } set { Data = (string)value; } }
        public int TokenType { get; set; }
        public int Channel { get; set; }
    }
}
