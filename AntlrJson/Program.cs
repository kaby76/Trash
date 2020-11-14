namespace AntlrJson
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;

    public class ParseTreeConverter : JsonConverter<IParseTree>
    {
        Parser parser;

        private static string Capitalized(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public ParseTreeConverter(Parser p) { parser = p; }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(IParseTree).IsAssignableFrom(typeToConvert);

        public override IParseTree Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!(reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.StartArray))
            {
                throw new JsonException();
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }
                string propertyName = reader.GetString();
                if (propertyName == "type")
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException();
                    }

                    string typeDiscriminator = reader.GetString();
                    typeDiscriminator = Capitalized(typeDiscriminator);
                    var all_types = parser.GetType().Assembly.GetTypes();
                    var type = all_types.Where(t => t.Name.StartsWith(typeDiscriminator)).First();
                    var foo = Activator.CreateInstance(type, new object[] { null, 0 });
                    IParseTree person = foo as IParseTree;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            return person;
                        }

                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            propertyName = reader.GetString();
                            reader.Read();
                            switch (propertyName)
                            {
                                case "children":
                                    {
                                        var elements = new List<IParseTree>();
                                        reader.Read();
                                        while (reader.TokenType != JsonTokenType.EndArray)
                                        {
                                            elements.Add(JsonSerializer.Deserialize<IParseTree>(ref reader, options));
                                            if (!reader.Read())
                                            {
                                                throw new JsonException();
                                            }
                                        }
                                        (person as ParserRuleContext).children = elements.ToArray();
                                    }
                                    break;
                            }
                        }
                    }
                }
                else if (propertyName == "tt")
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }
                    var tt = reader.GetInt32();
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException();
                    }
                    var value = reader.GetString();
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                    var index = reader.GetInt32();
                    reader.Read();
                    reader.Read();

                    var token = new CommonToken(tt) { Line = -1, Column = -1, Text = value };
                    var foo = new TerminalNodeImpl(token);
                    IParseTree person = foo as IParseTree;
                    return person;
                }
                throw new JsonException();
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                return null;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IParseTree person, JsonSerializerOptions options)
        {
            if (person is ParserRuleContext internal_node)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                var fixed_name = internal_node.GetType().ToString()
                        .Replace("Antlr4.Runtime.Tree.", "");
                fixed_name = Regex.Replace(fixed_name, "^.*[+]", "");
                fixed_name = fixed_name.Substring(0, fixed_name.Length - "Context".Length);
                fixed_name = fixed_name[0].ToString().ToLower()
                             + fixed_name.Substring(1);
                writer.WriteStringValue(fixed_name);
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var c in internal_node.children)
                {
                    Write(writer, c, options);
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else if (person is TerminalNodeImpl terminal_node)
            {
                if (terminal_node.Symbol.Type == -1) return;
                writer.WriteStartObject();
                writer.WritePropertyName("tt");
                writer.WriteNumberValue(terminal_node.Symbol.Type);
                writer.WritePropertyName("value");
                writer.WriteStringValue(terminal_node.GetText());
                writer.WritePropertyName("index");
                writer.WriteNumberValue(terminal_node.Symbol.TokenIndex);
                writer.WriteEndObject();
            }
        }
    }

    public class TokenConverter : JsonConverter<IToken>
    {
        public class MyTokenSource : ITokenSource
        {
            public int Line { get; set; }

            public int Column { get; set; }

            public ICharStream InputStream { get; set; }

            public string SourceName { get; set; }

            public ITokenFactory TokenFactory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [return: NotNull]
            public IToken NextToken()
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(IToken).IsAssignableFrom(typeToConvert);

        public override IToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!(reader.TokenType == JsonTokenType.StartObject))
            {
                throw new JsonException();
            }

            Dictionary<string, object> tuple = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    CommonToken token = new CommonToken(
                        new Tuple<ITokenSource, ICharStream>(new MyTokenSource(), null),
                        (int)tuple["tt"],
                        (int)tuple["ch"],
                        (int)tuple["st"],
                        (int)tuple["en"]);
                    token.TokenIndex = (int)tuple["in"];
                    return token;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "tx":
                        tuple[propertyName] = reader.GetString();
                        break;
                    case "li":
                    case "tt":
                    case "co":
                    case "ch":
                    case "in":
                    case "st":
                    case "en":
                        tuple[propertyName] = reader.GetInt32();
                        break;
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IToken person, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("tt");
            writer.WriteNumberValue(person.Type);
            writer.WritePropertyName("tx");
            writer.WriteStringValue(person.Text);
            writer.WritePropertyName("li");
            writer.WriteNumberValue(person.Line);
            writer.WritePropertyName("co");
            writer.WriteNumberValue(person.Column);
            writer.WritePropertyName("ch");
            writer.WriteNumberValue(person.Channel);
            writer.WritePropertyName("in");
            writer.WriteNumberValue(person.TokenIndex);
            writer.WritePropertyName("st");
            writer.WriteNumberValue(person.StartIndex);
            writer.WritePropertyName("en");
            writer.WriteNumberValue(person.StopIndex);
            writer.WriteEndObject();
        }
    }

    public class TokenStreamConverter : JsonConverter<ITokenStream>
    {
        public class MyTokenStream : ITokenStream
        {
            private ITokenSource _tokenSource;

            /// <summary>A moving window buffer of the data being scanned.</summary>
            /// <remarks>
            /// A moving window buffer of the data being scanned. While there's a marker,
            /// we keep adding to buffer. Otherwise,
            /// <see cref="Consume()">consume()</see>
            /// resets so
            /// we start filling at index 0 again.
            /// </remarks>
            protected internal List<IToken> tokens;

            /// <summary>
            /// The number of tokens currently in
            /// <see cref="tokens">tokens</see>
            /// .
            /// <p>This is not the buffer capacity, that's
            /// <c>tokens.length</c>
            /// .</p>
            /// </summary>
            protected internal int n;

            /// <summary>
            /// 0..n-1 index into
            /// <see cref="tokens">tokens</see>
            /// of next token.
            /// <p>The
            /// <c>LT(1)</c>
            /// token is
            /// <c>tokens[p]</c>
            /// . If
            /// <c>p == n</c>
            /// , we are
            /// out of buffered tokens.</p>
            /// </summary>
            protected internal int p = 0;

            /// <summary>
            /// Count up with
            /// <see cref="Mark()">mark()</see>
            /// and down with
            /// <see cref="Release(int)">release()</see>
            /// . When we
            /// <c>release()</c>
            /// the last mark,
            /// <c>numMarkers</c>
            /// reaches 0 and we reset the buffer. Copy
            /// <c>tokens[p]..tokens[n-1]</c>
            /// to
            /// <c>tokens[0]..tokens[(n-1)-p]</c>
            /// .
            /// </summary>
            protected internal int numMarkers = 0;

            /// <summary>
            /// This is the
            /// <c>LT(-1)</c>
            /// token for the current position.
            /// </summary>
            protected internal IToken lastToken;

            /// <summary>
            /// When
            /// <c>numMarkers &gt; 0</c>
            /// , this is the
            /// <c>LT(-1)</c>
            /// token for the
            /// first token in
            /// <see cref="tokens"/>
            /// . Otherwise, this is
            /// <see langword="null"/>
            /// .
            /// </summary>
            protected internal IToken lastTokenBufferStart;

            /// <summary>Absolute token index.</summary>
            /// <remarks>
            /// Absolute token index. It's the index of the token about to be read via
            /// <c>LT(1)</c>
            /// . Goes from 0 to the number of tokens in the entire stream,
            /// although the stream size is unknown before the end is reached.
            /// <p>This value is used to set the token indexes if the stream provides tokens
            /// that implement
            /// <see cref="IWritableToken"/>
            /// .</p>
            /// </remarks>
            protected internal int currentTokenIndex = 0;

            public MyTokenStream()
            {
                this.tokens = new List<IToken>();
                n = 0;
            }

            public MyTokenStream(ITokenSource tokenSource, int bufferSize)
            {
                this.tokens = new List<IToken>();
                n = 0;
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

            /// <summary>
            /// Make sure we have 'need' elements from current position
            /// <see cref="p">p</see>
            /// . Last valid
            /// <c>p</c>
            /// index is
            /// <c>tokens.length-1</c>
            /// .
            /// <c>p+need-1</c>
            /// is the tokens index 'need' elements
            /// ahead.  If we need 1 element,
            /// <c>(p+1-1)==p</c>
            /// must be less than
            /// <c>tokens.length</c>
            /// .
            /// </summary>
            protected internal virtual void Sync(int want)
            {
            }

            /// <summary>
            /// Add
            /// <paramref name="n"/>
            /// elements to the buffer. Returns the number of tokens
            /// actually added to the buffer. If the return value is less than
            /// <paramref name="n"/>
            /// ,
            /// then EOF was reached before
            /// <paramref name="n"/>
            /// tokens could be added.
            /// </summary>
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

            /// <summary>Return a marker that we can release later.</summary>
            /// <remarks>
            /// Return a marker that we can release later.
            /// <p>The specific marker value used for this class allows for some level of
            /// protection against misuse where
            /// <c>seek()</c>
            /// is called on a mark or
            /// <c>release()</c>
            /// is called in the wrong order.</p>
            /// </remarks>
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

        public override bool CanConvert(Type typeToConvert) =>
            typeof(ITokenStream).IsAssignableFrom(typeToConvert);

        public override ITokenStream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!(reader.TokenType == JsonTokenType.StartObject))
            {
                throw new JsonException();
            }

            var token_stream = new MyTokenStream();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return token_stream;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "tokens":
                        reader.Read();
                        {
                            var elements = new List<IToken>();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                token_stream.Add(JsonSerializer.Deserialize<IToken>(ref reader, options));
                                if (!reader.Read())
                                {
                                    throw new JsonException();
                                }
                            }
                        }
                        break;
                    default:
                        throw new JsonException();
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, ITokenStream person, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("tokens");
            writer.WriteStartArray();
            person.Seek(0);
            for (int i = 0; ; ++i)
            {
                var token = person.Get(i);
                if (token.Type == Antlr4.Runtime.TokenConstants.EOF) break;
                Write(writer, token, options);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public void Write(Utf8JsonWriter writer, IToken person, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("tt");
            writer.WriteNumberValue(person.Type);
            writer.WritePropertyName("tx");
            writer.WriteStringValue(person.Text);
            writer.WritePropertyName("li");
            writer.WriteNumberValue(person.Line);
            writer.WritePropertyName("co");
            writer.WriteNumberValue(person.Column);
            writer.WritePropertyName("ch");
            writer.WriteNumberValue(person.Channel);
            writer.WritePropertyName("in");
            writer.WriteNumberValue(person.TokenIndex);
            writer.WritePropertyName("st");
            writer.WriteNumberValue(person.StartIndex);
            writer.WritePropertyName("en");
            writer.WriteNumberValue(person.StopIndex);
            writer.WriteEndObject();
        }
    }

    //private static string ToLiteral(string input)
    //{
    //    var literal = input;
    //    literal = literal.Replace("\\", "\\\\");
    //    literal = literal.Replace("\b", "\\b");
    //    literal = literal.Replace("\n", "\\n");
    //    literal = literal.Replace("\t", "\\t");
    //    literal = literal.Replace("\r", "\\r");
    //    literal = literal.Replace("\f", "\\f");
    //    literal = literal.Replace("\"", "\\\"");
    //    literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
    //    return literal;
    //}
    //public static string PerformEscapes(string s)
    //{
    //    StringBuilder new_s = new StringBuilder();
    //    new_s.Append(ToLiteral(s));
    //    return new_s.ToString();
    //}

    //static void Try(string input)
    //{
    //    var str = new AntlrInputStream(input);
    //    System.Console.WriteLine(input);
    //    var lexer = new arithmeticLexer(str);
    //    var tokens = new CommonTokenStream(lexer);
    //    var parser = new arithmeticParser(tokens);
    //    var listener_lexer = new ErrorListener<int>();
    //    var listener_parser = new ErrorListener<IToken>();
    //    lexer.AddErrorListener(listener_lexer);
    //    parser.AddErrorListener(listener_parser);
    //    var tree = parser.file();
    //    if (listener_lexer.had_error || listener_parser.had_error)
    //    {
    //        System.Console.WriteLine("error in parse.");
    //        return;
    //    }
    //    else
    //    {
    //        System.Console.WriteLine("parse completed.");
    //    }
    //    var serializeOptions = new JsonSerializerOptions();
    //    serializeOptions.Converters.Add(new ParseTreeConverter(parser));
    //    serializeOptions.Converters.Add(new TokenConverter());
    //    serializeOptions.Converters.Add(new TokenStreamConverter());
    //    serializeOptions.WriteIndented = true;
    //    string js1 = JsonSerializer.Serialize(tree, serializeOptions);
    //    string js2 = JsonSerializer.Serialize(tokens, serializeOptions);
    //    var obj1 = JsonSerializer.Deserialize<IParseTree>(js1, serializeOptions);
    //    var obj2 = JsonSerializer.Deserialize<ITokenStream>(js2, serializeOptions);
    //}
}

