/*
 * Copyright (c) Mike Lischke. All rights reserved.
 * Licensed under the BSD 3-clause License. See License.txt in the project root for license information.
 */

import { CharStream, Lexer, Token } from "antlr4ng";

import { G4XLexer } from "./G4XLexer.js";

export abstract class LexerAdaptor extends Lexer {
    private lexerGrammar = false;
    private sawLexerKeyword = false;
    private headerComplete = false;
    private inRuleBody = false;
    private braceDepth = 0;
    private previousTokenType = Token.INVALID_TYPE;

    public constructor(input: CharStream) {
        super(input);
    }

    protected handleBeginArgument(): void {
        const followsReference = this.previousTokenType === G4XLexer.ID;
        if (this.inRuleBody && (this.lexerGrammar || !followsReference)) {
            this.pushMode(G4XLexer.LexerCharSet);
            this.more();
        } else {
            this.pushMode(G4XLexer.Argument);
        }
    }

    protected handleEndArgument(): void {
        this.popMode();
        if (this.modeStack.length > 0) {
            this.type = G4XLexer.ARGUMENT_CONTENT;
        }
    }

    public override emit(): Token {
        const type = this.type;
        if (!this.headerComplete && type === G4XLexer.LEXER) {
            this.sawLexerKeyword = true;
        } else if (!this.headerComplete && type === G4XLexer.GRAMMAR) {
            this.lexerGrammar = this.sawLexerKeyword;
        } else if (type === G4XLexer.OPTIONS || type === G4XLexer.TOKENS || type === G4XLexer.CHANNELS) {
            this.braceDepth++;
        } else if (type === G4XLexer.RBRACE && this.braceDepth > 0) {
            this.braceDepth--;
        } else if (type === G4XLexer.COLON && this.headerComplete && this.braceDepth === 0) {
            this.inRuleBody = true;
        } else if (type === G4XLexer.SEMI && this.braceDepth === 0) {
            this.headerComplete = true;
            this.inRuleBody = false;
        }
        if (this.channel === Token.DEFAULT_CHANNEL) {
            this.previousTokenType = type;
        }
        return super.emit();
    }

    public override reset(): void {
        this.lexerGrammar = false;
        this.sawLexerKeyword = false;
        this.headerComplete = false;
        this.inRuleBody = false;
        this.braceDepth = 0;
        this.previousTokenType = Token.INVALID_TYPE;
        super.reset();
    }
}
