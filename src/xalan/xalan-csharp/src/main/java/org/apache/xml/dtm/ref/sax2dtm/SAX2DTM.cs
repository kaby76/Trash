using System;
using System.Collections;

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
 * $Id: SAX2DTM.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref.sax2dtm
{

	using org.apache.xml.dtm;
	using org.apache.xml.dtm.@ref;
	using StringVector = org.apache.xml.utils.StringVector;
	using IntVector = org.apache.xml.utils.IntVector;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using IntStack = org.apache.xml.utils.IntStack;
	using SuballocatedIntVector = org.apache.xml.utils.SuballocatedIntVector;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using WrappedRuntimeException = org.apache.xml.utils.WrappedRuntimeException;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using org.xml.sax;
	using org.xml.sax.ext;

	/// <summary>
	/// This class implements a DTM that tends to be optimized more for speed than
	/// for compactness, that is constructed via SAX2 ContentHandler events.
	/// </summary>
	public class SAX2DTM : DTMDefaultBaseIterators, EntityResolver, DTDHandler, ContentHandler, ErrorHandler, DeclHandler, LexicalHandler
	{
	  /// <summary>
	  /// Set true to monitor SAX events and similar diagnostic info. </summary>
	  private const bool DEBUG = false;

	  /// <summary>
	  /// If we're building the model incrementally on demand, we need to
	  /// be able to tell the source when to send us more data.
	  /// 
	  /// Note that if this has not been set, and you attempt to read ahead
	  /// of the current build point, we'll probably throw a null-pointer
	  /// exception. We could try to wait-and-retry instead, as a very poor
	  /// fallback, but that has all the known problems with multithreading
	  /// on multiprocessors and we Don't Want to Go There.
	  /// </summary>
	  /// <seealso cref="setIncrementalSAXSource"/>
	  private IncrementalSAXSource m_incrementalSAXSource = null;

	  /// <summary>
	  /// All the character content, including attribute values, are stored in
	  /// this buffer.
	  /// 
	  /// %REVIEW% Should this have an option of being shared across DTMs?
	  /// Sequentially only; not threadsafe... Currently, I think not.
	  /// 
	  /// %REVIEW% Initial size was pushed way down to reduce weight of RTFs.
	  /// pending reduction in number of RTF DTMs. Now that we're sharing a DTM
	  /// between RTFs, and tail-pruning... consider going back to the larger/faster.
	  /// 
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  //private FastStringBuffer m_chars = new FastStringBuffer(13, 13);
	  protected internal FastStringBuffer m_chars;

	  /// <summary>
	  /// This vector holds offset and length data.
	  /// </summary>
	  protected internal SuballocatedIntVector m_data;

	  /// <summary>
	  /// The parent stack, needed only for construction.
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  [NonSerialized]
	  protected internal IntStack m_parents;

	  /// <summary>
	  /// The current previous node, needed only for construction time.
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_previous = 0;

	  /// <summary>
	  /// Namespace support, only relevent at construction time.
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  [NonSerialized]
	  protected internal ArrayList m_prefixMappings = new ArrayList();

	  /// <summary>
	  /// Namespace support, only relevent at construction time.
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  [NonSerialized]
	  protected internal IntStack m_contextIndexes;

	  /// <summary>
	  /// Type of next characters() event within text block in prgress. </summary>
	  [NonSerialized]
	  protected internal int m_textType = DTM.TEXT_NODE;

	  /// <summary>
	  /// Type of coalesced text block. See logic in the characters()
	  /// method.
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_coalescedTextType = DTM.TEXT_NODE;

	  /// <summary>
	  /// The SAX Document locator </summary>
	  [NonSerialized]
	  protected internal Locator m_locator = null;

	  /// <summary>
	  /// The SAX Document system-id </summary>
	  [NonSerialized]
	  private string m_systemId = null;

	  /// <summary>
	  /// We are inside the DTD.  This is used for ignoring comments. </summary>
	  [NonSerialized]
	  protected internal bool m_insideDTD = false;

	  /// <summary>
	  /// Tree Walker for dispatchToEvents. </summary>
	  protected internal DTMTreeWalker m_walker = new DTMTreeWalker();

	  /// <summary>
	  /// pool of string values that come as strings. </summary>
	  protected internal DTMStringPool m_valuesOrPrefixes;

	  /// <summary>
	  /// End document has been reached.
	  /// Made protected rather than private so SAX2RTFDTM can access it.
	  /// </summary>
	  protected internal bool m_endDocumentOccured = false;

	  /// <summary>
	  /// Data or qualified name values, one array element for each node. </summary>
	  protected internal SuballocatedIntVector m_dataOrQName;

	  /// <summary>
	  /// This table holds the ID string to node associations, for
	  /// XML IDs.
	  /// </summary>
	  protected internal Hashtable m_idAttributes = new Hashtable();

	  /// <summary>
	  /// fixed dom-style names.
	  /// </summary>
	  private static readonly string[] m_fixednames = new string[] {null, null, null, "#text", "#cdata_section", null, null, null, "#comment", "#document", null, "#document-fragment", null};

	  /// <summary>
	  /// Vector of entities.  Each record is composed of four Strings:
	  ///  publicId, systemID, notationName, and name.
	  /// </summary>
	  private ArrayList m_entities = null;

	  /// <summary>
	  /// m_entities public ID offset. </summary>
	  private const int ENTITY_FIELD_PUBLICID = 0;

	  /// <summary>
	  /// m_entities system ID offset. </summary>
	  private const int ENTITY_FIELD_SYSTEMID = 1;

	  /// <summary>
	  /// m_entities notation name offset. </summary>
	  private const int ENTITY_FIELD_NOTATIONNAME = 2;

	  /// <summary>
	  /// m_entities name offset. </summary>
	  private const int ENTITY_FIELD_NAME = 3;

	  /// <summary>
	  /// Number of entries per record for m_entities. </summary>
	  private const int ENTITY_FIELDS_PER = 4;

	  /// <summary>
	  /// The starting offset within m_chars for the text or
	  /// CDATA_SECTION node currently being acumulated,
	  /// or -1 if there is no text node in progress
	  /// </summary>
	  protected internal int m_textPendingStart = -1;

	  /// <summary>
	  /// Describes whether information about document source location
	  /// should be maintained or not.
	  /// 
	  /// Made protected for access by SAX2RTFDTM.
	  /// </summary>
	  protected internal bool m_useSourceLocationProperty = false;

	   /// <summary>
	   /// Made protected for access by SAX2RTFDTM.
	   /// </summary>
	  protected internal StringVector m_sourceSystemId;
	   /// <summary>
	   /// Made protected for access by SAX2RTFDTM.
	   /// </summary>
	  protected internal IntVector m_sourceLine;
	   /// <summary>
	   /// Made protected for access by SAX2RTFDTM.
	   /// </summary>
	  protected internal IntVector m_sourceColumn;

	  /// <summary>
	  /// Construct a SAX2DTM object using the default block size.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> the JAXP 1.1 Source object for this DTM. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> XMLString factory for creating character content. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  public SAX2DTM(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : this(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, DEFAULT_BLOCKSIZE, true, false)
	  {

	  }

	  /// <summary>
	  /// Construct a SAX2DTM object ready to be constructed from SAX2
	  /// ContentHandler events.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> the JAXP 1.1 Source object for this DTM. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> XMLString factory for creating character content. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  /// <param name="blocksize"> The block size of the DTM. </param>
	  /// <param name="usePrevsib"> true if we want to build the previous sibling node array. </param>
	  /// <param name="newNameTable"> true if we want to use a new ExpandedNameTable for this DTM. </param>
	  public SAX2DTM(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool usePrevsib, bool newNameTable) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, blocksize, usePrevsib, newNameTable)
	  {


		// %OPT% Use smaller sizes for all internal storage units when
		// the blocksize is small. This reduces the cost of creating an RTF.
		if (blocksize <= 64)
		{
		  m_data = new SuballocatedIntVector(blocksize, DEFAULT_NUMBLOCKS_SMALL);
		  m_dataOrQName = new SuballocatedIntVector(blocksize, DEFAULT_NUMBLOCKS_SMALL);
		  m_valuesOrPrefixes = new DTMStringPool(16);
		  m_chars = new FastStringBuffer(7, 10);
		  m_contextIndexes = new IntStack(4);
		  m_parents = new IntStack(4);
		}
		else
		{
		  m_data = new SuballocatedIntVector(blocksize, DEFAULT_NUMBLOCKS);
		  m_dataOrQName = new SuballocatedIntVector(blocksize, DEFAULT_NUMBLOCKS);
		  m_valuesOrPrefixes = new DTMStringPool();
		  m_chars = new FastStringBuffer(10, 13);
		  m_contextIndexes = new IntStack();
		  m_parents = new IntStack();
		}

		// %REVIEW%  Initial size pushed way down to reduce weight of RTFs
		// (I'm not entirely sure 0 would work, so I'm playing it safe for now.)
		//m_data = new SuballocatedIntVector(doIndexing ? (1024*2) : 512, 1024);
		//m_data = new SuballocatedIntVector(blocksize);

		m_data.addElement(0); // Need placeholder in case index into here must be <0.

		//m_dataOrQName = new SuballocatedIntVector(blocksize);

		// m_useSourceLocationProperty=org.apache.xalan.processor.TransformerFactoryImpl.m_source_location;
		m_useSourceLocationProperty = mgr.Source_location;
		m_sourceSystemId = (m_useSourceLocationProperty) ? new StringVector() : null;
		 m_sourceLine = (m_useSourceLocationProperty) ? new IntVector() : null;
		m_sourceColumn = (m_useSourceLocationProperty) ? new IntVector() : null;
	  }

	  /// <summary>
	  /// Set whether information about document source location
	  /// should be maintained or not. 
	  /// </summary>
	  public virtual bool UseSourceLocation
	  {
		  set
		  {
			m_useSourceLocationProperty = value;
		  }
	  }

	  /// <summary>
	  /// Get the data or qualified name for the given node identity.
	  /// </summary>
	  /// <param name="identity"> The node identity.
	  /// </param>
	  /// <returns> The data or qualified name, or DTM.NULL. </returns>
	  protected internal virtual int _dataOrQName(int identity)
	  {

		if (identity < m_size)
		{
		  return m_dataOrQName.elementAt(identity);
		}

		// Check to see if the information requested has been processed, and,
		// if not, advance the iterator until we the information has been
		// processed.
		while (true)
		{
		  bool isMore = nextNode();

		  if (!isMore)
		  {
			return NULL;
		  }
		  else if (identity < m_size)
		  {
			return m_dataOrQName.elementAt(identity);
		  }
		}
	  }

	  /// <summary>
	  /// Ask the CoRoutine parser to doTerminate and clear the reference.
	  /// </summary>
	  public virtual void clearCoRoutine()
	  {
		clearCoRoutine(true);
	  }

	  /// <summary>
	  /// Ask the CoRoutine parser to doTerminate and clear the reference. If
	  /// the CoRoutine parser has already been cleared, this will have no effect.
	  /// </summary>
	  /// <param name="callDoTerminate"> true of doTerminate should be called on the
	  /// coRoutine parser. </param>
	  public virtual void clearCoRoutine(bool callDoTerminate)
	  {

		if (null != m_incrementalSAXSource)
		{
		  if (callDoTerminate)
		  {
			m_incrementalSAXSource.deliverMoreNodes(false);
		  }

		  m_incrementalSAXSource = null;
		}
	  }

	  /// <summary>
	  /// Bind a IncrementalSAXSource to this DTM. If we discover we need nodes
	  /// that have not yet been built, we will ask this object to send us more
	  /// events, and it will manage interactions with its data sources.
	  /// 
	  /// Note that we do not actually build the IncrementalSAXSource, since we don't
	  /// know what source it's reading from, what thread that source will run in,
	  /// or when it will run.
	  /// </summary>
	  /// <param name="incrementalSAXSource"> The parser that we want to recieve events from
	  /// on demand. </param>
	  public virtual IncrementalSAXSource IncrementalSAXSource
	  {
		  set
		  {
    
			// Establish coroutine link so we can request more data
			//
			// Note: It's possible that some versions of IncrementalSAXSource may
			// not actually use a CoroutineManager, and hence may not require
			// that we obtain an Application Coroutine ID. (This relies on the
			// coroutine transaction details having been encapsulated in the
			// IncrementalSAXSource.do...() methods.)
			m_incrementalSAXSource = value;
    
			// Establish SAX-stream link so we can receive the requested data
			value.ContentHandler = this;
			value.LexicalHandler = this;
			value.DTDHandler = this;
    
			// Are the following really needed? value doesn't yet
			// support them, and they're mostly no-ops here...
			//value.setErrorHandler(this);
			//value.setDeclHandler(this);
		  }
	  }

	  /// <summary>
	  /// getContentHandler returns "our SAX builder" -- the thing that
	  /// someone else should send SAX events to in order to extend this
	  /// DTM model.
	  /// 
	  /// %REVIEW% Should this return null if constrution already done/begun?
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX events,
	  /// "this" if the DTM object has a built-in SAX ContentHandler,
	  /// the IncrementalSAXSource if we're bound to one and should receive
	  /// the SAX stream via it for incremental build purposes... </returns>
	  public override ContentHandler ContentHandler
	  {
		  get
		  {
    
			if (m_incrementalSAXSource is IncrementalSAXSource_Filter)
			{
			  return (ContentHandler) m_incrementalSAXSource;
			}
			else
			{
			  return this;
			}
		  }
	  }

	  /// <summary>
	  /// Return this DTM's lexical handler.
	  /// 
	  /// %REVIEW% Should this return null if constrution already done/begun?
	  /// </summary>
	  /// <returns> null if this model doesn't respond to lexical SAX events,
	  /// "this" if the DTM object has a built-in SAX ContentHandler,
	  /// the IncrementalSAXSource if we're bound to one and should receive
	  /// the SAX stream via it for incremental build purposes... </returns>
	  public override LexicalHandler LexicalHandler
	  {
		  get
		  {
    
			if (m_incrementalSAXSource is IncrementalSAXSource_Filter)
			{
			  return (LexicalHandler) m_incrementalSAXSource;
			}
			else
			{
			  return this;
			}
		  }
	  }

	  /// <summary>
	  /// Return this DTM's EntityResolver.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX entity ref events. </returns>
	  public override EntityResolver EntityResolver
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's DTDHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX dtd events. </returns>
	  public override DTDHandler DTDHandler
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's ErrorHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX error events. </returns>
	  public override ErrorHandler ErrorHandler
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's DeclHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX Decl events. </returns>
	  public override DeclHandler DeclHandler
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <returns> true iff we're building this model incrementally (eg
	  /// we're partnered with a IncrementalSAXSource) and thus require that the
	  /// transformation and the parse run simultaneously. Guidance to the
	  /// DTMManager. </returns>
	  public override bool needsTwoThreads()
	  {
		return null != m_incrementalSAXSource;
	  }

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
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, boolean normalize) throws SAXException
	  public override void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, bool normalize)
	  {

		int identity = makeNodeIdentity(nodeHandle);

		if (identity == DTM.NULL)
		{
		  return;
		}

		int type = _type(identity);

		if (isTextType(type))
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);
		  int offset = m_data.elementAt(dataIndex);
		  int length = m_data.elementAt(dataIndex + 1);

		  if (normalize)
		  {
			m_chars.sendNormalizedSAXcharacters(ch, offset, length);
		  }
		  else
		  {
			m_chars.sendSAXcharacters(ch, offset, length);
		  }
		}
		else
		{
		  int firstChild = _firstch(identity);

		  if (DTM.NULL != firstChild)
		  {
			int offset = -1;
			int length = 0;
			int startNode = identity;

			identity = firstChild;

			do
			{
			  type = _type(identity);

			  if (isTextType(type))
			  {
				int dataIndex = _dataOrQName(identity);

				if (-1 == offset)
				{
				  offset = m_data.elementAt(dataIndex);
				}

				length += m_data.elementAt(dataIndex + 1);
			  }

			  identity = getNextNodeIdentity(identity);
			} while (DTM.NULL != identity && (_parent(identity) >= startNode));

			if (length > 0)
			{
			  if (normalize)
			  {
				m_chars.sendNormalizedSAXcharacters(ch, offset, length);
			  }
			  else
			  {
				m_chars.sendSAXcharacters(ch, offset, length);
			  }
			}
		  }
		  else if (type != DTM.ELEMENT_NODE)
		  {
			int dataIndex = _dataOrQName(identity);

			if (dataIndex < 0)
			{
			  dataIndex = -dataIndex;
			  dataIndex = m_data.elementAt(dataIndex + 1);
			}

			string str = m_valuesOrPrefixes.indexToString(dataIndex);

			  if (normalize)
			  {
				FastStringBuffer.sendNormalizedSAXcharacters(str.ToCharArray(), 0, str.Length, ch);
			  }
			  else
			  {
				ch.characters(str.ToCharArray(), 0, str.Length);
			  }
		  }
		}
	  }


	  /// <summary>
	  /// Given a node handle, return its DOM-style node name. This will
	  /// include names such as #text or #document.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string.
	  /// %REVIEW% Document when empty string is possible...
	  /// %REVIEW-COMMENT% It should never be empty, should it? </returns>
	  public override string getNodeName(int nodeHandle)
	  {

		int expandedTypeID = getExpandedTypeID(nodeHandle);
		// If just testing nonzero, no need to shift...
		int namespaceID = m_expandedNameTable.getNamespaceID(expandedTypeID);

		if (0 == namespaceID)
		{
		  // Don't retrieve name until/unless needed
		  // String name = m_expandedNameTable.getLocalName(expandedTypeID);
		  int type = getNodeType(nodeHandle);

		  if (type == DTM.NAMESPACE_NODE)
		  {
			if (null == m_expandedNameTable.getLocalName(expandedTypeID))
			{
			  return "xmlns";
			}
			else
			{
			  return "xmlns:" + m_expandedNameTable.getLocalName(expandedTypeID);
			}
		  }
		  else if (0 == m_expandedNameTable.getLocalNameID(expandedTypeID))
		  {
			return m_fixednames[type];
		  }
		  else
		  {
			return m_expandedNameTable.getLocalName(expandedTypeID);
		  }
		}
		else
		{
		  int qnameIndex = m_dataOrQName.elementAt(makeNodeIdentity(nodeHandle));

		  if (qnameIndex < 0)
		  {
			qnameIndex = -qnameIndex;
			qnameIndex = m_data.elementAt(qnameIndex);
		  }

		  return m_valuesOrPrefixes.indexToString(qnameIndex);
		}
	  }

	  /// <summary>
	  /// Given a node handle, return the XPath node name.  This should be
	  /// the name as described by the XPath data model, NOT the DOM-style
	  /// name.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string. </returns>
	  public override string getNodeNameX(int nodeHandle)
	  {

		int expandedTypeID = getExpandedTypeID(nodeHandle);
		int namespaceID = m_expandedNameTable.getNamespaceID(expandedTypeID);

		if (0 == namespaceID)
		{
		  string name = m_expandedNameTable.getLocalName(expandedTypeID);

		  if (string.ReferenceEquals(name, null))
		  {
			return "";
		  }
		  else
		  {
			return name;
		  }
		}
		else
		{
		  int qnameIndex = m_dataOrQName.elementAt(makeNodeIdentity(nodeHandle));

		  if (qnameIndex < 0)
		  {
			qnameIndex = -qnameIndex;
			qnameIndex = m_data.elementAt(qnameIndex);
		  }

		  return m_valuesOrPrefixes.indexToString(qnameIndex);
		}
	  }

	  /// <summary>
	  ///     5. [specified] A flag indicating whether this attribute was actually
	  ///        specified in the start-tag of its element, or was defaulted from the
	  ///        DTD.
	  /// </summary>
	  /// <param name="attributeHandle"> Must be a valid handle to an attribute node. </param>
	  /// <returns> <code>true</code> if the attribute was specified;
	  ///         <code>false</code> if it was defaulted. </returns>
	  public override bool isAttributeSpecified(int attributeHandle)
	  {

		// I'm not sure if I want to do anything with this...
		return true; // ??
	  }

	  /// <summary>
	  ///   A document type declaration information item has the following properties:
	  /// 
	  ///     1. [system identifier] The system identifier of the external subset, if
	  ///        it exists. Otherwise this property has no value.
	  /// </summary>
	  /// <returns> the system identifier String object, or null if there is none. </returns>
	  public override string DocumentTypeDeclarationSystemIdentifier
	  {
		  get
		  {
    
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMDefaultBase abstract method </summary>
			error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"Not yet supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Get the next node identity value in the list, and call the iterator
	  /// if it hasn't been added yet.
	  /// </summary>
	  /// <param name="identity"> The node identity (index). </param>
	  /// <returns> identity+1, or DTM.NULL. </returns>
	  protected internal override int getNextNodeIdentity(int identity)
	  {

		identity += 1;

		while (identity >= m_size)
		{
		  if (null == m_incrementalSAXSource)
		  {
			return DTM.NULL;
		  }

		  nextNode();
		}

		return identity;
	  }

	  /// <summary>
	  /// Directly create SAX parser events from a subtree.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public override void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch)
	  {

		DTMTreeWalker treeWalker = m_walker;
		ContentHandler prevCH = treeWalker.getcontentHandler();

		if (null != prevCH)
		{
		  treeWalker = new DTMTreeWalker();
		}

		treeWalker.setcontentHandler(ch);
		treeWalker.DTM = this;

		try
		{
		  treeWalker.traverse(nodeHandle);
		}
		finally
		{
		  treeWalker.setcontentHandler(null);
		}
	  }

	  /// <summary>
	  /// Get the number of nodes that have been added.
	  /// </summary>
	  /// <returns> The number of that are currently in the tree. </returns>
	  public override int NumberOfNodes
	  {
		  get
		  {
			return m_size;
		  }
	  }

	  /// <summary>
	  /// This method should try and build one or more nodes in the table.
	  /// </summary>
	  /// <returns> The true if a next node is found or false if
	  ///         there are no more nodes. </returns>
	  protected internal override bool nextNode()
	  {

		if (null == m_incrementalSAXSource)
		{
		  return false;
		}

		if (m_endDocumentOccured)
		{
		  clearCoRoutine();

		  return false;
		}

		object gotMore = m_incrementalSAXSource.deliverMoreNodes(true);

		// gotMore may be a Boolean (TRUE if still parsing, FALSE if
		// EOF) or an exception if IncrementalSAXSource malfunctioned
		// (code error rather than user error).
		//
		// %REVIEW% Currently the ErrorHandlers sketched herein are
		// no-ops, so I'm going to initially leave this also as a
		// no-op.
		if (!(gotMore is Boolean))
		{
		  if (gotMore is Exception)
		  {
			throw (RuntimeException)gotMore;
		  }
		  else if (gotMore is Exception)
		  {
			throw new WrappedRuntimeException((Exception)gotMore);
		  }
		  // for now...
		  clearCoRoutine();

		  return false;

		  // %TBD%
		}

		if (gotMore != true)
		{

		  // EOF reached without satisfying the request
		  clearCoRoutine(); // Drop connection, stop trying

		  // %TBD% deregister as its listener?
		}

		return true;
	  }

	  /// <summary>
	  /// Bottleneck determination of text type.
	  /// </summary>
	  /// <param name="type"> oneof DTM.XXX_NODE.
	  /// </param>
	  /// <returns> true if this is a text or cdata section. </returns>
	  private bool isTextType(int type)
	  {
		return (DTM.TEXT_NODE == type || DTM.CDATA_SECTION_NODE == type);
	  }

	//    /**
	//     * Ensure that the size of the information arrays can hold another entry
	//     * at the given index.
	//     *
	//     * @param on exit from this function, the information arrays sizes must be
	//     * at least index+1.
	//     *
	//     * NEEDSDOC @param index
	//     */
	//    protected void ensureSize(int index)
	//    {
	//          // dataOrQName is an SuballocatedIntVector and hence self-sizing.
	//          // But DTMDefaultBase may need fixup.
	//        super.ensureSize(index);
	//    }

	  /// <summary>
	  /// Construct the node map from the node.
	  /// </summary>
	  /// <param name="type"> raw type ID, one of DTM.XXX_NODE. </param>
	  /// <param name="expandedTypeID"> The expended type ID. </param>
	  /// <param name="parentIndex"> The current parent index. </param>
	  /// <param name="previousSibling"> The previous sibling index. </param>
	  /// <param name="dataOrPrefix"> index into m_data table, or string handle. </param>
	  /// <param name="canHaveFirstChild"> true if the node can have a first child, false
	  ///                          if it is atomic.
	  /// </param>
	  /// <returns> The index identity of the node that was added. </returns>
	  protected internal virtual int addNode(int type, int expandedTypeID, int parentIndex, int previousSibling, int dataOrPrefix, bool canHaveFirstChild)
	  {
		// Common to all nodes:
		int nodeIndex = m_size++;

		// Have we overflowed a DTM Identity's addressing range?
		if (m_dtmIdent.size() == ((int)((uint)nodeIndex >> DTMManager.IDENT_DTM_NODE_BITS)))
		{
		  addNewDTMID(nodeIndex);
		}

		m_firstch.addElement(canHaveFirstChild ? NOTPROCESSED : DTM.NULL);
		m_nextsib.addElement(NOTPROCESSED);
		m_parent.addElement(parentIndex);
		m_exptype.addElement(expandedTypeID);
		m_dataOrQName.addElement(dataOrPrefix);

		if (m_prevsib != null)
		{
		  m_prevsib.addElement(previousSibling);
		}

		if (DTM.NULL != previousSibling)
		{
		  m_nextsib.setElementAt(nodeIndex,previousSibling);
		}

		if (m_locator != null && m_useSourceLocationProperty)
		{
		  setSourceLocation();
		}

		// Note that nextSibling is not processed until charactersFlush()
		// is called, to handle successive characters() events.

		// Special handling by type: Declare namespaces, attach first child
		switch (type)
		{
		case DTM.NAMESPACE_NODE:
		  declareNamespaceInContext(parentIndex,nodeIndex);
		  break;
		case DTM.ATTRIBUTE_NODE:
		  break;
		default:
		  if (DTM.NULL == previousSibling && DTM.NULL != parentIndex)
		  {
			m_firstch.setElementAt(nodeIndex,parentIndex);
		  }
		  break;
		}

		return nodeIndex;
	  }

	  /// <summary>
	  /// Get a new DTM ID beginning at the specified node index. </summary>
	  /// <param name="nodeIndex"> The node identity at which the new DTM ID will begin
	  /// addressing. </param>
	  protected internal virtual void addNewDTMID(int nodeIndex)
	  {
		try
		{
		  if (m_mgr == null)
		  {
			throw new System.InvalidCastException();
		  }

								  // Handle as Extended Addressing
		  DTMManagerDefault mgrD = (DTMManagerDefault)m_mgr;
		  int id = mgrD.FirstFreeDTMID;
		  mgrD.addDTM(this,id,nodeIndex);
		  m_dtmIdent.addElement(id << DTMManager.IDENT_DTM_NODE_BITS);
		}
		catch (System.InvalidCastException)
		{
		  // %REVIEW% Wrong error message, but I've been told we're trying
		  // not to add messages right not for I18N reasons.
		  // %REVIEW% Should this be a Fatal Error?
		  error(XMLMessages.createXMLMessage(XMLErrorResources.ER_NO_DTMIDS_AVAIL, null)); //"No more DTM IDs are available";
		}
	  }

	  /// <summary>
	  /// Migrate a DTM built with an old DTMManager to a new DTMManager.
	  /// After the migration, the new DTMManager will treat the DTM as
	  /// one that is built by itself.
	  /// This is used to support DTM sharing between multiple transformations. </summary>
	  /// <param name="manager"> the DTMManager </param>
	  public override void migrateTo(DTMManager manager)
	  {
		base.migrateTo(manager);

		// We have to reset the information in m_dtmIdent and
		// register the DTM with the new manager. 
		int numDTMs = m_dtmIdent.size();
		int dtmId = m_mgrDefault.FirstFreeDTMID;
		int nodeIndex = 0;
		for (int i = 0; i < numDTMs; i++)
		{
		  m_dtmIdent.setElementAt(dtmId << DTMManager.IDENT_DTM_NODE_BITS, i);
		  m_mgrDefault.addDTM(this, dtmId, nodeIndex);
		  dtmId++;
		  nodeIndex += (1 << DTMManager.IDENT_DTM_NODE_BITS);
		}
	  }

	  /// <summary>
	  /// Store the source location of the current node.  This method must be called
	  /// as every node is added to the DTM or for no node.
	  /// </summary>
	  protected internal virtual void setSourceLocation()
	  {
		m_sourceSystemId.addElement(m_locator.getSystemId());
		m_sourceLine.addElement(m_locator.getLineNumber());
		m_sourceColumn.addElement(m_locator.getColumnNumber());

		//%REVIEW% %BUG% Prevent this from arising in the first place
		// by not allowing the enabling conditions to change after we start
		// building the document.
		if (m_sourceSystemId.size() != m_size)
		{
			string msg = "CODING ERROR in Source Location: " + m_size + " != " + m_sourceSystemId.size();
			Console.Error.WriteLine(msg);
			throw new Exception(msg);
		}
	  }

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
	  public override string getNodeValue(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int type = _type(identity);

		if (isTextType(type))
		{
		  int dataIndex = _dataOrQName(identity);
		  int offset = m_data.elementAt(dataIndex);
		  int length = m_data.elementAt(dataIndex + 1);

		  // %OPT% We should cache this, I guess.
		  return m_chars.getString(offset, length);
		}
		else if (DTM.ELEMENT_NODE == type || DTM.DOCUMENT_FRAGMENT_NODE == type || DTM.DOCUMENT_NODE == type)
		{
		  return null;
		}
		else
		{
		  int dataIndex = _dataOrQName(identity);

		  if (dataIndex < 0)
		  {
			dataIndex = -dataIndex;
			dataIndex = m_data.elementAt(dataIndex + 1);
		  }

		  return m_valuesOrPrefixes.indexToString(dataIndex);
		}
	  }

	  /// <summary>
	  /// Given a node handle, return its XPath-style localname.
	  /// (As defined in Namespaces, this is the portion of the name after any
	  /// colon character).
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Local name of this node. </returns>
	  public override string getLocalName(int nodeHandle)
	  {
		return m_expandedNameTable.getLocalName(_exptype(makeNodeIdentity(nodeHandle)));
	  }

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
	  public override string getUnparsedEntityURI(string name)
	  {

		string url = "";

		if (null == m_entities)
		{
		  return url;
		}

		int n = m_entities.Count;

		for (int i = 0; i < n; i += ENTITY_FIELDS_PER)
		{
		  string ename = (string) m_entities[i + ENTITY_FIELD_NAME];

		  if (null != ename && ename.Equals(name))
		  {
			string nname = (string) m_entities[i + ENTITY_FIELD_NOTATIONNAME];

			if (null != nname)
			{

			  // The draft says: "The XSLT processor may use the public
			  // identifier to generate a URI for the entity instead of the URI
			  // specified in the system identifier. If the XSLT processor does
			  // not use the public identifier to generate the URI, it must use
			  // the system identifier; if the system identifier is a relative
			  // URI, it must be resolved into an absolute URI using the URI of
			  // the resource containing the entity declaration as the base
			  // URI [RFC2396]."
			  // So I'm falling a bit short here.
			  url = (string) m_entities[i + ENTITY_FIELD_SYSTEMID];

			  if (null == url)
			  {
				url = (string) m_entities[i + ENTITY_FIELD_PUBLICID];
			  }
			}

			break;
		  }
		}

		return url;
	  }

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
	  public override string getPrefix(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int type = _type(identity);

		if (DTM.ELEMENT_NODE == type)
		{
		  int prefixIndex = _dataOrQName(identity);

		  if (0 == prefixIndex)
		  {
			return "";
		  }
		  else
		  {
			string qname = m_valuesOrPrefixes.indexToString(prefixIndex);

			return getPrefix(qname, null);
		  }
		}
		else if (DTM.ATTRIBUTE_NODE == type)
		{
		  int prefixIndex = _dataOrQName(identity);

		  if (prefixIndex < 0)
		  {
			prefixIndex = m_data.elementAt(-prefixIndex);

			string qname = m_valuesOrPrefixes.indexToString(prefixIndex);

			return getPrefix(qname, null);
		  }
		}

		return "";
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
	  public override int getAttributeNode(int nodeHandle, string namespaceURI, string name)
	  {

		for (int attrH = getFirstAttribute(nodeHandle); DTM.NULL != attrH; attrH = getNextAttribute(attrH))
		{
		  string attrNS = getNamespaceURI(attrH);
		  string attrName = getLocalName(attrH);
		  bool nsMatch = string.ReferenceEquals(namespaceURI, attrNS) || (!string.ReferenceEquals(namespaceURI, null) && namespaceURI.Equals(attrNS));

		  if (nsMatch && name.Equals(attrName))
		  {
			return attrH;
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Return the public identifier of the external subset,
	  /// normalized as described in 4.2.2 External Entities [XML]. If there is
	  /// no external subset or if it has no public identifier, this property
	  /// has no value.
	  /// </summary>
	  /// <returns> the public identifier String object, or null if there is none. </returns>
	  public override string DocumentTypeDeclarationPublicIdentifier
	  {
		  get
		  {
    
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMDefaultBase abstract method </summary>
			error(XMLMessages.createXMLMessage(XMLErrorResources.ER_METHOD_NOT_SUPPORTED, null)); //"Not yet supported!");
    
			return null;
		  }
	  }

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
	  public override string getNamespaceURI(int nodeHandle)
	  {

		return m_expandedNameTable.getNamespace(_exptype(makeNodeIdentity(nodeHandle)));
	  }

	  /// <summary>
	  /// Get the string-value of a node as a String object
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  public override XMLString getStringValue(int nodeHandle)
	  {
		int identity = makeNodeIdentity(nodeHandle);
		int type;
		if (identity == DTM.NULL) // Separate lines because I wanted to breakpoint it
		{
		  type = DTM.NULL;
		}
		else
		{
		  type = _type(identity);
		}

		if (isTextType(type))
		{
		  int dataIndex = _dataOrQName(identity);
		  int offset = m_data.elementAt(dataIndex);
		  int length = m_data.elementAt(dataIndex + 1);

		  return m_xstrf.newstr(m_chars, offset, length);
		}
		else
		{
		  int firstChild = _firstch(identity);

		  if (DTM.NULL != firstChild)
		  {
			int offset = -1;
			int length = 0;
			int startNode = identity;

			identity = firstChild;

			do
			{
			  type = _type(identity);

			  if (isTextType(type))
			  {
				int dataIndex = _dataOrQName(identity);

				if (-1 == offset)
				{
				  offset = m_data.elementAt(dataIndex);
				}

				length += m_data.elementAt(dataIndex + 1);
			  }

			  identity = getNextNodeIdentity(identity);
			} while (DTM.NULL != identity && (_parent(identity) >= startNode));

			if (length > 0)
			{
			  return m_xstrf.newstr(m_chars, offset, length);
			}
		  }
		  else if (type != DTM.ELEMENT_NODE)
		  {
			int dataIndex = _dataOrQName(identity);

			if (dataIndex < 0)
			{
			  dataIndex = -dataIndex;
			  dataIndex = m_data.elementAt(dataIndex + 1);
			}
			return m_xstrf.newstr(m_valuesOrPrefixes.indexToString(dataIndex));
		  }
		}

		return m_xstrf.emptystr();
	  }

	  /// <summary>
	  /// Determine if the string-value of a node is whitespace
	  /// </summary>
	  /// <param name="nodeHandle"> The node Handle.
	  /// </param>
	  /// <returns> Return true if the given node is whitespace. </returns>
	  public virtual bool isWhitespace(int nodeHandle)
	  {
		int identity = makeNodeIdentity(nodeHandle);
		int type;
		if (identity == DTM.NULL) // Separate lines because I wanted to breakpoint it
		{
		  type = DTM.NULL;
		}
		else
		{
		  type = _type(identity);
		}

		if (isTextType(type))
		{
		  int dataIndex = _dataOrQName(identity);
		  int offset = m_data.elementAt(dataIndex);
		  int length = m_data.elementAt(dataIndex + 1);

		  return m_chars.isWhitespace(offset, length);
		}
		return false;
	  }

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
	  public override int getElementById(string elementId)
	  {

		int? intObj;
		bool isMore = true;

		do
		{
		  intObj = (int?) m_idAttributes[elementId];

		  if (null != intObj)
		  {
			return makeNodeHandle(intObj.Value);
		  }

		  if (!isMore || m_endDocumentOccured)
		  {
			break;
		  }

		  isMore = nextNode();
		} while (null == intObj);

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Get a prefix either from the qname or from the uri mapping, or just make
	  /// one up!
	  /// </summary>
	  /// <param name="qname"> The qualified name, which may be null. </param>
	  /// <param name="uri"> The namespace URI, which may be null.
	  /// </param>
	  /// <returns> The prefix if there is one, or null. </returns>
	  public virtual string getPrefix(string qname, string uri)
	  {

		string prefix;
		int uriIndex = -1;

		if (null != uri && uri.Length > 0)
		{

		  do
		  {
			uriIndex = m_prefixMappings.indexOf(uri, ++uriIndex);
		  } while ((uriIndex & 0x01) == 0);

		  if (uriIndex >= 0)
		  {
			prefix = (string) m_prefixMappings[uriIndex - 1];
		  }
		  else if (null != qname)
		  {
			int indexOfNSSep = qname.IndexOf(':');

			if (qname.Equals("xmlns"))
			{
			  prefix = "";
			}
			else if (qname.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  prefix = qname.Substring(indexOfNSSep + 1);
			}
			else
			{
			  prefix = (indexOfNSSep > 0) ? qname.Substring(0, indexOfNSSep) : null;
			}
		  }
		  else
		  {
			prefix = null;
		  }
		}
		else if (null != qname)
		{
		  int indexOfNSSep = qname.IndexOf(':');

		  if (indexOfNSSep > 0)
		  {
			if (qname.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  prefix = qname.Substring(indexOfNSSep + 1);
			}
			else
			{
			  prefix = qname.Substring(0, indexOfNSSep);
			}
		  }
		  else
		  {
			  if (qname.Equals("xmlns"))
			  {
				prefix = "";
			  }
			  else
			  {
				prefix = null;
			  }
		  }
		}
		else
		{
		  prefix = null;
		}

		return prefix;
	  }

	  /// <summary>
	  /// Get a prefix either from the uri mapping, or just make
	  /// one up!
	  /// </summary>
	  /// <param name="uri"> The namespace URI, which may be null.
	  /// </param>
	  /// <returns> The prefix if there is one, or null. </returns>
	  public virtual int getIdForNamespace(string uri)
	  {

		 return m_valuesOrPrefixes.stringToIndex(uri);

	  }

		/// <summary>
		/// Get a prefix either from the qname or from the uri mapping, or just make
		/// one up!
		/// </summary>
		/// <returns> The prefix if there is one, or null. </returns>
	  public virtual string getNamespaceURI(string prefix)
	  {

		string uri = "";
		int prefixIndex = m_contextIndexes.peek() - 1;

		if (null == prefix)
		{
		  prefix = "";
		}

		  do
		  {
			prefixIndex = m_prefixMappings.indexOf(prefix, ++prefixIndex);
		  } while ((prefixIndex >= 0) && (prefixIndex & 0x01) == 0x01);

		  if (prefixIndex > -1)
		  {
			uri = (string) m_prefixMappings[prefixIndex + 1];
		  }


		return uri;
	  }

	  /// <summary>
	  /// Set an ID string to node association in the ID table.
	  /// </summary>
	  /// <param name="id"> The ID string. </param>
	  /// <param name="elem"> The associated element handle. </param>
	  public virtual void setIDAttribute(string id, int elem)
	  {
		m_idAttributes[id] = new int?(elem);
	  }

	  /// <summary>
	  /// Check whether accumulated text should be stripped; if not,
	  /// append the appropriate flavor of text/cdata node.
	  /// </summary>
	  protected internal virtual void charactersFlush()
	  {

		if (m_textPendingStart >= 0) // -1 indicates no-text-in-progress
		{
		  int length = m_chars.size() - m_textPendingStart;
		  bool doStrip = false;

		  if (ShouldStripWhitespace)
		  {
			doStrip = m_chars.isWhitespace(m_textPendingStart, length);
		  }

		  if (doStrip)
		  {
			m_chars.Length = m_textPendingStart; // Discard accumulated text
		  }
		  else
		  {
			// Guard against characters/ignorableWhitespace events that
			// contained no characters.  They should not result in a node.
			if (length > 0)
			{
			  int exName = m_expandedNameTable.getExpandedTypeID(DTM.TEXT_NODE);
			  int dataIndex = m_data.size();

			  m_previous = addNode(m_coalescedTextType, exName, m_parents.peek(), m_previous, dataIndex, false);

			  m_data.addElement(m_textPendingStart);
			  m_data.addElement(length);
			}
		  }

		  // Reset for next text block
		  m_textPendingStart = -1;
		  m_textType = m_coalescedTextType = DTM.TEXT_NODE;
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the EntityResolver interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Resolve an external entity.
	  /// 
	  /// <para>Always return null, so that the parser will use the system
	  /// identifier provided in the XML document.  This method implements
	  /// the SAX default behaviour: application writers can override it
	  /// in a subclass to do special translations such as catalog lookups
	  /// or URI redirection.</para>
	  /// </summary>
	  /// <param name="publicId"> The public identifer, or null if none is
	  ///                 available. </param>
	  /// <param name="systemId"> The system identifier provided in the XML
	  ///                 document. </param>
	  /// <returns> The new input source, or null to require the
	  ///         default behaviour. </returns>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.EntityResolver.resolveEntity"
	  ////>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public InputSource resolveEntity(String publicId, String systemId) throws SAXException
	  public virtual InputSource resolveEntity(string publicId, string systemId)
	  {
		return null;
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of DTDHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive notification of a notation declaration.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass if they wish to keep track of the notations
	  /// declared in a document.</para>
	  /// </summary>
	  /// <param name="name"> The notation name. </param>
	  /// <param name="publicId"> The notation public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The notation system identifier. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.DTDHandler.notationDecl"
	  ////>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void notationDecl(String name, String publicId, String systemId) throws SAXException
	  public virtual void notationDecl(string name, string publicId, string systemId)
	  {

		// no op
	  }

	  /// <summary>
	  /// Receive notification of an unparsed entity declaration.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to keep track of the unparsed entities
	  /// declared in a document.</para>
	  /// </summary>
	  /// <param name="name"> The entity name. </param>
	  /// <param name="publicId"> The entity public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The entity system identifier. </param>
	  /// <param name="notationName"> The name of the associated notation. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"
	  ////>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void unparsedEntityDecl(String name, String publicId, String systemId, String notationName) throws SAXException
	  public virtual void unparsedEntityDecl(string name, string publicId, string systemId, string notationName)
	  {

		if (null == m_entities)
		{
		  m_entities = new ArrayList();
		}

		try
		{
		  systemId = SystemIDResolver.getAbsoluteURI(systemId, DocumentBaseURI);
		}
		catch (Exception e)
		{
		  throw new org.xml.sax.SAXException(e);
		}

		//  private static final int ENTITY_FIELD_PUBLICID = 0;
		m_entities.Add(publicId);

		//  private static final int ENTITY_FIELD_SYSTEMID = 1;
		m_entities.Add(systemId);

		//  private static final int ENTITY_FIELD_NOTATIONNAME = 2;
		m_entities.Add(notationName);

		//  private static final int ENTITY_FIELD_NAME = 3;
		m_entities.Add(name);
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of ContentHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive a Locator object for document events.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass if they wish to store the locator for use
	  /// with other document events.</para>
	  /// </summary>
	  /// <param name="locator"> A locator for all SAX document events. </param>
	  /// <seealso cref="org.xml.sax.ContentHandler.setDocumentLocator"/>
	  /// <seealso cref="org.xml.sax.Locator"/>
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
			m_locator = value;
			m_systemId = value.getSystemId();
		  }
	  }

	  /// <summary>
	  /// Receive notification of the beginning of the document.
	  /// </summary>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.startDocument"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws SAXException
	  public virtual void startDocument()
	  {
		if (DEBUG)
		{
		  Console.WriteLine("startDocument");
		}


		int doc = addNode(DTM.DOCUMENT_NODE, m_expandedNameTable.getExpandedTypeID(DTM.DOCUMENT_NODE), DTM.NULL, DTM.NULL, 0, true);

		m_parents.push(doc);
		m_previous = DTM.NULL;

		m_contextIndexes.push(m_prefixMappings.Count); // for the next element.
	  }

	  /// <summary>
	  /// Receive notification of the end of the document.
	  /// </summary>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.endDocument"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws SAXException
	  public virtual void endDocument()
	  {
		if (DEBUG)
		{
		  Console.WriteLine("endDocument");
		}

			charactersFlush();

		m_nextsib.setElementAt(NULL,0);

		if (m_firstch.elementAt(0) == NOTPROCESSED)
		{
		  m_firstch.setElementAt(NULL,0);
		}

		if (DTM.NULL != m_previous)
		{
		  m_nextsib.setElementAt(DTM.NULL,m_previous);
		}

		m_parents = null;
		m_prefixMappings = null;
		m_contextIndexes = null;

		m_endDocumentOccured = true;

		// Bugzilla 4858: throw away m_locator. we cache m_systemId
		m_locator = null;
	  }

	  /// <summary>
	  /// Receive notification of the start of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each Namespace prefix scope (such as storing the prefix mapping).</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <param name="uri"> The Namespace URI mapped to the prefix. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.startPrefixMapping"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("startPrefixMapping: prefix: " + prefix + ", uri: " + uri);
		}

		if (null == prefix)
		{
		  prefix = "";
		}
		m_prefixMappings.Add(prefix); // JDK 1.1.x compat -sc
		m_prefixMappings.Add(uri); // JDK 1.1.x compat -sc
	  }

	  /// <summary>
	  /// Receive notification of the end of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end of
	  /// each prefix mapping.</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.endPrefixMapping"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("endPrefixMapping: prefix: " + prefix);
		}

		if (null == prefix)
		{
		  prefix = "";
		}

		int index = m_contextIndexes.peek() - 1;

		do
		{
		  index = m_prefixMappings.indexOf(prefix, ++index);
		} while ((index >= 0) && ((index & 0x01) == 0x01));


		if (index > -1)
		{
		  m_prefixMappings[index] = "%@$#^@#";
		  m_prefixMappings[index + 1] = "%@$#^@#";
		}

		// no op
	  }

	  /// <summary>
	  /// Check if a declaration has already been made for a given prefix.
	  /// </summary>
	  /// <param name="prefix"> non-null prefix string.
	  /// </param>
	  /// <returns> true if the declaration has already been declared in the
	  ///         current context. </returns>
	  protected internal virtual bool declAlreadyDeclared(string prefix)
	  {

		int startDecls = m_contextIndexes.peek();
		ArrayList prefixMappings = m_prefixMappings;
		int nDecls = prefixMappings.Count;

		for (int i = startDecls; i < nDecls; i += 2)
		{
		  string prefixDecl = (string) prefixMappings[i];

		  if (string.ReferenceEquals(prefixDecl, null))
		  {
			continue;
		  }

		  if (prefixDecl.Equals(prefix))
		  {
			return true;
		  }
		}

		return false;
	  }

		internal bool m_pastFirstElement = false;

	  /// <summary>
	  /// Receive notification of the start of an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each element (such as allocating a new tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.startElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, Attributes attributes) throws SAXException
	  public virtual void startElement(string uri, string localName, string qName, Attributes attributes)
	  {
	   if (DEBUG)
	   {
		  Console.WriteLine("startElement: uri: " + uri + ", localname: " + localName + ", qname: " + qName + ", atts: " + attributes);

				bool DEBUG_ATTRS = true;
				if (DEBUG_ATTRS & attributes != null)
				{
					int n = attributes.getLength();
					if (n == 0)
					{
						Console.WriteLine("\tempty attribute list");
					}
					else
					{
						for (int i = 0; i < n; i++)
						{
						Console.WriteLine("\t attr: uri: " + attributes.getURI(i) + ", localname: " + attributes.getLocalName(i) + ", qname: " + attributes.getQName(i) + ", type: " + attributes.getType(i) + ", value: " + attributes.getValue(i));
						}
					}
				}
	   }

		charactersFlush();

		int exName = m_expandedNameTable.getExpandedTypeID(uri, localName, DTM.ELEMENT_NODE);
		string prefix = getPrefix(qName, uri);
		int prefixIndex = (null != prefix) ? m_valuesOrPrefixes.stringToIndex(qName) : 0;

		int elemNode = addNode(DTM.ELEMENT_NODE, exName, m_parents.peek(), m_previous, prefixIndex, true);

		if (m_indexing)
		{
		  indexNode(exName, elemNode);
		}


		m_parents.push(elemNode);

		int startDecls = m_contextIndexes.peek();
		int nDecls = m_prefixMappings.Count;
		int prev = DTM.NULL;

		if (!m_pastFirstElement)
		{
		  // SPECIAL CASE: Implied declaration at root element
		  prefix = "xml";
		  string declURL = "http://www.w3.org/XML/1998/namespace";
		  exName = m_expandedNameTable.getExpandedTypeID(null, prefix, DTM.NAMESPACE_NODE);
		  int val = m_valuesOrPrefixes.stringToIndex(declURL);
		  prev = addNode(DTM.NAMESPACE_NODE, exName, elemNode, prev, val, false);
		  m_pastFirstElement = true;
		}

		for (int i = startDecls; i < nDecls; i += 2)
		{
		  prefix = (string) m_prefixMappings[i];

		  if (string.ReferenceEquals(prefix, null))
		  {
			continue;
		  }

		  string declURL = (string) m_prefixMappings[i + 1];

		  exName = m_expandedNameTable.getExpandedTypeID(null, prefix, DTM.NAMESPACE_NODE);

		  int val = m_valuesOrPrefixes.stringToIndex(declURL);

		  prev = addNode(DTM.NAMESPACE_NODE, exName, elemNode, prev, val, false);
		}

		int n = attributes.getLength();

		for (int i = 0; i < n; i++)
		{
		  string attrUri = attributes.getURI(i);
		  string attrQName = attributes.getQName(i);
		  string valString = attributes.getValue(i);

		  prefix = getPrefix(attrQName, attrUri);

		  int nodeType;

		   string attrLocalName = attributes.getLocalName(i);

		  if ((null != attrQName) && (attrQName.Equals("xmlns") || attrQName.StartsWith("xmlns:", StringComparison.Ordinal)))
		  {
			if (declAlreadyDeclared(prefix))
			{
			  continue; // go to the next attribute.
			}

			nodeType = DTM.NAMESPACE_NODE;
		  }
		  else
		  {
			nodeType = DTM.ATTRIBUTE_NODE;

			if (attributes.getType(i).equalsIgnoreCase("ID"))
			{
			  setIDAttribute(valString, elemNode);
			}
		  }

		  // Bit of a hack... if somehow valString is null, stringToIndex will
		  // return -1, which will make things very unhappy.
		  if (null == valString)
		  {
			valString = "";
		  }

		  int val = m_valuesOrPrefixes.stringToIndex(valString);
		  //String attrLocalName = attributes.getLocalName(i);

		  if (null != prefix)
		  {

			prefixIndex = m_valuesOrPrefixes.stringToIndex(attrQName);

			int dataIndex = m_data.size();

			m_data.addElement(prefixIndex);
			m_data.addElement(val);

			val = -dataIndex;
		  }

		  exName = m_expandedNameTable.getExpandedTypeID(attrUri, attrLocalName, nodeType);
		  prev = addNode(nodeType, exName, elemNode, prev, val, false);
		}

		if (DTM.NULL != prev)
		{
		  m_nextsib.setElementAt(DTM.NULL,prev);
		}

		if (null != m_wsfilter)
		{
		  short wsv = m_wsfilter.getShouldStripSpace(makeNodeHandle(elemNode), this);
		  bool shouldStrip = (DTMWSFilter.INHERIT == wsv) ? ShouldStripWhitespace : (DTMWSFilter.STRIP == wsv);

		  pushShouldStripWhitespace(shouldStrip);
		}

		m_previous = DTM.NULL;

		m_contextIndexes.push(m_prefixMappings.Count); // for the children.
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end of
	  /// each element (such as finalising a tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified XML 1.0 name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.endElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws SAXException
	  public virtual void endElement(string uri, string localName, string qName)
	  {
	   if (DEBUG)
	   {
		  Console.WriteLine("endElement: uri: " + uri + ", localname: " + localName + ", qname: " + qName);
	   }

		charactersFlush();

		// If no one noticed, startPrefixMapping is a drag.
		// Pop the context for the last child (the one pushed by startElement)
		m_contextIndexes.quickPop(1);

		// Do it again for this one (the one pushed by the last endElement).
		int topContextIndex = m_contextIndexes.peek();
		if (topContextIndex != m_prefixMappings.Count)
		{
		  m_prefixMappings.Capacity = topContextIndex;
		}

		int lastNode = m_previous;

		m_previous = m_parents.pop();

		// If lastNode is still DTM.NULL, this element had no children
		if (DTM.NULL == lastNode)
		{
		  m_firstch.setElementAt(DTM.NULL,m_previous);
		}
		else
		{
		  m_nextsib.setElementAt(DTM.NULL,lastNode);
		}

		popShouldStripWhitespace();
	  }

	  /// <summary>
	  /// Receive notification of character data inside an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method to take specific actions for each chunk of character data
	  /// (such as adding the data to a node or buffer, or printing it to
	  /// a file).</para>
	  /// </summary>
	  /// <param name="ch"> The characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.characters"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {
		if (m_textPendingStart == -1) // First one in this block
		{
		  m_textPendingStart = m_chars.size();
		  m_coalescedTextType = m_textType;
		}
		// Type logic: If all adjacent text is CDATASections, the
		// concatentated text is treated as a single CDATASection (see
		// initialization above).  If any were ordinary Text, the whole
		// thing is treated as Text. This may be worth %REVIEW%ing.
		else if (m_textType == DTM.TEXT_NODE)
		{
		  m_coalescedTextType = DTM.TEXT_NODE;
		}

		m_chars.append(ch, start, length);
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method to take specific actions for each chunk of ignorable
	  /// whitespace (such as adding data to a node or buffer, or printing
	  /// it to a file).</para>
	  /// </summary>
	  /// <param name="ch"> The whitespace characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.ignorableWhitespace"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {

		// %OPT% We can probably take advantage of the fact that we know this 
		// is whitespace.
		characters(ch, start, length);
	  }

	  /// <summary>
	  /// Receive notification of a processing instruction.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.processingInstruction"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		if (DEBUG)
		{
			 Console.WriteLine("processingInstruction: target: " + target + ", data: " + data);
		}

		charactersFlush();

		int exName = m_expandedNameTable.getExpandedTypeID(null, target, DTM.PROCESSING_INSTRUCTION_NODE);
		int dataIndex = m_valuesOrPrefixes.stringToIndex(data);

		m_previous = addNode(DTM.PROCESSING_INSTRUCTION_NODE, exName, m_parents.peek(), m_previous, dataIndex, false);
	  }

	  /// <summary>
	  /// Receive notification of a skipped entity.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="name"> The name of the skipped entity. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.processingInstruction"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(String name) throws SAXException
	  public virtual void skippedEntity(string name)
	  {

		// %REVIEW% What should be done here?
		// no op
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the ErrorHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive notification of a parser warning.
	  /// 
	  /// <para>The default implementation does nothing.  Application writers
	  /// may override this method in a subclass to take specific actions
	  /// for each warning, such as inserting the message in a log file or
	  /// printing it to the console.</para>
	  /// </summary>
	  /// <param name="e"> The warning information encoded as an exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ErrorHandler.warning"/>
	  /// <seealso cref="org.xml.sax.SAXParseException"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warning(SAXParseException e) throws SAXException
	  public virtual void warning(SAXParseException e)
	  {

		// %REVIEW% Is there anyway to get the JAXP error listener here?
		Console.Error.WriteLine(e.Message);
	  }

	  /// <summary>
	  /// Receive notification of a recoverable parser error.
	  /// 
	  /// <para>The default implementation does nothing.  Application writers
	  /// may override this method in a subclass to take specific actions
	  /// for each error, such as inserting the message in a log file or
	  /// printing it to the console.</para>
	  /// </summary>
	  /// <param name="e"> The warning information encoded as an exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ErrorHandler.warning"/>
	  /// <seealso cref="org.xml.sax.SAXParseException"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(SAXParseException e) throws SAXException
	  public virtual void error(SAXParseException e)
	  {
		throw e;
	  }

	  /// <summary>
	  /// Report a fatal XML parsing error.
	  /// 
	  /// <para>The default implementation throws a SAXParseException.
	  /// Application writers may override this method in a subclass if
	  /// they need to take specific actions for each fatal error (such as
	  /// collecting all of the errors into a single report): in any case,
	  /// the application must stop all regular processing when this
	  /// method is invoked, since the document is no longer reliable, and
	  /// the parser may no longer report parsing events.</para>
	  /// </summary>
	  /// <param name="e"> The error information encoded as an exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ErrorHandler.fatalError"/>
	  /// <seealso cref="org.xml.sax.SAXParseException"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void fatalError(SAXParseException e) throws SAXException
	  public virtual void fatalError(SAXParseException e)
	  {
		throw e;
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the DeclHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Report an element type declaration.
	  /// 
	  /// <para>The content model will consist of the string "EMPTY", the
	  /// string "ANY", or a parenthesised group, optionally followed
	  /// by an occurrence indicator.  The model will be normalized so
	  /// that all whitespace is removed,and will include the enclosing
	  /// parentheses.</para>
	  /// </summary>
	  /// <param name="name"> The element type name. </param>
	  /// <param name="model"> The content model as a normalized string. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws SAXException
	  public virtual void elementDecl(string name, string model)
	  {

		// no op
	  }

	  /// <summary>
	  /// Report an attribute type declaration.
	  /// 
	  /// <para>Only the effective (first) declaration for an attribute will
	  /// be reported.  The type will be one of the strings "CDATA",
	  /// "ID", "IDREF", "IDREFS", "NMTOKEN", "NMTOKENS", "ENTITY",
	  /// "ENTITIES", or "NOTATION", or a parenthesized token group with
	  /// the separator "|" and all whitespace removed.</para>
	  /// </summary>
	  /// <param name="eName"> The name of the associated element. </param>
	  /// <param name="aName"> The name of the attribute. </param>
	  /// <param name="type"> A string representing the attribute type. </param>
	  /// <param name="valueDefault"> A string representing the attribute default
	  ///        ("#IMPLIED", "#REQUIRED", or "#FIXED") or null if
	  ///        none of these applies. </param>
	  /// <param name="value"> A string representing the attribute's default value,
	  ///        or null if there is none. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws SAXException
	  public virtual void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
	  {

		// no op
	  }

	  /// <summary>
	  /// Report an internal entity declaration.
	  /// 
	  /// <para>Only the effective (first) declaration for each entity
	  /// will be reported.</para>
	  /// </summary>
	  /// <param name="name"> The name of the entity.  If it is a parameter
	  ///        entity, the name will begin with '%'. </param>
	  /// <param name="value"> The replacement text of the entity. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".externalEntityDecl"/>
	  /// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws SAXException
	  public virtual void internalEntityDecl(string name, string value)
	  {

		// no op
	  }

	  /// <summary>
	  /// Report a parsed external entity declaration.
	  /// 
	  /// <para>Only the effective (first) declaration for each entity
	  /// will be reported.</para>
	  /// </summary>
	  /// <param name="name"> The name of the entity.  If it is a parameter
	  ///        entity, the name will begin with '%'. </param>
	  /// <param name="publicId"> The declared public identifier of the entity, or
	  ///        null if none was declared. </param>
	  /// <param name="systemId"> The declared system identifier of the entity. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".internalEntityDecl"/>
	  /// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws SAXException
	  public virtual void externalEntityDecl(string name, string publicId, string systemId)
	  {

		// no op
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the LexicalHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Report the start of DTD declarations, if any.
	  /// 
	  /// <para>Any declarations are assumed to be in the internal subset
	  /// unless otherwise indicated by a <seealso cref="startEntity startEntity"/>
	  /// event.</para>
	  /// 
	  /// <para>Note that the start/endDTD events will appear within
	  /// the start/endDocument events from ContentHandler and
	  /// before the first startElement event.</para>
	  /// </summary>
	  /// <param name="name"> The document type name. </param>
	  /// <param name="publicId"> The declared public identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <param name="systemId"> The declared system identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <exception cref="SAXException"> The application may raise an
	  ///            exception. </exception>
	  /// <seealso cref=".endDTD"/>
	  /// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws SAXException
	  public virtual void startDTD(string name, string publicId, string systemId)
	  {

		m_insideDTD = true;
	  }

	  /// <summary>
	  /// Report the end of DTD declarations.
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".startDTD"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDTD() throws SAXException
	  public virtual void endDTD()
	  {

		m_insideDTD = false;
	  }

	  /// <summary>
	  /// Report the beginning of an entity in content.
	  /// 
	  /// <para><strong>NOTE:</entity> entity references in attribute
	  /// values -- and the start and end of the document entity --
	  /// are never reported.</para>
	  /// 
	  /// <para>The start and end of the external DTD subset are reported
	  /// using the pseudo-name "[dtd]".  All other events must be
	  /// properly nested within start/end entity events.</para>
	  /// 
	  /// <para>Note that skipped entities will be reported through the
	  /// <seealso cref="org.xml.sax.ContentHandler.skippedEntity skippedEntity"/>
	  /// event, which is part of the ContentHandler interface.</para>
	  /// </summary>
	  /// <param name="name"> The name of the entity.  If it is a parameter
	  ///        entity, the name will begin with '%'. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".endEntity"/>
	  /// <seealso cref="org.xml.sax.ext.DeclHandler.internalEntityDecl"/>
	  /// <seealso cref="org.xml.sax.ext.DeclHandler.externalEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startEntity(String name) throws SAXException
	  public virtual void startEntity(string name)
	  {

		// no op
	  }

	  /// <summary>
	  /// Report the end of an entity.
	  /// </summary>
	  /// <param name="name"> The name of the entity that is ending. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endEntity(String name) throws SAXException
	  public virtual void endEntity(string name)
	  {

		// no op
	  }

	  /// <summary>
	  /// Report the start of a CDATA section.
	  /// 
	  /// <para>The contents of the CDATA section will be reported through
	  /// the regular {@link org.xml.sax.ContentHandler#characters
	  /// characters} event.</para>
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".endCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startCDATA() throws SAXException
	  public virtual void startCDATA()
	  {
		m_textType = DTM.CDATA_SECTION_NODE;
	  }

	  /// <summary>
	  /// Report the end of a CDATA section.
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref=".startCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCDATA() throws SAXException
	  public virtual void endCDATA()
	  {
		m_textType = DTM.TEXT_NODE;
	  }

	  /// <summary>
	  /// Report an XML comment anywhere in the document.
	  /// 
	  /// <para>This callback will be used for comments inside or outside the
	  /// document element, including comments in the external DTD
	  /// subset (if read).</para>
	  /// </summary>
	  /// <param name="ch"> An array holding the characters in the comment. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws SAXException
	  public virtual void comment(char[] ch, int start, int length)
	  {

		if (m_insideDTD) // ignore comments if we're inside the DTD
		{
		  return;
		}

		charactersFlush();

		int exName = m_expandedNameTable.getExpandedTypeID(DTM.COMMENT_NODE);

		// For now, treat comments as strings...  I guess we should do a
		// seperate FSB buffer instead.
		int dataIndex = m_valuesOrPrefixes.stringToIndex(new string(ch, start, length));


		m_previous = addNode(DTM.COMMENT_NODE, exName, m_parents.peek(), m_previous, dataIndex, false);
	  }

	  /// <summary>
	  /// Set a run time property for this DTM instance.
	  /// 
	  /// %REVIEW% Now that we no longer use this method to support
	  /// getSourceLocatorFor, can we remove it?
	  /// </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <param name="value"> an <code>Object</code> value </param>
	  public override void setProperty(string property, object value)
	  {
	  }

	  /// <summary>
	  /// Retrieve the SourceLocator associated with a specific node.
	  /// This is only meaningful if the XalanProperties.SOURCE_LOCATION flag was
	  /// set True using setProperty; if it was never set, or was set false, we
	  /// will return null. 
	  /// 
	  /// (We _could_ return a locator with the document's base URI and bogus 
	  /// line/column information. Trying that; see the else clause.)
	  /// 
	  /// </summary>
	  public override SourceLocator getSourceLocatorFor(int node)
	  {
		if (m_useSourceLocationProperty)
		{

		  node = makeNodeIdentity(node);


		  return new NodeLocator(null, m_sourceSystemId.elementAt(node), m_sourceLine.elementAt(node), m_sourceColumn.elementAt(node));
		}
		else if (m_locator != null)
		{
			return new NodeLocator(null,m_locator.getSystemId(),-1,-1);
		}
		else if (!string.ReferenceEquals(m_systemId, null))
		{
			return new NodeLocator(null,m_systemId,-1,-1);
		}
		return null;
	  }

	  public virtual string getFixedNames(int type)
	  {
		return m_fixednames[type];
	  }
	}

}