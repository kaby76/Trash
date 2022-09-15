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
 * $Id: ProcessorAttributeSet.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using ElemAttributeSet = org.apache.xalan.templates.ElemAttributeSet;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// This class processes parse events for an xsl:attribute-set. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"/>
	[Serializable]
	internal class ProcessorAttributeSet : XSLTElementProcessor
	{
		internal new const long serialVersionUID = -6473739251316787552L;

	  /// <summary>
	  /// Receive notification of the start of an xsl:attribute-set element.
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
	  /// <param name="attributes"> The attributes attached to the element.  If
	  ///        there are no attributes, it shall be an empty
	  ///        Attributes object.
	  /// </param>
	  /// <seealso cref="org.apache.xalan.processor.StylesheetHandler.startElement"/>
	  /// <seealso cref="org.xml.sax.ContentHandler.startElement"/>
	  /// <seealso cref="org.xml.sax.ContentHandler.endElement"/>
	  /// <seealso cref="org.xml.sax.Attributes"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {

		ElemAttributeSet eat = new ElemAttributeSet();

		eat.LocaterInfo = handler.Locator;
		try
		{
		  eat.Prefixes = handler.NamespaceSupport;
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}

		eat.DOMBackPointer = handler.OriginatingNode;
		setPropertiesFromAttributes(handler, rawName, attributes, eat);
		handler.Stylesheet.AttributeSet = eat;

		// handler.pushElemTemplateElement(eat);
		ElemTemplateElement parent = handler.ElemTemplateElement;

		parent.appendChild(eat);
		handler.pushElemTemplateElement(eat);
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
		handler.popElemTemplateElement();
	  }
	}

}