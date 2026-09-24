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
//package org.antlr.parser.antlr4;

#include "antlr4-runtime.h"
#include "LexerAdaptor.h"
#include "G4PlusLexer.h"
#include <cctype>

LexerAdaptor::LexerAdaptor(antlr4::CharStream* input) : antlr4::Lexer(input)
{
}

int LexerAdaptor::getCurrentRuleType() {
    return this->_currentRuleType;
}

void LexerAdaptor::setCurrentRuleType(int ruleType) {
    this->_currentRuleType = ruleType;
}

void LexerAdaptor::handleBeginArgument() {
    if (this->inLexerRule()) {
        this->pushMode(G4PlusLexer::LexerCharSet);
        this->more();
    } else {
        this->pushMode(G4PlusLexer::Argument);
    }
}

void LexerAdaptor::handleEndArgument() {
    this->popMode();
    if (this->modeStack.size() > 0) {
        this->setType(G4PlusLexer::ARGUMENT_CONTENT);
    }
}

antlr4::Token* LexerAdaptor::emit() {
    if ((this->type == G4PlusLexer::OPTIONS || this->type == G4PlusLexer::TOKENS || this->type == G4PlusLexer::CHANNELS)
          && getCurrentRuleType() == antlr4::Token::INVALID_TYPE) { // enter prequel construct ending with an RBRACE
        setCurrentRuleType(PREQUEL_CONSTRUCT);
    } else if (this->type == G4PlusLexer::OPTIONS && getCurrentRuleType() == G4PlusLexer::TOKEN_REF)
    {
        setCurrentRuleType(OPTIONS_CONSTRUCT);
    } else if (this->type == G4PlusLexer::RBRACE && getCurrentRuleType() == PREQUEL_CONSTRUCT) { // exit prequel construct
        setCurrentRuleType(antlr4::Token::INVALID_TYPE);
    } else if (this->type == G4PlusLexer::RBRACE && getCurrentRuleType() == OPTIONS_CONSTRUCT)
    { // exit options
        setCurrentRuleType(G4PlusLexer::TOKEN_REF);
    } else if (this->type == G4PlusLexer::AT && getCurrentRuleType() == antlr4::Token::INVALID_TYPE) { // enter action
        setCurrentRuleType(G4PlusLexer::AT);
    } else if (this->type == G4PlusLexer::SEMI && getCurrentRuleType() == OPTIONS_CONSTRUCT)
    { // ';' in options { .... }. Don't change anything.
    } else if (this->type == G4PlusLexer::ACTION && getCurrentRuleType() == G4PlusLexer::AT) { // exit action
        // Exit action.
            setCurrentRuleType(antlr4::Token::INVALID_TYPE);
    } else if (this->type == G4PlusLexer::ID) {
        auto firstChar = _input->getText(antlr4::misc::Interval(this->tokenStartCharIndex, this->tokenStartCharIndex));
        if (std::isupper(firstChar[0])) {
            this->type = G4PlusLexer::TOKEN_REF;
        } else {
            this->type = G4PlusLexer::RULE_REF;
        }

        if (getCurrentRuleType() == antlr4::Token::INVALID_TYPE) { // if outside of rule def
            setCurrentRuleType(this->type); // set to inside lexer or parser rule
        }
    } else if (this->type == G4PlusLexer::SEMI) { // exit rule def
        setCurrentRuleType(antlr4::Token::INVALID_TYPE);
    }

    return Lexer::emit();
}

bool LexerAdaptor::inLexerRule() {
    return getCurrentRuleType() == G4PlusLexer::TOKEN_REF;
}

bool LexerAdaptor::inParserRule() { // not used, but added for clarity
    return getCurrentRuleType() == G4PlusLexer::RULE_REF;
}

void LexerAdaptor::reset() {
    setCurrentRuleType(antlr4::Token::INVALID_TYPE);
    Lexer::reset();
}   
