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
 * $Id: PredicatedNodeTest.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XObject = org.apache.xpath.objects.XObject;
	using NodeTest = org.apache.xpath.patterns.NodeTest;

	[Serializable]
	public abstract class PredicatedNodeTest : NodeTest, SubContextList
	{
		internal new const long serialVersionUID = -6193530757296377351L;

	  /// <summary>
	  /// Construct an AxesWalker using a LocPathIterator.
	  /// </summary>
	  /// <param name="locPathIterator"> non-null reference to the parent iterator. </param>
	  internal PredicatedNodeTest(LocPathIterator locPathIterator)
	  {
		m_lpi = locPathIterator;
	  }

	  /// <summary>
	  /// Construct an AxesWalker.  The location path iterator will have to be set
	  /// before use.
	  /// </summary>
	  internal PredicatedNodeTest()
	  {
	  }

	  /// <summary>
	  /// Read the object from a serialization stream.
	  /// </summary>
	  /// <param name="stream"> Input stream to read from
	  /// </param>
	  /// <exception cref="java.io.IOException"> </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, javax.xml.transform.TransformerException
	  private void readObject(java.io.ObjectInputStream stream)
	  {
		try
		{
		  stream.defaultReadObject();
		  m_predicateIndex = -1;
		  resetProximityPositions();
		}
		catch (ClassNotFoundException cnfe)
		{
		  throw new javax.xml.transform.TransformerException(cnfe);
		}
	  }

	  /// <summary>
	  /// Get a cloned PrdicatedNodeTest.
	  /// </summary>
	  /// <returns> A new PredicatedNodeTest that can be used without mutating this one.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {
		// Do not access the location path itterator during this operation!

		PredicatedNodeTest clone = (PredicatedNodeTest) base.clone();

		if ((null != this.m_proximityPositions) && (this.m_proximityPositions == clone.m_proximityPositions))
		{
		  clone.m_proximityPositions = new int[this.m_proximityPositions.Length];

		  Array.Copy(this.m_proximityPositions, 0, clone.m_proximityPositions, 0, this.m_proximityPositions.Length);
		}

		if (clone.m_lpi == this)
		{
		  clone.m_lpi = (LocPathIterator)clone;
		}

		return clone;
	  }

	  // Only for clones for findLastPos.  See bug4638.
	  protected internal int m_predCount = -1;

	  /// <summary>
	  /// Get the number of predicates that this walker has.
	  /// </summary>
	  /// <returns> the number of predicates that this walker has. </returns>
	  public virtual int PredicateCount
	  {
		  get
		  {
			if (-1 == m_predCount)
			{
			  return (null == m_predicates) ? 0 : m_predicates.Length;
			}
			else
			{
			  return m_predCount;
			}
		  }
		  set
		  {
			if (value > 0)
			{
			  Expression[] newPredicates = new Expression[value];
			  for (int i = 0; i < value; i++)
			  {
				newPredicates[i] = m_predicates[i];
			  }
			  m_predicates = newPredicates;
			}
			else
			{
			  m_predicates = null;
			}
    
		  }
	  }


	  /// <summary>
	  /// Init predicate info.
	  /// </summary>
	  /// <param name="compiler"> The Compiler object that has information about this 
	  ///                 walker in the op map. </param>
	  /// <param name="opPos"> The op code position of this location step.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void initPredicateInfo(org.apache.xpath.compiler.Compiler compiler, int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual void initPredicateInfo(Compiler compiler, int opPos)
	  {

		int pos = compiler.getFirstPredicateOpPos(opPos);

		if (pos > 0)
		{
		  m_predicates = compiler.getCompiledPredicates(pos);
		  if (null != m_predicates)
		  {
			  for (int i = 0; i < m_predicates.Length; i++)
			  {
				  m_predicates[i].exprSetParent(this);
			  }
		  }
		}
	  }

	  /// <summary>
	  /// Get a predicate expression at the given index.
	  /// 
	  /// </summary>
	  /// <param name="index"> Index of the predicate.
	  /// </param>
	  /// <returns> A predicate expression. </returns>
	  public virtual Expression getPredicate(int index)
	  {
		return m_predicates[index];
	  }

	  /// <summary>
	  /// Get the current sub-context position.
	  /// </summary>
	  /// <returns> The node position of this walker in the sub-context node list. </returns>
	  public virtual int ProximityPosition
	  {
		  get
		  {
    
			// System.out.println("getProximityPosition - m_predicateIndex: "+m_predicateIndex);
			return getProximityPosition(m_predicateIndex);
		  }
	  }

	  /// <summary>
	  /// Get the current sub-context position.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context.
	  /// </param>
	  /// <returns> The node position of this walker in the sub-context node list. </returns>
	  public virtual int getProximityPosition(XPathContext xctxt)
	  {
		return ProximityPosition;
	  }

	  /// <summary>
	  /// Get the index of the last node that can be itterated to.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the index of the last node that can be itterated to. </returns>
	  public abstract int getLastPos(XPathContext xctxt);

	  /// <summary>
	  /// Get the current sub-context position.
	  /// </summary>
	  /// <param name="predicateIndex"> The index of the predicate where the proximity 
	  ///                       should be taken from.
	  /// </param>
	  /// <returns> The node position of this walker in the sub-context node list. </returns>
	  protected internal virtual int getProximityPosition(int predicateIndex)
	  {
		return (predicateIndex >= 0) ? m_proximityPositions[predicateIndex] : 0;
	  }

	  /// <summary>
	  /// Reset the proximity positions counts.
	  /// </summary>
	  public virtual void resetProximityPositions()
	  {
		int nPredicates = PredicateCount;
		if (nPredicates > 0)
		{
		  if (null == m_proximityPositions)
		  {
			m_proximityPositions = new int[nPredicates];
		  }

		  for (int i = 0; i < nPredicates; i++)
		  {
			try
			{
			  initProximityPosition(i);
			}
			catch (Exception e)
			{
			  // TODO: Fix this...
			  throw new org.apache.xml.utils.WrappedRuntimeException(e);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Init the proximity position to zero for a forward axes.
	  /// </summary>
	  /// <param name="i"> The index into the m_proximityPositions array.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void initProximityPosition(int i) throws javax.xml.transform.TransformerException
	  public virtual void initProximityPosition(int i)
	  {
		m_proximityPositions[i] = 0;
	  }

	  /// <summary>
	  /// Count forward one proximity position.
	  /// </summary>
	  /// <param name="i"> The index into the m_proximityPositions array, where the increment 
	  ///          will occur. </param>
	  protected internal virtual void countProximityPosition(int i)
	  {
		  // Note that in the case of a UnionChildIterator, this may be a 
		  // static object and so m_proximityPositions may indeed be null!
		  int[] pp = m_proximityPositions;
		if ((null != pp) && (i < pp.Length))
		{
		  pp[i]++;
		}
	  }

	  /// <summary>
	  /// Tells if this is a reverse axes.
	  /// </summary>
	  /// <returns> false, unless a derived class overrides. </returns>
	  public virtual bool ReverseAxes
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Get which predicate is executing.
	  /// </summary>
	  /// <returns> The current predicate index, or -1 if no predicate is executing. </returns>
	  public virtual int PredicateIndex
	  {
		  get
		  {
			return m_predicateIndex;
		  }
	  }

	  /// <summary>
	  /// Process the predicates.
	  /// </summary>
	  /// <param name="context"> The current context node. </param>
	  /// <param name="xctxt"> The XPath runtime context.
	  /// </param>
	  /// <returns> the result of executing the predicate expressions.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: boolean executePredicates(int context, org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  internal virtual bool executePredicates(int context, XPathContext xctxt)
	  {

		int nPredicates = PredicateCount;
		// System.out.println("nPredicates: "+nPredicates);
		if (nPredicates == 0)
		{
		  return true;
		}

		PrefixResolver savedResolver = xctxt.NamespaceContext;

		try
		{
		  m_predicateIndex = 0;
		  xctxt.pushSubContextList(this);
		  xctxt.pushNamespaceContext(m_lpi.PrefixResolver);
		  xctxt.pushCurrentNode(context);

		  for (int i = 0; i < nPredicates; i++)
		  {
			// System.out.println("Executing predicate expression - waiting count: "+m_lpi.getWaitingCount());
			XObject pred = m_predicates[i].execute(xctxt);
			// System.out.println("\nBack from executing predicate expression - waiting count: "+m_lpi.getWaitingCount());
			// System.out.println("pred.getType(): "+pred.getType());
			if (XObject.CLASS_NUMBER == pred.Type)
			{
			  if (DEBUG_PREDICATECOUNTING)
			  {
				System.out.flush();
				Console.WriteLine("\n===== start predicate count ========");
				Console.WriteLine("m_predicateIndex: " + m_predicateIndex);
				// System.out.println("getProximityPosition(m_predicateIndex): "
				//                   + getProximityPosition(m_predicateIndex));
				Console.WriteLine("pred.num(): " + pred.num());
			  }

			  int proxPos = this.getProximityPosition(m_predicateIndex);
			  int predIndex = (int) pred.num();
			  if (proxPos != predIndex)
			  {
				if (DEBUG_PREDICATECOUNTING)
				{
				  Console.WriteLine("\nnode context: " + nodeToString(context));
				  Console.WriteLine("index predicate is false: " + proxPos);
				  Console.WriteLine("\n===== end predicate count ========");
				}
				return false;
			  }
			  else if (DEBUG_PREDICATECOUNTING)
			  {
				Console.WriteLine("\nnode context: " + nodeToString(context));
				Console.WriteLine("index predicate is true: " + proxPos);
				Console.WriteLine("\n===== end predicate count ========");
			  }

			  // If there is a proximity index that will not change during the 
			  // course of itteration, then we know there can be no more true 
			  // occurances of this predicate, so flag that we're done after 
			  // this.
			  //
			  // bugzilla 14365
			  // We can't set m_foundLast = true unless we're sure that -all-
			  // remaining parameters are stable, or else last() fails. Fixed so
			  // only sets m_foundLast if on the last predicate
			  if (m_predicates[i].StableNumber && i == nPredicates - 1)
			  {
				m_foundLast = true;
			  }
			}
			else if (!pred.@bool())
			{
			  return false;
			}

			countProximityPosition(++m_predicateIndex);
		  }
		}
		finally
		{
		  xctxt.popCurrentNode();
		  xctxt.popNamespaceContext();
		  xctxt.popSubContextList();
		  m_predicateIndex = -1;
		}

		return true;
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

		int nPredicates = PredicateCount;

		for (int i = 0; i < nPredicates; i++)
		{
		  m_predicates[i].fixupVariables(vars, globalsSize);
		}
	  }


	  /// <summary>
	  /// Diagnostics.
	  /// </summary>
	  /// <param name="n"> Node to give diagnostic information about, or null.
	  /// </param>
	  /// <returns> Informative string about the argument. </returns>
	  protected internal virtual string nodeToString(int n)
	  {
		if (DTM.NULL != n)
		{
		  DTM dtm = m_lpi.XPathContext.getDTM(n);
		  return dtm.getNodeName(n) + "{" + (n + 1) + "}";
		}
		else
		{
		  return "null";
		}
	  }

	  //=============== NodeFilter Implementation ===============

	  /// <summary>
	  ///  Test whether a specified node is visible in the logical view of a
	  /// TreeWalker or NodeIterator. This function will be called by the
	  /// implementation of TreeWalker and NodeIterator; it is not intended to
	  /// be called directly from user code. </summary>
	  /// <param name="n">  The node to check to see if it passes the filter or not. </param>
	  /// <returns>  a constant to determine whether the node is accepted,
	  ///   rejected, or skipped, as defined  above . </returns>
	  public virtual short acceptNode(int n)
	  {

		XPathContext xctxt = m_lpi.XPathContext;

		try
		{
		  xctxt.pushCurrentNode(n);

		  XObject score = execute(xctxt, n);

		  // System.out.println("\n::acceptNode - score: "+score.num()+"::");
		  if (score != NodeTest.SCORE_NONE)
		  {
			if (PredicateCount > 0)
			{
			  countProximityPosition(0);

			  if (!executePredicates(n, xctxt))
			  {
				return DTMIterator.FILTER_SKIP;
			  }
			}

			return DTMIterator.FILTER_ACCEPT;
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

		return DTMIterator.FILTER_SKIP;
	  }


	  /// <summary>
	  /// Get the owning location path iterator.
	  /// </summary>
	  /// <returns> the owning location path iterator, which should not be null. </returns>
	  public virtual LocPathIterator LocPathIterator
	  {
		  get
		  {
			return m_lpi;
		  }
		  set
		  {
			m_lpi = value;
			if (this != value)
			{
			  value.exprSetParent(this);
			}
		  }
	  }


	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
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
		/// This will traverse the heararchy, calling the visitor for 
		/// each member.  If the called visitor method returns 
		/// false, the subtree should not be called.
		/// </summary>
		/// <param name="visitor"> The visitor whose appropriate method will be called. </param>
		public virtual void callPredicateVisitors(XPathVisitor visitor)
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
		}

		/// <seealso cref="Expression.deepEquals(Expression)"/>
		public override bool deepEquals(Expression expr)
		{
		  if (!base.deepEquals(expr))
		  {
				return false;
		  }

		  PredicatedNodeTest pnt = (PredicatedNodeTest) expr;
		  if (null != m_predicates)
		  {

			int n = m_predicates.Length;
			if ((null == pnt.m_predicates) || (pnt.m_predicates.Length != n))
			{
				  return false;
			}
			for (int i = 0; i < n; i++)
			{
			  if (!m_predicates[i].deepEquals(pnt.m_predicates[i]))
			  {
				  return false;
			  }
			}
		  }
		  else if (null != pnt.m_predicates)
		  {
				  return false;
		  }

		  return true;
		}

	  /// <summary>
	  /// This is true if nextNode returns null. </summary>
	  [NonSerialized]
	  protected internal bool m_foundLast = false;

	  /// <summary>
	  /// The owning location path iterator.
	  ///  @serial 
	  /// </summary>
	  protected internal LocPathIterator m_lpi;

	  /// <summary>
	  /// Which predicate we are executing.
	  /// </summary>
	  [NonSerialized]
	  internal int m_predicateIndex = -1;

	  /// <summary>
	  /// The list of predicate expressions. Is static and does not need 
	  ///  to be deep cloned.
	  ///  @serial 
	  /// </summary>
	  private Expression[] m_predicates;

	  /// <summary>
	  /// An array of counts that correspond to the number
	  /// of predicates the step contains.
	  /// </summary>
	  [NonSerialized]
	  protected internal int[] m_proximityPositions;

	  /// <summary>
	  /// If true, diagnostic messages about predicate execution will be posted. </summary>
	  internal const bool DEBUG_PREDICATECOUNTING = false;

	  internal class PredOwner : ExpressionOwner
	  {
		  private readonly PredicatedNodeTest outerInstance;

		  internal int m_index;

		  internal PredOwner(PredicatedNodeTest outerInstance, int index)
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

	}

}