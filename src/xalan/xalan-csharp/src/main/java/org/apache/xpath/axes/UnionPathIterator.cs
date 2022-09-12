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
 * $Id: UnionPathIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{

	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpCodes = org.apache.xpath.compiler.OpCodes;
	using OpMap = org.apache.xpath.compiler.OpMap;

	/// <summary>
	/// This class extends NodeSetDTM, which implements DTMIterator,
	/// and fetches nodes one at a time in document order based on a XPath
	/// <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	/// As each node is iterated via nextNode(), the node is also stored
	/// in the NodeVector, so that previousNode() can easily be done.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class UnionPathIterator : LocPathIterator, ICloneable, DTMIterator, PathComponent
	{
		internal new const long serialVersionUID = -3910351546843826781L;

	  /// <summary>
	  /// Constructor to create an instance which you can add location paths to.
	  /// </summary>
	  public UnionPathIterator() : base()
	  {


		// m_mutable = false;
		// m_cacheNodes = false;
		m_iterators = null;
		m_exprs = null;
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

		try
		{
		  if (null != m_exprs)
		  {
			int n = m_exprs.Length;
			DTMIterator[] newIters = new DTMIterator[n];

			for (int i = 0; i < n; i++)
			{
			  DTMIterator iter = m_exprs[i].asIterator(m_execContext, context);
			  newIters[i] = iter;
			  iter.nextNode();
			}
			m_iterators = newIters;
		  }
		}
		catch (Exception e)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(e);
		}
	  }

	  /// <summary>
	  /// Add an iterator to the union list.
	  /// </summary>
	  /// <param name="expr"> non-null reference to a location path iterator. </param>
	  public virtual void addIterator(DTMIterator expr)
	  {

		// Increase array size by only 1 at a time.  Fix this
		// if it looks to be a problem.
		if (null == m_iterators)
		{
		  m_iterators = new DTMIterator[1];
		  m_iterators[0] = expr;
		}
		else
		{
		  DTMIterator[] exprs = m_iterators;
		  int len = m_iterators.Length;

		  m_iterators = new DTMIterator[len + 1];

		  Array.Copy(exprs, 0, m_iterators, 0, len);

		  m_iterators[len] = expr;
		}
		expr.nextNode();
		if (expr is Expression)
		{
			((Expression)expr).exprSetParent(this);
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
			  if (m_allowDetach && null != m_iterators)
			  {
					  int n = m_iterators.Length;
					  for (int i = 0; i < n; i++)
					  {
							  m_iterators[i].detach();
					  }
					  m_iterators = null;
			  }
	  }


	  /// <summary>
	  /// Create a UnionPathIterator object, including creation 
	  /// of location path iterators from the opcode list, and call back 
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating 
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the 
	  /// opcode list from the compiler.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UnionPathIterator(org.apache.xpath.compiler.Compiler compiler, int opPos) throws javax.xml.transform.TransformerException
	  public UnionPathIterator(Compiler compiler, int opPos) : base()
	  {


		opPos = OpMap.getFirstChildPos(opPos);

		loadLocationPaths(compiler, opPos, 0);
	  }

	  /// <summary>
	  /// This will return an iterator capable of handling the union of paths given.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating 
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the 
	  /// opcode list from the compiler.
	  /// </param>
	  /// <returns> Object that is derived from LocPathIterator.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static LocPathIterator createUnionIterator(org.apache.xpath.compiler.Compiler compiler, int opPos) throws javax.xml.transform.TransformerException
	  public static LocPathIterator createUnionIterator(Compiler compiler, int opPos)
	  {
		  // For the moment, I'm going to first create a full UnionPathIterator, and 
		  // then see if I can reduce it to a UnionChildIterator.  It would obviously 
		  // be more effecient to just test for the conditions for a UnionChildIterator, 
		  // and then create that directly.
		  UnionPathIterator upi = new UnionPathIterator(compiler, opPos);
		  int nPaths = upi.m_exprs.Length;
		  bool isAllChildIterators = true;
		  for (int i = 0; i < nPaths; i++)
		  {
			  LocPathIterator lpi = upi.m_exprs[i];

			  if (lpi.Axis != Axis.CHILD)
			  {
				  isAllChildIterators = false;
				  break;
			  }
			  else
			  {
				  // check for positional predicates or position function, which won't work.
				  if (HasPositionalPredChecker.check(lpi))
				  {
					  isAllChildIterators = false;
					  break;
				  }
			  }
		  }
		  if (isAllChildIterators)
		  {
			  UnionChildIterator uci = new UnionChildIterator();

			  for (int i = 0; i < nPaths; i++)
			  {
				  PredicatedNodeTest lpi = upi.m_exprs[i];
				  // I could strip the lpi down to a pure PredicatedNodeTest, but 
				  // I don't think it's worth it.  Note that the test can be used 
				  // as a static object... so it doesn't have to be cloned.
				  uci.addNodeTest(lpi);
			  }
			  return uci;

		  }
		  else
		  {
			  return upi;
		  }
	  }

	  /// <summary>
	  /// Get the analysis bits for this walker, as defined in the WalkerFactory. </summary>
	  /// <returns> One of WalkerFactory#BIT_DESCENDANT, etc. </returns>
	  public override int AnalysisBits
	  {
		  get
		  {
			int bits = 0;
    
			if (m_exprs != null)
			{
			  int n = m_exprs.Length;
    
			  for (int i = 0; i < n; i++)
			  {
				  int bit = m_exprs[i].AnalysisBits;
				bits |= bit;
			  }
			}
    
			return bits;
		  }
	  }

	  /// <summary>
	  /// Read the object from a serialization stream.
	  /// </summary>
	  /// <param name="stream"> Input stream to read from
	  /// </param>
	  /// <exception cref="java.io.IOException"> </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, javax.xml.transform.TransformerException
	  private void readObject(java.io.ObjectInputStream stream)
	  {
		try
		{
		  stream.defaultReadObject();
		  m_clones = new IteratorPool(this);
		}
		catch (ClassNotFoundException cnfe)
		{
		  throw new javax.xml.transform.TransformerException(cnfe);
		}
	  }

	  /// <summary>
	  /// Get a cloned LocPathIterator that holds the same 
	  /// position as this iterator.
	  /// </summary>
	  /// <returns> A clone of this iterator that holds the same node position.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {

		UnionPathIterator clone = (UnionPathIterator) base.clone();
		if (m_iterators != null)
		{
		  int n = m_iterators.Length;

		  clone.m_iterators = new DTMIterator[n];

		  for (int i = 0; i < n; i++)
		  {
			clone.m_iterators[i] = (DTMIterator)m_iterators[i].clone();
		  }
		}

		return clone;
	  }


	  /// <summary>
	  /// Create a new location path iterator.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating 
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the 
	  /// </param>
	  /// <returns> New location path iterator.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected LocPathIterator createDTMIterator(org.apache.xpath.compiler.Compiler compiler, int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual LocPathIterator createDTMIterator(Compiler compiler, int opPos)
	  {
		LocPathIterator lpi = (LocPathIterator)WalkerFactory.newDTMIterator(compiler, opPos, (compiler.LocationPathDepth <= 0));
		return lpi;
	  }

	  /// <summary>
	  /// Initialize the location path iterators.  Recursive.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating 
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the 
	  /// opcode list from the compiler. </param>
	  /// <param name="count"> The insert position of the iterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void loadLocationPaths(org.apache.xpath.compiler.Compiler compiler, int opPos, int count) throws javax.xml.transform.TransformerException
	  protected internal virtual void loadLocationPaths(Compiler compiler, int opPos, int count)
	  {

		// TODO: Handle unwrapped FilterExpr
		int steptype = compiler.getOp(opPos);

		if (steptype == OpCodes.OP_LOCATIONPATH)
		{
		  loadLocationPaths(compiler, compiler.getNextOpPos(opPos), count + 1);

		  m_exprs[count] = createDTMIterator(compiler, opPos);
		  m_exprs[count].exprSetParent(this);
		}
		else
		{

		  // Have to check for unwrapped functions, which the LocPathIterator
		  // doesn't handle. 
		  switch (steptype)
		  {
		  case OpCodes.OP_VARIABLE :
		  case OpCodes.OP_EXTFUNCTION :
		  case OpCodes.OP_FUNCTION :
		  case OpCodes.OP_GROUP :
			loadLocationPaths(compiler, compiler.getNextOpPos(opPos), count + 1);

			WalkingIterator iter = new WalkingIterator(compiler.NamespaceContext);
			iter.exprSetParent(this);

			if (compiler.LocationPathDepth <= 0)
			{
			  iter.IsTopLevel = true;
			}

			iter.m_firstWalker = new org.apache.xpath.axes.FilterExprWalker(iter);

			iter.m_firstWalker.init(compiler, opPos, steptype);

			m_exprs[count] = iter;
			break;
		  default :
			m_exprs = new LocPathIterator[count];
		break;
		  }
		}
	  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a DTMIterator is created, the first call
	  /// to nextNode() returns the first node in the set. </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set. </returns>
	  public override int nextNode()
	  {
		  if (m_foundLast)
		  {
			  return org.apache.xml.dtm.DTM_Fields.NULL;
		  }

		// Loop through the iterators getting the current fetched 
		// node, and get the earliest occuring in document order
		int earliestNode = org.apache.xml.dtm.DTM_Fields.NULL;

		if (null != m_iterators)
		{
		  int n = m_iterators.Length;
		  int iteratorUsed = -1;

		  for (int i = 0; i < n; i++)
		  {
			int node = m_iterators[i].CurrentNode;

			if (org.apache.xml.dtm.DTM_Fields.NULL == node)
			{
			  continue;
			}
			else if (org.apache.xml.dtm.DTM_Fields.NULL == earliestNode)
			{
			  iteratorUsed = i;
			  earliestNode = node;
			}
			else
			{
			  if (node == earliestNode)
			  {

				// Found a duplicate, so skip past it.
				m_iterators[i].nextNode();
			  }
			  else
			  {
				DTM dtm = getDTM(node);

				if (dtm.isNodeAfter(node, earliestNode))
				{
				  iteratorUsed = i;
				  earliestNode = node;
				}
			  }
			}
		  }

		  if (org.apache.xml.dtm.DTM_Fields.NULL != earliestNode)
		  {
			m_iterators[iteratorUsed].nextNode();

			incrementCurrentPos();
		  }
		  else
		  {
			m_foundLast = true;
		  }
		}

		m_lastFetched = earliestNode;

		return earliestNode;
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
		for (int i = 0; i < m_exprs.Length; i++)
		{
		  m_exprs[i].fixupVariables(vars, globalsSize);
		}

	  }

	  /// <summary>
	  /// The location path iterators, one for each
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath">location
	  /// path</a> contained in the union expression.
	  /// @serial
	  /// </summary>
	  protected internal LocPathIterator[] m_exprs;


	  /// <summary>
	  /// The location path iterators, one for each
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath">location
	  /// path</a> contained in the union expression.
	  /// @serial
	  /// </summary>
	  protected internal DTMIterator[] m_iterators;

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public override int Axis
	  {
		  get
		  {
			// Could be smarter.
			return -1;
		  }
	  }

	  internal class iterOwner : ExpressionOwner
	  {
		  private readonly UnionPathIterator outerInstance;

		  internal int m_index;

		  internal iterOwner(UnionPathIterator outerInstance, int index)
		  {
			  this.outerInstance = outerInstance;
			  m_index = index;
		  }

		/// <seealso cref= ExpressionOwner#getExpression() </seealso>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_exprs[m_index];
			}
			set
			{
    
				if (!(value is LocPathIterator))
				{
					// Yuck.  Need FilterExprIter.  Or make it so m_exprs can be just 
					// plain expressions?
					WalkingIterator wi = new WalkingIterator(outerInstance.PrefixResolver);
					FilterExprWalker few = new FilterExprWalker(wi);
					wi.FirstWalker = few;
					few.InnerExpression = value;
					wi.exprSetParent(outerInstance);
					few.exprSetParent(wi);
					value.exprSetParent(few);
					value = wi;
				}
				else
				{
					value.exprSetParent(outerInstance);
				}
				outerInstance.m_exprs[m_index] = (LocPathIterator)value;
			}
		}


	  }

	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
			   if (visitor.visitUnionPath(owner, this))
			   {
				   if (null != m_exprs)
				   {
					   int n = m_exprs.Length;
					   for (int i = 0; i < n; i++)
					   {
						   m_exprs[i].callVisitors(new iterOwner(this, i), visitor);
					   }
				   }
			   }
	  }

		/// <seealso cref= Expression#deepEquals(Expression) </seealso>
		public override bool deepEquals(Expression expr)
		{
		  if (!base.deepEquals(expr))
		  {
				return false;
		  }

		  UnionPathIterator upi = (UnionPathIterator) expr;

		  if (null != m_exprs)
		  {
			int n = m_exprs.Length;

			if ((null == upi.m_exprs) || (upi.m_exprs.Length != n))
			{
				return false;
			}

			for (int i = 0; i < n; i++)
			{
			  if (!m_exprs[i].deepEquals(upi.m_exprs[i]))
			  {
				  return false;
			  }
			}
		  }
		  else if (null != upi.m_exprs)
		  {
			  return false;
		  }

		  return true;
		}


	}

}