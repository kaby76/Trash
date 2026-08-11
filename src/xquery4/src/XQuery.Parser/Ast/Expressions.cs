using XQuery.DataModel;

namespace XQuery.Parser.Ast;

#region Literals

public class IntegerLiteralExpr : ExprNode
{
    public long Value { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIntegerLiteral(this);
}

public class DecimalLiteralExpr : ExprNode
{
    public decimal Value { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitDecimalLiteral(this);
}

public class DoubleLiteralExpr : ExprNode
{
    public double Value { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitDoubleLiteral(this);
}

public class StringLiteralExpr : ExprNode
{
    public string Value { get; set; } = string.Empty;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitStringLiteral(this);
}

#endregion

#region Primary Expressions

public class VariableRefExpr : ExprNode
{
    public string Name { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public string? NamespaceUri { get; set; }

    public XdmQName QName => string.IsNullOrEmpty(NamespaceUri)
        ? new XdmQName(Name)
        : new XdmQName(NamespaceUri, Name, Prefix ?? string.Empty);

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitVariableRef(this);
}

public class ContextItemExpr : ExprNode
{
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitContextItem(this);
}

public class FunctionCallExpr : ExprNode
{
    public string Name { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public string? NamespaceUri { get; set; }
    public List<ExprNode> Arguments { get; set; } = new();

    public XdmQName QName => string.IsNullOrEmpty(NamespaceUri)
        ? (string.IsNullOrEmpty(Prefix) ? new XdmQName(XdmQName.FnNamespace, Name, "fn") : new XdmQName(Name))
        : new XdmQName(NamespaceUri, Name, Prefix ?? string.Empty);

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFunctionCall(this);
}

public class InlineFunctionExpr : ExprNode
{
    public List<ParameterNode> Parameters { get; set; } = new();
    public SequenceTypeNode? ReturnType { get; set; }
    public ExprNode Body { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitInlineFunctionExpr(this);
}

public class ParameterNode : AstNode
{
    public string Name { get; set; } = string.Empty;
    public SequenceTypeNode? Type { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

public class SequenceTypeNode : AstNode
{
    public ItemTypeNode ItemType { get; set; } = null!;
    public OccurrenceIndicator Occurrence { get; set; } = OccurrenceIndicator.ExactlyOne;
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

public class ItemTypeNode : AstNode
{
    public ItemTypeKind Kind { get; set; }
    public XdmQName? TypeName { get; set; }
    public XdmQName? ElementName { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

public class NamedFunctionRefExpr : ExprNode
{
    public string Name { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public string? NamespaceUri { get; set; }
    public int Arity { get; set; }

    public XdmQName QName => string.IsNullOrEmpty(NamespaceUri)
        ? new XdmQName(Name)
        : new XdmQName(NamespaceUri, Name, Prefix ?? string.Empty);

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNamedFunctionRef(this);
}

public class ParenthesizedExpr : ExprNode
{
    public ExprNode? Inner { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitParenthesized(this);
}

#endregion

#region Path Expressions

public class PathExpr : ExprNode
{
    public bool IsAbsolute { get; set; }
    public bool IsRootOnly { get; set; }
    public List<ExprNode> Steps { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitPathExpr(this);
}

public enum Axis
{
    Child,
    Descendant,
    Attribute,
    Self,
    DescendantOrSelf,
    FollowingSibling,
    Following,
    Parent,
    Ancestor,
    PrecedingSibling,
    Preceding,
    AncestorOrSelf,
    Namespace
}

public class AxisStepExpr : ExprNode
{
    public Axis Axis { get; set; } = Axis.Child;
    public ExprNode NodeTest { get; set; } = null!;
    public List<ExprNode> Predicates { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitAxisStep(this);
}

public class FilterExpr : ExprNode
{
    public ExprNode Primary { get; set; } = null!;
    public List<ExprNode> Predicates { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFilterExpr(this);
}

public class PredicateListExpr : ExprNode
{
    public List<ExprNode> Predicates { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitPredicateList(this);
}

public class NameTestExpr : ExprNode
{
    public string? Prefix { get; set; }
    public string LocalName { get; set; } = string.Empty;
    public string? NamespaceUri { get; set; }
    public bool IsWildcard { get; set; }
    public bool IsPrefixWildcard { get; set; }
    public bool IsLocalWildcard { get; set; }

    public XdmQName? QName => IsWildcard ? null :
        string.IsNullOrEmpty(NamespaceUri) ? new XdmQName(LocalName) :
        new XdmQName(NamespaceUri, LocalName, Prefix ?? string.Empty);

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNameTest(this);
}

public class KindTestExpr : ExprNode
{
    public XdmNodeKind Kind { get; set; }
    public XdmQName? Name { get; set; }
    public XdmQName? TypeName { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitKindTest(this);
}

#endregion

#region Operators

public enum BinaryOperator
{
    Add, Subtract, Multiply, Divide, IntegerDivide, Modulo,
    And, Or,
    Union, Intersect, Except
}

public class BinaryExpr : ExprNode
{
    public ExprNode Left { get; set; } = null!;
    public BinaryOperator Operator { get; set; }
    public ExprNode Right { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
}

public enum UnaryOperator
{
    Plus, Minus
}

public class UnaryExpr : ExprNode
{
    public UnaryOperator Operator { get; set; }
    public ExprNode Operand { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
}

public enum ComparisonOperator
{
    // Value comparisons
    Eq, Ne, Lt, Le, Gt, Ge,
    // General comparisons
    Equal, NotEqual, LessThan, LessOrEqual, GreaterThan, GreaterOrEqual,
    // Node comparisons
    Is, Precedes, Follows
}

public class ComparisonExpr : ExprNode
{
    public ExprNode Left { get; set; } = null!;
    public ComparisonOperator Operator { get; set; }
    public ExprNode Right { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitComparisonExpr(this);
}

public class RangeExpr : ExprNode
{
    public ExprNode Start { get; set; } = null!;
    public ExprNode End { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitRangeExpr(this);
}

public class ConcatExpr : ExprNode
{
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitConcatExpr(this);
}

#endregion

#region Sequence Expressions

public class SequenceExpr : ExprNode
{
    public List<ExprNode> Items { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitSequenceExpr(this);
}

public class UnionExpr : ExprNode
{
    public List<ExprNode> Operands { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitUnionExpr(this);
}

public class IntersectExceptExpr : ExprNode
{
    public ExprNode Left { get; set; } = null!;
    public bool IsIntersect { get; set; }
    public ExprNode Right { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIntersectExceptExpr(this);
}

#endregion

#region Conditional

public class IfExpr : ExprNode
{
    public ExprNode Condition { get; set; } = null!;
    public ExprNode Then { get; set; } = null!;
    public ExprNode Else { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitIfExpr(this);
}

public class SwitchExpr : ExprNode
{
    public ExprNode Operand { get; set; } = null!;
    public List<SwitchCaseClause> Cases { get; set; } = new();
    public ExprNode Default { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitSwitchExpr(this);
}

public class SwitchCaseClause
{
    public List<ExprNode> Values { get; set; } = new();
    public ExprNode Result { get; set; } = null!;
}

#endregion

#region FLWOR

public class FlworExpr : ExprNode
{
    public List<FlworClause> Clauses { get; set; } = new();
    public ExprNode Return { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFlworExpr(this);
}

public abstract class FlworClause : AstNode
{
}

public class ForClause : FlworClause
{
    public string Variable { get; set; } = string.Empty;
    public string? PositionalVariable { get; set; }
    public SequenceTypeNode? Type { get; set; }
    public bool AllowingEmpty { get; set; }
    public ExprNode Expression { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitForClause(this);
}

public class LetClause : FlworClause
{
    public string Variable { get; set; } = string.Empty;
    public SequenceTypeNode? Type { get; set; }
    public ExprNode Expression { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitLetClause(this);
}

public class WhereClause : FlworClause
{
    public ExprNode Condition { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitWhereClause(this);
}

public class OrderByClause : FlworClause
{
    public bool Stable { get; set; }
    public List<OrderSpec> Specs { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitOrderByClause(this);
}

public class OrderSpec
{
    public ExprNode Expression { get; set; } = null!;
    public bool Descending { get; set; }
    public bool EmptyGreatest { get; set; }
    public string? Collation { get; set; }
}

public class GroupByClause : FlworClause
{
    public List<GroupSpec> Specs { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitGroupByClause(this);
}

public class GroupSpec
{
    public string Variable { get; set; } = string.Empty;
    public SequenceTypeNode? Type { get; set; }
    public ExprNode? Expression { get; set; }
    public string? Collation { get; set; }
}

public class CountClause : FlworClause
{
    public string Variable { get; set; } = string.Empty;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCountClause(this);
}

#endregion

#region Quantified

public class QuantifiedExpr : ExprNode
{
    public bool IsSome { get; set; }
    public List<QuantifiedBinding> Bindings { get; set; } = new();
    public ExprNode Satisfies { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitQuantifiedExpr(this);
}

public class QuantifiedBinding
{
    public string Variable { get; set; } = string.Empty;
    public SequenceTypeNode? Type { get; set; }
    public ExprNode Expression { get; set; } = null!;
}

#endregion

#region Type Expressions

public class InstanceOfExpr : ExprNode
{
    public ExprNode Expression { get; set; } = null!;
    public SequenceTypeNode Type { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitInstanceOfExpr(this);
}

public class TreatExpr : ExprNode
{
    public ExprNode Expression { get; set; } = null!;
    public SequenceTypeNode Type { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitTreatExpr(this);
}

public class CastableExpr : ExprNode
{
    public ExprNode Expression { get; set; } = null!;
    public XdmQName TargetType { get; set; } = null!;
    public bool AllowEmpty { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCastableExpr(this);
}

public class CastExpr : ExprNode
{
    public ExprNode Expression { get; set; } = null!;
    public XdmQName TargetType { get; set; } = null!;
    public bool AllowEmpty { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCastExpr(this);
}

#endregion

#region Arrow/Lookup

public class ArrowExpr : ExprNode
{
    public ExprNode Argument { get; set; } = null!;
    public ExprNode Function { get; set; } = null!;
    public List<ExprNode> AdditionalArguments { get; set; } = new();
    public bool IsThinArrow { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitArrowExpr(this);
}

public class LookupExpr : ExprNode
{
    public ExprNode? KeyExpr { get; set; }
    public bool IsWildcard { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitLookupExpr(this);
}

public class PostfixLookupExpr : ExprNode
{
    public ExprNode Base { get; set; } = null!;
    public ExprNode? KeyExpr { get; set; }
    public bool IsWildcard { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitPostfixLookupExpr(this);
}

#endregion

#region Constructors

public class MapConstructorExpr : ExprNode
{
    public List<MapEntry> Entries { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitMapConstructor(this);
}

public class MapEntry
{
    public ExprNode Key { get; set; } = null!;
    public ExprNode Value { get; set; } = null!;
}

public class ArrayConstructorExpr : ExprNode
{
    public List<ExprNode> Members { get; set; } = new();
    public bool IsCurly { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitArrayConstructor(this);
}

public class ElementConstructorExpr : ExprNode
{
    public XdmQName? Name { get; set; }
    public ExprNode? NameExpr { get; set; }
    public List<ExprNode> Content { get; set; } = new();
    public bool IsComputed { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitElementConstructor(this);
}

public class AttributeConstructorExpr : ExprNode
{
    public XdmQName? Name { get; set; }
    public ExprNode? NameExpr { get; set; }
    public ExprNode? Value { get; set; }
    public bool IsComputed { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitAttributeConstructor(this);
}

public class TextConstructorExpr : ExprNode
{
    public ExprNode Content { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitTextConstructor(this);
}

public class CommentConstructorExpr : ExprNode
{
    public ExprNode Content { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitCommentConstructor(this);
}

public class PIConstructorExpr : ExprNode
{
    public string? Target { get; set; }
    public ExprNode? TargetExpr { get; set; }
    public ExprNode? Content { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitPIConstructor(this);
}

public class DocumentConstructorExpr : ExprNode
{
    public ExprNode Content { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitDocumentConstructor(this);
}

public class ComputedConstructorExpr : ExprNode
{
    public string ConstructorType { get; set; } = string.Empty;
    public ExprNode? Name { get; set; }
    public ExprNode? Content { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitComputedConstructor(this);
}

#endregion

#region XQuery Module

public class ModuleNode : AstNode
{
    public string? ModuleNamespace { get; set; }
    public string? ModulePrefix { get; set; }
    public PrologNode Prolog { get; set; } = new();
    public ExprNode? Body { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitModule(this);
}

public class PrologNode : AstNode
{
    public List<NamespaceDeclNode> NamespaceDecls { get; set; } = new();
    public List<ImportNode> Imports { get; set; } = new();
    public List<VariableDeclNode> VariableDecls { get; set; } = new();
    public List<FunctionDeclNode> FunctionDecls { get; set; } = new();
    public List<OptionDeclNode> OptionDecls { get; set; } = new();
    public string? DefaultElementNamespace { get; set; }
    public string? DefaultFunctionNamespace { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitProlog(this);
}

public class NamespaceDeclNode : AstNode
{
    public string Prefix { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNamespaceDecl(this);
}

public class ImportNode : AstNode
{
    public bool IsSchema { get; set; }
    public string? Prefix { get; set; }
    public string Namespace { get; set; } = string.Empty;
    public List<string> LocationHints { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

public class VariableDeclNode : AstNode
{
    public string Name { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public SequenceTypeNode? Type { get; set; }
    public ExprNode? Value { get; set; }
    public bool IsExternal { get; set; }
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitVariableDecl(this);
}

public class FunctionDeclNode : AstNode
{
    public string Name { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public List<ParameterNode> Parameters { get; set; } = new();
    public SequenceTypeNode? ReturnType { get; set; }
    public ExprNode? Body { get; set; }
    public bool IsExternal { get; set; }
    public List<AnnotationNode> Annotations { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitFunctionDecl(this);
}

public class AnnotationNode : AstNode
{
    public XdmQName Name { get; set; } = null!;
    public List<object> Values { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

public class OptionDeclNode : AstNode
{
    public XdmQName Name { get; set; } = null!;
    public string Value { get; set; } = string.Empty;
    public override T Accept<T>(IAstVisitor<T> visitor) => throw new NotImplementedException();
}

#endregion

#region Try/Catch

public class TryCatchExpr : ExprNode
{
    public ExprNode TryExpr { get; set; } = null!;
    public List<CatchClause> CatchClauses { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitTryCatchExpr(this);
}

public class CatchClause
{
    public List<XdmQName> Errors { get; set; } = new();
    public ExprNode Handler { get; set; } = null!;
}

#endregion

#region XQuery Update

public class InsertExpr : ExprNode
{
    public ExprNode Source { get; set; } = null!;
    public InsertPosition Position { get; set; }
    public ExprNode Target { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitInsertExpr(this);
}

public enum InsertPosition
{
    Into,
    AsFirst,
    AsLast,
    After,
    Before
}

public class DeleteExpr : ExprNode
{
    public ExprNode Target { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitDeleteExpr(this);
}

public class ReplaceExpr : ExprNode
{
    public bool ValueOf { get; set; }
    public ExprNode Target { get; set; } = null!;
    public ExprNode Replacement { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitReplaceExpr(this);
}

public class RenameExpr : ExprNode
{
    public ExprNode Target { get; set; } = null!;
    public ExprNode NewName { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitRenameExpr(this);
}

public class TransformExpr : ExprNode
{
    public List<CopyBinding> CopyBindings { get; set; } = new();
    public ExprNode ModifyExpr { get; set; } = null!;
    public ExprNode ReturnExpr { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitTransformExpr(this);
}

public class CopyBinding
{
    public string Variable { get; set; } = string.Empty;
    public ExprNode Expression { get; set; } = null!;
}

#endregion

#region Simple Map

public class SimpleMapExpr : ExprNode
{
    public List<ExprNode> Steps { get; set; } = new();
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitSimpleMapExpr(this);
}

#endregion

#region Otherwise

public class OtherwiseExpr : ExprNode
{
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitOtherwiseExpr(this);
}

#endregion
