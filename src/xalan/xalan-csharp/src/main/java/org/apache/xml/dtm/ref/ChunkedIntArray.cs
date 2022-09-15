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
 * $Id: ChunkedIntArray.java 1225427 2011-12-29 04:33:32Z mrglavas $
 */
namespace org.apache.xml.dtm.@ref
{
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	/// <summary>
	/// <code>ChunkedIntArray</code> is an extensible array of blocks of integers.
	/// (I'd consider Vector, but it's unable to handle integers except by
	/// turning them into Objects.)
	/// 
	/// <para>Making this a separate class means some call-and-return overhead. But
	/// doing it all inline tends to be fragile and expensive in coder time,
	/// not to mention driving up code size. If you want to inline it, feel free.
	/// The Java text suggest that private and Final methods may be inlined, 
	/// and one can argue that this beast need not be made subclassable...</para>
	/// 
	/// <para>%REVIEW% This has strong conceptual overlap with the IntVector class.
	/// </para>
	/// It would probably be a good thing to merge the two, when time permits.<para>
	/// </para>
	/// </summary>
	internal sealed class ChunkedIntArray
	{
	  internal const int slotsize = 4; // Locked, MUST be power of two in current code
	  // Debugging tip: Cranking lowbits down to 4 or so is a good
	  // way to pound on the array addressing code.
	  internal const int lowbits = 10; // How many bits address within chunks
	  internal static readonly int chunkalloc = 1 << lowbits;
	  internal static readonly int lowmask = chunkalloc - 1;

	  internal ChunksVector chunks = new ChunksVector();
	  internal readonly int[] fastArray = new int[chunkalloc];
	  internal int lastUsed = 0;

	  /// <summary>
	  /// Create a new CIA with specified record size. Currently record size MUST
	  /// be a power of two... and in fact is hardcoded to 4.
	  /// </summary>
	  internal ChunkedIntArray(int slotsize)
	  {
		if (slotsize < slotsize)
		{
		  throw new System.IndexOutOfRangeException(XMLMessages.createXMLMessage(XMLErrorResources.ER_CHUNKEDINTARRAY_NOT_SUPPORTED, new object[]{Convert.ToString(slotsize)})); //"ChunkedIntArray("+slotsize+") not currently supported");
		}
		else if (slotsize > slotsize)
		{
		  Console.WriteLine("*****WARNING: ChunkedIntArray(" + slotsize + ") wasting " + (slotsize - slotsize) + " words per slot");
		}
		chunks.addElement(fastArray);
	  }
	  /// <summary>
	  /// Append a 4-integer record to the CIA, starting with record 1. (Since
	  /// arrays are initialized to all-0, 0 has been reserved as the "unknown"
	  /// value in DTM.) </summary>
	  /// <returns> the index at which this record was inserted. </returns>
	  internal int appendSlot(int w0, int w1, int w2, int w3)
	  {
		/*
		try
		{
		  int newoffset = (lastUsed+1)*slotsize;
		  fastArray[newoffset] = w0;
		  fastArray[newoffset+1] = w1;
		  fastArray[newoffset+2] = w2;
		  fastArray[newoffset+3] = w3;
		  return ++lastUsed;
		}
		catch(ArrayIndexOutOfBoundsException aioobe)
		*/
		{
		  const int slotsize = 4;
		  int newoffset = (lastUsed + 1) * slotsize;
		  int chunkpos = newoffset >> lowbits;
		  int slotpos = (newoffset & lowmask);

		  // Grow if needed
		  if (chunkpos > chunks.size() - 1)
		  {
			chunks.addElement(new int[chunkalloc]);
		  }
		  int[] chunk = chunks.elementAt(chunkpos);
		  chunk[slotpos] = w0;
		  chunk[slotpos + 1] = w1;
		  chunk[slotpos + 2] = w2;
		  chunk[slotpos + 3] = w3;

		  return ++lastUsed;
		}
	  }
	  /// <summary>
	  /// Retrieve an integer from the CIA by record number and column within
	  /// the record, both 0-based (though position 0 is reserved for special
	  /// purposes). </summary>
	  /// <param name="position"> int Record number </param>
	  /// <param name="slotpos"> int Column number </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: int readEntry(int position, int offset) throws ArrayIndexOutOfBoundsException
	  internal int readEntry(int position, int offset)
	  {
		/*
		try
		{
		  return fastArray[(position*slotsize)+offset];
		}
		catch(ArrayIndexOutOfBoundsException aioobe)
		*/
		{
		  // System.out.println("Using slow read (1)");
		  if (offset >= slotsize)
		  {
			throw new System.IndexOutOfRangeException(XMLMessages.createXMLMessage(XMLErrorResources.ER_OFFSET_BIGGER_THAN_SLOT, null)); //"Offset bigger than slot");
		  }
		  position *= slotsize;
		  int chunkpos = position >> lowbits;
		  int slotpos = position & lowmask;
		  int[] chunk = chunks.elementAt(chunkpos);
		  return chunk[slotpos + offset];
		}
	  }

	  // Check that the node at index "position" is not an ancestor
	  // of the node at index "startPos". IF IT IS, DO NOT ACCEPT IT AND
	  // RETURN -1. If position is NOT an ancestor, return position.
	  // Special case: The Document node (position==0) is acceptable.
	  //
	  // This test supports DTM.getNextPreceding.
	  internal int specialFind(int startPos, int position)
	  {
			  // We have to look all the way up the ancestor chain
			  // to make sure we don't have an ancestor.
			  int ancestor = startPos;
			  while (ancestor > 0)
			  {
					// Get the node whose index == ancestor
					ancestor *= slotsize;
					int chunkpos = ancestor >> lowbits;
					int slotpos = ancestor & lowmask;
					int[] chunk = chunks.elementAt(chunkpos);

					// Get that node's parent (Note that this assumes w[1]
					// is the parent node index. That's really a DTM feature
					// rather than a ChunkedIntArray feature.)
					ancestor = chunk[slotpos + 1];

					if (ancestor == position)
					{
							 break;
					}
			  }

			  if (ancestor <= 0)
			  {
					  return position;
			  }
			  return -1;
	  }

	  /// <returns> int index of highest-numbered record currently in use </returns>
	  internal int slotsUsed()
	  {
		return lastUsed;
	  }

	  /// <summary>
	  /// Disard the highest-numbered record. This is used in the string-buffer
	  /// CIA; when only a single characters() chunk has been recieved, its index
	  /// is moved into the Text node rather than being referenced by indirection
	  /// into the text accumulator.
	  /// </summary>
	  internal void discardLast()
	  {
		--lastUsed;
	  }

	  /// <summary>
	  /// Overwrite the integer found at a specific record and column.
	  /// Used to back-patch existing records, most often changing their
	  /// "next sibling" reference from 0 (unknown) to something meaningful </summary>
	  /// <param name="position"> int Record number </param>
	  /// <param name="offset"> int Column number </param>
	  /// <param name="value"> int New contents </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void writeEntry(int position, int offset, int value) throws ArrayIndexOutOfBoundsException
	  internal void writeEntry(int position, int offset, int value)
	  {
		/*
		try
		{
		  fastArray[( position*slotsize)+offset] = value;
		}
		catch(ArrayIndexOutOfBoundsException aioobe)
		*/
		{
		  if (offset >= slotsize)
		  {
			throw new System.IndexOutOfRangeException(XMLMessages.createXMLMessage(XMLErrorResources.ER_OFFSET_BIGGER_THAN_SLOT, null)); //"Offset bigger than slot");
		  }
		  position *= slotsize;
		  int chunkpos = position >> lowbits;
		  int slotpos = position & lowmask;
		  int[] chunk = chunks.elementAt(chunkpos);
		  chunk[slotpos + offset] = value; // ATOMIC!
		}
	  }

	  /// <summary>
	  /// Overwrite an entire (4-integer) record at the specified index.
	  /// Mostly used to create record 0, the Document node. </summary>
	  /// <param name="position"> integer Record number </param>
	  /// <param name="w0"> int </param>
	  /// <param name="w1"> int </param>
	  /// <param name="w2"> int </param>
	  /// <param name="w3"> int </param>
	  internal void writeSlot(int position, int w0, int w1, int w2, int w3)
	  {
		  position *= slotsize;
		  int chunkpos = position >> lowbits;
		  int slotpos = (position & lowmask);

		// Grow if needed
		if (chunkpos > chunks.size() - 1)
		{
		  chunks.addElement(new int[chunkalloc]);
		}
		int[] chunk = chunks.elementAt(chunkpos);
		chunk[slotpos] = w0;
		chunk[slotpos + 1] = w1;
		chunk[slotpos + 2] = w2;
		chunk[slotpos + 3] = w3;
	  }

	  /// <summary>
	  /// Retrieve the contents of a record into a user-supplied buffer array.
	  /// Used to reduce addressing overhead when code will access several
	  /// columns of the record. </summary>
	  /// <param name="position"> int Record number </param>
	  /// <param name="buffer"> int[] Integer array provided by user, must be large enough
	  /// to hold a complete record. </param>
	  internal void readSlot(int position, int[] buffer)
	  {
		/*
		try
		{
		  System.arraycopy(fastArray, position*slotsize, buffer, 0, slotsize);
		}
		catch(ArrayIndexOutOfBoundsException aioobe)
		*/
		{
		  // System.out.println("Using slow read (2): "+position);
		  position *= slotsize;
		  int chunkpos = position >> lowbits;
		  int slotpos = (position & lowmask);

		  // Grow if needed
		  if (chunkpos > chunks.size() - 1)
		  {
			chunks.addElement(new int[chunkalloc]);
		  }
		  int[] chunk = chunks.elementAt(chunkpos);
		  Array.Copy(chunk,slotpos,buffer,0,slotsize);
		}
	  }

	  internal class ChunksVector
	  {
		internal const int BLOCKSIZE = 64;
		internal int[] m_map = new int[BLOCKSIZE][];
		internal int m_mapSize = BLOCKSIZE;
		internal int pos = 0;

		internal ChunksVector()
		{
		}

		internal int size()
		{
		  return pos;
		}

		internal virtual void addElement(int[] value)
		{
		  if (pos >= m_mapSize)
		  {
			int orgMapSize = m_mapSize;
			while (pos >= m_mapSize)
			{
			  m_mapSize += BLOCKSIZE;
			}
			int[] newMap[] = new int[m_mapSize][];
			Array.Copy(m_map, 0, newMap, 0, orgMapSize);
			m_map = newMap;
		  }
		  // For now, just do a simple append.  A sorted insert only 
		  // makes sense if we're doing an binary search or some such.
		  m_map[pos] = value;
		  pos++;
		}

		internal int[] elementAt(int pos)
		{
		  return m_map[pos];
		}
	  }
	}

}