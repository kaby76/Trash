namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;

    public class AntlrText : AntlrNode, Text
    {
        public AntlrText() : base() { }
        public string Data { get; set; }
        public override object NodeValue { get { return Data; } set { Data = (string)value; } }
    }
}
