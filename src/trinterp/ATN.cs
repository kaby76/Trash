using System;
using System.Collections.Generic;

namespace trinterp;

public class ATN
{
    public readonly ATNType grammarType;
    public readonly int maxTokenType;

    public readonly List<ATNState> states = new();
    public RuleStartState[] ruleToStartState;
    public RuleStopState[] ruleToStopState;
    public int[] ruleToTokenType;
    public readonly List<TokensStartState> modeToStartState = new();
    public readonly Dictionary<string, TokensStartState> modeNameToStartState = new();
    public readonly List<DecisionState> decisionToState = new();
    public ILexerAction[] lexerActions;

    public ATN(ATNType grammarType, int maxTokenType)
    {
        this.grammarType = grammarType;
        this.maxTokenType = maxTokenType;
    }

    public void AddState(ATNState state)
    {
        state.stateNumber = states.Count;
        states.Add(state);
    }

    public void RemoveState(ATNState state)
    {
        states[state.stateNumber] = null;
    }

    public void DefineDecisionState(DecisionState state)
    {
        decisionToState.Add(state);
    }
}
