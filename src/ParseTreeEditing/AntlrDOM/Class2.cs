namespace ParseTreeEditing.ParseTreeDOM
{
    public interface MyParseTreeListener
    {
        void VisitTerminal(UnvParseTreeNode node);

        void VisitErrorNode(UnvParseTreeNode node);

        void EnterEveryRule(UnvParseTreeNode ctx);

        void ExitEveryRule(UnvParseTreeNode ctx);
    }
}
