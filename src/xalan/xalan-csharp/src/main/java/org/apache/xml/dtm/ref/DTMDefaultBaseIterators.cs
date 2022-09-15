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
 * $Id: DTMDefaultBaseIterators.java 468653 2006-10-28 07:07:05Z minchau $
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
	/// </summary>
	public abstract class DTMDefaultBaseIterators : DTMDefaultBaseTraversers
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
	  public DTMDefaultBaseIterators(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing)
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
	  public DTMDefaultBaseIterators(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool usePrevsib, bool newNameTable) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, blocksize, usePrevsib, newNameTable)
	  {
	  }

	  /// <summary>
	  /// Get an iterator that can navigate over an XPath Axis, predicated by
	  /// the extended type ID.
	  /// Returns an iterator that must be initialized
	  /// with a start node (using iterator.setStartNode()).
	  /// </summary>
	  /// <param name="axis"> One of Axes.ANCESTORORSELF, etc. </param>
	  /// <param name="type"> An extended type ID.
	  /// </param>
	  /// <returns> A DTMAxisIterator, or null if the given axis isn't supported. </returns>
	  public override DTMAxisIterator getTypedAxisIterator(int axis, int type)
	  {

		DTMAxisIterator iterator = null;

		/* This causes an error when using patterns for elements that
		   do not exist in the DOM (translet types which do not correspond
		   to a DOM type are mapped to the DOM.ELEMENT type).
		*/

		//        if (type == NO_TYPE) {
		//            return(EMPTYITERATOR);
		//        }
		//        else if (type == ELEMENT) {
		//            iterator = new FilterIterator(getAxisIterator(axis),
		//                                          getElementFilter());
		//        }
		//        else 
		{
		  switch (axis)
		  {
		  case Axis.SELF :
			iterator = new TypedSingletonIterator(this, type);
			break;
		  case Axis.CHILD :
			iterator = new TypedChildrenIterator(this, type);
			break;
		  case Axis.PARENT :
			return ((new ParentIterator(this)).setNodeType(type));
		  case Axis.ANCESTOR :
			return (new TypedAncestorIterator(this, type));
		  case Axis.ANCESTORORSELF :
			return ((new TypedAncestorIterator(this, type)).includeSelf());
		  case Axis.ATTRIBUTE :
			return (new TypedAttributeIterator(this, type));
		  case Axis.DESCENDANT :
			iterator = new TypedDescendantIterator(this, type);
			break;
		  case Axis.DESCENDANTORSELF :
			iterator = (new TypedDescendantIterator(this, type)).includeSelf();
			break;
		  case Axis.FOLLOWING :
			iterator = new TypedFollowingIterator(this, type);
			break;
		  case Axis.PRECEDING :
			iterator = new TypedPrecedingIterator(this, type);
			break;
		  case Axis.FOLLOWINGSIBLING :
			iterator = new TypedFollowingSiblingIterator(this, type);
			break;
		  case Axis.PRECEDINGSIBLING :
			iterator = new TypedPrecedingSiblingIterator(this, type);
			break;
		  case Axis.NAMESPACE :
			iterator = new TypedNamespaceIterator(this, type);
			break;
		  case Axis.ROOT :
			iterator = new TypedRootIterator(this, type);
			break;
		  default :
			throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, new object[]{Axis.getNames(axis)}));
				//"Error: typed iterator for axis "
								   //+ Axis.names[axis] + "not implemented");
		  }
		}

		return (iterator);
	  }

	  /// <summary>
	  /// This is a shortcut to the iterators that implement the
	  /// XPath axes.
	  /// Returns a bare-bones iterator that must be initialized
	  /// with a start node (using iterator.setStartNode()).
	  /// </summary>
	  /// <param name="axis"> One of Axes.ANCESTORORSELF, etc.
	  /// </param>
	  /// <returns> A DTMAxisIterator, or null if the given axis isn't supported. </returns>
	  public override DTMAxisIterator getAxisIterator(in int axis)
	  {

		DTMAxisIterator iterator = null;

		switch (axis)
		{
		case Axis.SELF :
		  iterator = new SingletonIterator(this);
		  break;
		case Axis.CHILD :
		  iterator = new ChildrenIterator(this);
		  break;
		case Axis.PARENT :
		  return (new ParentIterator(this));
		case Axis.ANCESTOR :
		  return (new AncestorIterator(this));
		case Axis.ANCESTORORSELF :
		  return ((new AncestorIterator(this)).includeSelf());
		case Axis.ATTRIBUTE :
		  return (new AttributeIterator(this));
		case Axis.DESCENDANT :
		  iterator = new DescendantIterator(this);
		  break;
		case Axis.DESCENDANTORSELF :
		  iterator = (new DescendantIterator(this)).includeSelf();
		  break;
		case Axis.FOLLOWING :
		  iterator = new FollowingIterator(this);
		  break;
		case Axis.PRECEDING :
		  iterator = new PrecedingIterator(this);
		  break;
		case Axis.FOLLOWINGSIBLING :
		  iterator = new FollowingSiblingIterator(this);
		  break;
		case Axis.PRECEDINGSIBLING :
		  iterator = new PrecedingSiblingIterator(this);
		  break;
		case Axis.NAMESPACE :
		  iterator = new NamespaceIterator(this);
		  break;
		case Axis.ROOT :
		  iterator = new RootIterator(this);
		  break;
		default :
		  throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ITERATOR_AXIS_NOT_IMPLEMENTED, new object[]{Axis.getNames(axis)}));
			//"Error: iterator for axis '" + Axis.names[axis]
								 //+ "' not implemented");
		}

		return (iterator);
	  }

	  /// <summary>
	  /// Abstract superclass defining behaviors shared by all DTMDefault's
	  /// internal implementations of DTMAxisIterator. Subclass this (and
	  /// override, if necessary) to implement the specifics of an
	  /// individual axis iterator.
	  /// 
	  /// Currently there isn't a lot here
	  /// </summary>
	  public abstract class InternalAxisIteratorBase : DTMAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public InternalAxisIteratorBase(DTMDefaultBaseIterators outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		// %REVIEW% We could opt to share _nodeType and setNodeType() as
		// well, and simply ignore them in iterators which don't use them.
		// But Scott's worried about the overhead involved in cloning
		// these, and wants them to have as few fields as possible. Note
		// that we can't create a TypedInternalAxisIteratorBase because
		// those are often based on the untyped versions and Java doesn't
		// support multiple inheritance. <sigh/>

		/// <summary>
		/// Current iteration location. Usually this is the last location
		/// returned (starting point for the next() search); for single-node
		/// iterators it may instead be initialized to point to that single node.
		/// </summary>
		protected internal int _currentNode;

		/// <summary>
		/// Remembers the current node for the next call to gotoMark().
		/// 
		/// %REVIEW% Should this save _position too?
		/// </summary>
		public override void setMark()
		{
		  _markedNode = _currentNode;
		}

		/// <summary>
		/// Restores the current node remembered by setMark().
		/// 
		/// %REVEIW% Should this restore _position too?
		/// </summary>
		public override void gotoMark()
		{
		  _currentNode = _markedNode;
		}

	  } // end of InternalAxisIteratorBase

	  /// <summary>
	  /// Iterator that returns all immediate children of a given node
	  /// </summary>
	  public sealed class ChildrenIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public ChildrenIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Setting start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// 
		/// If the iterator is not restartable, this has no effect.
		/// %REVIEW% Should it return/throw something in that case,
		/// or set current node to END, to indicate request-not-honored?
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = (node == DTM.NULL) ? DTM.NULL : outerInstance._firstch(outerInstance.makeNodeIdentity(node));

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END if no more
		/// are available. </returns>
		public override int next()
		{
		  if (_currentNode != NULL)
		  {
			int node = _currentNode;
			_currentNode = outerInstance._nextsib(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }

		  return END;
		}
	  } // end of ChildrenIterator

	  /// <summary>
	  /// Iterator that returns the parent of a given node. Note that
	  /// this delivers only a single node; if you want all the ancestors,
	  /// see AncestorIterator.
	  /// </summary>
	  public sealed class ParentIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public ParentIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal int _nodeType = -1;

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = outerInstance.getParent(node);

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Set the node type of the parent that we're looking for.
		/// Note that this does _not_ mean "find the nearest ancestor of
		/// this type", but "yield the parent if it is of this type".
		/// 
		/// </summary>
		/// <param name="type"> extended type ID.
		/// </param>
		/// <returns> ParentIterator configured with the type filter set. </returns>
		public DTMAxisIterator setNodeType(in int type)
		{

		  _nodeType = type;

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration. In this case, we return
		/// only the immediate parent, _if_ it matches the requested nodeType.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  int result = _currentNode;

		  if (_nodeType >= DTM.NTYPES)
		  {
			if (_nodeType != outerInstance.getExpandedTypeID(_currentNode))
			{
			  result = END;
			}
		  }
		  else if (_nodeType != NULL)
		  {
			if (_nodeType != outerInstance.getNodeType(_currentNode))
			{
			  result = END;
			}
		  }

		  _currentNode = END;

		  return returnNode(result);
		}
	  } // end of ParentIterator

	  /// <summary>
	  /// Iterator that returns children of a given type for a given node.
	  /// The functionality chould be achieved by putting a filter on top
	  /// of a basic child iterator, but a specialised iterator is used
	  /// for efficiency (both speed and size of translet).
	  /// </summary>
	  public sealed class TypedChildrenIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedChildrenIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedChildrenIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = (node == DTM.NULL) ? DTM.NULL : outerInstance._firstch(outerInstance.makeNodeIdentity(_startNode));

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  int eType;
		  int node = _currentNode;

		  int nodeType = _nodeType;

		  if (nodeType >= DTM.NTYPES)
		  {
			while (node != DTM.NULL && outerInstance._exptype(node) != nodeType)
			{
			  node = outerInstance._nextsib(node);
			}
		  }
		  else
		  {
			while (node != DTM.NULL)
			{
			  eType = outerInstance._exptype(node);
			  if (eType < DTM.NTYPES)
			  {
				if (eType == nodeType)
				{
				  break;
				}
			  }
			  else if (outerInstance.m_expandedNameTable.getType(eType) == nodeType)
			  {
				break;
			  }
			  node = outerInstance._nextsib(node);
			}
		  }

		  if (node == DTM.NULL)
		  {
			_currentNode = DTM.NULL;
			return DTM.NULL;
		  }
		  else
		  {
			_currentNode = outerInstance._nextsib(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }

		}
	  } // end of TypedChildrenIterator

	  /// <summary>
	  /// Iterator that returns children within a given namespace for a
	  /// given node. The functionality chould be achieved by putting a
	  /// filter on top of a basic child iterator, but a specialised
	  /// iterator is used for efficiency (both speed and size of translet).
	  /// </summary>
	  public sealed class NamespaceChildrenIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID being requested. </summary>
		internal readonly int _nsType;

		/// <summary>
		/// Constructor NamespaceChildrenIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public NamespaceChildrenIterator(DTMDefaultBaseIterators outerInstance, in int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nsType = type;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = (node == DTM.NULL) ? DTM.NULL : NOTPROCESSED;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  if (_currentNode != DTM.NULL)
		  {
			for (int node = (NOTPROCESSED == _currentNode) ? outerInstance._firstch(outerInstance.makeNodeIdentity(_startNode)) : outerInstance._nextsib(_currentNode); node != END; node = outerInstance._nextsib(node))
			{
			  if (outerInstance.m_expandedNameTable.getNamespaceID(outerInstance._exptype(node)) == _nsType)
			  {
				_currentNode = node;

				return returnNode(node);
			  }
			}
		  }

		  return END;
		}
	  } // end of NamespaceChildrenIterator

	  /// <summary>
	  /// Iterator that returns the namespace nodes as defined by the XPath data model 
	  /// for a given node.
	  /// </summary>
	  public class NamespaceIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// Constructor NamespaceAttributeIterator
		/// </summary>
		public NamespaceIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		{

			this.outerInstance = outerInstance;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = outerInstance.getFirstNamespaceNode(node, true);

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  int node = _currentNode;

		  if (DTM.NULL != node)
		  {
			_currentNode = outerInstance.getNextNamespaceNode(_startNode, node, true);
		  }

		  return returnNode(node);
		}
	  } // end of NamespaceIterator

	  /// <summary>
	  /// Iterator that returns the namespace nodes as defined by the XPath data model 
	  /// for a given node, filtered by extended type ID.
	  /// </summary>
	  public class TypedNamespaceIterator : NamespaceIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedNamespaceIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedNamespaceIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
			int node;

		  for (node = _currentNode; node != END; node = outerInstance.getNextNamespaceNode(_startNode, node, true))
		  {
			if (outerInstance.getExpandedTypeID(node) == _nodeType || outerInstance.getNodeType(node) == _nodeType || outerInstance.getNamespaceType(node) == _nodeType)
			{
			  _currentNode = node;

			  return returnNode(node);
			}
		  }

		  return (_currentNode = END);
		}
	  } // end of TypedNamespaceIterator

	  /// <summary>
	  /// Iterator that returns the the root node as defined by the XPath data model 
	  /// for a given node.
	  /// </summary>
	  public class RootIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// Constructor RootIterator
		/// </summary>
		public RootIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		{

			this.outerInstance = outerInstance;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{

		  if (_isRestartable)
		  {
			_startNode = outerInstance.getDocumentRoot(node);
			_currentNode = NULL;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  if (_startNode == _currentNode)
		  {
			return NULL;
		  }

		  _currentNode = _startNode;

		  return returnNode(_startNode);
		}
	  } // end of RootIterator

	  /// <summary>
	  /// Iterator that returns the namespace nodes as defined by the XPath data model 
	  /// for a given node, filtered by extended type ID.
	  /// </summary>
	  public class TypedRootIterator : RootIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedRootIterator
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedRootIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
			if (_startNode == _currentNode)
			{
			return NULL;
			}

		  int nodeType = _nodeType;
		  int node = _startNode;
		  int expType = outerInstance.getExpandedTypeID(node);

		  _currentNode = node;

		  if (nodeType >= DTM.NTYPES)
		  {
			if (nodeType == expType)
			{
			  return returnNode(node);
			}
		  }
		  else
		  {
			if (expType < DTM.NTYPES)
			{
			  if (expType == nodeType)
			  {
				return returnNode(node);
			  }
			}
			else
			{
			  if (outerInstance.m_expandedNameTable.getType(expType) == nodeType)
			  {
				return returnNode(node);
			  }
			}
		  }

		  return END;
		}
	  } // end of TypedRootIterator

	  /// <summary>
	  /// Iterator that returns attributes within a given namespace for a node.
	  /// </summary>
	  public sealed class NamespaceAttributeIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID being requested. </summary>
		internal readonly int _nsType;

		/// <summary>
		/// Constructor NamespaceAttributeIterator
		/// 
		/// </summary>
		/// <param name="nsType"> The extended type ID being requested. </param>
		public NamespaceAttributeIterator(DTMDefaultBaseIterators outerInstance, int nsType) : base(outerInstance)
		{

			this.outerInstance = outerInstance;

		  _nsType = nsType;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = outerInstance.getFirstNamespaceNode(node, false);

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  int node = _currentNode;

		  if (DTM.NULL != node)
		  {
			_currentNode = outerInstance.getNextNamespaceNode(_startNode, node, false);
		  }

		  return returnNode(node);
		}
	  } // end of NamespaceAttributeIterator

	  /// <summary>
	  /// Iterator that returns all siblings of a given node.
	  /// </summary>
	  public class FollowingSiblingIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public FollowingSiblingIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = outerInstance.makeNodeIdentity(node);

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  _currentNode = (_currentNode == DTM.NULL) ? DTM.NULL : outerInstance._nextsib(_currentNode);
		  return returnNode(outerInstance.makeNodeHandle(_currentNode));
		}
	  } // end of FollowingSiblingIterator

	  /// <summary>
	  /// Iterator that returns all following siblings of a given node.
	  /// </summary>
	  public sealed class TypedFollowingSiblingIterator : FollowingSiblingIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedFollowingSiblingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedFollowingSiblingIterator(DTMDefaultBaseIterators outerInstance, int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = type;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  if (_currentNode == DTM.NULL)
		  {
			return DTM.NULL;
		  }

		  int node = _currentNode;
		  int eType;
		  int nodeType = _nodeType;

		  if (nodeType >= DTM.NTYPES)
		  {
			do
			{
			  node = outerInstance._nextsib(node);
			} while (node != DTM.NULL && outerInstance._exptype(node) != nodeType);
		  }
		  else
		  {
			while ((node = outerInstance._nextsib(node)) != DTM.NULL)
			{
			  eType = outerInstance._exptype(node);
			  if (eType < DTM.NTYPES)
			  {
				if (eType == nodeType)
				{
				  break;
				}
			  }
			  else if (outerInstance.m_expandedNameTable.getType(eType) == nodeType)
			  {
				break;
			  }
			}
		  }

		  _currentNode = node;

		  return (_currentNode == DTM.NULL) ? DTM.NULL : returnNode(outerInstance.makeNodeHandle(_currentNode));
		}
	  } // end of TypedFollowingSiblingIterator

	  /// <summary>
	  /// Iterator that returns attribute nodes (of what nodes?)
	  /// </summary>
	  public sealed class AttributeIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public AttributeIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		// assumes caller will pass element nodes

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			_currentNode = outerInstance.getFirstAttributeIdentity(outerInstance.makeNodeIdentity(node));

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int node = _currentNode;
		  int node = _currentNode;

		  if (node != NULL)
		  {
			_currentNode = outerInstance.getNextAttributeIdentity(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }

		  return NULL;
		}
	  } // end of AttributeIterator

	  /// <summary>
	  /// Iterator that returns attribute nodes of a given type
	  /// </summary>
	  public sealed class TypedAttributeIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedAttributeIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID that is requested. </param>
		public TypedAttributeIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		// assumes caller will pass element nodes

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
		  if (_isRestartable)
		  {
			_startNode = node;

			_currentNode = outerInstance.getTypedAttribute(node, _nodeType);

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int node = _currentNode;
		  int node = _currentNode;

		  // singleton iterator, since there can only be one attribute of 
		  // a given type.
		  _currentNode = NULL;

		  return returnNode(node);
		}
	  } // end of TypedAttributeIterator

	  /// <summary>
	  /// Iterator that returns preceding siblings of a given node
	  /// </summary>
	  public class PrecedingSiblingIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public PrecedingSiblingIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// The node identity of _startNode for this iterator
		/// </summary>
		protected internal int _startNodeID;

		/// <summary>
		/// True if this iterator has a reversed axis.
		/// </summary>
		/// <returns> true. </returns>
		public override bool Reverse
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;
			node = _startNodeID = outerInstance.makeNodeIdentity(node);

			if (node == NULL)
			{
			  _currentNode = node;
			  return resetPosition();
			}

			int type = outerInstance.m_expandedNameTable.getType(outerInstance._exptype(node));
			if (ExpandedNameTable.ATTRIBUTE == type || ExpandedNameTable.NAMESPACE == type)
			{
			  _currentNode = node;
			}
			else
			{
			  // Be careful to handle the Document node properly
			  _currentNode = outerInstance._parent(node);
			  if (NULL != _currentNode)
			  {
				_currentNode = outerInstance._firstch(_currentNode);
			  }
			  else
			  {
				_currentNode = node;
			  }
			}

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  if (_currentNode == _startNodeID || _currentNode == DTM.NULL)
		  {
			return NULL;
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int node = _currentNode;
			int node = _currentNode;
			_currentNode = outerInstance._nextsib(node);

			return returnNode(outerInstance.makeNodeHandle(node));
		  }
		}
	  } // end of PrecedingSiblingIterator

	  /// <summary>
	  /// Iterator that returns preceding siblings of a given type for
	  /// a given node
	  /// </summary>
	  public sealed class TypedPrecedingSiblingIterator : PrecedingSiblingIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedPrecedingSiblingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedPrecedingSiblingIterator(DTMDefaultBaseIterators outerInstance, int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = type;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  int node = _currentNode;
		  int expType;

		  int nodeType = _nodeType;
		  int startID = _startNodeID;

		  if (nodeType >= DTM.NTYPES)
		  {
			while (node != NULL && node != startID && outerInstance._exptype(node) != nodeType)
			{
			  node = outerInstance._nextsib(node);
			}
		  }
		  else
		  {
			while (node != NULL && node != startID)
			{
			  expType = outerInstance._exptype(node);
			  if (expType < DTM.NTYPES)
			  {
				if (expType == nodeType)
				{
				  break;
				}
			  }
			  else
			  {
				if (outerInstance.m_expandedNameTable.getType(expType) == nodeType)
				{
				  break;
				}
			  }
			  node = outerInstance._nextsib(node);
			}
		  }

		  if (node == DTM.NULL || node == _startNodeID)
		  {
			_currentNode = NULL;
			return NULL;
		  }
		  else
		  {
			_currentNode = outerInstance._nextsib(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }
		}
	  } // end of TypedPrecedingSiblingIterator

	  /// <summary>
	  /// Iterator that returns preceding nodes of a given node.
	  /// This includes the node set {root+1, start-1}, but excludes
	  /// all ancestors, attributes, and namespace nodes.
	  /// </summary>
	  public class PrecedingIterator : InternalAxisIteratorBase
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  _stack = new int[_maxAncestors];
		  }

		  private readonly DTMDefaultBaseIterators outerInstance;

		  public PrecedingIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }


		/// <summary>
		/// The max ancestors, but it can grow... </summary>
		internal readonly int _maxAncestors = 8;

		/// <summary>
		/// The stack of start node + ancestors up to the root of the tree,
		///  which we must avoid.
		/// </summary>
		protected internal int[] _stack;

		/// <summary>
		/// (not sure yet... -sb) </summary>
		protected internal int _sp, _oldsp;

		protected internal int _markedsp, _markedNode, _markedDescendant;

		/* _currentNode precedes candidates.  This is the identity, not the handle! */

		/// <summary>
		/// True if this iterator has a reversed axis.
		/// </summary>
		/// <returns> true since this iterator is a reversed axis. </returns>
		public override bool Reverse
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Returns a deep copy of this iterator.   The cloned iterator is not reset.
		/// </summary>
		/// <returns> a deep copy of this iterator. </returns>
		public override DTMAxisIterator cloneIterator()
		{
		  _isRestartable = false;

		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PrecedingIterator clone = (PrecedingIterator) super.clone();
			PrecedingIterator clone = (PrecedingIterator) base.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] stackCopy = new int[_stack.length];
			int[] stackCopy = new int[_stack.Length];
			Array.Copy(_stack, 0, stackCopy, 0, _stack.Length);

			clone._stack = stackCopy;

			// return clone.reset();
			return clone;
		  }
		  catch (CloneNotSupportedException)
		  {
			throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ITERATOR_CLONE_NOT_SUPPORTED, null)); //"Iterator clone not supported.");
		  }
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			node = outerInstance.makeNodeIdentity(node);

			// iterator is not a clone
			int parent, index;

		   if (outerInstance._type(node) == DTM.ATTRIBUTE_NODE)
		   {
			node = outerInstance._parent(node);
		   }

			_startNode = node;
			_stack[index = 0] = node;



			parent = node;
			while ((parent = outerInstance._parent(parent)) != NULL)
			{
				if (++index == _stack.Length)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] stack = new int[index + 4];
					int[] stack = new int[index + 4];
					Array.Copy(_stack, 0, stack, 0, index);
					_stack = stack;
				}
				_stack[index] = parent;
			}
			if (index > 0)
			{
				--index; // Pop actual root node (if not start) back off the stack
			}

			_currentNode = _stack[index]; // Last parent before root node

			_oldsp = _sp = index;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
			// Bugzilla 8324: We were forgetting to skip Attrs and NS nodes.
			// Also recoded the loop controls for clarity and to flatten out
			// the tail-recursion.
			   for (++_currentNode; _sp >= 0; ++_currentNode)
			   {
				   if (_currentNode < _stack[_sp])
				   {
					   if (outerInstance._type(_currentNode) != ATTRIBUTE_NODE && outerInstance._type(_currentNode) != NAMESPACE_NODE)
					   {
						   return returnNode(outerInstance.makeNodeHandle(_currentNode));
					   }
				   }
				   else
				   {
					   --_sp;
				   }
			   }
			   return NULL;
		}

		// redefine DTMAxisIteratorBase's reset

		/// <summary>
		/// Resets the iterator to the last start node.
		/// </summary>
		/// <returns> A DTMAxisIterator, which may or may not be the same as this
		///         iterator. </returns>
		public override DTMAxisIterator reset()
		{

		  _sp = _oldsp;

		  return resetPosition();
		}

		public override void setMark()
		{
			_markedsp = _sp;
			_markedNode = _currentNode;
			_markedDescendant = _stack[0];
		}

		public override void gotoMark()
		{
			_sp = _markedsp;
			_currentNode = _markedNode;
		}
	  } // end of PrecedingIterator

	  /// <summary>
	  /// Iterator that returns preceding nodes of agiven type for a
	  /// given node. This includes the node set {root+1, start-1}, but
	  /// excludes all ancestors.
	  /// </summary>
	  public sealed class TypedPrecedingIterator : PrecedingIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedPrecedingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedPrecedingIterator(DTMDefaultBaseIterators outerInstance, int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = type;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  int node = _currentNode;
		  int nodeType = _nodeType;

		  if (nodeType >= DTM.NTYPES)
		  {
			while (true)
			{
			  node = node + 1;

			  if (_sp < 0)
			  {
				node = NULL;
				break;
			  }
			  else if (node >= _stack[_sp])
			  {
				if (--_sp < 0)
				{
				  node = NULL;
				  break;
				}
			  }
			  else if (outerInstance._exptype(node) == nodeType)
			  {
				break;
			  }
			}
		  }
		  else
		  {
			int expType;

			while (true)
			{
			  node = node + 1;

			  if (_sp < 0)
			  {
				node = NULL;
				break;
			  }
			  else if (node >= _stack[_sp])
			  {
				if (--_sp < 0)
				{
				  node = NULL;
				  break;
				}
			  }
			  else
			  {
				expType = outerInstance._exptype(node);
				if (expType < DTM.NTYPES)
				{
				  if (expType == nodeType)
				  {
					break;
				  }
				}
				else
				{
				  if (outerInstance.m_expandedNameTable.getType(expType) == nodeType)
				  {
					break;
				  }
				}
			  }
			}
		  }

		  _currentNode = node;

		  return (node == NULL) ? NULL : returnNode(outerInstance.makeNodeHandle(node));
		}
	  } // end of TypedPrecedingIterator

	  /// <summary>
	  /// Iterator that returns following nodes of for a given node.
	  /// </summary>
	  public class FollowingIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		internal DTMAxisTraverser m_traverser; // easier for now

		public FollowingIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  m_traverser = outerInstance.getAxisTraverser(Axis.FOLLOWING);
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			_startNode = node;

			// ?? -sb
			// find rightmost descendant (or self)
			// int current;
			// while ((node = getLastChild(current = node)) != NULL){}
			// _currentNode = current;
			_currentNode = m_traverser.first(node);

			// _currentNode precedes possible following(node) nodes
			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  int node = _currentNode;

		  _currentNode = m_traverser.next(_startNode, _currentNode);

		  return returnNode(node);
		}
	  } // end of FollowingIterator

	  /// <summary>
	  /// Iterator that returns following nodes of a given type for a given node.
	  /// </summary>
	  public sealed class TypedFollowingIterator : FollowingIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedFollowingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedFollowingIterator(DTMDefaultBaseIterators outerInstance, int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = type;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  int node;

		  do
		  {
		   node = _currentNode;

		  _currentNode = m_traverser.next(_startNode, _currentNode);

		  } while (node != DTM.NULL && (outerInstance.getExpandedTypeID(node) != _nodeType && outerInstance.getNodeType(node) != _nodeType));

		  return (node == DTM.NULL ? DTM.NULL :returnNode(node));
		}
	  } // end of TypedFollowingIterator

	  /// <summary>
	  /// Iterator that returns the ancestors of a given node in document
	  /// order.  (NOTE!  This was changed from the XSLTC code!)
	  /// </summary>
	  public class AncestorIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public AncestorIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal org.apache.xml.utils.NodeVector m_ancestors = new org.apache.xml.utils.NodeVector();

		internal int m_ancestorsPos;

		internal int m_markedPos;

		/// <summary>
		/// The real start node for this axes, since _startNode will be adjusted. </summary>
		internal int m_realStartNode;

		/// <summary>
		/// Get start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <returns> The root node of the iteration. </returns>
		public override int StartNode
		{
			get
			{
			  return m_realStartNode;
			}
		}

		/// <summary>
		/// True if this iterator has a reversed axis.
		/// </summary>
		/// <returns> true since this iterator is a reversed axis. </returns>
		public sealed override bool Reverse
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Returns a deep copy of this iterator.  The cloned iterator is not reset.
		/// </summary>
		/// <returns> a deep copy of this iterator. </returns>
		public override DTMAxisIterator cloneIterator()
		{
		  _isRestartable = false; // must set to false for any clone

		  try
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AncestorIterator clone = (AncestorIterator) super.clone();
			AncestorIterator clone = (AncestorIterator) base.clone();

			clone._startNode = _startNode;

			// return clone.reset();
			return clone;
		  }
		  catch (CloneNotSupportedException)
		  {
			throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ITERATOR_CLONE_NOT_SUPPORTED, null)); //"Iterator clone not supported.");
		  }
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  m_realStartNode = node;

		  if (_isRestartable)
		  {
			int nodeID = outerInstance.makeNodeIdentity(node);

			if (!_includeSelf && node != DTM.NULL)
			{
			  nodeID = outerInstance._parent(nodeID);
			  node = outerInstance.makeNodeHandle(nodeID);
			}

			_startNode = node;

			while (nodeID != END)
			{
			  m_ancestors.addElement(node);
			  nodeID = outerInstance._parent(nodeID);
			  node = outerInstance.makeNodeHandle(nodeID);
			}
			m_ancestorsPos = m_ancestors.size() - 1;

			_currentNode = (m_ancestorsPos >= 0) ? m_ancestors.elementAt(m_ancestorsPos) : DTM.NULL;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Resets the iterator to the last start node.
		/// </summary>
		/// <returns> A DTMAxisIterator, which may or may not be the same as this
		///         iterator. </returns>
		public override DTMAxisIterator reset()
		{

		  m_ancestorsPos = m_ancestors.size() - 1;

		  _currentNode = (m_ancestorsPos >= 0) ? m_ancestors.elementAt(m_ancestorsPos) : DTM.NULL;

		  return resetPosition();
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  int next = _currentNode;

		  int pos = --m_ancestorsPos;

		  _currentNode = (pos >= 0) ? m_ancestors.elementAt(m_ancestorsPos) : DTM.NULL;

		  return returnNode(next);
		}

		public override void setMark()
		{
			m_markedPos = m_ancestorsPos;
		}

		public override void gotoMark()
		{
			m_ancestorsPos = m_markedPos;
			_currentNode = m_ancestorsPos >= 0 ? m_ancestors.elementAt(m_ancestorsPos) : DTM.NULL;
		}
	  } // end of AncestorIterator

	  /// <summary>
	  /// Typed iterator that returns the ancestors of a given node.
	  /// </summary>
	  public sealed class TypedAncestorIterator : AncestorIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedAncestorIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedAncestorIterator(DTMDefaultBaseIterators outerInstance, int type) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = type;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  m_realStartNode = node;

		  if (_isRestartable)
		  {
			int nodeID = outerInstance.makeNodeIdentity(node);
			int nodeType = _nodeType;

			if (!_includeSelf && node != DTM.NULL)
			{
			  nodeID = outerInstance._parent(nodeID);
			}

			_startNode = node;

			if (nodeType >= DTM.NTYPES)
			{
			  while (nodeID != END)
			  {
				int eType = outerInstance._exptype(nodeID);

				if (eType == nodeType)
				{
				  m_ancestors.addElement(outerInstance.makeNodeHandle(nodeID));
				}
				nodeID = outerInstance._parent(nodeID);
			  }
			}
			else
			{
			  while (nodeID != END)
			  {
				int eType = outerInstance._exptype(nodeID);

				if ((eType >= DTM.NTYPES && outerInstance.m_expandedNameTable.getType(eType) == nodeType) || (eType < DTM.NTYPES && eType == nodeType))
				{
				  m_ancestors.addElement(outerInstance.makeNodeHandle(nodeID));
				}
				nodeID = outerInstance._parent(nodeID);
			  }
			}
			m_ancestorsPos = m_ancestors.size() - 1;

			_currentNode = (m_ancestorsPos >= 0) ? m_ancestors.elementAt(m_ancestorsPos) : DTM.NULL;

			return resetPosition();
		  }

		  return this;
		}
	  } // end of TypedAncestorIterator

	  /// <summary>
	  /// Iterator that returns the descendants of a given node.
	  /// </summary>
	  public class DescendantIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;

		  public DescendantIterator(DTMDefaultBaseIterators outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isRestartable)
		  {
			node = outerInstance.makeNodeIdentity(node);
			_startNode = node;

			if (_includeSelf)
			{
			  node--;
			}

			_currentNode = node;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Tell if this node identity is a descendant.  Assumes that
		/// the node info for the element has already been obtained.
		/// 
		/// This one-sided test works only if the parent has been
		/// previously tested and is known to be a descendent. It fails if
		/// the parent is the _startNode's next sibling, or indeed any node
		/// that follows _startNode in document order.  That may suffice
		/// for this iterator, but it's not really an isDescendent() test.
		/// %REVIEW% rename?
		/// </summary>
		/// <param name="identity"> The index number of the node in question. </param>
		/// <returns> true if the index is a descendant of _startNode. </returns>
		protected internal virtual bool isDescendant(int identity)
		{
		  return (outerInstance._parent(identity) >= _startNode) || (_startNode == identity);
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  if (_startNode == NULL)
		  {
			return NULL;
		  }

		  if (_includeSelf && (_currentNode + 1) == _startNode)
		  {
			  return returnNode(outerInstance.makeNodeHandle(++_currentNode)); // | m_dtmIdent);
		  }

		  int node = _currentNode;
		  int type;

		  do
		  {
			node++;
			type = outerInstance._type(node);

			if (NULL == type || !isDescendant(node))
			{
			  _currentNode = NULL;
			  return END;
			}
		  } while (ATTRIBUTE_NODE == type || TEXT_NODE == type || NAMESPACE_NODE == type);

		  _currentNode = node;
		  return returnNode(outerInstance.makeNodeHandle(node)); // make handle.
		}

		/// <summary>
		/// Reset.
		/// 
		/// </summary>
	  public override DTMAxisIterator reset()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _isRestartable;
		bool temp = _isRestartable;

		_isRestartable = true;

		StartNode = outerInstance.makeNodeHandle(_startNode);

		_isRestartable = temp;

		return this;
	  }

	  } // end of DescendantIterator

	  /// <summary>
	  /// Typed iterator that returns the descendants of a given node.
	  /// </summary>
	  public sealed class TypedDescendantIterator : DescendantIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedDescendantIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> Extended type ID being requested. </param>
		public TypedDescendantIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
		  int node;
		  int type;

		  if (_startNode == NULL)
		  {
			return NULL;
		  }

		  node = _currentNode;

		  do
		  {
			node++;
			type = outerInstance._type(node);

			if (NULL == type || !isDescendant(node))
			{
			  _currentNode = NULL;
			  return END;
			}
		  } while (type != _nodeType && outerInstance._exptype(node) != _nodeType);

		  _currentNode = node;
		  return returnNode(outerInstance.makeNodeHandle(node));
		}
	  } // end of TypedDescendantIterator

	  /// <summary>
	  /// Iterator that returns the descendants of a given node.
	  /// I'm not exactly clear about this one... -sb
	  /// </summary>
	  public class NthDescendantIterator : DescendantIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The current nth position. </summary>
		internal int _pos;

		/// <summary>
		/// Constructor NthDescendantIterator
		/// 
		/// </summary>
		/// <param name="pos"> The nth position being requested. </param>
		public NthDescendantIterator(DTMDefaultBaseIterators outerInstance, int pos) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _pos = pos;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  // I'm not exactly clear yet what this is doing... -sb
		  int node;

		  while ((node = base.next()) != END)
		  {
			node = outerInstance.makeNodeIdentity(node);

			int parent = outerInstance._parent(node);
			int child = outerInstance._firstch(parent);
			int pos = 0;

			do
			{
			  int type = outerInstance._type(child);

			  if (ELEMENT_NODE == type)
			  {
				pos++;
			  }
			} while ((pos < _pos) && (child = outerInstance._nextsib(child)) != END);

			if (node == child)
			{
			  return node;
			}
		  }

		  return (END);
		}
	  } // end of NthDescendantIterator

	  /// <summary>
	  /// Class SingletonIterator.
	  /// </summary>
	  public class SingletonIterator : InternalAxisIteratorBase
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// (not sure yet what this is.  -sb)  (sc & sb remove final to compile in JDK 1.1.8) </summary>
		internal bool _isConstant;

		/// <summary>
		/// Constructor SingletonIterator
		/// 
		/// </summary>
		public SingletonIterator(DTMDefaultBaseIterators outerInstance) : this(outerInstance, int.MinValue, false)
		{
			this.outerInstance = outerInstance;
		}

		/// <summary>
		/// Constructor SingletonIterator
		/// 
		/// </summary>
		/// <param name="node"> The node handle to return. </param>
		public SingletonIterator(DTMDefaultBaseIterators outerInstance, int node) : this(outerInstance, node, false)
		{
			this.outerInstance = outerInstance;
		}

		/// <summary>
		/// Constructor SingletonIterator
		/// 
		/// </summary>
		/// <param name="node"> the node handle to return. </param>
		/// <param name="constant"> (Not sure what this is yet.  -sb) </param>
		public SingletonIterator(DTMDefaultBaseIterators outerInstance, int node, bool constant) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _currentNode = _startNode = node;
		  _isConstant = constant;
		}

		/// <summary>
		/// Set start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		/// <param name="node"> Sets the root of the iteration.
		/// </param>
		/// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
	//%HZ%: Added reference to DTMDefaultBase.ROOTNODE back in, temporarily
		  if (node == DTMDefaultBase.ROOTNODE)
		  {
			node = outerInstance.Document;
		  }
		  if (_isConstant)
		  {
			_currentNode = _startNode;

			return resetPosition();
		  }
		  else if (_isRestartable)
		  {
			_currentNode = _startNode = node;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Resets the iterator to the last start node.
		/// </summary>
		/// <returns> A DTMAxisIterator, which may or may not be the same as this
		///         iterator. </returns>
		public override DTMAxisIterator reset()
		{

		  if (_isConstant)
		  {
			_currentNode = _startNode;

			return resetPosition();
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _isRestartable;
			bool temp = _isRestartable;

			_isRestartable = true;

			StartNode = _startNode;

			_isRestartable = temp;
		  }

		  return this;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int result = _currentNode;
		  int result = _currentNode;

		  _currentNode = END;

		  return returnNode(result);
		}
	  } // end of SingletonIterator

	  /// <summary>
	  /// Iterator that returns a given node only if it is of a given type.
	  /// </summary>
	  public sealed class TypedSingletonIterator : SingletonIterator
	  {
		  private readonly DTMDefaultBaseIterators outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedSingletonIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedSingletonIterator(DTMDefaultBaseIterators outerInstance, int nodeType) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
		  _nodeType = nodeType;
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{

		  //final int result = super.next();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int result = _currentNode;
		  int result = _currentNode;
		  int nodeType = _nodeType;

		  _currentNode = END;

		  if (nodeType >= DTM.NTYPES)
		  {
			if (outerInstance.getExpandedTypeID(result) == nodeType)
			{
			  return returnNode(result);
			}
		  }
		  else
		  {
			if (outerInstance.getNodeType(result) == nodeType)
			{
			  return returnNode(result);
			}
		  }

		  return NULL;
		}
	  } // end of TypedSingletonIterator
	}

}