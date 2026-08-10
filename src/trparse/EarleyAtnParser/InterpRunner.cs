namespace Trash.EarleyAtn;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ParseTreeEditing.UnvParseTreeDOM;
using EditableAntlrTree;
using AntlrJson;

/// <summary>
/// Orchestrates interp-file-based parsing:
///   1. Read .interp files and deserialize ATNs.
///   2. Lex the input text using the lexer ATN (character-level NFA simulation).
///   3. Parse on-channel tokens with the Earley parser.
///   4. Convert the resulting ParserRuleContext tree to a ParsingResultSet.
/// </summary>
public static class InterpRunner
{
    public static ParsingResultSet Run(
        string parserInterpPath,
        string lexerInterpPath,
        string inputText,
        string fileName,
        bool lineNumbers)
    {
        // Read and deserialize .interp files
        var parserInterp = InterpFileReader.Read(File.ReadAllText(parserInterpPath));
        var lexerInterp  = InterpFileReader.Read(File.ReadAllText(lexerInterpPath));

        var parserAtn = AtnDeserializer.Deserialize(parserInterp.AtnData);
        var lexerAtn  = AtnDeserializer.Deserialize(lexerInterp.AtnData);

        // Build Antlr4 Vocabulary objects (used by ConvertToDOM / serializer)
        var lexerVocab  = new Vocabulary(lexerInterp.LiteralNames,  lexerInterp.SymbolicNames);
        var parserVocab = new Vocabulary(parserInterp.LiteralNames, parserInterp.SymbolicNames);

        // Lex the input → full token list (all channels)
        var sim = new LexerAtnSimulator(lexerAtn);
        var rawTokens = sim.Tokenize(inputText);

        // Convert to Antlr4 IToken objects (CommonToken is IWritableToken)
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

        // Build CommonTokenStream (all tokens buffered; CommonTokenStream filters on-channel when consuming)
        var charStream   = new AntlrInputStream(inputText);
        var tokenSource  = new ListTokenSource(antlrTokens);
        var tokenStream  = new CommonTokenStream(tokenSource);
        tokenStream.Fill(); // buffer all tokens (Fill also rewrites TokenIndex)

        // Extract on-channel tokens for the Earley parser (DEFAULT_CHANNEL + EOF)
        var onChannel = antlrTokens
            .Where(t => t.Channel == TokenConstants.DefaultChannel || t.Type == TokenConstants.EOF)
            .ToList();

        // Parse → parse tree (ParserRuleContext subclass)
        int startRule = 0;
        var parseTree = EarleyParser.Parse(parserAtn, onChannel, startRule);
        if (parseTree == null)
            throw new InvalidOperationException($"Earley parse failed for '{fileName}': input rejected by grammar.");

        // Build stub lexer/parser objects used by ConvertToDOM and the JSON serializer
        var myLexer = new MyLexer(charStream);
        myLexer._ruleNames   = lexerInterp.RuleNames;
        myLexer._modeNames   = lexerInterp.ModeNames.Length > 0 ? lexerInterp.ModeNames : new[] { "DEFAULT_MODE" };
        myLexer._channelNames = lexerInterp.ChannelNames.Length > 0
            ? lexerInterp.ChannelNames
            : new[] { "DEFAULT_TOKEN_CHANNEL", "HIDDEN" };
        myLexer._vocabulary  = lexerVocab;
        myLexer._tokenTypeMap = BuildTokenTypeMap(lexerInterp.SymbolicNames);
        myLexer._grammarFileName = Path.GetFileNameWithoutExtension(lexerInterpPath);

        var myParser = new MyParser();
        myParser._ruleNames      = parserInterp.RuleNames;
        myParser._vocabulary     = parserVocab;
        myParser._grammarFileName = Path.GetFileNameWithoutExtension(parserInterpPath);

        // Convert parse tree to UnvParseTreeNode (the DOM used by the Trash toolkit)
        var converter = new ConvertToDOM(lineNumbers);
        var domTree = converter.BottomUpConvert(parseTree, null, myParser, myLexer, tokenStream);

        return new ParsingResultSet
        {
            FileName = fileName,
            Nodes    = new[] { (UnvParseTreeNode)domTree },
            Parser   = myParser,
            Lexer    = myLexer
        };
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
