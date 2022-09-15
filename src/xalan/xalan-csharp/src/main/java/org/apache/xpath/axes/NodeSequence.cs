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
 * $Id: NodeSequence.java 469367 2006-10-31 04:41:08Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class is the dynamic wrapper for a Xalan DTMIterator instance, and 
	/// provides random access capabilities.
	/// </summary>
	[Serializable]
	public class NodeSequence : XObject, DTMIterator, Cloneable, PathComponent
	{
		internal new const long serialVersionUID = 3866261934726581044L;
	  /// <summary>
	  /// The index of the last node in the iteration. </summary>
	  protected internal int m_last = -1;

	  /// <summary>
	  /// The index of the next node to be fetched.  Useful if this
	  /// is a cached iterator, and is being used as random access
	  /// NodeList.
	  /// </summary>
	  protected internal int m_next = 0;

	  /// <summary>
	  /// A cache of a list of nodes obtained from the iterator so far.
	  /// This list is appended to until the iterator is exhausted and
	  /// the cache is complete.
	  /// <para>
	  /// Multiple NodeSequence objects may share the same cache.
	  /// </para>
	  /// </summary>
	  private IteratorCache m_cache;

	  /// <summary>
	  /// If this iterator needs to cache nodes that are fetched, they
	  /// are stored in the Vector in the generic object.
	  /// </summary>
	  protected internal virtual NodeVector Vector
	  {
		  get
		  {
			  NodeVector nv = (m_cache != null) ? m_cache.Vector : null;
			  return nv;
		  }
		  set
		  {
					m_vec2 = value;
					m_useCount2 = 1;
		  }
	  }

	  /// <summary>
	  /// Get the cache (if any) of nodes obtained from
	  /// the iterator so far. Note that the cache keeps
	  /// growing until the iterator is walked to exhaustion,
	  /// at which point the cache is "complete".
	  /// </summary>
	  private IteratorCache Cache
	  {
		  get
		  {
			  return m_cache;
		  }
	  }

	  /// <summary>
	  /// Set the vector where nodes will be cached.
	  /// </summary>
	  protected internal virtual void SetVector(NodeVector v)
	  {
		  Object = v;
	  }


	  /// <summary>
	  /// If the iterator needs to cache nodes as they are fetched,
	  /// then this method returns true. 
	  /// </summary>
	  public virtual bool hasCache()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.utils.NodeVector nv = getVector();
		NodeVector nv = ArrayList;
		  return (nv != null);
	  }

	  /// <summary>
	  /// If this NodeSequence has a cache, and that cache is 
	  /// fully populated then this method returns true, otherwise
	  /// if there is no cache or it is not complete it returns false.
	  /// </summary>
	  private bool cacheComplete()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean complete;
		  bool complete;
		  if (m_cache != null)
		  {
			  complete = m_cache.Complete;
		  }
		  else
		  {
			  complete = false;
		  }
		  return complete;
	  }

	  /// <summary>
	  /// If this NodeSequence has a cache, mark that it is complete.
	  /// This method should be called after the iterator is exhausted.
	  /// </summary>
	  private void markCacheComplete()
	  {
		  NodeVector nv = ArrayList;
		  if (nv != null)
		  {
			  m_cache.CacheComplete = true;
		  }
	  }


	  /// <summary>
	  /// The functional iterator that fetches nodes.
	  /// </summary>
	  protected internal DTMIterator m_iter;

	  /// <summary>
	  /// Set the functional iterator that fetches nodes. </summary>
	  /// <param name="iter"> The iterator that is to be contained. </param>
	  public DTMIterator Iter
	  {
		  set
		  {
			  m_iter = value;
		  }
	  }

	  /// <summary>
	  /// Get the functional iterator that fetches nodes. </summary>
	  /// <returns> The contained iterator. </returns>
	  public DTMIterator ContainedIter
	  {
		  get
		  {
			  return m_iter;
		  }
	  }

	  /// <summary>
	  /// The DTMManager to use if we're using a NodeVector only.
	  /// We may well want to do away with this, and store it in the NodeVector.
	  /// </summary>
	  protected internal DTMManager m_dtmMgr;

	  // ==== Constructors ====

	  /// <summary>
	  /// Create a new NodeSequence from a (already cloned) iterator.
	  /// </summary>
	  /// <param name="iter"> Cloned (not static) DTMIterator. </param>
	  /// <param name="context"> The initial context node. </param>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="shouldCacheNodes"> True if this sequence can random access. </param>
	  private NodeSequence(DTMIterator iter, int context, XPathContext xctxt, bool shouldCacheNodes)
	  {
		  Iter = iter;
		  setRoot(context, xctxt);
		  ShouldCacheNodes = shouldCacheNodes;
	  }

	  /// <summary>
	  /// Create a new NodeSequence from a (already cloned) iterator.
	  /// </summary>
	  /// <param name="nodeVector"> </param>
	  public NodeSequence(object nodeVector) : base(nodeVector)
	  {
		if (nodeVector is NodeVector)
		{
			SetVector((NodeVector) nodeVector);
		}
		  if (null != nodeVector)
		  {
			  assertion(nodeVector is NodeVector, "Must have a NodeVector as the object for NodeSequence!");
			  if (nodeVector is DTMIterator)
			  {
				  Iter = (DTMIterator)nodeVector;
				  m_last = ((DTMIterator)nodeVector).Length;
			  }

		  }
	  }

	  /// <summary>
	  /// Construct an empty XNodeSet object.  This is used to create a mutable 
	  /// nodeset to which random nodes may be added.
	  /// </summary>
	  private NodeSequence(DTMManager dtmMgr) : base(new NodeVector())
	  {
		m_last = 0;
		m_dtmMgr = dtmMgr;
	  }


	  /// <summary>
	  /// Create a new NodeSequence in an invalid (null) state.
	  /// </summary>
	  public NodeSequence()
	  {
		  return;
	  }


	  /// <seealso cref="DTMIterator.getDTM(int)"/>
	  public virtual DTM getDTM(int nodeHandle)
	  {
		  DTMManager mgr = DTMManager;
		  if (null != mgr)
		  {
			return DTMManager.getDTM(nodeHandle);
		  }
		else
		{
			assertion(false, "Can not get a DTM Unless a DTMManager has been set!");
			return null;
		}
	  }

	  /// <seealso cref="DTMIterator.getDTMManager()"/>
	  public virtual DTMManager DTMManager
	  {
		  get
		  {
			return m_dtmMgr;
		  }
	  }

	  /// <seealso cref="DTMIterator.getRoot()"/>
	  public virtual int Root
	  {
		  get
		  {
			  if (null != m_iter)
			  {
				return m_iter.Root;
			  }
			  else
			  {
				  // NodeSetDTM will call this, and so it's not a good thing to throw 
				  // an assertion here.
				  // assertion(false, "Can not get the root from a non-iterated NodeSequence!");
				  return DTM.NULL;
			  }
		  }
	  }

	  /// <seealso cref="DTMIterator.setRoot(int, Object)"/>
	  public virtual void setRoot(int nodeHandle, object environment)
	  {
		  if (null != m_iter)
		  {
			  XPathContext xctxt = (XPathContext)environment;
			  m_dtmMgr = xctxt.DTMManager;
			  m_iter.setRoot(nodeHandle, environment);
			  if (!m_iter.DocOrdered)
			  {
				  if (!hasCache())
				  {
					  ShouldCacheNodes = true;
				  }
				  runTo(-1);
				  m_next = 0;
			  }
		  }
		  else
		  {
			  assertion(false, "Can not setRoot on a non-iterated NodeSequence!");
		  }
	  }

	  /// <seealso cref="DTMIterator.reset()"/>
	  public override void reset()
	  {
		  m_next = 0;
		  // not resetting the iterator on purpose!!!
	  }

	  /// <seealso cref="DTMIterator.getWhatToShow()"/>
	  public virtual int WhatToShow
	  {
		  get
		  {
			return hasCache() ? (DTMFilter.SHOW_ALL & ~DTMFilter.SHOW_ENTITY_REFERENCE) : m_iter.WhatToShow;
		  }
	  }

	  /// <seealso cref="DTMIterator.getExpandEntityReferences()"/>
	  public virtual bool ExpandEntityReferences
	  {
		  get
		  {
			  if (null != m_iter)
			  {
				  return m_iter.ExpandEntityReferences;
			  }
			  else
			  {
				return true;
			  }
		  }
	  }

	  /// <seealso cref="DTMIterator.nextNode()"/>
	  public virtual int nextNode()
	  {
		// If the cache is on, and the node has already been found, then 
		// just return from the list.
		NodeVector vec = ArrayList;
		if (null != vec)
		{
			// There is a cache
			if (m_next < vec.size())
			{
				// The node is in the cache, so just return it.
				int next = vec.elementAt(m_next);
				m_next++;
				return next;
			}
			else if (cacheComplete() || (-1 != m_last) || (null == m_iter))
			{
				m_next++;
				return DTM.NULL;
			}
		}

	  if (null == m_iter)
	  {
		return DTM.NULL;
	  }

		 int next = m_iter.nextNode();
		if (DTM.NULL != next)
		{
			if (hasCache())
			{
				if (m_iter.DocOrdered)
				{
					Vector.addElement(next);
					m_next++;
				}
				else
				{
					int insertIndex = addNodeInDocOrder(next);
					if (insertIndex >= 0)
					{
						m_next++;
					}
				}
			}
			else
			{
				m_next++;
			}
		}
		else
		{
			// We have exhausted the iterator, and if there is a cache
			// it must have all nodes in it by now, so let the cache
			// know that it is complete.
			markCacheComplete();

			m_last = m_next;
			m_next++;
		}

		return next;
	  }

	  /// <seealso cref="DTMIterator.previousNode()"/>
	  public virtual int previousNode()
	  {
		  if (hasCache())
		  {
			  if (m_next <= 0)
			  {
				  return DTM.NULL;
			  }
			  else
			  {
				  m_next--;
				  return item(m_next);
			  }
		  }
		  else
		  {
			int n = m_iter.previousNode();
			m_next = m_iter.CurrentPos;
			return m_next;
		  }
	  }

	  /// <seealso cref="DTMIterator.detach()"/>
	  public override void detach()
	  {
		  if (null != m_iter)
		  {
			  m_iter.detach();
		  }
		  base.detach();
	  }

	  /// <summary>
	  /// Calling this with a value of false will cause the nodeset 
	  /// to be cached. </summary>
	  /// <seealso cref="DTMIterator.allowDetachToRelease(boolean)"/>
	  public override void allowDetachToRelease(bool allowRelease)
	  {
		  if ((false == allowRelease) && !hasCache())
		  {
			  ShouldCacheNodes = true;
		  }

		  if (null != m_iter)
		  {
			  m_iter.allowDetachToRelease(allowRelease);
		  }
		  base.allowDetachToRelease(allowRelease);
	  }

	  /// <seealso cref="DTMIterator.getCurrentNode()"/>
	  public virtual int CurrentNode
	  {
		  get
		  {
			  if (hasCache())
			  {
				  int currentIndex = m_next - 1;
				  NodeVector vec = ArrayList;
				  if ((currentIndex >= 0) && (currentIndex < vec.size()))
				  {
					  return vec.elementAt(currentIndex);
				  }
				  else
				  {
					  return DTM.NULL;
				  }
			  }
    
			  if (null != m_iter)
			  {
				return m_iter.CurrentNode;
			  }
			  else
			  {
				  return DTM.NULL;
			  }
		  }
	  }

	  /// <seealso cref="DTMIterator.isFresh()"/>
	  public virtual bool Fresh
	  {
		  get
		  {
			return (0 == m_next);
		  }
	  }

	  /// <seealso cref="DTMIterator.setShouldCacheNodes(boolean)"/>
	  public virtual bool ShouldCacheNodes
	  {
		  set
		  {
			if (value)
			{
			  if (!hasCache())
			  {
				SetVector(new NodeVector());
			  }
		//	  else
		//	    getVector().RemoveAllNoClear();  // Is this good?
			}
			else
			{
			  SetVector(null);
			}
		  }
	  }

	  /// <seealso cref="DTMIterator.isMutable()"/>
	  public virtual bool Mutable
	  {
		  get
		  {
			return hasCache(); // though may be surprising if it also has an iterator!
		  }
	  }

	  /// <seealso cref="DTMIterator.getCurrentPos()"/>
	  public virtual int CurrentPos
	  {
		  get
		  {
			return m_next;
		  }
		  set
		  {
			  runTo(value);
		  }
	  }

	  /// <seealso cref="DTMIterator.runTo(int)"/>
	  public virtual void runTo(int index)
	  {
		int n;

		if (-1 == index)
		{
		  int pos = m_next;
		  while (DTM.NULL != (n = nextNode()))
		  {
				  ;
		  }
		  m_next = pos;
		}
		else if (m_next == index)
		{
		  return;
		}
		else if (hasCache() && m_next < Vector.size())
		{
		  m_next = index;
		}
		else if ((null == Vector) && (index < m_next))
		{
		  while ((m_next >= index) && DTM.NULL != (n = previousNode()))
		  {
				  ;
		  }
		}
		else
		{
		  while ((m_next < index) && DTM.NULL != (n = nextNode()))
		  {
				  ;
		  }
		}

	  }


	  /// <seealso cref="DTMIterator.item(int)"/>
	  public virtual int item(int index)
	  {
		  CurrentPos = index;
		  int n = nextNode();
		  m_next = index;
		  return n;
	  }

	  /// <seealso cref="DTMIterator.setItem(int, int)"/>
	  public virtual void setItem(int node, int index)
	  {
		  NodeVector vec = ArrayList;
		  if (null != vec)
		  {
			int oldNode = vec.elementAt(index);
			if (oldNode != node && m_cache.useCount() > 1)
			{
				/* If we are going to set the node at the given index
				 * to a different value, and the cache is shared
				 * (has a use count greater than 1)
				 * then make a copy of the cache and use it
				 * so we don't overwrite the value for other
				 * users of the cache.
				 */
				IteratorCache newCache = new IteratorCache();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.utils.NodeVector nv;
				NodeVector nv;
				try
				{
					nv = (NodeVector) vec.clone();
				}
				catch (CloneNotSupportedException e)
				{
					// This should never happen
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					Exception rte = new Exception(e.Message);
					throw rte;
				}
				newCache.Vector = nv;
				newCache.CacheComplete = true;
				m_cache = newCache;
				vec = nv;

				// Keep our superclass informed of the current NodeVector
				base.Object = nv;

				/* When we get to here the new cache has
				 * a use count of 1 and when setting a
				 * bunch of values on the same NodeSequence,
				 * such as when sorting, we will keep setting
				 * values in that same copy which has a use count of 1.
				 */
			}
			  vec.setElementAt(node, index);
			  m_last = vec.size();
		  }
		  else
		  {
			  m_iter.setItem(node, index);
		  }
	  }

	  /// <seealso cref="DTMIterator.getLength()"/>
	  public virtual int Length
	  {
		  get
		  {
			IteratorCache cache = Cache;
    
			  if (cache != null)
			  {
				// Nodes from the iterator are cached
				if (cache.Complete)
				{
					// All of the nodes from the iterator are cached
					// so just return the number of nodes in the cache
					NodeVector nv = cache.Vector;
					return nv.size();
				}
    
				// If this NodeSequence wraps a mutable nodeset, then
				// m_last will not reflect the size of the nodeset if
				// it has been mutated...
				if (m_iter is NodeSetDTM)
				{
					return m_iter.Length;
				}
    
				  if (-1 == m_last)
				  {
					  int pos = m_next;
					  runTo(-1);
					  m_next = pos;
				  }
				return m_last;
			  }
			  else
			  {
				  return (-1 == m_last) ? (m_last = m_iter.Length) : m_last;
			  }
		  }
	  }

	  /// <summary>
	  /// Note: Not a deep clone. </summary>
	  /// <seealso cref="DTMIterator.cloneWithReset()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public virtual DTMIterator cloneWithReset()
	  {
		  NodeSequence seq = (NodeSequence)base.clone();
		seq.m_next = 0;
		if (m_cache != null)
		{
			// In making this clone of an iterator we are making
			// another NodeSequence object it has a reference
			// to the same IteratorCache object as the original
			// so we need to remember that more than one
			// NodeSequence object shares the cache.
			m_cache.increaseUseCount();
		}

		return seq;
	  }

	  /// <summary>
	  /// Get a clone of this iterator, but don't reset the iteration in the 
	  /// process, so that it may be used from the current position.
	  /// Note: Not a deep clone.
	  /// </summary>
	  /// <returns> A clone of this object.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {
			  NodeSequence clone = (NodeSequence) base.clone();
			  if (null != m_iter)
			  {
				  clone.m_iter = (DTMIterator) m_iter.clone();
			  }
			  if (m_cache != null)
			  {
				  // In making this clone of an iterator we are making
				  // another NodeSequence object it has a reference
				  // to the same IteratorCache object as the original
				  // so we need to remember that more than one
				  // NodeSequence object shares the cache.
				  m_cache.increaseUseCount();
			  }

			  return clone;
	  }


	  /// <seealso cref="DTMIterator.isDocOrdered()"/>
	  public virtual bool DocOrdered
	  {
		  get
		  {
			  if (null != m_iter)
			  {
				  return m_iter.DocOrdered;
			  }
			  else
			  {
				return true; // can't be sure?
			  }
		  }
	  }

	  /// <seealso cref="DTMIterator.getAxis()"/>
	  public virtual int Axis
	  {
		  get
		  {
			  if (null != m_iter)
			  {
				return m_iter.Axis;
			  }
			else
			{
				assertion(false, "Can not getAxis from a non-iterated node sequence!");
				return 0;
			}
		  }
	  }

	  /// <seealso cref="PathComponent.getAnalysisBits()"/>
	  public virtual int AnalysisBits
	  {
		  get
		  {
			  if ((null != m_iter) && (m_iter is PathComponent))
			  {
				return ((PathComponent)m_iter).AnalysisBits;
			  }
			else
			{
				return 0;
			}
		  }
	  }

	  /// <seealso cref="org.apache.xpath.Expression.fixupVariables(Vector, int)"/>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		  base.fixupVariables(vars, globalsSize);
	  }

	  /// <summary>
	  /// Add the node into a vector of nodes where it should occur in
	  /// document order. </summary>
	  /// <param name="node"> The node to be added. </param>
	  /// <returns> insertIndex. </returns>
	  /// <exception cref="RuntimeException"> thrown if this NodeSetDTM is not of 
	  /// a mutable type. </exception>
	   protected internal virtual int addNodeInDocOrder(int node)
	   {
		  assertion(hasCache(), "addNodeInDocOrder must be done on a mutable sequence!");

		  int insertIndex = -1;

		  NodeVector vec = ArrayList;

		  // This needs to do a binary search, but a binary search 
		  // is somewhat tough because the sequence test involves 
		  // two nodes.
		  int size = vec.size(), i;

		  for (i = size - 1; i >= 0; i--)
		  {
			int child = vec.elementAt(i);

			if (child == node)
			{
			  i = -2; // Duplicate, suppress insert

			  break;
			}

			DTM dtm = m_dtmMgr.getDTM(node);
			if (!dtm.isNodeAfter(node, child))
			{
			  break;
			}
		  }

		  if (i != -2)
		  {
			insertIndex = i + 1;

			vec.insertElementAt(node, insertIndex);
		  }

		  // checkDups();
		  return insertIndex;
	   } // end addNodeInDocOrder(Vector v, Object obj)

	   /// <summary>
	   /// It used to be that many locations in the code simply
	   /// did an assignment to this.m_obj directly, rather than
	   /// calling the setObject(Object) method. The problem is
	   /// that our super-class would be updated on what the 
	   /// cache associated with this NodeSequence, but
	   /// we wouldn't know ourselves.
	   /// <para>
	   /// All setting of m_obj is done through setObject() now,
	   /// and this method over-rides the super-class method.
	   /// So now we are in the loop have an opportunity
	   /// to update some caching information.
	   /// 
	   /// </para>
	   /// </summary>
	   protected internal override object Object
	   {
		   set
		   {
			   if (value is NodeVector)
			   {
				   // Keep our superclass informed of the current NodeVector
				   // ... if we don't the smoketest fails (don't know why).
				   base.Object = value;
    
				   // A copy of the code of what SetVector() would do.
				   NodeVector v = (NodeVector)value;
				   if (m_cache != null)
				   {
					   m_cache.Vector = v;
				   }
				   else if (v != null)
				   {
					   m_cache = new IteratorCache();
					   m_cache.Vector = v;
				   }
			   }
			   else if (value is IteratorCache)
			   {
				   IteratorCache cache = (IteratorCache) value;
				   m_cache = cache;
				   m_cache.increaseUseCount();
    
				   // Keep our superclass informed of the current NodeVector
				   base.Object = cache.Vector;
			   }
			   else
			   {
				   base.Object = value;
			   }
    
		   }
	   }

	   /// <summary>
	   /// Each NodeSequence object has an iterator which is "walked".
	   /// As an iterator is walked one obtains nodes from it.
	   /// As those nodes are obtained they may be cached, making
	   /// the next walking of a copy or clone of the iterator faster.
	   /// This field (m_cache) is a reference to such a cache, 
	   /// which is populated as the iterator is walked.
	   /// <para>
	   /// Note that multiple NodeSequence objects may hold a 
	   /// reference to the same cache, and also 
	   /// (and this is important) the same iterator.
	   /// The iterator and its cache may be shared among 
	   /// many NodeSequence objects.
	   /// </para>
	   /// <para>
	   /// If one of the NodeSequence objects walks ahead
	   /// of the others it fills in the cache.
	   /// As the others NodeSequence objects catch up they
	   /// get their values from
	   /// the cache rather than the iterator itself, so
	   /// the iterator is only ever walked once and everyone
	   /// benefits from the cache.
	   /// </para>
	   /// <para>
	   /// At some point the cache may be
	   /// complete due to walking to the end of one of
	   /// the copies of the iterator, and the cache is
	   /// then marked as "complete".
	   /// and the cache will have no more nodes added to it.
	   /// </para>
	   /// <para>
	   /// Its use-count is the number of NodeSequence objects that use it.
	   /// </para>
	   /// </summary>
	   private sealed class IteratorCache
	   {
		   /// <summary>
		   /// A list of nodes already obtained from the iterator.
		   /// As the iterator is walked the nodes obtained from
		   /// it are appended to this list.
		   /// <para>
		   /// Both an iterator and its corresponding cache can
		   /// be shared by multiple NodeSequence objects.
		   /// </para>
		   /// <para>
		   /// For example, consider three NodeSequence objects
		   /// ns1, ns2 and ns3 doing such sharing, and the
		   /// nodes to be obtaind from the iterator being 
		   /// the sequence { 33, 11, 44, 22, 55 }.
		   /// </para>
		   /// <para>
		   /// If ns3.nextNode() is called 3 times the the
		   /// underlying iterator will have walked through
		   /// 33, 11, 55 and these three nodes will have been put
		   /// in the cache.
		   /// </para>
		   /// <para>
		   /// If ns2.nextNode() is called 2 times it will return
		   /// 33 and 11 from the cache, leaving the iterator alone.
		   /// </para>
		   /// <para>
		   /// If ns1.nextNode() is called 6 times it will return
		   /// 33 and 11 from the cache, then get 44, 22, 55 from
		   /// the iterator, and appending 44, 22, 55 to the cache.
		   /// On the sixth call it is found that the iterator is
		   /// exhausted and the cache is marked complete.
		   /// </para>
		   /// <para>
		   /// Should ns2 or ns3 have nextNode() called they will
		   /// know that the cache is complete, and they will
		   /// obtain all subsequent nodes from the cache.
		   /// </para>
		   /// <para>
		   /// Note that the underlying iterator, though shared
		   /// is only ever walked once. 
		   /// </para>
		   /// </summary>
			internal NodeVector m_vec2;

			/// <summary>
			/// true if the associated iterator is exhausted and
			/// all nodes obtained from it are in the cache.
			/// </summary>
			internal bool m_isComplete2;

			internal int m_useCount2;

			internal IteratorCache()
			{
				m_vec2 = null;
				m_isComplete2 = false;
				m_useCount2 = 1;
				return;
			}

			/// <summary>
			/// Returns count of how many NodeSequence objects share this
			/// IteratorCache object.
			/// </summary>
			internal int useCount()
			{
				return m_useCount2;
			}

			/// <summary>
			/// This method is called when yet another
			/// NodeSequence object uses, or shares
			/// this same cache.
			/// 
			/// </summary>
			internal void increaseUseCount()
			{
				if (m_vec2 != null)
				{
					m_useCount2++;
				}

			}


			/// <summary>
			/// Get the cached list of nodes obtained from
			/// the iterator so far.
			/// </summary>
			internal NodeVector Vector
			{
				get
				{
					return m_vec2;
				}
			}

			/// <summary>
			/// Call this method with 'true' if the
			/// iterator is exhausted and the cached list
			/// is complete, or no longer growing.
			/// </summary>
			internal bool CacheComplete
			{
				set
				{
					m_isComplete2 = value;
    
				}
			}

			/// <summary>
			/// Returns true if no cache is complete
			/// and immutable.
			/// </summary>
			internal bool Complete
			{
				get
				{
					return m_isComplete2;
				}
			}
	   }

		/// <summary>
		/// Get the cached list of nodes appended with
		/// values obtained from the iterator as
		/// a NodeSequence is walked when its
		/// nextNode() method is called.
		/// </summary>
		protected internal virtual IteratorCache IteratorCache
		{
			get
			{
				return m_cache;
			}
		}
	}


}