using XQuery.DataModel;
using XQuery.Parser.Ast;
using XQuery.Functions;

namespace XQuery.Engine;

/// <summary>
/// Evaluates XPath 4.0 expressions.
/// </summary>
public class XPathEvaluator : IAstVisitor<XdmSequence>
{
    protected EvaluationContext _context;
    protected readonly FunctionLibrary _functions;

    public XPathEvaluator(EvaluationContext? context = null)
    {
        _context = context ?? EvaluationContext.CreateDefault();
        _functions = new FunctionLibrary();
    }

    /// <summary>
    /// Evaluates an expression and returns the result.
    /// </summary>
    public XdmSequence Evaluate(ExprNode expr)
    {
        return expr.Accept(this);
    }

    /// <summary>
    /// Evaluates an expression with a context item.
    /// </summary>
    public XdmSequence Evaluate(ExprNode expr, XdmItem contextItem)
    {
        var oldContext = _context;
        _context = _context.WithContextItem(contextItem);
        try
        {
            return expr.Accept(this);
        }
        finally
        {
            _context = oldContext;
        }
    }

    #region Literals

    public XdmSequence VisitIntegerLiteral(IntegerLiteralExpr node)
    {
        return new XdmSequence(new XdmAtomicValue(node.Value));
    }

    public XdmSequence VisitDecimalLiteral(DecimalLiteralExpr node)
    {
        return new XdmSequence(new XdmAtomicValue(node.Value));
    }

    public XdmSequence VisitDoubleLiteral(DoubleLiteralExpr node)
    {
        return new XdmSequence(new XdmAtomicValue(node.Value));
    }

    public XdmSequence VisitStringLiteral(StringLiteralExpr node)
    {
        return new XdmSequence(new XdmAtomicValue(node.Value));
    }

    #endregion

    #region Primary Expressions

    public XdmSequence VisitVariableRef(VariableRefExpr node)
    {
        return _context.GetVariable(node.QName);
    }

    public XdmSequence VisitContextItem(ContextItemExpr node)
    {
        if (_context.ContextItem == null)
            throw XdmException.NoContextItem();
        return new XdmSequence(_context.ContextItem);
    }

    public XdmSequence VisitFunctionCall(FunctionCallExpr node)
    {
        // Evaluate arguments
        var args = node.Arguments.Select(a => Evaluate(a)).ToArray();

        // Special handling for dynamic function calls
        if (node.Name == "__dynamic__")
        {
            var func = args[0].Single() as XdmFunction
                ?? throw new XdmException("XPTY0004", "Expected function item for dynamic call");
            return func.Invoke(args.Skip(1).ToArray());
        }

        // Look up function
        var qname = node.QName;
        var func2 = _context.GetFunction(qname, args.Length);

        if (func2 != null)
            return func2.Invoke(args);

        // Try built-in functions
        return _functions.Call(qname, args, _context);
    }

    public XdmSequence VisitInlineFunctionExpr(InlineFunctionExpr node)
    {
        var capturedContext = _context.Clone();
        var paramNames = node.Parameters.Select(p => p.Name).ToArray();

        var func = new XdmFunction(
            null,
            node.Parameters.Count,
            args =>
            {
                var evalContext = capturedContext.Clone();
                for (int i = 0; i < paramNames.Length; i++)
                {
                    evalContext.SetVariable(paramNames[i], args[i]);
                }

                var evaluator = new XPathEvaluator(evalContext);
                return evaluator.Evaluate(node.Body);
            });

        return new XdmSequence(func);
    }

    public XdmSequence VisitNamedFunctionRef(NamedFunctionRefExpr node)
    {
        var qname = node.QName;
        var arity = node.Arity;

        // Check context functions
        var func = _context.GetFunction(qname, arity);
        if (func != null)
            return new XdmSequence(func);

        // Check built-in functions
        func = _functions.GetFunction(qname, arity);
        if (func != null)
            return new XdmSequence(func);

        throw XdmException.UndefinedFunction(qname.ToString(), arity);
    }

    public XdmSequence VisitParenthesized(ParenthesizedExpr node)
    {
        if (node.Inner == null)
            return XdmSequence.Empty;
        return Evaluate(node.Inner);
    }

    #endregion

    #region Path Expressions

    public XdmSequence VisitPathExpr(PathExpr node)
    {
        XdmSequence result;

        if (node.IsAbsolute)
        {
            // Start from document root
            var contextItem = _context.ContextItem;
            if (contextItem is XdmNode contextNode)
            {
                result = new XdmSequence(contextNode.Root);
            }
            else
            {
                throw new XdmException("XPDY0002", "Context item is not a node for absolute path");
            }
        }
        else
        {
            result = _context.ContextItem != null
                ? new XdmSequence(_context.ContextItem)
                : XdmSequence.Empty;
        }

        if (node.IsRootOnly)
            return result;

        // Apply each step
        foreach (var step in node.Steps)
        {
            var newResult = new List<XdmItem>();
            var items = result.ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var stepContext = _context.WithContextItem(item, i + 1, items.Count);
                var oldContext = _context;
                _context = stepContext;

                try
                {
                    var stepResult = Evaluate(step);
                    foreach (var r in stepResult)
                    {
                        if (!newResult.Contains(r))
                            newResult.Add(r);
                    }
                }
                finally
                {
                    _context = oldContext;
                }
            }

            // Sort by document order and remove duplicates
            result = SortByDocumentOrder(newResult);
        }

        return result;
    }

    private XdmSequence SortByDocumentOrder(List<XdmItem> items)
    {
        var nodes = items.OfType<XdmNode>().ToList();
        if (nodes.Count == items.Count && nodes.Count > 1)
        {
            nodes.Sort((a, b) => a.CompareDocumentOrder(b));
            return new XdmSequence(nodes.Cast<XdmItem>().Distinct());
        }
        return new XdmSequence(items);
    }

    public XdmSequence VisitAxisStep(AxisStepExpr node)
    {
        var contextItem = _context.ContextItem;
        if (contextItem is not XdmNode contextNode)
            throw new XdmException("XPTY0020", "Context item is not a node");

        // Get nodes from axis
        var axisNodes = GetAxisNodes(contextNode, node.Axis);

        // Apply node test
        var filtered = new List<XdmNode>();
        foreach (var axisNode in axisNodes)
        {
            if (MatchesNodeTest(axisNode, node.NodeTest))
                filtered.Add(axisNode);
        }

        // Apply predicates
        XdmSequence result = new XdmSequence(filtered.Cast<XdmItem>());

        foreach (var predicate in node.Predicates)
        {
            result = ApplyPredicate(result, predicate);
        }

        return result;
    }

    private IEnumerable<XdmNode> GetAxisNodes(XdmNode node, Axis axis)
    {
        return axis switch
        {
            Axis.Child => node.Children,
            Axis.Descendant => node.Descendants(),
            Axis.DescendantOrSelf => node.DescendantsAndSelf(),
            Axis.Parent => node.Parent != null ? new[] { node.Parent } : Array.Empty<XdmNode>(),
            Axis.Ancestor => node.Ancestors(),
            Axis.AncestorOrSelf => node.AncestorsAndSelf(),
            Axis.Following => node.Following(),
            Axis.FollowingSibling => node.FollowingSiblings(),
            Axis.Preceding => node.Preceding(),
            Axis.PrecedingSibling => node.PrecedingSiblings(),
            Axis.Self => new[] { node },
            Axis.Attribute => node.Attributes.Cast<XdmNode>(),
            Axis.Namespace => Array.Empty<XdmNode>(), // Not commonly used
            _ => throw new NotImplementedException($"Axis {axis} not implemented")
        };
    }

    private bool MatchesNodeTest(XdmNode node, ExprNode nodeTest)
    {
        if (nodeTest is NameTestExpr nameTest)
        {
            if (nameTest.IsWildcard)
                return true;

            if (node.NodeName == null)
                return false;

            if (nameTest.IsLocalWildcard)
                return node.NodeName.NamespaceUri == nameTest.NamespaceUri;

            if (nameTest.IsPrefixWildcard)
                return node.NodeName.LocalName == nameTest.LocalName;

            // Match name
            if (!string.IsNullOrEmpty(nameTest.NamespaceUri))
            {
                return node.NodeName.NamespaceUri == nameTest.NamespaceUri &&
                       node.NodeName.LocalName == nameTest.LocalName;
            }

            return node.NodeName.LocalName == nameTest.LocalName;
        }

        if (nodeTest is KindTestExpr kindTest)
        {
            // node() matches everything
            if (kindTest.Kind == XdmNodeKind.Element && kindTest.Name == null && kindTest.TypeName == null)
            {
                // Check if this is node() (matches all) vs element() (matches elements only)
                // For now, assume it's node()
                return true;
            }

            if (node.NodeKind != kindTest.Kind)
                return false;

            if (kindTest.Name != null && node.NodeName != kindTest.Name)
                return false;

            return true;
        }

        return true;
    }

    public XdmSequence VisitFilterExpr(FilterExpr node)
    {
        var result = Evaluate(node.Primary);

        foreach (var predicate in node.Predicates)
        {
            result = ApplyPredicate(result, predicate);
        }

        return result;
    }

    private XdmSequence ApplyPredicate(XdmSequence sequence, ExprNode predicate)
    {
        var items = sequence.ToList();
        var filtered = new List<XdmItem>();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var predicateContext = _context.WithContextItem(item, i + 1, items.Count);
            var oldContext = _context;
            _context = predicateContext;

            try
            {
                var predicateResult = Evaluate(predicate);

                // Numeric predicate
                if (predicateResult.IsSingleton && predicateResult.First is XdmAtomicValue av && av.IsNumeric)
                {
                    var pos = av.AsDouble();
                    if (Math.Abs(pos - (i + 1)) < 0.0001)
                        filtered.Add(item);
                }
                else
                {
                    // Boolean predicate
                    if (predicateResult.EffectiveBooleanValue)
                        filtered.Add(item);
                }
            }
            finally
            {
                _context = oldContext;
            }
        }

        return new XdmSequence(filtered);
    }

    public XdmSequence VisitPredicateList(PredicateListExpr node)
    {
        throw new NotImplementedException("PredicateList should not be visited directly");
    }

    public XdmSequence VisitNameTest(NameTestExpr node)
    {
        // Name tests are handled in MatchesNodeTest
        throw new NotImplementedException("NameTest should not be visited directly");
    }

    public XdmSequence VisitKindTest(KindTestExpr node)
    {
        // Kind tests are handled in MatchesNodeTest
        throw new NotImplementedException("KindTest should not be visited directly");
    }

    #endregion

    #region Operators

    public XdmSequence VisitBinaryExpr(BinaryExpr node)
    {
        var left = Evaluate(node.Left);
        var right = Evaluate(node.Right);

        return node.Operator switch
        {
            BinaryOperator.Add => ArithmeticOp(left, right, (a, b) => a + b),
            BinaryOperator.Subtract => ArithmeticOp(left, right, (a, b) => a - b),
            BinaryOperator.Multiply => ArithmeticOp(left, right, (a, b) => a * b),
            BinaryOperator.Divide => ArithmeticOp(left, right, (a, b) =>
            {
                if (b == 0) throw XdmException.DivisionByZero();
                return a / b;
            }),
            BinaryOperator.IntegerDivide => ArithmeticOp(left, right, (a, b) =>
            {
                if (b == 0) throw XdmException.DivisionByZero();
                return Math.Truncate(a / b);
            }),
            BinaryOperator.Modulo => ArithmeticOp(left, right, (a, b) =>
            {
                if (b == 0) throw XdmException.DivisionByZero();
                return a % b;
            }),
            BinaryOperator.And => new XdmSequence(new XdmAtomicValue(
                left.EffectiveBooleanValue && right.EffectiveBooleanValue)),
            BinaryOperator.Or => new XdmSequence(new XdmAtomicValue(
                left.EffectiveBooleanValue || right.EffectiveBooleanValue)),
            _ => throw new NotImplementedException($"Operator {node.Operator} not implemented")
        };
    }

    private XdmSequence ArithmeticOp(XdmSequence left, XdmSequence right, Func<double, double, double> op)
    {
        if (left.IsEmpty || right.IsEmpty)
            return XdmSequence.Empty;

        var leftVal = left.Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");
        var rightVal = right.Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");

        var result = op(leftVal.AsDouble(), rightVal.AsDouble());

        // Determine result type
        if (leftVal.IsInteger && rightVal.IsInteger && double.IsFinite(result) && result == Math.Truncate(result))
            return new XdmSequence(new XdmAtomicValue((long)result));

        if (leftVal.IsDecimal && rightVal.IsDecimal)
            return new XdmSequence(new XdmAtomicValue((decimal)result));

        return new XdmSequence(new XdmAtomicValue(result));
    }

    public XdmSequence VisitUnaryExpr(UnaryExpr node)
    {
        var operand = Evaluate(node.Operand);

        if (operand.IsEmpty)
            return XdmSequence.Empty;

        var value = operand.Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");

        if (node.Operator == UnaryOperator.Minus)
        {
            if (value.IsInteger)
                return new XdmSequence(new XdmAtomicValue(-value.AsInteger()));
            if (value.IsDecimal)
                return new XdmSequence(new XdmAtomicValue(-value.AsDecimal()));
            return new XdmSequence(new XdmAtomicValue(-value.AsDouble()));
        }

        return operand; // Unary plus is identity
    }

    public XdmSequence VisitComparisonExpr(ComparisonExpr node)
    {
        var left = Evaluate(node.Left);
        var right = Evaluate(node.Right);

        return node.Operator switch
        {
            // Value comparisons
            ComparisonOperator.Eq => ValueCompare(left, right, 0),
            ComparisonOperator.Ne => ValueCompare(left, right, null, true),
            ComparisonOperator.Lt => ValueCompare(left, right, -1),
            ComparisonOperator.Le => ValueCompare(left, right, -1, false, true),
            ComparisonOperator.Gt => ValueCompare(left, right, 1),
            ComparisonOperator.Ge => ValueCompare(left, right, 1, false, true),

            // General comparisons
            ComparisonOperator.Equal => GeneralCompare(left, right, 0),
            ComparisonOperator.NotEqual => GeneralCompare(left, right, null, true),
            ComparisonOperator.LessThan => GeneralCompare(left, right, -1),
            ComparisonOperator.LessOrEqual => GeneralCompare(left, right, -1, false, true),
            ComparisonOperator.GreaterThan => GeneralCompare(left, right, 1),
            ComparisonOperator.GreaterOrEqual => GeneralCompare(left, right, 1, false, true),

            // Node comparisons
            ComparisonOperator.Is => NodeCompare(left, right, false),
            ComparisonOperator.Precedes => NodeCompare(left, right, true, true),
            ComparisonOperator.Follows => NodeCompare(left, right, true, false),

            _ => throw new NotImplementedException($"Comparison {node.Operator} not implemented")
        };
    }

    private XdmSequence ValueCompare(XdmSequence left, XdmSequence right, int? expectedCompare, bool notEqual = false, bool allowEqual = false)
    {
        if (left.IsEmpty || right.IsEmpty)
            return XdmSequence.Empty;

        var leftVal = left.Atomize().Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");
        var rightVal = right.Atomize().Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");

        if (notEqual)
            return new XdmSequence(new XdmAtomicValue(!leftVal.ValueEquals(rightVal)));

        if (expectedCompare == 0)
            return new XdmSequence(new XdmAtomicValue(leftVal.ValueEquals(rightVal)));

        var cmp = leftVal.CompareTo(rightVal);

        if (allowEqual && cmp == 0)
            return new XdmSequence(XdmAtomicValue.True);

        return new XdmSequence(new XdmAtomicValue(
            expectedCompare < 0 ? cmp < 0 : cmp > 0));
    }

    private XdmSequence GeneralCompare(XdmSequence left, XdmSequence right, int? expectedCompare, bool notEqual = false, bool allowEqual = false)
    {
        var leftAtomized = left.Atomize();
        var rightAtomized = right.Atomize();

        foreach (var l in leftAtomized)
        {
            foreach (var r in rightAtomized)
            {
                var leftVal = l as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic value");
                var rightVal = r as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic value");

                // Type promotion for comparison
                var (lv, rv) = PromoteForComparison(leftVal, rightVal);

                if (notEqual)
                {
                    if (!lv.ValueEquals(rv))
                        return new XdmSequence(XdmAtomicValue.True);
                }
                else if (expectedCompare == 0)
                {
                    if (lv.ValueEquals(rv))
                        return new XdmSequence(XdmAtomicValue.True);
                }
                else
                {
                    var cmp = lv.CompareTo(rv);
                    if (allowEqual && cmp == 0)
                        return new XdmSequence(XdmAtomicValue.True);
                    if (expectedCompare < 0 && cmp < 0)
                        return new XdmSequence(XdmAtomicValue.True);
                    if (expectedCompare > 0 && cmp > 0)
                        return new XdmSequence(XdmAtomicValue.True);
                }
            }
        }

        return new XdmSequence(XdmAtomicValue.False);
    }

    private (XdmAtomicValue, XdmAtomicValue) PromoteForComparison(XdmAtomicValue left, XdmAtomicValue right)
    {
        // If one is untyped atomic, cast to the type of the other
        if (left.IsUntypedAtomic && !right.IsUntypedAtomic)
        {
            left = left.CastAs(right.TypeName);
        }
        else if (!left.IsUntypedAtomic && right.IsUntypedAtomic)
        {
            right = right.CastAs(left.TypeName);
        }
        else if (left.IsUntypedAtomic && right.IsUntypedAtomic)
        {
            // Both untyped, compare as strings
            left = left.CastAs(XdmQName.XsString);
            right = right.CastAs(XdmQName.XsString);
        }

        return (left, right);
    }

    private XdmSequence NodeCompare(XdmSequence left, XdmSequence right, bool isOrder, bool isPrecedes = false)
    {
        if (left.IsEmpty || right.IsEmpty)
            return XdmSequence.Empty;

        var leftNode = left.Single() as XdmNode
            ?? throw XdmException.TypeError("Expected node");
        var rightNode = right.Single() as XdmNode
            ?? throw XdmException.TypeError("Expected node");

        if (!isOrder)
        {
            // is comparison - node identity
            return new XdmSequence(new XdmAtomicValue(ReferenceEquals(leftNode, rightNode)));
        }

        // << and >> - document order
        var cmp = leftNode.CompareDocumentOrder(rightNode);
        return new XdmSequence(new XdmAtomicValue(
            isPrecedes ? cmp < 0 : cmp > 0));
    }

    public XdmSequence VisitRangeExpr(RangeExpr node)
    {
        var start = Evaluate(node.Start);
        var end = Evaluate(node.End);

        if (start.IsEmpty || end.IsEmpty)
            return XdmSequence.Empty;

        var startVal = (start.Single() as XdmAtomicValue)?.AsInteger()
            ?? throw XdmException.TypeError("Expected integer");
        var endVal = (end.Single() as XdmAtomicValue)?.AsInteger()
            ?? throw XdmException.TypeError("Expected integer");

        return XdmSequence.Range(startVal, endVal);
    }

    public XdmSequence VisitConcatExpr(ConcatExpr node)
    {
        var left = Evaluate(node.Left);
        var right = Evaluate(node.Right);

        var leftStr = left.IsEmpty ? "" : left.Atomize().StringValue;
        var rightStr = right.IsEmpty ? "" : right.Atomize().StringValue;

        return new XdmSequence(new XdmAtomicValue(leftStr + rightStr));
    }

    #endregion

    #region Sequence Expressions

    public XdmSequence VisitSequenceExpr(SequenceExpr node)
    {
        var items = new List<XdmItem>();

        foreach (var item in node.Items)
        {
            var result = Evaluate(item);
            items.AddRange(result);
        }

        return new XdmSequence(items);
    }

    public XdmSequence VisitUnionExpr(UnionExpr node)
    {
        var nodes = new List<XdmNode>();

        foreach (var operand in node.Operands)
        {
            var result = Evaluate(operand);
            foreach (var item in result)
            {
                if (item is not XdmNode nodeItem)
                    throw XdmException.TypeError("Union requires node sequences");
                if (!nodes.Any(n => ReferenceEquals(n, nodeItem)))
                    nodes.Add(nodeItem);
            }
        }

        // Sort by document order
        nodes.Sort((a, b) => a.CompareDocumentOrder(b));
        return new XdmSequence(nodes.Cast<XdmItem>());
    }

    public XdmSequence VisitIntersectExceptExpr(IntersectExceptExpr node)
    {
        var left = Evaluate(node.Left);
        var right = Evaluate(node.Right);

        var leftNodes = left.ItemsAs<XdmNode>().ToList();
        var rightNodes = right.ItemsAs<XdmNode>().ToHashSet();

        var result = node.IsIntersect
            ? leftNodes.Where(n => rightNodes.Any(r => ReferenceEquals(r, n))).ToList()
            : leftNodes.Where(n => !rightNodes.Any(r => ReferenceEquals(r, n))).ToList();

        result.Sort((a, b) => a.CompareDocumentOrder(b));
        return new XdmSequence(result.Cast<XdmItem>());
    }

    #endregion

    #region Conditional

    public XdmSequence VisitIfExpr(IfExpr node)
    {
        var condition = Evaluate(node.Condition);

        if (condition.EffectiveBooleanValue)
            return Evaluate(node.Then);
        else
            return Evaluate(node.Else);
    }

    public XdmSequence VisitSwitchExpr(SwitchExpr node)
    {
        var operand = Evaluate(node.Operand).Atomize();
        var operandVal = operand.SingleOrDefault() as XdmAtomicValue;

        foreach (var caseClause in node.Cases)
        {
            foreach (var caseValue in caseClause.Values)
            {
                var caseResult = Evaluate(caseValue).Atomize();
                var caseVal = caseResult.SingleOrDefault() as XdmAtomicValue;

                if (operandVal == null && caseVal == null)
                    return Evaluate(caseClause.Result);

                if (operandVal != null && caseVal != null && operandVal.ValueEquals(caseVal))
                    return Evaluate(caseClause.Result);
            }
        }

        return Evaluate(node.Default);
    }

    #endregion

    #region FLWOR

    public virtual XdmSequence VisitFlworExpr(FlworExpr node)
    {
        // Start with a single tuple containing the current context
        var tuples = new List<EvaluationContext> { _context.Clone() };

        // Process each clause
        foreach (var clause in node.Clauses)
        {
            tuples = ProcessFlworClause(tuples, clause);
        }

        // Evaluate return for each tuple
        var results = new List<XdmItem>();
        foreach (var tuple in tuples)
        {
            var oldContext = _context;
            _context = tuple;
            try
            {
                var result = Evaluate(node.Return);
                results.AddRange(result);
            }
            finally
            {
                _context = oldContext;
            }
        }

        return new XdmSequence(results);
    }

    private List<EvaluationContext> ProcessFlworClause(List<EvaluationContext> tuples, FlworClause clause)
    {
        return clause switch
        {
            ForClause forClause => ProcessForClause(tuples, forClause),
            LetClause letClause => ProcessLetClause(tuples, letClause),
            WhereClause whereClause => ProcessWhereClause(tuples, whereClause),
            OrderByClause orderByClause => ProcessOrderByClause(tuples, orderByClause),
            GroupByClause groupByClause => ProcessGroupByClause(tuples, groupByClause),
            CountClause countClause => ProcessCountClause(tuples, countClause),
            _ => throw new NotImplementedException($"FLWOR clause {clause.GetType().Name} not implemented")
        };
    }

    private List<EvaluationContext> ProcessForClause(List<EvaluationContext> tuples, ForClause clause)
    {
        var result = new List<EvaluationContext>();

        foreach (var tuple in tuples)
        {
            var oldContext = _context;
            _context = tuple;

            try
            {
                var sequence = Evaluate(clause.Expression);
                var items = sequence.ToList();

                if (items.Count == 0 && clause.AllowingEmpty)
                {
                    var newTuple = tuple.WithVariable(clause.Variable, XdmSequence.Empty);
                    if (clause.PositionalVariable != null)
                        newTuple = newTuple.WithVariable(clause.PositionalVariable, new XdmSequence(new XdmAtomicValue(0L)));
                    result.Add(newTuple);
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var newTuple = tuple.WithVariable(clause.Variable, new XdmSequence(items[i]));
                        if (clause.PositionalVariable != null)
                            newTuple = newTuple.WithVariable(clause.PositionalVariable, new XdmSequence(new XdmAtomicValue((long)(i + 1))));
                        result.Add(newTuple);
                    }
                }
            }
            finally
            {
                _context = oldContext;
            }
        }

        return result;
    }

    private List<EvaluationContext> ProcessLetClause(List<EvaluationContext> tuples, LetClause clause)
    {
        var result = new List<EvaluationContext>();

        foreach (var tuple in tuples)
        {
            var oldContext = _context;
            _context = tuple;

            try
            {
                var value = Evaluate(clause.Expression);
                result.Add(tuple.WithVariable(clause.Variable, value));
            }
            finally
            {
                _context = oldContext;
            }
        }

        return result;
    }

    private List<EvaluationContext> ProcessWhereClause(List<EvaluationContext> tuples, WhereClause clause)
    {
        var result = new List<EvaluationContext>();

        foreach (var tuple in tuples)
        {
            var oldContext = _context;
            _context = tuple;

            try
            {
                var condition = Evaluate(clause.Condition);
                if (condition.EffectiveBooleanValue)
                    result.Add(tuple);
            }
            finally
            {
                _context = oldContext;
            }
        }

        return result;
    }

    private List<EvaluationContext> ProcessOrderByClause(List<EvaluationContext> tuples, OrderByClause clause)
    {
        // Compute sort keys for each tuple
        var keyed = tuples.Select(tuple =>
        {
            var oldContext = _context;
            _context = tuple;

            try
            {
                var keys = clause.Specs.Select(spec =>
                {
                    var key = Evaluate(spec.Expression);
                    return (value: key.Atomize().SingleOrDefault() as XdmAtomicValue, spec.Descending, spec.EmptyGreatest);
                }).ToList();

                return (tuple, keys);
            }
            finally
            {
                _context = oldContext;
            }
        }).ToList();

        // Sort
        keyed.Sort((a, b) =>
        {
            for (int i = 0; i < a.keys.Count; i++)
            {
                var aKey = a.keys[i];
                var bKey = b.keys[i];

                int cmp;
                if (aKey.value == null && bKey.value == null)
                    cmp = 0;
                else if (aKey.value == null)
                    cmp = aKey.EmptyGreatest ? 1 : -1;
                else if (bKey.value == null)
                    cmp = aKey.EmptyGreatest ? -1 : 1;
                else
                    cmp = aKey.value.CompareTo(bKey.value);

                if (aKey.Descending)
                    cmp = -cmp;

                if (cmp != 0)
                    return cmp;
            }
            return 0;
        });

        return keyed.Select(k => k.tuple).ToList();
    }

    private List<EvaluationContext> ProcessGroupByClause(List<EvaluationContext> tuples, GroupByClause clause)
    {
        // Group tuples by grouping key
        var groups = new Dictionary<string, List<EvaluationContext>>();
        var groupKeys = new Dictionary<string, XdmSequence>();

        foreach (var tuple in tuples)
        {
            var oldContext = _context;
            _context = tuple;

            try
            {
                // Compute grouping key
                var keyParts = new List<string>();
                var keyValues = new Dictionary<string, XdmSequence>();

                foreach (var spec in clause.Specs)
                {
                    XdmSequence value;
                    if (spec.Expression != null)
                        value = Evaluate(spec.Expression);
                    else
                        value = tuple.GetVariable(spec.Variable);

                    var atomized = value.Atomize();
                    var keyStr = atomized.IsEmpty ? "" : atomized.StringValue;
                    keyParts.Add($"{spec.Variable}={keyStr}");
                    keyValues[spec.Variable] = atomized;
                }

                var groupKey = string.Join("|", keyParts);

                if (!groups.ContainsKey(groupKey))
                {
                    groups[groupKey] = new List<EvaluationContext>();
                    groupKeys[groupKey] = new XdmSequence(); // Placeholder
                }

                groups[groupKey].Add(tuple);
            }
            finally
            {
                _context = oldContext;
            }
        }

        // Create result tuples
        var result = new List<EvaluationContext>();

        foreach (var group in groups)
        {
            var firstTuple = group.Value[0];
            var resultTuple = firstTuple.Clone();

            // Set grouping variables to the single key value
            foreach (var spec in clause.Specs)
            {
                var value = firstTuple.GetVariable(spec.Variable);
                resultTuple.SetVariable(spec.Variable, value);
            }

            // Collect non-grouping variables into sequences
            var allVars = new HashSet<string>();
            foreach (var t in group.Value)
            {
                // This is simplified - in reality we'd need to track all variables
            }

            result.Add(resultTuple);
        }

        return result;
    }

    private List<EvaluationContext> ProcessCountClause(List<EvaluationContext> tuples, CountClause clause)
    {
        var result = new List<EvaluationContext>();

        for (int i = 0; i < tuples.Count; i++)
        {
            var newTuple = tuples[i].WithVariable(clause.Variable, new XdmSequence(new XdmAtomicValue((long)(i + 1))));
            result.Add(newTuple);
        }

        return result;
    }

    public XdmSequence VisitForClause(ForClause node) => throw new NotImplementedException();
    public XdmSequence VisitLetClause(LetClause node) => throw new NotImplementedException();
    public XdmSequence VisitWhereClause(WhereClause node) => throw new NotImplementedException();
    public XdmSequence VisitOrderByClause(OrderByClause node) => throw new NotImplementedException();
    public XdmSequence VisitGroupByClause(GroupByClause node) => throw new NotImplementedException();
    public XdmSequence VisitCountClause(CountClause node) => throw new NotImplementedException();

    #endregion

    #region Quantified

    public XdmSequence VisitQuantifiedExpr(QuantifiedExpr node)
    {
        return EvaluateQuantified(node, 0, _context);
    }

    private XdmSequence EvaluateQuantified(QuantifiedExpr node, int bindingIndex, EvaluationContext context)
    {
        if (bindingIndex >= node.Bindings.Count)
        {
            // Evaluate satisfies clause
            var oldContext = _context;
            _context = context;
            try
            {
                var result = Evaluate(node.Satisfies);
                return new XdmSequence(new XdmAtomicValue(result.EffectiveBooleanValue));
            }
            finally
            {
                _context = oldContext;
            }
        }

        var binding = node.Bindings[bindingIndex];
        var oldCtx = _context;
        _context = context;

        try
        {
            var sequence = Evaluate(binding.Expression);

            foreach (var item in sequence)
            {
                var newContext = context.WithVariable(binding.Variable, new XdmSequence(item));
                var result = EvaluateQuantified(node, bindingIndex + 1, newContext);
                var satisfied = result.EffectiveBooleanValue;

                if (node.IsSome && satisfied)
                    return new XdmSequence(XdmAtomicValue.True);
                if (!node.IsSome && !satisfied)
                    return new XdmSequence(XdmAtomicValue.False);
            }

            // some: no match found -> false; every: all matched -> true
            return new XdmSequence(new XdmAtomicValue(!node.IsSome));
        }
        finally
        {
            _context = oldCtx;
        }
    }

    #endregion

    #region Type Expressions

    public XdmSequence VisitInstanceOfExpr(InstanceOfExpr node)
    {
        var value = Evaluate(node.Expression);
        var matches = MatchesSequenceType(value, node.Type);
        return new XdmSequence(new XdmAtomicValue(matches));
    }

    public XdmSequence VisitTreatExpr(TreatExpr node)
    {
        var value = Evaluate(node.Expression);
        if (!MatchesSequenceType(value, node.Type))
            throw new XdmException("XPDY0050", $"Value does not match required type {node.Type}");
        return value;
    }

    public XdmSequence VisitCastableExpr(CastableExpr node)
    {
        var value = Evaluate(node.Expression);

        if (value.IsEmpty)
            return new XdmSequence(new XdmAtomicValue(node.AllowEmpty));

        if (value.Count > 1)
            return new XdmSequence(XdmAtomicValue.False);

        try
        {
            var atomized = value.Atomize().Single() as XdmAtomicValue;
            atomized?.CastAs(node.TargetType);
            return new XdmSequence(XdmAtomicValue.True);
        }
        catch
        {
            return new XdmSequence(XdmAtomicValue.False);
        }
    }

    public XdmSequence VisitCastExpr(CastExpr node)
    {
        var value = Evaluate(node.Expression);

        if (value.IsEmpty)
        {
            if (node.AllowEmpty)
                return XdmSequence.Empty;
            throw new XdmException("XPTY0004", "Cannot cast empty sequence to non-optional type");
        }

        if (value.Count > 1)
            throw new XdmException("XPTY0004", "Cannot cast sequence of more than one item");

        var atomized = value.Atomize().Single() as XdmAtomicValue
            ?? throw XdmException.TypeError("Expected atomic value");

        return new XdmSequence(atomized.CastAs(node.TargetType));
    }

    private bool MatchesSequenceType(XdmSequence value, SequenceTypeNode type)
    {
        // Check cardinality
        switch (type.Occurrence)
        {
            case OccurrenceIndicator.ExactlyOne:
                if (value.Count != 1) return false;
                break;
            case OccurrenceIndicator.ZeroOrOne:
                if (value.Count > 1) return false;
                break;
            case OccurrenceIndicator.OneOrMore:
                if (value.Count < 1) return false;
                break;
        }

        // Check item types
        foreach (var item in value)
        {
            if (!MatchesItemType(item, type.ItemType))
                return false;
        }

        return true;
    }

    private bool MatchesItemType(XdmItem item, ItemTypeNode type)
    {
        return type.Kind switch
        {
            ItemTypeKind.Item => true,
            ItemTypeKind.Empty => false,
            ItemTypeKind.AtomicType => item is XdmAtomicValue av && IsSubtypeOf(av.TypeName, type.TypeName!),
            ItemTypeKind.Node => item is XdmNode,
            ItemTypeKind.Element => item is XdmElement,
            ItemTypeKind.Attribute => item is XdmAttribute,
            ItemTypeKind.Document => item is XdmDocument,
            ItemTypeKind.Text => item is XdmText,
            ItemTypeKind.Comment => item is XdmComment,
            ItemTypeKind.ProcessingInstruction => item is XdmProcessingInstruction,
            ItemTypeKind.Function => item is XdmFunction,
            ItemTypeKind.Map => item is XdmMap,
            ItemTypeKind.Array => item is XdmArray,
            _ => false
        };
    }

    private bool IsSubtypeOf(XdmQName actual, XdmQName expected)
    {
        if (actual == expected) return true;
        if (expected == XdmQName.XsAnyAtomicType) return true;

        // Check numeric hierarchy
        if (expected == XdmQName.XsDecimal)
            return actual == XdmQName.XsInteger;

        // Add more type hierarchy checks as needed
        return false;
    }

    #endregion

    #region Arrow/Lookup

    public XdmSequence VisitArrowExpr(ArrowExpr node)
    {
        var argument = Evaluate(node.Argument);

        // Get the function
        XdmFunction? func = null;

        if (node.Function is FunctionCallExpr funcCall)
        {
            // Create argument list with argument as first parameter
            var args = new List<XdmSequence> { argument };
            args.AddRange(node.AdditionalArguments.Select(a => Evaluate(a)));

            return _functions.Call(funcCall.QName, args.ToArray(), _context);
        }
        else
        {
            var funcResult = Evaluate(node.Function);
            func = funcResult.Single() as XdmFunction
                ?? throw new XdmException("XPTY0004", "Arrow expression requires a function");

            var args = new List<XdmSequence> { argument };
            args.AddRange(node.AdditionalArguments.Select(a => Evaluate(a)));

            return func.Invoke(args.ToArray());
        }
    }

    public XdmSequence VisitLookupExpr(LookupExpr node)
    {
        var contextItem = _context.ContextItem;
        return PerformLookup(contextItem, node.KeyExpr, node.IsWildcard);
    }

    public XdmSequence VisitPostfixLookupExpr(PostfixLookupExpr node)
    {
        var baseResult = Evaluate(node.Base);
        var results = new List<XdmItem>();

        foreach (var item in baseResult)
        {
            var lookupResult = PerformLookup(item, node.KeyExpr, node.IsWildcard);
            results.AddRange(lookupResult);
        }

        return new XdmSequence(results);
    }

    private XdmSequence PerformLookup(XdmItem? item, ExprNode? keyExpr, bool isWildcard)
    {
        if (item is XdmMap map)
        {
            if (isWildcard)
            {
                var results = new List<XdmItem>();
                foreach (var value in map.Values)
                    results.AddRange(value);
                return new XdmSequence(results);
            }

            var key = Evaluate(keyExpr!).Atomize().Single() as XdmAtomicValue
                ?? throw XdmException.TypeError("Map key must be atomic");
            return map[key];
        }

        if (item is XdmArray array)
        {
            if (isWildcard)
            {
                return array.Flatten();
            }

            var indexResult = Evaluate(keyExpr!);
            var index = (indexResult.Single() as XdmAtomicValue)?.AsInteger()
                ?? throw XdmException.TypeError("Array index must be integer");
            return array.Get((int)index);
        }

        throw new XdmException("XPTY0004", "Lookup requires map or array");
    }

    #endregion

    #region Constructors

    public XdmSequence VisitMapConstructor(MapConstructorExpr node)
    {
        var builder = XdmMap.Builder();

        foreach (var entry in node.Entries)
        {
            var key = Evaluate(entry.Key).Atomize().Single() as XdmAtomicValue
                ?? throw XdmException.TypeError("Map key must be atomic");
            var value = Evaluate(entry.Value);
            builder.Add(key, value);
        }

        return new XdmSequence(builder.Build());
    }

    public XdmSequence VisitArrayConstructor(ArrayConstructorExpr node)
    {
        if (node.IsCurly)
        {
            // Curly array: array { expr } - creates array from sequence
            if (node.Members.Count == 0)
                return new XdmSequence(XdmArray.Empty);

            var content = Evaluate(node.Members[0]);
            return new XdmSequence(XdmArray.FromSequence(content));
        }
        else
        {
            // Square bracket array: [a, b, c] - each expression is a member
            var builder = XdmArray.Builder();
            foreach (var member in node.Members)
            {
                builder.Add(Evaluate(member));
            }
            return new XdmSequence(builder.Build());
        }
    }

    public virtual XdmSequence VisitElementConstructor(ElementConstructorExpr node)
    {
        throw new NotImplementedException("Element constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitAttributeConstructor(AttributeConstructorExpr node)
    {
        throw new NotImplementedException("Attribute constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitTextConstructor(TextConstructorExpr node)
    {
        throw new NotImplementedException("Text constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitCommentConstructor(CommentConstructorExpr node)
    {
        throw new NotImplementedException("Comment constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitPIConstructor(PIConstructorExpr node)
    {
        throw new NotImplementedException("PI constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitDocumentConstructor(DocumentConstructorExpr node)
    {
        throw new NotImplementedException("Document constructor not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitComputedConstructor(ComputedConstructorExpr node)
    {
        throw new NotImplementedException("Computed constructor not implemented in XPath evaluator");
    }

    #endregion

    #region XQuery Module

    public virtual XdmSequence VisitModule(ModuleNode node)
    {
        throw new NotImplementedException("Module not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitProlog(PrologNode node)
    {
        throw new NotImplementedException("Prolog not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitFunctionDecl(FunctionDeclNode node)
    {
        throw new NotImplementedException("Function declaration not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitVariableDecl(VariableDeclNode node)
    {
        throw new NotImplementedException("Variable declaration not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitNamespaceDecl(NamespaceDeclNode node)
    {
        throw new NotImplementedException("Namespace declaration not implemented in XPath evaluator");
    }

    #endregion

    #region Try/Catch

    public XdmSequence VisitTryCatchExpr(TryCatchExpr node)
    {
        try
        {
            return Evaluate(node.TryExpr);
        }
        catch (XdmException ex)
        {
            foreach (var catchClause in node.CatchClauses)
            {
                // Check if error matches
                bool matches = catchClause.Errors.Any(e =>
                    e.LocalName == "*" || e.LocalName == ex.ErrorCode);

                if (matches)
                {
                    // Set error variables
                    var errorContext = _context.Clone();
                    errorContext.SetVariable("err:code", new XdmSequence(new XdmAtomicValue(ex.ErrorCode)));
                    errorContext.SetVariable("err:description", new XdmSequence(new XdmAtomicValue(ex.Message)));

                    var oldContext = _context;
                    _context = errorContext;
                    try
                    {
                        return Evaluate(catchClause.Handler);
                    }
                    finally
                    {
                        _context = oldContext;
                    }
                }
            }

            throw;
        }
    }

    #endregion

    #region XQuery Update

    public virtual XdmSequence VisitInsertExpr(InsertExpr node)
    {
        throw new NotImplementedException("Insert not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitDeleteExpr(DeleteExpr node)
    {
        throw new NotImplementedException("Delete not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitReplaceExpr(ReplaceExpr node)
    {
        throw new NotImplementedException("Replace not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitRenameExpr(RenameExpr node)
    {
        throw new NotImplementedException("Rename not implemented in XPath evaluator");
    }

    public virtual XdmSequence VisitTransformExpr(TransformExpr node)
    {
        throw new NotImplementedException("Transform not implemented in XPath evaluator");
    }

    #endregion

    #region Simple Map

    public XdmSequence VisitSimpleMapExpr(SimpleMapExpr node)
    {
        var result = _context.ContextItem != null
            ? new XdmSequence(_context.ContextItem)
            : XdmSequence.Empty;

        foreach (var step in node.Steps)
        {
            var newResult = new List<XdmItem>();
            var items = result.ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var stepContext = _context.WithContextItem(items[i], i + 1, items.Count);
                var oldContext = _context;
                _context = stepContext;

                try
                {
                    var stepResult = Evaluate(step);
                    newResult.AddRange(stepResult);
                }
                finally
                {
                    _context = oldContext;
                }
            }

            result = new XdmSequence(newResult);
        }

        return result;
    }

    #endregion

    #region Otherwise

    public XdmSequence VisitOtherwiseExpr(OtherwiseExpr node)
    {
        var left = Evaluate(node.Left);

        if (!left.IsEmpty)
            return left;

        return Evaluate(node.Right);
    }

    #endregion
}
