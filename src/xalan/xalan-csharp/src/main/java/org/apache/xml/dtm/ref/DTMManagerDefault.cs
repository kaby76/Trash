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
 * $Id: DTMManagerDefault.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{


	using DOM2DTM = org.apache.xml.dtm.@ref.dom2dtm.DOM2DTM;
	using SAX2DTM = org.apache.xml.dtm.@ref.sax2dtm.SAX2DTM;
	using SAX2RTFDTM = org.apache.xml.dtm.@ref.sax2dtm.SAX2RTFDTM;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using XMLReaderManager = org.apache.xml.utils.XMLReaderManager;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using XMLReader = org.xml.sax.XMLReader;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;

	/// <summary>
	/// The default implementation for the DTMManager.
	/// 
	/// %REVIEW% There is currently a reentrancy issue, since the finalizer
	/// for XRTreeFrag (which runs in the GC thread) wants to call
	/// DTMManager.release(), and may do so at the same time that the main
	/// transformation thread is accessing the manager. Our current solution is
	/// to make most of the manager's methods <code>synchronized</code>.
	/// Early tests suggest that doing so is not causing a significant
	/// performance hit in Xalan. However, it should be noted that there
	/// is a possible alternative solution: rewrite release() so it merely
	/// posts a request for release onto a threadsafe queue, and explicitly
	/// process that queue on an infrequent basis during main-thread
	/// activity (eg, when getDTM() is invoked). The downside of that solution
	/// would be a greater delay before the DTM's storage is actually released
	/// for reuse.
	/// 
	/// </summary>
	public class DTMManagerDefault : DTMManager
	{
	  //static final boolean JKESS_XNI_EXPERIMENT=true;

	  /// <summary>
	  /// Set this to true if you want a dump of the DTM after creation. </summary>
	  private const bool DUMPTREE = false;

	  /// <summary>
	  /// Set this to true if you want a basic diagnostics. </summary>
	  private const bool DEBUG = false;

	  /// <summary>
	  /// Map from DTM identifier numbers to DTM objects that this manager manages.
	  /// One DTM may have several prefix numbers, if extended node indexing
	  /// is in use; in that case, m_dtm_offsets[] will used to control which
	  /// prefix maps to which section of the DTM.
	  /// 
	  /// This array grows as necessary; see addDTM().
	  /// 
	  /// This array grows as necessary; see addDTM(). Growth is uncommon... but
	  /// access needs to be blindingly fast since it's used in node addressing.
	  /// </summary>
	  protected internal DTM[] m_dtms = new DTM[256];

	  /// <summary>
	  /// Map from DTM identifier numbers to offsets. For small DTMs with a 
	  /// single identifier, this will always be 0. In overflow addressing, where
	  /// additional identifiers are allocated to access nodes beyond the range of
	  /// a single Node Handle, this table is used to map the handle's node field
	  /// into the actual node identifier.
	  /// 
	  /// This array grows as necessary; see addDTM().
	  /// 
	  /// This array grows as necessary; see addDTM(). Growth is uncommon... but
	  /// access needs to be blindingly fast since it's used in node addressing.
	  /// (And at the moment, that includes accessing it from DTMDefaultBase,
	  /// which is why this is not Protected or Private.)
	  /// </summary>
	  internal int[] m_dtm_offsets = new int[256];

	  /// <summary>
	  /// The cache for XMLReader objects to be used if the user did not
	  /// supply an XMLReader for a SAXSource or supplied a StreamSource.
	  /// </summary>
	  protected internal XMLReaderManager m_readerManager = null;

	  /// <summary>
	  /// The default implementation of ContentHandler, DTDHandler and ErrorHandler.
	  /// </summary>
	  protected internal DefaultHandler m_defaultHandler = new DefaultHandler();

	  /// <summary>
	  /// Add a DTM to the DTM table. This convenience call adds it as the 
	  /// "base DTM ID", with offset 0. The other version of addDTM should 
	  /// be used if you want to add "extended" DTM IDs with nonzero offsets.
	  /// </summary>
	  /// <param name="dtm"> Should be a valid reference to a DTM. </param>
	  /// <param name="id"> Integer DTM ID to be bound to this DTM </param>
	  public virtual void addDTM(DTM dtm, int id)
	  {
		  lock (this)
		  {
			  addDTM(dtm,id,0);
		  }
	  }


	  /// <summary>
	  /// Add a DTM to the DTM table.
	  /// </summary>
	  /// <param name="dtm"> Should be a valid reference to a DTM. </param>
	  /// <param name="id"> Integer DTM ID to be bound to this DTM. </param>
	  /// <param name="offset"> Integer addressing offset. The internal DTM Node ID is
	  /// obtained by adding this offset to the node-number field of the 
	  /// public DTM Handle. For the first DTM ID accessing each DTM, this is 0;
	  /// for overflow addressing it will be a multiple of 1<<IDENT_DTM_NODE_BITS. </param>
	  public virtual void addDTM(DTM dtm, int id, int offset)
	  {
		  lock (this)
		  {
				if (id >= IDENT_MAX_DTMS)
				{
					// TODO: %REVIEW% Not really the right error message.
				throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NO_DTMIDS_AVAIL, null)); //"No more DTM IDs are available!");
				}
        
				// We used to just allocate the array size to IDENT_MAX_DTMS.
				// But we expect to increase that to 16 bits, and I'm not willing
				// to allocate that much space unless needed. We could use one of our
				// handy-dandy Fast*Vectors, but this will do for now.
				// %REVIEW%
				int oldlen = m_dtms.Length;
				if (oldlen <= id)
				{
					// Various growth strategies are possible. I think we don't want 
					// to over-allocate excessively, and I'm willing to reallocate
					// more often to get that. See also Fast*Vector classes.
					//
					// %REVIEW% Should throw a more diagnostic error if we go over the max...
					int newlen = Math.Min((id + 256),IDENT_MAX_DTMS);
        
					DTM[] new_m_dtms = new DTM[newlen];
					Array.Copy(m_dtms,0,new_m_dtms,0,oldlen);
					m_dtms = new_m_dtms;
					int[] new_m_dtm_offsets = new int[newlen];
					Array.Copy(m_dtm_offsets,0,new_m_dtm_offsets,0,oldlen);
					m_dtm_offsets = new_m_dtm_offsets;
				}
        
			m_dtms[id] = dtm;
				m_dtm_offsets[id] = offset;
			dtm.documentRegistration();
				// The DTM should have been told who its manager was when we created it.
				// Do we need to allow for adopting DTMs _not_ created by this manager?
		  }
	  }

	  /// <summary>
	  /// Get the first free DTM ID available. %OPT% Linear search is inefficient!
	  /// </summary>
	  public virtual int FirstFreeDTMID
	  {
		  get
		  {
			  lock (this)
			  {
				int n = m_dtms.Length;
				for (int i = 1; i < n; i++)
				{
				  if (null == m_dtms[i])
				  {
					return i;
				  }
				}
					return n; // count on addDTM() to throw exception if out of range
			  }
		  }
	  }

	  /// <summary>
	  /// The default table for exandedNameID lookups.
	  /// </summary>
	  private ExpandedNameTable m_expandedNameTable = new ExpandedNameTable();

	  /// <summary>
	  /// Constructor DTMManagerDefault
	  /// 
	  /// </summary>
	  public DTMManagerDefault()
	  {
	  }


	  /// <summary>
	  /// Get an instance of a DTM, loaded with the content from the
	  /// specified source.  If the unique flag is true, a new instance will
	  /// always be returned.  Otherwise it is up to the DTMManager to return a
	  /// new instance or an instance that it already created and may be being used
	  /// by someone else.
	  /// 
	  /// A bit of magic in this implementation: If the source is null, unique is true,
	  /// and incremental and doIndexing are both false, we return an instance of
	  /// SAX2RTFDTM, which see.
	  /// 
	  /// (I think more parameters will need to be added for error handling, and entity
	  /// resolution, and more explicit control of the RTF situation).
	  /// </summary>
	  /// <param name="source"> the specification of the source object. </param>
	  /// <param name="unique"> true if the returned DTM must be unique, probably because it
	  /// is going to be mutated. </param>
	  /// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
	  ///                         be null. </param>
	  /// <param name="incremental"> true if the DTM should be built incrementally, if
	  ///                    possible. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public override DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing)
	  {
		  lock (this)
		  {
        
			if (DEBUG && null != source)
			{
			  Console.WriteLine("Starting " + (unique ? "UNIQUE" : "shared") + " source: " + source.SystemId);
			}
        
			XMLStringFactory xstringFactory = m_xsf;
			int dtmPos = FirstFreeDTMID;
			int documentID = dtmPos << IDENT_DTM_NODE_BITS;
        
			if ((null != source) && source is DOMSource)
			{
			  DOM2DTM dtm = new DOM2DTM(this, (DOMSource) source, documentID, whiteSpaceFilter, xstringFactory, doIndexing);
        
			  addDTM(dtm, dtmPos, 0);
        
			  //      if (DUMPTREE)
			  //      {
			  //        dtm.dumpDTM();
			  //      }
        
			  return dtm;
			}
			else
			{
			  bool isSAXSource = (null != source) ? (source is SAXSource) : true;
			  bool isStreamSource = (null != source) ? (source is StreamSource) : false;
        
			  if (isSAXSource || isStreamSource)
			  {
				XMLReader reader = null;
				SAX2DTM dtm;
        
				try
				{
				  InputSource xmlSource;
        
				  if (null == source)
				  {
					xmlSource = null;
				  }
				  else
				  {
					reader = getXMLReader(source);
					xmlSource = SAXSource.sourceToInputSource(source);
        
					string urlOfSource = xmlSource.SystemId;
        
					if (null != urlOfSource)
					{
					  try
					  {
						urlOfSource = SystemIDResolver.getAbsoluteURI(urlOfSource);
					  }
					  catch (Exception)
					  {
						// %REVIEW% Is there a better way to send a warning?
						Console.Error.WriteLine("Can not absolutize URL: " + urlOfSource);
					  }
        
					  xmlSource.SystemId = urlOfSource;
					}
				  }
        
				  if (source == null && unique && !incremental && !doIndexing)
				  {
					// Special case to support RTF construction into shared DTM.
					// It should actually still work for other uses,
					// but may be slightly deoptimized relative to the base
					// to allow it to deal with carrying multiple documents.
					//
					// %REVIEW% This is a sloppy way to request this mode;
					// we need to consider architectural improvements.
					dtm = new SAX2RTFDTM(this, source, documentID, whiteSpaceFilter, xstringFactory, doIndexing);
				  }
				  /// <summary>
				  ///************************************************************
				  /// // EXPERIMENTAL 3/22/02
				  /// else if(JKESS_XNI_EXPERIMENT && m_incremental) {        	
				  ///  dtm = new XNI2DTM(this, source, documentID, whiteSpaceFilter,
				  ///                    xstringFactory, doIndexing);
				  /// }
				  /// *************************************************************
				  /// </summary>
				  // Create the basic SAX2DTM.
				  else
				  {
					dtm = new SAX2DTM(this, source, documentID, whiteSpaceFilter, xstringFactory, doIndexing);
				  }
        
				  // Go ahead and add the DTM to the lookup table.  This needs to be
				  // done before any parsing occurs. Note offset 0, since we've just
				  // created a new DTM.
				  addDTM(dtm, dtmPos, 0);
        
        
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				  bool haveXercesParser = (null != reader) && (reader.GetType().FullName.Equals("org.apache.xerces.parsers.SAXParser"));
        
				  if (haveXercesParser)
				  {
					incremental = true; // No matter what.  %REVIEW%
				  }
        
				  // If the reader is null, but they still requested an incremental
				  // build, then we still want to set up the IncrementalSAXSource stuff.
				  if (m_incremental && incremental)
					   /* || ((null == reader) && incremental) */
				  {
					IncrementalSAXSource coParser = null;
        
					if (haveXercesParser)
					{
					  // IncrementalSAXSource_Xerces to avoid threading.
					  try
					  {
						coParser = (IncrementalSAXSource) Type.GetType("org.apache.xml.dtm.ref.IncrementalSAXSource_Xerces").newInstance();
					  }
					  catch (Exception ex)
					  {
						Console.WriteLine(ex.ToString());
						Console.Write(ex.StackTrace);
						coParser = null;
					  }
					}
        
					if (coParser == null)
					{
					  // Create a IncrementalSAXSource to run on the secondary thread.
					  if (null == reader)
					  {
						coParser = new IncrementalSAXSource_Filter();
					  }
					  else
					  {
						IncrementalSAXSource_Filter filter = new IncrementalSAXSource_Filter();
						filter.XMLReader = reader;
						coParser = filter;
					  }
					}
        
        
					/// <summary>
					///************************************************************
					/// // EXPERIMENTAL 3/22/02
					/// if (JKESS_XNI_EXPERIMENT && m_incremental &&
					///      dtm instanceof XNI2DTM && 
					///      coParser instanceof IncrementalSAXSource_Xerces) {
					///    org.apache.xerces.xni.parser.XMLPullParserConfiguration xpc=
					///          ((IncrementalSAXSource_Xerces)coParser)
					///                               .getXNIParserConfiguration();
					///  if (xpc!=null) {
					///    // Bypass SAX; listen to the XNI stream
					///    ((XNI2DTM)dtm).setIncrementalXNISource(xpc);
					///  } else {
					///      // Listen to the SAX stream (will fail, diagnostically...)
					///    dtm.setIncrementalSAXSource(coParser);
					///  }
					/// } else
					/// **************************************************************
					/// </summary>
        
					// Have the DTM set itself up as IncrementalSAXSource's listener.
					dtm.IncrementalSAXSource = coParser;
        
					if (null == xmlSource)
					{
        
					  // Then the user will construct it themselves.
					  return dtm;
					}
        
					if (null == reader.ErrorHandler)
					{
					  reader.ErrorHandler = dtm;
					}
					reader.DTDHandler = dtm;
        
					try
					{
					  // Launch parsing coroutine.  Launches a second thread,
					  // if we're using IncrementalSAXSource.filter().
        
					  coParser.startParse(xmlSource);
					}
					catch (Exception re)
					{
        
					  dtm.clearCoRoutine();
        
					  throw re;
					}
					catch (Exception e)
					{
        
					  dtm.clearCoRoutine();
        
					  throw new org.apache.xml.utils.WrappedRuntimeException(e);
					}
				  }
				  else
				  {
					if (null == reader)
					{
        
					  // Then the user will construct it themselves.
					  return dtm;
					}
        
					// not incremental
					reader.ContentHandler = dtm;
					reader.DTDHandler = dtm;
					if (null == reader.ErrorHandler)
					{
					  reader.ErrorHandler = dtm;
					}
        
					try
					{
					  reader.setProperty("http://xml.org/sax/properties/lexical-handler", dtm);
					}
					catch (SAXNotRecognizedException)
					{
					}
					  catch (SAXNotSupportedException)
					  {
					  }
        
					try
					{
					  reader.parse(xmlSource);
					}
					catch (Exception re)
					{
					  dtm.clearCoRoutine();
        
					  throw re;
					}
					catch (Exception e)
					{
					  dtm.clearCoRoutine();
        
					  throw new org.apache.xml.utils.WrappedRuntimeException(e);
					}
				  }
        
				  if (DUMPTREE)
				  {
					Console.WriteLine("Dumping SAX2DOM");
					dtm.dumpDTM(System.err);
				  }
        
				  return dtm;
				}
				finally
				{
				  // Reset the ContentHandler, DTDHandler, ErrorHandler to the DefaultHandler
				  // after creating the DTM.
				  if (reader != null && !(m_incremental && incremental))
				  {
					reader.ContentHandler = m_defaultHandler;
					reader.DTDHandler = m_defaultHandler;
					reader.ErrorHandler = m_defaultHandler;
        
					// Reset the LexicalHandler to null after creating the DTM.
					try
					{
					  reader.setProperty("http://xml.org/sax/properties/lexical-handler", null);
					}
					catch (Exception)
					{
					}
				  }
				  releaseXMLReader(reader);
				}
			  }
			  else
			  {
        
				// It should have been handled by a derived class or the caller
				// made a mistake.
				throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NOT_SUPPORTED, new object[]{source})); //"Not supported: " + source);
			  }
			}
		  }
	  }

	  /// <summary>
	  /// Given a W3C DOM node, try and return a DTM handle.
	  /// Note: calling this may be non-optimal, and there is no guarantee that
	  /// the node will be found in any particular DTM.
	  /// </summary>
	  /// <param name="node"> Non-null reference to a DOM node.
	  /// </param>
	  /// <returns> a valid DTM handle. </returns>
	  public override int getDTMHandleFromNode(Node node)
	  {
		  lock (this)
		  {
			if (null == node)
			{
			  throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NODE_NON_NULL, null)); //"node must be non-null for getDTMHandleFromNode!");
			}
        
			if (node is org.apache.xml.dtm.@ref.DTMNodeProxy)
			{
			  return ((org.apache.xml.dtm.@ref.DTMNodeProxy) node).DTMNodeNumber;
			}
        
			else
			{
			  // Find the DOM2DTMs wrapped around this Document (if any)
			  // and check whether they contain the Node in question.
			  //
			  // NOTE that since a DOM2DTM may represent a subtree rather
			  // than a full document, we have to be prepared to check more
			  // than one -- and there is no guarantee that we will find
			  // one that contains ancestors or siblings of the node we're
			  // seeking.
			  //
			  // %REVIEW% We could search for the one which contains this
			  // node at the deepest level, and thus covers the widest
			  // subtree, but that's going to entail additional work
			  // checking more DTMs... and getHandleOfNode is not a
			  // cheap operation in most implementations.
					//
					// TODO: %REVIEW% If overflow addressing, we may recheck a DTM
					// already examined. Ouch. But with the increased number of DTMs,
					// scanning back to check this is painful. 
					// POSSIBLE SOLUTIONS: 
					//   Generate a list of _unique_ DTM objects?
					//   Have each DTM cache last DOM node search?
					int max = m_dtms.Length;
			  for (int i = 0; i < max; i++)
			  {
				  DTM thisDTM = m_dtms[i];
				  if ((null != thisDTM) && thisDTM is DOM2DTM)
				  {
					int handle = ((DOM2DTM)thisDTM).getHandleOfNode(node);
					if (handle != org.apache.xml.dtm.DTM_Fields.NULL)
					{
						return handle;
					}
				  }
			  }
        
					// Not found; generate a new DTM.
					//
					// %REVIEW% Is this really desirable, or should we return null
					// and make folks explicitly instantiate from a DOMSource? The
					// latter is more work but gives the caller the opportunity to
					// explicitly add the DTM to a DTMManager... and thus to know when
					// it can be discarded again, which is something we need to pay much
					// more attention to. (Especially since only DTMs which are assigned
					// to a manager can use the overflow addressing scheme.)
					//
					// %BUG% If the source node was a DOM2DTM$defaultNamespaceDeclarationNode
					// and the DTM wasn't registered with this DTMManager, we will create
					// a new DTM and _still_ not be able to find the node (since it will
					// be resynthesized). Another reason to push hard on making all DTMs
					// be managed DTMs.
        
					// Since the real root of our tree may be a DocumentFragment, we need to
			  // use getParent to find the root, instead of getOwnerDocument.  Otherwise
			  // DOM2DTM#getHandleOfNode will be very unhappy.
			  Node root = node;
			  Node p = (root.NodeType == Node.ATTRIBUTE_NODE) ? ((org.w3c.dom.Attr)root).OwnerElement : root.ParentNode;
			  for (; p != null; p = p.ParentNode)
			  {
				root = p;
			  }
        
			  DOM2DTM dtm = (DOM2DTM) getDTM(new DOMSource(root), false, null, true, true);
        
			  int handle;
        
			  if (node is org.apache.xml.dtm.@ref.dom2dtm.DOM2DTMdefaultNamespaceDeclarationNode)
			  {
						// Can't return the same node since it's unique to a specific DTM, 
						// but can return the equivalent node -- find the corresponding 
						// Document Element, then ask it for the xml: namespace decl.
						handle = dtm.getHandleOfNode(((org.w3c.dom.Attr)node).OwnerElement);
						handle = dtm.getAttributeNode(handle,node.NamespaceURI,node.LocalName);
			  }
			  else
			  {
						handle = ((DOM2DTM)dtm).getHandleOfNode(node);
			  }
        
			  if (org.apache.xml.dtm.DTM_Fields.NULL == handle)
			  {
				throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_COULD_NOT_RESOLVE_NODE, null)); //"Could not resolve the node to a handle!");
			  }
        
			  return handle;
			}
		  }
	  }

	  /// <summary>
	  /// This method returns the SAX2 parser to use with the InputSource
	  /// obtained from this URI.
	  /// It may return null if any SAX2-conformant XML parser can be used,
	  /// or if getInputSource() will also return null. The parser must
	  /// be free for use (i.e., not currently in use for another parse().
	  /// After use of the parser is completed, the releaseXMLReader(XMLReader)
	  /// must be called.
	  /// </summary>
	  /// <param name="inputSource"> The value returned from the URIResolver. </param>
	  /// <returns>  a SAX2 XMLReader to use to resolve the inputSource argument.
	  /// </returns>
	  /// <returns> non-null XMLReader reference ready to parse. </returns>
	  public virtual XMLReader getXMLReader(Source inputSource)
	  {
		  lock (this)
		  {
        
			try
			{
			  XMLReader reader = (inputSource is SAXSource) ? ((SAXSource) inputSource).XMLReader : null;
        
			  // If user did not supply a reader, ask for one from the reader manager
			  if (null == reader)
			  {
				if (m_readerManager == null)
				{
					m_readerManager = XMLReaderManager.Instance;
				}
        
				reader = m_readerManager.XMLReader;
			  }
        
			  return reader;
        
			}
			catch (SAXException se)
			{
			  throw new DTMException(se.Message, se);
			}
		  }
	  }

	  /// <summary>
	  /// Indicates that the XMLReader object is no longer in use for the transform.
	  /// 
	  /// Note that the getXMLReader method may return an XMLReader that was
	  /// specified on the SAXSource object by the application code.  Such a
	  /// reader should still be passed to releaseXMLReader, but the reader manager
	  /// will only re-use XMLReaders that it created.
	  /// </summary>
	  /// <param name="reader"> The XMLReader to be released. </param>
	  public virtual void releaseXMLReader(XMLReader reader)
	  {
		  lock (this)
		  {
			if (m_readerManager != null)
			{
			  m_readerManager.releaseXMLReader(reader);
			}
		  }
	  }

	  /// <summary>
	  /// Return the DTM object containing a representation of this node.
	  /// </summary>
	  /// <param name="nodeHandle"> DTM Handle indicating which node to retrieve
	  /// </param>
	  /// <returns> a reference to the DTM object containing this node. </returns>
	  public override DTM getDTM(int nodeHandle)
	  {
		  lock (this)
		  {
			try
			{
			  // Performance critical function.
			  return m_dtms[(int)((uint)nodeHandle >> IDENT_DTM_NODE_BITS)];
			}
			catch (System.IndexOutOfRangeException e)
			{
			  if (nodeHandle == org.apache.xml.dtm.DTM_Fields.NULL)
			  {
						return null; // Accept as a special case.
			  }
			  else
			  {
						throw e; // Programming error; want to know about it.
			  }
			}
		  }
	  }

	  /// <summary>
	  /// Given a DTM, find the ID number in the DTM tables which addresses
	  /// the start of the document. If overflow addressing is in use, other
	  /// DTM IDs may also be assigned to this DTM.
	  /// </summary>
	  /// <param name="dtm"> The DTM which (hopefully) contains this node.
	  /// </param>
	  /// <returns> The DTM ID (as the high bits of a NodeHandle, not as our
	  /// internal index), or -1 if the DTM doesn't belong to this manager. </returns>
	  public override int getDTMIdentity(DTM dtm)
	  {
		  lock (this)
		  {
			// Shortcut using DTMDefaultBase's extension hooks
			// %REVIEW% Should the lookup be part of the basic DTM API?
			if (dtm is DTMDefaultBase)
			{
				DTMDefaultBase dtmdb = (DTMDefaultBase)dtm;
				if (dtmdb.Manager == this)
				{
					return dtmdb.DTMIDs.elementAt(0);
				}
				else
				{
					return -1;
				}
			}
        
			int n = m_dtms.Length;
        
			for (int i = 0; i < n; i++)
			{
			  DTM tdtm = m_dtms[i];
        
			  if (tdtm == dtm && m_dtm_offsets[i] == 0)
			  {
				return i << IDENT_DTM_NODE_BITS;
			  }
			}
        
			return -1;
		  }
	  }

	  /// <summary>
	  /// Release the DTMManager's reference(s) to a DTM, making it unmanaged.
	  /// This is typically done as part of returning the DTM to the heap after
	  /// we're done with it.
	  /// </summary>
	  /// <param name="dtm"> the DTM to be released.
	  /// </param>
	  /// <param name="shouldHardDelete"> If false, this call is a suggestion rather than an
	  /// order, and we may not actually release the DTM. This is intended to 
	  /// support intelligent caching of documents... which is not implemented
	  /// in this version of the DTM manager.
	  /// </param>
	  /// <returns> true if the DTM was released, false if shouldHardDelete was set
	  /// and we decided not to. </returns>
	  public override bool release(DTM dtm, bool shouldHardDelete)
	  {
		  lock (this)
		  {
			if (DEBUG)
			{
			  Console.WriteLine("Releasing " + (shouldHardDelete ? "HARD" : "soft") + " dtm=" + dtm.DocumentBaseURI);
					 // Following shouldn't need a nodeHandle, but does...
					 // and doesn't seem to report the intended value
			}
        
			if (dtm is SAX2DTM)
			{
			  ((SAX2DTM) dtm).clearCoRoutine();
			}
        
				// Multiple DTM IDs may be assigned to a single DTM. 
				// The Right Answer is to ask which (if it supports
				// extension, the DTM will need a list anyway). The 
				// Wrong Answer, applied if the DTM can't help us,
				// is to linearly search them all; this may be very
				// painful.
				//
				// %REVIEW% Should the lookup move up into the basic DTM API?
				if (dtm is DTMDefaultBase)
				{
					org.apache.xml.utils.SuballocatedIntVector ids = ((DTMDefaultBase)dtm).DTMIDs;
					for (int i = ids.size() - 1;i >= 0;--i)
					{
						m_dtms[(int)((uint)ids.elementAt(i) >> DTMManager.IDENT_DTM_NODE_BITS)] = null;
					}
				}
				else
				{
					int i = getDTMIdentity(dtm);
					if (i >= 0)
					{
						m_dtms[(int)((uint)i >> DTMManager.IDENT_DTM_NODE_BITS)] = null;
					}
				}
        
			dtm.documentRelease();
			return true;
		  }
	  }

	  /// <summary>
	  /// Method createDocumentFragment
	  /// 
	  /// 
	  /// NEEDSDOC (createDocumentFragment) @return
	  /// </summary>
	  public override DTM createDocumentFragment()
	  {
		  lock (this)
		  {
        
			try
			{
			  DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        
			  dbf.NamespaceAware = true;
        
			  DocumentBuilder db = dbf.newDocumentBuilder();
			  Document doc = db.newDocument();
			  Node df = doc.createDocumentFragment();
        
			  return getDTM(new DOMSource(df), true, null, false, false);
			}
			catch (Exception e)
			{
			  throw new DTMException(e);
			}
		  }
	  }

	  /// <summary>
	  /// NEEDSDOC Method createDTMIterator
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="whatToShow"> </param>
	  /// NEEDSDOC <param name="filter"> </param>
	  /// NEEDSDOC <param name="entityReferenceExpansion">
	  /// 
	  /// NEEDSDOC (createDTMIterator) @return </param>
	  public override DTMIterator createDTMIterator(int whatToShow, DTMFilter filter, bool entityReferenceExpansion)
	  {
		  lock (this)
		  {
        
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMManager abstract method </summary>
			return null;
		  }
	  }

	  /// <summary>
	  /// NEEDSDOC Method createDTMIterator
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="xpathString"> </param>
	  /// NEEDSDOC <param name="presolver">
	  /// 
	  /// NEEDSDOC (createDTMIterator) @return </param>
	  public override DTMIterator createDTMIterator(string xpathString, PrefixResolver presolver)
	  {
		  lock (this)
		  {
        
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMManager abstract method </summary>
			return null;
		  }
	  }

	  /// <summary>
	  /// NEEDSDOC Method createDTMIterator
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="node">
	  /// 
	  /// NEEDSDOC (createDTMIterator) @return </param>
	  public override DTMIterator createDTMIterator(int node)
	  {
		  lock (this)
		  {
        
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMManager abstract method </summary>
			return null;
		  }
	  }

	  /// <summary>
	  /// NEEDSDOC Method createDTMIterator
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="xpathCompiler"> </param>
	  /// NEEDSDOC <param name="pos">
	  /// 
	  /// NEEDSDOC (createDTMIterator) @return </param>
	  public override DTMIterator createDTMIterator(object xpathCompiler, int pos)
	  {
		  lock (this)
		  {
        
			/// <summary>
			/// @todo: implement this org.apache.xml.dtm.DTMManager abstract method </summary>
			return null;
		  }
	  }

	  /// <summary>
	  /// return the expanded name table.
	  /// </summary>
	  /// NEEDSDOC <param name="dtm">
	  /// 
	  /// NEEDSDOC ($objectName$) @return </param>
	  public virtual ExpandedNameTable getExpandedNameTable(DTM dtm)
	  {
		return m_expandedNameTable;
	  }
	}

}