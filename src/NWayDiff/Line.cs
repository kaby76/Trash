using System;
using System.Collections.Generic;
using System.Text;

namespace NWayDiff
{
    public class Line<T> where T : class
    {
        public T text;
        public int mask;  // a bitmask

        public bool in_file(int fileid)
        {
            return (this.mask & (1 << fileid)) != 0;
        }

        public Line()
        {
            text = null;
            mask = 0;
        }

        public Line(T s, int m)
        {
            text = s;
            this.mask = m;
        }
    }
}
