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
 * $Id: ElemCopy.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using ClonerToResultTree = org.apache.xalan.transformer.ClonerToResultTree;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using SerializerUtils = org.apache.xalan.serialize.SerializerUtils;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Implement xsl:copy.
	/// <pre>
	/// <!ELEMENT xsl:copy %template;>
	/// <!ATTLIST xsl:copy
	///   %space-att;
	///   use-attribute-sets %qnames; #IMPLIED
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#copying">copying in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemCopy : ElemUse
	{
		internal new const long serialVersionUID = 5478580783896941384L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element  </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_COPY;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> This element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_COPY_STRING;
		  }
	  }

	  /// <summary>
	  /// The xsl:copy element provides an easy way of copying the current node.
	  /// Executing this function creates a copy of the current node into the
	  /// result tree.
	  /// <para>The namespace nodes of the current node are automatically
	  /// copied as well, but the attributes and children of the node are not
	  /// automatically copied. The content of the xsl:copy element is a
	  /// template for the attributes and children of the created node;
	  /// the content is instantiated only for nodes of types that can have
	  /// attributes or children (i.e. root nodes and element nodes).</para>
	  /// <para>The root node is treated specially because the root node of the
	  /// result tree is created implicitly. When the current node is the
	  /// root node, xsl:copy will not create a root node, but will just use
	  /// the content template.</para>
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
					XPathContext xctxt = transformer.XPathContext;

		try
		{
		  int sourceNode = xctxt.CurrentNode;
		  xctxt.pushCurrentNode(sourceNode);
		  DTM dtm = xctxt.getDTM(sourceNode);
		  short nodeType = dtm.getNodeType(sourceNode);

		  if ((org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE != nodeType) && (org.apache.xml.dtm.DTM_Fields.DOCUMENT_FRAGMENT_NODE != nodeType))
		  {
			SerializationHandler rthandler = transformer.SerializationHandler;

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEvent(this);
			}

			// TODO: Process the use-attribute-sets stuff
			ClonerToResultTree.cloneToResultTree(sourceNode, nodeType, dtm, rthandler, false);

			if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == nodeType)
			{
			  base.execute(transformer);
			  SerializerUtils.processNSDecls(rthandler, sourceNode, nodeType, dtm);
			  transformer.executeChildTemplates(this, true);

			  string ns = dtm.getNamespaceURI(sourceNode);
			  string localName = dtm.getLocalName(sourceNode);
			  transformer.ResultTreeHandler.endElement(ns, localName, dtm.getNodeName(sourceNode));
			}
			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEndEvent(this);
			}
		  }
		  else
		  {
			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEvent(this);
			}

			base.execute(transformer);
			transformer.executeChildTemplates(this, true);

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEndEvent(this);
			}
		  }
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  xctxt.popCurrentNode();
		}
	  }
	}

}