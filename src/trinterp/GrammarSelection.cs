using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace trinterp;

/// <summary>
/// Selects the root grammar named by desc.xml or by the import graph and
/// expands ANTLR parser/combined imports before token types are assigned.
/// </summary>
internal static class GrammarSelection
{
    internal sealed record Description(string GrammarName, string StartRule);
    internal sealed record Source(GrammarNode Syntax, string FileName, string Name,
        IReadOnlyList<string> Imports);

    internal static Description ReadDescription()
    {
        var file = Path.Combine(Environment.CurrentDirectory, "desc.xml");
        if (!File.Exists(file)) return new Description(null, null);
        var root = XDocument.Load(file).Root;
        var test = root?.Elements("test").FirstOrDefault();
        return new Description(
            (string)(test?.Element("grammar-name") ?? root?.Element("grammar-name")),
            (string)(test?.Element("entry-point") ?? root?.Element("entry-point")));
    }

    internal static Source FromSyntax(GrammarNode syntax, string fileName)
    {
        if (syntax.LocalName != "grammarSpec") return null;
        var decl = GrammarParser.Child(syntax, "grammarDecl");
        var name = GrammarParser.GetText(GrammarParser.Child(decl, "identifier")).Trim();
        var imports = GrammarParser.Children(syntax, "prequelConstruct")
            .Select(pre => GrammarParser.Child(pre, "delegateGrammars"))
            .Where(node => node != null)
            .SelectMany(node => GrammarParser.Children(node, "delegateGrammar"))
            .Select(node => GrammarParser.Children(node, "identifier").LastOrDefault())
            .Select(GrammarParser.GetText)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToArray();
        return new Source(syntax, fileName, name, imports);
    }

    internal static IReadOnlyList<Source> SelectAndExpand(
        IReadOnlyList<Source> sources, Description description)
    {
        if (sources.Count == 0) return sources;
        var byName = sources.ToDictionary(source => source.Name, StringComparer.Ordinal);
        var imported = new HashSet<string>(
            sources.SelectMany(source => source.Imports), StringComparer.Ordinal);
        var roots = sources.Where(source => !imported.Contains(source.Name)).ToList();
        if (!string.IsNullOrEmpty(description.GrammarName))
        {
            roots = roots.Where(source =>
                source.Name == description.GrammarName ||
                source.Name == description.GrammarName + "Parser").ToList();
            if (roots.Count != 1)
                throw new InvalidOperationException(
                    $"[trinterp] desc.xml grammar-name '{description.GrammarName}' " +
                    $"does not identify one top-level grammar. Candidates: " +
                    string.Join(", ", sources.Select(source => source.Name)));
            roots.AddRange(sources.Where(source => IsLexer(source.Syntax) &&
                !roots.Contains(source)));
        }
        if (roots.Count == 0)
            throw new InvalidOperationException("[trinterp] No top-level grammar found; check for cyclic imports.");

        var expanded = new List<Source>();
        foreach (var root in roots)
        {
            if (IsLexer(root.Syntax))
            {
                expanded.Add(root);
                // Lexer imports are resolved by GrammarBinding, which also
                // handles imported modes and token type remapping.
                void IncludeLexerImports(Source current)
                {
                    foreach (var importName in current.Imports)
                    {
                        if (!byName.TryGetValue(importName, out var dependency))
                            throw new InvalidOperationException(
                                $"[trinterp] Imported grammar '{importName}' for '{current.Name}' was not supplied.");
                        if (expanded.All(item => item.Name != dependency.Name))
                        {
                            expanded.Add(dependency);
                            IncludeLexerImports(dependency);
                        }
                    }
                }
                IncludeLexerImports(root);
                continue;
            }
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            var merged = Clone(root.Syntax);
            var rules = GrammarParser.Child(merged, "rules");
            if (rules == null)
            {
                rules = new GrammarNode("rules");
                merged.Children.Add(rules);
            }
            var names = new HashSet<string>(
                GrammarParser.Children(rules, "ruleSpec").Select(RuleName),
                StringComparer.Ordinal);
            void Visit(Source current)
            {
                if (!visiting.Add(current.Name))
                    throw new InvalidOperationException(
                        $"[trinterp] Cyclic grammar import involving '{current.Name}'.");
                foreach (var importName in current.Imports)
                {
                    if (!byName.TryGetValue(importName, out var dependency))
                        throw new InvalidOperationException(
                            $"[trinterp] Imported grammar '{importName}' for '{current.Name}' " +
                            "was not supplied.");
                    foreach (var rule in GrammarParser.Children(
                        GrammarParser.Child(dependency.Syntax, "rules"), "ruleSpec"))
                        if (names.Add(RuleName(rule)))
                            rules.Children.Add(Clone(rule));
                    Visit(dependency);
                }
                visiting.Remove(current.Name);
            }
            Visit(root);
            merged.Children.RemoveAll(pre => pre.LocalName == "prequelConstruct" &&
                GrammarParser.Child(pre, "delegateGrammars") != null);
            expanded.Add(root with { Syntax = merged, Imports = Array.Empty<string>() });
        }
        return expanded;
    }

    private static bool IsLexer(GrammarNode syntax)
    {
        var decl = GrammarParser.Child(syntax, "grammarDecl");
        return GrammarParser.ChildTerminal(
            GrammarParser.Child(decl, "grammarType"), "LEXER") != null;
    }

    private static string RuleName(GrammarNode ruleSpec)
    {
        var rule = GrammarParser.Child(ruleSpec, "parserRuleSpec") ??
            GrammarParser.Child(ruleSpec, "lexerRuleSpec");
        var name = GrammarParser.ChildTerminal(rule, "RULE_REF") ??
            GrammarParser.ChildTerminal(rule, "TOKEN_REF");
        return GrammarParser.GetText(name).Trim();
    }

    private static GrammarNode Clone(GrammarNode source)
    {
        var copy = new GrammarNode(source.LocalName)
        {
            Text = source.Text, Terminal = source.Terminal,
            Line = source.Line, Column = source.Column
        };
        foreach (var child in source.Children) copy.Children.Add(Clone(child));
        return copy;
    }
}
