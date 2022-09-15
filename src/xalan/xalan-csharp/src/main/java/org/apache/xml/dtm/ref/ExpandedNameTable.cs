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
 * $Id: ExpandedNameTable.java 1225372 2011-12-28 22:58:27Z mrglavas $
 */
namespace org.apache.xml.dtm.@ref
{
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// This is a default implementation of a table that manages mappings from
	/// expanded names to expandedNameIDs.
	/// 
	/// %OPT% The performance of the getExpandedTypeID() method is very important 
	/// to DTM building. To get the best performance out of this class, we implement
	/// a simple hash algorithm directly into this class, instead of using the
	/// inefficient java.util.Hashtable. The code for the get and put operations
	/// are combined in getExpandedTypeID() method to share the same hash calculation
	/// code. We only need to implement the rehash() interface which is used to
	/// expand the hash table.
	/// </summary>
	public class ExpandedNameTable
	{

	  /// <summary>
	  /// Array of extended types for this document </summary>
	  private ExtendedType[] m_extendedTypes;

	  /// <summary>
	  /// The initial size of the m_extendedTypes array </summary>
	  private static int m_initialSize = 128;

	  /// <summary>
	  /// Next available extended type </summary>
	  // %REVIEW% Since this is (should be) always equal 
	  // to the length of m_extendedTypes, do we need this? 
	  private int m_nextType;

	  // These are all the types prerotated, for caller convenience.
	  public static readonly int ELEMENT = ((int)DTM.ELEMENT_NODE);
	  public static readonly int ATTRIBUTE = ((int)DTM.ATTRIBUTE_NODE);
	  public static readonly int TEXT = ((int)DTM.TEXT_NODE);
	  public static readonly int CDATA_SECTION = ((int)DTM.CDATA_SECTION_NODE);
	  public static readonly int ENTITY_REFERENCE = ((int)DTM.ENTITY_REFERENCE_NODE);
	  public static readonly int ENTITY = ((int)DTM.ENTITY_NODE);
	  public static readonly int PROCESSING_INSTRUCTION = ((int)DTM.PROCESSING_INSTRUCTION_NODE);
	  public static readonly int COMMENT = ((int)DTM.COMMENT_NODE);
	  public static readonly int DOCUMENT = ((int)DTM.DOCUMENT_NODE);
	  public static readonly int DOCUMENT_TYPE = ((int)DTM.DOCUMENT_TYPE_NODE);
	  public static readonly int DOCUMENT_FRAGMENT = ((int)DTM.DOCUMENT_FRAGMENT_NODE);
	  public static readonly int NOTATION = ((int)DTM.NOTATION_NODE);
	  public static readonly int NAMESPACE = ((int)DTM.NAMESPACE_NODE);

	  /// <summary>
	  /// Workspace for lookup. NOT THREAD SAFE!
	  /// 
	  /// </summary>
	  internal ExtendedType hashET = new ExtendedType(-1, "", "");

	  /// <summary>
	  /// The array to store the default extended types. </summary>
	  private static ExtendedType[] m_defaultExtendedTypes;

	  /// <summary>
	  /// The default load factor of the Hashtable.
	  /// This is used to calcualte the threshold.
	  /// </summary>
	  private static float m_loadFactor = 0.75f;

	  /// <summary>
	  /// The initial capacity of the hash table. Use a bigger number
	  /// to avoid the cost of expanding the table.
	  /// </summary>
	  private static int m_initialCapacity = 203;

	  /// <summary>
	  /// The capacity of the hash table, i.e. the size of the
	  /// internal HashEntry array.
	  /// </summary>
	  private int m_capacity;

	  /// <summary>
	  /// The threshold of the hash table, which is equal to capacity * loadFactor.
	  /// If the number of entries in the hash table is bigger than the threshold,
	  /// the hash table needs to be expanded.
	  /// </summary>
	  private int m_threshold;

	  /// <summary>
	  /// The internal array to store the hash entries.
	  /// Each array member is a slot for a hash bucket.
	  /// </summary>
	  private HashEntry[] m_table;

	  /// <summary>
	  /// Init default values
	  /// </summary>
	  static ExpandedNameTable()
	  {
		m_defaultExtendedTypes = new ExtendedType[DTM.NTYPES];

		for (int i = 0; i < DTM.NTYPES; i++)
		{
		  m_defaultExtendedTypes[i] = new ExtendedType(i, "", "");
		}
	  }

	  /// <summary>
	  /// Create an expanded name table.
	  /// </summary>
	  public ExpandedNameTable()
	  {
		m_capacity = m_initialCapacity;
		m_threshold = (int)(m_capacity * m_loadFactor);
		m_table = new HashEntry[m_capacity];

		initExtendedTypes();
	  }


	  /// <summary>
	  ///  Initialize the vector of extended types with the
	  ///  basic DOM node types.
	  /// </summary>
	  private void initExtendedTypes()
	  {
		m_extendedTypes = new ExtendedType[m_initialSize];
		for (int i = 0; i < DTM.NTYPES; i++)
		{
			m_extendedTypes[i] = m_defaultExtendedTypes[i];
			m_table[i] = new HashEntry(m_defaultExtendedTypes[i], i, i, null);
		}

		m_nextType = DTM.NTYPES;
	  }

	  /// <summary>
	  /// Given an expanded name represented by namespace, local name and node type,
	  /// return an ID.  If the expanded-name does not exist in the internal tables,
	  /// the entry will be created, and the ID will be returned.  Any additional 
	  /// nodes that are created that have this expanded name will use this ID.
	  /// </summary>
	  /// <param name="namespace"> The namespace </param>
	  /// <param name="localName"> The local name </param>
	  /// <param name="type"> The node type
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  public virtual int getExpandedTypeID(string @namespace, string localName, int type)
	  {
		return getExpandedTypeID(@namespace, localName, type, false);
	  }

	  /// <summary>
	  /// Given an expanded name represented by namespace, local name and node type,
	  /// return an ID.  If the expanded-name does not exist in the internal tables,
	  /// the entry will be created, and the ID will be returned.  Any additional 
	  /// nodes that are created that have this expanded name will use this ID.
	  /// <para>
	  /// If searchOnly is true, we will return -1 if the name is not found in the 
	  /// table, otherwise the name is added to the table and the expanded name id
	  /// of the new entry is returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="namespace"> The namespace </param>
	  /// <param name="localName"> The local name </param>
	  /// <param name="type"> The node type </param>
	  /// <param name="searchOnly"> If it is true, we will only search for the expanded name.
	  /// -1 is return is the name is not found.
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  public virtual int getExpandedTypeID(string @namespace, string localName, int type, bool searchOnly)
	  {
		if (null == @namespace)
		{
		  @namespace = "";
		}
		if (null == localName)
		{
		  localName = "";
		}

		// Calculate the hash code
		int hash = type + @namespace.GetHashCode() + localName.GetHashCode();

		// Redefine the hashET object to represent the new expanded name.
		hashET.redefine(type, @namespace, localName, hash);

		// Calculate the index into the HashEntry table.
		int index = hash % m_capacity;
		if (index < 0)
		{
		  index = -index;
		}

		// Look up the expanded name in the hash table. Return the id if
		// the expanded name is already in the hash table.
		for (HashEntry e = m_table[index]; e != null; e = e.next)
		{
		  if (e.hash == hash && e.key.Equals(hashET))
		  {
			return e.value;
		  }
		}

		if (searchOnly)
		{
		  return DTM.NULL;
		}

		// Expand the internal HashEntry array if necessary.
		if (m_nextType > m_threshold)
		{
		  rehash();
		  index = hash % m_capacity;
		  if (index < 0)
		  {
			index = -index;
		  }
		}

		// Create a new ExtendedType object
		ExtendedType newET = new ExtendedType(type, @namespace, localName, hash);

		// Expand the m_extendedTypes array if necessary.
		if (m_extendedTypes.Length == m_nextType)
		{
			ExtendedType[] newArray = new ExtendedType[m_extendedTypes.Length * 2];
			Array.Copy(m_extendedTypes, 0, newArray, 0, m_extendedTypes.Length);
			m_extendedTypes = newArray;
		}

		m_extendedTypes[m_nextType] = newET;

		// Create a new hash entry for the new ExtendedType and put it into 
		// the table.
		HashEntry entry = new HashEntry(newET, m_nextType, hash, m_table[index]);
		m_table[index] = entry;

		return m_nextType++;
	  }

	  /// <summary>
	  /// Increases the capacity of and internally reorganizes the hashtable, 
	  /// in order to accommodate and access its entries more efficiently. 
	  /// This method is called when the number of keys in the hashtable exceeds
	  /// this hashtable's capacity and load factor.
	  /// </summary>
	  private void rehash()
	  {
		int oldCapacity = m_capacity;
		HashEntry[] oldTable = m_table;

		int newCapacity = 2 * oldCapacity + 1;
		m_capacity = newCapacity;
		m_threshold = (int)(newCapacity * m_loadFactor);

		m_table = new HashEntry[newCapacity];
		for (int i = oldCapacity - 1; i >= 0 ; i--)
		{
		  for (HashEntry old = oldTable[i]; old != null;)
		  {
			HashEntry e = old;
			old = old.next;

			int newIndex = e.hash % newCapacity;
			if (newIndex < 0)
			{
			  newIndex = -newIndex;
			}

			e.next = m_table[newIndex];
			m_table[newIndex] = e;
		  }
		}
	  }

	  /// <summary>
	  /// Given a type, return an expanded name ID.Any additional nodes that are
	  /// created that have this expanded name will use this ID.
	  /// </summary>
	  /// <returns> the expanded-name id of the node. </returns>
	  public virtual int getExpandedTypeID(int type)
	  {
		return type;
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the local name part.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String Local name of this node, or null if the node has no name. </returns>
	  public virtual string getLocalName(int ExpandedNameID)
	  {
		return m_extendedTypes[ExpandedNameID].LocalName;
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the local name ID.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> The id of this local name. </returns>
	  public int getLocalNameID(int ExpandedNameID)
	  {
		// ExtendedType etype = m_extendedTypes[ExpandedNameID];
		if (m_extendedTypes[ExpandedNameID].LocalName.Length == 0)
		{
		  return 0;
		}
		else
		{
		return ExpandedNameID;
		}
	  }


	  /// <summary>
	  /// Given an expanded-name ID, return the namespace URI part.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  public virtual string getNamespace(int ExpandedNameID)
	  {
		string @namespace = m_extendedTypes[ExpandedNameID].Namespace;
		return (@namespace.Length == 0 ? null : @namespace);
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the namespace URI ID.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> The id of this namespace. </returns>
	  public int getNamespaceID(int ExpandedNameID)
	  {
		//ExtendedType etype = m_extendedTypes[ExpandedNameID];
		if (m_extendedTypes[ExpandedNameID].Namespace.Length == 0)
		{
		  return 0;
		}
		else
		{
		return ExpandedNameID;
		}
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the local name ID.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> The id of this local name. </returns>
	  public short getType(int ExpandedNameID)
	  {
		//ExtendedType etype = m_extendedTypes[ExpandedNameID];
		return (short)m_extendedTypes[ExpandedNameID].NodeType;
	  }

	  /// <summary>
	  /// Return the size of the ExpandedNameTable
	  /// </summary>
	  /// <returns> The size of the ExpandedNameTable </returns>
	  public virtual int Size
	  {
		  get
		  {
			return m_nextType;
		  }
	  }

	  /// <summary>
	  /// Return the array of extended types
	  /// </summary>
	  /// <returns> The array of extended types </returns>
	  public virtual ExtendedType[] ExtendedTypes
	  {
		  get
		  {
			return m_extendedTypes;
		  }
	  }

	  /// <summary>
	  /// Inner class which represents a hash table entry.
	  /// The field next points to the next entry which is hashed into
	  /// the same bucket in the case of "hash collision".
	  /// </summary>
	  private sealed class HashEntry
	  {
		internal ExtendedType key;
		internal int value;
		internal int hash;
		internal HashEntry next;

		protected internal HashEntry(ExtendedType key, int value, int hash, HashEntry next)
		{
		  this.key = key;
		  this.value = value;
		  this.hash = hash;
		  this.next = next;
		}
	  }

	}

}