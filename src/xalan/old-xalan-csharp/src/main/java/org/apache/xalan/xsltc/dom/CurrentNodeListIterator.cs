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
 * $Id: CurrentNodeListIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using IntegerArray = org.apache.xalan.xsltc.util.IntegerArray;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// Iterators of this kind use a CurrentNodeListFilter to filter a subset of 
	/// nodes from a source iterator. For each node from the source, the boolean 
	/// method CurrentNodeListFilter.test() is called. 
	/// 
	/// All nodes from the source are read into an array upon calling setStartNode() 
	/// (this is needed to determine the value of last, a parameter to 
	/// CurrentNodeListFilter.test()). The method getLast() returns the last element 
	/// after applying the filter.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>

	public sealed class CurrentNodeListIterator : DTMAxisIteratorBase
	{
		/// <summary>
		/// A flag indicating if nodes are returned in document order.
		/// </summary>
		private bool _docOrder;

		/// <summary>
		/// The source for this iterator.
		/// </summary>
		private DTMAxisIterator _source;

		/// <summary>
		/// A reference to a filter object.
		/// </summary>
		private readonly CurrentNodeListFilter _filter;

		/// <summary>
		/// An integer array to store nodes from source iterator.
		/// </summary>
		private IntegerArray _nodes = new IntegerArray();

		/// <summary>
		/// Index in _nodes of the next node to filter.
		/// </summary>
		private int _currentIndex;

		/// <summary>
		/// The current node in the stylesheet at the time of evaluation.
		/// </summary>
		private readonly int _currentNode;

		/// <summary>
		/// A reference to the translet.
		/// </summary>
		private AbstractTranslet _translet;

		public CurrentNodeListIterator(DTMAxisIterator source, CurrentNodeListFilter filter, int currentNode, AbstractTranslet translet) : this(source, !source.Reverse, filter, currentNode, translet)
		{
		}

		public CurrentNodeListIterator(DTMAxisIterator source, bool docOrder, CurrentNodeListFilter filter, int currentNode, AbstractTranslet translet)
		{
		_source = source;
		_filter = filter;
		_translet = translet;
		_docOrder = docOrder;
		_currentNode = currentNode;
		}

		public DTMAxisIterator forceNaturalOrder()
		{
		_docOrder = true;
		return this;
		}

		public override bool Restartable
		{
			set
			{
			_isRestartable = value;
			_source.Restartable = value;
			}
		}

		public override bool Reverse
		{
			get
			{
			return !_docOrder;
			}
		}

		public override DTMAxisIterator cloneIterator()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CurrentNodeListIterator clone = (CurrentNodeListIterator) super.clone();
			CurrentNodeListIterator clone = (CurrentNodeListIterator) base.clone();
			clone._nodes = (IntegerArray) _nodes.clone();
			clone._source = _source.cloneIterator();
			clone._isRestartable = false;
			return clone.reset();
		}
		catch (CloneNotSupportedException e)
		{
			BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
			return null;
		}
		}

		public override DTMAxisIterator reset()
		{
		_currentIndex = 0;
		return resetPosition();
		}

		public override int next()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int last = _nodes.cardinality();
		int last = _nodes.cardinality();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currentNode = _currentNode;
		int currentNode = _currentNode;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.runtime.AbstractTranslet translet = _translet;
		AbstractTranslet translet = _translet;

		for (int index = _currentIndex; index < last;)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int position = _docOrder ? index + 1 : last - index;
			int position = _docOrder ? index + 1 : last - index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int node = _nodes.at(index++);
			int node = _nodes.at(index++); // note increment

			if (_filter.test(node, position, last, currentNode, translet, this))
			{
			_currentIndex = index;
			return returnNode(node);
			}
		}
		return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		}

		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			_source.StartNode = _startNode = node;

			_nodes.clear();
			while ((node = _source.next()) != org.apache.xml.dtm.DTMAxisIterator_Fields.END)
			{
			_nodes.add(node);
			}
			_currentIndex = 0;
			resetPosition();
		}
		return this;
		}

		public override int Last
		{
			get
			{
			if (_last == -1)
			{
				_last = computePositionOfLast();
			}
			return _last;
			}
		}

		public override void setMark()
		{
		_markedNode = _currentIndex;
		}

		public override void gotoMark()
		{
		_currentIndex = _markedNode;
		}

		private int computePositionOfLast()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int last = _nodes.cardinality();
			int last = _nodes.cardinality();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currNode = _currentNode;
			int currNode = _currentNode;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.runtime.AbstractTranslet translet = _translet;
		AbstractTranslet translet = _translet;

		int lastPosition = _position;
		for (int index = _currentIndex; index < last;)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int position = _docOrder ? index + 1 : last - index;
			int position = _docOrder ? index + 1 : last - index;
				int nodeIndex = _nodes.at(index++); // note increment

				if (_filter.test(nodeIndex, position, last, currNode, translet, this))
				{
					lastPosition++;
				}
		}
		return lastPosition;
		}
	}

}