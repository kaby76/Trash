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

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class PrincetonParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		SymSep=1, RuleSep=2, OP=3, CP=4, COMMENT=5, String=6, Symbol=7, WS=8;
	public const int
		RULE_prods = 0, RULE_prod = 1, RULE_lhs = 2, RULE_rhs = 3, RULE_atom = 4, 
		RULE_symbol = 5;
	public static readonly string[] ruleNames = {
		"prods", "prod", "lhs", "rhs", "atom", "symbol"
	};

	private static readonly string[] _LiteralNames = {
		null, "'::='", null, "'('", "')'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "SymSep", "RuleSep", "OP", "CP", "COMMENT", "String", "Symbol", 
		"WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Princeton.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static PrincetonParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public PrincetonParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public PrincetonParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ProdsContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(PrincetonParser.Eof, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ProdContext[] prod() {
			return GetRuleContexts<ProdContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ProdContext prod(int i) {
			return GetRuleContext<ProdContext>(i);
		}
		public ProdsContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_prods; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterProds(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitProds(this);
		}
	}

	[RuleVersion(0)]
	public ProdsContext prods() {
		ProdsContext _localctx = new ProdsContext(Context, State);
		EnterRule(_localctx, 0, RULE_prods);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 13;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 12;
				prod();
				}
				}
				State = 15;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( _la==RuleSep || _la==Symbol );
			State = 17;
			Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ProdContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode RuleSep() { return GetToken(PrincetonParser.RuleSep, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public LhsContext lhs() {
			return GetRuleContext<LhsContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode SymSep() { return GetToken(PrincetonParser.SymSep, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public RhsContext rhs() {
			return GetRuleContext<RhsContext>(0);
		}
		public ProdContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_prod; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterProd(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitProd(this);
		}
	}

	[RuleVersion(0)]
	public ProdContext prod() {
		ProdContext _localctx = new ProdContext(Context, State);
		EnterRule(_localctx, 2, RULE_prod);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 23;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==Symbol) {
				{
				State = 19;
				lhs();
				State = 20;
				Match(SymSep);
				State = 21;
				rhs();
				}
			}

			State = 25;
			Match(RuleSep);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LhsContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public SymbolContext symbol() {
			return GetRuleContext<SymbolContext>(0);
		}
		public LhsContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_lhs; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterLhs(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitLhs(this);
		}
	}

	[RuleVersion(0)]
	public LhsContext lhs() {
		LhsContext _localctx = new LhsContext(Context, State);
		EnterRule(_localctx, 4, RULE_lhs);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 27;
			symbol();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class RhsContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public AtomContext[] atom() {
			return GetRuleContexts<AtomContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public AtomContext atom(int i) {
			return GetRuleContext<AtomContext>(i);
		}
		public RhsContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_rhs; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterRhs(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitRhs(this);
		}
	}

	[RuleVersion(0)]
	public RhsContext rhs() {
		RhsContext _localctx = new RhsContext(Context, State);
		EnterRule(_localctx, 6, RULE_rhs);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 32;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 200L) != 0)) {
				{
				{
				State = 29;
				atom();
				}
				}
				State = 34;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class AtomContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public SymbolContext symbol() {
			return GetRuleContext<SymbolContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode String() { return GetToken(PrincetonParser.String, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode OP() { return GetToken(PrincetonParser.OP, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode CP() { return GetToken(PrincetonParser.CP, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public AtomContext[] atom() {
			return GetRuleContexts<AtomContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public AtomContext atom(int i) {
			return GetRuleContext<AtomContext>(i);
		}
		public AtomContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_atom; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterAtom(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitAtom(this);
		}
	}

	[RuleVersion(0)]
	public AtomContext atom() {
		AtomContext _localctx = new AtomContext(Context, State);
		EnterRule(_localctx, 8, RULE_atom);
		int _la;
		try {
			State = 45;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case Symbol:
				EnterOuterAlt(_localctx, 1);
				{
				State = 35;
				symbol();
				}
				break;
			case String:
				EnterOuterAlt(_localctx, 2);
				{
				State = 36;
				Match(String);
				}
				break;
			case OP:
				EnterOuterAlt(_localctx, 3);
				{
				State = 37;
				Match(OP);
				State = 41;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 200L) != 0)) {
					{
					{
					State = 38;
					atom();
					}
					}
					State = 43;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				State = 44;
				Match(CP);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class SymbolContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Symbol() { return GetToken(PrincetonParser.Symbol, 0); }
		public SymbolContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_symbol; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.EnterSymbol(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPrincetonListener typedListener = listener as IPrincetonListener;
			if (typedListener != null) typedListener.ExitSymbol(this);
		}
	}

	[RuleVersion(0)]
	public SymbolContext symbol() {
		SymbolContext _localctx = new SymbolContext(Context, State);
		EnterRule(_localctx, 10, RULE_symbol);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 47;
			Match(Symbol);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,8,50,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,1,0,4,0,14,8,
		0,11,0,12,0,15,1,0,1,0,1,1,1,1,1,1,1,1,3,1,24,8,1,1,1,1,1,1,2,1,2,1,3,
		5,3,31,8,3,10,3,12,3,34,9,3,1,4,1,4,1,4,1,4,5,4,40,8,4,10,4,12,4,43,9,
		4,1,4,3,4,46,8,4,1,5,1,5,1,5,0,0,6,0,2,4,6,8,10,0,0,49,0,13,1,0,0,0,2,
		23,1,0,0,0,4,27,1,0,0,0,6,32,1,0,0,0,8,45,1,0,0,0,10,47,1,0,0,0,12,14,
		3,2,1,0,13,12,1,0,0,0,14,15,1,0,0,0,15,13,1,0,0,0,15,16,1,0,0,0,16,17,
		1,0,0,0,17,18,5,0,0,1,18,1,1,0,0,0,19,20,3,4,2,0,20,21,5,1,0,0,21,22,3,
		6,3,0,22,24,1,0,0,0,23,19,1,0,0,0,23,24,1,0,0,0,24,25,1,0,0,0,25,26,5,
		2,0,0,26,3,1,0,0,0,27,28,3,10,5,0,28,5,1,0,0,0,29,31,3,8,4,0,30,29,1,0,
		0,0,31,34,1,0,0,0,32,30,1,0,0,0,32,33,1,0,0,0,33,7,1,0,0,0,34,32,1,0,0,
		0,35,46,3,10,5,0,36,46,5,6,0,0,37,41,5,3,0,0,38,40,3,8,4,0,39,38,1,0,0,
		0,40,43,1,0,0,0,41,39,1,0,0,0,41,42,1,0,0,0,42,44,1,0,0,0,43,41,1,0,0,
		0,44,46,5,4,0,0,45,35,1,0,0,0,45,36,1,0,0,0,45,37,1,0,0,0,46,9,1,0,0,0,
		47,48,5,7,0,0,48,11,1,0,0,0,5,15,23,32,41,45
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
