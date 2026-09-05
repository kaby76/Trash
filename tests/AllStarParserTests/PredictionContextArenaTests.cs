using AllStarAtnParser;
using Xunit;

namespace AllStarParserTests;

public sealed class PredictionContextArenaTests
{
    [Fact]
    public void CanonicalizesSingletonAndArrayContexts()
    {
        var arena = new PredictionContextArena();
        var left = arena.GetChild(PredictionContext.EMPTY, 10, 2);
        var sameLeft = arena.GetChild(PredictionContext.EMPTY, 10, 2);
        var right = arena.GetChild(PredictionContext.EMPTY, 20, 0);

        Assert.Same(left, sameLeft);
        Assert.Equal(left.Id, sameLeft.Id);

        var first = arena.GetArray(
            new PredictionContext[] { left, right },
            new[] { 30, 40 }, new[] { 0, 1 });
        var second = arena.GetArray(
            new PredictionContext[] { left, right },
            new[] { 30, 40 }, new[] { 0, 1 });

        Assert.Same(first, second);
        Assert.Equal(first.Id, second.Id);
        Assert.Equal(3, arena.Creations);
        Assert.Equal(2, arena.Hits);
    }

    [Fact]
    public void InternedIdentityHandlesDeepRecursiveCallChains()
    {
        var arena = new PredictionContextArena();
        PredictionContext context = PredictionContext.EMPTY;
        const int depth = 10_000;
        for (int i = 0; i < depth; i++)
            context = arena.GetChild(context, i + 1, i & 3);

        var sameLeaf = arena.GetChild(context.Parent, context.ReturnState,
            context.GetPrecedence(0));

        Assert.Same(context, sameLeaf);
        Assert.Equal(depth + 1, arena.Count);
    }

    [Fact]
    public void MergeResultsAreCanonicalized()
    {
        var arena = new PredictionContextArena();
        var left = arena.GetChild(PredictionContext.EMPTY, 10);
        var right = arena.GetChild(PredictionContext.EMPTY, 20);

        var first = PredictionContextMerger.Merge(arena, left, right);
        var second = PredictionContextMerger.Merge(arena, right, left);

        Assert.Same(first, second);
        Assert.Equal(2, first.Size);
    }
}
