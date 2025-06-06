//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from JSON5.g4 by ANTLR 4.13.1

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
public partial class JSON5Parser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, SINGLE_LINE_COMMENT=7, 
		MULTI_LINE_COMMENT=8, LITERAL=9, STRING=10, NUMBER=11, NUMERIC_LITERAL=12, 
		SYMBOL=13, IDENTIFIER=14, WS=15;
	public const int
		RULE_json5 = 0, RULE_obj = 1, RULE_pair = 2, RULE_key = 3, RULE_value = 4, 
		RULE_arr = 5, RULE_number = 6;
	public static readonly string[] ruleNames = {
		"json5", "obj", "pair", "key", "value", "arr", "number"
	};

	private static readonly string[] _LiteralNames = {
		null, "'{'", "','", "'}'", "':'", "'['", "']'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, "SINGLE_LINE_COMMENT", "MULTI_LINE_COMMENT", 
		"LITERAL", "STRING", "NUMBER", "NUMERIC_LITERAL", "SYMBOL", "IDENTIFIER", 
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

	public override string GrammarFileName { get { return "JSON5.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static JSON5Parser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public JSON5Parser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public JSON5Parser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class Json5Context : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(JSON5Parser.Eof, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public Json5Context(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_json5; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterJson5(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitJson5(this);
		}
	}

	[RuleVersion(0)]
	public Json5Context json5() {
		Json5Context _localctx = new Json5Context(Context, State);
		EnterRule(_localctx, 0, RULE_json5);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 15;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 15906L) != 0)) {
				{
				State = 14;
				value();
				}
			}

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

	public partial class ObjContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public PairContext[] pair() {
			return GetRuleContexts<PairContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public PairContext pair(int i) {
			return GetRuleContext<PairContext>(i);
		}
		public ObjContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_obj; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterObj(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitObj(this);
		}
	}

	[RuleVersion(0)]
	public ObjContext obj() {
		ObjContext _localctx = new ObjContext(Context, State);
		EnterRule(_localctx, 2, RULE_obj);
		int _la;
		try {
			int _alt;
			State = 35;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,3,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 19;
				Match(T__0);
				State = 20;
				pair();
				State = 25;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,1,Context);
				while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						State = 21;
						Match(T__1);
						State = 22;
						pair();
						}
						} 
					}
					State = 27;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,1,Context);
				}
				State = 29;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==T__1) {
					{
					State = 28;
					Match(T__1);
					}
				}

				State = 31;
				Match(T__2);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 33;
				Match(T__0);
				State = 34;
				Match(T__2);
				}
				break;
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

	public partial class PairContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public KeyContext key() {
			return GetRuleContext<KeyContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public PairContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_pair; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterPair(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitPair(this);
		}
	}

	[RuleVersion(0)]
	public PairContext pair() {
		PairContext _localctx = new PairContext(Context, State);
		EnterRule(_localctx, 4, RULE_pair);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 37;
			key();
			State = 38;
			Match(T__3);
			State = 39;
			value();
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

	public partial class KeyContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(JSON5Parser.STRING, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER() { return GetToken(JSON5Parser.IDENTIFIER, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LITERAL() { return GetToken(JSON5Parser.LITERAL, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMERIC_LITERAL() { return GetToken(JSON5Parser.NUMERIC_LITERAL, 0); }
		public KeyContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_key; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterKey(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitKey(this);
		}
	}

	[RuleVersion(0)]
	public KeyContext key() {
		KeyContext _localctx = new KeyContext(Context, State);
		EnterRule(_localctx, 6, RULE_key);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 41;
			_la = TokenStream.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 22016L) != 0)) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
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

	public partial class ValueContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(JSON5Parser.STRING, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public NumberContext number() {
			return GetRuleContext<NumberContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ObjContext obj() {
			return GetRuleContext<ObjContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ArrContext arr() {
			return GetRuleContext<ArrContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LITERAL() { return GetToken(JSON5Parser.LITERAL, 0); }
		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_value; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterValue(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitValue(this);
		}
	}

	[RuleVersion(0)]
	public ValueContext value() {
		ValueContext _localctx = new ValueContext(Context, State);
		EnterRule(_localctx, 8, RULE_value);
		try {
			State = 48;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				EnterOuterAlt(_localctx, 1);
				{
				State = 43;
				Match(STRING);
				}
				break;
			case NUMBER:
			case NUMERIC_LITERAL:
			case SYMBOL:
				EnterOuterAlt(_localctx, 2);
				{
				State = 44;
				number();
				}
				break;
			case T__0:
				EnterOuterAlt(_localctx, 3);
				{
				State = 45;
				obj();
				}
				break;
			case T__4:
				EnterOuterAlt(_localctx, 4);
				{
				State = 46;
				arr();
				}
				break;
			case LITERAL:
				EnterOuterAlt(_localctx, 5);
				{
				State = 47;
				Match(LITERAL);
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

	public partial class ArrContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext[] value() {
			return GetRuleContexts<ValueContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value(int i) {
			return GetRuleContext<ValueContext>(i);
		}
		public ArrContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_arr; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterArr(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitArr(this);
		}
	}

	[RuleVersion(0)]
	public ArrContext arr() {
		ArrContext _localctx = new ArrContext(Context, State);
		EnterRule(_localctx, 10, RULE_arr);
		int _la;
		try {
			int _alt;
			State = 66;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,7,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 50;
				Match(T__4);
				State = 51;
				value();
				State = 56;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,5,Context);
				while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						State = 52;
						Match(T__1);
						State = 53;
						value();
						}
						} 
					}
					State = 58;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,5,Context);
				}
				State = 60;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==T__1) {
					{
					State = 59;
					Match(T__1);
					}
				}

				State = 62;
				Match(T__5);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 64;
				Match(T__4);
				State = 65;
				Match(T__5);
				}
				break;
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

	public partial class NumberContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMERIC_LITERAL() { return GetToken(JSON5Parser.NUMERIC_LITERAL, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMBER() { return GetToken(JSON5Parser.NUMBER, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode SYMBOL() { return GetToken(JSON5Parser.SYMBOL, 0); }
		public NumberContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_number; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.EnterNumber(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IJSON5Listener typedListener = listener as IJSON5Listener;
			if (typedListener != null) typedListener.ExitNumber(this);
		}
	}

	[RuleVersion(0)]
	public NumberContext number() {
		NumberContext _localctx = new NumberContext(Context, State);
		EnterRule(_localctx, 12, RULE_number);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 69;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==SYMBOL) {
				{
				State = 68;
				Match(SYMBOL);
				}
			}

			State = 71;
			_la = TokenStream.LA(1);
			if ( !(_la==NUMBER || _la==NUMERIC_LITERAL) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
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

	private static int[] _serializedATN = {
		4,1,15,74,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,1,0,
		3,0,16,8,0,1,0,1,0,1,1,1,1,1,1,1,1,5,1,24,8,1,10,1,12,1,27,9,1,1,1,3,1,
		30,8,1,1,1,1,1,1,1,1,1,3,1,36,8,1,1,2,1,2,1,2,1,2,1,3,1,3,1,4,1,4,1,4,
		1,4,1,4,3,4,49,8,4,1,5,1,5,1,5,1,5,5,5,55,8,5,10,5,12,5,58,9,5,1,5,3,5,
		61,8,5,1,5,1,5,1,5,1,5,3,5,67,8,5,1,6,3,6,70,8,6,1,6,1,6,1,6,0,0,7,0,2,
		4,6,8,10,12,0,2,3,0,9,10,12,12,14,14,1,0,11,12,78,0,15,1,0,0,0,2,35,1,
		0,0,0,4,37,1,0,0,0,6,41,1,0,0,0,8,48,1,0,0,0,10,66,1,0,0,0,12,69,1,0,0,
		0,14,16,3,8,4,0,15,14,1,0,0,0,15,16,1,0,0,0,16,17,1,0,0,0,17,18,5,0,0,
		1,18,1,1,0,0,0,19,20,5,1,0,0,20,25,3,4,2,0,21,22,5,2,0,0,22,24,3,4,2,0,
		23,21,1,0,0,0,24,27,1,0,0,0,25,23,1,0,0,0,25,26,1,0,0,0,26,29,1,0,0,0,
		27,25,1,0,0,0,28,30,5,2,0,0,29,28,1,0,0,0,29,30,1,0,0,0,30,31,1,0,0,0,
		31,32,5,3,0,0,32,36,1,0,0,0,33,34,5,1,0,0,34,36,5,3,0,0,35,19,1,0,0,0,
		35,33,1,0,0,0,36,3,1,0,0,0,37,38,3,6,3,0,38,39,5,4,0,0,39,40,3,8,4,0,40,
		5,1,0,0,0,41,42,7,0,0,0,42,7,1,0,0,0,43,49,5,10,0,0,44,49,3,12,6,0,45,
		49,3,2,1,0,46,49,3,10,5,0,47,49,5,9,0,0,48,43,1,0,0,0,48,44,1,0,0,0,48,
		45,1,0,0,0,48,46,1,0,0,0,48,47,1,0,0,0,49,9,1,0,0,0,50,51,5,5,0,0,51,56,
		3,8,4,0,52,53,5,2,0,0,53,55,3,8,4,0,54,52,1,0,0,0,55,58,1,0,0,0,56,54,
		1,0,0,0,56,57,1,0,0,0,57,60,1,0,0,0,58,56,1,0,0,0,59,61,5,2,0,0,60,59,
		1,0,0,0,60,61,1,0,0,0,61,62,1,0,0,0,62,63,5,6,0,0,63,67,1,0,0,0,64,65,
		5,5,0,0,65,67,5,6,0,0,66,50,1,0,0,0,66,64,1,0,0,0,67,11,1,0,0,0,68,70,
		5,13,0,0,69,68,1,0,0,0,69,70,1,0,0,0,70,71,1,0,0,0,71,72,7,1,0,0,72,13,
		1,0,0,0,9,15,25,29,35,48,56,60,66,69
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
