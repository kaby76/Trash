using System;
using System.Collections.Generic;
using System.Text;

namespace NWayDiff
{
    public class Difdef_StringSet<T> where T : class
    {
        public int NUM_FILES;
        public Dictionary<T, List<int>> unique_lines;
        IEqualityComparer<T> comparer;

        public Difdef_StringSet(int num_files, IEqualityComparer<T> c)
        {
            NUM_FILES = num_files;
            comparer = c;
            unique_lines = new Dictionary<T, List<int>>(comparer);
        }

        public T add(int fileid, T text)
        {
            unique_lines.TryGetValue(text, out List<int> p);
            if (p == null)
            {
                var d = new List<int>();
                for (int i = 0; i < NUM_FILES; ++i) d.Add(0);
                d[fileid] = 1;
                unique_lines[text] = d;
                p = d;
            }
            else
            {
                p[fileid] = p[fileid] + 1;
            }
            return text;
        }

        public List<int> lookup(T text)
        {
            return unique_lines[text];
        }
    }
}
