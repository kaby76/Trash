namespace AllStarAtnParser;

using Atn;

/// <summary>
/// Fixed-point ATN interpreter used when a grammar contains indirect left
/// recursion. A recursive call initially observes an empty seed; repeated
/// evaluation grows that seed until no rule can consume more input. This is
/// the same seed-growing principle used for direct left recursion, generalized
/// to a mutually recursive set of rules.
/// </summary>
internal static class IndirectLeftRecursiveParser
{
    private const int DefaultChannel = 0;
    private const int EofType = -1;

    private readonly record struct RuleKey(int Rule, int Start, int Precedence);

    private sealed record RuleResult(int End, List<ParseEvent> Events);

    private readonly record struct StateKey(int State, int Position);

    private sealed record WorkItem(MyATNState State, int Position,
        List<ParseEvent> Events);

    public static List<ParseEvent> Parse(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        ParserStatistics statistics = null)
    {
        ArgumentNullException.ThrowIfNull(atn);
        if ((uint)startRuleIndex >= (uint)atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        var onChannel = new List<int>(allTokens.Count);
        for (int i = 0; i < allTokens.Count; i++)
            if (allTokens[i].Channel == DefaultChannel ||
                allTokens[i].Type == EofType)
                onChannel.Add(i);

        var evaluator = new Evaluator(atn, allTokens, onChannel, statistics);
        return evaluator.Parse(startRuleIndex);
    }

    private sealed class Evaluator
    {
        private readonly MyATN _atn;
        private readonly IReadOnlyList<LexerToken> _tokens;
        private readonly IReadOnlyList<int> _onChannel;
        private readonly ParserStatistics _statistics;
        private readonly Dictionary<RuleKey, List<RuleResult>> _memo = new();

        public Evaluator(MyATN atn, IReadOnlyList<LexerToken> tokens,
            IReadOnlyList<int> onChannel, ParserStatistics statistics)
        {
            _atn = atn;
            _tokens = tokens;
            _onChannel = onChannel;
            _statistics = statistics;
        }

        public List<ParseEvent> Parse(int startRule)
        {
            var root = new RuleKey(startRule, 0, 0);
            _memo[root] = new List<RuleResult>();

            bool changed;
            do
            {
                changed = false;
                var keys = _memo.Keys.ToArray();
                foreach (var key in keys)
                {
                    foreach (var result in Evaluate(key))
                    {
                        var results = _memo[key];
                        if (results.Any(r => r.End == result.End))
                            continue;
                        results.Add(result);
                        results.Sort((a, b) => a.End.CompareTo(b.End));
                        changed = true;
                    }
                }

                // Evaluate calls discovered during this pass on the next pass.
                if (_memo.Count != keys.Length)
                    changed = true;
            } while (changed);

            // Prefer the derivation consuming the most input, matching the
            // growth behavior of an ANTLR left-recursive rule.
            return _memo[root]
                .OrderByDescending(r => r.End)
                .FirstOrDefault()?.Events;
        }

        private IEnumerable<RuleResult> Evaluate(RuleKey key)
        {
            var work = new Queue<WorkItem>();
            var visited = new HashSet<StateKey>();
            work.Enqueue(new WorkItem(_atn.start[key.Rule], key.Start,
                new List<ParseEvent>()));

            while (work.Count > 0)
            {
                var item = work.Dequeue();
                if (!visited.Add(new StateKey(
                        item.State.stateNumber, item.Position)))
                    continue;

                if (_atn.stop.Contains(item.State) &&
                    item.State.ruleIndex == key.Rule)
                {
                    var events = new List<ParseEvent>(item.Events.Count + 2);
                    bool precedenceRule = _atn.start[key.Rule].isPrecedenceRule;
                    events.Add(precedenceRule
                        ? ParseEvent.EnterRecursionRule(key.Rule)
                        : ParseEvent.EnterRule(key.Rule));
                    events.AddRange(item.Events);
                    events.Add(precedenceRule
                        ? ParseEvent.ExitRecursionRule(key.Rule)
                        : ParseEvent.ExitRule(key.Rule));
                    yield return new RuleResult(item.Position, events);
                    continue;
                }

                foreach (var transition in item.State.transitions)
                {
                    switch (transition)
                    {
                        case MyRuleTransition rule:
                        {
                            var childKey = new RuleKey(
                                rule.ruleIndex, item.Position, rule.precedence);
                            if (!_memo.TryGetValue(childKey, out var children))
                            {
                                children = new List<RuleResult>();
                                _memo.Add(childKey, children);
                            }
                            foreach (var child in children)
                            {
                                var events = Append(item.Events, child.Events);
                                work.Enqueue(new WorkItem(
                                    rule.target, child.End, events));
                            }
                            break;
                        }

                        case MyPrecedencePredicateTransition predicate:
                            if (predicate.precedence >= key.Precedence)
                                work.Enqueue(new WorkItem(
                                    transition.target, item.Position, item.Events));
                            break;

                        case MyAtomTransition or MyRangeTransition or
                             MySetTransition or MyNotSetTransition or
                             MyWildcardTransition:
                            if (item.Position < _onChannel.Count)
                            {
                                int tokenIndex = _onChannel[item.Position];
                                if (transition.Matches(
                                        _tokens[tokenIndex].Type, 1,
                                        _atn.maxTokenType))
                                {
                                    var events = new List<ParseEvent>(
                                        item.Events.Count + 1);
                                    events.AddRange(item.Events);
                                    events.Add(ParseEvent.Consume(tokenIndex));
                                    work.Enqueue(new WorkItem(
                                        transition.target,
                                        item.Position + 1, events));
                                }
                            }
                            break;

                        default:
                            // Epsilon, action and ordinary predicate edges do
                            // not consume input in the interpreted runtime.
                            work.Enqueue(new WorkItem(
                                transition.target, item.Position, item.Events));
                            break;
                    }
                }
            }
        }

        private static List<ParseEvent> Append(
            List<ParseEvent> prefix, List<ParseEvent> suffix)
        {
            var result = new List<ParseEvent>(prefix.Count + suffix.Count);
            result.AddRange(prefix);
            result.AddRange(suffix);
            return result;
        }
    }
}
