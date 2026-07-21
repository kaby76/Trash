using System.Collections.Generic;
using ParseTreeEditing.UnvParseTreeDOM;

namespace trinterp;

public enum GrammarKind { Lexer, Parser, Combined }

/// <summary>Describes one rule in the grammar.</summary>
public class RuleModel
{
    public string Name;
    public int Index;           // 0-based position among all rules
    public bool IsFragment;     // lexer fragment rules
    public int TokenType;       // lexer only: the token type for this rule
    public string ModeName;     // lexer only: which mode the rule lives in
    public UnvParseTreeElement BodyNode; // parse tree node of the rule body
    public string ImplicitLiteral; // for T__N rules created from string literals in combined grammars

    // Actions/predicates collected during first pass (for actions-in-interp output)
    public List<ActionInfo> Actions = new(); // non-predicate actions, ordered
    public List<SemPredInfo> SemPreds = new(); // predicates, ordered within rule
}

/// <summary>Represents a named grammar action (@header, @members, @init, @after, etc.).</summary>
public class NamedActionInfo
{
    public string Scope;   // "lexer", "parser", or null for combined
    public string Name;    // "header", "members", "init", "after", etc.
    public string Text;    // raw action text (including braces)
}

/// <summary>An inline action inside a rule.</summary>
public class ActionInfo
{
    public int RuleIndex;
    public int ActionIndex; // per-rule index (predicates excluded)
    public string Text;
}

/// <summary>A semantic predicate inside a rule.</summary>
public class SemPredInfo
{
    public int RuleIndex;
    public int PredIndex;  // global across all rules
    public bool IsCtxDependent;
    public string Text;    // predicate expression (braces and ? stripped)
}

/// <summary>Encodes one lexer action (skip, more, channel, mode, pushMode, popMode, custom).</summary>
public class LexerActionInfo
{
    public int ActionIndex; // global across all lexer rules
    public Antlr4.Runtime.Atn.LexerActionType ActionType;
    public int Data1;
    public int Data2;
    public string Text; // for custom actions
}

/// <summary>
/// Complete model of one grammar (lexer, parser, or the parser half of a combined grammar).
/// </summary>
public class GrammarModel
{
    public string Name;
    public GrammarKind Kind;
    public string FileName;

    // Ordered rules. For combined grammars this is the parser half;
    // the lexer half is stored in ImplicitLexer.
    public List<RuleModel> Rules = new();
    public GrammarModel ImplicitLexer; // set only for the parser half of a combined grammar

    // Token / literal name→type maps (populated during first pass).
    public Dictionary<string, int> TokenNameToType = new();
    public Dictionary<string, int> StringLiteralToType = new();

    // tokenVocab option value (parser grammars only).
    public string TokenVocab;

    // Channel names beyond DEFAULT_TOKEN_CHANNEL and HIDDEN.
    public List<string> ExtraChannelNames = new();

    // Mode names in declaration order (lexer grammars only). DEFAULT_MODE is always first.
    public List<string> ModeNames = new();

    // Named grammar-level actions (@header, @members, etc.).
    public List<NamedActionInfo> NamedActions = new();

    // Global ordered sempred list (filled during ATN construction).
    public List<SemPredInfo> SemPreds = new();

    // Global ordered lexer action list (filled during ATN construction, lexer only).
    public List<LexerActionInfo> LexerActions = new();

    // Declared tokens (from tokens { ... } spec).
    public List<string> DeclaredTokens = new();

    public bool IsLexer => Kind == GrammarKind.Lexer;
    public bool IsParser => Kind == GrammarKind.Parser;

    public RuleModel GetRule(string name)
    {
        foreach (var r in Rules)
            if (r.Name == name) return r;
        return null;
    }

    public int GetMaxTokenType()
    {
        int max = 0;
        foreach (var v in TokenNameToType.Values)
            if (v > max) max = v;
        foreach (var v in StringLiteralToType.Values)
            if (v > max) max = v;
        return max;
    }
}
