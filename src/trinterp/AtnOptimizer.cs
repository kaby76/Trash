using System.Collections.Generic;

namespace trinterp;

/// <summary>
/// Post-construction ATN optimizations.
/// Ported from antlr-ng's ATNOptimizer.ts and antlr4's ATNOptimizer.java.
/// </summary>
public static class AtnOptimizer
{
    /// <summary>
    /// Compacts the ATN state array by removing null slots introduced by
    /// TailEpsilonRemover, and renumbers all remaining states contiguously from 0.
    /// </summary>
    public static void OptimizeStates(ATN atn)
    {
        var compressed = new List<ATNState>();
        int i = 0;
        foreach (var s in atn.states)
        {
            if (s != null)
            {
                compressed.Add(s);
                s.stateNumber = i++;
            }
        }
        atn.states.Clear();
        atn.states.AddRange(compressed);
    }

    /// <summary>
    /// Merges consecutive single-atom alternatives inside a block into a single
    /// SetTransition, then removes the now-redundant per-alt states and drops the
    /// block start from <see cref="ATN.decisionToState"/> when it collapses to one
    /// alternative (so it is no longer rendered as a decision record in DOT output).
    ///
    /// Applies to both lexer and parser ATNs (unlike antlr4's tool, which restricts
    /// this to lexer grammars).
    /// </summary>
    public static void OptimizeSets(ATN atn)
    {
        // Snapshot the list; we may remove entries during iteration.
        var decisions = new List<DecisionState>(atn.decisionToState);
        var toRemove  = new List<DecisionState>();

        foreach (var decision in decisions)
        {
            if (decision is not BlockStartState) continue;

            // ---- Identify eligible alt indices --------------------------------
            var eligible = new List<int>();
            for (int i = 0; i < decision.NumberOfTransitions; i++)
            {
                if (decision.Transition(i) is not EpsilonTransition) continue;
                var altState = decision.Transition(i).target;
                if (altState.NumberOfTransitions != 1) continue;
                var altTrans = altState.Transition(0);
                if (altTrans.target is not BlockEndState) continue;
                if (altTrans is NotSetTransition) continue;
                if (altTrans is AtomTransition || altTrans is RangeTransition || altTrans is SetTransition)
                    eligible.Add(i);
            }

            if (eligible.Count < 2) continue;

            // ---- Process each consecutive run in reverse order ----------------
            var runs = new List<(int Start, int End)>(ConsecutiveRuns(eligible));
            for (int ri = runs.Count - 1; ri >= 0; ri--)
            {
                int runStart = runs[ri].Start;
                int runEnd   = runs[ri].End;
                if (runEnd - runStart < 1) continue; // need at least 2

                var blockEnd = (BlockEndState)decision.Transition(runStart).target.Transition(0).target;
                var matchSet = new IntervalSet();

                for (int j = runStart; j <= runEnd; j++)
                {
                    var t = decision.Transition(j).target.Transition(0);
                    if      (t is AtomTransition  at) matchSet.Add(at.token);
                    else if (t is RangeTransition rt) matchSet.Add(rt.from, rt.to);
                    else if (t is SetTransition   st) matchSet.AddAll(st.set);
                }

                // Build new transition: single atom if possible, set otherwise.
                Transition newTrans = matchSet.ElementCount == 1
                    ? (Transition)new AtomTransition(blockEnd, matchSet.MinElement)
                    : new SetTransition(blockEnd, matchSet);

                // Replace the first alt state's transition with the merged one.
                decision.Transition(runStart).target.SetTransition(0, newTrans);

                // Remove subsequent alt transitions (and their states).
                for (int j = runStart + 1; j <= runEnd; j++)
                {
                    var stateToRemove = decision.Transition(runStart + 1).target;
                    decision.RemoveTransition(runStart + 1);
                    atn.RemoveState(stateToRemove);
                }
            }

            // If the block now has only one alternative it is no longer a decision.
            if (decision.NumberOfTransitions == 1)
                toRemove.Add(decision);
        }

        foreach (var d in toRemove)
            atn.decisionToState.Remove(d);

        // Rebuild the decision-index field on every remaining decision state.
        for (int i = 0; i < atn.decisionToState.Count; i++)
            atn.decisionToState[i].decision = i;
    }

    // -------------------------------------------------------------------------

    private static IEnumerable<(int Start, int End)> ConsecutiveRuns(List<int> sorted)
    {
        if (sorted.Count == 0) yield break;
        int runStart = sorted[0], prev = sorted[0];
        for (int i = 1; i < sorted.Count; i++)
        {
            if (sorted[i] != prev + 1)
            {
                yield return (runStart, prev);
                runStart = sorted[i];
            }
            prev = sorted[i];
        }
        yield return (runStart, prev);
    }
}
