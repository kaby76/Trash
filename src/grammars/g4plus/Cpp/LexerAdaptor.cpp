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

#include "LexerAdaptor.h"
#include "G4PlusLexer.h"

LexerAdaptor::LexerAdaptor(antlr4::CharStream* input) : antlr4::Lexer(input) {}

void LexerAdaptor::handleBeginArgument() {
    bool followsReference = previousTokenType == G4PlusLexer::ID;
    if (inRuleBody && (lexerGrammar || !followsReference)) {
        pushMode(G4PlusLexer::LexerCharSet);
        more();
    } else {
        pushMode(G4PlusLexer::Argument);
    }
}

void LexerAdaptor::handleEndArgument() {
    popMode();
    if (!modeStack.empty()) {
        setType(G4PlusLexer::ARGUMENT_CONTENT);
    }
}

antlr4::Token* LexerAdaptor::emit() {
    int tokenType = type;
    if (!headerComplete && tokenType == G4PlusLexer::LEXER) {
        sawLexerKeyword = true;
    } else if (!headerComplete && tokenType == G4PlusLexer::GRAMMAR) {
        lexerGrammar = sawLexerKeyword;
    } else if (tokenType == G4PlusLexer::OPTIONS || tokenType == G4PlusLexer::TOKENS || tokenType == G4PlusLexer::CHANNELS) {
        braceDepth++;
    } else if (tokenType == G4PlusLexer::RBRACE && braceDepth > 0) {
        braceDepth--;
    } else if (tokenType == G4PlusLexer::COLON && headerComplete && braceDepth == 0) {
        inRuleBody = true;
    } else if (tokenType == G4PlusLexer::SEMI && braceDepth == 0) {
        headerComplete = true;
        inRuleBody = false;
    }
    if (tokenType != G4PlusLexer::WS && tokenType != G4PlusLexer::DOC_COMMENT
        && tokenType != G4PlusLexer::BLOCK_COMMENT && tokenType != G4PlusLexer::LINE_COMMENT) {
        previousTokenType = tokenType;
    }
    return Lexer::emit();
}

void LexerAdaptor::reset() {
    lexerGrammar = false;
    sawLexerKeyword = false;
    headerComplete = false;
    inRuleBody = false;
    braceDepth = 0;
    previousTokenType = antlr4::Token::INVALID_TYPE;
    Lexer::reset();
}
