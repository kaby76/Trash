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

using System;
using System.IO;
using Antlr4.Runtime;

public abstract class LexerAdaptor : Lexer
{
    private bool _lexerGrammar;
    private bool _sawLexerKeyword;
    private bool _headerComplete;
    private bool _inRuleBody;
    private int _braceDepth;
    private int _previousTokenType = TokenConstants.InvalidType;

    protected LexerAdaptor(ICharStream input)
         : base(input, Console.Out, Console.Error)
    {
    }

    protected LexerAdaptor(ICharStream input, TextWriter output, TextWriter errorOutput)
         : base(input, output, errorOutput)
    {
    }

    protected void handleBeginArgument()
    {
        var followsReference = _previousTokenType == G4PlusLexer.ID;
        if (_inRuleBody && (_lexerGrammar || !followsReference))
        {
            PushMode(G4PlusLexer.LexerCharSet);
            More();
        }
        else
        {
            PushMode(G4PlusLexer.Argument);
        }
    }

    protected void handleEndArgument()
    {
        PopMode();
        if (ModeStack.Count > 0)
        {
            Type = G4PlusLexer.ARGUMENT_CONTENT;
        }
    }

    public override IToken Emit()
    {
        if (!_headerComplete && Type == G4PlusLexer.LEXER)
            _sawLexerKeyword = true;
        else if (!_headerComplete && Type == G4PlusLexer.GRAMMAR)
            _lexerGrammar = _sawLexerKeyword;
        else if (Type == G4PlusLexer.OPTIONS || Type == G4PlusLexer.TOKENS || Type == G4PlusLexer.CHANNELS)
            _braceDepth++;
        else if (Type == G4PlusLexer.RBRACE && _braceDepth > 0)
            _braceDepth--;
        else if (Type == G4PlusLexer.COLON && _headerComplete && _braceDepth == 0)
            _inRuleBody = true;
        else if (Type == G4PlusLexer.SEMI && _braceDepth == 0)
        {
            _headerComplete = true;
            _inRuleBody = false;
        }

        if (Channel == TokenConstants.DefaultChannel)
            _previousTokenType = Type;
        return base.Emit();
    }

    public override void Reset()
    {
        _lexerGrammar = false;
        _sawLexerKeyword = false;
        _headerComplete = false;
        _inRuleBody = false;
        _braceDepth = 0;
        _previousTokenType = TokenConstants.InvalidType;
        base.Reset();
    }
}
