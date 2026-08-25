using System;
using System.Collections.Generic;
using System.Linq;
using ParseTreeEditing.UnvParseTreeDOM;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>
/// Builds a lexer ATN from a <see cref="GrammarModel"/> whose Kind is Lexer.
/// Overrides the parser-specific walk methods from <see cref="ParserAtnFactory"/>
/// to handle lexer-grammar constructs (character ranges, char-sets, lexer commands, etc.).
/// Translated from antlr-ng's LexerATNFactory.ts.
/// </summary>
public class LexerAtnFactory : ParserAtnFactory
{
    // Deduplicated list of ILexerAction instances. Index = position in _atn.lexerActions.
    private readonly List<ILexerAction> _lexerActions = new();
    // Map from action identity string → index in _lexerActions.
    private readonly Dictionary<string, int> _lexerActionIndex = new();

    public LexerAtnFactory(GrammarModel grammar, OptimizeOptions optimize = null) : base(grammar, optimize) { }

    // =========================================================================
    // ATN creation override
    // =========================================================================

    public override ATN CreateATN()
    {
        // Create one TokensStartState per mode.
        if (_grammar.ModeNames.Count == 0)
            _grammar.ModeNames.Add("DEFAULT_MODE");

        foreach (var modeName in _grammar.ModeNames)
        {
            var modeStart = new TokensStartState();
            modeStart.ruleIndex = -1;
            _atn.AddState(modeStart);
            _atn.DefineDecisionState(modeStart);
            _atn.modeToStartState.Add(modeStart);
            _atn.modeNameToStartState[modeName] = modeStart;
        }

        CreateRuleStartStopStates();

        // Set ruleToTokenType (non-fragment rules only).
        _atn.ruleToTokenType = new int[_grammar.Rules.Count];
        foreach (var rule in _grammar.Rules)
        {
            _atn.ruleToTokenType[rule.Index] = rule.IsFragment ? 0 : rule.TokenType;
        }

        foreach (var rule in _grammar.Rules)
            BuildRule(rule);

        AddRuleFollowLinks();

        // Link each mode start state to the start states of all non-fragment rules in that mode.
        for (int modeIdx = 0; modeIdx < _grammar.ModeNames.Count; modeIdx++)
        {
            var modeName = _grammar.ModeNames[modeIdx];
            var modeStart = _atn.modeToStartState[modeIdx];
            foreach (var rule in _grammar.Rules)
            {
                if (!rule.IsFragment && rule.ModeName == modeName)
                {
                    var ruleStart = _atn.ruleToStartState[rule.Index];
                    AddEpsilon(modeStart, ruleStart);
                }
            }
        }

        // Expose the collected lexer actions.
        _atn.lexerActions = _lexerActions.ToArray();

        if (_optimize.MergeSets)   AtnOptimizer.OptimizeSets(_atn);
        if (_optimize.Any)         AtnOptimizer.OptimizeStates(_atn);
        return _atn;
    }

    // Returns the effective caseInsensitive setting for the current rule:
    // per-rule override if present, otherwise grammar-level setting.
    private bool CurrentCaseInsensitive =>
        _currentRule?.PerRuleCaseInsensitive ?? _grammar.IsCaseInsensitive;

    // =========================================================================
    // Rule body: lexerRuleBlock → lexerAltList
    // =========================================================================

    protected override AtnHandle WalkRuleBody(UnvParseTreeElement bodyNode)
    {
        // Implicit T__N rule (no parse tree body, just a string literal to match char-by-char).
        if (bodyNode == null && _currentRule?.ImplicitLiteral != null)
            return MakeCharSequence(_currentRule.ImplicitLiteral);
        if (bodyNode == null) return MakeEpsilonHandle();

        // bodyNode is "lexerRuleBlock"
        var altList = Child(bodyNode, "lexerAltList");
        if (altList == null) return MakeEpsilonHandle();
        return WalkLexerAltList(altList);
    }

    private AtnHandle WalkLexerAltList(UnvParseTreeElement altListNode)
    {
        // Mirror ANTLR4's BlockSetTransformer: if every alternative is a single
        // characterRange or STRING_LITERAL atom (no LEXER_CHAR_SET, no ruleref,
        // no commands), collapse them into one set transition without a block.
        var blockSet = TryBuildBlockSet(altListNode);
        if (blockSet != null) return blockSet;

        var alts = new List<AtnHandle>();
        int altIdx = 0;
        foreach (var child in Children(altListNode))
        {
            if (IsTerminal(child)) continue; // skip OR tokens
            if (child.LocalName != "lexerAlt") continue;
            _currentOuterAlt = altIdx++;
            var h = WalkLexerAlt(child);
            if (h != null) alts.Add(h);
        }
        return MakeBlock(altListNode, alts, null);
    }

    /// <summary>
    /// If there are 2+ alts and every alt is a collapsible single-atom
    /// (characterRange or a single-char terminalDef STRING_LITERAL, no LEXER_CHAR_SET,
    /// no commands, no suffix), merges them into one set handle (matching ANTLR4's
    /// BlockSetTransformer). Returns null if fewer than 2 alts or any alt is not eligible.
    /// </summary>
    private AtnHandle TryBuildBlockSet(UnvParseTreeElement altListNode)
    {
        var altChildren = Children(altListNode).Where(c => !IsTerminal(c) && c.LocalName == "lexerAlt").ToList();
        // BlockSetTransformer only collapses blocks with 2+ alternatives.
        if (altChildren.Count < 2) return null;

        var matchSet = new IntervalSet();
        foreach (var alt in altChildren)
        {
            // No lexer commands allowed.
            if (Child(alt, "lexerCommands") != null) return null;

            var elements = Child(alt, "lexerElements");
            if (elements == null) return null;

            // Exactly one lexerElement, no ebnfSuffix, no action.
            var elementList = Children(elements).Where(c => c.LocalName == "lexerElement").ToList();
            if (elementList.Count != 1) return null;
            var lexerElement = elementList[0];
            if (Child(lexerElement, "actionBlock") != null) return null;
            if (Children(lexerElement).Any(c => c.LocalName == "ebnfSuffix")) return null;

            var lexerAtom = Child(lexerElement, "lexerAtom");
            if (lexerAtom == null) return null;

            // characterRange: 'a'..'z'
            var charRange = Child(lexerAtom, "characterRange");
            if (charRange != null)
            {
                var lits = Children(charRange)
                    .Where(c => IsTerminal(c) && c.LocalName == "STRING_LITERAL").ToList();
                if (lits.Count < 2) return null;
                int a = CharValue(GetText(lits[0]).Trim());
                int b = CharValue(GetText(lits[1]).Trim());
                if (a < 0 || b < 0) return null;
                matchSet.Add(a, b);
                continue;
            }

            // terminalDef: single-char STRING_LITERAL only (not TOKEN_REF, not multi-char).
            var terminalDef = Child(lexerAtom, "terminalDef");
            if (terminalDef != null)
            {
                var strLit = ChildTerminal(terminalDef, "STRING_LITERAL");
                if (strLit == null) return null; // TOKEN_REF not eligible
                var litText = GetText(strLit).Trim();
                // Strip surrounding single quotes.
                var content = litText;
                if (content.Length >= 2 && content[0] == '\'') content = content[1..];
                if (content.Length >= 1 && content[^1] == '\'') content = content[..^1];
                // Must be exactly one effective character (not a multi-char string like 'abort').
                var (ch, clen) = NextCharInSequence(content, 0);
                if (clen == 0 || clen != content.Length) return null;
                matchSet.Add(ch);
                continue;
            }

            // LEXER_CHAR_SET, ruleref, notSet, wildcard → not eligible.
            return null;
        }

        if (CurrentCaseInsensitive) matchSet = CaseExpandSet(matchSet);
        // Use the most specific transition type (Atom/Range/Set) to match ANTLR4's output.
        var setLeft = NewState<BasicState>();
        var setRight = NewState<BasicState>();
        setLeft.AddTransition(MakeIntervalSetTransition(setRight, matchSet));
        return new AtnHandle(setLeft, setRight);
    }

    private AtnHandle WalkLexerAlt(UnvParseTreeElement lexerAlt)
    {
        // lexerAlt : lexerElements lexerCommands?
        var elements = Child(lexerAlt, "lexerElements");
        var commands = Child(lexerAlt, "lexerCommands");

        AtnHandle h = elements != null ? WalkLexerElements(elements) : MakeEpsilonHandle();

        if (commands != null)
        {
            var commandHandles = WalkLexerCommands(commands);
            if (commandHandles.Count > 0)
            {
                // Mirror ANTLR4's lexerAltCommands: add a plain epsilon from the body's
                // right state to the first command, keeping the body.Right state intact.
                // Using ElemList here would fold that state away (one fewer state than ANTLR4).
                var cmds = commandHandles.Count == 1
                    ? commandHandles[0]
                    : ElemList(commandHandles);
                AddEpsilon(h.Right, cmds.Left);
                h = new AtnHandle(h.Left, cmds.Right);
            }
        }

        return h;
    }

    private AtnHandle WalkLexerElements(UnvParseTreeElement lexerElements)
    {
        var handles = new List<AtnHandle>();
        foreach (var child in Children(lexerElements))
        {
            if (child.LocalName != "lexerElement") continue;
            var h = WalkLexerElement(child);
            if (h != null) handles.Add(h);
        }
        if (handles.Count == 0) return MakeEpsilonHandle();
        return ElemList(handles);
    }

    private AtnHandle WalkLexerElement(UnvParseTreeElement lexerElement)
    {
        // lexerElement : labeledLexerElement ebnfSuffix?
        //              | lexerAtom ebnfSuffix?
        //              | lexerBlock ebnfSuffix?
        //              | actionBlock QUESTION?

        var actionBlock = Child(lexerElement, "actionBlock");
        if (actionBlock != null)
        {
            bool isPred = Children(lexerElement).Any(c => IsTerminal(c) && GetText(c).Trim() == "?");
            if (isPred) return MakeSemPred(lexerElement, actionBlock);
            return MakeLexerCustomAction(actionBlock);
        }

        AtnHandle h = null;
        var lexerAtom = Child(lexerElement, "lexerAtom");
        var lexerBlock = Child(lexerElement, "lexerBlock");
        var labeledLexerElement = Child(lexerElement, "labeledLexerElement");

        if (labeledLexerElement != null)
        {
            lexerAtom = Child(labeledLexerElement, "lexerAtom");
            lexerBlock = Child(labeledLexerElement, "lexerBlock");
        }

        var suffix = Children(lexerElement).FirstOrDefault(c => c.LocalName == "ebnfSuffix");

        if (lexerAtom != null)
            h = WalkLexerAtom(lexerAtom);
        else if (lexerBlock != null)
            // Pass the suffix directly into MakeBlock so it creates StarBlockStartState /
            // PlusBlockStartState as the inner block start (matching ANTLR4's block() method),
            // instead of wrapping in an extra outer StarBlockStartState via WrapInBlock.
            return WalkLexerBlock(lexerBlock, suffix) ?? MakeEpsilonHandle();

        if (h != null && suffix != null)
            h = ApplySuffix(lexerElement, suffix, WrapInBlock(h, GetText(suffix).Trim()));

        return h ?? MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerAtom(UnvParseTreeElement lexerAtom)
    {
        // lexerAtom : characterRange
        //           | terminalDef
        //           | referenceModifier? ruleref  (fragment reference)
        //           | notSet
        //           | LEXER_CHAR_SET
        //           | wildcard   (wildcard : DOT elementOptions?)

        var charRange = Child(lexerAtom, "characterRange");
        if (charRange != null) return WalkCharacterRange(charRange);

        var terminalDef = Child(lexerAtom, "terminalDef");
        if (terminalDef != null) return WalkLexerTerminalDef(terminalDef);

        var ruleref = Child(lexerAtom, "ruleref");
        if (ruleref != null) return WalkRuleRef(ruleref);

        var notSet = Child(lexerAtom, "notSet");
        if (notSet != null) return WalkLexerNotSet(notSet);

        var charSetTerm = ChildTerminal(lexerAtom, "LEXER_CHAR_SET");
        if (charSetTerm != null) return WalkCharSetLiteral(GetText(charSetTerm).Trim());

        // wildcard : DOT elementOptions?  (the DOT is nested inside a wildcard subelement)
        var wildcard = Child(lexerAtom, "wildcard");
        if (wildcard != null) return MakeWildcard();

        return MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerTerminalDef(UnvParseTreeElement terminalDef)
    {
        // In lexer rules: STRING_LITERAL → char sequence; TOKEN_REF 'EOF' → AtomTransition(-1)
        var tokenRef = ChildTerminal(terminalDef, "TOKEN_REF");
        if (tokenRef != null)
        {
            var name = GetText(tokenRef).Trim();
            SetSrcRange(tokenRef, name.Length);
            return MakeLexerTokenRef(name);
        }
        var strLit = ChildTerminal(terminalDef, "STRING_LITERAL");
        if (strLit != null)
        {
            var lit = GetText(strLit).Trim();
            SetSrcRange(strLit, lit.Length);
            return MakeCharSequence(lit);
        }
        return MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerBlock(UnvParseTreeElement lexerBlock, UnvParseTreeElement ebnfSuffix = null)
    {
        // lexerBlock : LPAREN lexerAltList RPAREN
        var altList = Child(lexerBlock, "lexerAltList");
        if (altList == null) return MakeEpsilonHandle();

        // For suffix-free blocks, try the same BlockSetTransformer collapse as for rule-level alts.
        if (ebnfSuffix == null)
        {
            var blockSet = TryBuildBlockSet(altList);
            if (blockSet != null) return blockSet;
        }

        var alts = new List<AtnHandle>();
        int altIdx = 0;
        foreach (var child in Children(altList))
        {
            if (IsTerminal(child)) continue;
            if (child.LocalName != "lexerAlt") continue;
            _currentOuterAlt = altIdx++;
            var h = WalkLexerAlt(child);
            if (h != null) alts.Add(h);
        }
        return MakeBlock(lexerBlock, alts, ebnfSuffix);
    }

    private AtnHandle WalkCharacterRange(UnvParseTreeElement characterRange)
    {
        // characterRange : STRING_LITERAL RANGE STRING_LITERAL
        var literals = Children(characterRange)
            .Where(c => IsTerminal(c) && c.LocalName == "STRING_LITERAL")
            .ToList();
        if (literals.Count < 2) return MakeEpsilonHandle();

        var fromChar = CharValue(GetText(literals[0]).Trim());
        var toChar   = CharValue(GetText(literals[1]).Trim());
        if (fromChar < 0 || toChar < 0) return MakeEpsilonHandle();

        var endLit = GetText(literals[1]).Trim();
        SetSrcRange(literals[0], literals[1], endLit.Length);

        var set = new IntervalSet();
        set.Add(fromChar, toChar);
        if (CurrentCaseInsensitive) set = CaseExpandSet(set);
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(MakeIntervalSetTransition(right, set));
        return new AtnHandle(left, right);
    }

    private AtnHandle WalkLexerNotSet(UnvParseTreeElement notSet)
    {
        // notSet : NOT setElement | NOT blockSet
        // In lexer: elements are character-based (char ranges, char sets, string literals)
        var set = new IntervalSet();
        var setElement = Child(notSet, "setElement");
        var blockSet = Child(notSet, "blockSet");

        if (setElement != null) AddLexerSetElement(set, setElement);
        if (blockSet != null)
            foreach (var se in Children(blockSet, "setElement"))
                AddLexerSetElement(set, se);

        return MakeNotSet(set);
    }

    private void AddLexerSetElement(IntervalSet set, UnvParseTreeElement se)
    {
        var charRange = Child(se, "characterRange");
        if (charRange != null)
        {
            var literals = Children(charRange)
                .Where(c => IsTerminal(c) && c.LocalName == "STRING_LITERAL")
                .ToList();
            if (literals.Count >= 2)
            {
                var a = CharValue(GetText(literals[0]).Trim());
                var b = CharValue(GetText(literals[1]).Trim());
                if (a >= 0 && b >= 0)
                {
                    if (CurrentCaseInsensitive)
                        set.AddAll(CaseExpandSet(IntervalSet.Of(a, b)));
                    else
                        set.Add(a, b);
                }
            }
            return;
        }

        var charSetTerm = ChildTerminal(se, "LEXER_CHAR_SET");
        if (charSetTerm != null)
        {
            var s = ParseCharSet(GetText(charSetTerm).Trim());
            foreach (var interval in s.GetIntervals())
                set.Add(interval.a, interval.b);
            return;
        }

        var strLit = ChildTerminal(se, "STRING_LITERAL");
        if (strLit != null)
        {
            var c = CharValue(GetText(strLit).Trim());
            if (c >= 0)
            {
                if (CurrentCaseInsensitive)
                    set.AddAll(CaseExpandSet(IntervalSet.Of(c, c)));
                else
                    set.Add(c);
            }
            return;
        }

        // TOKEN_REF in a set is unusual for lexer but handle gracefully
        var tokenRef = ChildTerminal(se, "TOKEN_REF");
        if (tokenRef != null)
        {
            var tt = GetTokenType(GetText(tokenRef).Trim());
            if (tt > 0) set.Add(tt);
        }
    }

    private AtnHandle WalkCharSetLiteral(string charSetText)
    {
        var set = ParseCharSet(charSetText);
        return MakeSet(set);
    }

    // =========================================================================
    // Lexer commands: skip, more, popMode, mode(x), pushMode(x), type(x), channel(x)
    // =========================================================================

    private List<AtnHandle> WalkLexerCommands(UnvParseTreeElement lexerCommands)
    {
        // lexerCommands : RARROW lexerCommand (COMMA lexerCommand)*
        var result = new List<AtnHandle>();
        foreach (var cmd in Children(lexerCommands, "lexerCommand"))
        {
            var h = WalkLexerCommand(cmd);
            if (h != null) result.Add(h);
        }
        return result;
    }

    private AtnHandle WalkLexerCommand(UnvParseTreeElement lexerCommand)
    {
        // lexerCommand : lexerCommandName LPAREN lexerCommandExpr RPAREN
        //              | lexerCommandName
        var nameNode = Child(lexerCommand, "lexerCommandName");
        if (nameNode == null) return null;
        var cmdName = GetText(nameNode).Trim().ToLowerInvariant();

        var exprNode = Child(lexerCommand, "lexerCommandExpr");
        string exprText = exprNode != null ? GetText(exprNode).Trim() : null;

        ILexerAction action;
        switch (cmdName)
        {
            case "skip":
                action = LexerSkipAction.Instance;
                break;
            case "more":
                action = LexerMoreAction.Instance;
                break;
            case "popmode":
                action = LexerPopModeAction.Instance;
                break;
            case "mode":
                int modeIdx = GetModeIndex(exprText ?? "DEFAULT_MODE");
                action = new LexerModeAction(modeIdx);
                break;
            case "pushmode":
                int pushModeIdx = GetModeIndex(exprText ?? "DEFAULT_MODE");
                action = new LexerPushModeAction(pushModeIdx);
                break;
            case "type":
                int typeVal = exprText != null ? GetTokenType(exprText) : 0;
                action = new LexerTypeAction(typeVal);
                break;
            case "channel":
                int channelVal = GetChannelValue(exprText ?? "DEFAULT_TOKEN_CHANNEL");
                action = new LexerChannelAction(channelVal);
                break;
            default:
                // Unknown command → treat as custom action
                var customIdx = _currentRule?.Actions.Count ?? 0;
                var ruleIdx = _currentRule?.Index ?? -1;
                action = new LexerCustomAction(ruleIdx, customIdx);
                var info = new ActionInfo { RuleIndex = ruleIdx, ActionIndex = customIdx, Text = GetText(lexerCommand).Trim() };
                _currentRule?.Actions.Add(info);
                break;
        }

        int actionIndex = GetLexerActionIndex(action);
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new ActionTransition(right, _currentRule?.Index ?? -1, actionIndex, false));
        return new AtnHandle(left, right);
    }

    // =========================================================================
    // Inline action block (not a lexer command, but inline { } in lexer rule)
    // =========================================================================

    private AtnHandle MakeLexerCustomAction(UnvParseTreeElement actionBlock)
    {
        var ruleIndex = _currentRule?.Index ?? -1;
        var actionIndex = _currentRule?.Actions.Count ?? 0;
        var text = GetText(actionBlock).Trim();
        var info = new ActionInfo { RuleIndex = ruleIndex, ActionIndex = actionIndex, Text = text };
        _currentRule?.Actions.Add(info);

        var action = new LexerCustomAction(ruleIndex, actionIndex);
        int idx = GetLexerActionIndex(action);

        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new ActionTransition(right, ruleIndex, idx, false));
        return new AtnHandle(left, right);
    }

    // =========================================================================
    // Char sequence: string literal → AtomTransition per character
    // =========================================================================

    private AtnHandle MakeCharSequence(string grammarLiteral)
    {
        // Strip surrounding single quotes.
        var s = grammarLiteral;
        if (s.Length >= 2 && s[0] == '\'') s = s[1..];
        if (s.Length >= 1 && s[^1] == '\'') s = s[..^1];

        var chars = new List<int>();
        int i = 0;
        while (i < s.Length)
        {
            var (c, len) = NextCharInSequence(s, i);
            if (len == 0) break;
            chars.Add(c);
            i += len;
        }

        if (chars.Count == 0) return MakeEpsilonHandle();

        // Build a left → right chain of basic states.
        var states = new List<ATNState>();
        for (int j = 0; j <= chars.Count; j++)
            states.Add(NewState<BasicState>());

        for (int j = 0; j < chars.Count; j++)
        {
            int c = chars[j];
            if (CurrentCaseInsensitive)
            {
                int lo = char.ToLowerInvariant((char)c);
                int hi = char.ToUpperInvariant((char)c);
                if (lo != hi)
                {
                    var cs = new IntervalSet();
                    cs.Add(lo); cs.Add(hi);
                    states[j].AddTransition(new SetTransition(states[j + 1], cs));
                    continue;
                }
            }
            states[j].AddTransition(new AtomTransition(states[j + 1], c));
        }

        return new AtnHandle(states[0], states[^1]);
    }

    private AtnHandle MakeLexerTokenRef(string name)
    {
        if (name == "EOF")
        {
            var left = NewState<BasicState>();
            var right = NewState<BasicState>();
            left.AddTransition(new AtomTransition(right, -1));
            return new AtnHandle(left, right);
        }
        // Reference to another lexer rule (fragment or non-fragment).
        return MakeRuleRef(name);
    }

    // =========================================================================
    // Charset parsing
    // =========================================================================

    private IntervalSet ParseCharSet(string charSetText)
    {
        // ANTLR lexer character sets use '~' outside the brackets for
        // negation. A leading '^' inside the brackets is an ordinary member
        // of the set (for example [^v<>] matches the four arrow characters).
        var s = charSetText;
        // Strip surrounding brackets.
        if (s.Length >= 2 && s[0] == '[') s = s[1..];
        if (s.Length >= 1 && s[^1] == ']') s = s[..^1];

        var set = new IntervalSet();
        int idx = 0;

        while (idx < s.Length)
        {
            var (c, len) = NextCharInSet(s, idx);
            if (len == 0) break;
            idx += len;

            // Check for range: c '-' d
            if (idx < s.Length && s[idx] == '-' && idx + 1 < s.Length)
            {
                idx++; // consume '-'
                var (d, dlen) = NextCharInSet(s, idx);
                if (dlen > 0)
                {
                    idx += dlen;
                    set.Add(c, d);
                    continue;
                }
                // '-' at end — treat as literal hyphen
                set.Add(c);
                set.Add('-');
                continue;
            }
            set.Add(c);
        }

        if (CurrentCaseInsensitive) set = CaseExpandSet(set);
        return set;
    }

    private static (int ch, int len) NextCharInSequence(string s, int idx)
    {
        if (idx >= s.Length) return (0, 0);
        if (s[idx] != '\\') return (s[idx], 1);
        if (idx + 1 >= s.Length) return (s[idx], 1);
        int c = s[idx + 1] switch
        {
            'n' => '\n', 'r' => '\r', 't' => '\t', 'b' => '\b',
            'f' => '\f', '\\' => '\\', '\'' => '\'', '"' => '"',
            'u' when idx + 5 < s.Length => int.Parse(s.Substring(idx + 2, 4), System.Globalization.NumberStyles.HexNumber),
            _ => s[idx + 1]
        };
        int len = (s[idx + 1] == 'u' && idx + 5 < s.Length) ? 6 : 2;
        return (c, len);
    }

    private static (int ch, int len) NextCharInSet(string s, int idx)
    {
        if (idx >= s.Length) return (0, 0);
        if (s[idx] != '\\') return (s[idx], 1);
        if (idx + 1 >= s.Length) return (s[idx], 1);
        int c = s[idx + 1] switch
        {
            'n' => '\n', 'r' => '\r', 't' => '\t', 'b' => '\b',
            'f' => '\f', '\\' => '\\', ']' => ']', '-' => '-',
            'u' when idx + 5 < s.Length => int.Parse(s.Substring(idx + 2, 4), System.Globalization.NumberStyles.HexNumber),
            _ => s[idx + 1]
        };
        int len = (s[idx + 1] == 'u' && idx + 5 < s.Length) ? 6 : 2;
        return (c, len);
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private int GetLexerActionIndex(ILexerAction action)
    {
        var key = $"{(int)action.ActionType}:{ActionData1(action)}:{ActionData2(action)}";
        if (_lexerActionIndex.TryGetValue(key, out var idx)) return idx;
        idx = _lexerActions.Count;
        _lexerActions.Add(action);
        _lexerActionIndex[key] = idx;
        return idx;
    }

    private static int ActionData1(ILexerAction action) => action switch
    {
        LexerChannelAction a => a.Channel,
        LexerModeAction a => a.Mode,
        LexerPushModeAction a => a.Mode,
        LexerTypeAction a => a.Type,
        LexerCustomAction a => a.RuleIndex,
        _ => 0
    };

    private static int ActionData2(ILexerAction action) => action switch
    {
        LexerCustomAction a => a.ActionIndex,
        _ => 0
    };

    private int GetModeIndex(string modeName)
    {
        var idx = _grammar.ModeNames.IndexOf(modeName);
        if (idx >= 0) return idx;
        // Fallback: 0 = DEFAULT_MODE
        return 0;
    }

    private int GetChannelValue(string channelName) => channelName switch
    {
        "DEFAULT_TOKEN_CHANNEL" or "DEFAULT_CHANNEL" => 0,
        "HIDDEN" => 1,
        _ => 2 + (_grammar.ExtraChannelNames.IndexOf(channelName) is int i && i >= 0 ? i : 0)
    };
    // =========================================================================
    // Case-insensitive helpers
    // =========================================================================

    /// <summary>
    /// Expands every character in <paramref name="set"/> to include both its
    /// lower-case and upper-case form (using invariant culture), matching
    /// ANTLR4's caseInsensitive grammar option behaviour.
    /// </summary>
    private static IntervalSet CaseExpandSet(IntervalSet set)
    {
        var expanded = new IntervalSet();
        foreach (var iv in set.GetIntervals())
            for (int c = iv.a; c <= iv.b; c++)
            {
                expanded.Add(char.ToLowerInvariant((char)c));
                expanded.Add(char.ToUpperInvariant((char)c));
            }
        return expanded;
    }

    /// <summary>
    /// Creates the most specific transition type for the given interval set:
    /// AtomTransition for a single-element set, RangeTransition for a
    /// single two-endpoint interval, SetTransition otherwise.
    /// </summary>
    private static Transition MakeIntervalSetTransition(ATNState target, IntervalSet set)
    {
        var intervals = set.GetIntervals();
        if (intervals.Count == 1)
        {
            var iv = intervals[0];
            if (iv.a == iv.b) return new AtomTransition(target, iv.a);
            return new RangeTransition(target, iv.a, iv.b);
        }
        return new SetTransition(target, set);
    }


}
