// Generated from T1.g4 by ANTLR 4.13.2
// noinspection ES6UnusedImports,JSUnusedGlobalSymbols,JSUnusedLocalSymbols
import {
	ATN,
	ATNDeserializer,
	CharStream,
	DecisionState, DFA,
	Lexer,
	LexerATNSimulator,
	RuleContext,
	PredictionContextCache,
	Token
} from "antlr4";
export default class T1Lexer extends Lexer {
	public static readonly T__0 = 1;
	public static readonly EOF = Token.EOF;

	public static readonly channelNames: string[] = [ "DEFAULT_TOKEN_CHANNEL", "HIDDEN" ];
	public static readonly literalNames: (string | null)[] = [ null, "'a'" ];
	public static readonly symbolicNames: (string | null)[] = [  ];
	public static readonly modeNames: string[] = [ "DEFAULT_MODE", ];

	public static readonly ruleNames: string[] = [
		"T__0",
	];


	constructor(input: CharStream) {
		super(input);
		this._interp = new LexerATNSimulator(this, T1Lexer._ATN, T1Lexer.DecisionsToDFA, new PredictionContextCache());
	}

	public get grammarFileName(): string { return "T1.g4"; }

	public get literalNames(): (string | null)[] { return T1Lexer.literalNames; }
	public get symbolicNames(): (string | null)[] { return T1Lexer.symbolicNames; }
	public get ruleNames(): string[] { return T1Lexer.ruleNames; }

	public get serializedATN(): number[] { return T1Lexer._serializedATN; }

	public get channelNames(): string[] { return T1Lexer.channelNames; }

	public get modeNames(): string[] { return T1Lexer.modeNames; }

	public static readonly _serializedATN: number[] = [4,0,1,5,6,-1,2,0,7,0,
	1,0,1,0,0,0,1,1,1,1,0,0,4,0,1,1,0,0,0,1,3,1,0,0,0,3,4,5,97,0,0,4,2,1,0,
	0,0,1,0,0];

	private static __ATN: ATN;
	public static get _ATN(): ATN {
		if (!T1Lexer.__ATN) {
			T1Lexer.__ATN = new ATNDeserializer().deserialize(T1Lexer._serializedATN);
		}

		return T1Lexer.__ATN;
	}


	static DecisionsToDFA = T1Lexer._ATN.decisionToState.map( (ds: DecisionState, index: number) => new DFA(ds, index) );
}