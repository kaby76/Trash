namespace ParseTreeEditing.ParseTreeDOM
{
    public class ParseTreeWalker
    {
        public static readonly ParseTreeWalker Default = new ParseTreeWalker();

        public virtual void Walk(MyParseTreeListener listener, UnvParseTreeNode t)
        {
            //if (t is IErrorNode)
            //{
            //    listener.VisitErrorNode((IErrorNode)t);
            //    return;
            //}
            //else
            if (t is UnvParseTreeText)
            {
                listener.VisitTerminal((UnvParseTreeText)t);
                return;
            }
            else if (t is UnvParseTreeAttr a)
            {
                return;
            }
            EnterRule(listener, t);
            for (int i = 0; t.ChildNodes != null && i < t.ChildNodes.Length; ++i)
            {
                var c = (UnvParseTreeNode)t.ChildNodes.item(i);
                Walk(listener, c);
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
