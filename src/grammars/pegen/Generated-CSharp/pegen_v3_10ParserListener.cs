//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from pegen_v3_10Parser.g4 by ANTLR 4.13.1

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
/// <see cref="pegen_v3_10Parser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface Ipegen_v3_10ParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStart([NotNull] pegen_v3_10Parser.StartContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStart([NotNull] pegen_v3_10Parser.StartContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.grammar_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGrammar_([NotNull] pegen_v3_10Parser.Grammar_Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.grammar_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGrammar_([NotNull] pegen_v3_10Parser.Grammar_Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.metas"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMetas([NotNull] pegen_v3_10Parser.MetasContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.metas"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMetas([NotNull] pegen_v3_10Parser.MetasContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.meta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMeta([NotNull] pegen_v3_10Parser.MetaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.meta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMeta([NotNull] pegen_v3_10Parser.MetaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.rules"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRules([NotNull] pegen_v3_10Parser.RulesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.rules"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRules([NotNull] pegen_v3_10Parser.RulesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.rule_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRule_([NotNull] pegen_v3_10Parser.Rule_Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.rule_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRule_([NotNull] pegen_v3_10Parser.Rule_Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.rulename"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRulename([NotNull] pegen_v3_10Parser.RulenameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.rulename"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRulename([NotNull] pegen_v3_10Parser.RulenameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute([NotNull] pegen_v3_10Parser.AttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute([NotNull] pegen_v3_10Parser.AttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.memoflag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemoflag([NotNull] pegen_v3_10Parser.MemoflagContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.memoflag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemoflag([NotNull] pegen_v3_10Parser.MemoflagContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.alts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAlts([NotNull] pegen_v3_10Parser.AltsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.alts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAlts([NotNull] pegen_v3_10Parser.AltsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.more_alts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMore_alts([NotNull] pegen_v3_10Parser.More_altsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.more_alts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMore_alts([NotNull] pegen_v3_10Parser.More_altsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.alt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAlt([NotNull] pegen_v3_10Parser.AltContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.alt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAlt([NotNull] pegen_v3_10Parser.AltContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.items"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItems([NotNull] pegen_v3_10Parser.ItemsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.items"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItems([NotNull] pegen_v3_10Parser.ItemsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.named_item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamed_item([NotNull] pegen_v3_10Parser.Named_itemContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.named_item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamed_item([NotNull] pegen_v3_10Parser.Named_itemContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.attribute_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute_name([NotNull] pegen_v3_10Parser.Attribute_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.attribute_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute_name([NotNull] pegen_v3_10Parser.Attribute_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.forced_atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForced_atom([NotNull] pegen_v3_10Parser.Forced_atomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.forced_atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForced_atom([NotNull] pegen_v3_10Parser.Forced_atomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.lookahead"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLookahead([NotNull] pegen_v3_10Parser.LookaheadContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.lookahead"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLookahead([NotNull] pegen_v3_10Parser.LookaheadContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItem([NotNull] pegen_v3_10Parser.ItemContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItem([NotNull] pegen_v3_10Parser.ItemContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtom([NotNull] pegen_v3_10Parser.AtomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtom([NotNull] pegen_v3_10Parser.AtomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.action"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAction([NotNull] pegen_v3_10Parser.ActionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.action"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAction([NotNull] pegen_v3_10Parser.ActionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterName([NotNull] pegen_v3_10Parser.NameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitName([NotNull] pegen_v3_10Parser.NameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString([NotNull] pegen_v3_10Parser.StringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString([NotNull] pegen_v3_10Parser.StringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.newline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNewline([NotNull] pegen_v3_10Parser.NewlineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.newline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNewline([NotNull] pegen_v3_10Parser.NewlineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.indent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIndent([NotNull] pegen_v3_10Parser.IndentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.indent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIndent([NotNull] pegen_v3_10Parser.IndentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.dedent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDedent([NotNull] pegen_v3_10Parser.DedentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.dedent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDedent([NotNull] pegen_v3_10Parser.DedentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="pegen_v3_10Parser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumber([NotNull] pegen_v3_10Parser.NumberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="pegen_v3_10Parser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumber([NotNull] pegen_v3_10Parser.NumberContext context);
}