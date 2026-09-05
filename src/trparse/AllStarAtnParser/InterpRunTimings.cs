namespace AllStarAtnParser;

using EarleyAtnParser;

/// <summary>Independently measurable phases of an ALL(*) .interp run.</summary>
public sealed class InterpRunTimings
{
    public TimeSpan InterpFileReading { get; internal set; }
    public TimeSpan InterpParsing { get; internal set; }
    public TimeSpan AtnDeserialization { get; internal set; }
    public TimeSpan Initialization { get; internal set; }
    public TimeSpan Tokenization { get; internal set; }
    public TimeSpan TokenReconciliation { get; internal set; }
    public TimeSpan Parsing { get; internal set; }
    public TimeSpan TreeBuilding { get; internal set; }
    public LexerAtnSimulator.LexerDfaStatistics? LexerDfa { get; internal set; }

    public string Format(string prefix = "")
    {
        var lines = new List<string>
        {
            $"{prefix}Interp file reading: {InterpFileReading.TotalSeconds:F6} s",
            $"{prefix}Interp parsing: {InterpParsing.TotalSeconds:F6} s",
            $"{prefix}ATN deserialization: {AtnDeserialization.TotalSeconds:F6} s",
            $"{prefix}Interp initialization: {Initialization.TotalSeconds:F6} s",
            $"{prefix}Tokenization: {Tokenization.TotalSeconds:F6} s",
            $"{prefix}Token reconciliation: {TokenReconciliation.TotalSeconds:F6} s",
            $"{prefix}ALL(*) parsing: {Parsing.TotalSeconds:F6} s",
            $"{prefix}Tree building: {TreeBuilding.TotalSeconds:F6} s"
        };
        if (LexerDfa is { } dfa)
        {
            lines.Add($"{prefix}Lexer DFA: {dfa.States} states, " +
                $"{dfa.LiveTransitions} live transitions, " +
                $"{dfa.DeadTransitions} dead transitions");
            lines.Add($"{prefix}Lexer DFA storage: {dfa.DenseAsciiRows} dense ASCII rows, " +
                $"{dfa.SparseEntries} sparse entries, " +
                $"max {dfa.MaximumConfigurations} configurations/state, " +
                $"~{dfa.EstimatedRetainedBytes:N0} retained bytes");
            lines.Add($"{prefix}Lexer DFA cache: {dfa.CacheHits:N0} hits, " +
                $"{dfa.CacheMisses:N0} misses");
            lines.Add($"{prefix}Lexer DFA fast path: {dfa.FastPathRuns:N0} runs, " +
                $"{dfa.FastPathCharacters:N0} characters");
        }
        return string.Join(Environment.NewLine, lines);
    }
}
