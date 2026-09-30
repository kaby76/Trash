import antlr4 from 'antlr4';
import G4XLexer from './G4XLexer.js';

export default class LexerAdaptor extends antlr4.Lexer {
    constructor(input) {
        super(input);
        this.lexerGrammar = false;
        this.sawLexerKeyword = false;
        this.headerComplete = false;
        this.inRuleBody = false;
        this.braceDepth = 0;
        this.previousTokenType = antlr4.Token.INVALID_TYPE;
    }

    handleBeginArgument() {
        const followsReference = this.previousTokenType === G4XLexer.ID;
        if (this.inRuleBody && (this.lexerGrammar || !followsReference)) {
            this.pushMode(G4XLexer.LexerCharSet);
            this.more();
        } else {
            this.pushMode(G4XLexer.Argument);
        }
    }

    handleEndArgument() {
        this.popMode();
        if (this._modeStack.length > 0) {
            this._type = G4XLexer.ARGUMENT_CONTENT;
        }
    }

    emit() {
        const type = this._type;
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
        if (this._channel === antlr4.Token.DEFAULT_CHANNEL) {
            this.previousTokenType = type;
        }
        return super.emit();
    }

    reset() {
        this.lexerGrammar = false;
        this.sawLexerKeyword = false;
        this.headerComplete = false;
        this.inRuleBody = false;
        this.braceDepth = 0;
        this.previousTokenType = antlr4.Token.INVALID_TYPE;
        super.reset();
    }
}
