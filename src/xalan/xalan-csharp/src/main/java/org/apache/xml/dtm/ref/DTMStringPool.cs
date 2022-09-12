using System;
using System.Collections;

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
 * $Id: DTMStringPool.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xml.dtm.@ref
{

	using IntVector = org.apache.xml.utils.IntVector;

	/// <summary>
	/// <para>DTMStringPool is an "interning" mechanism for strings. It will
	/// create a stable 1:1 mapping between a set of string values and a set of
	/// integer index values, so the integers can be used to reliably and
	/// uniquely identify (and when necessary retrieve) the strings.</para>
	/// 
	/// <para>Design Priorities:
	/// <ul>
	/// <li>String-to-index lookup speed is critical.</li>
	/// <li>Index-to-String lookup speed is slightly less so.</li>
	/// <li>Threadsafety is not guaranteed at this level.
	/// Enforce that in the application if needed.</li>
	/// <li>Storage efficiency is an issue but not a huge one.
	/// It is expected that string pools won't exceed about 2000 entries.</li>
	/// </ul>
	/// </para>
	/// 
	/// <para>Implementation detail: A standard Hashtable is relatively
	/// inefficient when looking up primitive int values, especially when
	/// we're already maintaining an int-to-string vector.  So I'm
	/// maintaining a simple hash chain within this class.</para>
	/// 
	/// <para>NOTE: There is nothing in the code that has a real dependency upon
	/// String. It would work with any object type that implements reliable
	/// .hashCode() and .equals() operations. The API enforces Strings because
	/// it's safer that way, but this could trivially be turned into a general
	/// ObjectPool if one was needed.</para>
	/// 
	/// <para>Status: Passed basic test in main().</para>
	/// 
	/// </summary>
	public class DTMStringPool
	{
	  internal ArrayList m_intToString;
	  internal const int HASHPRIME = 101;
	  internal int[] m_hashStart = new int[HASHPRIME];
	  internal IntVector m_hashChain;
	  public const int NULL = -1;

	  /// <summary>
	  /// Create a DTMStringPool using the given chain size
	  /// </summary>
	  /// <param name="chainSize"> The size of the hash chain vector </param>
	  public DTMStringPool(int chainSize)
	  {
		  m_intToString = new ArrayList();
		  m_hashChain = new IntVector(chainSize);
		  removeAllElements();

		  // -sb Add this to force empty strings to be index 0.
		  stringToIndex("");
	  }

	  public DTMStringPool() : this(512)
	  {
	  }

	  public virtual void removeAllElements()
	  {
		  m_intToString.Clear();
		  for (int i = 0;i < HASHPRIME;++i)
		  {
			m_hashStart[i] = NULL;
		  }
		  m_hashChain.removeAllElements();
	  }

	  /// <returns> string whose value is uniquely identified by this integer index. </returns>
	  /// <exception cref="java.lang.ArrayIndexOutOfBoundsException">
	  ///  if index doesn't map to a string.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String indexToString(int i) throws java.lang.ArrayIndexOutOfBoundsException
	  public virtual string indexToString(int i)
	  {
		  if (i == NULL)
		  {
			  return null;
		  }
		  return (string) m_intToString[i];
	  }

	  /// <returns> integer index uniquely identifying the value of this string. </returns>
	  public virtual int stringToIndex(string s)
	  {
		  if (string.ReferenceEquals(s, null))
		  {
			  return NULL;
		  }

		  int hashslot = s.GetHashCode() % HASHPRIME;
		  if (hashslot < 0)
		  {
			  hashslot = -hashslot;
		  }

		  // Is it one we already know?
		  int hashlast = m_hashStart[hashslot];
		  int hashcandidate = hashlast;
		  while (hashcandidate != NULL)
		  {
			  if (m_intToString[hashcandidate].Equals(s))
			  {
				return hashcandidate;
			  }

			  hashlast = hashcandidate;
			  hashcandidate = m_hashChain.elementAt(hashcandidate);
		  }

		  // New value. Add to tables.
		  int newIndex = m_intToString.Count;
		  m_intToString.Add(s);

		  m_hashChain.addElement(NULL); // Initialize to no-following-same-hash
		  if (hashlast == NULL) // First for this hash
		  {
			m_hashStart[hashslot] = newIndex;
		  }
		  else // Link from previous with same hash
		  {
			m_hashChain.setElementAt(newIndex,hashlast);
		  }

		  return newIndex;
	  }

	  /// <summary>
	  /// Command-line unit test driver. This test relies on the fact that
	  /// this version of the pool assigns indices consecutively, starting
	  /// from zero, as new unique strings are encountered.
	  /// </summary>
	  public static void Main(string[] args)
	  {
		string[] word = new string[] {"Zero","One","Two","Three","Four","Five", "Six","Seven","Eight","Nine","Ten", "Eleven","Twelve","Thirteen","Fourteen","Fifteen", "Sixteen","Seventeen","Eighteen","Nineteen","Twenty", "Twenty-One","Twenty-Two","Twenty-Three","Twenty-Four", "Twenty-Five","Twenty-Six","Twenty-Seven","Twenty-Eight", "Twenty-Nine","Thirty","Thirty-One","Thirty-Two", "Thirty-Three","Thirty-Four","Thirty-Five","Thirty-Six", "Thirty-Seven","Thirty-Eight","Thirty-Nine"};

		DTMStringPool pool = new DTMStringPool();

		Console.WriteLine("If no complaints are printed below, we passed initial test.");

		for (int pass = 0;pass <= 1;++pass)
		{
			int i;

			for (i = 0;i < word.Length;++i)
			{
				int j = pool.stringToIndex(word[i]);
				if (j != i)
				{
				  Console.WriteLine("\tMismatch populating pool: assigned " + j + " for create " + i);
				}
			}

			for (i = 0;i < word.Length;++i)
			{
				int j = pool.stringToIndex(word[i]);
				if (j != i)
				{
				  Console.WriteLine("\tMismatch in stringToIndex: returned " + j + " for lookup " + i);
				}
			}

			for (i = 0;i < word.Length;++i)
			{
				string w = pool.indexToString(i);
				if (!word[i].Equals(w))
				{
				  Console.WriteLine("\tMismatch in indexToString: returned" + w + " for lookup " + i);
				}
			}

			pool.removeAllElements();

			Console.WriteLine("\nPass " + pass + " complete\n");
		} // end pass loop
	  }
	}

}