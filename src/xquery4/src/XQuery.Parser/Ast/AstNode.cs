namespace XQuery.Parser.Ast;

/// <summary>
/// Base class for all AST nodes.
/// </summary>
public abstract class AstNode
{
    /// <summary>
    /// Line number where this node starts.
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Column number where this node starts.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Accept a visitor.
    /// </summary>
    public abstract T Accept<T>(IAstVisitor<T> visitor);
}

/// <summary>
/// Visitor interface for AST nodes.
/// </summary>
public interface IAstVisitor<T>
{
    // Literals
    T VisitIntegerLiteral(IntegerLiteralExpr node);
    T VisitDecimalLiteral(DecimalLiteralExpr node);
    T VisitDoubleLiteral(DoubleLiteralExpr node);
    T VisitStringLiteral(StringLiteralExpr node);

    // Primary expressions
    T VisitVariableRef(VariableRefExpr node);
    T VisitContextItem(ContextItemExpr node);
    T VisitFunctionCall(FunctionCallExpr node);
    T VisitInlineFunctionExpr(InlineFunctionExpr node);
    T VisitNamedFunctionRef(NamedFunctionRefExpr node);
    T VisitParenthesized(ParenthesizedExpr node);

    // Path expressions
    T VisitPathExpr(PathExpr node);
    T VisitAxisStep(AxisStepExpr node);
    T VisitFilterExpr(FilterExpr node);
    T VisitPredicateList(PredicateListExpr node);

    // Operators
    T VisitBinaryExpr(BinaryExpr node);
    T VisitUnaryExpr(UnaryExpr node);
    T VisitComparisonExpr(ComparisonExpr node);
    T VisitRangeExpr(RangeExpr node);
    T VisitConcatExpr(ConcatExpr node);

    // Sequence expressions
    T VisitSequenceExpr(SequenceExpr node);
    T VisitUnionExpr(UnionExpr node);
    T VisitIntersectExceptExpr(IntersectExceptExpr node);

    // Conditional
    T VisitIfExpr(IfExpr node);
    T VisitSwitchExpr(SwitchExpr node);

    // FLWOR
    T VisitFlworExpr(FlworExpr node);
    T VisitForClause(ForClause node);
    T VisitLetClause(LetClause node);
    T VisitWhereClause(WhereClause node);
    T VisitOrderByClause(OrderByClause node);
    T VisitGroupByClause(GroupByClause node);
    T VisitCountClause(CountClause node);

    // Quantified
    T VisitQuantifiedExpr(QuantifiedExpr node);

    // Type expressions
    T VisitInstanceOfExpr(InstanceOfExpr node);
    T VisitTreatExpr(TreatExpr node);
    T VisitCastableExpr(CastableExpr node);
    T VisitCastExpr(CastExpr node);

    // Arrow/Lookup
    T VisitArrowExpr(ArrowExpr node);
    T VisitLookupExpr(LookupExpr node);
    T VisitPostfixLookupExpr(PostfixLookupExpr node);

    // Constructors
    T VisitMapConstructor(MapConstructorExpr node);
    T VisitArrayConstructor(ArrayConstructorExpr node);
    T VisitElementConstructor(ElementConstructorExpr node);
    T VisitAttributeConstructor(AttributeConstructorExpr node);
    T VisitTextConstructor(TextConstructorExpr node);
    T VisitCommentConstructor(CommentConstructorExpr node);
    T VisitPIConstructor(PIConstructorExpr node);
    T VisitDocumentConstructor(DocumentConstructorExpr node);
    T VisitComputedConstructor(ComputedConstructorExpr node);

    // Node tests
    T VisitNameTest(NameTestExpr node);
    T VisitKindTest(KindTestExpr node);

    // XQuery module
    T VisitModule(ModuleNode node);
    T VisitProlog(PrologNode node);
    T VisitFunctionDecl(FunctionDeclNode node);
    T VisitVariableDecl(VariableDeclNode node);
    T VisitNamespaceDecl(NamespaceDeclNode node);

    // Try/Catch
    T VisitTryCatchExpr(TryCatchExpr node);

    // XQuery Update
    T VisitInsertExpr(InsertExpr node);
    T VisitDeleteExpr(DeleteExpr node);
    T VisitReplaceExpr(ReplaceExpr node);
    T VisitRenameExpr(RenameExpr node);
    T VisitTransformExpr(TransformExpr node);

    // Simple map
    T VisitSimpleMapExpr(SimpleMapExpr node);

    // Otherwise
    T VisitOtherwiseExpr(OtherwiseExpr node);
}

/// <summary>
/// Base class for expression AST nodes.
/// </summary>
public abstract class ExprNode : AstNode
{
}
