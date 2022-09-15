using System;
using System.Collections;
using System.Text;

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
 * $Id: StepPattern.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.patterns
{
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using SubContextList = org.apache.xpath.axes.SubContextList;
	using PsuedoNames = org.apache.xpath.compiler.PsuedoNames;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class represents a single pattern match step.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class StepPattern : NodeTest, SubContextList, ExpressionOwner
	{
		internal new const long serialVersionUID = 9071668960168152644L;

	  /// <summary>
	  /// The axis for this test. </summary>
	  protected internal int m_axis;

	  /// <summary>
	  /// Construct a StepPattern that tests for namespaces and node names.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  /// <param name="namespace"> The namespace to be tested. </param>
	  /// <param name="name"> The local name to be tested. </param>
	  /// <param name="axis"> The Axis for this test, one of of Axes.ANCESTORORSELF, etc. </param>
	  /// <param name="axisForPredicate"> No longer used. </param>
	  public StepPattern(int whatToShow, string @namespace, string name, int axis, int axisForPredicate) : base(whatToShow, @namespace, name)
	  {


		m_axis = axis;
	  }

	  /// <summary>
	  /// Construct a StepPattern that doesn't test for node names.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  /// <param name="axis"> The Axis for this test, one of of Axes.ANCESTORORSELF, etc. </param>
	  /// <param name="axisForPredicate"> No longer used. </param>
	  public StepPattern(int whatToShow, int axis, int axisForPredicate) : base(whatToShow)
	  {


		m_axis = axis;
	  }

	  /// <summary>
	  /// The target local name or psuedo name, for hash table lookup optimization.
	  ///  @serial
	  /// </summary>
	  internal string m_targetString; // only calculate on head

	  /// <summary>
	  /// Calculate the local name or psuedo name of the node that this pattern will test,
	  /// for hash table lookup optimization.
	  /// </summary>
	  /// <seealso cref="org.apache.xpath.compiler.PsuedoNames"/>
	  public virtual void calcTargetString()
	  {

		int whatToShow = WhatToShow;

		switch (whatToShow)
		{
		case DTMFilter.SHOW_COMMENT :
		  m_targetString = PsuedoNames.PSEUDONAME_COMMENT;
		  break;
		case DTMFilter.SHOW_TEXT :
		case DTMFilter.SHOW_CDATA_SECTION :
		case (DTMFilter.SHOW_TEXT | DTMFilter.SHOW_CDATA_SECTION) :
		  m_targetString = PsuedoNames.PSEUDONAME_TEXT;
		  break;
		case DTMFilter.SHOW_ALL :
		  m_targetString = PsuedoNames.PSEUDONAME_ANY;
		  break;
		case DTMFilter.SHOW_DOCUMENT :
		case DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT :
		  m_targetString = PsuedoNames.PSEUDONAME_ROOT;
		  break;
		case DTMFilter.SHOW_ELEMENT :
		  if (string.ReferenceEquals(WILD, m_name))
		  {
			m_targetString = PsuedoNames.PSEUDONAME_ANY;
		  }
		  else
		  {
			m_targetString = m_name;
		  }
		  break;
		default :
		  m_targetString = PsuedoNames.PSEUDONAME_ANY;
		  break;
		}
	  }

	  /// <summary>
	  /// Get the local name or psuedo name of the node that this pattern will test,
	  /// for hash table lookup optimization.
	  /// 
	  /// </summary>
	  /// <returns> local name or psuedo name of the node. </returns>
	  /// <seealso cref="org.apache.xpath.compiler.PsuedoNames"/>
	  public virtual string TargetString
	  {
		  get
		  {
			return m_targetString;
		  }
	  }

	  /// <summary>
	  /// Reference to nodetest and predicate for
	  /// parent or ancestor.
	  /// @serial
	  /// </summary>
	  internal StepPattern m_relativePathPattern;

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list
	  /// should be searched backwards for the first qualified name that
	  /// corresponds to the variable reference qname.  The position of the
	  /// QName in the vector from the start of the vector will be its position
	  /// in the stack frame (but variables above the globalsTop value will need
	  /// to be offset to the current stack frame). </param>
	  /// <param name="globalsSize"> The number of variables in the global variable area. </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {

		base.fixupVariables(vars, globalsSize);

		if (null != m_predicates)
		{
		  for (int i = 0; i < m_predicates.Length; i++)
		  {
			m_predicates[i].fixupVariables(vars, globalsSize);
		  }
		}

		if (null != m_relativePathPattern)
		{
		  m_relativePathPattern.fixupVariables(vars, globalsSize);
		}
	  }

	  /// <summary>
	  /// Set the reference to nodetest and predicate for
	  /// parent or ancestor.
	  /// 
	  /// </summary>
	  /// <param name="expr"> The relative pattern expression. </param>
	  public virtual StepPattern RelativePathPattern
	  {
		  set
		  {
    
			m_relativePathPattern = value;
			value.exprSetParent(this);
    
			calcScore();
		  }
		  get
		  {
			return m_relativePathPattern;
		  }
	  }


	  //  /**
	  //   * Set the list of predicate expressions for this pattern step.
	  //   * @param predicates List of expression objects.
	  //   */
	  //  public void setPredicates(Expression[] predicates)
	  //  {
	  //    m_predicates = predicates;
	  //  }

	  /// <summary>
	  /// Set the list of predicate expressions for this pattern step. </summary>
	  /// <returns> List of expression objects. </returns>
	  public virtual Expression[] Predicates
	  {
		  get
		  {
			return m_predicates;
		  }
		  set
		  {
    
			m_predicates = value;
			if (null != value)
			{
				for (int i = 0; i < value.Length; i++)
				{
					value[i].exprSetParent(this);
				}
			}
    
			calcScore();
		  }
	  }

	  /// <summary>
	  /// The list of predicate expressions for this pattern step.
	  ///  @serial
	  /// </summary>
	  internal Expression[] m_predicates;

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// 
	  /// NOTE: Ancestors tests with predicates are problematic, and will require
	  /// special treatment.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public override bool canTraverseOutsideSubtree()
	  {

		int n = PredicateCount;

		for (int i = 0; i < n; i++)
		{
		  if (getPredicate(i).canTraverseOutsideSubtree())
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Get a predicate expression.
	  /// 
	  /// </summary>
	  /// <param name="i"> The index of the predicate.
	  /// </param>
	  /// <returns> A predicate expression. </returns>
	  public virtual Expression getPredicate(int i)
	  {
		return m_predicates[i];
	  }

	  /// <summary>
	  /// Get the number of predicates for this match pattern step.
	  /// 
	  /// </summary>
	  /// <returns> the number of predicates for this match pattern step. </returns>
	  public int PredicateCount
	  {
		  get
		  {
			return (null == m_predicates) ? 0 : m_predicates.Length;
		  }
	  }


	  /// <summary>
	  /// Static calc of match score.
	  /// </summary>
	  public override void calcScore()
	  {

		if ((PredicateCount > 0) || (null != m_relativePathPattern))
		{
		  m_score = SCORE_OTHER;
		}
		else
		{
		  base.calcScore();
		}

		if (null == m_targetString)
		{
		  calcTargetString();
		}
	  }

	  /// <summary>
	  /// Execute this pattern step, including predicates.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context. </param>
	  /// <param name="currentNode"> The current node context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int currentNode) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int currentNode)
	  {

		DTM dtm = xctxt.getDTM(currentNode);

		if (dtm != null)
		{
		  int expType = dtm.getExpandedTypeID(currentNode);

		  return execute(xctxt, currentNode, dtm, expType);
		}

		return NodeTest.SCORE_NONE;
	  }

	  /// <summary>
	  /// Execute this pattern step, including predicates.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		return execute(xctxt, xctxt.CurrentNode);
	  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="currentNode"> The currentNode. </param>
	  /// <param name="dtm"> The DTM of the current node. </param>
	  /// <param name="expType"> The expanded type ID of the current node.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int currentNode, org.apache.xml.dtm.DTM dtm, int expType) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int currentNode, DTM dtm, int expType)
	  {

		if (m_whatToShow == NodeTest.SHOW_BYFUNCTION)
		{
		  if (null != m_relativePathPattern)
		  {
			return m_relativePathPattern.execute(xctxt);
		  }
		  else
		  {
			return NodeTest.SCORE_NONE;
		  }
		}

		XObject score;

		score = base.execute(xctxt, currentNode, dtm, expType);

		if (score == NodeTest.SCORE_NONE)
		{
		  return NodeTest.SCORE_NONE;
		}

		if (PredicateCount != 0)
		{
		  if (!executePredicates(xctxt, dtm, currentNode))
		  {
			return NodeTest.SCORE_NONE;
		  }
		}

		if (null != m_relativePathPattern)
		{
		  return m_relativePathPattern.executeRelativePathPattern(xctxt, dtm, currentNode);
		}

		return score;
	  }

	  /// <summary>
	  /// New Method to check whether the current node satisfies a position predicate
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="predPos"> Which predicate we're evaluating of foo[1][2][3]. </param>
	  /// <param name="dtm"> The DTM of the current node. </param>
	  /// <param name="context"> The currentNode. </param>
	  /// <param name="pos"> The position being requested, i.e. the value returned by 
	  ///            m_predicates[predPos].execute(xctxt).
	  /// </param>
	  /// <returns> true of the position of the context matches pos, false otherwise. </returns>
	  private bool checkProximityPosition(XPathContext xctxt, int predPos, DTM dtm, int context, int pos)
	  {

		try
		{
		  DTMAxisTraverser traverser = dtm.getAxisTraverser(Axis.PRECEDINGSIBLING);

		  for (int child = traverser.first(context); DTM.NULL != child; child = traverser.next(context, child))
		  {
			try
			{
			  xctxt.pushCurrentNode(child);

			  if (NodeTest.SCORE_NONE != base.execute(xctxt, child))
			  {
				bool pass = true;

				try
				{
				  xctxt.pushSubContextList(this);

				  for (int i = 0; i < predPos; i++)
				  {
					xctxt.pushPredicatePos(i);
					try
					{
					  XObject pred = m_predicates[i].execute(xctxt);

					  try
					  {
						if (XObject.CLASS_NUMBER == pred.Type)
						{
						  throw new Exception("Why: Should never have been called");
						}
						else if (!pred.boolWithSideEffects())
						{
						  pass = false;

						  break;
						}
					  }
					  finally
					  {
						pred.detach();
					  }
					}
					finally
					{
					  xctxt.popPredicatePos();
					}
				  }
				}
				finally
				{
				  xctxt.popSubContextList();
				}

				if (pass)
				{
				  pos--;
				}

				if (pos < 1)
				{
				  return false;
				}
			  }
			}
			finally
			{
			  xctxt.popCurrentNode();
			}
		  }
		}
		catch (javax.xml.transform.TransformerException se)
		{

		  // TODO: should keep throw sax exception...
		  throw new Exception(se.Message);
		}

		return (pos == 1);
	  }

	  /// <summary>
	  /// Get the proximity position index of the current node based on this
	  /// node test.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context. </param>
	  /// <param name="predPos"> Which predicate we're evaluating of foo[1][2][3]. </param>
	  /// <param name="findLast"> If true, don't terminate when the context node is found.
	  /// </param>
	  /// <returns> the proximity position index of the current node based on the
	  ///         node test. </returns>
	  private int getProximityPosition(XPathContext xctxt, int predPos, bool findLast)
	  {

		int pos = 0;
		int context = xctxt.CurrentNode;
		DTM dtm = xctxt.getDTM(context);
		int parent = dtm.getParent(context);

		try
		{
		  DTMAxisTraverser traverser = dtm.getAxisTraverser(Axis.CHILD);

		  for (int child = traverser.first(parent); DTM.NULL != child; child = traverser.next(parent, child))
		  {
			try
			{
			  xctxt.pushCurrentNode(child);

			  if (NodeTest.SCORE_NONE != base.execute(xctxt, child))
			  {
				bool pass = true;

				try
				{
				  xctxt.pushSubContextList(this);

				  for (int i = 0; i < predPos; i++)
				  {
					xctxt.pushPredicatePos(i);
					try
					{
					  XObject pred = m_predicates[i].execute(xctxt);

					  try
					  {
						if (XObject.CLASS_NUMBER == pred.Type)
						{
						  if ((pos + 1) != (int) pred.numWithSideEffects())
						  {
							pass = false;

							break;
						  }
						}
						else if (!pred.boolWithSideEffects())
						{
						  pass = false;

						  break;
						}
					  }
					  finally
					  {
						pred.detach();
					  }
					}
					finally
					{
					  xctxt.popPredicatePos();
					}
				  }
				}
				finally
				{
				  xctxt.popSubContextList();
				}

				if (pass)
				{
				  pos++;
				}

				if (!findLast && child == context)
				{
				  return pos;
				}
			  }
			}
			finally
			{
			  xctxt.popCurrentNode();
			}
		  }
		}
		catch (javax.xml.transform.TransformerException se)
		{

		  // TODO: should keep throw sax exception...
		  throw new Exception(se.Message);
		}

		return pos;
	  }

	  /// <summary>
	  /// Get the proximity position index of the current node based on this
	  /// node test.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the proximity position index of the current node based on the
	  ///         node test. </returns>
	  public virtual int getProximityPosition(XPathContext xctxt)
	  {
		return getProximityPosition(xctxt, xctxt.PredicatePos, false);
	  }

	  /// <summary>
	  /// Get the count of the nodes that match the test, which is the proximity
	  /// position of the last node that can pass this test in the sub context
	  /// selection.  In XSLT 1-based indexing, this count is the index of the last
	  /// node.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the count of the nodes that match the test. </returns>
	  public virtual int getLastPos(XPathContext xctxt)
	  {
		return getProximityPosition(xctxt, xctxt.PredicatePos, true);
	  }

	  /// <summary>
	  /// Execute the match pattern step relative to another step.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="dtm"> The DTM of the current node. </param>
	  /// <param name="currentNode"> The current node context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final org.apache.xpath.objects.XObject executeRelativePathPattern(org.apache.xpath.XPathContext xctxt, org.apache.xml.dtm.DTM dtm, int currentNode) throws javax.xml.transform.TransformerException
	  protected internal XObject executeRelativePathPattern(XPathContext xctxt, DTM dtm, int currentNode)
	  {

		XObject score = NodeTest.SCORE_NONE;
		int context = currentNode;
		DTMAxisTraverser traverser;

		traverser = dtm.getAxisTraverser(m_axis);

		for (int relative = traverser.first(context); DTM.NULL != relative; relative = traverser.next(context, relative))
		{
		  try
		  {
			xctxt.pushCurrentNode(relative);

			score = execute(xctxt);

			if (score != NodeTest.SCORE_NONE)
			{
			  break;
			}
		  }
		  finally
		  {
			xctxt.popCurrentNode();
		  }
		}

		return score;
	  }

	  /// <summary>
	  /// Execute the predicates on this step to determine if the current node 
	  /// should be filtered or accepted.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="dtm"> The DTM of the current node. </param>
	  /// <param name="currentNode"> The current node context.
	  /// </param>
	  /// <returns> true if the node should be accepted, false otherwise.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final boolean executePredicates(org.apache.xpath.XPathContext xctxt, org.apache.xml.dtm.DTM dtm, int currentNode) throws javax.xml.transform.TransformerException
	  protected internal bool executePredicates(XPathContext xctxt, DTM dtm, int currentNode)
	  {

		bool result = true;
		bool positionAlreadySeen = false;
		int n = PredicateCount;

		try
		{
		  xctxt.pushSubContextList(this);

		  for (int i = 0; i < n; i++)
		  {
			xctxt.pushPredicatePos(i);

			try
			{
			  XObject pred = m_predicates[i].execute(xctxt);

			  try
			  {
				if (XObject.CLASS_NUMBER == pred.Type)
				{
				  int pos = (int) pred.num();

				  if (positionAlreadySeen)
				  {
					result = (pos == 1);

					break;
				  }
				  else
				  {
					positionAlreadySeen = true;

					if (!checkProximityPosition(xctxt, i, dtm, currentNode, pos))
					{
					  result = false;

					  break;
					}
				  }

				}
				else if (!pred.boolWithSideEffects())
				{
				  result = false;

				  break;
				}
			  }
			  finally
			  {
				pred.detach();
			  }
			}
			finally
			{
			  xctxt.popPredicatePos();
			}
		  }
		}
		finally
		{
		  xctxt.popSubContextList();
		}

		return result;
	  }

	  /// <summary>
	  /// Get the string represenentation of this step for diagnostic purposes.
	  /// 
	  /// </summary>
	  /// <returns> A string representation of this step, built by reverse-engineering 
	  /// the contained info. </returns>
	  public override string ToString()
	  {

		StringBuilder buf = new StringBuilder();

		for (StepPattern pat = this; pat != null; pat = pat.m_relativePathPattern)
		{
		  if (pat != this)
		  {
			buf.Append("/");
		  }

		  buf.Append(Axis.getNames(pat.m_axis));
		  buf.Append("::");

		  if (0x000005000 == pat.m_whatToShow)
		  {
			buf.Append("doc()");
		  }
		  else if (DTMFilter.SHOW_BYFUNCTION == pat.m_whatToShow)
		  {
			buf.Append("function()");
		  }
		  else if (DTMFilter.SHOW_ALL == pat.m_whatToShow)
		  {
			buf.Append("node()");
		  }
		  else if (DTMFilter.SHOW_TEXT == pat.m_whatToShow)
		  {
			buf.Append("text()");
		  }
		  else if (DTMFilter.SHOW_PROCESSING_INSTRUCTION == pat.m_whatToShow)
		  {
			buf.Append("processing-instruction(");

			if (null != pat.m_name)
			{
			  buf.Append(pat.m_name);
			}

			buf.Append(")");
		  }
		  else if (DTMFilter.SHOW_COMMENT == pat.m_whatToShow)
		  {
			buf.Append("comment()");
		  }
		  else if (null != pat.m_name)
		  {
			if (DTMFilter.SHOW_ATTRIBUTE == pat.m_whatToShow)
			{
			  buf.Append("@");
			}

			if (null != pat.m_namespace)
			{
			  buf.Append("{");
			  buf.Append(pat.m_namespace);
			  buf.Append("}");
			}

			buf.Append(pat.m_name);
		  }
		  else if (DTMFilter.SHOW_ATTRIBUTE == pat.m_whatToShow)
		  {
			buf.Append("@");
		  }
		  else if ((DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT) == pat.m_whatToShow)
		  {
			buf.Append("doc-root()");
		  }
		  else
		  {
			buf.Append("?" + Convert.ToString(pat.m_whatToShow, 16));
		  }

		  if (null != pat.m_predicates)
		  {
			for (int i = 0; i < pat.m_predicates.Length; i++)
			{
			  buf.Append("[");
			  buf.Append(pat.m_predicates[i]);
			  buf.Append("]");
			}
		  }
		}

		return buf.ToString();
	  }

	  /// <summary>
	  /// Set to true to send diagnostics about pattern matches to the consol. </summary>
	  private const bool DEBUG_MATCHES = false;

	  /// <summary>
	  /// Get the match score of the given node.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="context"> The node to be tested.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double getMatchScore(org.apache.xpath.XPathContext xctxt, int context) throws javax.xml.transform.TransformerException
	  public virtual double getMatchScore(XPathContext xctxt, int context)
	  {

		xctxt.pushCurrentNode(context);
		xctxt.pushCurrentExpressionNode(context);

		try
		{
		  XObject score = execute(xctxt);

		  return score.num();
		}
		finally
		{
		  xctxt.popCurrentNode();
		  xctxt.popCurrentExpressionNode();
		}

		// return XPath.MATCH_SCORE_NONE;
	  }

	  /// <summary>
	  /// Set the axis that this step should follow. 
	  /// 
	  /// </summary>
	  /// <param name="axis"> The Axis for this test, one of of Axes.ANCESTORORSELF, etc. </param>
	  public virtual int Axis
	  {
		  set
		  {
			m_axis = value;
		  }
		  get
		  {
			return m_axis;
		  }
	  }


	  internal class PredOwner : ExpressionOwner
	  {
		  private readonly StepPattern outerInstance;

		  internal int m_index;

		  internal PredOwner(StepPattern outerInstance, int index)
		  {
			  this.outerInstance = outerInstance;
			  m_index = index;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_predicates[m_index];
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_predicates[m_index] = value;
			}
		}


	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
			   if (visitor.visitMatchPattern(owner, this))
			   {
				   callSubtreeVisitors(visitor);
			   }
	  }

	  /// <summary>
	  /// Call the visitors on the subtree.  Factored out from callVisitors 
	  /// so it may be called by derived classes.
	  /// </summary>
	  protected internal virtual void callSubtreeVisitors(XPathVisitor visitor)
	  {
		if (null != m_predicates)
		{
		  int n = m_predicates.Length;
		  for (int i = 0; i < n; i++)
		  {
			ExpressionOwner predOwner = new PredOwner(this, i);
			if (visitor.visitPredicate(predOwner, m_predicates[i]))
			{
			  m_predicates[i].callVisitors(predOwner, visitor);
			}
		  }
		}
		if (null != m_relativePathPattern)
		{
		  m_relativePathPattern.callVisitors(this, visitor);
		}
	  }


	  /// <seealso cref="ExpressionOwner.getExpression()"/>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_relativePathPattern;
		  }
		  set
		  {
			value.exprSetParent(this);
			  m_relativePathPattern = (StepPattern)value;
		  }
	  }


	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  StepPattern sp = (StepPattern)expr;

		if (null != m_predicates)
		{
			int n = m_predicates.Length;
			if ((null == sp.m_predicates) || (sp.m_predicates.Length != n))
			{
				  return false;
			}
			for (int i = 0; i < n; i++)
			{
			  if (!m_predicates[i].deepEquals(sp.m_predicates[i]))
			  {
				  return false;
			  }
			}
		}
		else if (null != sp.m_predicates)
		{
			return false;
		}

		  if (null != m_relativePathPattern)
		  {
			  if (!m_relativePathPattern.deepEquals(sp.m_relativePathPattern))
			  {
				  return false;
			  }
		  }
		  else if (sp.m_relativePathPattern != null)
		  {
			  return false;
		  }

		  return true;
	  }


	}

}