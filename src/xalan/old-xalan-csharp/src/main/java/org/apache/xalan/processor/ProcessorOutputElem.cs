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
 * $Id: ProcessorOutputElem.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{


	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using OutputProperties = org.apache.xalan.templates.OutputProperties;
	using OutputPropertiesFactory = org.apache.xml.serializer.OutputPropertiesFactory;
	using QName = org.apache.xml.utils.QName;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// TransformerFactory for xsl:output markup. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#output">output in XSLT Specification</a> </seealso>
	[Serializable]
	internal class ProcessorOutputElem : XSLTElementProcessor
	{
		internal new const long serialVersionUID = 3513742319582547590L;

	  /// <summary>
	  /// The output properties, set temporarily while the properties are 
	  ///  being set from the attributes, and then nulled after that operation 
	  ///  is completed.  
	  /// </summary>
	  private OutputProperties m_outputProperties;

	  /// <summary>
	  /// Set the cdata-section-elements property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#CDATA_SECTION_ELEMENTS </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual ArrayList CdataSectionElements
	  {
		  set
		  {
			m_outputProperties.setQNameProperties(OutputKeys.CDATA_SECTION_ELEMENTS, value);
		  }
	  }

	  /// <summary>
	  /// Set the doctype-public property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#DOCTYPE_PUBLIC </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual string DoctypePublic
	  {
		  set
		  {
			m_outputProperties.setProperty(OutputKeys.DOCTYPE_PUBLIC, value);
		  }
	  }

	  /// <summary>
	  /// Set the doctype-system property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#DOCTYPE_SYSTEM </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual string DoctypeSystem
	  {
		  set
		  {
			m_outputProperties.setProperty(OutputKeys.DOCTYPE_SYSTEM, value);
		  }
	  }

	  /// <summary>
	  /// Set the encoding property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#ENCODING </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual string Encoding
	  {
		  set
		  {
			m_outputProperties.setProperty(OutputKeys.ENCODING, value);
		  }
	  }

	  /// <summary>
	  /// Set the indent property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#INDENT </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual bool Indent
	  {
		  set
		  {
			m_outputProperties.setBooleanProperty(OutputKeys.INDENT, value);
		  }
	  }

	  /// <summary>
	  /// Set the media type property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#MEDIA_TYPE </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual string MediaType
	  {
		  set
		  {
			m_outputProperties.setProperty(OutputKeys.MEDIA_TYPE, value);
		  }
	  }

	  /// <summary>
	  /// Set the method property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#METHOD </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual QName Method
	  {
		  set
		  {
			m_outputProperties.setQNameProperty(OutputKeys.METHOD, value);
		  }
	  }

	  /// <summary>
	  /// Set the omit-xml-declaration property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#OMIT_XML_DECLARATION </seealso>
	  /// <param name="newValue"> processed attribute value. </param>
	  public virtual bool OmitXmlDeclaration
	  {
		  set
		  {
			m_outputProperties.setBooleanProperty(OutputKeys.OMIT_XML_DECLARATION, value);
		  }
	  }

	  /// <summary>
	  /// Set the standalone property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#STANDALONE </seealso>
	  /// <param name="newValue"> processed attribute value. </param>
	  public virtual bool Standalone
	  {
		  set
		  {
			m_outputProperties.setBooleanProperty(OutputKeys.STANDALONE, value);
		  }
	  }

	  /// <summary>
	  /// Set the version property from the attribute value. </summary>
	  /// <seealso cref= javax.xml.transform.OutputKeys#VERSION </seealso>
	  /// <param name="newValue"> non-null reference to processed attribute value. </param>
	  public virtual string Version
	  {
		  set
		  {
			m_outputProperties.setProperty(OutputKeys.VERSION, value);
		  }
	  }

	  /// <summary>
	  /// Set a foreign property from the attribute value. </summary>
	  /// <param name="newValue"> non-null reference to attribute value. </param>
	  public virtual void setForeignAttr(string attrUri, string attrLocalName, string attrRawName, string attrValue)
	  {
		QName key = new QName(attrUri, attrLocalName);
		m_outputProperties.setProperty(key, attrValue);
	  }

	  /// <summary>
	  /// Set a foreign property from the attribute value. </summary>
	  /// <param name="newValue"> non-null reference to attribute value. </param>
	  public virtual void addLiteralResultAttribute(string attrUri, string attrLocalName, string attrRawName, string attrValue)
	  {
		QName key = new QName(attrUri, attrLocalName);
		m_outputProperties.setProperty(key, attrValue);
	  }

	  /// <summary>
	  /// Receive notification of the start of an xsl:output element.
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
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {
		// Hmmm... for the moment I don't think I'll have default properties set for this. -sb
		m_outputProperties = new OutputProperties();

		m_outputProperties.DOMBackPointer = handler.OriginatingNode;
		m_outputProperties.LocaterInfo = handler.Locator;
		m_outputProperties.Uid = handler.nextUid();
		setPropertiesFromAttributes(handler, rawName, attributes, this);

		// Access this only from the Hashtable level... we don't want to 
		// get default properties.
		string entitiesFileName = (string) m_outputProperties.Properties.get(OutputPropertiesFactory.S_KEY_ENTITIES);

		if (null != entitiesFileName)
		{
		  try
		  {
			string absURL = SystemIDResolver.getAbsoluteURI(entitiesFileName, handler.BaseIdentifier);
			m_outputProperties.Properties.put(OutputPropertiesFactory.S_KEY_ENTITIES, absURL);
		  }
		  catch (TransformerException te)
		  {
			handler.error(te.Message, te);
		  }
		}

		handler.Stylesheet.Output = m_outputProperties;

		ElemTemplateElement parent = handler.ElemTemplateElement;
		parent.appendChild(m_outputProperties);

		m_outputProperties = null;
	  }
	}

}