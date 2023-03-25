namespace ParseTreeEditing.ParseTreeDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.w3c.dom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ConvertToDOM
    {
        public AntlrDynamicContext Try(IEnumerable<UnvParseTreeNode> trees, Parser parser)
        {
            var document = new UnvParseTreeDocument();
            document.NodeType = NodeConstants.DOCUMENT_NODE;
            UnvParseTreeNodeList nl = new UnvParseTreeNodeList();
            document.ChildNodes = nl;
            AntlrDynamicContext result = new AntlrDynamicContext(document);
            result.Document = document;
            foreach (var tree in trees)
            {
                nl.Add(tree);
            }
            return result;
        }

        public AntlrDynamicContext Try(UnvParseTreeNode tree, Parser parser)
        {
            // Perform bottom up traversal to derive equivalent tree in "dom".
            Stack<UnvParseTreeNode> stack = new Stack<UnvParseTreeNode>();
            var document = new UnvParseTreeDocument();
            document.NodeType = NodeConstants.DOCUMENT_NODE;
            UnvParseTreeNodeList nl = new UnvParseTreeNodeList();
            nl.Add(tree);
            document.ChildNodes = nl;
            AntlrDynamicContext result = new AntlrDynamicContext(document);
            result.Document = document;
            return result;
        }

        public static UnvParseTreeElement BottomUpConvert(IParseTree tree, UnvParseTreeElement parent, Parser parser, Lexer lexer, CommonTokenStream tokstream, ICharStream charstream)
        {
            if (tree is TerminalNodeImpl)
            {
                TerminalNodeImpl t = tree as TerminalNodeImpl;
                var new_node = new UnvParseTreeElement();
                Interval interval = t.SourceInterval;
                new_node.NodeType = NodeConstants.ELEMENT_NODE;
                var fixed_name = parser.Vocabulary.GetSymbolicName(t.Symbol.Type);
                new_node.LocalName = fixed_name;
                var nl = new UnvParseTreeNodeList();
                new_node.ChildNodes = nl;
                var map = new AntlrNamedNodeMap();
                new_node.Attributes = map;
                new_node.RuleIndex = -1;

                // The terminal leaf must have a token.
                var term_token = t.Payload;
                if (term_token == null) throw new System.Exception("Malformed parse tree from Antlr. Expecting a terminal node with a token, but it is null instead.");
                var term_index = term_token.TokenIndex;

                // back up to previous channel for terminal token,
                // or the beginning of token stream.

                // Add in all previous hidden tokens or skips. Note, some of these
                // attributes get added to the parent of the new AntlrElement node
                // which corresponds to a parser rule node. The reason for this is
                // because we don't want hidden tokens associated with the TerminalImplNode
                // itself.

                // Set up copy to first hidden token or terminal token.
                int stop_token_index;
                var hidden_tokens = tokstream.GetHiddenTokensToLeft(term_index);
                int i;
                if (hidden_tokens != null && hidden_tokens.Count > 0)
                {
                    i = hidden_tokens.First().TokenIndex;
                }
                else
                {
                    i = term_index;
                }

                // Start at the current token and create a new attribute for
                // intertoken channel text.
                int start_cs;
                int stop_cs;
                int channel;
                int tt;
                if (i == 0)
                {
                    start_cs = 0;
                    stop_cs = tokstream.Get(i).StartIndex;
                }
                else
                {
                    start_cs = tokstream.Get(i-1).StopIndex + 1;
                    stop_cs = tokstream.Get(i).StartIndex;
                }

                for (; ;)
                {
                    // Get text in interval [start_cs, stop_cs] and make attribute.
                    if (stop_cs - start_cs > 0)
                    {
                        if (i <= 0)
                        {
                            start_cs = 0;
                        }
                        else
                        {
                            start_cs = tokstream.Get(i - 1).StopIndex + 1;
                        }
                        stop_cs = tokstream.Get(i).StartIndex;
                        if (stop_cs > 0) stop_cs--;
                        channel = -1;
                        tt = -1;
                        var attr = new UnvParseTreeAttr();
                        attr.Name = "Before";
                        attr.StringValue = charstream.GetText(new Interval(start_cs, stop_cs));
                        attr.ParentNode = parent;
                        attr.TokenType = tt;
                        attr.Channel = channel;
                        parent.ChildNodes.Add(attr);
                        map.Add(attr);
                    }

                    if (i == term_index) break;

                    {
                        channel = tokstream.Get(i).Channel;
                        tt = tokstream.Get(i).Type;
                        var attr = new UnvParseTreeAttr();
                        attr.Name = "Before";
                        start_cs = tokstream.Get(i).StartIndex;
                        stop_cs = tokstream.Get(i).StopIndex;
                        attr.StringValue = charstream.GetText(new Interval(start_cs, stop_cs));
                        attr.ParentNode = parent;
                        attr.TokenType = tt;
                        attr.Channel = channel;
                        parent.ChildNodes.Add(attr);
                        map.Add(attr);
                    }

                    {
                        i++;
                        start_cs = tokstream.Get(i - 1).StopIndex + 1;
                        stop_cs = tokstream.Get(i).StartIndex;
                        channel = tokstream.Get(i - 1).Channel;
                        tt = tokstream.Get(i - 1).Type;
                    }
                }

                //var p3 = TreeEdits.GetText(hidden_tokens);
                //var attr = new AntlrAttr();
                //var child_count = t.ChildCount;
                //attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //attr.Name = "Before";
                //attr.StringValue = p3;
                //attr.ParentNode = result;
                //nl.Add(attr);
                //map.Add(attr);

                parent.ChildNodes.Add(new_node);

                var child = new UnvParseTreeText();
                child.NodeType = NodeConstants.TEXT_NODE;
                //                child.Data = new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree().PerformEscapes(/*"'" + */ tree.GetText() /*+ "'"*/);
                channel = tokstream.Get(i).Channel;
                tt = tokstream.Get(i).Type;
                child.Data = tt < 0 ? "" : tree.GetText();
                child.ParentNode = new_node;
                new_node.ChildNodes.Add(child);
                //{
                //    var attr = new AntlrAttr();
                //    var child_count = t.ChildCount;
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "ChildCount";
                //    attr.LocalName = "ChildCount";
                //    attr.StringValue = child_count.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    var text = t.Symbol.Text;
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "Text";
                //    attr.LocalName = "Text";
                //    attr.StringValue = text;
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    var b = source_interval.b;
                //    attr.Name = "SI";
                //    attr.LocalName = "SI";
                //    attr.StringValue = "[" + a + "," + b + "]";
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    var b = source_interval.b;
                //    attr.Name = "Start";
                //    attr.LocalName = "Start";
                //    attr.StringValue = a.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    var b = source_interval.b;
                //    attr.Name = "End";
                //    attr.LocalName = "End";
                //    attr.StringValue = b.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    var symbol = t.Symbol;
                //    var v = symbol.Line;
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "Line";
                //    attr.LocalName = "Line";
                //    attr.StringValue = v.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    var symbol = t.Symbol;
                //    var v = symbol.Column;
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "Column";
                //    attr.LocalName = "Column";
                //    attr.StringValue = v.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                return new_node;
            }
            else
            {
                var new_node = new UnvParseTreeElement();
                var t = tree as ParserRuleContext;
                var t2 = tree as ObserverParserRuleContext;
                if (t2 != null) t2.Subscribe(new_node);
                new_node.NodeType = NodeConstants.ELEMENT_NODE;
                var name = parser.RuleNames[(tree as RuleContext).RuleIndex];
                new_node.LocalName = name;
                var nl = new UnvParseTreeNodeList();
                new_node.ChildNodes = nl;
                new_node.RuleIndex = t.RuleIndex;
                var map = new AntlrNamedNodeMap();
                new_node.Attributes = map;
                if (parent != null) parent.ChildNodes.Add(new_node);
                //{
                //    var attr = new AntlrAttr();
                //    var child_count = t.ChildCount;
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "ChildCount";
                //    attr.LocalName = "ChildCount";
                //    attr.StringValue = child_count.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    attr.Name = "Text";
                //    attr.LocalName = "Text";
                //    attr.StringValue = tree.GetText();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    var b = source_interval.b;
                //    attr.Name = "SI";
                //    attr.LocalName = "SI";
                //    attr.StringValue = "[" + a + "," + b + "]";
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    attr.Name = "Start";
                //    attr.LocalName = "Start";
                //    attr.StringValue = a.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var source_interval = t.SourceInterval;
                //    var a = source_interval.a;
                //    var b = source_interval.b;
                //    attr.Name = "End";
                //    attr.LocalName = "End";
                //    attr.StringValue = b.ToString();
                //    attr.ParentNode = result;
                //    nl.Add(attr);
                //    map.Add(attr);
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var a = t.Start;
                //    if (a != null)
                //    {
                //        attr.Name = "TStart";
                //        attr.LocalName = "TStart";
                //        attr.StringValue = a.ToString();
                //        attr.ParentNode = result;
                //        nl.Add(attr);
                //        map.Add(attr);
                //    }
                //}
                //{
                //    var attr = new AntlrAttr();
                //    attr.NodeType = NodeConstants.ATTRIBUTE_NODE;
                //    var b = t.Stop;
                //    if (b != null)
                //    {
                //        attr.Name = "TEnd";
                //        attr.LocalName = "TEnd";
                //        attr.StringValue = b.ToString();
                //        attr.ParentNode = result;
                //        nl.Add(attr);
                //        map.Add(attr);
                //    }
                //}
                //{
                //    var attr = new AntlrAttr();
                //    var interval = t.SourceInterval;
                //}
                //{
                //    var attr = new AntlrAttr();
                //    var interval = t.SourceInterval;
                //}
                for (int i = 0; i < tree.ChildCount; ++i)
                {
                    var child = tree.GetChild(i);
                    BottomUpConvert(child, new_node, parser, lexer, tokstream, charstream);
                }
//                Node prev = null;
                for (int i = 0; i < new_node.ChildNodes.Length; ++i)
                {
//                    Node curr = new_node.ChildNodes.item(i);
//                    if (curr is AntlrElement)
//                    {
//                    }
                    if (i > 0)
                    {
                        var pre = new_node.ChildNodes.item(i - 1);
                        var x = new_node.ChildNodes.item(i);
                        x.PreviousSibling = pre;
                        pre.NextSibling = x;
                    }
                }
                return new_node;
            }
        }
    }
}
