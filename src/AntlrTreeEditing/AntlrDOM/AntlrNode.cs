namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;
    using System;

    public class AntlrNode : Node, IAntlrObserver
    {
        private AntlrNode() { }
        public AntlrNode(IParseTree n) { AntlrIParseTree = n; }
        public IParseTree AntlrIParseTree { get; set; }
        public short NodeType { get; set; }
        public virtual string LocalName { get; set; }
        public virtual Document OwnerDocument { get; set; }
        public virtual NodeList ChildNodes { get; set; }
        public virtual Node NextSibling { get; set; }
        public virtual string BaseURI { get; set; }
        public virtual NamedNodeMap Attributes { get; set; }
        public virtual object NodeValue { get; set; }
        public virtual string NamespaceURI { get; set; }
        public virtual object NodeName { get; set; }
        public virtual Node ParentNode { get; set; }
        public virtual Node PreviousSibling { get; set; }
        public virtual bool isSameNode(Node nodeValue)
        {
            throw new NotImplementedException();
        }

        public virtual short compareDocumentPosition(Node nodeB)
        {
            throw new NotImplementedException();
        }

        public bool isEqualNode(Node node)
        {
            throw new NotImplementedException();
        }

        public virtual bool hasChildNodes()
        {
            throw new NotImplementedException();
        }

        public virtual bool hasAttributes()
        {
            throw new NotImplementedException();
        }

        public virtual void OnParentDisconnect(IParseTree value)
        {
            if (ParentNode != null)
            {
                AntlrNodeList children = ParentNode.ChildNodes as AntlrNodeList;
                children.Delete(this);
            }
            ParentNode = null;
        }

        public virtual void OnParentConnect(IParseTree value)
        {
        }

        public virtual void OnChildDisconnect(IParseTree value)
        {
        }

        public virtual void OnChildConnect(IParseTree value)
        {
        }

        public virtual void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNext(ObserverParserRuleContext value)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            var tree = AntlrIParseTree as ObserverParserRuleContext;
            if (tree != null)
            {
                tree.Unsubscribe(this);
            }
            if (ChildNodes != null)
            {
                for (int i = 0; i < ChildNodes.Length; ++i)
                {
                    Node c = ChildNodes.item(i);
                    var cc = c as AntlrNode;
                    cc?.Dispose();
                }
            }
        }
    }
}
