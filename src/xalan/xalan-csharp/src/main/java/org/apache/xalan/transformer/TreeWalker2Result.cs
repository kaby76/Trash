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
 * $Id: TreeWalker2Result.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{
	using SerializerUtils = org.apache.xalan.serialize.SerializerUtils;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMTreeWalker = org.apache.xml.dtm.@ref.DTMTreeWalker;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Handle a walk of a tree, but screen out attributes for
	/// the result tree.
	/// @xsl.usage internal
	/// </summary>
	public class TreeWalker2Result : DTMTreeWalker
	{

	  /// <summary>
	  /// The transformer instance </summary>
	  internal TransformerImpl m_transformer;

	  /// <summary>
	  /// The result tree handler </summary>
	  internal SerializationHandler m_handler;

	  /// <summary>
	  /// Node where to start the tree walk </summary>
	  internal int m_startNode;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="transformer"> Non-null transformer instance </param>
	  /// <param name="handler"> The Result tree handler to use </param>
	  public TreeWalker2Result(TransformerImpl transformer, SerializationHandler handler) : base(handler, null)
	  {


		m_transformer = transformer;
		m_handler = handler;
	  }

	  /// <summary>
	  /// Perform a pre-order traversal non-recursive style.
	  /// </summary>
	  /// <param name="pos"> Start node for traversal
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void traverse(int pos) throws org.xml.sax.SAXException
	  public override void traverse(int pos)
	  {
		m_dtm = m_transformer.XPathContext.getDTM(pos);
		m_startNode = pos;

		base.traverse(pos);
	  }

			/// <summary>
			/// End processing of given node 
			/// 
			/// </summary>
			/// <param name="node"> Node we just finished processing
			/// </param>
			/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void endNode(int node) throws org.xml.sax.SAXException
	  protected internal override void endNode(int node)
	  {
		base.endNode(node);
		if (DTM.ELEMENT_NODE == m_dtm.getNodeType(node))
		{
		  m_transformer.XPathContext.popCurrentNode();
		}
	  }

	  /// <summary>
	  /// Start traversal of the tree at the given node
	  /// 
	  /// </summary>
	  /// <param name="node"> Starting node for traversal
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void startNode(int node) throws org.xml.sax.SAXException
	  protected internal override void startNode(int node)
	  {

		XPathContext xcntxt = m_transformer.XPathContext;
		try
		{

		  if (DTM.ELEMENT_NODE == m_dtm.getNodeType(node))
		  {
			xcntxt.pushCurrentNode(node);

			if (m_startNode != node)
			{
			  base.startNode(node);
			}
			else
			{
			  string elemName = m_dtm.getNodeName(node);
			  string localName = m_dtm.getLocalName(node);
			  string @namespace = m_dtm.getNamespaceURI(node);

			  //xcntxt.pushCurrentNode(node);       
			  // SAX-like call to allow adding attributes afterwards
			  m_handler.startElement(@namespace, localName, elemName);
			  bool hasNSDecls = false;
			  DTM dtm = m_dtm;
			  for (int ns = dtm.getFirstNamespaceNode(node, true); DTM.NULL != ns; ns = dtm.getNextNamespaceNode(node, ns, true))
			  {
				SerializerUtils.ensureNamespaceDeclDeclared(m_handler,dtm, ns);
			  }


			  for (int attr = dtm.getFirstAttribute(node); DTM.NULL != attr; attr = dtm.getNextAttribute(attr))
			  {
				SerializerUtils.addAttribute(m_handler, attr);
			  }
			}

		  }
		  else
		  {
			xcntxt.pushCurrentNode(node);
			base.startNode(node);
			xcntxt.popCurrentNode();
		  }
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }
	}

}