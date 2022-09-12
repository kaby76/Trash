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
 * $Id: ToTextSAXHandler.java 475978 2006-11-16 23:31:20Z minchau $
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
	/// This class converts SAX-like event to SAX events for
	/// xsl:output method "text". 
	/// 
	/// This class is only to be used internally. This class is not a public API.
	/// </summary>
	/// @deprecated As of Xalan 2.7.1, replaced by the use of <seealso cref="ToXMLSAXHandler"/>.
	/// 
	/// @xsl.usage internal 
	public sealed class ToTextSAXHandler : ToSAXHandler
	{
		/// <summary>
		/// From XSLTC </summary>
		/// <seealso cref= ExtendedContentHandler#endElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String elemName) throws org.xml.sax.SAXException
		public override void endElement(string elemName)
		{
			if (m_tracer != null)
			{
				base.fireEndElem(elemName);
			}
		}

		/// <seealso cref= org.xml.sax.ContentHandler#endElement(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public void endElement(string arg0, string arg1, string arg2)
		{
			if (m_tracer != null)
			{
				base.fireEndElem(arg2);
			}
		}

		public ToTextSAXHandler(ContentHandler hdlr, LexicalHandler lex, string encoding) : base(hdlr, lex, encoding)
		{
		}

			/// <summary>
			/// From XSLTC
			/// </summary>
		public ToTextSAXHandler(ContentHandler handler, string encoding) : base(handler,encoding)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
		public void comment(char[] ch, int start, int length)
		{
			if (m_tracer != null)
			{
				base.fireCommentEvent(ch, start, length);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(String data) throws org.xml.sax.SAXException
		public override void comment(string data)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = data.length();
			int length = data.Length;
			if (length > m_charsBuff.Length)
			{
				m_charsBuff = new char[length * 2 + 1];
			}
			data.CopyTo(0, m_charsBuff, 0, length - 0);
			comment(m_charsBuff, 0, length);
		}

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
		/// Does nothing because 
		/// the indent attribute is ignored for text output.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void indent(int n) throws org.xml.sax.SAXException
		public void indent(int n)
		{
		}

		/// <seealso cref= Serializer#reset() </seealso>
		public override bool reset()
		{
			return false;
		}

		/// <seealso cref= DOMSerializer#serialize(Node) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public override void serialize(Node node)
		{
		}

		/// <seealso cref= SerializationHandler#setEscaping(boolean) </seealso>
		public override bool setEscaping(bool escape)
		{
			return false;
		}

		/// <seealso cref= SerializationHandler#setIndent(boolean) </seealso>
		public override bool Indent
		{
			set
			{
			}
		}




		/// <seealso cref= ExtendedContentHandler#addAttribute(String, String, String, String, String) </seealso>
		public override void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
		}

		/// <seealso cref= org.xml.sax.ext.DeclHandler#attributeDecl(String, String, String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void attributeDecl(String arg0, String arg1, String arg2, String arg3, String arg4) throws org.xml.sax.SAXException
		public void attributeDecl(string arg0, string arg1, string arg2, string arg3, string arg4)
		{
		}

		/// <seealso cref= org.xml.sax.ext.DeclHandler#elementDecl(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void elementDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public void elementDecl(string arg0, string arg1)
		{
		}

		/// <seealso cref= org.xml.sax.ext.DeclHandler#externalEntityDecl(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void externalEntityDecl(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public void externalEntityDecl(string arg0, string arg1, string arg2)
		{
		}

		/// <seealso cref= org.xml.sax.ext.DeclHandler#internalEntityDecl(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void internalEntityDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public void internalEntityDecl(string arg0, string arg1)
		{
		}

		/// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String arg0) throws org.xml.sax.SAXException
		public void endPrefixMapping(string arg0)
		{
		}

		/// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public void ignorableWhitespace(char[] arg0, int arg1, int arg2)
		{
		}

		/// <summary>
		/// From XSLTC </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#processingInstruction(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String arg0, String arg1) throws org.xml.sax.SAXException
		public override void processingInstruction(string arg0, string arg1)
		{
			if (m_tracer != null)
			{
				base.fireEscapingEvent(arg0, arg1);
			}
		}

		/// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator(Locator) </seealso>
		public override Locator DocumentLocator
		{
			set
			{
			}
		}

		/// <seealso cref= org.xml.sax.ContentHandler#skippedEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String arg0) throws org.xml.sax.SAXException
		public void skippedEntity(string arg0)
		{
		}

		/// <seealso cref= org.xml.sax.ContentHandler#startElement(String, String, String, Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String arg0, String arg1, String arg2, org.xml.sax.Attributes arg3) throws org.xml.sax.SAXException
		public override void startElement(string arg0, string arg1, string arg2, Attributes arg3)
		{
			flushPending();
			base.startElement(arg0, arg1, arg2, arg3);
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public void endCDATA()
		{
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endDTD() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public void endDTD()
		{
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public void startCDATA()
		{
		}


		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(String arg0) throws org.xml.sax.SAXException
		public void startEntity(string arg0)
		{
		}


		/// <summary>
		/// From XSLTC </summary>
		/// <seealso cref= ExtendedContentHandler#startElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementNamespaceURI, String elementLocalName, String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementNamespaceURI, string elementLocalName, string elementName)
		{
			base.startElement(elementNamespaceURI, elementLocalName, elementName);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementName)
		{
			base.startElement(elementName);
		}


		/// <summary>
		/// From XSLTC </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#endDocument() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public void endDocument()
		{

			flushPending();
			m_saxHandler.endDocument();

			if (m_tracer != null)
			{
				base.fireEndDoc();
			}
		}

		/// 
		/// <seealso cref= ExtendedContentHandler#characters(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String characters) throws org.xml.sax.SAXException
		public override void characters(string characters)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = characters.length();
			int length = characters.Length;
			if (length > m_charsBuff.Length)
			{
				m_charsBuff = new char[length * 2 + 1];
			}
			characters.CopyTo(0, m_charsBuff, 0, length - 0);

			m_saxHandler.characters(m_charsBuff, 0, length);

		}
		/// <seealso cref= org.xml.sax.ContentHandler#characters(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] characters, int offset, int length) throws org.xml.sax.SAXException
		public void characters(char[] characters, int offset, int length)
		{

			m_saxHandler.characters(characters, offset, length);

			// time to fire off characters event
			if (m_tracer != null)
			{
				base.fireCharEvent(characters, offset, length);
			}
		}

		/// <summary>
		/// From XSLTC
		/// </summary>
		public override void addAttribute(string name, string value)
		{
			// do nothing
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public override bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{
			// no namespace support for HTML
			return false;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public void startPrefixMapping(string prefix, string uri)
		{
			// no namespace support for HTML
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(final String prefix, final String uri) throws org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public override void namespaceAfterStartElement(string prefix, string uri)
		{
			// no namespace support for HTML
		}

	}

}