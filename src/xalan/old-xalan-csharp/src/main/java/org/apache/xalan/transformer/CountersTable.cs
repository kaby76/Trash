using System;
using System.Collections;

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
 * $Id: CountersTable.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using ElemNumber = org.apache.xalan.templates.ElemNumber;
	using DTM = org.apache.xml.dtm.DTM;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// This is a table of counters, keyed by ElemNumber objects, each
	/// of which has a list of Counter objects.  This really isn't a true
	/// table, it is more like a list of lists (there must be a technical
	/// term for that...).
	/// @xsl.usage internal
	/// </summary>
	public class CountersTable : Hashtable
	{
		internal const long serialVersionUID = 2159100770924179875L;

	  /// <summary>
	  /// Construct a CountersTable.
	  /// </summary>
	  public CountersTable()
	  {
	  }

	  /// <summary>
	  /// Get the list of counters that corresponds to
	  /// the given ElemNumber object.
	  /// </summary>
	  /// <param name="numberElem"> the given xsl:number element.
	  /// </param>
	  /// <returns> the list of counters that corresponds to
	  /// the given ElemNumber object. </returns>
	  internal virtual ArrayList getCounters(ElemNumber numberElem)
	  {

		ArrayList counters = (ArrayList) this[numberElem];

		return (null == counters) ? putElemNumber(numberElem) : counters;
	  }

	  /// <summary>
	  /// Put a counter into the table and create an empty
	  /// vector as it's value.
	  /// </summary>
	  /// <param name="numberElem"> the given xsl:number element.
	  /// </param>
	  /// <returns> an empty vector to be used to store counts
	  /// for this number element. </returns>
	  internal virtual ArrayList putElemNumber(ElemNumber numberElem)
	  {

		ArrayList counters = new ArrayList();

		this[numberElem] = counters;

		return counters;
	  }

	  /// <summary>
	  /// Place to collect new counters.
	  /// </summary>
	  [NonSerialized]
	  private NodeSetDTM m_newFound;

	  /// <summary>
	  /// Add a list of counted nodes that were built in backwards document
	  /// order, or a list of counted nodes that are in forwards document
	  /// order.
	  /// </summary>
	  /// <param name="flist"> Vector of nodes built in forwards document order </param>
	  /// <param name="blist"> Vector of nodes built in backwards document order </param>
	  internal virtual void appendBtoFList(NodeSetDTM flist, NodeSetDTM blist)
	  {

		int n = blist.size();

		for (int i = (n - 1); i >= 0; i--)
		{
		  flist.addElement(blist.item(i));
		}
	  }

	  // For diagnostics

	  /// <summary>
	  /// Number of counters created so far </summary>
	  [NonSerialized]
	  internal int m_countersMade = 0;

	  /// <summary>
	  /// Count forward until the given node is found, or until
	  /// we have looked to the given amount.
	  /// </summary>
	  /// <param name="support"> The XPath context to use </param>
	  /// <param name="numberElem"> The given xsl:number element. </param>
	  /// <param name="node"> The node to count.
	  /// </param>
	  /// <returns> The node count, or 0 if not found.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int countNode(org.apache.xpath.XPathContext support, org.apache.xalan.templates.ElemNumber numberElem, int node) throws javax.xml.transform.TransformerException
	  public virtual int countNode(XPathContext support, ElemNumber numberElem, int node)
	  {

		int count = 0;
		ArrayList counters = getCounters(numberElem);
		int nCounters = counters.Count;

		// XPath countMatchPattern = numberElem.getCountMatchPattern(support, node);
		// XPath fromMatchPattern = numberElem.m_fromMatchPattern;
		int target = numberElem.getTargetNode(support, node);

		if (org.apache.xml.dtm.DTM_Fields.NULL != target)
		{
		  for (int i = 0; i < nCounters; i++)
		  {
			Counter counter = (Counter) counters[i];

			count = counter.getPreviouslyCounted(support, target);

			if (count > 0)
			{
			  return count;
			}
		  }

		  // In the loop below, we collect the nodes in backwards doc order, so 
		  // we don't have to do inserts, but then we store the nodes in forwards 
		  // document order, so we don't have to insert nodes into that list, 
		  // so that's what the appendBtoFList stuff is all about.  In cases 
		  // of forward counting by one, this will mean a single node copy from 
		  // the backwards list (m_newFound) to the forwards list (counter.m_countNodes).
		  count = 0;
		  if (m_newFound == null)
		  {
			m_newFound = new NodeSetDTM(support.DTMManager);
		  }

		  for (; org.apache.xml.dtm.DTM_Fields.NULL != target; target = numberElem.getPreviousNode(support, target))
		  {

			// First time in, we should not have to check for previous counts, 
			// since the original target node was already checked in the 
			// block above.
			if (0 != count)
			{
			  for (int i = 0; i < nCounters; i++)
			  {
				Counter counter = (Counter) counters[i];
				int cacheLen = counter.m_countNodes.size();

				if ((cacheLen > 0) && (counter.m_countNodes.elementAt(cacheLen - 1) == target))
				{
				  count += (cacheLen + counter.m_countNodesStartCount);

				  if (cacheLen > 0)
				  {
					appendBtoFList(counter.m_countNodes, m_newFound);
				  }

				  m_newFound.removeAllElements();

				  return count;
				}
			  }
			}

			m_newFound.addElement(target);

			count++;
		  }

		  // If we got to this point, then we didn't find a counter, so make 
		  // one and add it to the list.
		  Counter counter = new Counter(numberElem, new NodeSetDTM(support.DTMManager));

		  m_countersMade++; // for diagnostics

		  appendBtoFList(counter.m_countNodes, m_newFound);
		  m_newFound.removeAllElements();
		  counters.Add(counter);
		}

		return count;
	  }
	}

}