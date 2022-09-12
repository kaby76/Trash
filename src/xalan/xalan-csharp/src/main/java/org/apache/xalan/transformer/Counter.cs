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
 * $Id: Counter.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using ElemNumber = org.apache.xalan.templates.ElemNumber;
	using DTM = org.apache.xml.dtm.DTM;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// A class that does incremental counting for support of xsl:number.
	/// This class stores a cache of counted nodes (m_countNodes).
	/// It tries to cache the counted nodes in document order...
	/// the node count is based on its position in the cache list
	/// @xsl.usage internal
	/// </summary>
	public class Counter
	{

	  /// <summary>
	  /// Set the maximum ammount the m_countNodes list can
	  /// grow to.
	  /// </summary>
	  internal const int MAXCOUNTNODES = 500;

	  /// <summary>
	  /// The start count from where m_countNodes counts
	  /// from.  In other words, the count of a given node
	  /// in the m_countNodes vector is node position +
	  /// m_countNodesStartCount.
	  /// </summary>
	  internal int m_countNodesStartCount = 0;

	  /// <summary>
	  /// A vector of all nodes counted so far.
	  /// </summary>
	  internal NodeSetDTM m_countNodes;

	  /// <summary>
	  /// The node from where the counting starts.  This is needed to
	  /// find a counter if the node being counted is not immediatly
	  /// found in the m_countNodes vector.
	  /// </summary>
	  internal int m_fromNode = org.apache.xml.dtm.DTM_Fields.NULL;

	  /// <summary>
	  /// The owning xsl:number element.
	  /// </summary>
	  internal ElemNumber m_numberElem;

	  /// <summary>
	  /// Value to store result of last getCount call, for benifit
	  /// of returning val from CountersTable.getCounterByCounted,
	  /// who calls getCount.
	  /// </summary>
	  internal int m_countResult;

	  /// <summary>
	  /// Construct a counter object.
	  /// </summary>
	  /// <param name="numberElem"> The owning xsl:number element. </param>
	  /// <param name="countNodes"> A vector of all nodes counted so far.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Counter(org.apache.xalan.templates.ElemNumber numberElem, org.apache.xpath.NodeSetDTM countNodes) throws javax.xml.transform.TransformerException
	  internal Counter(ElemNumber numberElem, NodeSetDTM countNodes)
	  {
		m_countNodes = countNodes;
		m_numberElem = numberElem;
	  }

	  /// <summary>
	  /// Construct a counter object.
	  /// </summary>
	  /// <param name="numberElem"> The owning xsl:number element. 
	  /// </param>
	  /// <exception cref="TransformerException">
	  /// 
	  /// Counter(ElemNumber numberElem) throws TransformerException
	  /// {
	  ///  m_numberElem = numberElem;
	  /// } </exception>

	  /// <summary>
	  /// Try and find a node that was previously counted. If found,
	  /// return a positive integer that corresponds to the count.
	  /// </summary>
	  /// <param name="support"> The XPath context to use </param>
	  /// <param name="node"> The node to be counted.
	  /// </param>
	  /// <returns> The count of the node, or -1 if not found. </returns>
	  internal virtual int getPreviouslyCounted(XPathContext support, int node)
	  {

		int n = m_countNodes.size();

		m_countResult = 0;

		for (int i = n - 1; i >= 0; i--)
		{
		  int countedNode = m_countNodes.elementAt(i);

		  if (node == countedNode)
		  {

			// Since the list is in backwards order, the count is 
			// how many are in the rest of the list.
			m_countResult = i + 1 + m_countNodesStartCount;

			break;
		  }

		  DTM dtm = support.getDTM(countedNode);

		  // Try to see if the given node falls after the counted node...
		  // if it does, don't keep searching backwards.
		  if (dtm.isNodeAfter(countedNode, node))
		  {
			break;
		  }
		}

		return m_countResult;
	  }

	  /// <summary>
	  /// Get the last node in the list.
	  /// </summary>
	  /// <returns> the last node in the list. </returns>
	  internal virtual int Last
	  {
		  get
		  {
    
			int size = m_countNodes.size();
    
			return (size > 0) ? m_countNodes.elementAt(size - 1) : org.apache.xml.dtm.DTM_Fields.NULL;
		  }
	  }
	}

}