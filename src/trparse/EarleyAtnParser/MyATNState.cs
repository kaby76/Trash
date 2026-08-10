namespace Trash.EarleyAtn;

public class MyATNState
{
    public int stateNumber;
    public int ruleIndex;
    public MyStateType stateType;
    public List<MyTransition> transitions = new();

    // For LoopEnd states
    public MyATNState loopBackState;
    // For BlockStart/PlusBlockStart/StarBlockStart states
    public MyATNState endState;

    public void AddTransition(MyTransition t) => transitions.Add(t);

    public override string ToString() => $"{stateType}({stateNumber},r={ruleIndex})";
}
