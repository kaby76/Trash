using System;
using System.Collections.Generic;
using System.Text;

namespace trinterp;

public class IntervalSet
{
    private readonly List<Interval> _intervals = new();

    public static IntervalSet Of(int a) => Of(a, a);

    public static IntervalSet Of(int a, int b)
    {
        var s = new IntervalSet();
        s.Add(a, b);
        return s;
    }

    public void Add(int v) => Add(v, v);

    public void Add(int a, int b)
    {
        if (b < a) return;
        AddInterval(new Interval(a, b));
    }

    private void AddInterval(Interval addition)
    {
        int insertIdx = _intervals.Count;
        int firstOverlap = -1;
        int lastOverlap = -1;
        var merged = addition;

        for (int i = 0; i < _intervals.Count; i++)
        {
            var r = _intervals[i];
            if (addition.b < r.a - 1)
            {
                insertIdx = i;
                break;
            }
            if (addition.a > r.b + 1)
                continue;
            // overlap or adjacent
            if (firstOverlap < 0) firstOverlap = i;
            lastOverlap = i;
            merged = new Interval(Math.Min(merged.a, r.a), Math.Max(merged.b, r.b));
        }

        if (firstOverlap < 0)
            _intervals.Insert(insertIdx, merged);
        else
        {
            _intervals[firstOverlap] = merged;
            if (lastOverlap > firstOverlap)
                _intervals.RemoveRange(firstOverlap + 1, lastOverlap - firstOverlap);
        }
    }

    public bool Contains(int el)
    {
        foreach (var iv in _intervals)
        {
            if (el < iv.a) return false;
            if (el <= iv.b) return true;
        }
        return false;
    }

    public IList<Interval> GetIntervals() => _intervals;

    /// <summary>Total number of integer elements (sum of interval widths).</summary>
    public int ElementCount
    {
        get
        {
            int n = 0;
            foreach (var iv in _intervals) n += iv.b - iv.a + 1;
            return n;
        }
    }

    /// <summary>Number of intervals (not total elements).</summary>
    public int Count => _intervals.Count;

    /// <summary>Smallest element across all intervals, or -1 if empty.</summary>
    public int MinElement => _intervals.Count == 0 ? -1 : _intervals[0].a;

    public void AddAll(IntervalSet other)
    {
        foreach (var iv in other._intervals)
            Add(iv.a, iv.b);
    }

    public IntervalSet Complement(IntervalSet vocabulary)
    {
        var result = new IntervalSet();
        if (vocabulary._intervals.Count == 0) return result;
        int minEl = vocabulary._intervals[0].a;
        int maxEl = vocabulary._intervals[^1].b;
        int next = minEl;
        foreach (var iv in _intervals)
        {
            if (iv.b < next) continue;
            if (iv.a > maxEl) break;
            int gapEnd = Math.Min(iv.a - 1, maxEl);
            if (next <= gapEnd) result.Add(next, gapEnd);
            next = iv.b + 1;
            if (next > maxEl) break;
        }
        if (next <= maxEl) result.Add(next, maxEl);
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder("{");
        bool first = true;
        foreach (var iv in _intervals)
        {
            if (!first) sb.Append(", ");
            first = false;
            if (iv.a == iv.b) sb.Append(iv.a);
            else { sb.Append(iv.a); sb.Append(".."); sb.Append(iv.b); }
        }
        sb.Append("}");
        return sb.ToString();
    }
}
