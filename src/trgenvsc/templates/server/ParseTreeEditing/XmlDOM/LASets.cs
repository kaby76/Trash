// Template generated code from Antlr4BuildTasks.Template v 8.1
namespace XmlDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    internal class LASets
    {
        private Parser _parser;
        private CommonTokenStream _token_stream;
        private List<IToken> _input;
        private int _cursor;

        private readonly Dictionary<Tuple<ATNState, int, int>, bool> _visited =
            new Dictionary<Tuple<ATNState, int, int>, bool>();
        private readonly Dictionary<Tuple<ATNState, int, int, List<int>>, bool> _visited_extra =
            new Dictionary<Tuple<ATNState, int, int, List<int>>, bool>();
        private HashSet<ATNState> _stop_states;
        private HashSet<ATNState> _start_states;
        private bool _log_parse = false;
        private bool _log_closure = false;
        private static bool _reentry;

        private class Edge
        {
            public ATNState _from;
            public ATNState _to;
            public ATNState _follow;
            public TransitionType _type;
            public IntervalSet _label;
            public int _index_at_transition;
            public int _index; // Where we are in parse at _to state.
        }

        public LASets()
        {
        }

        public IntervalSet Compute(Parser parser, CommonTokenStream token_stream, int line, int col)
        {
            IntervalSet result = new IntervalSet();
            //_log_closure = true;
            //_log_parse = true;
            _input = new List<IToken>();
            _parser = parser;
            _token_stream = token_stream;
            _stop_states = new HashSet<ATNState>();
            foreach (ATNState s in parser.Atn.ruleToStopState.Select(t => parser.Atn.states[t.stateNumber]))
            {
                _stop_states.Add(s);
            }
            _start_states = new HashSet<ATNState>();
            foreach (ATNState s in parser.Atn.ruleToStartState.Select(t => parser.Atn.states[t.stateNumber]))
            {
                _start_states.Add(s);
            }
            int currentIndex = _token_stream.Index;
            _token_stream.Seek(0);
            int offset = 1;
            // Copy to _input the entire _token_stream.
            // Must take care to not come back here reentrancy from Antlr.
            if (_reentry) return result;
            _reentry = true;
            while (true)
            {
                IToken token = _token_stream.LT(offset++);
                _input.Add(token);
                _cursor = token.TokenIndex;
                if (token.Type == TokenConstants.EOF)
                {
                    break;
                }
                if (token.Line >= line && token.Column >= col)
                {
                    break;
                }
            }
            // Remove last token on input.
            _input.RemoveAt(_input.Count - 1);
            _token_stream.Seek(currentIndex);
            _reentry = false;
            // Start parse from a fake initial state.
            List<List<Edge>> all_parses = EnterState(new List<int>() {}, new Edge()
            {
                _index = 0,
                _index_at_transition = 0,
                _to = _parser.Atn.states[0],
                _type = TransitionType.EPSILON
            }, 0);
            // Eliminate all paths that don't consume all input.
            List<List<Edge>> temp = new List<List<Edge>>();
            if (all_parses != null)
            {
                foreach (List<Edge> p in all_parses)
                {
                    //System.Console.Error.WriteLine(PrintSingle(p));
                    if (Validate(p, _input))
                    {
                        temp.Add(p);
                    }
                }
            }
            all_parses = temp;
            if (all_parses != null && _log_closure)
            {
                foreach (List<Edge> p in all_parses)
                {
                    System.Console.Error.WriteLine("Path " + PrintSingle(p));
                }
            }
            if (all_parses != null)
            {
                foreach (List<Edge> p in all_parses)
                {
                    HashSet<ATNState> set = ComputeSingle(p);
                    if (_log_closure)
                    {
                        System.Console.Error.WriteLine("All states for path "
                                                       + string.Join(" ", set.ToList()));
                    }

                    foreach (ATNState s in set)
                    {
                        foreach (Transition t in s.TransitionsArray)
                        {
                            switch (t.TransitionType)
                            {
                                case TransitionType.RULE:
                                    break;

                                case TransitionType.PREDICATE:
                                    break;

                                case TransitionType.WILDCARD:
                                    break;

                                default:
                                    if (!t.IsEpsilon)
                                    {
                                        result.AddAll(t.Label);
                                    }

                                    break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private bool CheckPredicate(PredicateTransition transition)
        {
            return transition.Predicate.Eval(_parser, ParserRuleContext.EmptyContext);
        }

        private int entry_value;

        // Step to state and continue parsing input.
        // Returns a list of transitions leading to a state that accepts input.
        private List<List<Edge>> EnterState(List<int> p, Edge t, int indent)
        {
            int here = ++entry_value;
            int index_on_transition = t._index_at_transition;
            int token_index = t._index;
            ATNState state = t._to;
            var rule_match = _parser.Atn.ruleToStartState.Where(v => v.stateNumber == state.stateNumber);
            var start_rule = rule_match.Any() ? rule_match.FirstOrDefault().ruleIndex : -1;
            var q = p.ToList();
            if (start_rule >= 0) q.Add(start_rule);

            // Upon reaching the cursor, return match.
            bool at_match = token_index == _cursor;
            if (at_match)
            {
                if (_log_parse)
                {
                    System.Console.Error.Write(
                        new String(' ', indent * 2)
                        + "Entry "
                        + here
                        + " return ");
                }

                List<List<Edge>> res = new List<List<Edge>>() { new List<Edge>() { t } };
                if (_log_parse)
                {
                    string str = PrintResult(res);
                    System.Console.Error.WriteLine(
                        str);
                }
                return res;
            }

            IToken input_token = _input[token_index];

            if (_log_parse)
            {
                var name = (start_rule >= 0) ? (" " + _parser.RuleNames[start_rule]) : "";
                System.Console.Error.WriteLine(
                    new String(' ', indent * 2)
                    + "Entry "
                    + here
                    + " State "
                    + state
                    + name
                    + " tokenIndex "
                    + token_index
                    + " "
                    + input_token.Text
                );
            }

            bool at_match2 = input_token.TokenIndex >= _cursor;
            if (at_match2)
            {
                if (_log_parse)
                {
                    System.Console.Error.Write(
                        new String(' ', indent * 2)
                        + "Entry "
                        + here
                        + " return ");
                }

                List<List<Edge>> res = new List<List<Edge>>() { new List<Edge>() { t } };
                if (_log_parse)
                {
                    string str = PrintResult(res);
                    System.Console.Error.WriteLine(
                        str);
                }
                return res;
            }

            if (_visited.ContainsKey(new Tuple<ATNState, int, int>(state, token_index, indent)))
            {
                if (_visited_extra.ContainsKey(new Tuple<ATNState, int, int, List<int>>(
                    state, token_index, indent, p)))
                {
                    if (_log_parse)
                    {
                        System.Console.Error.WriteLine(
                            new String(' ', indent * 2)
                            + "already visited.");
                    }
                    return null;
                }
            }

            _visited_extra[new Tuple<ATNState, int, int, List<int>>(state, token_index, indent, q)] = true;
            _visited[new Tuple<ATNState, int, int>(state, token_index, indent)] = true;

            List<List<Edge>> result = new List<List<Edge>>();

            if (_stop_states.Contains(state))
            {
                if (_log_parse)
                {
                    var n = _parser.Atn.ruleToStartState.Where(v => v.stateNumber == state.stateNumber);
                    var r = n.Any() ? n.FirstOrDefault().ruleIndex : -1;
                    var name = (r >= 0) ? (" " + _parser.RuleNames[r]) : "";
                    System.Console.Error.Write(
                        new String(' ', indent * 2)
                        + "Entry "
                        + here
                        + " State "
                        + state
                        + name
                        + " tokenIndex "
                        + token_index
                        + " "
                        + input_token.Text
                        + " return ");
                }

                List<List<Edge>> res = new List<List<Edge>>() { new List<Edge>() { t } };
                if (_log_parse)
                {
                    string str = PrintResult(res);
                    System.Console.Error.WriteLine(
                        str);
                }
                return res;
            }

            // Search all transitions from state.
            foreach (Transition transition in state.TransitionsArray)
            {
                List<List<Edge>> matches = null;
                switch (transition.TransitionType)
                {
                    case TransitionType.RULE:
                        {
                            RuleTransition rule = (RuleTransition)transition;
                            ATNState sub_state = rule.target;
                            matches = EnterState(q, new Edge()
                            {
                                _from = state,
                                _to = rule.target,
                                _follow = rule.followState,
                                _label = rule.Label,
                                _type = rule.TransitionType,
                                _index = token_index,
                                _index_at_transition = token_index
                            }, indent + 1);
                            if (matches != null && matches.Count == 0)
                            {
                                if (_log_parse)
                                {
                                    System.Console.Error.WriteLine(
                                        new String(' ', indent * 2)
                                        + "throwing exception.");
                                }
                                throw new Exception();
                            }

                            if (matches != null)
                            {
                                List<List<Edge>> new_matches = new List<List<Edge>>();
                                foreach (List<Edge> match in matches)
                                {
                                    Edge f = match.First(); // "to" is possibly final state of submachine.
                                    Edge l = match.Last(); // "to" is start state of submachine.
                                    bool is_final = _stop_states.Contains(f._to);
                                    bool is_at_caret = f._index >= _cursor;
                                    if (!is_final)
                                    {
                                        new_matches.Add(match);
                                    }
                                    else
                                    {
                                        List<List<Edge>> xxx = EnterState(q, new Edge()
                                        {
                                            _from = f._to,
                                            _to = rule.followState,
                                            _label = null,
                                            _type = TransitionType.EPSILON,
                                            _index = f._index,
                                            _index_at_transition = f._index
                                        }, indent + 1);
                                        if (xxx != null && xxx.Count == 0)
                                        {
                                            if (_log_parse)
                                            {
                                                System.Console.Error.WriteLine(
                                                    new String(' ', indent * 2)
                                                    + "throwing exception.");
                                            }
                                            throw new Exception();
                                        }

                                        if (xxx != null)
                                        {
                                            foreach (List<Edge> y in xxx)
                                            {
                                                List<Edge> copy = y.ToList();
                                                foreach (Edge g in match)
                                                {
                                                    copy.Add(g);
                                                }
                                                new_matches.Add(copy);
                                            }

                                        }
                                    }
                                }
                                matches = new_matches;
                            }
                        }
                        break;

                    case TransitionType.PREDICATE:
                        if (CheckPredicate((PredicateTransition)transition))
                        {
                            matches = EnterState(q, new Edge()
                            {
                                _from = state,
                                _to = transition.target,
                                _label = transition.Label,
                                _type = transition.TransitionType,
                                _index = token_index,
                                _index_at_transition = token_index
                            }, indent + 1);
                            if (matches != null && matches.Count == 0)
                            {
                                if (_log_parse)
                                {
                                    System.Console.Error.WriteLine(
                                        new String(' ', indent * 2)
                                        + "throwing exception.");
                                }
                                throw new Exception();
                            }
                        }
                        break;

                    case TransitionType.WILDCARD:
                        matches = EnterState(q, new Edge()
                        {
                            _from = state,
                            _to = transition.target,
                            _label = transition.Label,
                            _type = transition.TransitionType,
                            _index = token_index + 1,
                            _index_at_transition = token_index
                        }, indent + 1);
                        if (matches != null && matches.Count == 0)
                        {
                            if (_log_parse)
                            {
                                System.Console.Error.WriteLine(
                                    new String(' ', indent * 2)
                                    + "throwing exception.");
                            }
                            throw new Exception();
                        }

                        break;

                    default:
                        if (transition.IsEpsilon)
                        {
                            matches = EnterState(q, new Edge()
                            {
                                _from = state,
                                _to = transition.target,
                                _label = transition.Label,
                                _type = transition.TransitionType,
                                _index = token_index,
                                _index_at_transition = token_index
                            }, indent + 1);
                            if (matches != null && matches.Count == 0)
                            {
                                if (_log_parse)
                                {
                                    System.Console.Error.WriteLine(
                                        new String(' ', indent * 2)
                                        + "throwing exception.");
                                }
                                throw new Exception();
                            }
                        }
                        else
                        {
                            IntervalSet set = transition.Label;
                            if (set != null && set.Count > 0)
                            {
                                if (transition.TransitionType == TransitionType.NOT_SET)
                                {
                                    set = set.Complement(IntervalSet.Of(TokenConstants.MinUserTokenType, _parser.Atn.maxTokenType));
                                }

                                if (set.Contains(input_token.Type))
                                {
                                    matches = EnterState(q, new Edge()
                                    {
                                        _from = state,
                                        _to = transition.target,
                                        _label = transition.Label,
                                        _type = transition.TransitionType,
                                        _index = token_index + 1,
                                        _index_at_transition = token_index
                                    }, indent + 1);
                                    if (matches != null && matches.Count == 0)
                                    {
                                        if (_log_parse)
                                        {
                                            System.Console.Error.WriteLine(
                                                new String(' ', indent * 2)
                                                + "throwing exception.");
                                        }
                                        throw new Exception();
                                    }
                                }
                            }
                        }
                        break;
                }

                if (matches != null)
                {
                    foreach (List<Edge> match in matches)
                    {
                        List<Edge> x = match.ToList();
                        if (t != null)
                        {
                            x.Add(t);
                            Edge prev = null;
                            foreach (Edge z in x)
                            {
                                ATNState ff = z._to;
                                if (prev != null)
                                {
                                    if (prev._from != ff)
                                    {
                                        System.Console.Error.WriteLine(
                                            new String(' ', indent * 2)
                                            + "Fail "
                                            + PrintSingle(x));
                                        Debug.Assert(false);
                                    }
                                }

                                prev = z;
                            }
                        }
                        result.Add(x);
                    }
                }
            }
            if (result.Count == 0)
            {
                if (_log_parse)
                {
                    System.Console.Error.WriteLine(
                        new String(' ', indent * 2)
                        + "result empty.");
                }
                return null;
            }

            if (_log_parse)
            {
                var n = _parser.Atn.ruleToStartState.Where(v => v.stateNumber == state.stateNumber);
                var r = n.Any() ? n.FirstOrDefault().ruleIndex : -1;
                var name = (r >= 0) ? (" " + _parser.RuleNames[r]) : "";
                System.Console.Error.Write(
                    new String(' ', indent * 2)
                    + "Entry "
                    + here
                    + " State "
                    + state
                    + name
                    + " tokenIndex "
                    + token_index
                    + " "
                    + input_token.Text
                    + " return ");
                string str = PrintResult(result);
                System.Console.Error.WriteLine(
                    str);
            }
            return result;
        }

        private HashSet<ATNState> closure(ATNState start)
        {
            if (start == null)
            {
                throw new Exception();
            }

            HashSet<ATNState> visited = new HashSet<ATNState>();
            Stack<ATNState> stack = new Stack<ATNState>();
            stack.Push(start);
            while (stack.Any())
            {
                ATNState state = stack.Pop();
                if (visited.Contains(state))
                {
                    continue;
                }

                visited.Add(state);
                foreach (Transition transition in state.TransitionsArray)
                {
                    switch (transition.TransitionType)
                    {
                        case TransitionType.RULE:
                            {
                                RuleTransition rule = (RuleTransition)transition;
                                ATNState sub_state = rule.target;
                                HashSet<ATNState> cl = closure(sub_state);
                                if (cl.Where(s => _stop_states.Contains(s) && s.atn == sub_state.atn).Any())
                                {
                                    HashSet<ATNState> cl2 = closure(rule.followState);
                                    cl.UnionWith(cl2);
                                }
                                foreach (ATNState c in cl)
                                {
                                    visited.Add(c);
                                }
                            }
                            break;

                        case TransitionType.PREDICATE:
                            if (CheckPredicate((PredicateTransition)transition))
                            {
                                if (transition.target == null)
                                {
                                    throw new Exception();
                                }

                                stack.Push(transition.target);
                            }
                            break;

                        case TransitionType.WILDCARD:
                            break;

                        default:
                            if (transition.IsEpsilon)
                            {
                                if (transition.target == null)
                                {
                                    throw new Exception();
                                }

                                stack.Push(transition.target);
                            }
                            break;
                    }
                }
            }
            return visited;
        }

        private HashSet<ATNState> ComputeSingle(List<Edge> parse)
        {
            List<Edge> copy = parse.ToList();
            HashSet<ATNState> result = new HashSet<ATNState>();
            if (_log_closure)
            {
                System.Console.Error.WriteLine("Computing closure for the following parse:");
                System.Console.Error.Write(PrintSingle(parse));
                System.Console.Error.WriteLine();
            }

            if (!copy.Any())
            {
                return result;
            }

            Edge last_transaction = copy.First();
            if (last_transaction == null)
            {
                return result;
            }

            ATNState current_state = last_transaction._to;
            if (current_state == null)
            {
                throw new Exception();
            }

            for (; ; )
            {
                if (_log_closure)
                {
                    System.Console.Error.WriteLine("Getting closure of " + current_state.stateNumber);
                }
                HashSet<ATNState> c = closure(current_state);
                if (_log_closure)
                {
                    System.Console.Error.WriteLine("closure " + string.Join(" ", c.Select(s => s.stateNumber)));
                }
                bool do_continue = false;
                ATN atn = current_state.atn;
                int rule = current_state.ruleIndex;
                RuleStartState start_state = atn.ruleToStartState[rule];
                RuleStopState stop_state = atn.ruleToStopState[rule];
                bool changed = false;
                foreach (ATNState s in c)
                {
                    if (result.Contains(s))
                    {
                        continue;
                    }

                    changed = true;
                    result.Add(s);
                    if (s == stop_state)
                    {
                        do_continue = true;
                    }
                }
                if (!changed)
                {
                    break;
                }

                if (!do_continue)
                {
                    break;
                }

                for (; ; )
                {
                    if (!copy.Any())
                    {
                        break;
                    }

                    copy.RemoveAt(0);
                    if (!copy.Any())
                    {
                        break;
                    }

                    last_transaction = copy.First();
                    if (start_state == last_transaction._from)
                    {
                        copy.RemoveAt(0);
                        if (!copy.Any())
                        {
                            break;
                        }

                        last_transaction = copy.First();
                        // Get follow state of rule-type transition.
                        ATNState from_state = last_transaction._from;
                        if (from_state == null)
                        {
                            break;
                        }

                        ATNState follow_state = last_transaction._follow;
                        current_state = follow_state;
                        if (current_state == null)
                        {
                            throw new Exception();
                        }

                        break;
                    }
                }
            }
            return result;
        }

        private bool Validate(List<Edge> parse, List<IToken> i)
        {
            List<Edge> q = parse.ToList();
            q.Reverse();
            List<IToken>.Enumerator ei = _input.GetEnumerator();
            List<Edge>.Enumerator eq = q.GetEnumerator();
            bool fei = false;
            bool feq = false;
            for (; ; )
            {
                fei = ei.MoveNext();
                IToken v = ei.Current;
                if (!fei)
                {
                    break;
                }

                bool empty = true;
                for (; empty;)
                {
                    feq = eq.MoveNext();
                    if (!feq)
                    {
                        break;
                    }

                    Edge x = eq.Current;
                    switch (x._type)
                    {
                        case TransitionType.RULE:
                            empty = true;
                            break;
                        case TransitionType.PREDICATE:
                            empty = true;
                            break;
                        case TransitionType.ACTION:
                            empty = true;
                            break;
                        case TransitionType.ATOM:
                            empty = false;
                            break;
                        case TransitionType.EPSILON:
                            empty = true;
                            break;
                        case TransitionType.INVALID:
                            empty = true;
                            break;
                        case TransitionType.NOT_SET:
                            empty = false;
                            break;
                        case TransitionType.PRECEDENCE:
                            empty = true;
                            break;
                        case TransitionType.SET:
                            empty = false;
                            break;
                        case TransitionType.WILDCARD:
                            empty = false;
                            break;
                        default:
                            throw new Exception();
                    }
                }
                Edge w = eq.Current;
                if (w == null && v == null)
                {
                    return true;
                }
                else if (w == null)
                {
                    return false;
                }
                else if (v == null)
                {
                    return false;
                }

                switch (w._type)
                {
                    case TransitionType.ATOM:
                        {
                            IntervalSet set = w._label;
                            if (set != null && set.Count > 0)
                            {
                                if (!set.Contains(v.Type))
                                {
                                    return false;
                                }
                            }
                            break;
                        }

                    case TransitionType.NOT_SET:
                        {
                            IntervalSet set = w._label;
                            set = set.Complement(IntervalSet.Of(TokenConstants.MinUserTokenType, _parser.Atn.maxTokenType));
                            if (set != null && set.Count > 0)
                            {
                                if (!set.Contains(v.Type))
                                {
                                    return false;
                                }
                            }
                            break;
                        }

                    case TransitionType.SET:
                        {
                            IntervalSet set = w._label;
                            if (set != null && set.Count > 0)
                            {
                                if (!set.Contains(v.Type))
                                {
                                    return false;
                                }
                            }
                            break;
                        }

                    case TransitionType.WILDCARD:
                        break;

                    default:
                        throw new Exception();
                }
            }
            return true;
        }

        private string PrintSingle(List<Edge> parse)
        {
            StringBuilder sb = new StringBuilder();
            List<Edge> q = parse.ToList();
            q.Reverse();
            foreach (Edge t in q)
            {
                string sym = "";
                switch (t._type)
                {
                    case TransitionType.ACTION:
                        sym = "on action (eps)";
                        break;
                    case TransitionType.ATOM:
                        sym = "on " + t._label.ToString() + " ('" + _input[t._index_at_transition].Text + "')";
                        break;
                    case TransitionType.EPSILON:
                        sym = "on eps";
                        break;
                    case TransitionType.INVALID:
                        sym = "invalid (eps)";
                        break;
                    case TransitionType.NOT_SET:
                        sym = "on not " + t._label.ToString();
                        break;
                    case TransitionType.PRECEDENCE:
                        sym = "on prec (eps)";
                        break;
                    case TransitionType.PREDICATE:
                        sym = "on pred (eps)";
                        break;
                    case TransitionType.RANGE:
                        sym = "on " + t._label.ToString() + " ('" + _input[t._index_at_transition].Text + "')";
                        break;
                    case TransitionType.RULE:
                        sym = "on " + _parser.RuleNames[t._to.ruleIndex] + " (eps)";
                        break;
                    case TransitionType.SET:
                        sym = "on " + t._label.ToString() + " ('" + _input[t._index_at_transition].Text + "')";
                        break;
                    case TransitionType.WILDCARD:
                        sym = "on wildcard ('" + _input[t._index_at_transition].Text + "')";
                        break;
                    default:
                        break;
                }
                sb.Append(" / " + t._from + " => " + t._to + " " + sym);
            }
            return sb.ToString();
        }

        private string PrintResult(List<List<Edge>> all_parses)
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<Edge> p in all_parses)
            {
                sb.Append("||| " + PrintSingle(p));
            }
            return sb.ToString();
        }
    }
}
