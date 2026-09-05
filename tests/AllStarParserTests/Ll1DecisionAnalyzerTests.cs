using AllStarAtnParser;
using Atn;
using Xunit;

namespace AllStarParserTests;

public sealed class Ll1DecisionAnalyzerTests
{
    [Fact]
    public void DisjointAlternativesAreEligibleIncludingEof()
    {
        var (atn, decision, first, second, stop, _) = CreateDecision(3);
        first.AddTransition(new MyAtomTransition(stop, -1));
        second.AddTransition(new MyAtomTransition(stop, 2));

        var analyzer = Ll1DecisionAnalyzer.For(atn);

        Assert.True(analyzer.IsEligible(decision));
        Assert.True(analyzer.TryPredict(decision, -1, out int eofAlt));
        Assert.Equal(1, eofAlt);
        Assert.True(analyzer.TryPredict(decision, 2, out int tokenAlt));
        Assert.Equal(2, tokenAlt);
    }

    [Fact]
    public void OverlapNullableAndPredicateDecisionsStayOnAllStarPath()
    {
        var overlap = CreateDecision(2);
        overlap.first.AddTransition(new MyAtomTransition(overlap.stop, 1));
        overlap.second.AddTransition(new MyAtomTransition(overlap.stop, 1));
        Assert.False(Ll1DecisionAnalyzer.For(overlap.atn)
            .IsEligible(overlap.decision));

        var nullable = CreateDecision(2);
        nullable.first.AddTransition(new MyAtomTransition(nullable.stop, 1));
        nullable.second.AddTransition(new MyEpsilonTransition(nullable.stop));
        Assert.False(Ll1DecisionAnalyzer.For(nullable.atn)
            .IsEligible(nullable.decision));

        var predicate = CreateDecision(2, extraState: true);
        predicate.first.AddTransition(new MyAtomTransition(predicate.stop, 1));
        predicate.second.AddTransition(new MyPredicateTransition(
            predicate.extra, ruleIndex: 0, predIndex: 0,
            isCtxDependent: true));
        predicate.extra.AddTransition(new MyAtomTransition(predicate.stop, 2));
        Assert.False(Ll1DecisionAnalyzer.For(predicate.atn)
            .IsEligible(predicate.decision));
    }

    private static (MyATN atn, int decision, MyATNState first,
        MyATNState second, MyATNState stop, MyATNState extra) CreateDecision(
        int maxTokenType, bool extraState = false)
    {
        var decision = State(0, MyStateType.BlockStart);
        var first = State(1, MyStateType.Basic);
        var second = State(2, MyStateType.Basic);
        var stop = State(3, MyStateType.RuleStop);
        var extra = extraState ? State(4, MyStateType.Basic) : stop;
        decision.AddTransition(new MyEpsilonTransition(first));
        decision.AddTransition(new MyEpsilonTransition(second));
        var states = extraState
            ? new[] { decision, first, second, stop, extra }
            : new[] { decision, first, second, stop };
        return (new MyATN
        {
            maxTokenType = maxTokenType,
            allStates = states,
            decisionToState = new[] { decision }
        }, 0, first, second, stop, extra);
    }

    private static MyATNState State(int number, MyStateType type) => new()
    {
        stateNumber = number,
        stateType = type,
        ruleIndex = 0
    };
}
