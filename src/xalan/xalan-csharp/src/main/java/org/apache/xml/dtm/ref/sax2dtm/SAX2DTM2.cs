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
 * $Id: SAX2DTM2.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref.sax2dtm
{
	using org.apache.xml.dtm;
	using org.apache.xml.dtm.@ref;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringDefault = org.apache.xml.utils.XMLStringDefault;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	using SuballocatedIntVector = org.apache.xml.utils.SuballocatedIntVector;
	using org.xml.sax;

	/// <summary>
	/// SAX2DTM2 is an optimized version of SAX2DTM which is used in non-incremental situation.
	/// It is used as the super class of the XSLTC SAXImpl. Many of the interfaces in SAX2DTM
	/// and DTMDefaultBase are overridden in SAX2DTM2 in order to allow fast, efficient
	/// access to the DTM model. Some nested iterators in DTMDefaultBaseIterators
	/// are also overridden in SAX2DTM2 for performance reasons.
	/// <para>
	/// Performance is the biggest consideration in the design of SAX2DTM2. To make the code most
	/// efficient, the incremental support is dropped in SAX2DTM2, which means that you should not
	/// use it in incremental situation. To reduce the overhead of pulling data from the DTM model,
	/// a few core interfaces in SAX2DTM2 have direct access to the internal arrays of the
	/// SuballocatedIntVectors.
	/// </para>
	/// <para>
	/// The design of SAX2DTM2 may limit its extensibilty. If you have a reason to extend the
	/// SAX2DTM model, please extend from SAX2DTM instead of this class.
	/// </para>
	/// <para>
	/// TODO: This class is currently only used by XSLTC. We need to investigate the possibility
	/// of also using it in Xalan-J Interpretive. Xalan's performance is likely to get an instant
	/// boost if we use SAX2DTM2 instead of SAX2DTM in non-incremental case.
	/// </para>
	/// <para>
	/// %MK% The code in this class is critical to the XSLTC_DTM performance. Be very careful
	/// when making changes here!
	/// </para>
	/// </summary>
	public class SAX2DTM2 : SAX2DTM
	{

	  /// <summary>
	  ///**************************************************************
	  ///       Optimized version of the nested iterators
	  /// ***************************************************************
	  /// </summary>

	  /// <summary>
	  /// Iterator that returns all immediate children of a given node
	  /// </summary>
	  public sealed class ChildrenIterator : InternalAxisIteratorBase
	  {
		  private readonly SAX2DTM2 outerInstance;

		  public ChildrenIterator(SAX2DTM2 outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Setting start to END should 'close' the iterator,
		/// i.e. subsequent call to next() should return END.
		/// <para>
		/// If the iterator is not restartable, this has no effect.
		/// %REVIEW% Should it return/throw something in that case,
		/// or set current node to END, to indicate request-not-honored?
		/// 
		/// </para>
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
			_currentNode = (node == DTM.NULL) ? DTM.NULL : outerInstance._firstch2(outerInstance.makeNodeIdentity(node));

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
			_currentNode = outerInstance._nextsib2(node);
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
		  private readonly SAX2DTM2 outerInstance;

		  public ParentIterator(SAX2DTM2 outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal int _nodeType = DTM.NULL;

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

			if (node != DTM.NULL)
			{
			  _currentNode = outerInstance._parent2(outerInstance.makeNodeIdentity(node));
			}
			else
			{
			  _currentNode = DTM.NULL;
			}

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
		  if (result == END)
		  {
			return DTM.NULL;
		  }

		  // %OPT% The most common case is handled first.
		  if (_nodeType == NULL)
		  {
			_currentNode = END;
			return returnNode(outerInstance.makeNodeHandle(result));
		  }
		  else if (_nodeType >= DTM.NTYPES)
		  {
			if (_nodeType == outerInstance._exptype2(result))
			{
			  _currentNode = END;
		  return returnNode(outerInstance.makeNodeHandle(result));
			}
		  }
		  else
		  {
			if (_nodeType == outerInstance._type2(result))
			{
		  _currentNode = END;
		  return returnNode(outerInstance.makeNodeHandle(result));
			}
		  }

		  return DTM.NULL;
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
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedChildrenIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedChildrenIterator(SAX2DTM2 outerInstance, int nodeType) : base(outerInstance)
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
			_currentNode = (node == DTM.NULL) ? DTM.NULL : outerInstance._firstch2(outerInstance.makeNodeIdentity(_startNode));

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
		  if (node == DTM.NULL)
		  {
			return DTM.NULL;
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;

		  if (nodeType != DTM.ELEMENT_NODE)
		  {
			while (node != DTM.NULL && outerInstance._exptype2(node) != nodeType)
			{
			  node = outerInstance._nextsib2(node);
			}
		  }
		  // %OPT% If the nodeType is element (matching child::*), we only
		  // need to compare the expType with DTM.NTYPES. A child node of
		  // an element can be either an element, text, comment or
		  // processing instruction node. Only element node has an extended
		  // type greater than or equal to DTM.NTYPES.
		  else
		  {
			  int eType;
			  while (node != DTM.NULL)
			  {
				eType = outerInstance._exptype2(node);
				if (eType >= DTM.NTYPES)
				{
				  break;
				}
				else
				{
				  node = outerInstance._nextsib2(node);
				}
			  }
		  }

		  if (node == DTM.NULL)
		  {
			_currentNode = DTM.NULL;
			return DTM.NULL;
		  }
		  else
		  {
			_currentNode = outerInstance._nextsib2(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }

		}

		/// <summary>
		/// Return the node at the given position.
		/// </summary>
		public override int getNodeByPosition(int position)
		{
		  if (position <= 0)
		  {
			return DTM.NULL;
		  }

		  int node = _currentNode;
		  int pos = 0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;
		  if (nodeType != DTM.ELEMENT_NODE)
		  {
			while (node != DTM.NULL)
			{
			  if (outerInstance._exptype2(node) == nodeType)
			  {
				pos++;
				if (pos == position)
				{
				  return outerInstance.makeNodeHandle(node);
				}
			  }

			  node = outerInstance._nextsib2(node);
			}
			return NULL;
		  }
		  else
		  {
			  while (node != DTM.NULL)
			  {
				if (outerInstance._exptype2(node) >= DTM.NTYPES)
				{
				  pos++;
				  if (pos == position)
				  {
					return outerInstance.makeNodeHandle(node);
				  }
				}
				node = outerInstance._nextsib2(node);
			  }
			  return NULL;
		  }
		}

	  } // end of TypedChildrenIterator

	  /// <summary>
	  /// Iterator that returns the namespace nodes as defined by the XPath data model
	  /// for a given node, filtered by extended type ID.
	  /// </summary>
	  public class TypedRootIterator : RootIterator
	  {
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedRootIterator
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedRootIterator(SAX2DTM2 outerInstance, int nodeType) : base(outerInstance)
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int node = _startNode;
		  int node = _startNode;
		  int expType = outerInstance._exptype2(outerInstance.makeNodeIdentity(node));

		  _currentNode = node;

		  if (_nodeType >= DTM.NTYPES)
		  {
			if (_nodeType == expType)
			{
			  return returnNode(node);
			}
		  }
		  else
		  {
			if (expType < DTM.NTYPES)
			{
			  if (expType == _nodeType)
			  {
				return returnNode(node);
			  }
			}
			else
			{
			  if (outerInstance.m_extendedTypes[expType].NodeType == _nodeType)
			  {
				return returnNode(node);
			  }
			}
		  }

		  return NULL;
		}
	  } // end of TypedRootIterator

	  /// <summary>
	  /// Iterator that returns all siblings of a given node.
	  /// </summary>
	  public class FollowingSiblingIterator : InternalAxisIteratorBase
	  {
		  private readonly SAX2DTM2 outerInstance;

		  public FollowingSiblingIterator(SAX2DTM2 outerInstance) : base(outerInstance)
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
		  _currentNode = (_currentNode == DTM.NULL) ? DTM.NULL : outerInstance._nextsib2(_currentNode);
		  return returnNode(outerInstance.makeNodeHandle(_currentNode));
		}
	  } // end of FollowingSiblingIterator

	  /// <summary>
	  /// Iterator that returns all following siblings of a given node.
	  /// </summary>
	  public sealed class TypedFollowingSiblingIterator : FollowingSiblingIterator
	  {
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedFollowingSiblingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedFollowingSiblingIterator(SAX2DTM2 outerInstance, int type) : base(outerInstance)
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;

		  if (nodeType != DTM.ELEMENT_NODE)
		  {
			while ((node = outerInstance._nextsib2(node)) != DTM.NULL && outerInstance._exptype2(node) != nodeType)
			{
			}
		  }
		  else
		  {
			while ((node = outerInstance._nextsib2(node)) != DTM.NULL && outerInstance._exptype2(node) < DTM.NTYPES)
			{
			}
		  }

		  _currentNode = node;

		  return (node == DTM.NULL) ? DTM.NULL : returnNode(outerInstance.makeNodeHandle(node));
		}

	  } // end of TypedFollowingSiblingIterator

	  /// <summary>
	  /// Iterator that returns attribute nodes (of what nodes?)
	  /// </summary>
	  public sealed class AttributeIterator : InternalAxisIteratorBase
	  {
		  private readonly SAX2DTM2 outerInstance;

		  public AttributeIterator(SAX2DTM2 outerInstance) : base(outerInstance)
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
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedAttributeIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID that is requested. </param>
		public TypedAttributeIterator(SAX2DTM2 outerInstance, int nodeType) : base(outerInstance)
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
		  private readonly SAX2DTM2 outerInstance;

		  public PrecedingSiblingIterator(SAX2DTM2 outerInstance) : base(outerInstance)
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

			int type = outerInstance._type2(node);
			if (ExpandedNameTable.ATTRIBUTE == type || ExpandedNameTable.NAMESPACE == type)
			{
			  _currentNode = node;
			}
			else
			{
			  // Be careful to handle the Document node properly
			  _currentNode = outerInstance._parent2(node);
			  if (NULL != _currentNode)
			  {
				_currentNode = outerInstance._firstch2(_currentNode);
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
			_currentNode = outerInstance._nextsib2(node);

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
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedPrecedingSiblingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedPrecedingSiblingIterator(SAX2DTM2 outerInstance, int type) : base(outerInstance)
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startNodeID = _startNodeID;
		  int startNodeID = _startNodeID;

		  if (nodeType != DTM.ELEMENT_NODE)
		  {
			while (node != NULL && node != startNodeID && outerInstance._exptype2(node) != nodeType)
			{
			  node = outerInstance._nextsib2(node);
			}
		  }
		  else
		  {
			while (node != NULL && node != startNodeID && outerInstance._exptype2(node) < DTM.NTYPES)
			{
			  node = outerInstance._nextsib2(node);
			}
		  }

		  if (node == DTM.NULL || node == startNodeID)
		  {
			_currentNode = NULL;
			return NULL;
		  }
		  else
		  {
			_currentNode = outerInstance._nextsib2(node);
			return returnNode(outerInstance.makeNodeHandle(node));
		  }
		}

		/// <summary>
		/// Return the index of the last node in this iterator.
		/// </summary>
		public override int Last
		{
			get
			{
			  if (_last != -1)
			  {
				return _last;
			  }
    
			  setMark();
    
			  int node = _currentNode;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int nodeType = _nodeType;
			  int nodeType = _nodeType;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int startNodeID = _startNodeID;
			  int startNodeID = _startNodeID;
    
			  int last = 0;
			  if (nodeType != DTM.ELEMENT_NODE)
			  {
				while (node != NULL && node != startNodeID)
				{
				  if (outerInstance._exptype2(node) == nodeType)
				  {
					last++;
				  }
				  node = outerInstance._nextsib2(node);
				}
			  }
			  else
			  {
				while (node != NULL && node != startNodeID)
				{
				  if (outerInstance._exptype2(node) >= DTM.NTYPES)
				  {
					last++;
				  }
				  node = outerInstance._nextsib2(node);
				}
			  }
    
			  gotoMark();
    
			  return (_last = last);
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

		  private readonly SAX2DTM2 outerInstance;

		  public PrecedingIterator(SAX2DTM2 outerInstance) : base(outerInstance)
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

		   if (outerInstance._type2(node) == DTM.ATTRIBUTE_NODE)
		   {
			 node = outerInstance._parent2(node);
		   }

			_startNode = node;
			_stack[index = 0] = node;

			   parent = node;
		while ((parent = outerInstance._parent2(parent)) != NULL)
		{
		  if (++index == _stack.Length)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] stack = new int[index*2];
			int[] stack = new int[index * 2];
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
			   int type = outerInstance._type2(_currentNode);
			   if (type != ATTRIBUTE_NODE && type != NAMESPACE_NODE)
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
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedPrecedingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedPrecedingIterator(SAX2DTM2 outerInstance, int type) : base(outerInstance)
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;

		  if (nodeType >= DTM.NTYPES)
		  {
			while (true)
			{
			  node++;

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
			  else if (outerInstance._exptype2(node) == nodeType)
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
			  node++;

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
				expType = outerInstance._exptype2(node);
				if (expType < DTM.NTYPES)
				{
				  if (expType == nodeType)
				  {
					break;
				  }
				}
				else
				{
				  if (outerInstance.m_extendedTypes[expType].NodeType == nodeType)
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
		  private readonly SAX2DTM2 outerInstance;

		//DTMAxisTraverser m_traverser; // easier for now

		public FollowingIterator(SAX2DTM2 outerInstance) : base(outerInstance)
		{
		  //m_traverser = getAxisTraverser(Axis.FOLLOWING);
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

			//_currentNode = m_traverser.first(node);

			node = outerInstance.makeNodeIdentity(node);

			int first;
			int type = outerInstance._type2(node);

			if ((DTM.ATTRIBUTE_NODE == type) || (DTM.NAMESPACE_NODE == type))
			{
			  node = outerInstance._parent2(node);
			  first = outerInstance._firstch2(node);

			  if (NULL != first)
			  {
				_currentNode = outerInstance.makeNodeHandle(first);
				return resetPosition();
			  }
			}

			do
			{
			  first = outerInstance._nextsib2(node);

			  if (NULL == first)
			  {
				node = outerInstance._parent2(node);
			  }
			} while (NULL == first && NULL != node);

			_currentNode = outerInstance.makeNodeHandle(first);

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

		  //_currentNode = m_traverser.next(_startNode, _currentNode);
		  int current = outerInstance.makeNodeIdentity(node);

		  while (true)
		  {
			current++;

			int type = outerInstance._type2(current);
			if (NULL == type)
			{
			  _currentNode = NULL;
			  return returnNode(node);
			}

			if (ATTRIBUTE_NODE == type || NAMESPACE_NODE == type)
			{
			  continue;
			}

			_currentNode = outerInstance.makeNodeHandle(current);
			return returnNode(node);
		  }
		}

	  } // end of FollowingIterator

	  /// <summary>
	  /// Iterator that returns following nodes of a given type for a given node.
	  /// </summary>
	  public sealed class TypedFollowingIterator : FollowingIterator
	  {
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedFollowingIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedFollowingIterator(SAX2DTM2 outerInstance, int type) : base(outerInstance)
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
		  int current;
		  int node;
		  int type;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;
		  int currentNodeID = outerInstance.makeNodeIdentity(_currentNode);

		  if (nodeType >= DTM.NTYPES)
		  {
			do
			{
			  node = currentNodeID;
		  current = node;

			  do
			  {
				current++;
				type = outerInstance._type2(current);
			  } while (type != NULL && (ATTRIBUTE_NODE == type || NAMESPACE_NODE == type));

			  currentNodeID = (type != NULL) ? current : NULL;
			} while (node != DTM.NULL && outerInstance._exptype2(node) != nodeType);
		  }
		  else
		  {
			do
			{
			  node = currentNodeID;
		  current = node;

			  do
			  {
				current++;
				type = outerInstance._type2(current);
			  } while (type != NULL && (ATTRIBUTE_NODE == type || NAMESPACE_NODE == type));

			  currentNodeID = (type != NULL) ? current : NULL;
			} while (node != DTM.NULL && (outerInstance._exptype2(node) != nodeType && outerInstance._type2(node) != nodeType));
		  }

		  _currentNode = outerInstance.makeNodeHandle(currentNodeID);
		  return (node == DTM.NULL ? DTM.NULL :returnNode(outerInstance.makeNodeHandle(node)));
		}
	  } // end of TypedFollowingIterator

	  /// <summary>
	  /// Iterator that returns the ancestors of a given node in document
	  /// order.  (NOTE!  This was changed from the XSLTC code!)
	  /// </summary>
	  public class AncestorIterator : InternalAxisIteratorBase
	  {
		  internal bool InstanceFieldsInitialized = false;

		  internal virtual void InitializeInstanceFields()
		  {
			  m_ancestors = new int[m_blocksize];
		  }

		  private readonly SAX2DTM2 outerInstance;

		  public AncestorIterator(SAX2DTM2 outerInstance) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  if (!InstanceFieldsInitialized)
			  {
				  InitializeInstanceFields();
				  InstanceFieldsInitialized = true;
			  }
		  }

		// The initial size of the ancestor array
		internal const int m_blocksize = 32;

		// The array for ancestor nodes. This array will grow dynamically.
		internal int[] m_ancestors;

		// Number of ancestor nodes in the array
		internal int m_size = 0;

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
			m_size = 0;

			if (nodeID == DTM.NULL)
			{
			  _currentNode = DTM.NULL;
			  m_ancestorsPos = 0;
			  return this;
			}

			// Start from the current node's parent if
			// _includeSelf is false.
			if (!_includeSelf)
			{
			  nodeID = outerInstance._parent2(nodeID);
			  node = outerInstance.makeNodeHandle(nodeID);
			}

			_startNode = node;

			while (nodeID != END)
			{
			  //m_ancestors.addElement(node);
			  if (m_size >= m_ancestors.Length)
			  {
				int[] newAncestors = new int[m_size * 2];
				Array.Copy(m_ancestors, 0, newAncestors, 0, m_ancestors.Length);
				m_ancestors = newAncestors;
			  }

			  m_ancestors[m_size++] = node;
			  nodeID = outerInstance._parent2(nodeID);
			  node = outerInstance.makeNodeHandle(nodeID);
			}

			m_ancestorsPos = m_size - 1;

			_currentNode = (m_ancestorsPos >= 0) ? m_ancestors[m_ancestorsPos] : DTM.NULL;

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

		  m_ancestorsPos = m_size - 1;

		  _currentNode = (m_ancestorsPos >= 0) ? m_ancestors[m_ancestorsPos] : DTM.NULL;

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

		  _currentNode = (pos >= 0) ? m_ancestors[m_ancestorsPos] : DTM.NULL;

		  return returnNode(next);
		}

		public override void setMark()
		{
			m_markedPos = m_ancestorsPos;
		}

		public override void gotoMark()
		{
			m_ancestorsPos = m_markedPos;
			_currentNode = m_ancestorsPos >= 0 ? m_ancestors[m_ancestorsPos] : DTM.NULL;
		}
	  } // end of AncestorIterator

	  /// <summary>
	  /// Typed iterator that returns the ancestors of a given node.
	  /// </summary>
	  public sealed class TypedAncestorIterator : AncestorIterator
	  {
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedAncestorIterator
		/// 
		/// </summary>
		/// <param name="type"> The extended type ID being requested. </param>
		public TypedAncestorIterator(SAX2DTM2 outerInstance, int type) : base(outerInstance)
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
			m_size = 0;

			if (nodeID == DTM.NULL)
			{
			  _currentNode = DTM.NULL;
			  m_ancestorsPos = 0;
			  return this;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
			int nodeType = _nodeType;

			if (!_includeSelf)
			{
			  nodeID = outerInstance._parent2(nodeID);
			  node = outerInstance.makeNodeHandle(nodeID);
			}

			_startNode = node;

			if (nodeType >= DTM.NTYPES)
			{
			  while (nodeID != END)
			  {
				int eType = outerInstance._exptype2(nodeID);

				if (eType == nodeType)
				{
				  if (m_size >= m_ancestors.Length)
				  {
					  int[] newAncestors = new int[m_size * 2];
					  Array.Copy(m_ancestors, 0, newAncestors, 0, m_ancestors.Length);
					  m_ancestors = newAncestors;
				  }
				  m_ancestors[m_size++] = outerInstance.makeNodeHandle(nodeID);
				}
				nodeID = outerInstance._parent2(nodeID);
			  }
			}
			else
			{
			  while (nodeID != END)
			  {
				int eType = outerInstance._exptype2(nodeID);

				if ((eType < DTM.NTYPES && eType == nodeType) || (eType >= DTM.NTYPES && outerInstance.m_extendedTypes[eType].NodeType == nodeType))
				{
				  if (m_size >= m_ancestors.Length)
				  {
					  int[] newAncestors = new int[m_size * 2];
					  Array.Copy(m_ancestors, 0, newAncestors, 0, m_ancestors.Length);
					  m_ancestors = newAncestors;
				  }
				  m_ancestors[m_size++] = outerInstance.makeNodeHandle(nodeID);
				}
				nodeID = outerInstance._parent2(nodeID);
			  }
			}
			m_ancestorsPos = m_size - 1;

			_currentNode = (m_ancestorsPos >= 0) ? m_ancestors[m_ancestorsPos] : DTM.NULL;

			return resetPosition();
		  }

		  return this;
		}

		/// <summary>
		/// Return the node at the given position.
		/// </summary>
		public override int getNodeByPosition(int position)
		{
		  if (position > 0 && position <= m_size)
		  {
			return m_ancestors[position - 1];
		  }
		  else
		  {
			return DTM.NULL;
		  }
		}

		/// <summary>
		/// Returns the position of the last node within the iteration, as
		/// defined by XPath.
		/// </summary>
		public override int Last
		{
			get
			{
			  return m_size;
			}
		}
	  } // end of TypedAncestorIterator

	  /// <summary>
	  /// Iterator that returns the descendants of a given node.
	  /// </summary>
	  public class DescendantIterator : InternalAxisIteratorBase
	  {
		  private readonly SAX2DTM2 outerInstance;

		  public DescendantIterator(SAX2DTM2 outerInstance) : base(outerInstance)
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
		protected internal bool isDescendant(int identity)
		{
		  return (outerInstance._parent2(identity) >= _startNode) || (_startNode == identity);
		}

		/// <summary>
		/// Get the next node in the iteration.
		/// </summary>
		/// <returns> The next node handle in the iteration, or END. </returns>
		public override int next()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startNode = _startNode;
		  int startNode = _startNode;
		  if (startNode == NULL)
		  {
			return NULL;
		  }

		  if (_includeSelf && (_currentNode + 1) == startNode)
		  {
			  return returnNode(outerInstance.makeNodeHandle(++_currentNode)); // | m_dtmIdent);
		  }

		  int node = _currentNode;
		  int type;

		  // %OPT% If the startNode is the root node, do not need
		  // to do the isDescendant() check.
		  if (startNode == ROOTNODE)
		  {
			int eType;
			do
			{
			  node++;
			  eType = outerInstance._exptype2(node);

			  if (NULL == eType)
			  {
				_currentNode = NULL;
				return END;
			  }
			} while (eType == TEXT_NODE || (type = outerInstance.m_extendedTypes[eType].NodeType) == ATTRIBUTE_NODE || type == NAMESPACE_NODE);
		  }
		  else
		  {
			do
			{
			  node++;
			  type = outerInstance._type2(node);

			  if (NULL == type || !isDescendant(node))
			  {
				_currentNode = NULL;
				return END;
			  }
			} while (ATTRIBUTE_NODE == type || TEXT_NODE == type || NAMESPACE_NODE == type);
		  }

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
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedDescendantIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> Extended type ID being requested. </param>
		public TypedDescendantIterator(SAX2DTM2 outerInstance, int nodeType) : base(outerInstance)
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startNode = _startNode;
		  int startNode = _startNode;
		  if (_startNode == NULL)
		  {
			return NULL;
		  }

		  int node = _currentNode;

		  int expType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = _nodeType;
		  int nodeType = _nodeType;

		  if (nodeType != DTM.ELEMENT_NODE)
		  {
			do
			{
			  node++;
		  expType = outerInstance._exptype2(node);

			  if (NULL == expType || outerInstance._parent2(node) < startNode && startNode != node)
			  {
				_currentNode = NULL;
				return END;
			  }
			} while (expType != nodeType);
		  }
		  // %OPT% If the start node is root (e.g. in the case of //node),
		  // we can save the isDescendant() check, because all nodes are
		  // descendants of root.
		  else if (startNode == DTMDefaultBase.ROOTNODE)
		  {
		do
		{
		  node++;
		  expType = outerInstance._exptype2(node);

		  if (NULL == expType)
		  {
			_currentNode = NULL;
			return END;
		  }
		} while (expType < DTM.NTYPES || outerInstance.m_extendedTypes[expType].NodeType != DTM.ELEMENT_NODE);
		  }
		  else
		  {
			do
			{
			  node++;
		  expType = outerInstance._exptype2(node);

			  if (NULL == expType || outerInstance._parent2(node) < startNode && startNode != node)
			  {
				_currentNode = NULL;
				return END;
			  }
			} while (expType < DTM.NTYPES || outerInstance.m_extendedTypes[expType].NodeType != DTM.ELEMENT_NODE);
		  }

		  _currentNode = node;
		  return returnNode(outerInstance.makeNodeHandle(node));
		}
	  } // end of TypedDescendantIterator

	  /// <summary>
	  /// Iterator that returns a given node only if it is of a given type.
	  /// </summary>
	  public sealed class TypedSingletonIterator : SingletonIterator
	  {
		  private readonly SAX2DTM2 outerInstance;


		/// <summary>
		/// The extended type ID that was requested. </summary>
		internal readonly int _nodeType;

		/// <summary>
		/// Constructor TypedSingletonIterator
		/// 
		/// </summary>
		/// <param name="nodeType"> The extended type ID being requested. </param>
		public TypedSingletonIterator(SAX2DTM2 outerInstance, int nodeType) : base(outerInstance)
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int result = _currentNode;
		  int result = _currentNode;
		  if (result == END)
		  {
			return DTM.NULL;
		  }

		  _currentNode = END;

		  if (_nodeType >= DTM.NTYPES)
		  {
			if (outerInstance._exptype2(outerInstance.makeNodeIdentity(result)) == _nodeType)
			{
			  return returnNode(result);
			}
		  }
		  else
		  {
			if (outerInstance._type2(outerInstance.makeNodeIdentity(result)) == _nodeType)
			{
			  return returnNode(result);
			}
		  }

		  return NULL;
		}
	  } // end of TypedSingletonIterator

	  /// <summary>
	  ///*****************************************************************
	  ///                End of nested iterators
	  /// ******************************************************************
	  /// </summary>


	  // %OPT% Array references which are used to cache the map0 arrays in
	  // SuballocatedIntVectors. Using the cached arrays reduces the level
	  // of indirection and results in better performance than just calling
	  // SuballocatedIntVector.elementAt().
	  private int[] m_exptype_map0;
	  private int[] m_nextsib_map0;
	  private int[] m_firstch_map0;
	  private int[] m_parent_map0;

	  // Double array references to the map arrays in SuballocatedIntVectors.
	  private int[][] m_exptype_map;
	  private int[][] m_nextsib_map;
	  private int[][] m_firstch_map;
	  private int[][] m_parent_map;

	  // %OPT% Cache the array of extended types in this class
	  protected internal ExtendedType[] m_extendedTypes;

	  // A Vector which is used to store the values of attribute, namespace,
	  // comment and PI nodes.
	  //
	  // %OPT% These values are unlikely to be equal. Storing
	  // them in a plain Vector is more efficient than storing in the
	  // DTMStringPool because we can save the cost for hash calculation.
	  //
	  // %REVISIT% Do we need a custom class (e.g. StringVector) here?
	  protected internal ArrayList m_values;

	  // The current index into the m_values Vector.
	  private int m_valueIndex = 0;

	  // The maximum value of the current node index.
	  private int m_maxNodeIndex;

	  // Cache the shift and mask values for the SuballocatedIntVectors.
	  protected internal int m_SHIFT;
	  protected internal int m_MASK;
	  protected internal int m_blocksize;

	  /// <summary>
	  /// %OPT% If the offset and length of a Text node are within certain limits,
	  /// we store a bitwise encoded value into an int, using 10 bits (max. 1024)
	  /// for length and 21 bits for offset. We can save two SuballocatedIntVector
	  /// calls for each getStringValueX() and dispatchCharacterEvents() call by
	  /// doing this.
	  /// </summary>
	  // The number of bits for the length of a Text node.
	  protected internal const int TEXT_LENGTH_BITS = 10;

	  // The number of bits for the offset of a Text node.
	  protected internal const int TEXT_OFFSET_BITS = 21;

	  // The maximum length value
	  protected internal static readonly int TEXT_LENGTH_MAX = (1 << TEXT_LENGTH_BITS) - 1;

	  // The maximum offset value
	  protected internal static readonly int TEXT_OFFSET_MAX = (1 << TEXT_OFFSET_BITS) - 1;

	  // True if we want to build the ID index table.
	  protected internal bool m_buildIdIndex = true;

	  // Constant for empty String
	  private const string EMPTY_STR = "";

	  // Constant for empty XMLString
	  private static readonly XMLString EMPTY_XML_STR = new XMLStringDefault("");

	  /// <summary>
	  /// Construct a SAX2DTM2 object using the default block size.
	  /// </summary>
	  public SAX2DTM2(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : this(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, DEFAULT_BLOCKSIZE, true, true, false)
	  {

	  }

	  /// <summary>
	  /// Construct a SAX2DTM2 object using the given block size.
	  /// </summary>
	  public SAX2DTM2(DTMManager mgr, Source source, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing, int blocksize, bool usePrevsib, bool buildIdIndex, bool newNameTable) : base(mgr, source, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing, blocksize, usePrevsib, newNameTable)
	  {


		// Initialize the values of m_SHIFT and m_MASK.
		int shift;
		for (shift = 0; (blocksize = (int)((uint)blocksize >> 1)) != 0; ++shift)
		{
			;
		}

		m_blocksize = 1 << shift;
		m_SHIFT = shift;
		m_MASK = m_blocksize - 1;

		m_buildIdIndex = buildIdIndex;

		// Some documents do not have attribute nodes. That is why
		// we set the initial size of this Vector to be small and set
		// the increment to a bigger number.
		m_values = new ArrayList(32, 512);

		m_maxNodeIndex = 1 << DTMManager.IDENT_DTM_NODE_BITS;

		// Set the map0 values in the constructor.
		m_exptype_map0 = m_exptype.Map0;
		m_nextsib_map0 = m_nextsib.Map0;
		m_firstch_map0 = m_firstch.Map0;
		m_parent_map0 = m_parent.Map0;
	  }

	  /// <summary>
	  /// Override DTMDefaultBase._exptype() by dropping the incremental code.
	  /// 
	  /// <para>This one is less efficient than _exptype2. It is only used during
	  /// DTM building. _exptype2 is used after the document is fully built.
	  /// </para>
	  /// </summary>
	  public sealed override int _exptype(int identity)
	  {
		return m_exptype.elementAt(identity);
	  }

	  /// <summary>
	  ///**********************************************************************
	  ///             DTM base accessor interfaces
	  /// 
	  /// %OPT% The code in the following interfaces (e.g. _exptype2, etc.) are
	  /// very important to the DTM performance. To have the best performace,
	  /// these several interfaces have direct access to the internal arrays of
	  /// the SuballocatedIntVectors. The final modifier also has a noticeable
	  /// impact on performance.
	  /// **********************************************************************
	  /// </summary>

	  /// <summary>
	  /// The optimized version of DTMDefaultBase._exptype().
	  /// </summary>
	  /// <param name="identity"> A node identity, which <em>must not</em> be equal to
	  ///        <code>DTM.NULL</code> </param>
	  public int _exptype2(int identity)
	  {
		//return m_exptype.elementAt(identity);

		if (identity < m_blocksize)
		{
		  return m_exptype_map0[identity];
		}
		else
		{
		  return m_exptype_map[(int)((uint)identity >> m_SHIFT)][identity & m_MASK];
		}
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase._nextsib().
	  /// </summary>
	  /// <param name="identity"> A node identity, which <em>must not</em> be equal to
	  ///        <code>DTM.NULL</code> </param>
	  public int _nextsib2(int identity)
	  {
		//return m_nextsib.elementAt(identity);

		if (identity < m_blocksize)
		{
		  return m_nextsib_map0[identity];
		}
		else
		{
		  return m_nextsib_map[(int)((uint)identity >> m_SHIFT)][identity & m_MASK];
		}
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase._firstch().
	  /// </summary>
	  /// <param name="identity"> A node identity, which <em>must not</em> be equal to
	  ///        <code>DTM.NULL</code> </param>
	  public int _firstch2(int identity)
	  {
		//return m_firstch.elementAt(identity);

		if (identity < m_blocksize)
		{
		  return m_firstch_map0[identity];
		}
		else
		{
		  return m_firstch_map[(int)((uint)identity >> m_SHIFT)][identity & m_MASK];
		}
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase._parent().
	  /// </summary>
	  /// <param name="identity"> A node identity, which <em>must not</em> be equal to
	  ///        <code>DTM.NULL</code> </param>
	  public int _parent2(int identity)
	  {
		//return m_parent.elementAt(identity);

		if (identity < m_blocksize)
		{
		  return m_parent_map0[identity];
		}
		else
		{
		  return m_parent_map[(int)((uint)identity >> m_SHIFT)][identity & m_MASK];
		}
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase._type().
	  /// </summary>
	  /// <param name="identity"> A node identity, which <em>must not</em> be equal to
	  ///        <code>DTM.NULL</code> </param>
	  public int _type2(int identity)
	  {
		//int eType = _exptype2(identity);
		int eType;
		if (identity < m_blocksize)
		{
		  eType = m_exptype_map0[identity];
		}
		else
		{
		  eType = m_exptype_map[(int)((uint)identity >> m_SHIFT)][identity & m_MASK];
		}

		if (NULL != eType)
		{
		  return m_extendedTypes[eType].NodeType;
		}
		else
		{
		  return NULL;
		}
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase.getExpandedTypeID(int).
	  /// 
	  /// <para>This one is only used by DOMAdapter.getExpandedTypeID(int), which
	  /// is mostly called from the compiled translets.
	  /// </para>
	  /// </summary>
	  public int getExpandedTypeID2(int nodeHandle)
	  {
		int nodeID = makeNodeIdentity(nodeHandle);

		//return (nodeID != NULL) ? _exptype2(nodeID) : NULL;

		if (nodeID != NULL)
		{
		  if (nodeID < m_blocksize)
		  {
			return m_exptype_map0[nodeID];
		  }
		  else
		  {
			return m_exptype_map[(int)((uint)nodeID >> m_SHIFT)][nodeID & m_MASK];
		  }
		}
		else
		{
		  return NULL;
		}
	  }

	  /// <summary>
	  ///***********************************************************************
	  ///                 END of DTM base accessor interfaces
	  /// ************************************************************************
	  /// </summary>


	  /// <summary>
	  /// Return the node type from the expanded type
	  /// </summary>
	  public int _exptype2Type(int exptype)
	  {
		if (NULL != exptype)
		{
		  return m_extendedTypes[exptype].NodeType;
		}
		else
		{
		  return NULL;
		}
	  }

	  /// <summary>
	  /// Get a prefix either from the uri mapping, or just make
	  /// one up!
	  /// </summary>
	  /// <param name="uri"> The namespace URI, which may be null.
	  /// </param>
	  /// <returns> The prefix if there is one, or null. </returns>
	  public override int getIdForNamespace(string uri)
	  {
		 int index = m_values.IndexOf(uri);
		 if (index < 0)
		 {
		   m_values.Add(uri);
		   return m_valueIndex++;
		 }
		 else
		 {
		   return index;
		 }
	  }

	  /// <summary>
	  /// Override SAX2DTM.startElement()
	  /// 
	  /// <para>Receive notification of the start of an element.
	  /// 
	  /// </para>
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each element (such as allocating a new tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.startElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, Attributes attributes) throws SAXException
	  public override void startElement(string uri, string localName, string qName, Attributes attributes)
	  {

		charactersFlush();

		int exName = m_expandedNameTable.getExpandedTypeID(uri, localName, DTM.ELEMENT_NODE);

		int prefixIndex = (qName.Length != localName.Length) ? m_valuesOrPrefixes.stringToIndex(qName) : 0;

		int elemNode = addNode(DTM.ELEMENT_NODE, exName, m_parents.peek(), m_previous, prefixIndex, true);

		if (m_indexing)
		{
		  indexNode(exName, elemNode);
		}

		m_parents.push(elemNode);

		int startDecls = m_contextIndexes.peek();
		int nDecls = m_prefixMappings.Count;
		string prefix;

		if (!m_pastFirstElement)
		{
		  // SPECIAL CASE: Implied declaration at root element
		  prefix = "xml";
		  string declURL = "http://www.w3.org/XML/1998/namespace";
		  exName = m_expandedNameTable.getExpandedTypeID(null, prefix, DTM.NAMESPACE_NODE);
		  m_values.Add(declURL);
		  int val = m_valueIndex++;
		  addNode(DTM.NAMESPACE_NODE, exName, elemNode, DTM.NULL, val, false);
		  m_pastFirstElement = true;
		}

		for (int i = startDecls; i < nDecls; i += 2)
		{
		  prefix = (string) m_prefixMappings[i];

		  if (string.ReferenceEquals(prefix, null))
		  {
			continue;
		  }

		  string declURL = (string) m_prefixMappings[i + 1];

		  exName = m_expandedNameTable.getExpandedTypeID(null, prefix, DTM.NAMESPACE_NODE);

		  m_values.Add(declURL);
		  int val = m_valueIndex++;

		  addNode(DTM.NAMESPACE_NODE, exName, elemNode, DTM.NULL, val, false);
		}

		int n = attributes.getLength();

		for (int i = 0; i < n; i++)
		{
		  string attrUri = attributes.getURI(i);
		  string attrQName = attributes.getQName(i);
		  string valString = attributes.getValue(i);

		  int nodeType;

		  string attrLocalName = attributes.getLocalName(i);

		  if ((null != attrQName) && (attrQName.Equals("xmlns") || attrQName.StartsWith("xmlns:", StringComparison.Ordinal)))
		  {
			prefix = getPrefix(attrQName, attrUri);
			if (declAlreadyDeclared(prefix))
			{
			  continue; // go to the next attribute.
			}

			nodeType = DTM.NAMESPACE_NODE;
		  }
		  else
		  {
			nodeType = DTM.ATTRIBUTE_NODE;

			if (m_buildIdIndex && attributes.getType(i).equalsIgnoreCase("ID"))
			{
			  setIDAttribute(valString, elemNode);
			}
		  }

		  // Bit of a hack... if somehow valString is null, stringToIndex will
		  // return -1, which will make things very unhappy.
		  if (null == valString)
		  {
			valString = "";
		  }

		  m_values.Add(valString);
		  int val = m_valueIndex++;

		  if (attrLocalName.Length != attrQName.Length)
		  {

			prefixIndex = m_valuesOrPrefixes.stringToIndex(attrQName);

			int dataIndex = m_data.size();

			m_data.addElement(prefixIndex);
			m_data.addElement(val);

			val = -dataIndex;
		  }

		  exName = m_expandedNameTable.getExpandedTypeID(attrUri, attrLocalName, nodeType);
		  addNode(nodeType, exName, elemNode, DTM.NULL, val, false);
		}

		if (null != m_wsfilter)
		{
		  short wsv = m_wsfilter.getShouldStripSpace(makeNodeHandle(elemNode), this);
		  bool shouldStrip = (DTMWSFilter.INHERIT == wsv) ? ShouldStripWhitespace : (DTMWSFilter.STRIP == wsv);

		  pushShouldStripWhitespace(shouldStrip);
		}

		m_previous = DTM.NULL;

		m_contextIndexes.push(m_prefixMappings.Count); // for the children.
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end of
	  /// each element (such as finalising a tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified XML 1.0 name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.endElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws SAXException
	  public override void endElement(string uri, string localName, string qName)
	  {
		charactersFlush();

		// If no one noticed, startPrefixMapping is a drag.
		// Pop the context for the last child (the one pushed by startElement)
		m_contextIndexes.quickPop(1);

		// Do it again for this one (the one pushed by the last endElement).
		int topContextIndex = m_contextIndexes.peek();
		if (topContextIndex != m_prefixMappings.Count)
		{
		  m_prefixMappings.Capacity = topContextIndex;
		}

		m_previous = m_parents.pop();

		popShouldStripWhitespace();
	  }

	  /// <summary>
	  /// Report an XML comment anywhere in the document.
	  /// 
	  /// <para>This callback will be used for comments inside or outside the
	  /// document element, including comments in the external DTD
	  /// subset (if read).</para>
	  /// </summary>
	  /// <param name="ch"> An array holding the characters in the comment. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws SAXException
	  public override void comment(char[] ch, int start, int length)
	  {

		if (m_insideDTD) // ignore comments if we're inside the DTD
		{
		  return;
		}

		charactersFlush();

		// %OPT% Saving the comment string in a Vector has a lower cost than
		// saving it in DTMStringPool.
		m_values.Add(new string(ch, start, length));
		int dataIndex = m_valueIndex++;

		m_previous = addNode(DTM.COMMENT_NODE, DTM.COMMENT_NODE, m_parents.peek(), m_previous, dataIndex, false);
	  }

	  /// <summary>
	  /// Receive notification of the beginning of the document.
	  /// </summary>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.startDocument"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws SAXException
	  public override void startDocument()
	  {

		int doc = addNode(DTM.DOCUMENT_NODE, DTM.DOCUMENT_NODE, DTM.NULL, DTM.NULL, 0, true);

		m_parents.push(doc);
		m_previous = DTM.NULL;

		m_contextIndexes.push(m_prefixMappings.Count); // for the next element.
	  }

	  /// <summary>
	  /// Receive notification of the end of the document.
	  /// </summary>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.endDocument"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws SAXException
	  public override void endDocument()
	  {
		base.endDocument();

		// Add a NULL entry to the end of the node arrays as
		// the end indication.
		m_exptype.addElement(NULL);
		m_parent.addElement(NULL);
		m_nextsib.addElement(NULL);
		m_firstch.addElement(NULL);

		// Set the cached references after the document is built.
		m_extendedTypes = m_expandedNameTable.ExtendedTypes;
		m_exptype_map = m_exptype.Map;
		m_nextsib_map = m_nextsib.Map;
		m_firstch_map = m_firstch.Map;
		m_parent_map = m_parent.Map;
	  }

	  /// <summary>
	  /// Construct the node map from the node.
	  /// </summary>
	  /// <param name="type"> raw type ID, one of DTM.XXX_NODE. </param>
	  /// <param name="expandedTypeID"> The expended type ID. </param>
	  /// <param name="parentIndex"> The current parent index. </param>
	  /// <param name="previousSibling"> The previous sibling index. </param>
	  /// <param name="dataOrPrefix"> index into m_data table, or string handle. </param>
	  /// <param name="canHaveFirstChild"> true if the node can have a first child, false
	  ///                          if it is atomic.
	  /// </param>
	  /// <returns> The index identity of the node that was added. </returns>
	  protected internal sealed override int addNode(int type, int expandedTypeID, int parentIndex, int previousSibling, int dataOrPrefix, bool canHaveFirstChild)
	  {
		// Common to all nodes:
		int nodeIndex = m_size++;

		// Have we overflowed a DTM Identity's addressing range?
		//if(m_dtmIdent.size() == (nodeIndex>>>DTMManager.IDENT_DTM_NODE_BITS))
		if (nodeIndex == m_maxNodeIndex)
		{
		  addNewDTMID(nodeIndex);
		  m_maxNodeIndex += (1 << DTMManager.IDENT_DTM_NODE_BITS);
		}

		m_firstch.addElement(DTM.NULL);
		m_nextsib.addElement(DTM.NULL);
		m_parent.addElement(parentIndex);
		m_exptype.addElement(expandedTypeID);
		m_dataOrQName.addElement(dataOrPrefix);

		if (m_prevsib != null)
		{
		  m_prevsib.addElement(previousSibling);
		}

		if (m_locator != null && m_useSourceLocationProperty)
		{
		  setSourceLocation();
		}

		// Note that nextSibling is not processed until charactersFlush()
		// is called, to handle successive characters() events.

		// Special handling by type: Declare namespaces, attach first child
		switch (type)
		{
		case DTM.NAMESPACE_NODE:
		  declareNamespaceInContext(parentIndex,nodeIndex);
		  break;
		case DTM.ATTRIBUTE_NODE:
		  break;
		default:
		  if (DTM.NULL != previousSibling)
		  {
			m_nextsib.setElementAt(nodeIndex,previousSibling);
		  }
		  else if (DTM.NULL != parentIndex)
		  {
			m_firstch.setElementAt(nodeIndex,parentIndex);
		  }
		  break;
		}

		return nodeIndex;
	  }

	  /// <summary>
	  /// Check whether accumulated text should be stripped; if not,
	  /// append the appropriate flavor of text/cdata node.
	  /// </summary>
	  protected internal sealed override void charactersFlush()
	  {

		if (m_textPendingStart >= 0) // -1 indicates no-text-in-progress
		{
		  int length = m_chars.size() - m_textPendingStart;
		  bool doStrip = false;

		  if (ShouldStripWhitespace)
		  {
			doStrip = m_chars.isWhitespace(m_textPendingStart, length);
		  }

		  if (doStrip)
		  {
			m_chars.Length = m_textPendingStart; // Discard accumulated text
		  }
		  else
		  {
			// Guard against characters/ignorableWhitespace events that
			// contained no characters.  They should not result in a node.
			if (length > 0)
			{
			  // If the offset and length do not exceed the given limits
			  // (offset < 2^21 and length < 2^10), then save both the offset
			  // and length in a bitwise encoded value.
			  if (length <= TEXT_LENGTH_MAX && m_textPendingStart <= TEXT_OFFSET_MAX)
			  {
				m_previous = addNode(m_coalescedTextType, DTM.TEXT_NODE, m_parents.peek(), m_previous, length + (m_textPendingStart << TEXT_LENGTH_BITS), false);

			  }
			  else
			  {
				// Store offset and length in the m_data array if one exceeds 
				// the given limits. Use a negative dataIndex as an indication.
				int dataIndex = m_data.size();
				m_previous = addNode(m_coalescedTextType, DTM.TEXT_NODE, m_parents.peek(), m_previous, -dataIndex, false);

				m_data.addElement(m_textPendingStart);
				m_data.addElement(length);
			  }
			}
		  }

		  // Reset for next text block
		  m_textPendingStart = -1;
		  m_textType = m_coalescedTextType = DTM.TEXT_NODE;
		}
	  }

	  /// <summary>
	  /// Override the processingInstruction() interface in SAX2DTM2.
	  /// <para>
	  /// %OPT% This one is different from SAX2DTM.processingInstruction()
	  /// in that we do not use extended types for PI nodes. The name of
	  /// the PI is saved in the DTMStringPool.
	  /// 
	  /// Receive notification of a processing instruction.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref="org.xml.sax.ContentHandler.processingInstruction"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws SAXException
	  public override void processingInstruction(string target, string data)
	  {

		charactersFlush();

		int dataIndex = m_data.size();
		m_previous = addNode(DTM.PROCESSING_INSTRUCTION_NODE, DTM.PROCESSING_INSTRUCTION_NODE, m_parents.peek(), m_previous, -dataIndex, false);

		m_data.addElement(m_valuesOrPrefixes.stringToIndex(target));
		m_values.Add(data);
		m_data.addElement(m_valueIndex++);

	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase.getFirstAttribute().
	  /// <para>
	  /// Given a node handle, get the index of the node's first attribute.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> Handle of first attribute, or DTM.NULL to indicate none exists. </returns>
	  public sealed override int getFirstAttribute(int nodeHandle)
	  {
		int nodeID = makeNodeIdentity(nodeHandle);

		if (nodeID == DTM.NULL)
		{
		  return DTM.NULL;
		}

		int type = _type2(nodeID);

		if (DTM.ELEMENT_NODE == type)
		{
		  // Assume that attributes and namespaces immediately follow the element.
		  while (true)
		  {
			nodeID++;
		// Assume this can not be null.
		type = _type2(nodeID);

		if (type == DTM.ATTRIBUTE_NODE)
		{
		  return makeNodeHandle(nodeID);
		}
		else if (DTM.NAMESPACE_NODE != type)
		{
		  break;
		}
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase.getFirstAttributeIdentity(int).
	  /// <para>
	  /// Given a node identity, get the index of the node's first attribute.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="identity"> int identity of the node. </param>
	  /// <returns> Identity of first attribute, or DTM.NULL to indicate none exists. </returns>
	  protected internal override int getFirstAttributeIdentity(int identity)
	  {
		if (identity == NULL)
		{
			return NULL;
		}
		int type = _type2(identity);

		if (DTM.ELEMENT_NODE == type)
		{
		  // Assume that attributes and namespaces immediately follow the element.
		  while (true)
		  {
			identity++;

			// Assume this can not be null.
			type = _type2(identity);

			if (type == DTM.ATTRIBUTE_NODE)
			{
			  return identity;
			}
			else if (DTM.NAMESPACE_NODE != type)
			{
			  break;
			}
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase.getNextAttributeIdentity(int).
	  /// <para>
	  /// Given a node identity for an attribute, advance to the next attribute.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="identity"> int identity of the attribute node.  This
	  /// <strong>must</strong> be an attribute node.
	  /// </param>
	  /// <returns> int DTM node-identity of the resolved attr,
	  /// or DTM.NULL to indicate none exists.
	  ///  </returns>
	  protected internal override int getNextAttributeIdentity(int identity)
	  {
		// Assume that attributes and namespace nodes immediately follow the element
		while (true)
		{
		  identity++;
		  int type = _type2(identity);

		  if (type == DTM.ATTRIBUTE_NODE)
		  {
			return identity;
		  }
		  else if (type != DTM.NAMESPACE_NODE)
		  {
			break;
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// The optimized version of DTMDefaultBase.getTypedAttribute(int, int).
	  /// <para>
	  /// Given a node handle and an expanded type ID, get the index of the node's
	  /// attribute of that type, if any.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <param name="attType"> int expanded type ID of the required attribute. </param>
	  /// <returns> Handle of attribute of the required type, or DTM.NULL to indicate
	  /// none exists. </returns>
	  protected internal sealed override int getTypedAttribute(int nodeHandle, int attType)
	  {

		int nodeID = makeNodeIdentity(nodeHandle);

		if (nodeID == DTM.NULL)
		{
		  return DTM.NULL;
		}

		int type = _type2(nodeID);

		if (DTM.ELEMENT_NODE == type)
		{
		  int expType;
		  while (true)
		  {
		nodeID++;
		expType = _exptype2(nodeID);

		if (expType != DTM.NULL)
		{
		  type = m_extendedTypes[expType].NodeType;
		}
		else
		{
		  return DTM.NULL;
		}

		if (type == DTM.ATTRIBUTE_NODE)
		{
		  if (expType == attType)
		  {
			  return makeNodeHandle(nodeID);
		  }
		}
		else if (DTM.NAMESPACE_NODE != type)
		{
		  break;
		}
		  }
		}

		return DTM.NULL;
	  }

	  /// <summary>
	  /// Override SAX2DTM.getLocalName() in SAX2DTM2.
	  /// <para>Processing for PIs is different.
	  /// 
	  /// Given a node handle, return its XPath- style localname. (As defined in
	  /// Namespaces, this is the portion of the name after any colon character).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Local name of this node. </returns>
	  public override string getLocalName(int nodeHandle)
	  {
		int expType = _exptype(makeNodeIdentity(nodeHandle));

		if (expType == DTM.PROCESSING_INSTRUCTION_NODE)
		{
		  int dataIndex = _dataOrQName(makeNodeIdentity(nodeHandle));
		  dataIndex = m_data.elementAt(-dataIndex);
		  return m_valuesOrPrefixes.indexToString(dataIndex);
		}
		else
		{
		  return m_expandedNameTable.getLocalName(expType);
		}
	  }

	  /// <summary>
	  /// The optimized version of SAX2DTM.getNodeNameX().
	  /// <para>
	  /// Given a node handle, return the XPath node name. This should be the name
	  /// as described by the XPath data model, NOT the DOM- style name.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string. </returns>
	  public sealed override string getNodeNameX(int nodeHandle)
	  {

		int nodeID = makeNodeIdentity(nodeHandle);
		int eType = _exptype2(nodeID);

		if (eType == DTM.PROCESSING_INSTRUCTION_NODE)
		{
		  int dataIndex = _dataOrQName(nodeID);
		  dataIndex = m_data.elementAt(-dataIndex);
		  return m_valuesOrPrefixes.indexToString(dataIndex);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExtendedType extType = m_extendedTypes[eType];
		ExtendedType extType = m_extendedTypes[eType];

		if (extType.Namespace.Length == 0)
		{
		  return extType.LocalName;
		}
		else
		{
		  int qnameIndex = m_dataOrQName.elementAt(nodeID);

		  if (qnameIndex == 0)
		  {
			return extType.LocalName;
		  }

		  if (qnameIndex < 0)
		  {
		qnameIndex = -qnameIndex;
		qnameIndex = m_data.elementAt(qnameIndex);
		  }

		  return m_valuesOrPrefixes.indexToString(qnameIndex);
		}
	  }

	  /// <summary>
	  /// The optimized version of SAX2DTM.getNodeName().
	  /// <para>
	  /// Given a node handle, return its DOM-style node name. This will include
	  /// names such as #text or #document.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string.
	  /// %REVIEW% Document when empty string is possible...
	  /// %REVIEW-COMMENT% It should never be empty, should it? </returns>
	  public override string getNodeName(int nodeHandle)
	  {

		int nodeID = makeNodeIdentity(nodeHandle);
		int eType = _exptype2(nodeID);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExtendedType extType = m_extendedTypes[eType];
		ExtendedType extType = m_extendedTypes[eType];
		if (extType.Namespace.Length == 0)
		{
		  int type = extType.NodeType;

		  string localName = extType.LocalName;
		  if (type == DTM.NAMESPACE_NODE)
		  {
		if (localName.Length == 0)
		{
		  return "xmlns";
		}
		else
		{
		  return "xmlns:" + localName;
		}
		  }
		  else if (type == DTM.PROCESSING_INSTRUCTION_NODE)
		  {
		int dataIndex = _dataOrQName(nodeID);
		dataIndex = m_data.elementAt(-dataIndex);
		return m_valuesOrPrefixes.indexToString(dataIndex);
		  }
		  else if (localName.Length == 0)
		  {
			return getFixedNames(type);
		  }
		  else
		  {
		return localName;
		  }
		}
		else
		{
		  int qnameIndex = m_dataOrQName.elementAt(nodeID);

		  if (qnameIndex == 0)
		  {
			return extType.LocalName;
		  }

		  if (qnameIndex < 0)
		  {
		qnameIndex = -qnameIndex;
		qnameIndex = m_data.elementAt(qnameIndex);
		  }

		  return m_valuesOrPrefixes.indexToString(qnameIndex);
		}
	  }

	  /// <summary>
	  /// Override SAX2DTM.getStringValue(int)
	  /// <para>
	  /// This method is only used by Xalan-J Interpretive. It is not used by XSLTC.
	  /// </para>
	  /// <para>
	  /// If the caller supplies an XMLStringFactory, the getStringValue() interface
	  /// in SAX2DTM will be called. Otherwise just calls getStringValueX() and
	  /// wraps the returned String in an XMLString.
	  /// 
	  /// Get the string-value of a node as a String object
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  public override XMLString getStringValue(int nodeHandle)
	  {
		int identity = makeNodeIdentity(nodeHandle);
		if (identity == DTM.NULL)
		{
		  return EMPTY_XML_STR;
		}

		int type = _type2(identity);

		if (type == DTM.ELEMENT_NODE || type == DTM.DOCUMENT_NODE)
		{
		  int startNode = identity;
		  identity = _firstch2(identity);
		  if (DTM.NULL != identity)
		  {
		int offset = -1;
		int length = 0;

		do
		{
		  type = _exptype2(identity);

		  if (type == DTM.TEXT_NODE || type == DTM.CDATA_SECTION_NODE)
		  {
			int dataIndex = m_dataOrQName.elementAt(identity);
			if (dataIndex >= 0)
			{
			  if (-1 == offset)
			  {
					offset = (int)((uint)dataIndex >> TEXT_LENGTH_BITS);
			  }

			  length += dataIndex & TEXT_LENGTH_MAX;
			}
			else
			{
			  if (-1 == offset)
			  {
					offset = m_data.elementAt(-dataIndex);
			  }

			  length += m_data.elementAt(-dataIndex + 1);
			}
		  }

		  identity++;
		} while (_parent2(identity) >= startNode);

		if (length > 0)
		{
		  if (m_xstrf != null)
		  {
			return m_xstrf.newstr(m_chars, offset, length);
		  }
		  else
		  {
			return new XMLStringDefault(m_chars.getString(offset, length));
		  }
		}
		else
		{
		  return EMPTY_XML_STR;
		}
		  }
		  else
		  {
			return EMPTY_XML_STR;
		  }
		}
		else if (DTM.TEXT_NODE == type || DTM.CDATA_SECTION_NODE == type)
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);
		  if (dataIndex >= 0)
		  {
			  if (m_xstrf != null)
			  {
				return m_xstrf.newstr(m_chars, (int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
			  }
			  else
			  {
				return new XMLStringDefault(m_chars.getString((int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX));
			  }
		  }
		  else
		  {
			if (m_xstrf != null)
			{
			  return m_xstrf.newstr(m_chars, m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
			}
			else
			{
			  return new XMLStringDefault(m_chars.getString(m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1)));
			}
		  }
		}
		else
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);

		  if (dataIndex < 0)
		  {
			dataIndex = -dataIndex;
			dataIndex = m_data.elementAt(dataIndex + 1);
		  }

		  if (m_xstrf != null)
		  {
			return m_xstrf.newstr((string)m_values[dataIndex]);
		  }
		  else
		  {
			return new XMLStringDefault((string)m_values[dataIndex]);
		  }
		}
	  }

	  /// <summary>
	  /// The optimized version of SAX2DTM.getStringValue(int).
	  /// <para>
	  /// %OPT% This is one of the most often used interfaces. Performance is
	  /// critical here. This one is different from SAX2DTM.getStringValue(int) in
	  /// that it returns a String instead of a XMLString.
	  /// 
	  /// Get the string- value of a node as a String object (see http: //www. w3.
	  /// org/TR/xpath#data- model for the definition of a node's string- value).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  public string getStringValueX(in int nodeHandle)
	  {
		int identity = makeNodeIdentity(nodeHandle);
		if (identity == DTM.NULL)
		{
		  return EMPTY_STR;
		}

		int type = _type2(identity);

		if (type == DTM.ELEMENT_NODE || type == DTM.DOCUMENT_NODE)
		{
		  int startNode = identity;
		  identity = _firstch2(identity);
		  if (DTM.NULL != identity)
		  {
		int offset = -1;
		int length = 0;

		do
		{
		  type = _exptype2(identity);

		  if (type == DTM.TEXT_NODE || type == DTM.CDATA_SECTION_NODE)
		  {
			int dataIndex = m_dataOrQName.elementAt(identity);
			if (dataIndex >= 0)
			{
			  if (-1 == offset)
			  {
					offset = (int)((uint)dataIndex >> TEXT_LENGTH_BITS);
			  }

			  length += dataIndex & TEXT_LENGTH_MAX;
			}
			else
			{
			  if (-1 == offset)
			  {
					offset = m_data.elementAt(-dataIndex);
			  }

			  length += m_data.elementAt(-dataIndex + 1);
			}
		  }

		  identity++;
		} while (_parent2(identity) >= startNode);

		if (length > 0)
		{
		  return m_chars.getString(offset, length);
		}
		else
		{
		  return EMPTY_STR;
		}
		  }
		  else
		  {
			return EMPTY_STR;
		  }
		}
		else if (DTM.TEXT_NODE == type || DTM.CDATA_SECTION_NODE == type)
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);
		  if (dataIndex >= 0)
		  {
			  return m_chars.getString((int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
		  }
		  else
		  {
			return m_chars.getString(m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
		  }
		}
		else
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);

		  if (dataIndex < 0)
		  {
			dataIndex = -dataIndex;
			dataIndex = m_data.elementAt(dataIndex + 1);
		  }

		  return (string)m_values[dataIndex];
		}
	  }

	  /// <summary>
	  /// Returns the string value of the entire tree
	  /// </summary>
	  public virtual string StringValue
	  {
		  get
		  {
			int child = _firstch2(ROOTNODE);
			if (child == DTM.NULL)
			{
				return EMPTY_STR;
			}
    
			// optimization: only create StringBuffer if > 1 child
			if ((_exptype2(child) == DTM.TEXT_NODE) && (_nextsib2(child) == DTM.NULL))
			{
			  int dataIndex = m_dataOrQName.elementAt(child);
			  if (dataIndex >= 0)
			  {
				return m_chars.getString((int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
			  }
			  else
			  {
				return m_chars.getString(m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
			  }
			}
			else
			{
			  return getStringValueX(Document);
			}
    
		  }
	  }

	  /// <summary>
	  /// The optimized version of SAX2DTM.dispatchCharactersEvents(int, ContentHandler, boolean).
	  /// <para>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value of the given node (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value). Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler. </param>
	  /// <param name="normalize"> true if the content should be normalized according to
	  /// the rules for the XPath
	  /// <a href="http://www.w3.org/TR/xpath#function-normalize-space">normalize-space</a>
	  /// function.
	  /// </param>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public final void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, boolean normalize) throws SAXException
	  public sealed override void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, bool normalize)
	  {

		int identity = makeNodeIdentity(nodeHandle);

		if (identity == DTM.NULL)
		{
		  return;
		}

		int type = _type2(identity);

		if (type == DTM.ELEMENT_NODE || type == DTM.DOCUMENT_NODE)
		{
		  int startNode = identity;
		  identity = _firstch2(identity);
		  if (DTM.NULL != identity)
		  {
		int offset = -1;
		int length = 0;

		do
		{
		  type = _exptype2(identity);

		  if (type == DTM.TEXT_NODE || type == DTM.CDATA_SECTION_NODE)
		  {
			int dataIndex = m_dataOrQName.elementAt(identity);

			if (dataIndex >= 0)
			{
			  if (-1 == offset)
			  {
					offset = (int)((uint)dataIndex >> TEXT_LENGTH_BITS);
			  }

			  length += dataIndex & TEXT_LENGTH_MAX;
			}
			else
			{
			  if (-1 == offset)
			  {
					offset = m_data.elementAt(-dataIndex);
			  }

			  length += m_data.elementAt(-dataIndex + 1);
			}
		  }

		  identity++;
		} while (_parent2(identity) >= startNode);

		if (length > 0)
		{
			  if (normalize)
			  {
				m_chars.sendNormalizedSAXcharacters(ch, offset, length);
			  }
			  else
			  {
			m_chars.sendSAXcharacters(ch, offset, length);
			  }
		}
		  }
		}
		else if (DTM.TEXT_NODE == type || DTM.CDATA_SECTION_NODE == type)
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);

		  if (dataIndex >= 0)
		  {
			  if (normalize)
			  {
				m_chars.sendNormalizedSAXcharacters(ch, (int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
			  }
			  else
			  {
				m_chars.sendSAXcharacters(ch, (int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
			  }
		  }
		  else
		  {
			if (normalize)
			{
			  m_chars.sendNormalizedSAXcharacters(ch, m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
			}
			else
			{
			  m_chars.sendSAXcharacters(ch, m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
			}
		  }
		}
		else
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);

		  if (dataIndex < 0)
		  {
			dataIndex = -dataIndex;
			dataIndex = m_data.elementAt(dataIndex + 1);
		  }

		  string str = (string)m_values[dataIndex];

		  if (normalize)
		  {
			FastStringBuffer.sendNormalizedSAXcharacters(str.ToCharArray(), 0, str.Length, ch);
		  }
		  else
		  {
			ch.characters(str.ToCharArray(), 0, str.Length);
		  }
		}
	  }

	  /// <summary>
	  /// Given a node handle, return its node value. This is mostly
	  /// as defined by the DOM, but may ignore some conveniences.
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> String Value of this node, or null if not
	  /// meaningful for this node type. </returns>
	  public override string getNodeValue(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);
		int type = _type2(identity);

		if (type == DTM.TEXT_NODE || type == DTM.CDATA_SECTION_NODE)
		{
		  int dataIndex = _dataOrQName(identity);
		  if (dataIndex > 0)
		  {
			  return m_chars.getString((int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
		  }
		  else
		  {
			return m_chars.getString(m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
		  }
		}
		else if (DTM.ELEMENT_NODE == type || DTM.DOCUMENT_FRAGMENT_NODE == type || DTM.DOCUMENT_NODE == type)
		{
		  return null;
		}
		else
		{
		  int dataIndex = m_dataOrQName.elementAt(identity);

		  if (dataIndex < 0)
		  {
			dataIndex = -dataIndex;
			dataIndex = m_data.elementAt(dataIndex + 1);
		  }

		  return (string)m_values[dataIndex];
		}
	  }

		/// <summary>
		/// Copy the String value of a Text node to a SerializationHandler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void copyTextNode(final int nodeID, org.apache.xml.serializer.SerializationHandler handler) throws SAXException
		protected internal void copyTextNode(in int nodeID, SerializationHandler handler)
		{
			if (nodeID != DTM.NULL)
			{
				  int dataIndex = m_dataOrQName.elementAt(nodeID);
				if (dataIndex >= 0)
				{
					m_chars.sendSAXcharacters(handler, (int)((uint)dataIndex >> TEXT_LENGTH_BITS), dataIndex & TEXT_LENGTH_MAX);
				}
				else
				{
					m_chars.sendSAXcharacters(handler, m_data.elementAt(-dataIndex), m_data.elementAt(-dataIndex + 1));
				}
			}
		}

		/// <summary>
		/// Copy an Element node to a SerializationHandler.
		/// </summary>
		/// <param name="nodeID"> The node identity </param>
		/// <param name="exptype"> The expanded type of the Element node </param>
		/// <param name="handler"> The SerializationHandler </param>
		/// <returns> The qualified name of the Element node. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final String copyElement(int nodeID, int exptype, org.apache.xml.serializer.SerializationHandler handler) throws SAXException
		protected internal string copyElement(int nodeID, int exptype, SerializationHandler handler)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExtendedType extType = m_extendedTypes[exptype];
			ExtendedType extType = m_extendedTypes[exptype];
			string uri = extType.Namespace;
			string name = extType.LocalName;

			if (uri.Length == 0)
			{
				handler.startElement(name);
				return name;
			}
			else
			{
				int qnameIndex = m_dataOrQName.elementAt(nodeID);

				if (qnameIndex == 0)
				{
					handler.startElement(name);
					handler.namespaceAfterStartElement(EMPTY_STR, uri);
					return name;
				}

				if (qnameIndex < 0)
				{
					qnameIndex = -qnameIndex;
					qnameIndex = m_data.elementAt(qnameIndex);
				}

				string qName = m_valuesOrPrefixes.indexToString(qnameIndex);
				handler.startElement(qName);
				int prefixIndex = qName.IndexOf(':');
				string prefix;
				if (prefixIndex > 0)
				{
					prefix = qName.Substring(0, prefixIndex);
				}
				else
				{
					prefix = null;
				}
				handler.namespaceAfterStartElement(prefix, uri);
				return qName;
			}

		}

		/// <summary>
		/// Copy  namespace nodes.
		/// </summary>
		/// <param name="nodeID"> The Element node identity </param>
		/// <param name="handler"> The SerializationHandler </param>
		/// <param name="inScope">  true if all namespaces in scope should be copied,
		///  false if only the namespace declarations should be copied. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void copyNS(final int nodeID, org.apache.xml.serializer.SerializationHandler handler, boolean inScope) throws SAXException
		protected internal void copyNS(in int nodeID, SerializationHandler handler, bool inScope)
		{
			// %OPT% Optimization for documents which does not have any explicit
			// namespace nodes. For these documents, there is an implicit
			// namespace node (xmlns:xml="http://www.w3.org/XML/1998/namespace")
			// declared on the root element node. In this case, there is no
			// need to do namespace copying. We can safely return without
			// doing anything.
			if (m_namespaceDeclSetElements != null && m_namespaceDeclSetElements.size() == 1 && m_namespaceDeclSets != null && ((SuballocatedIntVector)m_namespaceDeclSets[0]).size() == 1)
			{
				return;
			}

			SuballocatedIntVector nsContext = null;
			int nextNSNode;

			// Find the first namespace node
			if (inScope)
			{
				nsContext = findNamespaceContext(nodeID);
				if (nsContext == null || nsContext.size() < 1)
				{
					return;
				}
				else
				{
					nextNSNode = makeNodeIdentity(nsContext.elementAt(0));
				}
			}
			else
			{
				nextNSNode = getNextNamespaceNode2(nodeID);
			}

			int nsIndex = 1;
			while (nextNSNode != DTM.NULL)
			{
				// Retrieve the name of the namespace node
				int eType = _exptype2(nextNSNode);
				string nodeName = m_extendedTypes[eType].LocalName;

				// Retrieve the node value of the namespace node
				int dataIndex = m_dataOrQName.elementAt(nextNSNode);

				if (dataIndex < 0)
				{
					dataIndex = -dataIndex;
					dataIndex = m_data.elementAt(dataIndex + 1);
				}

				string nodeValue = (string)m_values[dataIndex];

				handler.namespaceAfterStartElement(nodeName, nodeValue);

				if (inScope)
				{
					if (nsIndex < nsContext.size())
					{
						nextNSNode = makeNodeIdentity(nsContext.elementAt(nsIndex));
						nsIndex++;
					}
					else
					{
						return;
					}
				}
				else
				{
					nextNSNode = getNextNamespaceNode2(nextNSNode);
				}
			}
		}

		/// <summary>
		/// Return the next namespace node following the given base node.
		/// 
		/// @baseID The node identity of the base node, which can be an
		/// element, attribute or namespace node. </summary>
		/// <returns> The namespace node immediately following the base node. </returns>
		protected internal int getNextNamespaceNode2(int baseID)
		{
			int type;
			while ((type = _type2(++baseID)) == DTM.ATTRIBUTE_NODE)
			{
					;
			}

			if (type == DTM.NAMESPACE_NODE)
			{
				return baseID;
			}
			else
			{
				return NULL;
			}
		}

		/// <summary>
		/// Copy  attribute nodes from an element .
		/// </summary>
		/// <param name="nodeID"> The Element node identity </param>
		/// <param name="handler"> The SerializationHandler </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void copyAttributes(final int nodeID, org.apache.xml.serializer.SerializationHandler handler) throws SAXException
		protected internal void copyAttributes(in int nodeID, SerializationHandler handler)
		{

		   for (int current = getFirstAttributeIdentity(nodeID); current != DTM.NULL; current = getNextAttributeIdentity(current))
		   {
				int eType = _exptype2(current);
				copyAttribute(current, eType, handler);
		   }
		}



		/// <summary>
		/// Copy an Attribute node to a SerializationHandler
		/// </summary>
		/// <param name="nodeID"> The node identity </param>
		/// <param name="exptype"> The expanded type of the Element node </param>
		/// <param name="handler"> The SerializationHandler </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void copyAttribute(int nodeID, int exptype, org.apache.xml.serializer.SerializationHandler handler) throws SAXException
		protected internal void copyAttribute(int nodeID, int exptype, SerializationHandler handler)
		{
			/*
			    final String uri = getNamespaceName(node);
			    if (uri.length() != 0) {
			        final String prefix = getPrefix(node);
			        handler.namespaceAfterStartElement(prefix, uri);
			    }
			    handler.addAttribute(getNodeName(node), getNodeValue(node));
			*/
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExtendedType extType = m_extendedTypes[exptype];
			ExtendedType extType = m_extendedTypes[exptype];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = extType.getNamespace();
			string uri = extType.Namespace;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = extType.getLocalName();
			string localName = extType.LocalName;

			string prefix = null;
			string qname = null;
			int dataIndex = _dataOrQName(nodeID);
			int valueIndex = dataIndex;
				if (dataIndex <= 0)
				{
					int prefixIndex = m_data.elementAt(-dataIndex);
					valueIndex = m_data.elementAt(-dataIndex + 1);
					qname = m_valuesOrPrefixes.indexToString(prefixIndex);
					int colonIndex = qname.IndexOf(':');
					if (colonIndex > 0)
					{
						prefix = qname.Substring(0, colonIndex);
					}
				}
				if (uri.Length != 0)
				{
					handler.namespaceAfterStartElement(prefix, uri);
				}

			string nodeName = (!string.ReferenceEquals(prefix, null)) ? qname : localName;
			string nodeValue = (string)m_values[valueIndex];

			handler.addAttribute(nodeName, nodeValue);
		}

	}

}