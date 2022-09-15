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
 * $Id: ProcessorKey.java 469688 2006-10-31 22:39:43Z minchau $
 */
namespace org.apache.xalan.processor
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using KeyDeclaration = org.apache.xalan.templates.KeyDeclaration;
	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// TransformerFactory for xsl:key markup.
	/// <pre>
	/// <!ELEMENT xsl:key EMPTY>
	/// <!ATTLIST xsl:key
	///   name %qname; #REQUIRED
	///   match %pattern; #REQUIRED
	///   use %expr; #REQUIRED
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#key">key in XSLT Specification</a> </seealso>
	[Serializable]
	internal class ProcessorKey : XSLTElementProcessor
	{
		internal new const long serialVersionUID = 4285205417566822979L;

	  /// <summary>
	  /// Receive notification of the start of an xsl:key element.
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

		KeyDeclaration kd = new KeyDeclaration(handler.Stylesheet, handler.nextUid());

		kd.DOMBackPointer = handler.OriginatingNode;
		kd.LocaterInfo = handler.Locator;
		setPropertiesFromAttributes(handler, rawName, attributes, kd);
		handler.Stylesheet.Key = kd;
	  }

	  /// <summary>
	  /// Set the properties of an object from the given attribute list. </summary>
	  /// <param name="handler"> The stylesheet's Content handler, needed for
	  ///                error reporting. </param>
	  /// <param name="rawName"> The raw name of the owner element, needed for
	  ///                error reporting. </param>
	  /// <param name="attributes"> The list of attributes. </param>
	  /// <param name="target"> The target element where the properties will be set. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setPropertiesFromAttributes(StylesheetHandler handler, String rawName, org.xml.sax.Attributes attributes, org.apache.xalan.templates.ElemTemplateElement target) throws org.xml.sax.SAXException
	  internal override void setPropertiesFromAttributes(StylesheetHandler handler, string rawName, Attributes attributes, org.apache.xalan.templates.ElemTemplateElement target)
	  {

		XSLTElementDef def = ElemDef;

		// Keep track of which XSLTAttributeDefs have been processed, so 
		// I can see which default values need to be set.
		IList processedDefs = new ArrayList();
		int nAttrs = attributes.Length;

		for (int i = 0; i < nAttrs; i++)
		{
		  string attrUri = attributes.getURI(i);
		  string attrLocalName = attributes.getLocalName(i);
		  XSLTAttributeDef attrDef = def.getAttributeDef(attrUri, attrLocalName);

		  if (null == attrDef)
		  {

			// Then barf, because this element does not allow this attribute.
			handler.error(attributes.getQName(i) + "attribute is not allowed on the " + rawName + " element!", null);
		  }
		  else
		  {
			string valueString = attributes.getValue(i);

			if (valueString.IndexOf(org.apache.xpath.compiler.Keywords.FUNC_KEY_STRING + "(", StringComparison.Ordinal) >= 0)
			{
			  handler.error(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_KEY_CALL, null), null);
			}

			processedDefs.Add(attrDef);
			attrDef.setAttrValue(handler, attrUri, attrLocalName, attributes.getQName(i), attributes.getValue(i), target);
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
			if (!processedDefs.Contains(attrDef))
			{
			  handler.error(XSLMessages.createMessage(XSLTErrorResources.ER_REQUIRES_ATTRIB, new object[]{rawName, attrDef.Name}), null);
			}
		  }
		}
	  }
	}

}