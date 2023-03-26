namespace ParseTreeEditing.UnvParseTreeDOM
{
    public interface MyParseTreeVisitor<out Result>
    {
        Result Visit(UnvParseTreeNode tree);

        Result VisitChildren(UnvParseTreeNode node);

        Result VisitTerminal(UnvParseTreeNode node);

        Result VisitErrorNode(UnvParseTreeNode node);
    }
}
