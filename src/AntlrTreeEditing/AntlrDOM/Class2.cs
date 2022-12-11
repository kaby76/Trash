namespace AntlrTreeEditing.AntlrDOM
{
    public class MyParseTreeWalker
    {
        public static readonly MyParseTreeWalker Default = new MyParseTreeWalker();

        public virtual void Walk(IMyParseTreeListener listener, AntlrElement t)
        {
            //if (t is IErrorNode)
            //{
            //    listener.VisitErrorNode((IErrorNode)t);
            //    return;
            //}

            if (t.RuleIndex < 0)
            {
                listener.VisitTerminal(t);
                return;
            }

            EnterRule(listener, t);
            int childCount = t.ChildNodes.Length;
            for (int i = 0; i < childCount; i++)
            {
                var it = t.ChildNodes.item(i);
                if (it is AntlrElement e) Walk(listener, e);
            }

            ExitRule(listener, t);
        }

        protected internal virtual void EnterRule(IMyParseTreeListener listener, AntlrElement r)
        {
            listener.EnterEveryRule(r);
            r.EnterRule(listener);
        }

        protected internal virtual void ExitRule(IMyParseTreeListener listener, AntlrElement r)
        {
            r.ExitRule(listener);
            listener.ExitEveryRule(r);
        }
    }
}
