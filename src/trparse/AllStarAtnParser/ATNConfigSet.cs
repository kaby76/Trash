namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

// A set of ATNConfig values for ALL(*) prediction.
// Deduplication key: (stateNumber, alt) — one config per (state, alt) pair.
// This is correct for SLL (EMPTY context) and a pragmatic approximation for LL.

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

public sealed class ATNConfigSet
{
    private readonly HashSet<(int stateNum, int alt)> _keys = new();
    private readonly List<ATNConfig> _configs = new();

    public IReadOnlyList<ATNConfig> Configs => _configs;
    public bool IsEmpty => _configs.Count == 0;

    // Add a config. Returns true if it was actually added (not a duplicate).
    public bool Add(ATNConfig c)
    {
        if (!_keys.Add((c.State.stateNumber, c.Alt)))
            return false;
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

    // Lowest alternative number present, or -1 if empty.
    public int MinAlt()
    {
        int min = int.MaxValue;
        foreach (var c in _configs)
            if (c.Alt < min) min = c.Alt;
        return min == int.MaxValue ? -1 : min;
    }
}
