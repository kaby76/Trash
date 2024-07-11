using OneOf;

namespace Trash;

public class FilterEvaluator : FilterBaseVisitor<OneOf.OneOf<object, bool, int>>
{
    private int s0_;
    private int s1_;
    private int e0_;
    private int e1_;

    public FilterEvaluator(int s0, int s1, int e0, int e1)
    {
        s0_ = s0;
        s1_ = s1;
        e0_ = e0;
        e1_ = e1;
    }

    public override OneOf<object, bool, int> VisitStart(FilterParser.StartContext context)
    {
        var result = new OneOf<object, bool, int>();
        var e = context.expr();
        if (e == null)
        {
            return result;
        }
        var v = Visit(e);
        if (v.IsT1)
        {
            result = v.AsT1;
        }
        else
        {
            result = new OneOf<object, bool, int>();
        }
        return result;
    }

    public override OneOf<object, bool, int> VisitExpr(FilterParser.ExprContext context)
    {
        if (context.NUM() != null)
        {
            return int.Parse(context.NUM().GetText());
        }
        else if (context.ID() != null && context.ID().GetText().Equals("s0"))
        {
            return s0_;
        }
        else if (context.ID() != null && context.ID().GetText().Equals("s1"))
        {
            return s1_;
        }
        else if (context.ID() != null && context.ID().GetText().Equals("e0"))
        {
            return e0_;
        }
        else if (context.ID() != null && context.ID().GetText().Equals("e1"))
        {
            return e1_;
        }
        else if (context.ID() != null)
        {
            return null;
        }
        else if (context.expr().Length == 2)
        {
            var v1 = VisitExpr(context.expr(0));
            var v2 = VisitExpr(context.expr(1));
            if (v1.IsT2 && v2.IsT2 && context.LT() != null)
            {
                return v1.AsT2 < v2.AsT2;
            }
            else if (v1.IsT2 && v2.IsT2 && context.LE() != null)
            {
                return v1.AsT2 <= v2.AsT2;
            }
            else if (v1.IsT2 && v2.IsT2 && context.GT() != null)
            {
                return v1.AsT2 > v2.AsT2;
            }
            else if (v1.IsT2 && v2.IsT2 && context.GE() != null)
            {
                return v1.AsT2 >= v2.AsT2;
            }
            else if (v1.IsT1 && v2.IsT1 && context.AND() != null)
            {
                return v1.AsT1 && v2.AsT1;
            }
            else if (v1.IsT1 && v2.IsT1 && context.OR() != null)
            {
                return v1.AsT1 || v2.AsT1;
            }
            else
            {
                return null;
            }
        }
        else if (context.expr().Length == 1)
        {
            var v1 = VisitExpr(context.expr(0));
            if (v1.IsT1)
            {
                return v1.AsT1;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
