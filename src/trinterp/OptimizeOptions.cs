using System.Collections.Generic;

namespace trinterp;

/// <summary>
/// Controls which ATN optimizations are applied during and after ATN construction.
/// </summary>
public sealed class OptimizeOptions
{
    /// <summary>
    /// Remove tail-epsilon states: a BasicState whose only transition is an epsilon to
    /// a BlockEndState, PlusLoopbackState, or StarLoopbackState is bypassed and removed.
    /// </summary>
    public bool TailEpsilon { get; init; }

    /// <summary>
    /// Merge consecutive single-atom alternatives inside a block into a single
    /// SetTransition, reducing the number of states and decision points.
    /// </summary>
    public bool MergeSets { get; init; }

    public static readonly OptimizeOptions All  = new() { TailEpsilon = true,  MergeSets = true  };
    public static readonly OptimizeOptions None = new() { TailEpsilon = false, MergeSets = false };

    /// <summary>
    /// Parse a collection of optimization names (e.g. "tail-epsilon", "merge-sets").
    /// Passing "none" disables all.
    /// </summary>
    public static OptimizeOptions FromNames(IEnumerable<string> names)
    {
        bool tailEps = false, mergeSets = false;
        foreach (var name in names)
        {
            switch (name.Trim().ToLowerInvariant())
            {
                case "tail-epsilon": tailEps  = true; break;
                case "merge-sets":   mergeSets = true; break;
                case "none":         return None;
            }
        }
        return new() { TailEpsilon = tailEps, MergeSets = mergeSets };
    }

    public bool Any => TailEpsilon || MergeSets;
}
