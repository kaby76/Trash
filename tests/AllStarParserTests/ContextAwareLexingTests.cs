using AllStarAtnParser;
using Atn;
using EarleyAtnParser;
using Xunit;

namespace AllStarParserTests;

public class ContextAwareLexingTests
{
    private const int Name = 1;
    private const int Colon = 2;
    private const int Value = 3;

    [Fact]
    public void PrefersExpectedTokenBeforeMaximalMunch()
    {
        var lexer = new LexerAtnSimulator(BuildLexerAtn());
        var ordinary = lexer.Tokenize("BDAY:2000-01-01");
        Assert.Equal(Value, ordinary[0].Type);
        Assert.Equal("BDAY:2000-01-01", ordinary[0].Text);

        var cursor = new LexerAtnSimulator.Cursor();
        var contextual = lexer.NextToken(
            "BDAY:2000-01-01", cursor, new HashSet<int> { Name });
        Assert.Equal(Name, contextual.Type);
        Assert.Equal("BDAY", contextual.Text);
    }

    [Fact]
    public void FallsBackToOrdinaryLexerPolicyWhenExpectedRuleDoesNotMatch()
    {
        var lexer = new LexerAtnSimulator(BuildLexerAtn());
        var cursor = new LexerAtnSimulator.Cursor();

        var token = lexer.NextToken(
            "BDAY:2000-01-01", cursor, new HashSet<int> { Colon });

        Assert.Equal(Value, token.Type);
        Assert.Equal("BDAY:2000-01-01", token.Text);
    }

    [Fact]
    public void AllStarParserDrivesContextAwareTokenization()
    {
        var parserAtn = BuildParserAtn();
        var lexerAtn = BuildLexerAtn();
        var input = "BDAY:2000-01-01";

        var ordinaryTokens = new LexerAtnSimulator(lexerAtn).Tokenize(input);
        Assert.Null(AllStarParser.Parse(parserAtn, ordinaryTokens, 0));

        var events = AllStarParser.ParseContextAware(
            parserAtn, lexerAtn, input, 0, out var contextualTokens);

        Assert.NotNull(events);
        Assert.Equal(
            new[] { (Name, "BDAY"), (Colon, ":"), (Value, "2000-01-01"), (-1, "<EOF>") },
            contextualTokens
                .Where(token => token.Channel == 0 || token.Type == -1)
                .Select(token => (token.Type, token.Text))
                .ToArray());
    }

    [Fact]
    public void InterpRunnerParsesIssue708Vcard()
    {
        var directory = Path.Combine(
            AppContext.BaseDirectory, "TestData", "context-aware");
        var parserInterp = Path.Combine(directory, "Overlap.interp");
        var lexerInterp = Path.Combine(directory, "OverlapLexer.interp");
        const string input =
            "BEGIN:VCARD\r\n" +
            "BDAY:2000-01-01T00:00:00\r\n" +
            "END:VCARD\r\n";

        Assert.Throws<InvalidOperationException>(() =>
            AllStarAtnParser.InterpRunner.Run(
                parserInterp, lexerInterp, input, "vcard.txt",
                lineNumbers: false, contextAwareLexing: false));

        var (result, tokenCount) = AllStarAtnParser.InterpRunner.Run(
            parserInterp, lexerInterp, input, "vcard.txt",
            lineNumbers: false, contextAwareLexing: true);

        Assert.NotNull(result);
        Assert.Equal(9, tokenCount);
    }

    private static MyATN BuildParserAtn()
    {
        var states = Enumerable.Range(0, 6)
            .Select(number => new MyATNState
            {
                stateNumber = number,
                ruleIndex = 0,
                stateType = number switch
                {
                    0 => MyStateType.RuleStart,
                    5 => MyStateType.RuleStop,
                    _ => MyStateType.Basic
                }
            })
            .ToArray();
        states[0].AddTransition(new MyAtomTransition(states[1], Name));
        states[1].AddTransition(new MyAtomTransition(states[2], Colon));
        states[2].AddTransition(new MyAtomTransition(states[3], Value));
        states[3].AddTransition(new MyAtomTransition(states[4], -1));
        states[4].AddTransition(new MyEpsilonTransition(states[5]));

        return new MyATN
        {
            grammarType = MyATNType.Parser,
            maxTokenType = Value,
            allStates = states,
            start = [states[0]],
            ruleToStopState = [states[5]],
            stop = new HashSet<MyATNState> { states[5] }
        };
    }

    private static MyATN BuildLexerAtn()
    {
        var states = new List<MyATNState>();
        var modeStart = State(MyStateType.TokenStart, -1, states);

        var nameStart = State(MyStateType.RuleStart, 0, states);
        var nameLoop = State(MyStateType.Basic, 0, states);
        var nameStop = State(MyStateType.RuleStop, 0, states);
        modeStart.AddTransition(new MyEpsilonTransition(nameStart));
        nameStart.AddTransition(new MyRangeTransition(nameLoop, 'A', 'Z'));
        nameLoop.AddTransition(new MyRangeTransition(nameLoop, 'A', 'Z'));
        nameLoop.AddTransition(new MyEpsilonTransition(nameStop));

        var colonStart = State(MyStateType.RuleStart, 1, states);
        var colonStop = State(MyStateType.RuleStop, 1, states);
        modeStart.AddTransition(new MyEpsilonTransition(colonStart));
        colonStart.AddTransition(new MyAtomTransition(colonStop, ':'));

        var valueStart = State(MyStateType.RuleStart, 2, states);
        var valueLoop = State(MyStateType.Basic, 2, states);
        var valueStop = State(MyStateType.RuleStop, 2, states);
        modeStart.AddTransition(new MyEpsilonTransition(valueStart));
        AddValueTransitions(valueStart, valueLoop);
        AddValueTransitions(valueLoop, valueLoop);
        valueLoop.AddTransition(new MyEpsilonTransition(valueStop));

        return new MyATN
        {
            grammarType = MyATNType.Lexer,
            maxTokenType = Value,
            allStates = states.ToArray(),
            start = [nameStart, colonStart, valueStart],
            ruleToStopState = [nameStop, colonStop, valueStop],
            ruleToTokenType = [Name, Colon, Value],
            modeToStartState = [modeStart]
        };
    }

    private static void AddValueTransitions(MyATNState source, MyATNState target)
    {
        source.AddTransition(new MyRangeTransition(target, 'A', 'Z'));
        source.AddTransition(new MyRangeTransition(target, '0', '9'));
        source.AddTransition(new MyAtomTransition(target, ':'));
        source.AddTransition(new MyAtomTransition(target, '-'));
    }

    private static MyATNState State(
        MyStateType type, int ruleIndex, List<MyATNState> states)
    {
        var state = new MyATNState
        {
            stateType = type,
            ruleIndex = ruleIndex,
            stateNumber = states.Count
        };
        states.Add(state);
        return state;
    }
}
