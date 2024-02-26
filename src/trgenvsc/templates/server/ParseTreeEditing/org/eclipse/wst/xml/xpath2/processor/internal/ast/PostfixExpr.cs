using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.eclipse.wst.xml.xpath2.processor.@internal.ast;

namespace xpath.org.eclipse.wst.xml.xpath2.processor.@internal.ast
{
    public class PostfixExpr : StepExpr
    {
        private PrimaryExpr _pexpr;
        private ICollection<ICollection<Expr>> _exprs;

        public virtual PrimaryExpr primary()
        {
            return _pexpr;
        }

        public IEnumerator GetEnumerator()
        {
            return _exprs.GetEnumerator();
        }

        public PostfixExpr(object pexpr, ICollection<ICollection<Expr>> exprs)
        {
            _pexpr = (PrimaryExpr)pexpr;
            _exprs = exprs;
        }

        public override object accept(XPathVisitor v)
        {
            return v.visit(this);
        }

        public virtual IEnumerator<ICollection<Expr>> iterator()
        {
            return _exprs.GetEnumerator();
        }

        public virtual int predicate_count()
        {
            return _exprs.Count;
        }


        public override ICollection<XPathNode> GetAllChildren()
        {
            var list = new List<XPathNode>();
            list.Add(_pexpr);
            foreach (var col in _exprs)
            {
                foreach (var e in col)
                    list.Add((XPathNode)e);
            }
            return list;
        }

        public override string QuickInfo()
        {
            return "";
        }
    }
}
