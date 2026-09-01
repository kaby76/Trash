using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using XQuery.DataModel;
using XQuery.Parser.Ast;

namespace XQuery.Parser;

/// <summary>
/// XQuery 4.0 parser using the ANTLR4-generated recognizer.
/// Provides Parse() for standalone expressions and ParseModule() for full modules.
/// </summary>
public class XQueryParser
{
    private readonly string _input;
    private readonly Dictionary<string, string> _namespaces = new();

    public XQueryParser(string input)
    {
        _input = input;
        _namespaces["xs"] = XdmQName.XsNamespace;
        _namespaces["fn"] = XdmQName.FnNamespace;
        _namespaces["map"] = XdmQName.MapNamespace;
        _namespaces["array"] = XdmQName.ArrayNamespace;
        _namespaces["math"] = XdmQName.MathNamespace;
        _namespaces["xml"] = XdmQName.XmlNamespace;
    }

    public void AddNamespace(string prefix, string uri) => _namespaces[prefix] = uri;

    /// <summary>Parses a standalone XQuery expression.</summary>
    public ExprNode Parse()
    {
        var (parser, tokens, errors) = CreateParser();
        var tree = parser.queryBody();
        if (errors.Count > 0) throw new XPathParseException(errors[0]);
        return new AstBuilder(_namespaces).BuildExprPublic(tree.expr());
    }

    /// <summary>Parses a full XQuery module (or bare expression body).</summary>
    public ModuleNode ParseModule()
    {
        var (parser, tokens, errors) = CreateParser();
        var tree = parser.queryList();
        if (errors.Count > 0) throw new XPathParseException(errors[0]);
        return new AstBuilder(_namespaces).BuildModule(tree);
    }

    private (XQuery4Parser parser, CommonTokenStream tokens, List<string> errors) CreateParser()
    {
        var stream = new AntlrInputStream(_input);
        var lexer  = new XQuery4Lexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new XQuery4Parser(tokens);
        var errors = new List<string>();
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new LexerErrorCollector(errors));
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new SyntaxErrorCollector(errors));
        return (parser, tokens, errors);
    }

    private sealed class SyntaxErrorCollector : BaseErrorListener
    {
        private readonly List<string> _errors;
        public SyntaxErrorCollector(List<string> errors) => _errors = errors;
        public override void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer,
            IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            => _errors.Add($"Line {line}:{charPositionInLine} {msg}");
    }

    private sealed class LexerErrorCollector : IAntlrErrorListener<int>
    {
        private readonly List<string> _errors;
        public LexerErrorCollector(List<string> errors) => _errors = errors;
        public void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer,
            int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            => _errors.Add($"Line {line}:{charPositionInLine} {msg}");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AstBuilder
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class AstBuilder
    {
        private readonly Dictionary<string, string> _ns;

        public AstBuilder(Dictionary<string, string> namespaces) => _ns = namespaces;

        // Exposed for Parse() entry point
        public ExprNode BuildExprPublic(XQuery4Parser.ExprContext ctx) => BuildExpr(ctx);

        // ── Module ────────────────────────────────────────────────────────────

        public ModuleNode BuildModule(XQuery4Parser.QueryListContext ctx)
        {
            var mod = ctx.module_()[0];
            return BuildModule_(mod);
        }

        private ModuleNode BuildModule_(XQuery4Parser.Module_Context ctx)
        {
            var node = new ModuleNode();
            if (ctx.mainModule() is { } mm)
            {
                node.Prolog = BuildProlog(mm.prolog());
                var body = mm.queryBody()?.expr();
                node.Body = body != null ? BuildExpr(body) : null;
            }
            else if (ctx.libraryModule() is { } lm)
            {
                var decl = lm.moduleDecl();
                node.ModulePrefix = decl.QName().GetText();
                node.ModuleNamespace = StripQuotes(decl.uriLiteral().StringLiteral().GetText());
                node.Prolog = BuildProlog(lm.prolog());
            }
            return node;
        }

        private PrologNode BuildProlog(XQuery4Parser.PrologContext ctx)
        {
            var prolog = new PrologNode();
            if (ctx == null) return prolog;

            foreach (var child in ctx.children ?? Enumerable.Empty<IParseTree>())
            {
                switch (child)
                {
                    case XQuery4Parser.NamespaceDeclContext nd:
                        prolog.NamespaceDecls.Add(new NamespaceDeclNode
                        {
                            Prefix = nd.QName().GetText(),
                            Uri    = StripQuotes(nd.uriLiteral().StringLiteral().GetText())
                        });
                        break;

                    case XQuery4Parser.DefaultNamespaceDeclContext dnd:
                    {
                        var uri = StripQuotes(dnd.uriLiteral().StringLiteral().GetText());
                        var text = dnd.GetText();
                        if (text.Contains("element")) prolog.DefaultElementNamespace  = uri;
                        else                          prolog.DefaultFunctionNamespace = uri;
                        break;
                    }

                    case XQuery4Parser.VarDeclContext vd:
                    {
                        var vnt = vd.varNameAndType();
                        var (name, prefix, seqType) = GetVarNameAndType(vnt);
                        var value = vd.varValue()?.exprSingle() is { } vs ? BuildExprSingle(vs) : null;
                        prolog.VariableDecls.Add(new VariableDeclNode
                        {
                            Name = name, Prefix = prefix, Type = seqType,
                            Value = value, IsExternal = vd.KW_EXTERNAL() != null
                        });
                        break;
                    }

                    case XQuery4Parser.FunctionDeclContext fd:
                    {
                        var qname = ResolveEqName(fd.eqName());
                        var parms = fd.paramListWithDefaults()?.paramWithDefault().Select(p =>
                        {
                            var (n, _, st) = GetVarNameAndType(p.varNameAndType());
                            return new ParameterNode { Name = n, Type = st };
                        }).ToList() ?? new List<ParameterNode>();
                        var retType = fd.typeDeclaration() is { } td ? BuildSequenceType(td.sequenceType()) : null;
                        var body    = fd.functionBody()?.enclosedExpr() is { } fb ? BuildEnclosedExpr(fb) : null;
                        prolog.FunctionDecls.Add(new FunctionDeclNode
                        {
                            Name = qname.LocalName, Prefix = qname.Prefix,
                            Parameters = parms, ReturnType = retType,
                            Body = body, IsExternal = fd.KW_EXTERNAL() != null
                        });
                        break;
                    }
                }
            }
            return prolog;
        }

        // ── Expr ──────────────────────────────────────────────────────────────

        private ExprNode BuildExpr(XQuery4Parser.ExprContext ctx)
        {
            var singles = ctx.exprSingle();
            if (singles.Length == 1) return BuildExprSingle(singles[0]);
            return new SequenceExpr
            {
                Items  = singles.Select(BuildExprSingle).ToList(),
                Line   = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        private ExprNode BuildExprSingle(XQuery4Parser.ExprSingleContext ctx)
        {
            if (ctx.flworExpr()      is { } fe) return BuildFlworExpr(fe);
            if (ctx.quantifiedExpr() is { } qe) return BuildQuantifiedExpr(qe);
            if (ctx.switchExpr()     is { } se) return BuildSwitchExpr(se);
            if (ctx.typeswitchExpr() is { } te) return BuildTypeswitchExpr(te);
            if (ctx.ifExpr()         is { } ie) return BuildIfExpr(ie);
            if (ctx.tryCatchExpr()   is { } tc) return BuildTryCatchExpr(tc);
            // XQuery Update
            if (ctx.insertExpr()     is { } ins) return BuildInsertExpr(ins);
            if (ctx.deleteExpr()     is { } del) return BuildDeleteExpr(del);
            if (ctx.renameExpr()     is { } ren) return BuildRenameExpr(ren);
            if (ctx.replaceExpr()    is { } rep) return BuildReplaceExpr(rep);
            if (ctx.transformExpr()  is { } tr)  return BuildTransformExpr(tr);
            return BuildOrExpr(ctx.orExpr()!);
        }

        // ── FLWOR ─────────────────────────────────────────────────────────────

        private ExprNode BuildFlworExpr(XQuery4Parser.FlworExprContext ctx)
        {
            var clauses = new List<FlworClause>();
            BuildInitialClause(ctx.initialClause(), clauses);
            foreach (var ic in ctx.intermediateClause())
                BuildIntermediateClause(ic, clauses);
            var ret = BuildExprSingle(ctx.returnClause().exprSingle());
            return new FlworExpr { Clauses = clauses, Return = ret, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private void BuildInitialClause(XQuery4Parser.InitialClauseContext ctx, List<FlworClause> clauses)
        {
            if (ctx.forClause() is { } fc) BuildForClause(fc, clauses);
            else if (ctx.letClause() is { } lc) BuildLetClause(lc, clauses);
            // windowClause: not supported; treated as error
        }

        private void BuildIntermediateClause(XQuery4Parser.IntermediateClauseContext ctx, List<FlworClause> clauses)
        {
            if (ctx.initialClause() is { } ic)      BuildInitialClause(ic, clauses);
            else if (ctx.whereClause()   is { } wc) BuildWhereClause(wc, clauses);
            else if (ctx.orderByClause() is { } ob) BuildOrderByClause(ob, clauses);
            else if (ctx.groupByClause() is { } gb) BuildGroupByClause(gb, clauses);
            else if (ctx.countClause()   is { } cc) BuildCountClause(cc, clauses);
        }

        private void BuildForClause(XQuery4Parser.ForClauseContext ctx, List<FlworClause> clauses)
        {
            foreach (var binding in ctx.forBinding())
            {
                if (binding.forItemBinding() is { } fb)
                {
                    var (name, _, seqType) = GetVarNameAndType(fb.varNameAndType());
                    var posVar = fb.positionalVar()?.varName()?.eqName()?.GetText();
                    clauses.Add(new ForClause
                    {
                        Variable = name, PositionalVariable = posVar, Type = seqType,
                        AllowingEmpty = fb.allowingEmpty() != null,
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

        private void BuildLetClause(XQuery4Parser.LetClauseContext ctx, List<FlworClause> clauses)
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
                    throw new XPathParseException(
                        $"Destructuring let bindings are not supported (line {ctx.Start.Line})");
            }
        }

        private void BuildWhereClause(XQuery4Parser.WhereClauseContext ctx, List<FlworClause> clauses)
            => clauses.Add(new WhereClause { Condition = BuildExprSingle(ctx.exprSingle()), Line = ctx.Start.Line, Column = ctx.Start.Column });

        private void BuildOrderByClause(XQuery4Parser.OrderByClauseContext ctx, List<FlworClause> clauses)
        {
            var specs = ctx.orderSpec().Select(os =>
            {
                var mod = os.orderModifier();
                bool desc = mod.GetText().Contains("descending");
                bool emptyGreatest = mod.GetText().Contains("greatest");
                string coll = mod.uriLiteral()?.StringLiteral()?.GetText() is { } c ? StripQuotes(c) : null;
                return new OrderSpec { Expression = BuildExprSingle(os.exprSingle()), Descending = desc, EmptyGreatest = emptyGreatest, Collation = coll };
            }).ToList();
            clauses.Add(new OrderByClause { Stable = ctx.KW_STABLE() != null, Specs = specs, Line = ctx.Start.Line, Column = ctx.Start.Column });
        }

        private void BuildGroupByClause(XQuery4Parser.GroupByClauseContext ctx, List<FlworClause> clauses)
        {
            var specs = ctx.groupingSpec().Select(gs =>
            {
                var (name, _, seqType) = GetVarNameAndType(gs.varName().eqName(), gs.typeDeclaration());
                var expr = gs.exprSingle() is { } e ? BuildExprSingle(e) : null;
                string coll = gs.uriLiteral()?.StringLiteral()?.GetText() is { } c ? StripQuotes(c) : null;
                return new GroupSpec { Variable = name, Type = seqType, Expression = expr, Collation = coll };
            }).ToList();
            clauses.Add(new GroupByClause { Specs = specs, Line = ctx.Start.Line, Column = ctx.Start.Column });
        }

        private void BuildCountClause(XQuery4Parser.CountClauseContext ctx, List<FlworClause> clauses)
            => clauses.Add(new CountClause { Variable = ctx.varName().eqName().GetText(), Line = ctx.Start.Line, Column = ctx.Start.Column });

        // ── Quantified ────────────────────────────────────────────────────────

        private ExprNode BuildQuantifiedExpr(XQuery4Parser.QuantifiedExprContext ctx)
        {
            bool isSome = ctx.KW_SOME() != null;
            var bindings = ctx.quantifierBinding().Select(qb =>
            {
                var (name, _, seqType) = GetVarNameAndType(qb.varNameAndType());
                return new QuantifiedBinding { Variable = name, Type = seqType, Expression = BuildExprSingle(qb.exprSingle()) };
            }).ToList();
            return new QuantifiedExpr
            {
                IsSome = isSome, Bindings = bindings,
                Satisfies = BuildExprSingle(ctx.exprSingle()),
                Line = ctx.Start.Line, Column = ctx.Start.Column
            };
        }

        // ── If ────────────────────────────────────────────────────────────────

        private ExprNode BuildIfExpr(XQuery4Parser.IfExprContext ctx)
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

        // ── Switch / Typeswitch ─────────────────────────────────────────────────

        private ExprNode BuildSwitchExpr(XQuery4Parser.SwitchExprContext ctx)
        {
            var operand = ctx.switchComparand().expr() is { } e ? BuildExpr(e) : new SequenceExpr { Items = [] };
            var cases_ = ctx.switchCases() ?? ctx.bracedSwitchCases()?.switchCases();
            if (cases_ == null) throw new XPathParseException($"switch without cases at line {ctx.Start.Line}");
            var cases = cases_.switchCaseClause().Select(c => new SwitchCaseClause
            {
                Values = c.switchCaseOperand().Select(op => BuildExpr(op.expr())).ToList(),
                Result = BuildExprSingle(c.exprSingle())
            }).ToList();
            var default_ = BuildExprSingle(cases_.exprSingle());
            return new SwitchExpr { Operand = operand, Cases = cases, Default = default_, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildTypeswitchExpr(XQuery4Parser.TypeswitchExprContext ctx)
        {
            // Minimal: evaluate operand, return first matching branch.
            // Full typeswitch semantics require runtime type inspection beyond current scope.
            var operand = BuildExpr(ctx.expr());
            throw new XPathParseException($"typeswitch is not yet supported (line {ctx.Start.Line})");
        }

        // ── Try/Catch ─────────────────────────────────────────────────────────

        private ExprNode BuildTryCatchExpr(XQuery4Parser.TryCatchExprContext ctx)
        {
            var tryExpr = BuildEnclosedExpr(ctx.tryClause().enclosedExpr()) ?? new SequenceExpr { Items = [] };
            var catches = ctx.catchClause().Select(cc =>
            {
                var handler = BuildEnclosedExpr(cc.enclosedExpr()) ?? new SequenceExpr { Items = [] };
                return new CatchClause { Errors = [], Handler = handler };
            }).ToList();
            return new TryCatchExpr { TryExpr = tryExpr, CatchClauses = catches, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        // ── XQuery Update ─────────────────────────────────────────────────────

        private ExprNode BuildInsertExpr(XQuery4Parser.InsertExprContext ctx)
        {
            var source = BuildExprSingle(ctx.sourceExpr().exprSingle());
            var target = BuildExprSingle(ctx.targetExpr().exprSingle());
            var choiceText = ctx.insertExprTargetChoice().GetText();
            InsertPosition pos =
                choiceText.Contains("before")  ? InsertPosition.Before :
                choiceText.Contains("after")   ? InsertPosition.After  :
                choiceText.Contains("first")   ? InsertPosition.AsFirst :
                choiceText.Contains("last")    ? InsertPosition.AsLast :
                                                 InsertPosition.Into;
            return new InsertExpr { Source = source, Target = target, Position = pos, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildDeleteExpr(XQuery4Parser.DeleteExprContext ctx)
        {
            var target = BuildExprSingle(ctx.targetExpr().exprSingle());
            return new DeleteExpr { Target = target, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildReplaceExpr(XQuery4Parser.ReplaceExprContext ctx)
        {
            // 'replace' ('value' 'of')? 'node' targetExpr 'with' exprSingle
            bool valueOf = ctx.KW_VALUE() != null;
            var target      = BuildExprSingle(ctx.targetExpr().exprSingle());
            var replacement = BuildExprSingle(ctx.exprSingle());
            return new ReplaceExpr { ValueOf = valueOf, Target = target, Replacement = replacement, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildRenameExpr(XQuery4Parser.RenameExprContext ctx)
        {
            var target  = BuildExprSingle(ctx.targetExpr().exprSingle());
            var newName = BuildExprSingle(ctx.newNameExpr().exprSingle());
            return new RenameExpr { Target = target, NewName = newName, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildTransformExpr(XQuery4Parser.TransformExprContext ctx)
        {
            var varNames    = ctx.varName();
            var exprSingles = ctx.exprSingle();
            var bindings = varNames.Select((vn, i) => new CopyBinding
            {
                Variable   = vn.eqName().GetText(),
                Expression = BuildExprSingle(exprSingles[i])
            }).ToList();
            var modifyExpr = BuildExprSingle(exprSingles[varNames.Length]);
            var returnExpr = BuildExprSingle(exprSingles[varNames.Length + 1]);
            return new TransformExpr { CopyBindings = bindings, ModifyExpr = modifyExpr, ReturnExpr = returnExpr, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        // ── Boolean operators ─────────────────────────────────────────────────

        private ExprNode BuildOrExpr(XQuery4Parser.OrExprContext ctx)
        {
            var ops  = ctx.andExpr();
            var left = BuildAndExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
                left = new BinaryExpr { Left = left, Operator = BinaryOperator.Or, Right = BuildAndExpr(ops[i]), Line = left.Line, Column = left.Column };
            return left;
        }

        private ExprNode BuildAndExpr(XQuery4Parser.AndExprContext ctx)
        {
            var ops  = ctx.comparisonExpr();
            var left = BuildComparisonExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
                left = new BinaryExpr { Left = left, Operator = BinaryOperator.And, Right = BuildComparisonExpr(ops[i]), Line = left.Line, Column = left.Column };
            return left;
        }

        private ExprNode BuildComparisonExpr(XQuery4Parser.ComparisonExprContext ctx)
        {
            var ops  = ctx.otherwiseExpr();
            var left = BuildOtherwiseExpr(ops[0]);
            if (ops.Length == 1) return left;
            var right = BuildOtherwiseExpr(ops[1]);
            ComparisonOperator op;
            if      (ctx.valueComp()   is { } vc) op = GetValueCompOp(vc);
            else if (ctx.generalComp() is { } gc) op = GetGeneralCompOp(gc);
            else                                  op = GetNodeCompOp(ctx.nodeComp()!);
            return new ComparisonExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
        }

        private ExprNode BuildOtherwiseExpr(XQuery4Parser.OtherwiseExprContext ctx)
        {
            var ops  = ctx.stringConcatExpr();
            var left = BuildStringConcatExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
                left = new OtherwiseExpr { Left = left, Right = BuildStringConcatExpr(ops[i]), Line = left.Line, Column = left.Column };
            return left;
        }

        private ExprNode BuildStringConcatExpr(XQuery4Parser.StringConcatExprContext ctx)
        {
            var ops  = ctx.rangeExpr();
            var left = BuildRangeExpr(ops[0]);
            for (int i = 1; i < ops.Length; i++)
                left = new ConcatExpr { Left = left, Right = BuildRangeExpr(ops[i]), Line = left.Line, Column = left.Column };
            return left;
        }

        private ExprNode BuildRangeExpr(XQuery4Parser.RangeExprContext ctx)
        {
            var ops  = ctx.additiveExpr();
            var left = BuildAdditiveExpr(ops[0]);
            if (ops.Length == 1) return left;
            return new RangeExpr { Start = left, End = BuildAdditiveExpr(ops[1]), Line = left.Line, Column = left.Column };
        }

        // ── Arithmetic ────────────────────────────────────────────────────────

        private ExprNode BuildAdditiveExpr(XQuery4Parser.AdditiveExprContext ctx)
        {
            var operands = ctx.multiplicativeExpr();
            var left     = BuildMultiplicativeExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right   = BuildMultiplicativeExpr(operands[i]);
                var op = opToken?.Symbol.Type == XQuery4Lexer.PLUS ? BinaryOperator.Add : BinaryOperator.Subtract;
                left = new BinaryExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildMultiplicativeExpr(XQuery4Parser.MultiplicativeExprContext ctx)
        {
            var operands = ctx.unionExpr();
            var left     = BuildUnionExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right   = BuildUnionExpr(operands[i]);
                int tt      = opToken?.Symbol.Type ?? -1;
                BinaryOperator op =
                    tt == XQuery4Lexer.STAR       || tt == XQuery4Lexer.STAR_ALT    ? BinaryOperator.Multiply      :
                    tt == XQuery4Lexer.KW_DIV     || tt == XQuery4Lexer.DIV_ALT     ? BinaryOperator.Divide        :
                    tt == XQuery4Lexer.KW_IDIV                                      ? BinaryOperator.IntegerDivide :
                                                                                      BinaryOperator.Modulo;
                left = new BinaryExpr { Left = left, Operator = op, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        // ── Union / intersect / except ────────────────────────────────────────

        private ExprNode BuildUnionExpr(XQuery4Parser.UnionExprContext ctx)
        {
            var ops = ctx.intersectExceptExpr();
            if (ops.Length == 1) return BuildIntersectExceptExpr(ops[0]);
            return new UnionExpr { Operands = ops.Select(BuildIntersectExceptExpr).ToList(), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildIntersectExceptExpr(XQuery4Parser.IntersectExceptExprContext ctx)
        {
            var operands = ctx.recordPutExpr();
            var left     = BuildRecordPutExpr(operands[0]);
            for (int i = 1; i < operands.Length; i++)
            {
                var opToken    = ctx.GetChild(2 * i - 1) as ITerminalNode;
                var right      = BuildRecordPutExpr(operands[i]);
                bool isIntersect = opToken?.Symbol.Type == XQuery4Lexer.KW_INTERSECT;
                left = new IntersectExceptExpr { Left = left, IsIntersect = isIntersect, Right = right, Line = left.Line, Column = left.Column };
            }
            return left;
        }

        private ExprNode BuildRecordPutExpr(XQuery4Parser.RecordPutExprContext ctx)
        {
            var ops = ctx.instanceofExpr();
            if (ops.Length > 1)
                throw new XPathParseException($"Record-put operator (+:=) is not supported (line {ctx.Start.Line})");
            return BuildInstanceOfExpr(ops[0]);
        }

        // ── Type expressions ──────────────────────────────────────────────────

        private ExprNode BuildInstanceOfExpr(XQuery4Parser.InstanceofExprContext ctx)
        {
            var expr = BuildTreatExpr(ctx.treatExpr());
            if (ctx.sequenceType() is { } st)
                return new InstanceOfExpr { Expression = expr, Type = BuildSequenceType(st), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildTreatExpr(XQuery4Parser.TreatExprContext ctx)
        {
            var expr = BuildCastableExpr(ctx.castableExpr());
            if (ctx.sequenceType() is { } st)
                return new TreatExpr { Expression = expr, Type = BuildSequenceType(st), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildCastableExpr(XQuery4Parser.CastableExprContext ctx)
        {
            var expr = BuildCastExpr(ctx.castExpr());
            if (ctx.castTarget() is { } ct)
            {
                var (qname, allowEmpty) = BuildCastTarget(ct, ctx.occurrenceIndicator());
                return new CastableExpr { Expression = expr, TargetType = qname, AllowEmpty = allowEmpty, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            return expr;
        }

        private ExprNode BuildCastExpr(XQuery4Parser.CastExprContext ctx)
        {
            var expr = BuildPipelineExpr(ctx.pipelineExpr());
            if (ctx.castTarget() is { } ct)
            {
                var (qname, allowEmpty) = BuildCastTarget(ct, ctx.occurrenceIndicator());
                return new CastExpr { Expression = expr, TargetType = qname, AllowEmpty = allowEmpty, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            return expr;
        }

        private (XdmQName qname, bool allowEmpty) BuildCastTarget(
            XQuery4Parser.CastTargetContext ctx, XQuery4Parser.OccurrenceIndicatorContext occ)
        {
            if (ctx.typeName_() is { } tn)
                return (ResolveEqName(tn.eqName()), occ?.QM() != null);
            throw new XPathParseException($"Complex cast targets are not supported (line {ctx.Start.Line})");
        }

        // ── Pipeline / Arrow ─────────────────────────────────────────────────

        private ExprNode BuildPipelineExpr(XQuery4Parser.PipelineExprContext ctx)
        {
            var arrowExprs = ctx.arrowExpr();
            var result = BuildArrowExpr(arrowExprs[0]);
            for (int i = 1; i < arrowExprs.Length; i++)
            {
                var rhsCtx = arrowExprs[i];
                if (rhsCtx.children.OfType<XQuery4Parser.SequenceArrowTargetContext>().Any() ||
                    rhsCtx.children.OfType<XQuery4Parser.MappingArrowTargetContext>().Any())
                    throw new XPathParseException($"Arrow-target chains on the right-hand side of '->' are not supported (line {rhsCtx.Start.Line})");
                var rhs = BuildUnaryExpr(rhsCtx.unaryExpr());
                result = new ArrowExpr { Argument = result, Function = rhs, AdditionalArguments = [], IsThinArrow = true, Line = result.Line, Column = result.Column };
            }
            return result;
        }

        private ExprNode BuildArrowExpr(XQuery4Parser.ArrowExprContext ctx)
        {
            var result = BuildUnaryExpr(ctx.unaryExpr());
            foreach (var child in ctx.children)
            {
                if (child is XQuery4Parser.SequenceArrowTargetContext sat)
                    result = BuildArrowTarget(sat.arrowTarget(), result, isThinArrow: false);
                else if (child is XQuery4Parser.MappingArrowTargetContext mat)
                    result = BuildArrowTarget(mat.arrowTarget(), result, isThinArrow: true);
            }
            return result;
        }

        private ExprNode BuildArrowTarget(XQuery4Parser.ArrowTargetContext ctx, ExprNode arg, bool isThinArrow)
        {
            if (ctx.functionCall() is { } fc)
            {
                var qname   = ResolveEqName(fc.eqName());
                var addlArgs = BuildArgumentListPositional(fc.argumentList());
                var funcNode = new FunctionCallExpr { Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri, Arguments = [], Line = ctx.Start.Line, Column = ctx.Start.Column };
                return new ArrowExpr { Argument = arg, Function = funcNode, AdditionalArguments = addlArgs, IsThinArrow = isThinArrow, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            throw new XPathParseException($"Dynamic arrow calls are not supported (line {ctx.Start.Line})");
        }

        // ── Unary / simple-map / path ─────────────────────────────────────────

        private ExprNode BuildUnaryExpr(XQuery4Parser.UnaryExprContext ctx)
        {
            var expr       = BuildSimpleMapExpr(ctx.valueExpr().simpleMapExpr());
            int minusCount = 0;
            bool hasPlus   = false;
            foreach (var child in ctx.children)
            {
                if (child is ITerminalNode tn)
                {
                    if (tn.Symbol.Type == XQuery4Lexer.MINUS) minusCount++;
                    else if (tn.Symbol.Type == XQuery4Lexer.PLUS) hasPlus = true;
                }
            }
            if (minusCount % 2 == 1)
                return new UnaryExpr { Operator = UnaryOperator.Minus, Operand = expr, Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (hasPlus && minusCount == 0)
                return new UnaryExpr { Operator = UnaryOperator.Plus, Operand = expr, Line = ctx.Start.Line, Column = ctx.Start.Column };
            return expr;
        }

        private ExprNode BuildSimpleMapExpr(XQuery4Parser.SimpleMapExprContext ctx)
        {
            var ops = ctx.pathExpr();
            if (ops.Length == 1) return BuildPathExpr(ops[0]);
            return new SimpleMapExpr { Steps = ops.Select(BuildPathExpr).ToList(), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildPathExpr(XQuery4Parser.PathExprContext ctx)
        {
            if (ctx.absolutePathExpr() is { } ape) return BuildAbsolutePathExpr(ape);
            return BuildRelativePathExpr(ctx.relativePathExpr()!);
        }

        private static readonly KindTestExpr AnyNodeTest = new() { Kind = XdmNodeKind.Element };

        private ExprNode BuildAbsolutePathExpr(XQuery4Parser.AbsolutePathExprContext ctx)
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

        private ExprNode BuildRelativePathExpr(XQuery4Parser.RelativePathExprContext ctx)
        {
            var steps = BuildRelativePathExprSteps(ctx);
            if (steps.Count == 1) return steps[0];
            return new PathExpr { Steps = steps, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private List<ExprNode> BuildRelativePathExprSteps(XQuery4Parser.RelativePathExprContext ctx)
        {
            var result = new List<ExprNode>();
            foreach (var child in ctx.children)
            {
                if (child is XQuery4Parser.StepExprContext se)
                    result.Add(BuildStepExpr(se));
                else if (child is ITerminalNode tn && tn.Symbol.Type == XQuery4Lexer.SS)
                    result.Add(new AxisStepExpr { Axis = Axis.DescendantOrSelf, NodeTest = AnyNodeTest, Line = tn.Symbol.Line, Column = tn.Symbol.Column });
            }
            return result;
        }

        private ExprNode BuildStepExpr(XQuery4Parser.StepExprContext ctx)
        {
            if (ctx.axisStep()   is { } ax) return BuildAxisStep(ax);
            return BuildPostfixExpr(ctx.postfixExpr()!);
        }

        // ── Axis step ─────────────────────────────────────────────────────────

        private ExprNode BuildAxisStep(XQuery4Parser.AxisStepContext ctx)
        {
            Axis    axis;
            ExprNode nodeTest;

            if (ctx.abbreviatedStep() is { } abr)
            {
                if (abr.DD() != null)
                {
                    axis     = Axis.Parent;
                    nodeTest = AnyNodeTest;
                }
                else if (abr.AT() != null)
                {
                    axis     = Axis.Attribute;
                    nodeTest = abr.nodeTest() != null
                        ? BuildNodeTest(abr.nodeTest()!)
                        : BuildSimpleNodeTest(abr.simpleNodeTest()!);
                }
                else
                {
                    axis     = Axis.Child;
                    nodeTest = BuildSimpleNodeTest(abr.simpleNodeTest()!);
                }
            }
            else
            {
                var full = ctx.fullStep()!;
                axis     = BuildAxis(full.axis());
                nodeTest = BuildNodeTest(full.nodeTest());
            }

            var predicates = ctx.predicate().Select(p => BuildExpr(p.expr())).ToList();
            return new AxisStepExpr { Axis = axis, NodeTest = nodeTest, Predicates = predicates, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private static Axis BuildAxis(XQuery4Parser.AxisContext ctx)
        {
            var first = ctx.GetChild(0) as ITerminalNode;
            return first?.Symbol.Type switch
            {
                XQuery4Lexer.KW_ANCESTOR              => Axis.Ancestor,
                XQuery4Lexer.KW_ANCESTOR_OR_SELF      => Axis.AncestorOrSelf,
                XQuery4Lexer.KW_ATTRIBUTE             => Axis.Attribute,
                XQuery4Lexer.KW_CHILD                 => Axis.Child,
                XQuery4Lexer.KW_DESCENDANT            => Axis.Descendant,
                XQuery4Lexer.KW_DESCENDANT_OR_SELF    => Axis.DescendantOrSelf,
                XQuery4Lexer.KW_FOLLOWING                  => Axis.Following,
                XQuery4Lexer.KW_FOLLOWING_OR_SELF          => Axis.FollowingOrSelf,
                XQuery4Lexer.KW_FOLLOWING_SIBLING          => Axis.FollowingSibling,
                XQuery4Lexer.KW_FOLLOWING_SIBLING_OR_SELF  => Axis.FollowingSiblingOrSelf,
                XQuery4Lexer.KW_PARENT                     => Axis.Parent,
                XQuery4Lexer.KW_PRECEDING                  => Axis.Preceding,
                XQuery4Lexer.KW_PRECEDING_OR_SELF          => Axis.PrecedingOrSelf,
                XQuery4Lexer.KW_PRECEDING_SIBLING          => Axis.PrecedingSibling,
                XQuery4Lexer.KW_PRECEDING_SIBLING_OR_SELF  => Axis.PrecedingSiblingOrSelf,
                XQuery4Lexer.KW_SELF                  => Axis.Self,
                _                                     => Axis.Child
            };
        }

        private ExprNode BuildNodeTest(XQuery4Parser.NodeTestContext ctx)
        {
            if (ctx.unionNodeTest() is { } unt) return BuildSimpleNodeTest(unt.simpleNodeTest()[0]);
            if (ctx.simpleNodeTest() is { } snt) return BuildSimpleNodeTest(snt);
            throw new XPathParseException($"Dynamic node tests are not supported (line {ctx.Start.Line})");
        }

        private ExprNode BuildSimpleNodeTest(XQuery4Parser.SimpleNodeTestContext ctx)
        {
            if (ctx.typeTest() is { } tt) return BuildTypeTest(tt);
            return BuildSelector(ctx.selector()!);
        }

        private ExprNode BuildTypeTest(XQuery4Parser.TypeTestContext ctx)
        {
            if (ctx.xNodeType() is { } xnt) return BuildXNodeType(xnt);
            return new KindTestExpr { Kind = XdmNodeKind.Element, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildXNodeType(XQuery4Parser.XNodeTypeContext ctx)
        {
            int ln = ctx.Start.Line, col = ctx.Start.Column;
            if (ctx.anyXNodeType()                != null) return new KindTestExpr { Kind = XdmNodeKind.Element,               Line = ln, Column = col };
            if (ctx.textNodeType()                != null) return new KindTestExpr { Kind = XdmNodeKind.Text,                  Line = ln, Column = col };
            if (ctx.commentNodeType()             != null) return new KindTestExpr { Kind = XdmNodeKind.Comment,               Line = ln, Column = col };
            if (ctx.namespaceNodeType()           != null) return new KindTestExpr { Kind = XdmNodeKind.Namespace,             Line = ln, Column = col };
            if (ctx.documentNodeType()            != null) return new KindTestExpr { Kind = XdmNodeKind.Document,              Line = ln, Column = col };
            if (ctx.schemaElementNodeType()       != null) return new KindTestExpr { Kind = XdmNodeKind.Element,               Line = ln, Column = col };
            if (ctx.schemaAttributeNodeType()     != null) return new KindTestExpr { Kind = XdmNodeKind.Attribute,             Line = ln, Column = col };

            if (ctx.processingInstructionNodeType() is { } pi)
            {
                XdmQName name = pi.QName() != null ? new XdmQName(pi.QName().GetText()) : null;
                return new KindTestExpr { Kind = XdmNodeKind.ProcessingInstruction, Name = name, Line = ln, Column = col };
            }
            if (ctx.elementNodeType() is { } en)
            {
                XdmQName name = null;
                var nt = en.nameTestUnion()?.nameTest();
                if (nt is { Length: > 0 } && nt[0].eqName() is { } eqn) name = ResolveEqName(eqn);
                return new KindTestExpr { Kind = XdmNodeKind.Element, Name = name, Line = ln, Column = col };
            }
            if (ctx.attributeNodeType() is { } an)
            {
                XdmQName name = null;
                var nt = an.nameTestUnion()?.nameTest();
                if (nt is { Length: > 0 } && nt[0].eqName() is { } eqn) name = ResolveEqName(eqn);
                return new KindTestExpr { Kind = XdmNodeKind.Attribute, Name = name, Line = ln, Column = col };
            }
            return new KindTestExpr { Kind = XdmNodeKind.Element, Line = ln, Column = col };
        }

        private ExprNode BuildSelector(XQuery4Parser.SelectorContext ctx)
        {
            if (ctx.wildcard() is { } wc) return BuildWildcard(wc, ctx.Start.Line, ctx.Start.Column);
            return BuildNameTest(ctx.eqName()!, ctx.Start.Line, ctx.Start.Column);
        }

        private static ExprNode BuildWildcard(XQuery4Parser.WildcardContext ctx, int line, int col)
        {
            // XQuery4Parser wildcard is simply '*'; full wildcard forms live in nameTest.
            return new NameTestExpr { IsWildcard = true, LocalName = "*", Line = line, Column = col };
        }

        private ExprNode BuildNameTest(XQuery4Parser.EqNameContext ctx, int line, int col)
        {
            var (local, prefix, ns) = SplitEqName(ctx.GetText());
            return new NameTestExpr { Prefix = prefix, LocalName = local, NamespaceUri = ns, Line = line, Column = col };
        }

        // ── Postfix ───────────────────────────────────────────────────────────

        private ExprNode BuildPostfixExpr(XQuery4Parser.PostfixExprContext ctx)
        {
            var result = BuildPrimaryExpr(ctx.primaryExpr());
            if (ctx.ChildCount == 1) return result;

            for (int i = 1; i < ctx.ChildCount; i++)
            {
                var child = ctx.GetChild(i);
                if (child is XQuery4Parser.PredicateContext pred)
                {
                    if (result is FilterExpr fe) fe.Predicates.Add(BuildExpr(pred.expr()));
                    else result = new FilterExpr { Primary = result, Predicates = [BuildExpr(pred.expr())], Line = result.Line, Column = result.Column };
                }
                else if (child is XQuery4Parser.LookupContext lkp)
                {
                    var ks = lkp.keySpecifier();
                    result = new PostfixLookupExpr { Base = result, KeyExpr = BuildKeySpecifier(ks), IsWildcard = ks.lookupWildcard() != null, Line = lkp.Start.Line, Column = lkp.Start.Column };
                }
                else if (child is XQuery4Parser.PositionalArgumentListContext pal)
                {
                    var args = BuildPositionalArgList(pal);
                    result = new FilterExpr { Primary = result, Predicates = args, Line = result.Line, Column = result.Column };
                }
                else if (child is ITerminalNode tn2 && tn2.Symbol.Type == XQuery4Lexer.METHOD_ARROW)
                {
                    i++;
                    var qname = ctx.GetChild(i).GetText();
                    i++;
                    var methodPal = ctx.GetChild(i) as XQuery4Parser.PositionalArgumentListContext;
                    var funcNode  = new FunctionCallExpr { Name = qname, Arguments = [], Line = result.Line, Column = result.Column };
                    result = new ArrowExpr { Argument = result, Function = funcNode, AdditionalArguments = methodPal != null ? BuildPositionalArgList(methodPal) : [], Line = result.Line, Column = result.Column };
                }
            }
            return result;
        }

        private ExprNode BuildKeySpecifier(XQuery4Parser.KeySpecifierContext ctx)
        {
            if (ctx.lookupWildcard()    != null) return null;
            if (ctx.QName()             is { } qn)  return new StringLiteralExpr { Value = qn.GetText(),   Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.literal()           is { } lit) return BuildLiteral(lit);
            if (ctx.varRef()            is { } vr)  return BuildVarRef(vr);
            if (ctx.parenthesizedExpr() is { } pe)  return BuildParenthesizedExpr(pe);
            if (ctx.contextValueRef()   != null)     return new ContextItemExpr { Line = ctx.Start.Line, Column = ctx.Start.Column };
            return null;
        }

        private List<ExprNode> BuildPositionalArgList(XQuery4Parser.PositionalArgumentListContext ctx)
        {
            if (ctx.positionalArguments() is { } pa)
                return pa.argument().Select(BuildArgument).Where(a => a != null).Select(a => a!).ToList();
            return [];
        }

        // ── Primary ───────────────────────────────────────────────────────────

        private ExprNode BuildPrimaryExpr(XQuery4Parser.PrimaryExprContext ctx)
        {
            if (ctx.literal()           is { } lit) return BuildLiteral(lit);
            if (ctx.varRef()            is { } vr)  return BuildVarRef(vr);
            if (ctx.parenthesizedExpr() is { } pe)  return BuildParenthesizedExpr(pe);
            if (ctx.contextValueRef()   != null)     return new ContextItemExpr { Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.functionCall()      is { } fc)  return BuildFunctionCall(fc);
            if (ctx.nodeConstructor()   is { } nc)  return BuildNodeConstructor(nc);
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

        private ExprNode BuildLiteral(XQuery4Parser.LiteralContext ctx)
        {
            if (ctx.StringLiteral() is { } sl)
            {
                var raw   = sl.GetText();
                var inner = raw.Length >= 2 ? raw[1..^1] : raw;
                inner = raw[0] == '"' ? inner.Replace("\"\"", "\"") : inner.Replace("''", "'");
                inner = ResolveXmlEscapes(inner);
                return new StringLiteralExpr { Value = inner, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            var num = ctx.numericLiteral()!;
            if (num.IntegerLiteral() is { } il) return new IntegerLiteralExpr { Value = long.Parse(il.GetText()), Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (num.DecimalLiteral() is { } dl) return new DecimalLiteralExpr { Value = decimal.Parse(dl.GetText(), System.Globalization.CultureInfo.InvariantCulture), Line = ctx.Start.Line, Column = ctx.Start.Column };
            return new DoubleLiteralExpr { Value = double.Parse(num.DoubleLiteral()!.GetText(), System.Globalization.CultureInfo.InvariantCulture), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildVarRef(XQuery4Parser.VarRefContext ctx)
        {
            var (local, prefix, ns) = SplitEqName(ctx.eqName().GetText());
            return new VariableRefExpr { Name = local, Prefix = prefix, NamespaceUri = ns, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildParenthesizedExpr(XQuery4Parser.ParenthesizedExprContext ctx)
        {
            if (ctx.expr() == null) return new SequenceExpr { Items = [], Line = ctx.Start.Line, Column = ctx.Start.Column };
            return new ParenthesizedExpr { Inner = BuildExpr(ctx.expr()!), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildFunctionCall(XQuery4Parser.FunctionCallContext ctx)
        {
            var qname = ResolveEqName(ctx.eqName());
            var args  = BuildArgumentListPositional(ctx.argumentList());
            return new FunctionCallExpr { Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri, Arguments = args, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private List<ExprNode> BuildArgumentListPositional(XQuery4Parser.ArgumentListContext ctx)
        {
            if (ctx.positionalArguments() is { } pa)
                return pa.argument().Select(BuildArgument).Where(a => a != null).Select(a => a!).ToList();
            return [];
        }

        private ExprNode BuildArgument(XQuery4Parser.ArgumentContext ctx)
        {
            if (ctx.exprSingle() is { } es) return BuildExprSingle(es);
            return null;
        }

        private ExprNode BuildNodeConstructor(XQuery4Parser.NodeConstructorContext ctx)
        {
            if (ctx.computedConstructor() is { } cc) return BuildComputedConstructor(cc);
            return BuildDirectConstructor(ctx.directConstructor()!);
        }

        private ExprNode BuildDirectConstructor(XQuery4Parser.DirectConstructorContext ctx)
        {
            int ln = ctx.Start.Line, col = ctx.Start.Column;
            if (ctx.dirElemConstructor() is { } elem)
                return BuildDirectElementConstructor(elem);

            if (ctx.dirCommentConstructor() is { } comment)
            {
                var text = comment.GetText();
                var commentValue = text.Length >= 7 ? text[4..^3] : string.Empty;
                return new CommentConstructorExpr
                {
                    Content = StringLiteral(commentValue, comment.Start.Line, comment.Start.Column),
                    Line = ln,
                    Column = col
                };
            }

            var piText = ctx.dirPIConstructor()!.GetText();
            var body = piText.Length >= 4 ? piText[2..^2] : string.Empty;
            int separator = body.IndexOfAny([' ', '\t', '\r', '\n']);
            string target = separator < 0 ? body : body[..separator];
            string value = separator < 0 ? string.Empty : body[(separator + 1)..].TrimStart(' ', '\t', '\r', '\n');
            return new PIConstructorExpr
            {
                Target = target,
                Content = StringLiteral(value, ln, col),
                Line = ln,
                Column = col
            };
        }

        private ExprNode BuildDirectElementConstructor(XQuery4Parser.DirElemConstructorContext ctx)
        {
            var names = ctx.QName();
            string openName = names[0].GetText();
            if (names.Length > 1 && names[^1].GetText() != openName)
                throw new XPathParseException($"Direct element constructor start tag <{openName}> does not match end tag </{names[^1].GetText()}> (line {ctx.Start.Line})");

            var element = new ElementConstructorExpr
            {
                Name = ResolveDirectQName(openName),
                Line = ctx.Start.Line,
                Column = ctx.Start.Column
            };

            var attributes = ctx.dirAttributeList();
            var attributeNames = attributes.QName();
            var attributeValues = attributes.dirAttributeValue();
            for (int i = 0; i < attributeNames.Length; i++)
            {
                element.Content.Add(new AttributeConstructorExpr
                {
                    Name = ResolveDirectQName(attributeNames[i].GetText()),
                    Value = BuildDirectAttributeValue(attributeValues[i]),
                    Line = attributeNames[i].Symbol.Line,
                    Column = attributeNames[i].Symbol.Column
                });
            }

            foreach (var content in ctx.dirElemContent())
                element.Content.Add(BuildDirectElementContent(content));

            return element;
        }

        private ExprNode BuildDirectAttributeValue(XQuery4Parser.DirAttributeValueContext ctx)
        {
            var parts = new List<ExprNode>();
            foreach (var part in ctx.quotAttrValueContent())
                parts.Add(BuildDirectAttributePart(part.GetText(), part.commonContent(), part.Start.Line, part.Start.Column));
            foreach (var part in ctx.aposAttrValueContent())
                parts.Add(BuildDirectAttributePart(part.GetText(), part.commonContent(), part.Start.Line, part.Start.Column));
            return ConcatParts(parts, ctx.Start.Line, ctx.Start.Column);
        }

        private ExprNode BuildDirectAttributePart(string text, XQuery4Parser.CommonContentContext common, int line, int column)
        {
            if (common?.enclosedExpr() is { } enclosed)
                return BuildEnclosedExpr(enclosed) ?? StringLiteral(string.Empty, line, column);
            if (text == "\"\"") text = "\"";
            else if (text == "''") text = "'";
            else if (text == "{{") text = "{";
            else if (text == "}}") text = "}";
            else text = ResolveXmlEscapes(text);
            return StringLiteral(text, line, column);
        }

        private ExprNode BuildDirectElementContent(XQuery4Parser.DirElemContentContext ctx)
        {
            if (ctx.directConstructor() is { } direct)
                return BuildDirectConstructor(direct);
            if (ctx.commonContent()?.enclosedExpr() is { } enclosed)
                return BuildEnclosedExpr(enclosed) ?? StringLiteral(string.Empty, ctx.Start.Line, ctx.Start.Column);

            string text = ctx.GetText();
            if (ctx.cDataSection() != null && text.Length >= 12)
                text = text[9..^3];
            else if (text == "{{") text = "{";
            else if (text == "}}") text = "}";
            else text = ResolveXmlEscapes(text);

            return new TextConstructorExpr
            {
                Content = StringLiteral(text, ctx.Start.Line, ctx.Start.Column),
                Line = ctx.Start.Line,
                Column = ctx.Start.Column
            };
        }

        private XdmQName ResolveDirectQName(string text)
        {
            var (local, prefix, ns) = SplitEqName(text);
            return new XdmQName(ns ?? string.Empty, local, prefix ?? string.Empty);
        }

        private static ExprNode StringLiteral(string value, int line, int column)
            => new StringLiteralExpr { Value = value, Line = line, Column = column };

        private static ExprNode ConcatParts(List<ExprNode> parts, int line, int column)
        {
            if (parts.Count == 0) return StringLiteral(string.Empty, line, column);
            ExprNode result = parts[0];
            for (int i = 1; i < parts.Count; i++)
                result = new ConcatExpr { Left = result, Right = parts[i], Line = line, Column = column };
            return result;
        }

        private ExprNode BuildComputedConstructor(XQuery4Parser.ComputedConstructorContext ctx)
        {
            int ln = ctx.Start.Line, col = ctx.Start.Column;
            if (ctx.compDocConstructor() is { } doc)
            {
                var content = BuildEnclosedExpr(doc.enclosedExpr()) ?? new SequenceExpr { Items = [] };
                return new DocumentConstructorExpr { Content = content, Line = ln, Column = col };
            }
            if (ctx.compElemConstructor() is { } elem)
            {
                var name = BuildCompNodeName(elem.compNodeName());
                var content = BuildEnclosedExpr(elem.enclosedContentExpr().enclosedExpr()) ?? new SequenceExpr { Items = [] };
                if (name is StringLiteralExpr sle)
                    return new ElementConstructorExpr { Name = new XdmQName(sle.Value), Content = [content], Line = ln, Column = col };
                return new ElementConstructorExpr { NameExpr = name, Content = [content], Line = ln, Column = col };
            }
            if (ctx.compAttrConstructor() is { } attr)
            {
                var name = BuildCompNodeName(attr.compNodeName());
                var value = BuildEnclosedExpr(attr.enclosedExpr());
                if (name is StringLiteralExpr sle)
                    return new AttributeConstructorExpr { Name = new XdmQName(sle.Value), Value = value, Line = ln, Column = col };
                return new AttributeConstructorExpr { NameExpr = name, Value = value, Line = ln, Column = col };
            }
            if (ctx.compTextConstructor() is { } text)
            {
                var content = BuildEnclosedExpr(text.enclosedExpr()) ?? new SequenceExpr { Items = [] };
                return new TextConstructorExpr { Content = content, Line = ln, Column = col };
            }
            if (ctx.compCommentConstructor() is { } comment)
            {
                var content = BuildEnclosedExpr(comment.enclosedExpr()) ?? new SequenceExpr { Items = [] };
                return new CommentConstructorExpr { Content = content, Line = ln, Column = col };
            }
            if (ctx.compPIConstructor() is { } pi)
            {
                var target = BuildCompNodeNCName(pi.compNodeNCName());
                var content = BuildEnclosedExpr(pi.enclosedExpr());
                return new PIConstructorExpr { Target = target, Content = content, Line = ln, Column = col };
            }
            if (ctx.compNamespaceConstructor() is { })
                throw new XPathParseException($"Namespace constructor not supported (line {ln})");
            throw new XPathParseException($"Unknown computed constructor at line {ln}");
        }

        private ExprNode BuildCompNodeName(XQuery4Parser.CompNodeNameContext ctx)
        {
            if (ctx.qNameLiteral() is { } ql) return new StringLiteralExpr { Value = ql.eqName().GetText(), Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.unreservedName() is { } un) return new StringLiteralExpr { Value = un.eqName().GetText(), Line = ctx.Start.Line, Column = ctx.Start.Column };
            if (ctx.expr() is { } e) return BuildExpr(e);
            return new StringLiteralExpr { Value = ctx.GetText(), Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private string BuildCompNodeNCName(XQuery4Parser.CompNodeNCNameContext ctx)
        {
            if (ctx.unreservedNCName() is { } un) return un.NCName().GetText();
            if (ctx.markedNCName()     is { } mn) return mn.QName().GetText();
            return null;
        }

        private ExprNode BuildFunctionItemExpr(XQuery4Parser.FunctionItemExprContext ctx)
        {
            if (ctx.namedFunctionRef() is { } nfr)
            {
                var qname = ResolveEqName(nfr.eqName());
                return new NamedFunctionRefExpr { Name = qname.LocalName, Prefix = qname.Prefix, NamespaceUri = qname.NamespaceUri, Arity = int.Parse(nfr.IntegerLiteral().GetText()), Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            var ifn = ctx.inlineFunctionExpr()!;
            var sig = ifn.functionSignature();
            var parms = sig?.paramList()?.varNameAndType().Select(vnt =>
            {
                var (name, _, seqType) = GetVarNameAndType(vnt);
                return new ParameterNode { Name = name, Type = seqType, Line = vnt.Start.Line, Column = vnt.Start.Column };
            }).ToList() ?? [];
            SequenceTypeNode retType = sig?.typeDeclaration() is { } td ? BuildSequenceType(td.sequenceType()) : null;
            var body = BuildEnclosedExpr(ifn.functionBody().enclosedExpr()) ?? new SequenceExpr { Items = [] };
            return new InlineFunctionExpr { Parameters = parms, ReturnType = retType, Body = body, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildEnclosedExpr(XQuery4Parser.EnclosedExprContext ctx)
            => ctx.expr() != null ? BuildExpr(ctx.expr()!) : null;

        private ExprNode BuildMapConstructor(XQuery4Parser.MapConstructorContext ctx)
        {
            var entries = ctx.mapConstructorEntry().Select(e =>
            {
                var singles = e.exprSingle();
                if (singles.Length < 2) throw new XPathParseException($"Map entry at line {e.Start.Line} missing value");
                return new MapEntry { Key = BuildExprSingle(singles[0]), Value = BuildExprSingle(singles[1]) };
            }).ToList();
            return new MapConstructorExpr { Entries = entries, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        private ExprNode BuildArrayConstructor(XQuery4Parser.ArrayConstructorContext ctx)
        {
            if (ctx.squareArrayConstructor() is { } sq)
            {
                var members = sq.exprSingle().Select(BuildExprSingle).ToList();
                return new ArrayConstructorExpr { Members = members, IsCurly = false, Line = ctx.Start.Line, Column = ctx.Start.Column };
            }
            var inner = BuildEnclosedExpr(ctx.curlyArrayConstructor()!.enclosedExpr());
            return new ArrayConstructorExpr { Members = inner != null ? [inner] : [], IsCurly = true, Line = ctx.Start.Line, Column = ctx.Start.Column };
        }

        // ── Comparison operators ────────────────────────────────────────────────

        private static ComparisonOperator GetValueCompOp(XQuery4Parser.ValueCompContext ctx)
        {
            var text = ctx.GetText();
            return text switch
            {
                "eq" => ComparisonOperator.Eq, "ne" => ComparisonOperator.Ne,
                "lt" => ComparisonOperator.Lt, "le" => ComparisonOperator.Le,
                "gt" => ComparisonOperator.Gt, "ge" => ComparisonOperator.Ge,
                _    => ComparisonOperator.Eq
            };
        }

        private static ComparisonOperator GetGeneralCompOp(XQuery4Parser.GeneralCompContext ctx)
        {
            var tok = (ctx.GetChild(0) as ITerminalNode)?.Symbol.Type ?? -1;
            return tok switch
            {
                XQuery4Lexer.EQ => ComparisonOperator.Equal,        XQuery4Lexer.NE => ComparisonOperator.NotEqual,
                XQuery4Lexer.LT => ComparisonOperator.LessThan,     XQuery4Lexer.LE => ComparisonOperator.LessOrEqual,
                XQuery4Lexer.GT => ComparisonOperator.GreaterThan,  XQuery4Lexer.GE => ComparisonOperator.GreaterOrEqual,
                _               => ComparisonOperator.Equal
            };
        }

        private static ComparisonOperator GetNodeCompOp(XQuery4Parser.NodeCompContext ctx)
        {
            var text = ctx.GetText();
            return text switch
            {
                "is" => ComparisonOperator.Is,
                ">>" => ComparisonOperator.Follows,
                "<<" => ComparisonOperator.Precedes,
                _    => ComparisonOperator.Is
            };
        }

        // ── Sequence type ─────────────────────────────────────────────────────

        private SequenceTypeNode BuildSequenceType(XQuery4Parser.SequenceTypeContext ctx)
        {
            if (ctx.KW_EMPTY_SEQUENCE() != null)
                return new SequenceTypeNode { ItemType = new ItemTypeNode { Kind = ItemTypeKind.Empty }, Occurrence = OccurrenceIndicator.ExactlyOne };
            var itemType = BuildItemType(ctx.itemType()!);
            var occ = ctx.occurrenceIndicator() is { } oi ? GetOccurrence(oi) : OccurrenceIndicator.ExactlyOne;
            return new SequenceTypeNode { ItemType = itemType, Occurrence = occ };
        }

        private static ItemTypeNode BuildItemType(XQuery4Parser.ItemTypeContext ctx)
        {
            if (ctx.regularItemType()?.anyItemType() != null)
                return new ItemTypeNode { Kind = ItemTypeKind.Item };
            if (ctx.typeName_() is { } tn)
                return new ItemTypeNode { Kind = ItemTypeKind.AtomicType, TypeName = new XdmQName(tn.eqName().GetText()) };
            return new ItemTypeNode { Kind = ItemTypeKind.Item };
        }

        private static OccurrenceIndicator GetOccurrence(XQuery4Parser.OccurrenceIndicatorContext ctx)
        {
            var text = ctx.GetText();
            return text switch { "?" => OccurrenceIndicator.ZeroOrOne, "*" => OccurrenceIndicator.ZeroOrMore, "+" => OccurrenceIndicator.OneOrMore, _ => OccurrenceIndicator.ExactlyOne };
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private (string name, string prefix, SequenceTypeNode seqType) GetVarNameAndType(XQuery4Parser.VarNameAndTypeContext ctx)
        {
            var (local, prefix, _) = SplitEqName(ctx.eqName().GetText());
            var seqType = ctx.typeDeclaration() is { } td ? BuildSequenceType(td.sequenceType()) : null;
            return (local, prefix, seqType);
        }

        private (string name, string prefix, SequenceTypeNode seqType) GetVarNameAndType(
            XQuery4Parser.EqNameContext nameCtx, XQuery4Parser.TypeDeclarationContext tdCtx)
        {
            var (local, prefix, _) = SplitEqName(nameCtx.GetText());
            var seqType = tdCtx != null ? BuildSequenceType(tdCtx.sequenceType()) : null;
            return (local, prefix, seqType);
        }

        private XdmQName ResolveEqName(XQuery4Parser.EqNameContext ctx)
        {
            var (local, prefix, _) = SplitEqName(ctx.GetText());
            if (prefix == null) return new XdmQName(local);
            _ns.TryGetValue(prefix, out var ns);
            return new XdmQName(ns ?? string.Empty, local, prefix);
        }

        private (string local, string prefix, string ns) SplitEqName(string text)
        {
            // Q{uri}local or Q{uri}prefix:local
            if (text.StartsWith("Q{"))
            {
                var end = text.IndexOf('}');
                var uri  = end > 2 ? text[2..end] : string.Empty;
                var rest = text[(end + 1)..];
                var ci   = rest.IndexOf(':');
                if (ci >= 0) return (rest[(ci + 1)..], rest[..ci], uri);
                return (rest, null, uri);
            }
            var colon = text.IndexOf(':');
            if (colon < 0) return (text, null, null);
            var prefix = text[..colon];
            var local  = text[(colon + 1)..];
            _ns.TryGetValue(prefix, out var resolvedNs);
            return (local, prefix, resolvedNs);
        }

        private static string StripQuotes(string raw)
        {
            if (raw.Length >= 2 && ((raw[0] == '"' && raw[^1] == '"') || (raw[0] == '\'' && raw[^1] == '\'')))
                return raw[1..^1];
            return raw;
        }

        /// <summary>
        /// Resolves XML character references (&#N; &#xNN;) and predefined entity
        /// references (&amp; &lt; &gt; &quot; &apos;) in the body of an XQuery
        /// string literal, per XQuery 1.0 spec §2.6.
        /// </summary>
        private static string ResolveXmlEscapes(string s)
        {
            if (!s.Contains('&')) return s;
            var sb = new System.Text.StringBuilder(s.Length);
            int i = 0;
            while (i < s.Length)
            {
                if (s[i] != '&') { sb.Append(s[i++]); continue; }
                int semi = s.IndexOf(';', i + 1);
                if (semi < 0) { sb.Append(s[i++]); continue; }
                var entity = s[(i + 1)..semi];
                switch (entity)
                {
                    case "amp":  sb.Append('&');  break;
                    case "lt":   sb.Append('<');  break;
                    case "gt":   sb.Append('>');  break;
                    case "quot": sb.Append('"');  break;
                    case "apos": sb.Append('\''); break;
                    default:
                        if (entity.Length > 1 && entity[0] == '#')
                        {
                            bool hex = entity.Length > 2 && (entity[1] == 'x' || entity[1] == 'X');
                            var digits = hex ? entity[2..] : entity[1..];
                            if (int.TryParse(digits,
                                    hex ? System.Globalization.NumberStyles.HexNumber
                                        : System.Globalization.NumberStyles.Integer,
                                    null, out int cp))
                                sb.Append(char.ConvertFromUtf32(cp));
                            else
                                sb.Append('&').Append(entity).Append(';');
                        }
                        else
                            sb.Append('&').Append(entity).Append(';');
                        break;
                }
                i = semi + 1;
            }
            return sb.ToString();
        }
    }
}
