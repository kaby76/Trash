using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>
/// Lower basic RFC 5234 ABNF to the common grammar model. Disjoint lexer rules
/// consume one UTF-16 code unit each, so parser alternatives retain ABNF's
/// character-level matching rather than ANTLR's maximal-munch tokenization.
/// </summary>
public sealed class AbnfFrontend : IGrammarFrontend
{
    private static readonly string[] CoreNames =
    [
        "alpha", "bit", "char", "cr", "crlf", "ctl", "digit", "dquote",
        "hexdig", "htab", "lf", "lwsp", "octet", "sp", "vchar", "wsp"
    ];

    private readonly string _name;
    private readonly Dictionary<string, List<GrammarNode>> _definitions = new(StringComparer.Ordinal);
    private readonly List<string> _order = new();
    private readonly List<(int First, int Last, string Name)> _alphabet = new();
    private readonly HashSet<string> _names = new(StringComparer.Ordinal);

    public AbnfFrontend(string fileName) => _name = "Abnf_" + Regex.Replace(
        Path.GetFileNameWithoutExtension(fileName), @"[^\p{L}\p{Nd}_]", "_");

    private static GrammarNode T(string kind, string text, GrammarNode source = null)
    {
        var (line, column) = SourceOf(source);
        return new GrammarNode(kind) { Terminal = true, Text = text, Line = line, Column = column };
    }

    private static string Name(string original) => original.ToLowerInvariant().Replace('-', '_');
    private static GrammarNode Ref(string name, bool token = false) => new("element", new GrammarNode("atom",
        new GrammarNode(token ? "terminalDef" : "ruleref", T(token ? "TOKEN_REF" : "RULE_REF", name))));
    private static GrammarNode Sequence(params GrammarNode[] elements) => new("alternative", elements);
    private static GrammarNode Group(IEnumerable<GrammarNode> alternatives, string suffix = null) => new("element",
        new GrammarNode("ebnf", new GrammarNode("block", new GrammarNode("altList", alternatives.ToArray())),
            suffix == null ? null : new GrammarNode("blockSuffix", new GrammarNode("ebnfSuffix", T("SUFFIX", suffix)))));
    private static GrammarNode Rule(string name, IEnumerable<GrammarNode> alternatives) => new("ruleSpec",
        new GrammarNode("parserRuleSpec", T("RULE_REF", name), new GrammarNode("ruleBlock",
            new GrammarNode("ruleAltList", alternatives.Select(a => new GrammarNode("labeledAlt", a)).ToArray()))));

    public GrammarNode Lower(GrammarNode syntax)
    {
        _definitions.Clear(); _order.Clear(); _alphabet.Clear(); _names.Clear();
        if (syntax.LocalName != "rulelist") throw new InvalidOperationException("Expected an ABNF rulelist.");
        foreach (var rule in Children(syntax, "rule_"))
        {
            string original = GetText(Child(rule, "rulename"));
            string name = Name(original);
            string op = Children(Child(rule, "defined_as"))
                .Where(n => n.Terminal && n.GetText() is "=" or "=/")
                .Select(n => n.GetText()).Single();
            var body = Child(Child(rule, "elements"), "alternation");
            if (op == "=")
            {
                if (_definitions.ContainsKey(name))
                    throw new InvalidOperationException($"Duplicate ABNF rule '{original}'. Use =/ for incremental alternatives.");
                _definitions.Add(name, new List<GrammarNode> { body });
                _order.Add(name);
            }
            else
            {
                if (!_definitions.TryGetValue(name, out var parts))
                    throw new InvalidOperationException($"ABNF incremental alternative for undefined rule '{original}'.");
                parts.Add(body);
            }
        }
        if (_order.Count == 0) throw new InvalidOperationException("ABNF requires at least one rule.");

        // ASCII is split into individual characters. Beyond ASCII, only the
        // literal/range endpoints used by the grammar split the alphabet.
        var boundaries = new SortedSet<int> { 0xd800, 0xe000, 0x10000, 0x100 };
        for (int c = 0; c <= 128; ++c) boundaries.Add(c);
        foreach (var node in _definitions.Values.SelectMany(p => p).SelectMany(p => p.DescendantsAndSelf()))
        {
            if (node.LocalName == "char_val")
                foreach (char c in StringValue(node)) AddBoundary(boundaries, c, c);
            if (node.LocalName == "num_val")
                foreach (var (first, last) in NumberValue(node)) AddBoundary(boundaries, first, last);
            if (node.LocalName == "prose_val")
                throw new NotSupportedException("ABNF prose values are not executable grammar expressions.");
        }
        var points = boundaries.ToArray();
        for (int i = 0; i + 1 < points.Length; ++i)
            if (points[i] < 0xd800 || points[i] >= 0xe000)
                _alphabet.Add((points[i], points[i + 1] - 1, "ABNF_CHAR_" + i));

        foreach (var name in _order) _names.Add(name);
        foreach (var name in CoreNames) _names.Add(name);
        string start = "abnf_start";
        for (int suffix = 1; _names.Contains(start); ++suffix) start = "abnf_start" + suffix;
        _names.Add(start);

        var rules = new GrammarNode("rules");
        // The first definition is the entry rule; EOF belongs to this wrapper,
        // not to a user rule that might also be referenced recursively.
        rules.Children.Add(Rule(start, [Sequence(Ref(_order[0]), Ref("EOF", true))]));
        foreach (var name in _order)
            rules.Children.Add(Rule(name, _definitions[name].SelectMany(Alternatives)));
        foreach (var name in CoreNames.Where(name => !_definitions.ContainsKey(name)))
            rules.Children.Add(Rule(name, CoreAlternatives(name)));

        foreach (var (first, last, name) in _alphabet)
        {
            static string Escape(int c) => "\\u{" + c.ToString("X") + "}";
            string set = "[" + Escape(first) + (first == last ? "" : "-" + Escape(last)) + "]";
            rules.Children.Add(new GrammarNode("ruleSpec", new GrammarNode("lexerRuleSpec", T("TOKEN_REF", name),
                new GrammarNode("lexerRuleBlock", new GrammarNode("lexerAltList", new GrammarNode("lexerAlt",
                    new GrammarNode("lexerElements", new GrammarNode("lexerElement", new GrammarNode("lexerAtom", T("LEXER_CHAR_SET", set))))))))));
        }
        return new GrammarNode("grammarSpec", new GrammarNode("grammarDecl",
            new GrammarNode("grammarType", T("GRAMMAR", "grammar")), new GrammarNode("identifier", T("ID", _name))), rules);
    }

    private static void AddBoundary(SortedSet<int> boundaries, int first, int last)
    {
        Check(first); Check(last);
        if (first > last) throw new InvalidOperationException("Reversed ABNF numeric range.");
        boundaries.Add(first);
        boundaries.Add(last + 1);
    }

    private static void Check(int c)
    {
        if (c < 0 || c > 0xffff || c is >= 0xd800 and <= 0xdfff)
            throw new NotSupportedException("The basic ABNF front end supports non-surrogate BMP characters only.");
    }

    private static string StringValue(GrammarNode node)
    {
        string quoted = GetText(Child(node, "STRING"));
        if (quoted.Length < 2 || quoted[0] != '"' || quoted[^1] != '"')
            throw new InvalidOperationException("Invalid ABNF quoted string.");
        return quoted[1..^1];
    }

    private static IEnumerable<(int First, int Last)> NumberValue(GrammarNode node)
    {
        string text = GetText(Child(node, "NumberValue"));
        int radix = char.ToLowerInvariant(text[1]) switch
        {
            'b' => 2, 'd' => 10, 'x' => 16,
            _ => throw new InvalidOperationException($"Invalid ABNF numeric value '{text}'.")
        };
        string digits = text[2..];
        int Parse(string part)
        {
            try { return Convert.ToInt32(part, radix); }
            catch (Exception e) when (e is FormatException or OverflowException)
            { throw new InvalidOperationException($"Invalid ABNF numeric value '{text}'.", e); }
        }
        if (digits.Contains('-'))
        {
            var endpoints = digits.Split('-');
            if (endpoints.Length != 2) throw new InvalidOperationException($"Invalid ABNF numeric range '{text}'.");
            yield return (Parse(endpoints[0]), Parse(endpoints[1]));
        }
        else
            foreach (string part in digits.Split('.'))
            {
                int c = Parse(part);
                yield return (c, c);
            }
    }

    private IEnumerable<GrammarNode> Alternatives(GrammarNode alternation) =>
        Children(alternation, "concatenation").Select(Concatenation);

    private GrammarNode Concatenation(GrammarNode concatenation) => Sequence(
        Children(concatenation, "repetition").SelectMany(Repetition).ToArray());

    private IEnumerable<GrammarNode> Repetition(GrammarNode repetition)
    {
        var item = Child(repetition, "element");
        var repeat = Child(repetition, "repeat_");
        if (repeat == null) { yield return Element(item); yield break; }
        string text = GetText(repeat);
        int star = text.IndexOf('*');
        int min = star < 0 ? int.Parse(text) : star == 0 ? 0 : int.Parse(text[..star]);
        int? max = star < 0 ? min : star == text.Length - 1 ? null : int.Parse(text[(star + 1)..]);
        if (min > 256 || max > 256) throw new NotSupportedException("ABNF repetition bounds above 256 are not supported.");
        if (max < min) throw new InvalidOperationException($"Invalid ABNF repetition '{text}': maximum is below minimum.");
        for (int i = 0; i < min; ++i) yield return Element(item);
        if (max == null) yield return Group([Sequence(Element(item))], "*");
        else for (int i = min; i < max; ++i) yield return Group([Sequence(Element(item))], "?");
    }

    private GrammarNode Element(GrammarNode element)
    {
        if (Child(element, "rulename") is { } nameNode)
        {
            string name = Name(GetText(nameNode));
            if (!_names.Contains(name)) throw new InvalidOperationException($"Undefined ABNF rule '{GetText(nameNode)}'.");
            return Ref(name);
        }
        if (Child(element, "group") is { } group)
            return Group(Alternatives(Child(group, "alternation")));
        if (Child(element, "option") is { } option)
            return Group(Alternatives(Child(option, "alternation")), "?");
        if (Child(element, "char_val") is { } literal)
            return Group([Sequence(StringValue(literal).SelectMany(CaseInsensitiveChar).ToArray())]);
        if (Child(element, "num_val") is { } numeric)
        {
            var pieces = NumberValue(numeric).Select(range => Range(range.First, range.Last)).ToArray();
            return Group([Sequence(pieces)]);
        }
        throw new NotSupportedException("ABNF prose values are not executable grammar expressions.");
    }

    private IEnumerable<GrammarNode> CaseInsensitiveChar(char c)
    {
        Check(c);
        char other = c is >= 'a' and <= 'z' ? char.ToUpperInvariant(c)
            : c is >= 'A' and <= 'Z' ? char.ToLowerInvariant(c) : c;
        yield return other == c ? Char(c) : Group([Sequence(Char(c)), Sequence(Char(other))]);
    }

    private GrammarNode Char(int c)
    {
        var token = _alphabet.FirstOrDefault(p => p.First <= c && c <= p.Last);
        if (token.Name == null) throw new InvalidOperationException($"No ABNF lexer token for U+{c:X4}.");
        return Ref(token.Name, true);
    }

    private GrammarNode Range(int first, int last)
    {
        Check(first); Check(last);
        if (first > last) throw new InvalidOperationException("Reversed ABNF numeric range.");
        var alternatives = _alphabet.Where(p => p.First >= first && p.Last <= last)
            .Select(p => Sequence(Ref(p.Name, true))).ToArray();
        if (alternatives.Length == 0) throw new InvalidOperationException("Empty ABNF character range.");
        return alternatives.Length == 1 ? alternatives[0].Children[0] : Group(alternatives);
    }

    private IEnumerable<GrammarNode> CoreAlternatives(string name) => name switch
    {
        "alpha" => [Sequence(Range('A', 'Z')), Sequence(Range('a', 'z'))],
        "bit" => [Sequence(Range('0', '1'))],
        "char" => [Sequence(Range(1, 127))],
        "cr" => [Sequence(Char(13))],
        "crlf" => [Sequence(Ref("cr"), Ref("lf"))],
        "ctl" => [Sequence(Range(0, 31)), Sequence(Char(127))],
        "digit" => [Sequence(Range('0', '9'))],
        "dquote" => [Sequence(Char('"'))],
        "hexdig" => [Sequence(Ref("digit")), Sequence(Range('A', 'F')), Sequence(Range('a', 'f'))],
        "htab" => [Sequence(Char(9))],
        "lf" => [Sequence(Char(10))],
        "lwsp" => [Sequence(Group([Sequence(Ref("wsp")), Sequence(Ref("crlf"), Ref("wsp"))], "*"))],
        "octet" => [Sequence(Range(0, 255))],
        "sp" => [Sequence(Char(32))],
        "vchar" => [Sequence(Range(33, 126))],
        "wsp" => [Sequence(Ref("sp")), Sequence(Ref("htab"))],
        _ => throw new InvalidOperationException($"Unknown ABNF core rule '{name}'.")
    };
}
