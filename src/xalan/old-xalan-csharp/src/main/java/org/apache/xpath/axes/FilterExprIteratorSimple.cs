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
 * $Id: FilterExprIteratorSimple.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{

	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;

	/// <summary>
	/// Class to use for one-step iteration that doesn't have a predicate, and 
	/// doesn't need to set the context.
	/// </summary>
	[Serializable]
	public class FilterExprIteratorSimple : LocPathIterator
	{
		internal new const long serialVersionUID = -6978977187025375579L;
	  /// <summary>
	  /// The contained expression. Should be non-null.
	  ///  @serial   
	  /// </summary>
	  private Expression m_expr;

	  /// <summary>
	  /// The result of executing m_expr.  Needs to be deep cloned on clone op. </summary>
	  [NonSerialized]
	  private XNodeSet m_exprObj;

	  private bool m_mustHardReset = false;
	  private bool m_canDetachNodeset = true;

	  /// <summary>
	  /// Create a FilterExprIteratorSimple object.
	  /// 
	  /// </summary>
	  public FilterExprIteratorSimple() : base(null)
	  {
	  }

	  /// <summary>
	  /// Create a FilterExprIteratorSimple object.
	  /// 
	  /// </summary>
	  public FilterExprIteratorSimple(Expression expr) : base(null)
	  {
		m_expr = expr;
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
		  m_exprObj = executeFilterExpr(context, m_execContext, PrefixResolver, IsTopLevel, m_stackFrame, m_expr);
	  }

	  /// <summary>
	  /// Execute the expression.  Meant for reuse by other FilterExpr iterators 
	  /// that are not derived from this object.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.xpath.objects.XNodeSet executeFilterExpr(int context, org.apache.xpath.XPathContext xctxt, org.apache.xml.utils.PrefixResolver prefixResolver, boolean isTopLevel, int stackFrame, org.apache.xpath.Expression expr) throws org.apache.xml.utils.WrappedRuntimeException
	  public static XNodeSet executeFilterExpr(int context, XPathContext xctxt, PrefixResolver prefixResolver, bool isTopLevel, int stackFrame, Expression expr)
	  {
		PrefixResolver savedResolver = xctxt.NamespaceContext;
		XNodeSet result = null;

		try
		{
		  xctxt.pushCurrentNode(context);
		  xctxt.NamespaceContext = prefixResolver;

		  // The setRoot operation can take place with a reset operation, 
		  // and so we may not be in the context of LocPathIterator#nextNode, 
		  // so we have to set up the variable context, execute the expression, 
		  // and then restore the variable context.

		  if (isTopLevel)
		  {
			// System.out.println("calling m_expr.execute(getXPathContext())");
			VariableStack vars = xctxt.VarStack;

			// These three statements need to be combined into one operation.
			int savedStart = vars.StackFrame;
			vars.StackFrame = stackFrame;

			result = (XNodeSet) expr.execute(xctxt);
			result.ShouldCacheNodes = true;

			// These two statements need to be combined into one operation.
			vars.StackFrame = savedStart;
		  }
		  else
		  {
			  result = (XNodeSet) expr.execute(xctxt);
		  }

		}
		catch (javax.xml.transform.TransformerException se)
		{

		  // TODO: Fix...
		  throw new org.apache.xml.utils.WrappedRuntimeException(se);
		}
		finally
		{
		  xctxt.popCurrentNode();
		  xctxt.NamespaceContext = savedResolver;
		}
		return result;
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
			  return org.apache.xml.dtm.DTM_Fields.NULL;
		  }

		int next;

		if (null != m_exprObj)
		{
		  m_lastFetched = next = m_exprObj.nextNode();
		}
		else
		{
		  m_lastFetched = next = org.apache.xml.dtm.DTM_Fields.NULL;
		}

		// m_lastFetched = next;
		if (org.apache.xml.dtm.DTM_Fields.NULL != next)
		{
		  m_pos++;
		  return next;
		}
		else
		{
		  m_foundLast = true;

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Detaches the walker from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state.
	  /// </summary>
	  public override void detach()
	  {
		if (m_allowDetach)
		{
			  base.detach();
			  m_exprObj.detach();
			  m_exprObj = null;
		}
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
		m_expr.fixupVariables(vars, globalsSize);
	  }

	  /// <summary>
	  /// Get the inner contained expression of this filter.
	  /// </summary>
	  public virtual Expression InnerExpression
	  {
		  get
		  {
			return m_expr;
		  }
		  set
		  {
			value.exprSetParent(this);
			m_expr = value;
		  }
	  }


	  /// <summary>
	  /// Get the analysis bits for this walker, as defined in the WalkerFactory. </summary>
	  /// <returns> One of WalkerFactory#BIT_DESCENDANT, etc. </returns>
	  public override int AnalysisBits
	  {
		  get
		  {
			if (null != m_expr && m_expr is PathComponent)
			{
			  return ((PathComponent) m_expr).AnalysisBits;
			}
			return WalkerFactory.BIT_FILTER;
		  }
	  }

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// Warning: This can only be called after setRoot has been called!
	  /// </summary>
	  /// <returns> true as a default. </returns>
	  public override bool DocOrdered
	  {
		  get
		  {
			return m_exprObj.DocOrdered;
		  }
	  }

	  internal class filterExprOwner : ExpressionOwner
	  {
		  private readonly FilterExprIteratorSimple outerInstance;

		  public filterExprOwner(FilterExprIteratorSimple outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <seealso cref= ExpressionOwner#getExpression() </seealso>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_expr;
			}
			set
			{
			  value.exprSetParent(outerInstance);
			  outerInstance.m_expr = value;
			}
		}


	  }

	  /// <summary>
	  /// This will traverse the heararchy, calling the visitor for 
	  /// each member.  If the called visitor method returns 
	  /// false, the subtree should not be called.
	  /// </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  public override void callPredicateVisitors(XPathVisitor visitor)
	  {
		m_expr.callVisitors(new filterExprOwner(this), visitor);

		base.callPredicateVisitors(visitor);
	  }

	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		if (!base.deepEquals(expr))
		{
		  return false;
		}

		FilterExprIteratorSimple fet = (FilterExprIteratorSimple) expr;
		if (!m_expr.deepEquals(fet.m_expr))
		{
		  return false;
		}

		return true;
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
			  if (null != m_exprObj)
			  {
				return m_exprObj.Axis;
			  }
			else
			{
				return Axis.FILTEREDLIST;
			}
		  }
	  }


	}


}