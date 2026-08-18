namespace Trash.EarleyAtn;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

// Interpretive ALL(*) parser.
// Drives a top-down LL parse using AdaptivePredict at every decision point.
// Produces an IParseTree (GenericRuleContext / TerminalNodeImpl) compatible with ConvertToDOM.
public static class AllStarParser
{
    // allTokens: full token list (all channels); EOF at end.
    // Returns the root parse tree, or null if parsing fails.
    public static IParseTree Parse(MyATN atn, IReadOnlyList<IToken> allTokens, int startRuleIndex)
    {
        if (atn == null) throw new ArgumentNullException(nameof(atn));
        if (startRuleIndex < 0 || startRuleIndex >= atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        // Build on-channel token list (DEFAULT_CHANNEL + EOF), same filter as EarleyParser.
        var onChannel = new List<IToken>();
        foreach (var t in allTokens)
            if (t.Channel == TokenConstants.DefaultChannel || t.Type == TokenConstants.EOF)
                onChannel.Add(t);

        // Extract token types for the simulator (no Antlr4 types needed there).
        var tokenTypes = new int[onChannel.Count];
        for (int i = 0; i < onChannel.Count; i++)
            tokenTypes[i] = onChannel[i].Type;

        // Build reverse map: ATN state number → decision index.
        var stateToDecision = new Dictionary<int, int>();
        for (int i = 0; i < atn.decisionToState.Length; i++)
            stateToDecision[atn.decisionToState[i].stateNumber] = i;

        var instance = new ParserInstance(atn, onChannel, tokenTypes, stateToDecision);
        return instance.ParseRule(startRuleIndex, null, PredictionContext.EMPTY);
    }

    // =========================================================================
    // Internal parser instance — carries mutable state (token position).
    // =========================================================================

    private sealed class ParserInstance
    {
        private readonly MyATN _atn;
        private readonly IReadOnlyList<IToken> _tokens; // on-channel
        private readonly int[] _tokenTypes;             // parallel int[] for simulator
        private readonly Dictionary<int, int> _stateToDecision;
        private readonly AllStarSimulator _sim;

        public int Pos { get; private set; } // current on-channel token position

        public ParserInstance(MyATN atn, IReadOnlyList<IToken> tokens, int[] tokenTypes,
                              Dictionary<int, int> stateToDecision)
        {
            _atn = atn;
            _tokens = tokens;
            _tokenTypes = tokenTypes;
            _stateToDecision = stateToDecision;
            _sim = new AllStarSimulator(atn);
        }

        // Parse one rule; returns the rule context or null on error.
        // callerCtx: the PredictionContext representing the call stack above this rule.
        public GenericRuleContext ParseRule(int ruleIndex, GenericRuleContext parent, PredictionContext callerCtx)
        {
            var ctx = new GenericRuleContext(parent, -1, ruleIndex);
            if (Pos < _tokens.Count)
                ctx.Start = _tokens[Pos];

            var state = _atn.start[ruleIndex];

            while (state.stateType != MyStateType.RuleStop)
            {
                if (state.transitions.Count == 0)
                    throw new InvalidOperationException($"Dead ATN state {state.stateNumber}");

                if (_stateToDecision.TryGetValue(state.stateNumber, out int decision))
                {
                    // Decision point: use ALL(*) prediction to choose an alternative.
                    int alt = _sim.AdaptivePredict(decision, _tokenTypes, Pos, callerCtx);
                    if (alt <= 0 || alt > state.transitions.Count)
                        return null; // prediction failure

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
                            // Push the follow state onto the context for LL prediction inside the sub-rule.
                            var childCtx = new SingletonPredictionContext(callerCtx, rt.target.stateNumber);
                            var child = ParseRule(rt.ruleIndex, ctx, childCtx);
                            if (child != null) ctx.AddChild(child);
                            // Advance to the follow state (after the rule call).
                            state = rt.target;
                            break;

                        default:
                            // Terminal transition: consume the next token.
                            if (!ConsumeToken(tr, ctx))
                                return null;
                            state = tr.target;
                            break;
                    }
                }
                else
                {
                    // Multiple transitions but not in decisionToState — shouldn't happen in a valid ATN.
                    throw new InvalidOperationException(
                        $"ATN state {state.stateNumber} (type={state.stateType}) has " +
                        $"{state.transitions.Count} transitions but is not a registered decision state.");
                }
            }

            if (Pos > 0)
                ctx.Stop = _tokens[Math.Min(Pos - 1, _tokens.Count - 1)];
            return ctx;
        }

        private bool ConsumeToken(MyTransition tr, GenericRuleContext ctx)
        {
            if (Pos >= _tokens.Count) return false;
            var tok = _tokens[Pos];
            if (!tr.Matches(tok.Type, 0, _atn.maxTokenType)) return false;
            var term = new TerminalNodeImpl(tok);
            ctx.AddChild(term);
            ctx.Stop = tok;
            Pos++;
            return true;
        }
    }

    // =========================================================================
    // Generic parse-tree node (same shape as EarleyParser's private equivalent).
    // =========================================================================

    private sealed class GenericRuleContext : ParserRuleContext
    {
        private readonly int _ruleIndex;
        public override int RuleIndex => _ruleIndex;

        public GenericRuleContext(ParserRuleContext parent, int invokingState, int ruleIndex)
            : base(parent, invokingState) => _ruleIndex = ruleIndex;
    }
}
