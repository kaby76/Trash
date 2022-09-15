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
 * $Id: NodeIteratorBase.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public abstract class NodeIteratorBase : NodeIterator
	{
		public abstract void gotoMark();
		public abstract void setMark();
		public abstract int next();

		/// <summary>
		/// Cached computed value of last().
		/// </summary>
		protected internal int _last = -1;

		/// <summary>
		/// Value of position() in this iterator. Incremented in
		/// returnNode().
		/// </summary>
		protected internal int _position = 0;

		/// <summary>
		/// Store node in call to setMark().
		/// </summary>
		protected internal int _markedNode;

		/// <summary>
		/// Store node in call to setStartNode().
		/// </summary>
		protected internal int _startNode = org.apache.xalan.xsltc.NodeIterator_Fields.END;

		/// <summary>
		/// Flag indicating if "self" should be returned.
		/// </summary>
		protected internal bool _includeSelf = false;

		/// <summary>
		/// Flag indicating if iterator can be restarted.
		/// </summary>
		protected internal bool _isRestartable = true;

		/// <summary>
		/// Setter for _isRestartable flag. 
		/// </summary>
		public virtual bool Restartable
		{
			set
			{
			_isRestartable = value;
			}
		}

		/// <summary>
		/// Initialize iterator using a node. If iterator is not
		/// restartable, then do nothing. If node is equal to END then
		/// subsequent calls to next() must return END.
		/// </summary>
		public abstract NodeIterator setStartNode(int node);

		/// <summary>
		/// Reset this iterator using state from last call to
		/// setStartNode().
		/// </summary>
		public virtual NodeIterator reset()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _isRestartable;
		bool temp = _isRestartable;
		_isRestartable = true;
		// Must adjust _startNode if self is included
		StartNode = _includeSelf ? _startNode + 1 : _startNode;
		_isRestartable = temp;
		return this;
		}

		/// <summary>
		/// Setter for _includeSelf flag.
		/// </summary>
		public virtual NodeIterator includeSelf()
		{
		_includeSelf = true;
		return this;
		}

		/// <summary>
		/// Default implementation of getLast(). Stores current position
		/// and current node, resets the iterator, counts all nodes and
		/// restores iterator to original state.
		/// </summary>
		public virtual int Last
		{
			get
			{
			if (_last == -1)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int temp = _position;
				int temp = _position;
				setMark();
				reset();
				do
				{
				_last++;
				} while (next() != org.apache.xalan.xsltc.NodeIterator_Fields.END);
				gotoMark();
				_position = temp;
			}
			return _last;
			}
		}

		/// <summary>
		/// Returns the position() in this iterator.
		/// </summary>
		public virtual int Position
		{
			get
			{
			return _position == 0 ? 1 : _position;
			}
		}

		/// <summary>
		/// Indicates if position in this iterator is computed in reverse
		/// document order. Note that nodes are always returned in document
		/// order.
		/// </summary>
		public virtual bool Reverse
		{
			get
			{
			return false;
			}
		}

		/// <summary>
		/// Clones and resets this iterator. Note that the cloned iterator is 
		/// not restartable. This is because cloning is needed for variable 
		/// references, and the context node of the original variable 
		/// declaration must be preserved.
		/// </summary>
		public virtual NodeIterator cloneIterator()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final NodeIteratorBase clone = (NodeIteratorBase)super.clone();
			NodeIteratorBase clone = (NodeIteratorBase)base.clone();
			clone._isRestartable = false;
			return clone.reset();
		}
		catch (CloneNotSupportedException e)
		{
			BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
			return null;
		}
		}

		/// <summary>
		/// Utility method that increments position and returns its
		/// argument.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected final int returnNode(final int node)
		protected internal int returnNode(int node)
		{
		_position++;
		return node;
		}

		/// <summary>
		/// Reset the position in this iterator.
		/// </summary>
		protected internal NodeIterator resetPosition()
		{
		_position = 0;
		return this;
		}
	}

}