using System.IO;
using System.Reflection;

namespace AntlrJson.Impl2
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ParseTreeConverter : JsonConverter<IParseTree>
    {

        public class MyCharStream : ICharStream
        {
            public string Text { get; set; }
            public int Index => throw new NotImplementedException();
            public int Size => throw new NotImplementedException();
            public string SourceName { get; set; }
            public void Consume()
            {
                throw new NotImplementedException();
            }

            [return: NotNull]
            public string GetText(Interval interval)
            {
                return this.Text.Substring(interval.a, interval.b - interval.a + 1);
            }

            public int LA(int i)
            {
                throw new NotImplementedException();
            }

            public int Mark()
            {
                throw new NotImplementedException();
            }

            public void Release(int marker)
            {
                throw new NotImplementedException();
            }

            public void Seek(int index)
            {
                throw new NotImplementedException();
            }
        }

        public class MyToken : IToken
        {
            public string Text { get; set; }
            public int Type { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }
            public int Channel { get; set; }
            public int TokenIndex { get; set; }
            public int StartIndex { get; set; }
            public int StopIndex { get; set; }
            public ITokenSource TokenSource { get; set; }
            public ICharStream InputStream { get; set; }
        }

        public class MyTokenStream : ITokenStream
        {
            private ITokenSource _tokenSource;
            protected internal List<IToken> tokens;
            protected internal int n;
            protected internal int p = 0;
            protected internal int numMarkers = 0;
            protected internal IToken lastToken;
            protected internal IToken lastTokenBufferStart;
            protected internal int currentTokenIndex = 0;

            public MyTokenStream()
            {
                this.tokens = new List<IToken>();
                n = 0;
            }

            public MyTokenStream(string t)
            {
                Text = t;
                this.tokens = new List<IToken>();
                n = 0;
            }

            public virtual IToken Get(int i)
            {
                int bufferStartIndex = GetBufferStartIndex();
                if (i < bufferStartIndex || i >= bufferStartIndex + n)
                {
                    throw new ArgumentOutOfRangeException("get(" + i + ") outside buffer: " + bufferStartIndex + ".." + (bufferStartIndex + n));
                }
                return tokens[i - bufferStartIndex];
            }

            public virtual IToken LT(int i)
            {
                if (i == -1)
                {
                    return lastToken;
                }
                Sync(i);
                int index = p + i - 1;
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("LT(" + i + ") gives negative index");
                }
                if (index >= n)
                {
                    System.Diagnostics.Debug.Assert(n > 0 && tokens[n - 1].Type == TokenConstants.EOF);
                    return tokens[n - 1];
                }
                return tokens[index];
            }

            public virtual int LA(int i)
            {
                return LT(i).Type;
            }

            public virtual ITokenSource TokenSource
            {
                get
                {
                    return _tokenSource;
                }
                set
                {
                    _tokenSource = value;
                }
            }

            [return: NotNull]
            public virtual string GetText()
            {
                return string.Empty;
            }

            [return: NotNull]
            public virtual string GetText(RuleContext ctx)
            {
                return GetText(ctx.SourceInterval);
            }

            [return: NotNull]
            public virtual string GetText(IToken start, IToken stop)
            {
                if (start != null && stop != null)
                {
                    return GetText(Interval.Of(start.TokenIndex, stop.TokenIndex));
                }
                throw new NotSupportedException("The specified start and stop symbols are not supported.");
            }

            public virtual void Consume()
            {
                if (LA(1) == TokenConstants.EOF)
                {
                    throw new InvalidOperationException("cannot consume EOF");
                }
                // buf always has at least tokens[p==0] in this method due to ctor
                lastToken = tokens[p];
                // track last token for LT(-1)
                // if we're at last token and no markers, opportunity to flush buffer
                if (p == n - 1 && numMarkers == 0)
                {
                    n = 0;
                    p = -1;
                    // p++ will leave this at 0
                    lastTokenBufferStart = lastToken;
                }
                p++;
                currentTokenIndex++;
                Sync(1);
            }

            protected internal virtual void Sync(int want)
            {
            }

            protected internal virtual int Fill(int n)
            {
                return n;
            }

            protected internal virtual void Add(IToken t)
            {
                if (t is IWritableToken)
                {
                    ((IWritableToken)t).TokenIndex = GetBufferStartIndex() + n;
                }
                tokens.Add(t);
                n++;
            }

            public virtual int Mark()
            {
                if (numMarkers == 0)
                {
                    lastTokenBufferStart = lastToken;
                }
                int mark = -numMarkers - 1;
                numMarkers++;
                return mark;
            }

            public virtual void Release(int marker)
            {
                int expectedMark = -numMarkers;
                if (marker != expectedMark)
                {
                    throw new InvalidOperationException("release() called with an invalid marker.");
                }
                numMarkers--;
                if (numMarkers == 0)
                {
                    // can we release buffer?
                    if (p > 0)
                    {
                        // Copy tokens[p]..tokens[n-1] to tokens[0]..tokens[(n-1)-p], reset ptrs
                        // p is last valid token; move nothing if p==n as we have no valid char
                        //                        System.Array.Copy(tokens, p, tokens, 0, n - p);
                        // shift n-p tokens from p to 0
                        n = n - p;
                        p = 0;
                    }
                    lastTokenBufferStart = lastToken;
                }
            }

            public virtual int Index
            {
                get
                {
                    return currentTokenIndex;
                }
            }

            public virtual void Seek(int index)
            {
                // seek to absolute index
                if (index == currentTokenIndex)
                {
                    return;
                }
                if (index > currentTokenIndex)
                {
                    Sync(index - currentTokenIndex);
                    index = Math.Min(index, GetBufferStartIndex() + n - 1);
                }
                int bufferStartIndex = GetBufferStartIndex();
                int i = index - bufferStartIndex;
                if (i < 0)
                {
                    throw new ArgumentException("cannot seek to negative index " + index);
                }
                else
                {
                    if (i >= n)
                    {
                        throw new NotSupportedException("seek to index outside buffer: " + index + " not in " + bufferStartIndex + ".." + (bufferStartIndex + n));
                    }
                }
                p = i;
                currentTokenIndex = index;
                if (p == 0)
                {
                    lastToken = lastTokenBufferStart;
                }
                else
                {
                    lastToken = tokens[p - 1];
                }
            }

            public virtual int Size
            {
                get
                {
                    return tokens.Count();
                }
            }

            public virtual string SourceName
            {
                get
                {
                    return TokenSource.SourceName;
                }
            }

            public string Text { get; internal set; }

            [return: NotNull]
            public virtual string GetText(Interval interval)
            {
                int bufferStartIndex = GetBufferStartIndex();
                int bufferStopIndex = bufferStartIndex + tokens.Count() - 1;
                int start = interval.a;
                int stop = interval.b;
                if (start < bufferStartIndex || stop > bufferStopIndex)
                {
                    throw new NotSupportedException("interval " + interval + " not in token buffer window: " + bufferStartIndex + ".." + bufferStopIndex);
                }
                int a = start - bufferStartIndex;
                int b = stop - bufferStartIndex;
                StringBuilder buf = new StringBuilder();
                for (int i = a; i <= b; i++)
                {
                    IToken t = tokens[i];
                    buf.Append(t.Text);
                }
                return buf.ToString();
            }

            protected internal int GetBufferStartIndex()
            {
                return currentTokenIndex - p;
            }
        }

        public class MyFakeLexer : Lexer, ITokenSource
        {
            public int Line => throw new NotImplementedException();

            public int Column => throw new NotImplementedException();

            public ICharStream InputStream { get; set; }

            public string SourceName => throw new NotImplementedException();

            public ITokenFactory TokenFactory
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override string[] RuleNames => throw new NotImplementedException();

            public Vocabulary _vocabulary;
            public override IVocabulary Vocabulary
            {
                get { return _vocabulary; }
            }

            public override string GrammarFileName => throw new NotImplementedException();

            public string[] _modeNames;
            public override string[] ModeNames
            {
                get { return _modeNames; }
            }

            public string[] _channelNames;
            public override string[] ChannelNames
            {
                get { return _channelNames; }
            }

            [return: NotNull]
            public IToken NextToken()
            {
                throw new NotImplementedException();
            }

            public MyFakeLexer(ICharStream input) : base(input)
            {
            }

            public MyFakeLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
            {
            }
        }

        string code;
        Lexer lexer;
        Parser parser;
        ITokenStream in_token_stream;

        public ParseTreeConverter(string c, ITokenStream ts, Lexer l, Parser p)
        {
            code = c;
            in_token_stream = ts;
            parser = p;
            lexer = l;
        }


        private static string Capitalized(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(IParseTree).IsAssignableFrom(typeToConvert);

        public override IParseTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            MyTokenStream out_token_stream = new MyTokenStream();
            MyFakeLexer fake_lexer = new MyFakeLexer(null);
            MyCharStream fake_char_stream = new MyCharStream();
            fake_lexer.InputStream = fake_char_stream;
            if (!(reader.TokenType == JsonTokenType.StartObject)) throw new JsonException();
            reader.Read();
            List<string> mode_names = new List<string>();
            List<string> channel_names = new List<string>();
            List<string> lexer_rule_names = new List<string>();
            List<string> literal_names = new List<string>();
            List<string> symbolic_names = new List<string>();
            Dictionary<string, int> token_type_map = new Dictionary<string, int>();
            List<Type> parser_rule_types = new List<Type>();
            List<Type> lexer_rule_types = new List<Type>();
            Dictionary<int, IParseTree> nodes = new Dictionary<int, IParseTree>();
            List<MyTuple<int, string, int, int, int>> tokens = new List<MyTuple<int, string, int, int, int>>();
            while (reader.TokenType == JsonTokenType.PropertyName)
            {
                string pn = reader.GetString();
                reader.Read();
                if (pn == "FileName")
                {
                    var name = reader.GetString();
                    fake_char_stream.SourceName = name;
                    reader.Read();
                }
                else if (pn == "Text")
                {
                    out_token_stream.Text = reader.GetString();
                    fake_char_stream.Text = out_token_stream.Text;
                    reader.Read();
                }
                else if (pn == "Tokens")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.Number)
                    {
                        var type = reader.GetInt32();
                        reader.Read();
                        var start = reader.GetInt32();
                        reader.Read();
                        var stop = reader.GetInt32();
                        reader.Read();
                        var channel = reader.GetInt32();
                        reader.Read();
                        var token = new MyToken();
                        token.Type = type;
                        token.StartIndex = start;
                        token.StopIndex = stop;
                        token.Channel = channel;
                        token.InputStream = fake_lexer.InputStream;
                        token.TokenSource = fake_lexer;
                        token.Text =
                            out_token_stream.Text.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
                        out_token_stream.Add(token);
                    }

                    reader.Read();
                }
                else if (pn == "ModeNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        mode_names.Add(reader.GetString());
                        reader.Read();
                    }

                    reader.Read();
                    fake_lexer._modeNames = mode_names.ToArray();
                }
                else if (pn == "ChannelNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String)
                    {
                        channel_names.Add(reader.GetString());
                        reader.Read();
                    }

                    reader.Read();
                    fake_lexer._channelNames = channel_names.ToArray();
                }
                else if (pn == "LiteralNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        literal_names.Add(reader.GetString());
                        reader.Read();
                    }
                    reader.Read();
                }
                else if (pn == "SymbolicNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        symbolic_names.Add(reader.GetString());
                        reader.Read();
                    }
                    reader.Read();
                }
                else if (pn == "LexerRuleNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        lexer_rule_names.Add(reader.GetString());
                        reader.Read();
                    }
                    reader.Read();
                }
                else if (pn == "ParserRuleNames")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        var name = reader.GetString();
                        lexer_rule_names.Add(name);
                        reader.Read();
                        var typeDiscriminator = Capitalized(name);
                        var all_types = parser.GetType().Assembly.GetTypes();
                        var type = all_types.Where(t => t.Name.StartsWith(typeDiscriminator)).First();
                        parser_rule_types.Add(type);
                    }
                    reader.Read();
                }
                else if (pn == "TokenTypeMap")
                {
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
                    {
                        var name = reader.GetString();
                        reader.Read();
                        var tt = reader.GetInt32();
                        reader.Read();
                        token_type_map[name] = tt;
                    }
                    reader.Read();
                }
                else if (pn == "Nodes")
                {
                    List<IParseTree> list_of_nodes = new List<IParseTree>();
                    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
                    reader.Read();
                    int current = 1;
                    while (reader.TokenType == JsonTokenType.Number)
                    {
                        int parent = reader.GetInt32();
                        reader.Read();
                        int type_of_node = reader.GetInt32();
                        reader.Read();
                        var parent_node = parent > 0 ? nodes[parent] as ParserRuleContext : null;
                        if (type_of_node < 1000000)
                        {
                            Type type = parser_rule_types[type_of_node];
                            var foo = (IParseTree)Activator.CreateInstance(type, new object[] { parent_node, 0 });
                            nodes[current] = foo;
                            parent_node?.AddChild((Antlr4.Runtime.RuleContext)foo);
                        }
                        else
                        {
                            var index = type_of_node - 1000000;
                            var symbol = out_token_stream.Get(index);
                            var foo = new TerminalNodeImpl(symbol);
                            nodes[current] = foo;
                            foo.Parent = parent_node;
                            parent_node?.AddChild(foo);
                        }
                        current++;
                    }
                    reader.Read();
                }
                else
                    throw new JsonException();
            }
            fake_lexer._vocabulary = new Vocabulary(literal_names.ToArray(), symbolic_names.ToArray());
            return nodes[1];

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IParseTree person, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("FileName");
            writer.WriteStringValue(parser.GrammarFileName);

            writer.WritePropertyName("Text");
            writer.WriteStringValue(code);

            writer.WritePropertyName("Tokens");
            writer.WriteStartArray();
            in_token_stream.Seek(0);
            for (int i = 0; ; ++i)
            {
                var token = in_token_stream.Get(i);
                writer.WriteNumberValue(token.Type);
                writer.WriteNumberValue(token.StartIndex);
                writer.WriteNumberValue(token.StopIndex);
                writer.WriteNumberValue(token.Channel);
                if (token.Type == Antlr4.Runtime.TokenConstants.EOF) break;
            }
            writer.WriteEndArray();

            writer.WritePropertyName("ModeNames");
            writer.WriteStartArray();
            foreach (var n in lexer.ModeNames)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("ChannelNames");
            writer.WriteStartArray();
            foreach (var n in lexer.ChannelNames)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("LiteralNames");
            writer.WriteStartArray();
            // ROYAL PAIN IN THE ASS ANTLR HIDING.
            var vocab = lexer.Vocabulary;
            var vocab_type = vocab.GetType();
            FieldInfo myFieldInfo1 = vocab_type.GetField("literalNames",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var literal_names = myFieldInfo1.GetValue(vocab) as string[];
            FieldInfo myFieldInfo2 = vocab_type.GetField("symbolicNames",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var symbolic_names = myFieldInfo2.GetValue(vocab) as string[];
            foreach (var n in literal_names)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("SymbolicNames");
            writer.WriteStartArray();
            foreach (var n in symbolic_names)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("LexerRuleNames");
            writer.WriteStartArray();
            foreach (var n in lexer.RuleNames)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("ParserRuleNames");
            writer.WriteStartArray();
            foreach (var n in parser.RuleNames)
            {
                writer.WriteStringValue(n);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("TokenTypeMap");
            writer.WriteStartArray();
            foreach (var pair in lexer.TokenTypeMap)
            {
                writer.WriteStringValue(pair.Key);
                writer.WriteNumberValue(pair.Value);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("Nodes");
            writer.WriteStartArray();
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(person);
            Dictionary<IParseTree, int> preorder = new Dictionary<IParseTree, int>();
            int number = 1;
            while (stack.Any())
            {
                var node = stack.Pop();
                preorder[node] = number++;
                if (node is ParserRuleContext n)
                {
                    if (n.Parent != null)
                        writer.WriteNumberValue(preorder[n.Parent]);
                    else
                        writer.WriteNumberValue(0);
                    var type = n.RuleIndex;
                    writer.WriteNumberValue(type);
                    if (n.children != null)
                    {
                        foreach (var c in n.children.Reverse())
                        {
                            stack.Push(c);
                        }
                    }
                } else if (node is TerminalNodeImpl t)
                {
                    if (t.Parent != null)
                        writer.WriteNumberValue(preorder[t.Parent]);
                    else
                        writer.WriteNumberValue(0);
                    var type = t.Symbol.TokenIndex + 1000000;
                    writer.WriteNumberValue(type);
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}

