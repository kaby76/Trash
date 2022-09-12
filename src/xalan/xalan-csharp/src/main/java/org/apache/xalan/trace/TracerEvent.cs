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
 * $Id: TracerEvent.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;

	using Attr = org.w3c.dom.Attr;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// Parent class of events generated for tracing the
	/// progress of the XSL processor.
	/// @xsl.usage advanced
	/// </summary>
	public class TracerEvent : java.util.EventListener
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
	  /// The current mode.
	  /// </summary>
	  public readonly QName m_mode;

	  /// <summary>
	  /// Create an event originating at the given node of the style tree. </summary>
	  /// <param name="processor"> The XSLT TransformerFactory. </param>
	  /// <param name="sourceNode"> The current context node. </param>
	  /// <param name="mode"> The current mode. </param>
	  /// <param name="styleNode"> The stylesheet element that is executing. </param>
	  public TracerEvent(TransformerImpl processor, Node sourceNode, QName mode, ElemTemplateElement styleNode)
	  {

		this.m_processor = processor;
		this.m_sourceNode = sourceNode;
		this.m_mode = mode;
		this.m_styleNode = styleNode;
	  }

	  /// <summary>
	  /// Returns a string representation of the node.
	  /// The string returned for elements will contain the element name
	  /// and any attributes enclosed in angle brackets.
	  /// The string returned for attributes will be of form, "name=value."
	  /// </summary>
	  /// <param name="n"> any DOM node. Must not be null.
	  /// </param>
	  /// <returns> a string representation of the given node. </returns>
	  public static string printNode(Node n)
	  {

		string r = n.GetHashCode() + " ";

		if (n is Element)
		{
		  r += "<" + n.NodeName;

		  Node c = n.FirstChild;

		  while (null != c)
		  {
			if (c is Attr)
			{
			  r += printNode(c) + " ";
			}

			c = c.NextSibling;
		  }

		  r += ">";
		}
		else
		{
		  if (n is Attr)
		  {
			r += n.NodeName + "=" + n.NodeValue;
		  }
		  else
		  {
			r += n.NodeName;
		  }
		}

		return r;
	  }

	  /// <summary>
	  /// Returns a string representation of the node list.
	  /// The string will contain the list of nodes inside square braces.
	  /// Elements will contain the element name
	  /// and any attributes enclosed in angle brackets.
	  /// Attributes will be of form, "name=value."
	  /// </summary>
	  /// <param name="l"> any DOM node list. Must not be null.
	  /// </param>
	  /// <returns> a string representation of the given node list. </returns>
	  public static string printNodeList(NodeList l)
	  {

		string r = l.GetHashCode() + "[";
		int len = l.Length - 1;
		int i = 0;

		while (i < len)
		{
		  Node n = l.item(i);

		  if (null != n)
		  {
			r += printNode(n) + ", ";
		  }

		  ++i;
		}

		if (i == len)
		{
		  Node n = l.item(len);

		  if (null != n)
		  {
			r += printNode(n);
		  }
		}

		return r + "]";
	  }
	}

}