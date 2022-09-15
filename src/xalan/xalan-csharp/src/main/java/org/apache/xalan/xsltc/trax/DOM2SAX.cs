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
 * $Id: DOM2SAX.java 469688 2006-10-31 22:39:43Z minchau $
 */


namespace org.apache.xalan.xsltc.trax
{

	using SAXImpl = org.apache.xalan.xsltc.dom.SAXImpl;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using EntityResolver = org.xml.sax.EntityResolver;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using XMLReader = org.xml.sax.XMLReader;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	using AttributesImpl = org.xml.sax.helpers.AttributesImpl;

	/// <summary>
	/// @author G. Todd Miller 
	/// </summary>
	public class DOM2SAX : XMLReader, Locator
	{

		private const string EMPTYSTRING = "";
		private const string XMLNS_PREFIX = "xmlns";

		private Node _dom = null;
		private ContentHandler _sax = null;
		private LexicalHandler _lex = null;
		private SAXImpl _saxImpl = null;
		private Hashtable _nsPrefixes = new Hashtable();

		public DOM2SAX(Node root)
		{
		_dom = root;
		}

		public virtual ContentHandler ContentHandler
		{
			get
			{
			return _sax;
			}
			set
			{
			_sax = value;
			if (value is LexicalHandler)
			{
				_lex = (LexicalHandler) value;
			}
    
			if (value is SAXImpl)
			{
				_saxImpl = (SAXImpl)value;
			}
			}
		}


		/// <summary>
		/// Begin the scope of namespace prefix. Forward the event to the 
		/// SAX handler only if the prefix is unknown or it is mapped to a 
		/// different URI.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private boolean startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		private bool startPrefixMapping(string prefix, string uri)
		{
		bool pushed = true;
		System.Collections.Stack uriStack = (System.Collections.Stack) _nsPrefixes[prefix];

		if (uriStack != null)
		{
			if (uriStack.Count == 0)
			{
			_sax.startPrefixMapping(prefix, uri);
			uriStack.Push(uri);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String lastUri = (String) uriStack.peek();
			string lastUri = (string) uriStack.Peek();
			if (!lastUri.Equals(uri))
			{
				_sax.startPrefixMapping(prefix, uri);
				uriStack.Push(uri);
			}
			else
			{
				pushed = false;
			}
			}
		}
		else
		{
			_sax.startPrefixMapping(prefix, uri);
			_nsPrefixes[prefix] = uriStack = new System.Collections.Stack();
			uriStack.Push(uri);
		}
		return pushed;
		}

		/*
		 * End the scope of a name prefix by popping it from the stack and 
		 * passing the event to the SAX Handler.
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		private void endPrefixMapping(string prefix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Stack uriStack = (java.util.Stack) _nsPrefixes.get(prefix);
		System.Collections.Stack uriStack = (System.Collections.Stack) _nsPrefixes[prefix];

		if (uriStack != null)
		{
			_sax.endPrefixMapping(prefix);
			uriStack.Pop();
		}
		}

		/// <summary>
		/// If the DOM was created using a DOM 1.0 API, the local name may be 
		/// null. If so, get the local name from the qualified name before 
		/// generating the SAX event. 
		/// </summary>
		private static string getLocalName(Node node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = node.getLocalName();
		string localName = node.getLocalName();

		if (string.ReferenceEquals(localName, null))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qname = node.getNodeName();
			string qname = node.getNodeName();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.lastIndexOf(':');
			int col = qname.LastIndexOf(':');
			return (col > 0) ? qname.Substring(col + 1) : qname;
		}
		return localName;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void parse(org.xml.sax.InputSource unused) throws IOException, org.xml.sax.SAXException
		public virtual void parse(InputSource unused)
		{
			parse(_dom);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void parse() throws IOException, org.xml.sax.SAXException
		public virtual void parse()
		{
		if (_dom != null)
		{
			bool isIncomplete = (_dom.getNodeType() != Node.DOCUMENT_NODE);

			if (isIncomplete)
			{
			_sax.startDocument();
			parse(_dom);
			_sax.endDocument();
			}
			else
			{
			parse(_dom);
			}
		}
		}

		/// <summary>
		/// Traverse the DOM and generate SAX events for a handler. A 
		/// startElement() event passes all attributes, including namespace 
		/// declarations. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void parse(org.w3c.dom.Node node) throws IOException, org.xml.sax.SAXException
		private void parse(Node node)
		{
			Node first = null;
		 if (node == null)
		 {
			 return;
		 }

			switch (node.getNodeType())
			{
		case Node.ATTRIBUTE_NODE: // handled by ELEMENT_NODE
		case Node.DOCUMENT_FRAGMENT_NODE:
		case Node.DOCUMENT_TYPE_NODE :
		case Node.ENTITY_NODE :
		case Node.ENTITY_REFERENCE_NODE:
		case Node.NOTATION_NODE :
			// These node types are ignored!!!
			break;
		case Node.CDATA_SECTION_NODE:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String cdata = node.getNodeValue();
			string cdata = node.getNodeValue();
			if (_lex != null)
			{
			_lex.startCDATA();
				_sax.characters(cdata.ToCharArray(), 0, cdata.Length);
			_lex.endCDATA();
			}
			else
			{
			// in the case where there is no lex handler, we still
			// want the text of the cdate to make its way through.
				_sax.characters(cdata.ToCharArray(), 0, cdata.Length);
			}
			break;

		case Node.COMMENT_NODE: // should be handled!!!
			if (_lex != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = node.getNodeValue();
			string value = node.getNodeValue();
			_lex.comment(value.ToCharArray(), 0, value.Length);
			}
			break;
		case Node.DOCUMENT_NODE:
			_sax.setDocumentLocator(this);

			_sax.startDocument();
			Node next = node.getFirstChild();
			while (next != null)
			{
			parse(next);
			next = next.getNextSibling();
			}
			_sax.endDocument();
			break;

		case Node.ELEMENT_NODE:
			string prefix;
			System.Collections.IList pushedPrefixes = new ArrayList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.helpers.AttributesImpl attrs = new org.xml.sax.helpers.AttributesImpl();
			AttributesImpl attrs = new AttributesImpl();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.NamedNodeMap map = node.getAttributes();
			NamedNodeMap map = node.getAttributes();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = map.getLength();
			int length = map.getLength();

			// Process all namespace declarations
			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = map.item(i);
			Node attr = map.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qnameAttr = attr.getNodeName();
			string qnameAttr = attr.getNodeName();

			// Ignore everything but NS declarations here
			if (qnameAttr.StartsWith(XMLNS_PREFIX, StringComparison.Ordinal))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uriAttr = attr.getNodeValue();
				string uriAttr = attr.getNodeValue();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = qnameAttr.lastIndexOf(':');
				int colon = qnameAttr.LastIndexOf(':');
				prefix = (colon > 0) ? qnameAttr.Substring(colon + 1) : EMPTYSTRING;
				if (startPrefixMapping(prefix, uriAttr))
				{
				pushedPrefixes.Add(prefix);
				}
			}
			}

			// Process all other attributes
			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = map.item(i);
			Node attr = map.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qnameAttr = attr.getNodeName();
			string qnameAttr = attr.getNodeName();

			// Ignore NS declarations here
			if (!qnameAttr.StartsWith(XMLNS_PREFIX, StringComparison.Ordinal))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uriAttr = attr.getNamespaceURI();
				string uriAttr = attr.getNamespaceURI();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localNameAttr = getLocalName(attr);
				string localNameAttr = getLocalName(attr);

				// Uri may be implicitly declared
				if (!string.ReferenceEquals(uriAttr, null))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = qnameAttr.lastIndexOf(':');
				int colon = qnameAttr.LastIndexOf(':');
				prefix = (colon > 0) ? qnameAttr.Substring(0, colon) : EMPTYSTRING;
				if (startPrefixMapping(prefix, uriAttr))
				{
					pushedPrefixes.Add(prefix);
				}
				}

				// Add attribute to list
				attrs.addAttribute(attr.getNamespaceURI(), getLocalName(attr), qnameAttr, "CDATA", attr.getNodeValue());
			}
			}

			// Now process the element itself
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qname = node.getNodeName();
			string qname = node.getNodeName();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = node.getNamespaceURI();
			string uri = node.getNamespaceURI();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = getLocalName(node);
			string localName = getLocalName(node);

			// Uri may be implicitly declared
			if (!string.ReferenceEquals(uri, null))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = qname.lastIndexOf(':');
			int colon = qname.LastIndexOf(':');
			prefix = (colon > 0) ? qname.Substring(0, colon) : EMPTYSTRING;
			if (startPrefixMapping(prefix, uri))
			{
				pushedPrefixes.Add(prefix);
			}
			}

			// Generate SAX event to start element
			if (_saxImpl != null)
			{
				_saxImpl.startElement(uri, localName, qname, attrs, node);
			}
			else
			{
				_sax.startElement(uri, localName, qname, attrs);
			}

			// Traverse all child nodes of the element (if any)
			next = node.getFirstChild();
			while (next != null)
			{
			parse(next);
			next = next.getNextSibling();
			}

			// Generate SAX event to close element
			_sax.endElement(uri, localName, qname);

			// Generate endPrefixMapping() for all pushed prefixes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nPushedPrefixes = pushedPrefixes.size();
			int nPushedPrefixes = pushedPrefixes.Count;
			for (int i = 0; i < nPushedPrefixes; i++)
			{
			endPrefixMapping((string) pushedPrefixes[i]);
			}
			break;

		case Node.PROCESSING_INSTRUCTION_NODE:
			_sax.processingInstruction(node.getNodeName(), node.getNodeValue());
			break;

		case Node.TEXT_NODE:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String data = node.getNodeValue();
			string data = node.getNodeValue();
			_sax.characters(data.ToCharArray(), 0, data.Length);
			break;
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual DTDHandler DTDHandler
		{
			get
			{
			return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual ErrorHandler ErrorHandler
		{
			get
			{
			return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean getFeature(String name) throws SAXNotRecognizedException, org.xml.sax.SAXNotSupportedException
		public virtual bool getFeature(string name)
		{
		return false;
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setFeature(String name, boolean value) throws SAXNotRecognizedException, org.xml.sax.SAXNotSupportedException
		public virtual void setFeature(string name, bool value)
		{
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void parse(String sysId) throws IOException, org.xml.sax.SAXException
		public virtual void parse(string sysId)
		{
		throw new IOException("This method is not yet implemented.");
		}


		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setEntityResolver(org.xml.sax.EntityResolver resolver) throws NullPointerException
		public virtual EntityResolver EntityResolver
		{
			set
			{
			}
			get
			{
			return null;
			}
		}



		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setProperty(String name, Object value) throws SAXNotRecognizedException, org.xml.sax.SAXNotSupportedException
		public virtual void setProperty(string name, object value)
		{
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object getProperty(String name) throws SAXNotRecognizedException, org.xml.sax.SAXNotSupportedException
		public virtual object getProperty(string name)
		{
		return null;
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual int ColumnNumber
		{
			get
			{
			return 0;
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual int LineNumber
		{
			get
			{
			return 0;
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual string PublicId
		{
			get
			{
			return null;
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual string SystemId
		{
			get
			{
			return null;
			}
		}

		// Debugging 
		private string getNodeTypeFromCode(short code)
		{
		string retval = null;
		switch (code)
		{
		case Node.ATTRIBUTE_NODE :
			retval = "ATTRIBUTE_NODE";
			break;
		case Node.CDATA_SECTION_NODE :
			retval = "CDATA_SECTION_NODE";
			break;
		case Node.COMMENT_NODE :
			retval = "COMMENT_NODE";
			break;
		case Node.DOCUMENT_FRAGMENT_NODE :
			retval = "DOCUMENT_FRAGMENT_NODE";
			break;
		case Node.DOCUMENT_NODE :
			retval = "DOCUMENT_NODE";
			break;
		case Node.DOCUMENT_TYPE_NODE :
			retval = "DOCUMENT_TYPE_NODE";
			break;
		case Node.ELEMENT_NODE :
			retval = "ELEMENT_NODE";
			break;
		case Node.ENTITY_NODE :
			retval = "ENTITY_NODE";
			break;
		case Node.ENTITY_REFERENCE_NODE :
			retval = "ENTITY_REFERENCE_NODE";
			break;
		case Node.NOTATION_NODE :
			retval = "NOTATION_NODE";
			break;
		case Node.PROCESSING_INSTRUCTION_NODE :
			retval = "PROCESSING_INSTRUCTION_NODE";
			break;
		case Node.TEXT_NODE:
			retval = "TEXT_NODE";
			break;
		}
		return retval;
		}
	}

}