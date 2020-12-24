namespace AntlrJson
{
    using System.IO;
    using Antlr4.Runtime;

    public class MyParser : Parser
    {
        public MyParser(ITokenStream input) : base(input)
        {
        }

        public MyParser(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput)
        {
        }

        public string[] _ruleNames;
        public override string[] RuleNames
        {
            get { return _ruleNames; }
        }

        public string _grammarFileName;
        public override string GrammarFileName
        {
            get { return _grammarFileName; }
        }

        public Vocabulary _vocabulary;
        public override IVocabulary Vocabulary
        {
            get { return _vocabulary; }
        }
    }
}
