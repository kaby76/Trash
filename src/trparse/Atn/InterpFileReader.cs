namespace Atn;

public class ParsedInterp
{
    public string[] LiteralNames = Array.Empty<string>();
    public string[] SymbolicNames = Array.Empty<string>();
    public string[] RuleNames = Array.Empty<string>();
    public string[] ChannelNames = Array.Empty<string>();
    public string[] ModeNames = Array.Empty<string>();
    public int[] AtnData = Array.Empty<int>();
    /// <summary>
    /// ATN state number of the parser start rule's start state, or -1 if
    /// the 'start-rule:' section is absent (lexer interp files and old files).
    /// </summary>
    public int StartStateNumber = -1;
    public Dictionary<(int Rule, int Predicate), MyATN.G4XExclusionOperand[]> G4XExclusions = new();
}

public static class InterpFileReader
{
    public static ParsedInterp Read(string text)
    {
        var result = new ParsedInterp();
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        int i = 0;

        SkipToSection(lines, ref i, "token literal names:");
        result.LiteralNames = ReadSection(lines, ref i);

        SkipToSection(lines, ref i, "token symbolic names:");
        result.SymbolicNames = ReadSection(lines, ref i);

        SkipToSection(lines, ref i, "rule names:");
        result.RuleNames = ReadSection(lines, ref i);

        // channel names / mode names are lexer-only (optional)
        int saved = i;
        if (TrySkipToSection(lines, ref i, "channel names:"))
            result.ChannelNames = ReadSection(lines, ref i);
        else
            i = saved;

        saved = i;
        if (TrySkipToSection(lines, ref i, "mode names:"))
            result.ModeNames = ReadSection(lines, ref i);
        else
            i = saved;

        // ATN data: single line "[n1, n2, ...]"
        SkipToSection(lines, ref i, "atn:");
        string atnStr = "";
        while (i < lines.Length)
        {
            var line = lines[i++].Trim();
            if (line.Length > 0) { atnStr = line; break; }
        }

        if (atnStr.StartsWith('[') && atnStr.EndsWith(']'))
            atnStr = atnStr[1..^1];

        var parts = atnStr.Split(',');
        result.AtnData = new int[parts.Length];
        for (int j = 0; j < parts.Length; j++)
            result.AtnData[j] = int.Parse(parts[j].Trim());

        // Optional start-rule section (parser interp files only).
        if (TrySkipToSection(lines, ref i, "start-rule:"))
        {
            while (i < lines.Length)
            {
                var line = lines[i++].Trim();
                if (line.Length > 0) { result.StartStateNumber = int.Parse(line); break; }
            }
        }

        if (TrySkipToSection(lines, ref i, "g4x-set-differences:"))
        {
            foreach (var entry in ReadSection(lines, ref i))
            {
                var fields = entry.Split(':', 3);
                if (fields.Length != 3 || !int.TryParse(fields[0], out int rule) ||
                    !int.TryParse(fields[1], out int predicate) ||
                    rule < 0 || rule >= result.RuleNames.Length)
                    throw new InvalidDataException($"Invalid G4X set-difference entry '{entry}'.");
                var pairs = System.Text.Json.JsonSerializer.Deserialize<string[][]>(
                    Convert.FromBase64String(fields[2]))
                    ?? throw new InvalidDataException("Empty G4X set-difference operand list.");
                var operands = pairs.Select(pair =>
                {
                    if (pair.Length != 2 || pair[0] is not ("rule" or "literal"))
                        throw new InvalidDataException($"Invalid G4X set-difference operand in '{entry}'.");
                    if (pair[0] == "rule")
                    {
                        int excludedRule = Array.IndexOf(result.RuleNames, pair[1]);
                        if (excludedRule < 0)
                            throw new InvalidDataException($"Unknown G4X exclusion rule '{pair[1]}'.");
                        return new MyATN.G4XExclusionOperand("rule", excludedRule.ToString());
                    }
                    return new MyATN.G4XExclusionOperand("literal", pair[1]);
                }).ToArray();
                result.G4XExclusions.Add((rule, predicate), operands);
            }
        }

        return result;
    }

    private static void SkipToSection(string[] lines, ref int i, string header)
    {
        while (i < lines.Length && lines[i].Trim() != header) i++;
        if (i >= lines.Length)
            throw new InvalidDataException(
                $"Required section '{header}' not found in .interp file.");
        i++; // skip the header line
    }

    private static bool TrySkipToSection(string[] lines, ref int i, string header)
    {
        int saved = i;
        while (i < lines.Length && lines[i].Trim() != header) i++;
        if (i >= lines.Length) { i = saved; return false; }
        i++; // skip header
        return true;
    }

    // Read lines until blank line. "null" → null entry, else the raw line value.
    private static string[] ReadSection(string[] lines, ref int i)
    {
        var result = new List<string>();
        while (i < lines.Length)
        {
            var line = lines[i];
            if (line.Length == 0 || line == "\r") break;
            result.Add(line == "null" ? null : line);
            i++;
        }
        return result.ToArray();
    }
}
