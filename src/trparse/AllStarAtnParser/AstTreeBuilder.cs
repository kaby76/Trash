namespace AllStarAtnParser;

using Atn;
using ParseTreeEditing.UnvParseTreeDOM;

/// <summary>
/// Reduces committed parser events to a DOM-compatible AST at rule exit.
/// No CST or event list is retained. The result is valid only after a
/// successful parse.
/// </summary>
public sealed class AstTreeBuilder
{
    private readonly Mapping _mapping;
    private readonly string[] _parserRuleNames;
    private readonly string[] _tokenNames;
    private readonly IReadOnlyList<LexerToken> _tokens;
    private readonly Stack<Frame> _frames = new();
    private readonly Dictionary<string, int> _astRuleIndices = new(StringComparer.Ordinal);

    public UnvParseTreeNode Root { get; private set; }
    public string[] AstRuleNames { get; }

    public AstTreeBuilder(Mapping mapping, string[] parserRuleNames,
        string[] tokenNames, IReadOnlyList<LexerToken> tokens)
    {
        _mapping = mapping;
        _parserRuleNames = parserRuleNames;
        _tokenNames = tokenNames;
        _tokens = tokens;
        var names = parserRuleNames.ToList();
        foreach (var rule in mapping.Rules)
            RegisterKind(rule.Result, names);
        AstRuleNames = names.ToArray();
    }

    public void OnEvent(ParseEvent evt)
    {
        switch (evt.Kind)
        {
            case ParseEventKind.EnterRule:
            case ParseEventKind.EnterRecursionRule:
                _frames.Push(new Frame(_parserRuleNames[evt.Index]));
                break;
            case ParseEventKind.Consume:
                var token = _tokens[evt.Index];
                var name = token.Type < 0 ? "EOF" :
                    token.Type < _tokenNames.Length &&
                    !string.IsNullOrEmpty(_tokenNames[token.Type])
                        ? _tokenNames[token.Type] : $"token{token.Type}";
                _frames.Peek().Children.Add(new Child(name, token.Text,
                    Terminal(name, token.Text, token.Type, token.Channel)));
                break;
            case ParseEventKind.PushRecursionContext:
                var previous = _frames.Pop();
                if (previous.Name != _parserRuleNames[evt.Index])
                    throw new InvalidOperationException("Mismatched recursive parser rule events.");
                var wrapped = new Frame(previous.Name);
                wrapped.Children.Add(new Child(previous.Name,
                    string.Concat(previous.Children.Select(c => c.Text)),
                    Reduce(previous)));
                _frames.Push(wrapped);
                break;
            case ParseEventKind.ExitRule:
            case ParseEventKind.ExitRecursionRule:
                var frame = _frames.Pop();
                if (frame.Name != _parserRuleNames[evt.Index])
                    throw new InvalidOperationException("Mismatched parser rule events.");
                var node = Reduce(frame);
                if (_frames.Count == 0)
                    Root = node;
                else
                    _frames.Peek().Children.Add(new Child(frame.Name,
                        string.Concat(frame.Children.Select(c => c.Text)), node));
                break;
            default:
                throw new NotSupportedException(
                    $"AST construction does not yet handle {evt.Kind} events.");
        }
    }

    private UnvParseTreeNode Reduce(Frame frame)
    {
        foreach (var rule in _mapping.Rules)
        {
            if (rule.RuleName != frame.Name)
                continue;
            if (TryMatch(rule.Pattern, frame.Children, 0, 0,
                    new Dictionary<string, List<UnvParseTreeNode>>(StringComparer.Ordinal),
                    out var captures))
                return Instantiate(rule.Result, captures);
        }
        if (_mapping.SingleChild && frame.Children.Count == 1)
            return frame.Children[0].Node;
        if (_mapping.ErrorOtherwise)
            throw new InvalidOperationException(
                $"No AST rewrite for {frame.Name}({string.Join(", ", frame.Children.Select(c => c.Name))}).");
        var fallback = Element(frame.Name,
            Array.IndexOf(_parserRuleNames, frame.Name));
        foreach (var child in frame.Children)
            Append(fallback, child.Node);
        return fallback;
    }

    private static bool TryMatch(PatternPart[] pattern, List<Child> children,
        int patternIndex, int childIndex,
        Dictionary<string, List<UnvParseTreeNode>> captures,
        out Dictionary<string, List<UnvParseTreeNode>> result)
    {
        if (patternIndex == pattern.Length)
        {
            result = captures;
            return childIndex == children.Count;
        }
        var part = pattern[patternIndex];
        var available = 0;
        while (available < part.Max && childIndex + available < children.Count &&
               Matches(part, children[childIndex + available]))
            available++;
        for (var count = available; count >= part.Min; count--)
        {
            var next = new Dictionary<string, List<UnvParseTreeNode>>(captures,
                StringComparer.Ordinal);
            if (part.Capture is not null)
            {
                if (next.ContainsKey(part.Capture))
                    throw new InvalidOperationException(
                        $"Duplicate AST capture '{part.Capture}'.");
                next.Add(part.Capture, children.GetRange(childIndex, count)
                    .Select(child => child.Node).ToList());
            }
            if (TryMatch(pattern, children, patternIndex + 1,
                    childIndex + count, next, out result))
                return true;
        }
        result = captures;
        return false;
    }

    private static bool Matches(PatternPart pattern, Child child) =>
        (pattern.NodeName is null or "_" || pattern.NodeName == child.Name) &&
        (pattern.Literal is null || pattern.Literal == child.Text) &&
        (pattern.AstKind is null || child.Node is UnvParseTreeElement element &&
            element.LocalName == pattern.AstKind);

    private UnvParseTreeNode Instantiate(Template template,
        IReadOnlyDictionary<string, List<UnvParseTreeNode>> captures)
    {
        if (template is CaptureTemplate capture)
            return captures.TryGetValue(capture.Name, out var values) && values.Count == 1
                ? values[0]
                : throw new InvalidOperationException(
                    $"AST capture '{capture.Name}' must contain exactly one node.");
        if (template is SpliceTemplate)
            throw new InvalidOperationException("A splice must be inside an AST node.");
        if (template is not NodeTemplate node)
            throw new InvalidOperationException("Unknown AST template.");
        var result = Element(node.Kind, _astRuleIndices[node.Kind]);
        if (node.TextCapture is not null)
        {
            if (!captures.TryGetValue(node.TextCapture, out var sources) ||
                sources.Count == 0)
                throw new InvalidOperationException(
                    $"AST text capture '{node.TextCapture}' must not be empty.");
            var source = sources[0];
            var sourceText = source is UnvParseTreeElement sourceElement &&
                sourceElement.ChildNodes.Length > 0 &&
                sourceElement.ChildNodes.item(0) is UnvParseTreeText terminalText
                    ? terminalText : null;
            Append(result, Terminal("value",
                string.Concat(sources.Select(UnvParseTreeElement.Reconstruct)),
                sourceText?.TokenType ?? 0, sourceText?.Channel ?? 0));
        }
        foreach (var child in node.Children)
        {
            if (child is SpliceTemplate splice)
            {
                if (!captures.TryGetValue(splice.Name, out var sources))
                    throw new InvalidOperationException(
                        $"Unknown AST splice capture '{splice.Name}'.");
                foreach (var source in sources)
                {
                    if (source is not UnvParseTreeElement sourceElement ||
                        sourceElement.LocalName != node.Kind)
                    {
                        Append(result, source);
                        continue;
                    }
                    var sourceChildren = (UnvParseTreeNodeList)sourceElement.ChildNodes;
                    var moved = sourceChildren._node_list
                        .Cast<UnvParseTreeNode>().ToArray();
                    sourceChildren._node_list.Clear();
                    foreach (var item in moved)
                    {
                        item.ParentNode = null;
                        item.PreviousSibling = null;
                        item.NextSibling = null;
                        Append(result, item);
                    }
                }
            }
            else
                Append(result, Instantiate(child, captures));
        }
        return result;
    }

    private void RegisterKind(Template template, List<string> names)
    {
        if (template is not NodeTemplate node) return;
        if (!_astRuleIndices.ContainsKey(node.Kind))
        {
            _astRuleIndices.Add(node.Kind, names.Count);
            names.Add(node.Kind);
        }
        foreach (var child in node.Children)
            RegisterKind(child, names);
    }

    private static UnvParseTreeElement Element(string name, int ruleIndex) =>
        new() { LocalName = name, NodeName = name, RuleIndex = ruleIndex };

    private static UnvParseTreeElement Terminal(string name, string text,
        int tokenType, int channel)
    {
        var element = Element(name, -1);
        var value = new UnvParseTreeText
        {
            Data = text, TokenType = tokenType, Channel = channel
        };
        Append(element, value);
        return element;
    }

    private static void Append(UnvParseTreeElement parent, UnvParseTreeNode child)
    {
        if (child.ParentNode is not null)
            throw new InvalidOperationException(
                "An AST capture was inserted more than once.");
        if (parent.ChildNodes.Length != 0)
        {
            var previous = parent.ChildNodes.item(parent.ChildNodes.Length - 1);
            previous.NextSibling = child;
            child.PreviousSibling = previous;
        }
        parent.ChildNodes.Add(child);
        child.ParentNode = parent;
    }

    private sealed class Frame(string name)
    {
        public string Name { get; } = name;
        public List<Child> Children { get; } = new();
    }

    private sealed record Child(string Name, string Text, UnvParseTreeNode Node);
}
