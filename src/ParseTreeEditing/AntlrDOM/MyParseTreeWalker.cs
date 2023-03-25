namespace ParseTreeEditing.ParseTreeDOM
{
    public class MyParseTreeWalker
    {
        public static readonly MyParseTreeWalker Default = new MyParseTreeWalker();

        public virtual void Walk(MyParseTreeListener listener, UnvParseTreeNode t)
        {
            //if (t is IErrorNode)
            //{
            //    listener.VisitErrorNode((IErrorNode)t);
            //    return;
            //}

            if (t is UnvParseTreeText)
            {
                listener.VisitTerminal(t);
                return;
            }

            EnterRule(listener, t);
            int childCount = t.ChildNodes.Length;
            for (int i = 0; i < childCount; i++)
            {
                var it = t.ChildNodes.item(i);
                if (it is UnvParseTreeElement e) Walk(listener, e);
            }

            ExitRule(listener, t);
        }

        protected internal virtual void EnterRule(MyParseTreeListener listener, UnvParseTreeNode r)
        {
            listener.EnterEveryRule(r);
            r.EnterRule(listener);
        }

        protected internal virtual void ExitRule(MyParseTreeListener listener, UnvParseTreeNode r)
        {
            r.ExitRule(listener);
            listener.ExitEveryRule(r);
        }
    }
}
