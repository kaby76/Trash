using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWayDiff
{
    class Classical<T> where T : class
    {
        IEqualityComparer<T> comparer;

        public Classical(IEqualityComparer<T> c)
        {
            comparer = c;
        }

        public List<T> classical_lcs(List<T> a, List<T> b, int i, int j, Dictionary<Tuple<int, int>, List<T>> memo)
        {
            Tuple<int, int> key = new Tuple<int, int>(i, j);
            memo.TryGetValue(key, out List<T> val);
            if (val != null)
                return val;
            else if (i == 0 || j == 0)
            {
                List<T> result = new List<T>();
                memo[key] = result;
                return result;
            }
            else if (comparer.Equals(a[i-1], b[j-1]))
            {
                //T xxx = a[i - 1];
                //int oldi = i;
                while (i > 0 && j > 0 && comparer.Equals(a[i-1], b[j-1]))
                {
                    --i;
                    --j;
                }
                List<T> result = classical_lcs(a, b, i, j, memo);
                throw new Exception();
                // result.insert(result.end(), a.begin() + i, a.begin() + oldi);
                //return result;
            }
            else
            {
                List<T> result1 = classical_lcs(a, b, i - 1, j, memo);
                List<T> result2 = classical_lcs(a, b, i, j - 1, memo);
                if (result1.Count > result2.Count)
                {
                    memo[key] = result1;
                    return result1;
                }
                else
                {
                    memo[key] = result2;
                    return result2;
                }
            }
        }
    }
}
