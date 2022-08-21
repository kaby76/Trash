namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;

    public class AntlrAttr : AntlrNode, Attr
    {
        private AntlrAttr() : base(null) { }
        public AntlrAttr(IParseTree n) : base(n) { }
        public string Prefix { get; set; }
        public object Name { get; set; }
        public string Value { get; set; }
        public Node OwnerElement { get; set; }
        public TypeInfo SchemaTypeInfo { get; set; }
    }
}
