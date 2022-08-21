namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using org.w3c.dom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ConvertToDOM
    {
        Dictionary<IParseTree, AntlrNode> nodes = new Dictionary<IParseTree, AntlrNode>();

        public AntlrNode FindDomNode(IParseTree tree)
        {
            nodes.TryGetValue(tree, out AntlrNode result);
            return result;
        }

        public AntlrDynamicContext Try(IEnumerable<IParseTree> trees, Parser parser)
        {
            var document = new AntlrDocument(null);
            document.NodeType = NodeConstants.DOCUMENT_NODE;
            AntlrNodeList nl = new AntlrNodeList();
            document.ChildNodes = nl;
            AntlrDynamicContext result = new AntlrDynamicContext();
            result.Document = document;
            foreach (var tree in trees)
            {
                var node = FindDomNode(tree);
                if (node == null)
                {
                    var converted_tree = BottomUpConvert(tree, parser);
                    Stack<AntlrNode> stack = new Stack<AntlrNode>();
                    stack.Push(converted_tree);
                    while (stack.Any())
                    {
                        var n = stack.Pop();
                        var l = n.ChildNodes;
                        if (l != null)
                        {
                            for (int i = 0; i < l.Length; ++i)
                            {
                                stack.Push((AntlrNode)l.item(i));
                            }
                        }
                    }
                    nl.Add(converted_tree);
                }
                else
                {
                    nl.Add(node);
                }
            }
            return result;
        }

        public AntlrDynamicContext Try(IParseTree tree, Parser parser)
        {
            // Perform bottom up traversal to derive equivalent tree in "dom".
            var converted_tree = BottomUpConvert(tree, parser);
            Stack<AntlrNode> stack = new Stack<AntlrNode>();
            stack.Push(converted_tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                var l = n.ChildNodes;
                if (l != null)
                {
                    for (int i = 0; i < l.Length; ++i)
                    {
                        stack.Push((AntlrNode)l.item(i));
                    }
                }
            }
            var document = new AntlrDocument(null);
            document.NodeType = NodeConstants.DOCUMENT_NODE;
            AntlrNodeList nl = new AntlrNodeList();
            nl.Add(converted_tree);
            document.ChildNodes = nl;
            AntlrDynamicContext result = new AntlrDynamicContext();
            result.Document = document;
            return result;
        }

        private AntlrNode BottomUpConvert(IParseTree tree, Parser parser)
        {
            if (tree is TerminalNodeImpl)
            {
                var result = new AntlrElement(tree);
                //result.AntlrIParseTree = tree;
                TerminalNodeImpl t = tree as TerminalNodeImpl;
                Interval interval = t.SourceInterval;
                result.NodeType = NodeConstants.ELEMENT_NODE;
                var fixed_name = parser.Vocabulary.GetSymbolicName(t.Symbol.Type);
                result.LocalName = fixed_name;
                var nl = new AntlrNodeList();
                result.ChildNodes = nl;
                var map = new AntlrNamedNodeMap();
                result.Attributes = map;
                var child = new AntlrText(tree);
                child.AntlrIParseTree = tree;
                nodes[tree] = child;
                child.NodeType = NodeConstants.TEXT_NODE;
                //                child.Data = new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree().PerformEscapes(/*"'" + */ tree.GetText() /*+ "'"*/);
                child.Data = tree.GetText();
                child.ParentNode = result;
                nl.Add(child);
                {
                    var attr = new AntlrAttr(null);
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
                    var attr = new AntlrAttr(null);
                    var text = t.Symbol.Text;
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "Text";
                    attr.LocalName = "Text";
                    attr.Value = text;
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    var b = source_interval.b;
                    attr.Name = "SI";
                    attr.LocalName = "SI";
                    attr.Value = "[" + a + "," + b + "]";
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
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
                    var attr = new AntlrAttr(null);
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
                {
                    var attr = new AntlrAttr(null);
                    var symbol = t.Symbol;
                    var v = symbol.Line;
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "Line";
                    attr.LocalName = "Line";
                    attr.Value = v.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
                    var symbol = t.Symbol;
                    var v = symbol.Column;
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "Column";
                    attr.LocalName = "Column";
                    attr.Value = v.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                return result;
            }
            else
            {
                var result = new AntlrElement(tree);
                var t = tree as ParserRuleContext;
                var t2 = tree as ObserverParserRuleContext;
                if (t2 != null) t2.Subscribe(result);
                result.AntlrIParseTree = tree;
                nodes[tree] = result;
                result.NodeType = NodeConstants.ELEMENT_NODE;
                var name = parser.RuleNames[(tree as RuleContext).RuleIndex];
                result.LocalName = name;
                var nl = new AntlrNodeList();
                result.ChildNodes = nl;
                var map = new AntlrNamedNodeMap();
                result.Attributes = map;
                {
                    var attr = new AntlrAttr(null);
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
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    attr.Name = "Text";
                    attr.LocalName = "Text";
                    attr.Value = tree.GetText();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    var b = source_interval.b;
                    attr.Name = "SI";
                    attr.LocalName = "SI";
                    attr.Value = "[" + a + "," + b + "]";
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var source_interval = t.SourceInterval;
                    var a = source_interval.a;
                    attr.Name = "Start";
                    attr.LocalName = "Start";
                    attr.Value = a.ToString();
                    attr.ParentNode = result;
                    nl.Add(attr);
                    map.Add(attr);
                }
                {
                    var attr = new AntlrAttr(null);
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
                {
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var a = t.Start;
                    if (a != null)
                    {
                        attr.Name = "TStart";
                        attr.LocalName = "TStart";
                        attr.Value = a.ToString();
                        attr.ParentNode = result;
                        nl.Add(attr);
                        map.Add(attr);
                    }
                }
                {
                    var attr = new AntlrAttr(null);
                    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                    var b = t.Stop;
                    if (b != null)
                    {
                        attr.Name = "TEnd";
                        attr.LocalName = "TEnd";
                        attr.Value = b.ToString();
                        attr.ParentNode = result;
                        nl.Add(attr);
                        map.Add(attr);
                    }
                }
                {
                    var attr = new AntlrAttr(null);
                    var interval = t.SourceInterval;
                    if (interval.a <= interval.b)
                    {
                        var n = t as AltAntlr.MyParserRuleContext;
                        var s = n.TokenStream.Get(interval.a);
                        if (s != null)
                        {
                            var v = s.Line;
                            attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                            attr.Name = "Line";
                            attr.LocalName = "Line";
                            attr.Value = v.ToString();
                            attr.ParentNode = result;
                            nl.Add(attr);
                            map.Add(attr);
                        }
                    }
                }
                {
                    var attr = new AntlrAttr(null);
                    var interval = t.SourceInterval;
                    if (interval.a <= interval.b)
                    {
                        var n = t as AltAntlr.MyParserRuleContext;
                        var s = n.TokenStream.Get(interval.a);
                        if (s != null)
                        {
                            var v = s.Column;
                            attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                            attr.Name = "Column";
                            attr.LocalName = "Column";
                            attr.Value = v.ToString();
                            attr.ParentNode = result;
                            nl.Add(attr);
                            map.Add(attr);
                        }
                    }
                }
                for (int i = 0; i < tree.ChildCount; ++i)
                {
                    var child = tree.GetChild(i);
                    var convert = BottomUpConvert(child, parser);
                    nl.Add(convert);
                    convert.ParentNode = result;
                }
                for (int i = 0; i < nl.Length; ++i)
                {
                    var x = nl._node_list[i];
                    if (i > 0)
                    {
                        var pre = nl._node_list[i - 1];
                        x.PreviousSibling = pre;
                        pre.NextSibling = x;
                    }
                }
                return result;
            }
        }
    }
}
