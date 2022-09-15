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
 * $Id: ObjectArray.java 468638 2006-10-28 06:52:06Z minchau $
 */
namespace org.apache.xalan.lib.sql
{

	/// <summary>
	/// Provide a simple Array storage mechinsim where  native Arrays will be use as
	/// the basic storage mechinism but the Arrays will be stored as blocks.
	/// The size of the Array blocks is determine during object construction.
	/// This is intended to be a simple storage mechinsim where the storage only
	/// can grow. Array elements can not be removed, only added to.
	/// </summary>
	public class ObjectArray
	{
	  private int m_minArraySize = 10;
	  /// <summary>
	  /// The container of all the sub arrays
	  /// </summary>
	  private ArrayList m_Arrays = new ArrayList(200);

	  /// <summary>
	  /// An index that porvides the Vector entry for the current Array that is
	  /// being appended to.
	  /// </summary>
	  private _ObjectArray m_currentArray;


	  /// <summary>
	  /// The next offset in the current Array to append a new object
	  /// </summary>
	  private int m_nextSlot;


	  public ObjectArray()
	  {
		//
		// Default constructor will work with a minimal fixed size
		//
		init(10);
	  }

	  /// <param name="minArraySize"> The size of the Arrays stored in the Vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public ObjectArray(final int minArraySize)
	  public ObjectArray(int minArraySize)
	  {
		init(minArraySize);
	  }

	  /// <param name="size">
	  ///  </param>
	  private void init(int size)
	  {
		m_minArraySize = size;
		m_currentArray = new _ObjectArray(this, m_minArraySize);
	  }

	  /// <param name="idx"> Index of the Object in the Array
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Object getAt(final int idx)
	  public virtual object getAt(int idx)
	  {
		int arrayIndx = idx / m_minArraySize;
		int arrayOffset = idx - (arrayIndx * m_minArraySize);

		//
		// If the array has been off loaded to the Vector Storage them
		// grab it from there.
		if (arrayIndx < m_Arrays.Count)
		{
		  _ObjectArray a = (_ObjectArray)m_Arrays[arrayIndx];
		  return a.objects[arrayOffset];
		}
		else
		{
		  // We must be in the current array, so pull it from there

		  // %REVIEW% We may want to check to see if arrayIndx is only
		  // one freater that the m_Arrays.size(); This code is safe but
		  // will repete if the index is greater than the array size.
		  return m_currentArray.objects[arrayOffset];
		}
	  }

	  /// <param name="idx"> Index of the Object in the Array </param>
	  /// <param name="obj"> , The value to set in the Array
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setAt(final int idx, final Object obj)
	  public virtual void setAt(int idx, object obj)
	  {
		int arrayIndx = idx / m_minArraySize;
		int arrayOffset = idx - (arrayIndx * m_minArraySize);

		//
		// If the array has been off loaded to the Vector Storage them
		// grab it from there.
		if (arrayIndx < m_Arrays.Count)
		{
		  _ObjectArray a = (_ObjectArray)m_Arrays[arrayIndx];
		  a.objects[arrayOffset] = obj;
		}
		else
		{
		  // We must be in the current array, so pull it from there

		  // %REVIEW% We may want to check to see if arrayIndx is only
		  // one freater that the m_Arrays.size(); This code is safe but
		  // will repete if the index is greater than the array size.
		  m_currentArray.objects[arrayOffset] = obj;
		}
	  }



	  /// <param name="o"> Object to be appended to the Array
	  ///  </param>
	  public virtual int append(object o)
	  {
		if (m_nextSlot >= m_minArraySize)
		{
		  m_Arrays.Add(m_currentArray);
		  m_nextSlot = 0;
		  m_currentArray = new _ObjectArray(this, m_minArraySize);
		}

		m_currentArray.objects[m_nextSlot] = o;

		int pos = (m_Arrays.Count * m_minArraySize) + m_nextSlot;

		m_nextSlot++;

		return pos;
	  }


	  internal class _ObjectArray
	  {
		  private readonly ObjectArray outerInstance;

		public object[] objects;
		/// <param name="size"> </param>
		public _ObjectArray(ObjectArray outerInstance, int size)
		{
			this.outerInstance = outerInstance;
		  objects = new object[size];
		}
	  }

	  /// <param name="args">
	  ///  </param>
	  public static void Main(string[] args)
	  {
		string[] word = new string[] {"Zero","One","Two","Three","Four","Five", "Six","Seven","Eight","Nine","Ten", "Eleven","Twelve","Thirteen","Fourteen","Fifteen", "Sixteen","Seventeen","Eighteen","Nineteen","Twenty", "Twenty-One","Twenty-Two","Twenty-Three","Twenty-Four", "Twenty-Five","Twenty-Six","Twenty-Seven","Twenty-Eight", "Twenty-Nine","Thirty","Thirty-One","Thirty-Two", "Thirty-Three","Thirty-Four","Thirty-Five","Thirty-Six", "Thirty-Seven","Thirty-Eight","Thirty-Nine"};

		ObjectArray m_ObjectArray = new ObjectArray();
		// Add them in, using the default block size
		for (int x = 0; x < word.Length; x++)
		{
		  Console.Write(" - " + m_ObjectArray.append(word[x]));
		}

		Console.WriteLine("\n");
		// Now let's read them out sequentally
		for (int x = 0; x < word.Length; x++)
		{
		  string s = (string) m_ObjectArray.getAt(x);
		  Console.WriteLine(s);
		}

		// Some Random Access
		Console.WriteLine((string) m_ObjectArray.getAt(5));
		Console.WriteLine((string) m_ObjectArray.getAt(10));
		Console.WriteLine((string) m_ObjectArray.getAt(20));
		Console.WriteLine((string) m_ObjectArray.getAt(2));
		Console.WriteLine((string) m_ObjectArray.getAt(15));
		Console.WriteLine((string) m_ObjectArray.getAt(30));
		Console.WriteLine((string) m_ObjectArray.getAt(6));
		Console.WriteLine((string) m_ObjectArray.getAt(8));

		// Out of bounds
		Console.WriteLine((string) m_ObjectArray.getAt(40));

	  }
	}

}