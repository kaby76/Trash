namespace AllStarAtnParser;

using System.Text.Json;
using Atn;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.Parser;
using XQuery.Parser.Ast;

/// <summary>
/// Per-input, target-neutral XQuery4 hooks for the interpreted parser.
/// Predicates are read-only; parser-rule hooks commit symbol-table changes.
/// </summary>
public sealed class XQueryHooks
{
    private const string ContextNamespace = "urn:trash:context";
    private readonly string _input;
    private readonly string[] _parserRules;
    private readonly string[] _tokenNames;
    private readonly Dictionary<(int Rule, int Predicate), ExprNode> _lexerPredicates = new();
    private readonly Dictionary<(int Rule, int Predicate), (ExprNode Query, bool Predict)>
        _parserPredicates = new();
    private readonly Dictionary<int, ExprNode> _ruleEnter = new();
    private readonly Dictionary<int, ExprNode> _ruleExit = new();
    private readonly HashSet<string> _declared = new(StringComparer.Ordinal);
    private XdmDocument _tree;
    private bool _inDeclaration;

    public bool HasParserPredicates => _parserPredicates.Count != 0;

    private XQueryHooks(string input, string[] parserRules, string[] tokenNames)
    {
        _input = input;
        _parserRules = parserRules;
        _tokenNames = tokenNames;
        _tree = new XdmDocument();
        _tree.AppendChild(new XdmElement("parse"));
    }

    public static XQueryHooks Load(string path, string input,
        string[] lexerRules, string[] parserRules, string[] tokenNames)
    {
        var manifest = JsonSerializer.Deserialize<Manifest>(File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException($"Empty XQuery hook manifest '{path}'.");
        var hooks = new XQueryHooks(input, parserRules, tokenNames);
        string directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
        foreach (var spec in manifest.LexerPredicates ?? [])
        {
            int rule = Array.IndexOf(lexerRules, spec.Rule);
            if (rule < 0) throw new InvalidDataException($"Unknown lexer rule '{spec.Rule}'.");
            hooks._lexerPredicates.Add((rule, spec.Predicate),
                Compile(directory, spec.Query));
        }
        foreach (var spec in manifest.ParserPredicates ?? [])
        {
            int rule = Array.IndexOf(parserRules, spec.Rule);
            if (rule < 0) throw new InvalidDataException($"Unknown parser rule '{spec.Rule}'.");
            hooks._parserPredicates.Add((rule, spec.Predicate),
                (Compile(directory, spec.Query), spec.Predict));
        }
        foreach (var spec in manifest.ParserRules ?? [])
        {
            int rule = Array.IndexOf(parserRules, spec.Rule);
            if (rule < 0) throw new InvalidDataException($"Unknown parser rule '{spec.Rule}'.");
            if (!string.IsNullOrWhiteSpace(spec.Enter))
                hooks._ruleEnter.Add(rule, Compile(directory, spec.Enter));
            if (!string.IsNullOrWhiteSpace(spec.Exit))
                hooks._ruleExit.Add(rule, Compile(directory, spec.Exit));
        }
        return hooks;
    }

    private static ExprNode Compile(string directory, string file)
    {
        if (string.IsNullOrWhiteSpace(file))
            throw new InvalidDataException("XQuery hook filename is missing.");
        var parser = new XQueryParser(File.ReadAllText(Path.Combine(directory, file)));
        parser.AddNamespace("ctx", ContextNamespace);
        return parser.Parse();
    }

    public bool EvaluateLexerPredicate(int rule, int predicate, int start, int position)
    {
        if (!_lexerPredicates.TryGetValue((rule, predicate), out var expression))
            throw new InvalidOperationException(
                $"No XQuery binding for lexer predicate {rule}:{predicate}.");
        int end = start;
        while (end < _input.Length && IsIdentifierContinuation(_input[end])) end++;
        string candidate = _input[start..end];
        var context = CreateContext();
        context.SetVariable("token-start", Atomic(start));
        context.SetVariable("offset", Atomic(position - start));
        context.SetVariable("candidate", Atomic(candidate));
        context.SetVariable("in-declaration", Atomic(_inDeclaration));
        var name = new XdmQName(ContextNamespace, "declared-prefix", "ctx");
        context.RegisterFunction(name, 2, new XdmFunction(name, 2, args =>
            Atomic(HasDeclaredPrefix(args[0].StringValue,
                int.Parse(args[1].StringValue)))));
        return new XQueryEvaluator(context).Evaluate(expression).EffectiveBooleanValue;
    }

    public bool EvaluateParserPredicate(int rule, int predicate,
        IReadOnlyList<int> tokenTypes, int position,
        IReadOnlyList<ParseEvent> events, IReadOnlyList<LexerToken> tokens,
        bool speculative)
    {
        if (!_parserPredicates.TryGetValue((rule, predicate), out var binding))
            throw new InvalidOperationException(
                $"No XQuery binding for parser predicate {rule}:{predicate}.");
        if (speculative && !binding.Predict) return true;

        var (tree, node) = Snapshot(events, tokens);
        if (!speculative) _tree = tree;
        var context = CreateContext(node);
        context.SetVariable("tree", new XdmSequence(tree));
        context.SetVariable("lookahead1", Atomic(TokenName(tokenTypes, position)));
        context.SetVariable("lookahead2", Atomic(TokenName(tokenTypes, position + 1)));
        return new XQueryEvaluator(context).Evaluate(binding.Query)
            .EffectiveBooleanValue;
    }

    private string TokenName(IReadOnlyList<int> types, int position)
    {
        if (position >= types.Count) return "EOF";
        int type = types[position];
        if (type < 0) return "EOF";
        return type < _tokenNames.Length
            ? _tokenNames[type] ?? type.ToString()
            : type.ToString();
    }

    private bool HasDeclaredPrefix(string candidate, int hyphenOffset)
    {
        foreach (string name in _declared)
            if (hyphenOffset < name.Length &&
                candidate.StartsWith(name, StringComparison.Ordinal) &&
                (candidate.Length == name.Length || candidate[name.Length] == '-'))
                return true;
        return false;
    }

    public void OnRuleEnter(int rule, IReadOnlyList<ParseEvent> events,
        IReadOnlyList<LexerToken> tokens)
    {
        if (!_ruleEnter.TryGetValue(rule, out var expression)) return;
        var (tree, node) = Snapshot(events, tokens);
        _tree = tree;
        _inDeclaration = new XQueryEvaluator(CreateContext(node))
            .Evaluate(expression).EffectiveBooleanValue;
    }

    public void OnRuleExit(int rule, IReadOnlyList<ParseEvent> events,
        IReadOnlyList<LexerToken> tokens)
    {
        if (!_ruleExit.TryGetValue(rule, out var expression)) return;
        try
        {
            var (tree, node) = Snapshot(events, tokens);
            _tree = tree;
            var context = CreateContext(node);
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (events[i].Kind != ParseEventKind.Consume) continue;
                var token = tokens[events[i].Index];
                if (token.Type < 0 || token.Type >= _tokenNames.Length ||
                    _tokenNames[token.Type] != "IDENTIFIER") continue;
                context.SetVariable("start", Atomic(token.StartIndex));
                context.SetVariable("length", Atomic(token.Text.Length));
                break;
            }
            foreach (var item in new XQueryEvaluator(context).Evaluate(expression))
                _declared.Add(item.StringValue);
        }
        finally { _inDeclaration = false; }
    }

    private EvaluationContext CreateContext(XdmItem node = null)
    {
        var context = EvaluationContext.CreateDefault().WithContextItem(node);
        context.SetVariable("tree", new XdmSequence(_tree));
        context.SetVariable("input", Atomic(_input));
        return context;
    }

    private (XdmDocument Tree, XdmElement Current) Snapshot(
        IReadOnlyList<ParseEvent> events, IReadOnlyList<LexerToken> tokens)
    {
        var document = new XdmDocument();
        var root = new XdmElement("parse");
        document.AppendChild(root);
        var stack = new Stack<XdmElement>();
        stack.Push(root);
        foreach (var parseEvent in events)
        {
            switch (parseEvent.Kind)
            {
                case ParseEventKind.EnterRule:
                case ParseEventKind.EnterRecursionRule:
                    var rule = new XdmElement(_parserRules[parseEvent.Index]);
                    stack.Peek().AppendChild(rule);
                    stack.Push(rule);
                    break;
                case ParseEventKind.ExitRule:
                case ParseEventKind.ExitRecursionRule:
                    if (stack.Count > 1) stack.Pop();
                    break;
                case ParseEventKind.Consume:
                    var token = tokens[parseEvent.Index];
                    string tokenName = token.Type >= 0 && token.Type < _tokenNames.Length
                        ? _tokenNames[token.Type] : null;
                    var terminal = new XdmElement(tokenName ?? "terminal");
                    terminal.AppendChild(new XdmText(token.Text));
                    stack.Peek().AppendChild(terminal);
                    break;
            }
        }
        return (document, stack.Peek());
    }

    private static XdmSequence Atomic(string value) =>
        new(new XdmAtomicValue(value));
    private static XdmSequence Atomic(int value) =>
        new(new XdmAtomicValue(value));
    private static XdmSequence Atomic(bool value) =>
        new(new XdmAtomicValue(value));
    private static bool IsIdentifierContinuation(char ch) =>
        ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or
            >= '0' and <= '9' or '-';

    private sealed class Manifest
    {
        public List<LexerPredicateSpec> LexerPredicates { get; set; }
        public List<ParserPredicateSpec> ParserPredicates { get; set; }
        public List<ParserRuleSpec> ParserRules { get; set; }
    }
    private sealed class ParserPredicateSpec
    {
        public string Rule { get; set; }
        public int Predicate { get; set; }
        public string Query { get; set; }
        public bool Predict { get; set; }
    }
    private sealed class LexerPredicateSpec
    {
        public string Rule { get; set; }
        public int Predicate { get; set; }
        public string Query { get; set; }
    }
    private sealed class ParserRuleSpec
    {
        public string Rule { get; set; }
        public string Enter { get; set; }
        public string Exit { get; set; }
    }
}
