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
 * $Id: AxesWalker.java 513117 2007-03-01 03:28:52Z minchau $
 */
namespace org.apache.xpath.axes
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Serves as common interface for axes Walkers, and stores common
	/// state variables.
	/// </summary>
	[Serializable]
	public class AxesWalker : PredicatedNodeTest, ICloneable, PathComponent, ExpressionOwner
	{
		internal new const long serialVersionUID = -2966031951306601247L;

	  /// <summary>
	  /// Construct an AxesWalker using a LocPathIterator.
	  /// </summary>
	  /// <param name="locPathIterator"> non-null reference to the parent iterator. </param>
	  public AxesWalker(LocPathIterator locPathIterator, int axis) : base(locPathIterator)
	  {
		m_axis = axis;
	  }

	  public WalkingIterator wi()
	  {
		return (WalkingIterator)m_lpi;
	  }

	  /// <summary>
	  /// Initialize an AxesWalker during the parse of the XPath expression.
	  /// </summary>
	  /// <param name="compiler"> The Compiler object that has information about this 
	  ///                 walker in the op map. </param>
	  /// <param name="opPos"> The op code position of this location step. </param>
	  /// <param name="stepType">  The type of location step.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init(org.apache.xpath.compiler.Compiler compiler, int opPos, int stepType) throws javax.xml.transform.TransformerException
	  public virtual void init(Compiler compiler, int opPos, int stepType)
	  {

		initPredicateInfo(compiler, opPos);

		// int testType = compiler.getOp(nodeTestOpPos);
	  }

	  /// <summary>
	  /// Get a cloned AxesWalker.
	  /// </summary>
	  /// <returns> A new AxesWalker that can be used without mutating this one.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {
		// Do not access the location path itterator during this operation!

		AxesWalker clone = (AxesWalker) base.clone();

		//clone.setCurrentNode(clone.m_root);

		// clone.m_isFresh = true;

		return clone;
	  }

	  /// <summary>
	  /// Do a deep clone of this walker, including next and previous walkers.
	  /// If the this AxesWalker is on the clone list, don't clone but 
	  /// return the already cloned version.
	  /// </summary>
	  /// <param name="cloneOwner"> non-null reference to the cloned location path 
	  ///                   iterator to which this clone will be added. </param>
	  /// <param name="cloneList"> non-null vector of sources in odd elements, and the 
	  ///                  corresponding clones in even vectors.
	  /// </param>
	  /// <returns> non-null clone, which may be a new clone, or may be a clone 
	  ///         contained on the cloneList. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: AxesWalker cloneDeep(WalkingIterator cloneOwner, java.util.Vector cloneList) throws CloneNotSupportedException
	  internal virtual AxesWalker cloneDeep(WalkingIterator cloneOwner, ArrayList cloneList)
	  {
		AxesWalker clone = findClone(this, cloneList);
		if (null != clone)
		{
		  return clone;
		}
		clone = (AxesWalker)this.clone();
		clone.LocPathIterator = cloneOwner;
		if (null != cloneList)
		{
		  cloneList.Add(this);
		  cloneList.Add(clone);
		}

		if (wi().m_lastUsedWalker == this)
		{
		  cloneOwner.m_lastUsedWalker = clone;
		}

		if (null != m_nextWalker)
		{
		  clone.m_nextWalker = m_nextWalker.cloneDeep(cloneOwner, cloneList);
		}

		// If you don't check for the cloneList here, you'll go into an 
		// recursive infinate loop.  
		if (null != cloneList)
		{
		  if (null != m_prevWalker)
		  {
			clone.m_prevWalker = m_prevWalker.cloneDeep(cloneOwner, cloneList);
		  }
		}
		else
		{
		  if (null != m_nextWalker)
		  {
			clone.m_nextWalker.m_prevWalker = clone;
		  }
		}
		return clone;
	  }

	  /// <summary>
	  /// Find a clone that corresponds to the key argument.
	  /// </summary>
	  /// <param name="key"> The original AxesWalker for which there may be a clone. </param>
	  /// <param name="cloneList"> vector of sources in odd elements, and the 
	  ///                  corresponding clones in even vectors, may be null.
	  /// </param>
	  /// <returns> A clone that corresponds to the key, or null if key not found. </returns>
	  internal static AxesWalker findClone(AxesWalker key, ArrayList cloneList)
	  {
		if (null != cloneList)
		{
		  // First, look for clone on list.
		  int n = cloneList.Count;
		  for (int i = 0; i < n; i += 2)
		  {
			if (key == cloneList[i])
			{
			  return (AxesWalker)cloneList[i + 1];
			}
		  }
		}
		return null;
	  }

	  /// <summary>
	  /// Detaches the walker from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state.
	  /// </summary>
	  public virtual void detach()
	  {
		  m_currentNode = org.apache.xml.dtm.DTM_Fields.NULL;
		  m_dtm = null;
		  m_traverser = null;
		  m_isFresh = true;
		  m_root = org.apache.xml.dtm.DTM_Fields.NULL;
	  }

	  //=============== TreeWalker Implementation ===============

	  /// <summary>
	  /// The root node of the TreeWalker, as specified in setRoot(int root).
	  /// Note that this may actually be below the current node.
	  /// </summary>
	  /// <returns> The context node of the step. </returns>
	  public virtual int Root
	  {
		  get
		  {
			return m_root;
		  }
		  set
		  {
			// %OPT% Get this directly from the lpi.
			XPathContext xctxt = wi().XPathContext;
			m_dtm = xctxt.getDTM(value);
			m_traverser = m_dtm.getAxisTraverser(m_axis);
			m_isFresh = true;
			m_foundLast = false;
			m_root = value;
			m_currentNode = value;
    
			if (org.apache.xml.dtm.DTM_Fields.NULL == value)
			{
			  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_SETTING_WALKER_ROOT_TO_NULL, null)); //"\n !!!! Error! Setting the root of a walker to null!!!");
			}
    
			resetProximityPositions();
		  }
	  }

	  /// <summary>
	  /// Get the analysis bits for this walker, as defined in the WalkerFactory. </summary>
	  /// <returns> One of WalkerFactory#BIT_DESCENDANT, etc. </returns>
	  public virtual int AnalysisBits
	  {
		  get
		  {
			  int axis = Axis;
			  int bit = WalkerFactory.getAnalysisBitFromAxes(axis);
			  return bit;
		  }
	  }


	  /// <summary>
	  /// The node at which the TreeWalker is currently positioned.
	  /// <br> The value must not be null. Alterations to the DOM tree may cause
	  /// the current node to no longer be accepted by the TreeWalker's
	  /// associated filter. currentNode may also be explicitly set to any node,
	  /// whether or not it is within the subtree specified by the root node or
	  /// would be accepted by the filter and whatToShow flags. Further
	  /// traversal occurs relative to currentNode even if it is not part of the
	  /// current view by applying the filters in the requested direction (not
	  /// changing currentNode where no traversal is possible).
	  /// </summary>
	  /// <returns> The node at which the TreeWalker is currently positioned, only null 
	  /// if setRoot has not yet been called. </returns>
	  public int CurrentNode
	  {
		  get
		  {
			return m_currentNode;
		  }
	  }

	  /// <summary>
	  /// Set the next walker in the location step chain.
	  /// 
	  /// </summary>
	  /// <param name="walker"> Reference to AxesWalker derivative, or may be null. </param>
	  public virtual AxesWalker NextWalker
	  {
		  set
		  {
			m_nextWalker = value;
		  }
		  get
		  {
			return m_nextWalker;
		  }
	  }


	  /// <summary>
	  /// Set or clear the previous walker reference in the location step chain.
	  /// 
	  /// </summary>
	  /// <param name="walker"> Reference to previous walker reference in the location 
	  ///               step chain, or null. </param>
	  public virtual AxesWalker PrevWalker
	  {
		  set
		  {
			m_prevWalker = value;
		  }
		  get
		  {
			return m_prevWalker;
		  }
	  }


	  /// <summary>
	  /// This is simply a way to bottle-neck the return of the next node, for 
	  /// diagnostic purposes.
	  /// </summary>
	  /// <param name="n"> Node to return, or null.
	  /// </param>
	  /// <returns> The argument. </returns>
	  private int returnNextNode(int n)
	  {

		return n;
	  }

	  /// <summary>
	  /// Get the next node in document order on the axes.
	  /// </summary>
	  /// <returns> the next node in document order on the axes, or null. </returns>
	  protected internal virtual int NextNode
	  {
		  get
		  {
			if (m_foundLast)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}
    
			if (m_isFresh)
			{
			  m_currentNode = m_traverser.first(m_root);
			  m_isFresh = false;
			}
			// I shouldn't have to do this the check for current node, I think.
			// numbering\numbering24.xsl fails if I don't do this.  I think 
			// it occurs as the walkers are backing up. -sb
			else if (org.apache.xml.dtm.DTM_Fields.NULL != m_currentNode)
			{
			  m_currentNode = m_traverser.next(m_root, m_currentNode);
			}
    
			if (org.apache.xml.dtm.DTM_Fields.NULL == m_currentNode)
			{
			  this.m_foundLast = true;
			}
    
			return m_currentNode;
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
	  public virtual int nextNode()
	  {
		int nextNode = org.apache.xml.dtm.DTM_Fields.NULL;
		AxesWalker walker = wi().LastUsedWalker;

		while (true)
		{
		  if (null == walker)
		  {
			break;
		  }

		  nextNode = walker.NextNode;

		  if (org.apache.xml.dtm.DTM_Fields.NULL == nextNode)
		  {

			walker = walker.m_prevWalker;
		  }
		  else
		  {
			if (walker.acceptNode(nextNode) != org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT)
			{
			  continue;
			}

			if (null == walker.m_nextWalker)
			{
			  wi().LastUsedWalker = walker;

			  // return walker.returnNextNode(nextNode);
			  break;
			}
			else
			{
			  AxesWalker prev = walker;

			  walker = walker.m_nextWalker;

			  walker.Root = nextNode;

			  walker.m_prevWalker = prev;

			  continue;
			}
		  } // if(null != nextNode)
		} // while(null != walker)

		return nextNode;
	  }

	  //============= End TreeWalker Implementation =============

	  /// <summary>
	  /// Get the index of the last node that can be itterated to.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the index of the last node that can be itterated to. </returns>
	  public override int getLastPos(XPathContext xctxt)
	  {

		int pos = ProximityPosition;

		AxesWalker walker;

		try
		{
		  walker = (AxesWalker) clone();
		}
		catch (CloneNotSupportedException)
		{
		  return -1;
		}

		walker.PredicateCount = m_predicateIndex;
		walker.NextWalker = null;
		walker.PrevWalker = null;

		WalkingIterator lpi = wi();
		AxesWalker savedWalker = lpi.LastUsedWalker;

		try
		{
		  lpi.LastUsedWalker = walker;

		  int next;

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (next = walker.nextNode()))
		  {
			pos++;
		  }

		  // TODO: Should probably save this in the iterator.
		}
		finally
		{
		  lpi.LastUsedWalker = savedWalker;
		}

		// System.out.println("pos: "+pos);
		return pos;
	  }

	  //============= State Data =============

	  /// <summary>
	  /// The DTM for the root.  This can not be used, or must be changed, 
	  /// for the filter walker, or any walker that can have nodes 
	  /// from multiple documents.
	  /// Never, ever, access this value without going through getDTM(int node).
	  /// </summary>
	  private DTM m_dtm;

	  /// <summary>
	  /// Set the DTM for this walker.
	  /// </summary>
	  /// <param name="dtm"> Non-null reference to a DTM. </param>
	  public virtual DTM DefaultDTM
	  {
		  set
		  {
			m_dtm = value;
		  }
	  }

	  /// <summary>
	  /// Get the DTM for this walker.
	  /// </summary>
	  /// <returns> Non-null reference to a DTM. </returns>
	  public virtual DTM getDTM(int node)
	  {
		//
		return wi().XPathContext.getDTM(node);
	  }

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// Warning: This can only be called after setRoot has been called!
	  /// </summary>
	  /// <returns> true as a default. </returns>
	  public virtual bool DocOrdered
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public virtual int Axis
	  {
		  get
		  {
			return m_axis;
		  }
	  }

	  /// <summary>
	  /// This will traverse the heararchy, calling the visitor for 
	  /// each member.  If the called visitor method returns 
	  /// false, the subtree should not be called.
	  /// </summary>
	  /// <param name="owner"> The owner of the visitor, where that path may be 
	  ///              rewritten if needed. </param>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  if (visitor.visitStep(owner, this))
		  {
			  callPredicateVisitors(visitor);
			  if (null != m_nextWalker)
			  {
				  m_nextWalker.callVisitors(this, visitor);
			  }
		  }
	  }

	  /// <seealso cref= ExpressionOwner#getExpression() </seealso>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_nextWalker;
		  }
		  set
		  {
			  value.exprSetParent(this);
			  m_nextWalker = (AxesWalker)value;
		  }
	  }


		/// <seealso cref= Expression#deepEquals(Expression) </seealso>
		public override bool deepEquals(Expression expr)
		{
		  if (!base.deepEquals(expr))
		  {
					return false;
		  }

		  AxesWalker walker = (AxesWalker)expr;
		  if (this.m_axis != walker.m_axis)
		  {
			  return false;
		  }

		  return true;
		}

	  /// <summary>
	  ///  The root node of the TreeWalker, as specified when it was created.
	  /// </summary>
	  [NonSerialized]
	  internal int m_root = org.apache.xml.dtm.DTM_Fields.NULL;

	  /// <summary>
	  ///  The node at which the TreeWalker is currently positioned.
	  /// </summary>
	  [NonSerialized]
	  private int m_currentNode = org.apache.xml.dtm.DTM_Fields.NULL;

	  /// <summary>
	  /// True if an itteration has not begun. </summary>
	  [NonSerialized]
	  internal bool m_isFresh;

	  /// <summary>
	  /// The next walker in the location step chain.
	  ///  @serial  
	  /// </summary>
	  protected internal AxesWalker m_nextWalker;

	  /// <summary>
	  /// The previous walker in the location step chain, or null.
	  ///  @serial   
	  /// </summary>
	  internal AxesWalker m_prevWalker;

	  /// <summary>
	  /// The traversal axis from where the nodes will be filtered. </summary>
	  protected internal int m_axis = -1;

	  /// <summary>
	  /// The DTM inner traversal class, that corresponds to the super axis. </summary>
	  protected internal DTMAxisTraverser m_traverser;
	}

}