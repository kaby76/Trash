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
 * $Id: ProcessorTemplate.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;

	/// <summary>
	/// TransformerFactory for xsl:template markup.
	/// </summary>
	[Serializable]
	internal class ProcessorTemplate : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = -8457812845473603860L;

	  /// <summary>
	  /// Append the current template element to the current
	  /// template element, and then push it onto the current template
	  /// element stack.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="elem"> Must be a non-null reference to a <seealso cref="org.apache.xalan.templates.ElemTemplate"/> object.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void appendAndPush(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  protected internal override void appendAndPush(StylesheetHandler handler, ElemTemplateElement elem)
	  {

		base.appendAndPush(handler, elem);
		elem.DOMBackPointer = handler.OriginatingNode;
		handler.Stylesheet.Template = (ElemTemplate) elem;
	  }
	}

}