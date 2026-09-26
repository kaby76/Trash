using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>Scannerless BMP iXML lowered through a disjoint character-token alphabet.</summary>
public sealed class IxmlFrontend : IGrammarFrontend
{
    private readonly string _name;
    private readonly Dictionary<GrammarNode, bool[]> _sets = new();
    private readonly List<(int First, int Last, string Name)> _alphabet = new();
    private readonly HashSet<string> _names = new(StringComparer.Ordinal);
    public IxmlFrontend(string fileName) => _name = "Ixml_" + Regex.Replace(
        Path.GetFileNameWithoutExtension(fileName), @"[^\p{L}\p{Nd}_]", "_");

    private static GrammarNode T(string kind, string text) => new(kind) { Terminal = true, Text = text };
    private static GrammarNode Ref(string name, bool token = false) => new("element", new GrammarNode("atom",
        new GrammarNode(token ? "terminalDef" : "ruleref", T(token ? "TOKEN_REF" : "RULE_REF", name))));
    private static GrammarNode Group(IEnumerable<GrammarNode> alternatives, string suffix = null) => new("element",
        new GrammarNode("ebnf", new GrammarNode("block", new GrammarNode("altList", alternatives.ToArray())),
            suffix == null ? null : new GrammarNode("blockSuffix", new GrammarNode("ebnfSuffix", T("SUFFIX", suffix)))));
    private static GrammarNode Sequence(params GrammarNode[] elements) => new("alternative", elements);
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
        var productions = Children(syntax, "rule_").ToList();
        if (productions.Count == 0) throw new InvalidOperationException("iXML requires at least one rule.");
        foreach (var p in productions)
        {
            var name = GetText(Child(p, "name"));
            if (!_names.Add(name)) throw new InvalidOperationException($"Duplicate iXML rule '{name}'.");
        }
        foreach (var n in syntax.DescendantsAndSelf())
        {
            if (n.LocalName is "mark" or "tmark" or "insertion" or "class_")
                throw new NotSupportedException($"iXML {n.LocalName} is not supported by the basic trinterp front end.");
            if (n.LocalName == "nonterminal" && !_names.Contains(GetText(Child(n, "name"))))
                throw new InvalidOperationException($"Undefined iXML rule '{GetText(Child(n, "name"))}'.");
            if (n.LocalName == "terminal_") _sets.Add(n, CharacterSet(n));
        }
        // Split at every membership change. Each token consumes exactly one character;
        // no rule ordering or maximal-munch decision can alter scannerless semantics.
        var boundaries = new SortedSet<int> { 0, 0xd800, 0xe000, 0x10000 };
        foreach (var set in _sets.Values)
            for (int c = 1; c < 65536; ++c)
                if (set[c] != set[c - 1]) boundaries.Add(c);
        // Literal strings need individually distinguishable characters, even adjacent ones.
        foreach (var n in _sets.Keys.Where(n => Child(n, "literal") != null))
            foreach (var c in Literal(Child(n, "literal"))) { boundaries.Add(c); boundaries.Add(c + 1); }
        var points = boundaries.ToArray();
        for (int i = 0; i + 1 < points.Length; ++i)
            if (points[i] < 0xd800 || points[i] >= 0xe000)
                _alphabet.Add((points[i], points[i + 1] - 1, Unique("IXML_CHAR_" + i)));
        var rules = new GrammarNode("rules");
        // Do not add EOF to the user's first rule: recursive references must not consume EOF.
        rules.Children.Add(Rule(Unique("ixml_start"), new[] {
            Sequence(Ref(GetText(Child(productions[0], "name"))), Ref("EOF", true)) }));
        foreach (var p in productions)
            rules.Children.Add(Rule(GetText(Child(p, "name")), Alternatives(Child(p, "alts"))));
        foreach (var (first, last, name) in _alphabet)
        {
            string Escape(int c) => "\\u{" + c.ToString("X") + "}";
            var charset = "[" + Escape(first) + (first == last ? "" : "-" + Escape(last)) + "]";
            rules.Children.Add(new GrammarNode("ruleSpec", new GrammarNode("lexerRuleSpec", T("TOKEN_REF", name),
                new GrammarNode("lexerRuleBlock", new GrammarNode("lexerAltList", new GrammarNode("lexerAlt",
                    new GrammarNode("lexerElements", new GrammarNode("lexerElement", new GrammarNode("lexerAtom", T("LEXER_CHAR_SET", charset))))))))));
        }
        return new GrammarNode("grammarSpec", new GrammarNode("grammarDecl",
            new GrammarNode("grammarType", T("GRAMMAR", "grammar")), new GrammarNode("identifier", T("ID", _name))), rules);
    }

    private IEnumerable<GrammarNode> Alternatives(GrammarNode alts) => Children(alts, "alt")
        .Select(a => Sequence(Children(a, "term_").Select(Term).ToArray()));
    private GrammarNode Term(GrammarNode term)
    {
        var item = term.Children.First(n => !n.Terminal);
        if (item.LocalName == "factor") return Factor(item);
        var factor = Child(item, "factor");
        var separator = Child(item, "sep");
        string suffix = item.LocalName switch { "option" => "?", "repeat0" => "*", "repeat1" => "+", _ => throw new NotSupportedException(item.LocalName) };
        if (separator == null) return Group(new[] { Sequence(Factor(factor)) }, suffix);
        var sequence = Sequence(Factor(factor), Group(new[] {
            Sequence(Factor(Child(separator, "factor")), Factor(factor)) }, "*"));
        return Group(new[] { sequence }, suffix == "*" ? "?" : null);
    }
    private GrammarNode Factor(GrammarNode factor)
    {
        if (Child(factor, "nonterminal") is { } reference) return Ref(GetText(Child(reference, "name")));
        if (Child(factor, "alts") is { } group) return Group(Alternatives(group));
        var terminal = Child(factor, "terminal_");
        if (Child(terminal, "literal") is { } literal)
            return Group(new[] { Sequence(Literal(literal).Select(c => Ref(_alphabet.First(p => p.First <= c && c <= p.Last).Name, true)).ToArray()) });
        var set = _sets[terminal];
        var alternatives = _alphabet.Where(p => set[p.First]).Select(p => Sequence(Ref(p.Name, true))).ToArray();
        // Empty character sets match nothing, not epsilon.
        if (alternatives.Length == 0) throw new NotSupportedException("Empty iXML character sets are not supported.");
        return Group(alternatives);
    }
    private static int Check(int c)
    {
        if (c < 0 || c > 0xffff || c is >= 0xd800 and <= 0xdfff)
            throw new NotSupportedException("The basic iXML front end supports non-surrogate BMP characters only.");
        return c;
    }
    private static int Hex(GrammarNode node) => Check(int.Parse(GetText(node), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
    private static int[] String(GrammarNode node)
    {
        var text = GetText(node);
        return text[1..^1].Replace(new string(text[0], 2), text[0].ToString()).Select(c => Check(c)).ToArray();
    }
    private static int[] Literal(GrammarNode literal) => Child(literal, "quoted") is { } quoted
        ? String(Child(quoted, "string_")) : new[] { Hex(Child(Child(literal, "encoded"), "hex")) };
    private static int Character(GrammarNode node)
    {
        if (Child(node, "hex") is { } hex) return Hex(hex);
        var value = String(node.Children.First(n => n.Terminal && n.LocalName.EndsWith("STRING")));
        if (value.Length != 1) throw new InvalidOperationException("iXML range endpoints must be single characters.");
        return value[0];
    }
    private static bool[] CharacterSet(GrammarNode terminal)
    {
        var result = new bool[65536];
        if (Child(terminal, "literal") is { } literal)
        {
            foreach (int c in Literal(literal)) result[c] = true;
            return result;
        }
        var charset = Child(terminal, "charset");
        var exclusion = Child(charset, "exclusion");
        foreach (var member in Children(Child(exclusion ?? Child(charset, "inclusion"), "set_"), "member"))
        {
            if (Child(member, "string_") is { } text) foreach (int c in String(text)) result[c] = true;
            else if (Child(member, "hex") is { } hex) result[Hex(hex)] = true;
            else if (Child(member, "range_") is { } range)
            {
                int first = Character(Child(Child(range, "from_"), "character"));
                int last = Character(Child(Child(range, "to_"), "character"));
                if (first > last) throw new InvalidOperationException("Reversed iXML character range.");
                for (int c = first; c <= last; ++c) result[c] = true;
            }
            else throw new NotSupportedException("iXML Unicode character categories are not supported.");
        }
        if (exclusion != null) for (int c = 0; c < result.Length; ++c) result[c] = !result[c];
        return result;
    }
}
