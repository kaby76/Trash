using System;

namespace AntlrTreeEditing.AntlrDOM
{
    public interface IAntlrObserver : IObserver<ObserverParserRuleContext>, IDisposable
    {
        void OnParentDisconnect(AntlrNode value);
        void OnParentConnect(AntlrNode value);
        void OnChildDisconnect(AntlrNode value);
        void OnChildConnect(AntlrNode value);
    }
}
