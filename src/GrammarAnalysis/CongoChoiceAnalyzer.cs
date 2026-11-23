using System;
using System.Collections.Generic;
using System.Linq;

namespace trinterp;

/// <summary>
/// Computes conservative CongoCC lookahead facts from the normalized ANTLR4
/// parser syntax. A fact is emitted only when its FIRST/FOLLOW sets can be
/// represented as CongoCC token lookahead. Unknown constructs remain untouched.
/// </summary>
public sealed class CongoChoiceAnalyzer
{
    private readonly Dictionary<string, GrammarNode> _rules;

    private CongoChoiceAnalyzer(GrammarNode root)
    {
        _rules = Children(Child(root, "rules"), "ruleSpec")
            .Select(spec => Child(spec, "parserRuleSpec"))
            .Where(rule => rule != null)
            .ToDictionary(rule => Child(rule, "RULE_REF").GetText(), StringComparer.Ordinal);
    }

    /// <summary>
    /// Returns delimited facts: containing rule, repeated rule, and a
    /// CongoCC lookahead expression. The converter applies the optional prefix
    /// to the repeated rule when it renders the SCAN.
    /// </summary>
    public static IEnumerable<string> Analyze(GrammarNode root)
    {
        if (root.LocalName != "grammarSpec") yield break;
        var analyzer = new CongoChoiceAnalyzer(root);
        // A partial FIRST/FOLLOW set is unsafe as a committed CongoCC choice.
        // Leave grammars with predicates or unrecognized parser elements alone.
        if (!analyzer._rules.Values.All(IsSupportedRule)) yield break;
        foreach (var fact in analyzer.FindTailRepetitionGuards()) yield return fact;
        foreach (var fact in analyzer.FindNestedRuleBoundaryGuards()) yield return fact;
        foreach (var fact in analyzer.FindRepeatedPrefixGuards()) yield return fact;
        foreach (var fact in analyzer.FindAlternativeGuards()) yield return fact;
    }

    /// <summary>Reports overlapping top-level choices the pass cannot resolve.</summary>
    public static IEnumerable<string> Diagnostics(GrammarNode root)
    {
        if (root.LocalName != "grammarSpec") yield break;
        var analyzer = new CongoChoiceAnalyzer(root);
        if (!analyzer._rules.Values.All(IsSupportedRule))
        {
            yield return "CongoCC choice analysis skipped: parser rules contain unsupported elements or predicates.";
            yield break;
        }
        var guarded = analyzer.FindAlternativeGuards().ToHashSet(StringComparer.Ordinal);
        foreach (var (name, rule) in analyzer._rules)
        {
            var alternatives = Alternatives(rule).ToArray();
            // Direct left recursion is rewritten by the renderer, not predicted
            // as an ordinary CongoCC choice.
            if (alternatives.Any(alt => RuleReference(Children(alt, "element").FirstOrDefault()) == name))
                continue;
            var unresolved = new List<int>();
            for (var i = 0; i < alternatives.Length - 1; i++)
            {
                var first = analyzer.FirstElements(
                    Children(alternatives[i], "element").ToArray(), new HashSet<string>());
                if (!alternatives.Skip(i + 1).Any(other => first.Overlaps(
                    analyzer.FirstElements(Children(other, "element").ToArray(), new HashSet<string>()))))
                    continue;
                var key = $"{name}#A#{i + 1}#";
                var syntaxKey = $"{name}#S#{i + 1}#";
                if (!guarded.Any(fact => fact.StartsWith(key, StringComparison.Ordinal) ||
                    fact.StartsWith(syntaxKey, StringComparison.Ordinal)))
                    unresolved.Add(i + 1);
            }
            if (unresolved.Count != 0)
                yield return $"CongoCC rule '{name}' has {unresolved.Count} overlapping alternatives without a safe SCAN " +
                    $"(indices: {string.Join(", ", unresolved.Take(8))}{(unresolved.Count > 8 ? ", ..." : "")}).";
        }
    }

    private static bool IsSupportedRule(GrammarNode rule) =>
        Alternatives(rule).All(alt => Children(alt, "element").All(IsSupportedElement));

    private static bool IsSupportedElement(GrammarNode element)
    {
        if (RuleReference(element) != null) return true;
        var terminal = Child(Child(element, "atom"), "terminalDef");
        if (Child(terminal, "TOKEN_REF") != null ||
            Child(terminal, "STRING_LITERAL") != null) return true;
        var block = Child(Child(element, "ebnf"), "block");
        return block != null && Children(Child(block, "altList"), "alternative")
            .All(alt => Children(alt, "element").All(IsSupportedElement));
    }

    private IEnumerable<string> FindTailRepetitionGuards()
    {
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (ownerName, rule) in _rules)
        {
            if (!IsRepeatedElsewhere(ownerName)) continue;
            foreach (var (tail, suffix) in Occurrences(rule))
            {
                if (suffix.Length != 0) continue;
                if (Suffix(tail) is not ('*' or '+')) continue;
                var name = RuleReference(tail);
                if (name == null) continue;
                var repeatedFirst = FirstRule(name, new HashSet<string>());
                var ownerFirst = FirstRule(ownerName, new HashSet<string>());
                if (!repeatedFirst.Overlaps(ownerFirst)) continue;
                var following = Follow(ownerName, new HashSet<string>());
                if (following.Count == 0) continue;
                var fact = string.Join('#', ownerName, "E", name,
                    string.Join(" | ", following.OrderBy(value => value, StringComparer.Ordinal)));
                if (emitted.Add(fact)) yield return fact;
            }
        }
    }

    // A repeated outer rule may have a distinctive two-token header while a
    // nested rule repeats an item that can consume the header's first token.
    // An ordinary SCAN of that item commits too early. Guard the nested
    // repetition against the complete next-rule header instead.
    private IEnumerable<string> FindNestedRuleBoundaryGuards()
    {
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (outerName, outerRule) in _rules)
        {
            if (!IsRepeatedElsewhere(outerName)) continue;
            var alternatives = Alternatives(outerRule).ToArray();
            if (alternatives.Length != 1) continue;
            var elements = Children(alternatives[0], "element").ToArray();
            if (elements.Length < 3) continue;
            var first = Terminal(elements[0]);
            var second = Terminal(elements[1]);
            if (first == null || second == null) continue;

            // The second header token must not occur in another parser rule;
            // otherwise suppressing the nested item could suppress valid input.
            if (_rules.Values.SelectMany(AllElements)
                .Count(element => TerminalText(element) == second) != 1) continue;

            var reachable = new HashSet<string>(StringComparer.Ordinal);
            var pending = new Queue<string>(elements.Skip(2)
                .SelectMany(AllElementsIncludingSelf)
                .Select(RuleReference).Where(name => name != null));
            while (pending.Count != 0)
            {
                var name = pending.Dequeue();
                if (!reachable.Add(name) || !_rules.TryGetValue(name, out var rule)) continue;
                foreach (var childName in AllElements(rule).Select(RuleReference).Where(value => value != null))
                    pending.Enqueue(childName);
            }

            foreach (var ownerName in reachable)
            {
                if (!_rules.TryGetValue(ownerName, out var rule)) continue;
                foreach (var (tail, suffix) in Occurrences(rule))
                {
                    if (suffix.Length != 0 || Suffix(tail) is not ('*' or '+')) continue;
                    var itemName = RuleReference(tail);
                    if (itemName == null || !FirstRule(itemName, new HashSet<string>()).Contains(first)) continue;
                    var fact = string.Join('#', ownerName, "N", itemName, first + " " + second);
                    if (emitted.Add(fact)) yield return fact;
                }
            }
        }
    }

    private static IEnumerable<GrammarNode> AllElements(GrammarNode node) =>
        node.Children.SelectMany(AllElementsIncludingSelf);

    private static IEnumerable<GrammarNode> AllElementsIncludingSelf(GrammarNode node)
    {
        if (node.LocalName == "element") yield return node;
        foreach (var child in node.Children)
            foreach (var element in AllElementsIncludingSelf(child)) yield return element;
    }

    private static string TerminalText(GrammarNode element)
    {
        var terminal = Child(Child(element, "atom"), "terminalDef");
        var token = Child(terminal, "TOKEN_REF")?.GetText();
        if (token != null) return $"<{token}>";
        var literal = Child(terminal, "STRING_LITERAL")?.GetText();
        return literal == null ? null : QuoteLiteral(literal);
    }

    private IEnumerable<string> FindRepeatedPrefixGuards()
    {
        foreach (var (ownerName, rule) in _rules)
        {
            var alternatives = Alternatives(rule).ToArray();
            for (var altIndex = 0; altIndex < alternatives.Length; altIndex++)
            {
                var elements = Children(alternatives[altIndex], "element").ToArray();
                for (var elementIndex = 0; elementIndex + 2 < elements.Length; elementIndex++)
                {
                    var block = Child(Child(elements[elementIndex], "ebnf"), "block");
                    if (block == null || Suffix(Child(Child(elements[elementIndex], "ebnf"), "blockSuffix")) != '*')
                        continue;
                    var branches = Children(Child(block, "altList"), "alternative").ToArray();
                    if (branches.Length != 1) continue;
                    var repeated = Children(branches[0], "element").ToArray();
                    if (repeated.Length != 2) continue;
                    var ruleName = RuleReference(repeated[0]);
                    if (ruleName == null || ruleName != RuleReference(elements[elementIndex + 1])) continue;
                    var repeatedTail = Terminal(repeated[1]);
                    var followingTail = Terminal(elements[elementIndex + 2]);
                    if (repeatedTail == null || followingTail == null || repeatedTail == followingTail) continue;
                    yield return $"{ownerName}#B#{altIndex + 1}#{elementIndex + 1}#";
                }
            }
        }
    }

    private static string Terminal(GrammarNode element)
    {
        if (Suffix(element) != '\0') return null;
        var terminal = Child(Child(element, "atom"), "terminalDef");
        var token = Child(terminal, "TOKEN_REF")?.GetText();
        if (token != null) return $"<{token}>";
        var literal = Child(terminal, "STRING_LITERAL")?.GetText();
        return literal == null ? null : QuoteLiteral(literal);
    }

    private IEnumerable<string> FindAlternativeGuards()
    {
        foreach (var (ownerName, rule) in _rules)
        {
            var alternatives = Alternatives(rule).ToArray();
            if (alternatives.Length < 2) continue;
            for (var i = 0; i < alternatives.Length - 1; i++)
            {
                var elements = Children(alternatives[i], "element").ToArray();
                var first = FirstElements(elements, new HashSet<string>());
                if (!alternatives.Skip(i + 1).Any(other => first.Overlaps(
                    FirstElements(Children(other, "element").ToArray(), new HashSet<string>()))))
                    continue;
                // For bounded terminal patterns, a complete syntactic SCAN is
                // safe when no word of this alternative is a prefix of a word
                // in a later alternative (or vice versa). This handles, for
                // example, an optional sign followed by FLOAT versus INT.
                if (TryTerminalWords(elements, out var words) &&
                    alternatives.Skip(i + 1).All(other =>
                        TryTerminalWords(Children(other, "element").ToArray(), out var later) &&
                        words.All(word => later.All(candidate => !PrefixRelated(word, candidate)))))
                {
                    yield return $"{ownerName}#S#{i + 1}#";
                    continue;
                }
                if (!IsLinearTerminalSequence(elements)) continue;
                var following = Follow(ownerName, new HashSet<string>());
                if (following.Count == 0) continue;
                yield return string.Join('#', ownerName, "A", (i + 1).ToString(),
                    string.Join(" | ", following.OrderBy(value => value, StringComparer.Ordinal)));
            }
        }
    }

    private static bool IsLinearTerminalSequence(GrammarNode[] elements) =>
        elements.Length != 0 && elements.All(element =>
            Suffix(element) == '\0' &&
            (Child(Child(Child(element, "atom"), "terminalDef"), "TOKEN_REF") != null ||
             Child(Child(Child(element, "atom"), "terminalDef"), "STRING_LITERAL") != null));

    private static bool PrefixRelated(string[] left, string[] right) =>
        left.Take(Math.Min(left.Length, right.Length))
            .SequenceEqual(right.Take(Math.Min(left.Length, right.Length)), StringComparer.Ordinal);

    // Enumerate only small, finite terminal languages. A repetition or rule
    // reference leaves this proof unavailable; it must not acquire a guessed
    // numeric SCAN limit from an observed corpus.
    private static bool TryTerminalWords(GrammarNode[] elements, out List<string[]> words)
    {
        words = new List<string[]> { Array.Empty<string>() };
        foreach (var element in elements)
        {
            var terminal = Child(Child(element, "atom"), "terminalDef");
            var token = Child(terminal, "TOKEN_REF")?.GetText();
            var literal = Child(terminal, "STRING_LITERAL")?.GetText();
            List<string[]> choices;
            if (token != null || literal != null)
                choices = new List<string[]> { new[] { token != null ? $"<{token}>" : QuoteLiteral(literal) } };
            else if (Child(Child(element, "ebnf"), "block") is { } block)
            {
                choices = new List<string[]>();
                foreach (var alternative in Children(Child(block, "altList"), "alternative"))
                {
                    if (!TryTerminalWords(Children(alternative, "element").ToArray(), out var branch))
                        return false;
                    choices.AddRange(branch);
                }
            }
            else return false;

            var suffix = Suffix(element);
            if (suffix == '\0') suffix = Suffix(Child(Child(element, "ebnf"), "blockSuffix"));
            if (suffix is '*' or '+') return false;
            if (suffix == '?') choices.Add(Array.Empty<string>());
            if (choices.Count == 0 || words.Count * choices.Count > 64) return false;
            words = words.SelectMany(prefix => choices.Select(choice => prefix.Concat(choice).ToArray())).ToList();
            if (words.Any(word => word.Length > 8)) return false;
        }
        return true;
    }

    private bool IsRepeatedElsewhere(string name) => _rules.Values
        .SelectMany(Occurrences)
        .Any(item => RuleReference(item.element) == name && Suffix(item.element) is '*' or '+');

    private static IEnumerable<(GrammarNode element, GrammarNode[] suffix)> Occurrences(GrammarNode rule)
    {
        foreach (var alternative in Alternatives(rule))
            foreach (var item in Occurrences(alternative, Array.Empty<GrammarNode>()))
                yield return item;
    }

    private static IEnumerable<(GrammarNode element, GrammarNode[] suffix)> Occurrences(
        GrammarNode alternative, GrammarNode[] continuation)
    {
        var elements = Children(alternative, "element").ToArray();
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var suffix = elements.Skip(i + 1).Concat(continuation).ToArray();
            if (RuleReference(element) != null) yield return (element, suffix);
            var block = Child(Child(element, "ebnf"), "block");
            if (block == null) continue;
            foreach (var alt in Children(Child(block, "altList"), "alternative"))
                foreach (var item in Occurrences(alt, suffix)) yield return item;
        }
    }

    private HashSet<string> Follow(string name, HashSet<string> seen)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        if (!seen.Add(name)) return result;
        foreach (var (ownerName, rule) in _rules)
        foreach (var (element, suffix) in Occurrences(rule))
        {
            if (RuleReference(element) != name) continue;
            result.UnionWith(FirstElements(suffix, new HashSet<string>()));
            if (Suffix(element) is '*' or '+')
                result.UnionWith(FirstRule(name, new HashSet<string>()));
            if (NullableElements(suffix, new HashSet<string>()))
                result.UnionWith(Follow(ownerName, new HashSet<string>(seen)));
        }
        return result;
    }

    private HashSet<string> FirstRule(string name, HashSet<string> seen)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        if (!seen.Add(name) || !_rules.TryGetValue(name, out var rule)) return result;
        foreach (var alt in Alternatives(rule))
            result.UnionWith(FirstElements(Children(alt, "element").ToArray(), new HashSet<string>(seen)));
        return result;
    }

    private HashSet<string> FirstElements(GrammarNode[] elements, HashSet<string> seen)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in elements)
        {
            result.UnionWith(FirstElement(element, new HashSet<string>(seen)));
            if (!NullableElement(element, new HashSet<string>(seen))) break;
        }
        return result;
    }

    private HashSet<string> FirstElement(GrammarNode element, HashSet<string> seen)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var atom = Child(element, "atom");
        var terminal = Child(atom, "terminalDef");
        var token = Child(terminal, "TOKEN_REF")?.GetText();
        if (token != null) result.Add($"<{token}>");
        else if (Child(terminal, "STRING_LITERAL") is { } literal)
            result.Add(QuoteLiteral(literal.GetText()));
        else if (RuleReference(element) is { } rule)
            result.UnionWith(FirstRule(rule, seen));
        else if (Child(Child(element, "ebnf"), "block") is { } block)
            foreach (var alt in Children(Child(block, "altList"), "alternative"))
                result.UnionWith(FirstElements(Children(alt, "element").ToArray(), new HashSet<string>(seen)));
        return result;
    }

    private bool NullableRule(string name, HashSet<string> seen) =>
        seen.Add(name) && _rules.TryGetValue(name, out var rule) &&
        Alternatives(rule).Any(alt => NullableElements(
            Children(alt, "element").ToArray(), new HashSet<string>(seen)));

    private bool NullableElements(GrammarNode[] elements, HashSet<string> seen) =>
        elements.All(element => NullableElement(element, new HashSet<string>(seen)));

    private bool NullableElement(GrammarNode element, HashSet<string> seen)
    {
        if (Suffix(element) is '*' or '?') return true;
        var ebnf = Child(element, "ebnf");
        if (Suffix(Child(ebnf, "blockSuffix")) is '*' or '?') return true;
        if (RuleReference(element) is { } rule) return NullableRule(rule, seen);
        if (Child(ebnf, "block") is { } block)
            return Children(Child(block, "altList"), "alternative").Any(alt =>
                NullableElements(Children(alt, "element").ToArray(), new HashSet<string>(seen)));
        return Child(element, "actionBlock") != null;
    }

    private static string QuoteLiteral(string raw) => "\"" +
        raw[1..^1].Replace("\"", "\\\"") + "\"";

    private static string RuleReference(GrammarNode element) =>
        Child(Child(Child(element, "atom"), "ruleref"), "RULE_REF")?.GetText();

    private static char Suffix(GrammarNode element)
    {
        var suffix = Child(element, "ebnfSuffix");
        if (Child(suffix, "STAR") != null) return '*';
        if (Child(suffix, "PLUS") != null) return '+';
        if (Child(suffix, "QUESTION") != null) return '?';
        return '\0';
    }

    private static IEnumerable<GrammarNode> Alternatives(GrammarNode rule) =>
        Children(Child(Child(rule, "ruleBlock"), "ruleAltList"), "labeledAlt")
            .Select(alt => Child(alt, "alternative")).Where(alt => alt != null);

    private static GrammarNode Child(GrammarNode parent, string name) =>
        parent?.Children.FirstOrDefault(child => child.LocalName == name);

    private static IEnumerable<GrammarNode> Children(GrammarNode parent, string name) =>
        parent?.Children.Where(child => child.LocalName == name) ?? Enumerable.Empty<GrammarNode>();
}
