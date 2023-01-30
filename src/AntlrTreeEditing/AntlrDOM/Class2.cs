namespace AntlrTreeEditing.AntlrDOM
{
    public interface MyParseTreeListener
    {
        void VisitTerminal(AntlrNode node);

        void VisitErrorNode(AntlrNode node);

        void EnterEveryRule(AntlrNode ctx);

        void ExitEveryRule(AntlrNode ctx);
    }
}
