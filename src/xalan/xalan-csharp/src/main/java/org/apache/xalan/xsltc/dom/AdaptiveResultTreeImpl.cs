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
 * $Id: AdaptiveResultTreeImpl.java 468651 2006-10-28 07:04:25Z minchau $
 */
namespace org.apache.xalan.xsltc.dom
{
	using DOM = org.apache.xalan.xsltc.DOM;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using AttributeList = org.apache.xalan.xsltc.runtime.AttributeList;

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using XMLString = org.apache.xml.utils.XMLString;

	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// AdaptiveResultTreeImpl is a adaptive DOM model for result tree fragments (RTF). It is
	/// used in the case where the RTF is likely to be pure text yet it can still be a DOM tree. 
	/// It is designed for RTFs which have &lt;xsl:call-template&gt; or &lt;xsl:apply-templates&gt; in 
	/// the contents. Example:
	/// <pre>
	///    &lt;xsl:variable name = "x"&gt;
	///      &lt;xsl:call-template name = "test"&gt;
	///         &lt;xsl:with-param name="a" select="."/&gt;
	///      &lt;/xsl:call-template&gt;
	///    &lt;/xsl:variable>
	/// </pre>
	/// <para>In this example the result produced by <xsl:call-template> is likely to be a single
	/// Text node. But it can also be a DOM tree. This kind of RTF cannot be modelled by 
	/// SimpleResultTreeImpl. 
	/// </para>
	/// <para>
	/// AdaptiveResultTreeImpl can be considered as a smart switcher between SimpleResultTreeImpl
	/// and SAXImpl. It treats the RTF as simple Text and uses the SimpleResultTreeImpl model
	/// at the beginning. However, if it receives a call which indicates that this is a DOM tree
	/// (e.g. startElement), it will automatically transform itself into a wrapper around a 
	/// SAXImpl. In this way we can have a light-weight model when the result only contains
	/// simple text, while at the same time it still works when the RTF is a DOM tree.
	/// </para>
	/// <para>
	/// All methods in this class are overridden to delegate the action to the wrapped SAXImpl object
	/// if it is non-null, or delegate the action to the SimpleResultTreeImpl if there is no
	/// wrapped SAXImpl.
	/// </para>
	/// <para>
	/// %REVISIT% Can we combine this class with SimpleResultTreeImpl? I think it is possible, but
	/// it will make SimpleResultTreeImpl more expensive. I will use two separate classes at 
	/// this time.
	/// </para>
	/// </summary>
	public class AdaptiveResultTreeImpl : SimpleResultTreeImpl
	{

		// Document URI index, which increases by 1 at each getDocumentURI() call.
		private static int _documentURIIndex = 0;

		// The SAXImpl object wrapped by this class, if the RTF is a tree.
		private SAXImpl _dom;

		/// <summary>
		/// The following fields are only used for the nested SAXImpl * </summary>

		// The whitespace filter
		private DTMWSFilter _wsfilter;

		// The size of the RTF
		private int _initSize;

		// True if we want to build the ID index table
		private bool _buildIdIndex;

		// The AttributeList
		private readonly AttributeList _attributes = new AttributeList();

		// The element name
		private string _openElementName;


		// Create a AdaptiveResultTreeImpl
		public AdaptiveResultTreeImpl(XSLTCDTMManager dtmManager, int documentID, DTMWSFilter wsfilter, int initSize, bool buildIdIndex) : base(dtmManager, documentID)
		{

			_wsfilter = wsfilter;
			_initSize = initSize;
			_buildIdIndex = buildIdIndex;
		}

		// Return the DOM object wrapped in this object.
		public virtual DOM NestedDOM
		{
			get
			{
				return _dom;
			}
		}

		// Return the document ID
		public override int Document
		{
			get
			{
				if (_dom != null)
				{
					return _dom.Document;
				}
				else
				{
					return base.Document;
				}
			}
		}

		// Return the String value of the RTF
		public override string StringValue
		{
			get
			{
				if (_dom != null)
				{
					return _dom.StringValue;
				}
				else
				{
					return base.StringValue;
				}
			}
		}

		public override DTMAxisIterator Iterator
		{
			get
			{
				if (_dom != null)
				{
					return _dom.Iterator;
				}
				else
				{
					return base.Iterator;
				}
			}
		}

		public override DTMAxisIterator getChildren(in int node)
		{
			if (_dom != null)
			{
				return _dom.getChildren(node);
			}
			else
			{
				return base.getChildren(node);
			}
		}

		public override DTMAxisIterator getTypedChildren(in int type)
		{
			if (_dom != null)
			{
				return _dom.getTypedChildren(type);
			}
			else
			{
				return base.getTypedChildren(type);
			}
		}

		public override DTMAxisIterator getAxisIterator(in int axis)
		{
			if (_dom != null)
			{
				return _dom.getAxisIterator(axis);
			}
			else
			{
				return base.getAxisIterator(axis);
			}
		}

		public override DTMAxisIterator getTypedAxisIterator(in int axis, in int type)
		{
			if (_dom != null)
			{
				return _dom.getTypedAxisIterator(axis, type);
			}
			else
			{
				return base.getTypedAxisIterator(axis, type);
			}
		}

		public override DTMAxisIterator getNthDescendant(int node, int n, bool includeself)
		{
			if (_dom != null)
			{
				return _dom.getNthDescendant(node, n, includeself);
			}
			else
			{
				return base.getNthDescendant(node, n, includeself);
			}
		}

		public override DTMAxisIterator getNamespaceAxisIterator(in int axis, in int ns)
		{
			if (_dom != null)
			{
				return _dom.getNamespaceAxisIterator(axis, ns);
			}
			else
			{
				return base.getNamespaceAxisIterator(axis, ns);
			}
		}

		public override DTMAxisIterator getNodeValueIterator(DTMAxisIterator iter, int returnType, string value, bool op)
		{
			if (_dom != null)
			{
				return _dom.getNodeValueIterator(iter, returnType, value, op);
			}
			else
			{
				return base.getNodeValueIterator(iter, returnType, value, op);
			}
		}

		public override DTMAxisIterator orderNodes(DTMAxisIterator source, int node)
		{
			if (_dom != null)
			{
				return _dom.orderNodes(source, node);
			}
			else
			{
				return base.orderNodes(source, node);
			}
		}

		public override string getNodeName(in int node)
		{
			if (_dom != null)
			{
				return _dom.getNodeName(node);
			}
			else
			{
				return base.getNodeName(node);
			}
		}

		public override string getNodeNameX(in int node)
		{
			if (_dom != null)
			{
				return _dom.getNodeNameX(node);
			}
			else
			{
				return base.getNodeNameX(node);
			}
		}

		public override string getNamespaceName(in int node)
		{
			if (_dom != null)
			{
				return _dom.getNamespaceName(node);
			}
			else
			{
				return base.getNamespaceName(node);
			}
		}

		// Return the expanded type id of a given node
		public override int getExpandedTypeID(in int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getExpandedTypeID(nodeHandle);
			}
			else
			{
				return base.getExpandedTypeID(nodeHandle);
			}
		}

		public override int getNamespaceType(in int node)
		{
			if (_dom != null)
			{
				return _dom.getNamespaceType(node);
			}
			else
			{
				return base.getNamespaceType(node);
			}
		}

		public override int getParent(in int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getParent(nodeHandle);
			}
			else
			{
				return base.getParent(nodeHandle);
			}
		}

		public override int getAttributeNode(in int gType, in int element)
		{
			if (_dom != null)
			{
				return _dom.getAttributeNode(gType, element);
			}
			else
			{
				return base.getAttributeNode(gType, element);
			}
		}

		public override string getStringValueX(in int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getStringValueX(nodeHandle);
			}
			else
			{
				return base.getStringValueX(nodeHandle);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public override void copy(in int node, SerializationHandler handler)
		{
			if (_dom != null)
			{
				_dom.copy(node, handler);
			}
			else
			{
				base.copy(node, handler);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public override void copy(DTMAxisIterator nodes, SerializationHandler handler)
		{
			if (_dom != null)
			{
				_dom.copy(nodes, handler);
			}
			else
			{
				base.copy(nodes, handler);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public override string shallowCopy(in int node, SerializationHandler handler)
		{
			if (_dom != null)
			{
				return _dom.shallowCopy(node, handler);
			}
			else
			{
				return base.shallowCopy(node, handler);
			}
		}

		public override bool lessThan(in int node1, in int node2)
		{
			if (_dom != null)
			{
				return _dom.lessThan(node1, node2);
			}
			else
			{
				return base.lessThan(node1, node2);
			}
		}

		/// <summary>
		/// Dispatch the character content of a node to an output handler.
		/// 
		/// The escape setting should be taken care of when outputting to
		/// a handler.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public override void characters(in int node, SerializationHandler handler)
		{
			if (_dom != null)
			{
				_dom.characters(node, handler);
			}
			else
			{
				base.characters(node, handler);
			}
		}

		public override Node makeNode(int index)
		{
			if (_dom != null)
			{
				return _dom.makeNode(index);
			}
			else
			{
				return base.makeNode(index);
			}
		}

		public override Node makeNode(DTMAxisIterator iter)
		{
			if (_dom != null)
			{
				return _dom.makeNode(iter);
			}
			else
			{
				return base.makeNode(iter);
			}
		}

		public override NodeList makeNodeList(int index)
		{
			if (_dom != null)
			{
				return _dom.makeNodeList(index);
			}
			else
			{
				return base.makeNodeList(index);
			}
		}

		public override NodeList makeNodeList(DTMAxisIterator iter)
		{
			if (_dom != null)
			{
				return _dom.makeNodeList(iter);
			}
			else
			{
				return base.makeNodeList(iter);
			}
		}

		public override string getLanguage(int node)
		{
			if (_dom != null)
			{
				return _dom.getLanguage(node);
			}
			else
			{
				return base.getLanguage(node);
			}
		}

		public override int Size
		{
			get
			{
				if (_dom != null)
				{
					return _dom.Size;
				}
				else
				{
					return base.Size;
				}
			}
		}

		public override string getDocumentURI(int node)
		{
			if (_dom != null)
			{
				return _dom.getDocumentURI(node);
			}
			else
			{
				return "adaptive_rtf" + _documentURIIndex++;
			}
		}

		public override StripFilter Filter
		{
			set
			{
				if (_dom != null)
				{
					_dom.Filter = value;
				}
				else
				{
					base.Filter = value;
				}
			}
		}

		public override void setupMapping(string[] names, string[] uris, int[] types, string[] namespaces)
		{
			if (_dom != null)
			{
				_dom.setupMapping(names, uris, types, namespaces);
			}
			else
			{
				base.setupMapping(names, uris, types, namespaces);
			}
		}

		public override bool isElement(in int node)
		{
			if (_dom != null)
			{
				return _dom.isElement(node);
			}
			else
			{
				return base.isElement(node);
			}
		}

		public override bool isAttribute(in int node)
		{
			if (_dom != null)
			{
				return _dom.isAttribute(node);
			}
			else
			{
				return base.isAttribute(node);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws org.apache.xalan.xsltc.TransletException
		public override string lookupNamespace(int node, string prefix)
		{
			if (_dom != null)
			{
				return _dom.lookupNamespace(node, prefix);
			}
			else
			{
				return base.lookupNamespace(node, prefix);
			}
		}

		/// <summary>
		/// Return the node identity from a node handle.
		/// </summary>
		public sealed override int getNodeIdent(in int nodehandle)
		{
			if (_dom != null)
			{
				return _dom.getNodeIdent(nodehandle);
			}
			else
			{
				return base.getNodeIdent(nodehandle);
			}
		}

		/// <summary>
		/// Return the node handle from a node identity.
		/// </summary>
		public sealed override int getNodeHandle(in int nodeId)
		{
			if (_dom != null)
			{
				return _dom.getNodeHandle(nodeId);
			}
			else
			{
				return base.getNodeHandle(nodeId);
			}
		}

		public override DOM getResultTreeFrag(int initialSize, int rtfType)
		{
			if (_dom != null)
			{
				return _dom.getResultTreeFrag(initialSize, rtfType);
			}
			else
			{
				return base.getResultTreeFrag(initialSize, rtfType);
			}
		}

		public override SerializationHandler OutputDomBuilder
		{
			get
			{
				return this;
			}
		}

		public override int getNSType(int node)
		{
			if (_dom != null)
			{
				return _dom.getNSType(node);
			}
			else
			{
				return base.getNSType(node);
			}
		}

		public override string getUnparsedEntityURI(string name)
		{
			if (_dom != null)
			{
				return _dom.getUnparsedEntityURI(name);
			}
			else
			{
				return base.getUnparsedEntityURI(name);
			}
		}

		public override Hashtable ElementsWithIDs
		{
			get
			{
				if (_dom != null)
				{
					return _dom.ElementsWithIDs;
				}
				else
				{
					return base.ElementsWithIDs;
				}
			}
		}

		/// <summary>
		/// Implementation of the SerializationHandler interfaces * </summary>

		/// <summary>
		/// The code in some of the following interfaces are copied from SAXAdapter. * </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void maybeEmitStartElement() throws org.xml.sax.SAXException
		private void maybeEmitStartElement()
		{
		if (!string.ReferenceEquals(_openElementName, null))
		{

		   int index;
		   if ((index = _openElementName.IndexOf(":", StringComparison.Ordinal)) < 0)
		   {
			   _dom.startElement(null, _openElementName, _openElementName, _attributes);
		   }
		   else
		   {
			   _dom.startElement(null, _openElementName.Substring(index + 1), _openElementName, _attributes);
		   }


			_openElementName = null;
		}
		}

		// Create and initialize the wrapped SAXImpl object
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void prepareNewDOM() throws org.xml.sax.SAXException
		private void prepareNewDOM()
		{
			_dom = (SAXImpl)_dtmManager.getDTM(null, true, _wsfilter, true, false, false, _initSize, _buildIdIndex);
			_dom.startDocument();
			// Flush pending Text nodes to SAXImpl
			for (int i = 0; i < _size; i++)
			{
				string str = _textArray[i];
				_dom.characters(str.ToCharArray(), 0, str.Length);
			}
			_size = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public override void startDocument()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public override void endDocument()
		{
			if (_dom != null)
			{
				_dom.endDocument();
			}
			else
			{
				base.endDocument();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(String str) throws org.xml.sax.SAXException
		public override void characters(string str)
		{
			if (_dom != null)
			{
				characters(str.ToCharArray(), 0, str.Length);
			}
			else
			{
				base.characters(str);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char[] ch, int offset, int length) throws org.xml.sax.SAXException
		public override void characters(char[] ch, int offset, int length)
		{
			if (_dom != null)
			{
			maybeEmitStartElement();
			_dom.characters(ch, offset, length);
			}
			else
			{
				base.characters(ch, offset, length);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException
		public override bool setEscaping(bool escape)
		{
			if (_dom != null)
			{
				return _dom.setEscaping(escape);
			}
			else
			{
				return base.setEscaping(escape);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementName)
		{
			if (_dom == null)
			{
				prepareNewDOM();
			}

		maybeEmitStartElement();
		_openElementName = elementName;
		_attributes.clear();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
		public override void startElement(string uri, string localName, string qName)
		{
			startElement(qName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
		public override void startElement(string uri, string localName, string qName, Attributes attributes)
		{
			startElement(qName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String elementName) throws org.xml.sax.SAXException
		public override void endElement(string elementName)
		{
		maybeEmitStartElement();
		_dom.endElement(null, null, elementName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
		public override void endElement(string uri, string localName, string qName)
		{
			endElement(qName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addUniqueAttribute(String qName, String value, int flags) throws org.xml.sax.SAXException
	 public override void addUniqueAttribute(string qName, string value, int flags)
	 {
			addAttribute(qName, value);
	 }

		public override void addAttribute(string name, string value)
		{
		if (!string.ReferenceEquals(_openElementName, null))
		{
			_attributes.add(name, value);
		}
		else
		{
			BasisLibrary.runTimeError(BasisLibrary.STRAY_ATTRIBUTE_ERR, name);
		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void namespaceAfterStartElement(String prefix, String uri) throws org.xml.sax.SAXException
		public override void namespaceAfterStartElement(string prefix, string uri)
		{
		if (_dom == null)
		{
		   prepareNewDOM();
		}

		_dom.startPrefixMapping(prefix, uri);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(String comment) throws org.xml.sax.SAXException
		public override void comment(string comment)
		{
		if (_dom == null)
		{
		   prepareNewDOM();
		}

		maybeEmitStartElement();
			char[] chars = comment.ToCharArray();
			_dom.comment(chars, 0, chars.Length);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char[] chars, int offset, int length) throws org.xml.sax.SAXException
		public override void comment(char[] chars, int offset, int length)
		{
		if (_dom == null)
		{
		   prepareNewDOM();
		}

		maybeEmitStartElement();
			_dom.comment(chars, offset, length);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public override void processingInstruction(string target, string data)
		{
		if (_dom == null)
		{
		   prepareNewDOM();
		}

		maybeEmitStartElement();
		_dom.processingInstruction(target, data);
		}

		/// <summary>
		/// Implementation of the DTM interfaces * </summary>

		public override void setFeature(string featureId, bool state)
		{
			if (_dom != null)
			{
				_dom.setFeature(featureId, state);
			}
		}

		public override void setProperty(string property, object value)
		{
			if (_dom != null)
			{
				_dom.setProperty(property, value);
			}
		}

		public override DTMAxisTraverser getAxisTraverser(in int axis)
		{
			if (_dom != null)
			{
				return _dom.getAxisTraverser(axis);
			}
			else
			{
				return base.getAxisTraverser(axis);
			}
		}

		public override bool hasChildNodes(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.hasChildNodes(nodeHandle);
			}
			else
			{
				return base.hasChildNodes(nodeHandle);
			}
		}

		public override int getFirstChild(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getFirstChild(nodeHandle);
			}
			else
			{
				return base.getFirstChild(nodeHandle);
			}
		}

		public override int getLastChild(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getLastChild(nodeHandle);
			}
			else
			{
				return base.getLastChild(nodeHandle);
			}
		}

		public override int getAttributeNode(int elementHandle, string namespaceURI, string name)
		{
			if (_dom != null)
			{
				return _dom.getAttributeNode(elementHandle, namespaceURI, name);
			}
			else
			{
				return base.getAttributeNode(elementHandle, namespaceURI, name);
			}
		}

		public override int getFirstAttribute(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getFirstAttribute(nodeHandle);
			}
			else
			{
				return base.getFirstAttribute(nodeHandle);
			}
		}

		public override int getFirstNamespaceNode(int nodeHandle, bool inScope)
		{
			if (_dom != null)
			{
				return _dom.getFirstNamespaceNode(nodeHandle, inScope);
			}
			else
			{
				return base.getFirstNamespaceNode(nodeHandle, inScope);
			}
		}

		public override int getNextSibling(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNextSibling(nodeHandle);
			}
			else
			{
				return base.getNextSibling(nodeHandle);
			}
		}

		public override int getPreviousSibling(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getPreviousSibling(nodeHandle);
			}
			else
			{
				return base.getPreviousSibling(nodeHandle);
			}
		}

		public override int getNextAttribute(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNextAttribute(nodeHandle);
			}
			else
			{
				return base.getNextAttribute(nodeHandle);
			}
		}

		public override int getNextNamespaceNode(int baseHandle, int namespaceHandle, bool inScope)
		{
			if (_dom != null)
			{
				return _dom.getNextNamespaceNode(baseHandle, namespaceHandle, inScope);
			}
			else
			{
				return base.getNextNamespaceNode(baseHandle, namespaceHandle, inScope);
			}
		}

		public override int getOwnerDocument(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getOwnerDocument(nodeHandle);
			}
			else
			{
				return base.getOwnerDocument(nodeHandle);
			}
		}

		public override int getDocumentRoot(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getDocumentRoot(nodeHandle);
			}
			else
			{
				return base.getDocumentRoot(nodeHandle);
			}
		}

		public override XMLString getStringValue(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getStringValue(nodeHandle);
			}
			else
			{
				return base.getStringValue(nodeHandle);
			}
		}

		public override int getStringValueChunkCount(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getStringValueChunkCount(nodeHandle);
			}
			else
			{
				return base.getStringValueChunkCount(nodeHandle);
			}
		}

		public override char[] getStringValueChunk(int nodeHandle, int chunkIndex, int[] startAndLen)
		{
			if (_dom != null)
			{
				return _dom.getStringValueChunk(nodeHandle, chunkIndex, startAndLen);
			}
			else
			{
				return base.getStringValueChunk(nodeHandle, chunkIndex, startAndLen);
			}
		}

		public override int getExpandedTypeID(string @namespace, string localName, int type)
		{
			if (_dom != null)
			{
				return _dom.getExpandedTypeID(@namespace, localName, type);
			}
			else
			{
				return base.getExpandedTypeID(@namespace, localName, type);
			}
		}

		public override string getLocalNameFromExpandedNameID(int ExpandedNameID)
		{
			if (_dom != null)
			{
				return _dom.getLocalNameFromExpandedNameID(ExpandedNameID);
			}
			else
			{
				return base.getLocalNameFromExpandedNameID(ExpandedNameID);
			}
		}

		public override string getNamespaceFromExpandedNameID(int ExpandedNameID)
		{
			if (_dom != null)
			{
				return _dom.getNamespaceFromExpandedNameID(ExpandedNameID);
			}
			else
			{
				return base.getNamespaceFromExpandedNameID(ExpandedNameID);
			}
		}

		public override string getLocalName(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getLocalName(nodeHandle);
			}
			else
			{
				return base.getLocalName(nodeHandle);
			}
		}

		public override string getPrefix(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getPrefix(nodeHandle);
			}
			else
			{
				return base.getPrefix(nodeHandle);
			}
		}

		public override string getNamespaceURI(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNamespaceURI(nodeHandle);
			}
			else
			{
				return base.getNamespaceURI(nodeHandle);
			}
		}

		public override string getNodeValue(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNodeValue(nodeHandle);
			}
			else
			{
				return base.getNodeValue(nodeHandle);
			}
		}

		public override short getNodeType(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNodeType(nodeHandle);
			}
			else
			{
				return base.getNodeType(nodeHandle);
			}
		}

		public override short getLevel(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getLevel(nodeHandle);
			}
			else
			{
				return base.getLevel(nodeHandle);
			}
		}

		public override bool isSupported(string feature, string version)
		{
			if (_dom != null)
			{
				return _dom.isSupported(feature, version);
			}
			else
			{
				return base.isSupported(feature, version);
			}
		}

		public override string DocumentBaseURI
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DocumentBaseURI;
				}
				else
				{
					return base.DocumentBaseURI;
				}
			}
			set
			{
				if (_dom != null)
				{
					_dom.DocumentBaseURI = value;
				}
				else
				{
					base.DocumentBaseURI = value;
				}
			}
		}


		public override string getDocumentSystemIdentifier(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getDocumentSystemIdentifier(nodeHandle);
			}
			else
			{
				return base.getDocumentSystemIdentifier(nodeHandle);
			}
		}

		public override string getDocumentEncoding(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getDocumentEncoding(nodeHandle);
			}
			else
			{
				return base.getDocumentEncoding(nodeHandle);
			}
		}

		public override string getDocumentStandalone(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getDocumentStandalone(nodeHandle);
			}
			else
			{
				return base.getDocumentStandalone(nodeHandle);
			}
		}

		public override string getDocumentVersion(int documentHandle)
		{
			if (_dom != null)
			{
				return _dom.getDocumentVersion(documentHandle);
			}
			else
			{
				return base.getDocumentVersion(documentHandle);
			}
		}

		public override bool DocumentAllDeclarationsProcessed
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DocumentAllDeclarationsProcessed;
				}
				else
				{
					return base.DocumentAllDeclarationsProcessed;
				}
			}
		}

		public override string DocumentTypeDeclarationSystemIdentifier
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DocumentTypeDeclarationSystemIdentifier;
				}
				else
				{
					return base.DocumentTypeDeclarationSystemIdentifier;
				}
			}
		}

		public override string DocumentTypeDeclarationPublicIdentifier
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DocumentTypeDeclarationPublicIdentifier;
				}
				else
				{
					return base.DocumentTypeDeclarationPublicIdentifier;
				}
			}
		}

		public override int getElementById(string elementId)
		{
			if (_dom != null)
			{
				return _dom.getElementById(elementId);
			}
			else
			{
				return base.getElementById(elementId);
			}
		}

		public override bool supportsPreStripping()
		{
			if (_dom != null)
			{
				return _dom.supportsPreStripping();
			}
			else
			{
				return base.supportsPreStripping();
			}
		}

		public override bool isNodeAfter(int firstNodeHandle, int secondNodeHandle)
		{
			if (_dom != null)
			{
				return _dom.isNodeAfter(firstNodeHandle, secondNodeHandle);
			}
			else
			{
				return base.isNodeAfter(firstNodeHandle, secondNodeHandle);
			}
		}

		public override bool isCharacterElementContentWhitespace(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.isCharacterElementContentWhitespace(nodeHandle);
			}
			else
			{
				return base.isCharacterElementContentWhitespace(nodeHandle);
			}
		}

		public override bool isDocumentAllDeclarationsProcessed(int documentHandle)
		{
			if (_dom != null)
			{
				return _dom.isDocumentAllDeclarationsProcessed(documentHandle);
			}
			else
			{
				return base.isDocumentAllDeclarationsProcessed(documentHandle);
			}
		}

		public override bool isAttributeSpecified(int attributeHandle)
		{
			if (_dom != null)
			{
				return _dom.isAttributeSpecified(attributeHandle);
			}
			else
			{
				return base.isAttributeSpecified(attributeHandle);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize) throws org.xml.sax.SAXException
		public override void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, bool normalize)
		{
			if (_dom != null)
			{
				_dom.dispatchCharactersEvents(nodeHandle, ch, normalize);
			}
			else
			{
				base.dispatchCharactersEvents(nodeHandle, ch, normalize);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
		public override void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch)
		{
			if (_dom != null)
			{
				_dom.dispatchToEvents(nodeHandle, ch);
			}
			else
			{
				base.dispatchToEvents(nodeHandle, ch);
			}
		}

		public override Node getNode(int nodeHandle)
		{
			if (_dom != null)
			{
				return _dom.getNode(nodeHandle);
			}
			else
			{
				return base.getNode(nodeHandle);
			}
		}

		public override bool needsTwoThreads()
		{
			if (_dom != null)
			{
				return _dom.needsTwoThreads();
			}
			else
			{
				return base.needsTwoThreads();
			}
		}

		public override org.xml.sax.ContentHandler ContentHandler
		{
			get
			{
				if (_dom != null)
				{
					return _dom.ContentHandler;
				}
				else
				{
					return base.ContentHandler;
				}
			}
		}

		public override org.xml.sax.ext.LexicalHandler LexicalHandler
		{
			get
			{
				if (_dom != null)
				{
					return _dom.LexicalHandler;
				}
				else
				{
					return base.LexicalHandler;
				}
			}
		}

		public override org.xml.sax.EntityResolver EntityResolver
		{
			get
			{
				if (_dom != null)
				{
					return _dom.EntityResolver;
				}
				else
				{
					return base.EntityResolver;
				}
			}
		}

		public override org.xml.sax.DTDHandler DTDHandler
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DTDHandler;
				}
				else
				{
					return base.DTDHandler;
				}
			}
		}

		public override org.xml.sax.ErrorHandler ErrorHandler
		{
			get
			{
				if (_dom != null)
				{
					return _dom.ErrorHandler;
				}
				else
				{
					return base.ErrorHandler;
				}
			}
		}

		public override org.xml.sax.ext.DeclHandler DeclHandler
		{
			get
			{
				if (_dom != null)
				{
					return _dom.DeclHandler;
				}
				else
				{
					return base.DeclHandler;
				}
			}
		}

		public override void appendChild(int newChild, bool clone, bool cloneDepth)
		{
			if (_dom != null)
			{
				_dom.appendChild(newChild, clone, cloneDepth);
			}
			else
			{
				base.appendChild(newChild, clone, cloneDepth);
			}
		}

		public override void appendTextChild(string str)
		{
			if (_dom != null)
			{
				_dom.appendTextChild(str);
			}
			else
			{
				base.appendTextChild(str);
			}
		}

		public override SourceLocator getSourceLocatorFor(int node)
		{
			if (_dom != null)
			{
				return _dom.getSourceLocatorFor(node);
			}
			else
			{
				return base.getSourceLocatorFor(node);
			}
		}

		public override void documentRegistration()
		{
			if (_dom != null)
			{
				_dom.documentRegistration();
			}
			else
			{
				base.documentRegistration();
			}
		}

		public override void documentRelease()
		{
			if (_dom != null)
			{
				_dom.documentRelease();
			}
			else
			{
				base.documentRelease();
			}
		}

	}

}