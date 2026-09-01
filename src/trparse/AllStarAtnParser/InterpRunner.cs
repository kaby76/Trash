namespace AllStarAtnParser;

using ParseTreeEditing.UnvParseTreeDOM;
using EditableAntlrTree;
using AntlrJson;
using Atn;

/// <summary>
/// Orchestrates interp-file-based parsing using the ALL(*) parser.
/// Drop-in parallel to InterpRunner; uses AllStarParser instead of EarleyParser.
/// </summary>
public static class InterpRunner
{
    public static bool show_tokens = false;
    public static bool numeric_token_types = false;

    public static (ParsingResultSet Result, int TokenCount) Run(
        string parserInterpPath,
        string lexerInterpPath,
        string inputText,
        string fileName,
        bool lineNumbers,
        bool contextAwareLexing = false)
    {
        // Get options to lexer from process args.
        var args = Environment.GetCommandLineArgs().ToList();

        // Determine which preprocessor to run: gcc or cl.exe or clang.
        show_tokens = args?.Where(a => a.IndexOf("--tokens", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;
        numeric_token_types = args?.Where(a => a.IndexOf("--numeric-token-types", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;

        var parserInterp = InterpFileReader.Read(File.ReadAllText(parserInterpPath));
        var lexerInterp  = InterpFileReader.Read(File.ReadAllText(lexerInterpPath));

        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn  = AtnDeserializer.Deserialize(lexerInterp.AtnData);

        var lexerVocab  = new Antlr4.Runtime.Vocabulary(lexerInterp.LiteralNames,  lexerInterp.SymbolicNames);
        var parserVocab = new Antlr4.Runtime.Vocabulary(parserInterp.LiteralNames, parserInterp.SymbolicNames);

        // Determine the start rule from the 'start-rule:' section in the parser interp file.
        int startRule = 0;
        if (parserInterp.StartStateNumber >= 0)
        {
            bool found = false;
            for (int ri = 0; ri < parserAtn.start.Length; ri++)
            {
                if (parserAtn.start[ri].stateNumber == parserInterp.StartStateNumber)
                {
                    startRule = ri;
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new InvalidOperationException(
                    $"Start state {parserInterp.StartStateNumber} not found in deserialized parser ATN.");
        }

        List<LexerToken> rawTokens;
        List<ParseEvent> events;
        if (contextAwareLexing)
        {
            events = AllStarParser.ParseContextAware(
                parserAtn, lexerAtn, inputText, startRule, out rawTokens);
        }
        else
        {
            var sim = new EarleyAtnParser.LexerAtnSimulator(lexerAtn);
            rawTokens = sim.Tokenize(inputText);
            ReconcileLiteralTokenTypes(rawTokens, lexerInterp.SymbolicNames,
                lexerInterp.LiteralNames, parserInterp.LiteralNames);
            events = AllStarParser.Parse(parserAtn, rawTokens, startRule);
        }
        if (events == null)
            throw new InvalidOperationException(
                $"ALL(*) parse failed for '{fileName}': input rejected by grammar.");

        if (show_tokens)
        {
            var symNames = lexerInterp.SymbolicNames;
            foreach (var tok in rawTokens)
            {
                string typeName = (!numeric_token_types && tok.Type >= 0 && tok.Type < symNames.Length && symNames[tok.Type] != null)
                    ? symNames[tok.Type] : tok.Type.ToString();
                string text = tok.Text
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
                string channel = tok.Channel != 0 ? $",channel={tok.Channel}" : "";
                System.Console.Error.WriteLine(
                    $"[@{tok.TokenIndex},{tok.StartIndex}:{tok.StopIndex}='{text}',<{typeName}>{channel},{tok.Line}:{tok.Column}]");
            }
        }

        var domTree = DomBuilder.Build(
            events, rawTokens,
            parserInterp.RuleNames,
            parserInterp.SymbolicNames,
            parserInterp.LiteralNames,
            lexerInterp.RuleNames,
            lineNumbers);

        // Stub lexer/parser objects required by ParsingResultSet and the JSON serializer.
        var charStream = new Antlr4.Runtime.AntlrInputStream(inputText);
        var myLexer = new MyLexer(charStream);
        myLexer._ruleNames       = lexerInterp.RuleNames;
        myLexer._modeNames       = lexerInterp.ModeNames.Length > 0
            ? lexerInterp.ModeNames : new[] { "DEFAULT_MODE" };
        myLexer._channelNames    = lexerInterp.ChannelNames.Length > 0
            ? lexerInterp.ChannelNames : new[] { "DEFAULT_TOKEN_CHANNEL", "HIDDEN" };
        myLexer._vocabulary      = lexerVocab;
        myLexer._tokenTypeMap    = BuildTokenTypeMap(lexerInterp.SymbolicNames);
        myLexer._grammarFileName = Path.GetFileNameWithoutExtension(lexerInterpPath);

        var myParser = new EditableAntlrTree.MyParser();
        myParser._ruleNames       = parserInterp.RuleNames;
        myParser._vocabulary      = parserVocab;
        myParser._grammarFileName = Path.GetFileNameWithoutExtension(parserInterpPath);

        int tokenCount = 0;
        foreach (var t in rawTokens)
            if (t.Channel == 0 || t.Type == -1) tokenCount++;

        return (new ParsingResultSet
        {
            FileName = fileName,
            Nodes    = new[] { (UnvParseTreeNode)domTree },
            Parser   = myParser,
            Lexer    = myLexer
        }, tokenCount);
    }

    private static IDictionary<string, int> BuildTokenTypeMap(string[] symbolicNames)
    {
        var map = new Dictionary<string, int>();
        for (int i = 0; i < symbolicNames.Length; i++)
            if (symbolicNames[i] != null)
                map[symbolicNames[i]] = i;
        return map;
    }

    // A lexer rule with a command may have no literal-name entry even though its
    // symbolic name spells the keyword (for example TABLE -> 'table'). If a parser
    // interp was generated from such a token vocabulary, ANTLR can assign a separate
    // implicit token type to the literal used by the parser rule. Reconcile that
    // duplicate so the independently interpreted lexer and parser share a vocabulary.
    internal static void ReconcileLiteralTokenTypes(
        List<LexerToken> tokens, string[] lexerSymbolicNames,
        string[] lexerLiteralNames, string[] parserLiteralNames)
    {
        var remap = new Dictionary<int, int>();
        for (int lexerType = 0; lexerType < lexerSymbolicNames.Length; lexerType++)
        {
            var symbolicName = lexerSymbolicNames[lexerType];
            if (symbolicName == null ||
                (lexerType < lexerLiteralNames.Length && lexerLiteralNames[lexerType] != null))
                continue;

            var expectedLiteral = $"'{symbolicName.ToLowerInvariant()}'";
            for (int parserType = 0; parserType < parserLiteralNames.Length; parserType++)
            {
                if (parserType != lexerType &&
                    string.Equals(parserLiteralNames[parserType], expectedLiteral,
                        StringComparison.Ordinal))
                {
                    remap[lexerType] = parserType;
                    break;
                }
            }
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (!remap.TryGetValue(token.Type, out int parserType)) continue;
            var symbolicName = lexerSymbolicNames[token.Type];
            if (!string.Equals(token.Text, symbolicName, StringComparison.OrdinalIgnoreCase))
                continue;
            token.Type = parserType;
            tokens[i] = token;
        }
    }
}
