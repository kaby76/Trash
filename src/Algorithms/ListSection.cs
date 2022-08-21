using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// A data type to encapsulate sections of a list, acting itself as a list. The reason is so you do not need
    /// to keep track of offsets into lists.
    /// NB: At the moment, DO NOT MODIFY THE LIST! There is no adjustment of offsets!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListSection<T>
    {
        private readonly List<T> _list;
        private readonly int _base;
        private readonly int _len;

        public List<T> List => _list;

        public int Base => _base;

        public int Len => _len;

        public ListSection(List<T> list, int b, int l)
        {
            _list = list;
            _base = b;
            _len = l;
        }

        public T this[int i]
        {
            get => _list[_base + i];
            set => _list[_base + i] = value;
        }

        private static void Resize(ref ListSection<T> arr, int new_length)
        {
        }
    }
}
