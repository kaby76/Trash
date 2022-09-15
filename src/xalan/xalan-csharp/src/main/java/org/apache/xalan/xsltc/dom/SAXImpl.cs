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
 * $Id: SAXImpl.java 1225579 2011-12-29 15:56:06Z mrglavas $
 */

namespace org.apache.xalan.xsltc.dom
{


	using DOM = org.apache.xalan.xsltc.DOM;
	using DOMEnhancedForDTM = org.apache.xalan.xsltc.DOMEnhancedForDTM;
	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using DTMAxisIterNodeList = org.apache.xml.dtm.@ref.DTMAxisIterNodeList;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;
	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;
	using EmptyIterator = org.apache.xml.dtm.@ref.EmptyIterator;
	using SAX2DTM2 = org.apache.xml.dtm.@ref.sax2dtm.SAX2DTM2;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using ToXMLSAXHandler = org.apache.xml.serializer.ToXMLSAXHandler;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;
	using Document = org.w3c.dom.Document;
	using DocumentType = org.w3c.dom.DocumentType;
	using Entity = org.w3c.dom.Entity;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	/// SAXImpl is the core model for SAX input source. SAXImpl objects are
	/// usually created from an XSLTCDTMManager.
	/// 
	/// <para>DOMSource inputs are handled using DOM2SAX + SAXImpl. SAXImpl has a
	/// few specific fields (e.g. _node2Ids, _document) to keep DOM-related
	/// information. They are used when the processing behavior between DOM and
	/// SAX has to be different. Examples of these include id function and 
	/// unparsed entity.
	/// 
	/// </para>
	/// <para>SAXImpl extends SAX2DTM2 instead of SAX2DTM for better performance.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Douglas Sellers <douglasjsellers@hotmail.com>
	/// </para>
	/// </summary>
	public sealed class SAXImpl : SAX2DTM2, DOMEnhancedForDTM, DOMBuilder
	{

		/* ------------------------------------------------------------------- */
		/* DOMBuilder fields BEGIN                                             */
		/* ------------------------------------------------------------------- */

		// Namespace prefix-to-uri mapping stuff
		private int _uriCount = 0;
		private int _prefixCount = 0;

		// Stack used to keep track of what whitespace text nodes are protected
		// by xml:space="preserve" attributes and which nodes that are not.
		private int[] _xmlSpaceStack;
		private int _idx = 1;
		private bool _preserve = false;

		private const string XML_STRING = "xml:";
		private const string XML_PREFIX = "xml";
		private const string XMLSPACE_STRING = "xml:space";
		private const string PRESERVE_STRING = "preserve";
		private const string XMLNS_PREFIX = "xmlns";
		private const string XML_URI = "http://www.w3.org/XML/1998/namespace";

		private bool _escaping = true;
		private bool _disableEscaping = false;
		private int _textNodeToProcess = DTM.NULL;

		/* ------------------------------------------------------------------- */
		/* DOMBuilder fields END                                               */
		/* ------------------------------------------------------------------- */

		// empty String for null attribute values
		private const string EMPTYSTRING = "";

		// empty iterator to be returned when there are no children
		private static readonly DTMAxisIterator EMPTYITERATOR = EmptyIterator.Instance;
		// The number of expanded names
		private int _namesSize = -1;

		// Namespace related stuff
		private Hashtable _nsIndex = new Hashtable();

		// The initial size of the text buffer
		private int _size = 0;

		// Tracks which textnodes are not escaped
		private BitArray _dontEscape = null;

		// The URI to this document
		private string _documentURI = null;
		private static int _documentURIIndex = 0;

		// The owner Document when the input source is DOMSource.
		private Document _document;

		// The hashtable for org.w3c.dom.Node to node id mapping.
		// This is only used when the input is a DOMSource and the
		// buildIdIndex flag is true.
		private Hashtable _node2Ids = null;

		// True if the input source is a DOMSource.
		private bool _hasDOMSource = false;

		// The DTMManager
		private XSLTCDTMManager _dtmManager;

		// Support for access/navigation through org.w3c.dom API
		private Node[] _nodes;
		private NodeList[] _nodeLists;
		private const string XML_LANG_ATTRIBUTE = "http://www.w3.org/XML/1998/namespace:@lang";

		/// <summary>
		/// Define the origin of the document from which the tree was built
		/// </summary>
		public string DocumentURI
		{
			set
			{
				if (!string.ReferenceEquals(value, null))
				{
					DocumentBaseURI = SystemIDResolver.getAbsoluteURI(value);
				}
			}
			get
			{
				string baseURI = DocumentBaseURI;
				return (!string.ReferenceEquals(baseURI, null)) ? baseURI : "rtf" + _documentURIIndex++;
			}
		}


		public string getDocumentURI(int node)
		{
			return DocumentURI;
		}

		public void setupMapping(string[] names, string[] urisArray, int[] typesArray, string[] namespaces)
		{
			// This method only has a function in DOM adapters
		}

		/// <summary>
		/// Lookup a namespace URI from a prefix starting at node. This method
		/// is used in the execution of xsl:element when the prefix is not known
		/// at compile time.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws org.apache.xalan.xsltc.TransletException
		public string lookupNamespace(int node, string prefix)
		{
			int anode, nsnode;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AncestorIterator ancestors = new AncestorIterator();
			AncestorIterator ancestors = new AncestorIterator(this, this);

			if (isElement(node))
			{
				ancestors.includeSelf();
			}

			ancestors.StartNode = node;
			while ((anode = ancestors.next()) != DTM.NULL)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NamespaceIterator namespaces = new NamespaceIterator();
				NamespaceIterator namespaces = new NamespaceIterator(this);

				namespaces.StartNode = anode;
				while ((nsnode = namespaces.next()) != DTM.NULL)
				{
					if (getLocalName(nsnode).Equals(prefix))
					{
						return getNodeValue(nsnode);
					}
				}
			}

			BasisLibrary.runTimeError(BasisLibrary.NAMESPACE_PREFIX_ERR, prefix);
			return null;
		}

		/// <summary>
		/// Returns 'true' if a specific node is an element (of any type)
		/// </summary>
		public bool isElement(in int node)
		{
			return getNodeType(node) == DTM.ELEMENT_NODE;
		}

		/// <summary>
		/// Returns 'true' if a specific node is an attribute (of any type)
		/// </summary>
		public bool isAttribute(in int node)
		{
			return getNodeType(node) == DTM.ATTRIBUTE_NODE;
		}

		/// <summary>
		/// Returns the number of nodes in the tree (used for indexing)
		/// </summary>
		public int Size
		{
			get
			{
				return NumberOfNodes;
			}
		}

		/// <summary>
		/// Part of the DOM interface - no function here.
		/// </summary>
		public StripFilter Filter
		{
			set
			{
			}
		}


		/// <summary>
		/// Returns true if node1 comes before node2 in document order
		/// </summary>
		public bool lessThan(int node1, int node2)
		{
			if (node1 == DTM.NULL)
			{
				return false;
			}

			if (node2 == DTM.NULL)
			{
				return true;
			}

			return (node1 < node2);
		}

		/// <summary>
		/// Create an org.w3c.dom.Node from a node in the tree
		/// </summary>
		public Node makeNode(int index)
		{
			if (_nodes == null)
			{
				_nodes = new Node[_namesSize];
			}

			int nodeID = makeNodeIdentity(index);
			if (nodeID < 0)
			{
				return null;
			}
			else if (nodeID < _nodes.Length)
			{
				return (_nodes[nodeID] != null) ? _nodes[nodeID] : (_nodes[nodeID] = new DTMNodeProxy((DTM)this, index));
			}
			else
			{
				return new DTMNodeProxy((DTM)this, index);
			}
		}

		/// <summary>
		/// Create an org.w3c.dom.Node from a node in an iterator
		/// The iterator most be started before this method is called
		/// </summary>
		public Node makeNode(DTMAxisIterator iter)
		{
			return makeNode(iter.next());
		}

		/// <summary>
		/// Create an org.w3c.dom.NodeList from a node in the tree
		/// </summary>
		public NodeList makeNodeList(int index)
		{
			if (_nodeLists == null)
			{
				_nodeLists = new NodeList[_namesSize];
			}

			int nodeID = makeNodeIdentity(index);
			if (nodeID < 0)
			{
				return null;
			}
			else if (nodeID < _nodeLists.Length)
			{
				return (_nodeLists[nodeID] != null) ? _nodeLists[nodeID] : (_nodeLists[nodeID] = new DTMAxisIterNodeList(this, new SingletonIterator(this, index)));
			}
			else
			{
				return new DTMAxisIterNodeList(this, new SingletonIterator(this, index));
			}
		}

		/// <summary>
		/// Create an org.w3c.dom.NodeList from a node iterator
		/// The iterator most be started before this method is called
		/// </summary>
		public NodeList makeNodeList(DTMAxisIterator iter)
		{
			return new DTMAxisIterNodeList(this, iter);
		}

		/// <summary>
		/// Iterator that returns the namespace nodes as defined by the XPath data
		/// model for a given node, filtered by extended type ID.
		/// </summary>
		public class TypedNamespaceIterator : NamespaceIterator
		{
			private readonly SAXImpl outerInstance;


			internal string _nsPrefix;

			/// <summary>
			/// Constructor TypedChildrenIterator
			/// 
			/// </summary>
			/// <param name="nodeType"> The extended type ID being requested. </param>
			public TypedNamespaceIterator(SAXImpl outerInstance, int nodeType) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				if (outerInstance.m_expandedNameTable != null)
				{
					_nsPrefix = outerInstance.m_expandedNameTable.getLocalName(nodeType);
				}
			}

		   /// <summary>
		   /// Get the next node in the iteration.
		   /// </summary>
		   /// <returns> The next node handle in the iteration, or END. </returns>
			public override int next()
			{
				if ((string.ReferenceEquals(_nsPrefix, null)) || (_nsPrefix.Length == 0))
				{
					return (END);
				}
				int node = END;
				for (node = base.next(); node != END; node = base.next())
				{
					if (string.CompareOrdinal(_nsPrefix, outerInstance.getLocalName(node)) == 0)
					{
						return returnNode(node);
					}
				}
				return (END);
			}
		} // end of TypedNamespaceIterator



		/// <summary>
		///************************************************************
		/// This is a specialised iterator for predicates comparing node or
		/// attribute values to variable or parameter values.
		/// </summary>
		private sealed class NodeValueIterator : InternalAxisIteratorBase
		{
			private readonly SAXImpl outerInstance;


		internal DTMAxisIterator _source;
		internal string _value;
		internal bool _op;
		internal readonly bool _isReverse;
		internal int _returnType = RETURN_PARENT;

		public NodeValueIterator(SAXImpl outerInstance, DTMAxisIterator source, int returnType, string value, bool op)
		{
			this.outerInstance = outerInstance;
			_source = source;
			_returnType = returnType;
			_value = value;
			_op = op;
			_isReverse = source.Reverse;
		}

		public override bool Reverse
		{
			get
			{
				return _isReverse;
			}
		}

			public override DTMAxisIterator cloneIterator()
			{
				try
				{
					NodeValueIterator clone = (NodeValueIterator)base.clone();
					clone._isRestartable = false;
					clone._source = _source.cloneIterator();
					clone._value = _value;
					clone._op = _op;
					return clone.reset();
				}
				catch (CloneNotSupportedException e)
				{
					BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
					return null;
				}
			}

			public override bool Restartable
			{
				set
				{
				_isRestartable = value;
				_source.Restartable = value;
				}
			}

		public override DTMAxisIterator reset()
		{
			_source.reset();
			return resetPosition();
		}

		public override int next()
		{
				int node;
				while ((node = _source.next()) != END)
				{
					string val = outerInstance.getStringValueX(node);
					if (_value.Equals(val) == _op)
					{
						if (_returnType == RETURN_CURRENT)
						{
							return returnNode(node);
						}
						else
						{
							return returnNode(outerInstance.getParent(node));
						}
					}
				}
				return END;
		}

		public override DTMAxisIterator setStartNode(int node)
		{
				if (_isRestartable)
				{
					_source.StartNode = _startNode = node;
					return resetPosition();
				}
				return this;
		}

		public override void setMark()
		{
			_source.setMark();
		}

		public override void gotoMark()
		{
			_source.gotoMark();
		}
		} // end NodeValueIterator

		public DTMAxisIterator getNodeValueIterator(DTMAxisIterator iterator, int type, string value, bool op)
		{
			return (DTMAxisIterator)(new NodeValueIterator(this, iterator, type, value, op));
		}

		/// <summary>
		/// Encapsulates an iterator in an OrderedIterator to ensure node order
		/// </summary>
		public DTMAxisIterator orderNodes(DTMAxisIterator source, int node)
		{
			return new DupFilterIterator(source);
		}

		/// <summary>
		/// Returns singleton iterator containg the document root
		/// Works for them main document (mark == 0).  It cannot be made
		/// to point to any other node through setStartNode().
		/// </summary>
		public DTMAxisIterator Iterator
		{
			get
			{
				return new SingletonIterator(this, Document, true);
			}
		}

		 /// <summary>
		 /// Get mapping from DOM namespace types to external namespace types
		 /// </summary>
		public int getNSType(int node)
		{
			string s = getNamespaceURI(node);
			if (string.ReferenceEquals(s, null))
			{
				return 0;
			}
			int eType = getIdForNamespace(s);
			return ((int?)_nsIndex.get(new int?(eType))).Value;
		}



		/// <summary>
		/// Returns the namespace type of a specific node
		/// </summary>
		public override int getNamespaceType(in int node)
		{
			return base.getNamespaceType(node);
		}

		/// <summary>
		/// Sets up a translet-to-dom type mapping table
		/// </summary>
		private int[] setupMapping(string[] names, string[] uris, int[] types, int nNames)
		{
			// Padding with number of names, because they
			// may need to be added, i.e for RTFs. See copy03  
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] result = new int[m_expandedNameTable.getSize()];
			int[] result = new int[m_expandedNameTable.Size];
			for (int i = 0; i < nNames; i++)
			{
				//int type = getGeneralizedType(namesArray[i]);
				int type = m_expandedNameTable.getExpandedTypeID(uris[i], names[i], types[i], false);
				result[type] = type;
			}
			return result;
		}

		/// <summary>
		/// Returns the internal type associated with an expanded QName
		/// </summary>
		public int getGeneralizedType(in string name)
		{
			return getGeneralizedType(name, true);
		}

		/// <summary>
		/// Returns the internal type associated with an expanded QName
		/// </summary>
		public int getGeneralizedType(in string name, bool searchOnly)
		{
			string lName, ns = null;
			int index = -1;
			int code;

			// Is there a prefix?
			if ((index = name.LastIndexOf(':')) > -1)
			{
				ns = name.Substring(0, index);
			}

			// Local part of name is after colon.  lastIndexOf returns -1 if
			// there is no colon, so lNameStartIdx will be zero in that case.
			int lNameStartIdx = index + 1;

			// Distinguish attribute and element names.  Attribute has @ before
			// local part of name.
			if (name[lNameStartIdx] == '@')
			{
				code = DTM.ATTRIBUTE_NODE;
				lNameStartIdx++;
			}
			else
			{
				code = DTM.ELEMENT_NODE;
			}

			// Extract local name
			lName = (lNameStartIdx == 0) ? name : name.Substring(lNameStartIdx);

			return m_expandedNameTable.getExpandedTypeID(ns, lName, code, searchOnly);
		}

		/// <summary>
		/// Get mapping from DOM element/attribute types to external types
		/// </summary>
		public short[] getMapping(string[] names, string[] uris, int[] types)
		{
			// Delegate the work to getMapping2 if the document is not fully built.
			// Some of the processing has to be different in this case.
			if (_namesSize < 0)
			{
				return getMapping2(names, uris, types);
			}

			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int namesLength = names.length;
			int namesLength = names.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int exLength = m_expandedNameTable.getSize();
			int exLength = m_expandedNameTable.Size;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final short[] result = new short[exLength];
			short[] result = new short[exLength];

			// primitive types map to themselves
			for (i = 0; i < DTM.NTYPES; i++)
			{
				result[i] = (short)i;
			}

			for (i = NTYPES; i < exLength; i++)
			{
				  result[i] = m_expandedNameTable.getType(i);
			}

			// actual mapping of caller requested names
			for (i = 0; i < namesLength; i++)
			{
				int genType = m_expandedNameTable.getExpandedTypeID(uris[i], names[i], types[i], true);
				if (genType >= 0 && genType < exLength)
				{
					result[genType] = (short)(i + DTM.NTYPES);
				}
			}

			return result;
		}

		/// <summary>
		/// Get mapping from external element/attribute types to DOM types
		/// </summary>
		public int[] getReverseMapping(string[] names, string[] uris, int[] types)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] result = new int[names.length + org.apache.xml.dtm.DTM.NTYPES];
			int[] result = new int[names.Length + DTM.NTYPES];

			// primitive types map to themselves
			for (i = 0; i < DTM.NTYPES; i++)
			{
				result[i] = i;
			}

			// caller's types map into appropriate dom types
			for (i = 0; i < names.Length; i++)
			{
				int type = m_expandedNameTable.getExpandedTypeID(uris[i], names[i], types[i], true);
				result[i + DTM.NTYPES] = type;
			}
			return (result);
		}

		/// <summary>
		/// Get mapping from DOM element/attribute types to external types.
		/// This method is used when the document is not fully built.
		/// </summary>
		private short[] getMapping2(string[] names, string[] uris, int[] types)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int namesLength = names.length;
			int namesLength = names.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int exLength = m_expandedNameTable.getSize();
			int exLength = m_expandedNameTable.Size;
			int[] generalizedTypes = null;
			if (namesLength > 0)
			{
				generalizedTypes = new int[namesLength];
			}

			int resultLength = exLength;

			for (i = 0; i < namesLength; i++)
			{
				// When the document is not fully built, the searchOnly
				// flag should be set to false. That means we should add
				// the type if it is not already in the expanded name table.
				//generalizedTypes[i] = getGeneralizedType(names[i], false);
				generalizedTypes[i] = m_expandedNameTable.getExpandedTypeID(uris[i], names[i], types[i], false);
				if (_namesSize < 0 && generalizedTypes[i] >= resultLength)
				{
					resultLength = generalizedTypes[i] + 1;
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final short[] result = new short[resultLength];
			short[] result = new short[resultLength];

			// primitive types map to themselves
			for (i = 0; i < DTM.NTYPES; i++)
			{
				result[i] = (short)i;
			}

			for (i = NTYPES; i < exLength; i++)
			{
				result[i] = m_expandedNameTable.getType(i);
			}

			// actual mapping of caller requested names
			for (i = 0; i < namesLength; i++)
			{
				int genType = generalizedTypes[i];
				if (genType >= 0 && genType < resultLength)
				{
					result[genType] = (short)(i + DTM.NTYPES);
				}
			}

			return (result);
		}
		/// <summary>
		/// Get mapping from DOM namespace types to external namespace types
		/// </summary>
		public short[] getNamespaceMapping(string[] namespaces)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nsLength = namespaces.length;
			int nsLength = namespaces.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mappingLength = _uriCount;
			int mappingLength = _uriCount;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final short[] result = new short[mappingLength];
			short[] result = new short[mappingLength];

			// Initialize all entries to -1
			for (i = 0; i < mappingLength; i++)
			{
				result[i] = (short)(-1);
			}

			for (i = 0; i < nsLength; i++)
			{
				int eType = getIdForNamespace(namespaces[i]);
				int? type = (int?)_nsIndex.get(new int?(eType));
				if (type != null)
				{
					result[type.Value] = (short)i;
				}
			}

			return (result);
		}

		/// <summary>
		/// Get mapping from external namespace types to DOM namespace types
		/// </summary>
		public short[] getReverseNamespaceMapping(string[] namespaces)
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = namespaces.length;
			int length = namespaces.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final short[] result = new short[length];
			short[] result = new short[length];

			for (i = 0; i < length; i++)
			{
				int eType = getIdForNamespace(namespaces[i]);
				int? type = (int?)_nsIndex.get(new int?(eType));
				result[i] = (type == null) ? -1 : (short)type.Value;
			}

			return result;
		}

		/// <summary>
		/// Construct a SAXImpl object using the default block size.
		/// </summary>
		public SAXImpl(XSLTCDTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, bool buildIdIndex) : this(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, DEFAULT_BLOCKSIZE, buildIdIndex, false)
		{
		}

		/// <summary>
		/// Construct a SAXImpl object using the given block size.
		/// </summary>
		public SAXImpl(XSLTCDTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool buildIdIndex, bool newNameTable) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, blocksize, false, buildIdIndex, newNameTable)
		{

			_dtmManager = mgr;
			_size = blocksize;

			// Use a smaller size for the space stack if the blocksize is small
			_xmlSpaceStack = new int[blocksize <= 64 ? 4 : 64];

			/* From DOMBuilder */ 
			_xmlSpaceStack[0] = DTMDefaultBase.ROOTNODE;

			// If the input source is DOMSource, set the _document field and
			// create the node2Ids table.
			if (source is DOMSource)
			{
				_hasDOMSource = true;
				DOMSource domsrc = (DOMSource)source;
				Node node = domsrc.getNode();
				if (node is Document)
				{
					_document = (Document)node;
				}
				else
				{
					_document = node.getOwnerDocument();
				}
				_node2Ids = new Hashtable();
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
			if (manager is XSLTCDTMManager)
			{
				_dtmManager = (XSLTCDTMManager)manager;
			}
		}

		/// <summary>
		/// Return the node identity for a given id String
		/// </summary>
		/// <param name="idString"> The id String </param>
		/// <returns> The identity of the node whose id is the given String. </returns>
		public override int getElementById(string idString)
		{
			Node node = _document.getElementById(idString);
			if (node != null)
			{
				int? id = (int?)_node2Ids.get(node);
				return (id != null) ? id.Value : DTM.NULL;
			}
			else
			{
				return DTM.NULL;
			}
		}

		/// <summary>
		/// Return true if the input source is DOMSource.
		/// </summary>
		public bool hasDOMSource()
		{
			return _hasDOMSource;
		}

		/*---------------------------------------------------------------------------*/
		/* DOMBuilder methods begin                                                  */
		/*---------------------------------------------------------------------------*/

		/// <summary>
		/// Call this when an xml:space attribute is encountered to
		/// define the whitespace strip/preserve settings.
		/// </summary>
		private void xmlSpaceDefine(string val, in int node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean setting = val.equals(PRESERVE_STRING);
			bool setting = val.Equals(PRESERVE_STRING);
			if (setting != _preserve)
			{
				_xmlSpaceStack[_idx++] = node;
				_preserve = setting;
			}
		}

		/// <summary>
		/// Call this from endElement() to revert strip/preserve setting
		/// to whatever it was before the corresponding startElement().
		/// </summary>
		private void xmlSpaceRevert(in int node)
		{
			if (node == _xmlSpaceStack[_idx - 1])
			{
				_idx--;
				_preserve = !_preserve;
			}
		}

		/// <summary>
		/// Find out whether or not to strip whitespace nodes.
		/// 
		/// </summary>
		/// <returns> whether or not to strip whitespace nodes. </returns>
		protected internal override bool ShouldStripWhitespace
		{
			get
			{
				return _preserve ? false : base.ShouldStripWhitespace;
			}
		}

		/// <summary>
		/// Creates a text-node and checks if it is a whitespace node.
		/// </summary>
		private void handleTextEscaping()
		{
			if (_disableEscaping && _textNodeToProcess != DTM.NULL && _type(_textNodeToProcess) == DTM.TEXT_NODE)
			{
				if (_dontEscape == null)
				{
					_dontEscape = new BitArray(_size);
				}

				// Resize the _dontEscape BitArray if necessary.
				if (_textNodeToProcess >= _dontEscape.size())
				{
					_dontEscape.resize(_dontEscape.size() * 2);
				}

				_dontEscape.Bit = _textNodeToProcess;
				_disableEscaping = false;
			}
			_textNodeToProcess = DTM.NULL;
		}


		/// <summary>
		///************************************************************* </summary>
		/*               SAX Interface Starts Here                      */
		/// <summary>
		///************************************************************* </summary>

		/// <summary>
		/// SAX2: Receive notification of character data.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public override void characters(char[] ch, int start, int length)
		{
			base.characters(ch, start, length);

			_disableEscaping = !_escaping;
			_textNodeToProcess = NumberOfNodes;
		}

		/// <summary>
		/// SAX2: Receive notification of the beginning of a document.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public override void startDocument()
		{
			base.startDocument();

			_nsIndex.put(new int?(0), new int?(_uriCount++));
			definePrefixAndUri(XML_PREFIX, XML_URI);
		}

		/// <summary>
		/// SAX2: Receive notification of the end of a document.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public override void endDocument()
		{
			base.endDocument();

			handleTextEscaping();
			_namesSize = m_expandedNameTable.Size;
		}

		/// <summary>
		/// Specialized interface used by DOM2SAX. This one has an extra Node
		/// parameter to build the Node -> id map.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qname, org.xml.sax.Attributes attributes, org.w3c.dom.Node node) throws org.xml.sax.SAXException
		public void startElement(string uri, string localName, string qname, Attributes attributes, Node node)
		{
			this.startElement(uri, localName, qname, attributes);

			if (m_buildIdIndex)
			{
				_node2Ids.put(node, new int?(m_parents.peek()));
			}
		}

		/// <summary>
		/// SAX2: Receive notification of the beginning of an element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qname, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
		public void startElement(string uri, string localName, string qname, Attributes attributes)
		{
			base.startElement(uri, localName, qname, attributes);

			handleTextEscaping();

			if (m_wsfilter != null)
			{
				// Look for any xml:space attributes
				// Depending on the implementation of attributes, this
				// might be faster than looping through all attributes. ILENE
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = attributes.getIndex(XMLSPACE_STRING);
				int index = attributes.getIndex(XMLSPACE_STRING);
				if (index >= 0)
				{
					xmlSpaceDefine(attributes.getValue(index), m_parents.peek());
				}
			}
		}

		/// <summary>
		/// SAX2: Receive notification of the end of an element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String qname) throws org.xml.sax.SAXException
		public override void endElement(string namespaceURI, string localName, string qname)
		{
			base.endElement(namespaceURI, localName, qname);

			handleTextEscaping();

			// Revert to strip/preserve-space setting from before this element
			if (m_wsfilter != null)
			{
				xmlSpaceRevert(m_previous);
			}
		}

		/// <summary>
		/// SAX2: Receive notification of a processing instruction.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public override void processingInstruction(string target, string data)
		{
			base.processingInstruction(target, data);
			handleTextEscaping();
		}

		/// <summary>
		/// SAX2: Receive notification of ignorable whitespace in element
		/// content. Similar to characters(char[], int, int).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public override void ignorableWhitespace(char[] ch, int start, int length)
		{
			base.ignorableWhitespace(ch, start, length);
			_textNodeToProcess = NumberOfNodes;
		}

		/// <summary>
		/// SAX2: Begin the scope of a prefix-URI Namespace mapping.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public override void startPrefixMapping(string prefix, string uri)
		{
			base.startPrefixMapping(prefix, uri);
			handleTextEscaping();

			definePrefixAndUri(prefix, uri);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void definePrefixAndUri(String prefix, String uri) throws org.xml.sax.SAXException
		private void definePrefixAndUri(string prefix, string uri)
		{
			// Check if the URI already exists before pushing on stack
			int? eType = new int?(getIdForNamespace(uri));
			if ((int?)_nsIndex.get(eType.Value) == null)
			{
				_nsIndex.put(eType.Value, new int?(_uriCount++));
			}
		}

		/// <summary>
		/// SAX2: Report an XML comment anywhere in the document.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public override void comment(char[] ch, int start, int length)
		{
			base.comment(ch, start, length);
			handleTextEscaping();
		}

		public bool setEscaping(bool value)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _escaping;
			bool temp = _escaping;
			_escaping = value;
			return temp;
		}

	   /*---------------------------------------------------------------------------*/
	   /* DOMBuilder methods end                                                    */
	   /*---------------------------------------------------------------------------*/

		/// <summary>
		/// Prints the whole tree to standard output
		/// </summary>
		public void print(int node, int level)
		{
			switch (getNodeType(node))
			{
			case DTM.ROOT_NODE:
			case DTM.DOCUMENT_NODE:
				print(getFirstChild(node), level);
				break;
			case DTM.TEXT_NODE:
			case DTM.COMMENT_NODE:
			case DTM.PROCESSING_INSTRUCTION_NODE:
				Console.Write(getStringValueX(node));
				break;
			default:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getNodeName(node);
				string name = getNodeName(node);
				Console.Write("<" + name);
				for (int a = getFirstAttribute(node); a != DTM.NULL; a = getNextAttribute(a))
				{
				Console.Write("\n" + getNodeName(a) + "=\"" + getStringValueX(a) + "\"");
				}
				Console.Write('>');
				for (int child = getFirstChild(node); child != DTM.NULL; child = getNextSibling(child))
				{
				print(child, level + 1);
				}
				Console.WriteLine("</" + name + '>');
				break;
			}
		}

		/// <summary>
		/// Returns the name of a node (attribute or element).
		/// </summary>
		public override string getNodeName(in int node)
		{
		// Get the node type and make sure that it is within limits
		int nodeh = node;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final short type = getNodeType(nodeh);
		short type = getNodeType(nodeh);
		switch (type)
		{
			case DTM.ROOT_NODE:
			case DTM.DOCUMENT_NODE:
			case DTM.TEXT_NODE:
			case DTM.COMMENT_NODE:
				return EMPTYSTRING;
			case DTM.NAMESPACE_NODE:
			return this.getLocalName(nodeh);
			default:
				return base.getNodeName(nodeh);
		}
		}

		/// <summary>
		/// Returns the namespace URI to which a node belongs
		/// </summary>
		public string getNamespaceName(in int node)
		{
			if (node == DTM.NULL)
			{
				return "";
			}

			string s;
			return string.ReferenceEquals((s = getNamespaceURI(node)), null) ? EMPTYSTRING : s;
		}


		/// <summary>
		/// Returns the attribute node of a given type (if any) for an element
		/// </summary>
		public int getAttributeNode(in int type, in int element)
		{
			for (int attr = getFirstAttribute(element); attr != DTM.NULL; attr = getNextAttribute(attr))
			{
				if (getExpandedTypeID(attr) == type)
				{
					return attr;
				}
			}
			return DTM.NULL;
		}

		/// <summary>
		/// Returns the value of a given attribute type of a given element
		/// </summary>
		public string getAttributeValue(in int type, in int element)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int attr = getAttributeNode(type, element);
			int attr = getAttributeNode(type, element);
			return (attr != DTM.NULL) ? getStringValueX(attr) : EMPTYSTRING;
		}

		/// <summary>
		/// This method is for testing/debugging only
		/// </summary>
		public string getAttributeValue(in string name, in int element)
		{
			return getAttributeValue(getGeneralizedType(name), element);
		}

		/// <summary>
		/// Returns an iterator with all the children of a given node
		/// </summary>
		public DTMAxisIterator getChildren(in int node)
		{
			return (new ChildrenIterator(this, this)).setStartNode(node);
		}

		/// <summary>
		/// Returns an iterator with all children of a specific type
		/// for a given node (element)
		/// </summary>
		public DTMAxisIterator getTypedChildren(in int type)
		{
			return (new TypedChildrenIterator(this, this, type));
		}

		/// <summary>
		/// This is a shortcut to the iterators that implement the
		/// supported XPath axes (only namespace::) is not supported.
		/// Returns a bare-bones iterator that must be initialized
		/// with a start node (using iterator.setStartNode()).
		/// </summary>
		public override DTMAxisIterator getAxisIterator(in int axis)
		{
			switch (axis)
			{
				case Axis.SELF:
					return new SingletonIterator(this);
				case Axis.CHILD:
					return new ChildrenIterator(this, this);
				case Axis.PARENT:
					return new ParentIterator(this, this);
				case Axis.ANCESTOR:
					return new AncestorIterator(this, this);
				case Axis.ANCESTORORSELF:
					return (new AncestorIterator(this, this)).includeSelf();
				case Axis.ATTRIBUTE:
					return new AttributeIterator(this, this);
				case Axis.DESCENDANT:
					return new DescendantIterator(this, this);
				case Axis.DESCENDANTORSELF:
					return (new DescendantIterator(this, this)).includeSelf();
				case Axis.FOLLOWING:
					return new FollowingIterator(this, this);
				case Axis.PRECEDING:
					return new PrecedingIterator(this, this);
				case Axis.FOLLOWINGSIBLING:
					return new FollowingSiblingIterator(this, this);
				case Axis.PRECEDINGSIBLING:
					return new PrecedingSiblingIterator(this, this);
				case Axis.NAMESPACE:
					return new NamespaceIterator(this);
				case Axis.ROOT:
					return new RootIterator(this);
				default:
					BasisLibrary.runTimeError(BasisLibrary.AXIS_SUPPORT_ERR, Axis.getNames(axis));
				break;
			}
			return null;
		}

		/// <summary>
		/// Similar to getAxisIterator, but this one returns an iterator
		/// containing nodes of a typed axis (ex.: child::foo)
		/// </summary>
		public override DTMAxisIterator getTypedAxisIterator(int axis, int type)
		{
			// Most common case handled first
			if (axis == Axis.CHILD)
			{
				return new TypedChildrenIterator(this, this, type);
			}

			if (type == NO_TYPE)
			{
				return (EMPTYITERATOR);
			}

			switch (axis)
			{
				case Axis.SELF:
					return new TypedSingletonIterator(this, this, type);
				case Axis.CHILD:
					return new TypedChildrenIterator(this, this, type);
				case Axis.PARENT:
					return (new ParentIterator(this, this)).setNodeType(type);
				case Axis.ANCESTOR:
					return new TypedAncestorIterator(this, this, type);
				case Axis.ANCESTORORSELF:
					return (new TypedAncestorIterator(this, this, type)).includeSelf();
				case Axis.ATTRIBUTE:
					return new TypedAttributeIterator(this, this, type);
				case Axis.DESCENDANT:
					return new TypedDescendantIterator(this, this, type);
				case Axis.DESCENDANTORSELF:
					return (new TypedDescendantIterator(this, this, type)).includeSelf();
				case Axis.FOLLOWING:
					return new TypedFollowingIterator(this, this, type);
				case Axis.PRECEDING:
					return new TypedPrecedingIterator(this, this, type);
				case Axis.FOLLOWINGSIBLING:
					return new TypedFollowingSiblingIterator(this, this, type);
				case Axis.PRECEDINGSIBLING:
					return new TypedPrecedingSiblingIterator(this, this, type);
				case Axis.NAMESPACE:
					return new TypedNamespaceIterator(this, this, type);
				case Axis.ROOT:
					return new TypedRootIterator(this, this, type);
				default:
					BasisLibrary.runTimeError(BasisLibrary.TYPED_AXIS_SUPPORT_ERR, Axis.getNames(axis));
				break;
			}
			return null;
		}

		/// <summary>
		/// Do not think that this returns an iterator for the namespace axis.
		/// It returns an iterator with nodes that belong in a certain namespace,
		/// such as with <xsl:apply-templates select="blob/foo:*"/>
		/// The 'axis' specifies the axis for the base iterator from which the
		/// nodes are taken, while 'ns' specifies the namespace URI type.
		/// </summary>
		public DTMAxisIterator getNamespaceAxisIterator(int axis, int ns)
		{

			DTMAxisIterator iterator = null;

			if (ns == NO_TYPE)
			{
				return EMPTYITERATOR;
			}
			else
			{
				switch (axis)
				{
					case Axis.CHILD:
						return new NamespaceChildrenIterator(this, this, ns);
					case Axis.ATTRIBUTE:
						return new NamespaceAttributeIterator(this, this, ns);
					default:
						return new NamespaceWildcardIterator(this, axis, ns);
				}
			}
		}

		/// <summary>
		/// Iterator that handles node tests that test for a namespace, but have
		/// a wild card for the local name of the node, i.e., node tests of the
		/// form <axis>::<prefix>:*
		/// </summary>
		public sealed class NamespaceWildcardIterator : InternalAxisIteratorBase
		{
			private readonly SAXImpl outerInstance;

			/// <summary>
			/// The namespace type index.
			/// </summary>
			protected internal int m_nsType;

			/// <summary>
			/// A nested typed axis iterator that retrieves nodes of the principal
			/// node kind for that axis.
			/// </summary>
			protected internal DTMAxisIterator m_baseIterator;

			/// <summary>
			/// Constructor NamespaceWildcard
			/// </summary>
			/// <param name="axis"> The axis that this iterator will traverse </param>
			/// <param name="nsType"> The namespace type index </param>
			public NamespaceWildcardIterator(SAXImpl outerInstance, int axis, int nsType)
			{
				this.outerInstance = outerInstance;
				m_nsType = nsType;

				// Create a nested iterator that will select nodes of
				// the principal node kind for the selected axis.
				switch (axis)
				{
					case Axis.ATTRIBUTE:
					{
						// For "attribute::p:*", the principal node kind is
						// attribute
						m_baseIterator = outerInstance.getAxisIterator(axis);
					}
						goto case org.apache.xml.dtm.Axis.NAMESPACE;
					case Axis.NAMESPACE:
					{
						// This covers "namespace::p:*".  It is syntactically
						// correct, though it doesn't make much sense.
						m_baseIterator = outerInstance.getAxisIterator(axis);
					}
						goto default;
					default:
					{
						// In all other cases, the principal node kind is
						// element
						m_baseIterator = outerInstance.getTypedAxisIterator(axis, DTM.ELEMENT_NODE);
					}
				break;
				}
			}

			/// <summary>
			/// Set start to END should 'close' the iterator,
			/// i.e. subsequent call to next() should return END.
			/// </summary>
			/// <param name="node"> Sets the root of the iteration.
			/// </param>
			/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
			public override DTMAxisIterator setStartNode(int node)
			{
				if (_isRestartable)
				{
					_startNode = node;
					m_baseIterator.StartNode = node;
					resetPosition();
				}
				return this;
			}

			/// <summary>
			/// Get the next node in the iteration.
			/// </summary>
			/// <returns> The next node handle in the iteration, or END. </returns>
			public override int next()
			{
				int node;

				while ((node = m_baseIterator.next()) != END)
				{
					// Return only nodes that are in the selected namespace
					if (outerInstance.getNSType(node) == m_nsType)
					{
						return returnNode(node);
					}
				}

				return END;
			}

			/// <summary>
			/// Returns a deep copy of this iterator.  The cloned iterator is not
			/// reset.
			/// </summary>
			/// <returns> a deep copy of this iterator. </returns>
			public override DTMAxisIterator cloneIterator()
			{
				try
				{
					DTMAxisIterator nestedClone = m_baseIterator.cloneIterator();
					NamespaceWildcardIterator clone = (NamespaceWildcardIterator) base.clone();

					clone.m_baseIterator = nestedClone;
					clone.m_nsType = m_nsType;
					clone._isRestartable = false;

					return clone;
				}
				catch (CloneNotSupportedException e)
				{
					BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
					return null;
				}
			}

			/// <summary>
			/// True if this iterator has a reversed axis.
			/// </summary>
			/// <returns> <code>true</code> if this iterator is a reversed axis. </returns>
			public override bool Reverse
			{
				get
				{
					return m_baseIterator.Reverse;
				}
			}

			public override void setMark()
			{
				m_baseIterator.setMark();
			}

			public override void gotoMark()
			{
				m_baseIterator.gotoMark();
			}
		}

		/// <summary>
		/// Iterator that returns children within a given namespace for a
		/// given node. The functionality chould be achieved by putting a
		/// filter on top of a basic child iterator, but a specialised
		/// iterator is used for efficiency (both speed and size of translet).
		/// </summary>
		public sealed class NamespaceChildrenIterator : InternalAxisIteratorBase
		{
			private readonly SAXImpl outerInstance;


			/// <summary>
			/// The extended type ID being requested. </summary>
			internal readonly int _nsType;

			/// <summary>
			/// Constructor NamespaceChildrenIterator
			/// 
			/// </summary>
			/// <param name="type"> The extended type ID being requested. </param>
			public NamespaceChildrenIterator(SAXImpl outerInstance, in int type)
			{
				this.outerInstance = outerInstance;
				_nsType = type;
			}

			/// <summary>
			/// Set start to END should 'close' the iterator,
			/// i.e. subsequent call to next() should return END.
			/// </summary>
			/// <param name="node"> Sets the root of the iteration.
			/// </param>
			/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
			public override DTMAxisIterator setStartNode(int node)
			{
				//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
				if (node == DTMDefaultBase.ROOTNODE)
				{
					node = outerInstance.Document;
				}

				if (_isRestartable)
				{
					_startNode = node;
					_currentNode = (node == DTM.NULL) ? DTM.NULL : NOTPROCESSED;

					return resetPosition();
				}

				return this;
			}

			/// <summary>
			/// Get the next node in the iteration.
			/// </summary>
			/// <returns> The next node handle in the iteration, or END. </returns>
			public override int next()
			{
				if (_currentNode != DTM.NULL)
				{
					for (int node = (NOTPROCESSED == _currentNode) ? outerInstance._firstch(outerInstance.makeNodeIdentity(_startNode)) : outerInstance._nextsib(_currentNode); node != END; node = outerInstance._nextsib(node))
					{
						int nodeHandle = outerInstance.makeNodeHandle(node);

						if (outerInstance.getNSType(nodeHandle) == _nsType)
						{
							_currentNode = node;

							return returnNode(nodeHandle);
						}
					}
				}

				return END;
			}
		} // end of NamespaceChildrenIterator

		/// <summary>
		/// Iterator that returns attributes within a given namespace for a node.
		/// </summary>
		public sealed class NamespaceAttributeIterator : InternalAxisIteratorBase
		{
			private readonly SAXImpl outerInstance;


			/// <summary>
			/// The extended type ID being requested. </summary>
			internal readonly int _nsType;

			/// <summary>
			/// Constructor NamespaceAttributeIterator
			/// 
			/// </summary>
			/// <param name="nsType"> The extended type ID being requested. </param>
			public NamespaceAttributeIterator(SAXImpl outerInstance, int nsType) : base(outerInstance)
			{
				this.outerInstance = outerInstance;

				_nsType = nsType;
			}

			/// <summary>
			/// Set start to END should 'close' the iterator,
			/// i.e. subsequent call to next() should return END.
			/// </summary>
			/// <param name="node"> Sets the root of the iteration.
			/// </param>
			/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
			public override DTMAxisIterator setStartNode(int node)
			{
				//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
				if (node == DTMDefaultBase.ROOTNODE)
				{
					node = outerInstance.Document;
				}

				if (_isRestartable)
				{
					int nsType = _nsType;

					_startNode = node;

					for (node = outerInstance.getFirstAttribute(node); node != END; node = outerInstance.getNextAttribute(node))
					{
						if (outerInstance.getNSType(node) == nsType)
						{
							break;
						}
					}

					_currentNode = node;
					return resetPosition();
				}

				return this;
			}

			/// <summary>
			/// Get the next node in the iteration.
			/// </summary>
			/// <returns> The next node handle in the iteration, or END. </returns>
			public override int next()
			{
				int node = _currentNode;
				int nsType = _nsType;
				int nextNode;

				if (node == END)
				{
					return END;
				}

				for (nextNode = outerInstance.getNextAttribute(node); nextNode != END; nextNode = outerInstance.getNextAttribute(nextNode))
				{
					if (outerInstance.getNSType(nextNode) == nsType)
					{
						break;
					}
				}

				_currentNode = nextNode;

				return returnNode(node);
			}
		} // end of NamespaceAttributeIterator

		/// <summary>
		/// Returns an iterator with all descendants of a node that are of
		/// a given type.
		/// </summary>
		public DTMAxisIterator getTypedDescendantIterator(int type)
		{
			return new TypedDescendantIterator(this, this, type);
		}

		/// <summary>
		/// Returns the nth descendant of a node
		/// </summary>
		public DTMAxisIterator getNthDescendant(int type, int n, bool includeself)
		{
			DTMAxisIterator source = (DTMAxisIterator) new TypedDescendantIterator(this, this, type);
			return new NthDescendantIterator(this, n);
		}

		/// <summary>
		/// Copy the string value of a node directly to an output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void characters(in int node, SerializationHandler handler)
		{
			if (node != DTM.NULL)
			{
				try
				{
					dispatchCharactersEvents(node, handler, false);
				}
				catch (SAXException e)
				{
					throw new TransletException(e);
				}
			}
		}

		/// <summary>
		/// Copy a node-set to an output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void copy(DTMAxisIterator nodes, SerializationHandler handler)
		{
			int node;
			while ((node = nodes.next()) != DTM.NULL)
			{
				copy(node, handler);
			}
		}

		/// <summary>
		/// Copy the whole tree to an output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void copy(SerializationHandler handler)
		{
			copy(Document, handler);
		}

		/// <summary>
		/// Performs a deep copy (ref. XSLs copy-of())
		/// 
		/// TODO: Copy namespace declarations. Can't be done until we
		///       add namespace nodes and keep track of NS prefixes
		/// TODO: Copy comment nodes
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void copy(in int node, SerializationHandler handler)
		{
			copy(node, handler, false);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private final void copy(final int node, org.apache.xml.serializer.SerializationHandler handler, boolean isChild) throws org.apache.xalan.xsltc.TransletException
	 private void copy(in int node, SerializationHandler handler, bool isChild)
	 {
		 int nodeID = makeNodeIdentity(node);
			int eType = _exptype2(nodeID);
			int type = _exptype2Type(eType);

			try
			{
				switch (type)
				{
					case DTM.ROOT_NODE:
					case DTM.DOCUMENT_NODE:
						for (int c = _firstch2(nodeID); c != DTM.NULL; c = _nextsib2(c))
						{
							copy(makeNodeHandle(c), handler, true);
						}
						break;
					case DTM.PROCESSING_INSTRUCTION_NODE:
						copyPI(node, handler);
						break;
					case DTM.COMMENT_NODE:
						handler.comment(getStringValueX(node));
						break;
					case DTM.TEXT_NODE:
						bool oldEscapeSetting = false;
						bool escapeBit = false;

						if (_dontEscape != null)
						{
							escapeBit = _dontEscape.getBit(getNodeIdent(node));
							if (escapeBit)
							{
								oldEscapeSetting = handler.setEscaping(false);
							}
						}

						copyTextNode(nodeID, handler);

						if (escapeBit)
						{
							handler.Escaping = oldEscapeSetting;
						}
						break;
					case DTM.ATTRIBUTE_NODE:
						copyAttribute(nodeID, eType, handler);
						break;
					case DTM.NAMESPACE_NODE:
						handler.namespaceAfterStartElement(getNodeNameX(node), getNodeValue(node));
						break;
					default:
						if (type == DTM.ELEMENT_NODE)
						{
							// Start element definition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = copyElement(nodeID, eType, handler);
							string name = copyElement(nodeID, eType, handler);
							//if(isChild) => not to copy any namespaces  from parents
							// else copy all namespaces in scope
							copyNS(nodeID, handler,!isChild);
							copyAttributes(nodeID, handler);
							// Copy element children
							for (int c = _firstch2(nodeID); c != DTM.NULL; c = _nextsib2(c))
							{
								copy(makeNodeHandle(c), handler, true);
							}

							// Close element definition
							handler.endElement(name);
						}
						// Shallow copy of attribute to output handler
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = getNamespaceName(node);
							string uri = getNamespaceName(node);
							if (uri.Length != 0)
							{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = getPrefix(node);
								string prefix = getPrefix(node);
								handler.namespaceAfterStartElement(prefix, uri);
							}
							handler.addAttribute(getNodeName(node), getNodeValue(node));
						}
						break;
				}
			}
			catch (Exception e)
			{
				throw new TransletException(e);
			}

	 }
		/// <summary>
		/// Copies a processing instruction node to an output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void copyPI(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		private void copyPI(in int node, SerializationHandler handler)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String target = getNodeName(node);
			string target = getNodeName(node);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = getStringValueX(node);
			string value = getStringValueX(node);

			try
			{
				handler.processingInstruction(target, value);
			}
			catch (Exception e)
			{
				throw new TransletException(e);
			}
		}

		/// <summary>
		/// Performs a shallow copy (ref. XSLs copy())
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public string shallowCopy(in int node, SerializationHandler handler)
		{
			int nodeID = makeNodeIdentity(node);
			int exptype = _exptype2(nodeID);
			int type = _exptype2Type(exptype);

			try
			{
				switch (type)
				{
					case DTM.ELEMENT_NODE:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = copyElement(nodeID, exptype, handler);
						string name = copyElement(nodeID, exptype, handler);
						copyNS(nodeID, handler, true);
						return name;
					case DTM.ROOT_NODE:
					case DTM.DOCUMENT_NODE:
						return EMPTYSTRING;
					case DTM.TEXT_NODE:
						copyTextNode(nodeID, handler);
						return null;
					case DTM.PROCESSING_INSTRUCTION_NODE:
						copyPI(node, handler);
						return null;
					case DTM.COMMENT_NODE:
						handler.comment(getStringValueX(node));
						return null;
					case DTM.NAMESPACE_NODE:
						handler.namespaceAfterStartElement(getNodeNameX(node), getNodeValue(node));
						return null;
					case DTM.ATTRIBUTE_NODE:
						copyAttribute(nodeID, exptype, handler);
						return null;
					default:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri1 = getNamespaceName(node);
						string uri1 = getNamespaceName(node);
						if (uri1.Length != 0)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = getPrefix(node);
							string prefix = getPrefix(node);
							handler.namespaceAfterStartElement(prefix, uri1);
						}
						handler.addAttribute(getNodeName(node), getNodeValue(node));
						return null;
				}
			}
			catch (Exception e)
			{
				throw new TransletException(e);
			}
		}

		/// <summary>
		/// Returns a node' defined language for a node (if any)
		/// </summary>
		public string getLanguage(int node)
		{
			int parent = node;
			while (DTM.NULL != parent)
			{
				if (DTM.ELEMENT_NODE == getNodeType(parent))
				{
					int langAttr = getAttributeNode(parent, "http://www.w3.org/XML/1998/namespace", "lang");

					if (DTM.NULL != langAttr)
					{
						return getNodeValue(langAttr);
					}
				}

				parent = getParent(parent);
			}
			return (null);
		}

		/// <summary>
		/// Returns an instance of the DOMBuilder inner class
		/// This class will consume the input document through a SAX2
		/// interface and populate the tree.
		/// </summary>
		public DOMBuilder Builder
		{
			get
			{
			return this;
			}
		}

		/// <summary>
		/// Return a SerializationHandler for output handling.
		/// This method is used by Result Tree Fragments.
		/// </summary>
		public SerializationHandler OutputDomBuilder
		{
			get
			{
				return new ToXMLSAXHandler(this, "UTF-8");
			}
		}

		/// <summary>
		/// Return a instance of a DOM class to be used as an RTF
		/// </summary>
		public DOM getResultTreeFrag(int initSize, int rtfType)
		{
			return getResultTreeFrag(initSize, rtfType, true);
		}

		/// <summary>
		/// Return a instance of a DOM class to be used as an RTF
		/// </summary>
		/// <param name="initSize"> The initial size of the DOM. </param>
		/// <param name="rtfType"> The type of the RTF </param>
		/// <param name="addToManager"> true if the RTF should be registered with the DTMManager. </param>
		/// <returns> The DOM object which represents the RTF. </returns>
		public DOM getResultTreeFrag(int initSize, int rtfType, bool addToManager)
		{
			if (rtfType == DOM.SIMPLE_RTF)
			{
				if (addToManager)
				{
					int dtmPos = _dtmManager.FirstFreeDTMID;
					SimpleResultTreeImpl rtf = new SimpleResultTreeImpl(_dtmManager, dtmPos << DTMManager.IDENT_DTM_NODE_BITS);
					_dtmManager.addDTM(rtf, dtmPos, 0);
					return rtf;
				}
				else
				{
					return new SimpleResultTreeImpl(_dtmManager, 0);
				}
			}
			else if (rtfType == DOM.ADAPTIVE_RTF)
			{
				if (addToManager)
				{
					int dtmPos = _dtmManager.FirstFreeDTMID;
					AdaptiveResultTreeImpl rtf = new AdaptiveResultTreeImpl(_dtmManager, dtmPos << DTMManager.IDENT_DTM_NODE_BITS, m_wsfilter, initSize, m_buildIdIndex);
					_dtmManager.addDTM(rtf, dtmPos, 0);
					return rtf;

				}
				else
				{
					return new AdaptiveResultTreeImpl(_dtmManager, 0, m_wsfilter, initSize, m_buildIdIndex);
				}
			}
			else
			{
				return (DOM) _dtmManager.getDTM(null, true, m_wsfilter, true, false, false, initSize, m_buildIdIndex);
			}
		}

		/// <summary>
		/// %HZ% Need Javadoc
		/// </summary>
		public Hashtable ElementsWithIDs
		{
			get
			{
				if (m_idAttributes == null)
				{
					return null;
				}
    
				// Convert a java.util.Hashtable to an xsltc.runtime.Hashtable
				System.Collections.IEnumerator idEntries = m_idAttributes.SetOfKeyValuePairs().GetEnumerator();
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (!idEntries.hasNext())
				{
					return null;
				}
    
				Hashtable idAttrsTable = new Hashtable();
    
				while (idEntries.MoveNext())
				{
					DictionaryEntry entry = (DictionaryEntry) idEntries.Current;
					idAttrsTable.put(entry.Key, entry.Value);
				}
    
				return idAttrsTable;
			}
		}

		/// <summary>
		/// The getUnparsedEntityURI function returns the URI of the unparsed
		/// entity with the specified name in the same document as the context
		/// node (see [3.3 Unparsed Entities]). It returns the empty string if
		/// there is no such entity.
		/// </summary>
		public override string getUnparsedEntityURI(string name)
		{
			// Special handling for DOM input
			if (_document != null)
			{
				string uri = "";
				DocumentType doctype = _document.getDoctype();
				if (doctype != null)
				{
					NamedNodeMap entities = doctype.getEntities();

					if (entities == null)
					{
						return uri;
					}

					Entity entity = (Entity) entities.getNamedItem(name);

					if (entity == null)
					{
						return uri;
					}

					string notationName = entity.getNotationName();
					if (!string.ReferenceEquals(notationName, null))
					{
						uri = entity.getSystemId();
						if (string.ReferenceEquals(uri, null))
						{
							uri = entity.getPublicId();
						}
					}
				}
				return uri;
			}
			else
			{
				return base.getUnparsedEntityURI(name);
			}
		}

	}

}