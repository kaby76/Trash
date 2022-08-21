using System.Linq;

namespace XmlDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using java.net;
    using java.util;
    using javax.xml.datatype;
    using javax.xml.@namespace;
    using org.eclipse.wst.xml.xpath2.api;
    using org.w3c.dom;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;

    public class ConvertToDOM : XMLParserBaseVisitor<object>
    {
        public static AntlrDynamicContext Try(IParseTree tree, Parser parser)
        {
            // Perform bottom up traversal to derive equivalent tree in "dom".
            var visitor = new ConvertToDOM();
            var document = visitor.Visit(tree) as XmlDocument;
            AntlrDynamicContext result = new AntlrDynamicContext();
            result.Document = document;
            return result;
        }

        private static XmlNode BottomUpConvert(IParseTree tree, Parser parser)
        {
            if (tree is TerminalNodeImpl)
            {
                var result = new XmlElement();
                result.AntlrIParseTree = tree;
                TerminalNodeImpl t = tree as TerminalNodeImpl;
                Interval interval = t.SourceInterval;
                result.NodeType = NodeConstants.ELEMENT_NODE;
                var common_token_stream = parser.InputStream as CommonTokenStream;
                var lexer = common_token_stream.TokenSource as Lexer;
                var fixed_name = parser.Vocabulary.GetSymbolicName(t.Symbol.Type);
                result.LocalName = fixed_name;
                var nl = new AntlrNodeList();
                result.ChildNodes = nl;

                var child = new XmlText();
                child.AntlrIParseTree = tree;
                child.NodeType = NodeConstants.TEXT_NODE;
                child.Data = new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree().PerformEscapes(/*"'" + */ tree.GetText() /*+ "'"*/);
                child.ParentNode = result;
                nl.Add(child);

                {
                    var attr = new XmlAttr();
                    var child_count = t.ChildCount;
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "ChildCount";
                    attr.Value = child_count.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                }

                {
                    var attr = new XmlAttr();
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    var b = source_interval.b;
                    attr.Name = "SourceInterval";
                    attr.Value = "[" + a + "," + b + "]";
                    attr.ParentNode = result;
                    nl.Add(attr);
                }

                return result;
            }
            else
            {
                var result = new XmlElement();
                var t = tree as ParserRuleContext;
                result.AntlrIParseTree = tree;
                result.NodeType = NodeConstants.ELEMENT_NODE;
                var x = tree as RuleContext;
                var ri = x.RuleIndex;
                var name = parser.RuleNames[ri];
                result.LocalName = name;
                var nl = new AntlrNodeList();
                result.ChildNodes = nl;

                var map = new AntlrNamedNodeMap();
                result.Attributes = map;

                {
                    var attr = new XmlAttr();
                    var child_count = t.ChildCount;
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "ChildCount";
                    attr.LocalName = "ChildCount";
                    attr.Value = child_count.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }

                {
                    var attr = new XmlAttr();
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    var b = source_interval.b;
                    attr.Name = "Start";
                    attr.LocalName = "Start";
                    attr.Value = a.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new XmlAttr();
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    var b = source_interval.b;
                    attr.Name = "End";
                    attr.LocalName = "End";
                    attr.Value = b.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }

                for (int i = 0; i < tree.ChildCount; ++i)
                {
                    var child = tree.GetChild(i);
                    var convert = BottomUpConvert(child, parser);
                    nl.Add(convert);
                    convert.ParentNode = result;
                }

                return result;
            }
        }

        public override object /* XmlAttr */ VisitAttribute(XMLParser.AttributeContext context)
        {
            var attr = new XmlAttr();
            attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
            attr.Name = context.Name();
            attr.Value = context.STRING().GetText();
            attr.ChildNodes = new AntlrNodeList();
            return attr;
        }

        public override object /* XmlText */ VisitChardata(XMLParser.ChardataContext context)
        {
            var result = new XmlText();
            result.NodeType = NodeConstants.TEXT_NODE;
            result.ChildNodes = new AntlrNodeList();
            if (context.TEXT() != null)
            {
                result.Data = context.TEXT().GetText();
            }
            return result;
        }

        public override object /* NodeList */ VisitContent(XMLParser.ContentContext context)
        {
            var result = new AntlrNodeList();
            for (int i = 0; i < context.ChildCount; ++i)
            {
                var c = context.GetChild(i);
                if (c is XMLParser.ChardataContext cc)
                {
                    var rc = VisitChardata(cc) as XmlText;
                    result.Add(rc);
                }
                else if (c is XMLParser.ElementContext e)
                {
                    var re = VisitElement(e) as XmlElement;
                    result.Add(re);
                }
            }
            return result;
        }

        public override object /* XmlDocument */ VisitDocument(XMLParser.DocumentContext context)
        {
            var document = new XmlDocument();
            document.NodeType = NodeConstants.DOCUMENT_NODE;
            var element = VisitElement(context.element()) as XmlElement;
            AntlrNodeList nl = new AntlrNodeList();
            nl.Add(element);
            document.ChildNodes = nl;
            element.ParentNode = document;
            return document;
        }

        public override object /* XmlElement */ VisitElement(XMLParser.ElementContext context)
        {
            var names = context.Name();
            string name = null;
            foreach (var n in names.Select(t=>t.GetText()))
            {
                if (name == null) name = n;
                if (n != name)
                    throw new Exception();
            }
            var element = new XmlElement();
            element.NodeType = NodeConstants.ELEMENT_NODE;
            element.NodeName = name;
            element.LocalName = name;

            var map = new AntlrNamedNodeMap();
            element.Attributes = map;
            var attrs = context.attribute();
            foreach (var a in attrs)
            {
                var new_attr = VisitAttribute(a) as Attr;
                map.Add(new_attr);
                new_attr.ParentNode = element;
            }

            if (context.content() != null)
            {
                var content = VisitContent(context.content()) as NodeList;
                element.ChildNodes = content;
                for (int i = 0; i < content.Length; ++i)
                {
                    var c = content.item(i);
                    c.ParentNode = element;
                }
            }

            return element;
        }
    }

    public class AntlrDynamicContext : DynamicContext
    {
        public AntlrDynamicContext() { }

        public Node LimitNode { get; }
        public Document Document { get; set; }
        public ResultSequence getVariable(QName name)
        {
            throw new NotImplementedException();
        }

        public URI resolveUri(string uri)
        {
            throw new NotImplementedException();
        }

        public GregorianCalendar CurrentDateTime { get; }
        public Duration TimezoneOffset { get; }
        public Document getDocument(URI uri)
        {
            return Document;
        }

        public IDictionary<string, IList<Document>> Collections { get; }
        public IList<Document> DefaultCollection { get; }
        public CollationProvider CollationProvider
        {
            get { return new AntlrCollationProvider(); }
        }
    }


    public class AntlrCollationProvider : CollationProvider
    {
        public class StringCmp : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.CompareTo(y);
            }
        }

        public IComparer<string> getCollation(string name)
        {
            return new StringCmp();
        }

        public string DefaultCollation { get; }
    }

    public class AntlrNodeList : NodeList
    {
        public List<XmlNode> _node_list = new List<XmlNode>();

        public int Length
        {
            get { return _node_list.Count; }
            set { throw new Exception(); }
        }

        public Node item(int i)
        {
            return _node_list[i];
        }

        public void Add(XmlNode e)
        {
            _node_list.Add(e);
        }
    }

    public class XmlText : XmlNode, Text
    {
        public string Data { get; set; }
    }

    public class XmlAttr : XmlNode, Attr
    {
        public string Prefix { get; set; }
        public object Name { get; set; }
        public string Value { get; set; }
        public Node OwnerElement { get; set; }
        public TypeInfo SchemaTypeInfo { get; set; }
    }

    public class XmlDocument : XmlNode, Document
    {
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

    public class XmlElement : XmlNode, Element
    {
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
    }

    public class AntlrNamedNodeMap : NamedNodeMap
    {
        private List<Attr> _attrs = new List<Attr>();

        public void Add(Attr a)
        {
            _attrs.Add(a);
        }

        public int Length
        {
            get
            {
                return _attrs.Count;
            }
            set
            {
                throw new Exception();
            }
        }

        // Zero based???
        public Attr item(int i)
        {
            return _attrs[i];
        }
    }

    public class XmlNode : Node
    {
        public IParseTree AntlrIParseTree { get; set; }
        public short NodeType { get; set; }
        public string LocalName { get; set; }
        public Document OwnerDocument { get; set; }
        public NodeList ChildNodes { get; set; }
        public Node NextSibling { get; set; }
        public string BaseURI { get; set; }
        public NamedNodeMap Attributes { get; set; }
        public object NodeValue { get; set; }
        public string NamespaceURI { get; set; }
        public object NodeName { get; set; }
        public Node ParentNode { get; set; }
        public Node PreviousSibling { get; set; }
        public bool isSameNode(Node nodeValue)
        {
            throw new NotImplementedException();
        }

        public short compareDocumentPosition(Node nodeB)
        {
            throw new NotImplementedException();
        }

        public bool isEqualNode(Node node)
        {
            throw new NotImplementedException();
        }

        public bool hasChildNodes()
        {
            throw new NotImplementedException();
        }

        public bool hasAttributes()
        {
            throw new NotImplementedException();
        }
    }
}
