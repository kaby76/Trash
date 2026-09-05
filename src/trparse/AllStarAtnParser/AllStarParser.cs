namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

/// <summary>
/// Interpretive ALL(*) parser.
/// Drives a top-down LL parse using AdaptivePredict at every decision point.
/// Produces a ParseEvent sequence from which DomBuilder builds a DOM tree.
/// </summary>
public static class AllStarParser
{
    /// <summary>Set to true to emit prediction/consume trace lines on stderr.</summary>
    public static bool Trace { get; set; } = false;

    private const int DEFAULT_CHANNEL = 0;
    private const int EOF_TYPE = -1;

    /// <summary>
    /// Parse allTokens (all channels, EOF at end) and return an ordered
    /// ParseEvent list, or null if the input is rejected by the grammar.
    /// </summary>
    public static List<ParseEvent> Parse(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex)
    {
        return ParseCore(atn, allTokens, startRuleIndex, buildEvents: true).Events;
    }

    /// <summary>
    /// Recognize an already-tokenized input without constructing parse events
    /// or a parse tree. This isolates parser prediction and committed ATN
    /// traversal for performance measurement.
    /// </summary>
    public static bool Recognize(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex)
    {
        return ParseCore(atn, allTokens, startRuleIndex, buildEvents: false).Success;
    }

    private static (bool Success, List<ParseEvent> Events) ParseCore(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        bool buildEvents)
    {
        if (atn == null) throw new ArgumentNullException(nameof(atn));
        if (startRuleIndex < 0 || startRuleIndex >= atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        // Build on-channel index map: on-channel position → index in allTokens.
        var onIdx = new List<int>();
        for (int i = 0; i < allTokens.Count; i++)
        {
            var t = allTokens[i];
            if (t.Channel == DEFAULT_CHANNEL || t.Type == EOF_TYPE)
                onIdx.Add(i);
        }

        // Parallel int[] of token types for the simulator (no Antlr4 types).
        var tokenTypes = new int[onIdx.Count];
        for (int i = 0; i < onIdx.Count; i++)
            tokenTypes[i] = allTokens[onIdx[i]].Type;

        // Build reverse map: ATN state number → decision index.
        var stateToDecision = new Dictionary<int, int>();
        for (int i = 0; i < atn.decisionToState.Length; i++)
            stateToDecision[atn.decisionToState[i].stateNumber] = i;

        var events = buildEvents ? new List<ParseEvent>() : null;
        var instance = new ParserInstance(atn, allTokens, onIdx, tokenTypes, stateToDecision);
        if (!instance.ParseRule(startRuleIndex, events, PredictionContext.EMPTY))
            return (false, null);
        return (true, events);
    }

    /// <summary>
    /// Parse while lexing lazily. The parser's valid-lookahead set is used to
    /// prefer context-valid lexer matches, with ordinary ANTLR selection as the
    /// fallback when no context-valid rule matches.
    /// </summary>
    public static List<ParseEvent> ParseContextAware(
        MyATN parserAtn, MyATN lexerAtn, string input, int startRuleIndex,
        out List<LexerToken> allTokens,
        LexerStatistics lexerStatistics = null)
    {
        if (parserAtn == null) throw new ArgumentNullException(nameof(parserAtn));
        if (lexerAtn == null) throw new ArgumentNullException(nameof(lexerAtn));
        if (startRuleIndex < 0 || startRuleIndex >= parserAtn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        var stateToDecision = new Dictionary<int, int>();
        for (int i = 0; i < parserAtn.decisionToState.Length; i++)
            stateToDecision[parserAtn.decisionToState[i].stateNumber] = i;

        allTokens = new List<LexerToken>();
        var events = new List<ParseEvent>();
        var instance = new ParserInstance(
            parserAtn, lexerAtn, input, allTokens, stateToDecision,
            lexerStatistics);
        if (!instance.ParseRule(startRuleIndex, events, PredictionContext.EMPTY))
            return null;
        return events;
    }

    // =========================================================================
    // Internal parser instance — carries mutable state (token position).
    // =========================================================================

    private sealed class ParserInstance
    {
        private readonly MyATN _atn;
        private readonly List<LexerToken> _allTokens;
        private readonly List<int> _onIdx;  // on-channel pos → all-token index
        private readonly int[] _tokenTypes;
        private readonly Dictionary<int, int> _stateToDecision;
        private readonly AllStarSimulator _sim;
        private readonly LexerAtnSimulator _lexer;
        private readonly LexerAtnSimulator.Cursor _lexerCursor;
        private readonly string _input;
        private readonly bool _contextAware;

        public int Pos { get; private set; } // current on-channel token position

        public ParserInstance(MyATN atn, IReadOnlyList<LexerToken> allTokens,
                              IReadOnlyList<int> onIdx, int[] tokenTypes,
                              Dictionary<int, int> stateToDecision)
        {
            _atn = atn;
            _allTokens = allTokens as List<LexerToken> ?? allTokens.ToList();
            _onIdx = onIdx as List<int> ?? onIdx.ToList();
            _tokenTypes = tokenTypes;
            _stateToDecision = stateToDecision;
            _sim = new AllStarSimulator(atn);
        }

        public ParserInstance(MyATN parserAtn, MyATN lexerAtn, string input,
                              List<LexerToken> allTokens,
                              Dictionary<int, int> stateToDecision,
                              LexerStatistics lexerStatistics)
        {
            _atn = parserAtn;
            _allTokens = allTokens;
            _onIdx = new List<int>();
            _tokenTypes = Array.Empty<int>();
            _stateToDecision = stateToDecision;
            _sim = new AllStarSimulator(parserAtn);
            _lexer = new LexerAtnSimulator(lexerAtn, lexerStatistics);
            _lexerCursor = new LexerAtnSimulator.Cursor();
            _input = input;
            _contextAware = true;
        }

        /// <summary>Parse one rule; emits Enter/Exit/Consume events. Returns false on error.</summary>
        public bool ParseRule(int ruleIndex, List<ParseEvent> events,
                              PredictionContext callerCtx, int precedence = 0)
        {
            bool isRecursion = _atn.start[ruleIndex].isPrecedenceRule;
            events?.Add(isRecursion ? ParseEvent.EnterRecursionRule(ruleIndex) : ParseEvent.EnterRule(ruleIndex));
            var state = _atn.start[ruleIndex];

            while (state.stateType != MyStateType.RuleStop)
            {
                if (state.transitions.Count == 0)
                    throw new InvalidOperationException($"Dead ATN state {state.stateNumber}");

                if (_stateToDecision.TryGetValue(state.stateNumber, out int decision))
                {
                    // Decision point: use ALL(*) prediction to choose an alternative.
                    int[] predictionTokens;
                    int predictionStart;
                    if (_contextAware)
                    {
                        var expected = _sim.GetExpectedTokenTypes(
                            state, callerCtx, precedence);
                        predictionTokens = BuildPredictionTokens(expected);
                        predictionStart = 0;
                    }
                    else
                    {
                        predictionTokens = _tokenTypes;
                        predictionStart = Pos;
                    }
                    int alt = _sim.AdaptivePredict(
                        decision, predictionTokens, predictionStart,
                        callerCtx, precedence);
                    if (AllStarParser.Trace)
                        Console.Error.WriteLine(
                            $"[ALLSTAR] dec={decision} state={state.stateNumber} pos={Pos} " +
                            $"tok={(predictionTokens.Length > predictionStart ? predictionTokens[predictionStart] : -1)} " +
                            $"prec={precedence} → alt={alt}");
                    if (alt <= 0 || alt > state.transitions.Count)
                    {
                        if (AllStarParser.Trace)
                            Console.Error.WriteLine($"[ALLSTAR] FAIL: alt out of range");
                        return false;
                    }
                    var nextState = state.transitions[alt - 1].target;
                    // Mirror ANTLR4 ParserInterpreter.visitDecisionState: when taking a non-exit
                    // path from the precedence suffix loop, wrap the accumulated context as the
                    // first child of a fresh rule element (PushNewRecursionContext equivalent).
                    if (state.isPrecedenceDecision && nextState.stateType != MyStateType.LoopEnd)
                        events?.Add(ParseEvent.PushRecursionContext(state.ruleIndex));
                    state = nextState;
                }
                else if (state.transitions.Count == 1)
                {
                    var tr = state.transitions[0];
                    switch (tr)
                    {
                        case MyEpsilonTransition:
                        case MyActionTransition:
                        case MyPredicateTransition:
                        case MyPrecedencePredicateTransition:
                            state = tr.target;
                            break;

                        case MyRuleTransition rt:
                            if (AllStarParser.Trace && rt.precedence != 0)
                                Console.Error.WriteLine(
                                    $"[ALLSTAR] call rule={rt.ruleIndex} from={state.stateNumber} " +
                                    $"prec={rt.precedence} pos={Pos}");
                            // Push follow state onto context for LL prediction inside the sub-rule.
                            var childCtx = new SingletonPredictionContext(
                                callerCtx, rt.target.stateNumber, precedence);
                            if (!ParseRule(rt.ruleIndex, events, childCtx, rt.precedence))
                                return false;
                            state = rt.target;
                            break;

                        default:
                            // Terminal transition: consume the next on-channel token.
                            if (!ConsumeToken(tr, events))
                            {
                                if (AllStarParser.Trace)
                                {
                                    int tokType = _onIdx.Count > Pos ? _allTokens[_onIdx[Pos]].Type : -999;
                                    Console.Error.WriteLine(
                                        $"[ALLSTAR] FAIL: ConsumeToken at state={state.stateNumber} " +
                                        $"rule={state.ruleIndex} pos={Pos} tok={tokType} " +
                                        $"tr={DescribeTransition(tr)}");
                                }
                                return false;
                            }
                            state = tr.target;
                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        $"ATN state {state.stateNumber} (type={state.stateType}) has " +
                        $"{state.transitions.Count} transitions but is not a registered decision state.");
                }
            }

            events?.Add(isRecursion ? ParseEvent.ExitRecursionRule(ruleIndex) : ParseEvent.ExitRule(ruleIndex));
            return true;
        }

        private bool ConsumeToken(MyTransition tr, List<ParseEvent> events)
        {
            if (_contextAware)
            {
                var expected = TokenTypesForTransition(tr);
                int contextualTokenIndex = ReadOnChannelToken(expected);
                var contextualToken = _allTokens[contextualTokenIndex];
                if (!TerminalMatches(tr, contextualToken.Type)) return false;
                if (AllStarParser.Trace)
                    Console.Error.WriteLine(
                        $"[ALLSTAR] consume pos={Pos} tok={contextualToken.Type} '{contextualToken.Text}'");
                events?.Add(ParseEvent.Consume(contextualTokenIndex));
                Pos++;
                return true;
            }
            if (Pos >= _onIdx.Count) return false;
            int allTokIdx = _onIdx[Pos];
            var tok = _allTokens[allTokIdx];
            if (!TerminalMatches(tr, tok.Type)) return false;
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[ALLSTAR] consume pos={Pos} tok={tok.Type} '{tok.Text}'");
            events?.Add(ParseEvent.Consume(allTokIdx));
            Pos++;
            return true;
        }

        private int[] BuildPredictionTokens(IReadOnlySet<int> expected)
        {
            var cursor = _lexerCursor.Clone();
            var types = new List<int>();
            bool firstOnChannel = true;
            while (true)
            {
                var token = _lexer.NextToken(
                    _input, cursor, firstOnChannel ? expected : null,
                    recordStatistics: false);
                if (token.Channel == DEFAULT_CHANNEL || token.Type == EOF_TYPE)
                {
                    types.Add(token.Type);
                    firstOnChannel = false;
                }
                if (token.Type == EOF_TYPE) break;
            }
            return types.ToArray();
        }

        private int ReadOnChannelToken(IReadOnlySet<int> expected)
        {
            while (true)
            {
                var token = _lexer.NextToken(_input, _lexerCursor, expected);
                int index = _allTokens.Count;
                token.TokenIndex = index;
                _allTokens.Add(token);
                if (token.Channel == DEFAULT_CHANNEL || token.Type == EOF_TYPE)
                {
                    _onIdx.Add(index);
                    return index;
                }
            }
        }

        private HashSet<int> TokenTypesForTransition(MyTransition transition)
        {
            var result = new HashSet<int>();
            if (transition.Matches(EOF_TYPE, 0, _atn.maxTokenType))
                result.Add(EOF_TYPE);
            for (int tokenType = 1; tokenType <= _atn.maxTokenType; tokenType++)
                if (transition.Matches(tokenType, 1, _atn.maxTokenType))
                    result.Add(tokenType);
            return result;
        }

        private static string DescribeTransition(MyTransition transition) => transition switch
        {
            MyAtomTransition atom => $"atom({atom.label})",
            MyRangeTransition range => $"range({range.from}..{range.to})",
            MySetTransition set => $"set({set.set})",
            MyNotSetTransition set => $"not-set({set.set})",
            MyWildcardTransition => "wildcard",
            _ => transition.GetType().Name
        };
    }

    // =========================================================================
    // Terminal helpers
    // =========================================================================

    private static bool TerminalMatches(MyTransition t, int tokenType) => t switch
    {
        MyAtomTransition a    => a.label == tokenType,
        MySetTransition s     => s.set.Contains(tokenType),
        MyNotSetTransition ns => !ns.set.Contains(tokenType) && tokenType != EOF_TYPE,
        MyWildcardTransition  => tokenType != EOF_TYPE,
        MyRangeTransition r   => tokenType >= r.from && tokenType <= r.to,
        _ => false
    };
}
