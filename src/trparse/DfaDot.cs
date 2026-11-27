namespace Trash;

internal static class DfaDot
{
    internal static string Quote(string value) => "\"" + value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("\r", "\\r", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal)
        .Replace("\t", "\\t", StringComparison.Ordinal) + "\"";

    internal static string Character(int value) => value switch
    {
        -1 => "EOF",
        '\n' => "\\n",
        '\r' => "\\r",
        '\t' => "\\t",
        < 32 or > 126 => $"U+{value:X4}",
        _ => ((char)value).ToString()
    };
}
