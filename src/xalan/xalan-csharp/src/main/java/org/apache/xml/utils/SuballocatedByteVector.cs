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
 * $Id: SuballocatedByteVector.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple table that stores a list of byte. Very similar API to our
	/// IntVector class (same API); different internal storage.
	/// 
	/// This version uses an array-of-arrays solution. Read/write access is thus
	/// a bit slower than the simple IntVector, and basic storage is a trifle
	/// higher due to the top-level array -- but appending is O(1) fast rather
	/// than O(N**2) slow, which will swamp those costs in situations where
	/// long vectors are being built up.
	/// 
	/// Known issues:
	/// 
	/// Some methods are private because they haven't yet been tested properly.
	/// 
	/// If an element has not been set (because we skipped it), its value will
	/// initially be 0. Shortening the vector does not clear old storage; if you
	/// then skip values and setElementAt a higher index again, you may see old data
	/// reappear in the truncated-and-restored section. Doing anything else would
	/// have performance costs.
	/// @xsl.usage internal
	/// </summary>
	public class SuballocatedByteVector
	{
	  /// <summary>
	  /// Size of blocks to allocate </summary>
	  protected internal int m_blocksize;

	  /// <summary>
	  /// Number of blocks to (over)allocate by </summary>
	  protected internal int m_numblocks = 32;

	  /// <summary>
	  /// Array of arrays of bytes </summary>
	  protected internal sbyte[][] m_map;

	  /// <summary>
	  /// Number of bytes in array </summary>
	  protected internal int m_firstFree = 0;

	  /// <summary>
	  /// "Shortcut" handle to m_map[0] </summary>
	  protected internal sbyte[] m_map0;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public SuballocatedByteVector() : this(2048)
	  {
	  }

	  /// <summary>
	  /// Construct a ByteVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public SuballocatedByteVector(int blocksize)
	  {
		m_blocksize = blocksize;
		m_map0 = new sbyte[blocksize];
		m_map = new sbyte[m_numblocks][];
		m_map[0] = m_map0;
	  }

	  /// <summary>
	  /// Construct a ByteVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public SuballocatedByteVector(int blocksize, int increaseSize) : this(blocksize)
	  {
		// increaseSize not currently used.
	  }


	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> length of the list </returns>
	  public virtual int size()
	  {
		return m_firstFree;
	  }

	  /// <summary>
	  /// Set the length of the list.
	  /// </summary>
	  /// <returns> length of the list </returns>
	  private int Size
	  {
		  set
		  {
			if (m_firstFree < value)
			{
			  m_firstFree = value;
			}
		  }
	  }

	  /// <summary>
	  /// Append a byte onto the vector.
	  /// </summary>
	  /// <param name="value"> Byte to add to the list  </param>
	  public virtual void addElement(sbyte value)
	  {
		if (m_firstFree < m_blocksize)
		{
		  m_map0[m_firstFree++] = value;
		}
		else
		{
		  int index = m_firstFree / m_blocksize;
		  int offset = m_firstFree % m_blocksize;
		  ++m_firstFree;

		  if (index >= m_map.Length)
		  {
			int newsize = index + m_numblocks;
			sbyte[][] newMap = new sbyte[newsize][];
			Array.Copy(m_map, 0, newMap, 0, m_map.Length);
			m_map = newMap;
		  }
		  sbyte[] block = m_map[index];
		  if (null == block)
		  {
			block = m_map[index] = new sbyte[m_blocksize];
		  }
		  block[offset] = value;
		}
	  }

	  /// <summary>
	  /// Append several byte values onto the vector.
	  /// </summary>
	  /// <param name="value"> Byte to add to the list  </param>
	  private void addElements(sbyte value, int numberOfElements)
	  {
		if (m_firstFree + numberOfElements < m_blocksize)
		{
		  for (int i = 0; i < numberOfElements; i++)
		  {
			m_map0[m_firstFree++] = value;
		  }
		}
		else
		{
		  int index = m_firstFree / m_blocksize;
		  int offset = m_firstFree % m_blocksize;
		  m_firstFree += numberOfElements;
		  while (numberOfElements > 0)
		  {
			if (index >= m_map.Length)
			{
			  int newsize = index + m_numblocks;
			  sbyte[][] newMap = new sbyte[newsize][];
			  Array.Copy(m_map, 0, newMap, 0, m_map.Length);
			  m_map = newMap;
			}
			sbyte[] block = m_map[index];
			if (null == block)
			{
			  block = m_map[index] = new sbyte[m_blocksize];
			}
			int copied = (m_blocksize - offset < numberOfElements) ? m_blocksize - offset : numberOfElements;
			numberOfElements -= copied;
			while (copied-- > 0)
			{
			  block[offset++] = value;
			}

			++index;
			offset = 0;
		  }
		}
	  }

	  /// <summary>
	  /// Append several slots onto the vector, but do not set the values.
	  /// Note: "Not Set" means the value is unspecified.
	  /// </summary>
	  /// <param name="numberOfElements"> </param>
	  private void addElements(int numberOfElements)
	  {
		int newlen = m_firstFree + numberOfElements;
		if (newlen > m_blocksize)
		{
		  int index = m_firstFree % m_blocksize;
		  int newindex = (m_firstFree + numberOfElements) % m_blocksize;
		  for (int i = index + 1;i <= newindex;++i)
		  {
			m_map[i] = new sbyte[m_blocksize];
		  }
		}
		m_firstFree = newlen;
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// 
	  /// Insertion may be an EXPENSIVE operation!
	  /// </summary>
	  /// <param name="value"> Byte to insert </param>
	  /// <param name="at"> Index of where to insert  </param>
	  private void insertElementAt(sbyte value, int at)
	  {
		if (at == m_firstFree)
		{
		  addElement(value);
		}
		else if (at > m_firstFree)
		{
		  int index = at / m_blocksize;
		  if (index >= m_map.Length)
		  {
			int newsize = index + m_numblocks;
			sbyte[][] newMap = new sbyte[newsize][];
			Array.Copy(m_map, 0, newMap, 0, m_map.Length);
			m_map = newMap;
		  }
		  sbyte[] block = m_map[index];
		  if (null == block)
		  {
			block = m_map[index] = new sbyte[m_blocksize];
		  }
		  int offset = at % m_blocksize;
		  block[offset] = value;
		  m_firstFree = offset + 1;
		}
		else
		{
		  int index = at / m_blocksize;
		  int maxindex = m_firstFree+1 / m_blocksize;
		  ++m_firstFree;
		  int offset = at % m_blocksize;
		  sbyte push;

		  // ***** Easier to work down from top?
		  while (index <= maxindex)
		  {
			int copylen = m_blocksize - offset - 1;
			sbyte[] block = m_map[index];
			if (null == block)
			{
			  push = 0;
			  block = m_map[index] = new sbyte[m_blocksize];
			}
			else
			{
			  push = block[m_blocksize-1];
			  Array.Copy(block, offset, block, offset + 1, copylen);
			}
			block[offset] = value;
			value = push;
			offset = 0;
			++index;
		  }
		}
	  }

	  /// <summary>
	  /// Wipe it out. 
	  /// </summary>
	  public virtual void removeAllElements()
	  {
		m_firstFree = 0;
	  }

	  /// <summary>
	  /// Removes the first occurrence of the argument from this vector.
	  /// If the object is found in this vector, each component in the vector
	  /// with an index greater or equal to the object's index is shifted
	  /// downward to have an index one smaller than the value it had
	  /// previously.
	  /// </summary>
	  /// <param name="s"> Byte to remove from array
	  /// </param>
	  /// <returns> True if the byte was removed, false if it was not found </returns>
	  private bool removeElement(sbyte s)
	  {
		int at = indexOf(s,0);
		if (at < 0)
		{
		  return false;
		}
		removeElementAt(at);
		return true;
	  }

	  /// <summary>
	  /// Deletes the component at the specified index. Each component in
	  /// this vector with an index greater or equal to the specified
	  /// index is shifted downward to have an index one smaller than
	  /// the value it had previously.
	  /// </summary>
	  /// <param name="at"> index of where to remove a byte </param>
	  private void removeElementAt(int at)
	  {
		// No point in removing elements that "don't exist"...  
		if (at < m_firstFree)
		{
		  int index = at / m_blocksize;
		  int maxindex = m_firstFree / m_blocksize;
		  int offset = at % m_blocksize;

		  while (index <= maxindex)
		  {
			int copylen = m_blocksize - offset - 1;
			sbyte[] block = m_map[index];
			if (null == block)
			{
			  block = m_map[index] = new sbyte[m_blocksize];
			}
			else
			{
			  Array.Copy(block, offset + 1, block, offset, copylen);
			}
			if (index < maxindex)
			{
			  sbyte[] next = m_map[index + 1];
			  if (next != null)
			  {
				block[m_blocksize-1] = (next != null) ? next[0] : 0;
			  }
			}
			else
			{
			  block[m_blocksize-1] = 0;
			}
			offset = 0;
			++index;
		  }
		}
		--m_firstFree;
	  }

	  /// <summary>
	  /// Sets the component at the specified index of this vector to be the
	  /// specified object. The previous component at that position is discarded.
	  /// 
	  /// The index must be a value greater than or equal to 0 and less
	  /// than the current size of the vector.
	  /// </summary>
	  /// <param name="value"> </param>
	  /// <param name="at">     Index of where to set the object </param>
	  public virtual void setElementAt(sbyte value, int at)
	  {
		if (at < m_blocksize)
		{
		  m_map0[at] = value;
		  return;
		}

		int index = at / m_blocksize;
		int offset = at % m_blocksize;

		if (index >= m_map.Length)
		{
		  int newsize = index + m_numblocks;
		  sbyte[][] newMap = new sbyte[newsize][];
		  Array.Copy(m_map, 0, newMap, 0, m_map.Length);
		  m_map = newMap;
		}

		sbyte[] block = m_map[index];
		if (null == block)
		{
		  block = m_map[index] = new sbyte[m_blocksize];
		}
		block[offset] = value;

		if (at >= m_firstFree)
		{
		  m_firstFree = at + 1;
		}
	  }

	  /// <summary>
	  /// Get the nth element. This is often at the innermost loop of an
	  /// application, so performance is critical.
	  /// </summary>
	  /// <param name="i"> index of value to get
	  /// </param>
	  /// <returns> value at given index. If that value wasn't previously set,
	  /// the result is undefined for performance reasons. It may throw an
	  /// exception (see below), may return zero, or (if setSize has previously
	  /// been used) may return stale data.
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> if the index was _clearly_
	  /// unreasonable (negative, or past the highest block).
	  /// </exception>
	  /// <exception cref="NullPointerException"> if the index points to a block that could
	  /// have existed (based on the highest index used) but has never had anything
	  /// set into it.
	  /// %REVIEW% Could add a catch to create the block in that case, or return 0.
	  /// Try/Catch is _supposed_ to be nearly free when not thrown to. Do we
	  /// believe that? Should we have a separate safeElementAt? </exception>
	  public virtual sbyte elementAt(int i)
	  {
		// %OPT% Does this really buy us anything? Test versus division for small,
		// test _plus_ division for big docs.
		if (i < m_blocksize)
		{
		  return m_map0[i];
		}

		return m_map[i / m_blocksize][i % m_blocksize];
	  }

	  /// <summary>
	  /// Tell if the table contains the given node.
	  /// </summary>
	  /// <param name="s"> object to look for
	  /// </param>
	  /// <returns> true if the object is in the list </returns>
	  private bool contains(sbyte s)
	  {
		return (indexOf(s,0) >= 0);
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
	  public virtual int indexOf(sbyte elem, int index)
	  {
		if (index >= m_firstFree)
		{
		  return -1;
		}

		int bindex = index / m_blocksize;
		int boffset = index % m_blocksize;
		int maxindex = m_firstFree / m_blocksize;
		sbyte[] block;

		for (;bindex < maxindex;++bindex)
		{
		  block = m_map[bindex];
		  if (block != null)
		  {
			for (int offset = boffset;offset < m_blocksize;++offset)
			{
			  if (block[offset] == elem)
			  {
				return offset + bindex * m_blocksize;
			  }
			}
		  }
		  boffset = 0; // after first
		}
		// Last block may need to stop before end
		int maxoffset = m_firstFree % m_blocksize;
		block = m_map[maxindex];
		for (int offset = boffset;offset < maxoffset;++offset)
		{
		  if (block[offset] == elem)
		  {
			return offset + maxindex * m_blocksize;
		  }
		}

		return -1;
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
	  public virtual int indexOf(sbyte elem)
	  {
		return indexOf(elem,0);
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
	  private int lastIndexOf(sbyte elem)
	  {
		int boffset = m_firstFree % m_blocksize;
		for (int index = m_firstFree / m_blocksize; index >= 0; --index)
		{
		  sbyte[] block = m_map[index];
		  if (block != null)
		  {
			for (int offset = boffset; offset >= 0; --offset)
			{
			  if (block[offset] == elem)
			  {
				return offset + index * m_blocksize;
			  }
			}
		  }
		  boffset = 0; // after first
		}
		return -1;
	  }

	}

}