namespace AntlrJson
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using AntlrTreeEditing.AntlrDOM;
    using EditableAntlrTree;
    using org.w3c.dom;
    using Antlr4.Runtime;

    public class ParseTreeConverter : JsonConverter<ParsingResultSet[]>
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

        //public override ParsingResultSet[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //    reader.Read();
        //    List<ParsingResultSet> results = new List<ParsingResultSet>();
        //    while (reader.TokenType == JsonTokenType.StartObject)
        //    {
        //        if (!(reader.TokenType == JsonTokenType.StartObject)) throw new JsonException();
        //        reader.Read();
        //        string file_name = "";
        //        string text = null;
        //        string parser_grammarFileName = null;
        //        string lexer_grammarFileName = null;
        //        List<string> mode_names = new List<string>();
        //        List<string> channel_names = new List<string>();
        //        List<string> lexer_rule_names = new List<string>();
        //        List<string> literal_names = new List<string>();
        //        List<string> symbolic_names = new List<string>();
        //        Dictionary<string, int> token_type_map = new Dictionary<string, int>();
        //        List<string> parser_rule_names = new List<string>();
        //        Dictionary<int, AntlrNode> nodes = new Dictionary<int, AntlrNode>();
        //        List<MyToken> list_of_tokens = new List<MyToken>();
        //        List<AntlrNode> result = new List<AntlrNode>();
        //        List<int> parents = new List<int>();
        //        List<int> type_of_nodes = new List<int>();
        //        while (reader.TokenType == JsonTokenType.PropertyName)
        //        {
        //            string pn = reader.GetString();
        //            reader.Read();
        //            if (pn == "FileName")
        //            {
        //                file_name = reader.GetString();
        //                reader.Read();
        //            }
        //            else if (pn == "Text")
        //            {
        //                text = reader.GetString();
        //                reader.Read();
        //            }
        //            else if (pn == "IdentityOfParser")
        //            {
        //                parser_grammarFileName = reader.GetString();
        //                reader.Read();
        //            }
        //            else if (pn == "IdentityOfLexer")
        //            {
        //                lexer_grammarFileName = reader.GetString();
        //                reader.Read();
        //            }
        //            else if (pn == "Tokens")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                int token_index = 0;
        //                while (reader.TokenType == JsonTokenType.Number)
        //                {
        //                    var type = reader.GetInt32();
        //                    reader.Read();
        //                    var start = reader.GetInt32();
        //                    reader.Read();
        //                    var stop = reader.GetInt32();
        //                    reader.Read();
        //                    var line = reader.GetInt32();
        //                    reader.Read();
        //                    var column = reader.GetInt32();
        //                    reader.Read();
        //                    var channel = reader.GetInt32();
        //                    reader.Read();
        //                    var token = new MyToken();
        //                    token.Type = type;
        //                    token.StartIndex = start;
        //                    token.StopIndex = stop;
        //                    token.Line = line;
        //                    token.Column = column;
        //                    token.Channel = channel;
        //                    token.TokenIndex = token_index++;
        //                    list_of_tokens.Add(token);
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "ModeNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    mode_names.Add(reader.GetString());
        //                    reader.Read();
        //                }

        //                reader.Read();
        //            }
        //            else if (pn == "ChannelNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String)
        //                {
        //                    channel_names.Add(reader.GetString());
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "LiteralNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    literal_names.Add(reader.GetString());
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "SymbolicNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    symbolic_names.Add(reader.GetString());
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "LexerRuleNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    lexer_rule_names.Add(reader.GetString());
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "ParserRuleNames")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    var name = reader.GetString();
        //                    parser_rule_names.Add(name);
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "TokenTypeMap")
        //            {
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Null)
        //                {
        //                    var name = reader.GetString();
        //                    reader.Read();
        //                    var tt = reader.GetInt32();
        //                    reader.Read();
        //                    token_type_map[name] = tt;
        //                }
        //                reader.Read();
        //            }
        //            else if (pn == "Nodes")
        //            {
        //                List<AntlrNode> list_of_nodes = new List<AntlrNode>();
        //                if (!(reader.TokenType == JsonTokenType.StartArray)) throw new JsonException();
        //                reader.Read();
        //                while (reader.TokenType == JsonTokenType.Number)
        //                {
        //                    int parent = reader.GetInt32();
        //                    parents.Add(parent);
        //                    reader.Read();
        //                    int type_of_node = reader.GetInt32();
        //                    type_of_nodes.Add(type_of_node);
        //                    reader.Read();
        //                }
        //                reader.Read();
        //            }
        //            else
        //                throw new JsonException();
        //        }
        //        if (!(reader.TokenType == JsonTokenType.EndObject)) throw new JsonException();
        //        reader.Read();
        //        var vocab = new Vocabulary(literal_names.ToArray(), symbolic_names.ToArray());
        //        MyLexer lexer = new MyLexer(null);
        //        MyParser parser = new MyParser();
        //        foreach (var t in list_of_tokens)
        //        {
        //            t.TokenSource = lexer;
        //        }
        //        parser._vocabulary = vocab;
        //        parser._grammarFileName = parser_grammarFileName;
        //        parser._ruleNames = parser_rule_names.ToArray();
        //        lexer._vocabulary = vocab;
        //        lexer._grammarFileName = lexer_grammarFileName;
        //        lexer._ruleNames = lexer_rule_names.ToArray();
        //        lexer._tokenTypeMap = token_type_map;
        //        lexer._modeNames = mode_names.ToArray();
        //        lexer._channelNames = channel_names.ToArray();
        //        int current = 1;
        //        for (int k = 0; k < parents.Count; ++k)
        //        {
        //            var parent = parents[k];
        //            var type_of_node = type_of_nodes[k];
        //            var parent_node = parent > 0 ? nodes[parent] : null;
        //            if (type_of_node < 1000000)
        //            {
        //                AntlrNode foo = new AntlrNode()
        //                {
        //                    ParentNode = parent_node,
        //                    _ruleIndex = type_of_node
        //                };
        //                nodes[current] = foo;
        //                if (parent_node == null)
        //                {
        //                    result.Add(foo);
        //                }
        //                else
        //                {
        //                    (parent_node.ChildNodes as AntlrNodeList).Add(foo);
        //                }
        //            }
        //            else
        //            {
        //                var index = type_of_node - 1000000;
        //                var symbol = out_token_stream.Get(index);
        //                var foo = new MyTerminalNodeImpl(symbol);
        //                nodes[current] = foo;
        //                foo.Parent = parent_node;
        //                if (parent_node == null)
        //                {
        //                    result.Add(foo);
        //                }
        //                else
        //                {
        //                    parent_node.AddChild(foo);
        //                }
        //            }
        //            current++;
        //        }
        //        var res = new AntlrJson.ParsingResultSet()
        //        {
        //            FileName = file_name,
        //            Nodes = result.ToArray(),
        //            Lexer = lexer,
        //            Parser = parser,
        //            Text = text
        //        };
        //        results.Add(res);
        //    }
        //    reader.Read();

        //    return results.ToArray();
        //}

        private Utf8JsonWriter _writer;
        public override void Write(Utf8JsonWriter writer, ParsingResultSet[] data, JsonSerializerOptions options)
        {
            _writer = writer;
            writer.WriteStartArray();
            foreach (var tuple in data)
            {
                writer.WriteStartObject();

                if (tuple.FileName != null)
                {
                    writer.WritePropertyName("FileName");
                    writer.WriteStringValue(tuple.FileName);
                }

                if (tuple.Text != null)
                {
                    writer.WritePropertyName("Text");
                    writer.WriteStringValue(tuple.Text);
                }

                if (tuple.Parser != null && tuple.Parser.GrammarFileName != null)
                {
                    writer.WritePropertyName("IdentityOfParser");
                    writer.WriteStringValue(tuple.Parser.GrammarFileName);
                }

                if (tuple.Lexer != null && tuple.Lexer.GrammarFileName != null)
                {
                    writer.WritePropertyName("IdentityOfLexer");
                    writer.WriteStringValue(tuple.Lexer.GrammarFileName);
                }

                //if (tuple.Nodes != null && tuple.Nodes.Any())
                //{
                //    writer.WritePropertyName("Tokens");
                //    writer.WriteStartArray();
                //    // In order traverse and output.

                //    var in_token_stream = tuple.Stream as ITokenStream;
                //    in_token_stream.Seek(0);
                //    for (int i = 0; i < in_token_stream.Size; ++i)
                //    {
                //        var token = in_token_stream.Get(i);
                //        writer.WriteNumberValue(token.Type);
                //        writer.WriteNumberValue(token.StartIndex);
                //        writer.WriteNumberValue(token.StopIndex);
                //        writer.WriteNumberValue(token.Line);
                //        writer.WriteNumberValue(token.Column);
                //        writer.WriteNumberValue(token.Channel);
                //        if (token.Type == Antlr4.Runtime.TokenConstants.EOF) break;
                //    }
                //    writer.WriteEndArray();
                //}

                if (tuple.Lexer != null)
                {
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
                    writer.WritePropertyName("TokenTypeMap");
                    writer.WriteStartArray();
                    foreach (var pair in lexer.TokenTypeMap)
                    {
                        writer.WriteStringValue(pair.Key);
                        writer.WriteNumberValue(pair.Value);
                    }
                    writer.WriteEndArray();
                }
                if (tuple.Parser != null)
                {
                    writer.WritePropertyName("ParserRuleNames");
                    writer.WriteStartArray();
                    var parser = tuple.Parser as Parser;
                    foreach (var n in parser.RuleNames)
                    {
                        writer.WriteStringValue(n);
                    }
                    writer.WriteEndArray();
                }

                if (tuple.Nodes != null && tuple.Nodes.Any())
                {
                    writer.WritePropertyName("Nodes");
                    writer.WriteStartArray();
                    foreach (var node in tuple.Nodes) WriteRecurseTree(node);
                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        private void WriteRecurseTree(Node node)
        {
            _writer.WriteStartObject();
            _writer.WriteNumberValue(
                node switch
                {
                    AntlrDocument => 4,
                    AntlrAttr => 3,
                    AntlrText => 2,
                    AntlrNode => 1,
                    _ => throw new NotImplementedException(),
                }
                );
            if (node is AntlrAttr a)
            {
                _writer.WritePropertyName("Nm");
                _writer.WriteStringValue(a.Name as string);
                _writer.WritePropertyName("Va");
                _writer.WriteStringValue(a.StringValue);
            }
            if (node is AntlrText t)
            {
                _writer.WriteStringValue(t.Data);
            }
            if (node is AntlrNode n)
            {
                _writer.WriteNumberValue(n.RuleIndex);
            }
            if (node.ChildNodes != null)
            {
                _writer.WriteStartArray();
                for (int i = 0; i < node.ChildNodes.Length; ++i)
                {
                    org.w3c.dom.Node c = node.ChildNodes.item(i);
                    WriteRecurseTree(c);
                }
                _writer.WriteEndArray();
            }
            _writer.WriteEndObject();
        }

        public override ParsingResultSet[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

