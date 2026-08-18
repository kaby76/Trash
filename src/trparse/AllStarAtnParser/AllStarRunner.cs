namespace Trash.EarleyAtn;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
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
        // Read and deserialize .interp files (same infrastructure as InterpRunner).
        var parserInterp = InterpFileReader.Read(File.ReadAllText(parserInterpPath));
        var lexerInterp  = InterpFileReader.Read(File.ReadAllText(lexerInterpPath));

        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn  = AtnDeserializer.Deserialize(lexerInterp.AtnData);

        var lexerVocab  = new Vocabulary(lexerInterp.LiteralNames,  lexerInterp.SymbolicNames);
        var parserVocab = new Vocabulary(parserInterp.LiteralNames, parserInterp.SymbolicNames);

        // Lex the input using the same character-level NFA simulator.
        var sim = new LexerAtnSimulator(lexerAtn);
        var rawTokens = sim.Tokenize(inputText);

        var antlrTokens = new List<IToken>(rawTokens.Count);
        foreach (var lt in rawTokens)
        {
            var ct = new CommonToken(lt.Type)
            {
                Channel    = lt.Channel,
                Text       = lt.Text,
                StartIndex = lt.StartIndex,
                StopIndex  = lt.StopIndex,
                Line       = lt.Line,
                Column     = lt.Column,
                TokenIndex = lt.TokenIndex
            };
            antlrTokens.Add(ct);
        }

        var charStream  = new AntlrInputStream(inputText);
        var tokenSource = new ListTokenSource(antlrTokens);
        var tokenStream = new CommonTokenStream(tokenSource);
        tokenStream.Fill();

        var onChannel = antlrTokens
            .Where(t => t.Channel == TokenConstants.DefaultChannel || t.Type == TokenConstants.EOF)
            .ToList();

        // Determine start rule from the 'start-rule:' section.
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

        var parseTree = AllStarParser.Parse(parserAtn, antlrTokens, startRule);
        if (parseTree == null)
            throw new InvalidOperationException(
                $"ALL(*) parse failed for '{fileName}': input rejected by grammar.");

        // Build stub lexer/parser objects used by ConvertToDOM and the JSON serializer.
        var myLexer = new MyLexer(charStream);
        myLexer._ruleNames    = lexerInterp.RuleNames;
        myLexer._modeNames    = lexerInterp.ModeNames.Length > 0
            ? lexerInterp.ModeNames : new[] { "DEFAULT_MODE" };
        myLexer._channelNames = lexerInterp.ChannelNames.Length > 0
            ? lexerInterp.ChannelNames : new[] { "DEFAULT_TOKEN_CHANNEL", "HIDDEN" };
        myLexer._vocabulary   = lexerVocab;
        myLexer._tokenTypeMap = BuildTokenTypeMap(lexerInterp.SymbolicNames);
        myLexer._grammarFileName = Path.GetFileNameWithoutExtension(lexerInterpPath);

        var myParser = new MyParser();
        myParser._ruleNames       = parserInterp.RuleNames;
        myParser._vocabulary      = parserVocab;
        myParser._grammarFileName = Path.GetFileNameWithoutExtension(parserInterpPath);

        var converter = new ConvertToDOM(lineNumbers);
        var domTree = converter.BottomUpConvert(parseTree, null, myParser, myLexer, tokenStream);

        return (new ParsingResultSet
        {
            FileName = fileName,
            Nodes    = new[] { (UnvParseTreeNode)domTree },
            Parser   = myParser,
            Lexer    = myLexer
        }, onChannel.Count);
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
