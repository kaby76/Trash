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
    protected readonly OptimizeOptions _optimize;
    protected RuleModel _currentRule;
    protected int _currentOuterAlt;

    // For post-construction epsilon-closure checks.
    protected readonly List<(RuleModel, ATNState, ATNState)> _preventEpsilonClosureBlocks = new();
    protected readonly List<(RuleModel, ATNState, ATNState)> _preventEpsilonOptionalBlocks = new();

    // Global sempred counter (incremented as predicates are encountered).
    protected int _nextPredIndex;

    // Source location of the grammar construct currently being translated into states.
    // Set by SetSrc() / SetSrcRange() before any NewState<T>() call.
    // _srcLine/_srcCol    = start of the grammar element (stamped on every new state).
    // _srcEndLine/_srcEndCol = exclusive end of the grammar element (stamped only on match
    //                          states; -1 for structural states like block-starts/-ends).
    private int _srcLine    = -1;
    private int _srcCol     = -1;
    private int _srcEndLine = -1;
    private int _srcEndCol  = -1;


    public ParserAtnFactory(GrammarModel grammar, OptimizeOptions optimize = null)
    {
        _grammar  = grammar;
        _optimize = optimize ?? OptimizeOptions.All;
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
        // antlr4 does not apply OptimizeSets to parser grammars or parser rules;
        // parser codegen does not support SetTransition. Only LexerAtnFactory calls it.
        if (_optimize.Any)         AtnOptimizer.OptimizeStates(_atn);
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
            SetSrc(rule.SourceLine, rule.SourceColumn);
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

        if (IsImmediatelyLeftRecursive(rule))
        {
            BuildLeftRecursiveRule(rule);
            return;
        }

        // ruleBlock -> ruleAltList  (parser)
        // lexerRuleBlock -> lexerAltList  (lexer, handled in subclass)
        var blk = WalkRuleBody(rule.BodyNode);
        if (blk == null) return;
        ConnectRuleBody(rule, blk);
    }

    // =========================================================================
    // Left-recursive rule detection and transformation
    // =========================================================================

    /// <summary>
    /// True if any outer alt of rule has a direct self-reference as its first element.
    /// </summary>
    private bool IsImmediatelyLeftRecursive(RuleModel rule)
    {
        if (rule.BodyNode == null) return false;
        var altList = Child(rule.BodyNode, "ruleAltList") ?? Child(rule.BodyNode, "altList");
        if (altList == null) return false;
        foreach (var child in Children(altList))
        {
            if (IsTerminal(child)) continue;
            var elements = GetAltElementNodes(child);
            if (elements.Count > 0 && IsDirectSelfRef(elements[0], rule.Name))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the top-level element nodes for a labeledAlt or alternative node.
    /// </summary>
    private static List<UnvParseTreeElement> GetAltElementNodes(UnvParseTreeElement altOrLabeled)
    {
        var altNode = altOrLabeled.LocalName == "labeledAlt"
            ? Child(altOrLabeled, "alternative")
            : altOrLabeled;
        if (altNode == null) return new List<UnvParseTreeElement>();
        return Children(altNode)
            .Where(c => !IsTerminal(c) && c.LocalName == "element")
            .ToList();
    }

    /// <summary>
    /// True if element is a direct (possibly labeled) atom ruleref to ruleName,
    /// without any ebnfSuffix on the element.
    /// </summary>
    private static bool IsDirectSelfRef(UnvParseTreeElement element, string ruleName)
    {
        // Reject ebnfSuffix (name*, name+, name?)
        if (Children(element).Any(c => !IsTerminal(c) && c.LocalName == "ebnfSuffix"))
            return false;
        // Reject ebnf block or action/predicate
        if (Child(element, "ebnf") != null) return false;
        if (Child(element, "actionBlock") != null) return false;

        var labeled = Child(element, "labeledElement");
        var atom = labeled != null ? Child(labeled, "atom") : Child(element, "atom");
        if (atom == null) return false;

        var ruleref = Child(atom, "ruleref");
        if (ruleref == null) return false;

        var nameNode = ChildTerminal(ruleref, "RULE_REF");
        return nameNode != null && GetText(nameNode).Trim() == ruleName;
    }

    /// <summary>
    /// True if the last non-epsilon top-level element (excluding the first element)
    /// is also a direct self-reference — the ANTLR4 "binary" pattern.
    /// </summary>
    private static bool IsBinaryAlt(List<UnvParseTreeElement> allElements, string ruleName)
    {
        for (int i = allElements.Count - 1; i >= 1; i--)
        {
            if (IsEpsilonElement(allElements[i])) continue;
            return IsDirectSelfRef(allElements[i], ruleName);
        }
        return false;
    }

    /// <summary>
    /// True if the last non-epsilon element of a primary (non-LR) alt is a direct self-ref.
    /// E.g. `'!' expression` → true; `'{' zelist '}'` → false.
    /// </summary>
    private static bool PrimaryAltEndsWithSelfRef(List<UnvParseTreeElement> elements, string ruleName)
    {
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            if (IsEpsilonElement(elements[i])) continue;
            return IsDirectSelfRef(elements[i], ruleName);
        }
        return false;
    }

    /// <summary>
    /// Walks a primary alt whose last element is a direct self-ref.
    /// All elements except the rightmost self-ref use WalkElement (prec=0);
    /// the rightmost self-ref uses WalkSelfRefWithPrec(trailingPrec).
    /// </summary>
    private AtnHandle WalkPrimaryAltTrailingSelfRef(
        List<UnvParseTreeElement> elements, string ruleName, int trailingPrec)
    {
        int rightmostIdx = -1;
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            if (!IsEpsilonElement(elements[i]) && IsDirectSelfRef(elements[i], ruleName))
            { rightmostIdx = i; break; }
        }
        var handles = new List<AtnHandle>();
        for (int i = 0; i < elements.Count; i++)
        {
            AtnHandle h = (i == rightmostIdx)
                ? WalkSelfRefWithPrec(elements[i], trailingPrec)
                : WalkElement(elements[i]);
            if (h != null) handles.Add(h);
        }
        if (handles.Count == 0) return null;
        if (handles.Count == 1) return handles[0];
        return ElemList(handles);
    }

    /// <summary>Element is epsilon-like (action or sempred).</summary>
    private static bool IsEpsilonElement(UnvParseTreeElement element)
        => Child(element, "actionBlock") != null;

    /// <summary>True if the alt carries assoc=right in its elementOptions.</summary>
    private static bool IsAltRightAssoc(UnvParseTreeElement altOrLabeled)
    {
        var altNode = altOrLabeled.LocalName == "labeledAlt"
            ? Child(altOrLabeled, "alternative")
            : altOrLabeled;
        if (altNode == null) return false;
        var opts = Child(altNode, "elementOptions");
        if (opts == null) return false;
        foreach (var opt in Children(opts))
        {
            if (IsTerminal(opt)) continue;
            // Look for assign nodes: children should be ID(assoc) and ID(right)
            // elementOption children: id (non-terminal), '=' (terminal), id (non-terminal)
            // Filter for non-terminals to get the id texts "assoc" and "right".
            var ids = Children(opt).Where(c => !IsTerminal(c)).Select(c => GetText(c).Trim()).ToList();
            if (ids.Count >= 2 && ids[0] == "assoc" && ids[1] == "right")
                return true;
        }
        return false;
    }

    /// <summary>
    /// Builds the precedence-climbing ATN for a directly left-recursive rule,
    /// matching ANTLR4's LeftRecursiveRuleTransformer output.
    ///
    /// Transformed structure:
    ///   ( {} primary1 | primary2 | ... )
    ///   ( {prec_n >= _p}?  op_n_body | ... )*
    /// </summary>
    private void BuildLeftRecursiveRule(RuleModel rule)
    {
        _atn.ruleToStartState[rule.Index].isLeftRecursiveRule = true;

        var altList = Child(rule.BodyNode, "ruleAltList") ?? Child(rule.BodyNode, "altList");
        if (altList == null) return;

        // Collect outer alts with their 1-based original indices.
        var allAlts = new List<(UnvParseTreeElement node, int origIndex)>();
        int idx = 1;
        foreach (var child in Children(altList))
        {
            if (IsTerminal(child)) continue;
            if (child.LocalName is "labeledAlt" or "alternative")
                allAlts.Add((child, idx++));
        }
        int numAlts = allAlts.Count;

        // Classify: primary (non-LR) vs operator (LR).
        var primaryAlts  = new List<(UnvParseTreeElement node, int origIndex)>();
        var operatorAlts = new List<(UnvParseTreeElement node, int origIndex)>();
        foreach (var alt in allAlts)
        {
            var elems = GetAltElementNodes(alt.node);
            if (elems.Count > 0 && IsDirectSelfRef(elems[0], rule.Name))
                operatorAlts.Add(alt);
            else
                primaryAlts.Add(alt);
        }

        // ── 1. Build primary alt handles ──────────────────────────────────────
        // Walk primary alts first so any explicit predicates consume pred indices
        // before the wasted prec-pred indices are reserved.
        var primaryHandles = new List<AtnHandle>();
        for (int i = 0; i < primaryAlts.Count; i++)
        {
            _currentOuterAlt = primaryAlts[i].origIndex - 1;
            var an = primaryAlts[i].node;
            // ANTLR4: when a primary alt's LAST element is a direct self-ref,
            // that call uses the alt's reversed prec instead of 0
            // (e.g. `'!' expression` → expression[reversedPrec]).
            // Alts where self-refs are in the middle (e.g. `'{' zelist '}'`) use 0.
            var elems = GetAltElementNodes(an);
            int prec = numAlts - primaryAlts[i].origIndex + 1;
            AtnHandle h = PrimaryAltEndsWithSelfRef(elems, rule.Name)
                ? WalkPrimaryAltTrailingSelfRef(elems, rule.Name, prec)
                : (an.LocalName == "labeledAlt" ? WalkLabeledAlt(an) : WalkAlternative(an));
            if (i == 0)
            {
                // ANTLR4 prepends an empty {} action to the first primary alt.
                var actionH = MakeLrEmptyAction();
                h = ElemList(new List<AtnHandle> { actionH, h });
            }
            primaryHandles.Add(h);
        }

        // Build primary block. Single-alt case: no BasicBlockStartState (matches ANTLR4).
        AtnHandle primaryBlock;
        if (primaryHandles.Count == 1)
        {
            primaryBlock = primaryHandles[0];
        }
        else
        {
            var ps = NewState<BasicBlockStartState>();
            _atn.DefineDecisionState(ps);
            primaryBlock = ConnectBlock(ps, primaryHandles);
        }

        // ── 2. Build operator alt handles ─────────────────────────────────────
        // ANTLR4 orders: binary alts first (in grammar order), then suffix alts.
        // Binary = last top-level non-epsilon element is a direct self-reference.
        var opInfos = operatorAlts.Select(a =>
        {
            var elems = GetAltElementNodes(a.node);
            return (a.node, a.origIndex, elems, isBinary: IsBinaryAlt(elems, rule.Name));
        }).ToList();
        var orderedOps = opInfos.Where(x => x.isBinary)
                                .Concat(opInfos.Where(x => !x.isBinary))
                                .ToList();

        var opHandles = new List<AtnHandle>();
        foreach (var (an, origIndex, elems, isBinary) in orderedOps)
        {
            _currentOuterAlt = origIndex - 1;
            int prec = numAlts - origIndex + 1;
            bool isRightAssoc  = IsAltRightAssoc(an);
            int  nextPrec      = isRightAssoc ? prec : prec + 1;

            // Reserve the pred index ANTLR4 wastes on the prec sempred.
            _nextPredIndex++;

            var predH  = MakePrecedencePredicate(prec);
            var bodyH  = WalkLrOperatorBody(elems.Skip(1).ToList(), rule.Name, isBinary, nextPrec);
            AtnHandle altH = bodyH != null
                ? ElemList(new List<AtnHandle> { predH, bodyH })
                : predH;
            opHandles.Add(altH);
        }

        // ── 3. Operator star block ────────────────────────────────────────────
        // Decision is defined AFTER all op alt bodies are walked (inner decisions first).
        var starBlockStart = NewState<StarBlockStartState>();
        if (opHandles.Count > 1) _atn.DefineDecisionState(starBlockStart);
        var starBlock = ConnectBlock(starBlockStart, opHandles);

        // ── 4. Star loop (StarLoopEntry gets its decision last) ───────────────
        var starLoop = MakeStar(null, starBlock, nonGreedy: false);

        // ── 5. Wire up: rule-start → primaryBlock → starLoop → rule-stop ──────
        ConnectRuleBody(rule, ElemList(new List<AtnHandle> { primaryBlock, starLoop }));
    }

    /// <summary>Empty {} action that ANTLR4 prepends to the first primary alt.</summary>
    private AtnHandle MakeLrEmptyAction()
    {
        var ruleIndex = _currentRule?.Index ?? -1;
        var left  = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new ActionTransition(right, ruleIndex, -1, false));
        return new AtnHandle(left, right);
    }

    private AtnHandle MakePrecedencePredicate(int prec)
    {
        var left  = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new PrecedencePredicateTransition(right, prec));
        return new AtnHandle(left, right);
    }

    /// <summary>
    /// Walks the elements that follow the stripped leading self-ref in an operator alt.
    /// In binary alts the rightmost direct self-ref gets precedence nextPrec;
    /// all other rule refs to the same rule get precedence 0 (via normal MakeRuleRef).
    /// </summary>
    private AtnHandle WalkLrOperatorBody(
        List<UnvParseTreeElement> elements, string ruleName, bool isBinary, int nextPrec)
    {
        if (elements.Count == 0) return null;

        // Find rightmost direct self-ref for binary alts.
        int rightmostIdx = -1;
        if (isBinary)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                if (!IsEpsilonElement(elements[i]) && IsDirectSelfRef(elements[i], ruleName))
                { rightmostIdx = i; break; }
            }
        }

        var handles = new List<AtnHandle>();
        for (int i = 0; i < elements.Count; i++)
        {
            AtnHandle h = (i == rightmostIdx)
                ? WalkSelfRefWithPrec(elements[i], nextPrec)
                : WalkElement(elements[i]);
            if (h != null) handles.Add(h);
        }

        if (handles.Count == 0) return null;
        if (handles.Count == 1) return handles[0];
        return ElemList(handles);
    }

    /// <summary>
    /// Builds a RuleTransition to the self-ref rule with the given precedence,
    /// bypassing the default prec=0 in MakeRuleRef.
    /// </summary>
    private AtnHandle WalkSelfRefWithPrec(UnvParseTreeElement element, int prec)
    {
        var labeled    = Child(element, "labeledElement");
        var atom       = labeled != null ? Child(labeled, "atom") : Child(element, "atom");
        var ruleref    = atom != null ? Child(atom, "ruleref") : null;
        var nameNode   = ruleref != null ? ChildTerminal(ruleref, "RULE_REF") : null;
        if (nameNode == null) return WalkElement(element);

        var name = GetText(nameNode).Trim();
        var r    = _grammar.GetRule(name);
        if (r == null) return MakeEpsilonHandle();

        var ruleStart = _atn.ruleToStartState[r.Index];
        var left  = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new RuleTransition(ruleStart, r.Index, prec, right));
        return new AtnHandle(left, right);
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
        SetSrc(altNode);
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
        SetSrc(element);

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

        // For '?' the caller (MakeOptional) will add a bypass epsilon from start to end,
        // giving start 2 outgoing transitions — making it a genuine decision point.
        // Register it now so the decision index is set before transitions are added,
        // matching antlr4's ParserATNFactory behaviour for single-element optional blocks.
        if (suffixText != null && suffixText.StartsWith("?"))
            _atn.DefineDecisionState(start);

        AddEpsilon(start, h.Left);
        AddEpsilon(h.Right, end);
        if (_optimize.TailEpsilon)
            new TailEpsilonRemover(_atn).Visit(h.Left);
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
        SetSrc(blockNode);
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
            SetSrcRange(tokenRef, name.Length);
            return MakeTokenRef(name);
        }
        var strLit = ChildTerminal(terminalDef, "STRING_LITERAL");
        if (strLit != null)
        {
            var lit = GetText(strLit).Trim();
            SetSrcRange(strLit, lit.Length);
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
        SetSrcRange(nameNode, name.Length);
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

        // antlr4's BlockSetTransformer pre-processes the grammar AST to collapse
        // blocks where every alternative is a single atom/range/set into one SET
        // transition. Implement the equivalent here at construction time so we
        // never create a BasicBlockStartState / BlockEndState for such blocks.
        if (alts.Count > 1)
        {
            var reduced = TryReduceAltsToSet(alts);
            if (reduced != null) alts = new List<AtnHandle> { reduced };
        }

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

    /// <summary>
    /// If every alt is a single-atom handle (BasicState –Atom/Range/Set→ BasicState)
    /// with no intervening epsilon or rule call, collapse all of them into one
    /// MakeSet handle and remove the per-alt states.
    /// Returns null if the block is not eligible.
    /// </summary>
    private AtnHandle TryReduceAltsToSet(List<AtnHandle> alts)
    {
        var matchSet = new IntervalSet();
        foreach (var alt in alts)
        {
            if (alt.Left.StateType  != StateType.Basic) return null;
            if (alt.Right == null)                       return null;
            if (alt.Right.StateType != StateType.Basic)  return null;
            if (alt.Left.NumberOfTransitions != 1)       return null;
            var tr = alt.Left.Transition(0);
            if (tr.target != alt.Right)                  return null;
            // Only pure single-token atoms are eligible; range/set alts are left
            // for ATNOptimizer.OptimizeSets to merge post-construction (matching
            // ANTLR4's BlockSetTransformer which only considers atom alts).
            if (tr is AtomTransition at)  matchSet.Add(at.token);
            else return null;
        }

        // All alts are simple atoms — remove the per-alt states and return a
        // single handle using the most specific transition type.
        foreach (var alt in alts)
        {
            _atn.RemoveState(alt.Left);
            _atn.RemoveState(alt.Right);
        }
        var setLeft = NewState<BasicState>();
        var setRight = NewState<BasicState>();
        var ivs = matchSet.GetIntervals();
        Transition setTr;
        if (matchSet.ElementCount == 1)
            setTr = new AtomTransition(setRight, matchSet.MinElement);
        else if (ivs.Count == 1 && _grammar.IsLexer)
            setTr = new RangeTransition(setRight, ivs[0].a, ivs[0].b);
        else
            setTr = new SetTransition(setRight, matchSet);
        setLeft.AddTransition(setTr);
        return new AtnHandle(setLeft, setRight);
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
            if (_optimize.TailEpsilon)
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
        // antlr4's ParserATNFactory.set() always creates SetTransition regardless of set size.
        var left = NewState<BasicState>();
        var right = NewState<BasicState>();
        left.AddTransition(new SetTransition(right, set));
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
        // ANTLR4 uses -1 for all inline rule actions (unnamed/non-indexed).
        const int actionIndex = -1;

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
        s.ruleIndex       = _currentRule?.Index ?? -1;
        s.SourceLine      = _srcLine;
        s.SourceColumn    = _srcCol;
        s.SourceEndLine   = _srcEndLine;
        s.SourceEndColumn = _srcEndCol;
        _atn.AddState(s);
        return s;
    }

    protected T NewState<T>(RuleModel rule) where T : ATNState, new()
    {
        var s = new T();
        s.ruleIndex       = rule?.Index ?? -1;
        s.SourceLine      = _srcLine;
        s.SourceColumn    = _srcCol;
        s.SourceEndLine   = _srcEndLine;
        s.SourceEndColumn = _srcEndCol;
        _atn.AddState(s);
        return s;
    }

    /// <summary>
    /// Set the source location context (start only) that will be stamped onto the next NewState call(s).
    /// Clears any previously set end location.
    /// </summary>
    protected void SetSrc(int line, int col)
    {
        _srcLine    = line;
        _srcCol     = col;
        _srcEndLine = -1;
        _srcEndCol  = -1;
    }

    /// <summary>
    /// Set the source location context from a parse-tree node.
    /// Walks into the first reachable terminal if <paramref name="node"/> is a non-terminal.
    /// Clears any previously set end location.
    /// </summary>
    protected void SetSrc(UnvParseTreeElement node)
    {
        var (line, col) = SourceOf(node);
        _srcLine    = line;
        _srcCol     = col;
        _srcEndLine = -1;
        _srcEndCol  = -1;
    }

    /// <summary>
    /// Set both start and exclusive-end source location from a terminal token node.
    /// Call this instead of <see cref="SetSrc(UnvParseTreeElement)"/> when creating "match" states
    /// so that <see cref="StateLocationMap"/> can compute post-transition locations for the
    /// successor state via <c>DirectPostLocs</c>.
    /// </summary>
    protected void SetSrcRange(UnvParseTreeElement terminal, int textLength)
    {
        var line    = SafeGetLine(terminal);
        var col     = SafeGetColumn(terminal);
        _srcLine    = line;
        _srcCol     = col;
        _srcEndLine = line >= 0 ? line : -1;
        _srcEndCol  = col >= 0 ? col + textLength : -1;
    }

    /// <summary>
    /// Set start location from <paramref name="startTerminal"/> and exclusive-end location
    /// from the end of <paramref name="endTerminal"/> (its column + <paramref name="endTextLength"/>).
    /// Use for multi-token grammar elements like character ranges (<c>'a'..'z'</c>).
    /// </summary>
    protected void SetSrcRange(UnvParseTreeElement startTerminal, UnvParseTreeElement endTerminal, int endTextLength)
    {
        var startLine = SafeGetLine(startTerminal);
        var startCol  = SafeGetColumn(startTerminal);
        var endLine   = SafeGetLine(endTerminal);
        var endCol    = SafeGetColumn(endTerminal);
        _srcLine    = startLine;
        _srcCol     = startCol;
        _srcEndLine = endLine >= 0 ? endLine : -1;
        _srcEndCol  = endCol >= 0 ? endCol + endTextLength : -1;
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
