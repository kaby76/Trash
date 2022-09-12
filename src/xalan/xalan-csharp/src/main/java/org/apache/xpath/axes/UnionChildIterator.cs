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
 * $Id: UnionChildIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XObject = org.apache.xpath.objects.XObject;
	using NodeTest = org.apache.xpath.patterns.NodeTest;

	/// <summary>
	/// This class defines a simplified type of union iterator that only 
	/// tests along the child axes.  If the conditions are right, it is 
	/// much faster than using a UnionPathIterator.
	/// </summary>
	[Serializable]
	public class UnionChildIterator : ChildTestIterator
	{
		internal new const long serialVersionUID = 3500298482193003495L;
	  /// <summary>
	  /// Even though these may hold full LocPathIterators, this array does 
	  /// not have to be cloned, since only the node test and predicate 
	  /// portion are used, and these only need static information.  However, 
	  /// also note that index predicates can not be used!
	  /// </summary>
	  private PredicatedNodeTest[] m_nodeTests = null;

	  /// <summary>
	  /// Constructor for UnionChildIterator
	  /// </summary>
	  public UnionChildIterator() : base(null)
	  {
	  }

	  /// <summary>
	  /// Add a node test to the union list.
	  /// </summary>
	  /// <param name="test"> reference to a NodeTest, which will be added 
	  /// directly to the list of node tests (in other words, it will 
	  /// not be cloned).  The parent of this test will be set to 
	  /// this object. </param>
	  public virtual void addNodeTest(PredicatedNodeTest test)
	  {

		// Increase array size by only 1 at a time.  Fix this
		// if it looks to be a problem.
		if (null == m_nodeTests)
		{
		  m_nodeTests = new PredicatedNodeTest[1];
		  m_nodeTests[0] = test;
		}
		else
		{
		  PredicatedNodeTest[] tests = m_nodeTests;
		  int len = m_nodeTests.Length;

		  m_nodeTests = new PredicatedNodeTest[len + 1];

		  Array.Copy(tests, 0, m_nodeTests, 0, len);

		  m_nodeTests[len] = test;
		}
		test.exprSetParent(this);
	  }

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		base.fixupVariables(vars, globalsSize);
		if (m_nodeTests != null)
		{
		  for (int i = 0; i < m_nodeTests.Length; i++)
		  {
			m_nodeTests[i].fixupVariables(vars, globalsSize);
		  }
		}
	  }

	  /// <summary>
	  /// Test whether a specified node is visible in the logical view of a
	  /// TreeWalker or NodeIterator. This function will be called by the
	  /// implementation of TreeWalker and NodeIterator; it is not intended to
	  /// be called directly from user code. </summary>
	  /// <param name="n">  The node to check to see if it passes the filter or not. </param>
	  /// <returns>  a constant to determine whether the node is accepted,
	  ///   rejected, or skipped, as defined  above . </returns>
	  public override short acceptNode(int n)
	  {
		XPathContext xctxt = XPathContext;
		try
		{
		  xctxt.pushCurrentNode(n);
		  for (int i = 0; i < m_nodeTests.Length; i++)
		  {
			PredicatedNodeTest pnt = m_nodeTests[i];
			XObject score = pnt.execute(xctxt, n);
			if (score != NodeTest.SCORE_NONE)
			{
			  // Note that we are assuming there are no positional predicates!
			  if (pnt.PredicateCount > 0)
			  {
				if (pnt.executePredicates(n, xctxt))
				{
				  return org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT;
				}
			  }
			  else
			  {
				return org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT;
			  }

			}
		  }
		}
		catch (javax.xml.transform.TransformerException se)
		{

		  // TODO: Fix this.
		  throw new Exception(se.Message);
		}
		finally
		{
		  xctxt.popCurrentNode();
		}
		return org.apache.xml.dtm.DTMIterator_Fields.FILTER_SKIP;
	  }

	}

}