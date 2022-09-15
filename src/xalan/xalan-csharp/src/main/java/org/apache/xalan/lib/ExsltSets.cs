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
 * $Id: ExsltSets.java 469688 2006-10-31 22:39:43Z minchau $
 */
namespace org.apache.xalan.lib
{

	using DOMHelper = org.apache.xml.utils.DOMHelper;
	using NodeSet = org.apache.xpath.NodeSet;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// This class contains EXSLT set extension functions.
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:set="http://exslt.org/sets"
	/// </pre>
	/// 
	/// The documentation for each function has been copied from the relevant
	/// EXSLT Implementer page. 
	/// </summary>
	/// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"
	/// @xsl.usage general/>
	public class ExsltSets : ExsltBase
	{
	  /// <summary>
	  /// The set:leading function returns the nodes in the node set passed as the first argument that
	  /// precede, in document order, the first node in the node set passed as the second argument. If
	  /// the first node in the second node set is not contained in the first node set, then an empty
	  /// node set is returned. If the second node set is empty, then the first node set is returned.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set. </param>
	  /// <param name="nl2"> NodeList for second node-set. </param>
	  /// <returns> a NodeList containing the nodes in nl1 that precede in document order the first
	  /// node in nl2; an empty node-set if the first node in nl2 is not in nl1; all of nl1 if nl2
	  /// is empty.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static NodeList leading(NodeList nl1, NodeList nl2)
	  {
		if (nl2.getLength() == 0)
		{
		  return nl1;
		}

		NodeSet ns1 = new NodeSet(nl1);
		NodeSet leadNodes = new NodeSet();
		Node endNode = nl2.item(0);
		if (!ns1.contains(endNode))
		{
		  return leadNodes; // empty NodeSet
		}

		for (int i = 0; i < nl1.getLength(); i++)
		{
		  Node testNode = nl1.item(i);
		  if (DOMHelper.isNodeAfter(testNode, endNode) && !DOMHelper.isNodeTheSame(testNode, endNode))
		  {
			leadNodes.addElement(testNode);
		  }
		}
		return leadNodes;
	  }

	  /// <summary>
	  /// The set:trailing function returns the nodes in the node set passed as the first argument that 
	  /// follow, in document order, the first node in the node set passed as the second argument. If 
	  /// the first node in the second node set is not contained in the first node set, then an empty 
	  /// node set is returned. If the second node set is empty, then the first node set is returned. 
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set. </param>
	  /// <param name="nl2"> NodeList for second node-set. </param>
	  /// <returns> a NodeList containing the nodes in nl1 that follow in document order the first
	  /// node in nl2; an empty node-set if the first node in nl2 is not in nl1; all of nl1 if nl2
	  /// is empty.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static NodeList trailing(NodeList nl1, NodeList nl2)
	  {
		if (nl2.getLength() == 0)
		{
		  return nl1;
		}

		NodeSet ns1 = new NodeSet(nl1);
		NodeSet trailNodes = new NodeSet();
		Node startNode = nl2.item(0);
		if (!ns1.contains(startNode))
		{
		  return trailNodes; // empty NodeSet
		}

		for (int i = 0; i < nl1.getLength(); i++)
		{
		  Node testNode = nl1.item(i);
		  if (DOMHelper.isNodeAfter(startNode, testNode) && !DOMHelper.isNodeTheSame(startNode, testNode))
		  {
			trailNodes.addElement(testNode);
		  }
		}
		return trailNodes;
	  }

	  /// <summary>
	  /// The set:intersection function returns a node set comprising the nodes that are within 
	  /// both the node sets passed as arguments to it.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set. </param>
	  /// <param name="nl2"> NodeList for second node-set. </param>
	  /// <returns> a NodeList containing the nodes in nl1 that are also
	  /// in nl2.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static NodeList intersection(NodeList nl1, NodeList nl2)
	  {
		NodeSet ns1 = new NodeSet(nl1);
		NodeSet ns2 = new NodeSet(nl2);
		NodeSet inter = new NodeSet();

		inter.ShouldCacheNodes = true;

		for (int i = 0; i < ns1.Length; i++)
		{
		  Node n = ns1.elementAt(i);

		  if (ns2.contains(n))
		  {
			inter.addElement(n);
		  }
		}

		return inter;
	  }

	  /// <summary>
	  /// The set:difference function returns the difference between two node sets - those nodes that 
	  /// are in the node set passed as the first argument that are not in the node set passed as the 
	  /// second argument.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set. </param>
	  /// <param name="nl2"> NodeList for second node-set. </param>
	  /// <returns> a NodeList containing the nodes in nl1 that are not in nl2.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static NodeList difference(NodeList nl1, NodeList nl2)
	  {
		NodeSet ns1 = new NodeSet(nl1);
		NodeSet ns2 = new NodeSet(nl2);

		NodeSet diff = new NodeSet();

		diff.ShouldCacheNodes = true;

		for (int i = 0; i < ns1.Length; i++)
		{
		  Node n = ns1.elementAt(i);

		  if (!ns2.contains(n))
		  {
			diff.addElement(n);
		  }
		}

		return diff;
	  }

	  /// <summary>
	  /// The set:distinct function returns a subset of the nodes contained in the node-set NS passed 
	  /// as the first argument. Specifically, it selects a node N if there is no node in NS that has 
	  /// the same string value as N, and that precedes N in document order. 
	  /// </summary>
	  /// <param name="nl"> NodeList for the node-set. </param>
	  /// <returns> a NodeList with nodes from nl containing distinct string values.
	  /// In other words, if more than one node in nl contains the same string value,
	  /// only include the first such node found.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static NodeList distinct(NodeList nl)
	  {
		NodeSet dist = new NodeSet();
		dist.ShouldCacheNodes = true;

		System.Collections.IDictionary stringTable = new Hashtable();

		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node currNode = nl.item(i);
		  string key = toString(currNode);

		  if (string.ReferenceEquals(key, null))
		  {
			dist.addElement(currNode);
		  }
		  else if (!stringTable.Contains(key))
		  {
			stringTable[key] = currNode;
			dist.addElement(currNode);
		  }
		}

		return dist;
	  }

	  /// <summary>
	  /// The set:has-same-node function returns true if the node set passed as the first argument shares 
	  /// any nodes with the node set passed as the second argument. If there are no nodes that are in both
	  /// node sets, then it returns false. 
	  /// 
	  /// The Xalan extensions MethodResolver converts 'has-same-node' to 'hasSameNode'.
	  /// 
	  /// Note: Not to be confused with hasSameNodes in the Xalan namespace, which returns true if
	  /// the two node sets contain the exactly the same nodes (perhaps in a different order), 
	  /// otherwise false.
	  /// </summary>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static bool hasSameNode(NodeList nl1, NodeList nl2)
	  {

		NodeSet ns1 = new NodeSet(nl1);
		NodeSet ns2 = new NodeSet(nl2);

		for (int i = 0; i < ns1.Length; i++)
		{
		  if (ns2.contains(ns1.elementAt(i)))
		  {
			return true;
		  }
		}
		return false;
	  }

	}

}