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
 * $Id: NodeVector.java 1225445 2011-12-29 06:08:53Z mrglavas $
 */
namespace org.apache.xml.utils
{

	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// A very simple table that stores a list of Nodes.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class NodeVector : ICloneable
	{
		internal const long serialVersionUID = -713473092200731870L;

	  /// <summary>
	  /// Size of blocks to allocate.
	  ///  @serial          
	  /// </summary>
	  private int m_blocksize;

	  /// <summary>
	  /// Array of nodes this points to.
	  ///  @serial          
	  /// </summary>
	  private int[] m_map;

	  /// <summary>
	  /// Number of nodes in this NodeVector.
	  ///  @serial          
	  /// </summary>
	  protected internal int m_firstFree = 0;

	  /// <summary>
	  /// Size of the array this points to.
	  ///  @serial           
	  /// </summary>
	  private int m_mapSize; // lazy initialization

	  /// <summary>
	  /// Default constructor.
	  /// </summary>
	  public NodeVector()
	  {
		m_blocksize = 32;
		m_mapSize = 0;
	  }

	  /// <summary>
	  /// Construct a NodeVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of blocks to allocate </param>
	  public NodeVector(int blocksize)
	  {
		m_blocksize = blocksize;
		m_mapSize = 0;
	  }

	  /// <summary>
	  /// Get a cloned LocPathIterator.
	  /// </summary>
	  /// <returns> A clone of this
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {

		NodeVector clone = (NodeVector) base.clone();

		if ((null != this.m_map) && (this.m_map == clone.m_map))
		{
		  clone.m_map = new int[this.m_map.Length];

		  Array.Copy(this.m_map, 0, clone.m_map, 0, this.m_map.Length);
		}

		return clone;
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of nodes in this NodeVector </returns>
	  public virtual int size()
	  {
		return m_firstFree;
	  }

	  /// <summary>
	  /// Append a Node onto the vector.
	  /// </summary>
	  /// <param name="value"> Node to add to the vector </param>
	  public virtual void addElement(int value)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  if (null == m_map)
		  {
			m_map = new int[m_blocksize];
			m_mapSize = m_blocksize;
		  }
		  else
		  {
			m_mapSize += m_blocksize;

			int[] newMap = new int[m_mapSize];

			Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

			m_map = newMap;
		  }
		}

		m_map[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Append a Node onto the vector.
	  /// </summary>
	  /// <param name="value"> Node to add to the vector </param>
	  public void push(int value)
	  {

		int ff = m_firstFree;

		if ((ff + 1) >= m_mapSize)
		{
		  if (null == m_map)
		  {
			m_map = new int[m_blocksize];
			m_mapSize = m_blocksize;
		  }
		  else
		  {
			m_mapSize += m_blocksize;

			int[] newMap = new int[m_mapSize];

			Array.Copy(m_map, 0, newMap, 0, ff + 1);

			m_map = newMap;
		  }
		}

		m_map[ff] = value;

		ff++;

		m_firstFree = ff;
	  }

	  /// <summary>
	  /// Pop a node from the tail of the vector and return the result.
	  /// </summary>
	  /// <returns> the node at the tail of the vector </returns>
	  public int pop()
	  {

		m_firstFree--;

		int n = m_map[m_firstFree];

		m_map[m_firstFree] = DTM.NULL;

		return n;
	  }

	  /// <summary>
	  /// Pop a node from the tail of the vector and return the
	  /// top of the stack after the pop.
	  /// </summary>
	  /// <returns> The top of the stack after it's been popped </returns>
	  public int popAndTop()
	  {

		m_firstFree--;

		m_map[m_firstFree] = DTM.NULL;

		return (m_firstFree == 0) ? DTM.NULL : m_map[m_firstFree - 1];
	  }

	  /// <summary>
	  /// Pop a node from the tail of the vector.
	  /// </summary>
	  public void popQuick()
	  {

		m_firstFree--;

		m_map[m_firstFree] = DTM.NULL;
	  }

	  /// <summary>
	  /// Return the node at the top of the stack without popping the stack.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <returns> Node at the top of the stack or null if stack is empty. </returns>
	  public int peepOrNull()
	  {
		return ((null != m_map) && (m_firstFree > 0)) ? m_map[m_firstFree - 1] : DTM.NULL;
	  }

	  /// <summary>
	  /// Push a pair of nodes into the stack.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <param name="v1"> First node to add to vector </param>
	  /// <param name="v2"> Second node to add to vector </param>
	  public void pushPair(int v1, int v2)
	  {

		if (null == m_map)
		{
		  m_map = new int[m_blocksize];
		  m_mapSize = m_blocksize;
		}
		else
		{
		  if ((m_firstFree + 2) >= m_mapSize)
		  {
			m_mapSize += m_blocksize;

			int[] newMap = new int[m_mapSize];

			Array.Copy(m_map, 0, newMap, 0, m_firstFree);

			m_map = newMap;
		  }
		}

		m_map[m_firstFree] = v1;
		m_map[m_firstFree + 1] = v2;
		m_firstFree += 2;
	  }

	  /// <summary>
	  /// Pop a pair of nodes from the tail of the stack.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  public void popPair()
	  {

		m_firstFree -= 2;
		m_map[m_firstFree] = DTM.NULL;
		m_map[m_firstFree + 1] = DTM.NULL;
	  }

	  /// <summary>
	  /// Set the tail of the stack to the given node.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <param name="n"> Node to set at the tail of vector </param>
	  public int Tail
	  {
		  set
		  {
			m_map[m_firstFree - 1] = value;
		  }
	  }

	  /// <summary>
	  /// Set the given node one position from the tail.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <param name="n"> Node to set </param>
	  public int TailSub1
	  {
		  set
		  {
			m_map[m_firstFree - 2] = value;
		  }
	  }

	  /// <summary>
	  /// Return the node at the tail of the vector without popping
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <returns> Node at the tail of the vector </returns>
	  public int peepTail()
	  {
		return m_map[m_firstFree - 1];
	  }

	  /// <summary>
	  /// Return the node one position from the tail without popping.
	  /// Special purpose method for TransformerImpl, pushElemTemplateElement.
	  /// Performance critical.
	  /// </summary>
	  /// <returns> Node one away from the tail </returns>
	  public int peepTailSub1()
	  {
		return m_map[m_firstFree - 2];
	  }

	  /// <summary>
	  /// Insert a node in order in the list.
	  /// </summary>
	  /// <param name="value"> Node to insert </param>
	  public virtual void insertInOrder(int value)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (value < m_map[i])
		  {
			insertElementAt(value, i);

			return;
		  }
		}

		addElement(value);
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// </summary>
	  /// <param name="value"> Node to insert </param>
	  /// <param name="at"> Position where to insert </param>
	  public virtual void insertElementAt(int value, int at)
	  {

		if (null == m_map)
		{
		  m_map = new int[m_blocksize];
		  m_mapSize = m_blocksize;
		}
		else if ((m_firstFree + 1) >= m_mapSize)
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
	  /// Append the nodes to the list.
	  /// </summary>
	  /// <param name="nodes"> NodeVector to append to this list </param>
	  public virtual void appendNodes(NodeVector nodes)
	  {

		int nNodes = nodes.size();

		if (null == m_map)
		{
		  m_mapSize = nNodes + m_blocksize;
		  m_map = new int[m_mapSize];
		}
		else if ((m_firstFree + nNodes) >= m_mapSize)
		{
		  m_mapSize += (nNodes + m_blocksize);

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + nNodes);

		  m_map = newMap;
		}

		Array.Copy(nodes.m_map, 0, m_map, m_firstFree, nNodes);

		m_firstFree += nNodes;
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// </summary>
	  public virtual void removeAllElements()
	  {

		if (null == m_map)
		{
		  return;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  m_map[i] = DTM.NULL;
		}

		m_firstFree = 0;
	  }

	  /// <summary>
	  /// Set the length to zero, but don't clear the array.
	  /// </summary>
	  public virtual void RemoveAllNoClear()
	  {

		if (null == m_map)
		{
		  return;
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
	  /// <param name="s"> Node to remove from the list
	  /// </param>
	  /// <returns> True if the node was successfully removed </returns>
	  public virtual bool removeElement(int s)
	  {

		if (null == m_map)
		{
		  return false;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  int node = m_map[i];

		  if (node == s)
		  {
			if (i > m_firstFree)
			{
			  Array.Copy(m_map, i + 1, m_map, i - 1, m_firstFree - i);
			}
			else
			{
			  m_map[i] = DTM.NULL;
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
	  /// <param name="i"> Index of node to remove </param>
	  public virtual void removeElementAt(int i)
	  {

		if (null == m_map)
		{
		  return;
		}

		if (i > m_firstFree)
		{
		  Array.Copy(m_map, i + 1, m_map, i - 1, m_firstFree - i);
		}
		else
		{
		  m_map[i] = DTM.NULL;
		}
	  }

	  /// <summary>
	  /// Sets the component at the specified index of this vector to be the
	  /// specified object. The previous component at that position is discarded.
	  /// 
	  /// The index must be a value greater than or equal to 0 and less
	  /// than the current size of the vector.
	  /// </summary>
	  /// <param name="node"> Node to set </param>
	  /// <param name="index"> Index of where to set the node </param>
	  public virtual void setElementAt(int node, int index)
	  {

		if (null == m_map)
		{
		  m_map = new int[m_blocksize];
		  m_mapSize = m_blocksize;
		}

		if (index == -1)
		{
			addElement(node);
		}

		m_map[index] = node;
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> Index of node to get
	  /// </param>
	  /// <returns> Node at specified index </returns>
	  public virtual int elementAt(int i)
	  {

		if (null == m_map)
		{
		  return DTM.NULL;
		}

		return m_map[i];
	  }

	  /// <summary>
	  /// Tell if the table contains the given node.
	  /// </summary>
	  /// <param name="s"> Node to look for
	  /// </param>
	  /// <returns> True if the given node was found. </returns>
	  public virtual bool contains(int s)
	  {

		if (null == m_map)
		{
		  return false;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  int node = m_map[i];

		  if (node == s)
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
	  /// <param name="elem"> Node to look for </param>
	  /// <param name="index"> Index of where to start the search </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public virtual int indexOf(int elem, int index)
	  {

		if (null == m_map)
		{
		  return -1;
		}

		for (int i = index; i < m_firstFree; i++)
		{
		  int node = m_map[i];

		  if (node == elem)
		  {
			return i;
		  }
		}

		return -1;
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> Node to look for </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public virtual int indexOf(int elem)
	  {

		if (null == m_map)
		{
		  return -1;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  int node = m_map[i];

		  if (node == elem)
		  {
			return i;
		  }
		}

		return -1;
	  }

	  /// <summary>
	  /// Sort an array using a quicksort algorithm.
	  /// </summary>
	  /// <param name="a"> The array to be sorted. </param>
	  /// <param name="lo0">  The low index. </param>
	  /// <param name="hi0">  The high index.
	  /// </param>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void sort(int a[], int lo0, int hi0) throws Exception
	  public virtual void sort(int[] a, int lo0, int hi0)
	  {

		int lo = lo0;
		int hi = hi0;

		// pause(lo, hi);
		if (lo >= hi)
		{
		  return;
		}
		else if (lo == hi - 1)
		{

		  /*
		   *  sort a two element list by swapping if necessary
		   */
		  if (a[lo] > a[hi])
		  {
			int T = a[lo];

			a[lo] = a[hi];
			a[hi] = T;
		  }

		  return;
		}

		/*
		 *  Pick a pivot and move it out of the way
		 */
		int mid = (int)((uint)(lo + hi) >> 1);
		int pivot = a[mid];

		a[mid] = a[hi];
		a[hi] = pivot;

		while (lo < hi)
		{

		  /*
		   *  Search forward from a[lo] until an element is found that
		   *  is greater than the pivot or lo >= hi
		   */
		  while (a[lo] <= pivot && lo < hi)
		  {
			lo++;
		  }

		  /*
		   *  Search backward from a[hi] until element is found that
		   *  is less than the pivot, or lo >= hi
		   */
		  while (pivot <= a[hi] && lo < hi)
		  {
			hi--;
		  }

		  /*
		   *  Swap elements a[lo] and a[hi]
		   */
		  if (lo < hi)
		  {
			int T = a[lo];

			a[lo] = a[hi];
			a[hi] = T;

			// pause();
		  }

		  // if (stopRequested) {
		  //    return;
		  // }
		}

		/*
		 *  Put the median in the "center" of the list
		 */
		a[hi0] = a[hi];
		a[hi] = pivot;

		/*
		 *  Recursive calls, elements a[lo0] to a[lo-1] are less than or
		 *  equal to pivot, elements a[hi+1] to a[hi0] are greater than
		 *  pivot.
		 */
		sort(a, lo0, lo - 1);
		sort(a, hi + 1, hi0);
	  }

	  /// <summary>
	  /// Sort an array using a quicksort algorithm.
	  /// </summary>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void sort() throws Exception
	  public virtual void sort()
	  {
		sort(m_map, 0, m_firstFree - 1);
	  }
	}

}