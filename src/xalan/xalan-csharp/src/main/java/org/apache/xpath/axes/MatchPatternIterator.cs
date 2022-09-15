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
 * $Id: MatchPatternIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XPathContext = org.apache.xpath.XPathContext;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpMap = org.apache.xpath.compiler.OpMap;
	using XObject = org.apache.xpath.objects.XObject;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using StepPattern = org.apache.xpath.patterns.StepPattern;

	/// <summary>
	/// This class treats a 
	/// <a href="http://www.w3.org/TR/xpath#location-paths">LocationPath</a> as a 
	/// filtered iteration over the tree, evaluating each node in a super axis 
	/// traversal against the LocationPath interpreted as a match pattern.  This 
	/// class is useful to find nodes in document order that are complex paths 
	/// whose steps probably criss-cross each other.
	/// </summary>
	[Serializable]
	public class MatchPatternIterator : LocPathIterator
	{
		internal new const long serialVersionUID = -5201153767396296474L;

	  /// <summary>
	  /// This is the select pattern, translated into a match pattern. </summary>
	  protected internal StepPattern m_pattern;

	  /// <summary>
	  /// The traversal axis from where the nodes will be filtered. </summary>
	  protected internal int m_superAxis = -1;

	  /// <summary>
	  /// The DTM inner traversal class, that corresponds to the super axis. </summary>
	  protected internal DTMAxisTraverser m_traverser;

	  /// <summary>
	  /// DEBUG flag for diagnostic dumps. </summary>
	  private const bool DEBUG = false;

	//  protected int m_nsElemBase = DTM.NULL;

	  /// <summary>
	  /// Create a LocPathIterator object, including creation
	  /// of step walkers from the opcode list, and call back
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the
	  /// opcode list from the compiler. </param>
	  /// <param name="analysis"> Analysis bits that give general information about the 
	  /// LocationPath.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: MatchPatternIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal MatchPatternIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis, false)
	  {


		int firstStepPos = OpMap.getFirstChildPos(opPos);

		m_pattern = WalkerFactory.loadSteps(this, compiler, firstStepPos, 0);

		bool fromRoot = false;
		bool walkBack = false;
		bool walkDescendants = false;
		bool walkAttributes = false;

		if (0 != (analysis & (WalkerFactory.BIT_ROOT | WalkerFactory.BIT_ANY_DESCENDANT_FROM_ROOT)))
		{
		  fromRoot = true;
		}

		if (0 != (analysis & (WalkerFactory.BIT_ANCESTOR | WalkerFactory.BIT_ANCESTOR_OR_SELF | WalkerFactory.BIT_PRECEDING | WalkerFactory.BIT_PRECEDING_SIBLING | WalkerFactory.BIT_FOLLOWING | WalkerFactory.BIT_FOLLOWING_SIBLING | WalkerFactory.BIT_PARENT | WalkerFactory.BIT_FILTER)))
		{
		  walkBack = true;
		}

		if (0 != (analysis & (WalkerFactory.BIT_DESCENDANT_OR_SELF | WalkerFactory.BIT_DESCENDANT | WalkerFactory.BIT_CHILD)))
		{
		  walkDescendants = true;
		}

		if (0 != (analysis & (WalkerFactory.BIT_ATTRIBUTE | WalkerFactory.BIT_NAMESPACE)))
		{
		  walkAttributes = true;
		}

		if (false || DEBUG)
		{
		  Console.Write("analysis: " + Convert.ToString(analysis, 2));
		  Console.WriteLine(", " + WalkerFactory.getAnalysisString(analysis));
		}

		if (fromRoot || walkBack)
		{
		  if (walkAttributes)
		  {
			m_superAxis = Axis.ALL;
		  }
		  else
		  {
			m_superAxis = Axis.DESCENDANTSFROMROOT;
		  }
		}
		else if (walkDescendants)
		{
		  if (walkAttributes)
		  {
			m_superAxis = Axis.ALLFROMNODE;
		  }
		  else
		  {
			m_superAxis = Axis.DESCENDANTORSELF;
		  }
		}
		else
		{
		  m_superAxis = Axis.ALL;
		}
		if (false || DEBUG)
		{
		  Console.WriteLine("axis: " + Axis.getNames(m_superAxis));
		}

	  }


	  /// <summary>
	  /// Initialize the context values for this expression
	  /// after it is cloned.
	  /// </summary>
	  /// <param name="context"> The XPath runtime context for this
	  /// transformation. </param>
	  public override void setRoot(int context, object environment)
	  {
		base.setRoot(context, environment);
		m_traverser = m_cdtm.getAxisTraverser(m_superAxis);
	  }

	  /// <summary>
	  ///  Detaches the iterator from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state. After<code>detach</code> has been invoked, calls to
	  /// <code>nextNode</code> or<code>previousNode</code> will raise the
	  /// exception INVALID_STATE_ERR.
	  /// </summary>
	  public override void detach()
	  {
		if (m_allowDetach)
		{
		  m_traverser = null;

		  // Always call the superclass detach last!
		  base.detach();
		}
	  }

	  /// <summary>
	  /// Get the next node via getNextXXX.  Bottlenecked for derived class override. </summary>
	  /// <returns> The next node on the axis, or DTM.NULL. </returns>
	  protected internal virtual int NextNode
	  {
		  get
		  {
			m_lastFetched = (DTM.NULL == m_lastFetched) ? m_traverser.first(m_context) : m_traverser.next(m_context, m_lastFetched);
			return m_lastFetched;
		  }
	  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a NodeIterator is created, the first call
	  /// to nextNode() returns the first node in the set. </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set. </returns>
	  public override int nextNode()
	  {
		  if (m_foundLast)
		  {
			  return DTM.NULL;
		  }

		int next;

		org.apache.xpath.VariableStack vars;
		int savedStart;
		if (-1 != m_stackFrame)
		{
		  vars = m_execContext.VarStack;

		  // These three statements need to be combined into one operation.
		  savedStart = vars.StackFrame;

		  vars.StackFrame = m_stackFrame;
		}
		else
		{
		  // Yuck.  Just to shut up the compiler!
		  vars = null;
		  savedStart = 0;
		}

		try
		{
		  if (DEBUG)
		  {
			Console.WriteLine("m_pattern" + m_pattern.ToString());
		  }

		  do
		  {
			next = NextNode;

			if (DTM.NULL != next)
			{
			  if (DTMIterator.FILTER_ACCEPT == acceptNode(next, m_execContext))
			  {
				break;
			  }
			  else
			  {
				continue;
			  }
			}
			else
			{
			  break;
			}
		  } while (next != DTM.NULL);

		  if (DTM.NULL != next)
		  {
			if (DEBUG)
			{
			  Console.WriteLine("next: " + next);
			  Console.WriteLine("name: " + m_cdtm.getNodeName(next));
			}
			incrementCurrentPos();

			return next;
		  }
		  else
		  {
			m_foundLast = true;

			return DTM.NULL;
		  }
		}
		finally
		{
		  if (-1 != m_stackFrame)
		  {
			// These two statements need to be combined into one operation.
			vars.StackFrame = savedStart;
		  }
		}

	  }

	  /// <summary>
	  ///  Test whether a specified node is visible in the logical view of a
	  /// TreeWalker or NodeIterator. This function will be called by the
	  /// implementation of TreeWalker and NodeIterator; it is not intended to
	  /// be called directly from user code. </summary>
	  /// <param name="n">  The node to check to see if it passes the filter or not. </param>
	  /// <returns>  a constant to determine whether the node is accepted,
	  ///   rejected, or skipped, as defined  above . </returns>
	  public virtual short acceptNode(int n, XPathContext xctxt)
	  {

		try
		{
		  xctxt.pushCurrentNode(n);
		  xctxt.pushIteratorRoot(m_context);
		  if (DEBUG)
		  {
			Console.WriteLine("traverser: " + m_traverser);
			Console.Write("node: " + n);
			Console.WriteLine(", " + m_cdtm.getNodeName(n));
			// if(m_cdtm.getNodeName(n).equals("near-east"))
			Console.WriteLine("pattern: " + m_pattern.ToString());
			m_pattern.debugWhatToShow(m_pattern.WhatToShow);
		  }

		  XObject score = m_pattern.execute(xctxt);

		  if (DEBUG)
		  {
			// System.out.println("analysis: "+Integer.toBinaryString(m_analysis));
			Console.WriteLine("score: " + score);
			Console.WriteLine("skip: " + (score == NodeTest.SCORE_NONE));
		  }

		  // System.out.println("\n::acceptNode - score: "+score.num()+"::");
		  return (score == NodeTest.SCORE_NONE) ? DTMIterator.FILTER_SKIP : DTMIterator.FILTER_ACCEPT;
		}
		catch (javax.xml.transform.TransformerException se)
		{

		  // TODO: Fix this.
		  throw new Exception(se.Message);
		}
		finally
		{
		  xctxt.popCurrentNode();
		  xctxt.popIteratorRoot();
		}

	  }

	}

}