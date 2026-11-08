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
    private readonly int[] _nextSibling;
    private readonly int _nodeCount;
    private readonly int _edgeCount;
    private readonly int _root;
    private readonly TokenStore _tokens;

    internal CompactParseTree(CompactNode[] nodes, int nodeCount,
        int[] nextSibling, int edgeCount, int root, TokenStore tokens)
    {
        _nodes = nodes;
        _nodeCount = nodeCount;
        _nextSibling = nextSibling;
        _edgeCount = edgeCount;
        _root = root;
        _tokens = tokens;
    }

    public int Count => _root >= 0 ? 1 : 0;
    internal int NodeCount => _nodeCount;
    internal int EdgeCount => _edgeCount;

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
        writer.WriteNumberValue((short)(node.Kind == CompactNodeKind.Terminal
            ? CompactNodeKind.Element : node.Kind));
        switch (node.Kind)
        {
            case CompactNodeKind.Element:
            case CompactNodeKind.Terminal:
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
        if (node.Kind == CompactNodeKind.Terminal)
            WriteTerminalText(writer, node);
        for (var child = node.ChildStart; child >= 0;
             child = _nextSibling[child])
            WriteNode(writer, child);
        writer.WriteEndArray();
        writer.WriteEndArray();
    }

    private void WriteTerminalText(Utf8JsonWriter writer, in CompactNode terminal)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue((short)CompactNodeKind.Text);
        WriteValue(writer, terminal);
        writer.WriteNumberValue(0);
        writer.WriteNumberValue(0);
        writer.WriteStartArray();
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
            CompactNodeKind.Terminal => new UnvParseTreeElement
            {
                LocalName = compact.Name,
                RuleIndex = -1,
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
        if (compact.Kind == CompactNodeKind.Terminal)
        {
            previous = new UnvParseTreeText
            {
                Data = Value(compact),
                Channel = 0,
                TokenType = 0,
                ParentNode = result
            };
            result.ChildNodes.Add(previous);
        }
        for (var childId = compact.ChildStart; childId >= 0;
             childId = _nextSibling[childId])
        {
            var child = MaterializeNode(childId, result);
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
    Attribute = 5,
    // Internally combines the public element and its sole token-text child.
    // Serialization and materialization expand it back to the established
    // two-node representation.
    Terminal = 6
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
        var builder = new StreamingBuilder(allTokens, ruleNames, symbolicNames,
            literalNames, lexerRuleNames, lineNumbers);
        foreach (var ev in events)
            builder.OnEvent(ev);
        return builder.Complete();
    }

    /// <summary>
    /// Receives committed parse events as they occur, without first retaining
    /// an event list. Discard this builder if the parser rejects the input.
    /// </summary>
    public sealed class StreamingBuilder
    {
        private readonly IReadOnlyList<LexerToken> _allTokens;
        private readonly string[] _ruleNames;
        private readonly string[] _symbolicNames;
        private readonly string[] _lexerRuleNames;
        private readonly bool _lineNumbers;
        private readonly Builder _builder;
        private readonly Stack<int> _stack = new();
        private readonly Stack<int> _recursionParents = new();
        private int _root = -1;
        private int _previousToken = -1;

        public StreamingBuilder(
            IReadOnlyList<LexerToken> allTokens,
            string[] ruleNames,
            string[] symbolicNames,
            string[] literalNames,
            string[] lexerRuleNames,
            bool lineNumbers)
        {
            _allTokens = allTokens;
            _ruleNames = ruleNames;
            _symbolicNames = symbolicNames;
            _lexerRuleNames = lexerRuleNames;
            _lineNumbers = lineNumbers;
            _builder = new Builder(allTokens as TokenStore);
        }

        public void OnEvent(ParseEvent ev)
        {
            switch (ev.Kind)
            {
                case ParseEventKind.EnterRule:
                {
                    var id = _builder.Element(_ruleNames[ev.Index], ev.Index);
                    if (_root < 0) _root = id;
                    if (_stack.Count > 0) _builder.AddChild(_stack.Peek(), id);
                    _stack.Push(id);
                    break;
                }
                case ParseEventKind.ExitRule:
                    if (_stack.Count > 0)
                    {
                        var done = _stack.Pop();
                        if (_lineNumbers) PropagateLineColumn(_builder, done);
                    }
                    break;
                case ParseEventKind.EnterRecursionRule:
                {
                    var parent = _stack.Count > 0 ? _stack.Peek() : -1;
                    var id = _builder.Element(_ruleNames[ev.Index], ev.Index);
                    if (_root < 0 && parent < 0) _root = id;
                    _recursionParents.Push(parent);
                    _stack.Push(id);
                    break;
                }
                case ParseEventKind.PushRecursionContext:
                {
                    var previous = _stack.Pop();
                    if (_lineNumbers) PropagateLineColumn(_builder, previous);
                    var next = _builder.Element(_ruleNames[ev.Index], ev.Index);
                    _builder.AddChild(next, previous);
                    _stack.Push(next);
                    break;
                }
                case ParseEventKind.ExitRecursionRule:
                    if (_stack.Count > 0)
                    {
                        var done = _stack.Pop();
                        if (_lineNumbers) PropagateLineColumn(_builder, done);
                        var parent = _recursionParents.Pop();
                        if (parent >= 0) _builder.AddChild(parent, done);
                        else _root = done;
                    }
                    break;
                case ParseEventKind.Consume:
                {
                    var tokenIndex = ev.Index;
                    var token = _allTokens[tokenIndex];
                    var parent = _stack.Count > 0 ? _stack.Peek() : -1;
                    if (parent >= 0)
                        EmitHiddenTokens(_builder, parent, _allTokens, _previousToken,
                            tokenIndex, _symbolicNames, _lexerRuleNames, _lineNumbers);

                    var terminal = _builder.Terminal(
                        GetTokenName(token.Type, _symbolicNames, _lexerRuleNames),
                        token.Type == EofType ? -1 : tokenIndex,
                        token.Type == EofType ? "" : _builder.HasTokenStore
                            ? null : token.Text ?? "");
                    if (_lineNumbers)
                    {
                        _builder.AddChild(terminal, _builder.Attribute("Line", token.Line.ToString(), 0, 0));
                        _builder.AddChild(terminal, _builder.Attribute("Column", token.Column.ToString(), 0, 0));
                    }
                    if (parent >= 0) _builder.AddChild(parent, terminal);
                    _previousToken = tokenIndex;
                    break;
                }
            }
        }

        public CompactParseTree Complete() => _builder.Complete(_root);
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
        private readonly NodeBuffer _nodes = new();
        private int _edgeCount;

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

        public int Terminal(string name, int tokenIndex, string value) =>
            Add(new CompactNode
            {
                Kind = CompactNodeKind.Terminal,
                Name = name,
                Value = value,
                RuleIndex = -1,
                ChildStart = -1,
                TokenIndex = tokenIndex,
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
            => _nodes.Add(node);

        public void AddChild(int parent, int child)
        {
            ref var node = ref _nodes.Nodes[parent];
            if (node.ChildStart < 0) node.ChildStart = child;
            else _nodes.Next[_nodes.Last[parent]] = child;
            _nodes.Last[parent] = child;
            node.ChildCount++;
            _edgeCount++;
        }

        public void AddFirst(int parent, int child)
        {
            ref var node = ref _nodes.Nodes[parent];
            _nodes.Next[child] = node.ChildStart;
            node.ChildStart = child;
            if (_nodes.Last[parent] < 0) _nodes.Last[parent] = child;
            node.ChildCount++;
            _edgeCount++;
        }

        public int FirstElementChild(int id)
        {
            for (var child = _nodes.Nodes[id].ChildStart; child >= 0;
                 child = _nodes.Next[child])
                if (_nodes.Nodes[child].Kind is CompactNodeKind.Element or
                    CompactNodeKind.Terminal)
                    return child;
            return -1;
        }

        public List<CompactNode> LineColumnAttributes(int id)
        {
            var result = new List<CompactNode>(2);
            for (var child = _nodes.Nodes[id].ChildStart; child >= 0;
                 child = _nodes.Next[child])
            {
                var node = _nodes.Nodes[child];
                if (node.Kind == CompactNodeKind.Attribute &&
                    (node.Name == "Line" || node.Name == "Column"))
                    result.Add(node);
            }
            return result;
        }

        public CompactParseTree Complete(int root)
        {
            return new CompactParseTree(
                _nodes.Nodes, _nodes.Count,
                _nodes.Next, _edgeCount, root, _tokens);
        }

        /// <summary>
        /// Grows node and relationship storage together with one capacity
        /// check per node instead of four independent List&lt;T&gt;.Add calls.
        /// The backing arrays transfer directly to CompactParseTree.
        /// </summary>
        private sealed class NodeBuffer
        {
            private const int InitialCapacity = 256;
            public CompactNode[] Nodes = Array.Empty<CompactNode>();
            public int[] Last = Array.Empty<int>();
            public int[] Next = Array.Empty<int>();
            public int Count { get; private set; }

            public int Add(CompactNode node)
            {
                if (Count == Nodes.Length) Grow();
                int id = Count++;
                Nodes[id] = node;
                Last[id] = -1;
                Next[id] = -1;
                return id;
            }

            private void Grow()
            {
                int capacity = Nodes.Length == 0
                    ? InitialCapacity : checked(Nodes.Length * 2);
                Array.Resize(ref Nodes, capacity);
                Array.Resize(ref Last, capacity);
                Array.Resize(ref Next, capacity);
            }
        }
    }
}
