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
 * $Id: DOM2TO.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{

	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
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
	using NamespaceMappings = org.apache.xml.serializer.NamespaceMappings;

	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class DOM2TO : XMLReader, Locator
	{

		private const string EMPTYSTRING = "";
		private const string XMLNS_PREFIX = "xmlns";

		/// <summary>
		/// A reference to the DOM to be traversed.
		/// </summary>
		private Node _dom;

		/// <summary>
		/// A reference to the output handler receiving the events.
		/// </summary>
		private SerializationHandler _handler;

		public DOM2TO(Node root, SerializationHandler handler)
		{
		_dom = root;
		_handler = handler;
		}

		public virtual ContentHandler ContentHandler
		{
			get
			{
			return null;
			}
			set
			{
			// Empty
			}
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
			_handler.startDocument();
			parse(_dom);
			_handler.endDocument();
			}
			else
			{
			parse(_dom);
			}
		}
		}

		/// <summary>
		/// Traverse the DOM and generate TO events for a handler. Notice that 
		/// we need to handle implicit namespace declarations too.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void parse(org.w3c.dom.Node node) throws IOException, org.xml.sax.SAXException
		private void parse(Node node)
		{
		 if (node == null)
		 {
			 return;
		 }

			switch (node.getNodeType())
			{
		case Node.ATTRIBUTE_NODE: // handled by ELEMENT_NODE
		case Node.DOCUMENT_TYPE_NODE :
		case Node.ENTITY_NODE :
		case Node.ENTITY_REFERENCE_NODE:
		case Node.NOTATION_NODE :
			// These node types are ignored!!!
			break;
		case Node.CDATA_SECTION_NODE:
			_handler.startCDATA();
			_handler.characters(node.getNodeValue());
			_handler.endCDATA();
			break;

		case Node.COMMENT_NODE: // should be handled!!!
			_handler.comment(node.getNodeValue());
			break;

		case Node.DOCUMENT_NODE:
			_handler.startDocument();
			Node next = node.getFirstChild();
			while (next != null)
			{
			parse(next);
			next = next.getNextSibling();
			}
			_handler.endDocument();
			break;

		case Node.DOCUMENT_FRAGMENT_NODE:
			next = node.getFirstChild();
			while (next != null)
			{
			parse(next);
			next = next.getNextSibling();
			}
			break;

		case Node.ELEMENT_NODE:
			// Generate SAX event to start element
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qname = node.getNodeName();
			string qname = node.getNodeName();
			_handler.startElement(null, null, qname);

				int colon;
			string prefix;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.NamedNodeMap map = node.getAttributes();
			NamedNodeMap map = node.getAttributes();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = map.getLength();
			int length = map.getLength();

			// Process all namespace attributes first
			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = map.item(i);
			Node attr = map.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qnameAttr = attr.getNodeName();
			string qnameAttr = attr.getNodeName();

					// Is this a namespace declaration?
			if (qnameAttr.StartsWith(XMLNS_PREFIX, StringComparison.Ordinal))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uriAttr = attr.getNodeValue();
				string uriAttr = attr.getNodeValue();
				colon = qnameAttr.LastIndexOf(':');
				prefix = (colon > 0) ? qnameAttr.Substring(colon + 1) : EMPTYSTRING;
				_handler.namespaceAfterStartElement(prefix, uriAttr);
			}
			}

			// Process all non-namespace attributes next
				NamespaceMappings nm = new NamespaceMappings();
			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = map.item(i);
			Node attr = map.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String qnameAttr = attr.getNodeName();
			string qnameAttr = attr.getNodeName();

					// Is this a regular attribute?
			if (!qnameAttr.StartsWith(XMLNS_PREFIX, StringComparison.Ordinal))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uriAttr = attr.getNamespaceURI();
				string uriAttr = attr.getNamespaceURI();
				// Uri may be implicitly declared
				if (!string.ReferenceEquals(uriAttr, null) && !uriAttr.Equals(EMPTYSTRING))
				{
				colon = qnameAttr.LastIndexOf(':');

							// Fix for bug 26319
							// For attributes not given an prefix explictly
							// but having a namespace uri we need
							// to explicitly generate the prefix
							string newPrefix = nm.lookupPrefix(uriAttr);
							if (string.ReferenceEquals(newPrefix, null))
							{
								newPrefix = nm.generateNextPrefix();
							}
				prefix = (colon > 0) ? qnameAttr.Substring(0, colon) : newPrefix;
				_handler.namespaceAfterStartElement(prefix, uriAttr);
					_handler.addAttribute((prefix + ":" + qnameAttr), attr.getNodeValue());
				}
				else
				{
							 _handler.addAttribute(qnameAttr, attr.getNodeValue());
				}
			}
			}

			// Now element namespace and children
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = node.getNamespaceURI();
			string uri = node.getNamespaceURI();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = node.getLocalName();
				string localName = node.getLocalName();

			// Uri may be implicitly declared
			if (!string.ReferenceEquals(uri, null))
			{
			colon = qname.LastIndexOf(':');
			prefix = (colon > 0) ? qname.Substring(0, colon) : EMPTYSTRING;
			_handler.namespaceAfterStartElement(prefix, uri);
			}
			else
			{
					  // Fix for bug 26319
					  // If an element foo is created using
					  // createElementNS(null,locName)
					  // then the  element should be serialized
					  // <foo xmlns=" "/> 
					  if (string.ReferenceEquals(uri, null) && !string.ReferenceEquals(localName, null))
					  {
				  prefix = EMPTYSTRING;
				 _handler.namespaceAfterStartElement(prefix, EMPTYSTRING);
					  }
			}

			// Traverse all child nodes of the element (if any)
			next = node.getFirstChild();
			while (next != null)
			{
			parse(next);
			next = next.getNextSibling();
			}

			// Generate SAX event to close element
			_handler.endElement(qname);
			break;

		case Node.PROCESSING_INSTRUCTION_NODE:
			_handler.processingInstruction(node.getNodeName(), node.getNodeValue());
			break;

		case Node.TEXT_NODE:
			_handler.characters(node.getNodeValue());
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