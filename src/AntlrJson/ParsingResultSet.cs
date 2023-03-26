namespace AntlrJson
{
    using Antlr4.Runtime;
    using ParseTreeEditing.UnvParseTreeDOM;

    public class ParsingResultSet
    {
        public string FileName { get; set; }
        public string Text { get; set; }
        public string StartSymbol { get; set; }
        public string MetaStartSymbol { get; set; }
        public UnvParseTreeNode[] Nodes { get; set; }
        public Lexer Lexer { get; set; }
        public Parser Parser { get; set; }
    }
}
