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
 * $Id: XNodeSetForDOM.java 469368 2006-10-31 04:41:36Z minchau $
 */
namespace org.apache.xpath.objects
{

	using DTMManager = org.apache.xml.dtm.DTMManager;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// This class overrides the XNodeSet#object() method to provide the original 
	/// Node object, NodeList object, or NodeIterator.
	/// </summary>
	[Serializable]
	public class XNodeSetForDOM : XNodeSet
	{
		internal new const long serialVersionUID = -8396190713754624640L;
	  internal object m_origObj;

	  public XNodeSetForDOM(Node node, DTMManager dtmMgr)
	  {
		m_dtmMgr = dtmMgr;
		m_origObj = node;
		int dtmHandle = dtmMgr.getDTMHandleFromNode(node);
		Object = new NodeSetDTM(dtmMgr);
		((NodeSetDTM) m_obj).addNode(dtmHandle);
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> Value of the XNodeSet object </param>
	  public XNodeSetForDOM(XNodeSet val) : base(val)
	  {
		  if (val is XNodeSetForDOM)
		  {
			m_origObj = ((XNodeSetForDOM)val).m_origObj;
		  }
	  }

	  public XNodeSetForDOM(NodeList nodeList, XPathContext xctxt)
	  {
		m_dtmMgr = xctxt.DTMManager;
		m_origObj = nodeList;

		// JKESS 20020514: Longer-term solution is to force
		// folks to request length through an accessor, so we can defer this
		// retrieval... but that requires an API change.
		// m_obj=new org.apache.xpath.NodeSetDTM(nodeList, xctxt);
		NodeSetDTM nsdtm = new NodeSetDTM(nodeList, xctxt);
		m_last = nsdtm.Length;
		Object = nsdtm;
	  }

	  public XNodeSetForDOM(NodeIterator nodeIter, XPathContext xctxt)
	  {
		m_dtmMgr = xctxt.DTMManager;
		m_origObj = nodeIter;

		// JKESS 20020514: Longer-term solution is to force
		// folks to request length through an accessor, so we can defer this
		// retrieval... but that requires an API change.
		// m_obj = new org.apache.xpath.NodeSetDTM(nodeIter, xctxt);
		NodeSetDTM nsdtm = new NodeSetDTM(nodeIter, xctxt);
		m_last = nsdtm.Length;
		Object = nsdtm;
	  }

	  /// <summary>
	  /// Return the original DOM object that the user passed in.  For use primarily
	  /// by the extension mechanism.
	  /// </summary>
	  /// <returns> The object that this class wraps </returns>
	  public override object @object()
	  {
		return m_origObj;
	  }

	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.traversal.NodeIterator nodeset() throws javax.xml.transform.TransformerException
	  public override NodeIterator nodeset()
	  {
		return (m_origObj is NodeIterator) ? (NodeIterator)m_origObj : base.nodeset();
	  }

	  /// <summary>
	  /// Cast result object to a nodelist. Always issues an error.
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.NodeList nodelist() throws javax.xml.transform.TransformerException
	  public override NodeList nodelist()
	  {
		return (m_origObj is NodeList) ? (NodeList)m_origObj : base.nodelist();
	  }



	}

}