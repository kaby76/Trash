namespace trinterp;

/// <summary>
/// Post-construction ATN optimizations.
/// Ported from antlr-ng's ATNOptimizer.ts.
/// </summary>
public static class AtnOptimizer
{
    /// <summary>
    /// Compacts the ATN state array by removing null slots introduced by
    /// TailEpsilonRemover and ElemList, and renumbers all remaining states
    /// contiguously from 0.
    /// </summary>
    public static void OptimizeStates(ATN atn)
    {
        var compressed = new System.Collections.Generic.List<ATNState>();
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
}
