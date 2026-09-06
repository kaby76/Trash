namespace AllStarAtnParser;

using Antlr4.Runtime;
using Atn;
using EarleyAtnParser;

/// <summary>
/// Command-scoped immutable runtime data loaded from one parser/lexer interp
/// pair. Multiple input files reuse parsing, ATN deserialization, vocabularies,
/// and start-rule resolution.
/// </summary>
public sealed class InterpRuntimeCache
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<(string Parser, string Lexer), RuntimeData>
        _entries = new();

    public int Count
    {
        get { lock (_syncRoot) return _entries.Count; }
    }

    internal bool TryGet(string parserPath, string lexerPath,
        out RuntimeData runtime)
    {
        var key = Key(parserPath, lexerPath);
        lock (_syncRoot) return _entries.TryGetValue(key, out runtime);
    }

    internal RuntimeData Add(string parserPath, string lexerPath,
        RuntimeData runtime)
    {
        var key = Key(parserPath, lexerPath);
        lock (_syncRoot)
        {
            if (_entries.TryGetValue(key, out var existing)) return existing;
            _entries.Add(key, runtime);
            return runtime;
        }
    }

    public void Clear()
    {
        lock (_syncRoot) _entries.Clear();
    }

    private static (string, string) Key(string parserPath, string lexerPath) =>
        (Path.GetFullPath(parserPath), Path.GetFullPath(lexerPath));

    internal sealed record RuntimeData(
        ParsedInterp ParserInterp,
        ParsedInterp LexerInterp,
        MyATN ParserAtn,
        MyATN LexerAtn,
        Vocabulary ParserVocabulary,
        Vocabulary LexerVocabulary,
        int StartRule);
}
