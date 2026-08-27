namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

// A set of ATNConfig values for ALL(*) prediction.
// Full-context prediction must retain configurations which reach the same state
// and alternative through different call stacks. Those contexts can have
// different viable return paths, so dropping either one changes prediction.

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

public sealed class ATNConfigSet
{
    private readonly Dictionary<(int stateNum, int alt, int precedence), int> _configIndex = new();
    private readonly List<ATNConfig> _configs = new();

    public IReadOnlyList<ATNConfig> Configs => _configs;
    public bool IsEmpty => _configs.Count == 0;

    // Add a config. Returns true if it was actually added (not a duplicate).
    public bool Add(ATNConfig c)
    {
        var key = (c.State.stateNumber, c.Alt, c.Precedence);
        if (_configIndex.TryGetValue(key, out int index))
        {
            var existing = _configs[index];
            var merged = PredictionContextMerger.Merge(existing.Context, c.Context);
            if (ReferenceEquals(merged, existing.Context)) return false;
            _configs[index] = existing.WithStateAndContext(existing.State, merged);
            return true;
        }
        _configIndex.Add(key, _configs.Count);
        _configs.Add(c);
        return true;
    }

    // All distinct alternatives present in the set.
    public HashSet<int> GetAlts()
    {
        var alts = new HashSet<int>();
        foreach (var c in _configs) alts.Add(c.Alt);
        return alts;
    }

    // If every config has the same alt, return it; otherwise -1.
    public int GetUniqueAlt()
    {
        int? unique = null;
        foreach (var c in _configs)
        {
            if (unique == null) unique = c.Alt;
            else if (unique != c.Alt) return -1;
        }
        return unique ?? -1;
    }

    // Minimum alternative which has completed at the outer prediction
    // boundary. It must be recognized before another token is consumed;
    // otherwise the stop configuration disappears from the reach set and a
    // different alternative can look falsely unique.
    public int GetCompletedAlt()
    {
        int minimum = int.MaxValue;
        foreach (var c in _configs)
            if (c.State.stateType == MyStateType.RuleStop && c.Context.HasEmptyPath && c.Alt < minimum)
                minimum = c.Alt;
        return minimum == int.MaxValue ? -1 : minimum;
    }

    // Return the minimum alternative when every surviving ATN state/context
    // pair contains the same conflicting set of alternatives. At that point
    // the alternatives have reconverged and their future behaviour is
    // identical, so additional lookahead cannot distinguish them.
    public int GetExactAmbiguityAlt()
    {
        Dictionary<(int stateNum, PredictionContext context, int precedence), HashSet<int>> groups = new();
        foreach (var c in _configs)
        {
            var key = (c.State.stateNumber, c.Context, c.Precedence);
            if (!groups.TryGetValue(key, out var alts))
            {
                alts = new HashSet<int>();
                groups.Add(key, alts);
            }
            alts.Add(c.Alt);
        }

        HashSet<int> expected = null;
        foreach (var alts in groups.Values)
        {
            if (alts.Count < 2)
                return -1;
            if (expected == null)
                expected = alts;
            else if (!expected.SetEquals(alts))
                return -1;
        }

        return expected == null ? -1 : expected.Min();
    }

    // ANTLR's terminating-conflict condition: if every state/context subset
    // contains multiple alternatives, no singleton path remains which could
    // make one alternative uniquely viable with more lookahead.
    public int GetAllSubsetsConflictAlt()
    {
        Dictionary<(int stateNum, PredictionContext context, int precedence), HashSet<int>> groups = new();
        int minimum = int.MaxValue;
        foreach (var c in _configs)
        {
            var key = (c.State.stateNumber, c.Context, c.Precedence);
            if (!groups.TryGetValue(key, out var alts))
            {
                alts = new HashSet<int>();
                groups.Add(key, alts);
            }
            alts.Add(c.Alt);
            if (c.Alt < minimum)
                minimum = c.Alt;
        }

        foreach (var alts in groups.Values)
            if (alts.Count < 2)
                return -1;

        return minimum == int.MaxValue ? -1 : minimum;
    }

    // Lowest alternative number present, or -1 if empty.
    public int MinAlt()
    {
        int min = int.MaxValue;
        foreach (var c in _configs)
            if (c.Alt < min) min = c.Alt;
        return min == int.MaxValue ? -1 : min;
    }
}
