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

    public LexerAtnFactory(GrammarModel grammar) : base(grammar) { }

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

        return _atn;
    }

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
                // Sequence: append command action handles after the element handles.
                var seq = new List<AtnHandle> { h };
                seq.AddRange(commandHandles);
                h = ElemList(seq);
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

        if (lexerAtom != null)
            h = WalkLexerAtom(lexerAtom);
        else if (lexerBlock != null)
            h = WalkLexerBlock(lexerBlock);

        var suffix = Children(lexerElement).FirstOrDefault(c => c.LocalName == "ebnfSuffix");
        if (h != null && suffix != null)
            h = ApplySuffix(lexerElement, suffix, WrapInBlock(h));

        return h ?? MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerAtom(UnvParseTreeElement lexerAtom)
    {
        // lexerAtom : characterRange
        //           | terminalDef
        //           | referenceModifier? ruleref  (fragment reference)
        //           | notSet
        //           | LEXER_CHAR_SET
        //           | DOT elementOptions?

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

        // DOT = wildcard
        var dot = Children(lexerAtom).FirstOrDefault(c => IsTerminal(c) && GetText(c).Trim() == ".");
        if (dot != null) return MakeWildcard();

        return MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerTerminalDef(UnvParseTreeElement terminalDef)
    {
        // In lexer rules: STRING_LITERAL → char sequence; TOKEN_REF 'EOF' → AtomTransition(-1)
        var tokenRef = ChildTerminal(terminalDef, "TOKEN_REF");
        if (tokenRef != null)
        {
            var name = GetText(tokenRef).Trim();
            return MakeLexerTokenRef(name);
        }
        var strLit = ChildTerminal(terminalDef, "STRING_LITERAL");
        if (strLit != null)
        {
            var lit = GetText(strLit).Trim();
            return MakeCharSequence(lit);
        }
        return MakeEpsilonHandle();
    }

    private AtnHandle WalkLexerBlock(UnvParseTreeElement lexerBlock)
    {
        // lexerBlock : LPAREN lexerAltList RPAREN
        var altList = Child(lexerBlock, "lexerAltList");
        if (altList == null) return MakeEpsilonHandle();

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
        return MakeBlock(lexerBlock, alts, null);
    }

    private AtnHandle WalkCharacterRange(UnvParseTreeElement characterRange)
    {
        // characterRange : STRING_LITERAL RANGE STRING_LITERAL
        var literals = Children(characterRange)
            .Where(c => IsTerminal(c) && c.LocalName == "STRING_LITERAL")
            .ToList();
        if (literals.Count < 2) return MakeEpsilonHandle();

        var fromChar = CharValue(GetText(literals[0]).Trim());
        var toChar = CharValue(GetText(literals[1]).Trim());
        if (fromChar < 0 || toChar < 0) return MakeEpsilonHandle();

        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new RangeTransition(right, fromChar, toChar));
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
                if (a >= 0 && b >= 0) set.Add(a, b);
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
            if (c >= 0) set.Add(c);
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
            states[j].AddTransition(new AtomTransition(states[j + 1], chars[j]));

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
        // charSetText looks like "[a-zA-Z_]" or "[^\n]"
        var s = charSetText;
        // Strip surrounding brackets.
        if (s.Length >= 2 && s[0] == '[') s = s[1..];
        if (s.Length >= 1 && s[^1] == ']') s = s[..^1];

        var set = new IntervalSet();
        bool negate = false;
        int idx = 0;

        if (idx < s.Length && s[idx] == '^')
        {
            negate = true;
            idx++;
        }

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

        if (negate)
        {
            var complement = set.Complement(IntervalSet.Of(0, 0xFFFF));
            return complement;
        }
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
}
