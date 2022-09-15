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
 * $Id: FilterExprIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;

	[Serializable]
	public class FilterExprIterator : BasicTestIterator
	{
		internal new const long serialVersionUID = 2552176105165737614L;
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
	  /// Create a FilterExprIterator object.
	  /// 
	  /// </summary>
	  public FilterExprIterator() : base(null)
	  {
	  }

	  /// <summary>
	  /// Create a FilterExprIterator object.
	  /// 
	  /// </summary>
	  public FilterExprIterator(Expression expr) : base(null)
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

		  m_exprObj = FilterExprIteratorSimple.executeFilterExpr(context, m_execContext, PrefixResolver, IsTopLevel, m_stackFrame, m_expr);
	  }


	  /// <summary>
	  /// Get the next node via getNextXXX.  Bottlenecked for derived class override. </summary>
	  /// <returns> The next node on the axis, or DTM.NULL. </returns>
	  protected internal override int NextNode
	  {
		  get
		  {
			if (null != m_exprObj)
			{
			  m_lastFetched = m_exprObj.nextNode();
			}
			else
			{
			  m_lastFetched = org.apache.xml.dtm.DTM_Fields.NULL;
			}
    
			return m_lastFetched;
		  }
	  }

	  /// <summary>
	  /// Detaches the walker from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state.
	  /// </summary>
	  public override void detach()
	  {
		  base.detach();
		  m_exprObj.detach();
		  m_exprObj = null;
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
		  private readonly FilterExprIterator outerInstance;

		  public filterExprOwner(FilterExprIterator outerInstance)
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

		FilterExprIterator fet = (FilterExprIterator) expr;
		if (!m_expr.deepEquals(fet.m_expr))
		{
		  return false;
		}

		return true;
	  }

	}

}