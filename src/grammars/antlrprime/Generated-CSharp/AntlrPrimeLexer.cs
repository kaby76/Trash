//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from AntlrPrimeLexer.g4 by ANTLR 4.13.1

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
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class AntlrPrimeLexer : LexerAdaptor {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		TOKEN_REF=1, RULE_REF=2, LEXER_CHAR_SET=3, DOC_COMMENT=4, BLOCK_COMMENT=5, 
		LINE_COMMENT=6, INT=7, STRING_LITERAL=8, UNTERMINATED_STRING_LITERAL=9, 
		BEGIN_ARGUMENT=10, BEGIN_ACTION=11, OPTIONS=12, TOKENS=13, CHANNELS=14, 
		IMPORT=15, FRAGMENT=16, LEXER=17, PARSER=18, GRAMMAR=19, PROTECTED=20, 
		PUBLIC=21, PRIVATE=22, RETURNS=23, LOCALS=24, THROWS=25, CATCH=26, FINALLY=27, 
		MODE=28, COLON=29, COLONCOLON=30, COMMA=31, SEMI=32, LPAREN=33, RPAREN=34, 
		LBRACE=35, RBRACE=36, RARROW=37, LT=38, GT=39, ASSIGN=40, QUESTION=41, 
		STAR=42, PLUS_ASSIGN=43, PLUS=44, BANG=45, ROOT=46, OR=47, DOLLAR=48, 
		RANGE=49, DOT=50, AT=51, POUND=52, NOT=53, ID=54, WS=55, ERRCHAR=56, END_ARGUMENT=57, 
		UNTERMINATED_ARGUMENT=58, ARGUMENT_CONTENT=59, END_ACTION=60, UNTERMINATED_ACTION=61, 
		ACTION_CONTENT=62, UNTERMINATED_CHAR_SET=63;
	public const int
		OFF_CHANNEL=2, COMMENT=3;
	public const int
		Argument=1, TargetLanguageAction=2, LexerCharSet=3;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN", "OFF_CHANNEL", "COMMENT"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "Argument", "TargetLanguageAction", "LexerCharSet"
	};

	public static readonly string[] ruleNames = {
		"DOC_COMMENT", "BLOCK_COMMENT", "LINE_COMMENT", "INT", "STRING_LITERAL", 
		"UNTERMINATED_STRING_LITERAL", "BEGIN_ARGUMENT", "BEGIN_ACTION", "OPTIONS", 
		"TOKENS", "CHANNELS", "WSNLCHARS", "IMPORT", "FRAGMENT", "LEXER", "PARSER", 
		"GRAMMAR", "PROTECTED", "PUBLIC", "PRIVATE", "RETURNS", "LOCALS", "THROWS", 
		"CATCH", "FINALLY", "MODE", "COLON", "COLONCOLON", "COMMA", "SEMI", "LPAREN", 
		"RPAREN", "LBRACE", "RBRACE", "RARROW", "LT", "GT", "ASSIGN", "QUESTION", 
		"STAR", "PLUS_ASSIGN", "PLUS", "BANG", "ROOT", "OR", "DOLLAR", "RANGE", 
		"DOT", "AT", "POUND", "NOT", "ID", "WS", "ERRCHAR", "Ws", "Hws", "Vws", 
		"BlockComment", "DocComment", "LineComment", "EscSeq", "EscAny", "UnicodeEsc", 
		"DecimalNumeral", "HexDigit", "DecDigit", "BoolLiteral", "CharLiteral", 
		"SQuoteLiteral", "DQuoteLiteral", "USQuoteLiteral", "NameChar", "NameStartChar", 
		"Int", "Esc", "Colon", "DColon", "SQuote", "DQuote", "LParen", "RParen", 
		"LBrace", "RBrace", "LBrack", "RBrack", "RArrow", "Lt", "Gt", "Equal", 
		"Question", "Star", "Plus", "PlusAssign", "Underscore", "Pipe", "Root", 
		"Bang", "Dollar", "Comma", "Semi", "Dot", "Range", "At", "Pound", "Tilde", 
		"NESTED_ARGUMENT", "ARGUMENT_ESCAPE", "ARGUMENT_STRING_LITERAL", "ARGUMENT_CHAR_LITERAL", 
		"END_ARGUMENT", "UNTERMINATED_ARGUMENT", "ARGUMENT_CONTENT", "NESTED_ACTION", 
		"ACTION_ESCAPE", "ACTION_STRING_LITERAL", "ACTION_CHAR_LITERAL", "ACTION_DOC_COMMENT", 
		"ACTION_BLOCK_COMMENT", "ACTION_LINE_COMMENT", "END_ACTION", "UNTERMINATED_ACTION", 
		"ACTION_CONTENT", "LEXER_CHAR_SET_BODY", "LEXER_CHAR_SET", "UNTERMINATED_CHAR_SET", 
		"Id"
	};


	public AntlrPrimeLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public AntlrPrimeLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, "'import'", "'fragment'", "'lexer'", "'parser'", "'grammar'", 
		"'protected'", "'public'", "'private'", "'returns'", "'locals'", "'throws'", 
		"'catch'", "'finally'", "'mode'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "TOKEN_REF", "RULE_REF", "LEXER_CHAR_SET", "DOC_COMMENT", "BLOCK_COMMENT", 
		"LINE_COMMENT", "INT", "STRING_LITERAL", "UNTERMINATED_STRING_LITERAL", 
		"BEGIN_ARGUMENT", "BEGIN_ACTION", "OPTIONS", "TOKENS", "CHANNELS", "IMPORT", 
		"FRAGMENT", "LEXER", "PARSER", "GRAMMAR", "PROTECTED", "PUBLIC", "PRIVATE", 
		"RETURNS", "LOCALS", "THROWS", "CATCH", "FINALLY", "MODE", "COLON", "COLONCOLON", 
		"COMMA", "SEMI", "LPAREN", "RPAREN", "LBRACE", "RBRACE", "RARROW", "LT", 
		"GT", "ASSIGN", "QUESTION", "STAR", "PLUS_ASSIGN", "PLUS", "BANG", "ROOT", 
		"OR", "DOLLAR", "RANGE", "DOT", "AT", "POUND", "NOT", "ID", "WS", "ERRCHAR", 
		"END_ARGUMENT", "UNTERMINATED_ARGUMENT", "ARGUMENT_CONTENT", "END_ACTION", 
		"UNTERMINATED_ACTION", "ACTION_CONTENT", "UNTERMINATED_CHAR_SET"
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

	public override string GrammarFileName { get { return "AntlrPrimeLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static AntlrPrimeLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	public override void Action(RuleContext _localctx, int ruleIndex, int actionIndex) {
		switch (ruleIndex) {
		case 6 : BEGIN_ARGUMENT_action(_localctx, actionIndex); break;
		case 109 : END_ARGUMENT_action(_localctx, actionIndex); break;
		case 119 : END_ACTION_action(_localctx, actionIndex); break;
		}
	}
	private void BEGIN_ARGUMENT_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 0:  this.handleBeginArgument();  break;
		}
	}
	private void END_ARGUMENT_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 1:  this.handleEndArgument();  break;
		}
	}
	private void END_ACTION_action(RuleContext _localctx, int actionIndex) {
		switch (actionIndex) {
		case 2:  this.handleEndAction();  break;
		}
	}

	private static int[] _serializedATN = {
		4,0,63,791,6,-1,6,-1,6,-1,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,
		2,5,7,5,2,6,7,6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,
		2,13,7,13,2,14,7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,
		2,20,7,20,2,21,7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,
		2,27,7,27,2,28,7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,
		2,34,7,34,2,35,7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,
		2,41,7,41,2,42,7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,
		2,48,7,48,2,49,7,49,2,50,7,50,2,51,7,51,2,52,7,52,2,53,7,53,2,54,7,54,
		2,55,7,55,2,56,7,56,2,57,7,57,2,58,7,58,2,59,7,59,2,60,7,60,2,61,7,61,
		2,62,7,62,2,63,7,63,2,64,7,64,2,65,7,65,2,66,7,66,2,67,7,67,2,68,7,68,
		2,69,7,69,2,70,7,70,2,71,7,71,2,72,7,72,2,73,7,73,2,74,7,74,2,75,7,75,
		2,76,7,76,2,77,7,77,2,78,7,78,2,79,7,79,2,80,7,80,2,81,7,81,2,82,7,82,
		2,83,7,83,2,84,7,84,2,85,7,85,2,86,7,86,2,87,7,87,2,88,7,88,2,89,7,89,
		2,90,7,90,2,91,7,91,2,92,7,92,2,93,7,93,2,94,7,94,2,95,7,95,2,96,7,96,
		2,97,7,97,2,98,7,98,2,99,7,99,2,100,7,100,2,101,7,101,2,102,7,102,2,103,
		7,103,2,104,7,104,2,105,7,105,2,106,7,106,2,107,7,107,2,108,7,108,2,109,
		7,109,2,110,7,110,2,111,7,111,2,112,7,112,2,113,7,113,2,114,7,114,2,115,
		7,115,2,116,7,116,2,117,7,117,2,118,7,118,2,119,7,119,2,120,7,120,2,121,
		7,121,2,122,7,122,2,123,7,123,2,124,7,124,2,125,7,125,1,0,1,0,1,0,1,0,
		1,1,1,1,1,1,1,1,1,2,1,2,1,2,1,2,1,3,1,3,1,4,1,4,1,5,1,5,1,6,1,6,1,6,1,
		7,1,7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,5,8,291,8,8,10,8,12,
		8,294,9,8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,1,9,1,9,1,9,5,9,306,8,9,10,9,12,
		9,309,9,9,1,9,1,9,1,10,1,10,1,10,1,10,1,10,1,10,1,10,1,10,1,10,1,10,5,
		10,323,8,10,10,10,12,10,326,9,10,1,10,1,10,1,11,1,11,1,12,1,12,1,12,1,
		12,1,12,1,12,1,12,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,14,1,
		14,1,14,1,14,1,14,1,14,1,15,1,15,1,15,1,15,1,15,1,15,1,15,1,16,1,16,1,
		16,1,16,1,16,1,16,1,16,1,16,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,
		17,1,17,1,18,1,18,1,18,1,18,1,18,1,18,1,18,1,19,1,19,1,19,1,19,1,19,1,
		19,1,19,1,19,1,20,1,20,1,20,1,20,1,20,1,20,1,20,1,20,1,21,1,21,1,21,1,
		21,1,21,1,21,1,21,1,22,1,22,1,22,1,22,1,22,1,22,1,22,1,23,1,23,1,23,1,
		23,1,23,1,23,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,25,1,25,1,25,1,
		25,1,25,1,26,1,26,1,27,1,27,1,28,1,28,1,29,1,29,1,30,1,30,1,31,1,31,1,
		32,1,32,1,33,1,33,1,34,1,34,1,35,1,35,1,36,1,36,1,37,1,37,1,38,1,38,1,
		39,1,39,1,40,1,40,1,41,1,41,1,42,1,42,1,43,1,43,1,44,1,44,1,45,1,45,1,
		46,1,46,1,47,1,47,1,48,1,48,1,49,1,49,1,50,1,50,1,51,1,51,1,52,4,52,488,
		8,52,11,52,12,52,489,1,52,1,52,1,53,1,53,1,53,1,53,1,54,1,54,3,54,500,
		8,54,1,55,1,55,1,56,1,56,1,57,1,57,1,57,1,57,5,57,510,8,57,10,57,12,57,
		513,9,57,1,57,1,57,1,57,3,57,518,8,57,1,58,1,58,1,58,1,58,1,58,5,58,525,
		8,58,10,58,12,58,528,9,58,1,58,1,58,1,58,3,58,533,8,58,1,59,1,59,1,59,
		1,59,5,59,539,8,59,10,59,12,59,542,9,59,1,60,1,60,1,60,1,60,1,60,3,60,
		549,8,60,1,61,1,61,1,61,1,62,1,62,1,62,1,62,1,62,3,62,559,8,62,3,62,561,
		8,62,3,62,563,8,62,3,62,565,8,62,1,63,1,63,1,63,5,63,570,8,63,10,63,12,
		63,573,9,63,3,63,575,8,63,1,64,1,64,1,65,1,65,1,66,1,66,1,66,1,66,1,66,
		1,66,1,66,1,66,1,66,3,66,590,8,66,1,67,1,67,1,67,3,67,595,8,67,1,67,1,
		67,1,68,1,68,1,68,5,68,602,8,68,10,68,12,68,605,9,68,1,68,1,68,1,69,1,
		69,1,69,5,69,612,8,69,10,69,12,69,615,9,69,1,69,1,69,1,70,1,70,1,70,5,
		70,622,8,70,10,70,12,70,625,9,70,1,71,1,71,1,71,1,71,3,71,631,8,71,1,72,
		1,72,1,73,1,73,1,73,1,73,1,74,1,74,1,75,1,75,1,76,1,76,1,76,1,77,1,77,
		1,78,1,78,1,79,1,79,1,80,1,80,1,81,1,81,1,82,1,82,1,83,1,83,1,84,1,84,
		1,85,1,85,1,85,1,86,1,86,1,87,1,87,1,88,1,88,1,89,1,89,1,90,1,90,1,91,
		1,91,1,92,1,92,1,92,1,93,1,93,1,94,1,94,1,95,1,95,1,96,1,96,1,97,1,97,
		1,98,1,98,1,99,1,99,1,100,1,100,1,101,1,101,1,101,1,102,1,102,1,103,1,
		103,1,104,1,104,1,105,1,105,1,105,1,105,1,105,1,106,1,106,1,106,1,106,
		1,107,1,107,1,107,1,107,1,108,1,108,1,108,1,108,1,109,1,109,1,109,1,110,
		1,110,1,110,1,110,1,111,1,111,1,112,1,112,1,112,1,112,1,112,1,113,1,113,
		1,113,1,113,1,114,1,114,1,114,1,114,1,115,1,115,1,115,1,115,1,116,1,116,
		1,116,1,116,1,117,1,117,1,117,1,117,1,118,1,118,1,118,1,118,1,119,1,119,
		1,119,1,120,1,120,1,120,1,120,1,121,1,121,1,122,1,122,4,122,771,8,122,
		11,122,12,122,772,1,122,1,122,1,123,1,123,1,123,1,123,1,124,1,124,1,124,
		1,124,1,125,1,125,5,125,787,8,125,10,125,12,125,790,9,125,2,511,526,0,
		126,4,4,6,5,8,6,10,7,12,8,14,9,16,10,18,11,20,12,22,13,24,14,26,0,28,15,
		30,16,32,17,34,18,36,19,38,20,40,21,42,22,44,23,46,24,48,25,50,26,52,27,
		54,28,56,29,58,30,60,31,62,32,64,33,66,34,68,35,70,36,72,37,74,38,76,39,
		78,40,80,41,82,42,84,43,86,44,88,45,90,46,92,47,94,48,96,49,98,50,100,
		51,102,52,104,53,106,54,108,55,110,56,112,0,114,0,116,0,118,0,120,0,122,
		0,124,0,126,0,128,0,130,0,132,0,134,0,136,0,138,0,140,0,142,0,144,0,146,
		0,148,0,150,0,152,0,154,0,156,0,158,0,160,0,162,0,164,0,166,0,168,0,170,
		0,172,0,174,0,176,0,178,0,180,0,182,0,184,0,186,0,188,0,190,0,192,0,194,
		0,196,0,198,0,200,0,202,0,204,0,206,0,208,0,210,0,212,0,214,0,216,0,218,
		0,220,0,222,57,224,58,226,59,228,0,230,0,232,0,234,0,236,0,238,0,240,0,
		242,60,244,61,246,62,248,0,250,3,252,63,254,0,4,0,1,2,3,13,3,0,9,10,12,
		13,32,32,2,0,9,9,32,32,2,0,10,10,12,13,2,0,10,10,13,13,8,0,34,34,39,39,
		92,92,98,98,102,102,110,110,114,114,116,116,1,0,49,57,3,0,48,57,65,70,
		97,102,1,0,48,57,4,0,10,10,13,13,39,39,92,92,4,0,10,10,13,13,34,34,92,
		92,3,0,183,183,768,879,8255,8256,13,0,65,90,97,122,192,214,216,246,248,
		767,880,893,895,8191,8204,8205,8304,8591,11264,12271,12289,55295,63744,
		64975,65008,65533,1,0,92,93,767,0,4,1,0,0,0,0,6,1,0,0,0,0,8,1,0,0,0,0,
		10,1,0,0,0,0,12,1,0,0,0,0,14,1,0,0,0,0,16,1,0,0,0,0,18,1,0,0,0,0,20,1,
		0,0,0,0,22,1,0,0,0,0,24,1,0,0,0,0,28,1,0,0,0,0,30,1,0,0,0,0,32,1,0,0,0,
		0,34,1,0,0,0,0,36,1,0,0,0,0,38,1,0,0,0,0,40,1,0,0,0,0,42,1,0,0,0,0,44,
		1,0,0,0,0,46,1,0,0,0,0,48,1,0,0,0,0,50,1,0,0,0,0,52,1,0,0,0,0,54,1,0,0,
		0,0,56,1,0,0,0,0,58,1,0,0,0,0,60,1,0,0,0,0,62,1,0,0,0,0,64,1,0,0,0,0,66,
		1,0,0,0,0,68,1,0,0,0,0,70,1,0,0,0,0,72,1,0,0,0,0,74,1,0,0,0,0,76,1,0,0,
		0,0,78,1,0,0,0,0,80,1,0,0,0,0,82,1,0,0,0,0,84,1,0,0,0,0,86,1,0,0,0,0,88,
		1,0,0,0,0,90,1,0,0,0,0,92,1,0,0,0,0,94,1,0,0,0,0,96,1,0,0,0,0,98,1,0,0,
		0,0,100,1,0,0,0,0,102,1,0,0,0,0,104,1,0,0,0,0,106,1,0,0,0,0,108,1,0,0,
		0,0,110,1,0,0,0,1,214,1,0,0,0,1,216,1,0,0,0,1,218,1,0,0,0,1,220,1,0,0,
		0,1,222,1,0,0,0,1,224,1,0,0,0,1,226,1,0,0,0,2,228,1,0,0,0,2,230,1,0,0,
		0,2,232,1,0,0,0,2,234,1,0,0,0,2,236,1,0,0,0,2,238,1,0,0,0,2,240,1,0,0,
		0,2,242,1,0,0,0,2,244,1,0,0,0,2,246,1,0,0,0,3,248,1,0,0,0,3,250,1,0,0,
		0,3,252,1,0,0,0,4,256,1,0,0,0,6,260,1,0,0,0,8,264,1,0,0,0,10,268,1,0,0,
		0,12,270,1,0,0,0,14,272,1,0,0,0,16,274,1,0,0,0,18,277,1,0,0,0,20,281,1,
		0,0,0,22,297,1,0,0,0,24,312,1,0,0,0,26,329,1,0,0,0,28,331,1,0,0,0,30,338,
		1,0,0,0,32,347,1,0,0,0,34,353,1,0,0,0,36,360,1,0,0,0,38,368,1,0,0,0,40,
		378,1,0,0,0,42,385,1,0,0,0,44,393,1,0,0,0,46,401,1,0,0,0,48,408,1,0,0,
		0,50,415,1,0,0,0,52,421,1,0,0,0,54,429,1,0,0,0,56,434,1,0,0,0,58,436,1,
		0,0,0,60,438,1,0,0,0,62,440,1,0,0,0,64,442,1,0,0,0,66,444,1,0,0,0,68,446,
		1,0,0,0,70,448,1,0,0,0,72,450,1,0,0,0,74,452,1,0,0,0,76,454,1,0,0,0,78,
		456,1,0,0,0,80,458,1,0,0,0,82,460,1,0,0,0,84,462,1,0,0,0,86,464,1,0,0,
		0,88,466,1,0,0,0,90,468,1,0,0,0,92,470,1,0,0,0,94,472,1,0,0,0,96,474,1,
		0,0,0,98,476,1,0,0,0,100,478,1,0,0,0,102,480,1,0,0,0,104,482,1,0,0,0,106,
		484,1,0,0,0,108,487,1,0,0,0,110,493,1,0,0,0,112,499,1,0,0,0,114,501,1,
		0,0,0,116,503,1,0,0,0,118,505,1,0,0,0,120,519,1,0,0,0,122,534,1,0,0,0,
		124,543,1,0,0,0,126,550,1,0,0,0,128,553,1,0,0,0,130,574,1,0,0,0,132,576,
		1,0,0,0,134,578,1,0,0,0,136,589,1,0,0,0,138,591,1,0,0,0,140,598,1,0,0,
		0,142,608,1,0,0,0,144,618,1,0,0,0,146,630,1,0,0,0,148,632,1,0,0,0,150,
		634,1,0,0,0,152,638,1,0,0,0,154,640,1,0,0,0,156,642,1,0,0,0,158,645,1,
		0,0,0,160,647,1,0,0,0,162,649,1,0,0,0,164,651,1,0,0,0,166,653,1,0,0,0,
		168,655,1,0,0,0,170,657,1,0,0,0,172,659,1,0,0,0,174,661,1,0,0,0,176,664,
		1,0,0,0,178,666,1,0,0,0,180,668,1,0,0,0,182,670,1,0,0,0,184,672,1,0,0,
		0,186,674,1,0,0,0,188,676,1,0,0,0,190,679,1,0,0,0,192,681,1,0,0,0,194,
		683,1,0,0,0,196,685,1,0,0,0,198,687,1,0,0,0,200,689,1,0,0,0,202,691,1,
		0,0,0,204,693,1,0,0,0,206,695,1,0,0,0,208,698,1,0,0,0,210,700,1,0,0,0,
		212,702,1,0,0,0,214,704,1,0,0,0,216,709,1,0,0,0,218,713,1,0,0,0,220,717,
		1,0,0,0,222,721,1,0,0,0,224,724,1,0,0,0,226,728,1,0,0,0,228,730,1,0,0,
		0,230,735,1,0,0,0,232,739,1,0,0,0,234,743,1,0,0,0,236,747,1,0,0,0,238,
		751,1,0,0,0,240,755,1,0,0,0,242,759,1,0,0,0,244,762,1,0,0,0,246,766,1,
		0,0,0,248,770,1,0,0,0,250,776,1,0,0,0,252,780,1,0,0,0,254,784,1,0,0,0,
		256,257,3,120,58,0,257,258,1,0,0,0,258,259,6,0,0,0,259,5,1,0,0,0,260,261,
		3,118,57,0,261,262,1,0,0,0,262,263,6,1,0,0,263,7,1,0,0,0,264,265,3,122,
		59,0,265,266,1,0,0,0,266,267,6,2,0,0,267,9,1,0,0,0,268,269,3,130,63,0,
		269,11,1,0,0,0,270,271,3,140,68,0,271,13,1,0,0,0,272,273,3,144,70,0,273,
		15,1,0,0,0,274,275,3,170,83,0,275,276,6,6,1,0,276,17,1,0,0,0,277,278,3,
		166,81,0,278,279,1,0,0,0,279,280,6,7,2,0,280,19,1,0,0,0,281,282,5,111,
		0,0,282,283,5,112,0,0,283,284,5,116,0,0,284,285,5,105,0,0,285,286,5,111,
		0,0,286,287,5,110,0,0,287,288,5,115,0,0,288,292,1,0,0,0,289,291,3,26,11,
		0,290,289,1,0,0,0,291,294,1,0,0,0,292,290,1,0,0,0,292,293,1,0,0,0,293,
		295,1,0,0,0,294,292,1,0,0,0,295,296,5,123,0,0,296,21,1,0,0,0,297,298,5,
		116,0,0,298,299,5,111,0,0,299,300,5,107,0,0,300,301,5,101,0,0,301,302,
		5,110,0,0,302,303,5,115,0,0,303,307,1,0,0,0,304,306,3,26,11,0,305,304,
		1,0,0,0,306,309,1,0,0,0,307,305,1,0,0,0,307,308,1,0,0,0,308,310,1,0,0,
		0,309,307,1,0,0,0,310,311,5,123,0,0,311,23,1,0,0,0,312,313,5,99,0,0,313,
		314,5,104,0,0,314,315,5,97,0,0,315,316,5,110,0,0,316,317,5,110,0,0,317,
		318,5,101,0,0,318,319,5,108,0,0,319,320,5,115,0,0,320,324,1,0,0,0,321,
		323,3,26,11,0,322,321,1,0,0,0,323,326,1,0,0,0,324,322,1,0,0,0,324,325,
		1,0,0,0,325,327,1,0,0,0,326,324,1,0,0,0,327,328,5,123,0,0,328,25,1,0,0,
		0,329,330,7,0,0,0,330,27,1,0,0,0,331,332,5,105,0,0,332,333,5,109,0,0,333,
		334,5,112,0,0,334,335,5,111,0,0,335,336,5,114,0,0,336,337,5,116,0,0,337,
		29,1,0,0,0,338,339,5,102,0,0,339,340,5,114,0,0,340,341,5,97,0,0,341,342,
		5,103,0,0,342,343,5,109,0,0,343,344,5,101,0,0,344,345,5,110,0,0,345,346,
		5,116,0,0,346,31,1,0,0,0,347,348,5,108,0,0,348,349,5,101,0,0,349,350,5,
		120,0,0,350,351,5,101,0,0,351,352,5,114,0,0,352,33,1,0,0,0,353,354,5,112,
		0,0,354,355,5,97,0,0,355,356,5,114,0,0,356,357,5,115,0,0,357,358,5,101,
		0,0,358,359,5,114,0,0,359,35,1,0,0,0,360,361,5,103,0,0,361,362,5,114,0,
		0,362,363,5,97,0,0,363,364,5,109,0,0,364,365,5,109,0,0,365,366,5,97,0,
		0,366,367,5,114,0,0,367,37,1,0,0,0,368,369,5,112,0,0,369,370,5,114,0,0,
		370,371,5,111,0,0,371,372,5,116,0,0,372,373,5,101,0,0,373,374,5,99,0,0,
		374,375,5,116,0,0,375,376,5,101,0,0,376,377,5,100,0,0,377,39,1,0,0,0,378,
		379,5,112,0,0,379,380,5,117,0,0,380,381,5,98,0,0,381,382,5,108,0,0,382,
		383,5,105,0,0,383,384,5,99,0,0,384,41,1,0,0,0,385,386,5,112,0,0,386,387,
		5,114,0,0,387,388,5,105,0,0,388,389,5,118,0,0,389,390,5,97,0,0,390,391,
		5,116,0,0,391,392,5,101,0,0,392,43,1,0,0,0,393,394,5,114,0,0,394,395,5,
		101,0,0,395,396,5,116,0,0,396,397,5,117,0,0,397,398,5,114,0,0,398,399,
		5,110,0,0,399,400,5,115,0,0,400,45,1,0,0,0,401,402,5,108,0,0,402,403,5,
		111,0,0,403,404,5,99,0,0,404,405,5,97,0,0,405,406,5,108,0,0,406,407,5,
		115,0,0,407,47,1,0,0,0,408,409,5,116,0,0,409,410,5,104,0,0,410,411,5,114,
		0,0,411,412,5,111,0,0,412,413,5,119,0,0,413,414,5,115,0,0,414,49,1,0,0,
		0,415,416,5,99,0,0,416,417,5,97,0,0,417,418,5,116,0,0,418,419,5,99,0,0,
		419,420,5,104,0,0,420,51,1,0,0,0,421,422,5,102,0,0,422,423,5,105,0,0,423,
		424,5,110,0,0,424,425,5,97,0,0,425,426,5,108,0,0,426,427,5,108,0,0,427,
		428,5,121,0,0,428,53,1,0,0,0,429,430,5,109,0,0,430,431,5,111,0,0,431,432,
		5,100,0,0,432,433,5,101,0,0,433,55,1,0,0,0,434,435,3,154,75,0,435,57,1,
		0,0,0,436,437,3,156,76,0,437,59,1,0,0,0,438,439,3,200,98,0,439,61,1,0,
		0,0,440,441,3,202,99,0,441,63,1,0,0,0,442,443,3,162,79,0,443,65,1,0,0,
		0,444,445,3,164,80,0,445,67,1,0,0,0,446,447,3,166,81,0,447,69,1,0,0,0,
		448,449,3,168,82,0,449,71,1,0,0,0,450,451,3,174,85,0,451,73,1,0,0,0,452,
		453,3,176,86,0,453,75,1,0,0,0,454,455,3,178,87,0,455,77,1,0,0,0,456,457,
		3,180,88,0,457,79,1,0,0,0,458,459,3,182,89,0,459,81,1,0,0,0,460,461,3,
		184,90,0,461,83,1,0,0,0,462,463,3,188,92,0,463,85,1,0,0,0,464,465,3,186,
		91,0,465,87,1,0,0,0,466,467,3,196,96,0,467,89,1,0,0,0,468,469,3,194,95,
		0,469,91,1,0,0,0,470,471,3,192,94,0,471,93,1,0,0,0,472,473,3,198,97,0,
		473,95,1,0,0,0,474,475,3,206,101,0,475,97,1,0,0,0,476,477,3,204,100,0,
		477,99,1,0,0,0,478,479,3,208,102,0,479,101,1,0,0,0,480,481,3,210,103,0,
		481,103,1,0,0,0,482,483,3,212,104,0,483,105,1,0,0,0,484,485,3,254,125,
		0,485,107,1,0,0,0,486,488,3,112,54,0,487,486,1,0,0,0,488,489,1,0,0,0,489,
		487,1,0,0,0,489,490,1,0,0,0,490,491,1,0,0,0,491,492,6,52,3,0,492,109,1,
		0,0,0,493,494,9,0,0,0,494,495,1,0,0,0,495,496,6,53,4,0,496,111,1,0,0,0,
		497,500,3,114,55,0,498,500,3,116,56,0,499,497,1,0,0,0,499,498,1,0,0,0,
		500,113,1,0,0,0,501,502,7,1,0,0,502,115,1,0,0,0,503,504,7,2,0,0,504,117,
		1,0,0,0,505,506,5,47,0,0,506,507,5,42,0,0,507,511,1,0,0,0,508,510,9,0,
		0,0,509,508,1,0,0,0,510,513,1,0,0,0,511,512,1,0,0,0,511,509,1,0,0,0,512,
		517,1,0,0,0,513,511,1,0,0,0,514,515,5,42,0,0,515,518,5,47,0,0,516,518,
		5,0,0,1,517,514,1,0,0,0,517,516,1,0,0,0,518,119,1,0,0,0,519,520,5,47,0,
		0,520,521,5,42,0,0,521,522,5,42,0,0,522,526,1,0,0,0,523,525,9,0,0,0,524,
		523,1,0,0,0,525,528,1,0,0,0,526,527,1,0,0,0,526,524,1,0,0,0,527,532,1,
		0,0,0,528,526,1,0,0,0,529,530,5,42,0,0,530,533,5,47,0,0,531,533,5,0,0,
		1,532,529,1,0,0,0,532,531,1,0,0,0,533,121,1,0,0,0,534,535,5,47,0,0,535,
		536,5,47,0,0,536,540,1,0,0,0,537,539,8,3,0,0,538,537,1,0,0,0,539,542,1,
		0,0,0,540,538,1,0,0,0,540,541,1,0,0,0,541,123,1,0,0,0,542,540,1,0,0,0,
		543,548,3,152,74,0,544,549,7,4,0,0,545,549,3,128,62,0,546,549,9,0,0,0,
		547,549,5,0,0,1,548,544,1,0,0,0,548,545,1,0,0,0,548,546,1,0,0,0,548,547,
		1,0,0,0,549,125,1,0,0,0,550,551,3,152,74,0,551,552,9,0,0,0,552,127,1,0,
		0,0,553,564,5,117,0,0,554,562,3,132,64,0,555,560,3,132,64,0,556,558,3,
		132,64,0,557,559,3,132,64,0,558,557,1,0,0,0,558,559,1,0,0,0,559,561,1,
		0,0,0,560,556,1,0,0,0,560,561,1,0,0,0,561,563,1,0,0,0,562,555,1,0,0,0,
		562,563,1,0,0,0,563,565,1,0,0,0,564,554,1,0,0,0,564,565,1,0,0,0,565,129,
		1,0,0,0,566,575,5,48,0,0,567,571,7,5,0,0,568,570,3,134,65,0,569,568,1,
		0,0,0,570,573,1,0,0,0,571,569,1,0,0,0,571,572,1,0,0,0,572,575,1,0,0,0,
		573,571,1,0,0,0,574,566,1,0,0,0,574,567,1,0,0,0,575,131,1,0,0,0,576,577,
		7,6,0,0,577,133,1,0,0,0,578,579,7,7,0,0,579,135,1,0,0,0,580,581,5,116,
		0,0,581,582,5,114,0,0,582,583,5,117,0,0,583,590,5,101,0,0,584,585,5,102,
		0,0,585,586,5,97,0,0,586,587,5,108,0,0,587,588,5,115,0,0,588,590,5,101,
		0,0,589,580,1,0,0,0,589,584,1,0,0,0,590,137,1,0,0,0,591,594,3,158,77,0,
		592,595,3,124,60,0,593,595,8,8,0,0,594,592,1,0,0,0,594,593,1,0,0,0,595,
		596,1,0,0,0,596,597,3,158,77,0,597,139,1,0,0,0,598,603,3,158,77,0,599,
		602,3,124,60,0,600,602,8,8,0,0,601,599,1,0,0,0,601,600,1,0,0,0,602,605,
		1,0,0,0,603,601,1,0,0,0,603,604,1,0,0,0,604,606,1,0,0,0,605,603,1,0,0,
		0,606,607,3,158,77,0,607,141,1,0,0,0,608,613,3,160,78,0,609,612,3,124,
		60,0,610,612,8,9,0,0,611,609,1,0,0,0,611,610,1,0,0,0,612,615,1,0,0,0,613,
		611,1,0,0,0,613,614,1,0,0,0,614,616,1,0,0,0,615,613,1,0,0,0,616,617,3,
		160,78,0,617,143,1,0,0,0,618,623,3,158,77,0,619,622,3,124,60,0,620,622,
		8,8,0,0,621,619,1,0,0,0,621,620,1,0,0,0,622,625,1,0,0,0,623,621,1,0,0,
		0,623,624,1,0,0,0,624,145,1,0,0,0,625,623,1,0,0,0,626,631,3,148,72,0,627,
		631,2,48,57,0,628,631,3,190,93,0,629,631,7,10,0,0,630,626,1,0,0,0,630,
		627,1,0,0,0,630,628,1,0,0,0,630,629,1,0,0,0,631,147,1,0,0,0,632,633,7,
		11,0,0,633,149,1,0,0,0,634,635,5,105,0,0,635,636,5,110,0,0,636,637,5,116,
		0,0,637,151,1,0,0,0,638,639,5,92,0,0,639,153,1,0,0,0,640,641,5,58,0,0,
		641,155,1,0,0,0,642,643,5,58,0,0,643,644,5,58,0,0,644,157,1,0,0,0,645,
		646,5,39,0,0,646,159,1,0,0,0,647,648,5,34,0,0,648,161,1,0,0,0,649,650,
		5,40,0,0,650,163,1,0,0,0,651,652,5,41,0,0,652,165,1,0,0,0,653,654,5,123,
		0,0,654,167,1,0,0,0,655,656,5,125,0,0,656,169,1,0,0,0,657,658,5,91,0,0,
		658,171,1,0,0,0,659,660,5,93,0,0,660,173,1,0,0,0,661,662,5,45,0,0,662,
		663,5,62,0,0,663,175,1,0,0,0,664,665,5,60,0,0,665,177,1,0,0,0,666,667,
		5,62,0,0,667,179,1,0,0,0,668,669,5,61,0,0,669,181,1,0,0,0,670,671,5,63,
		0,0,671,183,1,0,0,0,672,673,5,42,0,0,673,185,1,0,0,0,674,675,5,43,0,0,
		675,187,1,0,0,0,676,677,5,43,0,0,677,678,5,61,0,0,678,189,1,0,0,0,679,
		680,5,95,0,0,680,191,1,0,0,0,681,682,5,124,0,0,682,193,1,0,0,0,683,684,
		5,94,0,0,684,195,1,0,0,0,685,686,5,33,0,0,686,197,1,0,0,0,687,688,5,36,
		0,0,688,199,1,0,0,0,689,690,5,44,0,0,690,201,1,0,0,0,691,692,5,59,0,0,
		692,203,1,0,0,0,693,694,5,46,0,0,694,205,1,0,0,0,695,696,5,46,0,0,696,
		697,5,46,0,0,697,207,1,0,0,0,698,699,5,64,0,0,699,209,1,0,0,0,700,701,
		5,35,0,0,701,211,1,0,0,0,702,703,5,126,0,0,703,213,1,0,0,0,704,705,3,170,
		83,0,705,706,1,0,0,0,706,707,6,105,5,0,707,708,6,105,6,0,708,215,1,0,0,
		0,709,710,3,126,61,0,710,711,1,0,0,0,711,712,6,106,5,0,712,217,1,0,0,0,
		713,714,3,142,69,0,714,715,1,0,0,0,715,716,6,107,5,0,716,219,1,0,0,0,717,
		718,3,140,68,0,718,719,1,0,0,0,719,720,6,108,5,0,720,221,1,0,0,0,721,722,
		3,172,84,0,722,723,6,109,7,0,723,223,1,0,0,0,724,725,5,0,0,1,725,726,1,
		0,0,0,726,727,6,110,8,0,727,225,1,0,0,0,728,729,9,0,0,0,729,227,1,0,0,
		0,730,731,3,166,81,0,731,732,1,0,0,0,732,733,6,112,9,0,733,734,6,112,2,
		0,734,229,1,0,0,0,735,736,3,126,61,0,736,737,1,0,0,0,737,738,6,113,9,0,
		738,231,1,0,0,0,739,740,3,142,69,0,740,741,1,0,0,0,741,742,6,114,9,0,742,
		233,1,0,0,0,743,744,3,140,68,0,744,745,1,0,0,0,745,746,6,115,9,0,746,235,
		1,0,0,0,747,748,3,120,58,0,748,749,1,0,0,0,749,750,6,116,9,0,750,237,1,
		0,0,0,751,752,3,118,57,0,752,753,1,0,0,0,753,754,6,117,9,0,754,239,1,0,
		0,0,755,756,3,122,59,0,756,757,1,0,0,0,757,758,6,118,9,0,758,241,1,0,0,
		0,759,760,3,168,82,0,760,761,6,119,10,0,761,243,1,0,0,0,762,763,5,0,0,
		1,763,764,1,0,0,0,764,765,6,120,8,0,765,245,1,0,0,0,766,767,9,0,0,0,767,
		247,1,0,0,0,768,771,8,12,0,0,769,771,3,126,61,0,770,768,1,0,0,0,770,769,
		1,0,0,0,771,772,1,0,0,0,772,770,1,0,0,0,772,773,1,0,0,0,773,774,1,0,0,
		0,774,775,6,122,11,0,775,249,1,0,0,0,776,777,3,172,84,0,777,778,1,0,0,
		0,778,779,6,123,8,0,779,251,1,0,0,0,780,781,5,0,0,1,781,782,1,0,0,0,782,
		783,6,124,8,0,783,253,1,0,0,0,784,788,3,148,72,0,785,787,3,146,71,0,786,
		785,1,0,0,0,787,790,1,0,0,0,788,786,1,0,0,0,788,789,1,0,0,0,789,255,1,
		0,0,0,790,788,1,0,0,0,33,0,1,2,3,292,307,324,489,499,511,517,526,532,540,
		548,558,560,562,564,571,574,589,594,601,603,611,613,621,623,630,770,772,
		788,12,0,3,0,1,6,0,5,2,0,0,2,0,0,1,0,7,59,0,5,1,0,1,109,1,4,0,0,7,62,0,
		1,119,2,3,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}