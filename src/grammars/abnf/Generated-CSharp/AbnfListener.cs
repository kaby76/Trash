//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Abnf.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="AbnfParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IAbnfListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.rulelist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRulelist([NotNull] AbnfParser.RulelistContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.rulelist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRulelist([NotNull] AbnfParser.RulelistContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.rule_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRule_([NotNull] AbnfParser.Rule_Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.rule_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRule_([NotNull] AbnfParser.Rule_Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.rulename"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRulename([NotNull] AbnfParser.RulenameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.rulename"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRulename([NotNull] AbnfParser.RulenameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.defined_as"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefined_as([NotNull] AbnfParser.Defined_asContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.defined_as"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefined_as([NotNull] AbnfParser.Defined_asContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.elements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElements([NotNull] AbnfParser.ElementsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.elements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElements([NotNull] AbnfParser.ElementsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.c_wsp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterC_wsp([NotNull] AbnfParser.C_wspContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.c_wsp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitC_wsp([NotNull] AbnfParser.C_wspContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.c_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterC_nl([NotNull] AbnfParser.C_nlContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.c_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitC_nl([NotNull] AbnfParser.C_nlContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.alternation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAlternation([NotNull] AbnfParser.AlternationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.alternation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAlternation([NotNull] AbnfParser.AlternationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.concatenation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConcatenation([NotNull] AbnfParser.ConcatenationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.concatenation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConcatenation([NotNull] AbnfParser.ConcatenationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.repetition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepetition([NotNull] AbnfParser.RepetitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.repetition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepetition([NotNull] AbnfParser.RepetitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.repeat_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeat_([NotNull] AbnfParser.Repeat_Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.repeat_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeat_([NotNull] AbnfParser.Repeat_Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.element"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElement([NotNull] AbnfParser.ElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.element"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElement([NotNull] AbnfParser.ElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGroup([NotNull] AbnfParser.GroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGroup([NotNull] AbnfParser.GroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.option"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOption([NotNull] AbnfParser.OptionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.option"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOption([NotNull] AbnfParser.OptionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.char_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChar_val([NotNull] AbnfParser.Char_valContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.char_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChar_val([NotNull] AbnfParser.Char_valContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.num_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNum_val([NotNull] AbnfParser.Num_valContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.num_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNum_val([NotNull] AbnfParser.Num_valContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AbnfParser.prose_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProse_val([NotNull] AbnfParser.Prose_valContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AbnfParser.prose_val"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProse_val([NotNull] AbnfParser.Prose_valContext context);
}