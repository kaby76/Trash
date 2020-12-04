namespace AntlrJson
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;


    public class ParseTreeConverter : JsonConverter<ParseInfo>
    {

        public ParseTreeConverter()
        {
        }


        private static string Capitalized(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public override bool CanConvert(Type typeToConvert) => true;

        public override ParseInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            MyTokenStream out_token_stream = new MyTokenStream();
            MyLexer lexer = new MyLexer(null);
            MyParser parser = new MyParser(out_token_stream);
            MyCharStream fake_char_stream = new MyCharStream();
            string text = null;

            lexer.InputStream = fake_char_stream;
            if (!(reader.TokenType == JsonTokenType.StartObject)) throw new JsonException();
            reader.Read();
            List<string> mode_names = new List<string>();
            List<string> channel_names = new List<string>();
            List<string> lexer_rule_names = new List<string>();
            List<string> literal_names = new List<string>();
            List<string> symbolic_names = new List<string>();
            Dictionary<string, int> token_type_map = new Dictionary<string, int>();
            List<string> parser_rule_names = new List<string>();
            Dictionary<int, IParseTree> nodes = new Dictionary<int, IParseTree>();
            List<IParseTree> result = new List<IParseTree>();
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
                    text = out_token_stream.Text;
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
                        token.InputStream = lexer.InputStream;
                        token.TokenSource = lexer;
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
                    lexer._modeNames = mode_names.ToArray();
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
                    lexer._channelNames = channel_names.ToArray();
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
                        parser_rule_names.Add(name);
                        reader.Read();
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
                            MyParserRuleContext foo = new MyParserRuleContext(parent_node, 0)
                            {
                                _ruleIndex = type_of_node
                            };
                            nodes[current] = foo;
                            if (parent_node == null) result.Add(foo);
                            parent_node?.AddChild((Antlr4.Runtime.RuleContext)foo);
                        }
                        else
                        {
                            var index = type_of_node - 1000000;
                            var symbol = out_token_stream.Get(index);
                            var foo = new TerminalNodeImpl(symbol);
                            nodes[current] = foo;
                            foo.Parent = parent_node;
                            if (parent_node == null) result.Add(foo);
                            parent_node?.AddChild(foo);
                        }
                        current++;
                    }
                    reader.Read();
                }
                else
                    throw new JsonException();
            }
            var vocab = new Vocabulary(literal_names.ToArray(), symbolic_names.ToArray());
            parser._vocabulary = vocab;
            parser._grammarFileName = fake_char_stream.SourceName;
            parser._ruleNames = parser_rule_names.ToArray();
            lexer._vocabulary = vocab;
            lexer._ruleNames = lexer_rule_names.ToArray();
            lexer._tokenTypeMap = token_type_map;
            var res = new AntlrJson.ParseInfo()
            {
                FileName= fake_char_stream.SourceName,
                Stream = out_token_stream,
                Nodes = result.ToArray(),
                Lexer = lexer,
                Parser = parser,
                Text = text
            };
            return res;
        }

        public override void Write(Utf8JsonWriter writer, ParseInfo tuple, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("FileName");
            writer.WriteStringValue(tuple.FileName);

            writer.WritePropertyName("Text");
            writer.WriteStringValue(tuple.Text);

            writer.WritePropertyName("Tokens");
            writer.WriteStartArray();
            var in_token_stream = tuple.Stream as ITokenStream;
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
            var lexer = tuple.Lexer as Lexer;
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
            var parser = tuple.Parser as Parser;
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
            foreach (var node in tuple.Nodes) stack.Push(node as IParseTree);
            Dictionary<IParseTree, int> preorder = new Dictionary<IParseTree, int>();
            int number = 1;
            while (stack.Any())
            {
                var node = stack.Pop();
                preorder[node] = number++;
                if (node is ParserRuleContext n)
                {
                    if (n.Parent != null)
                    {
                        // Note, the node may have a parent, but the tree that is being serialized may be
                        // a sub tree. If there is no key for the node, write out zero.
                        if (preorder.ContainsKey(n.Parent))
                            writer.WriteNumberValue(preorder[n.Parent]);
                        else
                            writer.WriteNumberValue(0);
                    }
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
                    {
                        if (preorder.ContainsKey(t.Parent))
                            writer.WriteNumberValue(preorder[t.Parent]);
                        else
                            writer.WriteNumberValue(0);
                    }
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

