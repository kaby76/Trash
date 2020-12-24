namespace AntlrJson
{
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    public class MyCharStream : ICharStream
    {
        public string Text { get; set; }
        public int Index => throw new NotImplementedException();
        public int Size => throw new NotImplementedException();
        public string SourceName { get; set; }
        public void Consume()
        {
            throw new NotImplementedException();
        }

        [return: NotNull]
        public string GetText(Interval interval)
        {
            return this.Text.Substring(interval.a, interval.b - interval.a + 1);
        }

        public int LA(int i)
        {
            throw new NotImplementedException();
        }

        public int Mark()
        {
            throw new NotImplementedException();
        }

        public void Release(int marker)
        {
            throw new NotImplementedException();
        }

        public void Seek(int index)
        {
            throw new NotImplementedException();
        }
    }
}
