//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from PestParser.g4 by ANTLR 4.10.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace LanguageServer {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="PestParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public interface IPestParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.grammar_rules"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGrammar_rules([NotNull] PestParser.Grammar_rulesContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.grammar_rule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGrammar_rule([NotNull] PestParser.Grammar_ruleContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.modifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitModifier([NotNull] PestParser.ModifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.silent_modifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSilent_modifier([NotNull] PestParser.Silent_modifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.atomic_modifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAtomic_modifier([NotNull] PestParser.Atomic_modifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.compound_atomic_modifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCompound_atomic_modifier([NotNull] PestParser.Compound_atomic_modifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.non_atomic_modifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNon_atomic_modifier([NotNull] PestParser.Non_atomic_modifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] PestParser.ExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTerm([NotNull] PestParser.TermContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.node"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNode([NotNull] PestParser.NodeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.terminal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTerminal([NotNull] PestParser.TerminalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.prefix_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrefix_operator([NotNull] PestParser.Prefix_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.infix_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfix_operator([NotNull] PestParser.Infix_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.postfix_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPostfix_operator([NotNull] PestParser.Postfix_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.positive_predicate_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPositive_predicate_operator([NotNull] PestParser.Positive_predicate_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.negative_predicate_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNegative_predicate_operator([NotNull] PestParser.Negative_predicate_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.sequence_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSequence_operator([NotNull] PestParser.Sequence_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.choice_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitChoice_operator([NotNull] PestParser.Choice_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.optional_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOptional_operator([NotNull] PestParser.Optional_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_operator([NotNull] PestParser.Repeat_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_once_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_once_operator([NotNull] PestParser.Repeat_once_operatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_exact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_exact([NotNull] PestParser.Repeat_exactContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_min"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_min([NotNull] PestParser.Repeat_minContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_max"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_max([NotNull] PestParser.Repeat_maxContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.repeat_min_max"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRepeat_min_max([NotNull] PestParser.Repeat_min_maxContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.push"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPush([NotNull] PestParser.PushContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PestParser.peek_slice"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPeek_slice([NotNull] PestParser.Peek_sliceContext context);
}
} // namespace LanguageServer