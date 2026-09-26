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

import 'package:antlr4/antlr4.dart';
import 'G4PlusLexer.dart';

abstract class LexerAdaptor extends Lexer {
    bool lexerGrammar = false;
    bool sawLexerKeyword = false;
    bool headerComplete = false;
    bool inRuleBody = false;
    int braceDepth = 0;
    int previousTokenType = Token.INVALID_TYPE;

    LexerAdaptor(CharStream input) : super(input);

    void handleBeginArgument() {
        bool followsReference = previousTokenType == G4PlusLexer.TOKEN_ID;
        if (inRuleBody && (lexerGrammar || !followsReference)) {
            pushMode(G4PlusLexer.LexerCharSet);
            more();
        } else {
            pushMode(G4PlusLexer.Argument);
        }
    }

    void handleEndArgument() {
        popMode();
        // Keep nested closing brackets as argument content.
        bool nested = true;
        var currentMode = mode_;
        try {
            var previousMode = popMode();
            pushMode(previousMode);
        } catch (_) {
            nested = false;
        }
        mode_ = currentMode;
        if (nested) {
            type = G4PlusLexer.TOKEN_ARGUMENT_CONTENT;
        }
    }

    @override Token emit() {
        int tokenType = type;
        if (!headerComplete && tokenType == G4PlusLexer.TOKEN_LEXER) {
            sawLexerKeyword = true;
        } else if (!headerComplete && tokenType == G4PlusLexer.TOKEN_GRAMMAR) {
            lexerGrammar = sawLexerKeyword;
        } else if (tokenType == G4PlusLexer.TOKEN_OPTIONS || tokenType == G4PlusLexer.TOKEN_TOKENS || tokenType == G4PlusLexer.TOKEN_CHANNELS) {
            braceDepth++;
        } else if (tokenType == G4PlusLexer.TOKEN_RBRACE && braceDepth > 0) {
            braceDepth--;
        } else if (tokenType == G4PlusLexer.TOKEN_COLON && headerComplete && braceDepth == 0) {
            inRuleBody = true;
        } else if (tokenType == G4PlusLexer.TOKEN_SEMI && braceDepth == 0) {
            headerComplete = true;
            inRuleBody = false;
        }
        if (channel == Token.DEFAULT_CHANNEL) {
            previousTokenType = tokenType;
        }
        return super.emit();
    }

    @override void reset([bool resetInput = false]) {
        lexerGrammar = false;
        sawLexerKeyword = false;
        headerComplete = false;
        inRuleBody = false;
        braceDepth = 0;
        previousTokenType = Token.INVALID_TYPE;
        super.reset(resetInput);
    }
}
