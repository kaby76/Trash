﻿using System.Collections.Generic;
using System.Linq;

namespace ParseTreeEditing.UnvParseTreeDOM
{
    using org.w3c.dom;
    using System;

    public abstract class UnvParseTreeNode : Node, IAntlrObserver
    {
        private static int generate = 0;
        public int Number;

        public UnvParseTreeNode()
        {
            Number = ++generate;
        }

        public UnvParseTreeNode(UnvParseTreeNode orig)
        {
            Number = ++generate;
            _NodeType = orig._NodeType;
            LocalName = orig.LocalName;
            NodeValue = orig.NodeValue;
            NodeName = orig.NodeName;
        }

        short _NodeType;
        public short NodeType
        {
            get { return _NodeType; }
            set { _NodeType = value; }
        }
        public virtual string LocalName { get; set; }
        public virtual Document OwnerDocument { get; set; }
        public virtual NodeList ChildNodes { get; set; } = new UnvParseTreeNodeList();
        public virtual IEnumerable<Node> AllChildren
        {
            get
            {
                return (ChildNodes as UnvParseTreeNodeList)._node_list;
            }
        }
        public virtual IEnumerable<ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement> Children
        {
            get
            {
                return (ChildNodes as UnvParseTreeNodeList)
                    ._node_list
                    .Where(c => c.GetType() == typeof(ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement))
                    .Select(c => c as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement);
            }
        }

        public virtual IEnumerable<string> GetChildrenText()
        {
            return (ChildNodes as UnvParseTreeNodeList)
                ._node_list
                .Where(c => c.GetType() == typeof(UnvParseTreeText))
                .Select(c => c as UnvParseTreeText)
                .Select(c => c.Data);
        }
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

        public virtual void OnParentDisconnect(UnvParseTreeNode value)
        {
            if (ParentNode != null)
            {
                UnvParseTreeNodeList children = ParentNode.ChildNodes as UnvParseTreeNodeList;
                children.Delete(this);
            }
            ParentNode = null;
        }

        public virtual void OnParentConnect(UnvParseTreeNode value)
        {
        }

        public virtual void OnChildDisconnect(UnvParseTreeNode value)
        {
        }

        public virtual void OnChildConnect(UnvParseTreeNode value)
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
                    var cc = c as UnvParseTreeNode;
                    cc?.Dispose();
                }
            }
        }

        public int RuleIndex { get; set; }

        private UnvParseTreeNode LeftMost(UnvParseTreeNode node)
        {
            if (node != null)
            {
                if (node.ChildNodes != null && node.ChildNodes.Length > 0)
                    return LeftMost(node.ChildNodes.item(0) as UnvParseTreeNode);
                else
                    return node;
            }
            return null;
        }

        private UnvParseTreeNode RightMost(UnvParseTreeNode node)
        {
            if (node != null)
            {
                if (node.ChildNodes != null && node.ChildNodes.Length > 0)
                    return RightMost(node.ChildNodes.item(node.ChildNodes.Length-1) as UnvParseTreeNode);
                else
                    return node;
            }
            return null;
        }

        public virtual void EnterRule(MyParseTreeListener listener)
        {
        }

        public virtual void ExitRule(MyParseTreeListener listener)
        {
        }

        public virtual void Validate(bool root)
        {
            // Parent must not be null.
            if (!(root || this.ParentNode != null))
                throw new Exception("Parent must not be null."); 
            if (ChildNodes != null)
            {
                for (int i = 0; i < ChildNodes.Length; ++i)
                {
                    Node c = ChildNodes.item(i);
                    var cc = c as UnvParseTreeNode;
                    cc.Validate(false);
                }
            }  
        }
    }
}
