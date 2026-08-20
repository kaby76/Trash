namespace Atn;

using ParseTreeEditing.UnvParseTreeDOM;

// Converts a ParseEvent sequence + raw LexerToken list into an UnvParseTreeElement tree.
// Replicates the structure that ConvertToDOM.BottomUpConvert produces, with no
// Antlr4.Runtime.Standard dependency.
//
// DOM structure per terminal token t (at all-token index k):
//   - Any non-default-channel tokens between the previous on-channel token and k are
//     added as UnvParseTreeAttr nodes on the current rule element (parent).
//   - The terminal itself becomes a child UnvParseTreeElement; its text is a
//     UnvParseTreeText child of that element.
//
// DEFAULT_CHANNEL = 0, EOF token type = -1 (ANTLR4 conventions, no runtime dependency).
public static class DomBuilder
{
    private const int DEFAULT_CHANNEL = 0;
    private const int EOF_TYPE = -1;

    public static UnvParseTreeElement Build(
        IReadOnlyList<ParseEvent> events,
        IReadOnlyList<LexerToken> allTokens,
        string[] ruleNames,
        string[] symbolicNames,
        string[] literalNames,
        string[] lexerRuleNames,
        bool lineNumbers)
    {
        var stack = new Stack<UnvParseTreeElement>();
        // For left-recursive rules: saves the parent element that will receive the
        // final accumulated rule element when ExitRecursionRule fires.
        var recursionParentStack = new Stack<UnvParseTreeElement>();
        UnvParseTreeElement root = null;
        int prevAllIdx = -1; // all-token index of the last consumed on-channel token

        foreach (var ev in events)
        {
            switch (ev.Kind)
            {
                case ParseEventKind.EnterRule:
                {
                    var elem = MakeRuleElement(ruleNames[ev.Index], ev.Index);
                    if (root == null) root = elem;
                    if (stack.Count > 0) AddChild(stack.Peek(), elem);
                    stack.Push(elem);
                    break;
                }

                case ParseEventKind.ExitRule:
                {
                    if (stack.Count > 0)
                    {
                        var done = stack.Pop();
                        SetSiblingLinks(done);
                        if (lineNumbers) PropagateLineColumn(done);
                    }
                    break;
                }

                // Left-recursive rule support (mirrors ANTLR4 ParserInterpreter behaviour)
                case ParseEventKind.EnterRecursionRule:
                {
                    // Create element but do NOT add to parent yet — parent is saved for ExitRecursionRule.
                    var parent = stack.Count > 0 ? stack.Peek() : null;
                    var elem = MakeRuleElement(ruleNames[ev.Index], ev.Index);
                    if (root == null && parent == null) root = elem;
                    recursionParentStack.Push(parent);
                    stack.Push(elem);
                    break;
                }

                case ParseEventKind.PushRecursionContext:
                {
                    // Wrap the accumulated context as the first child of a fresh element.
                    // Mirrors Parser.PushNewRecursionContext: previous._ctx becomes first child of new _ctx.
                    var prev = stack.Pop();
                    var next = MakeRuleElement(ruleNames[ev.Index], ev.Index);
                    prev.ParentNode = next;
                    next.ChildNodes.Add(prev);
                    stack.Push(next);
                    break;
                }

                case ParseEventKind.ExitRecursionRule:
                {
                    // Mirrors Parser.UnrollRecursionContexts: add accumulated element to saved parent.
                    if (stack.Count > 0)
                    {
                        var done = stack.Pop();
                        SetSiblingLinks(done);
                        if (lineNumbers) PropagateLineColumn(done);
                        var parent = recursionParentStack.Pop();
                        if (parent != null)
                            AddChild(parent, done);
                        else
                            root = done; // outermost recursion rule becomes the root
                    }
                    break;
                }

                case ParseEventKind.Consume:
                {
                    int allIdx = ev.Index;
                    var tok = allTokens[allIdx];
                    var parent = stack.Count > 0 ? stack.Peek() : null;

                    // Emit interleaved hidden tokens as attributes on the parent rule element.
                    if (parent != null)
                        EmitHiddenTokens(parent, allTokens, prevAllIdx, allIdx, symbolicNames, lexerRuleNames, lineNumbers);

                    // Build the terminal element.
                    string tokenName = GetTokenName(tok.Type, symbolicNames, lexerRuleNames);
                    var termElem = MakeTermElement(tokenName);
                    var textNode = new UnvParseTreeText { Data = tok.Type == EOF_TYPE ? "" : (tok.Text ?? "") };
                    textNode.ParentNode = termElem;
                    termElem.ChildNodes.Add(textNode);

                    if (lineNumbers)
                    {
                        AddAttr(termElem, "Line",   tok.Line.ToString());
                        AddAttr(termElem, "Column", tok.Column.ToString());
                    }

                    if (parent != null) AddChild(parent, termElem);
                    prevAllIdx = allIdx;
                    break;
                }
            }
        }

        return root;
    }

    // -----------------------------------------------------------------------

    private static void EmitHiddenTokens(
        UnvParseTreeElement parent,
        IReadOnlyList<LexerToken> allTokens,
        int prevAllIdx,
        int currentAllIdx,
        string[] symbolicNames,
        string[] lexerRuleNames,
        bool lineNumbers)
    {
        for (int j = prevAllIdx + 1; j < currentAllIdx; j++)
        {
            var ht = allTokens[j];
            string attrName = ht.Channel == LexerToken.SKIP_CHANNEL
                ? "Skip"
                : GetTokenName(ht.Type, symbolicNames, lexerRuleNames);
            var attr = new UnvParseTreeAttr
            {
                Name        = attrName,
                StringValue = ht.Text ?? "",
                TokenType   = ht.Type,
                Channel     = ht.Channel,
                ParentNode  = parent,
                OwnerElement = parent
            };
            parent.ChildNodes.Add(attr);

            if (lineNumbers)
            {
                AddAttr(attr, "Line",   ht.Line.ToString());
                AddAttr(attr, "Column", ht.Column.ToString());
            }
        }
    }

    // Copy Line/Column from the first child terminal to the rule element (position 0),
    // matching the ConvertToDOM behaviour when --line is requested.
    private static void PropagateLineColumn(UnvParseTreeElement elem)
    {
        for (int i = 0; i < elem.ChildNodes.Length; i++)
        {
            var child = elem.ChildNodes.item(i) as UnvParseTreeElement;
            if (child == null) continue;

            // Find Line/Column attrs on the child element.
            int insertAt = 0;
            for (int j = 0; j < child.ChildNodes.Length; j++)
            {
                if (child.ChildNodes.item(j) is UnvParseTreeAttr a &&
                    (a.Name as string == "Line" || a.Name as string == "Column"))
                {
                    var copy = new UnvParseTreeAttr
                    {
                        Name         = a.Name,
                        LocalName    = a.LocalName,
                        StringValue  = a.StringValue,
                        ParentNode   = elem,
                        OwnerElement = elem
                    };
                    elem.ChildNodes.Insert(insertAt++, copy);
                }
            }
            break; // only from the first child
        }
    }

    private static UnvParseTreeElement MakeRuleElement(string localName, int ruleIndex)
    {
        var elem = new UnvParseTreeElement
        {
            LocalName  = localName,
            RuleIndex  = ruleIndex,
            ChildNodes = new UnvParseTreeNodeList(),
            Attributes = new AntlrNamedNodeMap()
        };
        return elem;
    }

    private static UnvParseTreeElement MakeTermElement(string localName)
    {
        var elem = new UnvParseTreeElement
        {
            LocalName  = localName,
            RuleIndex  = -1,
            ChildNodes = new UnvParseTreeNodeList(),
            Attributes = new AntlrNamedNodeMap()
        };
        return elem;
    }

    private static void AddChild(UnvParseTreeElement parent, UnvParseTreeElement child)
    {
        child.ParentNode = parent;
        parent.ChildNodes.Add(child);
    }

    private static void AddAttr(UnvParseTreeNode target, string name, string value)
    {
        var attr = new UnvParseTreeAttr
        {
            Name         = name,
            LocalName    = name,
            StringValue  = value,
            ParentNode   = target,
            OwnerElement = target as UnvParseTreeElement
        };
        target.ChildNodes.Add(attr);
    }

    private static void SetSiblingLinks(UnvParseTreeElement elem)
    {
        for (int i = 1; i < elem.ChildNodes.Length; i++)
        {
            var curr = elem.ChildNodes.item(i);
            var prev = elem.ChildNodes.item(i - 1);
            curr.PreviousSibling = prev;
            prev.NextSibling    = curr;
        }
    }

    private static string GetTokenName(int type, string[] symbolicNames, string[] lexerRuleNames)
    {
        if (type == EOF_TYPE) return "EOF";
        if (type >= 0 && type < symbolicNames.Length && symbolicNames[type] != null)
            return symbolicNames[type];
        // Fall back to lexer rule name (mirrors ConvertToDOM: lexer.RuleNames[tokenType])
        if (type >= 0 && type < lexerRuleNames.Length && lexerRuleNames[type] != null)
            return lexerRuleNames[type];
        return "Unknown";
    }
}
