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
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        ParserStatistics statistics = null,
        ParserPredictionCache predictionCache = null)
    {
        return ParseCore(
            atn, allTokens, startRuleIndex, buildEvents: true, statistics,
            predictionCache, enableLl1Bypass: false).Events;
    }

    /// <summary>
    /// Recognize an already-tokenized input without constructing parse events
    /// or a parse tree. This isolates parser prediction and committed ATN
    /// traversal for performance measurement.
    /// </summary>
    public static bool Recognize(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        ParserStatistics statistics = null,
        ParserPredictionCache predictionCache = null)
    {
        return ParseCore(
            atn, allTokens, startRuleIndex, buildEvents: false, statistics,
            predictionCache, enableLl1Bypass: false).Success;
    }

    internal static List<ParseEvent> ParseWithLl1(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        ParserStatistics statistics = null)
    {
        return ParseCore(
            atn, allTokens, startRuleIndex, buildEvents: true,
            statistics, predictionCache: null,
            enableLl1Bypass: true).Events;
    }

    internal static bool RecognizeWithLl1(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex)
    {
        return ParseCore(
            atn, allTokens, startRuleIndex, buildEvents: false,
            statistics: null, predictionCache: null,
            enableLl1Bypass: true).Success;
    }

    private static (bool Success, List<ParseEvent> Events) ParseCore(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        bool buildEvents, ParserStatistics statistics,
        ParserPredictionCache predictionCache, bool enableLl1Bypass)
    {
        if (atn == null) throw new ArgumentNullException(nameof(atn));
        if (startRuleIndex < 0 || startRuleIndex >= atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        // Build on-channel index map: on-channel position → index in allTokens.
        var onIdx = new List<int>(allTokens.Count);
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

        var metadata = CommittedAtnMetadata.For(atn);

        var events = buildEvents ? new List<ParseEvent>() : null;
        var instance = new ParserInstance(
            atn, allTokens, onIdx, tokenTypes, metadata, events, statistics,
            predictionCache, enableLl1Bypass);
        bool success = instance.ParseRule(startRuleIndex, PredictionContext.EMPTY);
        instance.CaptureStatistics();
        if (!success)
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
        out TokenStore allTokens,
        LexerStatistics lexerStatistics = null,
        ParserStatistics parserStatistics = null,
        ParserPredictionCache predictionCache = null,
        LexerAtnSimulator.LexerDfaCache lexerDfaCache = null)
    {
        if (parserAtn == null) throw new ArgumentNullException(nameof(parserAtn));
        if (lexerAtn == null) throw new ArgumentNullException(nameof(lexerAtn));
        if (startRuleIndex < 0 || startRuleIndex >= parserAtn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        var metadata = CommittedAtnMetadata.For(parserAtn);

        allTokens = new TokenStore(input);
        var events = new List<ParseEvent>();
        var instance = new ParserInstance(
            parserAtn, lexerAtn, input, allTokens, metadata,
            events, lexerStatistics, parserStatistics, predictionCache,
            lexerDfaCache);
        bool success = instance.ParseRule(startRuleIndex, PredictionContext.EMPTY);
        instance.CaptureStatistics();
        if (!success)
            return null;
        return events;
    }

    // =========================================================================
    // Internal parser instance — carries mutable state (token position).
    // =========================================================================

    private sealed class ParserInstance
    {
        private readonly MyATN _atn;
        private readonly IReadOnlyList<LexerToken> _allTokens;
        private readonly TokenStore _contextTokens;
        private readonly List<int> _onIdx;  // on-channel pos → all-token index
        private readonly int[] _tokenTypes;
        private readonly CommittedAtnMetadata _metadata;
        private readonly AllStarSimulator _sim;
        private readonly LexerAtnSimulator _lexer;
        private readonly LexerAtnSimulator.Cursor _lexerCursor;
        private readonly string _input;
        private readonly bool _contextAware;
        private readonly ParserStatistics _statistics;
        private readonly ushort[][] _ll1Tables;
        private readonly List<ParseEvent> _events;

        public int Pos { get; private set; } // current on-channel token position

        public ParserInstance(MyATN atn, IReadOnlyList<LexerToken> allTokens,
                              IReadOnlyList<int> onIdx, int[] tokenTypes,
                              CommittedAtnMetadata metadata,
                              List<ParseEvent> events,
                              ParserStatistics statistics = null,
                              ParserPredictionCache predictionCache = null,
                              bool enableLl1Bypass = true)
        {
            _atn = atn;
            _allTokens = allTokens;
            _onIdx = onIdx as List<int> ?? onIdx.ToList();
            _tokenTypes = tokenTypes;
            _metadata = metadata;
            _events = events;
            _statistics = statistics;
            _ll1Tables = enableLl1Bypass
                ? Ll1DecisionAnalyzer.For(atn).Tables
                : null;
            _sim = new AllStarSimulator(atn, statistics, predictionCache);
        }

        public ParserInstance(MyATN parserAtn, MyATN lexerAtn, string input,
                              TokenStore allTokens,
                              CommittedAtnMetadata metadata,
                              List<ParseEvent> events,
                              LexerStatistics lexerStatistics,
                              ParserStatistics parserStatistics = null,
                              ParserPredictionCache predictionCache = null,
                              LexerAtnSimulator.LexerDfaCache lexerDfaCache = null)
        {
            _atn = parserAtn;
            _allTokens = allTokens;
            _contextTokens = allTokens;
            _onIdx = new List<int>();
            _tokenTypes = Array.Empty<int>();
            _metadata = metadata;
            _events = events;
            _statistics = parserStatistics;
            _ll1Tables = null;
            _sim = new AllStarSimulator(
                parserAtn, parserStatistics, predictionCache);
            _lexer = new LexerAtnSimulator(
                lexerAtn, lexerStatistics, lexerDfaCache);
            _lexer.SetInput(input);
            _lexerCursor = new LexerAtnSimulator.Cursor();
            _input = input;
            _contextAware = true;
        }

        /// <summary>Parse one rule; emits Enter/Exit/Consume events. Returns false on error.</summary>
        public bool ParseRule(int ruleIndex, PredictionContext callerCtx,
                              int precedence = 0,
                              int depth = 1)
        {
            if (_statistics != null)
            {
                _statistics.RuleCalls++;
                if (depth > _statistics.MaximumRuleDepth)
                    _statistics.MaximumRuleDepth = depth;
            }
            bool isRecursion = _atn.start[ruleIndex].isPrecedenceRule;
            AddEvent(isRecursion
                ? ParseEventKind.EnterRecursionRule
                : ParseEventKind.EnterRule, ruleIndex);
            var state = _metadata.SkipEpsilon(_atn.start[ruleIndex]);

            while (true)
            {
                int stateNumber = state.stateNumber;
                var stateKind = _metadata.Kind[stateNumber];
                if (stateKind == CommittedStateKind.Stop)
                    break;

                if (_statistics != null) _statistics.CommittedAtnStatesVisited++;
                if (stateKind == CommittedStateKind.Invalid)
                    throw new InvalidOperationException($"Dead ATN state {state.stateNumber}");

                int decision = _metadata.Decision[stateNumber];
                if (stateKind == CommittedStateKind.Decision)
                {
                    var decisionTargets = _metadata.DecisionTargets[stateNumber];
                    int currentTokenType = !_contextAware && Pos < _tokenTypes.Length
                        ? _tokenTypes[Pos]
                        : -1;
                    int alt;
                    var ll1Table = !_contextAware && _ll1Tables != null
                        ? _ll1Tables[decision]
                        : null;
                    int ll1Index = currentTokenType + 1;
                    if (ll1Table != null &&
                        (uint)ll1Index < (uint)ll1Table.Length &&
                        (alt = ll1Table[ll1Index]) != 0)
                    {
                        _statistics?.RecordLl1Bypass(decision);
                    }
                    else
                    {
                        // Unsafe, nullable, overlapping, predicate-dependent,
                        // or context-aware decisions retain ALL(*) prediction.
                        int[] predictionTokens;
                        int predictionStart;
                        if (_contextAware)
                        {
                            var expected = _sim.GetExpectedTokenTypes(
                                state, callerCtx, precedence);
                            predictionTokens = BuildPredictionTokens(expected);
                            predictionStart = 0;
                            currentTokenType = predictionTokens.Length > 0
                                ? predictionTokens[0]
                                : -1;
                        }
                        else
                        {
                            predictionTokens = _tokenTypes;
                            predictionStart = Pos;
                        }
                        alt = _sim.AdaptivePredict(
                            decision, predictionTokens, predictionStart,
                            callerCtx, precedence);
                    }
                    if (AllStarParser.Trace)
                        Console.Error.WriteLine(
                            $"[ALLSTAR] dec={decision} state={state.stateNumber} pos={Pos} " +
                            $"tok={currentTokenType} " +
                            $"prec={precedence} → alt={alt}");
                    if (alt <= 0 || alt > decisionTargets.Length)
                    {
                        if (AllStarParser.Trace)
                            Console.Error.WriteLine($"[ALLSTAR] FAIL: alt out of range");
                        return false;
                    }
                    var nextState = decisionTargets[alt - 1];
                    // Mirror ANTLR4 ParserInterpreter.visitDecisionState: when taking a non-exit
                    // path from the precedence suffix loop, wrap the accumulated context as the
                    // first child of a fresh rule element (PushNewRecursionContext equivalent).
                    if (state.isPrecedenceDecision && nextState.stateType != MyStateType.LoopEnd)
                        AddEvent(ParseEventKind.PushRecursionContext, state.ruleIndex);
                    state = _metadata.SkipEpsilon(nextState);
                }
                else
                {
                    var tr = _metadata.Transition[stateNumber];
                    switch (stateKind)
                    {
                        case CommittedStateKind.Epsilon:
                            state = _metadata.SkipEpsilon(tr.target);
                            break;

                        case CommittedStateKind.Rule:
                            var rt = _metadata.RuleTransition[stateNumber];
                            if (AllStarParser.Trace && rt.precedence != 0)
                                Console.Error.WriteLine(
                                    $"[ALLSTAR] call rule={rt.ruleIndex} from={state.stateNumber} " +
                                    $"prec={rt.precedence} pos={Pos}");
                            // Push follow state onto context for LL prediction inside the sub-rule.
                            var childCtx = _sim.GetChildContext(
                                callerCtx, rt.target.stateNumber, precedence);
                            if (!ParseRule(rt.ruleIndex, childCtx,
                                rt.precedence, depth + 1))
                                return false;
                            state = _metadata.SkipEpsilon(rt.target);
                            break;

                        case CommittedStateKind.Terminal:
                            // Terminal transition: consume the next on-channel token.
                            if (!ConsumeToken(stateNumber, tr))
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
                            state = _metadata.SkipEpsilon(tr.target);
                            break;
                        default:
                            throw new InvalidOperationException(
                                $"Unsupported committed ATN state {state.stateNumber}.");
                    }
                }
            }

            AddEvent(isRecursion
                ? ParseEventKind.ExitRecursionRule
                : ParseEventKind.ExitRule, ruleIndex);
            return true;
        }

        private bool ConsumeToken(int stateNumber, MyTransition tr)
        {
            if (_contextAware)
            {
                var expected = TokenTypesForTransition(tr);
                int contextualTokenIndex = ReadOnChannelToken(expected);
                var contextualToken = _allTokens[contextualTokenIndex];
                if (!_metadata.TerminalMatches(stateNumber, contextualToken.Type))
                    return false;
                if (AllStarParser.Trace)
                    Console.Error.WriteLine(
                        $"[ALLSTAR] consume pos={Pos} tok={contextualToken.Type} '{contextualToken.Text}'");
                AddEvent(ParseEventKind.Consume, contextualTokenIndex);
                Pos++;
                return true;
            }
            if (Pos >= _onIdx.Count) return false;
            int allTokIdx = _onIdx[Pos];
            var tok = _allTokens[allTokIdx];
            if (!_metadata.TerminalMatches(stateNumber, tok.Type)) return false;
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[ALLSTAR] consume pos={Pos} tok={tok.Type} '{tok.Text}'");
            AddEvent(ParseEventKind.Consume, allTokIdx);
            Pos++;
            return true;
        }

        public void CaptureStatistics() => _sim.CaptureRetainedStatistics();

        private void AddEvent(ParseEventKind kind, int index)
        {
            if (_events == null) return;
            _events.Add(new ParseEvent(kind, index));
            if (_statistics != null) _statistics.ParseEventsCreated++;
        }

        private int[] BuildPredictionTokens(IReadOnlySet<int> expected)
        {
            var cursor = _lexerCursor.Clone();
            var types = new List<int>();
            var speculativeTokens = new TokenStore(_input);
            bool firstOnChannel = true;
            bool previousRecordStatistics = _lexer.RecordStatistics;
            _lexer.RecordStatistics = false;
            try
            {
                while (true)
                {
                    int index = _lexer.NextToken(
                        speculativeTokens, cursor,
                        firstOnChannel ? expected : null);
                    var token = speculativeTokens[index];
                    if (token.Channel == DEFAULT_CHANNEL || token.Type == EOF_TYPE)
                    {
                        types.Add(token.Type);
                        firstOnChannel = false;
                    }
                    if (token.Type == EOF_TYPE) break;
                }
            }
            finally { _lexer.RecordStatistics = previousRecordStatistics; }
            return types.ToArray();
        }

        private int ReadOnChannelToken(IReadOnlySet<int> expected)
        {
            while (true)
            {
                int index = _lexer.NextToken(
                    _contextTokens, _lexerCursor, expected);
                var token = _contextTokens[index];
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

}
