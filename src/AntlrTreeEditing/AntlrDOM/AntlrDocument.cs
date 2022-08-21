namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;
    using System;

    public class AntlrDocument : AntlrNode, Document
    {
        public AntlrDocument(IParseTree n) : base(n) { }
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
