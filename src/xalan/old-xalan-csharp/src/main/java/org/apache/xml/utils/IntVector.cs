using System;

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
 * $Id: IntVector.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple table that stores a list of int.
	/// 
	/// This version is based on a "realloc" strategy -- a simle array is
	/// used, and when more storage is needed, a larger array is obtained
	/// and all existing data is recopied into it. As a result, read/write
	/// access to existing nodes is O(1) fast but appending may be O(N**2)
	/// slow. See also SuballocatedIntVector.
	/// @xsl.usage internal
	/// </summary>
	public class IntVector : ICloneable
	{

	  /// <summary>
	  /// Size of blocks to allocate </summary>
	  protected internal int m_blocksize;

	  /// <summary>
	  /// Array of ints </summary>
	  protected internal int[] m_map; // IntStack is trying to see this directly

	  /// <summary>
	  /// Number of ints in array </summary>
	  protected internal int m_firstFree = 0;

	  /// <summary>
	  /// Size of array </summary>
	  protected internal int m_mapSize;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public IntVector()
	  {

		m_blocksize = 32;
		m_mapSize = m_blocksize;
		m_map = new int[m_blocksize];
	  }

	  /// <summary>
	  /// Construct a IntVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public IntVector(int blocksize)
	  {

		m_blocksize = blocksize;
		m_mapSize = blocksize;
		m_map = new int[blocksize];
	  }

	  /// <summary>
	  /// Construct a IntVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public IntVector(int blocksize, int increaseSize)
	  {

		m_blocksize = increaseSize;
		m_mapSize = blocksize;
		m_map = new int[blocksize];
	  }

	  /// <summary>
	  /// Copy constructor for IntVector
	  /// </summary>
	  /// <param name="v"> Existing IntVector to copy </param>
	  public IntVector(IntVector v)
	  {
		  m_map = new int[v.m_mapSize];
		m_mapSize = v.m_mapSize;
		m_firstFree = v.m_firstFree;
		  m_blocksize = v.m_blocksize;
		  Array.Copy(v.m_map, 0, m_map, 0, m_firstFree);
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> length of the list </returns>
	  public int size()
	  {
		return m_firstFree;
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> length of the list </returns>
	  public int Size
	  {
		  set
		  {
			m_firstFree = value;
		  }
	  }


	  /// <summary>
	  /// Append a int onto the vector.
	  /// </summary>
	  /// <param name="value"> Int to add to the list  </param>
	  public void addElement(int value)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Append several int values onto the vector.
	  /// </summary>
	  /// <param name="value"> Int to add to the list  </param>
	  public void addElements(int value, int numberOfElements)
	  {

		if ((m_firstFree + numberOfElements) >= m_mapSize)
		{
		  m_mapSize += (m_blocksize + numberOfElements);

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		for (int i = 0; i < numberOfElements; i++)
		{
		  m_map[m_firstFree] = value;
		  m_firstFree++;
		}
	  }

	  /// <summary>
	  /// Append several slots onto the vector, but do not set the values.
	  /// </summary>
	  /// <param name="numberOfElements"> Int to add to the list  </param>
	  public void addElements(int numberOfElements)
	  {

		if ((m_firstFree + numberOfElements) >= m_mapSize)
		{
		  m_mapSize += (m_blocksize + numberOfElements);

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_firstFree += numberOfElements;
	  }


	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// </summary>
	  /// <param name="value"> Int to insert </param>
	  /// <param name="at"> Index of where to insert  </param>
	  public void insertElementAt(int value, int at)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		if (at <= (m_firstFree - 1))
		{
		  Array.Copy(m_map, at, m_map, at + 1, m_firstFree - at);
		}

		m_map[at] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// </summary>
	  public void removeAllElements()
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  m_map[i] = int.MinValue;
		}

		m_firstFree = 0;
	  }

	  /// <summary>
	  /// Removes the first occurrence of the argument from this vector.
	  /// If the object is found in this vector, each component in the vector
	  /// with an index greater or equal to the object's index is shifted
	  /// downward to have an index one smaller than the value it had
	  /// previously.
	  /// </summary>
	  /// <param name="s"> Int to remove from array
	  /// </param>
	  /// <returns> True if the int was removed, false if it was not found </returns>
	  public bool removeElement(int s)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i] == s)
		  {
			if ((i + 1) < m_firstFree)
			{
			  Array.Copy(m_map, i + 1, m_map, i - 1, m_firstFree - i);
			}
			else
			{
			  m_map[i] = int.MinValue;
			}

			m_firstFree--;

			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Deletes the component at the specified index. Each component in
	  /// this vector with an index greater or equal to the specified
	  /// index is shifted downward to have an index one smaller than
	  /// the value it had previously.
	  /// </summary>
	  /// <param name="i"> index of where to remove and int </param>
	  public void removeElementAt(int i)
	  {

		if (i > m_firstFree)
		{
		  Array.Copy(m_map, i + 1, m_map, i, m_firstFree);
		}
		else
		{
		  m_map[i] = int.MinValue;
		}

		m_firstFree--;
	  }

	  /// <summary>
	  /// Sets the component at the specified index of this vector to be the
	  /// specified object. The previous component at that position is discarded.
	  /// 
	  /// The index must be a value greater than or equal to 0 and less
	  /// than the current size of the vector.
	  /// </summary>
	  /// <param name="value"> object to set </param>
	  /// <param name="index"> Index of where to set the object </param>
	  public void setElementAt(int value, int index)
	  {
		m_map[index] = value;
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> index of object to get
	  /// </param>
	  /// <returns> object at given index </returns>
	  public int elementAt(int i)
	  {
		return m_map[i];
	  }

	  /// <summary>
	  /// Tell if the table contains the given node.
	  /// </summary>
	  /// <param name="s"> object to look for
	  /// </param>
	  /// <returns> true if the object is in the list </returns>
	  public bool contains(int s)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i] == s)
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> object to look for </param>
	  /// <param name="index"> Index of where to begin search </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public int indexOf(int elem, int index)
	  {

		for (int i = index; i < m_firstFree; i++)
		{
		  if (m_map[i] == elem)
		  {
			return i;
		  }
		}

		return int.MinValue;
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> object to look for </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public int indexOf(int elem)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i] == elem)
		  {
			return i;
		  }
		}

		return int.MinValue;
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> Object to look for </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public int lastIndexOf(int elem)
	  {

		for (int i = (m_firstFree - 1); i >= 0; i--)
		{
		  if (m_map[i] == elem)
		  {
			return i;
		  }
		}

		return int.MinValue;
	  }

	  /// <summary>
	  /// Returns clone of current IntVector
	  /// </summary>
	  /// <returns> clone of current IntVector </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {
		  return new IntVector(this);
	  }

	}

}