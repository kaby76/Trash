namespace AntlrJson
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;

    public class ParsingResultSet
    {
        public string FileName { get; set; }
        public string Text { get; set; }
        public string StartSymbol { get; set; }
        public string MetaStartSymbol { get; set; }
        public ITokenStream Stream { get; set; }
        public IParseTree[] Nodes { get; set; }
        public Lexer Lexer { get; set; }
        public Parser Parser { get; set; }
    }
}
