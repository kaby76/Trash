using OneOf;

namespace Trash;

public class FilterEvaluator : FilterBaseVisitor<OneOf.OneOf<object, bool, int>>
{
    private int l1_;
    private int l2_;

    public FilterEvaluator(int l1, int l2)
    {
        l1_ = l1;
        l2_ = l2;
    }

    public override OneOf<object, bool, int> VisitStart(FilterParser.StartContext context)
    {
        var result = new OneOf<object, bool, int>();
        foreach (var e in context.expr())
        {
            var v = Visit(e);
            if (v.IsT1 && result.IsT0)
            {
                result = v;
            }
            else if (v.IsT1 && result.IsT1)
            {
                result = result.AsT1 || v.AsT1;
            }
            else
            {
                result = new OneOf<object, bool, int>();
            }
        }

        return result;
    }

    public override OneOf<object, bool, int> VisitExpr(FilterParser.ExprContext context)
    {
        if (context.NUM() != null)
        {
            return int.Parse(context.NUM().GetText());
        }
        else if (context.ID() != null && context.ID().GetText().Equals("l1"))
        {
            return l1_;
        }
        else if (context.ID() != null && context.ID().GetText().Equals("l2"))
        {
            return l2_;
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
            else if (v1.IsT2 && v2.IsT2 && context.LE() != null)
            {
                return v1.AsT2 >= v2.AsT2;
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
