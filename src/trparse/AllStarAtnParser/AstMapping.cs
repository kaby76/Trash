#nullable enable
namespace AllStarAtnParser;

public sealed record PatternPart(string? Capture, string? NodeName, string? Literal);
public abstract record Template;
public sealed record CaptureTemplate(string Name) : Template;
public sealed record NodeTemplate(string Kind, string? TextCapture, Template[] Children) : Template;
public sealed record MappingRule(string RuleName, PatternPart[] Pattern, Template Result);
public sealed record Mapping(string Name, bool SingleChild, bool ErrorOtherwise, MappingRule[] Rules);

public sealed record Lexeme(string Text, int Offset);

public sealed class MappingParser
{
    private readonly List<Lexeme> _tokens;
    private int _position;

    private MappingParser(string input) => _tokens = Tokenize(input);
    public static Mapping Parse(string input) => new MappingParser(input).ReadMapping();

    private Mapping ReadMapping()
    {
        Expect("ast");
        var name = Identifier();
        Expect("{");
        var single = false;
        var error = false;
        var rules = new List<MappingRule>();
        while (!Take("}"))
        {
            if (Take("default"))
            {
                if (Take("single-child"))
                {
                    Expect("->"); Expect("child"); Expect(";");
                    single = true;
                }
                else
                {
                    Expect("otherwise"); Expect("->"); Expect("error"); Expect(";");
                    error = true;
                }
                continue;
            }
            var ruleName = Identifier();
            Expect("(");
            var parts = new List<PatternPart>();
            if (!Take(")"))
            {
                do { parts.Add(ReadPatternPart()); } while (Take(","));
                Expect(")");
            }
            Expect("->");
            var replacement = ReadTemplate();
            Expect(";");
            rules.Add(new MappingRule(ruleName, parts.ToArray(), replacement));
        }
        if (_position != _tokens.Count)
            throw Error("Unexpected text after mapping.");
        return new Mapping(name, single, error, rules.ToArray());
    }

    private PatternPart ReadPatternPart()
    {
        if (PeekQuoted())
            return new PatternPart(null, null, Unquote(Advance().Text));
        var first = Identifier();
        if (!Take(":")) return new PatternPart(null, first, null);
        if (PeekQuoted()) return new PatternPart(first, null, Unquote(Advance().Text));
        return new PatternPart(first, Identifier(), null);
    }

    private Template ReadTemplate()
    {
        if (!Take("^"))
            return new CaptureTemplate(Identifier());
        Expect("(");
        var kind = Identifier();
        string? textCapture = null;
        if (Take("["))
        {
            textCapture = Identifier();
            Expect("]");
        }
        var children = new List<Template>();
        while (!Take(")"))
            children.Add(ReadTemplate());
        return new NodeTemplate(kind, textCapture, children.ToArray());
    }

    private bool PeekQuoted() => _position < _tokens.Count && _tokens[_position].Text.StartsWith('\'');
    private static string Unquote(string token) => token[1..^1].Replace("\\'", "'").Replace("\\\\", "\\");
    private string Identifier()
    {
        var token = Advance().Text;
        if (token.Length == 0 || !char.IsLetter(token[0]) && token[0] != '_')
            throw Error($"Expected identifier, got '{token}'.");
        return token;
    }
    private void Expect(string value)
    {
        if (!Take(value)) throw Error($"Expected '{value}'.");
    }
    private bool Take(string value)
    {
        if (_position >= _tokens.Count || _tokens[_position].Text != value) return false;
        _position++;
        return true;
    }
    private Lexeme Advance() => _position < _tokens.Count
        ? _tokens[_position++] : throw Error("Unexpected end of mapping.");
    private FormatException Error(string message) => new($"{message} At offset {(_position < _tokens.Count ? _tokens[_position].Offset : -1)}.");

    private static List<Lexeme> Tokenize(string text)
    {
        var tokens = new List<Lexeme>();
        for (var i = 0; i < text.Length;)
        {
            if (char.IsWhiteSpace(text[i])) { i++; continue; }
            if (i + 1 < text.Length && text[i..(i + 2)] == "//")
            {
                while (i < text.Length && text[i] != '\n') i++;
                continue;
            }
            var start = i;
            if (i + 1 < text.Length && text[i..(i + 2)] == "->") i += 2;
            else if (text[i] == '\'')
            {
                i++;
                while (i < text.Length && text[i] != '\'')
                {
                    if (text[i] == '\\') i++;
                    if (i >= text.Length) break;
                    i++;
                }
                if (i >= text.Length) throw new FormatException($"Unterminated string at offset {start}.");
                i++;
            }
            else if (char.IsLetter(text[i]) || text[i] == '_')
            {
                do { i++; } while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] is '_' or '-'));
            }
            else if ("{}(),;:^[]".Contains(text[i])) i++;
            else throw new FormatException($"Unexpected character '{text[i]}' at offset {i}.");
            tokens.Add(new Lexeme(text[start..i], start));
        }
        return tokens;
    }
}
