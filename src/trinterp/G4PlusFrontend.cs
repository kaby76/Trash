using System;
using System.Collections.Generic;
using System.Linq;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>Lower G4Plus syntax into the compiler's common rule representation.</summary>
public sealed class G4PlusFrontend : IGrammarFrontend
{
    public GrammarNode Lower(GrammarNode root)
    {
        if (root.LocalName != "grammarSpec")
            throw new InvalidOperationException("Expected a G4Plus grammarSpec.");
        if (root.DescendantsAndSelf().Any(n => n.LocalName == "delegateGrammars"))
            throw new NotSupportedException("G4Plus grammar imports are not supported yet; supply a tokenVocab and its lexer grammar for token references.");
        bool lexer = Child(Child(root, "grammarDecl"), "grammarType")
            .Children.Any(n => n.LocalName == "LEXER");
        var rules = Children(Child(root, "rules"), "ruleSpec")
            .Concat(Children(root, "modeSpec").SelectMany(m => Children(m, "ruleSpec"))).ToList();
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rule in rules)
        {
            var name = GetText(Child(rule, "identifier"));
            if (!names.Add(name)) throw new InvalidOperationException($"Duplicate rule '{name}'.");
        }
        foreach (var rule in rules)
        {
            var name = Child(rule, "identifier");
            var body = Child(rule, "ruleBlock");
            var lowered = new GrammarNode(lexer ? "lexerRuleSpec" : "parserRuleSpec");
            lowered.Children.Add(Token(name, lexer ? "TOKEN_REF" : "RULE_REF"));
            var modifiers = Child(rule, "ruleModifiers");
            if (lexer && modifiers != null)
                lowered.Children.AddRange(modifiers.DescendantsAndSelf().Where(n => n.LocalName == "FRAGMENT"));
            else if (modifiers != null) lowered.Children.Add(modifiers);
            foreach (var child in rule.Children)
            {
                if (child == name || child == body || child == modifiers) continue;
                if (lexer && child.LocalName == "rulePrequel" && Child(child, "optionsSpec") is { } options)
                    lowered.Children.Add(options);
                else lowered.Children.Add(child);
            }
            lowered.Children.Add(LowerBody(body, lexer, names));
            rule.Children.Clear();
            rule.Children.Add(lowered);
        }
        // The existing model collector expects lexerRuleSpec directly in modes.
        foreach (var mode in Children(root, "modeSpec"))
        {
            if (!lexer) throw new InvalidOperationException("Modes require a lexer grammar.");
            for (int i = 0; i < mode.Children.Count; i++)
                if (mode.Children[i].LocalName == "ruleSpec") mode.Children[i] = mode.Children[i].Children.Single();
        }
        return root;
    }

    private static GrammarNode Token(GrammarNode source, string kind)
    {
        var (line, col) = SourceOf(source);
        return new GrammarNode(kind) { Terminal = true, Text = GetText(source), Line = line, Column = col };
    }

    private static GrammarNode LowerBody(GrammarNode body, bool lexer, HashSet<string> rules) =>
        new(lexer ? "lexerRuleBlock" : "ruleBlock", LowerAlts(Child(body, "ruleAltList"), lexer, rules, true));

    private static GrammarNode LowerAlts(GrammarNode list, bool lexer, HashSet<string> rules, bool outer)
    {
        var result = new GrammarNode(lexer ? "lexerAltList" : outer ? "ruleAltList" : "altList");
        foreach (var item in list.Children)
        {
            if (IsTerminal(item)) { result.Children.Add(item); continue; }
            var alt = item.LocalName == "labeledAlt" ? Child(item, "alternative") : item;
            if (Child(alt, "exclusion") != null)
                throw new NotSupportedException("G4Plus set-difference compilation is not supported yet; exclusion cannot be ignored.");
            if (!lexer && Child(alt, "lexerCommands") != null)
                throw new InvalidOperationException("Lexer commands require a lexer grammar.");
            var lowered = new GrammarNode(lexer ? "lexerAlt" : "alternative");
            var elements = lexer ? new GrammarNode("lexerElements") : lowered;
            foreach (var element in Children(alt, "element")) elements.Children.Add(LowerElement(element, lexer, rules));
            if (lexer)
            {
                lowered.Children.Add(elements);
                if (Child(alt, "lexerCommands") is { } commands) lowered.Children.Add(commands);
            }
            else
            {
                if (Child(alt, "elementOptions") is { } options) lowered.Children.Insert(0, options);
            }
            if (!lexer && item.LocalName == "labeledAlt")
            {
                var labeled = new GrammarNode("labeledAlt", lowered);
                labeled.Children.AddRange(item.Children.Where(c => c != alt));
                result.Children.Add(labeled);
            }
            else result.Children.Add(lowered);
        }
        return result;
    }

    private static GrammarNode LowerElement(GrammarNode element, bool lexer, HashSet<string> rules)
    {
        var result = new GrammarNode(lexer ? "lexerElement" : "element");
        foreach (var child in element.Children)
        {
            switch (child.LocalName)
            {
                case "atom": result.Children.Add(LowerAtom(child, lexer, rules)); break;
                case "labeledElement":
                    var label = new GrammarNode(lexer ? "labeledLexerElement" : "labeledElement");
                    foreach (var part in child.Children)
                        label.Children.Add(part.LocalName == "atom" ? LowerAtom(part, lexer, rules)
                            : part.LocalName == "block" ? LowerBlock(part, lexer, rules) : part);
                    result.Children.Add(label);
                    break;
                case "ebnf":
                    var block = LowerBlock(Child(child, "block"), lexer, rules);
                    if (lexer)
                    {
                        result.Children.Add(block);
                        var suffix = Child(Child(child, "blockSuffix"), "ebnfSuffix");
                        if (suffix != null) result.Children.Add(suffix);
                    }
                    else result.Children.Add(new GrammarNode("ebnf", block, Child(child, "blockSuffix")));
                    break;
                default: result.Children.Add(child); break;
            }
        }
        return result;
    }

    private static GrammarNode LowerBlock(GrammarNode block, bool lexer, HashSet<string> rules)
    {
        var result = new GrammarNode(lexer ? "lexerBlock" : "block");
        foreach (var child in block.Children)
            result.Children.Add(child.LocalName == "altList" ? LowerAlts(child, lexer, rules, false) : child);
        return result;
    }

    private static GrammarNode LowerAtom(GrammarNode atom, bool lexer, HashSet<string> rules)
    {
        var result = new GrammarNode(lexer ? "lexerAtom" : "atom");
        foreach (var child in atom.Children)
        {
            if (child.LocalName == "symbolRef")
            {
                var id = Child(child, "identifier");
                var name = GetText(id);
                bool rule = id != null && name != "EOF" && (lexer || rules.Contains(name));
                var reference = new GrammarNode(rule ? "ruleref" : "terminalDef");
                foreach (var part in child.Children)
                    reference.Children.Add(part == id ? Token(id, rule ? "RULE_REF" : "TOKEN_REF") : part);
                result.Children.Add(reference);
            }
            else if (child.LocalName == "notSet")
            {
                foreach (var se in child.DescendantsAndSelf().Where(n => n.LocalName == "setElement"))
                {
                    var id = Child(se, "identifier");
                    if (id != null)
                    {
                        if (lexer)
                            throw new NotSupportedException("Named rule references in lexer character sets require set expansion, which is not supported yet.");
                        if (!lexer && rules.Contains(GetText(id)))
                            throw new NotSupportedException("Parser rule references are not token-set members.");
                        se.Children[se.Children.IndexOf(id)] = Token(id, "TOKEN_REF");
                    }
                    var argument = Child(se, "argActionBlock");
                    if (argument != null)
                        se.Children[se.Children.IndexOf(argument)] = Token(argument, "LEXER_CHAR_SET");
                    if (!lexer && se.Children.Any(c => c.LocalName is "LEXER_CHAR_SET" or "characterRange"))
                        throw new NotSupportedException("Character sets and ranges in parser rules require scannerless compilation, which is not supported yet.");
                }
                result.Children.Add(child);
            }
            else
            {
                if (!lexer && child.LocalName is "LEXER_CHAR_SET" or "argActionBlock" or "characterRange")
                    throw new NotSupportedException("Character sets and ranges in parser rules require scannerless compilation, which is not supported yet.");
                result.Children.Add(child.LocalName == "argActionBlock" ? Token(child, "LEXER_CHAR_SET") : child);
            }
        }
        return result;
    }
}
