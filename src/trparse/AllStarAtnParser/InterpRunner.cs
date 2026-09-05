namespace AllStarAtnParser;

using ParseTreeEditing.UnvParseTreeDOM;
using EditableAntlrTree;
using AntlrJson;
using Atn;
using EarleyAtnParser;

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
        bool contextAwareLexing = false,
        bool lexerStats = false,
        bool lexerOverlaps = false,
        InterpRunTimings timings = null,
        ParserStatistics parserStatistics = null,
        ParserPredictionCache predictionCache = null,
        InterpRuntimeCache runtimeCache = null)
    {
        timings ??= new InterpRunTimings();
        timings.Files = 1;
        var timer = new System.Diagnostics.Stopwatch();
        // Get options to lexer from process args.
        var args = Environment.GetCommandLineArgs().ToList();

        // Determine which preprocessor to run: gcc or cl.exe or clang.
        show_tokens = args?.Where(a => a.IndexOf("--tokens", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;
        numeric_token_types = args?.Where(a => a.IndexOf("--numeric-token-types", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;

        InterpRuntimeCache.RuntimeData runtime;
        if (runtimeCache == null ||
            !runtimeCache.TryGet(parserInterpPath, lexerInterpPath, out runtime))
        {
            timer.Start();
            var parserInterpText = File.ReadAllText(parserInterpPath);
            var lexerInterpText = File.ReadAllText(lexerInterpPath);
            timer.Stop();
            timings.InterpFileReading = timer.Elapsed;

            timer.Restart();
            var loadedParserInterp = InterpFileReader.Read(parserInterpText);
            var loadedLexerInterp = InterpFileReader.Read(lexerInterpText);
            timer.Stop();
            timings.InterpParsing = timer.Elapsed;

            timer.Restart();
            var loadedParserAtn = predictionCache?.GetBoundAtn(
                loadedParserInterp.AtnData);
            if (loadedParserAtn == null)
            {
                loadedParserAtn = AtnDeserializer.Deserialize(
                    loadedParserInterp.AtnData);
                predictionCache?.Bind(
                    loadedParserAtn, loadedParserInterp.AtnData);
            }
            var loadedLexerAtn = AtnDeserializer.Deserialize(
                loadedLexerInterp.AtnData);
            timer.Stop();
            timings.AtnDeserialization = timer.Elapsed;

            timer.Restart();
            var loadedLexerVocab = new Antlr4.Runtime.Vocabulary(
                loadedLexerInterp.LiteralNames, loadedLexerInterp.SymbolicNames);
            var loadedParserVocab = new Antlr4.Runtime.Vocabulary(
                loadedParserInterp.LiteralNames, loadedParserInterp.SymbolicNames);
            int loadedStartRule = ResolveStartRule(
                loadedParserAtn, loadedParserInterp);
            timer.Stop();
            timings.Initialization = timer.Elapsed;

            runtime = new InterpRuntimeCache.RuntimeData(
                loadedParserInterp, loadedLexerInterp,
                loadedParserAtn, loadedLexerAtn,
                loadedParserVocab, loadedLexerVocab, loadedStartRule);
            if (runtimeCache != null)
                runtime = runtimeCache.Add(
                    parserInterpPath, lexerInterpPath, runtime);
        }
        var parserInterp = runtime.ParserInterp;
        var lexerInterp = runtime.LexerInterp;
        var parserAtn = runtime.ParserAtn;
        var lexerAtn = runtime.LexerAtn;
        var parserVocab = runtime.ParserVocabulary;
        var lexerVocab = runtime.LexerVocabulary;
        int startRule = runtime.StartRule;
        var statistics = lexerStats || lexerOverlaps
            ? new LexerStatistics()
            : null;

        List<LexerToken> rawTokens;
        List<ParseEvent> events;
        if (contextAwareLexing)
        {
            timer.Restart();
            events = AllStarParser.ParseContextAware(
                parserAtn, lexerAtn, inputText, startRule, out rawTokens,
                statistics, parserStatistics, predictionCache);
            timer.Stop();
            // Tokens are requested lazily here; this is combined lex/parse time.
            timings.Parsing = timer.Elapsed;
        }
        else
        {
            var sim = new EarleyAtnParser.LexerAtnSimulator(lexerAtn, statistics);
            timer.Restart();
            rawTokens = sim.Tokenize(inputText);
            timer.Stop();
            timings.Tokenization = timer.Elapsed;
            timings.LexerDfa = sim.GetDfaStatistics();
            timer.Restart();
            ReconcileLiteralTokenTypes(rawTokens, lexerInterp.SymbolicNames,
                lexerInterp.LiteralNames, parserInterp.LiteralNames);
            timer.Stop();
            timings.TokenReconciliation = timer.Elapsed;
            timer.Restart();
            events = AllStarParser.Parse(
                parserAtn, rawTokens, startRule, parserStatistics,
                predictionCache);
            timer.Stop();
            timings.Parsing = timer.Elapsed;
        }
        PrintLexerStatistics(
            statistics, lexerOverlaps, fileName,
            lexerInterp.RuleNames, lexerInterp.SymbolicNames);
        if (parserStatistics != null)
            Console.Error.WriteLine(parserStatistics.Format());
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

        timer.Restart();
        var domTree = DomBuilder.Build(
            events, rawTokens,
            parserInterp.RuleNames,
            parserInterp.SymbolicNames,
            parserInterp.LiteralNames,
            lexerInterp.RuleNames,
            lineNumbers);
        timer.Stop();
        timings.TreeBuilding = timer.Elapsed;

        timer.Restart();
        // Stub lexer/parser objects required by ParsingResultSet and the JSON serializer.
        var charStream = new Antlr4.Runtime.AntlrInputStream(inputText);
        var myLexer = new MyLexer(charStream);
        myLexer._ruleNames = lexerInterp.RuleNames;
        myLexer._modeNames = lexerInterp.ModeNames.Length > 0
            ? lexerInterp.ModeNames : new[] { "DEFAULT_MODE" };
        myLexer._channelNames = lexerInterp.ChannelNames.Length > 0
            ? lexerInterp.ChannelNames : new[] { "DEFAULT_TOKEN_CHANNEL", "HIDDEN" };
        myLexer._vocabulary = lexerVocab;
        myLexer._tokenTypeMap = BuildTokenTypeMap(lexerInterp.SymbolicNames);
        myLexer._grammarFileName = Path.GetFileNameWithoutExtension(lexerInterpPath);

        var myParser = new EditableAntlrTree.MyParser();
        myParser._ruleNames = parserInterp.RuleNames;
        myParser._vocabulary = parserVocab;
        myParser._grammarFileName = Path.GetFileNameWithoutExtension(parserInterpPath);

        int tokenCount = 0;
        foreach (var t in rawTokens)
            if (t.Channel == 0 || t.Type == -1) tokenCount++;

        var result = new ParsingResultSet
        {
            FileName = fileName,
            Nodes = new[] { (UnvParseTreeNode)domTree },
            Parser = myParser,
            Lexer = myLexer
        };
        timer.Stop();
        timings.ResultConstruction = timer.Elapsed;

        return (result, tokenCount);
    }

    private static int ResolveStartRule(MyATN parserAtn, ParsedInterp parserInterp)
    {
        if (parserInterp.StartStateNumber < 0) return 0;
        for (int rule = 0; rule < parserAtn.start.Length; rule++)
            if (parserAtn.start[rule].stateNumber == parserInterp.StartStateNumber)
                return rule;
        throw new InvalidOperationException(
            $"Start state {parserInterp.StartStateNumber} not found in deserialized parser ATN.");
    }

    internal static void PrintLexerStatistics(
        LexerStatistics statistics, bool includeOverlaps, string fileName,
        string[] ruleNames, string[] symbolicNames)
    {
        if (statistics == null) return;
        if (includeOverlaps)
            Console.Error.Write(statistics.FormatOverlaps(
                fileName, ruleNames, symbolicNames));
        Console.Error.Write(statistics.FormatSummary(ruleNames));
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
