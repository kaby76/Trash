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

    [Fact]
    public void NonGreedyLexerLoopRemainsNonGreedyAcrossGreedySuffix()
    {
        var interp = InterpFileReader.Read(File.ReadAllText(
            NativeTreeTestSupport.Asl.LexerInterp));
        var atn = AtnDeserializer.Deserialize(interp.AtnData);

        var tokens = new LexerAtnSimulator(atn)
            .Tokenize("# first\nvalue = TRUE\n# second\n")
            .Where(token => token.Channel == 0 && token.Type != -1)
            .Select(token => token.Text)
            .ToArray();

        Assert.Equal(["value", "=", "TRUE", "\n"], tokens);
    }

    [Fact]
    public void NonGreedyLexerLoopPreservesHigherPriorityEscapedDelimiter()
    {
        var lexerInterp = Path.Combine(
            AppContext.BaseDirectory, "TestData", "corundum", "interp",
            "CorundumLexer.interp");
        var interp = InterpFileReader.Read(File.ReadAllText(lexerInterp));
        var atn = AtnDeserializer.Deserialize(interp.AtnData);

        var literals = new LexerAtnSimulator(atn)
            .Tokenize("\"first\" \"Hello, I'm CORUN\\\"DUM\"")
            .Where(token => token.Channel == 0 && token.Type != -1)
            .Select(token => token.Text)
            .ToArray();

        Assert.Equal(["\"first\"", "\"Hello, I'm CORUN\\\"DUM\""], literals);
    }

    [Fact]
    public void NonGreedyLoopInFragmentDoesNotConsumeLaterOuterTokens()
    {
        var tokens = new LexerAtnSimulator(BuildNestedNonGreedyAtn())
            .Tokenize("<<a>>]<<b>>")
            .Where(token => token.Type != -1)
            .Select(token => token.Text)
            .ToArray();

        Assert.Equal(["<<a>>", "]", "<<b>>"], tokens);
    }

    [Fact]
    public void MutuallyRecursiveFragmentsReuseCanonicalContexts()
    {
        var simulator = new LexerAtnSimulator(BuildMutuallyRecursiveAtn());

        var first = simulator.Tokenize("<abab>");
        var contextsAfterFirstRun = simulator.LexerContextCount;
        var second = simulator.Tokenize("<abab>");

        Assert.Equal(["<abab>"], first.Where(t => t.Type != -1).Select(t => t.Text));
        Assert.Equal(first.Select(t => t.Text), second.Select(t => t.Text));
        Assert.True(contextsAfterFirstRun >= 4);
        Assert.Equal(contextsAfterFirstRun, simulator.LexerContextCount);
    }

    [Fact]
    public void NonAsciiDfaEdgesUseSparseStorageAndAreCached()
    {
        var simulator = new LexerAtnSimulator(BuildAtn(
            modeCount: 1, new RuleSpec(0, '\u00E9', 1)));

        var first = simulator.Tokenize("\u00E9\u00E9");
        var missesAfterFirstRun = simulator.DfaEdgeCacheMisses;
        var second = simulator.Tokenize("\u00E9\u00E9");
        var statistics = simulator.GetDfaStatistics();

        Assert.Equal(["\u00E9", "\u00E9"],
            first.Where(t => t.Type != -1).Select(t => t.Text));
        Assert.Equal(first.Select(t => t.Text), second.Select(t => t.Text));
        Assert.True(statistics.SparseEntries > 0);
        Assert.True(statistics.LiveTransitions > 0);
        Assert.Equal(missesAfterFirstRun, simulator.DfaEdgeCacheMisses);
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

    private static MyATN BuildNestedNonGreedyAtn()
    {
        var states = new List<MyATNState>();
        MyATNState New(MyStateType type, int rule)
        {
            var state = State(type, rule, states.Count);
            states.Add(state);
            return state;
        }

        var modeStart = New(MyStateType.TokenStart, -1);

        // HTML : '<' (TAG | ~[<>])* '>' ;
        var htmlStart = New(MyStateType.RuleStart, 0);
        var htmlLoop = New(MyStateType.StarLoopEntry, 0);
        var htmlClose = New(MyStateType.Basic, 0);
        var htmlStop = New(MyStateType.RuleStop, 0);
        htmlStart.AddTransition(new MyAtomTransition(htmlLoop, '<'));
        htmlLoop.AddTransition(new MyRuleTransition(htmlLoop, 1, 0));
        var angles = MyIntervalSet.Of('<');
        angles.Add('>', '>');
        htmlLoop.AddTransition(new MyNotSetTransition(htmlLoop, angles));
        htmlLoop.AddTransition(new MyEpsilonTransition(htmlClose));
        htmlClose.AddTransition(new MyAtomTransition(htmlStop, '>'));

        // fragment TAG : '<' .*? '>' ;
        var tagStart = New(MyStateType.RuleStart, 1);
        var tagLoop = New(MyStateType.StarLoopEntry, 1);
        tagLoop.nonGreedy = true;
        var tagClose = New(MyStateType.Basic, 1);
        var tagStop = New(MyStateType.RuleStop, 1);
        tagStart.AddTransition(new MyAtomTransition(tagLoop, '<'));
        tagLoop.AddTransition(new MyEpsilonTransition(tagClose));
        tagLoop.AddTransition(new MyWildcardTransition(tagLoop));
        tagClose.AddTransition(new MyAtomTransition(tagStop, '>'));

        // CAB : ']' ;
        var cabStart = New(MyStateType.RuleStart, 2);
        var cabStop = New(MyStateType.RuleStop, 2);
        cabStart.AddTransition(new MyAtomTransition(cabStop, ']'));

        modeStart.AddTransition(new MyEpsilonTransition(htmlStart));
        modeStart.AddTransition(new MyEpsilonTransition(cabStart));

        return new MyATN
        {
            grammarType = MyATNType.Lexer,
            maxTokenType = 2,
            allStates = states.ToArray(),
            start = [htmlStart, tagStart, cabStart],
            ruleToStopState = [htmlStop, tagStop, cabStop],
            ruleToTokenType = [1, 0, 2],
            modeToStartState = [modeStart]
        };
    }

    private static MyATN BuildMutuallyRecursiveAtn()
    {
        var states = new List<MyATNState>();
        MyATNState New(MyStateType type, int rule)
        {
            var state = State(type, rule, states.Count);
            states.Add(state);
            return state;
        }

        var modeStart = New(MyStateType.TokenStart, -1);

        // TOKEN : '<' A '>' ;
        var tokenStart = New(MyStateType.RuleStart, 0);
        var tokenCall = New(MyStateType.Basic, 0);
        var tokenClose = New(MyStateType.Basic, 0);
        var tokenStop = New(MyStateType.RuleStop, 0);
        tokenStart.AddTransition(new MyAtomTransition(tokenCall, '<'));
        tokenCall.AddTransition(new MyRuleTransition(tokenClose, 1, 0));
        tokenClose.AddTransition(new MyAtomTransition(tokenStop, '>'));

        // fragment A : 'a' B? ;
        var aStart = New(MyStateType.RuleStart, 1);
        var aChoice = New(MyStateType.Basic, 1);
        var aStop = New(MyStateType.RuleStop, 1);
        aStart.AddTransition(new MyAtomTransition(aChoice, 'a'));
        aChoice.AddTransition(new MyRuleTransition(aStop, 2, 0));
        aChoice.AddTransition(new MyEpsilonTransition(aStop));

        // fragment B : 'b' A? ;
        var bStart = New(MyStateType.RuleStart, 2);
        var bChoice = New(MyStateType.Basic, 2);
        var bStop = New(MyStateType.RuleStop, 2);
        bStart.AddTransition(new MyAtomTransition(bChoice, 'b'));
        bChoice.AddTransition(new MyRuleTransition(bStop, 1, 0));
        bChoice.AddTransition(new MyEpsilonTransition(bStop));

        modeStart.AddTransition(new MyEpsilonTransition(tokenStart));

        return new MyATN
        {
            grammarType = MyATNType.Lexer,
            maxTokenType = 1,
            allStates = states.ToArray(),
            start = [tokenStart, aStart, bStart],
            ruleToStopState = [tokenStop, aStop, bStop],
            ruleToTokenType = [1, 0, 0],
            modeToStartState = [modeStart]
        };
    }

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

    [Fact]
    public void WarmDfaRetainsTypeRemappedAcceptCandidates()
    {
        var lexer = new LexerAtnSimulator(BuildAtn(
            modeCount: 1,
            new RuleSpec(0, 't', 3,
                new MyLexerAction(MyLexerActionType.Type, 42, 0)),
            new RuleSpec(0, 't', 7)));

        var remapped = lexer.NextToken(
            "t", new LexerAtnSimulator.Cursor(), new HashSet<int> { 42 });
        Assert.Equal(42, remapped.Type);
        var warm = lexer.GetDfaStatistics();

        var secondRule = lexer.NextToken(
            "t", new LexerAtnSimulator.Cursor(), new HashSet<int> { 7 });
        Assert.Equal(7, secondRule.Type);
        var final = lexer.GetDfaStatistics();

        Assert.Equal(warm.States, final.States);
        Assert.Equal(warm.CacheMisses, final.CacheMisses);
        Assert.True(final.CacheHits > warm.CacheHits);
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
