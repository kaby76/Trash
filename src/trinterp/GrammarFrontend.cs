using System;
using System.Linq;

namespace trinterp;

/// <summary>
/// Boundary between source notation and the common grammar compiler.
/// A front end preserves declaration order and source locations while lowering
/// its syntax to the backend's grammar/rule/block/alternative/element vocabulary.
/// </summary>
public interface IGrammarFrontend
{
    GrammarNode Lower(GrammarNode syntax);
}

public sealed class Antlr4Frontend : IGrammarFrontend
{
    public GrammarNode Lower(GrammarNode syntax)
    {
        if (syntax.LocalName != "grammarSpec")
            throw new InvalidOperationException("Expected an ANTLRv4 grammarSpec.");
        // The common compiler vocabulary originated with ANTLRv4. Copying the
        // DOM into owned GrammarNodes has already normalized this notation.
        return syntax;
    }
}

public static class GrammarFrontends
{
    public static bool IsG4Plus(GrammarNode syntax, string fileName) =>
        syntax.DescendantsAndSelf().Any(n => n.LocalName == "symbolRef" ||
            (n.LocalName == "ruleSpec" && GrammarParser.Child(n, "identifier") != null)) ||
        fileName.EndsWith(".g4p", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".g4+", StringComparison.OrdinalIgnoreCase);
}
