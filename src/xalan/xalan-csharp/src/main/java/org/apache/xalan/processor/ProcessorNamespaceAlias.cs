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
 * $Id: ProcessorNamespaceAlias.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using NamespaceAlias = org.apache.xalan.templates.NamespaceAlias;
	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// TransformerFactory for xsl:namespace-alias markup.
	/// A stylesheet can use the xsl:namespace-alias element to
	/// declare that one namespace URI is an alias for another namespace URI.
	/// <pre>
	/// <!ELEMENT xsl:namespace-alias EMPTY>
	/// <!ATTLIST xsl:namespace-alias
	///   stylesheet-prefix CDATA #REQUIRED
	///   result-prefix CDATA #REQUIRED
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"/>
	[Serializable]
	internal class ProcessorNamespaceAlias : XSLTElementProcessor
	{
		internal new const long serialVersionUID = -6309867839007018964L;

	  /// <summary>
	  /// Receive notification of the start of an xsl:namespace-alias element.
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String resultNS;
		string resultNS;
		NamespaceAlias na = new NamespaceAlias(handler.nextUid());

		setPropertiesFromAttributes(handler, rawName, attributes, na);
		string prefix = na.StylesheetPrefix;
		if (prefix.Equals("#default"))
		{
		  prefix = "";
		  na.StylesheetPrefix = prefix;
		}
		string stylesheetNS = handler.getNamespaceForPrefix(prefix);
		na.StylesheetNamespace = stylesheetNS;
		prefix = na.ResultPrefix;
		if (prefix.Equals("#default"))
		{
		  prefix = "";
		  na.ResultPrefix = prefix;
		  resultNS = handler.getNamespaceForPrefix(prefix);
		  if (null == resultNS)
		  {
			handler.error(XSLTErrorResources.ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, null, null);
		  }
		}
		else
		{
			resultNS = handler.getNamespaceForPrefix(prefix);
			if (null == resultNS)
			{
			 handler.error(XSLTErrorResources.ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, new object[] {prefix}, null);
			}
		}

		na.ResultNamespace = resultNS;
		handler.Stylesheet.NamespaceAlias = na;
		handler.Stylesheet.appendChild(na);
	  }
	}

}