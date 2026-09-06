namespace Atn;

using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Text;
using System.Text.Json;

/// <summary>
/// Immutable, indexed representation of an interpreted parse tree. Names and
/// token text are retained by reference; relationships are integer indexes.
/// The mutable DOM is produced only when an in-process legacy consumer asks
/// for ParsingResultSet.Nodes.
/// </summary>
public sealed class CompactParseTree : IParsingResultNodeProvider
{
    private readonly CompactNode[] _nodes;
    private readonly int[] _children;
    private readonly int _root;
    private readonly TokenStore _tokens;

    internal CompactParseTree(CompactNode[] nodes, int[] children, int root,
        TokenStore tokens)
    {
        _nodes = nodes;
        _children = children;
        _root = root;
        _tokens = tokens;
    }

    public int Count => _root >= 0 ? 1 : 0;
    internal int NodeCount => _nodes.Length;
    internal int EdgeCount => _children.Length;

    public void WriteNodes(Utf8JsonWriter writer)
    {
        writer.WriteStartArray();
        if (_root >= 0) WriteNode(writer, _root);
        writer.WriteEndArray();
    }

    public UnvParseTreeNode[] Materialize()
    {
        if (_root < 0) return [];
        return [MaterializeNode(_root, null)];
    }

    private void WriteNode(Utf8JsonWriter writer, int id)
    {
        ref readonly var node = ref _nodes[id];
        writer.WriteStartArray();
        writer.WriteNumberValue((short)node.Kind);
        switch (node.Kind)
        {
            case CompactNodeKind.Element:
                writer.WriteNumberValue(node.RuleIndex);
                writer.WriteStringValue(node.Name);
                break;
            case CompactNodeKind.Attribute:
                writer.WriteStringValue(node.Name);
                WriteValue(writer, node);
                writer.WriteNumberValue(node.Channel);
                writer.WriteNumberValue(node.TokenType);
                break;
            case CompactNodeKind.Text:
                WriteValue(writer, node);
                writer.WriteNumberValue(node.Channel);
                writer.WriteNumberValue(node.TokenType);
                break;
            default:
                throw new InvalidOperationException($"Unknown compact node kind {node.Kind}.");
        }

        writer.WriteStartArray();
        for (var i = 0; i < node.ChildCount; i++)
            WriteNode(writer, _children[node.ChildStart + i]);
        writer.WriteEndArray();
        writer.WriteEndArray();
    }

    private void WriteValue(Utf8JsonWriter writer, in CompactNode node)
    {
        if (_tokens != null && node.TokenIndex >= 0)
            writer.WriteStringValue(_tokens.GetTextSpan(node.TokenIndex));
        else if (_tokens != null && node.SourceStart >= 0)
            writer.WriteStringValue(_tokens.GetSourceSpan(node.SourceStart, node.SourceLength));
        else
            writer.WriteStringValue(node.Value);
    }

    private string Value(in CompactNode node)
    {
        if (_tokens != null && node.TokenIndex >= 0)
            return _tokens.GetText(node.TokenIndex);
        if (_tokens != null && node.SourceStart >= 0)
            return _tokens.GetSourceSpan(node.SourceStart, node.SourceLength).ToString();
        return node.Value;
    }

    private UnvParseTreeNode MaterializeNode(int id, UnvParseTreeNode parent)
    {
        ref readonly var compact = ref _nodes[id];
        UnvParseTreeNode result = compact.Kind switch
        {
            CompactNodeKind.Element => new UnvParseTreeElement
            {
                LocalName = compact.Name,
                RuleIndex = compact.RuleIndex,
                ChildNodes = new UnvParseTreeNodeList(),
                Attributes = new AntlrNamedNodeMap()
            },
            CompactNodeKind.Attribute => new UnvParseTreeAttr
            {
                Name = compact.Name,
                StringValue = Value(compact),
                Channel = compact.Channel,
                TokenType = compact.TokenType
            },
            CompactNodeKind.Text => new UnvParseTreeText
            {
                Data = Value(compact),
                Channel = compact.Channel,
                TokenType = compact.TokenType
            },
            _ => throw new InvalidOperationException()
        };
        result.ParentNode = parent;
        if (result is UnvParseTreeAttr attr)
            attr.OwnerElement = parent as UnvParseTreeElement;

        UnvParseTreeNode previous = null;
        for (var i = 0; i < compact.ChildCount; i++)
        {
            var child = MaterializeNode(_children[compact.ChildStart + i], result);
            result.ChildNodes.Add(child);
            if (previous != null)
            {
                previous.NextSibling = child;
                child.PreviousSibling = previous;
            }
            previous = child;
        }
        return result;
    }
}

internal enum CompactNodeKind : short
{
    Text = 1,
    Element = 2,
    Attribute = 5
}

internal struct CompactNode
{
    public CompactNodeKind Kind;
    public int RuleIndex;
    public string Name;
    public string Value;
    public int Channel;
    public int TokenType;
    public int ChildStart;
    public int ChildCount;
    public int TokenIndex;
    public int SourceStart;
    public int SourceLength;
}

/// <summary>Builds a compact tree directly from parser events.</summary>
public static class CompactTreeBuilder
{
    private const int DefaultChannel = 0;
    private const int EofType = -1;

    public static CompactParseTree Build(
        IReadOnlyList<ParseEvent> events,
        IReadOnlyList<LexerToken> allTokens,
        string[] ruleNames,
        string[] symbolicNames,
        string[] literalNames,
        string[] lexerRuleNames,
        bool lineNumbers)
    {
        var builder = new Builder(allTokens as TokenStore);
        var stack = new Stack<int>();
        var recursionParents = new Stack<int>();
        var root = -1;
        var previousToken = -1;

        foreach (var ev in events)
        {
            switch (ev.Kind)
            {
                case ParseEventKind.EnterRule:
                {
                    var id = builder.Element(ruleNames[ev.Index], ev.Index);
                    if (root < 0) root = id;
                    if (stack.Count > 0) builder.AddChild(stack.Peek(), id);
                    stack.Push(id);
                    break;
                }
                case ParseEventKind.ExitRule:
                    if (stack.Count > 0)
                    {
                        var done = stack.Pop();
                        if (lineNumbers) PropagateLineColumn(builder, done);
                        builder.Finish(done);
                    }
                    break;
                case ParseEventKind.EnterRecursionRule:
                {
                    var parent = stack.Count > 0 ? stack.Peek() : -1;
                    var id = builder.Element(ruleNames[ev.Index], ev.Index);
                    if (root < 0 && parent < 0) root = id;
                    recursionParents.Push(parent);
                    stack.Push(id);
                    break;
                }
                case ParseEventKind.PushRecursionContext:
                {
                    var previous = stack.Pop();
                    if (lineNumbers) PropagateLineColumn(builder, previous);
                    builder.Finish(previous);
                    var next = builder.Element(ruleNames[ev.Index], ev.Index);
                    builder.AddChild(next, previous);
                    stack.Push(next);
                    break;
                }
                case ParseEventKind.ExitRecursionRule:
                    if (stack.Count > 0)
                    {
                        var done = stack.Pop();
                        if (lineNumbers) PropagateLineColumn(builder, done);
                        builder.Finish(done);
                        var parent = recursionParents.Pop();
                        if (parent >= 0) builder.AddChild(parent, done);
                        else root = done;
                    }
                    break;
                case ParseEventKind.Consume:
                {
                    var tokenIndex = ev.Index;
                    var token = allTokens[tokenIndex];
                    var parent = stack.Count > 0 ? stack.Peek() : -1;
                    if (parent >= 0)
                        EmitHiddenTokens(builder, parent, allTokens, previousToken,
                            tokenIndex, symbolicNames, lexerRuleNames, lineNumbers);

                    var terminal = builder.Element(
                        GetTokenName(token.Type, symbolicNames, lexerRuleNames), -1);
                    builder.AddChild(terminal, token.Type == EofType
                        ? builder.Text("")
                        : builder.HasTokenStore
                            ? builder.TextToken(tokenIndex)
                            : builder.Text(token.Text ?? ""));
                    if (lineNumbers)
                    {
                        builder.AddChild(terminal, builder.Attribute("Line", token.Line.ToString(), 0, 0));
                        builder.AddChild(terminal, builder.Attribute("Column", token.Column.ToString(), 0, 0));
                    }
                    builder.Finish(terminal);
                    if (parent >= 0) builder.AddChild(parent, terminal);
                    previousToken = tokenIndex;
                    break;
                }
            }
        }

        return builder.Complete(root);
    }

    private static void EmitHiddenTokens(
        Builder builder, int parent, IReadOnlyList<LexerToken> tokens,
        int previous, int current, string[] symbolicNames,
        string[] lexerRuleNames, bool lineNumbers)
    {
        for (var j = previous + 1; j < current; j++)
        {
            var token = tokens[j];
            int attr;
            if (token.Channel == LexerToken.SKIP_CHANNEL)
            {
                int first = j;
                StringBuilder text = builder.HasTokenStore ? null : new(token.Text ?? "");
                while (j + 1 < current &&
                       tokens[j + 1].Channel == LexerToken.SKIP_CHANNEL &&
                       tokens[j + 1].StartIndex == tokens[j].StopIndex + 1)
                {
                    j++;
                    text?.Append(tokens[j].Text ?? "");
                }
                attr = builder.HasTokenStore
                    ? builder.AttributeSource("Skip", token.StartIndex,
                        tokens[j].StopIndex - token.StartIndex + 1, -1, -1)
                    : builder.Attribute("Skip", text.ToString(), -1, -1);
            }
            else
            {
                var name = GetTokenName(token.Type, symbolicNames, lexerRuleNames);
                attr = builder.HasTokenStore
                    ? builder.AttributeToken(name, j, token.Channel, token.Type)
                    : builder.Attribute(name, token.Text ?? "", token.Channel, token.Type);
            }
            if (lineNumbers)
            {
                builder.AddChild(attr, builder.Attribute("Line", token.Line.ToString(), 0, 0));
                builder.AddChild(attr, builder.Attribute("Column", token.Column.ToString(), 0, 0));
            }
            builder.Finish(attr);
            builder.AddChild(parent, attr);
        }
    }

    private static void PropagateLineColumn(Builder builder, int element)
    {
        var child = builder.FirstElementChild(element);
        if (child < 0) return;
        var attributes = builder.LineColumnAttributes(child);
        for (var i = attributes.Count - 1; i >= 0; i--)
        {
            var source = attributes[i];
            builder.AddFirst(element,
                builder.Attribute(source.Name, source.Value, 0, 0));
        }
    }

    private static string GetTokenName(int type, string[] symbolicNames, string[] lexerRuleNames)
    {
        if (type == EofType) return "EOF";
        if (type >= 0 && type < symbolicNames.Length && symbolicNames[type] != null)
            return symbolicNames[type];
        if (type >= 0 && type < lexerRuleNames.Length && lexerRuleNames[type] != null)
            return lexerRuleNames[type];
        return "Unknown";
    }

    private sealed class Builder
    {
        private readonly TokenStore _tokens;
        private readonly List<CompactNode> _nodes = [];
        private readonly List<int> _first = [];
        private readonly List<int> _last = [];
        private readonly List<int> _next = [];
        private readonly List<int> _children = [];

        public Builder(TokenStore tokens) => _tokens = tokens;
        public bool HasTokenStore => _tokens != null;

        public int Element(string name, int ruleIndex) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Element,
            Name = name,
            RuleIndex = ruleIndex,
            ChildStart = -1,
            TokenIndex = -1,
            SourceStart = -1
        });

        public int Attribute(string name, string value, int channel, int tokenType) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Attribute,
            Name = name,
            Value = value,
            Channel = channel,
            TokenType = tokenType,
            RuleIndex = -1,
            ChildStart = -1,
            TokenIndex = -1,
            SourceStart = -1
        });

        public int AttributeToken(string name, int tokenIndex, int channel, int tokenType) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Attribute, Name = name, Channel = channel,
            TokenType = tokenType, RuleIndex = -1, ChildStart = -1,
            TokenIndex = tokenIndex, SourceStart = -1
        });

        public int AttributeSource(string name, int start, int length, int channel, int tokenType) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Attribute, Name = name, Channel = channel,
            TokenType = tokenType, RuleIndex = -1, ChildStart = -1,
            TokenIndex = -1, SourceStart = start, SourceLength = length
        });

        public int Text(string value) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Text,
            Value = value,
            RuleIndex = -1,
            ChildStart = -1,
            TokenIndex = -1,
            SourceStart = -1
        });

        public int TextToken(int tokenIndex) => Add(new CompactNode
        {
            Kind = CompactNodeKind.Text, RuleIndex = -1, ChildStart = -1,
            TokenIndex = tokenIndex, SourceStart = -1
        });

        private int Add(CompactNode node)
        {
            var id = _nodes.Count;
            _nodes.Add(node);
            _first.Add(-1);
            _last.Add(-1);
            _next.Add(-1);
            return id;
        }

        public void AddChild(int parent, int child)
        {
            if (_first[parent] < 0) _first[parent] = child;
            else _next[_last[parent]] = child;
            _last[parent] = child;
        }

        public void AddFirst(int parent, int child)
        {
            _next[child] = _first[parent];
            _first[parent] = child;
            if (_last[parent] < 0) _last[parent] = child;
        }

        public void Finish(int id)
        {
            var node = _nodes[id];
            if (node.ChildStart >= 0) return;
            node.ChildStart = _children.Count;
            var child = _first[id];
            while (child >= 0)
            {
                _children.Add(child);
                node.ChildCount++;
                child = _next[child];
            }
            _nodes[id] = node;
        }

        public int FirstElementChild(int id)
        {
            for (var child = _first[id]; child >= 0; child = _next[child])
                if (_nodes[child].Kind == CompactNodeKind.Element) return child;
            return -1;
        }

        public List<CompactNode> LineColumnAttributes(int id)
        {
            var result = new List<CompactNode>(2);
            for (var child = _first[id]; child >= 0; child = _next[child])
            {
                var node = _nodes[child];
                if (node.Kind == CompactNodeKind.Attribute &&
                    (node.Name == "Line" || node.Name == "Column"))
                    result.Add(node);
            }
            return result;
        }

        public CompactParseTree Complete(int root)
        {
            if (root >= 0) Finish(root);
            return new CompactParseTree(
                _nodes.ToArray(), _children.ToArray(), root, _tokens);
        }
    }
}
