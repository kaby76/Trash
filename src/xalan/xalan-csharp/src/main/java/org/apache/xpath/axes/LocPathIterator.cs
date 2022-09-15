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
 * $Id: LocPathIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class extends NodeSetDTM, which implements NodeIterator,
	/// and fetches nodes one at a time in document order based on a XPath
	/// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a>.
	/// 
	/// <para>If setShouldCacheNodes(true) is called,
	/// as each node is iterated via nextNode(), the node is also stored
	/// in the NodeVector, so that previousNode() can easily be done, except in
	/// the case where the LocPathIterator is "owned" by a UnionPathIterator,
	/// in which case the UnionPathIterator will cache the nodes.</para>
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public abstract class LocPathIterator : PredicatedNodeTest, Cloneable, DTMIterator, PathComponent
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			m_clones = new IteratorPool(this);
		}

		internal new const long serialVersionUID = -4602476357268405754L;

	  /// <summary>
	  /// Create a LocPathIterator object.
	  /// 
	  /// </summary>
	  protected internal LocPathIterator()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }


	  /// <summary>
	  /// Create a LocPathIterator object.
	  /// </summary>
	  /// <param name="nscontext"> The namespace context for this iterator,
	  /// should be OK if null. </param>
	  protected internal LocPathIterator(PrefixResolver nscontext)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }

		LocPathIterator = this;
		m_prefixResolver = nscontext;
	  }

	  /// <summary>
	  /// Create a LocPathIterator object, including creation
	  /// of step walkers from the opcode list, and call back
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the
	  /// opcode list from the compiler.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected LocPathIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  protected internal LocPathIterator(Compiler compiler, int opPos, int analysis) : this(compiler, opPos, analysis, true)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  /// <summary>
	  /// Create a LocPathIterator object, including creation
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
//ORIGINAL LINE: protected LocPathIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis, boolean shouldLoadWalkers) throws javax.xml.transform.TransformerException
	  protected internal LocPathIterator(Compiler compiler, int opPos, int analysis, bool shouldLoadWalkers)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		LocPathIterator = this;
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
		  m_clones = new IteratorPool(this);
		}
		catch (ClassNotFoundException cnfe)
		{
		  throw new javax.xml.transform.TransformerException(cnfe);
		}
	  }

	  /// <summary>
	  /// Set the environment in which this iterator operates, which should provide:
	  /// a node (the context node... same value as "root" defined below) 
	  /// a pair of non-zero positive integers (the context position and the context size) 
	  /// a set of variable bindings 
	  /// a function library 
	  /// the set of namespace declarations in scope for the expression.
	  /// 
	  /// <para>At this time the exact implementation of this environment is application 
	  /// dependent.  Probably a proper interface will be created fairly soon.</para>
	  /// </summary>
	  /// <param name="environment"> The environment object. </param>
	  public virtual object Environment
	  {
		  set
		  {
			// no-op for now.
		  }
	  }

	  /// <summary>
	  /// Get an instance of a DTM that "owns" a node handle.  Since a node 
	  /// iterator may be passed without a DTMManager, this allows the 
	  /// caller to easily get the DTM using just the iterator.
	  /// </summary>
	  /// <param name="nodeHandle"> the nodeHandle.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public virtual DTM getDTM(int nodeHandle)
	  {
		// %OPT%
		return m_execContext.getDTM(nodeHandle);
	  }

	  /// <summary>
	  /// Get an instance of the DTMManager.  Since a node 
	  /// iterator may be passed without a DTMManager, this allows the 
	  /// caller to easily get the DTMManager using just the iterator.
	  /// </summary>
	  /// <returns> a non-null DTMManager reference. </returns>
	  public virtual DTMManager DTMManager
	  {
		  get
		  {
			return m_execContext.DTMManager;
		  }
	  }

	  /// <summary>
	  /// Execute this iterator, meaning create a clone that can
	  /// store state, and initialize it for fast execution from
	  /// the current runtime state.  When this is called, no actual
	  /// query from the current context node is performed.
	  /// </summary>
	  /// <param name="xctxt"> The XPath execution context.
	  /// </param>
	  /// <returns> An XNodeSet reference that holds this iterator.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		XNodeSet iter = new XNodeSet((LocPathIterator)m_clones.Instance);

		iter.setRoot(xctxt.CurrentNode, xctxt);

		return iter;
	  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="handler"> The target content handler.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void executeCharsToContentHandler(org.apache.xpath.XPathContext xctxt, org.xml.sax.ContentHandler handler) throws javax.xml.transform.TransformerException, org.xml.sax.SAXException
	  public override void executeCharsToContentHandler(XPathContext xctxt, org.xml.sax.ContentHandler handler)
	  {
		LocPathIterator clone = (LocPathIterator)m_clones.Instance;

		int current = xctxt.CurrentNode;
		clone.setRoot(current, xctxt);

		int node = clone.nextNode();
		DTM dtm = clone.getDTM(node);
		clone.detach();

		if (node != DTM.NULL)
		{
		  dtm.dispatchCharactersEvents(node, handler, false);
		}
	  }

	  /// <summary>
	  /// Given an select expression and a context, evaluate the XPath
	  /// and return the resulting iterator.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <exception cref="TransformerException"> thrown if the active ProblemListener decides
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator asIterator(org.apache.xpath.XPathContext xctxt, int contextNode) throws javax.xml.transform.TransformerException
	  public override DTMIterator asIterator(XPathContext xctxt, int contextNode)
	  {
		XNodeSet iter = new XNodeSet((LocPathIterator)m_clones.Instance);

		iter.setRoot(contextNode, xctxt);

		return iter;
	  }


	  /// <summary>
	  /// Tell if the expression is a nodeset expression.
	  /// </summary>
	  /// <returns> true if the expression can be represented as a nodeset. </returns>
	  public override bool NodesetExpr
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Return the first node out of the nodeset, if this expression is 
	  /// a nodeset expression.  This is the default implementation for 
	  /// nodesets.  Derived classes should try and override this and return a 
	  /// value without having to do a clone operation. </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> the first node out of the nodeset, or DTM.NULL. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int asNode(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override int asNode(XPathContext xctxt)
	  {
		DTMIterator iter = (DTMIterator)m_clones.Instance;

		int current = xctxt.CurrentNode;

		iter.setRoot(current, xctxt);

		int next = iter.nextNode();
		// m_clones.freeInstance(iter);
		iter.detach();
		return next;
	  }

	  /// <summary>
	  /// Evaluate this operation directly to a boolean.
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> The result of the operation as a boolean.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean bool(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override bool @bool(XPathContext xctxt)
	  {
		return (asNode(xctxt) != DTM.NULL);
	  }


	  /// <summary>
	  /// Set if this is an iterator at the upper level of
	  /// the XPath.
	  /// </summary>
	  /// <param name="b"> true if this location path is at the top level of the
	  ///          expression.
	  /// @xsl.usage advanced </param>
	  public virtual bool IsTopLevel
	  {
		  set
		  {
			m_isTopLevel = value;
		  }
		  get
		  {
			return m_isTopLevel;
		  }
	  }


	  /// <summary>
	  /// Initialize the context values for this expression
	  /// after it is cloned.
	  /// </summary>
	  /// <param name="context"> The XPath runtime context for this
	  /// transformation. </param>
	  public virtual void setRoot(int context, object environment)
	  {

		m_context = context;

		XPathContext xctxt = (XPathContext)environment;
		m_execContext = xctxt;
		m_cdtm = xctxt.getDTM(context);

		m_currentContextNode = context; // only if top level?

		// Yech, shouldn't have to do this.  -sb
		if (null == m_prefixResolver)
		{
			m_prefixResolver = xctxt.NamespaceContext;
		}

		m_lastFetched = DTM.NULL;
		m_foundLast = false;
		m_pos = 0;
		m_length = -1;

		if (m_isTopLevel)
		{
		  this.m_stackFrame = xctxt.VarStack.StackFrame;
		}

		// reset();
	  }

	  /// <summary>
	  /// Set the next position index of this iterator.
	  /// </summary>
	  /// <param name="next"> A value greater than or equal to zero that indicates the next
	  /// node position to fetch. </param>
	  protected internal virtual int NextPosition
	  {
		  set
		  {
			assertion(false, "setNextPosition not supported in this iterator!");
		  }
	  }

	  /// <summary>
	  /// Get the current position, which is one less than
	  /// the next nextNode() call will retrieve.  i.e. if
	  /// you call getCurrentPos() and the return is 0, the next
	  /// fetch will take place at index 1.
	  /// </summary>
	  /// <returns> A value greater than or equal to zero that indicates the next
	  /// node position to fetch. </returns>
	  public int CurrentPos
	  {
		  get
		  {
			return m_pos;
		  }
		  set
		  {
			  assertion(false, "setCurrentPos not supported by this iterator!");
		  }
	  }


	  /// <summary>
	  /// If setShouldCacheNodes(true) is called, then nodes will
	  /// be cached.  They are not cached by default.
	  /// </summary>
	  /// <param name="b"> True if this iterator should cache nodes. </param>
	  public virtual bool ShouldCacheNodes
	  {
		  set
		  {
    
			assertion(false, "setShouldCacheNodes not supported by this iterater!");
		  }
	  }

	  /// <summary>
	  /// Tells if this iterator can have nodes added to it or set via 
	  /// the <code>setItem(int node, int index)</code> method.
	  /// </summary>
	  /// <returns> True if the nodelist can be mutated. </returns>
	  public virtual bool Mutable
	  {
		  get
		  {
			return false;
		  }
	  }


	  /// <summary>
	  /// Increment the current position in the node set.
	  /// </summary>
	  public virtual void incrementCurrentPos()
	  {
		  m_pos++;
	  }


	  /// <summary>
	  /// Get the length of the cached nodes.
	  /// 
	  /// <para>Note: for the moment at least, this only returns
	  /// the size of the nodes that have been fetched to date,
	  /// it doesn't attempt to run to the end to make sure we
	  /// have found everything.  This should be reviewed.</para>
	  /// </summary>
	  /// <returns> The size of the current cache list. </returns>
	  public virtual int size()
	  {
		assertion(false, "size() not supported by this iterator!");
		return 0;
	  }

	  /// <summary>
	  ///  Returns the <code>index</code> th item in the collection. If
	  /// <code>index</code> is greater than or equal to the number of nodes in
	  /// the list, this returns <code>null</code> . </summary>
	  /// <param name="index">  Index into the collection. </param>
	  /// <returns>  The node at the <code>index</code> th position in the
	  ///   <code>NodeList</code> , or <code>null</code> if that is not a valid
	  ///   index. </returns>
	  public virtual int item(int index)
	  {
		assertion(false, "item(int index) not supported by this iterator!");
		return 0;
	  }

	  /// <summary>
	  /// Sets the node at the specified index of this vector to be the
	  /// specified node. The previous component at that position is discarded.
	  /// 
	  /// <para>The index must be a value greater than or equal to 0 and less
	  /// than the current size of the vector.  
	  /// The iterator must be in cached mode.</para>
	  /// 
	  /// <para>Meant to be used for sorted iterators.</para>
	  /// </summary>
	  /// <param name="node"> Node to set </param>
	  /// <param name="index"> Index of where to set the node </param>
	  public virtual void setItem(int node, int index)
	  {
		assertion(false, "setItem not supported by this iterator!");
	  }

	  /// <summary>
	  ///  The number of nodes in the list. The range of valid child node indices
	  /// is 0 to <code>length-1</code> inclusive.
	  /// </summary>
	  /// <returns> The number of nodes in the list, always greater or equal to zero. </returns>
	  public virtual int Length
	  {
		  get
		  {
			// Tell if this is being called from within a predicate.
			  bool isPredicateTest = (this == m_execContext.SubContextList);
    
			// And get how many total predicates are part of this step.
			  int predCount = PredicateCount;
    
			// If we have already calculated the length, and the current predicate 
			// is the first predicate, then return the length.  We don't cache 
			// the anything but the length of the list to the first predicate.
			if (-1 != m_length && isPredicateTest && m_predicateIndex < 1)
			{
				  return m_length;
			}
    
			// I'm a bit worried about this one, since it doesn't have the 
			// checks found above.  I suspect it's fine.  -sb
			if (m_foundLast)
			{
				  return m_pos;
			}
    
			// Create a clone, and count from the current position to the end 
			// of the list, not taking into account the current predicate and 
			// predicates after the current one.
			int pos = (m_predicateIndex >= 0) ? ProximityPosition : m_pos;
    
			LocPathIterator clone;
    
			try
			{
			  clone = (LocPathIterator) this.clone();
			}
			catch (CloneNotSupportedException)
			{
			  return -1;
			}
    
			// We want to clip off the last predicate, but only if we are a sub 
			// context node list, NOT if we are a context list.  See pos68 test, 
			// also test against bug4638.
			if (predCount > 0 && isPredicateTest)
			{
			  // Don't call setPredicateCount, because it clones and is slower.
			  clone.m_predCount = m_predicateIndex;
			  // The line above used to be:
			  // clone.m_predCount = predCount - 1;
			  // ...which looks like a dumb bug to me. -sb
			}
    
			int next;
    
			while (DTM.NULL != (next = clone.nextNode()))
			{
			  pos++;
			}
    
			if (isPredicateTest && m_predicateIndex < 1)
			{
			  m_length = pos;
			}
    
			return pos;
		  }
	  }

	  /// <summary>
	  /// Tells if this NodeSetDTM is "fresh", in other words, if
	  /// the first nextNode() that is called will return the
	  /// first node in the set.
	  /// </summary>
	  /// <returns> true of nextNode has not been called. </returns>
	  public virtual bool Fresh
	  {
		  get
		  {
			return (m_pos == 0);
		  }
	  }

	  /// <summary>
	  ///  Returns the previous node in the set and moves the position of the
	  /// iterator backwards in the set. </summary>
	  /// <returns>  The previous <code>Node</code> in the set being iterated over,
	  ///   or<code>null</code> if there are no more members in that set. </returns>
	  public virtual int previousNode()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_CANNOT_ITERATE, null)); //"This NodeSetDTM can not iterate to a previous node!");
	  }

	  /// <summary>
	  /// This attribute determines which node types are presented via the
	  /// iterator. The available set of constants is defined in the
	  /// <code>NodeFilter</code> interface.
	  /// 
	  /// <para>This is somewhat useless at this time, since it doesn't
	  /// really return information that tells what this iterator will
	  /// show.  It is here only to fullfill the DOM NodeIterator
	  /// interface.</para>
	  /// </summary>
	  /// <returns> For now, always NodeFilter.SHOW_ALL & ~NodeFilter.SHOW_ENTITY_REFERENCE. </returns>
	  /// <seealso cref="org.w3c.dom.traversal.NodeIterator"/>
	  public override int WhatToShow
	  {
		  get
		  {
    
			// TODO: ??
			return DTMFilter.SHOW_ALL & ~DTMFilter.SHOW_ENTITY_REFERENCE;
		  }
	  }

	  /// <summary>
	  ///  The filter used to screen nodes.  Not used at this time,
	  /// this is here only to fullfill the DOM NodeIterator
	  /// interface.
	  /// </summary>
	  /// <returns> Always null. </returns>
	  /// <seealso cref="org.w3c.dom.traversal.NodeIterator"/>
	  public virtual DTMFilter Filter
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// The root node of the Iterator, as specified when it was created.
	  /// </summary>
	  /// <returns> The "root" of this iterator, which, in XPath terms,
	  /// is the node context for this iterator. </returns>
	  public virtual int Root
	  {
		  get
		  {
			return m_context;
		  }
	  }

	  /// <summary>
	  ///  The value of this flag determines whether the children of entity
	  /// reference nodes are visible to the iterator. If false, they will be
	  /// skipped over.
	  /// <br> To produce a view of the document that has entity references
	  /// expanded and does not expose the entity reference node itself, use the
	  /// whatToShow flags to hide the entity reference node and set
	  /// expandEntityReferences to true when creating the iterator. To produce
	  /// a view of the document that has entity reference nodes but no entity
	  /// expansion, use the whatToShow flags to show the entity reference node
	  /// and set expandEntityReferences to false.
	  /// </summary>
	  /// <returns> Always true, since entity reference nodes are not
	  /// visible in the XPath model. </returns>
	  public virtual bool ExpandEntityReferences
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Control over whether it is OK for detach to reset the iterator. </summary>
	  protected internal bool m_allowDetach = true;

	  /// <summary>
	  /// Specify if it's OK for detach to release the iterator for reuse.
	  /// </summary>
	  /// <param name="allowRelease"> true if it is OK for detach to release this iterator 
	  /// for pooling. </param>
	  public virtual void allowDetachToRelease(bool allowRelease)
	  {
		m_allowDetach = allowRelease;
	  }

	  /// <summary>
	  ///  Detaches the iterator from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state. After<code>detach</code> has been invoked, calls to
	  /// <code>nextNode</code> or<code>previousNode</code> will raise the
	  /// exception INVALID_STATE_ERR.
	  /// </summary>
	  public virtual void detach()
	  {
		if (m_allowDetach)
		{
		  // sb: allow reusing of cached nodes when possible?
		  // m_cachedNodes = null;
		  m_execContext = null;
		  // m_prefixResolver = null;  sb: Why would this ever want to be null?
		  m_cdtm = null;
		  m_length = -1;
		  m_pos = 0;
		  m_lastFetched = DTM.NULL;
		  m_context = DTM.NULL;
		  m_currentContextNode = DTM.NULL;

		  m_clones.freeInstance(this);
		}
	  }

	  /// <summary>
	  /// Reset the iterator.
	  /// </summary>
	  public virtual void reset()
	  {
		  assertion(false, "This iterator can not reset!");
	  }

	  /// <summary>
	  /// Get a cloned Iterator that is reset to the beginning
	  /// of the query.
	  /// </summary>
	  /// <returns> A cloned NodeIterator set of the start of the query.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public virtual DTMIterator cloneWithReset()
	  {
		LocPathIterator clone;
	//    clone = (LocPathIterator) clone();
		clone = (LocPathIterator)m_clones.InstanceOrThrow;
		clone.m_execContext = m_execContext;
		clone.m_cdtm = m_cdtm;

		clone.m_context = m_context;
		clone.m_currentContextNode = m_currentContextNode;
		clone.m_stackFrame = m_stackFrame;

		// clone.reset();

		return clone;
	  }

	//  /**
	//   * Get a cloned LocPathIterator that holds the same
	//   * position as this iterator.
	//   *
	//   * @return A clone of this iterator that holds the same node position.
	//   *
	//   * @throws CloneNotSupportedException
	//   */
	//  public Object clone() throws CloneNotSupportedException
	//  {
	//
	//    LocPathIterator clone = (LocPathIterator) super.clone();
	//
	//    return clone;
	//  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a NodeIterator is created, the first call
	  /// to nextNode() returns the first node in the set. </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set. </returns>
	  public abstract int nextNode();

	  /// <summary>
	  /// Bottleneck the return of a next node, to make returns
	  /// easier from nextNode().
	  /// </summary>
	  /// <param name="nextNode"> The next node found, may be null.
	  /// </param>
	  /// <returns> The same node that was passed as an argument. </returns>
	  protected internal virtual int returnNextNode(int nextNode)
	  {

		if (DTM.NULL != nextNode)
		{
		  m_pos++;
		}

		m_lastFetched = nextNode;

		if (DTM.NULL == nextNode)
		{
		  m_foundLast = true;
		}

		return nextNode;
	  }

	  /// <summary>
	  /// Return the last fetched node.  Needed to support the UnionPathIterator.
	  /// </summary>
	  /// <returns> The last fetched node, or null if the last fetch was null. </returns>
	  public virtual int CurrentNode
	  {
		  get
		  {
			return m_lastFetched;
		  }
	  }

	  /// <summary>
	  /// If an index is requested, NodeSetDTM will call this method
	  /// to run the iterator to the index.  By default this sets
	  /// m_next to the index.  If the index argument is -1, this
	  /// signals that the iterator should be run to the end.
	  /// </summary>
	  /// <param name="index"> The index to run to, or -1 if the iterator
	  /// should run to the end. </param>
	  public virtual void runTo(int index)
	  {

		if (m_foundLast || ((index >= 0) && (index <= CurrentPos)))
		{
		  return;
		}

		int n;

		if (-1 == index)
		{
		  while (DTM.NULL != (n = nextNode()))
		  {
				  ;
		  }
		}
		else
		{
		  while (DTM.NULL != (n = nextNode()))
		  {
			if (CurrentPos >= index)
			{
			  break;
			}
		  }
		}
	  }

	  /// <summary>
	  /// Tells if we've found the last node yet.
	  /// </summary>
	  /// <returns> true if the last nextNode returned null. </returns>
	  public bool FoundLast
	  {
		  get
		  {
			return m_foundLast;
		  }
	  }

	  /// <summary>
	  /// The XPath execution context we are operating on.
	  /// </summary>
	  /// <returns> XPath execution context this iterator is operating on,
	  /// or null if setRoot has not been called. </returns>
	  public XPathContext XPathContext
	  {
		  get
		  {
			return m_execContext;
		  }
	  }

	  /// <summary>
	  /// The node context for the iterator.
	  /// </summary>
	  /// <returns> The node context, same as getRoot(). </returns>
	  public int Context
	  {
		  get
		  {
			return m_context;
		  }
	  }

	  /// <summary>
	  /// The node context from where the expression is being
	  /// executed from (i.e. for current() support).
	  /// </summary>
	  /// <returns> The top-level node context of the entire expression. </returns>
	  public int CurrentContextNode
	  {
		  get
		  {
			return m_currentContextNode;
		  }
		  set
		  {
			m_currentContextNode = value;
		  }
	  }


	//  /**
	//   * Set the current context node for this iterator.
	//   *
	//   * @param n Must be a non-null reference to the node context.
	//   */
	//  public void setRoot(int n)
	//  {
	//    m_context = n;
	//    m_cdtm = m_execContext.getDTM(n);
	//  }

	  /// <summary>
	  /// Return the saved reference to the prefix resolver that
	  /// was in effect when this iterator was created.
	  /// </summary>
	  /// <returns> The prefix resolver or this iterator, which may be null. </returns>
	  public PrefixResolver PrefixResolver
	  {
		  get
		  {
			  if (null == m_prefixResolver)
			  {
				m_prefixResolver = (PrefixResolver)ExpressionOwner;
			  }
    
			return m_prefixResolver;
		  }
	  }

	//  /**
	//   * Get the analysis pattern built by the WalkerFactory.
	//   *
	//   * @return The analysis pattern built by the WalkerFactory.
	//   */
	//  int getAnalysis()
	//  {
	//    return m_analysis;
	//  }

	//  /**
	//   * Set the analysis pattern built by the WalkerFactory.
	//   *
	//   * @param a The analysis pattern built by the WalkerFactory.
	//   */
	//  void setAnalysis(int a)
	//  {
	//    m_analysis = a;
	//  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
			   if (visitor.visitLocationPath(owner, this))
			   {
				   visitor.visitStep(owner, this);
				   callPredicateVisitors(visitor);
			   }
	  }


	  //============= State Data =============

	  /// <summary>
	  /// The pool for cloned iterators.  Iterators need to be cloned
	  /// because the hold running state, and thus the original iterator
	  /// expression from the stylesheet pool can not be used.          
	  /// </summary>
	  [NonSerialized]
	  protected internal IteratorPool m_clones;

	  /// <summary>
	  /// The dtm of the context node.  Careful about using this... it may not 
	  /// be the dtm of the current node.
	  /// </summary>
	  [NonSerialized]
	  protected internal DTM m_cdtm;

	  /// <summary>
	  /// The stack frame index for this iterator.
	  /// </summary>
	  [NonSerialized]
	  internal int m_stackFrame = -1;

	  /// <summary>
	  /// Value determined at compile time, indicates that this is an
	  /// iterator at the top level of the expression, rather than inside
	  /// a predicate.
	  /// @serial
	  /// </summary>
	  private bool m_isTopLevel = false;

	  /// <summary>
	  /// The last node that was fetched, usually by nextNode. </summary>
	  [NonSerialized]
	  public int m_lastFetched = DTM.NULL;

	  /// <summary>
	  /// The context node for this iterator, which doesn't change through
	  /// the course of the iteration.
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_context = DTM.NULL;

	  /// <summary>
	  /// The node context from where the expression is being
	  /// executed from (i.e. for current() support).  Different
	  /// from m_context in that this is the context for the entire
	  /// expression, rather than the context for the subexpression.
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_currentContextNode = DTM.NULL;

	  /// <summary>
	  /// The current position of the context node.
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_pos = 0;

	  [NonSerialized]
	  protected internal int m_length = -1;

	  /// <summary>
	  /// Fast access to the current prefix resolver.  It isn't really
	  /// clear that this is needed.
	  /// @serial
	  /// </summary>
	  private PrefixResolver m_prefixResolver;

	  /// <summary>
	  /// The XPathContext reference, needed for execution of many
	  /// operations.
	  /// </summary>
	  [NonSerialized]
	  protected internal XPathContext m_execContext;

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
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
			return -1;
		  }
	  }


	//  /**
	//   * The analysis pattern built by the WalkerFactory.
	//   * TODO: Move to LocPathIterator.
	//   * @see org.apache.xpath.axes.WalkerFactory
	//   * @serial
	//   */
	//  protected int m_analysis = 0x00000000;
	  /// <seealso cref="PredicatedNodeTest.getLastPos(XPathContext)"/>
	  public override int getLastPos(XPathContext xctxt)
	  {
		return Length;
	  }

	}

}