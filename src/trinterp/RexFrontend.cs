using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>Basic, tokenized REx EBNF lowered to the common compiler syntax.</summary>
public sealed class RexFrontend : IGrammarFrontend
{
    private readonly string _name;
    private readonly HashSet<string> _parserRules = new(StringComparer.Ordinal);
    private readonly HashSet<string> _tokens = new(StringComparer.Ordinal);
    private readonly HashSet<string> _eof = new(StringComparer.Ordinal);

    public RexFrontend(string fileName)
    {
        _name = Regex.Replace(Path.GetFileNameWithoutExtension(fileName), @"[^\p{L}\p{Nd}_]", "_");
        if (string.IsNullOrEmpty(_name)) _name = "RexGrammar";
    }

    private static GrammarNode T(string kind, string text, GrammarNode source = null)
    {
        var (line, column) = SourceOf(source);
        return new GrammarNode(kind) { Terminal = true, Text = text, Line = line, Column = column };
    }

    public GrammarNode Lower(GrammarNode syntax)
    {
        _parserRules.Clear();
        _tokens.Clear();
        _eof.Clear();
        if (syntax.LocalName != "grammar_") throw new InvalidOperationException("Expected a REx grammar tree.");
        foreach (var node in syntax.DescendantsAndSelf())
        {
            if (node.LocalName is "processingInstruction" or "option" or "preference" or "delimiter" or "equivalence" or "context" or "encore")
                throw new NotSupportedException($"REx {node.LocalName} is not supported by the basic trinterp front end.");
            if (node.Terminal && node.LocalName is "Slash" or "Amp" or "Minus")
                throw new NotSupportedException($"REx operator '{node.GetText()}' is not supported by the basic trinterp front end.");
        }
        var parsers = Children(Child(syntax, "syntaxDefinition"), "syntaxProduction").ToList();
        var lexers = Children(Child(syntax, "lexicalDefinition"), "lexicalProduction").ToList();
        var allNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var production in parsers.Concat(lexers))
        {
            var name = GetText(Child(production, "name"));
            if (name.Length == 0) throw new NotSupportedException("REx character-universe definitions are not supported.");
            if (!allNames.Add(name)) throw new InvalidOperationException($"Duplicate REx symbol '{name}'.");
            if (production.LocalName == "syntaxProduction") _parserRules.Add(name);
            else
            {
                if (production.Children.Any(n => n.Terminal && n.GetText() == "?"))
                    throw new NotSupportedException("REx nongreedy token declarations are not supported.");
                if (GetText(Child(production, "contextChoice")) == "$") _eof.Add(name);
            }
        }
        foreach (var reference in parsers.SelectMany(p => p.DescendantsAndSelf()).Where(n => n.LocalName == "nameOrString"))
        {
            var name = Child(reference, "name");
            if (name == null) continue;
            var text = GetText(name);
            if (!_parserRules.Contains(text) && !_eof.Contains(text))
            {
                if (!allNames.Contains(text)) throw new InvalidOperationException($"Undefined REx symbol '{text}'.");
                _tokens.Add(text);
            }
        }
        var rules = new GrammarNode("rules");
        foreach (var production in parsers.Concat(lexers))
        {
            var name = Child(production, "name");
            var text = GetText(name);
            if (_eof.Contains(text)) continue;
            bool lexer = production.LocalName == "lexicalProduction";
            var body = new GrammarNode(lexer ? "lexerRuleBlock" : "ruleBlock",
                Choice(Child(production, lexer ? "contextChoice" : "syntaxChoice"), lexer, outer: true));
            var rule = new GrammarNode(lexer ? "lexerRuleSpec" : "parserRuleSpec", T(lexer ? "TOKEN_REF" : "RULE_REF", text, name), body);
            if (lexer && !_tokens.Contains(text)) rule.Children.Insert(0, T("FRAGMENT", "fragment"));
            rules.Children.Add(new GrammarNode("ruleSpec", rule));
        }
        return new GrammarNode("grammarSpec",
            new GrammarNode("grammarDecl", new GrammarNode("grammarType", T("GRAMMAR", "grammar")),
                new GrammarNode("identifier", T("ID", _name))), rules);
    }

    private GrammarNode Choice(GrammarNode source, bool lexer, bool outer = false)
    {
        var result = new GrammarNode(lexer ? "lexerAltList" : outer ? "ruleAltList" : "altList");
        foreach (var child in source.Children.Where(n => !n.Terminal))
        {
            var sequence = child.LocalName == "contextExpression" ? Child(child, "lexicalSequence") : child;
            var elements = new GrammarNode(lexer ? "lexerElements" : "alternative");
            foreach (var item in sequence.Children.Where(n => n.LocalName is "syntaxItem" or "lexicalItem"))
                elements.Children.Add(Item(item, lexer));
            result.Children.Add(lexer ? new GrammarNode("lexerAlt", elements)
                : outer ? new GrammarNode("labeledAlt", elements) : elements);
        }
        return result;
    }

    private GrammarNode Item(GrammarNode item, bool lexer)
    {
        var primary = Child(item, lexer ? "lexicalPrimary" : "syntaxPrimary");
        var suffix = item.Children.FirstOrDefault(n => n.Terminal && n.GetText() is "?" or "*" or "+");
        var quantifier = suffix == null ? null : new GrammarNode("ebnfSuffix", T("SUFFIX", suffix.GetText(), suffix));
        var group = Child(primary, lexer ? "lexicalChoice" : "syntaxChoice");
        var element = new GrammarNode(lexer ? "lexerElement" : "element");
        if (group != null)
        {
            var block = new GrammarNode(lexer ? "lexerBlock" : "block", Choice(group, lexer));
            if (lexer) element.Children.Add(block);
            else element.Children.Add(new GrammarNode("ebnf", block, quantifier == null ? null : new GrammarNode("blockSuffix", quantifier)));
            if (!lexer) return element;
        }
        else
        {
            var atom = new GrammarNode(lexer ? "lexerAtom" : "atom");
            var symbol = lexer ? primary : Child(primary, "nameOrString");
            var name = Child(symbol, "name");
            var literal = symbol?.Children.FirstOrDefault(n => n.LocalName == "StringLiteral");
            if (name != null)
            {
                string text = GetText(name);
                if (_eof.Contains(text)) atom.Children.Add(new GrammarNode("terminalDef", T("TOKEN_REF", "EOF", name)));
                else if (lexer || _parserRules.Contains(text)) atom.Children.Add(new GrammarNode("ruleref", T("RULE_REF", text, name)));
                else atom.Children.Add(new GrammarNode("terminalDef", T("TOKEN_REF", text, name)));
            }
            else if (literal != null)
                atom.Children.Add(new GrammarNode("terminalDef", T("STRING_LITERAL", Quote(literal.GetText()[1..^1]), literal)));
            else if (Child(primary, "charCode") is { } code)
                atom.Children.Add(new GrammarNode("terminalDef", T("STRING_LITERAL", "'" + Code(code.GetText()) + "'", code)));
            else if (Child(primary, "charClass") is { } set)
            {
                if (set.GetText().StartsWith("[^")) throw new NotSupportedException("REx complemented classes require character-universe support.");
                string content = string.Concat(set.Children.Where(n => n.Terminal && n.LocalName.StartsWith("SetChar")).Select(n => n.LocalName switch
                {
                    "SetCharCode" => Code(n.GetText()),
                    "SetCharCodeRange" => string.Join("-", n.GetText().Split('-').Select(Code)),
                    "SetCharRange" => SetChar(n.GetText()[0]) + "-" + SetChar(n.GetText()[2]),
                    _ => SetChar(n.GetText()[0])
                }));
                atom.Children.Add(T("LEXER_CHAR_SET", "[" + content + "]", set));
            }
            else throw new NotSupportedException($"Unsupported REx lexical primary '{primary.GetText()}'.");
            element.Children.Add(atom);
        }
        if (quantifier != null) element.Children.Add(quantifier);
        return element;
    }

    private static string Quote(string value) => "'" + value.Replace("\\", "\\\\").Replace("'", "\\'") + "'";
    private static string SetChar(char c) => c switch
    {
        '#' => throw new NotSupportedException("Use \\u hexadecimal escapes in REx character classes; the built-in REx parser does not recognize #x class escapes."),
        '\\' or '[' or ']' or '^' or '-' => "\\" + c,
        _ => c.ToString()
    };
    private static string Code(string value)
    {
        var hex = value[2..];
        if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int code) || code > 0x10ffff || code < 0)
            throw new InvalidOperationException($"Invalid REx character code '{value}'.");
        return "\\u{" + hex + "}";
    }
}
