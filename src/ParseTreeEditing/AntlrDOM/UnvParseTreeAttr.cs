namespace ParseTreeEditing.ParseTreeDOM
{
    using org.w3c.dom;

    public class UnvParseTreeAttr : UnvParseTreeNode, Attr
    {
        public UnvParseTreeAttr()
        {
            this.NodeType = NodeConstants.ATTRIBUTE_NODE;
        }
        public UnvParseTreeAttr(UnvParseTreeAttr c)
        {
            this.ParentNode = c.ParentNode;
            this.Prefix= c.Prefix;
            this.LocalName = c.LocalName;
            this.NamespaceURI = c.NamespaceURI;
            this.StringValue = c.StringValue;
            this.Attributes = c.Attributes;
            this.NodeType = c.NodeType;
            this.Name = c.Name;
            this.OwnerElement = c.OwnerElement;
            this.SchemaTypeInfo = c.SchemaTypeInfo;
        }
        public string Prefix { get; set; }
        public object Name { get; set; }
        public string StringValue { get; set; }
        public Node OwnerElement { get; set; }
        public TypeInfo SchemaTypeInfo { get; set; }
        public object Value
        {
            get { return StringValue; }
            set { StringValue = value as string; }
        }
        public int TokenType { get; set; }
        public int Channel { get; set; }
    }
}
