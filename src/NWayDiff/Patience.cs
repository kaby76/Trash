using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWayDiff
{
    public class Patience<T> where T : class
    {
        public int value;
        public Patience<T> left;
        public Patience<T> down;

        public Patience(int v, Patience<T> l, Patience<T> d)
        {
            value = v;
            left = l;
            down = d;
        }

        public static List<int> patience_longest_increasing_sequence(List<int> v)
        {
            List<int> result = new List<int>();
            if (!v.Any())
                return result;

            List<Patience<T>> top_cards = new List<Patience<T>>();
            for (int i = 0; i < v.Count; ++i)
            {
                int val = v[i];
                bool handled = false;
                for (int j = 0; j < top_cards.Count; ++j)
                {
                    if (top_cards[j].value > val)
                    {
                        var left = j > 0 ? top_cards[j - 1] : null;
                        top_cards[j] = new Patience<T>(val, left, top_cards[j]);
                        handled = true;
                        break;
                    }
                }
                if (!handled)
                {
                    var left = (!top_cards.Any()) ? null : top_cards[top_cards.Count - 1];
                    top_cards.Add(new Patience<T>(val, left, null));
                }
            }

            if (!top_cards.Any()) throw new Exception();
            int n = top_cards.Count;
            // result.resize(n);
            int[] interm = result.ToArray();
            Array.Resize(ref interm, n);
            result = interm.ToList();
            Patience<T> p = top_cards[n - 1];
            for (int i = 0; i < n; ++i)
            {
                if (p == null) throw new Exception();
                result[n - i - 1] = p.value;
                p = p.left;
            }
            if (p != null) throw new Exception();
            return result;
        }

        public static List<T> patience_unique_lcs(List<T> a, List<T> b, IEqualityComparer<T> comparer)
        {
            int n = a.Count;
            if (b.Count != n) throw new Exception();
            List<int> indices = new List<int>();
            for (int i = 0; i < n; ++i) indices.Add(0);
            for (int i = 0; i < n; ++i)
            {
                int index_of_ai_in_b = Class1<T>.MyIndexOf(b, a[i], comparer);
                if (!(0 <= index_of_ai_in_b && index_of_ai_in_b < n)) throw new Exception();
                indices[i] = index_of_ai_in_b;
            }
            List<int> ps = patience_longest_increasing_sequence(indices);
            List<T> result = new List<T>();
            for (int i = 0; i < ps.Count; ++i)
            {
                result.Add(b[ps[i]]);
            }
            return result;
        }
    }
}
