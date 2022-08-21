using System;
using System.Collections;
using System.Collections.Generic;

namespace Algorithms.Utils
{
    /// <summary>
    /// Represents a collection of keys and values.
    /// Multiple values can have the same key.
    /// </summary>
    /// <typeparam name="K">Type of the keys.</typeparam>
    /// <typeparam name="V">Type of the values.</typeparam>
    [System.Serializable]
    public class MultiMap<K, V> : IEnumerable<KeyValuePair<K, List<V>>>
    {
        private Dictionary<K, List<V>> the_list = null;

        public MultiMap()
            : base()
        {
            the_list = new Dictionary<K, List<V>>();
        }

        //public MultiMap(MultiMap<K, V> other)
        //    : base(other)
        //{
        //}

        public MultiMap(int capacity)
        {
            new Dictionary<K, List<V>>(capacity);
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.Dictionary<TKey,TValue>
        //     class that is empty, has the default initial capacity, and uses the specified
        //     System.Collections.Generic.IEqualityComparer<T>.
        //
        // Parameters:
        //   comparer:
        //     The System.Collections.Generic.IEqualityComparer<T> implementation to use
        //     when comparing keys, or null to use the default System.Collections.Generic.EqualityComparer<T>
        //     for the type of the key.
        public MultiMap(IEqualityComparer<K> comparer)
        {
            the_list = new Dictionary<K, List<V>>(comparer);
        }

        /// <summary>
        /// Adds an element with the specified key and value into the MultiMap. 
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(K key, V value)
        {
            //System.Console.WriteLine("In Add of MultiMap " + key + " " + value);
            if (the_list.TryGetValue(key, out List<V> valueList))
            {
                valueList.Add(value);
            }
            else
            {
                valueList = new List<V>
                {
                    value
                };
                the_list.Add(key, valueList);
            }
        }

        public void Add(K key)
        {
            //System.Console.WriteLine("In Add of MultiMap " + key);
            if (the_list.TryGetValue(key, out _))
            {
            }
            else
            {
                List<V> valueList = new List<V>();
                the_list.Add(key, valueList);
            }
        }

        /// <summary>
        /// Removes first occurence of an element with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element to remove.</param>
        /// <returns>true if the an element is removed;
        /// false if the key or the value were not found.</returns>
        public bool Remove(K key, V value)
        {

            if (the_list.TryGetValue(key, out List<V> valueList))
            {
                if (valueList.Remove(value))
                {
                    if (valueList.Count == 0)
                    {
                        the_list.Remove(key);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all occurences of elements with a specified key and value.
        /// </summary>
        /// <param name="key">The key of the elements to remove.</param>
        /// <param name="value">The value of the elements to remove.</param>
        /// <returns>Number of elements removed.</returns>
        public int RemoveAll(K key, V value)
        {
            int n = 0;

            if (the_list.TryGetValue(key, out List<V> valueList))
            {
                while (valueList.Remove(value))
                {
                    n++;
                }
                if (valueList.Count == 0)
                {
                    the_list.Remove(key);
                }
            }
            return n;
        }

        /// <summary>
        /// Gets the total number of values contained in the MultiMap.
        /// </summary>
        public int CountAll
        {
            get
            {
                int n = 0;

                foreach (List<V> valueList in the_list.Values)
                {
                    n += valueList.Count;
                }
                return n;
            }
        }

        /// <summary>
        /// Determines whether the MultiMap contains an element with a specific
        /// key / value pair.
        /// </summary>
        /// <param name="key">Key of the element to search for.</param>
        /// <param name="value">Value of the element to search for.</param>
        /// <returns>true if the element was found; otherwise false.</returns>
        public bool Contains(K key, V value)
        {

            if (the_list.TryGetValue(key, out List<V> valueList))
            {
                return valueList.Contains(value);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the MultiMap contains an element with a specific value.
        /// </summary>
        /// <param name="value">Value of the element to search for.</param>
        /// <returns>true if the element was found; otherwise false.</returns>
        public bool Contains(V value)
        {
            foreach (List<V> valueList in the_list.Values)
            {
                if (valueList.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual IList<Tuple<K, V>> GetPairs()
        {
            IList<Tuple<K, V>> pairs = new ArrayList<Tuple<K, V>>();
            foreach (var pair in the_list)
            {
                foreach (V value in pair.Value)
                {
                    pairs.Add(Tuple.Create(pair.Key, value));
                }
            }
            return pairs;
        }

        public IEnumerator<KeyValuePair<K, List<V>>> GetEnumerator()
        {
            foreach (KeyValuePair<K, List<V>> pair in the_list)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<K, List<V>> pair in the_list)
            {
                yield return pair;
            }
        }

        public bool TryGetValue(K key, out List<V> value)
        {
            var ret = the_list.TryGetValue(key, out List<V> v);
            value = v;
            return ret;
        }

        public bool ContainsKey(K key)
        {
            return the_list.ContainsKey(key);
        }

        public List<V> this[K key]
        {
            get { return the_list[key]; }
            set { the_list[key] = value; }
        }

        public int Count
        {
            get { return the_list.Count; }
        }

        public bool Remove(K key)
        {
            return the_list.Remove(key);
        }

        public void Clear()
        {
            the_list.Clear();
        }

        public IEnumerable<K> Keys
        {
            get { return the_list.Keys; }
        }

        public IList<V> Values
        {
            get
            {
                List<V> list = new List<V>();
                foreach (KeyValuePair<K, List<V>> pair in the_list)
                {
                    foreach (var v in pair.Value)
                    {
                        list.Add(v);
                    }
                }
                return list;
            }
        }
    }
}
