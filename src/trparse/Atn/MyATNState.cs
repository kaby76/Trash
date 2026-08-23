namespace Atn;

public class MyATNState
{
    public int stateNumber;
    public int ruleIndex;
    public MyStateType stateType;
    public List<MyTransition> transitions = new();

    // For RuleStart states of left-recursive rules
    public bool isPrecedenceRule;
    // For lexer decisions created from non-greedy operators such as .*?
    public bool nonGreedy;
    // For StarLoopEntry states that are the top-level suffix loop of a left-recursive rule
    public bool isPrecedenceDecision;

    // For LoopEnd states
    public MyATNState loopBackState;
    // For BlockStart/PlusBlockStart/StarBlockStart states
    public MyATNState endState;

    public void AddTransition(MyTransition t) => transitions.Add(t);

    public override string ToString() => $"{stateType}({stateNumber},r={ruleIndex})";
}
