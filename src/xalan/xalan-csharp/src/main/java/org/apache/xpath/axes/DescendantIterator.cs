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
 * $Id: DescendantIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Expression = org.apache.xpath.Expression;
	using XPathContext = org.apache.xpath.XPathContext;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpCodes = org.apache.xpath.compiler.OpCodes;
	using OpMap = org.apache.xpath.compiler.OpMap;
	using NodeTest = org.apache.xpath.patterns.NodeTest;

	/// <summary>
	/// This class implements an optimized iterator for
	/// descendant, descendant-or-self, or "//foo" patterns. </summary>
	/// <seealso cref="org.apache.xpath.axes.LocPathIterator"
	/// @xsl.usage advanced/>
	[Serializable]
	public class DescendantIterator : LocPathIterator
	{
		internal new const long serialVersionUID = -1190338607743976938L;
	  /// <summary>
	  /// Create a DescendantIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: DescendantIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal DescendantIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis, false)
	  {


		int firstStepPos = OpMap.getFirstChildPos(opPos);
		int stepType = compiler.getOp(firstStepPos);

		bool orSelf = (OpCodes.FROM_DESCENDANTS_OR_SELF == stepType);
		bool fromRoot = false;
		if (OpCodes.FROM_SELF == stepType)
		{
		  orSelf = true;
		  // firstStepPos += 8;
		}
		else if (OpCodes.FROM_ROOT == stepType)
		{
		  fromRoot = true;
		  // Ugly code... will go away when AST work is done.
		  int nextStepPos = compiler.getNextStepPos(firstStepPos);
		  if (compiler.getOp(nextStepPos) == OpCodes.FROM_DESCENDANTS_OR_SELF)
		  {
			orSelf = true;
		  }
		  // firstStepPos += 8;
		}

		// Find the position of the last step.
		int nextStepPos = firstStepPos;
		while (true)
		{
		  nextStepPos = compiler.getNextStepPos(nextStepPos);
		  if (nextStepPos > 0)
		  {
			int stepOp = compiler.getOp(nextStepPos);
			if (OpCodes.ENDOP != stepOp)
			{
			  firstStepPos = nextStepPos;
			}
			else
			{
			  break;
			}
		  }
		  else
		  {
			break;
		  }

		}

		// Fix for http://nagoya.apache.org/bugzilla/show_bug.cgi?id=1336
		if ((analysis & WalkerFactory.BIT_CHILD) != 0)
		{
		  orSelf = false;
		}

		if (fromRoot)
		{
		  if (orSelf)
		  {
			m_axis = Axis.DESCENDANTSORSELFFROMROOT;
		  }
		  else
		  {
			m_axis = Axis.DESCENDANTSFROMROOT;
		  }
		}
		else if (orSelf)
		{
		  m_axis = Axis.DESCENDANTORSELF;
		}
		else
		{
		  m_axis = Axis.DESCENDANT;
		}

		int whatToShow = compiler.getWhatToShow(firstStepPos);

		if ((0 == (whatToShow & (DTMFilter.SHOW_ATTRIBUTE | DTMFilter.SHOW_ELEMENT | DTMFilter.SHOW_PROCESSING_INSTRUCTION))) || (whatToShow == DTMFilter.SHOW_ALL))
		{
		  initNodeTest(whatToShow);
		}
		else
		{
		  initNodeTest(whatToShow, compiler.getStepNS(firstStepPos), compiler.getStepLocalName(firstStepPos));
		}
		initPredicateInfo(compiler, firstStepPos);
	  }

	  /// <summary>
	  /// Create a DescendantIterator object.
	  /// 
	  /// </summary>
	  public DescendantIterator() : base(null)
	  {
		m_axis = Axis.DESCENDANTSORSELFFROMROOT;
		int whatToShow = DTMFilter.SHOW_ALL;
		initNodeTest(whatToShow);
	  }


	  /// <summary>
	  ///  Get a cloned Iterator that is reset to the beginning
	  ///  of the query.
	  /// </summary>
	  ///  <returns> A cloned NodeIterator set of the start of the query.
	  /// </returns>
	  ///  <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public override DTMIterator cloneWithReset()
	  {

		DescendantIterator clone = (DescendantIterator) base.cloneWithReset();
		clone.m_traverser = m_traverser;

		clone.resetProximityPositions();

		return clone;
	  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a NodeIterator is created, the first call
	  /// to nextNode() returns the first node in the set.
	  /// </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set.
	  /// </returns>
	  /// <exception cref="DOMException">
	  ///    INVALID_STATE_ERR: Raised if this method is called after the
	  ///   <code>detach</code> method was invoked. </exception>
	  public override int nextNode()
	  {
		   if (m_foundLast)
		   {
			  return DTM.NULL;
		   }

		if (DTM.NULL == m_lastFetched)
		{
		  resetProximityPositions();
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
		  do
		  {
			if (0 == m_extendedTypeID)
			{
			  next = m_lastFetched = (DTM.NULL == m_lastFetched) ? m_traverser.first(m_context) : m_traverser.next(m_context, m_lastFetched);
			}
			else
			{
			  next = m_lastFetched = (DTM.NULL == m_lastFetched) ? m_traverser.first(m_context, m_extendedTypeID) : m_traverser.next(m_context, m_lastFetched, m_extendedTypeID);
			}

			if (DTM.NULL != next)
			{
			  if (DTMIterator.FILTER_ACCEPT == acceptNode(next))
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
			  m_pos++;
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
	  /// Initialize the context values for this expression
	  /// after it is cloned.
	  /// </summary>
	  /// <param name="context"> The XPath runtime context for this
	  /// transformation. </param>
	  public override void setRoot(int context, object environment)
	  {
		base.setRoot(context, environment);
		m_traverser = m_cdtm.getAxisTraverser(m_axis);

		string localName = LocalName;
		string @namespace = Namespace;
		int what = m_whatToShow;
		// System.out.println("what: ");
		// NodeTest.debugWhatToShow(what);
		if (DTMFilter.SHOW_ALL == what || NodeTest.WILD.Equals(localName) || NodeTest.WILD.Equals(@namespace))
		{
		  m_extendedTypeID = 0;
		}
		else
		{
		  int type = getNodeTypeTest(what);
		  m_extendedTypeID = m_cdtm.getExpandedTypeID(@namespace, localName, type);
		}

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
		if (PredicateCount > 0)
		{
		  return base.asNode(xctxt);
		}

		int current = xctxt.CurrentNode;

		DTM dtm = xctxt.getDTM(current);
		DTMAxisTraverser traverser = dtm.getAxisTraverser(m_axis);

		string localName = LocalName;
		string @namespace = Namespace;
		int what = m_whatToShow;

		// System.out.print(" (DescendantIterator) ");

		// System.out.println("what: ");
		// NodeTest.debugWhatToShow(what);
		if (DTMFilter.SHOW_ALL == what || string.ReferenceEquals(localName, NodeTest.WILD) || string.ReferenceEquals(@namespace, NodeTest.WILD))
		{
		  return traverser.first(current);
		}
		else
		{
		  int type = getNodeTypeTest(what);
		  int extendedType = dtm.getExpandedTypeID(@namespace, localName, type);
		  return traverser.first(current, extendedType);
		}
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
		  m_extendedTypeID = 0;

		  // Always call the superclass detach last!
		  base.detach();
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
			return m_axis;
		  }
	  }


	  /// <summary>
	  /// The traverser to use to navigate over the descendants. </summary>
	  [NonSerialized]
	  protected internal DTMAxisTraverser m_traverser;

	  /// <summary>
	  /// The axis that we are traversing. </summary>
	  protected internal int m_axis;

	  /// <summary>
	  /// The extended type ID, not set until setRoot. </summary>
	  protected internal int m_extendedTypeID;

	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  if (m_axis != ((DescendantIterator)expr).m_axis)
		  {
			  return false;
		  }

		  return true;
	  }


	}

}