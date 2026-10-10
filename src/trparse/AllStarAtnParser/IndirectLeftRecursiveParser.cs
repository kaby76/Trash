namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

/// <summary>
/// Fixed-point ATN interpreter used when a grammar contains indirect left
/// recursion. A recursive call initially observes an empty seed; repeated
/// evaluation grows that seed until no rule can consume more input. This is
/// the same seed-growing principle used for direct left recursion, generalized
/// to a mutually recursive set of rules.
/// </summary>
internal static class IndirectLeftRecursiveParser
{
    internal sealed class ParseProgress
    {
        public int FarthestPosition { get; set; }
    }

    private const int DefaultChannel = 0;
    private const int EofType = -1;

    private readonly record struct RuleKey(int Rule, int Start, int Precedence);

    private sealed record RuleResult(int End, List<ParseEvent> Events);

    private readonly record struct StateKey(int State, int Position);

    private sealed record WorkItem(MyATNState State, int Position,
        List<ParseEvent> Events);

    public static List<ParseEvent> Parse(
        MyATN atn, IReadOnlyList<LexerToken> allTokens, int startRuleIndex,
        ParserStatistics statistics = null, ParseProgress progress = null)
    {
        ArgumentNullException.ThrowIfNull(atn);
        if ((uint)startRuleIndex >= (uint)atn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));

        var onChannel = new List<int>(allTokens.Count);
        for (int i = 0; i < allTokens.Count; i++)
            if (allTokens[i].Channel == DefaultChannel ||
                allTokens[i].Type == EofType)
                onChannel.Add(i);

        var evaluator = new Evaluator(atn, allTokens, onChannel, statistics,
            progress);
        return evaluator.Parse(startRuleIndex);
    }

    /// <summary>
    /// Grow indirect-left-recursive seeds while lexing at each terminal edge.
    /// A lexer cursor (character position and mode stack), rather than a token
    /// index, identifies each memoized parser position. Only tokens on the
    /// selected derivation are copied into the returned token stream.
    /// </summary>
    public static List<ParseEvent> ParseContextAware(
        MyATN parserAtn, MyATN lexerAtn, string input, int startRuleIndex,
        out TokenStore selectedTokens, LexerStatistics lexerStatistics = null,
        ParserStatistics parserStatistics = null,
        LexerAtnSimulator.LexerDfaCache lexerDfaCache = null,
        ParseProgress progress = null)
    {
        ArgumentNullException.ThrowIfNull(parserAtn);
        ArgumentNullException.ThrowIfNull(lexerAtn);
        if ((uint)startRuleIndex >= (uint)parserAtn.start.Length)
            throw new ArgumentOutOfRangeException(nameof(startRuleIndex));
        var evaluator = new ContextEvaluator(parserAtn, lexerAtn, input,
            lexerStatistics, parserStatistics, lexerDfaCache, progress);
        return evaluator.Parse(startRuleIndex, out selectedTokens);
    }

    private sealed class Evaluator
    {
        private readonly MyATN _atn;
        private readonly IReadOnlyList<LexerToken> _tokens;
        private readonly IReadOnlyList<int> _onChannel;
        private readonly ParserStatistics _statistics;
        private readonly ParseProgress _progress;
        private readonly Dictionary<RuleKey, List<RuleResult>> _memo = new();
        private readonly Dictionary<RuleKey, HashSet<RuleKey>> _dependents = new();
        private readonly Queue<RuleKey> _pending = new();
        private readonly HashSet<RuleKey> _queued = new();

        public Evaluator(MyATN atn, IReadOnlyList<LexerToken> tokens,
            IReadOnlyList<int> onChannel, ParserStatistics statistics,
            ParseProgress progress)
        {
            _atn = atn;
            _tokens = tokens;
            _onChannel = onChannel;
            _statistics = statistics;
            _progress = progress;
        }

        public List<ParseEvent> Parse(int startRule)
        {
            var root = new RuleKey(startRule, 0, 0);
            _memo[root] = new List<RuleResult>();
            Schedule(root);
            while (_pending.Count > 0)
            {
                var key = _pending.Dequeue();
                _queued.Remove(key);
                foreach (var result in Evaluate(key))
                {
                    var results = _memo[key];
                    if (results.Any(r => r.End == result.End)) continue;
                    results.Add(result);
                    results.Sort((a, b) => a.End.CompareTo(b.End));
                    if (_dependents.TryGetValue(key, out var callers))
                        foreach (var caller in callers) Schedule(caller);
                }
            }

            // Prefer the derivation consuming the most input, matching the
            // growth behavior of an ANTLR left-recursive rule.
            return _memo[root]
                .OrderByDescending(r => r.End)
                .FirstOrDefault()?.Events;
        }

        private void Schedule(RuleKey key)
        {
            if (_queued.Add(key)) _pending.Enqueue(key);
        }

        private void AddDependency(RuleKey child, RuleKey caller)
        {
            if (!_dependents.TryGetValue(child, out var callers))
                _dependents.Add(child, callers = new HashSet<RuleKey>());
            callers.Add(caller);
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
                if (_progress != null)
                    _progress.FarthestPosition = Math.Max(
                        _progress.FarthestPosition, item.Position);
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
                                Schedule(childKey);
                            }
                            AddDependency(childKey, key);
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

                        case MyPredicateTransition predicate:
                            if (ParserPredicateMatches(predicate, item.Position))
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

        private bool ParserPredicateMatches(MyPredicateTransition predicate,
            int position)
        {
            if (!_atn.G4XExclusions.TryGetValue(
                    (predicate.ruleIndex, predicate.predIndex), out var exclusions))
                return true;
            if ((uint)position >= (uint)_onChannel.Count) return false;
            string text = _tokens[_onChannel[position]].Text;
            foreach (var operand in exclusions)
            {
                if (operand.Kind != "literal")
                    throw new NotSupportedException(
                        "Named parser-rule set-difference operands are not supported.");
                if (string.Equals(text, operand.Value, StringComparison.Ordinal))
                    return false;
            }
            return true;
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

    private readonly record struct LexPosition(int Offset, int Mode, string ModeStack);
    private readonly record struct ContextRuleKey(int Rule, LexPosition Start, int Precedence);
    private readonly record struct ContextStateKey(int State, LexPosition Position);
    private sealed record ContextResult(LexPosition End, List<ParseEvent> Events,
        List<int> TokenIndices);
    private sealed record ContextWorkItem(MyATNState State, LexPosition Position,
        List<ParseEvent> Events, List<int> TokenIndices);
    private sealed record LexStep(LexPosition End, int TokenIndex, int[] AllTokenIndices);

    private sealed class ContextEvaluator
    {
        private readonly MyATN _atn;
        private readonly string _input;
        private readonly LexerAtnSimulator _lexer;
        private readonly TokenStore _allTokens;
        private readonly ParserStatistics _statistics;
        private readonly ParseProgress _progress;
        private readonly AllStarSimulator _expected;
        private readonly Dictionary<LexPosition, LexerAtnSimulator.Cursor> _cursors = new();
        private readonly Dictionary<ContextRuleKey, List<ContextResult>> _memo = new();
        private readonly Dictionary<ContextRuleKey, HashSet<ContextRuleKey>> _dependents = new();
        private readonly Queue<ContextRuleKey> _pending = new();
        private readonly HashSet<ContextRuleKey> _queued = new();
        private readonly Dictionary<(LexPosition, MyTransition), LexStep> _lexSteps = new();
        private readonly Dictionary<MyTransition, HashSet<int>> _expectedTypes = new();

        public ContextEvaluator(MyATN parserAtn, MyATN lexerAtn, string input,
            LexerStatistics lexerStatistics, ParserStatistics statistics,
            LexerAtnSimulator.LexerDfaCache lexerDfaCache,
            ParseProgress progress)
        {
            _atn = parserAtn;
            _input = input;
            _statistics = statistics;
            _progress = progress;
            _allTokens = new TokenStore(input);
            _lexer = new LexerAtnSimulator(lexerAtn, lexerStatistics, lexerDfaCache);
            _lexer.SetInput(input);
            _expected = new AllStarSimulator(parserAtn);
            _cursors.Add(new LexPosition(0, 0, ""), new LexerAtnSimulator.Cursor());
        }

        public List<ParseEvent> Parse(int startRule, out TokenStore selectedTokens)
        {
            var root = new ContextRuleKey(startRule, new LexPosition(0, 0, ""), 0);
            _memo.Add(root, new List<ContextResult>());
            Schedule(root);
            while (_pending.Count > 0)
            {
                var key = _pending.Dequeue();
                _queued.Remove(key);
                foreach (var result in Evaluate(key))
                {
                    var results = _memo[key];
                    if (results.Any(r => r.End == result.End)) continue;
                    results.Add(result);
                    results.Sort((a, b) => a.End.Offset.CompareTo(b.End.Offset));
                    if (_dependents.TryGetValue(key, out var callers))
                        foreach (var caller in callers) Schedule(caller);
                }
            }

            var best = _memo[root].OrderByDescending(r => r.End.Offset).FirstOrDefault();
            selectedTokens = new TokenStore(_input);
            if (best == null) return null;
            var indexMap = new Dictionary<int, int>();
            foreach (int index in best.TokenIndices)
            {
                if (indexMap.ContainsKey(index)) continue;
                var token = _allTokens[index];
                indexMap.Add(index, selectedTokens.Add(token.Type, token.Channel,
                    token.StartIndex, token.StopIndex, token.Line, token.Column,
                    token.Type == EofType ? "<EOF>" : null));
            }
            var events = new List<ParseEvent>(best.Events.Count);
            foreach (var e in best.Events)
                events.Add(e.Kind == ParseEventKind.Consume
                    ? ParseEvent.Consume(indexMap[e.Index]) : e);
            return events;
        }

        private void Schedule(ContextRuleKey key)
        {
            if (_queued.Add(key)) _pending.Enqueue(key);
        }

        private void AddDependency(ContextRuleKey child, ContextRuleKey caller)
        {
            if (!_dependents.TryGetValue(child, out var callers))
                _dependents.Add(child, callers = new HashSet<ContextRuleKey>());
            callers.Add(caller);
        }

        private IEnumerable<ContextResult> Evaluate(ContextRuleKey key)
        {
            var work = new Queue<ContextWorkItem>();
            var visited = new HashSet<ContextStateKey>();
            work.Enqueue(new ContextWorkItem(_atn.start[key.Rule], key.Start,
                new List<ParseEvent>(), new List<int>()));
            while (work.Count > 0)
            {
                var item = work.Dequeue();
                if (_progress != null)
                    _progress.FarthestPosition = Math.Max(
                        _progress.FarthestPosition, item.Position.Offset);
                if (!visited.Add(new ContextStateKey(item.State.stateNumber,
                        item.Position))) continue;

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
                    yield return new ContextResult(item.Position, events,
                        item.TokenIndices);
                    continue;
                }

                foreach (var transition in item.State.transitions)
                {
                    switch (transition)
                    {
                        case MyRuleTransition rule:
                        {
                            var childKey = new ContextRuleKey(rule.ruleIndex,
                                item.Position, rule.precedence);
                            if (!_memo.TryGetValue(childKey, out var children))
                            {
                                children = new List<ContextResult>();
                                _memo.Add(childKey, children);
                                Schedule(childKey);
                            }
                            AddDependency(childKey, key);
                            foreach (var child in children)
                            {
                                work.Enqueue(new ContextWorkItem(rule.target,
                                    child.End, Append(item.Events, child.Events),
                                    Append(item.TokenIndices, child.TokenIndices)));
                            }
                            break;
                        }
                        case MyPrecedencePredicateTransition predicate:
                            if (predicate.precedence >= key.Precedence)
                                work.Enqueue(new ContextWorkItem(transition.target,
                                    item.Position, item.Events, item.TokenIndices));
                            break;
                        case MyPredicateTransition predicate:
                            if (ParserPredicateMatches(predicate, item.Position))
                                work.Enqueue(new ContextWorkItem(transition.target,
                                    item.Position, item.Events, item.TokenIndices));
                            break;
                        case MyAtomTransition or MyRangeTransition or
                             MySetTransition or MyNotSetTransition or
                             MyWildcardTransition:
                        {
                            var step = Lex(item.Position, transition,
                                ExpectedTypes(transition));
                            if (step == null || !transition.Matches(
                                    _allTokens[step.TokenIndex].Type, 1,
                                    _atn.maxTokenType)) break;
                            var events = new List<ParseEvent>(item.Events)
                                { ParseEvent.Consume(step.TokenIndex) };
                            var indices = new List<int>(item.TokenIndices);
                            indices.AddRange(step.AllTokenIndices);
                            work.Enqueue(new ContextWorkItem(transition.target,
                                step.End, events, indices));
                            break;
                        }
                        default:
                            work.Enqueue(new ContextWorkItem(transition.target,
                                item.Position, item.Events, item.TokenIndices));
                            break;
                    }
                }
            }
        }

        private bool ParserPredicateMatches(MyPredicateTransition predicate,
            LexPosition position)
        {
            if (!_atn.G4XExclusions.TryGetValue(
                    (predicate.ruleIndex, predicate.predIndex), out var exclusions))
                return true;
            var expected = _expected.GetExpectedTokenTypes(
                predicate.target, PredictionContext.EMPTY, 0);
            var step = Lex(position, predicate, expected);
            if (step == null) return false;
            var text = _allTokens[step.TokenIndex].Text;
            foreach (var operand in exclusions)
            {
                if (operand.Kind != "literal")
                    throw new NotSupportedException(
                        "Named parser-rule set-difference operands are not supported.");
                if (string.Equals(text, operand.Value, StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        private HashSet<int> ExpectedTypes(MyTransition transition)
        {
            if (_expectedTypes.TryGetValue(transition, out var types)) return types;
            types = new HashSet<int>();
            if (transition.Matches(EofType, 0, _atn.maxTokenType)) types.Add(EofType);
            for (int type = 1; type <= _atn.maxTokenType; type++)
                if (transition.Matches(type, 1, _atn.maxTokenType)) types.Add(type);
            _expectedTypes.Add(transition, types);
            return types;
        }

        private LexStep Lex(LexPosition position, MyTransition transition,
            IReadOnlySet<int> expected)
        {
            if (_lexSteps.TryGetValue((position, transition), out var cached))
                return cached;
            if (!_cursors.TryGetValue(position, out var saved) ||
                saved.Position > _input.Length) return null;
            var cursor = saved.Clone();
            var indices = new List<int>();
            try
            {
                while (true)
                {
                    int index = _lexer.NextToken(_allTokens, cursor, expected);
                    indices.Add(index);
                    var token = _allTokens[index];
                    if (token.Channel != DefaultChannel && token.Type != EofType)
                        continue;
                    var end = new LexPosition(cursor.Position, cursor.Mode,
                        string.Join(',', cursor.ModeStack));
                    _cursors.TryAdd(end, cursor);
                    var step = new LexStep(end, index, indices.ToArray());
                    _lexSteps.Add((position, transition), step);
                    return step;
                }
            }
            catch (InvalidOperationException ex) when (
                ex.Message.StartsWith("Lexer error", StringComparison.Ordinal))
            {
                return null; // an impossible branch, not a failure of every parse
            }
        }

        private static List<T> Append<T>(List<T> prefix, List<T> suffix)
        {
            var result = new List<T>(prefix.Count + suffix.Count);
            result.AddRange(prefix);
            result.AddRange(suffix);
            return result;
        }
    }
}
