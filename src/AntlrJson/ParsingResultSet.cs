using Antlr4.Runtime;
using ParseTreeEditing.UnvParseTreeDOM;

namespace AntlrJson;

public class ParsingResultSet
{
    public string FileName { get; set; }
    public string StartSymbol { get; set; }
    public string MetaStartSymbol { get; set; }
    public UnvParseTreeNode[] Nodes { get; set; }
    public Lexer Lexer { get; set; }
    public Parser Parser { get; set; }
}
