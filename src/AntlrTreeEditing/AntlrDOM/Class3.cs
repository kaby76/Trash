namespace AntlrTreeEditing.AntlrDOM
{
    public interface MyParseTreeVisitor<out Result>
    {
        Result Visit(AntlrNode tree);

        Result VisitChildren(AntlrNode node);

        Result VisitTerminal(AntlrNode node);

        Result VisitErrorNode(AntlrNode node);
    }
}
