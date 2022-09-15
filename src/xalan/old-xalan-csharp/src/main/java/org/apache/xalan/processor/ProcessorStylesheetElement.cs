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
 * $Id: ProcessorStylesheetElement.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{


	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using StylesheetComposed = org.apache.xalan.templates.StylesheetComposed;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// TransformerFactory for xsl:stylesheet or xsl:transform markup. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#stylesheet-element">stylesheet-element in XSLT Specification</a>
	/// 
	/// @xsl.usage internal </seealso>
	[Serializable]
	public class ProcessorStylesheetElement : XSLTElementProcessor
	{
		internal new const long serialVersionUID = -877798927447840792L;

	  /// <summary>
	  /// Receive notification of the start of an strip-space element.
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
	  ///        Attributes object. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {

			base.startElement(handler, uri, localName, rawName, attributes);
		try
		{
		  int stylesheetType = handler.StylesheetType;
		  Stylesheet stylesheet;

		  if (stylesheetType == StylesheetHandler.STYPE_ROOT)
		  {
			try
			{
			  stylesheet = getStylesheetRoot(handler);
			}
			catch (TransformerConfigurationException tfe)
			{
			  throw new TransformerException(tfe);
			}
		  }
		  else
		  {
			Stylesheet parent = handler.Stylesheet;

			if (stylesheetType == StylesheetHandler.STYPE_IMPORT)
			{
			  StylesheetComposed sc = new StylesheetComposed(parent);

			  parent.Import = sc;

			  stylesheet = sc;
			}
			else
			{
			  stylesheet = new Stylesheet(parent);

			  parent.Include = stylesheet;
			}
		  }

		  stylesheet.DOMBackPointer = handler.OriginatingNode;
		  stylesheet.LocaterInfo = handler.Locator;

		  stylesheet.Prefixes = handler.NamespaceSupport;
		  handler.pushStylesheet(stylesheet);
		  setPropertiesFromAttributes(handler, rawName, attributes, handler.Stylesheet);
		  handler.pushElemTemplateElement(handler.Stylesheet);
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// This method can be over-ridden by a class that extends this one. </summary>
	  /// <param name="handler"> The calling StylesheetHandler/TemplatesBuilder. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.xalan.templates.Stylesheet getStylesheetRoot(StylesheetHandler handler) throws javax.xml.transform.TransformerConfigurationException
	  protected internal virtual Stylesheet getStylesheetRoot(StylesheetHandler handler)
	  {
		StylesheetRoot stylesheet;
		stylesheet = new StylesheetRoot(handler.Schema, handler.StylesheetProcessor.ErrorListener);

		if (handler.StylesheetProcessor.SecureProcessing)
		{
		  stylesheet.SecureProcessing = true;
		}

		return stylesheet;
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
		handler.popElemTemplateElement();
		handler.popStylesheet();
	  }
	}

}