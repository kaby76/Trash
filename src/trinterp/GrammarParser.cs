using System;
using System.Collections.Generic;
using System.Linq;
using ParseTreeEditing.UnvParseTreeDOM;

namespace trinterp;

/// <summary>
/// Builds a <see cref="GrammarModel"/> by walking an ANTLRv4 parse tree produced
/// by trparse (using the ANTLRv4Lexer/ANTLRv4Parser grammars at
/// Trash/src/grammars/antlr4).
/// </summary>
public class GrammarParser
{
    // Pre-defined token types that appear in every grammar.
    private const int TokenEOF = -1;
    private const int TokenMinUser = 1; // first assignable type

    public GrammarModel Parse(UnvParseTreeElement root, string fileName)
    {
        // root.LocalName == "grammarSpec"
        var model = new GrammarModel { FileName = fileName };

        // --- grammar declaration ---
        var grammarDecl = Child(root, "grammarDecl");
        var grammarType = Child(grammarDecl, "grammarType");
        var identifier = Child(grammarDecl, "identifier");
        model.Name = GetText(identifier).Trim();
        model.Kind = ParseKind(grammarType);

        // --- prequel constructs (options, channels, tokens, actions) ---
        foreach (var pre in Children(root, "prequelConstruct"))
            ProcessPrequel(model, pre);

        // --- rules ---
        var rulesNode = Child(root, "rules");
        if (rulesNode != null)
        {
            foreach (var ruleSpec in Children(rulesNode, "ruleSpec"))
            {
                var parserRule = Child(ruleSpec, "parserRuleSpec");
                var lexerRule = Child(ruleSpec, "lexerRuleSpec");
                if (parserRule != null)
                    AddParserRule(model, parserRule, null);
                else if (lexerRule != null)
                    AddLexerRule(model, lexerRule, "DEFAULT_MODE");
            }
        }

        // --- mode specs (lexer grammars) ---
        foreach (var modeSpec in Children(root, "modeSpec"))
            ProcessModeSpec(model, modeSpec);

        // --- finalise token types ---
        AssignTokenTypes(model);

        // --- for combined grammars: split into lexer + parser halves ---
        if (model.Kind == GrammarKind.Combined)
            SplitCombined(model);

        return model;
    }

    // -------------------------------------------------------------------------
    // Prequel constructs
    // -------------------------------------------------------------------------

    private void ProcessPrequel(GrammarModel model, UnvParseTreeElement pre)
    {
        var optionsSpec = Child(pre, "optionsSpec");
        if (optionsSpec != null) { ProcessOptions(model, optionsSpec); return; }

        var channelsSpec = Child(pre, "channelsSpec");
        if (channelsSpec != null) { ProcessChannels(model, channelsSpec); return; }

        var tokensSpec = Child(pre, "tokensSpec");
        if (tokensSpec != null) { ProcessTokensSpec(model, tokensSpec); return; }

        var action = Child(pre, "action_");
        if (action != null) { ProcessNamedAction(model, action); return; }
    }

    private void ProcessOptions(GrammarModel model, UnvParseTreeElement optionsSpec)
    {
        foreach (var option in Children(optionsSpec, "option"))
        {
            var id = Child(option, "identifier");
            var val = Child(option, "optionValue");
            if (id == null || val == null) continue;
            var key = GetText(id).Trim();
            var value = GetText(val).Trim();
            if (key == "caseInsensitive" && value.ToLowerInvariant() == "true")
                model.IsCaseInsensitive = true;
            else if (key == "tokenVocab")
                model.TokenVocab = value;
            // Other options (superClass, etc.) are ignored for ATN purposes.
        }
    }

    private void ProcessChannels(GrammarModel model, UnvParseTreeElement channelsSpec)
    {
        var idList = Child(channelsSpec, "idList");
        if (idList == null) return;
        foreach (var id in Children(idList, "identifier"))
            model.ExtraChannelNames.Add(GetText(id).Trim());
    }

    private void ProcessTokensSpec(GrammarModel model, UnvParseTreeElement tokensSpec)
    {
        var idList = Child(tokensSpec, "idList");
        if (idList == null) return;
        foreach (var id in Children(idList, "identifier"))
        {
            var name = GetText(id).Trim();
            if (!model.DeclaredTokens.Contains(name))
                model.DeclaredTokens.Add(name);
        }
    }

    private void ProcessNamedAction(GrammarModel model, UnvParseTreeElement action)
    {
        // action_ : AT (actionScopeName COLONCOLON)? identifier actionBlock
        string scope = null;
        string name = null;
        string text = null;

        var children = Children(action).ToList();
        int i = 0;
        // skip AT
        while (i < children.Count && IsTerminal(children[i]) && GetText(children[i]).Trim() == "@") i++;

        // optional scope
        var scopeNode = Child(action, "actionScopeName");
        if (scopeNode != null) scope = GetText(scopeNode).Trim();

        var identNode = Child(action, "identifier");
        if (identNode != null) name = GetText(identNode).Trim();

        var actionBlock = Child(action, "actionBlock");
        if (actionBlock != null) text = GetText(actionBlock).Trim();

        if (name != null && text != null)
        {
            var key = scope != null ? scope + "::" + name : name;
            model.NamedActions.Add(new NamedActionInfo { Scope = scope, Name = name, Text = text });
        }
    }

    private void ProcessModeSpec(GrammarModel model, UnvParseTreeElement modeSpec)
    {
        // modeSpec : MODE identifier SEMI lexerRuleSpec*
        var idNode = Child(modeSpec, "identifier");
        if (idNode == null) return;
        var modeName = GetText(idNode).Trim();
        if (!model.ModeNames.Contains(modeName))
            model.ModeNames.Add(modeName);
        foreach (var lexerRule in Children(modeSpec, "lexerRuleSpec"))
            AddLexerRule(model, lexerRule, modeName);
    }

    // -------------------------------------------------------------------------
    // Rule collection
    // -------------------------------------------------------------------------

    private void AddParserRule(GrammarModel model, UnvParseTreeElement parserRuleSpec, string modeName)
    {
        // parserRuleSpec : ruleModifiers? RULE_REF ... COLON ruleBlock SEMI ...
        var nameNode = ChildTerminal(parserRuleSpec, "RULE_REF");
        if (nameNode == null) return;
        var name = GetText(nameNode).Trim();

        var ruleBlock = Child(parserRuleSpec, "ruleBlock");
        var rule = new RuleModel
        {
            Name = name,
            Index = model.Rules.Count,
            IsFragment = false,
            TokenType = 0,
            ModeName = modeName,
            BodyNode = ruleBlock
        };

        // Collect rule-level named actions (@init, @after)
        foreach (var prequel in Children(parserRuleSpec, "rulePrequel"))
        {
            var ra = Child(prequel, "ruleAction");
            if (ra != null)
            {
                var raidNode = Child(ra, "identifier");
                var rab = Child(ra, "actionBlock");
                if (raidNode != null && rab != null)
                {
                    var raName = GetText(raidNode).Trim();
                    var raText = GetText(rab).Trim();
                    model.NamedActions.Add(new NamedActionInfo { Scope = null, Name = name + "::" + raName, Text = raText });
                }
            }
        }

        model.Rules.Add(rule);
    }

    private void AddLexerRule(GrammarModel model, UnvParseTreeElement lexerRuleSpec, string modeName)
    {
        // lexerRuleSpec : FRAGMENT? TOKEN_REF optionsSpec? COLON lexerRuleBlock SEMI
        bool isFragment = Children(lexerRuleSpec).Any(c => IsTerminal(c) && GetText(c).Trim() == "fragment");
        var nameNode = ChildTerminal(lexerRuleSpec, "TOKEN_REF");
        if (nameNode == null) return;
        var name = GetText(nameNode).Trim();

        var lexerRuleBlock = Child(lexerRuleSpec, "lexerRuleBlock");
        var rule = new RuleModel
        {
            Name = name,
            Index = model.Rules.Count,
            IsFragment = isFragment,
            TokenType = 0, // assigned later
            ModeName = modeName ?? "DEFAULT_MODE",
            BodyNode = lexerRuleBlock
        };
        model.Rules.Add(rule);
    }

    // -------------------------------------------------------------------------
    // Token type assignment
    // -------------------------------------------------------------------------

    private void AssignTokenTypes(GrammarModel model)
    {
        // EOF is always -1
        model.TokenNameToType["EOF"] = TokenEOF;

        if (model.IsLexer || model.Kind == GrammarKind.Combined)
        {
            // Ensure DEFAULT_MODE is first
            if (!model.ModeNames.Contains("DEFAULT_MODE"))
                model.ModeNames.Insert(0, "DEFAULT_MODE");

            int nextType = TokenMinUser;

            // Declared tokens (from tokens { ... }) get types first.
            foreach (var t in model.DeclaredTokens)
            {
                if (!model.TokenNameToType.ContainsKey(t))
                    model.TokenNameToType[t] = nextType++;
            }

            // For combined grammars: assign T__ types FIRST (before named lexer rules),
            // matching ANTLR4's type-numbering order so set elements sort correctly.
            // Literals that already have a named single-literal lexer rule are skipped
            // (they will reuse that rule's type, assigned below).
            if (model.Kind == GrammarKind.Combined)
            {
                // Build set of literals covered by named single-literal lexer rules.
                var namedBodyLiterals = new System.Collections.Generic.HashSet<string>();
                foreach (var rule in model.Rules)
                {
                    if (rule.IsFragment || !IsUpperFirst(rule.Name) || rule.BodyNode == null) continue;
                    var l2 = new System.Collections.Generic.List<string>();
                    var s2 = new System.Collections.Generic.HashSet<string>();
                    CollectStringLiterals(rule.BodyNode, s2, l2);
                    if (l2.Count == 1 && IsExactlySingleLiteralBody(rule.BodyNode))
                        namedBodyLiterals.Add(l2[0]);
                }
                // Assign T__ types to unnamed parser-rule literals (in appearance order).
                var seenLits = new System.Collections.Generic.HashSet<string>();
                var orderedLits = new System.Collections.Generic.List<string>();
                foreach (var rule in model.Rules.Where(r => !IsUpperFirst(r.Name)))
                    CollectStringLiterals(rule.BodyNode, seenLits, orderedLits);
                foreach (var lit in orderedLits)
                    if (!namedBodyLiterals.Contains(lit) && !model.StringLiteralToType.ContainsKey(lit))
                        model.StringLiteralToType[lit] = nextType++;
            }

            // Each non-fragment TOKEN_REF rule gets a type (after T__ types for combined grammars,
            // matching ANTLR4's numbering where T__N types are lower than named rule types).
            foreach (var rule in model.Rules)
            {
                if (!rule.IsFragment && IsUpperFirst(rule.Name))
                {
                    if (!model.TokenNameToType.ContainsKey(rule.Name))
                        model.TokenNameToType[rule.Name] = nextType++;
                    rule.TokenType = model.TokenNameToType[rule.Name];
                }
            }

            // Populate StringLiteralToType from simple single-literal lexer rules
            // (e.g. While:'while' → 'while'→N, COLON:':' → ':'→COLON.type).
            // This lets ATN DOT rendering show the literal form rather than token name.
            foreach (var rule in model.Rules)
            {
                if (rule.IsFragment || rule.BodyNode == null || rule.TokenType == 0) continue;
                var lits = new System.Collections.Generic.List<string>();
                var seen = new System.Collections.Generic.HashSet<string>();
                CollectStringLiterals(rule.BodyNode, seen, lits);
                if (lits.Count == 1
                    && IsExactlySingleLiteralBody(rule.BodyNode)
                    && !model.StringLiteralToType.ContainsKey(lits[0]))
                    model.StringLiteralToType[lits[0]] = rule.TokenType;
            }
        }
        else
        {
            // Parser grammar: we don't assign types here; they should come
            // from the lexer grammar passed alongside (merged later in Command.cs).
            foreach (var t in model.DeclaredTokens)
            {
                if (!model.TokenNameToType.ContainsKey(t))
                    model.TokenNameToType[t] = 0; // placeholder
            }
        }
    }

    // -------------------------------------------------------------------------
    // Combined grammar splitting
    // -------------------------------------------------------------------------

    private void SplitCombined(GrammarModel model)
    {
        // Separate lexer rules (TOKEN_REF) from parser rules (RULE_REF).
        var lexerRules = model.Rules.Where(r => IsUpperFirst(r.Name)).ToList();
        var parserRules = model.Rules.Where(r => !IsUpperFirst(r.Name)).ToList();

        // Re-index parser rules.
        for (int i = 0; i < parserRules.Count; i++) parserRules[i].Index = i;

        // Build the implicit lexer model.
        var lexer = new GrammarModel
        {
            Name = model.Name + "Lexer",
            Kind = GrammarKind.Lexer,
            FileName = model.FileName,
            IsCaseInsensitive = model.IsCaseInsensitive,
            TokenNameToType = new Dictionary<string, int>(model.TokenNameToType),
            StringLiteralToType = new Dictionary<string, int>(model.StringLiteralToType),
            ExtraChannelNames = model.ExtraChannelNames,
            ModeNames = model.ModeNames.Count > 0 ? model.ModeNames : new List<string> { "DEFAULT_MODE" },
            DeclaredTokens = model.DeclaredTokens,
        };
        // Re-index lexer rules and add to lexer model.
        for (int i = 0; i < lexerRules.Count; i++)
        {
            lexerRules[i].Index = i;
            lexer.Rules.Add(lexerRules[i]);
        }
        if (!lexer.ModeNames.Contains("DEFAULT_MODE"))
            lexer.ModeNames.Insert(0, "DEFAULT_MODE");

        // Create implicit T__N lexer rules for string literals used in parser rules that
        // have NO corresponding named single-literal lexer rule (those reuse the named type).
        // ANTLR4 inserts T__ rules BEFORE the explicit lexer rules, so we prepend them.
        var namedLexerRuleTypes = new HashSet<int>(
            lexerRules.Where(r => !r.IsFragment && r.TokenType != 0).Select(r => r.TokenType));
        int tIdx = 0;
        var tImplicit = new List<RuleModel>();
        foreach (var kv in model.StringLiteralToType.OrderBy(x => x.Value))
        {
            if (namedLexerRuleTypes.Contains(kv.Value)) continue; // named rule covers this literal
            var name = "T__" + tIdx++;
            // Add to both models' TokenNameToType (for .tokens file output).
            lexer.TokenNameToType[name] = kv.Value;
            model.TokenNameToType[name] = kv.Value;
            // Ensure lexer has the literal type too (may already be copied).
            lexer.StringLiteralToType[kv.Key] = kv.Value;

            tImplicit.Add(new RuleModel
            {
                Name = name,
                IsFragment = false,
                TokenType = kv.Value,
                ModeName = "DEFAULT_MODE",
                ImplicitLiteral = kv.Key,
                BodyNode = null
            });
        }
        // Prepend T__ rules so explicit lexer rules get higher indices (matching ANTLR4).
        lexer.Rules.InsertRange(0, tImplicit);

        // Re-index lexer rules (including any newly added implicit rules).
        for (int i = 0; i < lexer.Rules.Count; i++)
            lexer.Rules[i].Index = i;

        // For combined grammars, populate the parser model's StringLiteralToType with
        // reverse mappings from simple single-literal lexer rules (e.g., A:'a' → 'a'→1).
        // This lets TokenLabel return the literal form ('a') instead of the token name (A)
        // when rendering ATN transitions, matching antlr4's vocabulary display behaviour.
        foreach (var rule in lexerRules)
        {
            if (rule.IsFragment || rule.BodyNode == null) continue;
            var lits = new System.Collections.Generic.List<string>();
            var seen = new System.Collections.Generic.HashSet<string>();
            CollectStringLiterals(rule.BodyNode, seen, lits);
            if (lits.Count == 1
                && IsExactlySingleLiteralBody(rule.BodyNode)
                && !model.StringLiteralToType.ContainsKey(lits[0]))
                model.StringLiteralToType[lits[0]] = rule.TokenType;
        }

        // Update the main (parser) model to contain only parser rules.
        model.Rules = parserRules;
        model.ImplicitLexer = lexer;
    }

    // -------------------------------------------------------------------------
    // Parse-tree helpers
    // -------------------------------------------------------------------------

    private static GrammarKind ParseKind(UnvParseTreeElement grammarType)
    {
        // Look at terminal children by token-type name to avoid including
        // hidden-channel tokens (block comments) that appear before the
        // grammar modifier keyword in the parse tree.
        foreach (var child in Children(grammarType))
            if (IsTerminal(child))
            {
                if (child.LocalName == "LEXER") return GrammarKind.Lexer;
                if (child.LocalName == "PARSER") return GrammarKind.Parser;
            }
        return GrammarKind.Combined;
    }

    /// <summary>
    /// Returns true when the lexer rule body is exactly one alternative containing
    /// a single bare string-literal atom with no EBNF suffix.
    /// E.g.  While : 'while' ;  → true
    ///       HexLiteral : '0' [xX] ... ;  → false (multiple atoms)
    /// This matches ANTLR4's criterion for assigning a literal alias to a token.
    /// </summary>
    private static bool IsExactlySingleLiteralBody(UnvParseTreeElement bodyNode)
    {
        if (bodyNode == null) return false;

        // Lexer rule body: lexerRuleBlock → lexerAltList → lexerAlt → lexerElements → lexerElement → lexerAtom → terminalDef
        var lexerAltList = Child(bodyNode, "lexerAltList");
        if (lexerAltList != null)
        {
            var lexerAlts = Children(lexerAltList).Where(c => c.LocalName == "lexerAlt").ToList();
            if (lexerAlts.Count != 1) return false;
            var lexerElements = Child(lexerAlts[0], "lexerElements");
            if (lexerElements == null) return false;
            var lexerElems = Children(lexerElements).Where(c => c.LocalName == "lexerElement").ToList();
            if (lexerElems.Count != 1) return false;
            var elem = lexerElems[0];
            if (Child(elem, "ebnfSuffix") != null) return false;
            var lexerAtom = Child(elem, "lexerAtom");
            if (lexerAtom == null) return false;
            var terminalDef = Child(lexerAtom, "terminalDef");
            if (terminalDef == null) return false;
            return ChildTerminal(terminalDef, "STRING_LITERAL") != null;
        }

        // Parser rule body: ruleBlock → ruleAltList → labeledAlt/alternative → element → atom → terminalDef
        var altList = Child(bodyNode, "ruleAltList") ?? Child(bodyNode, "altList");
        if (altList == null) return false;
        var nonTerms = Children(altList).Where(c => !IsTerminal(c)).ToList();
        if (nonTerms.Count != 1) return false;
        var altNode = nonTerms[0].LocalName == "labeledAlt"
            ? Child(nonTerms[0], "alternative")
            : nonTerms[0].LocalName == "alternative" ? nonTerms[0] : null;
        if (altNode == null) return false;
        var elements = Children(altNode).Where(c => c.LocalName == "element").ToList();
        if (elements.Count != 1) return false;
        var pElem = elements[0];
        if (Child(pElem, "ebnfSuffix") != null) return false;
        var atom = Child(pElem, "atom");
        if (atom == null) return false;
        var pTerminalDef = Child(atom, "terminalDef");
        if (pTerminalDef == null) return false;
        return ChildTerminal(pTerminalDef, "STRING_LITERAL") != null;
    }

    private static void CollectStringLiterals(UnvParseTreeElement node, System.Collections.Generic.HashSet<string> seen, System.Collections.Generic.List<string> ordered)
    {
        if (node == null) return;
        foreach (var child in Children(node))
        {
            if (IsTerminal(child) && child.LocalName == "STRING_LITERAL")
            {
                var lit = GetText(child).Trim();
                if (seen.Add(lit)) ordered.Add(lit);
            }
            else
            {
                CollectStringLiterals(child, seen, ordered);
            }
        }
    }

    /// <summary>
    /// Counts the total number of STRING_LITERAL terminal occurrences (including duplicates).
    /// Used to distinguish true single-literal rules (e.g. While:'while') from rules that
    /// happen to have only one unique literal but use it multiple times (e.g.
    /// StringLiteral : ... '"' ... '"' ...).
    /// </summary>
    private static int CountStringLiteralOccurrences(UnvParseTreeElement node)
    {
        if (node == null) return 0;
        int count = 0;
        foreach (var child in Children(node))
        {
            if (IsTerminal(child) && child.LocalName == "STRING_LITERAL")
                count++;
            else
                count += CountStringLiteralOccurrences(child);
        }
        return count;
    }

    public static string GetText(UnvParseTreeElement node) => node?.GetText() ?? "";

    public static bool IsTerminal(UnvParseTreeElement node) => node.IsTerminal();

    public static bool IsUpperFirst(string name) =>
        !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);

    /// <summary>Returns the first child element with the given LocalName.</summary>
    public static UnvParseTreeElement Child(UnvParseTreeElement node, string localName) =>
        node?.GetChildren(localName).FirstOrDefault();

    /// <summary>Returns all child elements (rule and terminal) in order.</summary>
    public static IEnumerable<UnvParseTreeElement> Children(UnvParseTreeElement node) =>
        node?.Children ?? Enumerable.Empty<UnvParseTreeElement>();

    /// <summary>Returns all child elements with the given LocalName.</summary>
    public static IEnumerable<UnvParseTreeElement> Children(UnvParseTreeElement node, string localName) =>
        node?.GetChildren(localName) ?? Enumerable.Empty<UnvParseTreeElement>();

    /// <summary>Returns the first terminal child with the given token-type name.</summary>
    public static UnvParseTreeElement ChildTerminal(UnvParseTreeElement node, string tokenTypeName) =>
        node?.Children.FirstOrDefault(c => c.IsTerminal() && c.LocalName == tokenTypeName);
}
