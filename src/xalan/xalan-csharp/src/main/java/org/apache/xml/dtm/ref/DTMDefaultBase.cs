using System;
using System.Collections;
using System.IO;
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
 * $Id: DTMDefaultBase.java 1225429 2011-12-29 04:44:11Z mrglavas $
 */
namespace org.apache.xml.dtm.@ref
{
	using org.apache.xml.dtm;
	using SuballocatedIntVector = org.apache.xml.utils.SuballocatedIntVector;
	using BoolStack = org.apache.xml.utils.BoolStack;


	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	using XMLMessages = org.apache.xml.res.XMLMessages;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;

	/// <summary>
	/// The <code>DTMDefaultBase</code> class serves as a helper base for DTMs.
	/// It sets up structures for navigation and type, while leaving data
	/// management and construction to the derived classes.
	/// </summary>
	public abstract class DTMDefaultBase : DTM
	{
		public abstract javax.xml.transform.SourceLocator getSourceLocatorFor(int node);
		public abstract org.xml.sax.ext.DeclHandler DeclHandler {get;}
		public abstract org.xml.sax.ErrorHandler ErrorHandler {get;}
		public abstract org.xml.sax.DTDHandler DTDHandler {get;}
		public abstract org.xml.sax.EntityResolver EntityResolver {get;}
		public abstract org.xml.sax.ext.LexicalHandler LexicalHandler {get;}
		public abstract org.xml.sax.ContentHandler ContentHandler {get;}
		public abstract bool needsTwoThreads();
		public abstract DTMAxisIterator getTypedAxisIterator(int axis, int type);
		public abstract DTMAxisIterator getAxisIterator(int axis);
		public abstract DTMAxisTraverser getAxisTraverser(int axis);
		public abstract void setProperty(string property, object value);
		internal const bool JJK_DEBUG = false;

	  // This constant is likely to be removed in the future. Use the 
	  // getDocument() method instead of ROOTNODE to get at the root 
	  // node of a DTM.
	  /// <summary>
	  /// The identity of the root node. </summary>
	  public const int ROOTNODE = 0;

	  /// <summary>
	  /// The number of nodes, which is also used to determine the next
	  ///  node index.
	  /// </summary>
	  protected internal int m_size = 0;

	  /// <summary>
	  /// The expanded names, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_exptype;

	  /// <summary>
	  /// First child values, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_firstch;

	  /// <summary>
	  /// Next sibling values, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_nextsib;

	  /// <summary>
	  /// Previous sibling values, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_prevsib;

	  /// <summary>
	  /// Previous sibling values, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_parent;

	  /// <summary>
	  /// Vector of SuballocatedIntVectors of NS decl sets </summary>
	  protected internal ArrayList m_namespaceDeclSets = null;

	  /// <summary>
	  /// SuballocatedIntVector  of elements at which corresponding
	  /// namespaceDeclSets were defined 
	  /// </summary>
	  protected internal SuballocatedIntVector m_namespaceDeclSetElements = null;

	  /// <summary>
	  /// These hold indexes to elements based on namespace and local name.
	  /// The base lookup is the the namespace.  The second lookup is the local
	  /// name, and the last array contains the the first free element
	  /// at the start, and the list of element handles following.
	  /// </summary>
	  protected internal int[][][] m_elemIndexes;

	  /// <summary>
	  /// The default block size of the node arrays </summary>
	  public const int DEFAULT_BLOCKSIZE = 512; // favor small docs.

	  /// <summary>
	  /// The number of blocks for the node arrays </summary>
	  public const int DEFAULT_NUMBLOCKS = 32;

	  /// <summary>
	  /// The number of blocks used for small documents & RTFs </summary>
	  public const int DEFAULT_NUMBLOCKS_SMALL = 4;

	  /// <summary>
	  /// The block size of the node arrays </summary>
	  //protected final int m_blocksize;

	  /// <summary>
	  /// The value to use when the information has not been built yet.
	  /// </summary>
	  protected internal static readonly int NOTPROCESSED = DTM.NULL - 1;

	  /// <summary>
	  /// The DTM manager who "owns" this DTM.
	  /// </summary>

	  public DTMManager m_mgr;

	  /// <summary>
	  /// m_mgr cast to DTMManagerDefault, or null if it isn't an instance
	  /// (Efficiency hook)
	  /// </summary>
	  protected internal DTMManagerDefault m_mgrDefault = null;


	  /// <summary>
	  /// The document identity number(s). If we have overflowed the addressing
	  /// range of the first that was assigned to us, we may add others. 
	  /// </summary>
	  protected internal SuballocatedIntVector m_dtmIdent;

	  /// <summary>
	  /// The mask for the identity.
	  ///    %REVIEW% Should this really be set to the _DEFAULT? What if
	  ///    a particular DTM wanted to use another value? 
	  /// </summary>
	  //protected final static int m_mask = DTMManager.IDENT_NODE_DEFAULT;

	  /// <summary>
	  /// The base URI for this document. </summary>
	  protected internal string m_documentBaseURI;

	  /// <summary>
	  /// The whitespace filter that enables elements to strip whitespace or not.
	  /// </summary>
	  protected internal DTMWSFilter m_wsfilter;

	  /// <summary>
	  /// Flag indicating whether to strip whitespace nodes </summary>
	  protected internal bool m_shouldStripWS = false;

	  /// <summary>
	  /// Stack of flags indicating whether to strip whitespace nodes </summary>
	  protected internal BoolStack m_shouldStripWhitespaceStack;

	  /// <summary>
	  /// The XMLString factory for creating XMLStrings. </summary>
	  protected internal XMLStringFactory m_xstrf;

	  /// <summary>
	  /// The table for exandedNameID lookups.  This may or may not be the same
	  /// table as is contained in the DTMManagerDefault.
	  /// </summary>
	  protected internal ExpandedNameTable m_expandedNameTable;

	  /// <summary>
	  /// true if indexing is turned on. </summary>
	  protected internal bool m_indexing;

	  /// <summary>
	  /// Construct a DTMDefaultBase object using the default block size.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> The object that is used to specify the construction source. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> The factory to use for creating XMLStrings. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  public DTMDefaultBase(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : this(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, DEFAULT_BLOCKSIZE, true, false)
	  {
	  }

	  /// <summary>
	  /// Construct a DTMDefaultBase object from a DOM node.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> The object that is used to specify the construction source. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> The factory to use for creating XMLStrings. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  /// <param name="blocksize"> The block size of the DTM. </param>
	  /// <param name="usePrevsib"> true if we want to build the previous sibling node array. </param>
	  /// <param name="newNameTable"> true if we want to use a new ExpandedNameTable for this DTM. </param>
	  public DTMDefaultBase(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool usePrevsib, bool newNameTable)
	  {
		// Use smaller sizes for the internal node arrays if the block size
		// is small.
		int numblocks;
		if (blocksize <= 64)
		{
		  numblocks = DEFAULT_NUMBLOCKS_SMALL;
		  m_dtmIdent = new SuballocatedIntVector(4, 1);
		}
		else
		{
		  numblocks = DEFAULT_NUMBLOCKS;
		  m_dtmIdent = new SuballocatedIntVector(32);
		}

		m_exptype = new SuballocatedIntVector(blocksize, numblocks);
		m_firstch = new SuballocatedIntVector(blocksize, numblocks);
		m_nextsib = new SuballocatedIntVector(blocksize, numblocks);
		m_parent = new SuballocatedIntVector(blocksize, numblocks);

		// Only create the m_prevsib array if the usePrevsib flag is true.
		// Some DTM implementations (e.g. SAXImpl) do not need this array.
		// We can save the time to build it in those cases.
		if (usePrevsib)
		{
		  m_prevsib = new SuballocatedIntVector(blocksize, numblocks);
		}

		m_mgr = mgr;
		if (mgr is DTMManagerDefault)
		{
		  m_mgrDefault = (DTMManagerDefault)mgr;
		}

		m_documentBaseURI = (null != source) ? source.getSystemId() : null;
		m_dtmIdent.setElementAt(dtmIdentity,0);
		m_wsfilter = whiteSpaceFilter;
		m_xstrf = xstringfactory;
		m_indexing = doIndexing;

		if (doIndexing)
		{
		  m_expandedNameTable = new ExpandedNameTable();
		}
		else
		{
		  // Note that this fails if we aren't talking to an instance of
		  // DTMManagerDefault
		  m_expandedNameTable = m_mgrDefault.getExpandedNameTable(this);
		}

		if (null != whiteSpaceFilter)
		{
		  m_shouldStripWhitespaceStack = new BoolStack();

		  pushShouldStripWhitespace(false);
		}
	  }

	  /// <summary>
	  /// Ensure that the size of the element indexes can hold the information.
	  /// </summary>
	  /// <param name="namespaceID"> Namespace ID index. </param>
	  /// <param name="LocalNameID"> Local name ID. </param>
	  protected internal virtual void ensureSizeOfIndex(int namespaceID, int LocalNameID)
	  {

		if (null == m_elemIndexes)
		{
		  m_elemIndexes = new int[namespaceID + 20][][];
		}
		else if (m_elemIndexes.Length <= namespaceID)
		{
		  int[][][] indexes = m_elemIndexes;

		  m_elemIndexes = new int[namespaceID + 20][][];

		  Array.Copy(indexes, 0, m_elemIndexes, 0, indexes.Length);
		}

		int[][] localNameIndex = m_elemIndexes[namespaceID];

		if (null == localNameIndex)
		{
		  localNameIndex = new int[LocalNameID + 100][];
		  m_elemIndexes[namespaceID] = localNameIndex;
		}
		else if (localNameIndex.Length <= LocalNameID)
		{
		  int[][] indexes = localNameIndex;

		  localNameIndex = new int[LocalNameID + 100][];

		  Array.Copy(indexes, 0, localNameIndex, 0, indexes.Length);

		  m_elemIndexes[namespaceID] = localNameIndex;
		}

		int[] elemHandles = localNameIndex[LocalNameID];

		if (null == elemHandles)
		{
		  elemHandles = new int[128];
		  localNameIndex[LocalNameID] = elemHandles;
		  elemHandles[0] = 1;
		}
		else if (elemHandles.Length <= elemHandles[0] + 1)
		{
		  int[] indexes = elemHandles;

		  elemHandles = new int[elemHandles[0] + 1024];

		  Array.Copy(indexes, 0, elemHandles, 0, indexes.Length);

		  localNameIndex[LocalNameID] = elemHandles;
		}
	  }

	  /// <summary>
	  /// Add a node to the element indexes. The node will not be added unless
	  /// it's an element.
	  /// </summary>
	  /// <param name="expandedTypeID"> The expanded type ID of the node. </param>
	  /// <param name="identity"> The node identity index. </param>
	  protected internal virtual void indexNode(int expandedTypeID, int identity)
	  {

		ExpandedNameTable ent = m_expandedNameTable;
		short type = ent.getType(expandedTypeID);

		if (DTM.ELEMENT_NODE == type)
		{
		  int namespaceID = ent.getNamespaceID(expandedTypeID);
		  int localNameID = ent.getLocalNameID(expandedTypeID);

		  ensureSizeOfIndex(namespaceID, localNameID);

		  int[] index = m_elemIndexes[namespaceID][localNameID];

		  index[index[0]] = identity;

		  index[0]++;
		}
	  }

	  /// <summary>
	  /// Find the first index that occurs in the list that is greater than or
	  /// equal to the given value.
	  /// </summary>
	  /// <param name="list"> A list of integers. </param>
	  /// <param name="start"> The start index to begin the search. </param>
	  /// <param name="len"> The number of items to search. </param>
	  /// <param name="value"> Find the slot that has a value that is greater than or
	  /// identical to this argument.
	  /// </param>
	  /// <returns> The index in the list of the slot that is higher or identical
	  /// to the identity argument, or -1 if no node is higher or equal. </returns>
	  protected internal virtual int findGTE(int[] list, int start, int len, int value)
	  {

		int low = start;
		int high = start + (len - 1);
		int end = high;

		while (low <= high)
		{
		  int mid = (int)((uint)(low + high) >> 1);
		  int c = list[mid];

		  if (c > value)
		  {
			high = mid - 1;
		  }
		  else if (c < value)
		  {
			low = mid + 1;
		  }
		  else
		  {
			return mid;
		  }
		}

		return (low <= end && list[low] > value) ? low : -1;
	  }

	  /// <summary>
	  /// Find the first matching element from the index at or after the
	  /// given node.
	  /// </summary>
	  /// <param name="nsIndex"> The namespace index lookup. </param>
	  /// <param name="lnIndex"> The local name index lookup. </param>
	  /// <param name="firstPotential"> The first potential match that is worth looking at.
	  /// </param>
	  /// <returns> The first node that is greater than or equal to the
	  ///         firstPotential argument, or DTM.NOTPROCESSED if not found. </returns>
	  internal virtual int findElementFromIndex(int nsIndex, int lnIndex, int firstPotential)
	  {

		int[][][] indexes = m_elemIndexes;

		if (null != indexes && nsIndex < indexes.Length)
		{
		  int[][] lnIndexs = indexes[nsIndex];

		  if (null != lnIndexs && lnIndex < lnIndexs.Length)
		  {
			int[] elems = lnIndexs[lnIndex];

			if (null != elems)
			{
			  int pos = findGTE(elems, 1, elems[0], firstPotential);

			  if (pos > -1)
			  {
				return elems[pos];
			  }
			}
		  }
		}

		return NOTPROCESSED;
	  }

	  /// <summary>
	  /// Get the next node identity value in the list, and call the iterator
	  /// if it hasn't been added yet.
	  /// </summary>
	  /// <param name="identity"> The node identity (index). </param>
	  /// <returns> identity+1, or DTM.NULL. </returns>
	  protected internal abstract int getNextNodeIdentity(int identity);

	  /// <summary>
	  /// This method should try and build one or more nodes in the table.
	  /// </summary>
	  /// <returns> The true if a next node is found or false if
	  ///         there are no more nodes. </returns>
	  protected internal abstract bool nextNode();

	  /// <summary>
	  /// Get the number of nodes that have been added.
	  /// </summary>
	  /// <returns> the number of nodes that have been mapped. </returns>
	  protected internal abstract int NumberOfNodes {get;}

	  /// <summary>
	  /// Stateless axis traversers, lazely built. </summary>
	  protected internal DTMAxisTraverser[] m_traversers;

	//    /**
	//     * Ensure that the size of the information arrays can hold another entry
	//     * at the given index.
	//     *
	//     * @param index On exit from this function, the information arrays sizes must be
	//     * at least index+1.
	//     */
	//    protected void ensureSize(int index)
	//    {
	//        // We've cut over to Suballocated*Vector, which are self-sizing.
	//    }

	  /// <summary>
	  /// Get the simple type ID for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The simple type ID, or DTM.NULL. </returns>
	  protected internal virtual short _type(int identity)
	  {

		int info = _exptype(identity);

		if (NULL != info)
		{
		  return m_expandedNameTable.getType(info);
		}
		else
		{
		  return NULL;
		}
	  }

	  /// <summary>
	  /// Get the expanded type ID for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The expanded type ID, or DTM.NULL. </returns>
	  protected internal virtual int _exptype(int identity)
	  {
		  if (identity == DTM.NULL)
		  {
		  return NULL;
		  }
		// Reorganized test and loop into single flow
		// Tiny performance improvement, saves a few bytes of code, clearer.
		// %OPT% Other internal getters could be treated simliarly
		while (identity >= m_size)
		{
		  if (!nextNode() && identity >= m_size)
		  {
			return NULL;
		  }
		}
		return m_exptype.elementAt(identity);

	  }

	  /// <summary>
	  /// Get the level in the tree for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The tree level, or DTM.NULL. </returns>
	  protected internal virtual int _level(int identity)
	  {
		while (identity >= m_size)
		{
		  bool isMore = nextNode();
		  if (!isMore && identity >= m_size)
		  {
			return NULL;
		  }
		}

		int i = 0;
		while (NULL != (identity = _parent(identity)))
		{
		  ++i;
		}
		return i;
	  }

	  /// <summary>
	  /// Get the first child for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The first child identity, or DTM.NULL. </returns>
	  protected internal virtual int _firstch(int identity)
	  {

		// Boiler-plate code for each of the _xxx functions, except for the array.
		int info = (identity >= m_size) ? NOTPROCESSED : m_firstch.elementAt(identity);

		// Check to see if the information requested has been processed, and,
		// if not, advance the iterator until we the information has been
		// processed.
		while (info == NOTPROCESSED)
		{
		  bool isMore = nextNode();

		  if (identity >= m_size && !isMore)
		  {
			return NULL;
		  }
		  else
		  {
			info = m_firstch.elementAt(identity);
			if (info == NOTPROCESSED && !isMore)
			{
			  return NULL;
			}
		  }
		}

		return info;
	  }

	  /// <summary>
	  /// Get the next sibling for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The next sibling identity, or DTM.NULL. </returns>
	  protected internal virtual int _nextsib(int identity)
	  {
		// Boiler-plate code for each of the _xxx functions, except for the array.
		int info = (identity >= m_size) ? NOTPROCESSED : m_nextsib.elementAt(identity);

		// Check to see if the information requested has been processed, and,
		// if not, advance the iterator until we the information has been
		// processed.
		while (info == NOTPROCESSED)
		{
		  bool isMore = nextNode();

		  if (identity >= m_size && !isMore)
		  {
			return NULL;
		  }
		  else
		  {
			info = m_nextsib.elementAt(identity);
			if (info == NOTPROCESSED && !isMore)
			{
			  return NULL;
			}
		  }
		}

		return info;
	  }

	  /// <summary>
	  /// Get the previous sibling for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The previous sibling identity, or DTM.NULL. </returns>
	  protected internal virtual int _prevsib(int identity)
	  {

		if (identity < m_size)
		{
		  return m_prevsib.elementAt(identity);
		}

		// Check to see if the information requested has been processed, and,
		// if not, advance the iterator until we the information has been
		// processed.
		while (true)
		{
		  bool isMore = nextNode();

		  if (identity >= m_size && !isMore)
		  {
			return NULL;
		  }
		  else if (identity < m_size)
		  {
			return m_prevsib.elementAt(identity);
		  }
		}
	  }

	  /// <summary>
	  /// Get the parent for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The parent identity, or DTM.NULL. </returns>
	  protected internal virtual int _parent(int identity)
	  {

		if (identity < m_size)
		{
		  return m_parent.elementAt(identity);
		}

		// Check to see if the information requested has been processed, and,
		// if not, advance the iterator until we the information has been
		// processed.
		while (true)
		{
		  bool isMore = nextNode();

		  if (identity >= m_size && !isMore)
		  {
			return NULL;
		  }
		  else if (identity < m_size)
		  {
			return m_parent.elementAt(identity);
		  }
		}
	  }

	  /// <summary>
	  /// Diagnostics function to dump the DTM.
	  /// </summary>
	  public virtual void dumpDTM(Stream os)
	  {
		try
		{
		  if (os == null)
		  {
			  File f = new File("DTMDump" + ((object)this).GetHashCode() + ".txt");
			   Console.Error.WriteLine("Dumping... " + f.getAbsolutePath());
			   os = new FileStream(f, FileMode.Create, FileAccess.Write);
		  }
		  PrintStream ps = new PrintStream(os);

		  while (nextNode())
		  {
		  }

		  int nRecords = m_size;

		  ps.println("Total nodes: " + nRecords);

		  for (int index = 0; index < nRecords; ++index)
		  {
			  int i = makeNodeHandle(index);
			ps.println("=========== index=" + index + " handle=" + i + " ===========");
			ps.println("NodeName: " + getNodeName(i));
			ps.println("NodeNameX: " + getNodeNameX(i));
			ps.println("LocalName: " + getLocalName(i));
			ps.println("NamespaceURI: " + getNamespaceURI(i));
			ps.println("Prefix: " + getPrefix(i));

			int exTypeID = _exptype(index);

			ps.println("Expanded Type ID: " + Convert.ToString(exTypeID, 16));

			int type = _type(index);
			string typestring;

			switch (type)
			{
			case DTM.ATTRIBUTE_NODE :
			  typestring = "ATTRIBUTE_NODE";
			  break;
			case DTM.CDATA_SECTION_NODE :
			  typestring = "CDATA_SECTION_NODE";
			  break;
			case DTM.COMMENT_NODE :
			  typestring = "COMMENT_NODE";
			  break;
			case DTM.DOCUMENT_FRAGMENT_NODE :
			  typestring = "DOCUMENT_FRAGMENT_NODE";
			  break;
			case DTM.DOCUMENT_NODE :
			  typestring = "DOCUMENT_NODE";
			  break;
			case DTM.DOCUMENT_TYPE_NODE :
			  typestring = "DOCUMENT_NODE";
			  break;
			case DTM.ELEMENT_NODE :
			  typestring = "ELEMENT_NODE";
			  break;
			case DTM.ENTITY_NODE :
			  typestring = "ENTITY_NODE";
			  break;
			case DTM.ENTITY_REFERENCE_NODE :
			  typestring = "ENTITY_REFERENCE_NODE";
			  break;
			case DTM.NAMESPACE_NODE :
			  typestring = "NAMESPACE_NODE";
			  break;
			case DTM.NOTATION_NODE :
			  typestring = "NOTATION_NODE";
			  break;
			case DTM.NULL :
			  typestring = "NULL";
			  break;
			case DTM.PROCESSING_INSTRUCTION_NODE :
			  typestring = "PROCESSING_INSTRUCTION_NODE";
			  break;
			case DTM.TEXT_NODE :
			  typestring = "TEXT_NODE";
			  break;
			default :
			  typestring = "Unknown!";
			  break;
			}

			ps.println("Type: " + typestring);

			int firstChild = _firstch(index);

			if (DTM.NULL == firstChild)
			{
			  ps.println("First child: DTM.NULL");
			}
			else if (NOTPROCESSED == firstChild)
			{
			  ps.println("First child: NOTPROCESSED");
			}
			else
			{
			  ps.println("First child: " + firstChild);
			}

			if (m_prevsib != null)
			{
			  int prevSibling = _prevsib(index);

			  if (DTM.NULL == prevSibling)
			  {
				ps.println("Prev sibling: DTM.NULL");
			  }
			  else if (NOTPROCESSED == prevSibling)
			  {
				ps.println("Prev sibling: NOTPROCESSED");
			  }
			  else
			  {
				ps.println("Prev sibling: " + prevSibling);
			  }
			}

			int nextSibling = _nextsib(index);

			if (DTM.NULL == nextSibling)
			{
			  ps.println("Next sibling: DTM.NULL");
			}
			else if (NOTPROCESSED == nextSibling)
			{
			  ps.println("Next sibling: NOTPROCESSED");
			}
			else
			{
			  ps.println("Next sibling: " + nextSibling);
			}

			int parent = _parent(index);

			if (DTM.NULL == parent)
			{
			  ps.println("Parent: DTM.NULL");
			}
			else if (NOTPROCESSED == parent)
			{
			  ps.println("Parent: NOTPROCESSED");
			}
			else
			{
			  ps.println("Parent: " + parent);
			}

			int level = _level(index);

			ps.println("Level: " + level);
			ps.println("Node Value: " + getNodeValue(i));
			ps.println("String Value: " + getStringValue(i));
		  }
		}
		catch (IOException ioe)
		{
		  ioe.printStackTrace(System.err);
			throw new Exception(ioe.Message);
		}
	  }

	  /// <summary>
	  /// Diagnostics function to dump a single node.
	  /// 
	  /// %REVIEW% KNOWN GLITCH: If you pass it a node index rather than a 
	  /// node handle, it works just fine... but the displayed identity 
	  /// number before the colon is different, which complicates comparing
	  /// it with nodes printed the other way. We could always OR the DTM ID
	  /// into the value, to suppress that distinction...
	  /// 
	  /// %REVIEW% This might want to be moved up to DTMDefaultBase, or possibly
	  /// DTM itself, since it's a useful diagnostic and uses only DTM's public
	  /// APIs.
	  /// </summary>
	  public virtual string dumpNode(int nodeHandle)
	  {
		  if (nodeHandle == DTM.NULL)
		  {
			  return "[null]";
		  }

			string typestring;
			switch (getNodeType(nodeHandle))
			{
			case DTM.ATTRIBUTE_NODE :
			  typestring = "ATTR";
			  break;
			case DTM.CDATA_SECTION_NODE :
			  typestring = "CDATA";
			  break;
			case DTM.COMMENT_NODE :
			  typestring = "COMMENT";
			  break;
			case DTM.DOCUMENT_FRAGMENT_NODE :
			  typestring = "DOC_FRAG";
			  break;
			case DTM.DOCUMENT_NODE :
			  typestring = "DOC";
			  break;
			case DTM.DOCUMENT_TYPE_NODE :
			  typestring = "DOC_TYPE";
			  break;
			case DTM.ELEMENT_NODE :
			  typestring = "ELEMENT";
			  break;
			case DTM.ENTITY_NODE :
			  typestring = "ENTITY";
			  break;
			case DTM.ENTITY_REFERENCE_NODE :
			  typestring = "ENT_REF";
			  break;
			case DTM.NAMESPACE_NODE :
			  typestring = "NAMESPACE";
			  break;
			case DTM.NOTATION_NODE :
			  typestring = "NOTATION";
			  break;
			case DTM.NULL :
			  typestring = "null";
			  break;
			case DTM.PROCESSING_INSTRUCTION_NODE :
			  typestring = "PI";
			  break;
			case DTM.TEXT_NODE :
			  typestring = "TEXT";
			  break;
			default :
			  typestring = "Unknown!";
			  break;
			}

		  StringBuilder sb = new StringBuilder();
		  sb.Append("[" + nodeHandle + ": " + typestring + "(0x" + Convert.ToString(getExpandedTypeID(nodeHandle), 16) + ") " + getNodeNameX(nodeHandle) + " {" + getNamespaceURI(nodeHandle) + "}" + "=\"" + getNodeValue(nodeHandle) + "\"]");
		  return sb.ToString();
	  }

	  // ========= DTM Implementation Control Functions. ==============

	  /// <summary>
	  /// Set an implementation dependent feature.
	  /// <para>
	  /// %REVIEW% Do we really expect to set features on DTMs?
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="featureId"> A feature URL. </param>
	  /// <param name="state"> true if this feature should be on, false otherwise. </param>
	  public virtual void setFeature(string featureId, bool state)
	  {
	  }

	  // ========= Document Navigation Functions =========

	  /// <summary>
	  /// Given a node handle, test if it has child nodes.
	  /// <para> %REVIEW% This is obviously useful at the DOM layer, where it
	  /// would permit testing this without having to create a proxy
	  /// node. It's less useful in the DTM API, where
	  /// (dtm.getFirstChild(nodeHandle)!=DTM.NULL) is just as fast and
	  /// almost as self-evident. But it's a convenience, and eases porting
	  /// of DOM code to DTM.  </para>
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int true if the given node has child nodes. </returns>
	  public virtual bool hasChildNodes(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int firstChild = _firstch(identity);

		return firstChild != DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node identity, return a node handle. If extended addressing
	  /// has been used (multiple DTM IDs), we need to map the high bits of the
	  /// identity into the proper DTM ID.
	  /// 
	  /// This has been made FINAL to facilitate inlining, since we do not expect
	  /// any subclass of DTMDefaultBase to ever change the algorithm. (I don't
	  /// really like doing so, and would love to have an excuse not to...)
	  /// 
	  /// %REVIEW% Is it worth trying to specialcase small documents?
	  /// %REVIEW% Should this be exposed at the package/public layers?
	  /// </summary>
	  /// <param name="nodeIdentity"> Internal offset to this node's records. </param>
	  /// <returns> NodeHandle (external representation of node)
	  ///  </returns>
	  public int makeNodeHandle(int nodeIdentity)
	  {
		if (NULL == nodeIdentity)
		{
			return NULL;
		}

		if (JJK_DEBUG && nodeIdentity > DTMManager.IDENT_NODE_DEFAULT)
		{
		  Console.Error.WriteLine("GONK! (only useful in limited situations)");
		}

		return m_dtmIdent.elementAt((int)((uint)nodeIdentity >> DTMManager.IDENT_DTM_NODE_BITS)) + (nodeIdentity & DTMManager.IDENT_NODE_DEFAULT);
	  }

	  /// <summary>
	  /// Given a node handle, return a node identity. If extended addressing
	  /// has been used (multiple DTM IDs), we need to map the high bits of the
	  /// identity into the proper DTM ID and thence find the proper offset
	  /// to add to the low bits of the identity
	  /// 
	  /// This has been made FINAL to facilitate inlining, since we do not expect
	  /// any subclass of DTMDefaultBase to ever change the algorithm. (I don't
	  /// really like doing so, and would love to have an excuse not to...)
	  /// 
	  /// %OPT% Performance is critical for this operation.
	  /// 
	  /// %REVIEW% Should this be exposed at the package/public layers?
	  /// </summary>
	  /// <param name="nodeHandle"> (external representation of node) </param>
	  /// <returns> nodeIdentity Internal offset to this node's records.
	  ///  </returns>
	  public int makeNodeIdentity(int nodeHandle)
	  {
		if (NULL == nodeHandle)
		{
			return NULL;
		}

		if (m_mgrDefault != null)
		{
		  // Optimization: use the DTMManagerDefault's fast DTMID-to-offsets
		  // table.  I'm not wild about this solution but this operation
		  // needs need extreme speed.

		  int whichDTMindex = (int)((uint)nodeHandle >> DTMManager.IDENT_DTM_NODE_BITS);

		  // %REVIEW% Wish I didn't have to perform the pre-test, but
		  // someone is apparently asking DTMs whether they contain nodes
		  // which really don't belong to them. That's probably a bug
		  // which should be fixed, but until it is:
		  if (m_mgrDefault.m_dtms[whichDTMindex] != this)
		  {
		return NULL;
		  }
		  else
		  {
		return m_mgrDefault.m_dtm_offsets[whichDTMindex] | (nodeHandle & DTMManager.IDENT_NODE_DEFAULT);
		  }
		}

		int whichDTMid = m_dtmIdent.indexOf(nodeHandle & DTMManager.IDENT_DTM_DEFAULT);
		return (whichDTMid == NULL) ? NULL : (whichDTMid << DTMManager.IDENT_DTM_NODE_BITS) + (nodeHandle & DTMManager.IDENT_NODE_DEFAULT);
	  }


	  /// <summary>
	  /// Given a node handle, get the handle of the node's first child.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int DTM node-number of first child, or DTM.NULL to indicate none exists. </returns>
	  public virtual int getFirstChild(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int firstChild = _firstch(identity);

		return makeNodeHandle(firstChild);
	  }

	  /// <summary>
	  /// Given a node handle, get the handle of the node's first child.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int DTM node-number of first child, or DTM.NULL to indicate none exists. </returns>
	  public virtual int getTypedFirstChild(int nodeHandle, int nodeType)
	  {

		int firstChild, eType;
		if (nodeType < DTM.NTYPES)
		{
		  for (firstChild = _firstch(makeNodeIdentity(nodeHandle)); firstChild != DTM.NULL; firstChild = _nextsib(firstChild))
		  {
			eType = _exptype(firstChild);
			if (eType == nodeType || (eType >= DTM.NTYPES && m_expandedNameTable.getType(eType) == nodeType))
			{
			  return makeNodeHandle(firstChild);
			}
		  }
		}
		else
		{
		  for (firstChild = _firstch(makeNodeIdentity(nodeHandle)); firstChild != DTM.NULL; firstChild = _nextsib(firstChild))
		  {
			if (_exptype(firstChild) == nodeType)
			{
			  return makeNodeHandle(firstChild);
			}
		  }
		}
		return DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node handle, advance to its last child.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int Node-number of last child,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getLastChild(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int child = _firstch(identity);
		int lastChild = DTM.NULL;

		while (child != DTM.NULL)
		{
		  lastChild = child;
		  child = _nextsib(child);
		}

		return makeNodeHandle(lastChild);
	  }

	  /// <summary>
	  /// Retrieves an attribute node by by qualified name and namespace URI.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node upon which to look up this attribute.. </param>
	  /// <param name="namespaceURI"> The namespace URI of the attribute to
	  ///   retrieve, or null. </param>
	  /// <param name="name"> The local name of the attribute to
	  ///   retrieve. </param>
	  /// <returns> The attribute node handle with the specified name (
	  ///   <code>nodeName</code>) or <code>DTM.NULL</code> if there is no such
	  ///   attribute. </returns>
	  public abstract int getAttributeNode(int nodeHandle, string namespaceURI, string name);

	  /// <summary>
	  /// Given a node handle, get the index of the node's first attribute.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> Handle of first attribute, or DTM.NULL to indicate none exists. </returns>
	  public virtual int getFirstAttribute(int nodeHandle)
	  {
		int nodeID = makeNodeIdentity(nodeHandle);

		return makeNodeHandle(getFirstAttributeIdentity(nodeID));
	  }

	  /// <summary>
	  /// Given a node identity, get the index of the node's first attribute.
	  /// </summary>
	  /// <param name="identity"> int identity of the node. </param>
	  /// <returns> Identity of first attribute, or DTM.NULL to indicate none exists. </returns>
	  protected internal virtual int getFirstAttributeIdentity(int identity)
	  {
		int type = _type(identity);

		if (DTM.ELEMENT_NODE == type)
		{
		  // Assume that attributes and namespaces immediately follow the element.
		  while (DTM.NULL != (identity = getNextNodeIdentity(identity)))
		  {

			// Assume this can not be null.
			type = _type(identity);

			if (type == DTM.ATTRIBUTE_NODE)
			{
			  return identity;
			}
			else if (DTM.NAMESPACE_NODE != type)
			{
			  break;
			}
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node handle and an expanded type ID, get the index of the node's
	  /// attribute of that type, if any.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <param name="attType"> int expanded type ID of the required attribute. </param>
	  /// <returns> Handle of attribute of the required type, or DTM.NULL to indicate
	  /// none exists. </returns>
	  protected internal virtual int getTypedAttribute(int nodeHandle, int attType)
	  {
		int type = getNodeType(nodeHandle);
		if (DTM.ELEMENT_NODE == type)
		{
		  int identity = makeNodeIdentity(nodeHandle);

		  while (DTM.NULL != (identity = getNextNodeIdentity(identity)))
		  {
			type = _type(identity);

			if (type == DTM.ATTRIBUTE_NODE)
			{
			  if (_exptype(identity) == attType)
			  {
				  return makeNodeHandle(identity);
			  }
			}
			else if (DTM.NAMESPACE_NODE != type)
			{
			  break;
			}
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node handle, advance to its next sibling.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again. </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int Node-number of next sibling,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getNextSibling(int nodeHandle)
	  {
		  if (nodeHandle == DTM.NULL)
		  {
		  return DTM.NULL;
		  }
		return makeNodeHandle(_nextsib(makeNodeIdentity(nodeHandle)));
	  }

	  /// <summary>
	  /// Given a node handle, advance to its next sibling.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again. </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int Node-number of next sibling,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getTypedNextSibling(int nodeHandle, int nodeType)
	  {
		  if (nodeHandle == DTM.NULL)
		  {
		  return DTM.NULL;
		  }
		  int node = makeNodeIdentity(nodeHandle);
		  int eType;
		  while ((node = _nextsib(node)) != DTM.NULL && ((eType = _exptype(node)) != nodeType && m_expandedNameTable.getType(eType) != nodeType))
		  {
				  ;
		  }
		  //_type(node) != nodeType));

		return (node == DTM.NULL ? DTM.NULL : makeNodeHandle(node));
	  }

	  /// <summary>
	  /// Given a node handle, find its preceeding sibling.
	  /// WARNING: DTM is asymmetric; this operation is resolved by search, and is
	  /// relatively expensive.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node-number of the previous sib,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getPreviousSibling(int nodeHandle)
	  {
		if (nodeHandle == DTM.NULL)
		{
		  return DTM.NULL;
		}

		if (m_prevsib != null)
		{
		  return makeNodeHandle(_prevsib(makeNodeIdentity(nodeHandle)));
		}
		else
		{
		  // If the previous sibling array is not built, we get at
		  // the previous sibling using the parent, firstch and 
		  // nextsib arrays. 
		  int nodeID = makeNodeIdentity(nodeHandle);
		  int parent = _parent(nodeID);
		  int node = _firstch(parent);
		  int result = DTM.NULL;
		  while (node != nodeID)
		  {
			result = node;
			node = _nextsib(node);
		  }
		  return makeNodeHandle(result);
		}
	  }

	  /// <summary>
	  /// Given a node handle, advance to the next attribute.
	  /// If an attr, we advance to
	  /// the next attr on the same node.  If not an attribute, we return NULL.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int DTM node-number of the resolved attr,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getNextAttribute(int nodeHandle)
	  {
		int nodeID = makeNodeIdentity(nodeHandle);

		if (_type(nodeID) == DTM.ATTRIBUTE_NODE)
		{
		  return makeNodeHandle(getNextAttributeIdentity(nodeID));
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node identity for an attribute, advance to the next attribute.
	  /// </summary>
	  /// <param name="identity"> int identity of the attribute node.  This
	  /// <strong>must</strong> be an attribute node.
	  /// </param>
	  /// <returns> int DTM node-identity of the resolved attr,
	  /// or DTM.NULL to indicate none exists.
	  ///  </returns>
	  protected internal virtual int getNextAttributeIdentity(int identity)
	  {
		// Assume that attributes and namespace nodes immediately follow the element
		while (DTM.NULL != (identity = getNextNodeIdentity(identity)))
		{
		  int type = _type(identity);

		  if (type == DTM.ATTRIBUTE_NODE)
		  {
			return identity;
		  }
		  else if (type != DTM.NAMESPACE_NODE)
		  {
			break;
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Lazily created namespace lists. </summary>
	  private ArrayList m_namespaceLists = null; // on demand


	  /// <summary>
	  /// Build table of namespace declaration
	  /// locations during DTM construction. Table is a Vector of
	  /// SuballocatedIntVectors containing the namespace node HANDLES declared at
	  /// that ID, plus an SuballocatedIntVector of the element node INDEXES at which
	  /// these declarations appeared.
	  /// 
	  /// NOTE: Since this occurs during model build, nodes will be encountered
	  /// in doucment order and thus the table will be ordered by element,
	  /// permitting binary-search as a possible retrieval optimization.
	  /// 
	  /// %REVIEW% Directly managed arrays rather than vectors?
	  /// %REVIEW% Handles or IDs? Given usage, I think handles.
	  /// 
	  /// </summary>
	  protected internal virtual void declareNamespaceInContext(int elementNodeIndex, int namespaceNodeIndex)
	  {
		SuballocatedIntVector nsList = null;
		if (m_namespaceDeclSets == null)
		{

			// First
			m_namespaceDeclSetElements = new SuballocatedIntVector(32);
			m_namespaceDeclSetElements.addElement(elementNodeIndex);
			m_namespaceDeclSets = new ArrayList();
			nsList = new SuballocatedIntVector(32);
			m_namespaceDeclSets.Add(nsList);
		}
		else
		{
			// Most recent. May be -1 (none) if DTM was pruned.
			// %OPT% Is there a lastElement() method? Should there be?
			int last = m_namespaceDeclSetElements.size() - 1;

			if (last >= 0 && elementNodeIndex == m_namespaceDeclSetElements.elementAt(last))
			{
				nsList = (SuballocatedIntVector)m_namespaceDeclSets[last];
			}
		}
		if (nsList == null)
		{
			m_namespaceDeclSetElements.addElement(elementNodeIndex);

			SuballocatedIntVector inherited = findNamespaceContext(_parent(elementNodeIndex));

			if (inherited != null)
			{
				// %OPT% Count-down might be faster, but debuggability may
				// be better this way, and if we ever decide we want to
				// keep this ordered by expanded-type...
				int isize = inherited.size();

				// Base the size of a new namespace list on the
				// size of the inherited list - but within reason!
				nsList = new SuballocatedIntVector(Math.Max(Math.Min(isize+16,2048), 32));

				for (int i = 0;i < isize;++i)
				{
					nsList.addElement(inherited.elementAt(i));
				}
			}
			else
			{
				nsList = new SuballocatedIntVector(32);
			}

			m_namespaceDeclSets.Add(nsList);
		}

		// Handle overwriting inherited.
		// %OPT% Keep sorted? (By expanded-name rather than by doc order...)
		// Downside: Would require insertElementAt if not found,
		// which has recopying costs. But these are generally short lists...
		int newEType = _exptype(namespaceNodeIndex);

		for (int i = nsList.size() - 1;i >= 0;--i)
		{
			if (newEType == getExpandedTypeID(nsList.elementAt(i)))
			{
				nsList.setElementAt(makeNodeHandle(namespaceNodeIndex),i);
				return;
			}
		}
		nsList.addElement(makeNodeHandle(namespaceNodeIndex));
	  }

	  /// <summary>
	  /// Retrieve list of namespace declaration locations
	  /// active at this node. List is an SuballocatedIntVector whose
	  /// entries are the namespace node HANDLES declared at that ID.
	  ///   
	  /// %REVIEW% Directly managed arrays rather than vectors?
	  /// %REVIEW% Handles or IDs? Given usage, I think handles.
	  /// 
	  /// </summary>
	  protected internal virtual SuballocatedIntVector findNamespaceContext(int elementNodeIndex)
	  {
		if (null != m_namespaceDeclSetElements)
		{
			// %OPT% Is binary-search really saving us a lot versus linear?
			// (... It may be, in large docs with many NS decls.)
			int wouldBeAt = findInSortedSuballocatedIntVector(m_namespaceDeclSetElements, elementNodeIndex);
			if (wouldBeAt >= 0) // Found it
			{
			  return (SuballocatedIntVector) m_namespaceDeclSets[wouldBeAt];
			}
			if (wouldBeAt == -1) // -1-wouldbeat == 0
			{
			  return null; // Not after anything; definitely not found
			}

			// Not found, but we know where it should have been.
			// Search back until we find an ancestor or run out.
			wouldBeAt = -1 - wouldBeAt;

			// Decrement wouldBeAt to find last possible ancestor
			int candidate = m_namespaceDeclSetElements.elementAt(--wouldBeAt);
			int ancestor = _parent(elementNodeIndex);

			// Special case: if the candidate is before the given node, and
			// is in the earliest possible position in the document, it
			// must have the namespace declarations we're interested in.
			if (wouldBeAt == 0 && candidate < ancestor)
			{
			  int rootHandle = getDocumentRoot(makeNodeHandle(elementNodeIndex));
			  int rootID = makeNodeIdentity(rootHandle);
			  int uppermostNSCandidateID;

			  if (getNodeType(rootHandle) == DTM.DOCUMENT_NODE)
			  {
				int ch = _firstch(rootID);
				uppermostNSCandidateID = (ch != DTM.NULL) ? ch : rootID;
			  }
			  else
			  {
				uppermostNSCandidateID = rootID;
			  }

			  if (candidate == uppermostNSCandidateID)
			  {
				return (SuballocatedIntVector)m_namespaceDeclSets[wouldBeAt];
			  }
			}

			while (wouldBeAt >= 0 && ancestor > 0)
			{
				if (candidate == ancestor)
				{
					// Found ancestor in list
					return (SuballocatedIntVector)m_namespaceDeclSets[wouldBeAt];
				}
				else if (candidate < ancestor)
				{
					// Too deep in tree
					do
					{
					  ancestor = _parent(ancestor);
					} while (candidate < ancestor);
				}
				else if (wouldBeAt > 0)
				{
				  // Too late in list
				  candidate = m_namespaceDeclSetElements.elementAt(--wouldBeAt);
				}
				else
				{
					break;
				}
			}
		}

		return null; // No namespaces known at this node
	  }

	  /// <summary>
	  /// Subroutine: Locate the specified node within
	  /// m_namespaceDeclSetElements, or the last element which
	  /// preceeds it in document order
	  ///   
	  /// %REVIEW% Inlne this into findNamespaceContext? Create SortedSuballocatedIntVector type?
	  /// </summary>
	  /// <returns> If positive or zero, the index of the found item.
	  /// If negative, index of the point at which it would have appeared,
	  /// encoded as -1-index and hence reconvertable by subtracting
	  /// it from -1. (Encoding because I don't want to recompare the strings
	  /// but don't want to burn bytes on a datatype to hold a flagged value.) </returns>
	  protected internal virtual int findInSortedSuballocatedIntVector(SuballocatedIntVector vector, int lookfor)
	  {
		// Binary search
		int i = 0;
		if (vector != null)
		{
		  int first = 0;
		  int last = vector.size() - 1;

		  while (first <= last)
		  {
			i = (first + last) / 2;
			int test = lookfor - vector.elementAt(i);
			if (test == 0)
			{
			  return i; // Name found
			}
			else if (test < 0)
			{
			  last = i - 1; // looked too late
			}
			else
			{
			  first = i + 1; // looked ot early
			}
		  }

		  if (first > i)
		  {
			i = first; // Clean up at loop end
		  }
		}

		return -1 - i; // not-found has to be encoded.
	  }


	  /// <summary>
	  /// Given a node handle, get the index of the node's first child.
	  /// If not yet resolved, waits for more nodes to be added to the document and
	  /// tries again
	  /// </summary>
	  /// <param name="nodeHandle"> handle to node, which should probably be an element
	  ///                   node, but need not be.
	  /// </param>
	  /// <param name="inScope">    true if all namespaces in scope should be returned,
	  ///                   false if only the namespace declarations should be
	  ///                   returned. </param>
	  /// <returns> handle of first namespace, or DTM.NULL to indicate none exists. </returns>
	  public virtual int getFirstNamespaceNode(int nodeHandle, bool inScope)
	  {
			if (inScope)
			{
				int identity = makeNodeIdentity(nodeHandle);
				if (_type(identity) == DTM.ELEMENT_NODE)
				{
				  SuballocatedIntVector nsContext = findNamespaceContext(identity);
				  if (nsContext == null || nsContext.size() < 1)
				  {
					return NULL;
				  }

				  return nsContext.elementAt(0);
				}
				else
				{
				  return NULL;
				}
			}
			else
			{
				// Assume that attributes and namespaces immediately
				// follow the element.
				//
				// %OPT% Would things be faster if all NS nodes were built
				// before all Attr nodes? Some costs at build time for 2nd
				// pass...
				int identity = makeNodeIdentity(nodeHandle);
				if (_type(identity) == DTM.ELEMENT_NODE)
				{
				  while (DTM.NULL != (identity = getNextNodeIdentity(identity)))
				  {
					int type = _type(identity);
					if (type == DTM.NAMESPACE_NODE)
					{
						return makeNodeHandle(identity);
					}
					else if (DTM.ATTRIBUTE_NODE != type)
					{
						break;
					}
				  }
				  return NULL;
				}
				else
				{
				  return NULL;
				}
			}
	  }

	  /// <summary>
	  /// Given a namespace handle, advance to the next namespace.
	  /// </summary>
	  /// <param name="baseHandle"> handle to original node from where the first namespace
	  /// was relative to (needed to return nodes in document order). </param>
	  /// <param name="nodeHandle"> A namespace handle for which we will find the next node. </param>
	  /// <param name="inScope"> true if all namespaces that are in scope should be processed,
	  /// otherwise just process the nodes in the given element handle. </param>
	  /// <returns> handle of next namespace, or DTM.NULL to indicate none exists. </returns>
	  public virtual int getNextNamespaceNode(int baseHandle, int nodeHandle, bool inScope)
	  {
			if (inScope)
			{
				//Since we've been given the base, try direct lookup
				//(could look from nodeHandle but this is at least one
				//comparison/get-parent faster)
				//SuballocatedIntVector nsContext=findNamespaceContext(nodeHandle & m_mask);

					SuballocatedIntVector nsContext = findNamespaceContext(makeNodeIdentity(baseHandle));

				if (nsContext == null)
				{
				  return NULL;
				}
				int i = 1 + nsContext.indexOf(nodeHandle);
				if (i <= 0 || i == nsContext.size())
				{
				  return NULL;
				}

				return nsContext.elementAt(i);
			}
			else
			{
				// Assume that attributes and namespace nodes immediately follow the element.
				int identity = makeNodeIdentity(nodeHandle);
				while (DTM.NULL != (identity = getNextNodeIdentity(identity)))
				{
					int type = _type(identity);
					if (type == DTM.NAMESPACE_NODE)
					{
						return makeNodeHandle(identity);
					}
					else if (type != DTM.ATTRIBUTE_NODE)
					{
						break;
					}
				}
			}
		 return DTM.NULL;
	  }

	  /// <summary>
	  /// Given a node handle, find its parent node.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node-number of parent,
	  /// or DTM.NULL to indicate none exists. </returns>
	  public virtual int getParent(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);

		if (identity > 0)
		{
		  return makeNodeHandle(_parent(identity));
		}
		else
		{
		  return DTM.NULL;
		}
	  }

	  /// <summary>
	  /// Find the Document node handle for the document currently under construction.
	  /// PLEASE NOTE that most people should use getOwnerDocument(nodeHandle) instead;
	  /// this version of the operation is primarily intended for use during negotiation
	  /// with the DTM Manager.
	  /// </summary>
	  ///  <returns> int Node handle of document, which should always be valid. </returns>
	  public virtual int Document
	  {
		  get
		  {
			return m_dtmIdent.elementAt(0); // makeNodeHandle(0)
		  }
	  }

	  /// <summary>
	  /// Given a node handle, find the owning document node.  This has the exact
	  /// same semantics as the DOM Document method of the same name, in that if
	  /// the nodeHandle is a document node, it will return NULL.
	  /// 
	  /// <para>%REVIEW% Since this is DOM-specific, it may belong at the DOM
	  /// binding layer. Included here as a convenience function and to
	  /// aid porting of DOM code to DTM.</para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node handle of owning document, or -1 if the node was a Docment </returns>
	  public virtual int getOwnerDocument(int nodeHandle)
	  {

		if (DTM.DOCUMENT_NODE == getNodeType(nodeHandle))
		{
			  return DTM.NULL;
		}

		return getDocumentRoot(nodeHandle);
	  }

	  /// <summary>
	  /// Given a node handle, find the owning document node.  Unlike the DOM,
	  /// this considers the owningDocument of a Document to be itself.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node handle of owning document, or the nodeHandle if it is
	  ///             a Document. </returns>
	  public virtual int getDocumentRoot(int nodeHandle)
	  {
		return Manager.getDTM(nodeHandle).Document;
	  }

	  /// <summary>
	  /// Get the string-value of a node as a String object
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  public abstract XMLString getStringValue(int nodeHandle);

	  /// <summary>
	  /// Get number of character array chunks in
	  /// the string-value of a node.
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// Note that a single text node may have multiple text chunks.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> number of character array chunks in
	  ///         the string-value of a node. </returns>
	  public virtual int getStringValueChunkCount(int nodeHandle)
	  {

		// %TBD%
		error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //("getStringValueChunkCount not yet supported!");

		return 0;
	  }

	  /// <summary>
	  /// Get a character array chunk in the string-value of a node.
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// Note that a single text node may have multiple text chunks.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="chunkIndex"> Which chunk to get. </param>
	  /// <param name="startAndLen"> An array of 2 where the start position and length of
	  ///                    the chunk will be returned.
	  /// </param>
	  /// <returns> The character array reference where the chunk occurs. </returns>
	  public virtual char[] getStringValueChunk(int nodeHandle, int chunkIndex, int[] startAndLen)
	  {

		// %TBD%
		error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"getStringValueChunk not yet supported!");

		return null;
	  }

	  /// <summary>
	  /// Given a node handle, return an ID that represents the node's expanded name.
	  /// </summary>
	  /// <param name="nodeHandle"> The handle to the node in question.
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  public virtual int getExpandedTypeID(int nodeHandle)
	  {
		// %REVIEW% This _should_ only be null if someone asked the wrong DTM about the node...
		// which one would hope would never happen...
		int id = makeNodeIdentity(nodeHandle);
		if (id == NULL)
		{
		  return NULL;
		}
		return _exptype(id);
	  }

	  /// <summary>
	  /// Given an expanded name, return an ID.  If the expanded-name does not
	  /// exist in the internal tables, the entry will be created, and the ID will
	  /// be returned.  Any additional nodes that are created that have this
	  /// expanded name will use this ID.
	  /// </summary>
	  /// <param name="type"> The simple type, i.e. one of ELEMENT, ATTRIBUTE, etc.
	  /// </param>
	  /// <param name="namespace"> The namespace URI, which may be null, may be an empty
	  ///                  string (which will be the same as null), or may be a
	  ///                  namespace URI. </param>
	  /// <param name="localName"> The local name string, which must be a valid
	  ///                  <a href="http://www.w3.org/TR/REC-xml-names/">NCName</a>.
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  public virtual int getExpandedTypeID(string @namespace, string localName, int type)
	  {

		ExpandedNameTable ent = m_expandedNameTable;

		return ent.getExpandedTypeID(@namespace, localName, type);
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the local name part.
	  /// </summary>
	  /// <param name="expandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String Local name of this node. </returns>
	  public virtual string getLocalNameFromExpandedNameID(int expandedNameID)
	  {
		return m_expandedNameTable.getLocalName(expandedNameID);
	  }

	  /// <summary>
	  /// Given an expanded-name ID, return the namespace URI part.
	  /// </summary>
	  /// <param name="expandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  public virtual string getNamespaceFromExpandedNameID(int expandedNameID)
	  {
		return m_expandedNameTable.getNamespace(expandedNameID);
	  }

	  /// <summary>
	  /// Returns the namespace type of a specific node </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> the ID of the namespace. </returns>
	  public virtual int getNamespaceType(in int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int expandedNameID = _exptype(identity);

		return m_expandedNameTable.getNamespaceID(expandedNameID);
	  }

	  /// <summary>
	  /// Given a node handle, return its DOM-style node name. This will
	  /// include names such as #text or #document.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string.
	  /// %REVIEW% Document when empty string is possible...
	  /// %REVIEW-COMMENT% It should never be empty, should it? </returns>
	  public abstract string getNodeName(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return the XPath node name.  This should be
	  /// the name as described by the XPath data model, NOT the DOM-style
	  /// name.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string. </returns>
	  public virtual string getNodeNameX(int nodeHandle)
	  {

		/// <summary>
		/// @todo: implement this org.apache.xml.dtm.DTMDefaultBase abstract method </summary>
		error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"Not yet supported!");

		return null;
	  }

	  /// <summary>
	  /// Given a node handle, return its XPath-style localname.
	  /// (As defined in Namespaces, this is the portion of the name after any
	  /// colon character).
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Local name of this node. </returns>
	  public abstract string getLocalName(int nodeHandle);

	  /// <summary>
	  /// Given a namespace handle, return the prefix that the namespace decl is
	  /// mapping.
	  /// Given a node handle, return the prefix used to map to the namespace.
	  /// 
	  /// <para> %REVIEW% Are you sure you want "" for no prefix?  </para>
	  /// <para> %REVIEW-COMMENT% I think so... not totally sure. -sb  </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String prefix of this node's name, or "" if no explicit
	  /// namespace prefix was given. </returns>
	  public abstract string getPrefix(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its DOM-style namespace URI
	  /// (As defined in Namespaces, this is the declared URI which this node's
	  /// prefix -- or default in lieu thereof -- was mapped to.)
	  /// 
	  /// <para>%REVIEW% Null or ""? -sb</para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  public abstract string getNamespaceURI(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its node value. This is mostly
	  /// as defined by the DOM, but may ignore some conveniences.
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> String Value of this node, or null if not
	  /// meaningful for this node type. </returns>
	  public abstract string getNodeValue(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its DOM-style node type.
	  /// <para>
	  /// %REVIEW% Generally, returning short is false economy. Return int?
	  /// %REVIEW% Make assumption that node has already arrived.  Is OK?
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> int Node type, as per the DOM's Node._NODE constants. </returns>
	  public virtual short getNodeType(int nodeHandle)
	  {
		  if (nodeHandle == DTM.NULL)
		  {
		  return DTM.NULL;
		  }
		return m_expandedNameTable.getType(_exptype(makeNodeIdentity(nodeHandle)));
	  }

	  /// <summary>
	  /// Get the depth level of this node in the tree (equals 1 for
	  /// a parentless node).
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> the number of ancestors, plus one
	  /// @xsl.usage internal </returns>
	  public virtual short getLevel(int nodeHandle)
	  {
		// Apparently, the axis walker stuff requires levels to count from 1.
		int identity = makeNodeIdentity(nodeHandle);
		return (short)(_level(identity) + 1);
	  }

	  /// <summary>
	  /// Get the identity of this node in the tree 
	  /// </summary>
	  /// <param name="nodeHandle"> The node handle. </param>
	  /// <returns> the node identity
	  /// @xsl.usage internal </returns>
	  public virtual int getNodeIdent(int nodeHandle)
	  {
		/*if (nodeHandle != DTM.NULL)
		  return nodeHandle & m_mask;
		else 
		  return DTM.NULL;*/

		  return makeNodeIdentity(nodeHandle);
	  }

	  /// <summary>
	  /// Get the handle of this node in the tree 
	  /// </summary>
	  /// <param name="nodeId"> The node identity. </param>
	  /// <returns> the node handle
	  /// @xsl.usage internal </returns>
	  public virtual int getNodeHandle(int nodeId)
	  {
		/*if (nodeId != DTM.NULL)
		  return nodeId | m_dtmIdent;
		else 
		  return DTM.NULL;*/

		  return makeNodeHandle(nodeId);
	  }

	  // ============== Document query functions ==============

	  /// <summary>
	  /// Tests whether DTM DOM implementation implements a specific feature and
	  /// that feature is supported by this node.
	  /// </summary>
	  /// <param name="feature"> The name of the feature to test. </param>
	  /// <param name="version"> This is the version number of the feature to test.
	  ///   If the version is not
	  ///   specified, supporting any version of the feature will cause the
	  ///   method to return <code>true</code>. </param>
	  /// <returns> Returns <code>true</code> if the specified feature is
	  ///   supported on this node, <code>false</code> otherwise. </returns>
	  public virtual bool isSupported(string feature, string version)
	  {

		// %TBD%
		return false;
	  }

	  /// <summary>
	  /// Return the base URI of the document entity. If it is not known
	  /// (because the document was parsed from a socket connection or from
	  /// standard input, for example), the value of this property is unknown.
	  /// </summary>
	  /// <returns> the document base URI String object or null if unknown. </returns>
	  public virtual string DocumentBaseURI
	  {
		  get
		  {
			return m_documentBaseURI;
		  }
		  set
		  {
			m_documentBaseURI = value;
		  }
	  }


	  /// <summary>
	  /// Return the system identifier of the document entity. If
	  /// it is not known, the value of this property is unknown.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the system identifier String object or null if unknown. </returns>
	  public virtual string getDocumentSystemIdentifier(int nodeHandle)
	  {

		// %REVIEW%  OK? -sb
		return m_documentBaseURI;
	  }

	  /// <summary>
	  /// Return the name of the character encoding scheme
	  ///        in which the document entity is expressed.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the document encoding String object.
	  /// @xsl.usage internal </returns>
	  public virtual string getDocumentEncoding(int nodeHandle)
	  {

		// %REVIEW%  OK??  -sb
		return "UTF-8";
	  }

	  /// <summary>
	  /// Return an indication of the standalone status of the document,
	  ///        either "yes" or "no". This property is derived from the optional
	  ///        standalone document declaration in the XML declaration at the
	  ///        beginning of the document entity, and has no value if there is no
	  ///        standalone document declaration.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the document standalone String object, either "yes", "no", or null. </returns>
	  public virtual string getDocumentStandalone(int nodeHandle)
	  {
		return null;
	  }

	  /// <summary>
	  /// Return a string representing the XML version of the document. This
	  /// property is derived from the XML declaration optionally present at the
	  /// beginning of the document entity, and has no value if there is no XML
	  /// declaration.
	  /// </summary>
	  /// <param name="documentHandle"> The document handle
	  /// </param>
	  /// <returns> the document version String object. </returns>
	  public virtual string getDocumentVersion(int documentHandle)
	  {
		return null;
	  }

	  /// <summary>
	  /// Return an indication of
	  /// whether the processor has read the complete DTD. Its value is a
	  /// boolean. If it is false, then certain properties (indicated in their
	  /// descriptions below) may be unknown. If it is true, those properties
	  /// are never unknown.
	  /// </summary>
	  /// <returns> <code>true</code> if all declarations were processed;
	  ///         <code>false</code> otherwise. </returns>
	  public virtual bool DocumentAllDeclarationsProcessed
	  {
		  get
		  {
    
			// %REVIEW% OK?
			return true;
		  }
	  }

	  /// <summary>
	  ///   A document type declaration information item has the following properties:
	  /// 
	  ///     1. [system identifier] The system identifier of the external subset, if
	  ///        it exists. Otherwise this property has no value.
	  /// </summary>
	  /// <returns> the system identifier String object, or null if there is none. </returns>
	  public abstract string DocumentTypeDeclarationSystemIdentifier {get;}

	  /// <summary>
	  /// Return the public identifier of the external subset,
	  /// normalized as described in 4.2.2 External Entities [XML]. If there is
	  /// no external subset or if it has no public identifier, this property
	  /// has no value.
	  /// </summary>
	  /// <returns> the public identifier String object, or null if there is none. </returns>
	  public abstract string DocumentTypeDeclarationPublicIdentifier {get;}

	  /// <summary>
	  /// Returns the <code>Element</code> whose <code>ID</code> is given by
	  /// <code>elementId</code>. If no such element exists, returns
	  /// <code>DTM.NULL</code>. Behavior is not defined if more than one element
	  /// has this <code>ID</code>. Attributes (including those
	  /// with the name "ID") are not of type ID unless so defined by DTD/Schema
	  /// information available to the DTM implementation.
	  /// Implementations that do not know whether attributes are of type ID or
	  /// not are expected to return <code>DTM.NULL</code>.
	  /// 
	  /// <para>%REVIEW% Presumably IDs are still scoped to a single document,
	  /// and this operation searches only within a single document, right?
	  /// Wouldn't want collisions between DTMs in the same process.</para>
	  /// </summary>
	  /// <param name="elementId"> The unique <code>id</code> value for an element. </param>
	  /// <returns> The handle of the matching element. </returns>
	  public abstract int getElementById(string elementId);

	  /// <summary>
	  /// The getUnparsedEntityURI function returns the URI of the unparsed
	  /// entity with the specified name in the same document as the context
	  /// node (see [3.3 Unparsed Entities]). It returns the empty string if
	  /// there is no such entity.
	  /// <para>
	  /// XML processors may choose to use the System Identifier (if one
	  /// is provided) to resolve the entity, rather than the URI in the
	  /// Public Identifier. The details are dependent on the processor, and
	  /// we would have to support some form of plug-in resolver to handle
	  /// this properly. Currently, we simply return the System Identifier if
	  /// present, and hope that it a usable URI or that our caller can
	  /// map it to one.
	  /// TODO: Resolve Public Identifiers... or consider changing function name.
	  /// </para>
	  /// <para>
	  /// If we find a relative URI
	  /// reference, XML expects it to be resolved in terms of the base URI
	  /// of the document. The DOM doesn't do that for us, and it isn't
	  /// entirely clear whether that should be done here; currently that's
	  /// pushed up to a higher level of our application. (Note that DOM Level
	  /// 1 didn't store the document's base URI.)
	  /// TODO: Consider resolving Relative URIs.
	  /// </para>
	  /// <para>
	  /// (The DOM's statement that "An XML processor may choose to
	  /// completely expand entities before the structure model is passed
	  /// to the DOM" refers only to parsed entities, not unparsed, and hence
	  /// doesn't affect this function.)
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="name"> A string containing the Entity Name of the unparsed
	  /// entity.
	  /// </param>
	  /// <returns> String containing the URI of the Unparsed Entity, or an
	  /// empty string if no such entity exists. </returns>
	  public abstract string getUnparsedEntityURI(string name);

	  // ============== Boolean methods ================

	  /// <summary>
	  /// Return true if the xsl:strip-space or xsl:preserve-space was processed
	  /// during construction of the DTM document.
	  /// </summary>
	  /// <returns> true if this DTM supports prestripping. </returns>
	  public virtual bool supportsPreStripping()
	  {
		return true;
	  }

	  /// <summary>
	  /// Figure out whether nodeHandle2 should be considered as being later
	  /// in the document than nodeHandle1, in Document Order as defined
	  /// by the XPath model. This may not agree with the ordering defined
	  /// by other XML applications.
	  /// <para>
	  /// There are some cases where ordering isn't defined, and neither are
	  /// the results of this function -- though we'll generally return false.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle1"> Node handle to perform position comparison on. </param>
	  /// <param name="nodeHandle2"> Second Node handle to perform position comparison on .
	  /// </param>
	  /// <returns> true if node1 comes before node2, otherwise return false.
	  /// You can think of this as
	  /// <code>(node1.documentOrderPosition &lt;= node2.documentOrderPosition)</code>. </returns>
	  public virtual bool isNodeAfter(int nodeHandle1, int nodeHandle2)
	  {
			// These return NULL if the node doesn't belong to this document.
		int index1 = makeNodeIdentity(nodeHandle1);
		int index2 = makeNodeIdentity(nodeHandle2);

		return index1 != NULL && index2 != NULL && index1 <= index2;
	  }

	  /// <summary>
	  ///     2. [element content whitespace] A boolean indicating whether the
	  ///        character is white space appearing within element content (see [XML],
	  ///        2.10 "White Space Handling"). Note that validating XML processors are
	  ///        required by XML 1.0 to provide this information. If there is no
	  ///        declaration for the containing element, this property has no value for
	  ///        white space characters. If no declaration has been read, but the [all
	  ///        declarations processed] property of the document information item is
	  ///        false (so there may be an unread declaration), then the value of this
	  ///        property is unknown for white space characters. It is always false for
	  ///        characters that are not white space.
	  /// </summary>
	  /// <param name="nodeHandle"> the node ID. </param>
	  /// <returns> <code>true</code> if the character data is whitespace;
	  ///         <code>false</code> otherwise. </returns>
	  public virtual bool isCharacterElementContentWhitespace(int nodeHandle)
	  {

		// %TBD%
		return false;
	  }

	  /// <summary>
	  ///    10. [all declarations processed] This property is not strictly speaking
	  ///        part of the infoset of the document. Rather it is an indication of
	  ///        whether the processor has read the complete DTD. Its value is a
	  ///        boolean. If it is false, then certain properties (indicated in their
	  ///        descriptions below) may be unknown. If it is true, those properties
	  ///        are never unknown.
	  /// </summary>
	  /// <param name="documentHandle"> A node handle that must identify a document. </param>
	  /// <returns> <code>true</code> if all declarations were processed;
	  ///         <code>false</code> otherwise. </returns>
	  public virtual bool isDocumentAllDeclarationsProcessed(int documentHandle)
	  {
		return true;
	  }

	  /// <summary>
	  ///     5. [specified] A flag indicating whether this attribute was actually
	  ///        specified in the start-tag of its element, or was defaulted from the
	  ///        DTD.
	  /// </summary>
	  /// <param name="attributeHandle"> The attribute handle in question.
	  /// </param>
	  /// <returns> <code>true</code> if the attribute was specified;
	  ///         <code>false</code> if it was defaulted. </returns>
	  public abstract bool isAttributeSpecified(int attributeHandle);

	  // ========== Direct SAX Dispatch, for optimization purposes ========

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value of the given node (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value). Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler. </param>
	  /// <param name="normalize"> true if the content should be normalized according to
	  /// the rules for the XPath
	  /// <a href="http://www.w3.org/TR/xpath#function-normalize-space">normalize-space</a>
	  /// function.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize) throws org.xml.sax.SAXException;
	  public abstract void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, bool normalize);

	  /// <summary>
	  /// Directly create SAX parser events from a subtree.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException;
	  public abstract void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch);

	  /// <summary>
	  /// Return an DOM node for the given node.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A node representation of the DTM node. </returns>
	  public virtual org.w3c.dom.Node getNode(int nodeHandle)
	  {
		return new DTMNodeProxy(this, nodeHandle);
	  }

	  // ==== Construction methods (may not be supported by some implementations!) =====

	  /// <summary>
	  /// Append a child to the end of the document. Please note that the node
	  /// is always cloned if it is owned by another document.
	  /// 
	  /// <para>%REVIEW% "End of the document" needs to be defined more clearly.
	  /// Does it become the last child of the Document? Of the root element?</para>
	  /// </summary>
	  /// <param name="newChild"> Must be a valid new node handle. </param>
	  /// <param name="clone"> true if the child should be cloned into the document. </param>
	  /// <param name="cloneDepth"> if the clone argument is true, specifies that the
	  ///                   clone should include all it's children. </param>
	  public virtual void appendChild(int newChild, bool clone, bool cloneDepth)
	  {
		error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"appendChild not yet supported!");
	  }

	  /// <summary>
	  /// Append a text node child that will be constructed from a string,
	  /// to the end of the document.
	  /// 
	  /// <para>%REVIEW% "End of the document" needs to be defined more clearly.
	  /// Does it become the last child of the Document? Of the root element?</para>
	  /// </summary>
	  /// <param name="str"> Non-null reverence to a string. </param>
	  public virtual void appendTextChild(string str)
	  {
		error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"appendTextChild not yet supported!");
	  }

	  /// <summary>
	  /// Simple error for asserts and the like.
	  /// </summary>
	  /// <param name="msg"> Error message to report. </param>
	  protected internal virtual void error(string msg)
	  {
		throw new DTMException(msg);
	  }

	  /// <summary>
	  /// Find out whether or not to strip whispace nodes.
	  /// 
	  /// </summary>
	  /// <returns> whether or not to strip whispace nodes. </returns>
	  protected internal virtual bool ShouldStripWhitespace
	  {
		  get
		  {
			return m_shouldStripWS;
		  }
		  set
		  {
    
			m_shouldStripWS = value;
    
			if (null != m_shouldStripWhitespaceStack)
			{
			  m_shouldStripWhitespaceStack.Top = value;
			}
		  }
	  }

	  /// <summary>
	  /// Set whether to strip whitespaces and push in current value of
	  /// m_shouldStripWS in m_shouldStripWhitespaceStack.
	  /// </summary>
	  /// <param name="shouldStrip"> Flag indicating whether to strip whitespace nodes </param>
	  protected internal virtual void pushShouldStripWhitespace(bool shouldStrip)
	  {

		m_shouldStripWS = shouldStrip;

		if (null != m_shouldStripWhitespaceStack)
		{
		  m_shouldStripWhitespaceStack.push(shouldStrip);
		}
	  }

	  /// <summary>
	  /// Set whether to strip whitespaces at this point by popping out
	  /// m_shouldStripWhitespaceStack.
	  /// 
	  /// </summary>
	  protected internal virtual void popShouldStripWhitespace()
	  {
		if (null != m_shouldStripWhitespaceStack)
		{
		  m_shouldStripWS = m_shouldStripWhitespaceStack.popAndTop();
		}
	  }


	  /// <summary>
	  /// A dummy routine to satisify the abstract interface. If the DTM
	  /// implememtation that extends the default base requires notification
	  /// of registration, they can override this method.
	  /// </summary>
	   public virtual void documentRegistration()
	   {
	   }

	  /// <summary>
	  /// A dummy routine to satisify the abstract interface. If the DTM
	  /// implememtation that extends the default base requires notification
	  /// when the document is being released, they can override this method
	  /// </summary>
	   public virtual void documentRelease()
	   {
	   }

	   /// <summary>
	   /// Migrate a DTM built with an old DTMManager to a new DTMManager.
	   /// After the migration, the new DTMManager will treat the DTM as
	   /// one that is built by itself.
	   /// This is used to support DTM sharing between multiple transformations. </summary>
	   /// <param name="mgr"> the DTMManager </param>
	   public virtual void migrateTo(DTMManager mgr)
	   {
		 m_mgr = mgr;
		 if (mgr is DTMManagerDefault)
		 {
		   m_mgrDefault = (DTMManagerDefault)mgr;
		 }
	   }

		 /// <summary>
		 /// Query which DTMManager this DTM is currently being handled by.
		 /// 
		 /// %REVEW% Should this become part of the base DTM API?
		 /// </summary>
		 /// <returns> a DTMManager, or null if this is a "stand-alone" DTM. </returns>
		 public virtual DTMManager Manager
		 {
			 get
			 {
				 return m_mgr;
			 }
		 }

		 /// <summary>
		 /// Query which DTMIDs this DTM is currently using within the DTMManager.
		 /// 
		 /// %REVEW% Should this become part of the base DTM API?
		 /// </summary>
		 /// <returns> an IntVector, or null if this is a "stand-alone" DTM. </returns>
		 public virtual SuballocatedIntVector DTMIDs
		 {
			 get
			 {
				 if (m_mgr == null)
				 {
					 return null;
				 }
				 return m_dtmIdent;
			 }
		 }
	}

}