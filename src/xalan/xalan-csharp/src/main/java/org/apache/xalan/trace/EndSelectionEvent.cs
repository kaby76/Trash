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
 * $Id: EndSelectionEvent.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;
	using XObject = org.apache.xpath.objects.XObject;

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Event triggered by completion of a xsl:for-each selection or a 
	/// xsl:apply-templates selection.
	/// @xsl.usage advanced
	/// </summary>
	public class EndSelectionEvent : SelectionEvent
	{

	  /// <summary>
	  /// Create an EndSelectionEvent.
	  /// </summary>
	  /// <param name="processor"> The XSLT TransformerFactory. </param>
	  /// <param name="sourceNode"> The current context node. </param>
	  /// <param name="styleNode"> node in the style tree reference for the event.
	  /// Should not be null.  That is not enforced. </param>
	  /// <param name="attributeName"> The attribute name from which the selection is made. </param>
	  /// <param name="xpath"> The XPath that executed the selection. </param>
	  /// <param name="selection"> The result of the selection. </param>
	  public EndSelectionEvent(TransformerImpl processor, Node sourceNode, ElemTemplateElement styleNode, string attributeName, XPath xpath, XObject selection) : base(processor, sourceNode, styleNode, attributeName, xpath, selection)
	  {

	  }
	}

}