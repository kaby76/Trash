namespace Trash.EarleyAtn;

// No Antlr4.Runtime.Standard types used anywhere in this file.

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
    public static List<ParseEvent>? Parse(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex)
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

        var events = new List<ParseEvent>();
        var instance = new ParserInstance(atn, allTokens, onIdx, tokenTypes, stateToDecision);
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
        private readonly IReadOnlyList<LexerToken> _allTokens;
        private readonly IReadOnlyList<int> _onIdx;  // on-channel pos → all-token index
        private readonly int[] _tokenTypes;
        private readonly Dictionary<int, int> _stateToDecision;
        private readonly AllStarSimulator _sim;

        public int Pos { get; private set; } // current on-channel token position

        public ParserInstance(MyATN atn, IReadOnlyList<LexerToken> allTokens,
                              IReadOnlyList<int> onIdx, int[] tokenTypes,
                              Dictionary<int, int> stateToDecision)
        {
            _atn = atn;
            _allTokens = allTokens;
            _onIdx = onIdx;
            _tokenTypes = tokenTypes;
            _stateToDecision = stateToDecision;
            _sim = new AllStarSimulator(atn);
        }

        /// <summary>Parse one rule; emits Enter/Exit/Consume events. Returns false on error.</summary>
        public bool ParseRule(int ruleIndex, List<ParseEvent> events, PredictionContext callerCtx)
        {
            events.Add(ParseEvent.EnterRule(ruleIndex));
            var state = _atn.start[ruleIndex];

            while (state.stateType != MyStateType.RuleStop)
            {
                if (state.transitions.Count == 0)
                    throw new InvalidOperationException($"Dead ATN state {state.stateNumber}");

                if (_stateToDecision.TryGetValue(state.stateNumber, out int decision))
                {
                    // Decision point: use ALL(*) prediction to choose an alternative.
                    int alt = _sim.AdaptivePredict(decision, _tokenTypes, Pos, callerCtx);
                    if (AllStarParser.Trace)
                        Console.Error.WriteLine(
                            $"[ALLSTAR] dec={decision} state={state.stateNumber} pos={Pos} " +
                            $"tok={(_onIdx.Count > Pos ? _allTokens[_onIdx[Pos]].Type : -1)} → alt={alt}");
                    if (alt <= 0 || alt > state.transitions.Count)
                    {
                        if (AllStarParser.Trace)
                            Console.Error.WriteLine($"[ALLSTAR] FAIL: alt out of range");
                        return false;
                    }
                    state = state.transitions[alt - 1].target;
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
                            // Push follow state onto context for LL prediction inside the sub-rule.
                            var childCtx = new SingletonPredictionContext(callerCtx, rt.target.stateNumber);
                            if (!ParseRule(rt.ruleIndex, events, childCtx))
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
                                        $"pos={Pos} tok={tokType} tr={tr.GetType().Name}");
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

            events.Add(ParseEvent.ExitRule(ruleIndex));
            return true;
        }

        private bool ConsumeToken(MyTransition tr, List<ParseEvent> events)
        {
            if (Pos >= _onIdx.Count) return false;
            int allTokIdx = _onIdx[Pos];
            var tok = _allTokens[allTokIdx];
            if (!TerminalMatches(tr, tok.Type)) return false;
            if (AllStarParser.Trace)
                Console.Error.WriteLine($"[ALLSTAR] consume pos={Pos} tok={tok.Type} '{tok.Text}'");
            events.Add(ParseEvent.Consume(allTokIdx));
            Pos++;
            return true;
        }
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
