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
            case ParseEventKind.ExitRule:
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
            if (rule.RuleName != frame.Name ||
                rule.Pattern.Length != frame.Children.Count)
                continue;
            var captures = new Dictionary<string, UnvParseTreeNode>(
                StringComparer.Ordinal);
            var matches = true;
            for (var i = 0; i < frame.Children.Count; i++)
            {
                var pattern = rule.Pattern[i];
                var child = frame.Children[i];
                if (pattern.NodeName is not null && pattern.NodeName != child.Name ||
                    pattern.Literal is not null && pattern.Literal != child.Text)
                {
                    matches = false;
                    break;
                }
                if (pattern.Capture is not null)
                    captures.Add(pattern.Capture, child.Node);
            }
            if (matches)
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

    private UnvParseTreeNode Instantiate(Template template,
        IReadOnlyDictionary<string, UnvParseTreeNode> captures)
    {
        if (template is CaptureTemplate capture)
            return captures.TryGetValue(capture.Name, out var value)
                ? value
                : throw new InvalidOperationException(
                    $"Unknown AST capture '{capture.Name}'.");
        if (template is not NodeTemplate node)
            throw new InvalidOperationException("Unknown AST template.");
        var result = Element(node.Kind, _astRuleIndices[node.Kind]);
        if (node.TextCapture is not null)
        {
            if (!captures.TryGetValue(node.TextCapture, out var source))
                throw new InvalidOperationException(
                    $"Unknown AST text capture '{node.TextCapture}'.");
            var sourceText = source is UnvParseTreeElement sourceElement &&
                sourceElement.ChildNodes.Length > 0 &&
                sourceElement.ChildNodes.item(0) is UnvParseTreeText terminalText
                    ? terminalText : null;
            Append(result, Terminal("value",
                UnvParseTreeElement.Reconstruct(source),
                sourceText?.TokenType ?? 0, sourceText?.Channel ?? 0));
        }
        foreach (var child in node.Children)
            Append(result, Instantiate(child, captures));
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
