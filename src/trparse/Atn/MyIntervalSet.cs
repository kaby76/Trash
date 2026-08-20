namespace Trash.EarleyAtn;

// Standalone sorted-merged interval set with no Antlr4 runtime dependencies.
public class MyIntervalSet
{
    private readonly List<(int a, int b)> _intervals = new();

    public static MyIntervalSet Of(int a) => Of(a, a);

    public static MyIntervalSet Of(int a, int b)
    {
        var s = new MyIntervalSet();
        s.Add(a, b);
        return s;
    }

    public void Add(int a, int b)
    {
        var merged = new List<(int a, int b)>();
        bool inserted = false;
        int na = a, nb = b;
        foreach (var iv in _intervals)
        {
            if (nb < iv.a - 1)
            {
                if (!inserted) { merged.Add((na, nb)); inserted = true; }
                merged.Add(iv);
            }
            else if (na > iv.b + 1)
            {
                merged.Add(iv);
            }
            else
            {
                na = Math.Min(na, iv.a);
                nb = Math.Max(nb, iv.b);
            }
        }
        if (!inserted) merged.Add((na, nb));
        _intervals.Clear();
        _intervals.AddRange(merged);
    }

    public bool Contains(int v)
    {
        foreach (var iv in _intervals)
        {
            if (v < iv.a) return false;
            if (v <= iv.b) return true;
        }
        return false;
    }

    public IReadOnlyList<(int a, int b)> GetIntervals() => _intervals;
}
