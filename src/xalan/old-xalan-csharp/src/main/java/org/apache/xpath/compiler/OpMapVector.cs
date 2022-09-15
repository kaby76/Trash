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
 * $Id: OpMapVector.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xpath.compiler
{

	/// 
	/// <summary>
	/// Like IntVector, but used only for the OpMap array.  Length of array
	/// is kept in the m_lengthPos position of the array.  Only the required methods 
	/// are in included here.
	/// @xsl.usage internal
	/// </summary>
	public class OpMapVector
	{

	 /// <summary>
	 /// Size of blocks to allocate </summary>
	  protected internal int m_blocksize;

	  /// <summary>
	  /// Array of ints </summary>
	  protected internal int[] m_map; // IntStack is trying to see this directly

	  /// <summary>
	  /// Position where size of array is kept </summary>
	  protected internal int m_lengthPos = 0;

	  /// <summary>
	  /// Size of array </summary>
	  protected internal int m_mapSize;

		/// <summary>
		/// Construct a OpMapVector, using the given block size.
		/// </summary>
		/// <param name="blocksize"> Size of block to allocate </param>
	  public OpMapVector(int blocksize, int increaseSize, int lengthPos)
	  {

		m_blocksize = increaseSize;
		m_mapSize = blocksize;
		m_lengthPos = lengthPos;
		m_map = new int[blocksize];
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
		if (index >= m_mapSize)
		{
		  int oldSize = m_mapSize;

		  m_mapSize += m_blocksize;

		  int[] newMap = new int[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, oldSize);

		  m_map = newMap;
		}

		m_map[index] = value;
	  }


	  /*
	   * Reset the array to the supplied size.  No checking is done.
	   * 
	   * @param size The size to trim to.
	   */
	  public int ToSize
	  {
		  set
		  {
    
			int[] newMap = new int[value];
    
			Array.Copy(m_map, 0, newMap, 0, m_map[m_lengthPos]);
    
			m_mapSize = value;
			m_map = newMap;
    
		  }
	  }

	}

}