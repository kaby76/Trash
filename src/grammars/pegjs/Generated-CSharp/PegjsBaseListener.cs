//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Pegjs.g4 by ANTLR 4.13.1

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
/// This class provides an empty implementation of <see cref="IPegjsListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class PegjsBaseListener : IPegjsListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.grammar_"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGrammar_([NotNull] PegjsParser.Grammar_Context context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.grammar_"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGrammar_([NotNull] PegjsParser.Grammar_Context context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.initializer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInitializer([NotNull] PegjsParser.InitializerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.initializer"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInitializer([NotNull] PegjsParser.InitializerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.eos"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEos([NotNull] PegjsParser.EosContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.eos"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEos([NotNull] PegjsParser.EosContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.rule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRule([NotNull] PegjsParser.RuleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.rule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRule([NotNull] PegjsParser.RuleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] PegjsParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] PegjsParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.choiceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterChoiceexpression([NotNull] PegjsParser.ChoiceexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.choiceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitChoiceexpression([NotNull] PegjsParser.ChoiceexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.actionexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterActionexpression([NotNull] PegjsParser.ActionexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.actionexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitActionexpression([NotNull] PegjsParser.ActionexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.sequenceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSequenceexpression([NotNull] PegjsParser.SequenceexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.sequenceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSequenceexpression([NotNull] PegjsParser.SequenceexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.labeledexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabeledexpression([NotNull] PegjsParser.LabeledexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.labeledexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabeledexpression([NotNull] PegjsParser.LabeledexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.labelidentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabelidentifier([NotNull] PegjsParser.LabelidentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.labelidentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabelidentifier([NotNull] PegjsParser.LabelidentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.prefixedexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrefixedexpression([NotNull] PegjsParser.PrefixedexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.prefixedexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrefixedexpression([NotNull] PegjsParser.PrefixedexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.prefixedoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrefixedoperator([NotNull] PegjsParser.PrefixedoperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.prefixedoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrefixedoperator([NotNull] PegjsParser.PrefixedoperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.suffixedexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSuffixedexpression([NotNull] PegjsParser.SuffixedexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.suffixedexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSuffixedexpression([NotNull] PegjsParser.SuffixedexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.suffixedoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSuffixedoperator([NotNull] PegjsParser.SuffixedoperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.suffixedoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSuffixedoperator([NotNull] PegjsParser.SuffixedoperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.primaryexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimaryexpression([NotNull] PegjsParser.PrimaryexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.primaryexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimaryexpression([NotNull] PegjsParser.PrimaryexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.rulereferenceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRulereferenceexpression([NotNull] PegjsParser.RulereferenceexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.rulereferenceexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRulereferenceexpression([NotNull] PegjsParser.RulereferenceexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.semanticpredicateexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSemanticpredicateexpression([NotNull] PegjsParser.SemanticpredicateexpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.semanticpredicateexpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSemanticpredicateexpression([NotNull] PegjsParser.SemanticpredicateexpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.semanticpredicateoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSemanticpredicateoperator([NotNull] PegjsParser.SemanticpredicateoperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.semanticpredicateoperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSemanticpredicateoperator([NotNull] PegjsParser.SemanticpredicateoperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIdentifier([NotNull] PegjsParser.IdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.identifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIdentifier([NotNull] PegjsParser.IdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PegjsParser.literalMatcher"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLiteralMatcher([NotNull] PegjsParser.LiteralMatcherContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PegjsParser.literalMatcher"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLiteralMatcher([NotNull] PegjsParser.LiteralMatcherContext context) { }

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
