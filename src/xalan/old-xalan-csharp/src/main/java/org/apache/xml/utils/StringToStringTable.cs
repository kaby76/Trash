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
 * $Id: StringToStringTable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A very simple lookup table that stores a list of strings, the even
	/// number strings being keys, and the odd number strings being values.
	/// @xsl.usage internal
	/// </summary>
	public class StringToStringTable
	{

	  /// <summary>
	  /// Size of blocks to allocate </summary>
	  private int m_blocksize;

	  /// <summary>
	  /// Array of strings this contains </summary>
	  private string[] m_map;

	  /// <summary>
	  /// Number of strings this contains </summary>
	  private int m_firstFree = 0;

	  /// <summary>
	  /// Size of this table </summary>
	  private int m_mapSize;

	  /// <summary>
	  /// Default constructor.  Note that the default
	  /// block size is very small, for small lists.
	  /// </summary>
	  public StringToStringTable()
	  {

		m_blocksize = 16;
		m_mapSize = m_blocksize;
		m_map = new string[m_blocksize];
	  }

	  /// <summary>
	  /// Construct a StringToStringTable, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of blocks to allocate  </param>
	  public StringToStringTable(int blocksize)
	  {

		m_blocksize = blocksize;
		m_mapSize = blocksize;
		m_map = new string[blocksize];
	  }

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> Number of strings in the list </returns>
	  public int Length
	  {
		  get
		  {
			return m_firstFree;
		  }
	  }

	  /// <summary>
	  /// Append a string onto the vector.
	  /// The strings go to the even locations in the array 
	  /// and the values in the odd. 
	  /// </summary>
	  /// <param name="key"> String to add to the list </param>
	  /// <param name="value"> Value of the string </param>
	  public void put(string key, string value)
	  {

		if ((m_firstFree + 2) >= m_mapSize)
		{
		  m_mapSize += m_blocksize;

		  string[] newMap = new string[m_mapSize];

		  Array.Copy(m_map, 0, newMap, 0, m_firstFree + 1);

		  m_map = newMap;
		}

		m_map[m_firstFree] = key;

		m_firstFree++;

		m_map[m_firstFree] = value;

		m_firstFree++;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="key"> String to look up
	  /// </param>
	  /// <returns> return the value of the string or null if not found.  </returns>
	  public string get(string key)
	  {

		for (int i = 0; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(key))
		  {
			return m_map[i + 1];
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Remove the given string and its value from this table.
	  /// </summary>
	  /// <param name="key"> String to remove from the table </param>
	  public void remove(string key)
	  {

		for (int i = 0; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(key))
		  {
			if ((i + 2) < m_firstFree)
			{
			  Array.Copy(m_map, i + 2, m_map, i, m_firstFree - (i + 2));
			}

			m_firstFree -= 2;
			m_map[m_firstFree] = null;
			m_map[m_firstFree + 1] = null;

			break;
		  }
		}
	  }

	  /// <summary>
	  /// Tell if the table contains the given string. Ignore case
	  /// </summary>
	  /// <param name="key"> String to look up
	  /// </param>
	  /// <returns> The value of the string or null if not found </returns>
	  public string getIgnoreCase(string key)
	  {

		if (null == key)
		{
		  return null;
		}

		for (int i = 0; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(key, StringComparison.CurrentCultureIgnoreCase))
		  {
			return m_map[i + 1];
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string in the value.
	  /// </summary>
	  /// <param name="val"> Value of the string to look up
	  /// </param>
	  /// <returns> the string associated with the given value or null if not found </returns>
	  public string getByValue(string val)
	  {

		for (int i = 1; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(val))
		  {
			return m_map[i - 1];
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> index of the string to look up.
	  /// </param>
	  /// <returns> The string at the given index. </returns>
	  public string elementAt(int i)
	  {
		return m_map[i];
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="key"> String to look up
	  /// </param>
	  /// <returns> True if the given string is in this table  </returns>
	  public bool contains(string key)
	  {

		for (int i = 0; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(key))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if the table contains the given string.
	  /// </summary>
	  /// <param name="val"> value to look up
	  /// </param>
	  /// <returns> True if the given value is in the table. </returns>
	  public bool containsValue(string val)
	  {

		for (int i = 1; i < m_firstFree; i += 2)
		{
		  if (m_map[i].Equals(val))
		  {
			return true;
		  }
		}

		return false;
	  }
	}

}