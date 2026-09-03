namespace AllStarAtnParser;

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

    public string Format(string prefix = "") => string.Join(Environment.NewLine,
        $"{prefix}Interp file reading: {InterpFileReading.TotalSeconds:F6} s",
        $"{prefix}Interp parsing: {InterpParsing.TotalSeconds:F6} s",
        $"{prefix}ATN deserialization: {AtnDeserialization.TotalSeconds:F6} s",
        $"{prefix}Interp initialization: {Initialization.TotalSeconds:F6} s",
        $"{prefix}Tokenization: {Tokenization.TotalSeconds:F6} s",
        $"{prefix}Token reconciliation: {TokenReconciliation.TotalSeconds:F6} s",
        $"{prefix}ALL(*) parsing: {Parsing.TotalSeconds:F6} s",
        $"{prefix}Tree building: {TreeBuilding.TotalSeconds:F6} s");
}
