/*
 [The "BSD licence"]
 Copyright (c) 2005-2007 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
    derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.Token;

public abstract class LexerAdaptor extends Lexer {
    private boolean lexerGrammar;
    private boolean sawLexerKeyword;
    private boolean headerComplete;
    private boolean inRuleBody;
    private int braceDepth;
    private int previousTokenType = Token.INVALID_TYPE;

    public LexerAdaptor(CharStream input) {
        super(input);
    }

    protected void handleBeginArgument() {
        boolean followsReference = previousTokenType == G4PlusLexer.ID;
        if (inRuleBody && (lexerGrammar || !followsReference)) {
            pushMode(G4PlusLexer.LexerCharSet);
            more();
        } else {
            pushMode(G4PlusLexer.Argument);
        }
    }

    protected void handleEndArgument() {
        popMode();
        if (!_modeStack.isEmpty()) {
            setType(G4PlusLexer.ARGUMENT_CONTENT);
        }
    }

    @Override
    public Token emit() {
        int type = _type;
        if (!headerComplete && type == G4PlusLexer.LEXER) {
            sawLexerKeyword = true;
        } else if (!headerComplete && type == G4PlusLexer.GRAMMAR) {
            lexerGrammar = sawLexerKeyword;
        } else if (type == G4PlusLexer.OPTIONS || type == G4PlusLexer.TOKENS || type == G4PlusLexer.CHANNELS) {
            braceDepth++;
        } else if (type == G4PlusLexer.RBRACE && braceDepth > 0) {
            braceDepth--;
        } else if (type == G4PlusLexer.COLON && headerComplete && braceDepth == 0) {
            inRuleBody = true;
        } else if (type == G4PlusLexer.SEMI && braceDepth == 0) {
            headerComplete = true;
            inRuleBody = false;
        }
        if (_channel == Token.DEFAULT_CHANNEL) {
            previousTokenType = type;
        }
        return super.emit();
    }

    @Override
    public void reset() {
        lexerGrammar = false;
        sawLexerKeyword = false;
        headerComplete = false;
        inRuleBody = false;
        braceDepth = 0;
        previousTokenType = Token.INVALID_TYPE;
        super.reset();
    }
}
