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
 * $Id: StringVector.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple table that stores a list of strings, optimized
	/// for small lists.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class StringVector
	{
		internal const long serialVersionUID = 4995234972032919748L;

	  /// <summary>
	  /// @serial Size of blocks to allocate </summary>
	  protected internal int m_blocksize;

	  /// <summary>
	  /// @serial Array of strings this contains </summary>
	  protected internal string[] m_map;

	  /// <summary>
	  /// @serial Number of strings this contains </summary>
	  protected internal int m_firstFree = 0;

	  /// <summary>
	  /// @serial Size of the array </summary>
	  protected internal int m_mapSize;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public StringVector()
	  {

		m_blocksize = 8;
		m_mapSize = m_blocksize;
		m_map = new string[m_blocksize];
	  }

	  /// <summary>
	  /// Construct a StringVector, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of the blocks to allocate  </param>
	  public StringVector(int blocksize)
	  {

		m_blocksize = blocksize;
		m_mapSize = blocksize;
		m_map = new string[blocksize];
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of strings in the list  </returns>
	  public virtual int Length
	  {
		  get
		  {
			return m_firstFree;
		  }
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of strings in the list </returns>
	  public int size()
	  {
		return m_firstFree;
	  }

	  /// <summary>
	  /// Append a string onto the vector.
	  /// </summary>
	  /// <param name="value"> Sting to add to the vector </param>
	  public void addElement(string value)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  string[] newMap = new string[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> Index of string to find
	  /// </param>
	  /// <returns> String at given index </returns>
	  public string elementAt(int i)
	  {
		return m_map[i];
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="s"> String to look for
	  /// </param>
	  /// <returns> True if the string is in this table   </returns>
	  public bool contains(string s)
	  {

		if (null == s)
		{
		  return false;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(s))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string. Ignore case.
	  /// </summary>
	  /// <param name="s"> String to find
	  /// </param>
	  /// <returns> True if the String is in this vector </returns>
	  public bool containsIgnoreCase(string s)
	  {

		if (null == s)
		{
		  return false;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(s, StringComparison.CurrentCultureIgnoreCase))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="s"> String to push into the vector </param>
	  public void push(string s)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  string[] newMap = new string[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = s;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Pop the tail of this vector.
	  /// </summary>
	  /// <returns> The String last added to this vector or null not found.
	  /// The string is removed from the vector. </returns>
	  public string pop()
	  {

		if (m_firstFree <= 0)
		{
		  return null;
		}

		m_firstFree--;

		string s = m_map[m_firstFree];

		m_map[m_firstFree] = null;

		return s;
	  }

	  /// <summary>
	  /// Get the string at the tail of this vector without popping.
	  /// </summary>
	  /// <returns> The string at the tail of this vector. </returns>
	  public string peek()
	  {
		return (m_firstFree <= 0) ? null : m_map[m_firstFree - 1];
	  }
	}

}