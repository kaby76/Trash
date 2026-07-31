using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using XQuery.DataModel;
using XQuery.Parser.Ast;

namespace XQuery.Parser;

/// <summary>
/// XPath 4.0 parser using the ANTLR4-generated recognizer.
/// Keeps the same public API as the old hand-written parser.
/// </summary>
public class XPathParser
{
    private readonly string _input;
    private readonly Dictionary<string, string> _namespaces = new();

    public XPathParser(string input)
    {
        _input = input;
        _namespaces["xs"] = XdmQName.XsNamespace;
        _namespaces["fn"] = XdmQName.FnNamespace;
        _namespaces["map"] = XdmQName.MapNamespace;
        _namespaces["array"] = XdmQName.ArrayNamespace;
        _namespaces["math"] = XdmQName.MathNamespace;
        _namespaces["xml"] = XdmQName.XmlNamespace;
    }

    public void AddNamespace(string prefix, string uri)
    {
        _namespaces[prefix] = uri;
    }

    public ExprNode Parse()
    {
        var stream = new AntlrInputStream(_input);
        var lexer = new XPath4Lexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new XPath4Parser(tokens);

        var errors = new List<string>();
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new LexerErrorCollector(errors));
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new SyntaxErrorCollector(errors));

        var tree = parser.xPath();

        if (errors.Count > 0)
            throw new XPathParseException(errors[0]);

        return new AstBuilder(_namespaces).Build(tree);
    }

    // Parser error listener (offending symbol = IToken)
    private sealed class SyntaxErrorCollector : BaseErrorListener
    {
        private readonly List<string> _errors;
        public SyntaxErrorCollector(List<string> errors) => _errors = errors;
        public override void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer,
            IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            => _errors.Add($"Line {line}:{charPositionInLine} {msg}");
    }

    // Lexer error listener (offending symbol = int token type)
    private sealed class LexerErrorCollector : IAntlrErrorListener<int>
    {
        private readonly List<string> _errors;
        public LexerErrorCollector(List<string> errors) => _errors = errors;
        public void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer,
            int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            => _errors.Add($"Line {line}:{charPositionInLine} {msg}");
    }

    private sealed class AstBuilder
    {
        private readonly Dictionary<string, string> _ns;

        public AstBuilder(Dictionary<string, string> namespaces) => _ns = namespaces;

        public ExprNode Build(XPath4Parser.XPathContext ctx) => BuildExpr(ctx.expr());

        // ── Expr ─────────────────────────────────────────────────────────

        private ExprNode BuildExpr(XPath4Parser.ExprContext ctx)
        {
            var singles = ctx.exprSingle();
            if (singles.Length == 1)
                return BuildExprSingle(singles[0]);
            return new SequenceExpr
            {
                Items = singles.Select(BuildExprSingle).ToList(),
                Line = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        private ExprNode BuildExprSingle(XPath4Parser.ExprSingleContext ctx)
        {
            if (ctx.forExpr()       is { } fe) return BuildForExpr(fe);
            if (ctx.letExpr()       is { } le) return BuildLetExpr(le);
            if (ctx.quantifiedExpr() is { } qe) return BuildQuantifiedExpr(qe);
            if (ctx.ifExpr()        is { } ie) return BuildIfExpr(ie);
            return BuildOrExpr(ctx.orExpr()!);
        }

        // ── FLWOR ────────────────────────────────────────────────────────

        private ExprNode BuildForExpr(XPath4Parser.ForExprContext ctx)
        {
            var clauses = new List<FlworClause>();
            BuildForClause(ctx.forClause(), clauses);
            var ret = BuildForLetReturn(ctx.forLetReturn(), clauses);
            return new FlworExpr { Clauses = clauses, Return = ret, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private void BuildForClause(XPath4Parser.ForClauseContext ctx, List<FlworClause> clauses)
        {
            foreach (var binding in ctx.forBinding())
            {
                if (binding.forItemBinding() is { } fb)
                {
                    var (name, _, seqType) = GetVarNameAndType(fb.varNameAndType());
                    var posVar = fb.positionalVar()?.eqName()?.GetText();
                    clauses.Add(new ForClause
                    {
                        Variable = name, PositionalVariable = posVar, Type = seqType,
                        Expression = BuildExprSingle(fb.exprSingle()),
                        Line = ctx.Start.Line, Column = ctx.Start.Column
                    });
                }
                else if (binding.forMemberBinding() is { } mb)
                {
                    var (name, _, seqType) = GetVarNameAndType(mb.varNameAndType());
                    clauses.Add(new ForClause
                    {
                        Variable = name, Type = seqType,
                        Expression = BuildExprSingle(mb.exprSingle()),
                        Line = ctx.Start.Line, Column = ctx.Start.Column
                    });
                }
                else if (binding.forEntryBinding() is { } eb)
                {
                    var valVnt = eb.forEntryValueBinding()?.varNameAndType();
                    var (name, _, seqType) = valVnt != null ? GetVarNameAndType(valVnt) : ("_", null, null);
                    clauses.Add(new ForClause
                    {
                        Variable = name, Type = seqType,
                        Expression = BuildExprSingle(eb.exprSingle()),
                        Line = ctx.Start.Line, Column = ctx.Start.Column
                    });
                }
            }
        }

        private ExprNode BuildForLetReturn(XPath4Parser.ForLetReturnContext ctx, List<FlworClause> clauses)
        {
            if (ctx.forClause() is { } fc)
            {
                BuildForClause(fc, clauses);
                return BuildForLetReturn(ctx.forLetReturn()!, clauses);
            }
            if (ctx.letClause() is { } lc)
            {
                BuildLetClause(lc, clauses);
                return BuildForLetReturn(ctx.forLetReturn()!, clauses);
            }
            return BuildExprSingle(ctx.exprSingle()!);
        }

        private ExprNode BuildLetExpr(XPath4Parser.LetExprContext ctx)
        {
            var clauses = new List<FlworClause>();
            BuildLetClause(ctx.letClause(), clauses);
            var ret = BuildForLetReturn(ctx.forLetReturn(), clauses);
            return new FlworExpr { Clauses = clauses, Return = ret, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private void BuildLetClause(XPath4Parser.LetClauseContext ctx, List<FlworClause> clauses)
        {
            foreach (var binding in ctx.letBinding())
            {
                if (binding.letValueBinding() is { } lvb)
                {
                    var (name, _, seqType) = GetVarNameAndType(lvb.varNameAndType());
                    clauses.Add(new LetClause
                    {
                        Variable = name, Type = seqType,
                        Expression = BuildExprSingle(lvb.exprSingle()),
                        Line = ctx.Start.Line, Column = ctx.Start.Column
                    });
                }
                else
                {
                    throw new XPathParseException(
                        $"Destructuring let bindings are not supported (line {ctx.Start.Line})");
                }
            }
        }

        // ── Quantified ───────────────────────────────────────────────────

        private ExprNode BuildQuantifiedExpr(XPath4Parser.QuantifiedExprContext ctx)
        {
            bool isSome = ctx.KW_SOME() != null;
            var bindings = ctx.quantifierBinding().Select(qb =>
            {
                var (name, _, seqType) = GetVarNameAndType(qb.varNameAndType());
                return new QuantifiedBinding
                {
                    Variable = name, Type = seqType,
                    Expression = BuildExprSingle(qb.exprSingle())
                };
            }).ToList();
            return new QuantifiedExpr
            {
                IsSome = isSome, Bindings = bindings,
                Satisfies = BuildExprSingle(ctx.exprSingle()),
                Line = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        // ── If ───────────────────────────────────────────────────────────

        private ExprNode BuildIfExpr(XPath4Parser.IfExprContext ctx)
        {
            var cond = BuildExpr(ctx.expr());
            ExprNode then, else_;
            if (ctx.unbracedActions() is { } ub)
            {
                var singles = ub.exprSingle();
                then  = BuildExprSingle(singles[0]);
                else_ = BuildExprSingle(singles[1]);
            }
            else
            {
                var inner = ctx.bracedAction()!.enclosedExpr().expr();
                then  = inner != null ? BuildExpr(inner) : new SequenceExpr { Items = [] };
                else_ = new SequenceExpr { Items = [] };
            }
            return new IfExpr { Condition = cond, Then = then, Else = else_, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        // ── Boolean operators ─────────────────────────────────────────────

        private ExprNode BuildOrExpr(XPath4Parser.OrExprContext ctx)
        {
            var ops = ctx.andExpr();
            var left = BuildAndExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
            {
                var right = BuildAndExpr(ops[i]);
                left = new BinaryExpr { Left = left, Operator = BinaryOperator.Or, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildAndExpr(XPath4Parser.AndExprContext ctx)
        {
            var ops = ctx.comparisonExpr();
            var left = BuildComparisonExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
            {
                var right = BuildComparisonExpr(ops[i]);
                left = new BinaryExpr { Left = left, Operator = BinaryOperator.And, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildComparisonExpr(XPath4Parser.ComparisonExprContext ctx)
        {
            var ops = ctx.otherwiseExpr();
            var left = BuildOtherwiseExpr(ops[0]);
            if (ops.Length == 1) return left;
            var right = BuildOtherwiseExpr(ops[1]);
            ComparisonOperator op;
            if      (ctx.valueComp()   is { } vc) op = GetValueCompOp(vc);
            else if (ctx.generalComp() is { } gc) op = GetGeneralCompOp(gc);
            else                                  op = GetNodeCompOp(ctx.nodeComp()!);
            return new ComparisonExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
        }

        // ── Otherwise / string-concat / range ────────────────────────────

        private ExprNode BuildOtherwiseExpr(XPath4Parser.OtherwiseExprContext ctx)
        {
            var ops = ctx.stringConcatExpr();
            var left = BuildStringConcatExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
            {
                var right = BuildStringConcatExpr(ops[i]);
                left = new OtherwiseExpr { Left = left, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildStringConcatExpr(XPath4Parser.StringConcatExprContext ctx)
        {
            var ops = ctx.rangeExpr();
            var left = BuildRangeExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
            {
                var right = BuildRangeExpr(ops[i]);
                left = new ConcatExpr { Left = left, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildRangeExpr(XPath4Parser.RangeExprContext ctx)
        {
            var ops = ctx.additiveExpr();
            var left = BuildAdditiveExpr(ops[0]);
            if (ops.Length == 1) return left;
            return new RangeExpr { Start = left, End = BuildAdditiveExpr(ops[1]), Line = left.Line, Column = left.Column };
        }

        // ── Arithmetic ────────────────────────────────────────────────────

        private ExprNode BuildAdditiveExpr(XPath4Parser.AdditiveExprContext ctx)
        {
            var operands = ctx.multiplicativeExpr();
            var left = BuildMultiplicativeExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right = BuildMultiplicativeExpr(operands[i]);
                var op = opToken?.Symbol.Type == XPath4Lexer.PLUS ? BinaryOperator.Add : BinaryOperator.Subtract;
                left = new BinaryExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildMultiplicativeExpr(XPath4Parser.MultiplicativeExprContext ctx)
        {
            var operands = ctx.unionExpr();
            var left = BuildUnionExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right = BuildUnionExpr(operands[i]);
                int tt = opToken?.Symbol.Type ?? -1;
                BinaryOperator op =
                    tt == XPath4Lexer.STAR       || tt == XPath4Lexer.TIMES_SIGN ? BinaryOperator.Multiply :
                    tt == XPath4Lexer.KW_DIV     || tt == XPath4Lexer.DIV_SIGN   ? BinaryOperator.Divide :
                    tt == XPath4Lexer.KW_IDIV                                    ? BinaryOperator.IntegerDivide :
                                                                                    BinaryOperator.Modulo;
                left = new BinaryExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        // ── Union / intersect / except ────────────────────────────────────

        private ExprNode BuildUnionExpr(XPath4Parser.UnionExprContext ctx)
        {
            var ops = ctx.intersectExceptExpr();
            if (ops.Length == 1) return BuildIntersectExceptExpr(ops[0]);
            return new UnionExpr
            {
                Operands = ops.Select(BuildIntersectExceptExpr).ToList(),
                Line = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        private ExprNode BuildIntersectExceptExpr(XPath4Parser.IntersectExceptExprContext ctx)
        {
            var operands = ctx.recordPutExpr();
            var left = BuildRecordPutExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right   = BuildRecordPutExpr(operands[i]);
                bool isIntersect = opToken?.Symbol.Type == XPath4Lexer.KW_INTERSECT;
                left = new IntersectExceptExpr { Left = left, IsIntersect = isIntersect, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildRecordPutExpr(XPath4Parser.RecordPutExprContext ctx)
        {
            var ops = ctx.instanceofExpr();
            if (ops.Length > 1)
                throw new XPathParseException($"Record-put operator (+:=) is not supported (line {ctx.Start.Line})");
            return BuildInstanceOfExpr(ops[0]);
        }

        // ── Type expressions ─────────────────────────────────────────────

        private ExprNode BuildInstanceOfExpr(XPath4Parser.InstanceofExprContext ctx)
        {
            var expr = BuildTreatExpr(ctx.treatExpr());
            if (ctx.sequenceType() is { } st)
                return new InstanceOfExpr { Expression = expr, Type = BuildSequenceType(st), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildTreatExpr(XPath4Parser.TreatExprContext ctx)
        {
            var expr = BuildCastableExpr(ctx.castableExpr());
            if (ctx.sequenceType() is { } st)
                return new TreatExpr { Expression = expr, Type = BuildSequenceType(st), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildCastableExpr(XPath4Parser.CastableExprContext ctx)
        {
            var expr = BuildCastExpr(ctx.castExpr());
            if (ctx.castTarget() is { } ct)
            {
                var (qname, allowEmpty) = BuildCastTarget(ct, ctx.occurrenceIndicator());
                return new CastableExpr { Expression = expr, TargetType = qname, AllowEmpty = allowEmpty, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            return expr;
        }

        private ExprNode BuildCastExpr(XPath4Parser.CastExprContext ctx)
        {
            var expr = BuildPipelineExpr(ctx.pipelineExpr());
            if (ctx.castTarget() is { } ct)
            {
                var (qname, allowEmpty) = BuildCastTarget(ct, ctx.occurrenceIndicator());
                return new CastExpr { Expression = expr, TargetType = qname, AllowEmpty = allowEmpty, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            return expr;
        }

        private ExprNode BuildPipelineExpr(XPath4Parser.PipelineExprContext ctx)
        {
            var arrowExprs = ctx.arrowExpr();
            var result = BuildArrowExpr(arrowExprs[0]);
            for (int i = 1; i < arrowExprs.Length; i++)
            {
                var rhs = BuildArrowExpr(arrowExprs[i]);
                result = new ArrowExpr { Argument = result, Function = rhs, AdditionalArguments = [], IsThinArrow = true, Line = result.Line, Column = result.Column };
            }
            return result;
        }

        private (XdmQName qname, bool allowEmpty) BuildCastTarget(
            XPath4Parser.CastTargetContext ctx, XPath4Parser.OccurrenceIndicatorContext? occ)
        {
            if (ctx.typeName_() is { } tn)
                return (ResolveEqName(tn.eqName()), occ?.QM() != null);
            throw new XPathParseException($"Complex cast targets are not supported (line {ctx.Start.Line})");
        }

        // ── Arrow expression ──────────────────────────────────────────────

        private ExprNode BuildArrowExpr(XPath4Parser.ArrowExprContext ctx)
        {
            var result = BuildUnaryExpr(ctx.unaryExpr());
            foreach (var child in ctx.children)
            {
                if (child is XPath4Parser.SequenceArrowTargetContext sat)
                    result = BuildArrowTarget(sat.arrowTarget(), result, isThinArrow: false);
                else if (child is XPath4Parser.MappingArrowTargetContext mat)
                    result = BuildArrowTarget(mat.arrowTarget(), result, isThinArrow: true);
            }
            return result;
        }

        private ExprNode BuildArrowTarget(XPath4Parser.ArrowTargetContext ctx, ExprNode arg, bool isThinArrow)
        {
            if (ctx.functionCall() is { } fc)
            {
                var qname = ResolveEqName(fc.eqName());
                var addlArgs = BuildArgumentListPositional(fc.argumentList());
                var funcNode = new FunctionCallExpr
                {
                    Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri,
                    Arguments = [],
                    Line = ctx.Start.Line, Column = ctx.Start.Column
                };
                return new ArrowExpr
                {
                    Argument = arg, Function = funcNode, AdditionalArguments = addlArgs,
                    IsThinArrow = isThinArrow,
                    Line = ctx.Start.Line, Column = ctx.Start.Column
                };
            }
            throw new XPathParseException($"Dynamic arrow calls are not supported (line {ctx.Start.Line})");
        }

        // ── Unary / simple-map / path ─────────────────────────────────────

        private ExprNode BuildUnaryExpr(XPath4Parser.UnaryExprContext ctx)
        {
            var expr = BuildSimpleMapExpr(ctx.valueExpr().simpleMapExpr());
            int minusCount = 0;
            bool hasPlus = false;
            foreach (var child in ctx.children)
            {
                if (child is ITerminalNode tn)
                {
                    if (tn.Symbol.Type == XPath4Lexer.MINUS) minusCount++;
                    else if (tn.Symbol.Type == XPath4Lexer.PLUS) hasPlus = true;
                }
            }
            if (minusCount % 2 == 1)
                return new UnaryExpr { Operator = UnaryOperator.Minus, Operand = expr, Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (hasPlus && minusCount == 0)
                return new UnaryExpr { Operator = UnaryOperator.Plus, Operand = expr, Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildSimpleMapExpr(XPath4Parser.SimpleMapExprContext ctx)
        {
            var ops = ctx.pathExpr();
            if (ops.Length == 1) return BuildPathExpr(ops[0]);
            return new SimpleMapExpr { Steps = ops.Select(BuildPathExpr).ToList(), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildPathExpr(XPath4Parser.PathExprContext ctx)
        {
            if (ctx.absolutePathExpr() is { } ape) return BuildAbsolutePathExpr(ape);
            return BuildRelativePathExpr(ctx.relativePathExpr()!);
        }

        private static readonly KindTestExpr AnyNodeTest = new() { Kind = XdmNodeKind.Element };

        private ExprNode BuildAbsolutePathExpr(XPath4Parser.AbsolutePathExprContext ctx)
        {
            var rel = ctx.relativePathExpr();
            if (rel == null)
                return new PathExpr { IsAbsolute = true, IsRootOnly = true, Steps = [], Line = ctx.Start.Line, Column = ctx.Start.Column };

            var inner = BuildRelativePathExprSteps(rel);
            bool isDescendant = ctx.SS() != null;
            if (isDescendant)
                inner.Insert(0, new AxisStepExpr { Axis = Axis.DescendantOrSelf, NodeTest = AnyNodeTest, Line = ctx.Start.Line, Column = ctx.Start.Column });
            return new PathExpr { IsAbsolute = true, Steps = inner, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildRelativePathExpr(XPath4Parser.RelativePathExprContext ctx)
        {
            var steps = BuildRelativePathExprSteps(ctx);
            if (steps.Count == 1) return steps[0];
            return new PathExpr { Steps = steps, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private List<ExprNode> BuildRelativePathExprSteps(XPath4Parser.RelativePathExprContext ctx)
        {
            var result = new List<ExprNode>();
            foreach (var child in ctx.children)
            {
                if (child is XPath4Parser.StepExprContext se)
                    result.Add(BuildStepExpr(se));
                else if (child is ITerminalNode tn && tn.Symbol.Type == XPath4Lexer.SS)
                    result.Add(new AxisStepExpr { Axis = Axis.DescendantOrSelf, NodeTest = AnyNodeTest, Line = tn.Symbol.Line, Column = tn.Symbol.Column });
            }
            return result;
        }

        private ExprNode BuildStepExpr(XPath4Parser.StepExprContext ctx)
        {
            if (ctx.axisStep()   is { } as_) return BuildAxisStep(as_);
            return BuildPostfixExpr(ctx.postfixExpr()!);
        }

        // ── Axis step ─────────────────────────────────────────────────────

        private ExprNode BuildAxisStep(XPath4Parser.AxisStepContext ctx)
        {
            Axis axis;
            ExprNode nodeTest;

            if (ctx.abbreviatedStep() is { } abr)
            {
                if (abr.DD() != null)
                {
                    axis = Axis.Parent;
                    nodeTest = AnyNodeTest;
                }
                else if (abr.AT() != null)
                {
                    axis = Axis.Attribute;
                    nodeTest = abr.nodeTest() != null
                        ? BuildNodeTest(abr.nodeTest()!)
                        : BuildSimpleNodeTest(abr.simpleNodeTest()!);
                }
                else
                {
                    axis = Axis.Child;
                    nodeTest = BuildSimpleNodeTest(abr.simpleNodeTest()!);
                }
            }
            else
            {
                var full = ctx.fullStep()!;
                axis = BuildAxis(full.axis());
                nodeTest = BuildNodeTest(full.nodeTest());
            }

            var predicates = ctx.predicate().Select(p => BuildExpr(p.expr())).ToList();
            return new AxisStepExpr { Axis = axis, NodeTest = nodeTest, Predicates = predicates, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private static Axis BuildAxis(XPath4Parser.AxisContext ctx)
        {
            var first = ctx.GetChild(0) as ITerminalNode;
            return first?.Symbol.Type switch
            {
                XPath4Lexer.KW_ANCESTOR              => Axis.Ancestor,
                XPath4Lexer.KW_ANCESTOR_OR_SELF      => Axis.AncestorOrSelf,
                XPath4Lexer.KW_ATTRIBUTE             => Axis.Attribute,
                XPath4Lexer.KW_CHILD                 => Axis.Child,
                XPath4Lexer.KW_DESCENDANT            => Axis.Descendant,
                XPath4Lexer.KW_DESCENDANT_OR_SELF    => Axis.DescendantOrSelf,
                XPath4Lexer.KW_FOLLOWING             => Axis.Following,
                XPath4Lexer.KW_FOLLOWING_SIBLING     => Axis.FollowingSibling,
                XPath4Lexer.KW_NAMESPACE             => Axis.Namespace,
                XPath4Lexer.KW_PARENT                => Axis.Parent,
                XPath4Lexer.KW_PRECEDING             => Axis.Preceding,
                XPath4Lexer.KW_PRECEDING_SIBLING     => Axis.PrecedingSibling,
                XPath4Lexer.KW_SELF                  => Axis.Self,
                _                                    => Axis.Child
            };
        }

        private ExprNode BuildNodeTest(XPath4Parser.NodeTestContext ctx)
        {
            if (ctx.unionNodeTest() is { } unt)
                return BuildSimpleNodeTest(unt.simpleNodeTest()[0]);
            if (ctx.simpleNodeTest() is { } snt)
                return BuildSimpleNodeTest(snt);
            throw new XPathParseException($"Dynamic node tests are not supported (line {ctx.Start.Line})");
        }

        private ExprNode BuildSimpleNodeTest(XPath4Parser.SimpleNodeTestContext ctx)
        {
            if (ctx.typeTest()  is { } tt) return BuildTypeTest(tt);
            return BuildSelector(ctx.selector()!);
        }

        private ExprNode BuildTypeTest(XPath4Parser.TypeTestContext ctx)
        {
            if (ctx.xNodeType() is { } xnt) return BuildXNodeType(xnt);
            // gnodetype / jnodetype: treat as any-node
            return new KindTestExpr { Kind = XdmNodeKind.Element, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildXNodeType(XPath4Parser.XNodeTypeContext ctx)
        {
            int ln = ctx.Start.Line, col = ctx.Start.Column;
            // node() — kind=Element + no Name acts as "matches all nodes" in the evaluator
            if (ctx.anyXNodeType()                != null) return new KindTestExpr { Kind = XdmNodeKind.Element,               Line = ln, Column = col };
            if (ctx.textNodeType()                != null) return new KindTestExpr { Kind = XdmNodeKind.Text,                  Line = ln, Column = col };
            if (ctx.commentNodeType()             != null) return new KindTestExpr { Kind = XdmNodeKind.Comment,               Line = ln, Column = col };
            if (ctx.namespaceNodeType()           != null) return new KindTestExpr { Kind = XdmNodeKind.Namespace,             Line = ln, Column = col };
            if (ctx.documentNodeType()            != null) return new KindTestExpr { Kind = XdmNodeKind.Document,              Line = ln, Column = col };
            if (ctx.schemaElementNodeType()       != null) return new KindTestExpr { Kind = XdmNodeKind.Element,               Line = ln, Column = col };
            if (ctx.schemaAttributeNodeType()     != null) return new KindTestExpr { Kind = XdmNodeKind.Attribute,             Line = ln, Column = col };

            if (ctx.processingInstructionNodeType() is { } pi)
            {
                XdmQName? name = pi.QName() != null ? new XdmQName(pi.QName().GetText()) : null;
                return new KindTestExpr { Kind = XdmNodeKind.ProcessingInstruction, Name = name, Line = ln, Column = col };
            }
            if (ctx.elementNodeType() is { } en)
            {
                XdmQName? name = null;
                var nt = en.nameTestUnion()?.nameTest();
                if (nt is { Length: > 0 } && nt[0].eqName() is { } eqn) name = ResolveEqName(eqn);
                return new KindTestExpr { Kind = XdmNodeKind.Element, Name = name, Line = ln, Column = col };
            }
            if (ctx.attributeNodeType() is { } an)
            {
                XdmQName? name = null;
                var nt = an.nameTestUnion()?.nameTest();
                if (nt is { Length: > 0 } && nt[0].eqName() is { } eqn) name = ResolveEqName(eqn);
                return new KindTestExpr { Kind = XdmNodeKind.Attribute, Name = name, Line = ln, Column = col };
            }
            return new KindTestExpr { Kind = XdmNodeKind.Element, Line = ln, Column = col };
        }

        private ExprNode BuildSelector(XPath4Parser.SelectorContext ctx)
        {
            if (ctx.wildcard() is { } wc) return BuildWildcard(wc, ctx.Start.Line, ctx.Start.Column);
            return BuildNameTest(ctx.eqName()!, ctx.Start.Line, ctx.Start.Column);
        }

        private static ExprNode BuildWildcard(XPath4Parser.WildcardContext ctx, int line, int col)
        {
            // STAR only → bare *
            if (ctx.CS() == null && ctx.SC() == null && ctx.BracedURILiteral() == null)
                return new NameTestExpr { IsWildcard = true, LocalName = "*", Line = line, Column = col };
            // QName CS → prefix:*
            if (ctx.CS() != null)
                return new NameTestExpr { Prefix = ctx.QName().GetText(), IsLocalWildcard = true, LocalName = "*", Line = line, Column = col };
            // SC QName → *:local
            if (ctx.SC() != null)
                return new NameTestExpr { IsPrefixWildcard = true, LocalName = ctx.QName().GetText(), Line = line, Column = col };
            // BracedURILiteral STAR → Q{uri}*
            return new NameTestExpr { IsWildcard = true, LocalName = "*", Line = line, Column = col };
        }

        private ExprNode BuildNameTest(XPath4Parser.EqNameContext ctx, int line, int col)
        {
            var (local, prefix, ns) = SplitEqName(ctx.GetText());
            return new NameTestExpr { Prefix = prefix, LocalName = local, NamespaceUri = ns, Line = line, Column = col };
        }

        // ── Postfix expression ────────────────────────────────────────────

        private ExprNode BuildPostfixExpr(XPath4Parser.PostfixExprContext ctx)
        {
            var result = BuildPrimaryExpr(ctx.primaryExpr());
            if (ctx.ChildCount == 1) return result;

            for (int i = 1; i < ctx.ChildCount; i++)
            {
                var child = ctx.GetChild(i);
                if (child is XPath4Parser.PredicateContext pred)
                {
                    if (result is FilterExpr fe)
                        fe.Predicates.Add(BuildExpr(pred.expr()));
                    else
                        result = new FilterExpr { Primary = result, Predicates = [BuildExpr(pred.expr())], Line = result.Line, Column = result.Column };
                }
                else if (child is XPath4Parser.LookupContext lkp)
                {
                    var ks = lkp.keySpecifier();
                    result = new PostfixLookupExpr
                    {
                        Base = result,
                        KeyExpr = BuildKeySpecifier(ks),
                        IsWildcard = ks.lookupWildcard() != null,
                        Line = lkp.Start.Line, Column = lkp.Start.Column
                    };
                }
                else if (child is XPath4Parser.PositionalArgumentListContext pal)
                {
                    // Dynamic function call: $f(args) — model as FilterExpr for now
                    var args = BuildPositionalArgList(pal);
                    result = new FilterExpr { Primary = result, Predicates = args, Line = result.Line, Column = result.Column };
                }
                else if (child is ITerminalNode tn && tn.Symbol.Type == XPath4Lexer.METHOD_ARROW)
                {
                    // METHOD_ARROW QName positionalargumentlist
                    i++;
                    var qname = ctx.GetChild(i).GetText();
                    i++;
                    var methodPal = ctx.GetChild(i) as XPath4Parser.PositionalArgumentListContext;
                    var funcNode = new FunctionCallExpr { Name = qname, Arguments = [], Line = result.Line, Column = result.Column };
                    result = new ArrowExpr
                    {
                        Argument = result, Function = funcNode,
                        AdditionalArguments = methodPal != null ? BuildPositionalArgList(methodPal) : [],
                        Line = result.Line, Column = result.Column
                    };
                }
            }
            return result;
        }

        private ExprNode? BuildKeySpecifier(XPath4Parser.KeySpecifierContext ctx)
        {
            if (ctx.lookupWildcard()    != null) return null;
            if (ctx.QName()             is { } qn) return new StringLiteralExpr { Value = qn.GetText(),   Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.literal()           is { } lit) return BuildLiteral(lit);
            if (ctx.varRef()            is { } vr)  return BuildVarRef(vr);
            if (ctx.parenthesizedExpr() is { } pe)  return BuildParenthesizedExpr(pe);
            if (ctx.contextValueRef()   != null)     return new ContextItemExpr { Line = ctx.Start.Line, Column = ctx.Start.Column };
            return null;
        }

        private List<ExprNode> BuildPositionalArgList(XPath4Parser.PositionalArgumentListContext ctx)
        {
            if (ctx.positionalArguments() is { } pa)
                return pa.argument().Select(BuildArgument).Where(a => a != null).Select(a => a!).ToList();
            return [];
        }

        // ── Primary expressions ───────────────────────────────────────────

        private ExprNode BuildPrimaryExpr(XPath4Parser.PrimaryExprContext ctx)
        {
            if (ctx.literal()           is { } lit) return BuildLiteral(lit);
            if (ctx.varRef()            is { } vr)  return BuildVarRef(vr);
            if (ctx.parenthesizedExpr() is { } pe)  return BuildParenthesizedExpr(pe);
            if (ctx.contextValueRef()   != null)     return new ContextItemExpr { Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.functionCall()      is { } fc)  return BuildFunctionCall(fc);
            if (ctx.nodeConstructor()   is { } nc)  return BuildComputedConstructor(nc.computedConstructor());
            if (ctx.functionItemExpr()  is { } fie) return BuildFunctionItemExpr(fie);
            if (ctx.mapConstructor()    is { } mc)  return BuildMapConstructor(mc);
            if (ctx.arrayConstructor()  is { } ac)  return BuildArrayConstructor(ac);
            if (ctx.stringTemplate()    != null)     return new StringLiteralExpr { Value = ctx.stringTemplate()!.GetText(), Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.unaryLookup()       is { } ul)
            {
                var ks = ul.lookup().keySpecifier();
                return new LookupExpr { KeyExpr = BuildKeySpecifier(ks), IsWildcard = ks.lookupWildcard() != null, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            throw new XPathParseException($"Unknown primary expression at line {ctx.Start.Line}");
        }

        private ExprNode BuildLiteral(XPath4Parser.LiteralContext ctx)
        {
            if (ctx.StringLiteral() is { } sl)
            {
                var raw = sl.GetText();
                string inner = raw.Length >= 2 ? raw[1..^1] : raw;
                inner = raw[0] == '"' ? inner.Replace("\"\"", "\"") : inner.Replace("''", "'");
                return new StringLiteralExpr { Value = inner, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            var num = ctx.numericLiteral()!;
            if (num.IntegerLiteral() is { } il)
                return new IntegerLiteralExpr { Value = long.Parse(il.GetText()), Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (num.DecimalLiteral() is { } dl)
                return new DecimalLiteralExpr { Value = decimal.Parse(dl.GetText(), System.Globalization.CultureInfo.InvariantCulture), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return new DoubleLiteralExpr { Value = double.Parse(num.DoubleLiteral()!.GetText(), System.Globalization.CultureInfo.InvariantCulture), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildVarRef(XPath4Parser.VarRefContext ctx)
        {
            var (local, prefix, ns) = SplitEqName(ctx.eqName().GetText());
            return new VariableRefExpr { Name = local, Prefix = prefix, NamespaceUri = ns, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildParenthesizedExpr(XPath4Parser.ParenthesizedExprContext ctx)
        {
            if (ctx.expr() == null)
                return new SequenceExpr { Items = [], Line = ctx.Start.Line, Column = ctx.Start.Column };
            var inner = BuildExpr(ctx.expr()!);
            return new ParenthesizedExpr { Inner = inner, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildFunctionCall(XPath4Parser.FunctionCallContext ctx)
        {
            var qname = ResolveEqName(ctx.eqName());
            var args  = BuildArgumentListPositional(ctx.argumentList());
            return new FunctionCallExpr
            {
                Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri,
                Arguments = args,
                Line = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        private List<ExprNode> BuildArgumentListPositional(XPath4Parser.ArgumentListContext ctx)
        {
            if (ctx.positionalArguments() is { } pa)
                return pa.argument().Select(BuildArgument).Where(a => a != null).Select(a => a!).ToList();
            return [];
        }

        private ExprNode? BuildArgument(XPath4Parser.ArgumentContext ctx)
        {
            if (ctx.exprSingle() is { } es) return BuildExprSingle(es);
            return null; // argumentplaceholder → omit
        }

        private ExprNode BuildFunctionItemExpr(XPath4Parser.FunctionItemExprContext ctx)
        {
            if (ctx.namedFunctionRef() is { } nfr)
            {
                var qname = ResolveEqName(nfr.eqName());
                return new NamedFunctionRefExpr
                {
                    Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri,
                    Arity = int.Parse(nfr.IntegerLiteral().GetText()),
                    Line = ctx.Start.Line, Column = ctx.Start.Column
                };
            }
            var ifn = ctx.inlineFunctionExpr()!;
            var sig = ifn.functionSignature();
            var parms = sig.paramList()?.varNameAndType().Select(vnt =>
            {
                var (name, _, seqType) = GetVarNameAndType(vnt);
                return new ParameterNode { Name = name, Type = seqType, Line = vnt.Start.Line, Column = vnt.Start.Column };
            }).ToList() ?? [];
            SequenceTypeNode? retType = sig.typeDeclaration() is { } td ? BuildSequenceType(td.sequenceType()) : null;
            var body = BuildEnclosedExpr(ifn.functionBody().enclosedExpr()) ?? new SequenceExpr { Items = [] };
            return new InlineFunctionExpr { Parameters = parms, ReturnType = retType, Body = body, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode? BuildEnclosedExpr(XPath4Parser.EnclosedExprContext ctx)
            => ctx.expr() != null ? BuildExpr(ctx.expr()!) : null;

        // ── Constructors ─────────────────────────────────────────────────

        private ExprNode BuildMapConstructor(XPath4Parser.MapConstructorContext ctx)
        {
            var entries = ctx.mapConstructorEntry().Select(e =>
            {
                var singles = e.exprSingle();
                return new MapEntry { Key = BuildExprSingle(singles[0]), Value = BuildExprSingle(singles[1]) };
            }).ToList();
            return new MapConstructorExpr { Entries = entries, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildArrayConstructor(XPath4Parser.ArrayConstructorContext ctx)
        {
            if (ctx.squareArrayConstructor() is { } sq)
                return new ArrayConstructorExpr { Members = sq.exprSingle().Select(BuildExprSingle).ToList(), IsCurly = false, Line = ctx.Start.Line, Column = ctx.Start.Column };
            var curly = ctx.curlyArrayConstructor()!;
            var inner = BuildEnclosedExpr(curly.enclosedExpr());
            return new ArrayConstructorExpr { Members = inner != null ? [inner] : [], IsCurly = true, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildComputedConstructor(XPath4Parser.ComputedConstructorContext ctx)
        {
            int ln = ctx.Start.Line, col = ctx.Start.Column;
            if (ctx.compTextConstructor() is { } tc)
                return new TextConstructorExpr    { Content = BuildEnclosedExpr(tc.enclosedExpr()) ?? EmptySeq(), Line = ln, Column = col };
            if (ctx.compCommentConstructor() is { } cc)
                return new CommentConstructorExpr { Content = BuildEnclosedExpr(cc.enclosedExpr()) ?? EmptySeq(), Line = ln, Column = col };
            if (ctx.compDocConstructor() is { } dc)
                return new DocumentConstructorExpr { Content = BuildEnclosedExpr(dc.enclosedExpr()) ?? EmptySeq(), Line = ln, Column = col };
            if (ctx.compElemConstructor() is { } ec)
            {
                var (name, nameExpr) = BuildCompNodeName(ec.compNodeName());
                var content = BuildEnclosedExpr(ec.enclosedContentExpr().enclosedExpr());
                return new ElementConstructorExpr { Name = name, NameExpr = nameExpr, Content = content != null ? [content] : [], IsComputed = true, Line = ln, Column = col };
            }
            if (ctx.compAttrConstructor() is { } ac)
            {
                var (name, nameExpr) = BuildCompNodeName(ac.compNodeName());
                return new AttributeConstructorExpr { Name = name, NameExpr = nameExpr, Value = BuildEnclosedExpr(ac.enclosedExpr()), IsComputed = true, Line = ln, Column = col };
            }
            if (ctx.compPIConstructor() is { } pi)
                return new PIConstructorExpr { Content = BuildEnclosedExpr(pi.enclosedExpr()), Line = ln, Column = col };
            throw new XPathParseException($"Unsupported computed constructor (line {ln})");
        }

        private (XdmQName? name, ExprNode? nameExpr) BuildCompNodeName(XPath4Parser.CompNodeNameContext ctx)
        {
            if (ctx.qNameLiteral() is { } qnl) return (ResolveEqName(qnl.eqName()), null);
            return (null, BuildExpr(ctx.expr()!));
        }

        private static SequenceExpr EmptySeq() => new() { Items = [] };

        // ── Sequence types ────────────────────────────────────────────────

        private SequenceTypeNode BuildSequenceType(XPath4Parser.SequenceTypeContext ctx)
        {
            if (ctx.KW_EMPTY_SEQUENCE() != null)
                return new SequenceTypeNode { ItemType = new ItemTypeNode { Kind = ItemTypeKind.Empty }, Occurrence = OccurrenceIndicator.ZeroOrMore };
            var itemType = BuildItemType(ctx.itemType()!);
            return new SequenceTypeNode { ItemType = itemType, Occurrence = BuildOccurrence(ctx.occurrenceIndicator()) };
        }

        private ItemTypeNode BuildItemType(XPath4Parser.ItemTypeContext ctx)
        {
            if (ctx.regularItemType() is { } rit)
            {
                if (rit.anyItemType() != null) return new ItemTypeNode { Kind = ItemTypeKind.Item };
                if (rit.xNodeType()   != null) return new ItemTypeNode { Kind = ItemTypeKind.Node };
                if (rit.mapType()     != null) return new ItemTypeNode { Kind = ItemTypeKind.Map };
                if (rit.arrayType()   != null) return new ItemTypeNode { Kind = ItemTypeKind.Array };
                return new ItemTypeNode { Kind = ItemTypeKind.Item };
            }
            if (ctx.functionType() != null) return new ItemTypeNode { Kind = ItemTypeKind.Function };
            if (ctx.typeName_()   is { } tn) return new ItemTypeNode { Kind = ItemTypeKind.AtomicType, TypeName = ResolveEqName(tn.eqName()) };
            return new ItemTypeNode { Kind = ItemTypeKind.Item };
        }

        private static OccurrenceIndicator BuildOccurrence(XPath4Parser.OccurrenceIndicatorContext? ctx)
        {
            if (ctx == null)       return OccurrenceIndicator.ExactlyOne;
            if (ctx.QM()   != null) return OccurrenceIndicator.ZeroOrOne;
            if (ctx.STAR() != null) return OccurrenceIndicator.ZeroOrMore;
            return OccurrenceIndicator.OneOrMore;
        }

        // ── Comparison operators ──────────────────────────────────────────

        private static ComparisonOperator GetValueCompOp(XPath4Parser.ValueCompContext ctx)
            => ctx.GetText() switch
            {
                "eq" => ComparisonOperator.Eq, "ne" => ComparisonOperator.Ne,
                "lt" => ComparisonOperator.Lt, "le" => ComparisonOperator.Le,
                "gt" => ComparisonOperator.Gt, "ge" => ComparisonOperator.Ge,
                _    => ComparisonOperator.Eq
            };

        private static ComparisonOperator GetGeneralCompOp(XPath4Parser.GeneralCompContext ctx)
        {
            var tt = (ctx.GetChild(0) as ITerminalNode)?.Symbol.Type ?? -1;
            return tt switch
            {
                XPath4Lexer.EQ => ComparisonOperator.Equal,
                XPath4Lexer.NE => ComparisonOperator.NotEqual,
                XPath4Lexer.LT => ComparisonOperator.LessThan,
                XPath4Lexer.LE => ComparisonOperator.LessOrEqual,
                XPath4Lexer.GT => ComparisonOperator.GreaterThan,
                XPath4Lexer.GE => ComparisonOperator.GreaterOrEqual,
                _              => ComparisonOperator.Equal
            };
        }

        private static ComparisonOperator GetNodeCompOp(XPath4Parser.NodeCompContext ctx)
        {
            if (ctx.KW_IS()        != null) return ComparisonOperator.Is;
            if (ctx.nodePrecedes() != null) return ComparisonOperator.Precedes;
            if (ctx.nodeFollows()  != null) return ComparisonOperator.Follows;
            return ComparisonOperator.Is;
        }

        // ── Name helpers ──────────────────────────────────────────────────

        private (string local, string? prefix, string? ns) SplitEqName(string text)
        {
            if (text.StartsWith("Q{"))
            {
                int end = text.IndexOf('}');
                return (text[(end + 1)..], null, text[2..end]);
            }
            int colon = text.IndexOf(':');
            if (colon > 0)
            {
                var prefix = text[..colon];
                var local  = text[(colon + 1)..];
                _ns.TryGetValue(prefix, out var ns);
                return (local, prefix, ns);
            }
            return (text, null, null);
        }

        private XdmQName ResolveEqName(XPath4Parser.EqNameContext ctx)
        {
            var (local, prefix, ns) = SplitEqName(ctx.GetText());
            if (ns != null) return new XdmQName(ns, local, prefix ?? "");
            return new XdmQName(local);
        }

        private (string name, string? prefix, SequenceTypeNode? seqType) GetVarNameAndType(XPath4Parser.VarNameAndTypeContext ctx)
        {
            var (local, prefix, _) = SplitEqName(ctx.eqName().GetText());
            SequenceTypeNode? seqType = ctx.typeDeclaration() is { } td ? BuildSequenceType(td.sequenceType()) : null;
            return (local, prefix, seqType);
        }
    }
}

public class XPathParseException : Exception
{
    public XPathParseException(string message) : base(message) { }
}
