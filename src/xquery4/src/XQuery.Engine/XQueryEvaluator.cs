using XQuery.DataModel;
using XQuery.Parser.Ast;
using XQuery.Functions;

namespace XQuery.Engine;

/// <summary>
/// Evaluates XQuery 4.0 expressions with update facility.
/// </summary>
public class XQueryEvaluator : XPathEvaluator
{
    private readonly PendingUpdateList _pendingUpdates = new();
    private bool _applyUpdatesOnComplete = true;

    public XQueryEvaluator(EvaluationContext? context = null) : base(context)
    {
    }

    /// <summary>
    /// Gets the pending update list.
    /// </summary>
    public PendingUpdateList PendingUpdates => _pendingUpdates;

    /// <summary>
    /// Gets or sets whether to automatically apply updates after evaluation.
    /// </summary>
    public bool ApplyUpdatesOnComplete
    {
        get => _applyUpdatesOnComplete;
        set => _applyUpdatesOnComplete = value;
    }

    /// <summary>
    /// Evaluates an XQuery expression and optionally applies pending updates.
    /// </summary>
    public new XdmSequence Evaluate(ExprNode expr)
    {
        var result = base.Evaluate(expr);

        if (_applyUpdatesOnComplete && _pendingUpdates.HasUpdates)
        {
            _pendingUpdates.Apply();
        }

        return result;
    }

    /// <summary>
    /// Evaluates an XQuery module.
    /// </summary>
    public XdmSequence EvaluateModule(ModuleNode module)
    {
        // Process prolog
        ProcessProlog(module.Prolog);

        // Evaluate body if present
        if (module.Body != null)
        {
            return Evaluate(module.Body);
        }

        return XdmSequence.Empty;
    }

    private void ProcessProlog(PrologNode prolog)
    {
        // Register namespace declarations
        foreach (var nsDecl in prolog.NamespaceDecls)
        {
            _context.RegisterNamespace(nsDecl.Prefix, nsDecl.Uri);
        }

        // Set default element namespace
        if (prolog.DefaultElementNamespace != null)
        {
            _context.SetDefaultElementNamespace(prolog.DefaultElementNamespace);
        }

        // Set default function namespace
        if (prolog.DefaultFunctionNamespace != null)
        {
            _context.SetDefaultFunctionNamespace(prolog.DefaultFunctionNamespace);
        }

        // Process variable declarations
        foreach (var varDecl in prolog.VariableDecls)
        {
            if (varDecl.Value != null)
            {
                var value = base.Evaluate(varDecl.Value);
                _context.SetVariable(varDecl.Name, value);
            }
        }

        // Process function declarations
        foreach (var funcDecl in prolog.FunctionDecls)
        {
            if (funcDecl.Body != null)
            {
                var paramNames = funcDecl.Parameters.Select(p => p.Name).ToArray();
                var capturedContext = _context.Clone();

                var func = new XdmFunction(
                    new XdmQName(funcDecl.Name),
                    funcDecl.Parameters.Count,
                    args =>
                    {
                        var evalContext = capturedContext.Clone();
                        for (int i = 0; i < paramNames.Length; i++)
                        {
                            evalContext.SetVariable(paramNames[i], args[i]);
                        }

                        var evaluator = new XQueryEvaluator(evalContext);
                        return evaluator.Evaluate(funcDecl.Body);
                    });

                _context.RegisterFunction(new XdmQName(funcDecl.Name), func);
            }
        }
    }

    #region Module Visitor Methods

    public override XdmSequence VisitModule(ModuleNode node)
    {
        return EvaluateModule(node);
    }

    public override XdmSequence VisitProlog(PrologNode node)
    {
        ProcessProlog(node);
        return XdmSequence.Empty;
    }

    public override XdmSequence VisitVariableDecl(VariableDeclNode node)
    {
        if (node.Value != null)
        {
            var value = base.Evaluate(node.Value);
            _context.SetVariable(node.Name, value);
        }
        return XdmSequence.Empty;
    }

    public override XdmSequence VisitFunctionDecl(FunctionDeclNode node)
    {
        if (node.Body != null)
        {
            var paramNames = node.Parameters.Select(p => p.Name).ToArray();
            var capturedContext = _context.Clone();

            var func = new XdmFunction(
                new XdmQName(node.Name),
                node.Parameters.Count,
                args =>
                {
                    var evalContext = capturedContext.Clone();
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        evalContext.SetVariable(paramNames[i], args[i]);
                    }

                    var evaluator = new XQueryEvaluator(evalContext);
                    return evaluator.Evaluate(node.Body);
                });

            _context.RegisterFunction(new XdmQName(node.Name), func);
        }
        return XdmSequence.Empty;
    }

    public override XdmSequence VisitNamespaceDecl(NamespaceDeclNode node)
    {
        _context.RegisterNamespace(node.Prefix, node.Uri);
        return XdmSequence.Empty;
    }

    #endregion

    #region Node Constructors

    public override XdmSequence VisitElementConstructor(ElementConstructorExpr node)
    {
        XdmQName name;

        if (node.Name != null)
        {
            name = node.Name;
        }
        else if (node.NameExpr != null)
        {
            var nameResult = base.Evaluate(node.NameExpr);
            var nameStr = nameResult.StringValue;
            name = ParseQName(nameStr);
        }
        else
        {
            throw new XdmException("XQDY0074", "Element name required");
        }

        var element = new XdmElement(name.NamespaceUri ?? "", name.LocalName, name.Prefix ?? "");

        // Process content (attributes first, then other content)
        var attributes = new List<XdmAttribute>();
        var children = new List<XdmNode>();

        foreach (var contentExpr in node.Content)
        {
            var content = base.Evaluate(contentExpr);

            foreach (var item in content)
            {
                if (item is XdmAttribute attr)
                {
                    attributes.Add(attr);
                }
                else if (item is XdmNode childNode)
                {
                    children.Add(childNode.DeepCopy());
                }
                else
                {
                    // Atomic values become text nodes
                    children.Add(new XdmText(item.StringValue));
                }
            }
        }

        // Add attributes
        foreach (var attr in attributes)
        {
            element.SetAttribute(attr.LocalName, attr.Value, attr.NamespaceUri, attr.Prefix);
        }

        // Add children
        foreach (var child in children)
        {
            element.AppendChild(child);
        }

        return new XdmSequence(element);
    }

    public override XdmSequence VisitAttributeConstructor(AttributeConstructorExpr node)
    {
        XdmQName name;

        if (node.Name != null)
        {
            name = node.Name;
        }
        else if (node.NameExpr != null)
        {
            var nameResult = base.Evaluate(node.NameExpr);
            var nameStr = nameResult.StringValue;
            name = ParseQName(nameStr);
        }
        else
        {
            throw new XdmException("XQDY0074", "Attribute name required");
        }

        string value = "";
        if (node.Value != null)
        {
            var valueResult = base.Evaluate(node.Value);
            value = valueResult.StringValue;
        }

        var attr = new XdmAttribute(name.NamespaceUri ?? "", name.LocalName, name.Prefix ?? "", value);
        return new XdmSequence(attr);
    }

    public override XdmSequence VisitTextConstructor(TextConstructorExpr node)
    {
        var content = base.Evaluate(node.Content);
        var text = new XdmText(content.StringValue);
        return new XdmSequence(text);
    }

    public override XdmSequence VisitCommentConstructor(CommentConstructorExpr node)
    {
        var content = base.Evaluate(node.Content);
        var comment = new XdmComment(content.StringValue);
        return new XdmSequence(comment);
    }

    public override XdmSequence VisitPIConstructor(PIConstructorExpr node)
    {
        string target;

        if (node.Target != null)
        {
            target = node.Target;
        }
        else if (node.TargetExpr != null)
        {
            var targetResult = base.Evaluate(node.TargetExpr);
            target = targetResult.StringValue;
        }
        else
        {
            throw new XdmException("XQDY0074", "Processing instruction target required");
        }

        string data = "";
        if (node.Content != null)
        {
            var dataResult = base.Evaluate(node.Content);
            data = dataResult.StringValue;
        }

        var pi = new XdmProcessingInstruction(target, data);
        return new XdmSequence(pi);
    }

    public override XdmSequence VisitDocumentConstructor(DocumentConstructorExpr node)
    {
        var doc = new XdmDocument();
        var content = base.Evaluate(node.Content);

        foreach (var item in content)
        {
            if (item is XdmNode node2)
            {
                doc.AppendChild(node2.DeepCopy());
            }
            else
            {
                doc.AppendChild(new XdmText(item.StringValue));
            }
        }

        return new XdmSequence(doc);
    }

    public override XdmSequence VisitComputedConstructor(ComputedConstructorExpr node)
    {
        return node.ConstructorType.ToLower() switch
        {
            "element" => VisitComputedElement(node),
            "attribute" => VisitComputedAttribute(node),
            "text" => VisitComputedText(node),
            "comment" => VisitComputedComment(node),
            "processing-instruction" => VisitComputedPI(node),
            "document" => VisitComputedDocument(node),
            "namespace" => VisitComputedNamespace(node),
            _ => throw new XdmException("XQST0125", $"Unknown constructor type: {node.ConstructorType}")
        };
    }

    private XdmSequence VisitComputedElement(ComputedConstructorExpr node)
    {
        var nameResult = base.Evaluate(node.Name!);
        var name = ParseQName(nameResult.StringValue);

        var element = new XdmElement(name.NamespaceUri ?? "", name.LocalName, name.Prefix ?? "");

        if (node.Content != null)
        {
            var content = base.Evaluate(node.Content);
            foreach (var item in content)
            {
                if (item is XdmAttribute attr)
                {
                    element.SetAttribute(attr.LocalName, attr.Value, attr.NamespaceUri, attr.Prefix);
                }
                else if (item is XdmNode childNode)
                {
                    element.AppendChild(childNode.DeepCopy());
                }
                else
                {
                    element.AppendChild(new XdmText(item.StringValue));
                }
            }
        }

        return new XdmSequence(element);
    }

    private XdmSequence VisitComputedAttribute(ComputedConstructorExpr node)
    {
        var nameResult = base.Evaluate(node.Name!);
        var name = ParseQName(nameResult.StringValue);

        string value = "";
        if (node.Content != null)
        {
            value = base.Evaluate(node.Content).StringValue;
        }

        var attr = new XdmAttribute(name.NamespaceUri ?? "", name.LocalName, name.Prefix ?? "", value);
        return new XdmSequence(attr);
    }

    private XdmSequence VisitComputedText(ComputedConstructorExpr node)
    {
        string value = "";
        if (node.Content != null)
        {
            value = base.Evaluate(node.Content).StringValue;
        }

        if (string.IsNullOrEmpty(value))
            return XdmSequence.Empty;

        return new XdmSequence(new XdmText(value));
    }

    private XdmSequence VisitComputedComment(ComputedConstructorExpr node)
    {
        string value = "";
        if (node.Content != null)
        {
            value = base.Evaluate(node.Content).StringValue;
        }

        return new XdmSequence(new XdmComment(value));
    }

    private XdmSequence VisitComputedPI(ComputedConstructorExpr node)
    {
        var targetResult = base.Evaluate(node.Name!);
        var target = targetResult.StringValue;

        string data = "";
        if (node.Content != null)
        {
            data = base.Evaluate(node.Content).StringValue;
        }

        return new XdmSequence(new XdmProcessingInstruction(target, data));
    }

    private XdmSequence VisitComputedDocument(ComputedConstructorExpr node)
    {
        var doc = new XdmDocument();

        if (node.Content != null)
        {
            var content = base.Evaluate(node.Content);
            foreach (var item in content)
            {
                if (item is XdmNode childNode)
                {
                    doc.AppendChild(childNode.DeepCopy());
                }
                else
                {
                    doc.AppendChild(new XdmText(item.StringValue));
                }
            }
        }

        return new XdmSequence(doc);
    }

    private XdmSequence VisitComputedNamespace(ComputedConstructorExpr node)
    {
        // Namespace nodes are typically created implicitly
        throw new XdmException("XQST0125", "Namespace constructor not supported");
    }

    private XdmQName ParseQName(string qname)
    {
        var colonIndex = qname.IndexOf(':');
        if (colonIndex < 0)
        {
            return new XdmQName(qname);
        }

        var prefix = qname.Substring(0, colonIndex);
        var localName = qname.Substring(colonIndex + 1);

        // Look up namespace
        var ns = _context.GetNamespace(prefix);
        if (ns == null)
        {
            throw new XdmException("XPST0081", $"Unknown namespace prefix: {prefix}");
        }

        return new XdmQName(ns, localName, prefix);
    }

    #endregion

    #region XQuery Update Facility

    public override XdmSequence VisitInsertExpr(InsertExpr node)
    {
        var source = base.Evaluate(node.Source);
        var target = base.Evaluate(node.Target);

        var targetNode = target.Single() as XdmNode
            ?? throw new XdmException("XUTY0005", "Insert target must be a node");

        foreach (var item in source)
        {
            if (item is not XdmNode sourceNode)
                throw new XdmException("XUTY0004", "Insert source must be a node");

            _pendingUpdates.AddInsert(sourceNode.DeepCopy(), targetNode, node.Position);
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitDeleteExpr(DeleteExpr node)
    {
        var target = base.Evaluate(node.Target);

        foreach (var item in target)
        {
            if (item is not XdmNode targetNode)
                throw new XdmException("XUTY0007", "Delete target must be a node");

            _pendingUpdates.AddDelete(targetNode);
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitReplaceExpr(ReplaceExpr node)
    {
        var target = base.Evaluate(node.Target);
        var replacement = base.Evaluate(node.Replacement);

        var targetNode = target.Single() as XdmNode
            ?? throw new XdmException("XUTY0008", "Replace target must be a single node");

        if (node.ValueOf)
        {
            // Replace value of node
            var newValue = replacement.StringValue;
            _pendingUpdates.AddReplaceValue(targetNode, newValue);
        }
        else
        {
            // Replace node
            var replacementNodes = new List<XdmNode>();
            foreach (var item in replacement)
            {
                if (item is XdmNode rNode)
                    replacementNodes.Add(rNode.DeepCopy());
                else
                    replacementNodes.Add(new XdmText(item.StringValue));
            }

            _pendingUpdates.AddReplaceNode(targetNode, replacementNodes);
        }

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitRenameExpr(RenameExpr node)
    {
        var target = base.Evaluate(node.Target);
        var newNameResult = base.Evaluate(node.NewName);

        var targetNode = target.Single() as XdmNode
            ?? throw new XdmException("XUTY0012", "Rename target must be a single node");

        var newName = ParseQName(newNameResult.StringValue);
        _pendingUpdates.AddRename(targetNode, newName);

        return XdmSequence.Empty;
    }

    public override XdmSequence VisitTransformExpr(TransformExpr node)
    {
        // Create copies of the bound nodes
        var transformContext = _context.Clone();

        foreach (var binding in node.CopyBindings)
        {
            var source = base.Evaluate(binding.Expression);
            var sourceNode = source.Single() as XdmNode
                ?? throw new XdmException("XUTY0013", "Copy binding must be a node");

            var copy = sourceNode.DeepCopy();
            transformContext.SetVariable(binding.Variable, new XdmSequence(copy));
        }

        // Evaluate modify expression with a sub-evaluator that collects updates
        var modifyEvaluator = new XQueryEvaluator(transformContext);
        modifyEvaluator.ApplyUpdatesOnComplete = false;

        var oldContext = _context;
        _context = transformContext;

        try
        {
            modifyEvaluator.Evaluate(node.ModifyExpr);

            // Apply the collected updates to the copies
            modifyEvaluator.PendingUpdates.Apply();

            // Return the modified copies
            return base.Evaluate(node.ReturnExpr);
        }
        finally
        {
            _context = oldContext;
        }
    }

    #endregion
}

/// <summary>
/// Represents a list of pending updates for XQuery Update Facility.
/// </summary>
public class PendingUpdateList
{
    private readonly List<UpdatePrimitive> _updates = new();

    public bool HasUpdates => _updates.Count > 0;

    public void AddInsert(XdmNode source, XdmNode target, InsertPosition position)
    {
        _updates.Add(new InsertPrimitive(source, target, position));
    }

    public void AddDelete(XdmNode target)
    {
        _updates.Add(new DeletePrimitive(target));
    }

    public void AddReplaceNode(XdmNode target, List<XdmNode> replacement)
    {
        _updates.Add(new ReplaceNodePrimitive(target, replacement));
    }

    public void AddReplaceValue(XdmNode target, string newValue)
    {
        _updates.Add(new ReplaceValuePrimitive(target, newValue));
    }

    public void AddRename(XdmNode target, XdmQName newName)
    {
        _updates.Add(new RenamePrimitive(target, newName));
    }

    /// <summary>
    /// Applies all pending updates to the DOM.
    /// </summary>
    public void Apply()
    {
        // Sort updates for proper application order:
        // 1. Renames (before any structural changes)
        // 2. Replace value operations
        // 3. Inserts (descending document order to avoid index issues)
        // 4. Replace node operations
        // 5. Deletes (descending document order)

        var renames = _updates.OfType<RenamePrimitive>().ToList();
        var replaceValues = _updates.OfType<ReplaceValuePrimitive>().ToList();
        var inserts = _updates.OfType<InsertPrimitive>().ToList();
        var replaceNodes = _updates.OfType<ReplaceNodePrimitive>().ToList();
        var deletes = _updates.OfType<DeletePrimitive>().ToList();

        // Apply renames
        foreach (var rename in renames)
        {
            rename.Apply();
        }

        // Apply replace value operations
        foreach (var replaceValue in replaceValues)
        {
            replaceValue.Apply();
        }

        // Apply inserts
        foreach (var insert in inserts)
        {
            insert.Apply();
        }

        // Apply replace node operations
        foreach (var replaceNode in replaceNodes)
        {
            replaceNode.Apply();
        }

        // Apply deletes (reverse document order)
        foreach (var delete in deletes)
        {
            delete.Apply();
        }

        _updates.Clear();
    }

    public void Clear()
    {
        _updates.Clear();
    }
}

/// <summary>
/// Base class for update primitives.
/// </summary>
public abstract class UpdatePrimitive
{
    public abstract void Apply();
}

/// <summary>
/// Represents an insert update.
/// </summary>
public class InsertPrimitive : UpdatePrimitive
{
    public XdmNode Source { get; }
    public XdmNode Target { get; }
    public InsertPosition Position { get; }

    public InsertPrimitive(XdmNode source, XdmNode target, InsertPosition position)
    {
        Source = source;
        Target = target;
        Position = position;
    }

    public override void Apply()
    {
        switch (Position)
        {
            case InsertPosition.Into:
            case InsertPosition.AsLast:
                if (Target is XdmElement elem)
                {
                    elem.AppendChild(Source);
                }
                else if (Target is XdmDocument doc)
                {
                    doc.AppendChild(Source);
                }
                break;

            case InsertPosition.AsFirst:
                if (Target is XdmElement elem2)
                {
                    elem2.InsertChildAt(0, Source);
                }
                else if (Target is XdmDocument doc2)
                {
                    doc2.InsertChildAt(0, Source);
                }
                break;

            case InsertPosition.Before:
                var parent = Target.Parent;
                if (parent != null)
                {
                    var index = parent.Children.ToList().IndexOf(Target);
                    if (index >= 0)
                    {
                        InsertChildAtIndex(parent, index, Source);
                    }
                }
                break;

            case InsertPosition.After:
                var parent2 = Target.Parent;
                if (parent2 != null)
                {
                    var index = parent2.Children.ToList().IndexOf(Target);
                    if (index >= 0)
                    {
                        InsertChildAtIndex(parent2, index + 1, Source);
                    }
                }
                break;
        }
    }

    private static void InsertChildAtIndex(XdmNode parent, int index, XdmNode child)
    {
        switch (parent)
        {
            case XdmElement elem:
                elem.InsertChildAt(index, child);
                break;
            case XdmDocument doc:
                doc.InsertChildAt(index, child);
                break;
            default:
                throw new XdmException("XUDY0024", $"Cannot insert child into {parent.NodeKind}");
        }
    }
}

/// <summary>
/// Represents a delete update.
/// </summary>
public class DeletePrimitive : UpdatePrimitive
{
    public XdmNode Target { get; }

    public DeletePrimitive(XdmNode target)
    {
        Target = target;
    }

    public override void Apply()
    {
        var parent = Target.Parent;
        if (parent != null)
        {
            RemoveChildFromParent(parent, Target);
        }
        else if (Target is XdmAttribute attr && attr.OwnerElement != null)
        {
            attr.OwnerElement.RemoveAttribute(attr.LocalName, attr.NamespaceUri);
        }
    }

    private static void RemoveChildFromParent(XdmNode parent, XdmNode child)
    {
        switch (parent)
        {
            case XdmElement elem:
                elem.RemoveChild(child);
                break;
            case XdmDocument doc:
                doc.RemoveChild(child);
                break;
            default:
                throw new XdmException("XUDY0024", $"Cannot remove child from {parent.NodeKind}");
        }
    }
}

/// <summary>
/// Represents a replace node update.
/// </summary>
public class ReplaceNodePrimitive : UpdatePrimitive
{
    public XdmNode Target { get; }
    public List<XdmNode> Replacement { get; }

    public ReplaceNodePrimitive(XdmNode target, List<XdmNode> replacement)
    {
        Target = target;
        Replacement = replacement;
    }

    public override void Apply()
    {
        var parent = Target.Parent;
        if (parent == null)
        {
            throw new XdmException("XUDY0009", "Cannot replace node without parent");
        }

        var index = parent.Children.ToList().IndexOf(Target);
        if (index >= 0)
        {
            RemoveChildFromParent(parent, Target);

            // Insert replacement nodes
            for (int i = 0; i < Replacement.Count; i++)
            {
                InsertChildAtIndex(parent, index + i, Replacement[i]);
            }
        }
    }

    private static void RemoveChildFromParent(XdmNode parent, XdmNode child)
    {
        switch (parent)
        {
            case XdmElement elem:
                elem.RemoveChild(child);
                break;
            case XdmDocument doc:
                doc.RemoveChild(child);
                break;
        }
    }

    private static void InsertChildAtIndex(XdmNode parent, int index, XdmNode child)
    {
        switch (parent)
        {
            case XdmElement elem:
                elem.InsertChildAt(index, child);
                break;
            case XdmDocument doc:
                doc.InsertChildAt(index, child);
                break;
        }
    }
}

/// <summary>
/// Represents a replace value update.
/// </summary>
public class ReplaceValuePrimitive : UpdatePrimitive
{
    public XdmNode Target { get; }
    public string NewValue { get; }

    public ReplaceValuePrimitive(XdmNode target, string newValue)
    {
        Target = target;
        NewValue = newValue;
    }

    public override void Apply()
    {
        switch (Target)
        {
            case XdmElement elem:
                elem.SetTextContent(NewValue);
                break;
            case XdmAttribute attr:
                attr.Value = NewValue;
                break;
            case XdmText text:
                text.Value = NewValue;
                break;
            case XdmComment comment:
                comment.Value = NewValue;
                break;
            case XdmProcessingInstruction pi:
                pi.Data = NewValue;
                break;
            default:
                throw new XdmException("XUTY0008", $"Cannot replace value of {Target.NodeKind}");
        }
    }
}

/// <summary>
/// Represents a rename update.
/// </summary>
public class RenamePrimitive : UpdatePrimitive
{
    public XdmNode Target { get; }
    public XdmQName NewName { get; }

    public RenamePrimitive(XdmNode target, XdmQName newName)
    {
        Target = target;
        NewName = newName;
    }

    public override void Apply()
    {
        switch (Target)
        {
            case XdmElement elem:
                elem.Rename(NewName);
                break;
            case XdmAttribute attr:
                attr.Rename(NewName);
                break;
            case XdmProcessingInstruction pi:
                pi.Target = NewName.LocalName;
                break;
            default:
                throw new XdmException("XUTY0012", $"Cannot rename {Target.NodeKind}");
        }
    }
}
