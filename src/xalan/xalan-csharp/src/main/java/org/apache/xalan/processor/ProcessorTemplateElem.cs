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
 * $Id: ProcessorTemplateElem.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// This class processes parse events for an XSLT template element. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-the-Result-Tree">section-Creating-the-Result-Tree in XSLT Specification</a> </seealso>
	[Serializable]
	public class ProcessorTemplateElem : XSLTElementProcessor
	{
		internal new const long serialVersionUID = 8344994001943407235L;

	  /// <summary>
	  /// Receive notification of the start of an element.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {

		base.startElement(handler, uri, localName, rawName, attributes);
		try
		{
		  // ElemTemplateElement parent = handler.getElemTemplateElement();
		  XSLTElementDef def = ElemDef;
		  Type classObject = def.ClassObject;
		  ElemTemplateElement elem = null;

		  try
		  {
			elem = (ElemTemplateElement) classObject.newInstance();

			elem.DOMBackPointer = handler.OriginatingNode;
			elem.LocaterInfo = handler.Locator;
			elem.Prefixes = handler.NamespaceSupport;
		  }
		  catch (InstantiationException ie)
		  {
			handler.error(XSLTErrorResources.ER_FAILED_CREATING_ELEMTMPL, null, ie); //"Failed creating ElemTemplateElement instance!", ie);
		  }
		  catch (IllegalAccessException iae)
		  {
			handler.error(XSLTErrorResources.ER_FAILED_CREATING_ELEMTMPL, null, iae); //"Failed creating ElemTemplateElement instance!", iae);
		  }

		  setPropertiesFromAttributes(handler, rawName, attributes, elem);
		  appendAndPush(handler, elem);
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Append the current template element to the current
	  /// template element, and then push it onto the current template
	  /// element stack.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="elem"> non-null reference to a the current template element.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void appendAndPush(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  protected internal virtual void appendAndPush(StylesheetHandler handler, ElemTemplateElement elem)
	  {

		ElemTemplateElement parent = handler.ElemTemplateElement;
		if (null != parent) // defensive, for better multiple error reporting. -sb
		{
		  parent.appendChild(elem);
		  handler.pushElemTemplateElement(elem);
		}
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
		base.endElement(handler, uri, localName, rawName);
		handler.popElemTemplateElement().EndLocaterInfo = handler.Locator;
	  }
	}

}