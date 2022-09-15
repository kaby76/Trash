using System;
using System.Text;

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
 * $Id: FastStringBuffer.java 469279 2006-10-30 21:18:02Z minchau $
 */
namespace org.apache.xml.utils
{
	/// <summary>
	/// Bare-bones, unsafe, fast string buffer. No thread-safety, no
	/// parameter range checking, exposed fields. Note that in typical
	/// applications, thread-safety of a StringBuffer is a somewhat
	/// dubious concept in any case.
	/// <para>
	/// Note that Stree and DTM used a single FastStringBuffer as a string pool,
	/// by recording start and length indices within this single buffer. This
	/// minimizes heap overhead, but of course requires more work when retrieving
	/// the data.
	/// </para>
	/// <para>
	/// FastStringBuffer operates as a "chunked buffer". Doing so
	/// reduces the need to recopy existing information when an append
	/// exceeds the space available; we just allocate another chunk and
	/// flow across to it. (The array of chunks may need to grow,
	/// admittedly, but that's a much smaller object.) Some excess
	/// recopying may arise when we extract Strings which cross chunk
	/// boundaries; larger chunks make that less frequent.
	/// </para>
	/// <para>
	/// The size values are parameterized, to allow tuning this code. In
	/// theory, Result Tree Fragments might want to be tuned differently 
	/// from the main document's text. 
	/// </para>
	/// <para>
	/// %REVIEW% An experiment in self-tuning is
	/// included in the code (using nested FastStringBuffers to achieve
	/// variation in chunk sizes), but this implementation has proven to
	/// be problematic when data may be being copied from the FSB into itself.
	/// We should either re-architect that to make this safe (if possible)
	/// or remove that code and clean up for performance/maintainability reasons.
	/// </para>
	/// <para>
	/// </para>
	/// </summary>
	public class FastStringBuffer
	{
	  // If nonzero, forces the inial chunk size.
	  internal const int DEBUG_FORCE_INIT_BITS = 0;

		  // %BUG% %REVIEW% *****PROBLEM SUSPECTED: If data from an FSB is being copied
		  // back into the same FSB (variable set from previous variable, for example) 
		  // and blocksize changes in mid-copy... there's risk of severe malfunction in 
		  // the read process, due to how the resizing code re-jiggers storage. Arggh. 
		  // If we want to retain the variable-size-block feature, we need to reconsider 
		  // that issue. For now, I have forced us into fixed-size mode.
		internal const bool DEBUG_FORCE_FIXED_CHUNKSIZE = true;

		/// <summary>
		/// Manifest constant: Suppress leading whitespace.
		/// This should be used when normalize-to-SAX is called for the first chunk of a
		/// multi-chunk output, or one following unsuppressed whitespace in a previous
		/// chunk. </summary>
		/// <seealso cref=".sendNormalizedSAXcharacters(org.xml.sax.ContentHandler,int,int)"/>
		public const int SUPPRESS_LEADING_WS = 0x01;

		/// <summary>
		/// Manifest constant: Suppress trailing whitespace.
		/// This should be used when normalize-to-SAX is called for the last chunk of a
		/// multi-chunk output; it may have to be or'ed with SUPPRESS_LEADING_WS.
		/// </summary>
		public const int SUPPRESS_TRAILING_WS = 0x02;

		/// <summary>
		/// Manifest constant: Suppress both leading and trailing whitespace.
		/// This should be used when normalize-to-SAX is called for a complete string.
		/// (I'm not wild about the name of this one. Ideas welcome.) </summary>
		/// <seealso cref=".sendNormalizedSAXcharacters(org.xml.sax.ContentHandler,int,int)"/>
		public const int SUPPRESS_BOTH = SUPPRESS_LEADING_WS | SUPPRESS_TRAILING_WS;

		/// <summary>
		/// Manifest constant: Carry trailing whitespace of one chunk as leading 
		/// whitespace of the next chunk. Used internally; I don't see any reason
		/// to make it public right now.
		/// </summary>
		private const int CARRY_WS = 0x04;

		/// <summary>
		/// Field m_chunkBits sets our chunking strategy, by saying how many
		/// bits of index can be used within a single chunk before flowing over
		/// to the next chunk. For example, if m_chunkbits is set to 15, each
		/// chunk can contain up to 2^15 (32K) characters  
		/// </summary>
	  internal int m_chunkBits = 15;

	  /// <summary>
	  /// Field m_maxChunkBits affects our chunk-growth strategy, by saying what
	  /// the largest permissible chunk size is in this particular FastStringBuffer
	  /// hierarchy. 
	  /// </summary>
	  internal int m_maxChunkBits = 15;

	  /// <summary>
	  /// Field m_rechunkBits affects our chunk-growth strategy, by saying how
	  /// many chunks should be allocated at one size before we encapsulate them
	  /// into the first chunk of the next size up. For example, if m_rechunkBits
	  /// is set to 3, then after 8 chunks at a given size we will rebundle
	  /// them as the first element of a FastStringBuffer using a chunk size
	  /// 8 times larger (chunkBits shifted left three bits).
	  /// </summary>
	  internal int m_rebundleBits = 2;

	  /// <summary>
	  /// Field m_chunkSize establishes the maximum size of one chunk of the array
	  /// as 2**chunkbits characters.
	  /// (Which may also be the minimum size if we aren't tuning for storage) 
	  /// </summary>
	  internal int m_chunkSize; // =1<<(m_chunkBits-1);

	  /// <summary>
	  /// Field m_chunkMask is m_chunkSize-1 -- in other words, m_chunkBits
	  /// worth of low-order '1' bits, useful for shift-and-mask addressing
	  /// within the chunks. 
	  /// </summary>
	  internal int m_chunkMask; // =m_chunkSize-1;

	  /// <summary>
	  /// Field m_array holds the string buffer's text contents, using an
	  /// array-of-arrays. Note that this array, and the arrays it contains, may be
	  /// reallocated when necessary in order to allow the buffer to grow;
	  /// references to them should be considered to be invalidated after any
	  /// append. However, the only time these arrays are directly exposed
	  /// is in the sendSAXcharacters call.
	  /// </summary>
	  internal char[][] m_array;

	  /// <summary>
	  /// Field m_lastChunk is an index into m_array[], pointing to the last
	  /// chunk of the Chunked Array currently in use. Note that additional
	  /// chunks may actually be allocated, eg if the FastStringBuffer had
	  /// previously been truncated or if someone issued an ensureSpace request.
	  /// <para>
	  /// The insertion point for append operations is addressed by the combination
	  /// of m_lastChunk and m_firstFree.
	  /// </para>
	  /// </summary>
	  internal int m_lastChunk = 0;

	  /// <summary>
	  /// Field m_firstFree is an index into m_array[m_lastChunk][], pointing to
	  /// the first character in the Chunked Array which is not part of the
	  /// FastStringBuffer's current content. Since m_array[][] is zero-based,
	  /// the length of that content can be calculated as
	  /// (m_lastChunk<<m_chunkBits) + m_firstFree 
	  /// </summary>
	  internal int m_firstFree = 0;

	  /// <summary>
	  /// Field m_innerFSB, when non-null, is a FastStringBuffer whose total
	  /// length equals m_chunkSize, and which replaces m_array[0]. This allows
	  /// building a hierarchy of FastStringBuffers, where early appends use
	  /// a smaller chunkSize (for less wasted memory overhead) but later
	  /// ones use a larger chunkSize (for less heap activity overhead).
	  /// </summary>
	  internal FastStringBuffer m_innerFSB = null;

	  /// <summary>
	  /// Construct a FastStringBuffer, with allocation policy as per parameters.
	  /// <para>
	  /// For coding convenience, I've expressed both allocation sizes in terms of
	  /// a number of bits. That's needed for the final size of a chunk,
	  /// to permit fast and efficient shift-and-mask addressing. It's less critical
	  /// for the inital size, and may be reconsidered.
	  /// </para>
	  /// <para>
	  /// An alternative would be to accept integer sizes and round to powers of two;
	  /// that really doesn't seem to buy us much, if anything.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="initChunkBits"> Length in characters of the initial allocation
	  /// of a chunk, expressed in log-base-2. (That is, 10 means allocate 1024
	  /// characters.) Later chunks will use larger allocation units, to trade off
	  /// allocation speed of large document against storage efficiency of small
	  /// ones. </param>
	  /// <param name="maxChunkBits"> Number of character-offset bits that should be used for
	  /// addressing within a chunk. Maximum length of a chunk is 2^chunkBits
	  /// characters. </param>
	  /// <param name="rebundleBits"> Number of character-offset bits that addressing should
	  /// advance before we attempt to take a step from initChunkBits to maxChunkBits </param>
	  public FastStringBuffer(int initChunkBits, int maxChunkBits, int rebundleBits)
	  {
		if (DEBUG_FORCE_INIT_BITS != 0)
		{
			initChunkBits = DEBUG_FORCE_INIT_BITS;
		}

		// %REVIEW%
		// Should this force to larger value, or smaller? Smaller less efficient, but if
		// someone requested variable mode it's because they care about storage space.
		// On the other hand, given the other changes I'm making, odds are that we should
		// adopt the larger size. Dither, dither, dither... This is just stopgap workaround
		// anyway; we need a permanant solution.
		//
		if (DEBUG_FORCE_FIXED_CHUNKSIZE)
		{
			maxChunkBits = initChunkBits;
		}
		//if(DEBUG_FORCE_FIXED_CHUNKSIZE) initChunkBits=maxChunkBits;

		m_array = new char[16][];

		// Don't bite off more than we're prepared to swallow!
		if (initChunkBits > maxChunkBits)
		{
		  initChunkBits = maxChunkBits;
		}

		m_chunkBits = initChunkBits;
		m_maxChunkBits = maxChunkBits;
		m_rebundleBits = rebundleBits;
		m_chunkSize = 1 << (initChunkBits);
		m_chunkMask = m_chunkSize - 1;
		m_array[0] = new char[m_chunkSize];
	  }

	  /// <summary>
	  /// Construct a FastStringBuffer, using a default rebundleBits value.
	  /// </summary>
	  /// NEEDSDOC <param name="initChunkBits"> </param>
	  /// NEEDSDOC <param name="maxChunkBits"> </param>
	  public FastStringBuffer(int initChunkBits, int maxChunkBits) : this(initChunkBits, maxChunkBits, 2)
	  {
	  }

	  /// <summary>
	  /// Construct a FastStringBuffer, using default maxChunkBits and
	  /// rebundleBits values.
	  /// <para>
	  /// ISSUE: Should this call assert initial size, or fixed size?
	  /// Now configured as initial, with a default for fixed.
	  /// 
	  /// </para>
	  /// </summary>
	  /// NEEDSDOC <param name="initChunkBits"> </param>
	  public FastStringBuffer(int initChunkBits) : this(initChunkBits, 15, 2)
	  {
	  }

	  /// <summary>
	  /// Construct a FastStringBuffer, using a default allocation policy.
	  /// </summary>
	  public FastStringBuffer() : this(10, 15, 2)
	  {

		// 10 bits is 1K. 15 bits is 32K. Remember that these are character
		// counts, so actual memory allocation unit is doubled for UTF-16 chars.
		//
		// For reference: In the original FastStringBuffer, we simply
		// overallocated by blocksize (default 1KB) on each buffer-growth.
	  }

	  /// <summary>
	  /// Get the length of the list. Synonym for length().
	  /// </summary>
	  /// <returns> the number of characters in the FastStringBuffer's content. </returns>
	  public int size()
	  {
		return (m_lastChunk << m_chunkBits) + m_firstFree;
	  }

	  /// <summary>
	  /// Get the length of the list. Synonym for size().
	  /// </summary>
	  /// <returns> the number of characters in the FastStringBuffer's content. </returns>
	  public int length()
	  {
		return (m_lastChunk << m_chunkBits) + m_firstFree;
	  }

	  /// <summary>
	  /// Discard the content of the FastStringBuffer, and most of the memory
	  /// that was allocated by it, restoring the initial state. Note that this
	  /// may eventually be different from setLength(0), which see.
	  /// </summary>
	  public void reset()
	  {

		m_lastChunk = 0;
		m_firstFree = 0;

		// Recover the original chunk size
		FastStringBuffer innermost = this;

		while (innermost.m_innerFSB != null)
		{
		  innermost = innermost.m_innerFSB;
		}

		m_chunkBits = innermost.m_chunkBits;
		m_chunkSize = innermost.m_chunkSize;
		m_chunkMask = innermost.m_chunkMask;

		// Discard the hierarchy
		m_innerFSB = null;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: m_array = new char[16][0];
		m_array = RectangularArrays.RectangularCharArray(16, 0);
		m_array[0] = new char[m_chunkSize];
	  }

	  /// <summary>
	  /// Directly set how much of the FastStringBuffer's storage is to be
	  /// considered part of its content. This is a fast but hazardous
	  /// operation. It is not protected against negative values, or values
	  /// greater than the amount of storage currently available... and even
	  /// if additional storage does exist, its contents are unpredictable.
	  /// The only safe use for our setLength() is to truncate the FastStringBuffer
	  /// to a shorter string.
	  /// </summary>
	  /// <param name="l"> New length. If l<0 or l>=getLength(), this operation will
	  /// not report an error but future operations will almost certainly fail. </param>
	  public void setLength(int l)
	  {
		m_lastChunk = (int)((uint)l >> m_chunkBits);

		if (m_lastChunk == 0 && m_innerFSB != null)
		{
		  // Replace this FSB with the appropriate inner FSB, truncated
		  m_innerFSB.setLength(l, this);
		}
		else
		{
		  m_firstFree = l & m_chunkMask;

		  // There's an edge case if l is an exact multiple of m_chunkBits, which risks leaving
		  // us pointing at the start of a chunk which has not yet been allocated. Rather than 
		  // pay the cost of dealing with that in the append loops (more scattered and more
		  // inner-loop), we correct it here by moving to the safe side of that
		  // line -- as we would have left the indexes had we appended up to that point.
		  if (m_firstFree == 0 && m_lastChunk > 0)
		  {
			  --m_lastChunk;
			  m_firstFree = m_chunkSize;
		  }
		}
	  }

	  /// <summary>
	  /// Subroutine for the public setLength() method. Deals with the fact
	  /// that truncation may require restoring one of the innerFSBs
	  /// </summary>
	  /// NEEDSDOC <param name="l"> </param>
	  /// NEEDSDOC <param name="rootFSB"> </param>
	  private void setLength(int l, FastStringBuffer rootFSB)
	  {

		m_lastChunk = (int)((uint)l >> m_chunkBits);

		if (m_lastChunk == 0 && m_innerFSB != null)
		{
		  m_innerFSB.setLength(l, rootFSB);
		}
		else
		{

		  // Undo encapsulation -- pop the innerFSB data back up to root.
		  // Inefficient, but attempts to keep the code simple.
		  rootFSB.m_chunkBits = m_chunkBits;
		  rootFSB.m_maxChunkBits = m_maxChunkBits;
		  rootFSB.m_rebundleBits = m_rebundleBits;
		  rootFSB.m_chunkSize = m_chunkSize;
		  rootFSB.m_chunkMask = m_chunkMask;
		  rootFSB.m_array = m_array;
		  rootFSB.m_innerFSB = m_innerFSB;
		  rootFSB.m_lastChunk = m_lastChunk;

		  // Finally, truncate this sucker.
		  rootFSB.m_firstFree = l & m_chunkMask;
		}
	  }

	  /// <summary>
	  /// Note that this operation has been somewhat deoptimized by the shift to a
	  /// chunked array, as there is no factory method to produce a String object
	  /// directly from an array of arrays and hence a double copy is needed.
	  /// By using ensureCapacity we hope to minimize the heap overhead of building
	  /// the intermediate StringBuffer.
	  /// <para>
	  /// (It really is a pity that Java didn't design String as a final subclass
	  /// of MutableString, rather than having StringBuffer be a separate hierarchy.
	  /// We'd avoid a <strong>lot</strong> of double-buffering.)
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the contents of the FastStringBuffer as a standard Java string. </returns>
	  public sealed override string ToString()
	  {

		int length = (m_lastChunk << m_chunkBits) + m_firstFree;

		return getString(new StringBuilder(length), 0, 0, length).ToString();
	  }

	  /// <summary>
	  /// Append a single character onto the FastStringBuffer, growing the
	  /// storage if necessary.
	  /// <para>
	  /// NOTE THAT after calling append(), previously obtained
	  /// references to m_array[][] may no longer be valid....
	  /// though in fact they should be in this instance.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="value"> character to be appended. </param>
	  public void append(char value)
	  {

		char[] chunk;

		// We may have preallocated chunks. If so, all but last should
		// be at full size.

		if (m_firstFree < m_chunkSize) // Simplified test single-character-fits
		{
		  chunk = m_array[m_lastChunk];
		}
		else
		{

		  // Extend array?
		  int i = m_array.Length;

		  if (m_lastChunk + 1 == i)
		  {
			char[][] newarray = new char[i + 16][];

			Array.Copy(m_array, 0, newarray, 0, i);

			m_array = newarray;
		  }

		  // Advance one chunk
		  chunk = m_array[++m_lastChunk];

		  if (chunk == null)
		  {

			// Hierarchical encapsulation
			if (m_lastChunk == 1 << m_rebundleBits && m_chunkBits < m_maxChunkBits)
			{

			  // Should do all the work of both encapsulating
			  // existing data and establishing new sizes/offsets
			  m_innerFSB = new FastStringBuffer(this);
			}

			// Add a chunk.
			chunk = m_array[m_lastChunk] = new char[m_chunkSize];
		  }

		  m_firstFree = 0;
		}

		// Space exists in the chunk. Append the character.
		chunk[m_firstFree++] = value;
	  }

	  /// <summary>
	  /// Append the contents of a String onto the FastStringBuffer,
	  /// growing the storage if necessary.
	  /// <para>
	  /// NOTE THAT after calling append(), previously obtained
	  /// references to m_array[] may no longer be valid.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="value"> String whose contents are to be appended. </param>
	  public void append(string value)
	  {

		if (string.ReferenceEquals(value, null))
		{
		  return;
		}
		int strlen = value.Length;

		if (0 == strlen)
		{
		  return;
		}

		int copyfrom = 0;
		char[] chunk = m_array[m_lastChunk];
		int available = m_chunkSize - m_firstFree;

		// Repeat while data remains to be copied
		while (strlen > 0)
		{

		  // Copy what fits
		  if (available > strlen)
		  {
			available = strlen;
		  }

		  value.CopyTo(copyfrom, m_array[m_lastChunk], m_firstFree, copyfrom + available - copyfrom);

		  strlen -= available;
		  copyfrom += available;

		  // If there's more left, allocate another chunk and continue
		  if (strlen > 0)
		  {

			// Extend array?
			int i = m_array.Length;

			if (m_lastChunk + 1 == i)
			{
			  char[][] newarray = new char[i + 16][];

			  Array.Copy(m_array, 0, newarray, 0, i);

			  m_array = newarray;
			}

			// Advance one chunk
			chunk = m_array[++m_lastChunk];

			if (chunk == null)
			{

			  // Hierarchical encapsulation
			  if (m_lastChunk == 1 << m_rebundleBits && m_chunkBits < m_maxChunkBits)
			  {

				// Should do all the work of both encapsulating
				// existing data and establishing new sizes/offsets
				m_innerFSB = new FastStringBuffer(this);
			  }

			  // Add a chunk. 
			  chunk = m_array[m_lastChunk] = new char[m_chunkSize];
			}

			available = m_chunkSize;
			m_firstFree = 0;
		  }
		}

		// Adjust the insert point in the last chunk, when we've reached it.
		m_firstFree += available;
	  }

	  /// <summary>
	  /// Append the contents of a StringBuffer onto the FastStringBuffer,
	  /// growing the storage if necessary.
	  /// <para>
	  /// NOTE THAT after calling append(), previously obtained
	  /// references to m_array[] may no longer be valid.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="value"> StringBuffer whose contents are to be appended. </param>
	  public void append(StringBuilder value)
	  {

		if (value == null)
		{
		  return;
		}
		int strlen = value.Length;

		if (0 == strlen)
		{
		  return;
		}

		int copyfrom = 0;
		char[] chunk = m_array[m_lastChunk];
		int available = m_chunkSize - m_firstFree;

		// Repeat while data remains to be copied
		while (strlen > 0)
		{

		  // Copy what fits
		  if (available > strlen)
		  {
			available = strlen;
		  }

		  value.getChars(copyfrom, copyfrom + available, m_array[m_lastChunk], m_firstFree);

		  strlen -= available;
		  copyfrom += available;

		  // If there's more left, allocate another chunk and continue
		  if (strlen > 0)
		  {

			// Extend array?
			int i = m_array.Length;

			if (m_lastChunk + 1 == i)
			{
			  char[][] newarray = new char[i + 16][];

			  Array.Copy(m_array, 0, newarray, 0, i);

			  m_array = newarray;
			}

			// Advance one chunk
			chunk = m_array[++m_lastChunk];

			if (chunk == null)
			{

			  // Hierarchical encapsulation
			  if (m_lastChunk == 1 << m_rebundleBits && m_chunkBits < m_maxChunkBits)
			  {

				// Should do all the work of both encapsulating
				// existing data and establishing new sizes/offsets
				m_innerFSB = new FastStringBuffer(this);
			  }

			  // Add a chunk.
			  chunk = m_array[m_lastChunk] = new char[m_chunkSize];
			}

			available = m_chunkSize;
			m_firstFree = 0;
		  }
		}

		// Adjust the insert point in the last chunk, when we've reached it.
		m_firstFree += available;
	  }

	  /// <summary>
	  /// Append part of the contents of a Character Array onto the
	  /// FastStringBuffer,  growing the storage if necessary.
	  /// <para>
	  /// NOTE THAT after calling append(), previously obtained
	  /// references to m_array[] may no longer be valid.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="chars"> character array from which data is to be copied </param>
	  /// <param name="start"> offset in chars of first character to be copied,
	  /// zero-based. </param>
	  /// <param name="length"> number of characters to be copied </param>
	  public void append(char[] chars, int start, int length)
	  {

		int strlen = length;

		if (0 == strlen)
		{
		  return;
		}

		int copyfrom = start;
		char[] chunk = m_array[m_lastChunk];
		int available = m_chunkSize - m_firstFree;

		// Repeat while data remains to be copied
		while (strlen > 0)
		{

		  // Copy what fits
		  if (available > strlen)
		  {
			available = strlen;
		  }

		  Array.Copy(chars, copyfrom, m_array[m_lastChunk], m_firstFree, available);

		  strlen -= available;
		  copyfrom += available;

		  // If there's more left, allocate another chunk and continue
		  if (strlen > 0)
		  {

			// Extend array?
			int i = m_array.Length;

			if (m_lastChunk + 1 == i)
			{
			  char[][] newarray = new char[i + 16][];

			  Array.Copy(m_array, 0, newarray, 0, i);

			  m_array = newarray;
			}

			// Advance one chunk
			chunk = m_array[++m_lastChunk];

			if (chunk == null)
			{

			  // Hierarchical encapsulation
			  if (m_lastChunk == 1 << m_rebundleBits && m_chunkBits < m_maxChunkBits)
			  {

				// Should do all the work of both encapsulating
				// existing data and establishing new sizes/offsets
				m_innerFSB = new FastStringBuffer(this);
			  }

			  // Add a chunk.
			  chunk = m_array[m_lastChunk] = new char[m_chunkSize];
			}

			available = m_chunkSize;
			m_firstFree = 0;
		  }
		}

		// Adjust the insert point in the last chunk, when we've reached it.
		m_firstFree += available;
	  }

	  /// <summary>
	  /// Append the contents of another FastStringBuffer onto
	  /// this FastStringBuffer, growing the storage if necessary.
	  /// <para>
	  /// NOTE THAT after calling append(), previously obtained
	  /// references to m_array[] may no longer be valid.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="value"> FastStringBuffer whose contents are
	  /// to be appended. </param>
	  public void append(FastStringBuffer value)
	  {

		// Complicating factor here is that the two buffers may use
		// different chunk sizes, and even if they're the same we're
		// probably on a different alignment due to previously appended
		// data. We have to work through the source in bite-sized chunks.
		if (value == null)
		{
		  return;
		}
		int strlen = value.length();

		if (0 == strlen)
		{
		  return;
		}

		int copyfrom = 0;
		char[] chunk = m_array[m_lastChunk];
		int available = m_chunkSize - m_firstFree;

		// Repeat while data remains to be copied
		while (strlen > 0)
		{

		  // Copy what fits
		  if (available > strlen)
		  {
			available = strlen;
		  }

		  int sourcechunk = (int)((uint)(copyfrom + value.m_chunkSize - 1) >> value.m_chunkBits);
		  int sourcecolumn = copyfrom & value.m_chunkMask;
		  int runlength = value.m_chunkSize - sourcecolumn;

		  if (runlength > available)
		  {
			runlength = available;
		  }

		  Array.Copy(value.m_array[sourcechunk], sourcecolumn, m_array[m_lastChunk], m_firstFree, runlength);

		  if (runlength != available)
		  {
			Array.Copy(value.m_array[sourcechunk + 1], 0, m_array[m_lastChunk], m_firstFree + runlength, available - runlength);
		  }

		  strlen -= available;
		  copyfrom += available;

		  // If there's more left, allocate another chunk and continue
		  if (strlen > 0)
		  {

			// Extend array?
			int i = m_array.Length;

			if (m_lastChunk + 1 == i)
			{
			  char[][] newarray = new char[i + 16][];

			  Array.Copy(m_array, 0, newarray, 0, i);

			  m_array = newarray;
			}

			// Advance one chunk
			chunk = m_array[++m_lastChunk];

			if (chunk == null)
			{

			  // Hierarchical encapsulation
			  if (m_lastChunk == 1 << m_rebundleBits && m_chunkBits < m_maxChunkBits)
			  {

				// Should do all the work of both encapsulating
				// existing data and establishing new sizes/offsets
				m_innerFSB = new FastStringBuffer(this);
			  }

			  // Add a chunk. 
			  chunk = m_array[m_lastChunk] = new char[m_chunkSize];
			}

			available = m_chunkSize;
			m_firstFree = 0;
		  }
		}

		// Adjust the insert point in the last chunk, when we've reached it.
		m_firstFree += available;
	  }

	  /// <returns> true if the specified range of characters are all whitespace,
	  /// as defined by XMLCharacterRecognizer.
	  /// <para>
	  /// CURRENTLY DOES NOT CHECK FOR OUT-OF-RANGE.
	  /// 
	  /// </para>
	  /// </returns>
	  /// <param name="start"> Offset of first character in the range. </param>
	  /// <param name="length"> Number of characters to send. </param>
	  public virtual bool isWhitespace(int start, int length)
	  {

		int sourcechunk = (int)((uint)start >> m_chunkBits);
		int sourcecolumn = start & m_chunkMask;
		int available = m_chunkSize - sourcecolumn;
		bool chunkOK;

		while (length > 0)
		{
		  int runlength = (length <= available) ? length : available;

		  if (sourcechunk == 0 && m_innerFSB != null)
		  {
			chunkOK = m_innerFSB.isWhitespace(sourcecolumn, runlength);
		  }
		  else
		  {
			chunkOK = org.apache.xml.utils.XMLCharacterRecognizer.isWhiteSpace(m_array[sourcechunk], sourcecolumn, runlength);
		  }

		  if (!chunkOK)
		  {
			return false;
		  }

		  length -= runlength;

		  ++sourcechunk;

		  sourcecolumn = 0;
		  available = m_chunkSize;
		}

		return true;
	  }

	  /// <param name="start"> Offset of first character in the range. </param>
	  /// <param name="length"> Number of characters to send. </param>
	  /// <returns> a new String object initialized from the specified range of
	  /// characters. </returns>
	  public virtual string getString(int start, int length)
	  {
		int startColumn = start & m_chunkMask;
		int startChunk = (int)((uint)start >> m_chunkBits);
		if (startColumn + length < m_chunkMask && m_innerFSB == null)
		{
		  return getOneChunkString(startChunk, startColumn, length);
		}
		return getString(new StringBuilder(length), startChunk, startColumn, length).ToString();
	  }

	  protected internal virtual string getOneChunkString(int startChunk, int startColumn, int length)
	  {
		return new string(m_array[startChunk], startColumn, length);
	  }

	  /// <param name="sb"> StringBuffer to be appended to </param>
	  /// <param name="start"> Offset of first character in the range. </param>
	  /// <param name="length"> Number of characters to send. </param>
	  /// <returns> sb with the requested text appended to it </returns>
	  internal virtual StringBuilder getString(StringBuilder sb, int start, int length)
	  {
		return getString(sb, (int)((uint)start >> m_chunkBits), start & m_chunkMask, length);
	  }

	  /// <summary>
	  /// Internal support for toString() and getString().
	  /// PLEASE NOTE SIGNATURE CHANGE from earlier versions; it now appends into
	  /// and returns a StringBuffer supplied by the caller. This simplifies
	  /// m_innerFSB support.
	  /// <para>
	  /// Note that this operation has been somewhat deoptimized by the shift to a
	  /// chunked array, as there is no factory method to produce a String object
	  /// directly from an array of arrays and hence a double copy is needed.
	  /// By presetting length we hope to minimize the heap overhead of building
	  /// the intermediate StringBuffer.
	  /// </para>
	  /// <para>
	  /// (It really is a pity that Java didn't design String as a final subclass
	  /// of MutableString, rather than having StringBuffer be a separate hierarchy.
	  /// We'd avoid a <strong>lot</strong> of double-buffering.)
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="sb"> </param>
	  /// <param name="startChunk"> </param>
	  /// <param name="startColumn"> </param>
	  /// <param name="length">
	  /// </param>
	  /// <returns> the contents of the FastStringBuffer as a standard Java string. </returns>
	  internal virtual StringBuilder getString(StringBuilder sb, int startChunk, int startColumn, int length)
	  {

		int stop = (startChunk << m_chunkBits) + startColumn + length;
		int stopChunk = (int)((uint)stop >> m_chunkBits);
		int stopColumn = stop & m_chunkMask;

		// Factored out
		//StringBuffer sb=new StringBuffer(length);
		for (int i = startChunk; i < stopChunk; ++i)
		{
		  if (i == 0 && m_innerFSB != null)
		  {
			m_innerFSB.getString(sb, startColumn, m_chunkSize - startColumn);
		  }
		  else
		  {
			sb.Append(m_array[i], startColumn, m_chunkSize - startColumn);
		  }

		  startColumn = 0; // after first chunk
		}

		if (stopChunk == 0 && m_innerFSB != null)
		{
		  m_innerFSB.getString(sb, startColumn, stopColumn - startColumn);
		}
		else if (stopColumn > startColumn)
		{
		  sb.Append(m_array[stopChunk], startColumn, stopColumn - startColumn);
		}

		return sb;
	  }

	  /// <summary>
	  /// Get a single character from the string buffer.
	  /// 
	  /// </summary>
	  /// <param name="pos"> character position requested. </param>
	  /// <returns> A character from the requested position. </returns>
	  public virtual char charAt(int pos)
	  {
		int startChunk = (int)((uint)pos >> m_chunkBits);

		if (startChunk == 0 && m_innerFSB != null)
		{
		  return m_innerFSB.charAt(pos & m_chunkMask);
		}
		else
		{
		  return m_array[startChunk][pos & m_chunkMask];
		}
	  }

	  /// <summary>
	  /// Sends the specified range of characters as one or more SAX characters()
	  /// events.
	  /// Note that the buffer reference passed to the ContentHandler may be
	  /// invalidated if the FastStringBuffer is edited; it's the user's
	  /// responsibility to manage access to the FastStringBuffer to prevent this
	  /// problem from arising.
	  /// <para>
	  /// Note too that there is no promise that the output will be sent as a
	  /// single call. As is always true in SAX, one logical string may be split
	  /// across multiple blocks of memory and hence delivered as several
	  /// successive events.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="ch"> SAX ContentHandler object to receive the event. </param>
	  /// <param name="start"> Offset of first character in the range. </param>
	  /// <param name="length"> Number of characters to send. </param>
	  /// <exception cref="org.xml.sax.SAXException"> may be thrown by handler's
	  /// characters() method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void sendSAXcharacters(org.xml.sax.ContentHandler ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void sendSAXcharacters(org.xml.sax.ContentHandler ch, int start, int length)
	  {

		int startChunk = (int)((uint)start >> m_chunkBits);
		int startColumn = start & m_chunkMask;
		if (startColumn + length < m_chunkMask && m_innerFSB == null)
		{
			ch.characters(m_array[startChunk], startColumn, length);
			return;
		}

		int stop = start + length;
		int stopChunk = (int)((uint)stop >> m_chunkBits);
		int stopColumn = stop & m_chunkMask;

		for (int i = startChunk; i < stopChunk; ++i)
		{
		  if (i == 0 && m_innerFSB != null)
		  {
			m_innerFSB.sendSAXcharacters(ch, startColumn, m_chunkSize - startColumn);
		  }
		  else
		  {
			ch.characters(m_array[i], startColumn, m_chunkSize - startColumn);
		  }

		  startColumn = 0; // after first chunk
		}

		// Last, or only, chunk
		if (stopChunk == 0 && m_innerFSB != null)
		{
		  m_innerFSB.sendSAXcharacters(ch, startColumn, stopColumn - startColumn);
		}
		else if (stopColumn > startColumn)
		{
		  ch.characters(m_array[stopChunk], startColumn, stopColumn - startColumn);
		}
	  }

	  /// <summary>
	  /// Sends the specified range of characters as one or more SAX characters()
	  /// events, normalizing the characters according to XSLT rules.
	  /// </summary>
	  /// <param name="ch"> SAX ContentHandler object to receive the event. </param>
	  /// <param name="start"> Offset of first character in the range. </param>
	  /// <param name="length"> Number of characters to send. </param>
	  /// <returns> normalization status to apply to next chunk (because we may
	  /// have been called recursively to process an inner FSB):
	  /// <dl>
	  /// <dt>0</dt>
	  /// <dd>if this output did not end in retained whitespace, and thus whitespace
	  /// at the start of the following chunk (if any) should be converted to a
	  /// single space.
	  /// <dt>SUPPRESS_LEADING_WS</dt>
	  /// <dd>if this output ended in retained whitespace, and thus whitespace
	  /// at the start of the following chunk (if any) should be completely
	  /// suppressed.</dd>
	  /// </dd>
	  /// </dl> </returns>
	  /// <exception cref="org.xml.sax.SAXException"> may be thrown by handler's
	  /// characters() method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int sendNormalizedSAXcharacters(org.xml.sax.ContentHandler ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual int sendNormalizedSAXcharacters(org.xml.sax.ContentHandler ch, int start, int length)
	  {
		// This call always starts at the beginning of the 
		// string being written out, either because it was called directly or
		// because it was an m_innerFSB recursion. This is important since
		// it gives us a well-known initial state for this flag:
		int stateForNextChunk = SUPPRESS_LEADING_WS;

		int stop = start + length;
		int startChunk = (int)((uint)start >> m_chunkBits);
		int startColumn = start & m_chunkMask;
		int stopChunk = (int)((uint)stop >> m_chunkBits);
		int stopColumn = stop & m_chunkMask;

		for (int i = startChunk; i < stopChunk; ++i)
		{
		  if (i == 0 && m_innerFSB != null)
		  {
					stateForNextChunk = m_innerFSB.sendNormalizedSAXcharacters(ch, startColumn, m_chunkSize - startColumn);
		  }
		  else
		  {
					stateForNextChunk = sendNormalizedSAXcharacters(m_array[i], startColumn, m_chunkSize - startColumn, ch,stateForNextChunk);
		  }

		  startColumn = 0; // after first chunk
		}

		// Last, or only, chunk
		if (stopChunk == 0 && m_innerFSB != null)
		{
				stateForNextChunk = m_innerFSB.sendNormalizedSAXcharacters(ch, startColumn, stopColumn - startColumn);
		}
		else if (stopColumn > startColumn)
		{
				stateForNextChunk = sendNormalizedSAXcharacters(m_array[stopChunk], startColumn, stopColumn - startColumn, ch, stateForNextChunk | SUPPRESS_TRAILING_WS);
		}
			return stateForNextChunk;
	  }

	  internal static readonly char[] SINGLE_SPACE = new char[] {' '};

	  /// <summary>
	  /// Internal method to directly normalize and dispatch the character array.
	  /// This version is aware of the fact that it may be called several times
	  /// in succession if the data is made up of multiple "chunks", and thus
	  /// must actively manage the handling of leading and trailing whitespace.
	  /// 
	  /// Note: The recursion is due to the possible recursion of inner FSBs.
	  /// </summary>
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <param name="handler"> SAX ContentHandler object to receive the event. </param>
	  /// <param name="edgeTreatmentFlags"> How leading/trailing spaces should be handled. 
	  /// This is a bitfield contining two flags, bitwise-ORed together:
	  /// <dl>
	  /// <dt>SUPPRESS_LEADING_WS</dt>
	  /// <dd>When false, causes leading whitespace to be converted to a single
	  /// space; when true, causes it to be discarded entirely.
	  /// Should be set TRUE for the first chunk, and (in multi-chunk output)
	  /// whenever the previous chunk ended in retained whitespace.</dd>
	  /// <dt>SUPPRESS_TRAILING_WS</dt>
	  /// <dd>When false, causes trailing whitespace to be converted to a single
	  /// space; when true, causes it to be discarded entirely.
	  /// Should be set TRUE for the last or only chunk.
	  /// </dd>
	  /// </dl> </param>
	  /// <returns> normalization status, as in the edgeTreatmentFlags parameter:
	  /// <dl>
	  /// <dt>0</dt>
	  /// <dd>if this output did not end in retained whitespace, and thus whitespace
	  /// at the start of the following chunk (if any) should be converted to a
	  /// single space.
	  /// <dt>SUPPRESS_LEADING_WS</dt>
	  /// <dd>if this output ended in retained whitespace, and thus whitespace
	  /// at the start of the following chunk (if any) should be completely
	  /// suppressed.</dd>
	  /// </dd>
	  /// </dl>
	  /// 
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static int sendNormalizedSAXcharacters(char ch[], int start, int length, org.xml.sax.ContentHandler handler, int edgeTreatmentFlags) throws org.xml.sax.SAXException
	  internal static int sendNormalizedSAXcharacters(char[] ch, int start, int length, org.xml.sax.ContentHandler handler, int edgeTreatmentFlags)
	  {
		 bool processingLeadingWhitespace = ((edgeTreatmentFlags & SUPPRESS_LEADING_WS) != 0);
		 bool seenWhitespace = ((edgeTreatmentFlags & CARRY_WS) != 0);
		 int currPos = start;
		 int limit = start + length;

		 // Strip any leading spaces first, if required
		 if (processingLeadingWhitespace)
		 {
			 for (; currPos < limit && XMLCharacterRecognizer.isWhiteSpace(ch[currPos]); currPos++)
			 {
			 }

			 // If we've only encountered leading spaces, the
			 // current state remains unchanged
			 if (currPos == limit)
			 {
				 return edgeTreatmentFlags;
			 }
		 }

		 // If we get here, there are no more leading spaces to strip
		 while (currPos < limit)
		 {
			 int startNonWhitespace = currPos;

			 // Grab a chunk of non-whitespace characters
			 for (; currPos < limit && !XMLCharacterRecognizer.isWhiteSpace(ch[currPos]); currPos++)
			 {
			 }

			 // Non-whitespace seen - emit them, along with a single
			 // space for any preceding whitespace characters
			 if (startNonWhitespace != currPos)
			 {
				 if (seenWhitespace)
				 {
					 handler.characters(SINGLE_SPACE, 0, 1);
					 seenWhitespace = false;
				 }
				 handler.characters(ch, startNonWhitespace, currPos - startNonWhitespace);
			 }

			 int startWhitespace = currPos;

			 // Consume any whitespace characters
			 for (; currPos < limit && XMLCharacterRecognizer.isWhiteSpace(ch[currPos]); currPos++)
			 {
			 }

			 if (startWhitespace != currPos)
			 {
				 seenWhitespace = true;
			 }
		 }

		 return (seenWhitespace ? CARRY_WS : 0) | (edgeTreatmentFlags & SUPPRESS_TRAILING_WS);
	  }

	  /// <summary>
	  /// Directly normalize and dispatch the character array.
	  /// </summary>
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <param name="handler"> SAX ContentHandler object to receive the event. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void sendNormalizedSAXcharacters(char ch[], int start, int length, org.xml.sax.ContentHandler handler) throws org.xml.sax.SAXException
	  public static void sendNormalizedSAXcharacters(char[] ch, int start, int length, org.xml.sax.ContentHandler handler)
	  {
			sendNormalizedSAXcharacters(ch, start, length, handler, SUPPRESS_BOTH);
	  }

		/// <summary>
		/// Sends the specified range of characters as sax Comment.
		/// <para>
		/// Note that, unlike sendSAXcharacters, this has to be done as a single 
		/// call to LexicalHandler#comment.
		///  
		/// </para>
		/// </summary>
		/// <param name="ch"> SAX LexicalHandler object to receive the event. </param>
		/// <param name="start"> Offset of first character in the range. </param>
		/// <param name="length"> Number of characters to send. </param>
		/// <exception cref="org.xml.sax.SAXException"> may be thrown by handler's
		/// characters() method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void sendSAXComment(org.xml.sax.ext.LexicalHandler ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void sendSAXComment(org.xml.sax.ext.LexicalHandler ch, int start, int length)
	  {

		// %OPT% Do it this way for now...
		string comment = getString(start, length);
		ch.comment(comment.ToCharArray(), 0, length);
	  }

	  /// <summary>
	  /// Copies characters from this string into the destination character
	  /// array.
	  /// </summary>
	  /// <param name="srcBegin">   index of the first character in the string
	  ///                        to copy. </param>
	  /// <param name="srcEnd">     index after the last character in the string
	  ///                        to copy. </param>
	  /// <param name="dst">        the destination array. </param>
	  /// <param name="dstBegin">   the start offset in the destination array. </param>
	  /// <exception cref="IndexOutOfBoundsException"> If any of the following
	  ///            is true:
	  ///            <ul><li><code>srcBegin</code> is negative.
	  ///            <li><code>srcBegin</code> is greater than <code>srcEnd</code>
	  ///            <li><code>srcEnd</code> is greater than the length of this
	  ///                string
	  ///            <li><code>dstBegin</code> is negative
	  ///            <li><code>dstBegin+(srcEnd-srcBegin)</code> is larger than
	  ///                <code>dst.length</code></ul> </exception>
	  /// <exception cref="NullPointerException"> if <code>dst</code> is <code>null</code> </exception>
	  private void getChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
	  {
		// %TBD% Joe needs to write this function.  Make public when implemented.
	  }

	  /// <summary>
	  /// Encapsulation c'tor. After this is called, the source FastStringBuffer
	  /// will be reset to use the new object as its m_innerFSB, and will have
	  /// had its chunk size reset appropriately. IT SHOULD NEVER BE CALLED
	  /// EXCEPT WHEN source.length()==1<<(source.m_chunkBits+source.m_rebundleBits)
	  /// </summary>
	  /// NEEDSDOC <param name="source"> </param>
	  private FastStringBuffer(FastStringBuffer source)
	  {

		// Copy existing information into new encapsulation
		m_chunkBits = source.m_chunkBits;
		m_maxChunkBits = source.m_maxChunkBits;
		m_rebundleBits = source.m_rebundleBits;
		m_chunkSize = source.m_chunkSize;
		m_chunkMask = source.m_chunkMask;
		m_array = source.m_array;
		m_innerFSB = source.m_innerFSB;

		// These have to be adjusted because we're calling just at the time
		// when we would be about to allocate another chunk
		m_lastChunk = source.m_lastChunk - 1;
		m_firstFree = source.m_chunkSize;

		// Establish capsule as the Inner FSB, reset chunk sizes/addressing
		source.m_array = new char[16][];
		source.m_innerFSB = this;

		// Since we encapsulated just as we were about to append another
		// chunk, return ready to create the chunk after the innerFSB
		// -- 1, not 0.
		source.m_lastChunk = 1;
		source.m_firstFree = 0;
		source.m_chunkBits += m_rebundleBits;
		source.m_chunkSize = 1 << (source.m_chunkBits);
		source.m_chunkMask = source.m_chunkSize - 1;
	  }
	}

}