using Antlr4.Runtime.Tree;
using System;

namespace ParseTreeEditing.AntlrDOM;

public interface IAntlrObserver : IObserver<ObserverParserRuleContext>, IDisposable
{
    void OnParentDisconnect(IParseTree value);
    void OnParentConnect(IParseTree value);
    void OnChildDisconnect(IParseTree value);
    void OnChildConnect(IParseTree value);
}
