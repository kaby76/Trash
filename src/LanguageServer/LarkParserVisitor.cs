//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from LarkParser.g4 by ANTLR 4.10.1

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
/// by <see cref="LarkParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public interface ILarkParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStart([NotNull] LarkParser.StartContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitItem([NotNull] LarkParser.ItemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.rule_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRule_([NotNull] LarkParser.Rule_Context context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.token"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitToken([NotNull] LarkParser.TokenContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.rule_params"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRule_params([NotNull] LarkParser.Rule_paramsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.token_params"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitToken_params([NotNull] LarkParser.Token_paramsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.priority"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPriority([NotNull] LarkParser.PriorityContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] LarkParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.import_path"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitImport_path([NotNull] LarkParser.Import_pathContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.name_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitName_list([NotNull] LarkParser.Name_listContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.expansions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpansions([NotNull] LarkParser.ExpansionsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.alias"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAlias([NotNull] LarkParser.AliasContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.expansion"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpansion([NotNull] LarkParser.ExpansionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] LarkParser.ExprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAtom([NotNull] LarkParser.AtomContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValue([NotNull] LarkParser.ValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="LarkParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitName([NotNull] LarkParser.NameContext context);
}
} // namespace LanguageServer