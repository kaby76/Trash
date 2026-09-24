/*
 * Copyright (c) Mike Lischke. All rights reserved.
 * Licensed under the BSD 3-clause License. See License.txt in the project root for license information.
 */

import { CommonToken, Lexer, CharStream, Token } from "antlr4";
import G4PlusLexer from './G4PlusLexer';

export default abstract class LexerAdaptor extends Lexer {
    /** Generic type for OPTIONS, TOKENS and CHANNELS */
    private static PREQUEL_CONSTRUCT = -10;
    private static OPTIONS_CONSTRUCT = -11;

    private currentRuleType: number = Token.INVALID_TYPE;

    public constructor(input: CharStream) {
        super(input);

        /**
         * Track whether we are inside of a rule and whether it is lexical parser. _currentRuleType==Token.INVALID_TYPE
         * means that we are outside of a rule. At the first sign of a rule name reference and _currentRuleType
         * ==invalid, we can assume that we are starting a parser rule. Similarly, seeing a token reference when not
         * already in rule means starting a token rule. The terminating ';' of a rule, flips this back to invalid type.
         *
         * This is not perfect logic but works. For example, "grammar T;" means that we start and stop a lexical rule
         * for the "T;". Dangerous but works.
         *
         * The whole point of this state information is to distinguish between [..arg actions..] and [char sets].
         * Char sets can only occur in lexical rules and arg actions cannot occur.
         */
        this.currentRuleType = Token.INVALID_TYPE;
    }

    public override reset(): void {
        this.currentRuleType = Token.INVALID_TYPE;
        super.reset();
    }

    public override emit(): Token {
        if ((this._type === G4PlusLexer.OPTIONS || this._type === G4PlusLexer.TOKENS
            || this._type === G4PlusLexer.CHANNELS)
            && this.currentRuleType === Token.INVALID_TYPE) {
            // Enter prequel construct ending with an RBRACE.
            this.currentRuleType = LexerAdaptor.PREQUEL_CONSTRUCT;
        } else if (this._type === G4PlusLexer.OPTIONS && this.currentRuleType === G4PlusLexer.TOKEN_REF) {
            this.currentRuleType = LexerAdaptor.OPTIONS_CONSTRUCT;
        } else if (this._type === G4PlusLexer.RBRACE
            && this.currentRuleType === LexerAdaptor.PREQUEL_CONSTRUCT) {
            // Exit prequel construct.
            this.currentRuleType = Token.INVALID_TYPE;
        } else if (this._type === G4PlusLexer.RBRACE
            && this.currentRuleType === LexerAdaptor.OPTIONS_CONSTRUCT) {
            // Exit options.
            this.currentRuleType = G4PlusLexer.TOKEN_REF;
        } else if (this._type === G4PlusLexer.AT && this.currentRuleType === Token.INVALID_TYPE) {
            // Enter action.
            this.currentRuleType = G4PlusLexer.AT;
        } else if (this._type === G4PlusLexer.SEMI
            && this.currentRuleType === LexerAdaptor.OPTIONS_CONSTRUCT) {
            // ';' in options { .... }. Don't change anything.
        } else if (this._type == G4PlusLexer.ACTION && this.currentRuleType == G4PlusLexer.AT) {
            // Exit action
            this.currentRuleType = Token.INVALID_TYPE;
        } else if (this._type === G4PlusLexer.ID) {
            const firstChar = this._input.getText(this._tokenStartCharIndex, this._tokenStartCharIndex);
            const c = firstChar.charAt(0);
            if (c === c.toUpperCase()) {
                this._type = G4PlusLexer.TOKEN_REF;
            } else {
                this._type = G4PlusLexer.RULE_REF;
            }

            // If outside of rule def.
            if (this.currentRuleType === Token.INVALID_TYPE) {
                // Set to inside lexer or parser rule.
                this.currentRuleType = this._type;
            }
        } else if (this._type === G4PlusLexer.SEMI) {
            // Exit rule def.
            this.currentRuleType = Token.INVALID_TYPE;
        }

        return super.emit();
    }

    protected handleBeginArgument(): void {
        if (this.currentRuleType === G4PlusLexer.TOKEN_REF) {
            this.pushMode(G4PlusLexer.LexerCharSet);
            this.more();
        } else {
            this.pushMode(G4PlusLexer.Argument);
        }
    }

    protected handleEndArgument(): void {
        this.popMode();
        if (this.getModeStack().length > 0) {
            this._type = G4PlusLexer.ARGUMENT_CONTENT;
        }
    }
}

