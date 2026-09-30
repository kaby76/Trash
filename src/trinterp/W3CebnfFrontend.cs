using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>Basic W3C EBNF lowered through a disjoint character-token alphabet.</summary>
public sealed class W3CebnfFrontend : IGrammarFrontend
{
    private readonly string _name;
    private readonly Dictionary<GrammarNode, bool[]> _sets = new();
    private readonly List<(int First, int Last, string Name)> _alphabet = new();
    private readonly HashSet<string> _names = new(StringComparer.Ordinal);

    public W3CebnfFrontend(string fileName) => _name = "W3Cebnf_" + Regex.Replace(
        Path.GetFileNameWithoutExtension(fileName), @"[^\p{L}\p{Nd}_]", "_");

    private static GrammarNode T(string kind, string text) => new(kind) { Terminal = true, Text = text };
    private static GrammarNode Ref(string name, bool token = false) => new("element", new GrammarNode("atom",
        new GrammarNode(token ? "terminalDef" : "ruleref", T(token ? "TOKEN_REF" : "RULE_REF", name))));
    private static GrammarNode Sequence(params GrammarNode[] elements) => new("alternative", elements);
    private static GrammarNode Group(IEnumerable<GrammarNode> alternatives, string suffix = null) => new("element",
        new GrammarNode("ebnf", new GrammarNode("block", new GrammarNode("altList", alternatives.ToArray())),
            suffix == null ? null : new GrammarNode("blockSuffix", new GrammarNode("ebnfSuffix", T("SUFFIX", suffix)))));
    private static GrammarNode Rule(string name, IEnumerable<GrammarNode> alternatives) => new("ruleSpec",
        new GrammarNode("parserRuleSpec", T("RULE_REF", name), new GrammarNode("ruleBlock",
            new GrammarNode("ruleAltList", alternatives.Select(a => new GrammarNode("labeledAlt", a)).ToArray()))));

    private string Unique(string prefix)
    {
        string name = prefix;
        for (int i = 1; !_names.Add(name); ++i) name = prefix + i;
        return name;
    }

    public GrammarNode Lower(GrammarNode syntax)
    {
        _sets.Clear(); _alphabet.Clear(); _names.Clear();
        if (syntax.LocalName != "grammar_") throw new InvalidOperationException("Expected a W3C EBNF grammar tree.");
        var productions = Children(syntax, "production").ToList();
        if (productions.Count == 0) throw new InvalidOperationException("W3C EBNF requires at least one production.");
        foreach (var production in productions)
        {
            string name = GetText(ChildTerminal(production, "SYMBOL"));
            if (!_names.Add(name)) throw new InvalidOperationException($"Duplicate W3C EBNF production '{name}'.");
        }
        foreach (var node in productions.SelectMany(p => p.DescendantsAndSelf()))
        {
            if (node.LocalName == "CONSTRAINT")
                throw new NotSupportedException("W3C EBNF validity constraints are not executable grammar expressions.");
            if (node.LocalName == "sequence_or_difference" &&
                node.Children.Any(n => n.Terminal && n.LocalName == "M"))
                throw new NotSupportedException("W3C EBNF production difference is not supported by the basic trinterp front end.");
            if (node.LocalName == "primary" && ChildTerminal(node, "SET") is { } set)
                _sets.Add(node, ParseSet(GetText(set)));
            if (node.LocalName == "primary" && ChildTerminal(node, "SYMBOL") is { } reference &&
                !_names.Contains(GetText(reference)))
                throw new InvalidOperationException($"Undefined W3C EBNF production '{GetText(reference)}'.");
        }

        // Every token consumes exactly one UTF-16 code unit. Split the alphabet
        // wherever a set changes membership or a literal uses an exact character.
        var boundaries = new SortedSet<int> { 0, 0xd800, 0xe000, 0x10000 };
        foreach (var set in _sets.Values)
            for (int c = 1; c < 65536; ++c)
                if (set[c] != set[c - 1]) boundaries.Add(c);
        foreach (var primary in productions.SelectMany(p => p.DescendantsAndSelf()).Where(n => n.LocalName == "primary"))
        {
            if (ChildTerminal(primary, "STRING") is { } literal)
                foreach (char c in StringValue(GetText(literal))) AddCharacter(boundaries, c);
            if (ChildTerminal(primary, "HEX") is { } hex)
                AddCharacter(boundaries, HexValue(GetText(hex)));
        }
        var points = boundaries.ToArray();
        for (int i = 0; i + 1 < points.Length; ++i)
            if (points[i] < 0xd800 || points[i] >= 0xe000)
                _alphabet.Add((points[i], points[i + 1] - 1, Unique("W3C_CHAR_" + i)));

        var rules = new GrammarNode("rules");
        string entry = Unique("w3c_start");
        rules.Children.Add(Rule(entry, [Sequence(Ref(GetText(ChildTerminal(productions[0], "SYMBOL"))), Ref("EOF", true))]));
        foreach (var production in productions)
            rules.Children.Add(Rule(GetText(ChildTerminal(production, "SYMBOL")), Choice(Child(production, "choice"))));
        foreach (var (first, last, name) in _alphabet)
        {
            static string Escape(int c) => "\\u{" + c.ToString("X") + "}";
            string charset = "[" + Escape(first) + (first == last ? "" : "-" + Escape(last)) + "]";
            rules.Children.Add(new GrammarNode("ruleSpec", new GrammarNode("lexerRuleSpec", T("TOKEN_REF", name),
                new GrammarNode("lexerRuleBlock", new GrammarNode("lexerAltList", new GrammarNode("lexerAlt",
                    new GrammarNode("lexerElements", new GrammarNode("lexerElement", new GrammarNode("lexerAtom", T("LEXER_CHAR_SET", charset))))))))));
        }
        return new GrammarNode("grammarSpec", new GrammarNode("grammarDecl",
            new GrammarNode("grammarType", T("GRAMMAR", "grammar")), new GrammarNode("identifier", T("ID", _name))), rules);
    }

    private IEnumerable<GrammarNode> Choice(GrammarNode choice) =>
        Children(choice, "sequence_or_difference").Select(sequence =>
            Sequence(Children(sequence, "item").Select(Item).ToArray()));

    private GrammarNode Item(GrammarNode item)
    {
        GrammarNode result = Primary(Child(item, "primary"));
        foreach (var suffix in item.Children.Where(n => n.Terminal && n.LocalName is "Q" or "S" or "P"))
            result = Group([Sequence(result)], GetText(suffix));
        return result;
    }

    private GrammarNode Primary(GrammarNode primary)
    {
        if (ChildTerminal(primary, "SYMBOL") is { } reference) return Ref(GetText(reference));
        if (Child(primary, "choice") is { } group) return Group(Choice(group));
        if (ChildTerminal(primary, "STRING") is { } literal)
            return Group([Sequence(StringValue(GetText(literal)).Select(c => Character(c)).ToArray())]);
        if (ChildTerminal(primary, "HEX") is { } hex) return Character(HexValue(GetText(hex)));
        if (_sets.TryGetValue(primary, out var set))
        {
            var alternatives = _alphabet.Where(p => set[p.First]).Select(p => Sequence(Ref(p.Name, true))).ToArray();
            if (alternatives.Length == 0) throw new NotSupportedException("An empty W3C EBNF character set cannot be compiled as an epsilon match.");
            return Group(alternatives);
        }
        throw new NotSupportedException($"Unsupported W3C EBNF primary '{GetText(primary)}'.");
    }

    private GrammarNode Character(int code)
    {
        var token = _alphabet.First(p => p.First <= code && code <= p.Last);
        return Ref(token.Name, true);
    }

    private static void AddCharacter(SortedSet<int> boundaries, int code)
    {
        Check(code);
        boundaries.Add(code);
        boundaries.Add(code + 1);
    }

    private static int Check(int code)
    {
        if (code < 0 || code > 0xffff || code is >= 0xd800 and <= 0xdfff)
            throw new NotSupportedException("The basic W3C EBNF front end supports non-surrogate BMP characters only.");
        return code;
    }

    private static int HexValue(string text) => Check(int.Parse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture));

    private static string StringValue(string text)
    {
        if (text.Length < 2 || text[0] != text[^1] || text[0] is not ('\'' or '"'))
            throw new InvalidOperationException($"Invalid W3C EBNF string {text}.");
        return text[1..^1];
    }

    private static bool[] ParseSet(string text)
    {
        var set = new bool[65536];
        string body = text[1..^1];
        bool complement = body.StartsWith('^');
        int offset = complement ? 1 : 0;
        while (offset < body.Length)
        {
            int first = SetCharacter(body, ref offset);
            int last = first;
            if (offset + 1 < body.Length && body[offset] == '-')
            {
                ++offset;
                last = SetCharacter(body, ref offset);
                if (last < first) throw new InvalidOperationException($"Reversed W3C EBNF character range in {text}.");
            }
            for (int code = first; code <= last; ++code)
                if (code is < 0xd800 or > 0xdfff) set[code] = true;
        }
        if (complement)
            for (int code = 0; code < set.Length; ++code)
                if (code is < 0xd800 or > 0xdfff) set[code] = !set[code];
        return set;
    }

    private static int SetCharacter(string body, ref int offset)
    {
        if (body.AsSpan(offset).StartsWith("#x", StringComparison.Ordinal))
        {
            int start = offset += 2;
            while (offset < body.Length && Uri.IsHexDigit(body[offset])) ++offset;
            if (start == offset) throw new InvalidOperationException("W3C EBNF #x must have hexadecimal digits.");
            return HexValue(body[(start - 2)..offset]);
        }
        return Check(body[offset++]);
    }
}
