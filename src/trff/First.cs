using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trff
{
    internal class First
    {
        private Parser _parser;
        private int _start_rule;

        public First(Parser parser, int start_rule)
        {
            _parser = parser;
            _start_rule = start_rule;
        }

        public virtual IntervalSet FIRST(int r)
        {
            var ss = _parser.Atn.ruleToStartState[r];
            int stateNumber = ss.stateNumber;
            _parser.State = stateNumber;
            _parser.Context = null;
            if (stateNumber < 0 || stateNumber >= _parser.Atn.states.Count)
            {
                throw new ArgumentException("Invalid state number.");
            }

            RuleContext ctx = null;
            ATNState s = _parser.Atn.states[stateNumber];
            IntervalSet following = _parser.Atn.NextTokens(s);
            return following;
        }


        private readonly Dictionary<int, IntervalSet> _followSets = new Dictionary<int, IntervalSet>();
        private bool _followBuilt = false;

        // Optional: expose the full map
        public IReadOnlyDictionary<int, IntervalSet> AllFollowSets
        {
            get { EnsureFollowBuilt(); return _followSets; }
        }

        // Ensure the fixed-point is computed once
        private void EnsureFollowBuilt()
        {
            if (_followBuilt) return;
            BuildFollowSetsFixedPoint();
            _followBuilt = true;
        }
        public virtual IntervalSet FOLLOW(int r)
        {
            EnsureFollowBuilt();
            if (_followSets.TryGetValue(r, out var set))
            {
                return new IntervalSet(set);
            }
            return new IntervalSet();
        }

        // ---- Fixed-point construction ----
        private void BuildFollowSetsFixedPoint()
        {
            var atn = _parser.Atn;
            int nRules = atn.ruleToStartState.Length;

            // Initialize sets
            for (int i = 0; i < nRules; i++)
                _followSets[i] = new IntervalSet();

            // Add EOF to the start rule's FOLLOW set
            _followSets[this._start_rule].Add(TokenConstants.EOF);

            // Iteratively apply constraints until convergence
            bool changed;
            const int EPSILON = -2; // ANTLR uses -2 for ε in IntervalSet
            const int EOF = -1;

            do
            {
                changed = false;

                // For every state and transition in the ATN
                foreach (var state in atn.states)
                {
                    if (state == null) continue;

                    int callerRule = state.ruleIndex; // Rule that owns this state

                    for (int k = 0; k < state.NumberOfTransitions; k++)
                    {
                        var trans = state.Transition(k);
                        if (trans is RuleTransition rt)
                        {
                            int calleeRule = rt.target.ruleIndex;

                            // Tokens that can appear immediately after the rule call
                            var fs = rt.followState.stateNumber;
                            IntervalSet next = atn.NextTokens(rt.followState);
                            // FIRST(followState) \ {ε}
                            var firstNoEps = new IntervalSet(next);
                            firstNoEps.Remove(EPSILON);

// NB: Originally, I thought 
//                            if (next.Contains(EOF))
//                            {
//                                // Special case. Add in FOLLOW of caller rule into callee.
//                                UnionInto(firstNoEps, _followSets[callerRule]);
//                            }

                            // FOLLOW(callee) += FIRST(after) \ {ε}
                            if (UnionInto(_followSets[calleeRule], firstNoEps))
                                changed = true;

                            // If ε can follow, then FOLLOW(callee) += FOLLOW(caller)
                            if (next.Contains(EPSILON) && callerRule >= 0)
                            {
                                if (UnionInto(_followSets[calleeRule], _followSets[callerRule]))
                                    changed = true;
                            }
                        }
                    }
                }
            } while (changed);
        }

        // Small helper: set union with change detection
        private static bool UnionInto(IntervalSet target, IntervalSet source)
        {
            var before = new IntervalSet(target);
            target.AddAll(source);
            return ! target.Equals(before);
        }
    }
}
