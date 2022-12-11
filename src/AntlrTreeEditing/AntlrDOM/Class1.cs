namespace AntlrTreeEditing.AntlrDOM
{
    public interface IMyParseTreeListener
    {
        void VisitTerminal(AntlrElement node);
        void VisitErrorNode(AntlrElement node);
        void EnterEveryRule(AntlrElement ctx);
        void ExitEveryRule(AntlrElement ctx);
    }
}
