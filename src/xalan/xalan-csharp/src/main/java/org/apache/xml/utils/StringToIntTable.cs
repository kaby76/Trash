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
 * $Id: StringToIntTable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple lookup table that stores a list of strings, the even
	/// number strings being keys, and the odd number strings being values.
	/// @xsl.usage internal
	/// </summary>
	public class StringToIntTable
	{

	  public const int INVALID_KEY = -10000;

	  /// <summary>
	  /// Block size to allocate </summary>
	  private int m_blocksize;

	  /// <summary>
	  /// Array of strings this table points to. Associated with ints
	  /// in m_values         
	  /// </summary>
	  private string[] m_map;

	  /// <summary>
	  /// Array of ints this table points. Associated with strings from
	  /// m_map.         
	  /// </summary>
	  private int[] m_values;

	  /// <summary>
	  /// Number of ints in the table </summary>
	  private int m_firstFree = 0;

	  /// <summary>
	  /// Size of this table </summary>
	  private int m_mapSize;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public StringToIntTable()
	  {

		m_blocksize = 8;
		m_mapSize = m_blocksize;
		m_map = new string[m_blocksize];
		m_values = new int[m_blocksize];
	  }

	  /// <summary>
	  /// Construct a StringToIntTable, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of block to allocate </param>
	  public StringToIntTable(int blocksize)
	  {

		m_blocksize = blocksize;
		m_mapSize = blocksize;
		m_map = new string[blocksize];
		m_values = new int[m_blocksize];
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> the length of the list  </returns>
	  public int Length
	  {
		  get
		  {
			return m_firstFree;
		  }
	  }

	  /// <summary>
	  /// Append a string onto the vector.
	  /// </summary>
	  /// <param name="key"> String to append </param>
	  /// <param name="value"> The int value of the string </param>
	  public void put(string key, int value)
	  {

		if ((m_firstFree + 1) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  string[] newMap = new string[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;

		  int[] newValues = new int[m_mapSize];

		  Array.Copy(m_values, 0, newValues, 0, m_firstFree + 1);

		  m_values = newValues;
		}

		m_map[m_firstFree] = key;
		m_values[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="key"> String to look for
	  /// </param>
	  /// <returns> The String's int value
	  ///  </returns>
	  public int get(string key)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(key))
		  {
			return m_values[i];
		  }
		}

		return INVALID_KEY;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string. Ignore case.
	  /// </summary>
	  /// <param name="key"> String to look for
	  /// </param>
	  /// <returns> The string's int value </returns>
	  public int getIgnoreCase(string key)
	  {

		if (null == key)
		{
			return INVALID_KEY;
		}

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(key, StringComparison.CurrentCultureIgnoreCase))
		  {
			return m_values[i];
		  }
		}

		return INVALID_KEY;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="key"> String to look for
	  /// </param>
	  /// <returns> True if the string is in the table </returns>
	  public bool contains(string key)
	  {

		for (int i = 0; i < m_firstFree; i++)
		{
		  if (m_map[i].Equals(key))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Return array of keys in the table.
	  /// </summary>
	  /// <returns> Array of strings </returns>
	  public string[] keys()
	  {
		string[] keysArr = new string[m_firstFree];

		for (int i = 0; i < m_firstFree; i++)
		{
		  keysArr[i] = m_map[i];
		}

		return keysArr;
	  }
	}

}