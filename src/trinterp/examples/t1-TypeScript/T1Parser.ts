// Generated from T1.g4 by ANTLR 4.13.2
// noinspection ES6UnusedImports,JSUnusedGlobalSymbols,JSUnusedLocalSymbols

import {
	ATN,
	ATNDeserializer, DecisionState, DFA, FailedPredicateException,
	RecognitionException, NoViableAltException, BailErrorStrategy,
	Parser, ParserATNSimulator,
	RuleContext, ParserRuleContext, PredictionMode, PredictionContextCache,
	TerminalNode, RuleNode,
	Token, TokenStream,
	Interval, IntervalSet
} from 'antlr4';
import T1Listener from "./T1Listener.js";
// for running tests with parameters, TODO: discuss strategy for typed parameters in CI
// eslint-disable-next-line no-unused-vars
type int = number;

export default class T1Parser extends Parser {
	public static readonly T__0 = 1;
	public static override readonly EOF = Token.EOF;
	public static readonly RULE_s = 0;
	public static readonly literalNames: (string | null)[] = [ null, "'a'" ];
	public static readonly symbolicNames: (string | null)[] = [  ];
	// tslint:disable:no-trailing-whitespace
	public static readonly ruleNames: string[] = [
		"s",
	];
	public get grammarFileName(): string { return "T1.g4"; }
	public get literalNames(): (string | null)[] { return T1Parser.literalNames; }
	public get symbolicNames(): (string | null)[] { return T1Parser.symbolicNames; }
	public get ruleNames(): string[] { return T1Parser.ruleNames; }
	public get serializedATN(): number[] { return T1Parser._serializedATN; }

	protected createFailedPredicateException(predicate?: string, message?: string): FailedPredicateException {
		return new FailedPredicateException(this, predicate, message);
	}

	constructor(input: TokenStream) {
		super(input);
		this._interp = new ParserATNSimulator(this, T1Parser._ATN, T1Parser.DecisionsToDFA, new PredictionContextCache());
	}
	// @RuleVersion(0)
	public s(): SContext {
		let localctx: SContext = new SContext(this, this._ctx, this.state);
		this.enterRule(localctx, 0, T1Parser.RULE_s);
		try {
			this.enterOuterAlt(localctx, 1);
			{
			this.state = 2;
			this.match(T1Parser.T__0);
			this.state = 3;
			this.match(T1Parser.EOF);
			}
		}
		catch (re) {
			if (re instanceof RecognitionException) {
				localctx.exception = re;
				this._errHandler.reportError(this, re);
				this._errHandler.recover(this, re);
			} else {
				throw re;
			}
		}
		finally {
			this.exitRule();
		}
		return localctx;
	}

	public static readonly _serializedATN: number[] = [4,1,1,6,2,0,7,0,1,0,
	1,0,1,0,1,0,0,0,1,0,0,0,4,0,2,1,0,0,0,2,3,5,1,0,0,3,4,5,0,0,1,4,1,1,0,0,
	0,0];

	private static __ATN: ATN;
	public static get _ATN(): ATN {
		if (!T1Parser.__ATN) {
			T1Parser.__ATN = new ATNDeserializer().deserialize(T1Parser._serializedATN);
		}

		return T1Parser.__ATN;
	}


	static DecisionsToDFA = T1Parser._ATN.decisionToState.map( (ds: DecisionState, index: number) => new DFA(ds, index) );

}

export class SContext extends ParserRuleContext {
	constructor(parser?: T1Parser, parent?: ParserRuleContext, invokingState?: number) {
		super(parent, invokingState);
    	this.parser = parser;
	}
	public EOF(): TerminalNode {
		return this.getToken(T1Parser.EOF, 0);
	}
    public get ruleIndex(): number {
    	return T1Parser.RULE_s;
	}
	public enterRule(listener: T1Listener): void {
	    if(listener.enterS) {
	 		listener.enterS(this);
		}
	}
	public exitRule(listener: T1Listener): void {
	    if(listener.exitS) {
	 		listener.exitS(this);
		}
	}
}
