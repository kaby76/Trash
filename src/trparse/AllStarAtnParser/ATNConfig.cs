namespace AllStarAtnParser;

using Atn;
using EarleyAtnParser;

// A single configuration in the ALL(*) ATN simulation:
// the ATN state we're at, which alternative we're tracking (1-indexed),
// and the prediction context (call stack above this point).

// Absolutely no Antlr4.Runtime.Standard types used anywhere in this
// file!

public sealed class ATNConfig
{
    public readonly MyATNState State;
    public readonly int Alt;       // 1-indexed alternative being explored
    public readonly PredictionContext Context;

    public ATNConfig(MyATNState state, int alt, PredictionContext context)
    {
        State = state;
        Alt = alt;
        Context = context;
    }

    public ATNConfig WithState(MyATNState state) =>
        new(state, Alt, Context);

    public ATNConfig WithStateAndContext(MyATNState state, PredictionContext ctx) =>
        new(state, Alt, ctx);
}
