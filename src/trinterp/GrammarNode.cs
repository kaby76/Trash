using System.Collections.Generic;
using System.Linq;
using ParseTreeEditing.UnvParseTreeDOM;

namespace trinterp;

/// <summary>
/// Owned compiler syntax, independent of the input DOM and generated parser.
/// Front ends lower into the common block/alternative/element vocabulary used
/// by the ATN builders. Terminal text and source positions survive lowering.
/// </summary>
public sealed class GrammarNode
{
    public string LocalName { get; set; }
    public string Text { get; set; }
    public bool Terminal { get; set; }
    public int Line { get; set; } = -1;
    public int Column { get; set; } = -1;
    public List<GrammarNode> Children { get; } = new();

    public GrammarNode(string kind, params GrammarNode[] children)
    {
        LocalName = kind;
        Children.AddRange(children.Where(c => c != null));
    }

    public bool IsTerminal() => Terminal;
    public int GetLine() => Line;
    public int GetColumn() => Column;
    public string GetText() => Text ?? string.Concat(Children.Select(c => c.GetText()));
    public IEnumerable<GrammarNode> GetChildren(string kind) => Children.Where(c => c.LocalName == kind);
    public IEnumerable<GrammarNode> DescendantsAndSelf()
    {
        yield return this;
        foreach (var child in Children)
            foreach (var node in child.DescendantsAndSelf()) yield return node;
    }

    public static GrammarNode FromDom(UnvParseTreeElement source)
    {
        var node = new GrammarNode(source.LocalName) { Terminal = source.IsTerminal() };
        if (node.Terminal)
        {
            node.Text = source.GetText();
            try { node.Line = source.GetLine(); node.Column = source.GetColumn(); } catch { }
        }
        foreach (var child in source.Children) node.Children.Add(FromDom(child));
        return node;
    }
}
