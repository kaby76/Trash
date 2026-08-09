using System.Collections.Generic;

namespace trinterp;

/// <summary>
/// Maps each ATN state number to the set of (line, col) pairs in the .g4 source
/// that it corresponds to.
///
/// Formal definition
/// -----------------
///   DirectPreLocs(s)  = { (s.SourceLine, s.SourceColumn) |
///                          s has ≥1 outgoing non-epsilon transition ∧ s.SourceLine ≥ 0 }
///
///   DirectPostLocs(u) = { (t.SourceEndLine, t.SourceEndColumn) |
///                          ∃ non-eps transition t→u ∧ t.SourceEndLine ≥ 0 }
///
///   fwd(s) = states reachable from s via ≥0 epsilon transitions (forward epsilon closure)
///   bwd(s) = states from which s is reachable via ≥0 epsilon transitions (backward epsilon closure)
///
///   Locs(s) = ⋃{ DirectPreLocs(t)  | t ∈ fwd(s) }
///           ∪ ⋃{ DirectPostLocs(t) | t ∈ bwd(s) }
///
/// Intuition
/// ---------
///   • A decision/entry state (s26, s124) maps to the start positions of all match
///     states reachable from it via epsilon transitions.
///   • A post-match merge state (s125 BlockEnd, s27 RuleStop) maps to the exclusive-end
///     positions of all grammar elements that epsilon-transition into it.
/// </summary>
public sealed class StateLocationMap
{
    /// <summary>An (line, col) pair in .g4 source (1-based line, 0-based col).</summary>
    public readonly record struct SourceLoc(int Line, int Col);

    // stateNumber → set of source locations
    private readonly Dictionary<int, HashSet<SourceLoc>> _map = new();

    private StateLocationMap() { }

    /// <summary>Build the map for the given ATN.</summary>
    public static StateLocationMap Build(ATN atn)
    {
        var m = new StateLocationMap();
        m.Compute(atn);
        return m;
    }

    /// <summary>
    /// Returns the set of source locations for <paramref name="stateNumber"/>,
    /// or an empty set if none.
    /// </summary>
    public IReadOnlySet<SourceLoc> this[int stateNumber] =>
        _map.TryGetValue(stateNumber, out var s) ? s : _empty;

    private static readonly HashSet<SourceLoc> _empty = new();

    // =========================================================================

    private void Compute(ATN atn)
    {
        var states = atn.states;

        // Step 1: Collect epsilon and non-epsilon edges.
        //
        // For each state with ≥1 non-epsilon outgoing transition:
        //   directPre[s] = { (s.SourceLine, s.SourceColumn) }  (if SourceLine ≥ 0)
        //
        // For each state u that is the target of a non-epsilon transition from t:
        //   directPost[u] += { (t.SourceEndLine, t.SourceEndColumn) }  (if SourceEndLine ≥ 0)
        //
        // epsEdges: list of (from, to) for epsilon transitions (used for closure BFS)

        var directPre  = new Dictionary<int, HashSet<SourceLoc>>();
        var directPost = new Dictionary<int, HashSet<SourceLoc>>();
        var epsEdgesFrom = new Dictionary<int, List<int>>(); // from → list of to
        var epsEdgesTo   = new Dictionary<int, List<int>>(); // to → list of from (reverse)

        foreach (var s in states)
        {
            if (s == null) continue;
            bool hasNonEps = false;
            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var t = s.Transition(i);
                if (t is EpsilonTransition)
                {
                    AddEdge(epsEdgesFrom, s.stateNumber, t.target.stateNumber);
                    AddEdge(epsEdgesTo,   t.target.stateNumber, s.stateNumber);
                }
                else
                {
                    hasNonEps = true;
                    // directPost for the post-transition state.
                    // For RuleTransition the logical "return" state is followState, not target
                    // (target is the called rule's start state, which is far away).
                    var postState = t is RuleTransition rt ? rt.followState : t.target;
                    if (s.SourceEndLine >= 0)
                    {
                        var loc = new SourceLoc(s.SourceEndLine, s.SourceEndColumn);
                        AddLoc(directPost, postState.stateNumber, loc);
                    }
                }
            }
            if (hasNonEps && s.SourceLine >= 0)
            {
                AddLoc(directPre, s.stateNumber, new SourceLoc(s.SourceLine, s.SourceColumn));
            }
        }

        // Step 2: Forward epsilon closure.
        // fwdPre[s] = ⋃{ directPre[t] | t ∈ fwd(s) }
        // We propagate directPre backwards through epsilon edges:
        // if there is an eps edge s→t, then fwdPre[s] ⊇ fwdPre[t].
        // This is equivalent to computing fwd(s) for each s and unioning directPre.
        // We do it efficiently via a reverse-topological BFS on the epsilon graph.

        var fwdPre = new Dictionary<int, HashSet<SourceLoc>>();

        // Seed with direct values
        foreach (var (sn, locs) in directPre)
            GetOrCreate(fwdPre, sn).UnionWith(locs);

        // Propagate: for each eps edge from→to, fwdPre[from] gets fwdPre[to].
        // We use a worklist: start with all states that have directPre, propagate
        // backwards through eps edges until fixed point.
        var worklist = new Queue<int>(directPre.Keys);
        while (worklist.Count > 0)
        {
            var sn = worklist.Dequeue();
            if (!fwdPre.TryGetValue(sn, out var locs) || locs.Count == 0) continue;
            // Any state that has an eps edge to sn should also inherit locs.
            if (!epsEdgesTo.TryGetValue(sn, out var preds)) continue;
            foreach (var pred in preds)
            {
                var predSet = GetOrCreate(fwdPre, pred);
                int before = predSet.Count;
                predSet.UnionWith(locs);
                if (predSet.Count != before)
                    worklist.Enqueue(pred);
            }
        }

        // Step 3: Backward epsilon closure.
        // bwdPost[s] = ⋃{ directPost[t] | t ∈ bwd(s) }
        // Propagate directPost forward through epsilon edges:
        // if there is an eps edge from→to, then bwdPost[to] ⊇ bwdPost[from].

        var bwdPost = new Dictionary<int, HashSet<SourceLoc>>();

        foreach (var (sn, locs) in directPost)
            GetOrCreate(bwdPost, sn).UnionWith(locs);

        worklist = new Queue<int>(directPost.Keys);
        while (worklist.Count > 0)
        {
            var sn = worklist.Dequeue();
            if (!bwdPost.TryGetValue(sn, out var locs) || locs.Count == 0) continue;
            if (!epsEdgesFrom.TryGetValue(sn, out var succs)) continue;
            foreach (var succ in succs)
            {
                var succSet = GetOrCreate(bwdPost, succ);
                int before = succSet.Count;
                succSet.UnionWith(locs);
                if (succSet.Count != before)
                    worklist.Enqueue(succ);
            }
        }

        // Step 4: Locs(s) = fwdPre[s] ∪ bwdPost[s]
        var allKeys = new HashSet<int>();
        foreach (var k in fwdPre.Keys)  allKeys.Add(k);
        foreach (var k in bwdPost.Keys) allKeys.Add(k);

        foreach (var sn in allKeys)
        {
            var combined = new HashSet<SourceLoc>();
            if (fwdPre.TryGetValue(sn,  out var pre))  combined.UnionWith(pre);
            if (bwdPost.TryGetValue(sn, out var post)) combined.UnionWith(post);
            if (combined.Count > 0)
                _map[sn] = combined;
        }
    }

    // =========================================================================
    // Helpers

    private static void AddEdge(Dictionary<int, List<int>> dict, int key, int value)
    {
        if (!dict.TryGetValue(key, out var list))
            dict[key] = list = new List<int>();
        list.Add(value);
    }

    private static void AddLoc(Dictionary<int, HashSet<SourceLoc>> dict, int key, SourceLoc loc)
    {
        if (!dict.TryGetValue(key, out var set))
            dict[key] = set = new HashSet<SourceLoc>();
        set.Add(loc);
    }

    private static HashSet<SourceLoc> GetOrCreate(Dictionary<int, HashSet<SourceLoc>> dict, int key)
    {
        if (!dict.TryGetValue(key, out var set))
            dict[key] = set = new HashSet<SourceLoc>();
        return set;
    }
}
