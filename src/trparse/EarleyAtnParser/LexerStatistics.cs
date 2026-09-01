namespace EarleyAtnParser;

/// <summary>A lexer rule which accepted a prefix at one committed input position.</summary>
public sealed record LexerCandidate(
    int RuleIndex,
    int TokenType,
    int EndPosition,
    int Channel,
    bool Skip,
    string Text);

/// <summary>Details of one committed lexical decision with overlapping rules.</summary>
public sealed record LexerOverlapEvent(
    int StartPosition,
    int Line,
    int Column,
    int Mode,
    IReadOnlyList<LexerCandidate> Candidates,
    LexerCandidate OrdinaryWinner,
    LexerCandidate SelectedWinner,
    IReadOnlyList<int> ExpectedTokenTypes,
    bool UsedContextFallback);

/// <summary>Runtime statistics for observed lexer-rule overlap.</summary>
public sealed class LexerStatistics
{
    public long TokenDecisions { get; internal set; }
    public long DecisionsWithOverlap { get; internal set; }
    public long MaximalMunchResolutions { get; internal set; }
    public long EqualLengthPriorityResolutions { get; internal set; }
    public long ContextOverrides { get; internal set; }
    public long ContextFallbacks { get; internal set; }
    public int MaximumCandidateCount { get; internal set; }

    public Dictionary<int, long> RuleOverlapCounts { get; } = new();
    public Dictionary<(int Left, int Right), long> RulePairOverlapCounts { get; } = new();
    public List<LexerOverlapEvent> Overlaps { get; } = new();

    internal void Record(
        int startPosition, int line, int column, int mode,
        IReadOnlyList<LexerCandidate> candidates,
        LexerCandidate ordinaryWinner, LexerCandidate selectedWinner,
        IReadOnlySet<int> expectedTokenTypes, bool usedContextFallback)
    {
        TokenDecisions++;
        if (expectedTokenTypes != null)
        {
            if (usedContextFallback) ContextFallbacks++;
            else if (ordinaryWinner.RuleIndex != selectedWinner.RuleIndex ||
                     ordinaryWinner.EndPosition != selectedWinner.EndPosition)
                ContextOverrides++;
        }

        if (candidates.Count < 2) return;

        DecisionsWithOverlap++;
        MaximumCandidateCount = Math.Max(MaximumCandidateCount, candidates.Count);
        int ordinaryLength = ordinaryWinner.EndPosition - startPosition;
        if (candidates.Any(candidate =>
                candidate.EndPosition - startPosition < ordinaryLength))
            MaximalMunchResolutions++;
        if (candidates.Count(candidate =>
                candidate.EndPosition == ordinaryWinner.EndPosition) > 1)
            EqualLengthPriorityResolutions++;

        var ordered = candidates.OrderBy(candidate => candidate.RuleIndex).ToArray();
        foreach (var candidate in ordered)
            RuleOverlapCounts[candidate.RuleIndex] =
                RuleOverlapCounts.GetValueOrDefault(candidate.RuleIndex) + 1;
        for (int i = 0; i < ordered.Length; i++)
        for (int j = i + 1; j < ordered.Length; j++)
        {
            var pair = (ordered[i].RuleIndex, ordered[j].RuleIndex);
            RulePairOverlapCounts[pair] = RulePairOverlapCounts.GetValueOrDefault(pair) + 1;
        }

        Overlaps.Add(new LexerOverlapEvent(
            startPosition, line, column, mode, ordered,
            ordinaryWinner, selectedWinner,
            expectedTokenTypes?.OrderBy(type => type).ToArray() ?? Array.Empty<int>(),
            usedContextFallback));
    }

    public string FormatSummary(IReadOnlyList<string> ruleNames)
    {
        var writer = new StringWriter();
        writer.WriteLine("Lexer statistics:");
        writer.WriteLine($"  token decisions: {TokenDecisions}");
        writer.WriteLine($"  decisions with overlapping rules: {DecisionsWithOverlap}");
        writer.WriteLine($"  maximal-munch resolutions: {MaximalMunchResolutions}");
        writer.WriteLine($"  equal-length priority resolutions: {EqualLengthPriorityResolutions}");
        writer.WriteLine($"  context overrides: {ContextOverrides}");
        writer.WriteLine($"  context fallbacks: {ContextFallbacks}");
        writer.WriteLine($"  maximum candidate rules: {MaximumCandidateCount}");
        writer.WriteLine("Overlapping rule pairs:");
        if (RulePairOverlapCounts.Count == 0)
            writer.WriteLine("  (none)");
        else
            foreach (var entry in RulePairOverlapCounts.OrderBy(entry => entry.Key.Left)
                         .ThenBy(entry => entry.Key.Right))
                writer.WriteLine(
                    $"  {RuleName(ruleNames, entry.Key.Left)} / " +
                    $"{RuleName(ruleNames, entry.Key.Right)}: {entry.Value}");
        return writer.ToString();
    }

    public string FormatOverlaps(
        string fileName, IReadOnlyList<string> ruleNames,
        IReadOnlyList<string> symbolicNames)
    {
        var writer = new StringWriter();
        foreach (var overlap in Overlaps)
        {
            writer.WriteLine(
                $"Lexer overlap at {fileName}:{overlap.Line}:{overlap.Column} " +
                $"(offset {overlap.StartPosition}, mode {overlap.Mode})");
            foreach (var candidate in overlap.Candidates)
                writer.WriteLine(
                    $"  {RuleName(ruleNames, candidate.RuleIndex)} " +
                    $"type={TokenName(symbolicNames, candidate.TokenType)} " +
                    $"length={candidate.Text.Length} text='{Escape(candidate.Text)}'");
            writer.WriteLine(
                $"  ordinary winner: {RuleName(ruleNames, overlap.OrdinaryWinner.RuleIndex)}");
            if (overlap.ExpectedTokenTypes.Count != 0)
                writer.WriteLine(
                    "  expected types: {" +
                    string.Join(", ", overlap.ExpectedTokenTypes.Select(
                        type => TokenName(symbolicNames, type))) + "}");
            writer.WriteLine(
                $"  selected winner: {RuleName(ruleNames, overlap.SelectedWinner.RuleIndex)}");
            if (overlap.UsedContextFallback)
                writer.WriteLine("  context fallback: yes");
        }
        return writer.ToString();
    }

    private static string RuleName(IReadOnlyList<string> names, int index) =>
        index >= 0 && index < names.Count && names[index] != null
            ? names[index]
            : index.ToString();

    private static string TokenName(IReadOnlyList<string> names, int type) =>
        type >= 0 && type < names.Count && names[type] != null
            ? names[type]
            : type.ToString();

    private static string Escape(string text) => text
        .Replace("\\", "\\\\")
        .Replace("'", "\\'")
        .Replace("\r", "\\r")
        .Replace("\n", "\\n")
        .Replace("\t", "\\t");
}
