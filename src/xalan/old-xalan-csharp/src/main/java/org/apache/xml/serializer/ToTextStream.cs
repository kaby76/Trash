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
 * $Id: ToTextStream.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This class is not a public API.
	/// It is only public because it is used in other packages. 
	/// This class converts SAX or SAX-like calls to a 
	/// serialized document for xsl:output method of "text".
	/// @xsl.usage internal
	/// </summary>
	public class ToTextStream : ToStream
	{


	  /// <summary>
	  /// Default constructor.
	  /// </summary>
	  public ToTextStream() : base()
	  {
	  }



	  /// <summary>
	  /// Receive notification of the beginning of a document.
	  /// 
	  /// <para>The SAX parser will invoke this method only once, before any
	  /// other methods in this interface or in DTDHandler (except for
	  /// setDocumentLocator).</para>
	  /// </summary>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception.
	  /// </exception>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void startDocumentInternal() throws org.xml.sax.SAXException
	  protected internal override void startDocumentInternal()
	  {
		base.startDocumentInternal();

		m_needToCallStartDocument = false;

		// No action for the moment.
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
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {
		flushPending();
		flushWriter();
		if (m_tracer != null)
		{
			base.fireEndDoc();
		}
	  }

	  /// <summary>
	  /// Receive notification of the beginning of an element.
	  /// 
	  /// <para>The Parser will invoke this method at the beginning of every
	  /// element in the XML document; there will be a corresponding
	  /// endElement() event for every startElement() event (even when the
	  /// element is empty). All of the element's content will be
	  /// reported, in order, before the corresponding endElement()
	  /// event.</para>
	  /// 
	  /// <para>If the element name has a namespace prefix, the prefix will
	  /// still be attached.  Note that the attribute list provided will
	  /// contain only attributes with explicit values (specified or
	  /// defaulted): #IMPLIED attributes will be omitted.</para>
	  /// 
	  /// </summary>
	  /// <param name="namespaceURI"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="name"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <param name="atts"> The attributes attached to the element, if any. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= #endElement </seealso>
	  /// <seealso cref= org.xml.sax.AttributeList
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String name, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
	  public override void startElement(string namespaceURI, string localName, string name, Attributes atts)
	  {
		// time to fire off startElement event
		if (m_tracer != null)
		{
			base.fireStartElem(name);
			this.firePseudoAttributes();
		}
		return;
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
	  /// <param name="namespaceURI"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="name"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception.
	  /// </exception>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String name) throws org.xml.sax.SAXException
	  public override void endElement(string namespaceURI, string localName, string name)
	  {
			if (m_tracer != null)
			{
				base.fireEndElem(name);
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
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= #ignorableWhitespace </seealso>
	  /// <seealso cref= org.xml.sax.Locator </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void characters(char[] ch, int start, int length)
	  {

		flushPending();

		try
		{
			if (inTemporaryOutputState())
			{
				/* leave characters un-processed as we are
				 * creating temporary output, the output generated by
				 * this serializer will be input to a final serializer 
				 * later on and it will do the processing in final
				 * output state (not temporary output state).
				 * 
				 * A "temporary" ToTextStream serializer is used to
				 * evaluate attribute value templates (for example),
				 * and the result of evaluating such a thing
				 * is fed into a final serializer later on.
				 */
				m_writer.write(ch, start, length);
			}
			else
			{
				// In final output state we do process the characters!
				writeNormalizedChars(ch, start, length, m_lineSepUse);
			}

			if (m_tracer != null)
			{
				base.fireCharEvent(ch, start, length);
			}
		}
		catch (IOException ioe)
		{
		  throw new SAXException(ioe);
		}
	  }

	  /// <summary>
	  /// If available, when the disable-output-escaping attribute is used,
	  /// output raw text without escaping.
	  /// </summary>
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void charactersRaw(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void charactersRaw(char[] ch, int start, int length)
	  {

		try
		{
		  writeNormalizedChars(ch, start, length, m_lineSepUse);
		}
		catch (IOException ioe)
		{
		  throw new SAXException(ioe);
		}
	  }

		/// <summary>
		/// Normalize the characters, but don't escape.  Different from 
		/// SerializerToXML#writeNormalizedChars because it does not attempt to do 
		/// XML escaping at all.
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <param name="useLineSep"> true if the operating systems 
		/// end-of-line separator should be output rather than a new-line character.
		/// </param>
		/// <exception cref="IOException"> </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeNormalizedChars(final char ch[], final int start, final int length, final boolean useLineSep) throws java.io.IOException, org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		internal virtual void writeNormalizedChars(char[] ch, int start, int length, bool useLineSep)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String encoding = getEncoding();
			string encoding = Encoding;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			java.io.Writer writer = m_writer;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = start + length;
			int end = start + length;

			/* copy a few "constants" before the loop for performance */
			const char S_LINEFEED = CharInfo.S_LINEFEED;

			// This for() loop always increments i by one at the end
			// of the loop.  Additional increments of i adjust for when
			// two input characters (a high/low UTF16 surrogate pair)
			// are processed.
			for (int i = start; i < end; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char c = ch[i];
				char c = ch[i];

				if (S_LINEFEED == c && useLineSep)
				{
					writer.write(m_lineSep, 0, m_lineSepLen);
					// one input char processed
				}
				else if (m_encodingInfo.isInEncoding(c))
				{
					writer.write(c);
					// one input char processed    
				}
				else if (Encodings.isHighUTF16Surrogate(c))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int codePoint = writeUTF16Surrogate(c, ch, i, end);
					int codePoint = writeUTF16Surrogate(c, ch, i, end);
					if (codePoint != 0)
					{
						// I think we can just emit the message,
						// not crash and burn.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String integralValue = Convert.ToString(codePoint);
						string integralValue = Convert.ToString(codePoint);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String msg = org.apache.xml.serializer.utils.Utils.messages.createMessage(org.apache.xml.serializer.utils.MsgKey.ER_ILLEGAL_CHARACTER, new Object[] { integralValue, encoding });
						string msg = Utils.messages.createMessage(MsgKey.ER_ILLEGAL_CHARACTER, new object[] {integralValue, encoding});

						//Older behavior was to throw the message,
						//but newer gentler behavior is to write a message to System.err
						//throw new SAXException(msg);
						Console.Error.WriteLine(msg);

					}
					i++; // two input chars processed
				}
				else
				{
					// Don't know what to do with this char, it is
					// not in the encoding and not a high char in
					// a surrogate pair, so write out as an entity ref
					if (!string.ReferenceEquals(encoding, null))
					{
						/* The output encoding is known, 
						 * so somthing is wrong.
						 */

						// not in the encoding, so write out a character reference
						writer.write('&');
						writer.write('#');
						writer.write(Convert.ToString(c));
						writer.write(';');

						// I think we can just emit the message,
						// not crash and burn.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String integralValue = Convert.ToString(c);
						string integralValue = Convert.ToString(c);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String msg = org.apache.xml.serializer.utils.Utils.messages.createMessage(org.apache.xml.serializer.utils.MsgKey.ER_ILLEGAL_CHARACTER, new Object[] { integralValue, encoding });
						string msg = Utils.messages.createMessage(MsgKey.ER_ILLEGAL_CHARACTER, new object[] {integralValue, encoding});

						//Older behavior was to throw the message,
						//but newer gentler behavior is to write a message to System.err
						//throw new SAXException(msg);
						Console.Error.WriteLine(msg);
					}
					else
					{
						/* The output encoding is not known,
						 * so just write it out as-is.
						 */
						writer.write(c);
					}

					// one input char was processed
				}
			}
		}

	  /// <summary>
	  /// Receive notification of cdata.
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
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= #ignorableWhitespace </seealso>
	  /// <seealso cref= org.xml.sax.Locator </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void cdata(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void cdata(char[] ch, int start, int length)
	  {
		try
		{
			writeNormalizedChars(ch, start, length, m_lineSepUse);
			if (m_tracer != null)
			{
				base.fireCDATAEvent(ch, start, length);
			}
		}
		catch (IOException ioe)
		{
		  throw new SAXException(ioe);
		}
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// 
	  /// <para>Validating Parsers must use this method to report each chunk
	  /// of ignorable whitespace (see the W3C XML 1.0 recommendation,
	  /// section 2.10): non-validating parsers may also use this method
	  /// if they are capable of parsing and using content models.</para>
	  /// 
	  /// <para>SAX parsers may return all contiguous whitespace in a single
	  /// chunk, or they may split it into several chunks; however, all of
	  /// the characters in any single event must come from the same
	  /// external entity, so that the Locator provides useful
	  /// information.</para>
	  /// 
	  /// <para>The application must not attempt to read from the array
	  /// outside of the specified range.</para>
	  /// </summary>
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= #characters
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void ignorableWhitespace(char[] ch, int start, int length)
	  {

		try
		{
		  writeNormalizedChars(ch, start, length, m_lineSepUse);
		}
		catch (IOException ioe)
		{
		  throw new SAXException(ioe);
		}
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		// flush anything pending first
		flushPending();

		if (m_tracer != null)
		{
			base.fireEscapingEvent(target, data);
		}
	  }

	  /// <summary>
	  /// Called when a Comment is to be constructed.
	  /// Note that Xalan will normally invoke the other version of this method.
	  /// %REVIEW% In fact, is this one ever needed, or was it a mistake?
	  /// </summary>
	  /// <param name="data">  The comment data. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
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

	  /// <summary>
	  /// Report an XML comment anywhere in the document.
	  /// 
	  /// This callback will be used for comments inside or outside the
	  /// document element, including comments in the external DTD
	  /// subset (if read).
	  /// </summary>
	  /// <param name="ch"> An array holding the characters in the comment. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void comment(char[] ch, int start, int length)
	  {

		flushPending();
		if (m_tracer != null)
		{
			base.fireCommentEvent(ch, start, length);
		}
	  }

	  /// <summary>
	  /// Receive notivication of a entityReference.
	  /// </summary>
	  /// <param name="name"> non-null reference to the name of the entity.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void entityReference(String name) throws org.xml.sax.SAXException
	  public override void entityReference(string name)
	  {
			if (m_tracer != null)
			{
				base.fireEntityReference(name);
			}
	  }

		/// <seealso cref= ExtendedContentHandler#addAttribute(String, String, String, String, String) </seealso>
		public override void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			// do nothing, just forget all about the attribute
		}

		/// <seealso cref= org.xml.sax.ext.LexicalHandler#endCDATA() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public override void endCDATA()
		{
			// do nothing
		}

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

		/// <summary>
		/// From XSLTC
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String elementNamespaceURI, String elementLocalName, String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementNamespaceURI, string elementLocalName, string elementName)
		{
			if (m_needToCallStartDocument)
			{
				startDocumentInternal();
			}
			// time to fire off startlement event.
			if (m_tracer != null)
			{
				base.fireStartElem(elementName);
				this.firePseudoAttributes();
			}

			return;
		}


		/// <summary>
		/// From XSLTC
		/// </summary>
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
			characters(m_charsBuff, 0, length);
		}


		/// <summary>
		/// From XSLTC
		/// </summary>
		public override void addAttribute(string name, string value)
		{
			// do nothing, forget about the attribute
		}

		/// <summary>
		/// Add a unique attribute
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addUniqueAttribute(String qName, String value, int flags) throws org.xml.sax.SAXException
		public override void addUniqueAttribute(string qName, string value, int flags)
		{
			// do nothing, forget about the attribute 
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
		public override void startPrefixMapping(string prefix, string uri)
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException
		public override void flushPending()
		{
				if (m_needToCallStartDocument)
				{
					startDocumentInternal();
					m_needToCallStartDocument = false;
				}
		}
	}

}