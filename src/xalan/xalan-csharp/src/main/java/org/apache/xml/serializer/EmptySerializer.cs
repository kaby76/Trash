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
 * $Id: EmptySerializer.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer
{



	using Node = org.w3c.dom.Node;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;

	/// <summary>
	/// This class is an adapter class. Its only purpose is to be extended and
	/// for that extended class to over-ride all methods that are to be used. 
	/// 
	/// This class is not a public API, it is only public because it is used
	/// across package boundaries.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public class EmptySerializer : SerializationHandler
	{
		protected internal const string ERR = "EmptySerializer method not over-ridden";
		/// <seealso cref= SerializationHandler#asContentHandler() </seealso>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void couldThrowIOException() throws java.io.IOException
		protected internal virtual void couldThrowIOException()
		{
			return; // don't do anything.
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void couldThrowSAXException() throws org.xml.sax.SAXException
		protected internal virtual void couldThrowSAXException()
		{
			return; // don't do anything.
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void couldThrowSAXException(char[] chars, int off, int len) throws org.xml.sax.SAXException
		protected internal virtual void couldThrowSAXException(char[] chars, int off, int len)
		{
			return; // don't do anything.
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void couldThrowSAXException(String elemQName) throws org.xml.sax.SAXException
		protected internal virtual void couldThrowSAXException(string elemQName)
		{
			return; // don't do anything.
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void couldThrowException() throws Exception
		protected internal virtual void couldThrowException()
		{
			return; // don't do anything.
		}

		internal virtual void aMethodIsCalled()
		{

			// throw new RuntimeException(err);
			return;
		}


		/// <seealso cref= SerializationHandler#asContentHandler() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException
		public virtual ContentHandler asContentHandler()
		{
			couldThrowIOException();
			return null;
		}
		/// <seealso cref= SerializationHandler#setContentHandler(org.xml.sax.ContentHandler) </seealso>
		public virtual ContentHandler ContentHandler
		{
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= SerializationHandler#close() </seealso>
		public virtual void close()
		{
			aMethodIsCalled();
		}
		/// <seealso cref= SerializationHandler#getOutputFormat() </seealso>
		public virtual Properties OutputFormat
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= SerializationHandler#getOutputStream() </seealso>
		public virtual System.IO.Stream OutputStream
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= SerializationHandler#getWriter() </seealso>
		public virtual Writer Writer
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= SerializationHandler#reset() </seealso>
		public virtual bool reset()
		{
			aMethodIsCalled();
			return false;
		}
		/// <seealso cref= SerializationHandler#serialize(org.w3c.dom.Node) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public virtual void serialize(Node node)
		{
			couldThrowIOException();
		}
		/// <seealso cref= SerializationHandler#setCdataSectionElements(java.util.Vector) </seealso>
		public virtual ArrayList CdataSectionElements
		{
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= SerializationHandler#setEscaping(boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException
		public virtual bool setEscaping(bool escape)
		{
			couldThrowSAXException();
			return false;
		}
		/// <seealso cref= SerializationHandler#setIndent(boolean) </seealso>
		public virtual bool Indent
		{
			set
			{
				aMethodIsCalled();
			}
			get
			{
				aMethodIsCalled();
				return false;
			}
		}
		/// <seealso cref= SerializationHandler#setIndentAmount(int) </seealso>
		public virtual int IndentAmount
		{
			set
			{
				aMethodIsCalled();
			}
			get
			{
				aMethodIsCalled();
				return 0;
			}
		}
		/// <seealso cref= SerializationHandler#setVersion(java.lang.String) </seealso>
		public virtual string Version
		{
			set
			{
				aMethodIsCalled();
			}
			get
			{
				aMethodIsCalled();
				return null;
			}
		}
		/// <seealso cref= SerializationHandler#setTransformer(javax.xml.transform.Transformer) </seealso>
		public virtual Transformer Transformer
		{
			set
			{
				aMethodIsCalled();
			}
			get
			{
				aMethodIsCalled();
				return null;
			}
		}
		/// <seealso cref= SerializationHandler#flushPending() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException
		public virtual void flushPending()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#addAttribute(java.lang.String, java.lang.String, java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value, boolean XSLAttribute) throws org.xml.sax.SAXException
		public virtual void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#addAttributes(org.xml.sax.Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttributes(org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public virtual void addAttributes(Attributes atts)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#addAttribute(java.lang.String, java.lang.String) </seealso>
		public virtual void addAttribute(string name, string value)
		{
			aMethodIsCalled();
		}

		/// <seealso cref= ExtendedContentHandler#characters(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String chars) throws org.xml.sax.SAXException
		public virtual void characters(string chars)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#endElement(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String elemName) throws org.xml.sax.SAXException
		public virtual void endElement(string elemName)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#startDocument() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public virtual void startDocument()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#startElement(java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
		public virtual void startElement(string uri, string localName, string qName)
		{
			couldThrowSAXException(qName);
		}
		/// <seealso cref= ExtendedContentHandler#startElement(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String qName) throws org.xml.sax.SAXException
		public virtual void startElement(string qName)
		{
			couldThrowSAXException(qName);
		}
		/// <seealso cref= ExtendedContentHandler#namespaceAfterStartElement(java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(String uri, String prefix) throws org.xml.sax.SAXException
		public virtual void namespaceAfterStartElement(string uri, string prefix)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#startPrefixMapping(java.lang.String, java.lang.String, boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public virtual bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{
			couldThrowSAXException();
			return false;
		}
		/// <seealso cref= ExtendedContentHandler#entityReference(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void entityReference(String entityName) throws org.xml.sax.SAXException
		public virtual void entityReference(string entityName)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedContentHandler#getNamespaceMappings() </seealso>
		public virtual NamespaceMappings NamespaceMappings
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= ExtendedContentHandler#getPrefix(java.lang.String) </seealso>
		public virtual string getPrefix(string uri)
		{
			aMethodIsCalled();
			return null;
		}
		/// <seealso cref= ExtendedContentHandler#getNamespaceURI(java.lang.String, boolean) </seealso>
		public virtual string getNamespaceURI(string name, bool isElement)
		{
			aMethodIsCalled();
			return null;
		}
		/// <seealso cref= ExtendedContentHandler#getNamespaceURIFromPrefix(java.lang.String) </seealso>
		public virtual string getNamespaceURIFromPrefix(string prefix)
		{
			aMethodIsCalled();
			return null;
		}
		/// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator(org.xml.sax.Locator) </seealso>
		public virtual Locator DocumentLocator
		{
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= org.xml.sax.ContentHandler#endDocument() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public virtual void endDocument()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping(java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String arg0, String arg1) throws org.xml.sax.SAXException
		public virtual void startPrefixMapping(string arg0, string arg1)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String arg0) throws org.xml.sax.SAXException
		public virtual void endPrefixMapping(string arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#startElement(java.lang.String, java.lang.String, java.lang.String, org.xml.sax.Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String arg0, String arg1, String arg2, org.xml.sax.Attributes arg3) throws org.xml.sax.SAXException
		public virtual void startElement(string arg0, string arg1, string arg2, Attributes arg3)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#endElement(java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void endElement(string arg0, string arg1, string arg2)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#characters(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public virtual void characters(char[] arg0, int arg1, int arg2)
		{
			couldThrowSAXException(arg0, arg1, arg2);
		}
		/// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public virtual void ignorableWhitespace(char[] arg0, int arg1, int arg2)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#processingInstruction(java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String arg0, String arg1) throws org.xml.sax.SAXException
		public virtual void processingInstruction(string arg0, string arg1)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ContentHandler#skippedEntity(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String arg0) throws org.xml.sax.SAXException
		public virtual void skippedEntity(string arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= ExtendedLexicalHandler#comment(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(String comment) throws org.xml.sax.SAXException
		public virtual void comment(string comment)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startDTD(java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDTD(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void startDTD(string arg0, string arg1, string arg2)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endDTD() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public virtual void endDTD()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startEntity(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(String arg0) throws org.xml.sax.SAXException
		public virtual void startEntity(string arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endEntity(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endEntity(String arg0) throws org.xml.sax.SAXException
		public virtual void endEntity(string arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public virtual void startCDATA()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public virtual void endCDATA()
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#comment(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public virtual void comment(char[] arg0, int arg1, int arg2)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= XSLOutputAttributes#getDoctypePublic() </seealso>
		public virtual string DoctypePublic
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#getDoctypeSystem() </seealso>
		public virtual string DoctypeSystem
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#getEncoding() </seealso>
		public virtual string Encoding
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#getMediaType() </seealso>
		public virtual string MediaType
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#getOmitXMLDeclaration() </seealso>
		public virtual bool OmitXMLDeclaration
		{
			get
			{
				aMethodIsCalled();
				return false;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#getStandalone() </seealso>
		public virtual string Standalone
		{
			get
			{
				aMethodIsCalled();
				return null;
			}
			set
			{
				aMethodIsCalled();
			}
		}
		/// <seealso cref= XSLOutputAttributes#setCdataSectionElements </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCdataSectionElements(java.util.Hashtable h) throws Exception
		public virtual Hashtable CdataSectionElements
		{
			set
			{
				couldThrowException();
			}
		}
		/// <seealso cref= XSLOutputAttributes#setDoctype(java.lang.String, java.lang.String) </seealso>
		public virtual void setDoctype(string system, string pub)
		{
			aMethodIsCalled();
		}
		/// <seealso cref= org.xml.sax.ext.DeclHandler#elementDecl(java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void elementDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public virtual void elementDecl(string arg0, string arg1)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.DeclHandler#attributeDecl(java.lang.String, java.lang.String, java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void attributeDecl(String arg0, String arg1, String arg2, String arg3, String arg4) throws org.xml.sax.SAXException
		public virtual void attributeDecl(string arg0, string arg1, string arg2, string arg3, string arg4)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.DeclHandler#internalEntityDecl(java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void internalEntityDecl(String arg0, String arg1) throws org.xml.sax.SAXException
		public virtual void internalEntityDecl(string arg0, string arg1)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ext.DeclHandler#externalEntityDecl(java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void externalEntityDecl(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void externalEntityDecl(string arg0, string arg1, string arg2)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ErrorHandler#warning(org.xml.sax.SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException arg0) throws org.xml.sax.SAXException
		public virtual void warning(SAXParseException arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ErrorHandler#error(org.xml.sax.SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException arg0) throws org.xml.sax.SAXException
		public virtual void error(SAXParseException arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.ErrorHandler#fatalError(org.xml.sax.SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException arg0) throws org.xml.sax.SAXException
		public virtual void fatalError(SAXParseException arg0)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= Serializer#asDOMSerializer() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException
		public virtual DOMSerializer asDOMSerializer()
		{
			couldThrowIOException();
			return null;
		}


		/// <seealso cref= ExtendedContentHandler#setSourceLocator(javax.xml.transform.SourceLocator) </seealso>
		public virtual SourceLocator SourceLocator
		{
			set
			{
				aMethodIsCalled();
			}
		}

		/// <seealso cref= ExtendedContentHandler#addUniqueAttribute(java.lang.String, java.lang.String, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addUniqueAttribute(String name, String value, int flags) throws org.xml.sax.SAXException
		public virtual void addUniqueAttribute(string name, string value, int flags)
		{
			couldThrowSAXException();
		}

		/// <seealso cref= ExtendedContentHandler#characters(org.w3c.dom.Node) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		public virtual void characters(Node node)
		{
			couldThrowSAXException();
		}

		/// <seealso cref= ExtendedContentHandler#addXSLAttribute(java.lang.String, java.lang.String, java.lang.String) </seealso>
		public virtual void addXSLAttribute(string qName, string value, string uri)
		{
			aMethodIsCalled();
		}

		/// <seealso cref= ExtendedContentHandler#addAttribute(java.lang.String, java.lang.String, java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value) throws org.xml.sax.SAXException
		public virtual void addAttribute(string uri, string localName, string rawName, string type, string value)
		{
			couldThrowSAXException();
		}
		/// <seealso cref= org.xml.sax.DTDHandler#notationDecl(java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notationDecl(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void notationDecl(string arg0, string arg1, string arg2)
		{
			couldThrowSAXException();
		}

		/// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl(java.lang.String, java.lang.String, java.lang.String, java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unparsedEntityDecl(String arg0, String arg1, String arg2, String arg3) throws org.xml.sax.SAXException
		public virtual void unparsedEntityDecl(string arg0, string arg1, string arg2, string arg3)
		{
			couldThrowSAXException();
		}

		/// <seealso cref= SerializationHandler#setDTDEntityExpansion(boolean) </seealso>
		public virtual bool DTDEntityExpansion
		{
			set
			{
				aMethodIsCalled();
    
			}
		}


		public virtual string getOutputProperty(string name)
		{
			aMethodIsCalled();
			return null;
		}

		public virtual string getOutputPropertyDefault(string name)
		{
			aMethodIsCalled();
			return null;
		}

		public virtual void setOutputProperty(string name, string val)
		{
			aMethodIsCalled();

		}

		public virtual void setOutputPropertyDefault(string name, string val)
		{
			aMethodIsCalled();

		}

		/// <seealso cref= org.apache.xml.serializer.Serializer#asDOM3Serializer() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object asDOM3Serializer() throws java.io.IOException
		public virtual object asDOM3Serializer()
		{
			couldThrowIOException();
			return null;
		}
	}

}