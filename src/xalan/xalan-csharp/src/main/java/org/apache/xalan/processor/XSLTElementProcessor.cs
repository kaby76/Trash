using System;
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
 * $Id: XSLTElementProcessor.java 1581058 2014-03-24 20:55:14Z ggregory $
 */
namespace org.apache.xalan.processor
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using IntStack = org.apache.xml.utils.IntStack;
	using Attributes = org.xml.sax.Attributes;
	using InputSource = org.xml.sax.InputSource;
	using AttributesImpl = org.xml.sax.helpers.AttributesImpl;

	/// <summary>
	/// This class acts as the superclass for all stylesheet element
	/// processors, and deals with things that are common to all elements. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	[Serializable]
	public class XSLTElementProcessor : ElemTemplateElement
	{
		internal new const long serialVersionUID = 5597421564955304421L;

	  /// <summary>
	  /// Construct a processor for top-level elements. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	  internal XSLTElementProcessor()
	  {
	  }

		private IntStack m_savedLastOrder;

	  /// <summary>
	  /// The element definition that this processor conforms to.
	  /// </summary>
	  private XSLTElementDef m_elemDef;

	  /// <summary>
	  /// Get the element definition that belongs to this element.
	  /// </summary>
	  /// <returns> The element definition object that produced and constrains this element. </returns>
	  internal virtual XSLTElementDef ElemDef
	  {
		  get
		  {
			return m_elemDef;
		  }
		  set
		  {
			m_elemDef = value;
		  }
	  }


	  /// <summary>
	  /// Resolve an external entity.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="publicId"> The public identifer, or null if none is
	  ///                 available. </param>
	  /// <param name="systemId"> The system identifier provided in the XML
	  ///                 document. </param>
	  /// <returns> The new input source, or null to require the
	  ///         default behaviour. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.InputSource resolveEntity(StylesheetHandler handler, String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual InputSource resolveEntity(StylesheetHandler handler, string publicId, string systemId)
	  {
		return null;
	  }

	  /// <summary>
	  /// Receive notification of a notation declaration.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="name"> The notation name. </param>
	  /// <param name="publicId"> The notation public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The notation system identifier. </param>
	  /// <seealso cref="org.xml.sax.DTDHandler.notationDecl"/>
	  public virtual void notationDecl(StylesheetHandler handler, string name, string publicId, string systemId)
	  {

		// no op
	  }

	  /// <summary>
	  /// Receive notification of an unparsed entity declaration.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="name"> The entity name. </param>
	  /// <param name="publicId"> The entity public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The entity system identifier. </param>
	  /// <param name="notationName"> The name of the associated notation. </param>
	  /// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"/>
	  public virtual void unparsedEntityDecl(StylesheetHandler handler, string name, string publicId, string systemId, string notationName)
	  {

		// no op
	  }

	  /// <summary>
	  /// Receive notification of the start of the non-text event.  This
	  /// is sent to the current processor when any non-text event occurs.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startNonText(StylesheetHandler handler) throws org.xml.sax.SAXException
	  public virtual void startNonText(StylesheetHandler handler)
	  {

		// no op
	  }

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
	  public virtual void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {

		if (m_savedLastOrder == null)
		{
					m_savedLastOrder = new IntStack();
		}
				m_savedLastOrder.push(ElemDef.LastOrder);
				ElemDef.LastOrder = -1;
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
	  public virtual void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {
			if (m_savedLastOrder != null && !m_savedLastOrder.empty())
			{
				ElemDef.LastOrder = m_savedLastOrder.pop();
			}

			if (!ElemDef.RequiredFound)
			{
				handler.error(XSLTErrorResources.ER_REQUIRED_ELEM_NOT_FOUND, new object[]{ElemDef.RequiredElem}, null);
			}
	  }

	  /// <summary>
	  /// Receive notification of character data inside an element.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="ch"> The characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(StylesheetHandler handler, char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(StylesheetHandler handler, char[] ch, int start, int length)
	  {
		handler.error(XSLTErrorResources.ER_CHARS_NOT_ALLOWED, null, null); //"Characters are not allowed at this point in the document!",
					  //null);
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="ch"> The whitespace characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(StylesheetHandler handler, char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(StylesheetHandler handler, char[] ch, int start, int length)
	  {

		// no op
	  }

	  /// <summary>
	  /// Receive notification of a processing instruction.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(StylesheetHandler handler, String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(StylesheetHandler handler, string target, string data)
	  {

		// no op
	  }

	  /// <summary>
	  /// Receive notification of a skipped entity.
	  /// 
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="name"> The name of the skipped entity. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(StylesheetHandler handler, String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(StylesheetHandler handler, string name)
	  {

		// no op
	  }

	  /// <summary>
	  /// Set the properties of an object from the given attribute list. </summary>
	  /// <param name="handler"> The stylesheet's Content handler, needed for
	  ///                error reporting. </param>
	  /// <param name="rawName"> The raw name of the owner element, needed for
	  ///                error reporting. </param>
	  /// <param name="attributes"> The list of attributes. </param>
	  /// <param name="target"> The target element where the properties will be set. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void setPropertiesFromAttributes(StylesheetHandler handler, String rawName, org.xml.sax.Attributes attributes, org.apache.xalan.templates.ElemTemplateElement target) throws org.xml.sax.SAXException
	  internal virtual void setPropertiesFromAttributes(StylesheetHandler handler, string rawName, Attributes attributes, ElemTemplateElement target)
	  {
		setPropertiesFromAttributes(handler, rawName, attributes, target, true);
	  }

	  /// <summary>
	  /// Set the properties of an object from the given attribute list. </summary>
	  /// <param name="handler"> The stylesheet's Content handler, needed for
	  ///                error reporting. </param>
	  /// <param name="rawName"> The raw name of the owner element, needed for
	  ///                error reporting. </param>
	  /// <param name="attributes"> The list of attributes. </param>
	  /// <param name="target"> The target element where the properties will be set. </param>
	  /// <param name="throwError"> True if it should throw an error if an
	  /// attribute is not defined. </param>
	  /// <returns> the attributes not allowed on this element.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: org.xml.sax.Attributes setPropertiesFromAttributes(StylesheetHandler handler, String rawName, org.xml.sax.Attributes attributes, org.apache.xalan.templates.ElemTemplateElement target, boolean throwError) throws org.xml.sax.SAXException
	  internal virtual Attributes setPropertiesFromAttributes(StylesheetHandler handler, string rawName, Attributes attributes, ElemTemplateElement target, bool throwError)
	  {

		XSLTElementDef def = ElemDef;
		AttributesImpl undefines = null;
		bool isCompatibleMode = ((null != handler.Stylesheet && handler.Stylesheet.CompatibleMode) || !throwError);
		if (isCompatibleMode)
		{
		  undefines = new AttributesImpl();
		}


		// Keep track of which XSLTAttributeDefs have been processed, so 
		// I can see which default values need to be set.
		System.Collections.IList processedDefs = new ArrayList();

		// Keep track of XSLTAttributeDefs that were invalid
		System.Collections.IList errorDefs = new ArrayList();
		int nAttrs = attributes.getLength();

		for (int i = 0; i < nAttrs; i++)
		{
		  string attrUri = attributes.getURI(i);
		  // Hack for Crimson.  -sb
		  if ((null != attrUri) && (attrUri.Length == 0) && (attributes.getQName(i).StartsWith("xmlns:") || attributes.getQName(i).Equals("xmlns")))
		  {
			attrUri = org.apache.xalan.templates.Constants.S_XMLNAMESPACEURI;
		  }
		  string attrLocalName = attributes.getLocalName(i);
		  XSLTAttributeDef attrDef = def.getAttributeDef(attrUri, attrLocalName);

		  if (null == attrDef)
		  {
			if (!isCompatibleMode)
			{

			  // Then barf, because this element does not allow this attribute.
			  handler.error(XSLTErrorResources.ER_ATTR_NOT_ALLOWED, new object[]{attributes.getQName(i), rawName}, null); //"\""+attributes.getQName(i)+"\""
							//+ " attribute is not allowed on the " + rawName
						   // + " element!", null);
			}
			else
			{
			  undefines.addAttribute(attrUri, attrLocalName, attributes.getQName(i), attributes.getType(i), attributes.getValue(i));
			}
		  }
		  else
		  {
			//handle secure processing
			if (handler.StylesheetProcessor == null)
			{
				Console.WriteLine("stylesheet processor null");
			}
			if (string.CompareOrdinal(attrDef.Name, "*") == 0 && handler.StylesheetProcessor.SecureProcessing)
			{
				//foreign attributes are not allowed in secure processing mode
				// Then barf, because this element does not allow this attribute.
				handler.error(XSLTErrorResources.ER_ATTR_NOT_ALLOWED, new object[]{attributes.getQName(i), rawName}, null); //"\""+attributes.getQName(i)+"\""
				//+ " attribute is not allowed on the " + rawName
				// + " element!", null);
			}
			else
			{


				bool success = attrDef.setAttrValue(handler, attrUri, attrLocalName, attributes.getQName(i), attributes.getValue(i), target);

				// Now we only add the element if it passed a validation check
				if (success)
				{
					processedDefs.Add(attrDef);
				}
				else
				{
					errorDefs.Add(attrDef);
				}
			}
		  }
		}

		XSLTAttributeDef[] attrDefs = def.Attributes;
		int nAttrDefs = attrDefs.Length;

		for (int i = 0; i < nAttrDefs; i++)
		{
		  XSLTAttributeDef attrDef = attrDefs[i];
		  string defVal = attrDef.Default;

		  if (null != defVal)
		  {
			if (!processedDefs.Contains(attrDef))
			{
			  attrDef.setDefAttrValue(handler, target);
			}
		  }

		  if (attrDef.Required)
		  {
			if ((!processedDefs.Contains(attrDef)) && (!errorDefs.Contains(attrDef)))
			{
			  handler.error(XSLMessages.createMessage(XSLTErrorResources.ER_REQUIRES_ATTRIB, new object[]{rawName, attrDef.Name}), null);
			}
		  }
		}

		return undefines;
	  }
	}

}