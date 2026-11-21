using System;
using System.Text.RegularExpressions;

namespace trinterp;

/// <summary>
/// Converts ANTLR Unicode-property character sets to explicit CongoCC ranges.
/// CongoCC's lexical grammar accepts Unicode code points but not \p{...}.
/// The source sets come from the same resolver used by the interpreted lexer.
/// </summary>
internal static class CongoUnicodeExpander
{
    private static readonly Regex Placeholder = new(
        "__TRASH_UNICODE_([A-Za-z0-9_]+)__", RegexOptions.Compiled);

    public static string Expand(string grammar) => Placeholder.Replace(grammar, match =>
    {
        var property = match.Groups[1].Value;
        var set = LexerAtnFactory.GetUnicodePropertySet(property);
        if (set.Count == 0)
            throw new InvalidOperationException($"Unicode property '{property}' is empty.");
        var parts = new string[set.Count];
        var intervals = set.GetIntervals();
        for (var i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            parts[i] = interval.a == interval.b
                ? Quote(interval.a)
                : Quote(interval.a) + "-" + Quote(interval.b);
        }
        return string.Join(", ", parts);
    });

    private static string Quote(int codePoint) => codePoint <= 0xFFFF
        ? $"\"\\u{codePoint:X4}\""
        : "\"" + char.ConvertFromUtf32(codePoint) + "\"";
}
