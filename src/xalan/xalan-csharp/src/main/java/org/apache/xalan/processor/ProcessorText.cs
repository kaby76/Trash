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
 * $Id: ProcessorText.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemText = org.apache.xalan.templates.ElemText;

	/// <summary>
	/// Process xsl:text. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#element-text">element-text in XSLT Specification</a> </seealso>
	[Serializable]
	public class ProcessorText : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = 5170229307201307523L;

	  /// <summary>
	  /// Append the current template element to the current
	  /// template element, and then push it onto the current template
	  /// element stack.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="elem"> non-null reference to a <seealso cref="org.apache.xalan.templates.ElemText"/>.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void appendAndPush(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  protected internal override void appendAndPush(StylesheetHandler handler, ElemTemplateElement elem)
	  {

		// Don't push this element onto the element stack.
		ProcessorCharacters charProcessor = (ProcessorCharacters) handler.getProcessorFor(null, "text()", "text");

		charProcessor.XslTextElement = (ElemText) elem;

		ElemTemplateElement parent = handler.ElemTemplateElement;

		parent.appendChild(elem);
		elem.DOMBackPointer = handler.OriginatingNode;
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(StylesheetHandler handler, String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public override void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {

		ProcessorCharacters charProcessor = (ProcessorCharacters) handler.getProcessorFor(null, "text()", "text");

		charProcessor.XslTextElement = null;

	  }
	}

}