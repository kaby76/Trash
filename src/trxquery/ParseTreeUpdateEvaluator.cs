using System.Linq;
using ParseTreeEditing.UnvParseTreeDOM;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.Parser.Ast;
using TreeEdits = ParseTreeEditing.UnvParseTreeDOM.TreeEdits;

namespace Trash;

/// <summary>
/// Subclass of XQueryEvaluator that overrides the XQuery Update Facility visitor
/// methods to apply updates directly to the underlying UnvParseTreeNode tree
/// via TreeEdits, bypassing PendingUpdateList entirely.
/// </summary>
class ParseTreeUpdateEvaluator : XQueryEvaluator
{
    public ParseTreeUpdateEvaluator(EvaluationContext? context = null) : base(context)
    {
        // Don't auto-apply PendingUpdateList — we bypass it entirely for adapter nodes.
        ApplyUpdatesOnComplete = false;
    }

    public override XdmSequence VisitDeleteExpr(DeleteExpr node)
    {
        var target = base.Evaluate(node.Target);

        foreach (var item in target)
        {
            var src = ExtractSource(item);
            if (src != null)
            {
                TreeEdits.Delete(src);
            }
            else
            {
                // Fall back to standard path for non-adapter nodes.
                if (item is XdmNode xdmNode)
                    PendingUpdates.AddDelete(xdmNode);
            }
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitInsertExpr(InsertExpr node)
    {
        var sourceSeq = base.Evaluate(node.Source);
        var targetSeq = base.Evaluate(node.Target);

        var targetNode = targetSeq.SingleOrDefault() as XdmNode
            ?? throw new XdmException("XUTY0005", "Insert target must be a single node");

        var targetSrc = ExtractSource(targetNode);
        if (targetSrc == null)
        {
            // Fall back to standard path.
            foreach (var item in sourceSeq)
            {
                if (item is XdmNode src)
                    PendingUpdates.AddInsert(src.DeepCopy(), targetNode, node.Position);
            }
            return XdmSequence.Empty;
        }

        foreach (var item in sourceSeq)
        {
            switch (node.Position)
            {
                case InsertPosition.Before:
                    InsertNodeBefore(targetSrc, item);
                    break;
                case InsertPosition.After:
                    InsertNodeAfter(targetSrc, item);
                    break;
                case InsertPosition.Into:
                case InsertPosition.AsLast:
                    InsertNodeAsLastChild(targetSrc, item);
                    break;
                case InsertPosition.AsFirst:
                    InsertNodeAsFirstChild(targetSrc, item);
                    break;
            }
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitReplaceExpr(ReplaceExpr node)
    {
        var targetSeq = base.Evaluate(node.Target);
        var replacementSeq = base.Evaluate(node.Replacement);

        var targetNode = targetSeq.SingleOrDefault() as XdmNode
            ?? throw new XdmException("XUTY0008", "Replace target must be a single node");

        var targetSrc = ExtractSource(targetNode);
        if (targetSrc == null)
        {
            // Fall back.
            if (node.ValueOf)
                PendingUpdates.AddReplaceValue(targetNode, replacementSeq.StringValue);
            else
            {
                var nodes = replacementSeq
                    .OfType<XdmNode>()
                    .Select(n => n.DeepCopy())
                    .ToList();
                PendingUpdates.AddReplaceNode(targetNode, nodes);
            }
            return XdmSequence.Empty;
        }

        if (node.ValueOf)
        {
            // Replace the text content of the target node.
            TreeEdits.Replace(targetSrc, replacementSeq.StringValue);
        }
        else
        {
            // Replace the node itself with the replacement sequence.
            var first = true;
            foreach (var item in replacementSeq)
            {
                var text = NodeToString(item);
                if (first)
                {
                    TreeEdits.Replace(targetSrc, text);
                    first = false;
                }
                else
                {
                    // Additional replacement nodes are inserted after the replaced text node.
                    // We inserted a text node in place of targetSrc, but we don't have easy
                    // access to the replacement node returned. Use InsertAfter on the parent
                    // by iterating — simplest fallback: log a warning and skip.
                    System.Console.Error.WriteLine(
                        "trxquery: warning: replace with multiple nodes — only first node applied.");
                    break;
                }
            }
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitRenameExpr(RenameExpr node)
    {
        var targetSeq = base.Evaluate(node.Target);
        var newNameSeq = base.Evaluate(node.NewName);

        var targetNode = targetSeq.SingleOrDefault() as XdmNode
            ?? throw new XdmException("XUTY0012", "Rename target must be a single node");

        if (targetNode is AdapterElement ae)
        {
            ae.Source.LocalName = newNameSeq.StringValue;
        }
        else if (targetNode is AdapterAttribute aa)
        {
            aa.Source.LocalName = newNameSeq.StringValue;
        }
        else if (targetNode is XdmNode xdmNode)
        {
            // Fall back to standard path.
            var newQName = new XdmQName(newNameSeq.StringValue);
            PendingUpdates.AddRename(xdmNode, newQName);
        }

        return XdmSequence.Empty;
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static UnvParseTreeNode? ExtractSource(XdmItem item) => item switch
    {
        AdapterElement ae  => ae.Source,
        AdapterText    at  => at.Source,
        AdapterAttribute aa => aa.Source,
        _                  => null
    };

    private static string NodeToString(XdmItem item)
    {
        var src = ExtractSource(item);
        if (src is UnvParseTreeElement elem)
            return SerializeElement(elem);
        return item.StringValue;
    }

    private static string SerializeElement(UnvParseTreeElement elem)
    {
        // Minimal serialisation to a string so TreeEdits.Replace can accept it.
        var sb = new System.Text.StringBuilder();
        sb.Append(elem.LocalName ?? "node");
        foreach (var child in elem.AllChildren)
        {
            if (child is UnvParseTreeText t) sb.Append(t.Data ?? "");
        }
        return sb.ToString();
    }

    private static void InsertNodeBefore(UnvParseTreeNode target, XdmItem item)
    {
        var src = ExtractSource(item);
        if (src != null)
            TreeEdits.InsertBefore(target, src);
        else
            TreeEdits.InsertBefore(target, item.StringValue);
    }

    private static void InsertNodeAfter(UnvParseTreeNode target, XdmItem item)
    {
        var src = ExtractSource(item);
        if (src != null)
            TreeEdits.InsertAfter(target, src);
        else
            TreeEdits.InsertAfter(target, item.StringValue);
    }

    private static void InsertNodeAsLastChild(UnvParseTreeNode target, XdmItem item)
    {
        // TreeEdits has no AppendChild; use InsertAfter on the last child, or
        // InsertBefore a sentinel. Simplest: InsertAfter the target itself if it
        // has no children, else after the last child.
        var lastChild = target.ChildNodes.Length > 0
            ? target.ChildNodes.item(target.ChildNodes.Length - 1) as UnvParseTreeNode
            : null;

        if (lastChild != null)
            InsertNodeAfter(lastChild, item);
        else
            InsertNodeAfter(target, item);
    }

    private static void InsertNodeAsFirstChild(UnvParseTreeNode target, XdmItem item)
    {
        var firstChild = target.ChildNodes.Length > 0
            ? target.ChildNodes.item(0) as UnvParseTreeNode
            : null;

        if (firstChild != null)
            InsertNodeBefore(firstChild, item);
        else
            InsertNodeAfter(target, item);
    }
}
