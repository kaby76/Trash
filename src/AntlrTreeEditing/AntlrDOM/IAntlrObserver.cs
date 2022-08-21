using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntlrTreeEditing.AntlrDOM
{
    public interface IAntlrObserver : IObserver<ObserverParserRuleContext>, IDisposable
    {
        void OnParentDisconnect(IParseTree value);
        void OnParentConnect(IParseTree value);
        void OnChildDisconnect(IParseTree value);
        void OnChildConnect(IParseTree value);
    }
}
