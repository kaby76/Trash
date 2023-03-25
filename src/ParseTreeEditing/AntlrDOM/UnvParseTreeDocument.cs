namespace ParseTreeEditing.ParseTreeDOM
{
    using org.w3c.dom;
    using System;

    public class UnvParseTreeDocument : UnvParseTreeNode, Document
    {
        public UnvParseTreeDocument()
        {
            this.NodeType = NodeConstants.DOCUMENT_NODE;
        }
        public string DocumentURI { get; set; }
        public NodeList getElementsByTagNameNS(string ns, string local)
        {
            throw new NotImplementedException();
        }

        public bool isSupported(string core, string s)
        {
            throw new NotImplementedException();
        }
    }
}
