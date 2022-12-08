namespace AntlrTreeEditing.AntlrDOM
{
    using org.w3c.dom;
    using System;

    public abstract class AntlrNode : Node, IAntlrObserver
    {
        public AntlrNode() { }
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

        public virtual void OnParentDisconnect(AntlrNode value)
        {
            if (ParentNode != null)
            {
                AntlrNodeList children = ParentNode.ChildNodes as AntlrNodeList;
                children.Delete(this);
            }
            ParentNode = null;
        }

        public virtual void OnParentConnect(AntlrNode value)
        {
        }

        public virtual void OnChildDisconnect(AntlrNode value)
        {
        }

        public virtual void OnChildConnect(AntlrNode value)
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

        public int _ruleIndex;
        public int RuleIndex
        {
            get { return _ruleIndex; }
        }

        private AntlrNode LeftMost(AntlrNode node)
        {
            if (node != null)
            {
                if (node.ChildNodes != null && node.ChildNodes.Length > 0)
                    return LeftMost(node.ChildNodes.item(0) as AntlrNode);
                else
                    return node;
            }
            return null;
        }

        private AntlrNode RightMost(AntlrNode node)
        {
            if (node != null)
            {
                if (node.ChildNodes != null && node.ChildNodes.Length > 0)
                    return RightMost(node.ChildNodes.item(node.ChildNodes.Length-1) as AntlrNode);
                else
                    return node;
            }
            return null;
        }
    }
}
