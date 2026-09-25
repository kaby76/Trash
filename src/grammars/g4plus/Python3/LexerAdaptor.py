# --------------------------------------------------------------------------------
# [The "BSD licence"]
# Copyright (c) 2005-2007 Terence Parr
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions
# are met:
# 1. Redistributions of source code must retain the above copyright
#    notice, this list of conditions and the following disclaimer.
# 2. Redistributions in binary form must reproduce the above copyright
#    notice, this list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
# 3. The name of the author may not be used to endorse or promote products
#    derived from this software without specific prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
# IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
# OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
# IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
# INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
# NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
# DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
# THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
# THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
# --------------------------------------------------------------------------------

from antlr4 import Lexer, Token
from G4PlusParser import G4PlusParser
import sys


class LexerAdaptor(Lexer):
    def __init__(self, input, output=sys.stdout):
        super().__init__(input, output)
        self.lexerGrammar = False
        self.sawLexerKeyword = False
        self.headerComplete = False
        self.inRuleBody = False
        self.braceDepth = 0
        self.previousTokenType = Token.INVALID_TYPE

    def handleBeginArgument(self):
        followsReference = self.previousTokenType == G4PlusParser.ID
        if self.inRuleBody and (self.lexerGrammar or not followsReference):
            # LexerCharSet is mode 2. Importing G4PlusLexer creates a circular import.
            self.pushMode(2)
            self.more()
        else:
            self.pushMode(1)

    def handleEndArgument(self):
        self.popMode()
        if self._modeStack:
            self.type = G4PlusParser.ARGUMENT_CONTENT

    def emit(self):
        tokenType = self._type
        if not self.headerComplete and tokenType == G4PlusParser.LEXER:
            self.sawLexerKeyword = True
        elif not self.headerComplete and tokenType == G4PlusParser.GRAMMAR:
            self.lexerGrammar = self.sawLexerKeyword
        elif tokenType in (G4PlusParser.OPTIONS, G4PlusParser.TOKENS, G4PlusParser.CHANNELS):
            self.braceDepth += 1
        elif tokenType == G4PlusParser.RBRACE and self.braceDepth > 0:
            self.braceDepth -= 1
        elif tokenType == G4PlusParser.COLON and self.headerComplete and self.braceDepth == 0:
            self.inRuleBody = True
        elif tokenType == G4PlusParser.SEMI and self.braceDepth == 0:
            self.headerComplete = True
            self.inRuleBody = False
        if self._channel == Token.DEFAULT_CHANNEL:
            self.previousTokenType = tokenType
        return super().emit()

    def reset(self):
        self.lexerGrammar = False
        self.sawLexerKeyword = False
        self.headerComplete = False
        self.inRuleBody = False
        self.braceDepth = 0
        self.previousTokenType = Token.INVALID_TYPE
        super().reset()
