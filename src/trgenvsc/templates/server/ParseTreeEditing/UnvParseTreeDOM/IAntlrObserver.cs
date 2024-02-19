using System;

namespace ParseTreeEditing.UnvParseTreeDOM
{
    public interface IAntlrObserver : IObserver<ObserverParserRuleContext>, IDisposable
    {
        void OnParentDisconnect(UnvParseTreeNode value);
        void OnParentConnect(UnvParseTreeNode value);
        void OnChildDisconnect(UnvParseTreeNode value);
        void OnChildConnect(UnvParseTreeNode value);
    }
}
