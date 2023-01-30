namespace AntlrTreeEditing.AntlrDOM
{
    public class ParseTreeWalker
    {
        public static readonly ParseTreeWalker Default = new ParseTreeWalker();

        public virtual void Walk(MyParseTreeListener listener, AntlrNode t)
        {
            //if (t is IErrorNode)
            //{
            //    listener.VisitErrorNode((IErrorNode)t);
            //    return;
            //}
            //else
            if (t is AntlrText)
            {
                listener.VisitTerminal((AntlrText)t);
                return;
            }
            else if (t is AntlrAttr a)
            {
                return;
            }
            EnterRule(listener, t);
            for (int i = 0; t.ChildNodes != null && i < t.ChildNodes.Length; ++i)
            {
                var c = (AntlrNode)t.ChildNodes.item(i);
                Walk(listener, c);
            }
            ExitRule(listener, t);
        }

        protected internal virtual void EnterRule(MyParseTreeListener listener, AntlrNode r)
        {
            listener.EnterEveryRule(r);
            r.EnterRule(listener);
        }

        protected internal virtual void ExitRule(MyParseTreeListener listener, AntlrNode r)
        {
            r.ExitRule(listener);
            listener.ExitEveryRule(r);
        }
    }
}
