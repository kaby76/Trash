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
 * $Id: ToSAXHandler.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// This class is used to provide a base behavior to be inherited
	/// by other To...SAXHandler serializers.
	/// 
	/// This class is not a public API.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public abstract class ToSAXHandler : SerializerBase
	{
		public ToSAXHandler()
		{
		}

		public ToSAXHandler(ContentHandler hdlr, LexicalHandler lex, string encoding)
		{
			ContentHandler = hdlr;
			LexHandler = lex;
			Encoding = encoding;
		}
		public ToSAXHandler(ContentHandler handler, string encoding)
		{
			ContentHandler = handler;
			Encoding = encoding;
		}

		/// <summary>
		/// Underlying SAX handler. Taken from XSLTC
		/// </summary>
		protected internal ContentHandler m_saxHandler;

		/// <summary>
		/// Underlying LexicalHandler. Taken from XSLTC
		/// </summary>
		protected internal LexicalHandler m_lexHandler;

		/// <summary>
		/// A startPrefixMapping() call on a ToSAXHandler will pass that call
		/// on to the wrapped ContentHandler, but should we also mirror these calls
		/// with matching attributes, if so this field is true.
		/// For example if this field is true then a call such as
		/// startPrefixMapping("prefix1","uri1") will also cause the additional
		/// internally generated attribute xmlns:prefix1="uri1" to be effectively added
		/// to the attributes passed to the wrapped ContentHandler.
		/// </summary>
		private bool m_shouldGenerateNSAttribute = true;

		/// <summary>
		/// If this is true, then the content handler wrapped by this
		/// serializer implements the TransformState interface which
		/// will give the content handler access to the state of
		/// the transform. 
		/// </summary>
		protected internal TransformStateSetter m_state = null;

		/// <summary>
		/// Pass callback to the SAX Handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void startDocumentInternal() throws org.xml.sax.SAXException
		protected internal override void startDocumentInternal()
		{
			if (m_needToCallStartDocument)
			{
				base.startDocumentInternal();

				m_saxHandler.startDocument();
				m_needToCallStartDocument = false;
			}
		}
		/// <summary>
		/// Do nothing. </summary>
		/// <seealso cref= org.xml.sax.ext.LexicalHandler#startDTD(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDTD(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void startDTD(string arg0, string arg1, string arg2)
		{
			// do nothing for now
		}

		/// <summary>
		/// Receive notification of character data.
		/// </summary>
		/// <param name="characters"> The string of characters to process.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException">
		/// </exception>
		/// <seealso cref= ExtendedContentHandler#characters(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String characters) throws org.xml.sax.SAXException
		public override void characters(string characters)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = characters.length();
			int len = characters.Length;
			if (len > m_charsBuff.Length)
			{
			   m_charsBuff = new char[len * 2 + 1];
			}
			characters.CopyTo(0, m_charsBuff, 0, len - 0);
			characters(m_charsBuff, 0, len);
		}

		/// <summary>
		/// Receive notification of a comment.
		/// </summary>
		/// <seealso cref= ExtendedLexicalHandler#comment(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(String comment) throws org.xml.sax.SAXException
		public override void comment(string comment)
		{
			flushPending();

			// Ignore if a lexical handler has not been set
			if (m_lexHandler != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = comment.length();
				int len = comment.Length;
				if (len > m_charsBuff.Length)
				{
				   m_charsBuff = new char[len * 2 + 1];
				}
				comment.CopyTo(0, m_charsBuff, 0, len - 0);
				m_lexHandler.comment(m_charsBuff, 0, len);
				// time to fire off comment event
				if (m_tracer != null)
				{
					base.fireCommentEvent(m_charsBuff, 0, len);
				}
			}

		}

		/// <summary>
		/// Do nothing as this is an abstract class. All subclasses will need to
		/// define their behavior if it is different. </summary>
		/// <seealso cref= org.xml.sax.ContentHandler#processingInstruction(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public virtual void processingInstruction(string target, string data)
		{
			// Redefined in SAXXMLOutput
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeStartTag() throws org.xml.sax.SAXException
		protected internal virtual void closeStartTag()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeCDATA() throws org.xml.sax.SAXException
		protected internal virtual void closeCDATA()
		{
			// Redefined in SAXXMLOutput
		}

		/// <summary>
		/// Receive notification of the beginning of an element, although this is a
		/// SAX method additional namespace or attribute information can occur before
		/// or after this call, that is associated with this element.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#startElement </seealso>
		/// <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
		/// <seealso cref= org.xml.sax.AttributeList
		/// </seealso>
		/// <exception cref="org.xml.sax.SAXException">
		/// </exception>
		/// <seealso cref= org.xml.sax.ContentHandler#startElement(String,String,String,Attributes) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String arg0, String arg1, String arg2, org.xml.sax.Attributes arg3) throws org.xml.sax.SAXException
		public virtual void startElement(string arg0, string arg1, string arg2, Attributes arg3)
		{
			if (m_state != null)
			{
				m_state.resetState(Transformer);
			}

			// fire off the start element event
			if (m_tracer != null)
			{
				base.fireStartElem(arg2);
			}
		}

		/// <summary>
		/// Sets the LexicalHandler. </summary>
		/// <param name="_lexHandler"> The LexicalHandler to set </param>
		public virtual LexicalHandler LexHandler
		{
			set
			{
				this.m_lexHandler = value;
			}
		}

		/// <summary>
		/// Sets the SAX ContentHandler. </summary>
		/// <param name="_saxHandler"> The ContentHandler to set </param>
		public override ContentHandler ContentHandler
		{
			set
			{
				this.m_saxHandler = value;
				if (m_lexHandler == null && value is LexicalHandler)
				{
					// we are not overwriting an existing LexicalHandler, and value
					// is also implements LexicalHandler, so lets use it
					m_lexHandler = (LexicalHandler) value;
				}
			}
		}

		/// <summary>
		/// Does nothing. The setting of CDATA section elements has an impact on
		/// stream serializers. </summary>
		/// <seealso cref= SerializationHandler#setCdataSectionElements(java.util.Vector) </seealso>
		public override ArrayList CdataSectionElements
		{
			set
			{
				// do nothing
			}
		}

		/// <summary>
		/// Set whether or not namespace declarations (e.g. 
		/// xmlns:foo) should appear as attributes of 
		/// elements </summary>
		/// <param name="doOutputNSAttr"> whether or not namespace declarations
		/// should appear as attributes </param>
		public virtual bool ShouldOutputNSAttr
		{
			set
			{
				m_shouldGenerateNSAttribute = value;
			}
			get
			{
				return m_shouldGenerateNSAttribute;
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

				if (m_elemContext.m_startTagOpen)
				{
					closeStartTag();
					m_elemContext.m_startTagOpen = false;
				}

				if (m_cdataTagOpen)
				{
					closeCDATA();
					m_cdataTagOpen = false;
				}

		}

		/// <summary>
		/// Pass in a reference to a TransformState object, which
		/// can be used during SAX ContentHandler events to obtain
		/// information about he state of the transformation. This
		/// method will be called  before each startDocument event.
		/// </summary>
		/// <param name="ts"> A reference to a TransformState object </param>
		public virtual TransformStateSetter TransformState
		{
			set
			{
				this.m_state = value;
			}
		}

		/// <summary>
		/// Receives notification that an element starts, but attributes are not
		/// fully known yet.
		/// </summary>
		/// <param name="uri"> the URI of the namespace of the element (optional) </param>
		/// <param name="localName"> the element name, but without prefix (optional) </param>
		/// <param name="qName"> the element name, with prefix, if any (required)
		/// </param>
		/// <seealso cref= ExtendedContentHandler#startElement(String, String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
		public override void startElement(string uri, string localName, string qName)
		{

			if (m_state != null)
			{
				m_state.resetState(Transformer);
			}

			// fire off the start element event
			if (m_tracer != null)
			{
				base.fireStartElem(qName);
			}
		}

		/// <summary>
		/// An element starts, but attributes are not fully known yet.
		/// </summary>
		/// <param name="qName"> the element name, with prefix (if any).
		/// </param>
		/// <seealso cref= ExtendedContentHandler#startElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String qName) throws org.xml.sax.SAXException
		public override void startElement(string qName)
		{
			if (m_state != null)
			{
				m_state.resetState(Transformer);
			}
			// fire off the start element event
			if (m_tracer != null)
			{
				base.fireStartElem(qName);
			}
		}

		/// <summary>
		/// This method gets the node's value as a String and uses that String as if
		/// it were an input character notification. </summary>
		/// <param name="node"> the Node to serialize </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		public override void characters(org.w3c.dom.Node node)
		{
			// remember the current node
			if (m_state != null)
			{
				m_state.CurrentNode = node;
			}

			// Get the node's value as a String and use that String as if
			// it were an input character notification.
			string data = node.NodeValue;
			if (!string.ReferenceEquals(data, null))
			{
				this.characters(data);
			}
		}

		/// <seealso cref= org.xml.sax.ErrorHandler#fatalError(SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public override void fatalError(SAXParseException exc)
		{
			base.fatalError(exc);

			m_needToCallStartDocument = false;

			if (m_saxHandler is ErrorHandler)
			{
				((ErrorHandler)m_saxHandler).fatalError(exc);
			}
		}

		/// <seealso cref= org.xml.sax.ErrorHandler#error(SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public override void error(SAXParseException exc)
		{
			base.error(exc);

			if (m_saxHandler is ErrorHandler)
			{
				((ErrorHandler)m_saxHandler).error(exc);
			}

		}

		/// <seealso cref= org.xml.sax.ErrorHandler#warning(SAXParseException) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public override void warning(SAXParseException exc)
		{
			base.warning(exc);

			if (m_saxHandler is ErrorHandler)
			{
				((ErrorHandler)m_saxHandler).warning(exc);
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
				resetToSAXHandler();
				wasReset = true;
			}
			return wasReset;
		}

		/// <summary>
		/// Reset all of the fields owned by ToSAXHandler class
		/// 
		/// </summary>
		private void resetToSAXHandler()
		{
			this.m_lexHandler = null;
			this.m_saxHandler = null;
			this.m_state = null;
			this.m_shouldGenerateNSAttribute = false;
		}

		/// <summary>
		/// Add a unique attribute
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addUniqueAttribute(String qName, String value, int flags) throws org.xml.sax.SAXException
		public override void addUniqueAttribute(string qName, string value, int flags)
		{
			addAttribute(qName, value);
		}
	}

}