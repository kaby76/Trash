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

    public static (ParsingResultSet Result, int TokenCount) Run(
        string parserInterpPath,
        string lexerInterpPath,
        string inputText,
        string fileName,
        bool lineNumbers)
    {
        // Get options to lexer from process args.
        var args = Environment.GetCommandLineArgs().ToList();

        // Determine which preprocessor to run: gcc or cl.exe or clang.
        show_tokens = args?.Where(a => a.IndexOf("--tokens", StringComparison.OrdinalIgnoreCase) >= 0).Any() ?? false;

        var parserInterp = InterpFileReader.Read(File.ReadAllText(parserInterpPath));
        var lexerInterp  = InterpFileReader.Read(File.ReadAllText(lexerInterpPath));

        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn  = AtnDeserializer.Deserialize(lexerInterp.AtnData);

        var lexerVocab  = new Antlr4.Runtime.Vocabulary(lexerInterp.LiteralNames,  lexerInterp.SymbolicNames);
        var parserVocab = new Antlr4.Runtime.Vocabulary(parserInterp.LiteralNames, parserInterp.SymbolicNames);

        var sim = new LexerAtnSimulator(lexerAtn);
        var rawTokens = sim.Tokenize(inputText);
        if (show_tokens)
        {
            var symNames = lexerInterp.SymbolicNames;
            foreach (var tok in rawTokens)
            {
                string typeName = tok.Type >= 0 && tok.Type < symNames.Length && symNames[tok.Type] != null
                    ? symNames[tok.Type] : tok.Type.ToString();
                string text = tok.Text
                    .Replace("\\", "\\\\")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
                System.Console.Error.WriteLine(
                    $"[@{tok.TokenIndex},{tok.StartIndex}:{tok.StopIndex}='{text}',<{typeName}>,channel={tok.Channel},{tok.Line}:{tok.Column}]");
            }
        }
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
}
