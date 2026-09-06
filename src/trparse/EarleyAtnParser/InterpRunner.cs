namespace EarleyAtnParser;

// Antlr4 runtime used temporarily until we can have all tools not use
// it. Unfortunately, right now, ParsingResultSet uses it.

using ParseTreeEditing.UnvParseTreeDOM;
using EditableAntlrTree;
using AntlrJson;
using Atn;

/// <summary>
/// Orchestrates interp-file-based parsing:
///   1. Read .interp files and deserialize ATNs.
///   2. Lex the input text using the lexer ATN (character-level NFA simulation).
///   3. Parse with the Earley parser → ParseEvent list.
///   4. Build a DOM tree directly via DomBuilder → ParsingResultSet.
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
        bool lexerStats = false,
        bool lexerOverlaps = false,
        AllStarAtnParser.InterpRuntimeCache runtimeCache = null)
    {
        // Get options to lexer from process args.
        var args = Environment.GetCommandLineArgs().ToList();

        // Determine which preprocessor to run: gcc or cl.exe or clang.
        show_tokens = args?.Where(a => a.IndexOf("--tokens", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;
        numeric_token_types = args?.Where(a => a.IndexOf("--numeric-token-types", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;

        AllStarAtnParser.InterpRuntimeCache.RuntimeData runtime;
        if (runtimeCache == null ||
            !runtimeCache.TryGet(parserInterpPath, lexerInterpPath, out runtime))
        {
            var loadedParserInterp = InterpFileReader.Read(
                File.ReadAllText(parserInterpPath));
            var loadedLexerInterp = InterpFileReader.Read(
                File.ReadAllText(lexerInterpPath));
            var loadedParserAtn = AtnDeserializer.Deserialize(
                loadedParserInterp.AtnData);
            var loadedLexerAtn = AtnDeserializer.Deserialize(
                loadedLexerInterp.AtnData);
            int loadedStartRule = ResolveStartRule(
                loadedParserAtn, loadedParserInterp);
            runtime = new AllStarAtnParser.InterpRuntimeCache.RuntimeData(
                loadedParserInterp, loadedLexerInterp,
                loadedParserAtn, loadedLexerAtn,
                new Antlr4.Runtime.Vocabulary(
                    loadedParserInterp.LiteralNames,
                    loadedParserInterp.SymbolicNames),
                new Antlr4.Runtime.Vocabulary(
                    loadedLexerInterp.LiteralNames,
                    loadedLexerInterp.SymbolicNames),
                loadedStartRule);
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

        var statistics = lexerStats || lexerOverlaps
            ? new LexerStatistics()
            : null;
        var sim = new LexerAtnSimulator(lexerAtn, statistics);
        var rawTokens = sim.Tokenize(inputText);
        AllStarAtnParser.InterpRunner.PrintLexerStatistics(
            statistics, lexerOverlaps, fileName,
            lexerInterp.RuleNames, lexerInterp.SymbolicNames);
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
        int startRule = runtime.StartRule;

        var events = EarleyParser.Parse(parserAtn, rawTokens, startRule);
        if (events == null)
            throw new InvalidOperationException($"Earley parse failed for '{fileName}': input rejected by grammar.");

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

        // Match CommonTokenStream.Size in the generated-target drivers: report
        // every retained token, including hidden-channel tokens and EOF.  The
        // parser itself still indexes only default-channel tokens.
        int tokenCount = rawTokens.Count;

        return (new ParsingResultSet
        {
            FileName = fileName,
            Nodes    = new[] { (UnvParseTreeNode)domTree },
            Parser   = myParser,
            Lexer    = myLexer
        }, tokenCount);
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

    private static IDictionary<string, int> BuildTokenTypeMap(string[] symbolicNames)
    {
        var map = new Dictionary<string, int>();
        for (int i = 0; i < symbolicNames.Length; i++)
            if (symbolicNames[i] != null)
                map[symbolicNames[i]] = i;
        return map;
    }
}
