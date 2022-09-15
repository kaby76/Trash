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
 * $Id: ProcessorGlobalVariableDecl.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemVariable = org.apache.xalan.templates.ElemVariable;

	/// <summary>
	/// This class processes parse events for an xsl:variable element. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"/>
	[Serializable]
	internal class ProcessorGlobalVariableDecl : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = -5954332402269819582L;

	  /// <summary>
	  /// Append the current template element to the current
	  /// template element, and then push it onto the current template
	  /// element stack.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="elem"> The non-null reference to the ElemVariable element.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void appendAndPush(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  protected internal override void appendAndPush(StylesheetHandler handler, ElemTemplateElement elem)
	  {

		// Just push, but don't append.
		handler.pushElemTemplateElement(elem);
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// </summary>
	  /// <param name="name"> The element type name. </param>
	  /// <param name="attributes"> The specified or defaulted attributes.
	  /// </param>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(StylesheetHandler handler, String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public override void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {

		ElemVariable v = (ElemVariable) handler.ElemTemplateElement;

		handler.Stylesheet.appendChild(v);
		handler.Stylesheet.Variable = v;
		base.endElement(handler, uri, localName, rawName);
	  }
	}

}