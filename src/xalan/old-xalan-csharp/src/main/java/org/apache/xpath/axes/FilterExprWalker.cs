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
 * $Id: FilterExprWalker.java 469367 2006-10-31 04:41:08Z minchau $
 */
namespace org.apache.xpath.axes
{

	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpCodes = org.apache.xpath.compiler.OpCodes;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;

	/// <summary>
	/// Walker for the OP_VARIABLE, or OP_EXTFUNCTION, or OP_FUNCTION, or OP_GROUP,
	/// op codes. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xpath#NT-FilterExpr">XPath FilterExpr descriptions</a> </seealso>
	[Serializable]
	public class FilterExprWalker : AxesWalker
	{
		internal new const long serialVersionUID = 5457182471424488375L;

	  /// <summary>
	  /// Construct a FilterExprWalker using a LocPathIterator.
	  /// </summary>
	  /// <param name="locPathIterator"> non-null reference to the parent iterator. </param>
	  public FilterExprWalker(WalkingIterator locPathIterator) : base(locPathIterator, Axis.FILTEREDLIST)
	  {
	  }

	  /// <summary>
	  /// Init a FilterExprWalker.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to the Compiler that is constructing. </param>
	  /// <param name="opPos"> positive opcode position for this step. </param>
	  /// <param name="stepType"> The type of step.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(org.apache.xpath.compiler.Compiler compiler, int opPos, int stepType) throws javax.xml.transform.TransformerException
	  public override void init(Compiler compiler, int opPos, int stepType)
	  {

		base.init(compiler, opPos, stepType);

		// Smooth over an anomily in the opcode map...
		switch (stepType)
		{
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_EXTFUNCTION :
			m_mustHardReset = true;
			goto case org.apache.xpath.compiler.OpCodes.OP_GROUP;
		case OpCodes.OP_GROUP :
		case OpCodes.OP_VARIABLE :
		  m_expr = compiler.compile(opPos);
		  m_expr.exprSetParent(this);
		  //if((OpCodes.OP_FUNCTION == stepType) && (m_expr instanceof org.apache.xalan.templates.FuncKey))
		  if (m_expr is org.apache.xpath.operations.Variable)
		  {
			  // hack/temp workaround
			  m_canDetachNodeset = false;
		  }
		  break;
		default :
		  m_expr = compiler.compile(opPos + 2);
		  m_expr.exprSetParent(this);
	  break;
		}
	//    if(m_expr instanceof WalkingIterator)
	//    {
	//      WalkingIterator wi = (WalkingIterator)m_expr;
	//      if(wi.getFirstWalker() instanceof FilterExprWalker)
	//      {
	//      	FilterExprWalker fw = (FilterExprWalker)wi.getFirstWalker();
	//      	if(null == fw.getNextWalker())
	//      	{
	//      		m_expr = fw.m_expr;
	//      		m_expr.exprSetParent(this);
	//      	}
	//      }
	//      		
	//    }
	  }

	  /// <summary>
	  /// Detaches the walker from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state.
	  /// </summary>
	  public override void detach()
	  {
		  base.detach();
		  if (m_canDetachNodeset)
		  {
			m_exprObj.detach();
		  }
		  m_exprObj = null;
	  }

	  /// <summary>
	  ///  Set the root node of the TreeWalker.
	  /// </summary>
	  /// <param name="root"> non-null reference to the root, or starting point of 
	  ///        the query. </param>
	  public override int Root
	  {
		  set
		  {
    
			base.Root = value;
    
			  m_exprObj = FilterExprIteratorSimple.executeFilterExpr(value, m_lpi.XPathContext, m_lpi.PrefixResolver, m_lpi.IsTopLevel, m_lpi.m_stackFrame, m_expr);
    
		  }
	  }

	  /// <summary>
	  /// Get a cloned FilterExprWalker.
	  /// </summary>
	  /// <returns> A new FilterExprWalker that can be used without mutating this one.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {

		FilterExprWalker clone = (FilterExprWalker) base.clone();

		if (null != m_exprObj)
		{
		  clone.m_exprObj = (XNodeSet) m_exprObj.clone();
		}

		return clone;
	  }

	  /// <summary>
	  /// This method needs to override AxesWalker.acceptNode because FilterExprWalkers
	  /// don't need to, and shouldn't, do a node test. </summary>
	  /// <param name="n">  The node to check to see if it passes the filter or not. </param>
	  /// <returns>  a constant to determine whether the node is accepted,
	  ///   rejected, or skipped, as defined  above . </returns>
	  public override short acceptNode(int n)
	  {

		try
		{
		  if (PredicateCount > 0)
		  {
			countProximityPosition(0);

			if (!executePredicates(n, m_lpi.XPathContext))
			{
			  return org.apache.xml.dtm.DTMIterator_Fields.FILTER_SKIP;
			}
		  }

		  return org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT;
		}
		catch (javax.xml.transform.TransformerException se)
		{
		  throw new Exception(se.Message);
		}
	  }

	  /// <summary>
	  ///  Moves the <code>TreeWalker</code> to the next visible node in document
	  /// order relative to the current node, and returns the new node. If the
	  /// current node has no next node,  or if the search for nextNode attempts
	  /// to step upward from the TreeWalker's root node, returns
	  /// <code>null</code> , and retains the current node. </summary>
	  /// <returns>  The new node, or <code>null</code> if the current node has no
	  ///   next node  in the TreeWalker's logical view. </returns>
	  public override int NextNode
	  {
		  get
		  {
    
			if (null != m_exprObj)
			{
			   int next = m_exprObj.nextNode();
			   return next;
			}
			else
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}
		  }
	  }

	  /// <summary>
	  /// Get the index of the last node that can be itterated to.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the index of the last node that can be itterated to. </returns>
	  public override int getLastPos(XPathContext xctxt)
	  {
		return m_exprObj.Length;
	  }

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

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public override int Axis
	  {
		  get
		  {
			return m_exprObj.Axis;
		  }
	  }

	  internal class filterExprOwner : ExpressionOwner
	  {
		  private readonly FilterExprWalker outerInstance;

		  public filterExprOwner(FilterExprWalker outerInstance)
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

		  FilterExprWalker walker = (FilterExprWalker)expr;
		  if (!m_expr.deepEquals(walker.m_expr))
		  {
			  return false;
		  }

		  return true;
		}



	}

}