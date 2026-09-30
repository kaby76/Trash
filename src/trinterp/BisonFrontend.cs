using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>
/// Lower the context-free part of a Bison grammar to a parser grammar. A
/// separate ANTLR4/G4X lexer supplies the named and literal token types.
/// </summary>
public sealed class BisonFrontend : IGrammarFrontend
{
    private readonly string _name;
    private readonly Dictionary<string, string> _aliases = new(StringComparer.Ordinal);
    private readonly HashSet<string> _rules = new(StringComparer.Ordinal);

    public BisonFrontend(string fileName)
    {
        var name = Regex.Replace(Path.GetFileNameWithoutExtension(fileName), @"[^\p{L}\p{Nd}_]", "_");
        _name = "Bison_" + (name.Length == 0 ? "Grammar" : name);
    }

    private static GrammarNode T(string kind, string text, GrammarNode source = null)
    {
        var (line, column) = SourceOf(source);
        return new GrammarNode(kind) { Terminal = true, Text = text, Line = line, Column = column };
    }

    private static GrammarNode Ref(string name, bool token, GrammarNode source = null) =>
        new("element", new GrammarNode("atom", new GrammarNode(token ? "terminalDef" : "ruleref",
            T(token ? "TOKEN_REF" : "RULE_REF", name, source))));

    private static GrammarNode Literal(string value, GrammarNode source) =>
        new("element", new GrammarNode("atom", new GrammarNode("terminalDef", T("STRING_LITERAL", value, source))));

    private static GrammarNode Rule(string name, IEnumerable<GrammarNode> alternatives, GrammarNode source = null) =>
        new("ruleSpec", new GrammarNode("parserRuleSpec", T("RULE_REF", name, source),
            new GrammarNode("ruleBlock", new GrammarNode("ruleAltList",
                alternatives.Select(a => new GrammarNode("labeledAlt", a)).ToArray()))));

    public GrammarNode Lower(GrammarNode syntax)
    {
        _aliases.Clear();
        _rules.Clear();
        if (syntax.LocalName != "input_") throw new InvalidOperationException("Expected a Bison input_ tree.");
        foreach (var directive in syntax.DescendantsAndSelf().Where(n => n.Terminal &&
                     n.LocalName is "GLR_PARSER" or "NONDETERMINISTIC_PARSER" or "DEFAULT_PREC" or "NO_DEFAULT_PREC"))
            throw new NotSupportedException($"Bison directive '{directive.GetText()}' is not supported by the interpreter.");

        var productions = Children(Child(syntax, "bison_grammar"), "rules_or_grammar_declaration")
            .Select(n => Child(n, "rules")).Where(n => n != null).ToList();
        if (productions.Count == 0) throw new InvalidOperationException("Bison grammar has no rules.");
        foreach (var production in productions)
        {
            var id = Child(production, "id");
            var name = Identifier(id);
            if (!_rules.Add(name)) throw new InvalidOperationException($"Duplicate Bison rule '{name}'.");
        }

        string start = null;
        foreach (var declaration in syntax.DescendantsAndSelf().Where(n => n.LocalName == "grammar_declaration"))
        {
            var symbolDeclaration = Child(declaration, "symbol_declaration");
            if (symbolDeclaration != null)
            {
                if (Child(symbolDeclaration, "precedence_declarator") != null)
                    throw new NotSupportedException("Bison precedence declarations require conflict resolution that the interpreter does not implement.");
                if (Child(symbolDeclaration, "token_decls") is { } tokenDeclarations)
                    foreach (var token in Children(tokenDeclarations, "token_decl"))
                    {
                        var alias = Child(Child(token, "alias"), "string_as_id");
                        if (alias == null) continue;
                        string literal = Quoted(GetText(alias));
                        string name = Identifier(Child(token, "id"));
                        if (!_aliases.TryAdd(literal, name))
                            throw new InvalidOperationException($"Duplicate Bison token alias {literal}.");
                    }
            }
            if (declaration.Children.Any(n => n.Terminal && n.LocalName == "PERCENT_START"))
            {
                if (start != null) throw new InvalidOperationException("Multiple Bison %start declarations.");
                start = Identifier(Child(Child(declaration, "symbol"), "id"));
            }
        }
        start ??= Identifier(Child(productions[0], "id"));
        if (!_rules.Contains(start)) throw new InvalidOperationException($"Bison start rule '{start}' is undefined.");

        string wrapper = "bison_start";
        for (int i = 1; _rules.Contains(wrapper); ++i) wrapper = "bison_start" + i;
        var rules = new GrammarNode("rules");
        rules.Children.Add(Rule(wrapper, [new GrammarNode("alternative", Ref(start, false), Ref("EOF", true))]));
        foreach (var production in productions)
        {
            string name = Identifier(Child(production, "id"));
            var alternatives = Children(Child(production, "rhses_1"), "rhs").Select(RightHandSide).ToArray();
            rules.Children.Add(Rule(name, alternatives, Child(production, "id")));
        }
        return new GrammarNode("grammarSpec", new GrammarNode("grammarDecl",
            new GrammarNode("grammarType", T("PARSER", "parser")),
            new GrammarNode("identifier", T("ID", _name))), rules);
    }

    private GrammarNode RightHandSide(GrammarNode rhs)
    {
        var result = new GrammarNode("alternative");
        bool empty = false;
        foreach (var child in rhs.Children)
        {
            if (child.LocalName == "symbol") result.Children.Add(Symbol(child));
            else if (child.Terminal && child.LocalName == "EMPTY_RULE") empty = true;
            else if (child.Terminal && child.LocalName is "BRACED_PREDICATE" or "PERCENT_PREC" or "DPREC" or "MERGE" or "EXPECT" or "EXPECT_RR")
                throw new NotSupportedException($"Bison directive '{child.GetText()}' cannot be preserved by the interpreter.");
            // Actions, named references, tags, and whitespace do not change
            // the sequence of symbols recognized by the parser.
        }
        if (empty && result.Children.Count != 0)
            throw new InvalidOperationException("Bison %empty must be the entire alternative.");
        return result;
    }

    private GrammarNode Symbol(GrammarNode symbol)
    {
        if (Child(symbol, "id") is { } id)
        {
            if (id.Children.Any(n => n.Terminal && n.LocalName == "CHAR"))
                return Literal(Quoted(GetText(id)), id);
            string name = Identifier(id);
            if (name == "error") throw new NotSupportedException("Bison's error-recovery token is not supported.");
            return Ref(name, !_rules.Contains(name), id);
        }
        var stringNode = Child(symbol, "string_as_id");
        string quoted = Quoted(GetText(stringNode));
        return _aliases.TryGetValue(quoted, out var alias) ? Ref(alias, true, stringNode) : Literal(quoted, stringNode);
    }

    private static string Identifier(GrammarNode id)
    {
        if (id == null || !id.Children.Any(n => n.Terminal && n.LocalName == "ID"))
            throw new InvalidOperationException("Expected a named Bison symbol.");
        return GetText(id);
    }

    private static string Quoted(string value)
    {
        if (value.Length < 2) throw new InvalidOperationException("Invalid Bison quoted symbol.");
        if (value[0] == '\'' && value[^1] == '\'') return value;
        if (value[0] != '"' || value[^1] != '"') throw new InvalidOperationException($"Invalid Bison quoted symbol '{value}'.");
        var content = new StringBuilder();
        for (int i = 1; i < value.Length - 1; ++i)
        {
            if (value[i] == '\\' && i + 1 < value.Length - 1 && value[i + 1] == '"')
            {
                content.Append('"'); ++i;
            }
            else if (value[i] == '\'') content.Append("\\'");
            else content.Append(value[i]);
        }
        return "'" + content + "'";
    }
}
