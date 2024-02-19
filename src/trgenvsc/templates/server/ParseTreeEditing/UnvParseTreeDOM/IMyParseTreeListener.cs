namespace ParseTreeEditing.UnvParseTreeDOM
{
    public interface IMyParseTreeListener
    {
        void VisitTerminal(UnvParseTreeElement node);
        void VisitErrorNode(UnvParseTreeElement node);
        void EnterEveryRule(UnvParseTreeElement ctx);
        void ExitEveryRule(UnvParseTreeElement ctx);
    }
}
