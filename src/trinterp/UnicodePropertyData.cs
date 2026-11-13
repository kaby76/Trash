using System;
using System.Collections.Generic;
using System.IO;

namespace trinterp;

/// <summary>ANTLR Unicode binary and enumerated property ranges used by grammar fixtures.</summary>
internal static class UnicodePropertyData
{
    private static readonly Lazy<Dictionary<string, IntervalSet>> Sets = new(Load);

    internal static bool TryGet(string name, out IntervalSet set)
    {
        if (Sets.Value.TryGetValue(name, out set))
            return true;

        var key = name switch
        {
            "id_start" or "id_continue" or "emoji" or "emoji_presentation"
                or "extended_pictographic" => "binary_property=" + name,
            _ => name
        };
        if (Sets.Value.TryGetValue(key, out set))
            return true;

        var emoji = Sets.Value["binary_property=emoji"];
        var presentation = Sets.Value["binary_property=emoji_presentation"];
        set = name switch
        {
            "emojipresentation=emojidefault" => Intersect(emoji, presentation),
            "emojipresentation=textdefault" =>
                Intersect(emoji, presentation.Complement(IntervalSet.Of(0, 0x10FFFF))),
            "emojipresentation=text" => emoji.Complement(IntervalSet.Of(0, 0x10FFFF)),
            _ => null
        };
        return set != null;
    }

    private static IntervalSet Intersect(IntervalSet a, IntervalSet b)
    {
        var result = new IntervalSet();
        var left = a.GetIntervals();
        var right = b.GetIntervals();
        int i = 0, j = 0;
        while (i < left.Count && j < right.Count)
        {
            int start = Math.Max(left[i].a, right[j].a);
            int end = Math.Min(left[i].b, right[j].b);
            if (start <= end)
                result.Add(start, end);
            if (left[i].b < right[j].b) i++;
            else j++;
        }
        return result;
    }

    private static Dictionary<string, IntervalSet> Load()
    {
        using var stream = typeof(UnicodePropertyData).Assembly.GetManifestResourceStream(
            "trinterp.UnicodePropertyData.txt")
            ?? throw new InvalidOperationException("Embedded Unicode property ranges are missing.");
        using var reader = new StreamReader(stream);
        var result = new Dictionary<string, IntervalSet>(StringComparer.Ordinal);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith('#')) continue;
            var separator = line.IndexOf('|');
            if (separator < 0)
                throw new FormatException("Invalid Unicode property data line.");
            var set = new IntervalSet();
            foreach (var range in line[(separator + 1)..].Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var ends = range.Split('-', 2);
                set.Add(Convert.ToInt32(ends[0], 16), Convert.ToInt32(ends[1], 16));
            }
            result.Add(line[..separator], set);
        }
        return result;
    }
}
