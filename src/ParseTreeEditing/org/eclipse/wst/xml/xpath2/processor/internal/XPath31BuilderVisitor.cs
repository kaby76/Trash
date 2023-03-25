using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Xpath;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;
using xpath.org.eclipse.wst.xml.xpath2.processor.@internal.ast;
using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
using AddExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AddExpr;
using AndExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AndExpr;
using AnyKindTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AnyKindTest;
using AttributeTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AttributeTest;
using AxisStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AxisStep;
using BinExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.BinExpr;
using CastExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CastExpr;
using CastableExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CastableExpr;
using CmpExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CmpExpr;
using CntxItemExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CntxItemExpr;
using CommentTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CommentTest;
using DecimalLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DecimalLiteral;
using DivExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DivExpr;
using DocumentTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DocumentTest;
using DoubleLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DoubleLiteral;
using ElementTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ElementTest;
using ExceptExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ExceptExpr;
using Expr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.Expr;
using FilterExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.FilterExpr;
using ForExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ForExpr;
using ForwardStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ForwardStep;
using FunctionCall = org.eclipse.wst.xml.xpath2.processor.@internal.ast.FunctionCall;
using IDivExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IDivExpr;
using IfExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IfExpr;
using InstOfExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.InstOfExpr;
using IntegerLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IntegerLiteral;
using IntersectExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IntersectExpr;
using ItemType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ItemType;
using MinusExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.MinusExpr;
using ModExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ModExpr;
using MulExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.MulExpr;
using NameTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.NameTest;
using OrExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.OrExpr;
using PITest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PITest;
using ParExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ParExpr;
using PipeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PipeExpr;
using PlusExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PlusExpr;
using QuantifiedExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.QuantifiedExpr;
using RangeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.RangeExpr;
using ReverseStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ReverseStep;
using SchemaAttrTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaAttrTest;
using SchemaElemTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaElemTest;
using SequenceType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SequenceType;
using SingleType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SingleType;
using StepExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StepExpr;
using StringLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StringLiteral;
using SubExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SubExpr;
using TextTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TextTest;
using TreatAsExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TreatAsExpr;
using UnionExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnionExpr;
using VarExprPair = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarExprPair;
using VarRef = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarRef;
using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
using XPathNode = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathNode;
using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;
using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
using LiteralUtils = org.eclipse.wst.xml.xpath2.processor.@internal.utils.LiteralUtils;

namespace xpath.org.eclipse.wst.xml.xpath2.processor.@internal
{

    /**
     *
     * @author sam
     */
    public class XPathBuilderVisitor : XPath31ParserBaseVisitor<object>
    {
        public static XPathBuilderVisitor INSTANCE = new XPathBuilderVisitor();

        // [1]
        public override object /* XPath */ VisitXpath(XPath31Parser.XpathContext ctx)
        {
            return new XPath((ICollection<Expr>)VisitExpr(ctx.expr()));
        }

        // [2]
        public override object VisitParamlist(XPath31Parser.ParamlistContext context)
        {
            throw new NotImplementedException();
        }

        // [3]
        public override object VisitParam(XPath31Parser.ParamContext context)
        {
            throw new NotImplementedException();
        }

        // [4]
        public override object VisitFunctionbody(XPath31Parser.FunctionbodyContext context)
        {
            throw new NotImplementedException();
        }

        // [5]
        public override object VisitEnclosedexpr(XPath31Parser.EnclosedexprContext context)
        {
            throw new NotImplementedException();
        }

        // [6]
        public override object /* ICollection<Expr> */ VisitExpr(XPath31Parser.ExprContext ctx)
        {
            ICollection<Expr> result = new List<Expr>();
            foreach (XPath31Parser.ExprsingleContext ex in ctx.exprsingle())
            {
                result.Add((Expr)VisitExprsingle(ex));
            }
            return result;
        }

        // [7]
        public override object /* Expr */ VisitExprsingle(XPath31Parser.ExprsingleContext ctx)
        {
            if (ctx.forexpr() != null)
            {
                return VisitForexpr(ctx.forexpr());
            }
            else if (ctx.quantifiedexpr() != null)
            {
                return VisitQuantifiedexpr(ctx.quantifiedexpr());
            }
            else if (ctx.ifexpr() != null)
            {
                return VisitIfexpr(ctx.ifexpr());
            }
            else
            {
                return VisitOrexpr(ctx.orexpr());
            }
        }

        // [8]
        public override object /* ForExpr */ VisitForexpr(XPath31Parser.ForexprContext ctx)
        {
            return new ForExpr((Collection<VarExprPair>)VisitSimpleforclause(ctx.simpleforclause()), (Expr)VisitExprsingle(ctx.exprsingle()));
        }

        // [9]
        public override object /* Collection<VarExprPair> */ VisitSimpleforclause(XPath31Parser.SimpleforclauseContext ctx)
        {
            Collection<VarExprPair> result = new Collection<VarExprPair>();
            foreach (var forbinding in ctx.simpleforbinding())
            {
                var b = VisitSimpleforbinding(forbinding);
                result.Add((VarExprPair)b);
            }
            return result;
        }

        // [10]
        public override object VisitSimpleforbinding(XPath31Parser.SimpleforbindingContext ctx)
        {
            var re = new VarExprPair((QName)VisitVarname(ctx.varname()), (Expr)VisitExprsingle(ctx.exprsingle()));
            return re;
        }

        // [11]
        public override object VisitLetexpr(XPath31Parser.LetexprContext context)
        {
            throw new NotImplementedException();
        }

        // [12]
        public override object VisitSimpleletclause(XPath31Parser.SimpleletclauseContext context)
        {
            throw new NotImplementedException();
        }

        // [13]
        public override object VisitSimpleletbinding(XPath31Parser.SimpleletbindingContext context)
        {
            throw new NotImplementedException();
        }

        // [14]
        public override object /* QuantifiedExpr */ VisitQuantifiedexpr(XPath31Parser.QuantifiedexprContext ctx)
        {
            var middle = new List<VarExprPair>();
            var varnames = ctx.varname();
            var exprsingles = ctx.exprsingle();
            for (int i = 0; i < varnames.Length; ++i)
            {
                var varname = (QName)VisitVarname(varnames[i]);
                var exprsingle = (Expr)VisitExprsingle(exprsingles[i]);
                var varexprpair = new VarExprPair(varname, exprsingle);
                middle.Add(varexprpair);
            }
            if (ctx.KW_SOME() != null)
            {
                return new QuantifiedExpr(QuantifiedExpr.SOME, middle, (Expr)VisitExprsingle(exprsingles[varnames.Length]));
            }
            else
            {
                return new QuantifiedExpr(QuantifiedExpr.ALL, middle, (Expr)VisitExprsingle(exprsingles[varnames.Length]));
            }
        }

        // [15]
        public override object /* IfExpr */ VisitIfexpr(XPath31Parser.IfexprContext ctx)
        {
            ICollection<Expr> condition = (ICollection<Expr>) VisitExpr(ctx.expr());
            Expr then = (Expr)VisitExprsingle(ctx.exprsingle(0));
            Expr els = (Expr)VisitExprsingle(ctx.exprsingle(1));
            return new IfExpr(condition, then, els);
        }

        // [16]
        public override object /* Expr */ VisitOrexpr(XPath31Parser.OrexprContext ctx)
        {
            Expr all = null;
            foreach (var a in ctx.andexpr())
            {
                Expr andExpr = (Expr)VisitAndexpr(a);
                if (all != null)
                {
                    all = new OrExpr(all, andExpr);
                }
                else
                {
                    all = andExpr;
                }
            }
            return all;
        }


        // [17]
        public override object /* Expr */ VisitAndexpr(XPath31Parser.AndexprContext ctx)
        {
            Expr all = null;
            foreach (var a in ctx.comparisonexpr())
            {
                Expr x = (Expr)VisitComparisonexpr(a);
                if (all != null)
                {
                    all = new AndExpr(all, x);
                }
                else
                {
                    all = x;
                }
            }
            return all;
        }

        // [18]
        public override object /* Expr */ VisitComparisonexpr(XPath31Parser.ComparisonexprContext ctx)
        {
            if (ctx.stringconcatexpr(1) == null)
            {
                return VisitStringconcatexpr(ctx.stringconcatexpr(0));
            }

            if (ctx.valuecomp() != null)
            {
                return new CmpExpr((Expr)VisitStringconcatexpr(ctx.stringconcatexpr(0)), (Expr)VisitStringconcatexpr(ctx.stringconcatexpr(1)),
                    (CmpExpr.Type)VisitValuecomp(ctx.valuecomp()));
            }
            else if (ctx.generalcomp() != null)
            {
                return new CmpExpr((Expr)VisitStringconcatexpr(ctx.stringconcatexpr(0)), (Expr)VisitStringconcatexpr(ctx.stringconcatexpr(1)),
                    (CmpExpr.Type)VisitGeneralcomp(ctx.generalcomp()));
            }
            else
            {
                return new CmpExpr((Expr)VisitStringconcatexpr(ctx.stringconcatexpr(0)), (Expr)VisitStringconcatexpr(ctx.stringconcatexpr(1)),
                    (CmpExpr.Type)VisitNodecomp(ctx.nodecomp()));
            }
        }

        // [19]
        public override object VisitStringconcatexpr(XPath31Parser.StringconcatexprContext ctx)
        {
            var rangeexpr = ctx.rangeexpr();
            if (rangeexpr.Length > 1)
                throw new NotImplementedException();
            return VisitRangeexpr(rangeexpr[0]);
        }

        // [20]
        public override object /* Expr */ VisitRangeexpr(XPath31Parser.RangeexprContext ctx)
        {
            if (ctx.KW_TO() == null)
            {
                return VisitAdditiveexpr(ctx.additiveexpr(0));
            }

            return new RangeExpr((Expr)VisitAdditiveexpr(ctx.additiveexpr(0)), (Expr)VisitAdditiveexpr(ctx.additiveexpr(1)));
        }

        // [21]
        public override object /* Expr */ VisitAdditiveexpr(XPath31Parser.AdditiveexprContext ctx)
        {
            Expr all = (Expr)VisitMultiplicativeexpr((XPath31Parser.MultiplicativeexprContext)ctx.GetChild(0));
            for (int i = 1; i < ctx.ChildCount; i += 2)
            {
                var o = ctx.GetChild(i);
                var a = (XPath31Parser.MultiplicativeexprContext)ctx.GetChild(i + 1);
                Expr x = (Expr)VisitMultiplicativeexpr(a);
                if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.PLUS)
                {
                    all = new AddExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.MINUS)
                {
                    all = new SubExpr(all, x);
                }
                else throw new Exception("Bad additiveexpr");
            }
            return all;
        }

        // [22]
        public override object /* Expr */ VisitMultiplicativeexpr(XPath31Parser.MultiplicativeexprContext ctx)
        {
            Expr all = (Expr)VisitUnionexpr((XPath31Parser.UnionexprContext)ctx.GetChild(0));
            for (int i = 1; i < ctx.ChildCount; i += 2)
            {
                var o = ctx.GetChild(i);
                var a = (XPath31Parser.UnionexprContext)ctx.GetChild(i + 1);
                Expr x = (Expr)VisitUnionexpr(a);
                if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.PLUS)
                {
                    all = new AddExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_DIV)
                {
                    all = new DivExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_IDIV)
                {
                    all = new IDivExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_MOD)
                {
                    all = new ModExpr(all, x);
                }
                else throw new Exception("Bad expr");
            }
            return all;
        }

        // [23]
        public override object /* Expr */ VisitUnionexpr(XPath31Parser.UnionexprContext ctx)
        {
            Expr all = (Expr)VisitIntersectexceptexpr((XPath31Parser.IntersectexceptexprContext)ctx.GetChild(0));
            for (int i = 1; i < ctx.ChildCount; i += 2)
            {
                var o = ctx.GetChild(i);
                var a = (XPath31Parser.IntersectexceptexprContext)ctx.GetChild(i + 1);
                Expr x = (Expr)VisitIntersectexceptexpr(a);
                if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_UNION)
                {
                    all = new UnionExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.P)
                {
                    all = new PipeExpr(all, x);
                }
                else throw new Exception("Bad expr");
            }
            return all;
        }

        // [24]
        public override object /* Expr */ VisitIntersectexceptexpr(XPath31Parser.IntersectexceptexprContext ctx)
        {
            Expr all = (Expr)VisitInstanceofexpr((XPath31Parser.InstanceofexprContext)ctx.GetChild(0));
            for (int i = 1; i < ctx.ChildCount; i += 2)
            {
                var o = ctx.GetChild(i);
                var a = (XPath31Parser.InstanceofexprContext)ctx.GetChild(i + 1);
                Expr x = (Expr)VisitInstanceofexpr(a);
                if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_INTERSECT)
                {
                    all = new IntersectExpr(all, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.KW_EXCEPT)
                {
                    all = new ExceptExpr(all, x);
                }
                else throw new Exception("Bad expr");
            }
            return all;
        }

        // [25]
        public override object /* Expr */ VisitInstanceofexpr(XPath31Parser.InstanceofexprContext ctx)
        {
            Expr treatExpr = (Expr)VisitTreatexpr(ctx.treatexpr());
            if (ctx.KW_INSTANCE() == null)
            {
                return treatExpr;
            }

            return new InstOfExpr(treatExpr, (SequenceType)VisitSequencetype(ctx.sequencetype()));
        }

        // [26]
        public override object /* Expr */ VisitTreatexpr(XPath31Parser.TreatexprContext ctx)
        {
            Expr castableExpr = (Expr)VisitCastableexpr(ctx.castableexpr());
            if (ctx.KW_TREAT() == null)
            {
                return castableExpr;
            }
            return new TreatAsExpr(castableExpr, (SequenceType)VisitSequencetype(ctx.sequencetype()));
        }

        // [27]
        public override object /* Expr */ VisitCastableexpr(XPath31Parser.CastableexprContext ctx)
        {
            Expr castExpr = (Expr)VisitCastexpr(ctx.castexpr());
            if (ctx.KW_CASTABLE() == null)
            {
                return castExpr;
            }
            return new CastableExpr(castExpr, (SingleType)VisitSingletype(ctx.singletype()));
        }

        // [28]
        public override object /* Expr */ VisitCastexpr(XPath31Parser.CastexprContext ctx)
        {
            Expr unaryExpr = (Expr)VisitArrowexpr(ctx.arrowexpr());
            if (ctx.KW_CAST() == null)
            {
                return unaryExpr;
            }
            return new CastExpr(unaryExpr, (SingleType)VisitSingletype(ctx.singletype()));
        }

        // [29]
        public override object VisitArrowexpr(XPath31Parser.ArrowexprContext ctx)
        {
            var unaryexpr = ctx.unaryexpr();
            var argumentlist = ctx.argumentlist();
            if (argumentlist.Length > 0)
                throw new NotImplementedException();
            return VisitUnaryexpr(unaryexpr);
        }

        // [30]
        public override object /* Expr */ VisitUnaryexpr(XPath31Parser.UnaryexprContext ctx)
        {
            Expr all = (Expr) VisitValueexpr(ctx.valueexpr());
            for (int i = 0; i < ctx.ChildCount - 1; ++i)
            {
                if ((ctx.GetChild(i) as TerminalNodeImpl)?.Symbol.Type == XPath31Parser.MINUS)
                {
                    all = new MinusExpr(all);
                }
                else if (ctx.PLUS() != null)
                {
                    all = new PlusExpr(all);
                }
                else
                {
                    throw new Exception("Bad expr");
                }
            }
            return all;
        }

        // [31]
        public override object /* Expr */ VisitValueexpr(XPath31Parser.ValueexprContext ctx)
        {
            return VisitSimplemapexpr(ctx.simplemapexpr());
        }

        // [32]
        public override object /* int */ VisitGeneralcomp(XPath31Parser.GeneralcompContext ctx)
        {
            switch ((ctx.GetChild(0) as TerminalNodeImpl)?.Symbol.Type)
            {
                case XPath31Lexer.EQ:
                    return CmpExpr.Type.EQUALS;
                case XPath31Lexer.NE:
                    return CmpExpr.Type.NOTEQUALS;
                case XPath31Lexer.LT:
                    return CmpExpr.Type.LESSTHAN;
                case XPath31Lexer.LE:
                    return CmpExpr.Type.LESSEQUAL;
                case XPath31Lexer.GT:
                    return CmpExpr.Type.GREATER;
                case XPath31Lexer.GE:
                    return CmpExpr.Type.GREATEREQUAL;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        // [33]
        public override object /* int */ VisitValuecomp(XPath31Parser.ValuecompContext ctx)
        {
            switch ((ctx.GetChild(0) as TerminalNodeImpl)?.Symbol.Type)
            {
                case XPath31Lexer.EQ:
                    return CmpExpr.Type.EQ;
                case XPath31Lexer.NE:
                    return CmpExpr.Type.NE;
                case XPath31Lexer.LT:
                    return CmpExpr.Type.LT;
                case XPath31Lexer.LE:
                    return CmpExpr.Type.LE;
                case XPath31Lexer.GT:
                    return CmpExpr.Type.GT;
                case XPath31Lexer.GE:
                    return CmpExpr.Type.GE;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        // [34]
        public override object /* int */ VisitNodecomp(XPath31Parser.NodecompContext ctx)
        {
            if (ctx.KW_IS() != null)
            {
                return CmpExpr.Type.IS;
            }
            else if (ctx.LL() != null)
            {
                return CmpExpr.Type.LESS_LESS;
            }
            else if (ctx.GG() != null)
            {
                return CmpExpr.Type.GREATER_GREATER;
            }
            else
            {
                throw new Exception("bad expr");
            }
        }

        // [35]
        public override object VisitSimplemapexpr(XPath31Parser.SimplemapexprContext ctx)
        {
            var pathexpr = ctx.pathexpr();
            if (pathexpr.Length > 1)
                throw new NotImplementedException();
            return VisitPathexpr(pathexpr[0]);
        }

        // [36]
        public override object /* Expr */ VisitPathexpr(XPath31Parser.PathexprContext ctx)
        {
            if (ctx.relativepathexpr() != null)
            {
                XPathExpr relativePathExpr = (XPathExpr)VisitRelativepathexpr(ctx.relativepathexpr());
                if (ctx.SLASH() != null)
                {
                    relativePathExpr.set_slashes(1);
                }
                else if (ctx.SS() != null)
                {
                    relativePathExpr.set_slashes(2);
                }
                return relativePathExpr;
            }
            else
            {
                return new XPathExpr(1, false, null);
            }
        }

        // [37]
        public override object /* XPathExpr */ VisitRelativepathexpr(XPath31Parser.RelativepathexprContext ctx)
        {
            StepExpr all = (StepExpr)VisitStepexpr(ctx.stepexpr()[0]);
            XPathExpr relativePathExpr = new XPathExpr(0, false, all);
            for (int i = 1; i < ctx.ChildCount; i += 2)
            {
                var o = ctx.GetChild(i);
                var a = (XPath31Parser.StepexprContext)ctx.GetChild(i + 1);
                StepExpr x = (StepExpr)VisitStepexpr(a);
                if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.SLASH)
                {
                    relativePathExpr.add_tail(1, x);
                }
                else if ((o as TerminalNodeImpl).Symbol.Type == XPath31Lexer.SS)
                {
                    relativePathExpr.add_tail(2, x);
                }
                else throw new Exception("Bad additiveexpr");
            }
            return relativePathExpr;
        }

        // [38]
        public override object /* StepExpr */ VisitStepexpr(XPath31Parser.StepexprContext ctx)
        {
            if (ctx.axisstep() != null)
            {
                var r = VisitAxisstep(ctx.axisstep());
                if (!(r is StepExpr))
                    throw new Exception();
                return r;
            }
            else
            {
                var r = VisitPostfixexpr(ctx.postfixexpr());
                if (!(r is StepExpr))
                    throw new Exception();
                return r;
            }
        }

        // [39]
        public override object /* AxisStep */ VisitAxisstep(XPath31Parser.AxisstepContext ctx)
        {
            if (ctx.forwardstep() != null)
            {
                return new AxisStep((Step)VisitForwardstep(ctx.forwardstep()), (ICollection<ICollection<Expr>>)VisitPredicatelist(ctx.predicatelist()));
            }
            else
            {
                return new AxisStep((Step)VisitReversestep(ctx.reversestep()), (ICollection<ICollection<Expr>>)VisitPredicatelist(ctx.predicatelist()));
            }
        }

        // [40]
        public override object /* ForwardStep */ VisitForwardstep(XPath31Parser.ForwardstepContext ctx)
        {
            if (ctx.forwardaxis() != null)
            {
                return new ForwardStep((ForwardStep.Type)VisitForwardaxis(ctx.forwardaxis()), (NodeTest)VisitNodetest(ctx.nodetest()));
            }
            else
            {
                return VisitAbbrevforwardstep(ctx.abbrevforwardstep());
            }
        }

        // [41]
        public override object /* Integer */ VisitForwardaxis(XPath31Parser.ForwardaxisContext ctx)
        {
            switch ((ctx.GetChild(0) as TerminalNodeImpl).Symbol.Type)
            {
                case XPath31Lexer.KW_CHILD:
                    return ForwardStep.Type.CHILD;
                case XPath31Lexer.KW_DESCENDANT:
                    return ForwardStep.Type.DESCENDANT;
                case XPath31Lexer.KW_ATTRIBUTE:
                    return ForwardStep.Type.ATTRIBUTE;
                case XPath31Lexer.KW_SELF:
                    return ForwardStep.Type.SELF;
                case XPath31Lexer.KW_DESCENDANT_OR_SELF:
                    return ForwardStep.Type.DESCENDANT_OR_SELF;
                case XPath31Lexer.KW_FOLLOWING_SIBLING:
                    return ForwardStep.Type.FOLLOWING_SIBLING;
                case XPath31Lexer.KW_FOLLOWING:
                    return ForwardStep.Type.FOLLOWING;
                case XPath31Lexer.KW_NAMESPACE:
                    return ForwardStep.Type.NAMESPACE;
                default:
                    Debug.Assert(false);
                    return ForwardStep.Type.NONE;
            }
        }

        // [42]
        public override object /* ForwardStep */ VisitAbbrevforwardstep(XPath31Parser.AbbrevforwardstepContext ctx)
        {
            NodeTest nodeTest = (NodeTest)VisitNodetest(ctx.nodetest());
            if (ctx.AT() != null)
            {
                return new ForwardStep(ForwardStep.Type.AT_SYM, nodeTest);
            }
            else
            {
                return new ForwardStep(ForwardStep.Type.NONE, nodeTest);
            }
        }

        // [43]
        public override object /* ReverseStep */ VisitReversestep(XPath31Parser.ReversestepContext ctx)
        {
            if (ctx.reverseaxis() != null)
            {
                return new ReverseStep((ReverseStep.Type)VisitReverseaxis(ctx.reverseaxis()), (NodeTest)VisitNodetest(ctx.nodetest()));
            }
            else
            {
                return VisitAbbrevreversestep(ctx.abbrevreversestep());
            }
        }

        // [44]
        public override object /* Integer */ VisitReverseaxis(XPath31Parser.ReverseaxisContext ctx)
        {
            switch ((ctx.GetChild(0) as TerminalNodeImpl).Symbol.Type)
            {
                case XPath31Lexer.KW_PARENT:
                    return ReverseStep.Type.PARENT;
                case XPath31Lexer.KW_ANCESTOR:
                    return ReverseStep.Type.ANCESTOR;
                case XPath31Lexer.KW_PRECEDING_SIBLING:
                    return ReverseStep.Type.PRECEDING_SIBLING;
                case XPath31Lexer.KW_PRECEDING:
                    return ReverseStep.Type.PRECEDING;
                case XPath31Lexer.KW_ANCESTOR_OR_SELF:
                    return ReverseStep.Type.ANCESTOR_OR_SELF;
                default:
                    Debug.Assert(false);
                    return ReverseStep.Type.DOTDOT;
            }
        }
     
        // [45]
        public override object /* ReverseStep */ VisitAbbrevreversestep(XPath31Parser.AbbrevreversestepContext ctx)
        {
            return new ReverseStep(ReverseStep.Type.DOTDOT, null);
        }

        // [46]
        public override object /* NodeTest */ VisitNodetest(XPath31Parser.NodetestContext ctx)
        {
            if (ctx.kindtest() != null)
            {
                return VisitKindtest(ctx.kindtest());
            }
            else
            {
                return VisitNametest(ctx.nametest());
            }
        }

        // [47]
        public override object /* NameTest */ VisitNametest(XPath31Parser.NametestContext ctx)
        {
            if (ctx.eqname() != null)
            {
                return new NameTest((QName)VisitEqname(ctx.eqname()));
            }
            else
            {
                return new NameTest((QName)VisitWildcard(ctx.wildcard()));
            }
        }

        // [48]
        public override object /* QName */ VisitWildcard(XPath31Parser.WildcardContext ctx)
        {
            if (ctx.BracedURILiteral() != null)
            {
                return new QName(ctx.BracedURILiteral().GetText(), "*");
            }
            else if (ctx.STAR() != null)
            {
                return new QName("*");
            }
            else if (ctx.CS() != null)
            {
                return new QName(ctx.NCName().GetText(), ctx.CS().GetText());
            }
            else if (ctx.SC() != null)
            {
                return new QName(ctx.SC().GetText(), ctx.NCName().GetText());
            }
            else throw new Exception("bad expr");
        }

        // [49]
        public override object VisitPostfixexpr(XPath31Parser.PostfixexprContext ctx)
        {
            var primaryexpr = VisitPrimaryexpr(ctx.primaryexpr());
            var rest = new List<ICollection<Expr>>();
            for (int i = 1; i < ctx.ChildCount; ++i)
            {
                var c = ctx.GetChild(i);
                if (c is XPath31Parser.PredicateContext)
                {
                    var predicate = (ICollection<Expr>)VisitPredicate(c as XPath31Parser.PredicateContext);
                    rest.Add(predicate);
                }
                else if (c is XPath31Parser.ArgumentlistContext)
                {
                    var predicate = (ICollection<Expr>)VisitArgumentlist(c as XPath31Parser.ArgumentlistContext);
                    rest.Add(predicate);
                }
                else if (c is XPath31Parser.LookupContext)
                {
                    var predicate = (ICollection<Expr>)VisitLookup(c as XPath31Parser.LookupContext);
                    rest.Add(predicate);
                }
                else throw new Exception();
            }
            var result = new PostfixExpr(primaryexpr, rest);
            return result;
        }

        // [50]
        public override object VisitArgumentlist(XPath31Parser.ArgumentlistContext ctx)
        {
            var list = new List<Expr>();
            foreach (var arg in ctx.argument())
            {
                var r = VisitArgument(arg);
                list.Add((Expr)r);
            }
            return list;
        }

        // [51]
        public override object /* ICollection<ICollection<Expr>> */ VisitPredicatelist(XPath31Parser.PredicatelistContext ctx)
        {
            ICollection<ICollection<Expr>> result = new List<ICollection<Expr>>();
            foreach (XPath31Parser.PredicateContext predicate in ctx.predicate())
            {
                result.Add((ICollection<Expr>)VisitPredicate(predicate));
            }
            return result;
        }

        // [52]
        public override object /* ICollection<Expr> */ VisitPredicate(XPath31Parser.PredicateContext ctx)
        {
            var result = VisitExpr(ctx.expr());
            return result;
        }

        // [53]
        public override object /* ICollection<Expr> */ VisitLookup(XPath31Parser.LookupContext context)
        {
            throw new NotImplementedException();
        }

        // [54]
        public override object VisitKeyspecifier(XPath31Parser.KeyspecifierContext context)
        {
            throw new NotImplementedException();
        }

        // [55]
        public override object VisitArrowfunctionspecifier(XPath31Parser.ArrowfunctionspecifierContext context)
        {
            throw new NotImplementedException();
        }

        // [56]
        public override object /* PrimaryExpr */ VisitPrimaryexpr(XPath31Parser.PrimaryexprContext ctx)
        {
            if (ctx.literal() != null)
            {
                return VisitLiteral(ctx.literal());
            }
            else if (ctx.varref() != null)
            {
                return VisitVarref(ctx.varref());
            }
            else if (ctx.parenthesizedexpr() != null)
            {
                return new ParExpr((ICollection<Expr>)VisitParenthesizedexpr(ctx.parenthesizedexpr()));
            }
            else if (ctx.contextitemexpr() != null)
            {
                return VisitContextitemexpr(ctx.contextitemexpr());
            }
            else if (ctx.functioncall() != null)
            {
                return VisitFunctioncall(ctx.functioncall());
            }
            else if (ctx.functionitemexpr() != null)
            {
                return VisitFunctionitemexpr(ctx.functionitemexpr());
            }
            else if (ctx.arrayconstructor() != null)
            {
                return VisitArrayconstructor(ctx.arrayconstructor());
            }
            else if (ctx.unarylookup() != null)
            {
                return VisitUnarylookup(ctx.unarylookup());
            }
            else
            {
                throw new Exception("Bad");
            }
        }

        // [57]
        public override object /* Literal */ VisitLiteral(XPath31Parser.LiteralContext ctx)
        {
            if (ctx.numericliteral() != null)
            {
                return VisitNumericliteral(ctx.numericliteral());
            }
            else if (ctx.StringLiteral() != null)
            {
                return new StringLiteral(LiteralUtils.unquote(ctx.StringLiteral().GetText()));
            }
            else
            {
                throw new Exception("Bad");
            }
        }

        // [58]
        public override object /* NumericLiteral */ VisitNumericliteral(XPath31Parser.NumericliteralContext ctx)
        {
            if (ctx.IntegerLiteral() != null)
            {
                BigInteger.TryParse(ctx.IntegerLiteral().GetText(), out BigInteger value);
                return new IntegerLiteral(value);
            }
            else if (ctx.DecimalLiteral() != null)
            {
                decimal.TryParse(ctx.DecimalLiteral().GetText(), out decimal value);
                return new DecimalLiteral(value);
            }
            else if (ctx.DoubleLiteral() != null)
            {
                Double.TryParse(ctx.DoubleLiteral().GetText(), out double value);
                return new DoubleLiteral(value);
            }
            else throw new Exception();
        }

        // [59]
        public override object /* VarRef */ VisitVarref(XPath31Parser.VarrefContext ctx)
        {
            return new VarRef((QName)VisitVarname(ctx.varname()));
        }

        // [60]
        public override object /* QName */ VisitVarname(XPath31Parser.VarnameContext ctx)
        {
            return VisitEqname(ctx.eqname());
        }

        // [61]
        public override object /* ICollection<Expr> */ VisitParenthesizedexpr(XPath31Parser.ParenthesizedexprContext ctx)
        {
            if (ctx.expr() == null)
            {
                return new ArrayList<Expr>();
            }
            return VisitExpr(ctx.expr());
        }

        // [62]
        public override object /* CntxItemExpr */ VisitContextitemexpr(XPath31Parser.ContextitemexprContext ctx)
        {
            return new CntxItemExpr();
        }

        // [63]
        public override object /* FunctionCall */ VisitFunctioncall(XPath31Parser.FunctioncallContext ctx)
        {
            return new FunctionCall((QName)VisitEqname(ctx.eqname()), (ICollection<Expr>)VisitArgumentlist(ctx.argumentlist()));
        }

        // [64]
        public override object VisitArgument(XPath31Parser.ArgumentContext ctx)
        {
            if (ctx.argumentplaceholder() != null)
            {
                return VisitArgumentplaceholder(ctx.argumentplaceholder());
            }
            else
            {
                return VisitExprsingle(ctx.exprsingle());
            }
        }

        // [65]
        public override object VisitArgumentplaceholder(XPath31Parser.ArgumentplaceholderContext context)
        {
            throw new NotImplementedException();
        }

        // [66]
        public override object VisitFunctionitemexpr(XPath31Parser.FunctionitemexprContext context)
        {
            throw new NotImplementedException();
        }

        // [67]
        public override object VisitNamedfunctionref(XPath31Parser.NamedfunctionrefContext context)
        {
            throw new NotImplementedException();
        }

        // [68]
        public override object VisitInlinefunctionexpr(XPath31Parser.InlinefunctionexprContext context)
        {
            throw new NotImplementedException();
        }

        // [69]
        public override object VisitMapconstructor(XPath31Parser.MapconstructorContext context)
        {
            throw new NotImplementedException();
        }

        // [70]
        public override object VisitMapconstructorentry(XPath31Parser.MapconstructorentryContext context)
        {
            throw new NotImplementedException();
        }

        // [71]
        public override object VisitMapkeyexpr(XPath31Parser.MapkeyexprContext context)
        {
            throw new NotImplementedException();
        }

        // [72]
        public override object VisitMapvalueexpr(XPath31Parser.MapvalueexprContext context)
        {
            throw new NotImplementedException();
        }

        // [73]
        public override object VisitArrayconstructor(XPath31Parser.ArrayconstructorContext context)
        {
            throw new NotImplementedException();
        }

        // [74]
        public override object VisitSquarearrayconstructor(XPath31Parser.SquarearrayconstructorContext context)
        {
            throw new NotImplementedException();
        }

        // [75]
        public override object VisitCurlyarrayconstructor(XPath31Parser.CurlyarrayconstructorContext context)
        {
            throw new NotImplementedException();
        }

        // [76]
        public override object VisitUnarylookup(XPath31Parser.UnarylookupContext context)
        {
            throw new NotImplementedException();
        }

        // [77]
        public override object /* SingleType */ VisitSingletype(XPath31Parser.SingletypeContext ctx)
        {
            QName a = (QName)VisitSimpletypename(ctx.simpletypename());
            if (ctx.QM() != null)
            {
                return new SingleType(a, true);
            }
            else
            {
                return new SingleType(a);
            }
        }

        // [78]
        public override object VisitTypedeclaration(XPath31Parser.TypedeclarationContext context)
        {
            throw new NotImplementedException();
        }

        // [79]
        public override object /* SequenceType */ VisitSequencetype(XPath31Parser.SequencetypeContext ctx)
        {
            if (ctx.itemtype() != null)
            {
                ItemType itemType = (ItemType)VisitItemtype(ctx.itemtype());
                if (ctx.occurrenceindicator() != null)
                {
                    return new SequenceType((int)VisitOccurrenceindicator(ctx.occurrenceindicator()), itemType);
                }
                else
                {
                    return new SequenceType(SequenceType.NONE, itemType);
                }
            }
            else
            {
                return new SequenceType(SequenceType.EMPTY, null);
            }
        }

        // [80]
        public override object /* Integer */ VisitOccurrenceindicator(XPath31Parser.OccurrenceindicatorContext ctx)
        {
            if (ctx.QM() != null)
            {
                return SequenceType.QUESTION;
            }
            else if (ctx.STAR() != null)
            {
                return SequenceType.STAR;
            }
            else if (ctx.PLUS() != null)
            {
                return SequenceType.PLUS;
            }
            else throw new Exception();
        }

        // [81]
        public override object /* ItemType */ VisitItemtype(XPath31Parser.ItemtypeContext ctx)
        {
            if (ctx.kindtest() != null)
            {
                return new ItemType(ItemType.KINDTEST, VisitKindtest(ctx.kindtest()));
            }
            else if (ctx.maptest() != null)
            {
                return new ItemType(ItemType.MAPTEST, VisitMaptest(ctx.maptest()));
            }
            else if (ctx.functiontest() != null)
            {
                return new ItemType(ItemType.FUNCTIONTEST, VisitFunctiontest(ctx.functiontest()));
            }
            else if (ctx.arraytest() != null)
            {
                return new ItemType(ItemType.ARRAYTEST, VisitArraytest(ctx.arraytest()));
            }
            else if (ctx.atomicoruniontype() != null)
            {
                return new ItemType(ItemType.ATOMICORUNIONTEST, VisitAtomicoruniontype(ctx.atomicoruniontype()));
            }
            else if (ctx.parenthesizeditemtype() != null)
            {
                return new ItemType(ItemType.PARENTHESIZEDITEMTYPE, VisitParenthesizeditemtype(ctx.parenthesizeditemtype()));
            }
            else if (ctx.KW_ITEM() != null)
            {
                return new ItemType(ItemType.ITEM, null);
            }
            else throw new Exception();
        }

        // [82]
        public override object VisitAtomicoruniontype(XPath31Parser.AtomicoruniontypeContext context)
        {
            throw new NotImplementedException();
        }

        // [83]
        public override object /* KindTest */ VisitKindtest(XPath31Parser.KindtestContext ctx)
        {
            if (ctx.documenttest() != null)
            {
                return VisitDocumenttest(ctx.documenttest());
            }
            else if (ctx.elementtest() != null)
            {
                return VisitElementtest(ctx.elementtest());
            }
            else if (ctx.attributetest() != null)
            {
                return VisitAttributetest(ctx.attributetest());
            }
            else if (ctx.schemaelementtest() != null)
            {
                return VisitSchemaelementtest(ctx.schemaelementtest());
            }
            else if (ctx.schemaattributetest() != null)
            {
                return VisitSchemaattributetest(ctx.schemaattributetest());
            }
            else if (ctx.pitest() != null)
            {
                return VisitPitest(ctx.pitest());
            }
            else if (ctx.commenttest() != null)
            {
                return VisitCommenttest(ctx.commenttest());
            }
            else if (ctx.texttest() != null)
            {
                return VisitTexttest(ctx.texttest());
            }
            else if (ctx.namespacenodetest() != null)
            {
                return VisitNamespacenodetest(ctx.namespacenodetest());
            }
            else if (ctx.anykindtest() != null)
            {
                return VisitAnykindtest(ctx.anykindtest());
            }
            else
            {
                throw new Exception();
            }
        }

        // [84]
        public override object /* AnyKindTest */VisitAnykindtest(XPath31Parser.AnykindtestContext ctx)
        {
            return new AnyKindTest();
        }

        // [85]
        public override object /* DocumentTest */ VisitDocumenttest(XPath31Parser.DocumenttestContext ctx)
        {
            if (ctx.elementtest() != null)
            {
                return new DocumentTest(DocumentTest.ELEMENT, VisitElementtest(ctx.elementtest()));
            }
            else if (ctx.schemaelementtest() != null)
            {
                return new DocumentTest(DocumentTest.SCHEMA_ELEMENT, VisitSchemaelementtest(ctx.schemaelementtest()));
            }
            else
            {
                return new DocumentTest();
            }
        }

        // [86]
        public override object /* TextTest */ VisitTexttest(XPath31Parser.TexttestContext ctx)
        {
            return new TextTest();
        }

        // [87]
        public override object /* CommentTest */ VisitCommenttest(XPath31Parser.CommenttestContext ctx)
        {
            return new CommentTest();
        }

        // [88]
        public override object VisitNamespacenodetest(XPath31Parser.NamespacenodetestContext context)
        {
            throw new NotImplementedException();
        }

        // [89]
        public override object /* PITest */ VisitPitest(XPath31Parser.PitestContext ctx)
        {
            if (ctx.NCName() != null)
            {
                return new PITest(ctx.NCName().GetText());
            }
            else if (ctx.StringLiteral() != null)
            {
                return new PITest(new StringLiteral(LiteralUtils.unquote(ctx.StringLiteral().GetText())).ToString());
            }
            else
            {
                return new PITest();
            }
        }

        // [90]
        public override object /* AttributeTest */ VisitAttributetest(XPath31Parser.AttributetestContext ctx)
        {
            if (ctx.attribnameorwildcard() != null)
            {
                QName attribNameOrWildcard = (QName)VisitAttribnameorwildcard(ctx.attribnameorwildcard());
                bool wildcard = attribNameOrWildcard == null;
                if (ctx.typename() != null)
                {
                    return new AttributeTest(attribNameOrWildcard, wildcard, (QName)VisitTypename(ctx.typename()));
                }
                else
                {
                    return new AttributeTest(attribNameOrWildcard, wildcard);
                }
            }
            else
            {
                return new AttributeTest();
            }
        }

        // [91]
        public override object /* QName */ VisitAttribnameorwildcard(XPath31Parser.AttribnameorwildcardContext ctx)
        {
            if (ctx.attributename() != null)
            {
                return VisitAttributename(ctx.attributename());
            }
            else
            {
                return null;
            }
        }

        // [92]
        public override object /* SchemaAttrTest */ VisitSchemaattributetest(XPath31Parser.SchemaattributetestContext ctx)
        {
            return new SchemaAttrTest((QName)VisitAttributedeclaration(ctx.attributedeclaration()));
        }

        // [93]
        public override object /* QName */ VisitAttributedeclaration(XPath31Parser.AttributedeclarationContext ctx)
        {
            return VisitAttributename(ctx.attributename());
        }

        // [94]
        public override object /* ElementTest */ VisitElementtest(XPath31Parser.ElementtestContext ctx)
        {
            if (ctx.elementnameorwildcard() != null)
            {
                QName elementNameOrWildcard = (QName)VisitElementnameorwildcard(ctx.elementnameorwildcard());
                bool wildcard = elementNameOrWildcard == null;
                if (ctx.typename() != null)
                {
                    QName typeName = (QName)VisitTypename(ctx.typename());
                    if (ctx.QM() != null)
                    {
                        return new ElementTest(elementNameOrWildcard, wildcard, typeName, true);
                    }
                    else
                    {
                        return new ElementTest(elementNameOrWildcard, wildcard, typeName);
                    }
                }
                else
                {
                    return new ElementTest(elementNameOrWildcard, wildcard);
                }
            }
            else
            {
                return new ElementTest();
            }
        }

        // [95]
        public override object /* QName */ VisitElementnameorwildcard(XPath31Parser.ElementnameorwildcardContext ctx)
        {
            if (ctx.elementname() != null)
            {
                return VisitElementname(ctx.elementname());
            }
            else
            {
                return null;
            }
        }

        // [96]
        public override object /* SchemaElemTest */ VisitSchemaelementtest(XPath31Parser.SchemaelementtestContext ctx)
        {
            return new SchemaElemTest((QName)VisitElementdeclaration(ctx.elementdeclaration()));
        }

        // [97]
        public override object /* QName */ VisitElementdeclaration(XPath31Parser.ElementdeclarationContext ctx)
        {
            return VisitElementname(ctx.elementname());
        }

        // [98]
        public override object /* QName */ VisitAttributename(XPath31Parser.AttributenameContext ctx)
        {
            return VisitEqname(ctx.eqname());
        }

        // [99]
        public override object /* QName */ VisitElementname(XPath31Parser.ElementnameContext ctx)
        {
            return VisitEqname(ctx.eqname());
        }

        // [100]
        public override /* QName */ object VisitSimpletypename(XPath31Parser.SimpletypenameContext ctx)
        {
            return VisitTypename(ctx.typename());
        }

        // [101]
        public override object /* QName */ VisitTypename(XPath31Parser.TypenameContext ctx)
        {
            return VisitEqname(ctx.eqname());
        }

        // [102]
        public override object VisitFunctiontest(XPath31Parser.FunctiontestContext ctx)
        {
            if (ctx.anyfunctiontest() != null)
                return VisitAnyfunctiontest(ctx.anyfunctiontest());
            else
                return VisitTypedfunctiontest(ctx.typedfunctiontest());
        }

        // [103]
        public override object VisitAnyfunctiontest(XPath31Parser.AnyfunctiontestContext ctx)
        {
            return base.VisitAnyfunctiontest(ctx);
        }

        // [104]
        public override object VisitTypedfunctiontest(XPath31Parser.TypedfunctiontestContext ctx)
        {
            return base.VisitTypedfunctiontest(ctx);
        }

        // [105]
        public override object VisitMaptest(XPath31Parser.MaptestContext ctx)
        {
            if (ctx.anymaptest() != null)
                return VisitAnymaptest(ctx.anymaptest());
            else
                return VisitTypedmaptest(ctx.typedmaptest());
        }

        // [106]
        public override object /* ? */ VisitAnymaptest(XPath31Parser.AnymaptestContext ctx)
        {
            return null;
        }

        // [107]
        public override object VisitTypedmaptest(XPath31Parser.TypedmaptestContext context)
        {
            throw new NotImplementedException();
        }

        // [108]
        public override object VisitArraytest(XPath31Parser.ArraytestContext context)
        {
            throw new NotImplementedException();
        }

        // [109]
        public override object VisitAnyarraytest(XPath31Parser.AnyarraytestContext context)
        {
            throw new NotImplementedException();
        }

        // [110]
        public override object VisitTypedarraytest(XPath31Parser.TypedarraytestContext context)
        {
            throw new NotImplementedException();
        }

        // [111]
        public override object VisitParenthesizeditemtype(XPath31Parser.ParenthesizeditemtypeContext context)
        {
            throw new NotImplementedException();
        }

        // [112]
        public override object /* String */ VisitEqname(XPath31Parser.EqnameContext ctx)
        {
            if (ctx.QName() != null)
            {
                var name = ctx.GetText();
                var index_of_colon = name.IndexOf(':');
                if (index_of_colon < 0)
                {
                    return new QName(name);
                }
                else
                {
                    var p1 = name.Substring(0, index_of_colon);
                    var p2 = name.Substring(index_of_colon + 1);
                    return new QName(p1, p2);
                }
            }
            else
            {
                var name = ctx.GetText();
                return new QName(name);
            }
        }

        //public override object /* String */ VisitPrefix(XPath31Parser. PrefixContext ctx)
        //{
        //    return visitNCName(ctx.nCName());
        //}



        //public override QName visitQName(XPath31Parser.QNameContext ctx)
        //{
        //    if (ctx.COLON() == null)
        //    {
        //        return new QName(visitNCName(ctx.nCName(0)));
        //    }
        //    else
        //    {
        //        return new QName(visitNCName(ctx.nCName(0)), visitNCName(ctx.nCName(1)));
        //    }
        //}


        //public override object /* QName */ VisitAtomictype(XPath31Parser.AtomicoruniontypeContext ctx)
        //{
        //    return visitQName(ctx.qName());
        //}



        //public override object /* Collection<VarExprPair> */ VisitQuantifiedexprmiddle(XPath31Parser.qu ctx)
        //{
        //    Collection<VarExprPair> result;
        //    if (ctx.quantifiedExprMiddle() != null)
        //    {
        //        result = visitQuantifiedExprMiddle(ctx.quantifiedExprMiddle());
        //    }
        //    else
        //    {
        //        result = new ArrayList<VarExprPair>();
        //    }

        //    result.add(new VarExprPair(visitVarName(ctx.varName()), visitExprSingle(ctx.exprSingle())));
        //    return result;
        //}



        //public override QName visitUnreservedQName(XPath31Parser.UnreservedQNameContext ctx)
        //{
        //    if (ctx.COLON() != null)
        //    {
        //        return new QName(visitNCName(ctx.nCName(0)), visitNCName(ctx.nCName(1)));
        //    }
        //    else
        //    {
        //        return new QName(visitUnreservedNCName(ctx.unreservedNCName()));
        //    }
        //}

        //public override String visitNCName(XPath31Parser.NCNameContext ctx)
        //{
        //    return ctx.getChild(0).getText();
        //}



        //public override object /* String */ VisitLocalpart(XPath31Parser.local ctx)
        //{
        //    return visitNCName(ctx.nCName());
        //}



        //public override object /* FilterExpr */ VisitFilterexpr(XPath31Parser.filter ctx)
        //{
        //    return new FilterExpr(visitPrimaryExpr(ctx.primaryExpr()), visitPredicateList(ctx.predicateList()));
        //}

    }
}
