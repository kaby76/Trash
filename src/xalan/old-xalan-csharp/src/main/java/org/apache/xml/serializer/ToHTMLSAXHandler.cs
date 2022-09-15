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
 * $Id: ToHTMLSAXHandler.java 475978 2006-11-16 23:31:20Z minchau $
 */

namespace org.apache.xml.serializer
{


	using Node = org.w3c.dom.Node;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// This class accepts SAX-like calls, then sends true SAX calls to a
	/// wrapped SAX handler.  There is optimization done knowing that the ultimate
	/// output is HTML.
	/// 
	/// This class is not a public API.
	/// </summary>
	/// @deprecated As of Xalan 2.7.1, replaced by the use of <seealso cref="ToXMLSAXHandler"/>.
	/// 
	/// @xsl.usage internal 
	public sealed class ToHTMLSAXHandler : ToSAXHandler
	{
		/// <summary>
		///  Handle document type declaration (for first element only)
		/// </summary>
		private bool m_dtdHandled = false;

		/// <summary>
		/// Keeps track of whether output escaping is currently enabled
		/// </summary>
		protected internal bool m_escapeSetting = true;

		/// <summary>
		/// Returns null. </summary>
		/// <returns> null </returns>
		/// <seealso cref= Serializer#getOutputFormat() </seealso>
		public override Properties OutputFormat
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
		/// Reurns null </summary>
		/// <returns> null </returns>
		/// <seealso cref= Serializer#getOutputStream() </seealso>
		public override System.IO.Stream OutputStream
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
		/// Returns null </summary>
		/// <returns> null </returns>
		/// <seealso cref= Serializer#getWriter() </seealso>
		public override Writer Writer
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
		/// Does nothing.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void indent(int n) throws org.xml.sax.SAXException
		public void indent(int n)
		{
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= DOMSerializer#serialize(Node) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public override void serialize(Node node)
		{
			return;
		}

		/// <summary>
		/// Turns special character escaping on/off.
		/// 
		/// </summary>
		/// <param name="escape"> true if escaping is to be set on.
		/// </param>
		/// <seealso cref= SerializationHandler#setEscaping(boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException
		public override bool setEscaping(bool escape)
		{
			bool oldEscapeSetting = m_escapeSetting;
			m_escapeSetting = escape;

			if (escape)
			{
				processingInstruction(Result.PI_ENABLE_OUTPUT_ESCAPING, "");
			}
			else
			{
				processingInstruction(Result.PI_DISABLE_OUTPUT_ESCAPING, "");
			}

			return oldEscapeSetting;
		}

		/// <summary>
		/// Does nothing </summary>
		/// <param name="indent"> the number of spaces to indent per indentation level
		/// (ignored) </param>
		/// <seealso cref= SerializationHandler#setIndent(boolean) </seealso>
		public override bool Indent
		{
			set
			{
			}
		}





		/// <seealso cref= org.xml.sax.ext.DeclHandler#attributeDecl(String, String, String, String, String) </seealso>
		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="eName"> this parameter is ignored </param>
		/// <param name="aName"> this parameter is ignored </param>
		/// <param name="type"> this parameter is ignored </param>
		/// <param name="valueDefault"> this parameter is ignored </param>
		/// <param name="value"> this parameter is ignored </param>
		/// <seealso cref= org.xml.sax.ext.DeclHandler#attributeDecl(String, String, String,String,String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws org.xml.sax.SAXException
		public void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
		{
		}


		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.DeclHandler#elementDecl(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws org.xml.sax.SAXException
		public void elementDecl(string name, string model)
		{
			return;
		}

		/// <seealso cref= org.xml.sax.ext.DeclHandler#externalEntityDecl(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void externalEntityDecl(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public void externalEntityDecl(string arg0, string arg1, string arg2)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException
		public void internalEntityDecl(string name, string value)
		{
		}

		/// <summary>
		/// Receive notification of the end of an element.
		/// 
		/// <para>The SAX parser will invoke this method at the end of every
		/// element in the XML document; there will be a corresponding
		/// startElement() event for every endElement() event (even when the
		/// element is empty).</para>
		/// 
		/// <para>If the element name has a namespace prefix, the prefix will
		/// still be attached to the name.</para>
		/// 
		/// </summary>
		/// <param name="uri"> The Namespace URI, or the empty string if the
		///        element has no Namespace URI or if Namespace
		///        processing is not being performed. </param>
		/// <param name="localName"> The local name (without prefix), or the
		///        empty string if Namespace processing is not being
		///        performed. </param>
		/// <param name="qName"> The qualified name (with prefix), or the
		///        empty string if qualified names are not available. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#endElement(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
		public void endElement(string uri, string localName, string qName)
		{
			flushPending();
			m_saxHandler.endElement(uri, localName, qName);

			// time to fire off endElement event
			if (m_tracer != null)
			{
				base.fireEndElem(qName);
			}
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		public void endPrefixMapping(string prefix)
		{
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public void ignorableWhitespace(char[] ch, int start, int length)
		{
		}

		/// <summary>
		/// Receive notification of a processing instruction.
		/// 
		/// <para>The Parser will invoke this method once for each processing
		/// instruction found: note that processing instructions may occur
		/// before or after the main document element.</para>
		/// 
		/// <para>A SAX parser should never report an XML declaration (XML 1.0,
		/// section 2.8) or a text declaration (XML 1.0, section 4.3.1)
		/// using this method.</para>
		/// </summary>
		/// <param name="target"> The processing instruction target. </param>
		/// <param name="data"> The processing instruction data, or null if
		///        none was supplied. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#processingInstruction(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public override void processingInstruction(string target, string data)
		{
			flushPending();
			m_saxHandler.processingInstruction(target,data);

			// time to fire off processing instruction event

			if (m_tracer != null)
			{
				base.fireEscapingEvent(target,data);
			}
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator(Locator) </seealso>
		public override Locator DocumentLocator
		{
			set
			{
				// do nothing
			}
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#skippedEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String arg0) throws org.xml.sax.SAXException
		public void skippedEntity(string arg0)
		{
		}

		/// <summary>
		/// Receive notification of the beginning of an element, although this is a
		/// SAX method additional namespace or attribute information can occur before
		/// or after this call, that is associated with this element.
		/// 
		/// </summary>
		/// <param name="namespaceURI"> The Namespace URI, or the empty string if the
		///        element has no Namespace URI or if Namespace
		///        processing is not being performed. </param>
		/// <param name="localName"> The local name (without prefix), or the
		///        empty string if Namespace processing is not being
		///        performed. </param>
		/// <param name="qName"> The elements name. </param>
		/// <param name="atts"> The attributes attached to the element, if any. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#startElement </seealso>
		/// <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
		/// <seealso cref= org.xml.sax.AttributeList
		/// </seealso>
		/// <exception cref="org.xml.sax.SAXException">
		/// </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#startElement(String, String, String, Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String qName, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public override void startElement(string namespaceURI, string localName, string qName, Attributes atts)
		{
			flushPending();
			base.startElement(namespaceURI, localName, qName, atts);
			m_saxHandler.startElement(namespaceURI, localName, qName, atts);
			m_elemContext.m_startTagOpen = false;
		}

		/// <summary>
		/// Receive notification of a comment anywhere in the document. This callback
		/// will be used for comments inside or outside the document element. </summary>
		/// <param name="ch"> An array holding the characters in the comment. </param>
		/// <param name="start"> The starting position in the array. </param>
		/// <param name="length"> The number of characters to use from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception.
		/// </exception>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#comment(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public void comment(char[] ch, int start, int length)
		{
			flushPending();
			if (m_lexHandler != null)
			{
				m_lexHandler.comment(ch, start, length);
			}

			// time to fire off comment event
			if (m_tracer != null)
			{
				base.fireCommentEvent(ch, start, length);
			}
			return;
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public void endCDATA()
		{
			return;
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endDTD() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public void endDTD()
		{
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public void startCDATA()
		{
		}

		/// <summary>
		/// Does nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(String arg0) throws org.xml.sax.SAXException
		public void startEntity(string arg0)
		{
		}

		/// <summary>
		/// Receive notification of the end of a document.
		/// 
		/// <para>The SAX parser will invoke this method only once, and it will
		/// be the last method invoked during the parse.  The parser shall
		/// not invoke this method until it has either abandoned parsing
		/// (because of an unrecoverable error) or reached the end of
		/// input.</para>
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException">
		/// 
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public void endDocument()
		{
			flushPending();

			// Close output document
			m_saxHandler.endDocument();

			if (m_tracer != null)
			{
				base.fireEndDoc();
			}
		}

		/// <summary>
		/// This method is called when all the data needed for a call to the
		/// SAX handler's startElement() method has been gathered.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeStartTag() throws org.xml.sax.SAXException
		protected internal override void closeStartTag()
		{

			m_elemContext.m_startTagOpen = false;

			// Now is time to send the startElement event
			m_saxHandler.startElement(SerializerConstants_Fields.EMPTYSTRING, m_elemContext.m_elementName, m_elemContext.m_elementName, m_attributes);
			m_attributes.clear();

		}

		/// <summary>
		/// Do nothing. </summary>
		/// <seealso cref= SerializationHandler#close() </seealso>
		public override void close()
		{
			return;
		}

		/// <summary>
		/// Receive notification of character data.
		/// </summary>
		/// <param name="chars"> The string of characters to process.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException">
		/// </exception>
		/// <seealso cref= ExtendedContentHandler#characters(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(final String chars) throws org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
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
		/// A constructor </summary>
		/// <param name="handler"> the wrapped SAX content handler </param>
		/// <param name="encoding"> the encoding of the output HTML document </param>
		public ToHTMLSAXHandler(ContentHandler handler, string encoding) : base(handler,encoding)
		{
		}
		/// <summary>
		/// A constructor. </summary>
		/// <param name="handler"> the wrapped SAX content handler </param>
		/// <param name="lex"> the wrapped lexical handler </param>
		/// <param name="encoding"> the encoding of the output HTML document </param>
		public ToHTMLSAXHandler(ContentHandler handler, LexicalHandler lex, string encoding) : base(handler,lex,encoding)
		{
		}

		/// <summary>
		/// An element starts, but attributes are not fully known yet.
		/// </summary>
		/// <param name="elementNamespaceURI"> the URI of the namespace of the element
		/// (optional) </param>
		/// <param name="elementLocalName"> the element name, but without prefix
		/// (optional) </param>
		/// <param name="elementName"> the element name, with prefix, if any (required)
		/// </param>
		/// <seealso cref= ExtendedContentHandler#startElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementNamespaceURI, String elementLocalName, String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementNamespaceURI, string elementLocalName, string elementName)
		{

			base.startElement(elementNamespaceURI, elementLocalName, elementName);

			flushPending();

			// Handle document type declaration (for first element only)
			if (!m_dtdHandled)
			{
				string doctypeSystem = DoctypeSystem;
				string doctypePublic = DoctypePublic;
				if ((!string.ReferenceEquals(doctypeSystem, null)) || (!string.ReferenceEquals(doctypePublic, null)))
				{
					if (m_lexHandler != null)
					{
						m_lexHandler.startDTD(elementName, doctypePublic, doctypeSystem);
					}
				}
				m_dtdHandled = true;
			}
			m_elemContext = m_elemContext.push(elementNamespaceURI, elementLocalName, elementName);
		}
		/// <summary>
		/// An element starts, but attributes are not fully known yet.
		/// </summary>
		/// <param name="elementName"> the element name, with prefix, if any
		/// </param>
		/// <seealso cref= ExtendedContentHandler#startElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementName)
		{
			this.startElement(null,null, elementName);
		}

		/// <summary>
		/// Receive notification of the end of an element. </summary>
		/// <param name="elementName"> The element type name </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///     wrapping another exception.
		/// </exception>
		/// <seealso cref= ExtendedContentHandler#endElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String elementName) throws org.xml.sax.SAXException
		public override void endElement(string elementName)
		{
			flushPending();
			m_saxHandler.endElement(SerializerConstants_Fields.EMPTYSTRING, elementName, elementName);

			// time to fire off endElement event
			if (m_tracer != null)
			{
				base.fireEndElem(elementName);
			}
		}

		/// <summary>
		/// Receive notification of character data.
		/// 
		/// <para>The Parser will call this method to report each chunk of
		/// character data.  SAX parsers may return all contiguous character
		/// data in a single chunk, or they may split it into several
		/// chunks; however, all of the characters in any single event
		/// must come from the same external entity, so that the Locator
		/// provides useful information.</para>
		/// 
		/// <para>The application must not attempt to read from the array
		/// outside of the specified range.</para>
		/// 
		/// <para>Note that some parsers will report whitespace using the
		/// ignorableWhitespace() method rather than this one (validating
		/// parsers must do so).</para>
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="off"> The start position in the array. </param>
		/// <param name="len"> The number of characters to read from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref= #ignorableWhitespace </seealso>
		/// <seealso cref= org.xml.sax.Locator
		/// </seealso>
		/// <exception cref="org.xml.sax.SAXException">
		/// </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#characters(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int off, int len) throws org.xml.sax.SAXException
		public void characters(char[] ch, int off, int len)
		{

			flushPending();
			m_saxHandler.characters(ch, off, len);

			// time to fire off characters event
			if (m_tracer != null)
			{
				base.fireCharEvent(ch, off, len);
			}
		}

		/// <summary>
		/// This method flushes any pending events, which can be startDocument()
		/// closing the opening tag of an element, or closing an open CDATA section.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException
		public override void flushPending()
		{
			if (m_needToCallStartDocument)
			{
				startDocumentInternal();
				m_needToCallStartDocument = false;
			}
			// Close any open element
			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}
		}
		/// <summary>
		/// Handle a prefix/uri mapping, which is associated with a startElement()
		/// that is soon to follow. Need to close any open start tag to make
		/// sure than any name space attributes due to this event are associated wih
		/// the up comming element, not the current one. </summary>
		/// <seealso cref= ExtendedContentHandler#startPrefixMapping
		/// </seealso>
		/// <param name="prefix"> The Namespace prefix being declared. </param>
		/// <param name="uri"> The Namespace URI the prefix is mapped to. </param>
		/// <param name="shouldFlush"> true if any open tags need to be closed first, this
		/// will impact which element the mapping applies to (open parent, or its up
		/// comming child) </param>
		/// <returns> returns true if the call made a change to the current
		/// namespace information, false if it did not change anything, e.g. if the
		/// prefix/namespace mapping was already in scope from before.
		/// </returns>
		/// <exception cref="org.xml.sax.SAXException"> The client may throw
		///            an exception during processing. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public override bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{
			// no namespace support for HTML
			if (shouldFlush)
			{
				flushPending();
			}
			m_saxHandler.startPrefixMapping(prefix,uri);
			return false;
		}

		/// <summary>
		/// Begin the scope of a prefix-URI Namespace mapping
		/// just before another element is about to start.
		/// This call will close any open tags so that the prefix mapping
		/// will not apply to the current element, but the up comming child.
		/// </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping
		/// </seealso>
		/// <param name="prefix"> The Namespace prefix being declared. </param>
		/// <param name="uri"> The Namespace URI the prefix is mapped to.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> The client may throw
		///            an exception during processing.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public void startPrefixMapping(string prefix, string uri)
		{
			startPrefixMapping(prefix,uri,true);
		}

		/// <summary>
		/// This method is used when a prefix/uri namespace mapping
		/// is indicated after the element was started with a
		/// startElement() and before and endElement().
		/// startPrefixMapping(prefix,uri) would be used before the
		/// startElement() call. </summary>
		/// <param name="prefix"> the prefix associated with the given URI. </param>
		/// <param name="uri"> the URI of the namespace
		/// </param>
		/// <seealso cref= ExtendedContentHandler#namespaceAfterStartElement(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(final String prefix, final String uri) throws org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public override void namespaceAfterStartElement(string prefix, string uri)
		{
			// hack for XSLTC with finding URI for default namespace
			if (string.ReferenceEquals(m_elemContext.m_elementURI, null))
			{
				string prefix1 = getPrefixPart(m_elemContext.m_elementName);
				if (string.ReferenceEquals(prefix1, null) && SerializerConstants_Fields.EMPTYSTRING.Equals(prefix))
				{
					// the elements URI is not known yet, and it
					// doesn't have a prefix, and we are currently
					// setting the uri for prefix "", so we have
					// the uri for the element... lets remember it
					m_elemContext.m_elementURI = uri;
				}
			}
			startPrefixMapping(prefix,uri,false);
		}

		/// <summary>
		/// Try's to reset the super class and reset this class for 
		/// re-use, so that you don't need to create a new serializer 
		/// (mostly for performance reasons).
		/// </summary>
		/// <returns> true if the class was successfuly reset. </returns>
		/// <seealso cref= Serializer#reset() </seealso>
		public override bool reset()
		{
			bool wasReset = false;
			if (base.reset())
			{
				resetToHTMLSAXHandler();
				wasReset = true;
			}
			return wasReset;
		}

		/// <summary>
		/// Reset all of the fields owned by ToHTMLSAXHandler class
		/// 
		/// </summary>
		private void resetToHTMLSAXHandler()
		{
			this.m_dtdHandled = false;
			this.m_escapeSetting = true;
		}
	}

}