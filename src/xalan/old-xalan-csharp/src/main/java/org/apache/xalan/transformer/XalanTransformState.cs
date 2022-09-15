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
 * $Id: XalanTransformState.java 468645 2006-10-28 06:57:24Z minchau $
 */

namespace org.apache.xalan.transformer
{

	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Node = org.w3c.dom.Node;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// Before the serializer merge, the TransformState interface was
	/// implemented by ResultTreeHandler.
	/// </summary>
	public class XalanTransformState : TransformState
	{

		internal Node m_node = null;
		internal ElemTemplateElement m_currentElement = null;
		internal ElemTemplate m_currentTemplate = null;
		internal ElemTemplate m_matchedTemplate = null;
		internal int m_currentNodeHandle = org.apache.xml.dtm.DTM_Fields.NULL;
		internal Node m_currentNode = null;
		internal int m_matchedNode = org.apache.xml.dtm.DTM_Fields.NULL;
		internal DTMIterator m_contextNodeList = null;
		internal bool m_elemPending = false;
		internal TransformerImpl m_transformer = null;

		/// <seealso cref= org.apache.xml.serializer.TransformStateSetter#setCurrentNode(Node) </seealso>
		public virtual Node CurrentNode
		{
			set
			{
				m_node = value;
			}
			get
			{
			  if (m_currentNode != null)
			  {
				 return m_currentNode;
			  }
			  else
			  {
				 DTM dtm = m_transformer.XPathContext.getDTM(m_transformer.CurrentNode);
				 return dtm.getNode(m_transformer.CurrentNode);
			  }
			}
		}

		/// <seealso cref= org.apache.xml.serializer.TransformStateSetter#resetState(Transformer) </seealso>
		public virtual void resetState(Transformer transformer)
		{
			if ((transformer != null) && (transformer is TransformerImpl))
			{
			   m_transformer = (TransformerImpl)transformer;
			   m_currentElement = m_transformer.CurrentElement;
			   m_currentTemplate = m_transformer.CurrentTemplate;
			   m_matchedTemplate = m_transformer.MatchedTemplate;
			   int currentNodeHandle = m_transformer.CurrentNode;
			   DTM dtm = m_transformer.XPathContext.getDTM(currentNodeHandle);
			   m_currentNode = dtm.getNode(currentNodeHandle);
			   m_matchedNode = m_transformer.MatchedNode;
			   m_contextNodeList = m_transformer.ContextNodeList;
			}
		}

		/// <seealso cref= org.apache.xalan.transformer.TransformState#getCurrentElement() </seealso>
		public virtual ElemTemplateElement CurrentElement
		{
			get
			{
			  if (m_elemPending)
			  {
				 return m_currentElement;
			  }
			  else
			  {
				 return m_transformer.CurrentElement;
			  }
			}
		}


		/// <seealso cref= org.apache.xalan.transformer.TransformState#getCurrentTemplate() </seealso>
		public virtual ElemTemplate CurrentTemplate
		{
			get
			{
			   if (m_elemPending)
			   {
				 return m_currentTemplate;
			   }
			   else
			   {
				 return m_transformer.CurrentTemplate;
			   }
			}
		}

		/// <seealso cref= org.apache.xalan.transformer.TransformState#getMatchedTemplate() </seealso>
		public virtual ElemTemplate MatchedTemplate
		{
			get
			{
			  if (m_elemPending)
			  {
				 return m_matchedTemplate;
			  }
			  else
			  {
				 return m_transformer.MatchedTemplate;
			  }
			}
		}

		/// <seealso cref= org.apache.xalan.transformer.TransformState#getMatchedNode() </seealso>
		public virtual Node MatchedNode
		{
			get
			{
    
			   if (m_elemPending)
			   {
				 DTM dtm = m_transformer.XPathContext.getDTM(m_matchedNode);
				 return dtm.getNode(m_matchedNode);
			   }
			   else
			   {
				 DTM dtm = m_transformer.XPathContext.getDTM(m_transformer.MatchedNode);
				 return dtm.getNode(m_transformer.MatchedNode);
			   }
			}
		}

		/// <seealso cref= org.apache.xalan.transformer.TransformState#getContextNodeList() </seealso>
		public virtual NodeIterator ContextNodeList
		{
			get
			{
			  if (m_elemPending)
			  {
				  return new org.apache.xml.dtm.@ref.DTMNodeIterator(m_contextNodeList);
			  }
			  else
			  {
				  return new org.apache.xml.dtm.@ref.DTMNodeIterator(m_transformer.ContextNodeList);
			  }
			}
		}
		/// <seealso cref= org.apache.xalan.transformer.TransformState#getTransformer() </seealso>
		public virtual Transformer Transformer
		{
			get
			{
				return m_transformer;
			}
		}

	}

}