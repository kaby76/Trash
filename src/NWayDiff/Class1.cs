using System;
using System.Collections.Generic;
using System.Text;

namespace NWayDiff
{
    public class Class1<T> where T : class
    {
        public static int MyIndexOf(List<T> list, T t, IEqualityComparer<T> comparer)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (comparer.Equals(list[i], t))
                    return i;
            }
            return -1;
        }
    }
}
