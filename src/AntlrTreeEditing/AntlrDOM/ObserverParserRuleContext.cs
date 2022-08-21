namespace AntlrTreeEditing.AntlrDOM
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using org.eclipse.wst.xml.xpath2.processor.@internal.function;
    using System;
    using System.Collections.Generic;

    public class ObserverParserRuleContext : ParserRuleContext, IAntlrObservable
    {
        private List<IAntlrObserver> observers;

        public IEnumerable<IAntlrObserver> Observers
        {
            get { return observers; }
        }

        public ObserverParserRuleContext(ParserRuleContext parent, int invokingState)
            : base(parent, invokingState)
        {
            observers = new List<IAntlrObserver>();
        }

        public ObserverParserRuleContext()
            : base()
        {
            observers = new List<IAntlrObserver>();
        }

        public override void AddChild(ITerminalNode t)
        {
            base.AddChild(t);
            this.NotifyAddChild(t);
            if (t is ObserverParserRuleContext o)
            {
                o.NotifyAddParent(this);
            }
        }

        public override ITerminalNode AddChild(IToken matchedToken)
        {
            return base.AddChild(matchedToken);
        }

        public override void AddChild(RuleContext ruleInvocation)
        {
            base.AddChild(ruleInvocation);
            this.NotifyAddChild(ruleInvocation);
            if (ruleInvocation is ObserverParserRuleContext o)
            {
                o.NotifyAddParent(this);
            }
        }


        public virtual void RemoveChild(ITerminalNode t)
        {
        }

        public virtual void RemoveChild(IToken matchedToken)
        {
        }

        public virtual void RemoveChild(RuleContext ruleInvocation)
        {
        }

        public virtual void ReplaceChild(IParseTree t)
        {
        }


        public override RuleContext Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                var before = base.Parent;
                if (before != null)
                {
                    this.NotifyRemoveParent(before);
                }
                base.Parent = value;
                if (value != null)
                {
                    this.NotifyAddParent(value);
                }
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IAntlrObserver> _observers;
            private IAntlrObserver _observer;

            public Unsubscriber(List<IAntlrObserver> observers, IAntlrObserver observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void SendMessage(ObserverParserRuleContext loc)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(loc);
            }
        }

        public void NotifyAddParent(IParseTree loc)
        {
            foreach (var observer in observers)
            {
                observer.OnParentConnect(loc);
            }
        }

        public void NotifyRemoveParent(IParseTree loc)
        {
            foreach (var observer in observers)
            {
                observer.OnParentDisconnect(loc);
            }
        }

        public void NotifyAddChild(IParseTree loc)
        {
            foreach (var observer in observers)
            {
                observer.OnChildConnect(loc);
            }
        }

        public void NotifyRemoveChild(IParseTree loc)
        {
            foreach (var observer in observers)
            {
                observer.OnChildDisconnect(loc);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }

        public IDisposable Subscribe(IAntlrObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

        public void Unsubscribe(IAntlrObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        public IDisposable Subscribe(IObserver<ObserverParserRuleContext> observer)
        {
            throw new NotImplementedException();
        }
    }

    public class ObserverTerminalNodeImpl : TerminalNodeImpl, IAntlrObservable
    {
        private List<IAntlrObserver> observers;

        public IEnumerable<IAntlrObserver> Observers
        {
            get { return observers; }
        }

        public ObserverTerminalNodeImpl(IToken token) : base(token)
        {
            observers = new List<IAntlrObserver>();
        }

        public IDisposable Subscribe(IAntlrObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

        public void Unsubscribe(IAntlrObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
        public IDisposable Subscribe(IObserver<ObserverParserRuleContext> observer)
        {
            throw new NotImplementedException();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IAntlrObserver> _observers;
            private IAntlrObserver _observer;

            public Unsubscriber(List<IAntlrObserver> observers, IAntlrObserver observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
