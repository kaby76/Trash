using System.Collections.Generic;
using System.IO;
using System.Text;

namespace trinterp;

/// <summary>
/// Generates Graphviz DOT files visualising the ATN (NFA) for a grammar.
///
/// --atn           : one &lt;ruleName&gt;.dot per rule (states scoped to that rule only)
/// --atn-combined  : one &lt;grammarName&gt;.atn.dot containing all rules in a single digraph
/// </summary>
public static class AtnDotWriter
{
    // ---- public entry points ------------------------------------------------

    /// <summary>Writes one &lt;ruleName&gt;.dot per rule to outDir.</summary>
    public static void WritePerRule(GrammarModel grammar, ATN atn, string outDir)
    {
        var decisionNumbers = BuildDecisionNumbers(atn);
        for (int i = 0; i < grammar.Rules.Count; i++)
        {
            var rule = grammar.Rules[i];
            var start = atn.ruleToStartState[i];
            var stop  = atn.ruleToStopState[i];
            var dot   = RuleToDot(rule.Name, start, stop, grammar, decisionNumbers);
            var path  = Path.Combine(outDir, grammar.Name + "." + rule.Name + ".dot");
            File.WriteAllText(path, dot);
        }
    }

    /// <summary>Writes a single &lt;grammarName&gt;.atn.dot with every rule's states combined.</summary>
    public static void WriteCombined(GrammarModel grammar, ATN atn, string outDir)
    {
        var dot  = CombinedToDot(grammar, atn);
        var path = Path.Combine(outDir, grammar.Name + ".atn.dot");
        File.WriteAllText(path, dot);
    }

    // ---- per-rule digraph ---------------------------------------------------

    private static string RuleToDot(string ruleName, RuleStartState start, RuleStopState stop,
                                    GrammarModel grammar, Dictionary<DecisionState, int> decisionNumbers)
    {
        // Collect states reachable from the rule start, scoped to this rule:
        //  - For RuleTransitions follow followState (skip called-rule interior)
        //  - Do NOT follow transitions out of the stop state (prevents bleeding
        //    into follow-states of callers that AddRuleFollowLinks added)
        var states = new HashSet<ATNState>();
        var queue  = new Queue<ATNState>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var s = queue.Dequeue();
            if (!states.Add(s)) continue;
            if (s == stop) continue; // stop state is a sink in the per-rule view
            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var tr   = s.Transition(i);
                var next = tr is RuleTransition rt ? rt.followState : tr.target;
                if (!states.Contains(next)) queue.Enqueue(next);
            }
        }
        states.Add(stop); // always include even if unreachable in edge cases

        var sb = new StringBuilder();
        sb.AppendLine("digraph ATN {");
        sb.AppendLine("rankdir=LR;");
        AppendNodes(sb, Sorted(states), decisionNumbers);
        AppendEdges(sb, Sorted(states), states, grammar, decisionNumbers, ruleTransitionToFollow: true);
        sb.AppendLine("}");
        return sb.ToString();
    }

    // ---- combined digraph ---------------------------------------------------

    private static string CombinedToDot(GrammarModel grammar, ATN atn)
    {
        var decisionNumbers = BuildDecisionNumbers(atn);
        var sb = new StringBuilder();
        sb.AppendLine($"digraph {DotId(grammar.Name)} {{");
        sb.AppendLine("rankdir=LR;");

        var allStates = new List<ATNState>();
        foreach (var s in atn.states)
            if (s != null) allStates.Add(s);
        allStates.Sort((a, b) => a.stateNumber.CompareTo(b.stateNumber));
        var allSet = new HashSet<ATNState>(allStates);

        AppendNodes(sb, allStates, decisionNumbers);
        // In the combined view show the actual RuleTransition target (rule start),
        // not the followState — the return arc is already an epsilon from the stop state.
        AppendEdges(sb, allStates, allSet, grammar, decisionNumbers, ruleTransitionToFollow: false);
        sb.AppendLine("}");
        return sb.ToString();
    }

    // ---- shared node / edge rendering --------------------------------------

    private static void AppendNodes(StringBuilder sb, IEnumerable<ATNState> states,
                                    Dictionary<DecisionState, int> decisionNumbers)
    {
        foreach (var s in states)
        {
            string nodeId = $"s{s.stateNumber}";
            // StarBlockStartState and PlusBlockStartState are always circles even when
            // they appear in decisionToState (multi-alt case); check them before the
            // generic DecisionState branch below.
            if (s is StarBlockStartState || s is PlusBlockStartState)
            {
                char symbol = s is PlusBlockStartState ? '+' : '*';
                if (decisionNumbers.TryGetValue((DecisionState)s, out int dLoop))
                {
                    // Multi-alternative block start: render as decision record (ANTLR4 style).
                    int n = s.NumberOfTransitions;
                    var ports = new StringBuilder();
                    for (int pi = 0; pi < n; pi++) { if (pi > 0) ports.Append('|'); ports.Append($"<p{pi}>"); }
                    string label = $"{{&rarr;\\n{s.stateNumber}{symbol}\\nd={dLoop}|{{{ports}}}}}";
                    sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=record, fixedsize=false, peripheries=1];");
                }
                else
                {
                    string label = $"&rarr;\\n{s.stateNumber}{symbol}";
                    sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
                }
                continue;
            }

            if (s is DecisionState ds && decisionNumbers.TryGetValue(ds, out int d))
            {
                // Actual decision point: record shape with ports.
                // BasicBlockStart/TokensStart: {→\nN\nd=D|{ports}}
                // StarLoopEntry: {N*\nd=D|{ports}}, PlusLoopback: {N+\nd=D|{ports}}
                int n = s.NumberOfTransitions;
                var ports = new StringBuilder();
                for (int pi = 0; pi < n; pi++)
                {
                    if (pi > 0) ports.Append('|');
                    ports.Append($"<p{pi}>");
                }
                string label;
                if (s is BasicBlockStartState || s is TokensStartState)
                    label = $"{{&rarr;\\n{s.stateNumber}\\nd={d}|{{{ports}}}}}";
                else if (s is StarLoopEntryState)
                    label = $"{{{s.stateNumber}*\\nd={d}|{{{ports}}}}}";
                else if (s is PlusLoopbackState)
                    label = $"{{{s.stateNumber}+\\nd={d}|{{{ports}}}}}";
                else
                    label = $"{{{s.stateNumber}\\nd={d}|{{{ports}}}}}";
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=record, fixedsize=false, peripheries=1];");
            }
            else if (s is StarLoopbackState)
            {
                string label = $"{s.stateNumber}*";
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
            }
            else if (s is BlockEndState)
            {
                string label = $"&larr;\\n{s.stateNumber}";
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
            }
            else if (s is LoopEndState)
            {
                // LoopEnd is a plain circle — no '←' prefix.
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{s.stateNumber}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
            }
            else if (s is RuleStopState)
            {
                sb.AppendLine($"{nodeId}[fontsize=11, label=\"{s.stateNumber}\", shape=doublecircle, fixedsize=true, width=.6];");
            }
            else
            {
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{s.stateNumber}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
            }
        }
    }

    private static void AppendEdges(StringBuilder sb, IEnumerable<ATNState> states,
                                    HashSet<ATNState> included, GrammarModel grammar,
                                    Dictionary<DecisionState, int> decisionNumbers,
                                    bool ruleTransitionToFollow)
    {
        foreach (var s in states)
        {
            // In the per-rule view the rule stop state is a sink: follow-link epsilons
            // added by AddRuleFollowLinks point into callers' ATNs and must not appear.
            if (ruleTransitionToFollow && s is RuleStopState) continue;

            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var tr = s.Transition(i);
                ATNState target;
                string   label;

                if (tr is RuleTransition rt)
                {
                    target = ruleTransitionToFollow ? rt.followState : rt.target;
                    label  = "<" + RuleName(rt.target, grammar) + ">";
                }
                else
                {
                    target = tr.target;
                    label  = TransitionLabel(tr, grammar);
                }

                if (!included.Contains(target)) continue;
                bool isEpsilon = tr is EpsilonTransition;
                // Dashed style for loopback edges.
                bool isDashed = s is StarLoopbackState ||
                                (s is PlusLoopbackState && target.stateNumber < s.stateNumber);
                string edgeAttrs = isEpsilon
                    ? "fontname=\"Times-Italic\", label=\"&epsilon;\"" + (isDashed ? ", style=\"dashed\"" : "")
                    : $"fontsize=11, fontname=\"Courier\", arrowsize=.7, label = \"{EscapeLabel(label)}\", arrowhead = normal";
                // Port notation for all states rendered as decision records
                // (StarBlockStartState / PlusBlockStartState use ports when they are decisions).
                bool isDecision = s is DecisionState ds2
                    && decisionNumbers.ContainsKey(ds2);
                string fromPort = isDecision ? $":p{i}" : "";
                sb.AppendLine($"s{s.stateNumber}{fromPort} -> s{target.stateNumber} [{edgeAttrs}];");
            }
        }
    }

    // ---- helpers ------------------------------------------------------------

    private static IEnumerable<ATNState> Sorted(HashSet<ATNState> states)
    {
        var list = new List<ATNState>(states);
        list.Sort((a, b) => a.stateNumber.CompareTo(b.stateNumber));
        return list;
    }

    private static Dictionary<DecisionState, int> BuildDecisionNumbers(ATN atn)
    {
        var dict = new Dictionary<DecisionState, int>();
        for (int i = 0; i < atn.decisionToState.Count; i++)
            dict[atn.decisionToState[i]] = i;
        return dict;
    }

    private static string TransitionLabel(Transition tr, GrammarModel grammar) => tr switch
    {
        EpsilonTransition                 => "ε",
        WildcardTransition                => ".",
        AtomTransition at                 => TokenLabel(at.token, grammar),
        RangeTransition rt                => $"{TokenLabel(rt.from, grammar)}..{TokenLabel(rt.to, grammar)}",
        NotSetTransition st when st.set.GetIntervals().Count == 1
                                       && st.set.GetIntervals()[0].a == st.set.GetIntervals()[0].b
                                          => "~" + TokenLabel(st.set.GetIntervals()[0].a, grammar),
        NotSetTransition st               => "~" + SetLabel(st.set, grammar),
        SetTransition st                  => SetLabel(st.set, grammar),
        ActionTransition at2              => $"action_{at2.ruleIndex}:{at2.actionIndex}",
        PredicateTransition pt            => $"pred_{pt.ruleIndex}:{pt.predIndex}",
        PrecedencePredicateTransition ppt => $"{ppt.precedence}>=p",
        _                                 => "?"
    };

    private static string TokenLabel(int token, GrammarModel grammar)
    {
        if (token == TokenConstants.EOF) return "EOF";
        if (grammar.IsLexer)
        {
            // Special characters that need explicit handling to match ANTLR4's DOT output.
            // EscapeLabel doubles any backslashes, so one backslash here → two in the DOT file.
            // Note: ANTLR4 itself uses an extra level of escaping for atom transitions vs set
            // elements; we target the set-element level (which appears more often).
            switch (token)
            {
                case '\t': return "'\t'";    // literal tab
                case '\n': return "'\\n'";   // backslash + n → EscapeLabel → two backslashes + n
                case '\r': return "''";      // CR: ANTLR4 emits empty char literal
                case '"':  return "'\"'";    // quote → EscapeLabel escapes " to \"
                case '\'': return "'''";     // three single-quotes
                case '\\': return "'\\'"    ;// one backslash → EscapeLabel → two backslashes
            }
            if (token >= 32 && token < 127)
                return $"'{(char)token}'";
            return token.ToString();
        }
        // Parser: resolve to string literal or token name.
        foreach (var kv in grammar.StringLiteralToType)
            if (kv.Value == token) return kv.Key;
        foreach (var kv in grammar.TokenNameToType)
            if (kv.Value == token) return kv.Key;
        return token.ToString();
    }

    private static string SetLabel(IntervalSet set, GrammarModel grammar)
    {
        var sb = new StringBuilder("{");
        bool first = true;
        foreach (var iv in set.GetIntervals())
        {
            // Range notation (a..b) is only meaningful for lexer character sets.
            // Parser token-type intervals contain unrelated token names and must be expanded.
            if (grammar.IsLexer && iv.a != iv.b)
            {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append($"{TokenLabel(iv.a, grammar)}..{TokenLabel(iv.b, grammar)}");
            }
            else
            {
                for (int el = iv.a; el <= iv.b; el++)
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(TokenLabel(el, grammar));
                }
            }
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static string RuleName(ATNState ruleStartTarget, GrammarModel grammar)
    {
        var idx = ruleStartTarget.ruleIndex;
        if (idx >= 0 && idx < grammar.Rules.Count) return grammar.Rules[idx].Name;
        return "?";
    }

    private static string DotId(string name)
    {
        foreach (var c in name)
            if (!char.IsLetterOrDigit(c) && c != '_') return $"\"{name}\"";
        return name;
    }

    private static string EscapeLabel(string s)
        => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
}
