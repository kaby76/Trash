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
 * $Id: ProcessorPreserveSpace.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using WhiteSpaceInfo = org.apache.xalan.templates.WhiteSpaceInfo;
	using XPath = org.apache.xpath.XPath;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// TransformerFactory for xsl:preserve-space markup.
	/// <pre>
	/// <!ELEMENT xsl:preserve-space EMPTY>
	/// <!ATTLIST xsl:preserve-space elements CDATA #REQUIRED>
	/// </pre>
	/// </summary>
	[Serializable]
	internal class ProcessorPreserveSpace : XSLTElementProcessor
	{
		internal new const long serialVersionUID = -5552836470051177302L;

	  /// <summary>
	  /// Receive notification of the start of an preserve-space element.
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
		Stylesheet thisSheet = handler.Stylesheet;
		WhitespaceInfoPaths paths = new WhitespaceInfoPaths(thisSheet);
		setPropertiesFromAttributes(handler, rawName, attributes, paths);

		ArrayList xpaths = paths.Elements;

		for (int i = 0; i < xpaths.Count; i++)
		{
		  WhiteSpaceInfo wsi = new WhiteSpaceInfo((XPath) xpaths[i], false, thisSheet);
		  wsi.Uid = handler.nextUid();

		  thisSheet.PreserveSpaces = wsi;
		}
		paths.clearElements();
	  }
	}

}