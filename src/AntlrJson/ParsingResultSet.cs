using Antlr4.Runtime;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Text.Json;

namespace AntlrJson;

public class ParsingResultSet
{
    private UnvParseTreeNode[] _nodes;

    public string FileName { get; set; }
    public string StartSymbol { get; set; }
    public string MetaStartSymbol { get; set; }
    public UnvParseTreeNode[] Nodes
    {
        get => _nodes ??= NodeProvider?.Materialize() ?? [];
        set => _nodes = value;
    }
    public IParsingResultNodeProvider NodeProvider { get; set; }
    public bool HasMaterializedNodes => _nodes != null;
    public Lexer Lexer { get; set; }
    public Parser Parser { get; set; }
}

public interface IParsingResultNodeProvider
{
    int Count { get; }
    void WriteNodes(Utf8JsonWriter writer);
    UnvParseTreeNode[] Materialize();
}
