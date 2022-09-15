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
 * $Id: TransformState.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using TransformStateSetter = org.apache.xml.serializer.TransformStateSetter;

	using Node = org.w3c.dom.Node;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// This interface is meant to be used by a consumer of
	/// SAX2 events produced by Xalan, and enables the consumer
	/// to get information about the state of the transform.  It
	/// is primarily intended as a tooling interface.  A content
	/// handler can get a reference to a TransformState by implementing
	/// the TransformerClient interface.  Xalan will check for
	/// that interface before it calls startDocument, and, if it
	/// is implemented, pass in a TransformState reference to the
	/// setTransformState method.
	/// 
	/// <para>Note that the current stylesheet and root stylesheet can
	/// be retrieved from the ElemTemplateElement obtained from
	/// either getCurrentElement() or getCurrentTemplate().</para>
	/// 
	/// This interface contains only getter methods, any setters are in the interface
	/// TransformStateSetter which this interface extends.
	/// </summary>
	/// <seealso cref="org.apache.xml.serializer.TransformStateSetter"/>
	public interface TransformState : TransformStateSetter
	{

	  /// <summary>
	  /// Retrieves the stylesheet element that produced
	  /// the SAX event.
	  /// 
	  /// <para>Please note that the ElemTemplateElement returned may
	  /// be in a default template, and thus may not be
	  /// defined in the stylesheet.</para>
	  /// </summary>
	  /// <returns> the stylesheet element that produced the SAX event. </returns>
	  ElemTemplateElement CurrentElement {get;}

	  /// <summary>
	  /// This method retrieves the current context node
	  /// in the source tree.
	  /// </summary>
	  /// <returns> the current context node in the source tree. </returns>
	  Node CurrentNode {get;}

	  /// <summary>
	  /// This method retrieves the xsl:template
	  /// that is in effect, which may be a matched template
	  /// or a named template.
	  /// 
	  /// <para>Please note that the ElemTemplate returned may
	  /// be a default template, and thus may not have a template
	  /// defined in the stylesheet.</para>
	  /// </summary>
	  /// <returns> the xsl:template that is in effect </returns>
	  ElemTemplate CurrentTemplate {get;}

	  /// <summary>
	  /// This method retrieves the xsl:template
	  /// that was matched.  Note that this may not be
	  /// the same thing as the current template (which
	  /// may be from getCurrentElement()), since a named
	  /// template may be in effect.
	  /// 
	  /// <para>Please note that the ElemTemplate returned may
	  /// be a default template, and thus may not have a template
	  /// defined in the stylesheet.</para>
	  /// </summary>
	  /// <returns> the xsl:template that was matched. </returns>
	  ElemTemplate MatchedTemplate {get;}

	  /// <summary>
	  /// Retrieves the node in the source tree that matched
	  /// the template obtained via getMatchedTemplate().
	  /// </summary>
	  /// <returns> the node in the source tree that matched
	  /// the template obtained via getMatchedTemplate(). </returns>
	  Node MatchedNode {get;}

	  /// <summary>
	  /// Get the current context node list.
	  /// </summary>
	  /// <returns> the current context node list. </returns>
	  NodeIterator ContextNodeList {get;}

	  /// <summary>
	  /// Get the TrAX Transformer object in effect.
	  /// </summary>
	  /// <returns> the TrAX Transformer object in effect. </returns>
	  Transformer Transformer {get;}



	}

}