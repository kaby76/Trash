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
 * $Id: SelectionEvent.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;
	using XObject = org.apache.xpath.objects.XObject;

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Event triggered by selection of a node in the style stree.
	/// @xsl.usage advanced
	/// </summary>
	public class SelectionEvent : java.util.EventListener
	{

	  /// <summary>
	  /// The node in the style tree where the event occurs.
	  /// </summary>
	  public readonly ElemTemplateElement m_styleNode;

	  /// <summary>
	  /// The XSLT processor instance.
	  /// </summary>
	  public readonly TransformerImpl m_processor;

	  /// <summary>
	  /// The current context node.
	  /// </summary>
	  public readonly Node m_sourceNode;

	  /// <summary>
	  /// The attribute name from which the selection is made.
	  /// </summary>
	  public readonly string m_attributeName;

	  /// <summary>
	  /// The XPath that executed the selection.
	  /// </summary>
	  public readonly XPath m_xpath;

	  /// <summary>
	  /// The result of the selection.
	  /// </summary>
	  public readonly XObject m_selection;

	  /// <summary>
	  /// Create an event originating at the given node of the style tree.
	  /// </summary>
	  /// <param name="processor"> The XSLT TransformerFactory. </param>
	  /// <param name="sourceNode"> The current context node. </param>
	  /// <param name="styleNode"> node in the style tree reference for the event.
	  /// Should not be null.  That is not enforced. </param>
	  /// <param name="attributeName"> The attribute name from which the selection is made. </param>
	  /// <param name="xpath"> The XPath that executed the selection. </param>
	  /// <param name="selection"> The result of the selection. </param>
	  public SelectionEvent(TransformerImpl processor, Node sourceNode, ElemTemplateElement styleNode, string attributeName, XPath xpath, XObject selection)
	  {

		this.m_processor = processor;
		this.m_sourceNode = sourceNode;
		this.m_styleNode = styleNode;
		this.m_attributeName = attributeName;
		this.m_xpath = xpath;
		this.m_selection = selection;
	  }
	}

}