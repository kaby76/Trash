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
 * $Id: NodeSetDTM.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;


	/// <summary>
	/// <para>The NodeSetDTM class can act as either a NodeVector,
	/// NodeList, or NodeIterator.  However, in order for it to
	/// act as a NodeVector or NodeList, it's required that
	/// setShouldCacheNodes(true) be called before the first
	/// nextNode() is called, in order that nodes can be added
	/// as they are fetched.  Derived classes that implement iterators
	/// must override runTo(int index), in order that they may
	/// run the iteration to the given index. </para>
	/// 
	/// <para>Note that we directly implement the DOM's NodeIterator
	/// interface. We do not emulate all the behavior of the
	/// standard NodeIterator. In particular, we do not guarantee
	/// to present a "live view" of the document ... but in XSLT,
	/// the source document should never be mutated, so this should
	/// never be an issue.</para>
	/// 
	/// <para>Thought: Should NodeSetDTM really implement NodeList and NodeIterator,
	/// or should there be specific subclasses of it which do so? The
	/// advantage of doing it all here is that all NodeSetDTMs will respond
	/// to the same calls; the disadvantage is that some of them may return
	/// less-than-enlightening results when you do so.</para>
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class NodeSetDTM : NodeVector, DTMIterator, ICloneable
	{
		internal new const long serialVersionUID = 7686480133331317070L;

	  /// <summary>
	  /// Create an empty nodelist.
	  /// </summary>
	  public NodeSetDTM(DTMManager dtmManager) : base()
	  {
		m_manager = dtmManager;
	  }

	  /// <summary>
	  /// Create an empty, using the given block size.
	  /// </summary>
	  /// <param name="blocksize"> Size of blocks to allocate </param>
	  /// <param name="dummy"> pass zero for right now... </param>
	  public NodeSetDTM(int blocksize, int dummy, DTMManager dtmManager) : base(blocksize)
	  {
		m_manager = dtmManager;
	  }

	  // %TBD%
	//  /**
	//   * Create a NodeSetDTM, and copy the members of the
	//   * given nodelist into it.
	//   *
	//   * @param nodelist List of Nodes to be made members of the new set.
	//   */
	//  public NodeSetDTM(NodeList nodelist)
	//  {
	//
	//    super();
	//
	//    addNodes(nodelist);
	//  }

	  /// <summary>
	  /// Create a NodeSetDTM, and copy the members of the
	  /// given NodeSetDTM into it.
	  /// </summary>
	  /// <param name="nodelist"> Set of Nodes to be made members of the new set. </param>
	  public NodeSetDTM(NodeSetDTM nodelist) : base()
	  {

		m_manager = nodelist.DTMManager;
		m_root = nodelist.Root;

		addNodes((DTMIterator) nodelist);
	  }

	  /// <summary>
	  /// Create a NodeSetDTM, and copy the members of the
	  /// given DTMIterator into it.
	  /// </summary>
	  /// <param name="ni"> Iterator which yields Nodes to be made members of the new set. </param>
	  public NodeSetDTM(DTMIterator ni) : base()
	  {


		m_manager = ni.DTMManager;
		m_root = ni.Root;
		addNodes(ni);
	  }

	  /// <summary>
	  /// Create a NodeSetDTM, and copy the members of the
	  /// given DTMIterator into it.
	  /// </summary>
	  /// <param name="iterator"> Iterator which yields Nodes to be made members of the new set. </param>
	  public NodeSetDTM(NodeIterator iterator, XPathContext xctxt) : base()
	  {


		Node node;
		m_manager = xctxt.DTMManager;

		while (null != (node = iterator.nextNode()))
		{
		  int handle = xctxt.getDTMHandleFromNode(node);
		  addNodeInDocOrder(handle, xctxt);
		}
	  }

	  /// <summary>
	  /// Create a NodeSetDTM, and copy the members of the
	  /// given DTMIterator into it.
	  /// 
	  /// </summary>
	  public NodeSetDTM(NodeList nodeList, XPathContext xctxt) : base()
	  {


		m_manager = xctxt.DTMManager;

		int n = nodeList.getLength();
		for (int i = 0; i < n; i++)
		{
		  Node node = nodeList.item(i);
		  int handle = xctxt.getDTMHandleFromNode(node);
		  // Do not reorder or strip duplicate nodes from the given DOM nodelist
		  addNode(handle); // addNodeInDocOrder(handle, xctxt);
		}
	  }


	  /// <summary>
	  /// Create a NodeSetDTM which contains the given Node.
	  /// </summary>
	  /// <param name="node"> Single node to be added to the new set. </param>
	  public NodeSetDTM(int node, DTMManager dtmManager) : base()
	  {

		m_manager = dtmManager;

		addNode(node);
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
			// no-op
		  }
	  }


	  /// <returns> The root node of the Iterator, as specified when it was created.
	  /// For non-Iterator NodeSetDTMs, this will be null. </returns>
	  public virtual int Root
	  {
		  get
		  {
			if (DTM.NULL == m_root)
			{
			  if (size() > 0)
			  {
				return item(0);
			  }
			  else
			  {
				return DTM.NULL;
			  }
			}
			else
			{
			  return m_root;
			}
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
		// no-op, I guess...  (-sb)
	  }

	  /// <summary>
	  /// Clone this NodeSetDTM.
	  /// At this time, we only expect this to be used with LocPathIterators;
	  /// it may not work with other kinds of NodeSetDTMs.
	  /// </summary>
	  /// <returns> a new NodeSetDTM of the same type, having the same state...
	  /// though unless overridden in the subclasses, it may not copy all
	  /// the state information.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> if this subclass of NodeSetDTM
	  /// does not support the clone() operation. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {

		NodeSetDTM clone = (NodeSetDTM) base.clone();

		return clone;
	  }

	  /// <summary>
	  /// Get a cloned Iterator, and reset its state to the beginning of the
	  /// iteration.
	  /// </summary>
	  /// <returns> a new NodeSetDTM of the same type, having the same state...
	  /// except that the reset() operation has been called.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> if this subclass of NodeSetDTM
	  /// does not support the clone() operation. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public virtual DTMIterator cloneWithReset()
	  {

		NodeSetDTM clone = (NodeSetDTM) this.clone();

		clone.reset();

		return clone;
	  }

	  /// <summary>
	  /// Reset the iterator. May have no effect on non-iterator Nodesets.
	  /// </summary>
	  public virtual void reset()
	  {
		m_next = 0;
	  }

	  /// <summary>
	  ///  This attribute determines which node types are presented via the
	  /// iterator. The available set of constants is defined in the
	  /// <code>DTMFilter</code> interface. For NodeSetDTMs, the mask has been
	  /// hardcoded to show all nodes except EntityReference nodes, which have
	  /// no equivalent in the XPath data model.
	  /// </summary>
	  /// <returns> integer used as a bit-array, containing flags defined in
	  /// the DOM's DTMFilter class. The value will be 
	  /// <code>SHOW_ALL & ~SHOW_ENTITY_REFERENCE</code>, meaning that
	  /// only entity references are suppressed. </returns>
	  public virtual int WhatToShow
	  {
		  get
		  {
			return DTMFilter.SHOW_ALL & ~DTMFilter.SHOW_ENTITY_REFERENCE;
		  }
	  }

	  /// <summary>
	  /// The filter object used to screen nodes. Filters are applied to
	  /// further reduce (and restructure) the DTMIterator's view of the
	  /// document. In our case, we will be using hardcoded filters built
	  /// into our iterators... but getFilter() is part of the DOM's 
	  /// DTMIterator interface, so we have to support it.
	  /// </summary>
	  /// <returns> null, which is slightly misleading. True, there is no
	  /// user-written filter object, but in fact we are doing some very
	  /// sophisticated custom filtering. A DOM purist might suggest
	  /// returning a placeholder object just to indicate that this is
	  /// not going to return all nodes selected by whatToShow. </returns>
	  public virtual DTMFilter Filter
	  {
		  get
		  {
			return null;
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
	  /// <returns> true for all iterators based on NodeSetDTM, meaning that the
	  /// contents of EntityRefrence nodes may be returned (though whatToShow
	  /// says that the EntityReferences themselves are not shown.) </returns>
	  public virtual bool ExpandEntityReferences
	  {
		  get
		  {
			return true;
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

		return m_manager.getDTM(nodeHandle);
	  }

	  /* An instance of the DTMManager. */
	  internal DTMManager m_manager;

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
    
			return m_manager;
		  }
	  }

	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a DTMIterator is created, the first call
	  /// to nextNode() returns the first node in the set. </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>DTM.NULL</code> if there are no more members in that set. </returns>
	  /// <exception cref="DOMException">
	  ///    INVALID_STATE_ERR: Raised if this method is called after the
	  ///   <code>detach</code> method was invoked. </exception>
	  public virtual int nextNode()
	  {

		if ((m_next) < this.size())
		{
		  int next = this.elementAt(m_next);

		  m_next++;

		  return next;
		}
		else
		{
		  return DTM.NULL;
		}
	  }

	  /// <summary>
	  ///  Returns the previous node in the set and moves the position of the
	  /// iterator backwards in the set. </summary>
	  /// <returns>  The previous <code>Node</code> in the set being iterated over,
	  ///   or<code>DTM.NULL</code> if there are no more members in that set. </returns>
	  /// <exception cref="DOMException">
	  ///    INVALID_STATE_ERR: Raised if this method is called after the
	  ///   <code>detach</code> method was invoked. </exception>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a cached type, and hence doesn't know what the previous node was. </exception>
	  public virtual int previousNode()
	  {

		if (!m_cacheNodes)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_CANNOT_ITERATE, null)); //"This NodeSetDTM can not iterate to a previous node!");
		}

		if ((m_next - 1) > 0)
		{
		  m_next--;

		  return this.elementAt(m_next);
		}
		else
		{
		  return DTM.NULL;
		}
	  }

	  /// <summary>
	  /// Detaches the iterator from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state. After<code>detach</code> has been invoked, calls to
	  /// <code>nextNode</code> or<code>previousNode</code> will raise the
	  /// exception INVALID_STATE_ERR.
	  /// <para>
	  /// This operation is a no-op in NodeSetDTM, and will not cause 
	  /// INVALID_STATE_ERR to be raised by later operations.
	  /// </para>
	  /// </summary>
	  public virtual void detach()
	  {
	  }

	  /// <summary>
	  /// Specify if it's OK for detach to release the iterator for reuse.
	  /// </summary>
	  /// <param name="allowRelease"> true if it is OK for detach to release this iterator 
	  /// for pooling. </param>
	  public virtual void allowDetachToRelease(bool allowRelease)
	  {
		// no action for right now.
	  }


	  /// <summary>
	  /// Tells if this NodeSetDTM is "fresh", in other words, if
	  /// the first nextNode() that is called will return the
	  /// first node in the set.
	  /// </summary>
	  /// <returns> true if nextNode() would return the first node in the set,
	  /// false if it would return a later one. </returns>
	  public virtual bool Fresh
	  {
		  get
		  {
			return (m_next == 0);
		  }
	  }

	  /// <summary>
	  /// If an index is requested, NodeSetDTM will call this method
	  /// to run the iterator to the index.  By default this sets
	  /// m_next to the index.  If the index argument is -1, this
	  /// signals that the iterator should be run to the end.
	  /// </summary>
	  /// <param name="index"> Position to advance (or retreat) to, with
	  /// 0 requesting the reset ("fresh") position and -1 (or indeed
	  /// any out-of-bounds value) requesting the final position. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not
	  /// one of the types which supports indexing/counting. </exception>
	  public virtual void runTo(int index)
	  {

		if (!m_cacheNodes)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_CANNOT_INDEX, null)); //"This NodeSetDTM can not do indexing or counting functions!");
		}

		if ((index >= 0) && (m_next < m_firstFree))
		{
		  m_next = index;
		}
		else
		{
		  m_next = m_firstFree - 1;
		}
	  }

	  /// <summary>
	  /// Returns the <code>index</code>th item in the collection. If
	  /// <code>index</code> is greater than or equal to the number of nodes in
	  /// the list, this returns <code>null</code>.
	  /// 
	  /// TODO: What happens if index is out of range?
	  /// </summary>
	  /// <param name="index"> Index into the collection. </param>
	  /// <returns> The node at the <code>index</code>th position in the
	  ///   <code>NodeList</code>, or <code>null</code> if that is not a valid
	  ///   index. </returns>
	  public virtual int item(int index)
	  {

		runTo(index);

		return this.elementAt(index);
	  }

	  /// <summary>
	  /// The number of nodes in the list. The range of valid child node indices is
	  /// 0 to <code>length-1</code> inclusive. Note that this operation requires
	  /// finding all the matching nodes, which may defeat attempts to defer
	  /// that work.
	  /// </summary>
	  /// <returns> integer indicating how many nodes are represented by this list. </returns>
	  public virtual int Length
	  {
		  get
		  {
    
			runTo(-1);
    
			return this.size();
		  }
	  }

	  /// <summary>
	  /// Add a node to the NodeSetDTM. Not all types of NodeSetDTMs support this
	  /// operation
	  /// </summary>
	  /// <param name="n"> Node to be added </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void addNode(int n)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		this.addElement(n);
	  }

	  /// <summary>
	  /// Insert a node at a given position.
	  /// </summary>
	  /// <param name="n"> Node to be added </param>
	  /// <param name="pos"> Offset at which the node is to be inserted,
	  /// with 0 being the first position. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void insertNode(int n, int pos)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		insertElementAt(n, pos);
	  }

	  /// <summary>
	  /// Remove a node.
	  /// </summary>
	  /// <param name="n"> Node to be added </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void removeNode(int n)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		this.removeElement(n);
	  }

	  // %TBD%
	//  /**
	//   * Copy NodeList members into this nodelist, adding in
	//   * document order.  If a node is null, don't add it.
	//   *
	//   * @param nodelist List of nodes which should now be referenced by
	//   * this NodeSetDTM.
	//   * @throws RuntimeException thrown if this NodeSetDTM is not of 
	//   * a mutable type.
	//   */
	//  public void addNodes(NodeList nodelist)
	//  {
	//
	//    if (!m_mutable)
	//      throw new RuntimeException("This NodeSetDTM is not mutable!");
	//
	//    if (null != nodelist)  // defensive to fix a bug that Sanjiva reported.
	//    {
	//      int nChildren = nodelist.getLength();
	//
	//      for (int i = 0; i < nChildren; i++)
	//      {
	//        int obj = nodelist.item(i);
	//
	//        if (null != obj)
	//        {
	//          addElement(obj);
	//        }
	//      }
	//    }
	//
	//    // checkDups();
	//  }

	  // %TBD%
	//  /**
	//   * <p>Copy NodeList members into this nodelist, adding in
	//   * document order.  Only genuine node references will be copied;
	//   * nulls appearing in the source NodeSetDTM will
	//   * not be added to this one. </p>
	//   * 
	//   * <p> In case you're wondering why this function is needed: NodeSetDTM
	//   * implements both DTMIterator and NodeList. If this method isn't
	//   * provided, Java can't decide which of those to use when addNodes()
	//   * is invoked. Providing the more-explicit match avoids that
	//   * ambiguity.)</p>
	//   *
	//   * @param ns NodeSetDTM whose members should be merged into this NodeSetDTM.
	//   * @throws RuntimeException thrown if this NodeSetDTM is not of 
	//   * a mutable type.
	//   */
	//  public void addNodes(NodeSetDTM ns)
	//  {
	//
	//    if (!m_mutable)
	//      throw new RuntimeException("This NodeSetDTM is not mutable!");
	//
	//    addNodes((DTMIterator) ns);
	//  }

	  /// <summary>
	  /// Copy NodeList members into this nodelist, adding in
	  /// document order.  Null references are not added.
	  /// </summary>
	  /// <param name="iterator"> DTMIterator which yields the nodes to be added. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void addNodes(DTMIterator iterator)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		if (null != iterator) // defensive to fix a bug that Sanjiva reported.
		{
		  int obj;

		  while (DTM.NULL != (obj = iterator.nextNode()))
		  {
			addElement(obj);
		  }
		}

		// checkDups();
	  }

	  // %TBD%
	//  /**
	//   * Copy NodeList members into this nodelist, adding in
	//   * document order.  If a node is null, don't add it.
	//   *
	//   * @param nodelist List of nodes to be added
	//   * @param support The XPath runtime context.
	//   * @throws RuntimeException thrown if this NodeSetDTM is not of 
	//   * a mutable type.
	//   */
	//  public void addNodesInDocOrder(NodeList nodelist, XPathContext support)
	//  {
	//
	//    if (!m_mutable)
	//      throw new RuntimeException("This NodeSetDTM is not mutable!");
	//
	//    int nChildren = nodelist.getLength();
	//
	//    for (int i = 0; i < nChildren; i++)
	//    {
	//      int node = nodelist.item(i);
	//
	//      if (null != node)
	//      {
	//        addNodeInDocOrder(node, support);
	//      }
	//    }
	//  }

	  /// <summary>
	  /// Copy NodeList members into this nodelist, adding in
	  /// document order.  If a node is null, don't add it.
	  /// </summary>
	  /// <param name="iterator"> DTMIterator which yields the nodes to be added. </param>
	  /// <param name="support"> The XPath runtime context. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void addNodesInDocOrder(DTMIterator iterator, XPathContext support)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		int node;

		while (DTM.NULL != (node = iterator.nextNode()))
		{
		  addNodeInDocOrder(node, support);
		}
	  }

	  // %TBD%
	//  /**
	//   * Add the node list to this node set in document order.
	//   *
	//   * @param start index.
	//   * @param end index.
	//   * @param testIndex index.
	//   * @param nodelist The nodelist to add.
	//   * @param support The XPath runtime context.
	//   *
	//   * @return false always.
	//   * @throws RuntimeException thrown if this NodeSetDTM is not of 
	//   * a mutable type.
	//   */
	//  private boolean addNodesInDocOrder(int start, int end, int testIndex,
	//                                     NodeList nodelist, XPathContext support)
	//  {
	//
	//    if (!m_mutable)
	//      throw new RuntimeException("This NodeSetDTM is not mutable!");
	//
	//    boolean foundit = false;
	//    int i;
	//    int node = nodelist.item(testIndex);
	//
	//    for (i = end; i >= start; i--)
	//    {
	//      int child = elementAt(i);
	//
	//      if (child == node)
	//      {
	//        i = -2;  // Duplicate, suppress insert
	//
	//        break;
	//      }
	//
	//      if (!support.getDOMHelper().isNodeAfter(node, child))
	//      {
	//        insertElementAt(node, i + 1);
	//
	//        testIndex--;
	//
	//        if (testIndex > 0)
	//        {
	//          boolean foundPrev = addNodesInDocOrder(0, i, testIndex, nodelist,
	//                                                 support);
	//
	//          if (!foundPrev)
	//          {
	//            addNodesInDocOrder(i, size() - 1, testIndex, nodelist, support);
	//          }
	//        }
	//
	//        break;
	//      }
	//    }
	//
	//    if (i == -1)
	//    {
	//      insertElementAt(node, 0);
	//    }
	//
	//    return foundit;
	//  }

	  /// <summary>
	  /// Add the node into a vector of nodes where it should occur in
	  /// document order. </summary>
	  /// <param name="node"> The node to be added. </param>
	  /// <param name="test"> true if we should test for doc order </param>
	  /// <param name="support"> The XPath runtime context. </param>
	  /// <returns> insertIndex. </returns>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual int addNodeInDocOrder(int node, bool test, XPathContext support)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		int insertIndex = -1;

		if (test)
		{

		  // This needs to do a binary search, but a binary search 
		  // is somewhat tough because the sequence test involves 
		  // two nodes.
		  int size = this.size(), i;

		  for (i = size - 1; i >= 0; i--)
		  {
			int child = elementAt(i);

			if (child == node)
			{
			  i = -2; // Duplicate, suppress insert

			  break;
			}

			DTM dtm = support.getDTM(node);
			if (!dtm.isNodeAfter(node, child))
			{
			  break;
			}
		  }

		  if (i != -2)
		  {
			insertIndex = i + 1;

			insertElementAt(node, insertIndex);
		  }
		}
		else
		{
		  insertIndex = this.size();

		  bool foundit = false;

		  for (int i = 0; i < insertIndex; i++)
		  {
			if (i == node)
			{
			  foundit = true;

			  break;
			}
		  }

		  if (!foundit)
		  {
			addElement(node);
		  }
		}

		// checkDups();
		return insertIndex;
	  } // end addNodeInDocOrder(Vector v, Object obj)

	  /// <summary>
	  /// Add the node into a vector of nodes where it should occur in
	  /// document order. </summary>
	  /// <param name="node"> The node to be added. </param>
	  /// <param name="support"> The XPath runtime context.
	  /// </param>
	  /// <returns> The index where it was inserted. </returns>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual int addNodeInDocOrder(int node, XPathContext support)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		return addNodeInDocOrder(node, true, support);
	  } // end addNodeInDocOrder(Vector v, Object obj)

	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> The size of this node set. </returns>
	  public override int size()
	  {
		return base.size();
	  }

	  /// <summary>
	  /// Append a Node onto the vector.
	  /// </summary>
	  /// <param name="value"> The node to be added. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void addElement(int value)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.addElement(value);
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously.
	  /// </summary>
	  /// <param name="value"> The node to be inserted. </param>
	  /// <param name="at"> The index where the insert should occur. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void insertElementAt(int value, int at)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.insertElementAt(value, at);
	  }

	  /// <summary>
	  /// Append the nodes to the list.
	  /// </summary>
	  /// <param name="nodes"> The nodes to be appended to this node set. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void appendNodes(NodeVector nodes)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.appendNodes(nodes);
	  }

	  /// <summary>
	  /// Inserts the specified node in this vector at the specified index.
	  /// Each component in this vector with an index greater or equal to
	  /// the specified index is shifted upward to have an index one greater
	  /// than the value it had previously. </summary>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void removeAllElements()
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.removeAllElements();
	  }

	  /// <summary>
	  /// Removes the first occurrence of the argument from this vector.
	  /// If the object is found in this vector, each component in the vector
	  /// with an index greater or equal to the object's index is shifted
	  /// downward to have an index one smaller than the value it had
	  /// previously.
	  /// </summary>
	  /// <param name="s"> The node to be removed.
	  /// </param>
	  /// <returns> True if the node was successfully removed </returns>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override bool removeElement(int s)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		return base.removeElement(s);
	  }

	  /// <summary>
	  /// Deletes the component at the specified index. Each component in
	  /// this vector with an index greater or equal to the specified
	  /// index is shifted downward to have an index one smaller than
	  /// the value it had previously.
	  /// </summary>
	  /// <param name="i"> The index of the node to be removed. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void removeElementAt(int i)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.removeElementAt(i);
	  }

	  /// <summary>
	  /// Sets the component at the specified index of this vector to be the
	  /// specified object. The previous component at that position is discarded.
	  /// 
	  /// The index must be a value greater than or equal to 0 and less
	  /// than the current size of the vector.
	  /// </summary>
	  /// <param name="node">  The node to be set. </param>
	  /// <param name="index"> The index of the node to be replaced. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public override void setElementAt(int node, int index)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.setElementAt(node, index);
	  }

	  /// <summary>
	  /// Same as setElementAt.
	  /// </summary>
	  /// <param name="node">  The node to be set. </param>
	  /// <param name="index"> The index of the node to be replaced. </param>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	  public virtual void setItem(int node, int index)
	  {

		if (!m_mutable)
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_NOT_MUTABLE, null)); //"This NodeSetDTM is not mutable!");
		}

		base.setElementAt(node, index);
	  }

	  /// <summary>
	  /// Get the nth element.
	  /// </summary>
	  /// <param name="i"> The index of the requested node.
	  /// </param>
	  /// <returns> Node at specified index. </returns>
	  public override int elementAt(int i)
	  {

		runTo(i);

		return base.elementAt(i);
	  }

	  /// <summary>
	  /// Tell if the table contains the given node.
	  /// </summary>
	  /// <param name="s"> Node to look for
	  /// </param>
	  /// <returns> True if the given node was found. </returns>
	  public override bool contains(int s)
	  {

		runTo(-1);

		return base.contains(s);
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> Node to look for </param>
	  /// <param name="index"> Index of where to start the search </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public override int indexOf(int elem, int index)
	  {

		runTo(-1);

		return base.indexOf(elem, index);
	  }

	  /// <summary>
	  /// Searches for the first occurence of the given argument,
	  /// beginning the search at index, and testing for equality
	  /// using the equals method.
	  /// </summary>
	  /// <param name="elem"> Node to look for </param>
	  /// <returns> the index of the first occurrence of the object
	  /// argument in this vector at position index or later in the
	  /// vector; returns -1 if the object is not found. </returns>
	  public override int indexOf(int elem)
	  {

		runTo(-1);

		return base.indexOf(elem);
	  }

	  /// <summary>
	  /// If this node is being used as an iterator, the next index that nextNode()
	  ///  will return.  
	  /// </summary>
	  [NonSerialized]
	  protected internal int m_next = 0;

	  /// <summary>
	  /// Get the current position, which is one less than
	  /// the next nextNode() call will retrieve.  i.e. if
	  /// you call getCurrentPos() and the return is 0, the next
	  /// fetch will take place at index 1.
	  /// </summary>
	  /// <returns> The the current position index. </returns>
	  public virtual int CurrentPos
	  {
		  get
		  {
			return m_next;
		  }
		  set
		  {
    
			if (!m_cacheNodes)
			{
			  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NODESETDTM_CANNOT_INDEX, null)); //"This NodeSetDTM can not do indexing or counting functions!");
			}
    
			m_next = value;
		  }
	  }


	  /// <summary>
	  /// Return the last fetched node.  Needed to support the UnionPathIterator.
	  /// </summary>
	  /// <returns> the last fetched node. </returns>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a cached type, and thus doesn't permit indexed access. </exception>
	  public virtual int CurrentNode
	  {
		  get
		  {
    
			if (!m_cacheNodes)
			{
			  throw new Exception("This NodeSetDTM can not do indexing or counting functions!");
			}
    
			int saved = m_next;
			// because nextNode always increments
			// But watch out for copy29, where the root iterator didn't
			// have nextNode called on it.
			int current = (m_next > 0) ? m_next - 1 : m_next;
			int n = (current < m_firstFree) ? elementAt(current) : DTM.NULL;
			m_next = saved; // HACK: I think this is a bit of a hack.  -sb
			return n;
		  }
	  }

	  /// <summary>
	  /// True if this list can be mutated. </summary>
	  [NonSerialized]
	  protected internal bool m_mutable = true;

	  /// <summary>
	  /// True if this list is cached.
	  ///  @serial  
	  /// </summary>
	  [NonSerialized]
	  protected internal bool m_cacheNodes = true;

	  /// <summary>
	  /// The root of the iteration, if available. </summary>
	  protected internal int m_root = DTM.NULL;

	  /// <summary>
	  /// Get whether or not this is a cached node set.
	  /// 
	  /// </summary>
	  /// <returns> True if this list is cached. </returns>
	  public virtual bool ShouldCacheNodes
	  {
		  get
		  {
			return m_cacheNodes;
		  }
		  set
		  {
    
			if (!Fresh)
			{
			  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_CANNOT_CALL_SETSHOULDCACHENODE, null)); //"Can not call setShouldCacheNodes after nextNode has been called!");
			}
    
			m_cacheNodes = value;
			m_mutable = true;
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
			return m_mutable;
		  }
	  }

	  [NonSerialized]
	  private int m_last = 0;

	  public virtual int Last
	  {
		  get
		  {
			return m_last;
		  }
		  set
		  {
			m_last = value;
		  }
	  }


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


	}

}