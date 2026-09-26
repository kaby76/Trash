/*
 * Copyright (c) Mike Lischke. All rights reserved.
 * Licensed under the BSD 3-clause License. See License.txt in the project root for license information.
 */

import { CharStream, Lexer, Token } from "antlr4ng";

import { G4PlusLexer } from "./G4PlusLexer.js";

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
        const followsReference = this.previousTokenType === G4PlusLexer.ID;
        if (this.inRuleBody && (this.lexerGrammar || !followsReference)) {
            this.pushMode(G4PlusLexer.LexerCharSet);
            this.more();
        } else {
            this.pushMode(G4PlusLexer.Argument);
        }
    }

    protected handleEndArgument(): void {
        this.popMode();
        if (this.modeStack.length > 0) {
            this.type = G4PlusLexer.ARGUMENT_CONTENT;
        }
    }

    public override emit(): Token {
        const type = this.type;
        if (!this.headerComplete && type === G4PlusLexer.LEXER) {
            this.sawLexerKeyword = true;
        } else if (!this.headerComplete && type === G4PlusLexer.GRAMMAR) {
            this.lexerGrammar = this.sawLexerKeyword;
        } else if (type === G4PlusLexer.OPTIONS || type === G4PlusLexer.TOKENS || type === G4PlusLexer.CHANNELS) {
            this.braceDepth++;
        } else if (type === G4PlusLexer.RBRACE && this.braceDepth > 0) {
            this.braceDepth--;
        } else if (type === G4PlusLexer.COLON && this.headerComplete && this.braceDepth === 0) {
            this.inRuleBody = true;
        } else if (type === G4PlusLexer.SEMI && this.braceDepth === 0) {
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
