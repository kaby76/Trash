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
 * $Id: DTMIterator.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{

	/// 
	/// <summary>
	/// <code>DTMIterators</code> are used to step through a (possibly
	/// filtered) set of nodes.  Their API is modeled largely after the DOM
	/// NodeIterator.
	/// 
	/// <para>A DTMIterator is a somewhat unusual type of iterator, in that it 
	/// can serve both single node iteration and random access.</para>
	/// 
	/// <para>The DTMIterator's traversal semantics, i.e. how it walks the tree,
	/// are specified when it is created, possibly and probably by an XPath
	/// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or 
	/// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.</para>
	/// 
	/// <para>A DTMIterator is meant to be created once as a master static object, and 
	/// then cloned many times for runtime use.  Or the master object itself may 
	/// be used for simpler use cases.</para>
	/// 
	/// <para>At this time, we do not expect DTMIterator to emulate
	/// NodeIterator's "maintain relative position" semantics under
	/// document mutation.  It's likely to respond more like the
	/// TreeWalker's "current node" semantics. However, since the base DTM
	/// is immutable, this issue currently makes no practical
	/// difference.</para>
	/// 
	/// <para>State: In progress!!</para> 
	/// </summary>
	public interface DTMIterator
	{

	  // Constants returned by acceptNode, borrowed from the DOM Traversal chapter
	  // %REVIEW% Should we explicitly initialize them from, eg,
	  // org.w3c.dom.traversal.NodeFilter.FILTER_ACCEPT?

	  /// <summary>
	  /// Accept the node.
	  /// </summary>

	  /// <summary>
	  /// Reject the node. Same behavior as FILTER_SKIP. (In the DOM these
	  /// differ when applied to a TreeWalker but have the same result when
	  /// applied to a NodeIterator).
	  /// </summary>

	  /// <summary>
	  /// Skip this single node. 
	  /// </summary>

	  /// <summary>
	  /// Get an instance of a DTM that "owns" a node handle.  Since a node 
	  /// iterator may be passed without a DTMManager, this allows the 
	  /// caller to easily get the DTM using just the iterator.
	  /// </summary>
	  /// <param name="nodeHandle"> the nodeHandle.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  DTM getDTM(int nodeHandle);

	  /// <summary>
	  /// Get an instance of the DTMManager.  Since a node 
	  /// iterator may be passed without a DTMManager, this allows the 
	  /// caller to easily get the DTMManager using just the iterator.
	  /// </summary>
	  /// <returns> a non-null DTMManager reference. </returns>
	  DTMManager DTMManager {get;}

	  /// <summary>
	  /// The root node of the <code>DTMIterator</code>, as specified when it
	  /// was created.  Note the root node is not the root node of the 
	  /// document tree, but the context node from where the iteration 
	  /// begins and ends.
	  /// </summary>
	  /// <returns> nodeHandle int Handle of the context node. </returns>
	  int Root {get;}

	  /// <summary>
	  /// Reset the root node of the <code>DTMIterator</code>, overriding
	  /// the value specified when it was created.  Note the root node is
	  /// not the root node of the document tree, but the context node from
	  /// where the iteration begins.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the context node. </param>
	  /// <param name="environment"> The environment object.  
	  /// The environment in which this iterator operates, which should provide:
	  /// <ul>
	  /// <li>a node (the context node... same value as "root" defined below) </li>
	  /// <li>a pair of non-zero positive integers (the context position and the context size) </li>
	  /// <li>a set of variable bindings </li>
	  /// <li>a function library </li>
	  /// <li>the set of namespace declarations in scope for the expression.</li>
	  /// <ul>
	  /// 
	  /// <para>At this time the exact implementation of this environment is application 
	  /// dependent.  Probably a proper interface will be created fairly soon.</para>
	  ///  </param>
	  void setRoot(int nodeHandle, object environment);

	  /// <summary>
	  /// Reset the iterator to the start. After resetting, the next node returned
	  /// will be the root node -- or, if that's filtered out, the first node
	  /// within the root's subtree which is _not_ skipped by the filters.
	  /// </summary>
	  void reset();

	  /// <summary>
	  /// This attribute determines which node types are presented via the
	  /// iterator. The available set of constants is defined above.  
	  /// Nodes not accepted by
	  /// <code>whatToShow</code> will be skipped, but their children may still
	  /// be considered.
	  /// </summary>
	  /// <returns> one of the SHOW_XXX constants, or several ORed together. </returns>
	  int WhatToShow {get;}

	  /// <summary>
	  /// <para>The value of this flag determines whether the children of entity
	  /// reference nodes are visible to the iterator. If false, they  and
	  /// their descendants will be rejected. Note that this rejection takes
	  /// precedence over <code>whatToShow</code> and the filter. </para>
	  /// 
	  /// <para> To produce a view of the document that has entity references
	  /// expanded and does not expose the entity reference node itself, use
	  /// the <code>whatToShow</code> flags to hide the entity reference node
	  /// and set <code>expandEntityReferences</code> to true when creating the
	  /// iterator. To produce a view of the document that has entity reference
	  /// nodes but no entity expansion, use the <code>whatToShow</code> flags
	  /// to show the entity reference node and set
	  /// <code>expandEntityReferences</code> to false.</para>
	  /// 
	  /// <para>NOTE: In Xalan's use of DTM we will generally have fully expanded
	  /// entity references when the document tree was built, and thus this
	  /// flag will have no effect.</para>
	  /// </summary>
	  /// <returns> true if entity references will be expanded.   </returns>
	  bool ExpandEntityReferences {get;}

	  /// <summary>
	  /// Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a <code>DTMIterator</code> has setRoot called,
	  /// the first call to <code>nextNode()</code> returns that root or (if it
	  /// is rejected by the filters) the first node within its subtree which is
	  /// not filtered out. </summary>
	  /// <returns> The next node handle in the set being iterated over, or
	  ///  <code>DTM.NULL</code> if there are no more members in that set. </returns>
	  int nextNode();

	  /// <summary>
	  /// Returns the previous node in the set and moves the position of the
	  /// <code>DTMIterator</code> backwards in the set. </summary>
	  /// <returns> The previous node handle in the set being iterated over,
	  ///   or <code>DTM.NULL</code> if there are no more members in that set. </returns>
	  int previousNode();

	  /// <summary>
	  /// Detaches the <code>DTMIterator</code> from the set which it iterated
	  /// over, releasing any computational resources and placing the iterator
	  /// in the INVALID state. After <code>detach</code> has been invoked,
	  /// calls to <code>nextNode</code> or <code>previousNode</code> will
	  /// raise a runtime exception.
	  /// </summary>
	  void detach();

	  /// <summary>
	  /// Specify if it's OK for detach to release the iterator for reuse.
	  /// </summary>
	  /// <param name="allowRelease"> true if it is OK for detach to release this iterator 
	  /// for pooling. </param>
	  void allowDetachToRelease(bool allowRelease);

	  /// <summary>
	  /// Get the current node in the iterator. Note that this differs from
	  /// the DOM's NodeIterator, where the current position lies between two
	  /// nodes (as part of the maintain-relative-position semantic).
	  /// </summary>
	  /// <returns> The current node handle, or -1. </returns>
	  int CurrentNode {get;}

	  /// <summary>
	  /// Tells if this NodeSetDTM is "fresh", in other words, if
	  /// the first nextNode() that is called will return the
	  /// first node in the set.
	  /// </summary>
	  /// <returns> true if the iteration of this list has not yet begun. </returns>
	  bool Fresh {get;}

	  //========= Random Access ==========

	  /// <summary>
	  /// If setShouldCacheNodes(true) is called, then nodes will
	  /// be cached, enabling random access, and giving the ability to do 
	  /// sorts and the like.  They are not cached by default.
	  /// 
	  /// %REVIEW% Shouldn't the other random-access methods throw an exception
	  /// if they're called on a DTMIterator with this flag set false?
	  /// </summary>
	  /// <param name="b"> true if the nodes should be cached. </param>
	  bool ShouldCacheNodes {set;}

	  /// <summary>
	  /// Tells if this iterator can have nodes added to it or set via 
	  /// the <code>setItem(int node, int index)</code> method.
	  /// </summary>
	  /// <returns> True if the nodelist can be mutated. </returns>
	  bool Mutable {get;}

	  /// <summary>
	  /// Get the current position within the cached list, which is one
	  /// less than the next nextNode() call will retrieve.  i.e. if you
	  /// call getCurrentPos() and the return is 0, the next fetch will
	  /// take place at index 1.
	  /// </summary>
	  /// <returns> The position of the iteration. </returns>
	  int CurrentPos {get;set;}

	  /// <summary>
	  /// If an index is requested, NodeSetDTM will call this method
	  /// to run the iterator to the index.  By default this sets
	  /// m_next to the index.  If the index argument is -1, this
	  /// signals that the iterator should be run to the end and
	  /// completely fill the cache.
	  /// </summary>
	  /// <param name="index"> The index to run to, or -1 if the iterator should be run
	  ///              to the end. </param>
	  void runTo(int index);


	  /// <summary>
	  /// Returns the <code>node handle</code> of an item in the collection. If
	  /// <code>index</code> is greater than or equal to the number of nodes in
	  /// the list, this returns <code>null</code>.
	  /// </summary>
	  /// <param name="index"> of the item. </param>
	  /// <returns> The node handle at the <code>index</code>th position in the
	  ///   <code>DTMIterator</code>, or <code>-1</code> if that is not a valid
	  ///   index. </returns>
	  int item(int index);

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
	  void setItem(int node, int index);

	  /// <summary>
	  /// The number of nodes in the list. The range of valid child node indices
	  /// is 0 to <code>length-1</code> inclusive. Note that this requires running
	  /// the iterator to completion, and presumably filling the cache.
	  /// </summary>
	  /// <returns> The number of nodes in the list. </returns>
	  int Length {get;}

	  //=========== Cloning operations. ============

	  /// <summary>
	  /// Get a cloned Iterator that is reset to the start of the iteration.
	  /// </summary>
	  /// <returns> A clone of this iteration that has been reset.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DTMIterator cloneWithReset() throws CloneNotSupportedException;
	  DTMIterator cloneWithReset();

	  /// <summary>
	  /// Get a clone of this iterator, but don't reset the iteration in the 
	  /// process, so that it may be used from the current position.
	  /// </summary>
	  /// <returns> A clone of this object.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException;
	  object clone();

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// </summary>
	  /// <returns> true if all the nodes in the iteration well be returned in document 
	  /// order. </returns>
	  bool DocOrdered {get;}

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  int Axis {get;}

	}

	public static class DTMIterator_Fields
	{
	  public const short FILTER_ACCEPT = 1;
	  public const short FILTER_REJECT = 2;
	  public const short FILTER_SKIP = 3;
	}

}