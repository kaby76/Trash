using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NWayDiff
{
    public class Difdef_impl<T> where T : class
    {
        public int NUM_FILES;
        public Difdef_StringSet<T> unique_lines;
        public List<List<T>> lines;
        public Func<T, T> filter;
        IEqualityComparer<T> comparer;

        public Difdef_impl(int num_files, IEqualityComparer<T> c)
        {
            NUM_FILES = num_files;
            lines = new List<List<T>>();
            comparer = c;
            unique_lines = new Difdef_StringSet<T>(num_files, comparer);
            for (int i = 0; i < num_files; ++i) lines.Add(new List<T>());
            filter = null;
        }

        //public void replace_file(int fileid, string fin)
        //{
        //    lines[fileid].Clear();
        //    if (fin != null)
        //    {
        //        var ls = System.IO.File.ReadAllLines(fin);
        //        foreach (var l in ls)
        //        {
        //            var s = this.unique_lines.add(fileid, l);
        //            lines[fileid].Add(s);
        //        }
        //    }
        //}

        public void set_up_sequence(int fileid, List<T> seq)
        {
            lines[fileid].Clear();
            if (seq != null)
            {
                foreach (var l in seq)
                {
                    var s = this.unique_lines.add(fileid, l);
                    lines[fileid].Add(s);
                }
            }
        }

        int diff_ending_priority(T text)
        {
            throw new NotImplementedException();
            //int i = 0;
            //while (i != text.Length && char.IsWhiteSpace(text[i])) ++i;
            //if (i == text.Length)
            //    return 1;
            //if (text[i] == '}')
            //    return Math.Max(100 - i, 10);
            //return 0;
        }

        bool contains(int mortals, int men)
        {
            return (men & ~mortals) == 0;
        }

        Diff<T> slide_diff_windows(Diff<T> d)
        {
            int N = d.lines.Count;
            int last_edge = 0;
            for (int i = 1; i < N; ++i)
            {
                if (d.lines[i - 1].mask == d.lines[i].mask)
                    continue;
                int inner_mask = d.lines[i - 1].mask;
                int outer_mask = d.lines[i].mask;
                if (last_edge != 0 && d.lines[last_edge-1].mask == outer_mask && contains(outer_mask, inner_mask))
                {
                    int window_down = 0;
                    int window_up = 0;
                    while (window_down < Math.Min(last_edge, i - last_edge)
                        && d.lines[last_edge - window_down - 1].mask == outer_mask
                        && comparer.Equals(d.lines[last_edge - window_down - 1].text, d.lines[i - window_down - 1].text))
                        ++window_down;
                    while (window_up < Math.Min(N - i, i - last_edge)
                        && d.lines[i + window_up].mask == outer_mask
                        && comparer.Equals(d.lines[i + window_up].text, d.lines[last_edge + window_up].text))
                        ++window_up;
                    int max_priority = 0;
                    int max_priority_edge = 0;
                    for (int j = 0; j < window_down + window_up; ++j)
                    {
                        var text = d.lines[last_edge - window_down + j].text;
                        int priority = diff_ending_priority(text);
                        if (priority > max_priority)
                        {
                            max_priority = priority;
                            max_priority_edge = j + 1;
                        }
                    }
                    for (int j = 0; j < max_priority_edge; ++j)
                    {
                        d.lines[i - window_down + j].mask = inner_mask;
                        d.lines[last_edge - window_down + j].mask = outer_mask;
                    }
                    for (int j = max_priority_edge; j < window_down + window_up; ++j)
                    {
                        d.lines[i - window_down + j].mask = outer_mask;
                        d.lines[last_edge - window_down + j].mask = inner_mask;
                    }
                    last_edge = i - window_down + max_priority_edge;
                    i = Math.Max(last_edge, i + window_up - 1);
                }
                else
                {
                    last_edge = i;
                }
            }
            return d;
        }

        public Diff<T> merge(int fmask)
        {
            Diff<T> d = new Diff<T>(NUM_FILES, 0);
            for (int i = 0; i < lines.Count; ++i)
            {
                add_vec_to_diff(ref d, i, lines[i]);
            }
            return slide_diff_windows(d);
        }

        static Random rrr = new Random();
        
        public T MyFun(T f)
        {
            int v = rrr.Next();
            if (v % 2 == 0)
                return f;
            else
                return null;
        }

        public void add_vec_to_diff(ref Diff<T> a, int fileid, List<T> b)
        {
            int bmask = 1 << fileid;
            Diff<T> result = new Diff<T>(a.dimension, a.mask | bmask);
            Diff<T> suffix = new Diff<T>(a.dimension, a.mask | bmask);
            int i = 0;
            while (i < a.lines.Count && i < b.Count && comparer.Equals(a.lines[i].text, b[i]))
            {
                T line = b[i];
                result.lines.Add(new Line<T>(line, a.lines[i].mask | bmask));
                ++i;
            }
            int ja = a.lines.Count;
            int jb = b.Count;
            List<T> ua = new List<T>();
            List<T> ub = new List<T>();
            for (int k = i; k < ja; ++k)
            {
                T line = a.lines[k].text;
                List<int> d = unique_lines.lookup(line);
                bool failed = d[fileid] == 0;
                if (failed) continue;
                for (int k2 = i; !failed && k2 < ja; ++k2)
                {
                    if (k2 == k) continue;
                    if (comparer.Equals(a.lines[k2].text, line)) failed = true;
                }
                if (failed) continue;
                bool found = false;
                for (int k2 = i; !failed && k2 < jb; ++k2)
                {
                    if (comparer.Equals(b[k2], line))
                    {
                        if (found) failed = true;
                        found = true;
                    }
                }
                if (failed || !found) continue;
                ua.Add(line);
            }
            for (int k = i; k < jb; ++k)
            {
                T line = b[k];
                if (Class1<T>.MyIndexOf(ua, line, comparer) >= 0)
                    ub.Add(line);
            }
            List<T> lcs = Patience<T>.patience_unique_lcs(ua, ub, comparer);
            if (lcs.Count == 0)
            {
                Diff<T> ta = new Diff<T>(a.dimension, a.mask | bmask);
                List<T> tb = new List<T>();
                for (int j = i; j < jb; ++j) tb.Add(b[j]);
                for (int k = i; k < ja; ++k)
                {
                    ta.lines.Add(new Line<T>(a.lines[k].text, a.lines[k].mask));
                }
                this.add_vec_to_diff_classical(ref ta, fileid, tb);
                result.append(ta);
            }
            else
            {
                int ak = i;
                int bk = i;
                Diff<T> ta = new Diff<T>(a.dimension, a.mask);
                List<T> tb = new List<T>();
                for (int lcx = 0; lcx < lcs.Count; ++lcx)
                {
                    while (! comparer.Equals(a.lines[ak].text, lcs[lcx]))
                    {
                        ta.lines.Add(a.lines[ak]);
                        ++ak;
                    }
                    while (! comparer.Equals(b[bk], lcs[lcx]))
                    {
                        tb.Add(b[bk]);
                        ++bk;
                    }
                    ta.mask = a.mask;
                    this.add_vec_to_diff(ref ta, fileid, tb);
                    result.append(ta);
                    ta.lines.Clear();
                    tb.Clear();
                    result.lines.Add(new Line<T>(lcs[lcx], a.lines[ak].mask | bmask));
                    ++ak;
                    ++bk;
                }
                while (ak < ja)
                {
                    ta.lines.Add(a.lines[ak]);
                    ++ak;
                }
                while (bk < jb)
                {
                    tb.Add(b[bk]);
                    ++bk;
                }
                ta.mask = a.mask;
                this.add_vec_to_diff(ref ta, fileid, tb);
                result.append(ta);
            }
            a = result; // COPY!
        }

        bool are_equal(List<T> a, List<T> b)
        {
            int n = a.Count;
            if (b.Count != n) return false;
            for (int i = 0; i < n; ++i)
            {
                if (! comparer.Equals(a[i], b[i]))
                    return false;
            }
            return true;
        }

        public Diff<T> simply_concatenate(List<List<T>> vec)
        {
            int num_files = vec.Count;
            int have_handled = 0;
            Diff<T> result = new Diff<T>(num_files, (1 << num_files) - 1);
            for (int v = 0; v < num_files; ++v)
            {
                int vmask = 1 << v;
                if ((have_handled & vmask) != 0) continue;
                for (int w = v; w < num_files; ++w)
                {
                    int wmask = 1 << w;
                    if ((have_handled & wmask) != 0) continue;
                    if (are_equal(vec[v], vec[w]))
                        vmask |= wmask;
                }
                for (int i = 0; i < vec[v].Count; ++i)
                {
                    result.lines.Add(new Line<T>(vec[v][i], vmask));
                }
                have_handled |= vmask;
            }
            return result;
        }

        public void add_vec_to_diff_classical(ref Diff<T> a, int fieldid, List<T> b)
        {
            int bmask = 1 << fieldid;
            if (b.Count == 0) return;
            List<T> ta = new List<T>();
            for (int i = 0; i < a.lines.Count; ++i)
            {
                T line = a.lines[i].text;
                var data = unique_lines.lookup(line);
                var v = data[fieldid];
                if (v > 0)
                    ta.Add(line);
            }

            Dictionary<Tuple<int, int>, List<T>> memo = new Dictionary<Tuple<int, int>, List<T>>();
            List<T> lcs = new Classical<T>(comparer).classical_lcs(ta, b, ta.Count, b.Count, memo);

            Diff<T> result = new Diff<T>(a.dimension, a.mask | bmask);
            int ak = 0;
            int bk = 0;
            for (int lcx = 0; lcx < lcs.Count; ++lcx)
            {
                while (! comparer.Equals(a.lines[ak].text, lcs[lcx]))
                {
                    result.lines.Add(new Line<T>(a.lines[ak].text, a.lines[ak].mask));
                    ++ak;
                }
                while (! comparer.Equals(b[bk], lcs[lcx]))
                {
                    result.lines.Add(new Line<T>(b[bk], bmask));
                    ++bk;
                }
                result.lines.Add(new Line<T>(lcs[lcx], a.lines[ak].mask | bmask));
                ++ak;
                ++bk;
            }
            for (; ak < a.lines.Count; ++ak)
                result.lines.Add(new Line<T>(a.lines[ak].text, a.lines[ak].mask));
            for (; bk < b.Count; ++bk)
                result.lines.Add(new Line<T>(b[bk], bmask));
            a = result; // COPY!
        }
    }
}
