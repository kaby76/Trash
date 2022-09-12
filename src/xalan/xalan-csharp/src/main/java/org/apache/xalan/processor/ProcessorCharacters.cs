using System;
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
 * $Id: ProcessorCharacters.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemText = org.apache.xalan.templates.ElemText;
	using ElemTextLiteral = org.apache.xalan.templates.ElemTextLiteral;
	using XMLCharacterRecognizer = org.apache.xml.utils.XMLCharacterRecognizer;

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This class processes character events for a XSLT template element. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-the-Result-Tree">section-Creating-the-Result-Tree in XSLT Specification</a> </seealso>
	[Serializable]
	public class ProcessorCharacters : XSLTElementProcessor
	{
		internal new const long serialVersionUID = 8632900007814162650L;

	  /// <summary>
	  /// Receive notification of the start of the non-text event.  This
	  /// is sent to the current processor when any non-text event occurs.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startNonText(StylesheetHandler handler) throws org.xml.sax.SAXException
	  public override void startNonText(StylesheetHandler handler)
	  {
		if (this == handler.CurrentProcessor)
		{
		  handler.popProcessor();
		}

		int nChars = m_accumulator.Length;

		if ((nChars > 0) && ((null != m_xslTextElement) || !XMLCharacterRecognizer.isWhiteSpace(m_accumulator)) || handler.SpacePreserve)
		{
		  ElemTextLiteral elem = new ElemTextLiteral();

		  elem.DOMBackPointer = m_firstBackPointer;
		  elem.LocaterInfo = handler.Locator;
		  try
		  {
			elem.Prefixes = handler.NamespaceSupport;
		  }
		  catch (TransformerException te)
		  {
			throw new org.xml.sax.SAXException(te);
		  }

		  bool doe = (null != m_xslTextElement) ? m_xslTextElement.DisableOutputEscaping : false;

		  elem.DisableOutputEscaping = doe;
		  elem.PreserveSpace = true;

		  char[] chars = new char[nChars];

		  m_accumulator.getChars(0, nChars, chars, 0);
		  elem.Chars = chars;

		  ElemTemplateElement parent = handler.ElemTemplateElement;

		  parent.appendChild(elem);
		}

		m_accumulator.Length = 0;
		m_firstBackPointer = null;
	  }

	  protected internal Node m_firstBackPointer = null;

	  /// <summary>
	  /// Receive notification of character data inside an element.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="ch"> The characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#characters </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(StylesheetHandler handler, char ch[], int start, int length) throws org.xml.sax.SAXException
	  public override void characters(StylesheetHandler handler, char[] ch, int start, int length)
	  {

		m_accumulator.Append(ch, start, length);

		if (null == m_firstBackPointer)
		{
		  m_firstBackPointer = handler.OriginatingNode;
		}

		// Catch all events until a non-character event.
		if (this != handler.CurrentProcessor)
		{
		  handler.pushProcessor(this);
		}
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// </summary>
	  /// <param name="handler"> The calling StylesheetHandler/TemplatesBuilder. </param>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="rawName"> The raw XML 1.0 name (with prefix), or the
	  ///        empty string if raw names are not available. </param>
	  /// <seealso cref= org.apache.xalan.processor.StylesheetHandler#startElement </seealso>
	  /// <seealso cref= org.apache.xalan.processor.StylesheetHandler#endElement </seealso>
	  /// <seealso cref= org.xml.sax.ContentHandler#startElement </seealso>
	  /// <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
	  /// <seealso cref= org.xml.sax.Attributes </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(StylesheetHandler handler, String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public override void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {

		// Since this has been installed as the current processor, we 
		// may get and end element event, in which case, we pop and clear 
		// and then call the real element processor.
		startNonText(handler);
		handler.CurrentProcessor.endElement(handler, uri, localName, rawName);
		handler.popProcessor();
	  }

	  /// <summary>
	  /// Accumulate characters, until a non-whitespace event has
	  /// occured.
	  /// </summary>
	  private StringBuilder m_accumulator = new StringBuilder();

	  /// <summary>
	  /// The xsl:text processor will call this to set a
	  /// preserve space state.
	  /// </summary>
	  private ElemText m_xslTextElement;

	  /// <summary>
	  /// Set the current setXslTextElement. The xsl:text 
	  /// processor will call this to set a preserve space state.
	  /// </summary>
	  /// <param name="xslTextElement"> The current xslTextElement that 
	  ///                       is preserving state, or null. </param>
	  internal virtual ElemText XslTextElement
	  {
		  set
		  {
			m_xslTextElement = value;
		  }
	  }
	}

}