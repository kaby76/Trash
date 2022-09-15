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
 * $Id: WalkingIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{
	using DTM = org.apache.xml.dtm.DTM;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpMap = org.apache.xpath.compiler.OpMap;

	/// <summary>
	/// Location path iterator that uses Walkers.
	/// </summary>

	[Serializable]
	public class WalkingIterator : LocPathIterator, ExpressionOwner
	{
		internal new const long serialVersionUID = 9110225941815665906L;
	  /// <summary>
	  /// Create a WalkingIterator iterator, including creation
	  /// of step walkers from the opcode list, and call back
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the
	  /// opcode list from the compiler. </param>
	  /// <param name="shouldLoadWalkers"> True if walkers should be
	  /// loaded, or false if this is a derived iterator and
	  /// it doesn't wish to load child walkers.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: WalkingIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis, boolean shouldLoadWalkers) throws javax.xml.transform.TransformerException
	  internal WalkingIterator(Compiler compiler, int opPos, int analysis, bool shouldLoadWalkers) : base(compiler, opPos, analysis, shouldLoadWalkers)
	  {

		int firstStepPos = OpMap.getFirstChildPos(opPos);

		if (shouldLoadWalkers)
		{
		  m_firstWalker = WalkerFactory.loadWalkers(this, compiler, firstStepPos, 0);
		  m_lastUsedWalker = m_firstWalker;
		}
	  }

	  /// <summary>
	  /// Create a WalkingIterator object.
	  /// </summary>
	  /// <param name="nscontext"> The namespace context for this iterator,
	  /// should be OK if null. </param>
	  public WalkingIterator(PrefixResolver nscontext) : base(nscontext)
	  {

	  }


	  /// <summary>
	  /// Get the analysis bits for this walker, as defined in the WalkerFactory. </summary>
	  /// <returns> One of WalkerFactory#BIT_DESCENDANT, etc. </returns>
	  public override int AnalysisBits
	  {
		  get
		  {
			int bits = 0;
			if (null != m_firstWalker)
			{
			  AxesWalker walker = m_firstWalker;
    
			  while (null != walker)
			  {
				int bit = walker.AnalysisBits;
				bits |= bit;
				walker = walker.NextWalker;
			  }
			}
			return bits;
		  }
	  }

	  /// <summary>
	  /// Get a cloned WalkingIterator that holds the same
	  /// position as this iterator.
	  /// </summary>
	  /// <returns> A clone of this iterator that holds the same node position.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {

		WalkingIterator clone = (WalkingIterator) base.clone();

		//    clone.m_varStackPos = this.m_varStackPos;
		//    clone.m_varStackContext = this.m_varStackContext;
		if (null != m_firstWalker)
		{
		  clone.m_firstWalker = m_firstWalker.cloneDeep(clone, null);
		}

		return clone;
	  }

	  /// <summary>
	  /// Reset the iterator.
	  /// </summary>
	  public override void reset()
	  {

		base.reset();

		if (null != m_firstWalker)
		{
		  m_lastUsedWalker = m_firstWalker;

		  m_firstWalker.Root = m_context;
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

		if (null != m_firstWalker)
		{
		  m_firstWalker.Root = context;
		  m_lastUsedWalker = m_firstWalker;
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

		// If the variable stack position is not -1, we'll have to 
		// set our position in the variable stack, so our variable access 
		// will be correct.  Iterators that are at the top level of the 
		// expression need to reset the variable stack, while iterators 
		// in predicates do not need to, and should not, since their execution
		// may be much later than top-level iterators.  
		// m_varStackPos is set in setRoot, which is called 
		// from the execute method.
		if (-1 == m_stackFrame)
		{
		  return returnNextNode(m_firstWalker.nextNode());
		}
		else
		{
		  VariableStack vars = m_execContext.VarStack;

		  // These three statements need to be combined into one operation.
		  int savedStart = vars.StackFrame;

		  vars.StackFrame = m_stackFrame;

		  int n = returnNextNode(m_firstWalker.nextNode());

		  // These two statements need to be combined into one operation.
		  vars.StackFrame = savedStart;

		  return n;
		}
	  }


	  /// <summary>
	  /// Get the head of the walker list.
	  /// </summary>
	  /// <returns> The head of the walker list, or null
	  /// if this iterator does not implement walkers.
	  /// @xsl.usage advanced </returns>
	  public AxesWalker FirstWalker
	  {
		  get
		  {
			return m_firstWalker;
		  }
		  set
		  {
			m_firstWalker = value;
		  }
	  }



	  /// <summary>
	  /// Set the last used walker.
	  /// </summary>
	  /// <param name="walker"> The last used walker, or null.
	  /// @xsl.usage advanced </param>
	  public AxesWalker LastUsedWalker
	  {
		  set
		  {
			m_lastUsedWalker = value;
		  }
		  get
		  {
			return m_lastUsedWalker;
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
			  AxesWalker walker = m_firstWalker;
			while (null != walker)
			{
			  walker.detach();
			  walker = walker.NextWalker;
			}

			m_lastUsedWalker = null;

			// Always call the superclass detach last!
			base.detach();
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
		m_predicateIndex = -1;

		AxesWalker walker = m_firstWalker;

		while (null != walker)
		{
		  walker.fixupVariables(vars, globalsSize);
		  walker = walker.NextWalker;
		}
	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
			   if (visitor.visitLocationPath(owner, this))
			   {
				   if (null != m_firstWalker)
				   {
					   m_firstWalker.callVisitors(this, visitor);
				   }
			   }
	  }


	  /// <summary>
	  /// The last used step walker in the walker list.
	  ///  @serial 
	  /// </summary>
	  protected internal AxesWalker m_lastUsedWalker;

	  /// <summary>
	  /// The head of the step walker list.
	  ///  @serial 
	  /// </summary>
	  protected internal AxesWalker m_firstWalker;

	  /// <seealso cref="ExpressionOwner.getExpression()"/>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_firstWalker;
		  }
		  set
		  {
			  value.exprSetParent(this);
			  m_firstWalker = (AxesWalker)value;
		  }
	  }


		/// <seealso cref="Expression.deepEquals(Expression)"/>
		public override bool deepEquals(Expression expr)
		{
		  if (!base.deepEquals(expr))
		  {
					return false;
		  }

		  AxesWalker walker1 = m_firstWalker;
		  AxesWalker walker2 = ((WalkingIterator)expr).m_firstWalker;
		  while ((null != walker1) && (null != walker2))
		  {
			if (!walker1.deepEquals(walker2))
			{
				return false;
			}
			walker1 = walker1.NextWalker;
			walker2 = walker2.NextWalker;
		  }

		  if ((null != walker1) || (null != walker2))
		  {
			  return false;
		  }

		  return true;
		}

	}

}