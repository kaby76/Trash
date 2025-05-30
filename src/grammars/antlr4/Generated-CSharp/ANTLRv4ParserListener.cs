//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ANTLRv4Parser.g4 by ANTLR 4.13.1

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
/// <see cref="ANTLRv4Parser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IANTLRv4ParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.grammarSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGrammarSpec([NotNull] ANTLRv4Parser.GrammarSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.grammarSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGrammarSpec([NotNull] ANTLRv4Parser.GrammarSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.grammarDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGrammarDecl([NotNull] ANTLRv4Parser.GrammarDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.grammarDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGrammarDecl([NotNull] ANTLRv4Parser.GrammarDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.grammarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGrammarType([NotNull] ANTLRv4Parser.GrammarTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.grammarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGrammarType([NotNull] ANTLRv4Parser.GrammarTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.prequelConstruct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrequelConstruct([NotNull] ANTLRv4Parser.PrequelConstructContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.prequelConstruct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrequelConstruct([NotNull] ANTLRv4Parser.PrequelConstructContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.optionsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionsSpec([NotNull] ANTLRv4Parser.OptionsSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.optionsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionsSpec([NotNull] ANTLRv4Parser.OptionsSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.option"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOption([NotNull] ANTLRv4Parser.OptionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.option"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOption([NotNull] ANTLRv4Parser.OptionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.optionValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionValue([NotNull] ANTLRv4Parser.OptionValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.optionValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionValue([NotNull] ANTLRv4Parser.OptionValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.delegateGrammars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDelegateGrammars([NotNull] ANTLRv4Parser.DelegateGrammarsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.delegateGrammars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDelegateGrammars([NotNull] ANTLRv4Parser.DelegateGrammarsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.delegateGrammar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDelegateGrammar([NotNull] ANTLRv4Parser.DelegateGrammarContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.delegateGrammar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDelegateGrammar([NotNull] ANTLRv4Parser.DelegateGrammarContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.tokensSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTokensSpec([NotNull] ANTLRv4Parser.TokensSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.tokensSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTokensSpec([NotNull] ANTLRv4Parser.TokensSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.channelsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChannelsSpec([NotNull] ANTLRv4Parser.ChannelsSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.channelsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChannelsSpec([NotNull] ANTLRv4Parser.ChannelsSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.idList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdList([NotNull] ANTLRv4Parser.IdListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.idList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdList([NotNull] ANTLRv4Parser.IdListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.action_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAction_([NotNull] ANTLRv4Parser.Action_Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.action_"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAction_([NotNull] ANTLRv4Parser.Action_Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.actionScopeName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterActionScopeName([NotNull] ANTLRv4Parser.ActionScopeNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.actionScopeName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitActionScopeName([NotNull] ANTLRv4Parser.ActionScopeNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.actionBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterActionBlock([NotNull] ANTLRv4Parser.ActionBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.actionBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitActionBlock([NotNull] ANTLRv4Parser.ActionBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.argActionBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArgActionBlock([NotNull] ANTLRv4Parser.ArgActionBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.argActionBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArgActionBlock([NotNull] ANTLRv4Parser.ArgActionBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.modeSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterModeSpec([NotNull] ANTLRv4Parser.ModeSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.modeSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitModeSpec([NotNull] ANTLRv4Parser.ModeSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.rules"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRules([NotNull] ANTLRv4Parser.RulesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.rules"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRules([NotNull] ANTLRv4Parser.RulesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleSpec([NotNull] ANTLRv4Parser.RuleSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleSpec([NotNull] ANTLRv4Parser.RuleSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.parserRuleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParserRuleSpec([NotNull] ANTLRv4Parser.ParserRuleSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.parserRuleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParserRuleSpec([NotNull] ANTLRv4Parser.ParserRuleSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.exceptionGroup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExceptionGroup([NotNull] ANTLRv4Parser.ExceptionGroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.exceptionGroup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExceptionGroup([NotNull] ANTLRv4Parser.ExceptionGroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.exceptionHandler"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExceptionHandler([NotNull] ANTLRv4Parser.ExceptionHandlerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.exceptionHandler"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExceptionHandler([NotNull] ANTLRv4Parser.ExceptionHandlerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.finallyClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFinallyClause([NotNull] ANTLRv4Parser.FinallyClauseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.finallyClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFinallyClause([NotNull] ANTLRv4Parser.FinallyClauseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.rulePrequel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRulePrequel([NotNull] ANTLRv4Parser.RulePrequelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.rulePrequel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRulePrequel([NotNull] ANTLRv4Parser.RulePrequelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleReturns"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleReturns([NotNull] ANTLRv4Parser.RuleReturnsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleReturns"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleReturns([NotNull] ANTLRv4Parser.RuleReturnsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.throwsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterThrowsSpec([NotNull] ANTLRv4Parser.ThrowsSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.throwsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitThrowsSpec([NotNull] ANTLRv4Parser.ThrowsSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.localsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocalsSpec([NotNull] ANTLRv4Parser.LocalsSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.localsSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocalsSpec([NotNull] ANTLRv4Parser.LocalsSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleAction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleAction([NotNull] ANTLRv4Parser.RuleActionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleAction"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleAction([NotNull] ANTLRv4Parser.RuleActionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleModifiers"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleModifiers([NotNull] ANTLRv4Parser.RuleModifiersContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleModifiers"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleModifiers([NotNull] ANTLRv4Parser.RuleModifiersContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleModifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleModifier([NotNull] ANTLRv4Parser.RuleModifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleModifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleModifier([NotNull] ANTLRv4Parser.RuleModifierContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleBlock([NotNull] ANTLRv4Parser.RuleBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleBlock([NotNull] ANTLRv4Parser.RuleBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleAltList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleAltList([NotNull] ANTLRv4Parser.RuleAltListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleAltList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleAltList([NotNull] ANTLRv4Parser.RuleAltListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.labeledAlt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabeledAlt([NotNull] ANTLRv4Parser.LabeledAltContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.labeledAlt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabeledAlt([NotNull] ANTLRv4Parser.LabeledAltContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerRuleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerRuleSpec([NotNull] ANTLRv4Parser.LexerRuleSpecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerRuleSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerRuleSpec([NotNull] ANTLRv4Parser.LexerRuleSpecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerRuleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerRuleBlock([NotNull] ANTLRv4Parser.LexerRuleBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerRuleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerRuleBlock([NotNull] ANTLRv4Parser.LexerRuleBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerAltList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerAltList([NotNull] ANTLRv4Parser.LexerAltListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerAltList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerAltList([NotNull] ANTLRv4Parser.LexerAltListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerAlt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerAlt([NotNull] ANTLRv4Parser.LexerAltContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerAlt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerAlt([NotNull] ANTLRv4Parser.LexerAltContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerElements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerElements([NotNull] ANTLRv4Parser.LexerElementsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerElements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerElements([NotNull] ANTLRv4Parser.LexerElementsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerElement([NotNull] ANTLRv4Parser.LexerElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerElement([NotNull] ANTLRv4Parser.LexerElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerBlock([NotNull] ANTLRv4Parser.LexerBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerBlock([NotNull] ANTLRv4Parser.LexerBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerCommands"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerCommands([NotNull] ANTLRv4Parser.LexerCommandsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerCommands"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerCommands([NotNull] ANTLRv4Parser.LexerCommandsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerCommand([NotNull] ANTLRv4Parser.LexerCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerCommand([NotNull] ANTLRv4Parser.LexerCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerCommandName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerCommandName([NotNull] ANTLRv4Parser.LexerCommandNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerCommandName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerCommandName([NotNull] ANTLRv4Parser.LexerCommandNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerCommandExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerCommandExpr([NotNull] ANTLRv4Parser.LexerCommandExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerCommandExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerCommandExpr([NotNull] ANTLRv4Parser.LexerCommandExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.altList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAltList([NotNull] ANTLRv4Parser.AltListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.altList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAltList([NotNull] ANTLRv4Parser.AltListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.alternative"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAlternative([NotNull] ANTLRv4Parser.AlternativeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.alternative"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAlternative([NotNull] ANTLRv4Parser.AlternativeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.element"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElement([NotNull] ANTLRv4Parser.ElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.element"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElement([NotNull] ANTLRv4Parser.ElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.predicateOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPredicateOptions([NotNull] ANTLRv4Parser.PredicateOptionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.predicateOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPredicateOptions([NotNull] ANTLRv4Parser.PredicateOptionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.predicateOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPredicateOption([NotNull] ANTLRv4Parser.PredicateOptionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.predicateOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPredicateOption([NotNull] ANTLRv4Parser.PredicateOptionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.labeledElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabeledElement([NotNull] ANTLRv4Parser.LabeledElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.labeledElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabeledElement([NotNull] ANTLRv4Parser.LabeledElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ebnf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEbnf([NotNull] ANTLRv4Parser.EbnfContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ebnf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEbnf([NotNull] ANTLRv4Parser.EbnfContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.blockSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlockSuffix([NotNull] ANTLRv4Parser.BlockSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.blockSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlockSuffix([NotNull] ANTLRv4Parser.BlockSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ebnfSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEbnfSuffix([NotNull] ANTLRv4Parser.EbnfSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ebnfSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEbnfSuffix([NotNull] ANTLRv4Parser.EbnfSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.lexerAtom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLexerAtom([NotNull] ANTLRv4Parser.LexerAtomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.lexerAtom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLexerAtom([NotNull] ANTLRv4Parser.LexerAtomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtom([NotNull] ANTLRv4Parser.AtomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.atom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtom([NotNull] ANTLRv4Parser.AtomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.wildcard"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWildcard([NotNull] ANTLRv4Parser.WildcardContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.wildcard"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWildcard([NotNull] ANTLRv4Parser.WildcardContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.notSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNotSet([NotNull] ANTLRv4Parser.NotSetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.notSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNotSet([NotNull] ANTLRv4Parser.NotSetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.blockSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlockSet([NotNull] ANTLRv4Parser.BlockSetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.blockSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlockSet([NotNull] ANTLRv4Parser.BlockSetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.setElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSetElement([NotNull] ANTLRv4Parser.SetElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.setElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSetElement([NotNull] ANTLRv4Parser.SetElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock([NotNull] ANTLRv4Parser.BlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock([NotNull] ANTLRv4Parser.BlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.ruleref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleref([NotNull] ANTLRv4Parser.RulerefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.ruleref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleref([NotNull] ANTLRv4Parser.RulerefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.characterRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCharacterRange([NotNull] ANTLRv4Parser.CharacterRangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.characterRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCharacterRange([NotNull] ANTLRv4Parser.CharacterRangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.terminalDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerminalDef([NotNull] ANTLRv4Parser.TerminalDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.terminalDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerminalDef([NotNull] ANTLRv4Parser.TerminalDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.elementOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElementOptions([NotNull] ANTLRv4Parser.ElementOptionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.elementOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElementOptions([NotNull] ANTLRv4Parser.ElementOptionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.elementOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElementOption([NotNull] ANTLRv4Parser.ElementOptionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.elementOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElementOption([NotNull] ANTLRv4Parser.ElementOptionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdentifier([NotNull] ANTLRv4Parser.IdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdentifier([NotNull] ANTLRv4Parser.IdentifierContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="ANTLRv4Parser.qualifiedIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterQualifiedIdentifier([NotNull] ANTLRv4Parser.QualifiedIdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ANTLRv4Parser.qualifiedIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitQualifiedIdentifier([NotNull] ANTLRv4Parser.QualifiedIdentifierContext context);
}
