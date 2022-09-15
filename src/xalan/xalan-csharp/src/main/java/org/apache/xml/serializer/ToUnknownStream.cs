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
 * $Id: ToUnknownStream.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer
{


	using Node = org.w3c.dom.Node;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	/// This class wraps another SerializationHandler. The wrapped object will either
	/// handler XML or HTML, which is not known until a little later when the first XML
	/// tag is seen.  If the first tag is <html> then the wrapped object is an HTML
	/// handler, otherwise it is an XML handler.
	/// 
	/// This class effectively caches the first few calls to it then passes them
	/// on to the wrapped handler (once it exists).  After that subsequent calls a
	/// simply passed directly to the wrapped handler.
	/// 
	/// The user of this class doesn't know if the output is ultimatley XML or HTML.
	/// 
	/// This class is not a public API, it is public because it is used within Xalan.
	/// @xsl.usage internal
	/// </summary>
	public sealed class ToUnknownStream : SerializerBase
	{

		/// <summary>
		/// The wrapped handler, initially XML but possibly switched to HTML
		/// </summary>
		private SerializationHandler m_handler;

		/// <summary>
		/// A String with no characters
		/// </summary>
		private const string EMPTYSTRING = "";

		/// <summary>
		/// true if the underlying handler (XML or HTML) is fully initialized
		/// </summary>
		private bool m_wrapped_handler_not_initialized = false;


		/// <summary>
		/// the prefix of the very first tag in the document
		/// </summary>
		private string m_firstElementPrefix;
		/// <summary>
		/// the element name (including any prefix) of the very first tag in the document
		/// </summary>
		private string m_firstElementName;

		/// <summary>
		/// the namespace URI associated with the first element
		/// </summary>
		private string m_firstElementURI;

		/// <summary>
		/// the local name (no prefix) associated with the first element
		/// </summary>
		private string m_firstElementLocalName = null;

		/// <summary>
		/// true if the first tag has been emitted to the wrapped handler
		/// </summary>
		private bool m_firstTagNotEmitted = true;

		/// <summary>
		/// A collection of namespace URI's (only for first element).
		/// _namespacePrefix has the matching prefix for these URI's
		/// </summary>
		private ArrayList m_namespaceURI = null;
		/// <summary>
		/// A collection of namespace Prefix (only for first element)
		/// _namespaceURI has the matching URIs for these prefix'
		/// </summary>
		private ArrayList m_namespacePrefix = null;

		/// <summary>
		/// true if startDocument() was called before the underlying handler
		/// was initialized
		/// </summary>
		private new bool m_needToCallStartDocument = false;
		/// <summary>
		/// true if setVersion() was called before the underlying handler
		/// was initialized
		/// </summary>
		private bool m_setVersion_called = false;
		/// <summary>
		/// true if setDoctypeSystem() was called before the underlying handler
		/// was initialized
		/// </summary>
		private bool m_setDoctypeSystem_called = false;
		/// <summary>
		/// true if setDoctypePublic() was called before the underlying handler
		/// was initialized
		/// </summary>
		private bool m_setDoctypePublic_called = false;
		/// <summary>
		/// true if setMediaType() was called before the underlying handler
		/// was initialized
		/// </summary>
		private bool m_setMediaType_called = false;

		/// <summary>
		/// Default constructor.
		/// Initially this object wraps an XML Stream object, so _handler is never null.
		/// That may change later to an HTML Stream object.
		/// </summary>
		public ToUnknownStream()
		{
			m_handler = new ToXMLStream();
		}

		/// <seealso cref="Serializer.asContentHandler()"/>
		/// <returns> the wrapped XML or HTML handler </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException
		public override ContentHandler asContentHandler()
		{
			/* don't return the real handler ( m_handler ) because
			 * that would expose the real handler to the outside.
			 * Keep m_handler private so it can be internally swapped
			 * to an HTML handler.
			 */
			return this;
		}

		/// <seealso cref="SerializationHandler.close()"/>
		public override void close()
		{
			m_handler.close();
		}

		/// <seealso cref="Serializer.getOutputFormat()"/>
		/// <returns> the properties of the underlying handler </returns>
		public override Properties OutputFormat
		{
			get
			{
				return m_handler.OutputFormat;
			}
			set
			{
				m_handler.OutputFormat = value;
			}
		}

		/// <seealso cref="Serializer.getOutputStream()"/>
		/// <returns> the OutputStream of the underlying XML or HTML handler </returns>
		public override Stream OutputStream
		{
			get
			{
				return m_handler.OutputStream;
			}
			set
			{
				m_handler.OutputStream = value;
			}
		}

		/// <seealso cref="Serializer.getWriter()"/>
		/// <returns> the Writer of the underlying XML or HTML handler </returns>
		public override Writer Writer
		{
			get
			{
				return m_handler.Writer;
			}
			set
			{
				m_handler.Writer = value;
			}
		}

		/// <summary>
		/// passes the call on to the underlying HTML or XML handler </summary>
		/// <seealso cref="Serializer.reset()"/>
		/// <returns> ??? </returns>
		public override bool reset()
		{
			return m_handler.reset();
		}

		/// <summary>
		/// Converts the DOM node to output </summary>
		/// <param name="node"> the DOM node to transform to output </param>
		/// <seealso cref="DOMSerializer.serialize(Node)"
		/// />
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public override void serialize(Node node)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.serialize(node);
		}

		/// <seealso cref="SerializationHandler.setEscaping(boolean)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException
		public override bool setEscaping(bool escape)
		{
			return m_handler.setEscaping(escape);
		}




		/// <summary>
		/// Adds an attribute to the currenly open tag </summary>
		/// <param name="uri"> the URI of a namespace </param>
		/// <param name="localName"> the attribute name, without prefix </param>
		/// <param name="rawName"> the attribute name, with prefix (if any) </param>
		/// <param name="type"> the type of the attribute, typically "CDATA" </param>
		/// <param name="value"> the value of the parameter </param>
		/// <param name="XSLAttribute"> true if this attribute is coming from an xsl:attribute element </param>
		/// <seealso cref="ExtendedContentHandler.addAttribute(String, String, String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value, boolean XSLAttribute) throws org.xml.sax.SAXException
		public override void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.addAttribute(uri, localName, rawName, type, value, XSLAttribute);
		}
		/// <summary>
		/// Adds an attribute to the currenly open tag </summary>
		/// <param name="rawName"> the attribute name, with prefix (if any) </param>
		/// <param name="value"> the value of the parameter </param>
		/// <seealso cref="ExtendedContentHandler.addAttribute(String, String)"/>
		public override void addAttribute(string rawName, string value)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.addAttribute(rawName, value);

		}

		/// <summary>
		/// Adds a unique attribute to the currenly open tag
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addUniqueAttribute(String rawName, String value, int flags) throws org.xml.sax.SAXException
		public override void addUniqueAttribute(string rawName, string value, int flags)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.addUniqueAttribute(rawName, value, flags);

		}

		/// <summary>
		/// Converts the String to a character array and calls the SAX method 
		/// characters(char[],int,int);
		/// </summary>
		/// <seealso cref="ExtendedContentHandler.characters(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(String chars) throws org.xml.sax.SAXException
		public override void characters(string chars)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = chars.length();
			int length = chars.Length;
			if (length > m_charsBuff.Length)
			{
				m_charsBuff = new char[length * 2 + 1];
			}
			chars.CopyTo(0, m_charsBuff, 0, length - 0);
			this.characters(m_charsBuff, 0, length);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="ExtendedContentHandler.endElement(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String elementName) throws org.xml.sax.SAXException
		public override void endElement(string elementName)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.endElement(elementName);
		}


		/// <seealso cref="org.xml.sax.ContentHandler.startPrefixMapping(String, String)"/>
		/// <param name="prefix"> The prefix that maps to the URI </param>
		/// <param name="uri"> The URI for the namespace </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public void startPrefixMapping(string prefix, string uri)
		{
			this.startPrefixMapping(prefix,uri, true);
		}

		/// <summary>
		/// This method is used when a prefix/uri namespace mapping
		/// is indicated after the element was started with a
		/// startElement() and before and endElement().
		/// startPrefixMapping(prefix,uri) would be used before the
		/// startElement() call. </summary>
		/// <param name="uri"> the URI of the namespace </param>
		/// <param name="prefix"> the prefix associated with the given URI.
		/// </param>
		/// <seealso cref="ExtendedContentHandler.namespaceAfterStartElement(String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void namespaceAfterStartElement(String prefix, String uri) throws org.xml.sax.SAXException
		public override void namespaceAfterStartElement(string prefix, string uri)
		{
			// hack for XSLTC with finding URI for default namespace
			if (m_firstTagNotEmitted && string.ReferenceEquals(m_firstElementURI, null) && !string.ReferenceEquals(m_firstElementName, null))
			{
				string prefix1 = getPrefixPart(m_firstElementName);
				if (string.ReferenceEquals(prefix1, null) && EMPTYSTRING.Equals(prefix))
				{
					// the elements URI is not known yet, and it
					// doesn't have a prefix, and we are currently
					// setting the uri for prefix "", so we have
					// the uri for the element... lets remember it
					m_firstElementURI = uri;
				}
			}
			startPrefixMapping(prefix,uri, false);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public override bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{
			bool pushed = false;
			if (m_firstTagNotEmitted)
			{
				if (!string.ReferenceEquals(m_firstElementName, null) && shouldFlush)
				{
					/* we've already seen a startElement, and this is a prefix mapping
					 * for the up coming element, so flush the old element
					 * then send this event on its way.
					 */
					flush();
					pushed = m_handler.startPrefixMapping(prefix, uri, shouldFlush);
				}
				else
				{
					if (m_namespacePrefix == null)
					{
						m_namespacePrefix = new ArrayList();
						m_namespaceURI = new ArrayList();
					}
					m_namespacePrefix.Add(prefix);
					m_namespaceURI.Add(uri);

					if (string.ReferenceEquals(m_firstElementURI, null))
					{
						if (prefix.Equals(m_firstElementPrefix))
						{
							m_firstElementURI = uri;
						}
					}
				}

			}
			else
			{
			   pushed = m_handler.startPrefixMapping(prefix, uri, shouldFlush);
			}
			return pushed;
		}

		/// <summary>
		/// This method cannot be cached because default is different in
		/// HTML and XML (we need more than a boolean).
		/// </summary>

		public override string Version
		{
			set
			{
				m_handler.Version = value;
    
				// Cache call to setVersion()
				//       super.setVersion(value);
				m_setVersion_called = true;
			}
			get
			{
				return m_handler.Version;
			}
		}

		/// <seealso cref="org.xml.sax.ContentHandler.startDocument()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public override void startDocument()
		{
			m_needToCallStartDocument = true;
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String qName) throws org.xml.sax.SAXException
		public override void startElement(string qName)
		{
			this.startElement(null, null, qName, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String qName) throws org.xml.sax.SAXException
		public override void startElement(string namespaceURI, string localName, string qName)
		{
			this.startElement(namespaceURI, localName, qName, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String elementName, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public void startElement(string namespaceURI, string localName, string elementName, Attributes atts)
		{
			/* we are notified of the start of an element */
			if (m_firstTagNotEmitted)
			{
				/* we have not yet sent the first element on its way */
				if (!string.ReferenceEquals(m_firstElementName, null))
				{
					/* this is not the first element, but a later one.
					 * But we have the old element pending, so flush it out,
					 * then send this one on its way. 
					 */
					flush();
					m_handler.startElement(namespaceURI, localName, elementName, atts);
				}
				else
				{
					/* this is the very first element that we have seen, 
					 * so save it for flushing later.  We may yet get to know its
					 * URI due to added attributes.
					 */

					m_wrapped_handler_not_initialized = true;
					m_firstElementName = elementName;

					// null if not known
					m_firstElementPrefix = getPrefixPartUnknown(elementName);

					// null if not known
					m_firstElementURI = namespaceURI;

					// null if not known
					m_firstElementLocalName = localName;

					if (m_tracer != null)
					{
						firePseudoElement(elementName);
					}

					/* we don't want to call our own addAttributes, which
					 * merely delegates to the wrapped handler, but we want to
					 * add these attributes to m_attributes. So me must call super.
					 * addAttributes() In this case m_attributes is only used for the
					 * first element, after that this class totally delegates to the
					 * wrapped handler which is either XML or HTML.
					 */
					if (atts != null)
					{
						base.addAttributes(atts);
					}

					// if there are attributes, then lets make the flush()
					// call the startElement on the handler and send the
					// attributes on their way.
					if (atts != null)
					{
						flush();
					}

				}
			}
			else
			{
				// this is not the first element, but a later one, so just
				// send it on its way.
				m_handler.startElement(namespaceURI, localName, elementName, atts);
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="ExtendedLexicalHandler.comment(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(String comment) throws org.xml.sax.SAXException
		public override void comment(string comment)
		{
			if (m_firstTagNotEmitted && !string.ReferenceEquals(m_firstElementName, null))
			{
				emitFirstTag();
			}
			else if (m_needToCallStartDocument)
			{
				m_handler.startDocument();
				m_needToCallStartDocument = false;
			}

			m_handler.comment(comment);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getDoctypePublic()"/>
		public override string DoctypePublic
		{
			get
			{
    
				return m_handler.DoctypePublic;
			}
			set
			{
				m_handler.DoctypePublic = value;
				m_setDoctypePublic_called = true;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getDoctypeSystem()"/>
		public override string DoctypeSystem
		{
			get
			{
				return m_handler.DoctypeSystem;
			}
			set
			{
				m_handler.DoctypeSystem = value;
				m_setDoctypeSystem_called = true;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getEncoding()"/>
		public override string Encoding
		{
			get
			{
				return m_handler.Encoding;
			}
			set
			{
				m_handler.Encoding = value;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getIndent()"/>
		public override bool Indent
		{
			get
			{
				return m_handler.Indent;
			}
			set
			{
				m_handler.Indent = value;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getIndentAmount()"/>
		public override int IndentAmount
		{
			get
			{
				return m_handler.IndentAmount;
			}
			set
			{
				m_handler.IndentAmount = value;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getMediaType()"/>
		public override string MediaType
		{
			get
			{
				return m_handler.MediaType;
			}
			set
			{
				m_handler.MediaType = value;
				m_setMediaType_called = true;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getOmitXMLDeclaration()"/>
		public override bool OmitXMLDeclaration
		{
			get
			{
				return m_handler.OmitXMLDeclaration;
			}
			set
			{
				m_handler.OmitXMLDeclaration = value;
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="XSLOutputAttributes.getStandalone()"/>
		public override string Standalone
		{
			get
			{
				return m_handler.Standalone;
			}
			set
			{
				m_handler.Standalone = value;
			}
		}


		/// <seealso cref="XSLOutputAttributes.setDoctype(String, String)"/>
		public override void setDoctype(string system, string pub)
		{
			m_handler.DoctypePublic = pub;
			m_handler.DoctypeSystem = system;
		}









		/// <seealso cref="XSLOutputAttributes.setVersion(String)"/>

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.attributeDecl(String, String, String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void attributeDecl(String arg0, String arg1, String arg2, String arg3, String arg4) throws org.xml.sax.SAXException
		public void attributeDecl(string arg0, string arg1, string arg2, string arg3, string arg4)
		{
			m_handler.attributeDecl(arg0, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.elementDecl(String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void elementDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public void elementDecl(string arg0, string arg1)
		{
			if (m_firstTagNotEmitted)
			{
				emitFirstTag();
			}
			m_handler.elementDecl(arg0, arg1);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.externalEntityDecl(String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public void externalEntityDecl(string name, string publicId, string systemId)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.externalEntityDecl(name, publicId, systemId);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.internalEntityDecl(String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void internalEntityDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public void internalEntityDecl(string arg0, string arg1)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.internalEntityDecl(arg0, arg1);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.characters(char[], int, int)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char[] characters, int offset, int length) throws org.xml.sax.SAXException
		public void characters(char[] characters, int offset, int length)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}

			m_handler.characters(characters, offset, length);

		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.endDocument()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public void endDocument()
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}

			m_handler.endDocument();


		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.endElement(String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String qName) throws org.xml.sax.SAXException
		public void endElement(string namespaceURI, string localName, string qName)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
				if (string.ReferenceEquals(namespaceURI, null) && !string.ReferenceEquals(m_firstElementURI, null))
				{
					namespaceURI = m_firstElementURI;
				}


				if (string.ReferenceEquals(localName, null) && !string.ReferenceEquals(m_firstElementLocalName, null))
				{
					localName = m_firstElementLocalName;
				}
			}

			m_handler.endElement(namespaceURI, localName, qName);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.endPrefixMapping(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		public void endPrefixMapping(string prefix)
		{
			m_handler.endPrefixMapping(prefix);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.ignorableWhitespace(char[], int, int)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public void ignorableWhitespace(char[] ch, int start, int length)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}
			m_handler.ignorableWhitespace(ch, start, length);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.processingInstruction(String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public void processingInstruction(string target, string data)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}

			m_handler.processingInstruction(target, data);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.setDocumentLocator(Locator)"/>
		public override Locator DocumentLocator
		{
			set
			{
				m_handler.setDocumentLocator(value);
			}
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.skippedEntity(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
		public void skippedEntity(string name)
		{
			m_handler.skippedEntity(name);
		}



		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.comment(char[], int, int)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public void comment(char[] ch, int start, int length)
		{
			if (m_firstTagNotEmitted)
			{
				flush();
			}

			m_handler.comment(ch, start, length);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.endCDATA()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public void endCDATA()
		{

			m_handler.endCDATA();
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.endDTD()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public void endDTD()
		{

			m_handler.endDTD();
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.endEntity(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endEntity(String name) throws org.xml.sax.SAXException
		public override void endEntity(string name)
		{
			if (m_firstTagNotEmitted)
			{
				emitFirstTag();
			}
			m_handler.endEntity(name);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.startCDATA()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public void startCDATA()
		{
			m_handler.startCDATA();
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.startDTD(String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public void startDTD(string name, string publicId, string systemId)
		{
			m_handler.startDTD(name, publicId, systemId);
		}

		/// <summary>
		/// Pass the call on to the underlying handler </summary>
		/// <seealso cref="org.xml.sax.ext.LexicalHandler.startEntity(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startEntity(String name) throws org.xml.sax.SAXException
		public void startEntity(string name)
		{
			m_handler.startEntity(name);
		}

		/// <summary>
		/// Initialize the wrapped output stream (XML or HTML).
		/// If the stream handler should be HTML, then replace the XML handler with
		/// an HTML handler. After than send the starting method calls that were cached
		/// to the wrapped handler.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initStreamOutput() throws org.xml.sax.SAXException
		private void initStreamOutput()
		{

			// Try to rule out if this is an not to be an HTML document based on prefix
			bool firstElementIsHTML = FirstElemHTML;

			if (firstElementIsHTML)
			{
				// create an HTML output handler, and initialize it

				// keep a reference to the old handler, ... it will soon be gone
				SerializationHandler oldHandler = m_handler;

				/* We have to make sure we get an output properties with the proper
				 * defaults for the HTML method.  The easiest way to do this is to
				 * have the OutputProperties class do it.
				 */

				Properties htmlProperties = OutputPropertiesFactory.getDefaultMethodProperties(Method.HTML);
				Serializer serializer = SerializerFactory.getSerializer(htmlProperties);

				// The factory should be returning a ToStream
				// Don't know what to do if it doesn't
				// i.e. the user has over-ridden the content-handler property
				// for html
				m_handler = (SerializationHandler) serializer;
				//m_handler = new ToHTMLStream();

				Writer writer = oldHandler.Writer;

				if (null != writer)
				{
					m_handler.Writer = writer;
				}
				else
				{
					Stream os = oldHandler.OutputStream;

					if (null != os)
					{
						m_handler.OutputStream = os;
					}
				}

				// need to copy things from the old handler to the new one here

				//            if (_setVersion_called)
				//            {
				m_handler.Version = oldHandler.Version;
				//            }
				//            if (_setDoctypeSystem_called)
				//            {
				m_handler.DoctypeSystem = oldHandler.DoctypeSystem;
				//            }
				//            if (_setDoctypePublic_called)
				//            {
				m_handler.DoctypePublic = oldHandler.DoctypePublic;
				//            }
				//            if (_setMediaType_called)
				//            {
				m_handler.MediaType = oldHandler.MediaType;
				//            }

				m_handler.Transformer = oldHandler.Transformer;
			}

			/* Now that we have a real wrapped handler (XML or HTML) lets
			 * pass any cached calls to it
			 */
			// Call startDocument() if necessary
			if (m_needToCallStartDocument)
			{
				m_handler.startDocument();
				m_needToCallStartDocument = false;
			}

			// the wrapped handler is now fully initialized
			m_wrapped_handler_not_initialized = false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void emitFirstTag() throws org.xml.sax.SAXException
		private void emitFirstTag()
		{
			if (!string.ReferenceEquals(m_firstElementName, null))
			{
				if (m_wrapped_handler_not_initialized)
				{
					initStreamOutput();
					m_wrapped_handler_not_initialized = false;
				}
				// Output first tag
				m_handler.startElement(m_firstElementURI, null, m_firstElementName, m_attributes);
				// don't need the collected attributes of the first element anymore.
				m_attributes = null;

				// Output namespaces of first tag
				if (m_namespacePrefix != null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = m_namespacePrefix.size();
					int n = m_namespacePrefix.Count;
					for (int i = 0; i < n; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (String) m_namespacePrefix.elementAt(i);
						string prefix = (string) m_namespacePrefix[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = (String) m_namespaceURI.elementAt(i);
						string uri = (string) m_namespaceURI[i];
						m_handler.startPrefixMapping(prefix, uri, false);
					}
					m_namespacePrefix = null;
					m_namespaceURI = null;
				}
				m_firstTagNotEmitted = false;
			}
		}

		/// <summary>
		/// Utility function for calls to local-name().
		/// 
		/// Don't want to override static function on SerializerBase
		/// So added Unknown suffix to method name.
		/// </summary>
		private string getLocalNameUnknown(string value)
		{
			int idx = value.LastIndexOf(':');
			if (idx >= 0)
			{
				value = value.Substring(idx + 1);
			}
			idx = value.LastIndexOf('@');
			if (idx >= 0)
			{
				value = value.Substring(idx + 1);
			}
			return (value);
		}

		/// <summary>
		/// Utility function to return prefix
		///     
		/// Don't want to override static function on SerializerBase
		/// So added Unknown suffix to method name.
		/// </summary>
		private string getPrefixPartUnknown(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = qname.indexOf(':');
			int index = qname.IndexOf(':');
			return (index > 0) ? qname.Substring(0, index) : EMPTYSTRING;
		}

		/// <summary>
		/// Determine if the firts element in the document is <html> or <HTML>
		/// This uses the cached first element name, first element prefix and the
		/// cached namespaces from previous method calls
		/// </summary>
		/// <returns> true if the first element is an opening <html> tag </returns>
		private bool FirstElemHTML
		{
			get
			{
				bool isHTML;
    
				// is the first tag html, not considering the prefix ?
				isHTML = getLocalNameUnknown(m_firstElementName).Equals("html", StringComparison.OrdinalIgnoreCase);
    
				// Try to rule out if this is not to be an HTML document based on URI
				if (isHTML && !string.ReferenceEquals(m_firstElementURI, null) && !EMPTYSTRING.Equals(m_firstElementURI))
				{
					// the <html> element has a non-trivial namespace
					isHTML = false;
				}
				// Try to rule out if this is an not to be an HTML document based on prefix
				if (isHTML && m_namespacePrefix != null)
				{
					/* the first element has a name of "html", but lets check the prefix.
					 * If the prefix points to a namespace with a URL that is not ""
					 * then the doecument doesn't start with an <html> tag, and isn't html
					 */
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int max = m_namespacePrefix.size();
					int max = m_namespacePrefix.Count;
					for (int i = 0; i < max; i++)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String prefix = (String) m_namespacePrefix.elementAt(i);
						string prefix = (string) m_namespacePrefix[i];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String uri = (String) m_namespaceURI.elementAt(i);
						string uri = (string) m_namespaceURI[i];
    
						if (!string.ReferenceEquals(m_firstElementPrefix, null) && m_firstElementPrefix.Equals(prefix) && !EMPTYSTRING.Equals(uri))
						{
							// The first element has a prefix, so it can't be <html>
							isHTML = false;
							break;
						}
					}
    
				}
				return isHTML;
			}
		}
		/// <seealso cref="Serializer.asDOMSerializer()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException
		public override DOMSerializer asDOMSerializer()
		{
			return m_handler.asDOMSerializer();
		}

		/// <param name="URI_and_localNames"> Vector a list of pairs of URI/localName
		/// specified in the cdata-section-elements attribute. </param>
		/// <seealso cref="SerializationHandler.setCdataSectionElements(java.util.Vector)"/>
		public override ArrayList CdataSectionElements
		{
			set
			{
				m_handler.CdataSectionElements = value;
			}
		}
		/// <seealso cref="ExtendedContentHandler.addAttributes(org.xml.sax.Attributes)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addAttributes(org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public override void addAttributes(Attributes atts)
		{
			m_handler.addAttributes(atts);
		}

		/// <summary>
		/// Get the current namespace mappings.
		/// Simply returns the mappings of the wrapped handler. </summary>
		/// <seealso cref="ExtendedContentHandler.getNamespaceMappings()"/>
		public override NamespaceMappings NamespaceMappings
		{
			get
			{
				NamespaceMappings mappings = null;
				if (m_handler != null)
				{
					mappings = m_handler.NamespaceMappings;
				}
				return mappings;
			}
		}
		/// <seealso cref="SerializationHandler.flushPending()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException
		public override void flushPending()
		{

			flush();

			m_handler.flushPending();
		}

		private void flush()
		{
			try
			{
			if (m_firstTagNotEmitted)
			{
				emitFirstTag();
			}
			if (m_needToCallStartDocument)
			{
				m_handler.startDocument();
				m_needToCallStartDocument = false;
			}
			}
			catch (SAXException e)
			{
				throw new Exception(e.ToString());
			}


		}

		/// <seealso cref="ExtendedContentHandler.getPrefix"/>
		public override string getPrefix(string namespaceURI)
		{
			return m_handler.getPrefix(namespaceURI);
		}
		/// <seealso cref="ExtendedContentHandler.entityReference(java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void entityReference(String entityName) throws org.xml.sax.SAXException
		public override void entityReference(string entityName)
		{
			m_handler.entityReference(entityName);
		}

		/// <seealso cref="ExtendedContentHandler.getNamespaceURI(java.lang.String, boolean)"/>
		public override string getNamespaceURI(string qname, bool isElement)
		{
			return m_handler.getNamespaceURI(qname, isElement);
		}

		public override string getNamespaceURIFromPrefix(string prefix)
		{
			return m_handler.getNamespaceURIFromPrefix(prefix);
		}

		public override Transformer Transformer
		{
			set
			{
				m_handler.Transformer = value;
				if ((value is SerializerTrace) && (((SerializerTrace) value).hasTraceListeners()))
				{
				   m_tracer = (SerializerTrace) value;
				}
				else
				{
				   m_tracer = null;
				}
			}
			get
			{
				return m_handler.Transformer;
			}
		}

		/// <seealso cref="SerializationHandler.setContentHandler(org.xml.sax.ContentHandler)"/>
		public override ContentHandler ContentHandler
		{
			set
			{
				m_handler.ContentHandler = value;
			}
		}
		/// <summary>
		/// This method is used to set the source locator, which might be used to
		/// generated an error message. </summary>
		/// <param name="locator"> the source locator
		/// </param>
		/// <seealso cref="ExtendedContentHandler.setSourceLocator(javax.xml.transform.SourceLocator)"/>
		public override SourceLocator SourceLocator
		{
			set
			{
				m_handler.SourceLocator = value;
			}
		}

		protected internal void firePseudoElement(string elementName)
		{

			if (m_tracer != null)
			{
				StringBuilder sb = new StringBuilder();

				sb.Append('<');
				sb.Append(elementName);

				// convert the StringBuffer to a char array and
				// emit the trace event that these characters "might"
				// be written
				char[] ch = sb.ToString().ToCharArray();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_OUTPUT_PSEUDO_CHARACTERS, ch, 0, ch.Length);
			}
		}

		/// <seealso cref="org.apache.xml.serializer.Serializer.asDOM3Serializer()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object asDOM3Serializer() throws java.io.IOException
		public override object asDOM3Serializer()
		{
			return m_handler.asDOM3Serializer();
		}
	}

}