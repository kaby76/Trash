//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from PestParser.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IPestParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class PestParserBaseListener : IPestParserListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.grammar_rules"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGrammar_rules([NotNull] PestParser.Grammar_rulesContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.grammar_rules"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGrammar_rules([NotNull] PestParser.Grammar_rulesContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.grammar_rule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGrammar_rule([NotNull] PestParser.Grammar_ruleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.grammar_rule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGrammar_rule([NotNull] PestParser.Grammar_ruleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterModifier([NotNull] PestParser.ModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitModifier([NotNull] PestParser.ModifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.silent_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSilent_modifier([NotNull] PestParser.Silent_modifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.silent_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSilent_modifier([NotNull] PestParser.Silent_modifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomic_modifier([NotNull] PestParser.Atomic_modifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomic_modifier([NotNull] PestParser.Atomic_modifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.compound_atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompound_atomic_modifier([NotNull] PestParser.Compound_atomic_modifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.compound_atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompound_atomic_modifier([NotNull] PestParser.Compound_atomic_modifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.non_atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNon_atomic_modifier([NotNull] PestParser.Non_atomic_modifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.non_atomic_modifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNon_atomic_modifier([NotNull] PestParser.Non_atomic_modifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] PestParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] PestParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerm([NotNull] PestParser.TermContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.term"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerm([NotNull] PestParser.TermContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.node"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNode([NotNull] PestParser.NodeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.node"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNode([NotNull] PestParser.NodeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.terminal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminal([NotNull] PestParser.TerminalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.terminal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminal([NotNull] PestParser.TerminalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.prefix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrefix_operator([NotNull] PestParser.Prefix_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.prefix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrefix_operator([NotNull] PestParser.Prefix_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.infix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInfix_operator([NotNull] PestParser.Infix_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.infix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInfix_operator([NotNull] PestParser.Infix_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.postfix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPostfix_operator([NotNull] PestParser.Postfix_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.postfix_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPostfix_operator([NotNull] PestParser.Postfix_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.positive_predicate_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPositive_predicate_operator([NotNull] PestParser.Positive_predicate_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.positive_predicate_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPositive_predicate_operator([NotNull] PestParser.Positive_predicate_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.negative_predicate_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNegative_predicate_operator([NotNull] PestParser.Negative_predicate_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.negative_predicate_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNegative_predicate_operator([NotNull] PestParser.Negative_predicate_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.sequence_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSequence_operator([NotNull] PestParser.Sequence_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.sequence_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSequence_operator([NotNull] PestParser.Sequence_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.choice_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterChoice_operator([NotNull] PestParser.Choice_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.choice_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitChoice_operator([NotNull] PestParser.Choice_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.optional_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptional_operator([NotNull] PestParser.Optional_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.optional_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptional_operator([NotNull] PestParser.Optional_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_operator([NotNull] PestParser.Repeat_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_operator([NotNull] PestParser.Repeat_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_once_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_once_operator([NotNull] PestParser.Repeat_once_operatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_once_operator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_once_operator([NotNull] PestParser.Repeat_once_operatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_exact"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_exact([NotNull] PestParser.Repeat_exactContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_exact"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_exact([NotNull] PestParser.Repeat_exactContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_min"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_min([NotNull] PestParser.Repeat_minContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_min"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_min([NotNull] PestParser.Repeat_minContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_max"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_max([NotNull] PestParser.Repeat_maxContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_max"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_max([NotNull] PestParser.Repeat_maxContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.repeat_min_max"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeat_min_max([NotNull] PestParser.Repeat_min_maxContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.repeat_min_max"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeat_min_max([NotNull] PestParser.Repeat_min_maxContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.push"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPush([NotNull] PestParser.PushContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.push"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPush([NotNull] PestParser.PushContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PestParser.peek_slice"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPeek_slice([NotNull] PestParser.Peek_sliceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PestParser.peek_slice"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPeek_slice([NotNull] PestParser.Peek_sliceContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
