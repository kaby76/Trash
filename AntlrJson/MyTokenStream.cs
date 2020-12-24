namespace AntlrJson
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    public class MyTokenStream : ITokenStream
    {
        private ITokenSource _tokenSource;
        protected internal List<IToken> tokens;
        protected internal int n;
        protected internal int p = 0;
        protected internal int numMarkers = 0;
        protected internal IToken lastToken;
        protected internal IToken lastTokenBufferStart;
        protected internal int currentTokenIndex = 0;

        public MyTokenStream()
        {
            this.tokens = new List<IToken>();
            n = 0;
        }

        public MyTokenStream(string t)
        {
            Text = t;
            this.tokens = new List<IToken>();
            n = 0;
        }

        public virtual IToken Get(int i)
        {
            int bufferStartIndex = GetBufferStartIndex();
            if (i < bufferStartIndex || i >= bufferStartIndex + n)
            {
                throw new ArgumentOutOfRangeException("get(" + i + ") outside buffer: " + bufferStartIndex + ".." + (bufferStartIndex + n));
            }
            return tokens[i - bufferStartIndex];
        }

        public virtual IToken LT(int i)
        {
            if (i == -1)
            {
                return lastToken;
            }
            Sync(i);
            int index = p + i - 1;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("LT(" + i + ") gives negative index");
            }
            if (index >= n)
            {
                System.Diagnostics.Debug.Assert(n > 0 && tokens[n - 1].Type == TokenConstants.EOF);
                return tokens[n - 1];
            }
            return tokens[index];
        }

        public virtual int LA(int i)
        {
            return LT(i).Type;
        }

        public virtual ITokenSource TokenSource
        {
            get
            {
                return _tokenSource;
            }
            set
            {
                _tokenSource = value;
            }
        }

        [return: NotNull]
        public virtual string GetText()
        {
            return string.Empty;
        }

        [return: NotNull]
        public virtual string GetText(RuleContext ctx)
        {
            return GetText(ctx.SourceInterval);
        }

        [return: NotNull]
        public virtual string GetText(IToken start, IToken stop)
        {
            if (start != null && stop != null)
            {
                return GetText(Interval.Of(start.TokenIndex, stop.TokenIndex));
            }
            throw new NotSupportedException("The specified start and stop symbols are not supported.");
        }

        public virtual void Consume()
        {
            if (LA(1) == TokenConstants.EOF)
            {
                throw new InvalidOperationException("cannot consume EOF");
            }
            // buf always has at least tokens[p==0] in this method due to ctor
            lastToken = tokens[p];
            // track last token for LT(-1)
            // if we're at last token and no markers, opportunity to flush buffer
            if (p == n - 1 && numMarkers == 0)
            {
                n = 0;
                p = -1;
                // p++ will leave this at 0
                lastTokenBufferStart = lastToken;
            }
            p++;
            currentTokenIndex++;
            Sync(1);
        }

        protected internal virtual void Sync(int want)
        {
        }

        protected internal virtual int Fill(int n)
        {
            return n;
        }

        protected internal virtual void Add(IToken t)
        {
            if (t is IWritableToken)
            {
                ((IWritableToken)t).TokenIndex = GetBufferStartIndex() + n;
            }
            tokens.Add(t);
            n++;
        }

        public virtual int Mark()
        {
            if (numMarkers == 0)
            {
                lastTokenBufferStart = lastToken;
            }
            int mark = -numMarkers - 1;
            numMarkers++;
            return mark;
        }

        public virtual void Release(int marker)
        {
            int expectedMark = -numMarkers;
            if (marker != expectedMark)
            {
                throw new InvalidOperationException("release() called with an invalid marker.");
            }
            numMarkers--;
            if (numMarkers == 0)
            {
                // can we release buffer?
                if (p > 0)
                {
                    // Copy tokens[p]..tokens[n-1] to tokens[0]..tokens[(n-1)-p], reset ptrs
                    // p is last valid token; move nothing if p==n as we have no valid char
                    //                        System.Array.Copy(tokens, p, tokens, 0, n - p);
                    // shift n-p tokens from p to 0
                    n = n - p;
                    p = 0;
                }
                lastTokenBufferStart = lastToken;
            }
        }

        public virtual int Index
        {
            get
            {
                return currentTokenIndex;
            }
        }

        public virtual void Seek(int index)
        {
            // seek to absolute index
            if (index == currentTokenIndex)
            {
                return;
            }
            if (index > currentTokenIndex)
            {
                Sync(index - currentTokenIndex);
                index = Math.Min(index, GetBufferStartIndex() + n - 1);
            }
            int bufferStartIndex = GetBufferStartIndex();
            int i = index - bufferStartIndex;
            if (i < 0)
            {
                throw new ArgumentException("cannot seek to negative index " + index);
            }
            else
            {
                if (i >= n)
                {
                    throw new NotSupportedException("seek to index outside buffer: " + index + " not in " + bufferStartIndex + ".." + (bufferStartIndex + n));
                }
            }
            p = i;
            currentTokenIndex = index;
            if (p == 0)
            {
                lastToken = lastTokenBufferStart;
            }
            else
            {
                lastToken = tokens[p - 1];
            }
        }

        public virtual int Size
        {
            get
            {
                return tokens.Count;
            }
        }

        public virtual string SourceName
        {
            get
            {
                return TokenSource.SourceName;
            }
        }

        public string Text { get; internal set; }

        [return: NotNull]
        public virtual string GetText(Interval interval)
        {
            int bufferStartIndex = GetBufferStartIndex();
            int bufferStopIndex = bufferStartIndex + tokens.Count - 1;
            int start = interval.a;
            int stop = interval.b;
            if (start < bufferStartIndex || stop > bufferStopIndex)
            {
                throw new NotSupportedException("interval " + interval + " not in token buffer window: " + bufferStartIndex + ".." + bufferStopIndex);
            }
            int a = start - bufferStartIndex;
            int b = stop - bufferStartIndex;
            StringBuilder buf = new StringBuilder();
            for (int i = a; i <= b; i++)
            {
                IToken t = tokens[i];
                buf.Append(t.Text);
            }
            return buf.ToString();
        }

        protected internal int GetBufferStartIndex()
        {
            return currentTokenIndex - p;
        }
    }
}
