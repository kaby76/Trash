using System.Collections.Generic;

namespace trinterp;

/// <summary>
/// Removes "tail epsilons" from ATN alternatives.
///
/// If a BasicState p has a single transition to BasicState q (either directly via
/// tr.target, or via RuleTransition.followState), and q has a single EpsilonTransition
/// to a BlockEndState, PlusLoopbackState, or StarLoopbackState r, then q is bypassed:
/// p's transition is updated to reach r directly and q is removed from the ATN.
///
/// Ported from antlr-ng's TailEpsilonRemover.ts.
/// </summary>
public class TailEpsilonRemover
{
    private readonly ATN _atn;

    public TailEpsilonRemover(ATN atn) { _atn = atn; }

    public void Visit(ATNState s) => DoVisit(s, new HashSet<int>());

    private void VisitState(ATNState p)
    {
        if (p.StateType != StateType.Basic || p.NumberOfTransitions != 1) return;

        var tr = p.Transition(0);
        var q = tr is RuleTransition rt ? rt.followState : tr.target;

        if (q.StateType != StateType.Basic || q.NumberOfTransitions != 1) return;

        var qTr = q.Transition(0);
        if (qTr is not EpsilonTransition) return;

        var r = qTr.target;
        if (r is not BlockEndState && r is not PlusLoopbackState && r is not StarLoopbackState) return;

        // Bypass q: point p's transition directly at r.
        if (tr is RuleTransition rt2)
            rt2.followState = r;
        else
            tr.target = r;

        _atn.RemoveState(q);
    }

    private void DoVisit(ATNState s, HashSet<int> visited)
    {
        if (!visited.Add(s.stateNumber)) return;
        VisitState(s);
        for (int i = 0; i < s.NumberOfTransitions; i++)
            DoVisit(s.Transition(i).target, visited);
    }
}
