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
 * $Id: ChildIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using XPathContext = org.apache.xpath.XPathContext;
	using Compiler = org.apache.xpath.compiler.Compiler;

	/// <summary>
	/// This class implements an optimized iterator for
	/// "node()" patterns, that is, any children of the
	/// context node. </summary>
	/// <seealso cref="org.apache.xpath.axes.LocPathIterator"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ChildIterator : LocPathIterator
	{
		internal new const long serialVersionUID = -6935428015142993583L;

	  /// <summary>
	  /// Create a ChildIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator. </param>
	  /// <param name="analysis"> Analysis bits of the entire pattern.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: ChildIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal ChildIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis, false)
	  {

		// This iterator matches all kinds of nodes
		initNodeTest(DTMFilter.SHOW_ALL);
	  }

	  /// <summary>
	  /// Return the first node out of the nodeset, if this expression is 
	  /// a nodeset expression.  This is the default implementation for 
	  /// nodesets.
	  /// <para>WARNING: Do not mutate this class from this function!</para> </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> the first node out of the nodeset, or DTM.NULL. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int asNode(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override int asNode(XPathContext xctxt)
	  {
		int current = xctxt.CurrentNode;

		DTM dtm = xctxt.getDTM(current);

		return dtm.getFirstChild(current);
	  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a NodeIterator is created, the first call
	  /// to nextNode() returns the first node in the set.
	  /// </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set. </returns>
	  public override int nextNode()
	  {
		  if (m_foundLast)
		  {
			  return DTM.NULL;
		  }

		int next;

		m_lastFetched = next = (DTM.NULL == m_lastFetched) ? m_cdtm.getFirstChild(m_context) : m_cdtm.getNextSibling(m_lastFetched);

		// m_lastFetched = next;
		if (DTM.NULL != next)
		{
		  m_pos++;
		  return next;
		}
		else
		{
		  m_foundLast = true;

		  return DTM.NULL;
		}
	  }

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public override int Axis
	  {
		  get
		  {
			return org.apache.xml.dtm.Axis.CHILD;
		  }
	  }


	}

}