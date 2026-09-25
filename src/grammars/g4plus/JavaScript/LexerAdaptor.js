import antlr4 from 'antlr4';
import G4PlusLexer from './G4PlusLexer.js';

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
        const followsReference = this.previousTokenType === G4PlusLexer.ID;
        if (this.inRuleBody && (this.lexerGrammar || !followsReference)) {
            this.pushMode(G4PlusLexer.LexerCharSet);
            this.more();
        } else {
            this.pushMode(G4PlusLexer.Argument);
        }
    }

    handleEndArgument() {
        this.popMode();
        if (this._modeStack.length > 0) {
            this._type = G4PlusLexer.ARGUMENT_CONTENT;
        }
    }

    emit() {
        const type = this._type;
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
