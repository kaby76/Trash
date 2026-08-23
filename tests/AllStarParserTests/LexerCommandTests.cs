using Atn;
using EarleyAtnParser;
using Xunit;

namespace AllStarParserTests;

public class LexerCommandTests
{
    [Fact]
    public void ExecutesModeAndTokenCommands()
    {
        var atn = BuildCommandAtn();

        var tokens = new LexerAtnSimulator(atn).Tokenize("<x>[y]stc");

        Assert.Collection(tokens,
            token => AssertToken(token, 1, 0, "<"),
            token => AssertToken(token, 6, 0, "x"),
            token => AssertToken(token, 7, 0, ">"),
            token => AssertToken(token, 2, 0, "["),
            token => AssertToken(token, 8, 0, "y"),
            token => AssertToken(token, 9, 0, "]"),
            token => AssertToken(token, 3, LexerToken.SKIP_CHANNEL, "s"),
            token => AssertToken(token, 42, 0, "t"),
            token => AssertToken(token, 5, 7, "c"),
            token => AssertToken(token, -1, 0, "<EOF>"));
    }

    [Fact]
    public void PopModeWithEmptyStackThrows()
    {
        var atn = BuildAtn(
            modeCount: 1,
            new RuleSpec(0, '!', 1, new(MyLexerActionType.PopMode, 0, 0)));

        var exception = Assert.Throws<InvalidOperationException>(
            () => new LexerAtnSimulator(atn).Tokenize("!"));

        Assert.Contains("mode stack is empty", exception.Message);
    }

    [Fact]
    public void IgnoresActionsInReferencedLexerRules()
    {
        var modeStart = State(MyStateType.TokenStart, -1, 0);
        var outerStart = State(MyStateType.RuleStart, 0, 1);
        var afterCall = State(MyStateType.Basic, 0, 2);
        var outerStop = State(MyStateType.RuleStop, 0, 3);
        var innerStart = State(MyStateType.RuleStart, 1, 4);
        var innerAction = State(MyStateType.Basic, 1, 5);
        var innerStop = State(MyStateType.RuleStop, 1, 6);

        modeStart.AddTransition(new MyEpsilonTransition(outerStart));
        outerStart.AddTransition(new MyRuleTransition(afterCall, 1, 0));
        afterCall.AddTransition(new MyEpsilonTransition(outerStop));
        innerStart.AddTransition(new MyAtomTransition(innerAction, 'z'));
        innerAction.AddTransition(new MyActionTransition(innerStop, 1, 0, false));

        var atn = new MyATN
        {
            grammarType = MyATNType.Lexer,
            modeToStartState = [modeStart],
            start = [outerStart, innerStart],
            ruleToStopState = [outerStop, innerStop],
            ruleToTokenType = [1, 2],
            lexerActions = [new MyLexerAction(MyLexerActionType.Channel, 7, 0)],
            allStates =
            [
                modeStart, outerStart, afterCall, outerStop,
                innerStart, innerAction, innerStop
            ]
        };

        var tokens = new LexerAtnSimulator(atn).Tokenize("z");

        AssertToken(tokens[0], 1, 0, "z");
    }

    [Fact]
    public void NonGreedyLexerLoopStopsAtFirstAccept()
    {
        var interp = InterpFileReader.Read(File.ReadAllText(
            NativeTreeTestSupport.SysML.LexerInterp));
        var atn = AtnDeserializer.Deserialize(interp.AtnData);

        var comments = new LexerAtnSimulator(atn)
            .Tokenize("/* first */ /* second */")
            .Where(token => token.Type == 223)
            .Select(token => token.Text)
            .ToArray();

        Assert.Equal(["/* first */", "/* second */"], comments);
    }

    private static MyATN BuildCommandAtn() => BuildAtn(
        modeCount: 3,
        new RuleSpec(0, '<', 1, new(MyLexerActionType.PushMode, 1, 0)),
        new RuleSpec(0, '[', 2, new(MyLexerActionType.Mode, 2, 0)),
        new RuleSpec(0, 's', 3, new(MyLexerActionType.Skip, 0, 0)),
        new RuleSpec(0, 't', 4, new(MyLexerActionType.Type, 42, 0)),
        new RuleSpec(0, 'c', 5, new(MyLexerActionType.Channel, 7, 0)),
        new RuleSpec(1, 'x', 6),
        new RuleSpec(1, '>', 7, new(MyLexerActionType.PopMode, 0, 0)),
        new RuleSpec(2, 'y', 8),
        new RuleSpec(2, ']', 9, new(MyLexerActionType.Mode, 0, 0)));

    private static MyATN BuildAtn(int modeCount, params RuleSpec[] rules)
    {
        var atn = new MyATN
        {
            grammarType = MyATNType.Lexer,
            modeToStartState = new MyATNState[modeCount],
            start = new MyATNState[rules.Length],
            ruleToStopState = new MyATNState[rules.Length],
            ruleToTokenType = rules.Select(rule => rule.TokenType).ToArray(),
            lexerActions = rules.Where(rule => rule.Action != null)
                .Select(rule => rule.Action!)
                .ToArray()
        };

        var states = new List<MyATNState>();
        for (var mode = 0; mode < modeCount; mode++)
        {
            var modeStart = State(MyStateType.TokenStart, -1, states.Count);
            states.Add(modeStart);
            atn.modeToStartState[mode] = modeStart;
        }

        var actionIndex = 0;
        for (var ruleIndex = 0; ruleIndex < rules.Length; ruleIndex++)
        {
            var rule = rules[ruleIndex];
            var start = State(MyStateType.RuleStart, ruleIndex, states.Count);
            states.Add(start);
            var afterCharacter = State(MyStateType.Basic, ruleIndex, states.Count);
            states.Add(afterCharacter);
            var stop = State(MyStateType.RuleStop, ruleIndex, states.Count);
            states.Add(stop);

            atn.modeToStartState[rule.Mode].AddTransition(new MyEpsilonTransition(start));
            start.AddTransition(new MyAtomTransition(afterCharacter, rule.Character));
            if (rule.Action == null)
                afterCharacter.AddTransition(new MyEpsilonTransition(stop));
            else
                afterCharacter.AddTransition(
                    new MyActionTransition(stop, ruleIndex, actionIndex++, false));

            atn.start[ruleIndex] = start;
            atn.ruleToStopState[ruleIndex] = stop;
        }

        atn.allStates = states.ToArray();
        return atn;
    }

    private static MyATNState State(MyStateType type, int ruleIndex, int number) =>
        new() { stateType = type, ruleIndex = ruleIndex, stateNumber = number };

    private static void AssertToken(LexerToken token, int type, int channel, string text)
    {
        Assert.Equal(type, token.Type);
        Assert.Equal(channel, token.Channel);
        Assert.Equal(text, token.Text);
    }

    private sealed record RuleSpec(
        int Mode,
        char Character,
        int TokenType,
        MyLexerAction? Action = null);
}
