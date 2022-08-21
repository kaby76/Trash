namespace AltAntlr
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    public class MyLexer : Lexer, ITokenSource
    {
        public IList<IToken> Tokens { get; set; }
        public int CurrentToken { get; set; }

        public override int Line => throw new NotImplementedException();

        public override int Column => throw new NotImplementedException();

        public ICharStream _inputstream;
        public new ICharStream InputStream { get { return _inputstream; } }

        public override string SourceName => throw new NotImplementedException();

        public override ITokenFactory TokenFactory
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string[] _ruleNames;
        public override string[] RuleNames
        {
            get { return _ruleNames; }
        }

        public Vocabulary _vocabulary;
        public override IVocabulary Vocabulary
        {
            get { return _vocabulary; }
        }

        public string _grammarFileName;
        public override string GrammarFileName
        {
            get { return _grammarFileName; }
        }

        public string[] _modeNames;
        public override string[] ModeNames
        {
            get { return _modeNames; }
        }

        public string[] _channelNames;
        public override string[] ChannelNames
        {
            get { return _channelNames; }
        }

        public IDictionary<string, int> _tokenTypeMap;
        public override IDictionary<string, int> TokenTypeMap
        {
            get { return _tokenTypeMap; }
        }

        [return: NotNull]
        public override IToken NextToken()
        {
            if (CurrentToken < Tokens.Count)
                return Tokens[CurrentToken++];
            throw new Exception("Reading past EOF.");
        }

        public MyLexer(ICharStream input) : base(input)
        {
        }

        public MyLexer(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        MyTokenStream _token_stream;
        public MyTokenStream TokenStream { get { return _token_stream; } set { _token_stream = value; } }
    }
}
