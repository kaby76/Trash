//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Princeton.g4 by ANTLR 4.13.1

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
/// <see cref="PrincetonParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IPrincetonListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.prods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProds([NotNull] PrincetonParser.ProdsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.prods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProds([NotNull] PrincetonParser.ProdsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.prod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProd([NotNull] PrincetonParser.ProdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.prod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProd([NotNull] PrincetonParser.ProdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.lhs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLhs([NotNull] PrincetonParser.LhsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.lhs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLhs([NotNull] PrincetonParser.LhsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.rhs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRhs([NotNull] PrincetonParser.RhsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.rhs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRhs([NotNull] PrincetonParser.RhsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtom([NotNull] PrincetonParser.AtomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtom([NotNull] PrincetonParser.AtomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrincetonParser.symbol"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSymbol([NotNull] PrincetonParser.SymbolContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrincetonParser.symbol"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSymbol([NotNull] PrincetonParser.SymbolContext context);
}