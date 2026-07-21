using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Atn;

namespace trinterp;

/// <summary>
/// Produces the text content of .interp and .tokens files from a
/// <see cref="GrammarModel"/> and its built <see cref="ATN"/>.
/// Format mirrors the output of antlr-ng's Tool.generateInterpreterData().
/// </summary>
public static class InterpFormatter
{
    // =========================================================================
    // .interp file
    // =========================================================================

    public static string FormatInterp(GrammarModel grammar, ATN atn, bool actionsInInterp)
    {
        var sb = new StringBuilder();

        int maxTokenType = grammar.GetMaxTokenType();

        // ---- token literal names ----
        // Index = token type. Populated from StringLiteralToType.
        var tokenLiteralNames = new string[maxTokenType + 1];
        foreach (var kv in grammar.StringLiteralToType)
        {
            if (kv.Value >= 0 && kv.Value <= maxTokenType)
                tokenLiteralNames[kv.Value] = kv.Key; // keep the single-quoted form
        }

        sb.AppendLine("token literal names:");
        for (int i = 0; i <= maxTokenType; i++)
            sb.AppendLine(tokenLiteralNames[i] ?? "null");
        sb.AppendLine();

        // ---- token symbolic names ----
        // Index = token type. Populated from TokenNameToType, excluding implicit T__N names.
        var tokenSymbolicNames = new string[maxTokenType + 1];
        foreach (var kv in grammar.TokenNameToType)
        {
            if (kv.Value >= 0 && kv.Value <= maxTokenType && !IsImplicitTokenName(kv.Key))
                tokenSymbolicNames[kv.Value] = kv.Key;
        }

        sb.AppendLine("token symbolic names:");
        for (int i = 0; i <= maxTokenType; i++)
            sb.AppendLine(tokenSymbolicNames[i] ?? "null");
        sb.AppendLine();

        // ---- rule names ----
        sb.AppendLine("rule names:");
        foreach (var rule in grammar.Rules)
            sb.AppendLine(rule.Name);
        sb.AppendLine();

        // ---- channel names (lexer only) ----
        if (grammar.IsLexer)
        {
            sb.AppendLine("channel names:");
            sb.AppendLine("DEFAULT_TOKEN_CHANNEL");
            sb.AppendLine("HIDDEN");
            // Extra channels occupy indices 2+; indices 0/1 are printed as "null" placeholders.
            if (grammar.ExtraChannelNames.Count > 0)
            {
                sb.AppendLine("null");
                sb.AppendLine("null");
                foreach (var ch in grammar.ExtraChannelNames)
                    sb.AppendLine(ch);
            }
            sb.AppendLine();

            // ---- mode names ----
            sb.AppendLine("mode names:");
            foreach (var mode in grammar.ModeNames)
                sb.AppendLine(mode);
            sb.AppendLine();
        }

        // ---- ATN ----
        // Lexer already has a blank line from the mode names section; parser does not.
        if (!grammar.IsLexer)
            sb.AppendLine();
        sb.AppendLine("atn:");
        sb.AppendLine(AtnSerializer.Serialize(atn));

        // ---- optional actions/predicates ----
        if (actionsInInterp)
        {
            FormatActions(sb, grammar);
        }

        return sb.ToString();
    }

    private static void FormatActions(StringBuilder sb, GrammarModel grammar)
    {
        // named actions
        if (grammar.NamedActions.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("named actions:");
            foreach (var na in grammar.NamedActions)
            {
                var key = na.Scope != null ? na.Scope + "::" + na.Name : na.Name;
                sb.AppendLine(key);
                sb.AppendLine(na.Text);
            }
            sb.AppendLine();
        }

        // sempreds
        if (grammar.SemPreds.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("sempreds:");
            foreach (var sp in grammar.SemPreds)
                sb.AppendLine($"{sp.RuleIndex}:{sp.PredIndex}:{sp.Text}");
            sb.AppendLine();
        }

        // actions (parser) / lexer actions (lexer)
        var allActions = CollectActions(grammar);
        if (grammar.IsLexer)
        {
            if (grammar.LexerActions.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("lexer actions:");
                foreach (var la in grammar.LexerActions)
                    sb.AppendLine($"{la.ActionIndex}:{la.Text}");
                sb.AppendLine();
            }
        }
        else
        {
            if (allActions.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("actions:");
                foreach (var a in allActions)
                    sb.AppendLine($"{a.RuleIndex}:{a.ActionIndex}:{a.Text}");
                sb.AppendLine();
            }
        }
    }

    private static List<ActionInfo> CollectActions(GrammarModel grammar)
    {
        var result = new List<ActionInfo>();
        foreach (var rule in grammar.Rules)
            result.AddRange(rule.Actions);
        return result;
    }

    // Returns true for auto-generated implicit token names like T__0, T__1, ...
    private static bool IsImplicitTokenName(string name) =>
        name.Length > 3 && name.StartsWith("T__") && name[3..].All(char.IsDigit);

    // =========================================================================
    // .tokens file
    // =========================================================================

    public static string FormatTokens(GrammarModel grammar)
    {
        var sb = new StringBuilder();

        foreach (var kv in grammar.TokenNameToType)
            sb.AppendLine($"{kv.Key}={kv.Value}");

        foreach (var kv in grammar.StringLiteralToType)
            sb.AppendLine($"{kv.Key}={kv.Value}");

        return sb.ToString();
    }
}
