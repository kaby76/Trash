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
        sb.AppendLine($"digraph {DotId(ruleName)} {{");
        sb.AppendLine("rankdir=LR;");
        AppendNodes(sb, Sorted(states), decisionNumbers);
        AppendEdges(sb, Sorted(states), states, grammar, ruleTransitionToFollow: true);
        sb.Append('}');
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
        AppendEdges(sb, allStates, allSet, grammar, ruleTransitionToFollow: false);
        sb.Append('}');
        return sb.ToString();
    }

    // ---- shared node / edge rendering --------------------------------------

    private static void AppendNodes(StringBuilder sb, IEnumerable<ATNState> states,
                                    Dictionary<DecisionState, int> decisionNumbers)
    {
        foreach (var s in states)
        {
            string nodeId = $"s{s.stateNumber}";
            if (s is DecisionState ds && decisionNumbers.TryGetValue(ds, out int d))
            {
                int n = s.NumberOfTransitions;
                var ports = new StringBuilder();
                for (int pi = 0; pi < n; pi++)
                {
                    if (pi > 0) ports.Append('|');
                    ports.Append($"<p{pi}>");
                }
                string label = $"{{&rarr;\\n{s.stateNumber}\\nd={d}|{{{ports}}}}}";
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=record, fixedsize=false, peripheries=1];");
            }
            else if (s is BlockEndState || s is LoopEndState)
            {
                string label = $"&larr;\\n{s.stateNumber}";
                sb.AppendLine($"{nodeId}[fontsize=11,label=\"{label}\", shape=circle, fixedsize=true, width=.55, peripheries=1];");
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
                                    bool ruleTransitionToFollow)
    {
        foreach (var s in states)
        {
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
                string edgeAttrs = isEpsilon
                    ? "fontname=\"Times-Italic\", label=\"&epsilon;\""
                    : $"fontsize=11, fontname=\"Courier\", arrowsize=.7, label = \"{EscapeLabel(label)}\", arrowhead = normal";
                string fromPort = (s is DecisionState) ? $":p{i}" : "";
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
        NotSetTransition st               => "~" + SetLabel(st.set, grammar),
        SetTransition st                  => SetLabel(st.set, grammar),
        ActionTransition                  => "{action}",
        PredicateTransition               => "{pred}?",
        PrecedencePredicateTransition ppt => $"{ppt.precedence}>=p",
        _                                 => "?"
    };

    private static string TokenLabel(int token, GrammarModel grammar)
    {
        if (token == TokenConstants.EOF) return "<EOF>";
        if (grammar.IsLexer)
        {
            if (token >= 32 && token < 127 && token != '\'' && token != '\\')
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
            if (!first) sb.Append(',');
            first = false;
            if (iv.a == iv.b) sb.Append(TokenLabel(iv.a, grammar));
            else { sb.Append(TokenLabel(iv.a, grammar)); sb.Append(".."); sb.Append(TokenLabel(iv.b, grammar)); }
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
