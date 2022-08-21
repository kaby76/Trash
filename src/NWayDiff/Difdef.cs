using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NWayDiff
{
    public class Difdef<T> where T : class
    {
        public const int MAX_FILES = 32;
        public int NUM_FILES;
        public Difdef_impl<T> impl;
        IEqualityComparer<T> comparer;

        public Difdef(int num_files, IEqualityComparer<T> c)
        {
            NUM_FILES = num_files;
            comparer = c;
            impl = new Difdef_impl<T>(num_files, comparer);
        }

        public void set_filter(Func<T, T> filter)
        {
            this.impl.filter = filter;
        }

        //public void replace_file(int fileid, string fin)
        //{
        //    this.impl.replace_file(fileid, fin);
        //}

        public void set_up_sequece(int fileid, List<T> seq)
        {
            this.impl.set_up_sequence(fileid, seq);
        }

        public Diff<T> merge()
        {
            int m = (1 << NUM_FILES) - 1;
            return impl.merge(m);
        }

        public Diff<T> merge(int fileid1, int fileid2)
        {
            int m = (1 << fileid1) | (1 << fileid2);
            return impl.merge(m);
        }
        public Diff<T> merge(HashSet<int> fileids)
        {
            int m = 0;
            foreach (var it in fileids)
            {
                m |= (1 << it);
            }
            return impl.merge(m);
        }

        public Diff<T> simply_concatenate(List<List<T>> lists) { throw new NotImplementedException(); }

    }
}
