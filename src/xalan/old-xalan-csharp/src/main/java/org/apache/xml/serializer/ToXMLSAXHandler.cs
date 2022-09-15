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
 * $Id: ToXMLSAXHandler.java 468654 2006-10-28 07:09:23Z minchau $
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
	/// This class receives notification of SAX-like events, and with gathered
	/// information over these calls it will invoke the equivalent SAX methods
	/// on a handler, the ultimate xsl:output method is known to be "xml".
	/// 
	/// This class is not a public API.
	/// @xsl.usage internal
	/// </summary>
	public sealed class ToXMLSAXHandler : ToSAXHandler
	{

		/// <summary>
		/// Keeps track of whether output escaping is currently enabled
		/// </summary>
		protected internal bool m_escapeSetting = true;

		public ToXMLSAXHandler()
		{
			// default constructor (need to set content handler ASAP !)
			m_prefixMap = new NamespaceMappings();
			initCDATA();
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
		/// Do nothing for SAX.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void indent(int n) throws org.xml.sax.SAXException
		public void indent(int n)
		{
		}


		/// <seealso cref= DOMSerializer#serialize(Node) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public override void serialize(Node node)
		{
		}

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

		/// <summary>
		/// Receives notification of the end of the document. </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#endDocument() </seealso>
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = getLocalName(m_elemContext.m_elementName);
			string localName = getLocalName(m_elemContext.m_elementName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = getNamespaceURI(m_elemContext.m_elementName, true);
			string uri = getNamespaceURI(m_elemContext.m_elementName, true);

			// Now is time to send the startElement event
			if (m_needToCallStartDocument)
			{
				startDocumentInternal();
			}
			m_saxHandler.startElement(uri, localName, m_elemContext.m_elementName, m_attributes);
			// we've sent the official SAX attributes on their way,
			// now we don't need them anymore.
			m_attributes.clear();

			if (m_state != null)
			{
			  m_state.CurrentNode = null;
			}
		}

		/// <summary>
		/// Closes ane open cdata tag, and
		/// unlike the this.endCDATA() method (from the LexicalHandler) interface,
		/// this "internal" method will send the endCDATA() call to the wrapped
		/// handler.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeCDATA() throws org.xml.sax.SAXException
		public override void closeCDATA()
		{

			// Output closing bracket - "]]>"
			if (m_lexHandler != null && m_cdataTagOpen)
			{
				m_lexHandler.endCDATA();
			}


			// There are no longer any calls made to 
			// m_lexHandler.startCDATA() without a balancing call to
			// m_lexHandler.endCDATA()
			// so we set m_cdataTagOpen to false to remember this.
			m_cdataTagOpen = false;
		}

		/// <seealso cref= org.xml.sax.ContentHandler#endElement(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String qName) throws org.xml.sax.SAXException
		public void endElement(string namespaceURI, string localName, string qName)
		{
			// Close any open elements etc.
			flushPending();

			if (string.ReferenceEquals(namespaceURI, null))
			{
				if (!string.ReferenceEquals(m_elemContext.m_elementURI, null))
				{
					namespaceURI = m_elemContext.m_elementURI;
				}
				else
				{
					namespaceURI = getNamespaceURI(qName, true);
				}
			}

			if (string.ReferenceEquals(localName, null))
			{
				if (!string.ReferenceEquals(m_elemContext.m_elementLocalName, null))
				{
					localName = m_elemContext.m_elementLocalName;
				}
				else
				{
					localName = getLocalName(qName);
				}
			}

			m_saxHandler.endElement(namespaceURI, localName, qName);

			if (m_tracer != null)
			{
				base.fireEndElem(qName);
			}

			/* Pop all namespaces at the current element depth.
			 * We are not waiting for official endPrefixMapping() calls.
			 */
			m_prefixMap.popNamespaces(m_elemContext.m_currentElemDepth, m_saxHandler);
			m_elemContext = m_elemContext.m_prev;
		}

		/// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		public void endPrefixMapping(string prefix)
		{
			/* poping all prefix mappings should have been done
			 * in endElement() already
			 */
			 return;
		}

		/// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public void ignorableWhitespace(char[] arg0, int arg1, int arg2)
		{
			m_saxHandler.ignorableWhitespace(arg0,arg1,arg2);
		}

		/// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator(Locator) </seealso>
		public override Locator DocumentLocator
		{
			set
			{
				m_saxHandler.DocumentLocator = value;
			}
		}

		/// <seealso cref= org.xml.sax.ContentHandler#skippedEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String arg0) throws org.xml.sax.SAXException
		public void skippedEntity(string arg0)
		{
			m_saxHandler.skippedEntity(arg0);
		}

		/// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping(String, String) </seealso>
		/// <param name="prefix"> The prefix that maps to the URI </param>
		/// <param name="uri"> The URI for the namespace </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public void startPrefixMapping(string prefix, string uri)
		{
		   startPrefixMapping(prefix, uri, true);
		}

		/// <summary>
		/// Remember the prefix/uri mapping at the current nested element depth.
		/// </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping(String, String) </seealso>
		/// <param name="prefix"> The prefix that maps to the URI </param>
		/// <param name="uri"> The URI for the namespace </param>
		/// <param name="shouldFlush"> a flag indicating if the mapping applies to the
		/// current element or an up coming child (not used). </param>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public override bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{

			/* Remember the mapping, and at what depth it was declared
			 * This is one greater than the current depth because these
			 * mappings will apply to the next depth. This is in
			 * consideration that startElement() will soon be called
			 */

			bool pushed;
			int pushDepth;
			if (shouldFlush)
			{
				flushPending();
				// the prefix mapping applies to the child element (one deeper)
				pushDepth = m_elemContext.m_currentElemDepth + 1;
			}
			else
			{
				// the prefix mapping applies to the current element
				pushDepth = m_elemContext.m_currentElemDepth;
			}
			pushed = m_prefixMap.pushNamespace(prefix, uri, pushDepth);

			if (pushed)
			{
				m_saxHandler.startPrefixMapping(prefix,uri);

				if (ShouldOutputNSAttr)
				{

					  /* I don't know if we really needto do this. The
					   * callers of this object should have injected both
					   * startPrefixMapping and the attributes.  We are
					   * just covering our butt here.
					   */
					  string name;
					  if (SerializerConstants_Fields.EMPTYSTRING.Equals(prefix))
					  {
						  name = "xmlns";
						  addAttributeAlways(SerializerConstants_Fields.XMLNS_URI, name, name,"CDATA",uri, false);
					  }
					  else
					  {
						  if (!SerializerConstants_Fields.EMPTYSTRING.Equals(uri)) // hack for attribset16 test
						  { // that maps ns1 prefix to "" URI
							  name = "xmlns:" + prefix;

							  /* for something like xmlns:abc="w3.pretend.org"
									 *  the uri is the value, that is why we pass it in the
									 * value, or 5th slot of addAttributeAlways()
								  */
							  addAttributeAlways(SerializerConstants_Fields.XMLNS_URI, prefix, name,"CDATA",uri, false);
						  }
					  }
				}
			}
			return pushed;
		}


		/// <seealso cref= org.xml.sax.ext.LexicalHandler#comment(char[], int, int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char[] arg0, int arg1, int arg2) throws org.xml.sax.SAXException
		public void comment(char[] arg0, int arg1, int arg2)
		{
			flushPending();
			if (m_lexHandler != null)
			{
				m_lexHandler.comment(arg0, arg1, arg2);
			}

			if (m_tracer != null)
			{
				base.fireCommentEvent(arg0, arg1, arg2);
			}
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public void endCDATA()
		{
			/* Normally we would do somthing with this but we ignore it.
			 * The neccessary call to m_lexHandler.endCDATA() will be made
			 * in flushPending().
			 * 
			 * This is so that if we get calls like these:
			 *   this.startCDATA();
			 *   this.characters(chars1, off1, len1);
			 *   this.endCDATA();
			 *   this.startCDATA();
			 *   this.characters(chars2, off2, len2);
			 *   this.endCDATA();
			 * 
			 * that we will only make these calls to the wrapped handlers:
			 * 
			 *   m_lexHandler.startCDATA();
			 *   m_saxHandler.characters(chars1, off1, len1);
			 *   m_saxHandler.characters(chars1, off2, len2);
			 *   m_lexHandler.endCDATA();
			 * 
			 * We will merge adjacent CDATA blocks.
			 */ 
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endDTD() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public void endDTD()
		{
			if (m_lexHandler != null)
			{
				m_lexHandler.endDTD();
			}
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startEntity(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(String arg0) throws org.xml.sax.SAXException
		public void startEntity(string arg0)
		{
			if (m_lexHandler != null)
			{
				m_lexHandler.startEntity(arg0);
			}
		}

		/// <seealso cref= ExtendedContentHandler#characters(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
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

		public ToXMLSAXHandler(ContentHandler handler, string encoding) : base(handler, encoding)
		{

			initCDATA();
			// initNamespaces();
			m_prefixMap = new NamespaceMappings();
		}

		public ToXMLSAXHandler(ContentHandler handler, LexicalHandler lex, string encoding) : base(handler, lex, encoding)
		{

			initCDATA();
			//      initNamespaces();
			m_prefixMap = new NamespaceMappings();
		}

		/// <summary>
		/// Start an element in the output document. This might be an XML element
		/// (<elem>data</elem> type) or a CDATA section.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementNamespaceURI, String elementLocalName, String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementNamespaceURI, string elementLocalName, string elementName)
		{
			startElement(elementNamespaceURI,elementLocalName,elementName, null);


		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementName)
		{
			startElement(null, null, elementName, null);
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int off, int len) throws org.xml.sax.SAXException
		public void characters(char[] ch, int off, int len)
		{
			// We do the first two things in flushPending() but we don't
			// close any open CDATA calls.        
			if (m_needToCallStartDocument)
			{
				startDocumentInternal();
				m_needToCallStartDocument = false;
			}

			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}

			if (m_elemContext.m_isCdataSection && !m_cdataTagOpen && m_lexHandler != null)
			{
				m_lexHandler.startCDATA();
				// We have made a call to m_lexHandler.startCDATA() with
				// no balancing call to m_lexHandler.endCDATA()
				// so we set m_cdataTagOpen true to remember this.
				m_cdataTagOpen = true;
			}

			/* If there are any occurances of "]]>" in the character data
			 * let m_saxHandler worry about it, we've already warned them with
			 * the previous call of m_lexHandler.startCDATA();
			 */ 
			m_saxHandler.characters(ch, off, len);

			// time to generate characters event
			if (m_tracer != null)
			{
				fireCharEvent(ch, off, len);
			}
		}


		/// <seealso cref= ExtendedContentHandler#endElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String elemName) throws org.xml.sax.SAXException
		public override void endElement(string elemName)
		{
			endElement(null, null, elemName);
		}


		/// <summary>
		/// Send a namespace declaration in the output document. The namespace
		/// declaration will not be include if the namespace is already in scope
		/// with the same prefix.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(final String prefix, final String uri) throws org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public override void namespaceAfterStartElement(string prefix, string uri)
		{
			startPrefixMapping(prefix,uri,false);
		}

		/// 
		/// <seealso cref= org.xml.sax.ContentHandler#processingInstruction(String, String)
		/// Send a processing instruction to the output document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public override void processingInstruction(string target, string data)
		{
			flushPending();

			// Pass the processing instruction to the SAX handler
			m_saxHandler.processingInstruction(target, data);

			// we don't want to leave serializer to fire off this event,
			// so do it here.
			if (m_tracer != null)
			{
				base.fireEscapingEvent(target, data);
			}
		}

		/// <summary>
		/// Undeclare the namespace that is currently pointed to by a given
		/// prefix. Inform SAX handler if prefix was previously mapped.
		/// </summary>
		protected internal bool popNamespace(string prefix)
		{
			try
			{
				if (m_prefixMap.popNamespace(prefix))
				{
					m_saxHandler.endPrefixMapping(prefix);
					return true;
				}
			}
			catch (SAXException)
			{
				// falls through
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public void startCDATA()
		{
			/* m_cdataTagOpen can only be true here if we have ignored the
			 * previous call to this.endCDATA() and the previous call 
			 * this.startCDATA() before that is still "open". In this way
			 * we merge adjacent CDATA. If anything else happened after the 
			 * ignored call to this.endCDATA() and this call then a call to 
			 * flushPending() would have been made which would have
			 * closed the CDATA and set m_cdataTagOpen to false.
			 */
			if (!m_cdataTagOpen)
			{
				flushPending();
				if (m_lexHandler != null)
				{
					m_lexHandler.startCDATA();

					// We have made a call to m_lexHandler.startCDATA() with
					// no balancing call to m_lexHandler.endCDATA()
					// so we set m_cdataTagOpen true to remember this.                
					m_cdataTagOpen = true;
				}
			}
		}

		/// <seealso cref= org.xml.sax.ContentHandler#startElement(String, String, String, Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String name, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public override void startElement(string namespaceURI, string localName, string name, Attributes atts)
		{
			flushPending();
			base.startElement(namespaceURI, localName, name, atts);

			// Handle document type declaration (for first element only)
			 if (m_needToOutputDocTypeDecl)
			 {
				 string doctypeSystem = DoctypeSystem;
				 if (!string.ReferenceEquals(doctypeSystem, null) && m_lexHandler != null)
				 {
					 string doctypePublic = DoctypePublic;
					 if (!string.ReferenceEquals(doctypeSystem, null))
					 {
						 m_lexHandler.startDTD(name, doctypePublic, doctypeSystem);
					 }
				 }
				 m_needToOutputDocTypeDecl = false;
			 }
			m_elemContext = m_elemContext.push(namespaceURI, localName, name);

			// ensurePrefixIsDeclared depends on the current depth, so
			// the previous increment is necessary where it is.
			if (!string.ReferenceEquals(namespaceURI, null))
			{
				ensurePrefixIsDeclared(namespaceURI, name);
			}

			// add the attributes to the collected ones
			if (atts != null)
			{
				addAttributes(atts);
			}


			// do we really need this CDATA section state?
			m_elemContext.m_isCdataSection = CdataSection;

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void ensurePrefixIsDeclared(String ns, String rawName) throws org.xml.sax.SAXException
		private void ensurePrefixIsDeclared(string ns, string rawName)
		{

			if (!string.ReferenceEquals(ns, null) && ns.Length > 0)
			{
				int index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean no_prefix = ((index = rawName.indexOf(":")) < 0);
				bool no_prefix = ((index = rawName.IndexOf(":", StringComparison.Ordinal)) < 0);
				string prefix = (no_prefix) ? "" : rawName.Substring(0, index);


				if (null != prefix)
				{
					string foundURI = m_prefixMap.lookupNamespace(prefix);

					if ((null == foundURI) || !foundURI.Equals(ns))
					{
						this.startPrefixMapping(prefix, ns, false);

						if (ShouldOutputNSAttr)
						{
							// Bugzilla1133: Generate attribute as well as namespace event.
							// SAX does expect both.
							this.addAttributeAlways("http://www.w3.org/2000/xmlns/", no_prefix ? "xmlns" : prefix, no_prefix ? "xmlns" : ("xmlns:" + prefix), "CDATA", ns, false); // qname -  local name
						}
					}

				}
			}
		}
		/// <summary>
		/// Adds the given attribute to the set of attributes, and also makes sure
		/// that the needed prefix/uri mapping is declared, but only if there is a
		/// currently open element.
		/// </summary>
		/// <param name="uri"> the URI of the attribute </param>
		/// <param name="localName"> the local name of the attribute </param>
		/// <param name="rawName">    the qualified name of the attribute </param>
		/// <param name="type"> the type of the attribute (probably CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="XSLAttribute"> true if this attribute is coming from an xsl:attribute element </param>
		/// <seealso cref= ExtendedContentHandler#addAttribute(String, String, String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value, boolean XSLAttribute) throws org.xml.sax.SAXException
		public override void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			if (m_elemContext.m_startTagOpen)
			{
				ensurePrefixIsDeclared(uri, rawName);
				addAttributeAlways(uri, localName, rawName, type, value, false);
			}

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
				resetToXMLSAXHandler();
				wasReset = true;
			}
			return wasReset;
		}

		/// <summary>
		/// Reset all of the fields owned by ToXMLSAXHandler class
		/// 
		/// </summary>
		private void resetToXMLSAXHandler()
		{
			this.m_escapeSetting = true;
		}

	}

 }