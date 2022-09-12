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
 * $Id: StringToStringTableVector.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple table that stores a list of StringToStringTables, optimized
	/// for small lists.
	/// @xsl.usage internal
	/// </summary>
	public class StringToStringTableVector
	{

	  /// <summary>
	  /// Size of blocks to allocate </summary>
	  private int m_blocksize;

	  /// <summary>
	  /// Array of StringToStringTable objects </summary>
	  private StringToStringTable[] m_map;

	  /// <summary>
	  /// Number of StringToStringTable objects in this array </summary>
	  private int m_firstFree = 0;

	  /// <summary>
	  /// Size of this array </summary>
	  private int m_mapSize;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public StringToStringTableVector()
	  {

		m_blocksize = 8;
		m_mapSize = m_blocksize;
		m_map = new StringToStringTable[m_blocksize];
	  }

	  /// <summary>
	  /// Construct a StringToStringTableVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of blocks to allocate  </param>
	  public StringToStringTableVector(int blocksize)
	  {

		m_blocksize = blocksize;
		m_mapSize = blocksize;
		m_map = new StringToStringTable[blocksize];
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of StringToStringTable objects in the list </returns>
	  public int Length
	  {
		  get
		  {
			return m_firstFree;
		  }
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of StringToStringTable objects in the list </returns>
	  public int size()
	  {
		return m_firstFree;
	  }

	  /// <summary>
	  /// Append a StringToStringTable object onto the vector.
	  /// </summary>
	  /// <param name="value"> StringToStringTable object to add </param>
	  public void addElement(StringToStringTable value)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  StringToStringTable[] newMap = new StringToStringTable[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Given a string, find the last added occurance value
	  /// that matches the key.
	  /// </summary>
	  /// <param name="key"> String to look up
	  /// </param>
	  /// <returns> the last added occurance value that matches the key
	  /// or null if not found. </returns>
	  public string get(string key)
	  {

		for (int i = m_firstFree - 1; i >= 0; --i)
		{
		  string nsuri = m_map[i].get(key);

		  if (!string.ReferenceEquals(nsuri, null))
		  {
			return nsuri;
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Given a string, find out if there is a value in this table
	  /// that matches the key.
	  /// </summary>
	  /// <param name="key"> String to look for  
	  /// </param>
	  /// <returns> True if the string was found in table, null if not </returns>
	  public bool containsKey(string key)
	  {

		for (int i = m_firstFree - 1; i >= 0; --i)
		{
		  if (!string.ReferenceEquals(m_map[i].get(key), null))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Remove the last element.
	  /// </summary>
	  public void removeLastElem()
	  {

		if (m_firstFree > 0)
		{
		  m_map[m_firstFree] = null;

		  m_firstFree--;
		}
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> Index of element to find
	  /// </param>
	  /// <returns> The StringToStringTable object at the given index </returns>
	  public StringToStringTable elementAt(int i)
	  {
		return m_map[i];
	  }

	  /// <summary>
	  /// Tell if the table contains the given StringToStringTable.
	  /// </summary>
	  /// <param name="s"> The StringToStringTable to find
	  /// </param>
	  /// <returns> True if the StringToStringTable is found </returns>
	  public bool contains(StringToStringTable s)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(s))
		  {
			return true;
		  }
		}

		return false;
	  }
	}

}