namespace Trash.EarleyAtn;

using Antlr4.Runtime;
using ParseTreeEditing.UnvParseTreeDOM;
using EditableAntlrTree;
using AntlrJson;

/// <summary>
/// Orchestrates interp-file-based parsing using the ALL(*) parser.
/// Drop-in parallel to InterpRunner; uses AllStarParser instead of EarleyParser.
/// </summary>
public static class AllStarRunner
{
    public static (ParsingResultSet Result, int TokenCount) Run(
        string parserInterpPath,
        string lexerInterpPath,
        string inputText,
        string fileName,
        bool lineNumbers)
    {
        var parserInterp = InterpFileReader.Read(File.ReadAllText(parserInterpPath));
        var lexerInterp  = InterpFileReader.Read(File.ReadAllText(lexerInterpPath));

        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn  = AtnDeserializer.Deserialize(lexerInterp.AtnData);

        var lexerVocab  = new Vocabulary(lexerInterp.LiteralNames,  lexerInterp.SymbolicNames);
        var parserVocab = new Vocabulary(parserInterp.LiteralNames, parserInterp.SymbolicNames);

        var sim = new LexerAtnSimulator(lexerAtn);
        var rawTokens = sim.Tokenize(inputText);

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

        var events = AllStarParser.Parse(parserAtn, rawTokens, startRule);
        if (events == null)
            throw new InvalidOperationException(
                $"ALL(*) parse failed for '{fileName}': input rejected by grammar.");

        var domTree = DomBuilder.Build(
            events, rawTokens,
            parserInterp.RuleNames,
            parserInterp.SymbolicNames,
            parserInterp.LiteralNames,
            lineNumbers);

        // Stub lexer/parser objects required by ParsingResultSet and the JSON serializer.
        var charStream = new AntlrInputStream(inputText);
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
