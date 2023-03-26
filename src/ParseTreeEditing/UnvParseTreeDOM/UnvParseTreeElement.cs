namespace ParseTreeEditing.UnvParseTreeDOM
{
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UnvParseTreeElement : UnvParseTreeNode, Element
    {
        public UnvParseTreeElement()
        {
            this.NodeType = NodeConstants.ELEMENT_NODE;
        }
        public UnvParseTreeElement(UnvParseTreeElement orig) : base(orig)
        {
            this.NodeType = NodeConstants.ELEMENT_NODE;
        }

        public virtual void EnterRule(IMyParseTreeListener listener)
        {
        }

        public virtual void ExitRule(IMyParseTreeListener listener)
        {
        }

        public object getAttributeNS(string sCHEMA_INSTANCE, string nIL_ATTRIBUTE)
        {
            return null;
        }

        public string Prefix { get; set; }
        public TypeInfo SchemaTypeInfo { get; set; }
        public string lookupNamespaceURI(string prefix)
        {
            throw new NotImplementedException();
        }

        public bool isDefaultNamespace(object elementNamespaceUri)
        {
            throw new NotImplementedException();
        }

        public string Reconstruct(Node tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            int last = -1;
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeAttr a)
                {
                    sb.Append(a.StringValue);
                }
                else if (n is UnvParseTreeText t)
                {
                    sb.Append(t.NodeValue);
                }
                else if (n is UnvParseTreeElement e)
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; i--)
                    {
                        stack.Push(n.ChildNodes.item(i));
                    }
                }
            }
            return sb.ToString();
        }

        public string GetText()
        {
            return Reconstruct(this);
        }
    }
}
