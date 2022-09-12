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
 * $Id: DTMDefaultBaseTraversers.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{

	using org.apache.xml.dtm;

	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using NodeCounter = org.apache.xalan.xsltc.dom.NodeCounter;

	/// <summary>
	/// This class implements the traversers for DTMDefaultBase.
	/// 
	/// PLEASE NOTE that the public interface for all traversers should be
	/// in terms of DTM Node Handles... but they may use the internal node
	/// identity indices within their logic, for efficiency's sake. Be very
	/// careful to avoid confusing these when maintaining this code.
	/// 
	/// </summary>
	public abstract class DTMDefaultBaseTraversers : DTMDefaultBase
	{

	  /// <summary>
	  /// Construct a DTMDefaultBaseTraversers object from a DOM node.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> The object that is used to specify the construction source. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> The factory to use for creating XMLStrings. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  public DTMDefaultBaseTraversers(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing)
	  {
	  }

	  /// <summary>
	  /// Construct a DTMDefaultBaseTraversers object from a DOM node.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="source"> The object that is used to specify the construction source. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> The factory to use for creating XMLStrings. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  /// <param name="blocksize"> The block size of the DTM. </param>
	  /// <param name="usePrevsib"> true if we want to build the previous sibling node array. </param>
	  /// <param name="newNameTable"> true if we want to use a new ExpandedNameTable for this DTM. </param>
	  public DTMDefaultBaseTraversers(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool usePrevsib, bool newNameTable) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, blocksize, usePrevsib, newNameTable)
	  {
	  }

	  /// <summary>
	  /// This returns a stateless "traverser", that can navigate
	  /// over an XPath axis, though perhaps not in document order.
	  /// </summary>
	  /// <param name="axis"> One of Axes.ANCESTORORSELF, etc.
	  /// </param>
	  /// <returns> A DTMAxisTraverser, or null if the given axis isn't supported. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DTMAxisTraverser getAxisTraverser(final int axis)
	  public override DTMAxisTraverser getAxisTraverser(int axis)
	  {

		DTMAxisTraverser traverser;

		if (null == m_traversers) // Cache of stateless traversers for this DTM
		{
		  m_traversers = new DTMAxisTraverser[Axis.NamesLength];
		  traverser = null;
		}
		else
		{
		  traverser = m_traversers[axis]; // Share/reuse existing traverser

		  if (traverser != null)
		  {
			return traverser;
		  }
		}

		switch (axis) // Generate new traverser
		{
		case Axis.ANCESTOR :
		  traverser = new AncestorTraverser(this);
		  break;
		case Axis.ANCESTORORSELF :
		  traverser = new AncestorOrSelfTraverser(this);
		  break;
		case Axis.ATTRIBUTE :
		  traverser = new AttributeTraverser(this);
		  break;
		case Axis.CHILD :
		  traverser = new ChildTraverser(this);
		  break;
		case Axis.DESCENDANT :
		  traverser = new DescendantTraverser(this);
		  break;
		case Axis.DESCENDANTORSELF :
		  traverser = new DescendantOrSelfTraverser(this);
		  break;
		case Axis.FOLLOWING :
		  traverser = new FollowingTraverser(this);
		  break;
		case Axis.FOLLOWINGSIBLING :
		  traverser = new FollowingSiblingTraverser(this);
		  break;
		case Axis.NAMESPACE :
		  traverser = new NamespaceTraverser(this);
		  break;
		case Axis.NAMESPACEDECLS :
		  traverser = new NamespaceDeclsTraverser(this);
		  break;
		case Axis.PARENT :
		  traverser = new ParentTraverser(this);
		  break;
		case Axis.PRECEDING :
		  traverser = new PrecedingTraverser(this);
		  break;
		case Axis.PRECEDINGSIBLING :
		  traverser = new PrecedingSiblingTraverser(this);
		  break;
		case Axis.SELF :
		  traverser = new SelfTraverser(this);
		  break;
		case Axis.ALL :
		  traverser = new AllFromRootTraverser(this);
		  break;
		case Axis.ALLFROMNODE :
		  traverser = new AllFromNodeTraverser(this);
		  break;
		case Axis.PRECEDINGANDANCESTOR :
		  traverser = new PrecedingAndAncestorTraverser(this);
		  break;
		case Axis.DESCENDANTSFROMROOT :
		  traverser = new DescendantFromRootTraverser(this);
		  break;
		case Axis.DESCENDANTSORSELFFROMROOT :
		  traverser = new DescendantOrSelfFromRootTraverser(this);
		  break;
		case Axis.ROOT :
		  traverser = new RootTraverser(this);
		  break;
		case Axis.FILTEREDLIST :
		  return null; // Don't want to throw an exception for this one.
		default :
		  throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_UNKNOWN_AXIS_TYPE, new object[]{Convert.ToString(axis)})); //"Unknown axis traversal type: "+axis);
		}

		if (null == traverser)
		{
		  throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_AXIS_TRAVERSER_NOT_SUPPORTED, new object[]{Axis.getNames(axis)}));
		}
		  // "Axis traverser not supported: "
		  //                       + Axis.names[axis]);

		m_traversers[axis] = traverser;

		return traverser;
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class AncestorTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public AncestorTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node if this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
				return outerInstance.getParent(current);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Process using identities
		  current = outerInstance.makeNodeIdentity(current);

		  while (DTM_Fields.NULL != (current = outerInstance.m_parent.elementAt(current)))
		  {
			if (outerInstance.m_exptype.elementAt(current) == expandedTypeID)
			{
			  return outerInstance.makeNodeHandle(current);
			}
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class AncestorOrSelfTraverser : AncestorTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public AncestorOrSelfTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  To see if
		/// the self node should be processed, use this function.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return context;
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  To see if
		/// the self node should be processed, use this function.  If the context
		/// node does not match the expanded type ID, this function will return
		/// false.
		/// </summary>
		/// <param name="context"> The context node of this traversal. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
				return (outerInstance.getExpandedTypeID(context) == expandedTypeID) ? context : next(context, context, expandedTypeID);
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Attribute access
	  /// </summary>
	  private class AttributeTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public AttributeTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
		  return (context == current) ? outerInstance.getFirstAttribute(context) : outerInstance.getNextAttribute(current);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  current = (context == current) ? outerInstance.getFirstAttribute(context) : outerInstance.getNextAttribute(current);

		  do
		  {
			if (outerInstance.getExpandedTypeID(current) == expandedTypeID)
			{
			  return current;
			}
		  } while (DTM_Fields.NULL != (current = outerInstance.getNextAttribute(current)));

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class ChildTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public ChildTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Get the next indexed node that matches the expanded type ID.  Before 
		/// calling this function, one should first call 
		/// <seealso cref="#isIndexed(int) isIndexed"/> to make sure that the index can 
		/// contain nodes that match the given expanded type ID.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis. </param>
		/// <param name="nextPotential"> The node found must match or occur after this node. </param>
		/// <param name="expandedTypeID"> The expanded type ID for the request.
		/// </param>
		/// <returns> The node ID or NULL if not found. </returns>
		protected internal virtual int getNextIndexed(int axisRoot, int nextPotential, int expandedTypeID)
		{

		  int nsIndex = outerInstance.m_expandedNameTable.getNamespaceID(expandedTypeID);
		  int lnIndex = outerInstance.m_expandedNameTable.getLocalNameID(expandedTypeID);

		  for (; ;)
		  {
			int nextID = outerInstance.findElementFromIndex(nsIndex, lnIndex, nextPotential);

			if (NOTPROCESSED != nextID)
			{
			  int parentID = outerInstance.m_parent.elementAt(nextID);

			  // Is it a child?
			  if (parentID == axisRoot)
			  {
				return nextID;
			  }

			  // If the parent occured before the subtree root, then 
			  // we know it is past the child axis.
			  if (parentID < axisRoot)
			  {
				  return org.apache.xml.dtm.DTM_Fields.NULL;
			  }

			  // Otherwise, it could be a descendant below the subtree root 
			  // children, or it could be after the subtree root.  So we have 
			  // to climb up until the parent is less than the subtree root, in 
			  // which case we return NULL, or until it is equal to the subtree 
			  // root, in which case we continue to look.
			  do
			  {
				parentID = outerInstance.m_parent.elementAt(parentID);
				if (parentID < axisRoot)
				{
				  return org.apache.xml.dtm.DTM_Fields.NULL;
				}
			  } while (parentID > axisRoot);

			  // System.out.println("Found node via index: "+first);
			  nextPotential = nextID + 1;
			  continue;
			}

			outerInstance.nextNode();

			if (!(outerInstance.m_nextsib.elementAt(axisRoot) == NOTPROCESSED))
			{
			  break;
			}
		  }

		  return DTM_Fields.NULL;
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse 
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// that the traversal starts from. </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return outerInstance.getFirstChild(context);
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse 
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  if (true)
		  {
			int identity = outerInstance.makeNodeIdentity(context);

			int firstMatch = getNextIndexed(identity, outerInstance._firstch(identity), expandedTypeID);

			return outerInstance.makeNodeHandle(firstMatch);
		  }
		  else
		  {
					// %REVIEW% Dead code. Eliminate?
			for (int current = outerInstance._firstch(outerInstance.makeNodeIdentity(context)); DTM_Fields.NULL != current; current = outerInstance._nextsib(current))
			{
			  if (outerInstance.m_exptype.elementAt(current) == expandedTypeID)
			  {
				  return outerInstance.makeNodeHandle(current);
			  }
			}
			return org.apache.xml.dtm.DTM_Fields.NULL;
		  }
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
		  return outerInstance.getNextSibling(current);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Process in Identifier space
		  for (current = outerInstance._nextsib(outerInstance.makeNodeIdentity(current)); DTM_Fields.NULL != current; current = outerInstance._nextsib(current))
		  {
			if (outerInstance.m_exptype.elementAt(current) == expandedTypeID)
			{
				return outerInstance.makeNodeHandle(current);
			}
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Super class for derived classes that want a convenient way to access 
	  /// the indexing mechanism.
	  /// </summary>
	  private abstract class IndexedDTMAxisTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public IndexedDTMAxisTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Tell if the indexing is on and the given expanded type ID matches 
		/// what is in the indexes.  Derived classes should call this before 
		/// calling <seealso cref="#getNextIndexed(int, int, int) getNextIndexed"/> method.
		/// </summary>
		/// <param name="expandedTypeID"> The expanded type ID being requested.
		/// </param>
		/// <returns> true if it is OK to call the 
		///         <seealso cref="#getNextIndexed(int, int, int) getNextIndexed"/> method. </returns>
		protected internal bool isIndexed(int expandedTypeID)
		{
		  return (outerInstance.m_indexing && ExpandedNameTable.ELEMENT == outerInstance.m_expandedNameTable.getType(expandedTypeID));
		}

		/// <summary>
		/// Tell if a node is outside the axis being traversed.  This method must be 
		/// implemented by derived classes, and must be robust enough to handle any 
		/// node that occurs after the axis root.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis. </param>
		/// <param name="identity"> The node in question.
		/// </param>
		/// <returns> true if the given node falls outside the axis being traversed. </returns>
		protected internal abstract bool isAfterAxis(int axisRoot, int identity);

		/// <summary>
		/// Tell if the axis has been fully processed to tell if a the wait for 
		/// an arriving node should terminate.  This method must be implemented 
		/// be a derived class.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis.
		/// </param>
		/// <returns> true if the axis has been fully processed. </returns>
		protected internal abstract bool axisHasBeenProcessed(int axisRoot);

		/// <summary>
		/// Get the next indexed node that matches the expanded type ID.  Before 
		/// calling this function, one should first call 
		/// <seealso cref="#isIndexed(int) isIndexed"/> to make sure that the index can 
		/// contain nodes that match the given expanded type ID.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis. </param>
		/// <param name="nextPotential"> The node found must match or occur after this node. </param>
		/// <param name="expandedTypeID"> The expanded type ID for the request.
		/// </param>
		/// <returns> The node ID or NULL if not found. </returns>
		protected internal virtual int getNextIndexed(int axisRoot, int nextPotential, int expandedTypeID)
		{

		  int nsIndex = outerInstance.m_expandedNameTable.getNamespaceID(expandedTypeID);
		  int lnIndex = outerInstance.m_expandedNameTable.getLocalNameID(expandedTypeID);

		  while (true)
		  {
			int next = outerInstance.findElementFromIndex(nsIndex, lnIndex, nextPotential);

			if (NOTPROCESSED != next)
			{
			  if (isAfterAxis(axisRoot, next))
			  {
				return org.apache.xml.dtm.DTM_Fields.NULL;
			  }

			  // System.out.println("Found node via index: "+first);
			  return next;
			}
			else if (axisHasBeenProcessed(axisRoot))
			{
			  break;
			}

			outerInstance.nextNode();
		  }

		  return DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class DescendantTraverser : IndexedDTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public DescendantTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// Get the first potential identity that can be returned.  This should 
		/// be overridded by classes that need to return the self node.
		/// </summary>
		/// <param name="identity"> The node identity of the root context of the traversal.
		/// </param>
		/// <returns> The first potential node that can be in the traversal. </returns>
		protected internal virtual int getFirstPotential(int identity)
		{
		  return identity + 1;
		}

		/// <summary>
		/// Tell if the axis has been fully processed to tell if a the wait for 
		/// an arriving node should terminate.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis.
		/// </param>
		/// <returns> true if the axis has been fully processed. </returns>
		protected internal override bool axisHasBeenProcessed(int axisRoot)
		{
		  return !(outerInstance.m_nextsib.elementAt(axisRoot) == NOTPROCESSED);
		}

		/// <summary>
		/// Get the subtree root identity from the handle that was passed in by 
		/// the caller.  Derived classes may override this to change the root 
		/// context of the traversal.
		/// </summary>
		/// <param name="handle"> handle to the root context. </param>
		/// <returns> identity of the root of the subtree. </returns>
		protected internal virtual int getSubtreeRoot(int handle)
		{
		  return outerInstance.makeNodeIdentity(handle);
		}

		/// <summary>
		/// Tell if this node identity is a descendant.  Assumes that
		/// the node info for the element has already been obtained.
		/// 
		/// %REVIEW% This is really parentFollowsRootInDocumentOrder ...
		/// which fails if the parent starts after the root ends.
		/// May be sufficient for this class's logic, but misleadingly named!
		/// </summary>
		/// <param name="subtreeRootIdentity"> The root context of the subtree in question. </param>
		/// <param name="identity"> The index number of the node in question. </param>
		/// <returns> true if the index is a descendant of _startNode. </returns>
		protected internal virtual bool isDescendant(int subtreeRootIdentity, int identity)
		{
		  return outerInstance._parent(identity) >= subtreeRootIdentity;
		}

		/// <summary>
		/// Tell if a node is outside the axis being traversed.  This method must be 
		/// implemented by derived classes, and must be robust enough to handle any 
		/// node that occurs after the axis root.
		/// </summary>
		/// <param name="axisRoot"> The root identity of the axis. </param>
		/// <param name="identity"> The node in question.
		/// </param>
		/// <returns> true if the given node falls outside the axis being traversed. </returns>
		protected internal override bool isAfterAxis(int axisRoot, int identity)
		{
		  // %REVIEW% Is there *any* cheaper way to do this?
				// Yes. In ID space, compare to axisRoot's successor
				// (next-sib or ancestor's-next-sib). Probably shallower search.
		  do
		  {
			if (identity == axisRoot)
			{
			  return false;
			}
			identity = outerInstance.m_parent.elementAt(identity);
		  } while (identity >= axisRoot);

		  return true;
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{

		  if (isIndexed(expandedTypeID))
		  {
			int identity = getSubtreeRoot(context);
			int firstPotential = getFirstPotential(identity);

			return outerInstance.makeNodeHandle(getNextIndexed(identity, firstPotential, expandedTypeID));
		  }

		  return next(context, context, expandedTypeID);
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{

		  int subtreeRootIdent = getSubtreeRoot(context);

		  for (current = outerInstance.makeNodeIdentity(current) + 1; ; current++)
		  {
			int type = outerInstance._type(current); // may call nextNode()

			if (!isDescendant(subtreeRootIdent, current))
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			if (org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE == type || org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE == type)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  int subtreeRootIdent = getSubtreeRoot(context);

		  current = outerInstance.makeNodeIdentity(current) + 1;

		  if (isIndexed(expandedTypeID))
		  {
			return outerInstance.makeNodeHandle(getNextIndexed(subtreeRootIdent, current, expandedTypeID));
		  }

		  for (; ; current++)
		  {
			int exptype = outerInstance._exptype(current); // may call nextNode()

			if (!isDescendant(subtreeRootIdent, current))
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			if (exptype != expandedTypeID)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class DescendantOrSelfTraverser : DescendantTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public DescendantOrSelfTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Get the first potential identity that can be returned, which is the 
		/// axis context, in this case.
		/// </summary>
		/// <param name="identity"> The node identity of the root context of the traversal.
		/// </param>
		/// <returns> The axis context. </returns>
		protected internal override int getFirstPotential(int identity)
		{
		  return identity;
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  To see if
		/// the self node should be processed, use this function.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return context;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the entire subtree, including the root node.
	  /// </summary>
	  private class AllFromNodeTraverser : DescendantOrSelfTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public AllFromNodeTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{

		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) + 1; ; current++)
		  {
			// Trickological code: _exptype() has the side-effect of
			// running nextNode until the specified node has been loaded,
			// and thus can be used to ensure that incremental construction of
			// the DTM has gotten this far. Using it just for that side-effect
			// is quite a kluge...
			outerInstance._exptype(current); // make sure it's here.

			if (!isDescendant(subtreeRootIdent, current))
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}
	  }

	  /// <summary>
	  /// Implements traversal of the following access, in document order.
	  /// </summary>
	  private class FollowingTraverser : DescendantTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public FollowingTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Get the first of the following.
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// that the traversal starts from. </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
				// Compute in ID space
				context = outerInstance.makeNodeIdentity(context);

		  int first;
		  int type = outerInstance._type(context);

		  if ((DTM_Fields.ATTRIBUTE_NODE == type) || (DTM_Fields.NAMESPACE_NODE == type))
		  {
			context = outerInstance._parent(context);
			first = outerInstance._firstch(context);

			if (org.apache.xml.dtm.DTM_Fields.NULL != first)
			{
			  return outerInstance.makeNodeHandle(first);
			}
		  }

		  do
		  {
			first = outerInstance._nextsib(context);

			if (org.apache.xml.dtm.DTM_Fields.NULL == first)
			{
			  context = outerInstance._parent(context);
			}
		  } while (org.apache.xml.dtm.DTM_Fields.NULL == first && org.apache.xml.dtm.DTM_Fields.NULL != context);

		  return outerInstance.makeNodeHandle(first);
		}

		/// <summary>
		/// Get the first of the following.
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
				// %REVIEW% This looks like it might want shift into identity space
				// to avoid repeated conversion in the individual functions
		  int first;
		  int type = outerInstance.getNodeType(context);

		  if ((DTM_Fields.ATTRIBUTE_NODE == type) || (DTM_Fields.NAMESPACE_NODE == type))
		  {
			context = outerInstance.getParent(context);
			first = outerInstance.getFirstChild(context);

			if (org.apache.xml.dtm.DTM_Fields.NULL != first)
			{
			  if (outerInstance.getExpandedTypeID(first) == expandedTypeID)
			  {
				return first;
			  }
			  else
			  {
				return next(context, first, expandedTypeID);
			  }
			}
		  }

		  do
		  {
			first = outerInstance.getNextSibling(context);

			if (org.apache.xml.dtm.DTM_Fields.NULL == first)
			{
			  context = outerInstance.getParent(context);
			}
			else
			{
			  if (outerInstance.getExpandedTypeID(first) == expandedTypeID)
			  {
				return first;
			  }
			  else
			  {
				return next(context, first, expandedTypeID);
			  }
			}
		  } while (org.apache.xml.dtm.DTM_Fields.NULL == first && org.apache.xml.dtm.DTM_Fields.NULL != context);

		  return first;
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
				// Compute in identity space
				current = outerInstance.makeNodeIdentity(current);

		  while (true)
		  {
			current++; // Only works on IDs, not handles.

					// %REVIEW% Are we using handles or indexes?
			int type = outerInstance._type(current); // may call nextNode()

			if (org.apache.xml.dtm.DTM_Fields.NULL == type)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			if (org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE == type || org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE == type)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Compute in ID space
				current = outerInstance.makeNodeIdentity(current);

		  while (true)
		  {
			current++;

			int etype = outerInstance._exptype(current); // may call nextNode()

			if (org.apache.xml.dtm.DTM_Fields.NULL == etype)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			if (etype != expandedTypeID)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class FollowingSiblingTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public FollowingSiblingTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
		  return outerInstance.getNextSibling(current);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  while (DTM_Fields.NULL != (current = outerInstance.getNextSibling(current)))
		  {
			if (outerInstance.getExpandedTypeID(current) == expandedTypeID)
			{
			  return current;
			}
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class NamespaceDeclsTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public NamespaceDeclsTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{

		  return (context == current) ? outerInstance.getFirstNamespaceNode(context, false) : outerInstance.getNextNamespaceNode(context, current, false);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  current = (context == current) ? outerInstance.getFirstNamespaceNode(context, false) : outerInstance.getNextNamespaceNode(context, current, false);

		  do
		  {
			if (outerInstance.getExpandedTypeID(current) == expandedTypeID)
			{
			  return current;
			}
		  } while (DTM_Fields.NULL != (current = outerInstance.getNextNamespaceNode(context, current, false)));

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class NamespaceTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public NamespaceTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{

		  return (context == current) ? outerInstance.getFirstNamespaceNode(context, true) : outerInstance.getNextNamespaceNode(context, current, true);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  current = (context == current) ? outerInstance.getFirstNamespaceNode(context, true) : outerInstance.getNextNamespaceNode(context, current, true);

		  do
		  {
			if (outerInstance.getExpandedTypeID(current) == expandedTypeID)
			{
			  return current;
			}
		  } while (DTM_Fields.NULL != (current = outerInstance.getNextNamespaceNode(context, current, true)));

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class ParentTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public ParentTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse 
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// that the traversal starts from. </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return outerInstance.getParent(context);
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse 
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int current, int expandedTypeID)
		{
				// Compute in ID space
		  current = outerInstance.makeNodeIdentity(current);

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (current = outerInstance.m_parent.elementAt(current)))
		  {
			if (outerInstance.m_exptype.elementAt(current) == expandedTypeID)
			{
			  return outerInstance.makeNodeHandle(current);
			}
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}



		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class PrecedingTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public PrecedingTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Tell if the current identity is an ancestor of the context identity.
		/// This is an expensive operation, made worse by the stateless traversal.
		/// But the preceding axis is used fairly infrequently.
		/// </summary>
		/// <param name="contextIdent"> The context node of the axis traversal. </param>
		/// <param name="currentIdent"> The node in question. </param>
		/// <returns> true if the currentIdent node is an ancestor of contextIdent. </returns>
		protected internal virtual bool isAncestor(int contextIdent, int currentIdent)
		{
				// %REVIEW% See comments in IsAfterAxis; using the "successor" of
				// contextIdent is probably more efficient.
		  for (contextIdent = outerInstance.m_parent.elementAt(contextIdent); DTM_Fields.NULL != contextIdent; contextIdent = outerInstance.m_parent.elementAt(contextIdent))
		  {
			if (contextIdent == currentIdent)
			{
			  return true;
			}
		  }

		  return false;
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
				// compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) - 1; current >= 0; current--)
		  {
			short type = outerInstance._type(current);

			if (org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE == type || org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE == type || isAncestor(subtreeRootIdent, current))
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) - 1; current >= 0; current--)
		  {
			int exptype = outerInstance.m_exptype.elementAt(current);

			if (exptype != expandedTypeID || isAncestor(subtreeRootIdent, current))
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor and the Preceding axis,
	  /// in reverse document order.
	  /// </summary>
	  private class PrecedingAndAncestorTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public PrecedingAndAncestorTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
				// Compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) - 1; current >= 0; current--)
		  {
			short type = outerInstance._type(current);

			if (org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE == type || org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE == type)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) - 1; current >= 0; current--)
		  {
			int exptype = outerInstance.m_exptype.elementAt(current);

			if (exptype != expandedTypeID)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class PrecedingSiblingTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public PrecedingSiblingTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
		  return outerInstance.getPreviousSibling(current);
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{

		  while (DTM_Fields.NULL != (current = outerInstance.getPreviousSibling(current)))
		  {
			if (outerInstance.getExpandedTypeID(current) == expandedTypeID)
			{
			  return current;
			}
		  }

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Self axis.
	  /// </summary>
	  private class SelfTraverser : DTMAxisTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public SelfTraverser(DTMDefaultBaseTraversers outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  To see if
		/// the self node should be processed, use this function.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return context;
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  To see if
		/// the self node should be processed, use this function.  If the context
		/// node does not match the expanded type ID, this function will return
		/// false.
		/// </summary>
		/// <param name="context"> The context node of this traversal. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  return (outerInstance.getExpandedTypeID(context) == expandedTypeID) ? context : org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> Always return NULL for this axis. </returns>
		public override int next(int context, int current)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Ancestor access, in reverse document order.
	  /// </summary>
	  private class AllFromRootTraverser : AllFromNodeTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public AllFromRootTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Return the root.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return outerInstance.getDocumentRoot(context);
		}

		/// <summary>
		/// Return the root if it matches the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this traversal. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  return (outerInstance.getExpandedTypeID(outerInstance.getDocumentRoot(context)) == expandedTypeID) ? context : next(context, context, expandedTypeID);
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current)
		{
				// Compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) + 1; ; current++)
		  {
					// Kluge test: Just make sure +1 yielded a real node
			int type = outerInstance._type(current); // may call nextNode()
			if (type == org.apache.xml.dtm.DTM_Fields.NULL)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
				// Compute in ID space
		  int subtreeRootIdent = outerInstance.makeNodeIdentity(context);

		  for (current = outerInstance.makeNodeIdentity(current) + 1; ; current++)
		  {
			int exptype = outerInstance._exptype(current); // may call nextNode()

			if (exptype == org.apache.xml.dtm.DTM_Fields.NULL)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}

			if (exptype != expandedTypeID)
			{
			  continue;
			}

			return outerInstance.makeNodeHandle(current); // make handle.
		  }
		}
	  }

	  /// <summary>
	  /// Implements traversal of the Self axis.
	  /// </summary>
	  private class RootTraverser : AllFromRootTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public RootTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// Return the root if it matches the expanded type ID,
		/// else return null (nothing found)
		/// </summary>
		/// <param name="context"> The context node of this traversal. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  int root = outerInstance.getDocumentRoot(context);
		  return (outerInstance.getExpandedTypeID(root) == expandedTypeID) ? root : org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration.
		/// </param>
		/// <returns> Always return NULL for this axis. </returns>
		public override int next(int context, int current)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Traverse to the next node after the current node that is matched
		/// by the expanded type ID.
		/// </summary>
		/// <param name="context"> The context node of this iteration. </param>
		/// <param name="current"> The current node of the iteration. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the next node in the iteration, or DTM.NULL. </returns>
		public override int next(int context, int current, int expandedTypeID)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// A non-xpath axis, returns all nodes that aren't namespaces or attributes,
	  /// from and including the root.
	  /// </summary>
	  private class DescendantOrSelfFromRootTraverser : DescendantTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public DescendantOrSelfFromRootTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Get the first potential identity that can be returned, which is the axis 
		/// root context in this case.
		/// </summary>
		/// <param name="identity"> The node identity of the root context of the traversal.
		/// </param>
		/// <returns> The identity argument. </returns>
		protected internal override int getFirstPotential(int identity)
		{
		  return identity;
		}

		/// <summary>
		/// Get the first potential identity that can be returned. </summary>
		/// <param name="handle"> handle to the root context. </param>
		/// <returns> identity of the root of the subtree. </returns>
		protected internal override int getSubtreeRoot(int handle)
		{
				// %REVIEW% Shouldn't this always be 0?
		  return outerInstance.makeNodeIdentity(outerInstance.Document);
		}

		/// <summary>
		/// Return the root.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return outerInstance.getDocumentRoot(context);
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  if (isIndexed(expandedTypeID))
		  {
			int identity = 0;
			int firstPotential = getFirstPotential(identity);

			return outerInstance.makeNodeHandle(getNextIndexed(identity, firstPotential, expandedTypeID));
		  }

		  int root = first(context);
		  return next(root, root, expandedTypeID);
		}
	  }

	  /// <summary>
	  /// A non-xpath axis, returns all nodes that aren't namespaces or attributes,
	  /// from but not including the root.
	  /// </summary>
	  private class DescendantFromRootTraverser : DescendantTraverser
	  {
		  private readonly DTMDefaultBaseTraversers outerInstance;

		  public DescendantFromRootTraverser(DTMDefaultBaseTraversers outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Get the first potential identity that can be returned, which is the axis 
		/// root context in this case.
		/// </summary>
		/// <param name="identity"> The node identity of the root context of the traversal.
		/// </param>
		/// <returns> The identity argument. </returns>
		protected internal override int getFirstPotential(int identity)
		{
		  return outerInstance._firstch(0);
		}

		/// <summary>
		/// Get the first potential identity that can be returned. </summary>
		/// <param name="handle"> handle to the root context. </param>
		/// <returns> identity of the root of the subtree. </returns>
		protected internal override int getSubtreeRoot(int handle)
		{
		  return 0;
		}

		/// <summary>
		/// Return the root.
		/// </summary>
		/// <param name="context"> The context node of this traversal.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context)
		{
		  return outerInstance.makeNodeHandle(outerInstance._firstch(0));
		}

		/// <summary>
		/// By the nature of the stateless traversal, the context node can not be
		/// returned or the iteration will go into an infinate loop.  So to traverse
		/// an axis, the first function must be used to get the first node.
		/// 
		/// <para>This method needs to be overloaded only by those axis that process
		/// the self node. <\p>
		/// 
		/// </para>
		/// </summary>
		/// <param name="context"> The context node of this traversal. This is the point
		/// of origin for the traversal -- its "root node" or starting point. </param>
		/// <param name="expandedTypeID"> The expanded type ID that must match.
		/// </param>
		/// <returns> the first node in the traversal. </returns>
		public override int first(int context, int expandedTypeID)
		{
		  if (isIndexed(expandedTypeID))
		  {
			int identity = 0;
			int firstPotential = getFirstPotential(identity);

			return outerInstance.makeNodeHandle(getNextIndexed(identity, firstPotential, expandedTypeID));
		  }

		  int root = outerInstance.getDocumentRoot(context);
		  return next(root, root, expandedTypeID);
		}

	  }

	}

}