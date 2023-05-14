namespace ParseTreeEditing.UnvParseTreeDOM
{
    using EditableAntlrTree;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using org.w3c.dom;
    using XmlDOM;
    using System.Xml.Linq;

    // Class to offer Antlr tree edits, both in-place and out-of-place,
    // and tree copying.
    public class TreeEdits
    {

        public delegate UnvParseTreeNode Fun(in UnvParseTreeNode arg1, out bool arg2);


        public static void AddChildren(UnvParseTreeNode parent, List<UnvParseTreeNode> list)
        {
            foreach (var mc in list)
            {
                mc.ParentNode = parent;
                parent.ChildNodes.Add(mc);
            }

        }

        public static void AddChildren(UnvParseTreeNode parent, UnvParseTreeNode child)
        {
            child.ParentNode = parent;
            parent.ChildNodes.Add(child);
        }

        public static void Delete(UnvParseTreeNode tree, Fun find)
        {
            if (tree == null) return;
            Stack<UnvParseTreeNode> stack = new Stack<UnvParseTreeNode>();
            stack.Push(tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                var found = find(n, out bool @continue);
                if (found != null)
                {
                    UnvParseTreeNode parent = n.ParentNode as UnvParseTreeNode;
                    var c = parent;
                    if (c != null)
                    {
                        for (int i = 0; i < c.ChildNodes.Length; ++i)
                        {
                            var child = c.ChildNodes.item(i);
                            if (child == n)
                            {
                                var t = c.ChildNodes.item(i);
                                t.ParentNode = null;
                                c.ChildNodes.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                if (!@continue)
                {
                }
                else
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; --i)
                    {
                        var c = n.ChildNodes.item(i);
                        stack.Push(c as UnvParseTreeNode);
                    }
                }
            }
        }

        public static void Delete(UnvParseTreeNode tree)
        {
            if (tree == null) return;
            var n = tree;
            var parent = n.ParentNode;
            var c = parent;
            if (c != null)
            {
                for (int i = 0; i < c.ChildNodes.Length; ++i)
                {
                    var child = c.ChildNodes.item(i);
                    if (child == n)
                    {
                        var temp = c.ChildNodes.item(i);
                        var t = temp;
                        t.ParentNode = null;
                        c.ChildNodes.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static void Delete(IEnumerable<UnvParseTreeNode> trees)
        {
            foreach (var t in trees) Delete(t);
        }

        public static IEnumerable<UnvParseTreeNode> FindTopDown(UnvParseTreeNode tree, Fun find)
        {
            Stack<UnvParseTreeNode> stack = new Stack<UnvParseTreeNode>();
            stack.Push(tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                var found = find(n, out bool @continue);
                if (found != null)
                    yield return found;
                if (!@continue)
                {
                }

                for (int i = 0; i < n.ChildNodes.Length; ++i)
                {
                    var c = n.ChildNodes.item(i);
                    if (c is UnvParseTreeNode child) stack.Push(child);
                }
            }
        }

        public static IEnumerable<UnvParseTreeNode> Frontier(UnvParseTreeNode tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is UnvParseTreeElement e)
                {
                    bool term = true;
                    for (int i = 0; i < e.ChildNodes.Length; ++i)
                    {
                        var t = e.ChildNodes.item(i);
                        if (t is UnvParseTreeElement)
                        {
                            term = false;
                            break;
                        }
                    }

                    if (term) yield return n as UnvParseTreeNode;
                }

                if (!(n is UnvParseTreeNode))
                    continue;
                if (n.ChildNodes == null) continue;
                for (int i = n.ChildNodes.Length - 1; i >= 0; --i)
                {
                    var c = n.ChildNodes.item(i);
                    if (!(c is UnvParseTreeElement)) continue;
                    stack.Push(c);
                }
            }
        }

        public static Node InsertBefore(Node node, string arbitrary_string)
        {
            var node_to_insert = new UnvParseTreeText();
            node_to_insert.Data = arbitrary_string;
            var parent = node.ParentNode;
            for (int i = 0; i < parent.ChildNodes.Length; ++i)
            {
                var child = parent.ChildNodes.item(i);
                if (child == node)
                {
                    parent.ChildNodes.Insert(i, node_to_insert);
                    node_to_insert.ParentNode = parent;
                    break;
                }
            }
            return node_to_insert;
        }

        public static void InsertBefore(Node node, Node node_to_insert)
        {
            var parent = node.ParentNode;
            for (int i = 0; i < parent.ChildNodes.Length; ++i)
            {
                var child = parent.ChildNodes.item(i);
                if (child == node)
                {
                    parent.ChildNodes.Insert(i, node_to_insert);
                    node_to_insert.ParentNode = parent;
                    break;
                }
            }
        }

        public static void InsertAfter(IEnumerable<UnvParseTreeNode> trees, string arbitrary_string)
        {
            foreach (var tree in trees) InsertAfter(tree, arbitrary_string);
        }

        public static Node InsertAfter(Node node, string arbitrary_string)
        {
            var node_to_insert = new UnvParseTreeText();
            node_to_insert.Data = arbitrary_string;
            var parent = node.ParentNode;
            for (int i = 0; i < parent.ChildNodes.Length; ++i)
            {
                var child = parent.ChildNodes.item(i);
                if (child == node)
                {
                    parent.ChildNodes.Insert(i + 1, node_to_insert);
                    node_to_insert.ParentNode = parent;
                    break;
                }
            }
            return node_to_insert;
        }

        public static void InsertAfter(Node node, Node node_to_insert)
        {
            var parent = node.ParentNode;
            for (int i = 0; i < parent.ChildNodes.Length; ++i)
            {
                var child = parent.ChildNodes.item(i);
                if (child == node)
                {
                    parent.ChildNodes.Insert(i + 1, node_to_insert);
                    node_to_insert.ParentNode = parent;
                    break;
                }
            }
        }

        public static bool InsertAfter(UnvParseTreeNode tree, Func<UnvParseTreeNode, UnvParseTreeNode> insert_point)
        {
            throw new NotImplementedException();
            //var insert_this = insert_point(tree);
            //if (insert_this != null)
            //{
            //    IParseTree parent = tree.Parent;
            //    var c = parent as ParserRuleContext;
            //    for (int i = 0; i < c.ChildCount; ++i)
            //    {
            //        var child = c.children[i];
            //        if (child == tree)
            //        {
            //            c.children.Insert(i + 1, insert_this);
            //            var r = insert_this as ParserRuleContext;
            //            r.Parent = c;
            //            break;
            //        }
            //    }

            //    var parent = node.ParentNode;
            //    for (int i = 0; i < parent.ChildNodes.Length; ++i)
            //    {
            //        var child = parent.ChildNodes.item(i);
            //        if (child == node)
            //        {
            //            parent.ChildNodes.Insert(i + 1, node_to_insert);
            //            node_to_insert.ParentNode = parent;
            //            break;
            //        }
            //    }

            //    return true; // done.
            //}
            //if (tree as TerminalNodeImpl != null)
            //{
            //    TerminalNodeImpl tok = tree as TerminalNodeImpl;
            //    if (tok.Symbol.Type == TokenConstants.EOF)
            //        return true;
            //    else
            //        return false;
            //}
            //else
            //{
            //    for (int i = 0; i < tree.ChildCount; ++i)
            //    {
            //        var c = tree.GetChild(i);
            //        if (InsertAfter(c, insert_point))
            //            return true;
            //    }
            //}
            //return false;
        }

        static Node GoToRoot(Node p)
        {
            if (p.ParentNode == null) return p;
            else return GoToRoot(p.ParentNode);
        }

        public static UnvParseTreeNode LeftMostToken(Node tree)
        {
            if (tree == null) return null;
            for (int i = 0; i < tree.ChildNodes.Length; ++i)
            {
                var c = tree.ChildNodes.item(i);
                if (c == null)
                    return null;
                var lmt = LeftMostToken(c as UnvParseTreeNode);
                if (lmt != null)
                    return lmt;
            }

            return null;
        }

        public static UnvParseTreeNode RightMostToken(Node tree)
        {
            if (tree == null) return null;
            for (int i = tree.ChildNodes.Length - 1; i >= 0; --i)
            {
                var c = tree.ChildNodes.item(i);
                if (c == null)
                    return null;
                var lmt = RightMostToken(c as UnvParseTreeNode);
                if (lmt != null)
                    return lmt;
            }

            return null;
        }

        public static UnvParseTreeNode NextToken(XmlAttr leaf)
        {
            if (leaf == null)
                throw new ArgumentNullException(nameof(leaf));
            for (Node v = leaf; v != null; v = v.ParentNode)
            {
                if (v == null) return null;
                var p = v.ParentNode;
                int start = -1;
                for (int i = 0; i < p.ChildNodes.Length; ++i)
                {
                    if (p.ChildNodes.item(i) == v && i + 1 < p.ChildNodes.Length)
                    {
                        start = i + 1;
                        break;
                    }
                }

                if (start < 0) continue;
                for (; start < p.ChildNodes.Length; ++start)
                {
                    var found = LeftMostToken(p.ChildNodes.item(start));
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public static string GetText(IList<IToken> list)
        {
            if (list == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (var l in list)
            {
                sb.Append(l.Text);
            }

            return sb.ToString();
        }

        public static (Dictionary<UnvParseTreeNode, string>, List<string>) TextToLeftOfLeaves(
            BufferedTokenStream stream, IParseTree tree)
        {
            throw new NotImplementedException();
            //var result = new Dictionary<TerminalNodeImpl, string>();
            //var result2 = new List<string>();
            //Stack<IParseTree> stack = new Stack<IParseTree>();
            //stack.Push(tree);
            //while (stack.Any())
            //{
            //    var n = stack.Pop();
            //    if (n is TerminalNodeImpl)
            //    {
            //        var nn = n as TerminalNodeImpl;
            //        {
            //            var p1 = TreeEdits.LeftMostToken(nn);
            //            var pp1 = p1.SourceInterval;
            //            var pp2 = p1.Payload;
            //            var index = pp2.TokenIndex;
            //            if (index >= 0)
            //            {
            //                var p2 = stream.GetHiddenTokensToLeft(index);
            //                var p3 = TreeEdits.GetText(p2);
            //                result.Add(nn, p3);
            //            }
            //            result2.Add(nn.GetText());
            //        }
            //    }
            //    else
            //    {
            //        if (!(n is ParserRuleContext p))
            //            continue;
            //        if (p.children == null)
            //            continue;
            //        if (p.children.Count == 0)
            //            continue;
            //        foreach (var c in p.children.Reverse())
            //        {
            //            stack.Push(c);
            //        }
            //    }
            //}
            //return (result, result2);
        }

        public static (Dictionary<UnvParseTreeNode, string>, List<string>) TextToLeftOfLeaves(
            BufferedTokenStream stream, IEnumerable<IParseTree> trees)
        {
            throw new NotImplementedException();
            //var result = new Dictionary<TerminalNodeImpl, string>();
            //var result2 = new List<string>();
            //Stack<IParseTree> stack = new Stack<IParseTree>();
            //foreach (var tree in trees)
            //{
            //    stack.Push(tree);
            //    while (stack.Any())
            //    {
            //        var n = stack.Pop();
            //        if (n is TerminalNodeImpl)
            //        {
            //            var nn = n as TerminalNodeImpl;
            //            {
            //                var p1 = TreeEdits.LeftMostToken(nn);
            //                var pp1 = p1.SourceInterval;
            //                var pp2 = p1.Payload;
            //                var index = pp2.TokenIndex;
            //                if (index >= 0)
            //                {
            //                    var p2 = stream.GetHiddenTokensToLeft(index);
            //                    var p3 = TreeEdits.GetText(p2);
            //                    result.Add(nn, p3);
            //                }
            //                result2.Add(nn.GetText());
            //            }
            //        }
            //        else
            //        {
            //            if (!(n is ParserRuleContext p))
            //                continue;
            //            if (p.children == null)
            //                continue;
            //            if (p.children.Count == 0)
            //                continue;
            //            foreach (var c in p.children.Reverse())
            //            {
            //                stack.Push(c);
            //            }
            //        }
            //    }
            //}
            //return (result, result2);
        }

        public static UnvParseTreeNode CopyTreeRecursive(UnvParseTreeNode original, IParseTree parent,
            Dictionary<TerminalNodeImpl, string> text_to_left)
        {
            throw new NotImplementedException();
            //if (original == null) return null;
            //else if (original is TerminalNodeImpl)
            //{
            //    var o = original as TerminalNodeImpl;
            //    var new_node = new TerminalNodeImpl(o.Symbol);
            //    if (text_to_left != null)
            //    {
            //        if (text_to_left.TryGetValue(o, out string value))
            //            text_to_left.Add(new_node, value);
            //    }
            //    if (parent != null)
            //    {
            //        var parent_rule_context = (ParserRuleContext)parent;
            //        new_node.Parent = parent_rule_context;
            //        parent_rule_context.AddChild(new_node);
            //    }
            //    return new_node;
            //}
            //else if (original is ParserRuleContext)
            //{
            //    var type = original.GetType();
            //    var new_node = (ParserRuleContext)Activator.CreateInstance(type, null, 0);
            //    if (parent != null)
            //    {
            //        var parent_rule_context = (ParserRuleContext)parent;
            //        new_node.Parent = parent_rule_context;
            //        parent_rule_context.AddChild(new_node);
            //    }
            //    int child_count = original.ChildCount;
            //    for (int i = 0; i < child_count; ++i)
            //    {
            //        var child = original.GetChild(i);
            //        CopyTreeRecursive(child, new_node, text_to_left);
            //    }
            //    return new_node;
            //}
            //else return null;
        }

        public static UnvParseTreeNode CopyTreeRecursiveAux(UnvParseTreeNode original, UnvParseTreeNode parent)
        {
            throw new NotImplementedException();
            //if (original == null) return null;
            //else if (original is AntlrAttr a)
            //{
            //    var o = a;
            //    var c = new AntlrAttr(a);
            //    var t = a.Value as MyToken;
            //    var tt = new MyToken() { Type = t.Type, Text = t.Text, Channel = t.Channel, StartIndex = t.StartIndex, StopIndex = t.StopIndex, TokenIndex = t.TokenIndex, InputStream = ncs };
            //    var oo = new MyTerminalNodeImpl(tt);
            //    oo.TokenStream = nts;
            //    oo.InputStream = ncs;
            //    if (parent != null)
            //    {
            //        var parent_rule_context = (ParserRuleContext)parent;
            //        oo.Parent = parent_rule_context;
            //        parent_rule_context.AddChild(oo);
            //    }
            //    return oo;
            //}
            //else if (original is MyParserTreeNode b)
            //{
            //    var p = parent as MyParserTreeNode;
            //    var new_node = new MyParserRuleContext(p, 0);
            //    new_node.TokenStream = nts;
            //    new_node.InputStream = ncs;
            //    new_node._ruleIndex = b.RuleIndex;
            //    if (p != null)
            //    {
            //        var parent_rule_context = p;
            //        new_node.Parent = parent_rule_context;
            //        parent_rule_context.AddChild(new_node);
            //    }
            //    new_node.Start = b.Start;
            //    new_node.Stop = b.Stop;
            //    int child_count = original.ChildCount;
            //    for (int i = 0; i < child_count; ++i)
            //    {
            //        var child = original.GetChild(i);
            //        CopyTreeRecursiveAux(child, new_node, nts, ncs);
            //    }
            //    return new_node;
            //}
            //else return null;
        }

        public static UnvParseTreeNode CopyTreeRecursive(UnvParseTreeNode original)
        {
            throw new NotImplementedException();
            //if (original == null) return (null, null, null);
            //// Make a copy of the token stream and char streams and attach them to this new tree.
            //var ncs = new MyCharStream();
            //var nts = new MyTokenStream();
            //if (original is MyParserTreeNode o_)
            //{
            //    var orig_ts = o_.TokenStream;
            //    nts = new MyTokenStream(orig_ts);
            //    var t = TreeEdits.LeftMostToken(o_);
            //    var ti = t.Payload.TokenSource;
            //    var ts = o_.TokenStream;
            //    var cs = ts._charstream;
            //    ncs.Text = String.Copy(cs.Text);
            //}
            //var copy = CopyTreeRecursiveAux(original, parent, nts, ncs);

            //return (copy, ncs, nts);
        }

        public static string Reconstruct(IEnumerable<UnvParseTreeNode> trees)
        {
            // Go up tree find root.
            var root = trees.First();
            while (root.ParentNode != null && root.ParentNode is UnvParseTreeNode)
            {
                root = (UnvParseTreeNode)root.ParentNode;
            }
            return Reconstruct(root);
        }

        public static string Reconstruct(UnvParseTreeNode tree)
        {
            Stack<Node> stack = new Stack<Node>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
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

        public static void Reconstruct(StringBuilder sb, UnvParseTreeNode tree)
        {
            throw new NotImplementedException();
            //if (tree as TerminalNodeImpl != null)
            //{
            //    TerminalNodeImpl tok = tree as TerminalNodeImpl;
            //    text_to_left.TryGetValue(tok, out string inter);
            //    if (inter == null)
            //        sb.Append(" ");
            //    else
            //        sb.Append(inter);
            //    if (tok.Symbol.Type == TokenConstants.EOF)
            //        return;
            //    sb.Append(tok.GetText());
            //}
            //else
            //{
            //    for (int i = 0; i < tree.ChildCount; ++i)
            //    {
            //        var c = tree.GetChild(i);
            //        Reconstruct(sb, c, text_to_left);
            //    }
            //}
        }

        public static UnvParseTreeNode Find(IToken token, UnvParseTreeNode tree)
        {
            throw new NotImplementedException();
            //if (tree == null) return null;
            //Stack<IParseTree> stack = new Stack<IParseTree>();
            //stack.Push(tree);
            //while (stack.Any())
            //{
            //    var n = stack.Pop();
            //    if (n is TerminalNodeImpl term)
            //    {
            //        if (term.Symbol == token)
            //            return term;
            //    }
            //    else
            //    {
            //        for (int i = n.ChildCount - 1; i >= 0; --i)
            //        {
            //            var c = n.GetChild(i);
            //            stack.Push(c);
            //        }
            //    }
            //}
            //return null;
        }

        public static void MoveAfter(IEnumerable<UnvParseTreeNode> from_list, UnvParseTreeNode to)
        {
            throw new NotImplementedException();
            //if (from_list == null) return;
            //if (to == null) return;
            //foreach (var from in from_list.Reverse())
            //{
            //    IParseTree parent_from = from.Parent;
            //    var ctx_parent_from = parent_from as ParserRuleContext;
            //    if (ctx_parent_from != null)
            //    {
            //        for (int i = 0; i < ctx_parent_from.ChildCount; ++i)
            //        {
            //            var child = ctx_parent_from.children[i];
            //            if (child == from)
            //            {
            //                var temp = ctx_parent_from.children[i];
            //                if (temp is TerminalNodeImpl)
            //                {
            //                    var t = temp as TerminalNodeImpl;
            //                    t.Parent = null;
            //                    ctx_parent_from.children.RemoveAt(i);
            //                }
            //                else if (temp is ParserRuleContext)
            //                {
            //                    var t = temp as ParserRuleContext;
            //                    t.Parent = null;
            //                    ctx_parent_from.children.RemoveAt(i);
            //                }
            //                else
            //                    throw new Exception("Tree contains something other than TerminalNodeImpl or ParserRuleContext");
            //                break;
            //            }
            //        }
            //    }
            //    IParseTree parent_to = to.Parent;
            //    var ctx_parent_to = parent_to as ParserRuleContext;
            //    for (int i = 0; i < ctx_parent_to.ChildCount; ++i)
            //    {
            //        var child = ctx_parent_to.children[i];
            //        if (child == to)
            //        {
            //            ctx_parent_to.children.Insert(i + 1, from);
            //            var r1 = from as TerminalNodeImpl;
            //            var r2 = from as ParserRuleContext;
            //            if (r1 != null) r1.Parent = ctx_parent_to;
            //            else if (r2 != null) r2.Parent = ctx_parent_to;
            //            break;
            //        }
            //    }
            //}
        }

        public static void MoveBefore(IEnumerable<UnvParseTreeElement> from_list, UnvParseTreeNode to)
        {
            if (from_list == null) return;
            if (to == null) return;
            foreach (var from in from_list)
            {
                MoveBefore(from, to);
            }
        }


        public static void NukeTokensSurrounding(UnvParseTreeNode node)
        {
            throw new NotImplementedException();
            //if (node == null) return;
            //EditableAntlrTree.MyToken token;
            //EditableAntlrTree.MyCharStream charstream;
            //EditableAntlrTree.MyLexer lexer;
            //EditableAntlrTree.MyParser parser;
            //EditableAntlrTree.MyTokenStream tokstream;
            //if (node is EditableAntlrTree.MyTerminalNodeImpl myterminalnode)
            //{
            //    lexer = myterminalnode.Lexer;
            //    parser = myterminalnode.Parser;
            //    tokstream = myterminalnode.TokenStream;
            //    token = myterminalnode.Payload as EditableAntlrTree.MyToken;
            //    charstream = myterminalnode.InputStream;
            //}
            //else if (node is EditableAntlrTree.MyParserTreeNode myinternalnode)
            //{
            //    lexer = myinternalnode.Lexer;
            //    parser = myinternalnode.Parser;
            //    tokstream = myinternalnode.TokenStream;
            //    var lmf = TreeEdits.LeftMostToken(node) as EditableAntlrTree.MyTerminalNodeImpl;
            //    token = lmf.Payload as EditableAntlrTree.MyToken;
            //    charstream = myinternalnode.InputStream;
            //}
            //else throw new Exception("Tree editing must be on AltAntlr tree.");
            //IParseTree parent_from = node.Parent;
            //var is_root = parent_from == null;
            //var old_buffer = charstream.Text;
            //var leaves_of_node = TreeEdits.Frontier(node).ToList();
            //var leftmost_leaf_of_node = leaves_of_node.First() as EditableAntlrTree.MyTerminalNodeImpl;
            //var leftmost_token_of_node = leftmost_leaf_of_node.Payload as EditableAntlrTree.MyToken;
            //var leftmost_token_of_node_tokenindex = leftmost_token_of_node.TokenIndex;
            //var rightmost_leaf_of_node = leaves_of_node.Last() as EditableAntlrTree.MyTerminalNodeImpl;
            //var rightmost_token_of_node = rightmost_leaf_of_node.Payload as EditableAntlrTree.MyToken;
            //var rightmost_token_of_node_tokenindex = rightmost_token_of_node.TokenIndex;
            //int token_index = rightmost_token_of_node_tokenindex + 1;
            //for (; ; )
            //{
            //    if (token_index >= tokstream.Size) break;
            //    if (token_index < 0) break;
            //    var tt = tokstream.Get(token_index);
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    if (tok.Type == TokenConstants.EOF) break;
            //    if (tok.Channel == Lexer.DefaultTokenChannel) break;
            //    token_index++;
            //}
            //--token_index;
            //var new_buffer = charstream.Text;
            //for (int i = token_index; i > rightmost_token_of_node_tokenindex; --i)
            //{
            //    tokstream.Seek(i);
            //    var tt = tokstream.Get(i);
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    var chars_to_delete_right = tok.StopIndex - tok.StartIndex + 1;
            //    tokstream.Seek(i);
            //    tokstream.Delete();
            //    new_buffer = new_buffer.Remove(rightmost_token_of_node.StopIndex + 1, chars_to_delete_right);
            //    for (int j = i; j < tokstream.Size; ++j)
            //    {
            //        var t2 = tokstream.Get(j);
            //        var tok2 = t2 as EditableAntlrTree.MyToken;
            //        tok2.StartIndex -= chars_to_delete_right;
            //        tok2.StopIndex -= chars_to_delete_right;
            //        tok2.TokenIndex -= 1;
            //        if (tok2.Type == TokenConstants.EOF) break;
            //    }
            //}
            //token_index = leftmost_token_of_node_tokenindex - 1;
            //for (; ; )
            //{
            //    if (token_index >= tokstream.Size) break;
            //    if (token_index < 0) break;
            //    var tt = tokstream.Get(token_index);
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    if (tok.Type == TokenConstants.EOF) break;
            //    if (tok.Channel == Lexer.DefaultTokenChannel) break;
            //    token_index--;
            //}
            //++token_index;
            //int diff = leftmost_token_of_node_tokenindex - token_index;
            //for (int i = 0; i < diff; ++i)
            //{
            //    tokstream.Seek(token_index);
            //    var tt = tokstream.Get(token_index);
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    var chars_to_delete_right = tok.StopIndex - tok.StartIndex + 1;
            //    var start = tok.StartIndex;
            //    tokstream.Seek(token_index);
            //    tokstream.Delete();
            //    new_buffer = new_buffer.Remove(start, chars_to_delete_right);
            //    for (int j = token_index; j < tokstream.Size; ++j)
            //    {
            //        var t2 = tokstream.Get(j);
            //        var tok2 = t2 as EditableAntlrTree.MyToken;
            //        tok2.StartIndex -= chars_to_delete_right;
            //        tok2.StopIndex -= chars_to_delete_right;
            //        tok2.TokenIndex -= 1;
            //        if (tok2.Type == TokenConstants.EOF) break;
            //    }
            //}
            //for (int i = 0; ; ++i)
            //{
            //    if (i >= tokstream.Size) break;
            //    var tt = tokstream.Get(i);
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    var new_index = tok.StartIndex;
            //    if (new_index >= 0)
            //    {
            //        var (line, col) = EditableAntlrTree.Util.GetLineColumn(new_index, new_buffer);
            //        tok.Line = line;
            //        tok.Column = col;
            //    }
            //    if (tt.Type == TokenConstants.EOF) break;
            //}
            //charstream.Text = new_buffer;
            //tokstream.Text = new_buffer;
            //var root = node;
            //for (; root.Parent != null; root = root.Parent) ;
            //Reset(root);
            //// Compare text of token with input.
            //for (int i = 0; i < tokstream.Size; ++i)
            //{
            //    tokstream.Seek(i);
            //    var tt = tokstream.Get(i);
            //    if (tt.Type == -1) break;
            //    var tok = tt as EditableAntlrTree.MyToken;
            //    var text1 = tt.Text;
            //    string text2;
            //    if (tt.StopIndex - tt.StartIndex + 1 < 0) text2 = "";
            //    else text2 = charstream.Text.Substring(tt.StartIndex, tt.StopIndex - tt.StartIndex + 1);
            //    if (text1 != text2) throw new Exception("mismatch after insert.");
            //    if (tok.Text != text2) throw new Exception("mismatch after insert.");
            //}
        }

        private static void MoveBefore(UnvParseTreeElement from, UnvParseTreeNode to)
        {
            throw new NotImplementedException();
        }

        public static void Replace(IEnumerable<UnvParseTreeNode> trees, string arbitrary_string)
        {
            foreach (var tree in trees) Replace(tree, arbitrary_string);
        }

        public static UnvParseTreeNode Replace(UnvParseTreeNode node, string arbitrary_string)
        {
            var node_to_insert = new UnvParseTreeText();
            node_to_insert.Data = arbitrary_string;
            var parent = node.ParentNode;
            for (int i = 0; i < parent.ChildNodes.Length; ++i)
            {
                var child = parent.ChildNodes.item(i);
                if (child == node)
                {
                    parent.ChildNodes.RemoveAt(i);
                    parent.ChildNodes.Insert(i, node_to_insert);
                    node_to_insert.ParentNode = parent;
                    child.ParentNode = null;
                    break;
                }
            }
            return node_to_insert;
        }

        public static void Replace(IEnumerable<UnvParseTreeNode> trees, Fun find)
        {
            foreach (var tree in trees) Replace(tree, find);
        }

        public static void Replace(UnvParseTreeNode tree, Fun find)
        {
            if (tree == null) return;
            Stack<UnvParseTreeNode> stack = new Stack<UnvParseTreeNode>();
            stack.Push(tree);
            while (stack.Any())
            {
                var n = stack.Pop();
                var found = find(n, out bool @continue);
                if (found != null)
                {
                    Node parent = n.ParentNode;
                    var c = parent as UnvParseTreeNode;
                    if (c != null)
                    {
                        for (int i = 0; i < c.ChildNodes.Length; ++i)
                        {
                            var child = c.ChildNodes.item(i);
                            if (child == n)
                            {
                                var temp = c.ChildNodes.item(i);
                                if (temp is UnvParseTreeNode)
                                {
                                    var t = temp as UnvParseTreeNode;
                                    t.ParentNode = null;
                                    c.ChildNodes.Replace(i, found);
                                    var rt = found as UnvParseTreeNode;
                                    if (rt != null) rt.ParentNode = c;
                                    var rp = found as UnvParseTreeNode;
                                    if (rp != null) rp.ParentNode = c;
                                }
                                else
                                    throw new Exception("Tree contains something other than TerminalNodeImpl or ParserRuleContext");
                                break;
                            }
                        }
                    }
                }
                if (!@continue) { }
                else
                {
                    for (int i = n.ChildNodes.Length - 1; i >= 0; --i)
                    {
                        var c = n.ChildNodes.item(i);
                        if (c is UnvParseTreeNode x) stack.Push(x);
                    }
                }
            }
        }

        public static void Replace(UnvParseTreeNode replace_this, UnvParseTreeNode with_this)
        {
            throw new NotImplementedException();
            //if (replace_this == null) return;
            //if (with_this == null) return;
            //var n = replace_this;
            //IParseTree parent = n.Parent;
            //var c = parent as ParserRuleContext;
            //if (c != null)
            //{
            //    for (int i = 0; i < c.ChildCount; ++i)
            //    {
            //        var child = c.children[i];
            //        if (child == n)
            //        {
            //            var temp = c.children[i];
            //            if (temp is TerminalNodeImpl)
            //            {
            //                var t = temp as TerminalNodeImpl;
            //                t.Parent = null;
            //                c.children[i] = with_this;
            //                var rt = with_this as TerminalNodeImpl;
            //                if (rt != null) rt.Parent = c;
            //                var rp = with_this as ParserRuleContext;
            //                if (rp != null) rp.Parent = c;
            //            }
            //            else if (temp is ParserRuleContext)
            //            {
            //                var t = temp as ParserRuleContext;
            //                t.Parent = null;
            //                c.children[i] = with_this;
            //                var rt = with_this as TerminalNodeImpl;
            //                if (rt != null) rt.Parent = c;
            //                var rp = with_this as ParserRuleContext;
            //                if (rp != null) rp.Parent = c;
            //            }
            //            else
            //                throw new Exception("Tree contains something other than TerminalNodeImpl or ParserRuleContext");
            //            break;
            //        }
            //    }
            //    // Shift all tokens over.
            //    // Shift text over.
            //}
        }

        public static void DeleteAndReattachChildren(UnvParseTreeElement node)
        {
            if (node == null) return;
            var n = node;
            var parent = n.ParentNode;
            var c = parent;
            if (c != null)
            {
                int i = 0;
                for (i = 0; i < c.ChildNodes.Length; ++i)
                {
                    var child = c.ChildNodes.item(i);
                    if (child == n)
                    {
                        var temp = c.ChildNodes.item(i);
                        var t = temp;
                        t.ParentNode = null;
                        c.ChildNodes.RemoveAt(i);
                        break;
                    }
                }
                for (int j = n.ChildNodes.Length - 1; j >= 0; --j)
                {
                    var child = n.ChildNodes.item(j);
                    child.ParentNode = c;
                    c.ChildNodes.Insert(i, child);
                }
            }
        }
    }
}
