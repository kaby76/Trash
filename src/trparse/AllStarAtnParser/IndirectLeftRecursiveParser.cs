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

    private sealed record RuleResult(int End, EventSequence<ParseEvent> Events);

    private readonly record struct StateKey(int State, int Position);

    private sealed record WorkItem(MyATNState State, int Position,
        EventSequence<ParseEvent> Events);

    // A result for each longer left-recursive seed shares its previous prefix.
    // Flatten only the selected derivation; copying full Lists here made long
    // expression chains increasingly expensive on every fixed-point pass.
    private sealed class EventSequence<T>
    {
        public static readonly EventSequence<T> Empty = new();

        private readonly EventSequence<T> _left;
        private readonly EventSequence<T> _right;
        private readonly IReadOnlyList<T> _items;
        public int Count { get; }

        private EventSequence() { }

        private EventSequence(IReadOnlyList<T> items)
        {
            _items = items;
            Count = items.Count;
        }

        private EventSequence(EventSequence<T> left, EventSequence<T> right)
        {
            _left = left;
            _right = right;
            Count = left.Count + right.Count;
        }

        public static EventSequence<T> One(T value) => new(new[] { value });

        public static EventSequence<T> From(IReadOnlyList<T> items) =>
            items.Count == 0 ? Empty : new EventSequence<T>(items);

        public static EventSequence<T> Concat(
            EventSequence<T> left, EventSequence<T> right) =>
            left.Count == 0 ? right : right.Count == 0 ? left : new(left, right);

        public static EventSequence<T> Append(EventSequence<T> prefix, T value) =>
            Concat(prefix, One(value));

        public IEnumerable<T> Items()
        {
            var pending = new Stack<EventSequence<T>>();
            pending.Push(this);
            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (current._items != null)
                {
                    for (int i = 0; i < current._items.Count; i++)
                        yield return current._items[i];
                }
                else if (current._left != null)
                {
                    pending.Push(current._right);
                    pending.Push(current._left);
                }
            }
        }

        public List<T> ToList()
        {
            var result = new List<T>(Count);
            result.AddRange(Items());
            return result;
        }
    }

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
        private readonly Dictionary<RuleKey, HashSet<int>> _resultEnds = new();
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
            _resultEnds[root] = new HashSet<int>();
            Schedule(root);
            while (_pending.Count > 0)
            {
                var key = _pending.Dequeue();
                _queued.Remove(key);
                foreach (var result in Evaluate(key))
                {
                    var results = _memo[key];
                    if (!_resultEnds[key].Add(result.End)) continue;
                    results.Add(result);
                    if (results.Count > 1 &&
                        results[^2].End > result.End)
                        results.Sort((a, b) => a.End.CompareTo(b.End));
                    if (_dependents.TryGetValue(key, out var callers))
                        foreach (var caller in callers) Schedule(caller);
                }
            }

            // Prefer the derivation consuming the most input, matching the
            // growth behavior of an ANTLR left-recursive rule.
            return _memo[root]
                .OrderByDescending(r => r.End)
                .FirstOrDefault()?.Events.ToList();
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
                EventSequence<ParseEvent>.Empty));
            visited.Add(new StateKey(_atn.start[key.Rule].stateNumber, key.Start));

            while (work.Count > 0)
            {
                var item = work.Dequeue();
                if (_progress != null)
                    _progress.FarthestPosition = Math.Max(
                        _progress.FarthestPosition, item.Position);
                if (_atn.stop.Contains(item.State) &&
                    item.State.ruleIndex == key.Rule)
                {
                    bool precedenceRule = _atn.start[key.Rule].isPrecedenceRule;
                    var events = WrapRuleEvents(item.Events, key.Rule,
                        precedenceRule);
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
                                _resultEnds.Add(childKey, new HashSet<int>());
                                Schedule(childKey);
                            }
                            AddDependency(childKey, key);
                            foreach (var child in children)
                            {
                                if (!visited.Add(new StateKey(
                                        rule.target.stateNumber, child.End)))
                                    continue;
                                var events = EventSequence<ParseEvent>.Concat(
                                    item.Events, child.Events);
                                work.Enqueue(new WorkItem(
                                    rule.target, child.End, events));
                            }
                            break;
                        }

                        case MyPrecedencePredicateTransition predicate:
                            if (predicate.precedence >= key.Precedence)
                                QueueUnchanged(work, visited,
                                    transition.target, item);
                            break;

                        case MyPredicateTransition predicate:
                            if (ParserPredicateMatches(predicate, item.Position))
                                QueueUnchanged(work, visited,
                                    transition.target, item);
                            break;

                        case MyAtomTransition or MyRangeTransition or
                             MySetTransition or MyNotSetTransition or
                             MyWildcardTransition:
                            if (item.Position < _onChannel.Count)
                            {
                                int tokenIndex = _onChannel[item.Position];
                                if (transition.Matches(
                                        _tokens[tokenIndex].Type, 1,
                                        _atn.maxTokenType) &&
                                    visited.Add(new StateKey(
                                        transition.target.stateNumber,
                                        item.Position + 1)))
                                {
                                    var events = EventSequence<ParseEvent>.Append(
                                        item.Events, ParseEvent.Consume(tokenIndex));
                                    work.Enqueue(new WorkItem(
                                        transition.target,
                                        item.Position + 1, events));
                                }
                            }
                            break;

                        default:
                            // Epsilon, action and ordinary predicate edges do
                            // not consume input in the interpreted runtime.
                            QueueUnchanged(work, visited,
                                transition.target, item);
                            break;
                    }
                }
            }

            static void QueueUnchanged(Queue<WorkItem> work,
                HashSet<StateKey> visited, MyATNState target, WorkItem item)
            {
                if (visited.Add(new StateKey(target.stateNumber, item.Position)))
                    work.Enqueue(new WorkItem(target, item.Position,
                        item.Events));
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
    }

    private static EventSequence<ParseEvent> WrapRuleEvents(
        EventSequence<ParseEvent> body, int rule, bool precedenceRule) =>
        EventSequence<ParseEvent>.Concat(
            EventSequence<ParseEvent>.One(precedenceRule
                ? ParseEvent.EnterRecursionRule(rule)
                : ParseEvent.EnterRule(rule)),
            EventSequence<ParseEvent>.Concat(body,
                EventSequence<ParseEvent>.One(precedenceRule
                    ? ParseEvent.ExitRecursionRule(rule)
                    : ParseEvent.ExitRule(rule))));

    private readonly record struct LexPosition(int Offset, int Mode, string ModeStack);
    private readonly record struct ContextRuleKey(int Rule, LexPosition Start, int Precedence);
    private readonly record struct ContextStateKey(int State, LexPosition Position);
    private sealed record ContextResult(LexPosition End,
        EventSequence<ParseEvent> Events, EventSequence<int> TokenIndices);
    private sealed record ContextWorkItem(MyATNState State, LexPosition Position,
        EventSequence<ParseEvent> Events, EventSequence<int> TokenIndices);
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
        private readonly Dictionary<ContextRuleKey, HashSet<LexPosition>> _resultEnds = new();
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
            _resultEnds.Add(root, new HashSet<LexPosition>());
            Schedule(root);
            while (_pending.Count > 0)
            {
                var key = _pending.Dequeue();
                _queued.Remove(key);
                foreach (var result in Evaluate(key))
                {
                    var results = _memo[key];
                    if (!_resultEnds[key].Add(result.End)) continue;
                    results.Add(result);
                    if (results.Count > 1 &&
                        results[^2].End.Offset > result.End.Offset)
                        results.Sort((a, b) => a.End.Offset.CompareTo(b.End.Offset));
                    if (_dependents.TryGetValue(key, out var callers))
                        foreach (var caller in callers) Schedule(caller);
                }
            }

            var best = _memo[root].OrderByDescending(r => r.End.Offset).FirstOrDefault();
            selectedTokens = new TokenStore(_input);
            if (best == null) return null;
            var indexMap = new Dictionary<int, int>();
            foreach (int index in best.TokenIndices.Items())
            {
                if (indexMap.ContainsKey(index)) continue;
                var token = _allTokens[index];
                indexMap.Add(index, selectedTokens.Add(token.Type, token.Channel,
                    token.StartIndex, token.StopIndex, token.Line, token.Column,
                    token.Type == EofType ? "<EOF>" : null));
            }
            var events = new List<ParseEvent>(best.Events.Count);
            foreach (var e in best.Events.Items())
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
                EventSequence<ParseEvent>.Empty, EventSequence<int>.Empty));
            visited.Add(new ContextStateKey(
                _atn.start[key.Rule].stateNumber, key.Start));
            while (work.Count > 0)
            {
                var item = work.Dequeue();
                if (_progress != null)
                    _progress.FarthestPosition = Math.Max(
                        _progress.FarthestPosition, item.Position.Offset);
                if (_atn.stop.Contains(item.State) &&
                    item.State.ruleIndex == key.Rule)
                {
                    bool precedenceRule = _atn.start[key.Rule].isPrecedenceRule;
                    var events = WrapRuleEvents(item.Events, key.Rule,
                        precedenceRule);
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
                                _resultEnds.Add(childKey, new HashSet<LexPosition>());
                                Schedule(childKey);
                            }
                            AddDependency(childKey, key);
                            foreach (var child in children)
                            {
                                if (!visited.Add(new ContextStateKey(
                                        rule.target.stateNumber, child.End)))
                                    continue;
                                work.Enqueue(new ContextWorkItem(rule.target,
                                    child.End,
                                    EventSequence<ParseEvent>.Concat(
                                        item.Events, child.Events),
                                    EventSequence<int>.Concat(
                                        item.TokenIndices, child.TokenIndices)));
                            }
                            break;
                        }
                        case MyPrecedencePredicateTransition predicate:
                            if (predicate.precedence >= key.Precedence)
                                QueueUnchanged(work, visited,
                                    transition.target, item);
                            break;
                        case MyPredicateTransition predicate:
                            if (ParserPredicateMatches(predicate, item.Position))
                                QueueUnchanged(work, visited,
                                    transition.target, item);
                            break;
                        case MyAtomTransition or MyRangeTransition or
                             MySetTransition or MyNotSetTransition or
                             MyWildcardTransition:
                        {
                            var step = Lex(item.Position, transition,
                                ExpectedTypes(transition));
                            if (step == null || !transition.Matches(
                                    _allTokens[step.TokenIndex].Type, 1,
                                    _atn.maxTokenType) ||
                                !visited.Add(new ContextStateKey(
                                    transition.target.stateNumber, step.End)))
                                break;
                            var events = EventSequence<ParseEvent>.Append(
                                item.Events, ParseEvent.Consume(step.TokenIndex));
                            var indices = EventSequence<int>.Concat(
                                item.TokenIndices,
                                EventSequence<int>.From(step.AllTokenIndices));
                            work.Enqueue(new ContextWorkItem(transition.target,
                                step.End, events, indices));
                            break;
                        }
                        default:
                            QueueUnchanged(work, visited,
                                transition.target, item);
                            break;
                    }
                }
            }

            static void QueueUnchanged(Queue<ContextWorkItem> work,
                HashSet<ContextStateKey> visited, MyATNState target,
                ContextWorkItem item)
            {
                if (visited.Add(new ContextStateKey(
                        target.stateNumber, item.Position)))
                    work.Enqueue(new ContextWorkItem(target, item.Position,
                        item.Events, item.TokenIndices));
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
    }
}
