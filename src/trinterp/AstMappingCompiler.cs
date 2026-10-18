using System;
using System.Collections.Generic;
using System.Linq;
using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;

namespace trinterp;

/// <summary>Compiles an .ast parse tree into the runtime rewrite table.</summary>
public static class AstMappingCompiler
{
    public static AstMappingTable Compile(UnvParseTreeElement root)
    {
        var syntax = GrammarNode.FromDom(root);
        if (syntax.LocalName != "mapping")
            throw new InvalidOperationException("Expected an .ast mapping parse tree.");

        bool singleChild = false;
        bool errorOtherwise = false;
        var rules = new List<AstMappingRuleTable>();
        foreach (var statement in Children(syntax, "statement"))
        {
            var fallback = Child(statement, "defaultStatement");
            if (fallback != null)
            {
                if (HasTerminal(fallback, "single-child")) singleChild = true;
                else errorOtherwise = true;
                continue;
            }

            var rule = RequiredChild(statement, "mappingRule");
            var pattern = Child(rule, "patternList") is { } list
                ? Children(list, "patternPart").Select(CompilePart).ToArray()
                : Array.Empty<AstPatternPartTable>();
            rules.Add(new AstMappingRuleTable(
                RequiredChild(rule, "identifier").GetText(), pattern,
                CompileTemplate(RequiredChild(rule, "template"))));
        }

        return new AstMappingTable(RequiredChild(syntax, "identifier").GetText(),
            singleChild, errorOtherwise, rules.ToArray());
    }

    private static AstPatternPartTable CompilePart(GrammarNode part)
    {
        var names = Children(part, "identifier").Select(n => n.GetText()).ToArray();
        var quoted = part.Children.FirstOrDefault(n => n.LocalName == "QuotedString");
        bool captured = HasTerminal(part, ":");
        bool hasKind = HasTerminal(part, "[");
        string capture = captured ? names[0] : null;
        string literal = quoted == null ? null : Unquote(quoted.GetText());
        string nodeName = quoted == null ? names[captured ? 1 : 0] : null;
        string astKind = hasKind ? names[^1] : null;
        var quantifier = Child(part, "quantifier")?.GetText();
        var (min, max) = quantifier switch
        {
            "?" => (0, 1),
            "*" => (0, int.MaxValue),
            "+" => (1, int.MaxValue),
            _ => (1, 1)
        };
        return new AstPatternPartTable(capture, nodeName, literal,
            astKind, min, max);
    }

    private static AstTemplateTable CompileTemplate(GrammarNode template)
    {
        var names = Children(template, "identifier").Select(n => n.GetText()).ToArray();
        if (HasTerminal(template, "..."))
            return new AstTemplateTable("splice", names[0], null, []);
        if (!HasTerminal(template, "^"))
            return new AstTemplateTable("capture", names[0], null, []);
        var children = Children(template, "template")
            .Select(CompileTemplate).ToArray();
        return new AstTemplateTable("node", names[0],
            HasTerminal(template, "[") ? names[1] : null, children);
    }

    private static string Unquote(string text) =>
        text[1..^1].Replace("\\'", "'").Replace("\\\\", "\\");

    private static GrammarNode Child(GrammarNode parent, string name) =>
        parent.Children.FirstOrDefault(n => n.LocalName == name);

    private static GrammarNode RequiredChild(GrammarNode parent, string name) =>
        Child(parent, name) ?? throw new InvalidOperationException(
            $"Missing {name} in .ast parse tree.");

    private static IEnumerable<GrammarNode> Children(GrammarNode parent,
        string name) => parent.Children.Where(n => n.LocalName == name);

    private static bool HasTerminal(GrammarNode parent, string text) =>
        parent.Children.Any(n => n.IsTerminal() && n.GetText() == text);
}
