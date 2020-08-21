using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.IO;

namespace Trash
{
    public class MyInputStream : BaseInputCharStream
    {
        private StreamReader stream;
        protected internal char[] data;

        public MyInputStream(StreamReader s)
        {
            data = new char[0];
            stream = s;
        }

        protected override string ConvertDataToString(int start, int count)
        {
            return new string(data, start, count);
        }

        protected override int ValueAt(int i)
        {
            return data[i];
        }

        /// <summary>How many characters are actually in the buffer</summary>
        //protected internal int n;

        /// <summary>0..n-1 index into string of next char</summary>
        //protected internal int p = 0;

        /// <summary>What is name or source of this char stream?</summary>
        //public string name;

        /// <summary>
        /// Reset the stream so that it's in the same state it was
        /// when the object was created *except* the data array is not
        /// touched.
        /// </summary>
        /// <remarks>
        /// Reset the stream so that it's in the same state it was
        /// when the object was created *except* the data array is not
        /// touched.
        /// </remarks>
        public override void Reset()
        {
            p = 0;
        }

        public override void Consume()
        {
            if (p >= n)
            {
                System.Diagnostics.Debug.Assert(LA(1) == IntStreamConstants.EOF);
                throw new InvalidOperationException("cannot consume EOF");
            }
            else
            {
                p++;
            }
        }

        //System.out.println("p moves to "+p+" (c='"+(char)data[p]+"')");
        public override int LA(int i)
        {
            if (i == 0)
            {
                return 0;
            }
            // undefined
            if (i < 0)
            {
                i++;
                // e.g., translate LA(-1) to use offset i=0; then data[p+0-1]
                if ((p + i - 1) < 0)
                {
                    return IntStreamConstants.EOF;
                }
            }
            //System.out.println("char LA("+i+")="+(char)data[p+i-1]+"; p="+p);
            //System.out.println("LA("+i+"); p="+p+" n="+n+" data.length="+data.length);
            return dataAt(p + i - 1);
        }

        public override int Lt(int i)
        {
            return LA(i);
        }

        /// <summary>
        /// Return the current input symbol index 0..n where n indicates the
        /// last symbol has been read.
        /// </summary>
        /// <remarks>
        /// Return the current input symbol index 0..n where n indicates the
        /// last symbol has been read.  The index is the index of char to
        /// be returned from LA(1).
        /// </remarks>
        public override int Index
        {
            get
            {
                return p;
            }
        }

        public override int Size
        {
            get
            {
                return n;
            }
        }

        /// <summary>mark/release do nothing; we have entire buffer</summary>
        public override int Mark()
        {
            return -1;
        }

        public override void Release(int marker)
        {
        }

        /// <summary>
        /// consume() ahead until p==index; can't just set p=index as we must
        /// update line and charPositionInLine.
        /// </summary>
        /// <remarks>
        /// consume() ahead until p==index; can't just set p=index as we must
        /// update line and charPositionInLine. If we seek backwards, just set p
        /// </remarks>
        public override void Seek(int index)
        {
            if (index <= p)
            {
                p = index;
                // just jump; don't update stream state (line, ...)
                return;
            }
            // seek forward, consume until p hits index or n (whichever comes first)
            index = Math.Min(index, n);
            while (p < index)
            {
                Consume();
            }
        }

        public override string GetText(Interval interval)
        {
            int start = interval.a;
            int stop = interval.b;
            if (stop >= n)
            {
                stop = n - 1;
            }
            int count = stop - start + 1;
            if (start >= n)
            {
                return string.Empty;
            }
            return ConvertDataToString(start, count);
        }

        public override string SourceName
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return IntStreamConstants.UnknownSourceName;
                }
                return name;
            }
        }

        private int dataAt(int i)
        {
            ensureRead(i);

            if (i < n)
            {
                return data[i];
            }
            else
            {
                // Nothing to read at that point.
                return IntStreamConstants.EOF;
            }
        }

        private void ensureRead(int i)
        {
            if (i < n)
            {
                // The data has been read.
                return;
            }

            int distance = i - n + 1;

            ensureCapacity(n + distance);

            // Crude way to copy from the byte stream into the char array.
            for (int pos = 0; pos < distance; ++pos)
            {
                int read;
                read = stream.Read();
                if (read < 0)
                {
                    break;
                }
                else
                {
                    data[n++] = (char)read;
                }
            }
        }

        private void ensureCapacity(int capacity)
        {
            if (capacity > n)
            {
                char[] newData = new char[capacity];
                System.Array.Copy(data, 0, newData, 0, n);
                data = newData;
            }
        }
    }
}
