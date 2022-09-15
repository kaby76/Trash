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
 * $Id: ProcessorLRE.java 475981 2006-11-16 23:35:53Z minchau $
 */
namespace org.apache.xalan.processor
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemExtensionCall = org.apache.xalan.templates.ElemExtensionCall;
	using ElemLiteralResult = org.apache.xalan.templates.ElemLiteralResult;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using XMLNSDecl = org.apache.xalan.templates.XMLNSDecl;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using XPath = org.apache.xpath.XPath;

	using Attributes = org.xml.sax.Attributes;
	using Locator = org.xml.sax.Locator;
	using AttributesImpl = org.xml.sax.helpers.AttributesImpl;

	/// <summary>
	/// Processes an XSLT literal-result-element, or something that looks 
	/// like one.  The actual <seealso cref="org.apache.xalan.templates.ElemTemplateElement"/>
	/// produced may be a <seealso cref="org.apache.xalan.templates.ElemLiteralResult"/>, 
	/// a <seealso cref="org.apache.xalan.templates.StylesheetRoot"/>, or a 
	/// <seealso cref="org.apache.xalan.templates.ElemExtensionCall"/>.
	/// </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"/>
	/// <seealso cref="org.apache.xalan.templates.ElemLiteralResult"
	/// @xsl.usage internal/>
	[Serializable]
	public class ProcessorLRE : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = -1490218021772101404L;
	  /// <summary>
	  /// Receive notification of the start of an element.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {

		try
		{
		  ElemTemplateElement p = handler.ElemTemplateElement;
		  bool excludeXSLDecl = false;
		  bool isLREAsStyleSheet = false;

		  if (null == p)
		  {

			// Literal Result Template as stylesheet.
			XSLTElementProcessor lreProcessor = handler.popProcessor();
			XSLTElementProcessor stylesheetProcessor = handler.getProcessorFor(Constants.S_XSLNAMESPACEURL, "stylesheet", "xsl:stylesheet");

			handler.pushProcessor(lreProcessor);

			Stylesheet stylesheet;
			try
			{
			  stylesheet = getStylesheetRoot(handler);
			}
			catch (TransformerConfigurationException tfe)
			{
			  throw new TransformerException(tfe);
			}

			// stylesheet.setDOMBackPointer(handler.getOriginatingNode());
			// ***** Note that we're assigning an empty locator. Is this necessary?
			SAXSourceLocator slocator = new SAXSourceLocator();
			Locator locator = handler.Locator;
			if (null != locator)
			{
			  slocator.setLineNumber(locator.getLineNumber());
			  slocator.setColumnNumber(locator.getColumnNumber());
			  slocator.setPublicId(locator.getPublicId());
			  slocator.setSystemId(locator.getSystemId());
			}
			stylesheet.LocaterInfo = slocator;
			stylesheet.Prefixes = handler.NamespaceSupport;
			handler.pushStylesheet(stylesheet);

			isLREAsStyleSheet = true;

			AttributesImpl stylesheetAttrs = new AttributesImpl();
			AttributesImpl lreAttrs = new AttributesImpl();
			int n = attributes.getLength();

			for (int i = 0; i < n; i++)
			{
			  string attrLocalName = attributes.getLocalName(i);
			  string attrUri = attributes.getURI(i);
			  string value = attributes.getValue(i);

			  if ((null != attrUri) && attrUri.Equals(Constants.S_XSLNAMESPACEURL))
			  {
				stylesheetAttrs.addAttribute(null, attrLocalName, attrLocalName, attributes.getType(i), attributes.getValue(i));
			  }
			  else if ((attrLocalName.StartsWith("xmlns:", StringComparison.Ordinal) || attrLocalName.Equals("xmlns")) && value.Equals(Constants.S_XSLNAMESPACEURL))
			  {

				// ignore
			  }
			  else
			  {
				lreAttrs.addAttribute(attrUri, attrLocalName, attributes.getQName(i), attributes.getType(i), attributes.getValue(i));
			  }
			}

			attributes = lreAttrs;

			// Set properties from the attributes, but don't throw 
			// an error if there is an attribute defined that is not 
			// allowed on a stylesheet.
					try
					{
			stylesheetProcessor.setPropertiesFromAttributes(handler, "stylesheet", stylesheetAttrs, stylesheet);
					}
					catch (Exception e)
					{
						// This is pretty ugly, but it will have to do for now. 
						// This is just trying to append some text specifying that
						// this error came from a missing or invalid XSLT namespace
						// declaration.
						// If someone comes up with a better solution, please feel 
						// free to contribute it. -mm

						if (stylesheet.DeclaredPrefixes == null || !declaredXSLNS(stylesheet))
						{
							throw new org.xml.sax.SAXException(XSLMessages.createWarning(XSLTErrorResources.WG_OLD_XSLT_NS, null));
						}
						else
						{
							throw new org.xml.sax.SAXException(e);
						}
					}
			handler.pushElemTemplateElement(stylesheet);

			ElemTemplate template = new ElemTemplate();
			if (slocator != null)
			{
				template.LocaterInfo = slocator;
			}

			appendAndPush(handler, template);

			XPath rootMatch = new XPath("/", stylesheet, stylesheet, XPath.MATCH, handler.StylesheetProcessor.ErrorListener);

			template.Match = rootMatch;

			// template.setDOMBackPointer(handler.getOriginatingNode());
			stylesheet.Template = template;

			p = handler.ElemTemplateElement;
			excludeXSLDecl = true;
		  }

		  XSLTElementDef def = ElemDef;
		  Type classObject = def.ClassObject;
		  bool isExtension = false;
		  bool isComponentDecl = false;
		  bool isUnknownTopLevel = false;

		  while (null != p)
		  {

			// System.out.println("Checking: "+p);
			if (p is ElemLiteralResult)
			{
			  ElemLiteralResult parentElem = (ElemLiteralResult) p;

			  isExtension = parentElem.containsExtensionElementURI(uri);
			}
			else if (p is Stylesheet)
			{
			  Stylesheet parentElem = (Stylesheet) p;

			  isExtension = parentElem.containsExtensionElementURI(uri);

			  if ((false == isExtension) && (null != uri) && (uri.Equals(Constants.S_BUILTIN_EXTENSIONS_URL) || uri.Equals(Constants.S_BUILTIN_OLD_EXTENSIONS_URL)))
			  {
				isComponentDecl = true;
			  }
			  else
			  {
				isUnknownTopLevel = true;
			  }
			}

			if (isExtension)
			{
			  break;
			}

			p = p.ParentElem;
		  }

		  ElemTemplateElement elem = null;

		  try
		  {
			if (isExtension)
			{

			  // System.out.println("Creating extension(1): "+uri);
			  elem = new ElemExtensionCall();
			}
			else if (isComponentDecl)
			{
			  elem = (ElemTemplateElement) System.Activator.CreateInstance(classObject);
			}
			else if (isUnknownTopLevel)
			{

			  // TBD: Investigate, not sure about this.  -sb
			  elem = (ElemTemplateElement) System.Activator.CreateInstance(classObject);
			}
			else
			{
			  elem = (ElemTemplateElement) System.Activator.CreateInstance(classObject);
			}

			elem.DOMBackPointer = handler.OriginatingNode;
			elem.LocaterInfo = handler.Locator;
			elem.setPrefixes(handler.NamespaceSupport, excludeXSLDecl);

			if (elem is ElemLiteralResult)
			{
			  ((ElemLiteralResult) elem).Namespace = uri;
			  ((ElemLiteralResult) elem).LocalName = localName;
			  ((ElemLiteralResult) elem).RawName = rawName;
			  ((ElemLiteralResult) elem).IsLiteralResultAsStylesheet = isLREAsStyleSheet;
			}
		  }
		  catch (InstantiationException ie)
		  {
			handler.error(XSLTErrorResources.ER_FAILED_CREATING_ELEMLITRSLT, null, ie); //"Failed creating ElemLiteralResult instance!", ie);
		  }
		  catch (IllegalAccessException iae)
		  {
			handler.error(XSLTErrorResources.ER_FAILED_CREATING_ELEMLITRSLT, null, iae); //"Failed creating ElemLiteralResult instance!", iae);
		  }

		  setPropertiesFromAttributes(handler, rawName, attributes, elem);

		  // bit of a hack here...
		  if (!isExtension && (elem is ElemLiteralResult))
		  {
			isExtension = ((ElemLiteralResult) elem).containsExtensionElementURI(uri);

			if (isExtension)
			{

			  // System.out.println("Creating extension(2): "+uri);
			  elem = new ElemExtensionCall();

			  elem.LocaterInfo = handler.Locator;
			  elem.Prefixes = handler.NamespaceSupport;
			  ((ElemLiteralResult) elem).Namespace = uri;
			  ((ElemLiteralResult) elem).LocalName = localName;
			  ((ElemLiteralResult) elem).RawName = rawName;
			  setPropertiesFromAttributes(handler, rawName, attributes, elem);
			}
		  }

		  appendAndPush(handler, elem);
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// This method could be over-ridden by a class that extends this class. </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <returns> an object that represents the stylesheet element. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(StylesheetHandler handler, String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public override void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {

		ElemTemplateElement elem = handler.ElemTemplateElement;

		if (elem is ElemLiteralResult)
		{
		  if (((ElemLiteralResult) elem).IsLiteralResultAsStylesheet)
		  {
			handler.popStylesheet();
		  }
		}

		base.endElement(handler, uri, localName, rawName);
	  }

		private bool declaredXSLNS(Stylesheet stylesheet)
		{
			System.Collections.IList declaredPrefixes = stylesheet.DeclaredPrefixes;
			int n = declaredPrefixes.Count;

			for (int i = 0; i < n; i++)
			{
				XMLNSDecl decl = (XMLNSDecl) declaredPrefixes[i];
				if (decl.URI.Equals(Constants.S_XSLNAMESPACEURL))
				{
					return true;
				}
			}
			return false;
		}
	}

}