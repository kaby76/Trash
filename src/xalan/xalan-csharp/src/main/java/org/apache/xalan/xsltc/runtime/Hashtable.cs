using System;
using System.Text;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: Hashtable.java 1225436 2011-12-29 05:09:31Z mrglavas $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// IMPORTANT NOTE:
	/// This code was taken from Sun's Java1.1 JDK java.util.HashTable.java
	/// All "synchronized" keywords and some methods we do not need have been 
	/// all been removed.
	/// </summary>

	/// <summary>
	/// Object that wraps entries in the hash-table
	/// @author Morten Jorgensen
	/// </summary>
	internal class HashtableEntry
	{
		internal int hash;
		internal object key;
		internal object value;
		internal HashtableEntry next;

		protected internal virtual object clone()
		{
		HashtableEntry entry = new HashtableEntry();
		entry.hash = hash;
		entry.key = key;
		entry.value = value;
		entry.next = (next != null) ? (HashtableEntry)next.clone() : null;
		return entry;
		}
	}

	/// <summary>
	/// The main hash-table implementation
	/// </summary>
	public class Hashtable
	{

		[NonSerialized]
		private HashtableEntry[] table; // hash-table entries
		[NonSerialized]
		private int count; // number of entries
		private int threshold; // current size of hash-tabke
		private float loadFactor; // load factor

		/// <summary>
		/// Constructs a new, empty hashtable with the specified initial 
		/// capacity and the specified load factor. 
		/// </summary>
		public Hashtable(int initialCapacity, float loadFactor)
		{
		if (initialCapacity <= 0)
		{
			initialCapacity = 11;
		}
		if (loadFactor <= 0.0)
		{
			loadFactor = 0.75f;
		}
		this.loadFactor = loadFactor;
		table = new HashtableEntry[initialCapacity];
		threshold = (int)(initialCapacity * loadFactor);
		}

		/// <summary>
		/// Constructs a new, empty hashtable with the specified initial capacity
		/// and default load factor.
		/// </summary>
		public Hashtable(int initialCapacity) : this(initialCapacity, 0.75f)
		{
		}

		/// <summary>
		/// Constructs a new, empty hashtable with a default capacity and load
		/// factor. 
		/// </summary>
		public Hashtable() : this(101, 0.75f)
		{
		}

		/// <summary>
		/// Returns the number of keys in this hashtable.
		/// </summary>
		public virtual int size()
		{
		return count;
		}

		/// <summary>
		/// Tests if this hashtable maps no keys to values.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
			return count == 0;
			}
		}

		/// <summary>
		/// Returns an enumeration of the keys in this hashtable.
		/// </summary>
		public virtual System.Collections.IEnumerator keys()
		{
		return new HashtableEnumerator(table, true);
		}

		/// <summary>
		/// Returns an enumeration of the values in this hashtable.
		/// Use the Enumeration methods on the returned object to fetch the elements
		/// sequentially.
		/// </summary>
		public virtual System.Collections.IEnumerator elements()
		{
		return new HashtableEnumerator(table, false);
		}

		/// <summary>
		/// Tests if some key maps into the specified value in this hashtable.
		/// This operation is more expensive than the <code>containsKey</code>
		/// method.
		/// </summary>
		public virtual bool contains(object value)
		{

		if (value == null)
		{
			throw new System.NullReferenceException();
		}

		int i;
		HashtableEntry e;
		HashtableEntry[] tab = table;

		for (i = tab.Length ; i-- > 0 ;)
		{
			for (e = tab[i] ; e != null ; e = e.next)
			{
			if (e.value.Equals(value))
			{
				return true;
			}
			}
		}
		return false;
		}

		/// <summary>
		/// Tests if the specified object is a key in this hashtable.
		/// </summary>
		public virtual bool containsKey(object key)
		{
		HashtableEntry e;
		HashtableEntry[] tab = table;
		int hash = key.GetHashCode();
		int index = (hash & 0x7FFFFFFF) % tab.Length;

		for (e = tab[index] ; e != null ; e = e.next)
		{
			if ((e.hash == hash) && e.key.Equals(key))
			{
			return true;
			}
		}

		return false;
		}

		/// <summary>
		/// Returns the value to which the specified key is mapped in this hashtable.
		/// </summary>
		public virtual object get(object key)
		{
		HashtableEntry e;
		HashtableEntry[] tab = table;
		int hash = key.GetHashCode();
		int index = (hash & 0x7FFFFFFF) % tab.Length;

		for (e = tab[index] ; e != null ; e = e.next)
		{
			if ((e.hash == hash) && e.key.Equals(key))
			{
			return e.value;
			}
		}

		return null;
		}

		/// <summary>
		/// Rehashes the contents of the hashtable into a hashtable with a 
		/// larger capacity. This method is called automatically when the 
		/// number of keys in the hashtable exceeds this hashtable's capacity 
		/// and load factor. 
		/// </summary>
		protected internal virtual void rehash()
		{
		HashtableEntry e, old;
		int i, index;
		int oldCapacity = table.Length;
		HashtableEntry[] oldTable = table;

		int newCapacity = oldCapacity * 2 + 1;
		HashtableEntry[] newTable = new HashtableEntry[newCapacity];

		threshold = (int)(newCapacity * loadFactor);
		table = newTable;

		for (i = oldCapacity ; i-- > 0 ;)
		{
			for (old = oldTable[i] ; old != null ;)
			{
			e = old;
			old = old.next;
			index = (e.hash & 0x7FFFFFFF) % newCapacity;
			e.next = newTable[index];
			newTable[index] = e;
			}
		}
		}

		/// <summary>
		/// Maps the specified <code>key</code> to the specified 
		/// <code>value</code> in this hashtable. Neither the key nor the 
		/// value can be <code>null</code>. 
		/// <para>
		/// The value can be retrieved by calling the <code>get</code> method 
		/// with a key that is equal to the original key. 
		/// </para>
		/// </summary>
		public virtual object put(object key, object value)
		{
		// Make sure the value is not null
		if (value == null)
		{
			throw new System.NullReferenceException();
		}

		// Makes sure the key is not already in the hashtable.
		HashtableEntry e;
		HashtableEntry[] tab = table;
		int hash = key.GetHashCode();
		int index = (hash & 0x7FFFFFFF) % tab.Length;

		for (e = tab[index] ; e != null ; e = e.next)
		{
			if ((e.hash == hash) && e.key.Equals(key))
			{
			object old = e.value;
			e.value = value;
			return old;
			}
		}

		// Rehash the table if the threshold is exceeded
		if (count >= threshold)
		{
			rehash();
			return put(key, value);
		}

		// Creates the new entry.
		e = new HashtableEntry();
		e.hash = hash;
		e.key = key;
		e.value = value;
		e.next = tab[index];
		tab[index] = e;
		count++;
		return null;
		}

		/// <summary>
		/// Removes the key (and its corresponding value) from this 
		/// hashtable. This method does nothing if the key is not in the hashtable.
		/// </summary>
		public virtual object remove(object key)
		{
		HashtableEntry e, prev;
		HashtableEntry[] tab = table;
		int hash = key.GetHashCode();
		int index = (hash & 0x7FFFFFFF) % tab.Length;
		for (e = tab[index], prev = null ; e != null ; prev = e, e = e.next)
		{
			if ((e.hash == hash) && e.key.Equals(key))
			{
			if (prev != null)
			{
				prev.next = e.next;
			}
			else
			{
				tab[index] = e.next;
			}
			count--;
			return e.value;
			}
		}
		return null;
		}

		/// <summary>
		/// Clears this hashtable so that it contains no keys. 
		/// </summary>
		public virtual void clear()
		{
		HashtableEntry[] tab = table;
		for (int index = tab.Length; --index >= 0;)
		{
			tab[index] = null;
		}
		count = 0;
		}

		/// <summary>
		/// Returns a rather long string representation of this hashtable.
		/// Handy for debugging - leave it here!!!
		/// </summary>
		public override string ToString()
		{
		int i;
		int max = size() - 1;
		StringBuilder buf = new StringBuilder();
		System.Collections.IEnumerator k = keys();
		System.Collections.IEnumerator e = elements();
		buf.Append("{");

		for (i = 0; i <= max; i++)
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			string s1 = k.nextElement().ToString();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			string s2 = e.nextElement().ToString();
			buf.Append(s1 + "=" + s2);
			if (i < max)
			{
				buf.Append(", ");
			}
		}
		buf.Append("}");
		return buf.ToString();
		}

		/// <summary>
		/// A hashtable enumerator class.  This class should remain opaque 
		/// to the client. It will use the Enumeration interface.
		/// </summary>
		internal class HashtableEnumerator : System.Collections.IEnumerator
		{
		internal bool keys;
		internal int index;
		internal HashtableEntry[] table;
		internal HashtableEntry entry;

		internal HashtableEnumerator(HashtableEntry[] table, bool keys)
		{
			this.table = table;
			this.keys = keys;
			this.index = table.Length;
		}

		public virtual bool hasMoreElements()
		{
			if (entry != null)
			{
			return true;
			}
			while (index-- > 0)
			{
			if ((entry = table[index]) != null)
			{
				return true;
			}
			}
			return false;
		}

		public virtual object nextElement()
		{
			if (entry == null)
			{
			while ((index-- > 0) && ((entry = table[index]) == null))
			{
					;
			}
			}
			if (entry != null)
			{
			HashtableEntry e = entry;
			entry = e.next;
			return keys ? e.key : e.value;
			}
			return null;
		}
		}

	}

}