using System;
using System.Collections.Generic;
using System.Linq;
using ParseTreeEditing.UnvParseTreeDOM;
using static trinterp.GrammarParser;

namespace trinterp;

/// <summary>
/// Builds a parser ATN from a <see cref="GrammarModel"/>.
/// Translated from antlr-ng's ParserATNFactory.ts and the ATNBuilder tree-walker.
/// </summary>
public class ParserAtnFactory
{
    protected readonly GrammarModel _grammar;
    protected readonly ATN _atn;
    protected RuleModel _currentRule;
    protected int _currentOuterAlt;

    // For post-construction epsilon-closure checks.
    protected readonly List<(RuleModel, ATNState, ATNState)> _preventEpsilonClosureBlocks = new();
    protected readonly List<(RuleModel, ATNState, ATNState)> _preventEpsilonOptionalBlocks = new();

    // Global sempred counter (incremented as predicates are encountered).
    protected int _nextPredIndex;

    public ParserAtnFactory(GrammarModel grammar)
    {
        _grammar = grammar;
        var atnType = grammar.IsLexer ? ATNType.Lexer : ATNType.Parser;
        _atn = new ATN(atnType, grammar.GetMaxTokenType());
    }

    // =========================================================================
    // Public entry point
    // =========================================================================

    public virtual ATN CreateATN()
    {
        CreateRuleStartStopStates();
        foreach (var rule in _grammar.Rules)
            BuildRule(rule);
        AddRuleFollowLinks();
        AddEOFTransitionToStartRules();
        AtnOptimizer.OptimizeStates(_atn);
        return _atn;
    }

    // =========================================================================
    // Rule-level construction
    // =========================================================================

    protected void CreateRuleStartStopStates()
    {
        int n = _grammar.Rules.Count;
        _atn.ruleToStartState = new RuleStartState[n];
        _atn.ruleToStopState = new RuleStopState[n];
        foreach (var rule in _grammar.Rules)
        {
            var start = NewState<RuleStartState>(rule);
            var stop = NewState<RuleStopState>(rule);
            start.stopState = stop;
            _atn.ruleToStartState[rule.Index] = start;
            _atn.ruleToStopState[rule.Index] = stop;
        }
    }

    protected void BuildRule(RuleModel rule)
    {
        _currentRule = rule;
        _currentOuterAlt = 0;

        if (rule.BodyNode == null && rule.ImplicitLiteral == null) return;

        // ruleBlock -> ruleAltList  (parser)
        // lexerRuleBlock -> lexerAltList  (lexer, handled in subclass)
        var blk = WalkRuleBody(rule.BodyNode);
        if (blk == null) return;

        ConnectRuleBody(rule, blk);
    }

    protected void ConnectRuleBody(RuleModel rule, AtnHandle blk)
    {
        var start = _atn.ruleToStartState[rule.Index];
        var stop = _atn.ruleToStopState[rule.Index];
        AddEpsilon(start, blk.Left);
        AddEpsilon(blk.Right, stop);
    }

    // =========================================================================
    // Body walking (ruleBlock / ruleAltList / labeledAlt / alternative / element)
    // =========================================================================

    protected virtual AtnHandle WalkRuleBody(UnvParseTreeElement bodyNode)
    {
        // bodyNode is "ruleBlock" which contains "ruleAltList"
        var altList = Child(bodyNode, "ruleAltList");
        if (altList == null) altList = Child(bodyNode, "altList"); // block alt list
        if (altList == null) return MakeEpsilonHandle();

        return WalkAltList(altList);
    }

    protected AtnHandle WalkAltList(UnvParseTreeElement altListNode)
    {
        // ruleAltList : labeledAlt (OR labeledAlt)*
        // altList     : alternative (OR alternative)*
        var alts = new List<AtnHandle>();
        int altIdx = 0;
        foreach (var child in Children(altListNode))
        {
            if (IsTerminal(child)) continue; // skip OR tokens
            _currentOuterAlt = altIdx++;
            AtnHandle h;
            if (child.LocalName == "labeledAlt")
                h = WalkLabeledAlt(child);
            else if (child.LocalName == "alternative")
                h = WalkAlternative(child);
            else
                continue;
            if (h != null) alts.Add(h);
        }
        return MakeBlock(altListNode, alts, null);
    }

    private AtnHandle WalkLabeledAlt(UnvParseTreeElement labeledAlt)
    {
        // labeledAlt : alternative (POUND identifier)?
        var alt = Child(labeledAlt, "alternative");
        return alt != null ? WalkAlternative(alt) : MakeEpsilonHandle();
    }

    protected AtnHandle WalkAlternative(UnvParseTreeElement altNode)
    {
        // alternative : elementOptions? element+  |  (empty)
        var elements = new List<AtnHandle>();
        foreach (var child in Children(altNode))
        {
            if (child.LocalName == "element")
            {
                var h = WalkElement(child);
                if (h != null) elements.Add(h);
            }
        }
        if (elements.Count == 0)
            return MakeEpsilonHandle();
        return ElemList(elements);
    }

    protected virtual AtnHandle WalkElement(UnvParseTreeElement element)
    {
        // element : labeledElement (ebnfSuffix |)
        //         | atom (ebnfSuffix |)
        //         | ebnf
        //         | actionBlock QUESTION? predicateOptions?

        var actionBlock = Child(element, "actionBlock");
        if (actionBlock != null)
        {
            bool isPred = Children(element).Any(c => IsTerminal(c) && GetText(c).Trim() == "?");
            return isPred ? MakeSemPred(element, actionBlock) : MakeAction(actionBlock);
        }

        var ebnf = Child(element, "ebnf");
        if (ebnf != null)
            return WalkEbnf(ebnf);

        // labeled element wraps an atom or block
        var labeledElement = Child(element, "labeledElement");
        UnvParseTreeElement atomOrBlock = labeledElement != null
            ? (Child(labeledElement, "atom") ?? (UnvParseTreeElement)Child(labeledElement, "block"))
            : (Child(element, "atom") ?? (UnvParseTreeElement)Child(element, "block"));

        // atom or block
        AtnHandle h = null;
        if (atomOrBlock?.LocalName == "atom")
            h = WalkAtom(atomOrBlock);
        else if (atomOrBlock?.LocalName == "block")
            h = WalkBlock(atomOrBlock, null);

        // optional ebnfSuffix
        var suffix = Child(element, "ebnfSuffix") ?? (labeledElement != null ? Child(element, "ebnfSuffix") : null);
        if (suffix == null && labeledElement != null)
            suffix = Child(element, "ebnfSuffix");
        // find ebnfSuffix among direct children
        suffix = Children(element).FirstOrDefault(c => c.LocalName == "ebnfSuffix");

        if (h != null && suffix != null)
            h = ApplySuffix(element, suffix, WrapInBlock(h, GetText(suffix).Trim()));

        return h ?? MakeEpsilonHandle();
    }

    protected AtnHandle WrapInBlock(AtnHandle h, string suffixText = null)
    {
        // Create the correct BlockStartState subtype based on the ebnf suffix so that
        // MakeStar / MakePlus receive the expected concrete type.
        BlockStartState start = suffixText switch
        {
            string s when s.StartsWith("*") => NewState<StarBlockStartState>(),
            string s when s.StartsWith("+") => NewState<PlusBlockStartState>(),
            _                               => NewState<BasicBlockStartState>(),
        };
        var end = NewState<BlockEndState>();
        start.endState = end;
        end.startState = start;
        AddEpsilon(start, h.Left);
        AddEpsilon(h.Right, end);
        return new AtnHandle(start, end);
    }

    private AtnHandle WalkEbnf(UnvParseTreeElement ebnf)
    {
        // ebnf : block blockSuffix?
        var block = Child(ebnf, "block");
        var suffix = Child(ebnf, "blockSuffix");
        var ebnfSuffix = suffix != null ? Child(suffix, "ebnfSuffix") : null;
        if (block == null) return MakeEpsilonHandle();
        return WalkBlock(block, ebnfSuffix);
    }

    protected AtnHandle WalkBlock(UnvParseTreeElement blockNode, UnvParseTreeElement ebnfSuffix)
    {
        // block : LPAREN (optionsSpec? ruleAction* COLON)? altList RPAREN
        var altList = Child(blockNode, "altList");
        if (altList == null) return MakeEpsilonHandle();

        var alts = new List<AtnHandle>();
        int altIdx = 0;
        foreach (var child in Children(altList))
        {
            if (IsTerminal(child)) continue;
            if (child.LocalName == "alternative")
            {
                _currentOuterAlt = altIdx++;
                var h = WalkAlternative(child);
                if (h != null) alts.Add(h);
            }
        }
        return MakeBlock(blockNode, alts, ebnfSuffix);
    }

    protected virtual AtnHandle WalkAtom(UnvParseTreeElement atom)
    {
        // atom : terminalDef | ruleref | notSet | wildcard
        var terminal = Child(atom, "terminalDef");
        if (terminal != null) return WalkTerminalDef(terminal, atom);

        var ruleref = Child(atom, "ruleref");
        if (ruleref != null) return WalkRuleRef(ruleref);

        var notSet = Child(atom, "notSet");
        if (notSet != null) return WalkNotSet(notSet);

        var wildcard = Child(atom, "wildcard");
        if (wildcard != null) return MakeWildcard();

        return MakeEpsilonHandle();
    }

    protected AtnHandle WalkTerminalDef(UnvParseTreeElement terminalDef, UnvParseTreeElement ctx)
    {
        // terminalDef : TOKEN_REF elementOptions? | STRING_LITERAL elementOptions?
        var tokenRef = ChildTerminal(terminalDef, "TOKEN_REF");
        if (tokenRef != null)
        {
            var name = GetText(tokenRef).Trim();
            return MakeTokenRef(name);
        }
        var strLit = ChildTerminal(terminalDef, "STRING_LITERAL");
        if (strLit != null)
        {
            var lit = GetText(strLit).Trim();
            return MakeStringLiteral(lit);
        }
        return MakeEpsilonHandle();
    }

    protected AtnHandle WalkRuleRef(UnvParseTreeElement ruleref)
    {
        // ruleref : RULE_REF argActionBlock? elementOptions?
        var nameNode = ChildTerminal(ruleref, "RULE_REF");
        if (nameNode == null) return null;
        var name = GetText(nameNode).Trim();
        return MakeRuleRef(name);
    }

    protected AtnHandle WalkNotSet(UnvParseTreeElement notSet)
    {
        // notSet : NOT setElement | NOT blockSet
        var setElement = Child(notSet, "setElement");
        var blockSet = Child(notSet, "blockSet");

        var set = new IntervalSet();
        if (setElement != null) AddSetElement(set, setElement);
        if (blockSet != null)
            foreach (var se in Children(blockSet, "setElement"))
                AddSetElement(set, se);

        return MakeNotSet(set);
    }

    private void AddSetElement(IntervalSet set, UnvParseTreeElement se)
    {
        var tokenRef = ChildTerminal(se, "TOKEN_REF");
        if (tokenRef != null)
        {
            var tt = GetTokenType(GetText(tokenRef).Trim());
            set.Add(tt);
            return;
        }
        var strLit = ChildTerminal(se, "STRING_LITERAL");
        if (strLit != null)
        {
            var tt = GetTokenType(GetText(strLit).Trim());
            if (tt > 0) set.Add(tt);
            return;
        }
        var charRange = Child(se, "characterRange");
        if (charRange != null)
        {
            var literals = Children(charRange).Where(c => IsTerminal(c) && c.LocalName == "STRING_LITERAL").ToList();
            if (literals.Count >= 2)
            {
                var a = CharValue(GetText(literals[0]).Trim());
                var b = CharValue(GetText(literals[1]).Trim());
                if (a >= 0 && b >= 0) set.Add(a, b);
            }
        }
    }

    // =========================================================================
    // Block construction (from alternatives)
    // =========================================================================

    protected AtnHandle MakeBlock(UnvParseTreeElement blkCtx, List<AtnHandle> alts, UnvParseTreeElement ebnfSuffix)
    {
        if (alts.Count == 0) return MakeEpsilonHandle();

        if (ebnfSuffix == null)
        {
            if (alts.Count == 1) return alts[0];
            var start = NewState<BasicBlockStartState>();
            _atn.DefineDecisionState(start);
            return ConnectBlock(start, alts);
        }

        var suffixText = GetText(ebnfSuffix).Trim();
        if (suffixText.StartsWith("?"))
        {
            var start = NewState<BasicBlockStartState>();
            _atn.DefineDecisionState(start);
            var h = ConnectBlock(start, alts);
            return MakeOptional(ebnfSuffix, h, IsNonGreedy(suffixText));
        }
        if (suffixText.StartsWith("*"))
        {
            var star = NewState<StarBlockStartState>();
            if (alts.Count > 1) _atn.DefineDecisionState(star);
            var h = ConnectBlock(star, alts);
            return MakeStar(ebnfSuffix, h, IsNonGreedy(suffixText));
        }
        if (suffixText.StartsWith("+"))
        {
            var plus = NewState<PlusBlockStartState>();
            if (alts.Count > 1) _atn.DefineDecisionState(plus);
            var h = ConnectBlock(plus, alts);
            return MakePlus(ebnfSuffix, h, IsNonGreedy(suffixText));
        }

        return alts.Count == 1 ? alts[0] : ConnectBlock(NewState<BasicBlockStartState>(), alts);
    }

    protected AtnHandle ApplySuffix(UnvParseTreeElement ctx, UnvParseTreeElement suffix, AtnHandle blk)
    {
        var suffixText = GetText(suffix).Trim();
        if (suffixText.StartsWith("?")) return MakeOptional(suffix, blk, IsNonGreedy(suffixText));
        if (suffixText.StartsWith("*")) return MakeStar(suffix, blk, IsNonGreedy(suffixText));
        if (suffixText.StartsWith("+")) return MakePlus(suffix, blk, IsNonGreedy(suffixText));
        return blk;
    }

    private static bool IsNonGreedy(string suffix) => suffix.Length > 1 && suffix[1] == '?';

    private AtnHandle ConnectBlock(BlockStartState start, List<AtnHandle> alts)
    {
        var end = NewState<BlockEndState>();
        start.endState = end;
        end.startState = start;
        foreach (var alt in alts)
        {
            AddEpsilon(start, alt.Left);
            AddEpsilon(alt.Right, end);
            new TailEpsilonRemover(_atn).Visit(alt.Left);
        }
        _preventEpsilonClosureBlocks.Add((_currentRule, start, end));
        return new AtnHandle(start, end);
    }

    protected AtnHandle MakeOptional(UnvParseTreeElement ctx, AtnHandle blk, bool nonGreedy)
    {
        var blkStart = (BlockStartState)blk.Left;
        if (nonGreedy) blkStart.nonGreedy = true;
        AddEpsilon(blkStart, blk.Right, prepend: nonGreedy);
        _preventEpsilonOptionalBlocks.Add((_currentRule, blkStart, blk.Right));
        return blk;
    }

    protected AtnHandle MakeStar(UnvParseTreeElement ctx, AtnHandle blk, bool nonGreedy)
    {
        var blkStart = (StarBlockStartState)blk.Left;
        var blkEnd = (BlockEndState)blk.Right;
        _preventEpsilonClosureBlocks.Add((_currentRule, blkStart, blkEnd));

        var entry = NewState<StarLoopEntryState>();
        entry.nonGreedy = nonGreedy;
        _atn.DefineDecisionState(entry);
        var end = NewState<LoopEndState>();
        var loop = NewState<StarLoopbackState>();
        entry.loopBackState = loop;
        end.loopBackState = loop;

        if (!nonGreedy)
        {
            AddEpsilon(entry, blkStart);
            AddEpsilon(entry, end);
        }
        else
        {
            AddEpsilon(entry, end);
            AddEpsilon(entry, blkStart);
        }
        AddEpsilon(blkEnd, loop);
        AddEpsilon(loop, entry);
        return new AtnHandle(entry, end);
    }

    protected AtnHandle MakePlus(UnvParseTreeElement ctx, AtnHandle blk, bool nonGreedy)
    {
        var blkStart = (PlusBlockStartState)blk.Left;
        var blkEnd = (BlockEndState)blk.Right;
        _preventEpsilonClosureBlocks.Add((_currentRule, blkStart, blkEnd));

        var loop = NewState<PlusLoopbackState>();
        loop.nonGreedy = nonGreedy;
        _atn.DefineDecisionState(loop);
        var end = NewState<LoopEndState>();
        blkStart.loopBackState = loop;
        end.loopBackState = loop;

        AddEpsilon(blkEnd, loop);
        if (!nonGreedy)
        {
            AddEpsilon(loop, blkStart);
            AddEpsilon(loop, end);
        }
        else
        {
            AddEpsilon(loop, end);
            AddEpsilon(loop, blkStart);
        }
        return new AtnHandle(blkStart, end);
    }

    // =========================================================================
    // Atom handles
    // =========================================================================

    protected virtual AtnHandle MakeTokenRef(string name)
    {
        int tt = GetTokenType(name);
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new AtomTransition(right, tt));
        return new AtnHandle(left, right);
    }

    protected virtual AtnHandle MakeStringLiteral(string lit)
    {
        // In a parser grammar a string literal is just a token atom.
        int tt = GetStringLiteralType(lit);
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new AtomTransition(right, tt));
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeRuleRef(string name)
    {
        var rule = _grammar.GetRule(name);
        if (rule == null) return MakeEpsilonHandle();
        var ruleStart = _atn.ruleToStartState[rule.Index];
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new RuleTransition(ruleStart, rule.Index, 0, right));
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeWildcard()
    {
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new WildcardTransition(right));
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeNotSet(IntervalSet set)
    {
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new NotSetTransition(right, set));
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeSet(IntervalSet set)
    {
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        if (set.Count == 1)
        {
            var interval = set.GetIntervals()[0];
            if (interval.a == interval.b)
                left.AddTransition(new AtomTransition(right, interval.a));
            else
                left.AddTransition(new RangeTransition(right, interval.a, interval.b));
        }
        else
        {
            left.AddTransition(new SetTransition(right, set));
        }
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeEpsilonHandle()
    {
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        AddEpsilon(left, right);
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeAction(UnvParseTreeElement actionBlock)
    {
        var text = GetText(actionBlock).Trim();
        // Strip outer braces for the text we store.
        var rawText = text.Length >= 2 && text[0] == '{' && text[^1] == '}'
            ? text[1..^1].Trim() : text;

        var ruleIndex = _currentRule?.Index ?? -1;
        var actionIndex = _currentRule?.Actions.Count ?? 0;

        var info = new ActionInfo { RuleIndex = ruleIndex, ActionIndex = actionIndex, Text = text };
        _currentRule?.Actions.Add(info);
        _grammar.SemPreds.Count.ToString(); // touch

        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new ActionTransition(right, ruleIndex, actionIndex, false));
        return new AtnHandle(left, right);
    }

    protected AtnHandle MakeSemPred(UnvParseTreeElement element, UnvParseTreeElement actionBlock)
    {
        var text = GetText(actionBlock).Trim();
        // Strip braces: {expr}? → expr
        var rawText = text;
        if (rawText.StartsWith("{") && rawText.EndsWith("}"))
            rawText = rawText[1..^1].Trim();

        var ruleIndex = _currentRule?.Index ?? -1;
        var predIndex = _nextPredIndex++;

        var info = new SemPredInfo
        {
            RuleIndex = ruleIndex,
            PredIndex = predIndex,
            IsCtxDependent = true, // conservative
            Text = rawText
        };
        _currentRule?.SemPreds.Add(info);
        _grammar.SemPreds.Add(info);

        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new PredicateTransition(right, ruleIndex, predIndex, true));
        return new AtnHandle(left, right);
    }

    // =========================================================================
    // Sequence linking (elemList)
    // =========================================================================

    protected AtnHandle ElemList(List<AtnHandle> elements)
    {
        int n = elements.Count;
        for (int i = 0; i < n - 1; i++)
        {
            var el = elements[i];
            var next = elements[i + 1];

            // Optimise: if el is a simple o-x->o and next starts right after,
            // re-wire the transition target instead of adding an epsilon.
            Transition tr = null;
            if (el.Left.NumberOfTransitions == 1)
                tr = el.Left.Transition(0);

            bool isRule = tr is RuleTransition;
            bool canSkip = el.Left.StateType == StateType.Basic
                && el.Right != null && el.Right.StateType == StateType.Basic
                && tr != null
                && (isRule
                    ? ((RuleTransition)tr).followState == el.Right
                    : tr.target == el.Right);

            if (canSkip)
            {
                if (isRule)
                    ((RuleTransition)tr).followState = next.Left;
                else
                    tr.target = next.Left;
                _atn.RemoveState(el.Right);
            }
            else
            {
                AddEpsilon(el.Right, next.Left);
            }
        }
        return new AtnHandle(elements[0].Left, elements[^1].Right);
    }

    // =========================================================================
    // Post-construction wiring
    // =========================================================================

    protected void AddRuleFollowLinks()
    {
        foreach (var state in _atn.states)
        {
            if (state == null) continue;
            if (state.StateType != StateType.Basic) continue;
            if (state.NumberOfTransitions != 1) continue;
            if (!(state.Transition(0) is RuleTransition rt)) continue;
            var stop = _atn.ruleToStopState[rt.target.ruleIndex];
            AddEpsilon(stop, rt.followState);
        }
    }

    protected void AddEOFTransitionToStartRules()
    {
        var eofTarget = NewState<BasicState>();
        foreach (var rule in _grammar.Rules)
        {
            var stop = _atn.ruleToStopState[rule.Index];
            if (stop.NumberOfTransitions > 0) continue;
            stop.AddTransition(new AtomTransition(eofTarget, TokenConstants.EOF));
        }
    }

    // =========================================================================
    // Token type lookup
    // =========================================================================

    protected int GetTokenType(string name)
    {
        if (name == "EOF") return TokenConstants.EOF;
        if (_grammar.TokenNameToType.TryGetValue(name, out var tt)) return tt;
        // Assign on-demand for parser grammars (token types may be unknown).
        int next = _grammar.GetMaxTokenType() + 1;
        _grammar.TokenNameToType[name] = next;
        return next;
    }

    protected int GetStringLiteralType(string lit)
    {
        if (_grammar.StringLiteralToType.TryGetValue(lit, out var tt)) return tt;
        int next = _grammar.GetMaxTokenType() + 1;
        _grammar.StringLiteralToType[lit] = next;
        return next;
    }

    // =========================================================================
    // State helpers
    // =========================================================================

    protected T NewState<T>() where T : ATNState, new()
    {
        var s = new T();
        s.ruleIndex = _currentRule?.Index ?? -1;
        _atn.AddState(s);
        return s;
    }

    protected T NewState<T>(RuleModel rule) where T : ATNState, new()
    {
        var s = new T();
        s.ruleIndex = rule?.Index ?? -1;
        _atn.AddState(s);
        return s;
    }

    protected void AddEpsilon(ATNState from, ATNState to, bool prepend = false)
    {
        if (from == null) return;
        if (prepend)
            from.AddTransition(0, new EpsilonTransition(to));
        else
            from.AddTransition(new EpsilonTransition(to));
    }

    // =========================================================================
    // Character value helper
    // =========================================================================

    protected static int CharValue(string grammarLiteral)
    {
        // Grammar char literals are single-quoted: 'a', '\n', '\u0041'
        var s = grammarLiteral;
        if (s.Length >= 2 && s[0] == '\'') s = s[1..];
        if (s.Length >= 1 && s[^1] == '\'') s = s[..^1];
        if (s.Length == 0) return -1;
        if (s[0] != '\\') return s[0];
        if (s.Length < 2) return -1;
        return s[1] switch
        {
            'n' => '\n', 'r' => '\r', 't' => '\t', 'b' => '\b',
            'f' => '\f', '\\' => '\\', '\'' => '\'', '"' => '"',
            'u' when s.Length >= 6 => int.Parse(s[2..6], System.Globalization.NumberStyles.HexNumber),
            _ => s[1]
        };
    }
}
