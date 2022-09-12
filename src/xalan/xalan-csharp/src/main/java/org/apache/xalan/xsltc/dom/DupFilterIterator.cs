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
 * $Id: DupFilterIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using IntegerArray = org.apache.xalan.xsltc.util.IntegerArray;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;

	/// <summary>
	/// Removes duplicates and sorts a source iterator. The nodes from the 
	/// source are collected in an array upon calling setStartNode(). This
	/// array is later sorted and duplicates are ignored in next().
	/// @author G. Todd Miller 
	/// </summary>
	public sealed class DupFilterIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// Reference to source iterator.
		/// </summary>
		private DTMAxisIterator _source;

		/// <summary>
		/// Array to cache all nodes from source.
		/// </summary>
		private IntegerArray _nodes = new IntegerArray();

		/// <summary>
		/// Index in _nodes array to current node.
		/// </summary>
		private int _current = 0;

		/// <summary>
		/// Cardinality of _nodes array.
		/// </summary>
		private int _nodesSize = 0;

		/// <summary>
		/// Last value returned by next().
		/// </summary>
		private int _lastNext = org.apache.xml.dtm.DTMAxisIterator_Fields.END;

		/// <summary>
		/// Temporary variable to store _lastNext.
		/// </summary>
		private int _markedLastNext = org.apache.xml.dtm.DTMAxisIterator_Fields.END;

		public DupFilterIterator(DTMAxisIterator source)
		{
		_source = source;
	// System.out.println("DFI source = " + source + " this = " + this);

		// Cache contents of id() or key() index right away. Necessary for
		// union expressions containing multiple calls to the same index, and
		// correct as well since start-node is irrelevant for id()/key() exrp.
		if (source is KeyIndex)
		{
			StartNode = DTMDefaultBase.ROOTNODE;
		}
		}

		/// <summary>
		/// Set the start node for this iterator </summary>
		/// <param name="node"> The start node </param>
		/// <returns> A reference to this node iterator </returns>
		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			// KeyIndex iterators are always relative to the root node, so there
			// is never any point in re-reading the iterator (and we SHOULD NOT).
				bool sourceIsKeyIndex = _source is KeyIndex;

			if (sourceIsKeyIndex && _startNode == DTMDefaultBase.ROOTNODE)
			{
			return this;
			}

			if (node != _startNode)
			{
			_source.StartNode = _startNode = node;

			_nodes.clear();
			while ((node = _source.next()) != org.apache.xml.dtm.DTMAxisIterator_Fields.END)
			{
				_nodes.add(node);
			}

					// Nodes produced by KeyIndex are known to be in document order.
					// Take advantage of it.
					if (!sourceIsKeyIndex)
					{
						_nodes.sort();
					}

			_nodesSize = _nodes.cardinality();
			_current = 0;
			_lastNext = org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			resetPosition();
			}
		}
		return this;
		}

		public override int next()
		{
		while (_current < _nodesSize)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int next = _nodes.at(_current++);
			int next = _nodes.at(_current++);
			if (next != _lastNext)
			{
			return returnNode(_lastNext = next);
			}
		}
		return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		}

		public override DTMAxisIterator cloneIterator()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DupFilterIterator clone = (DupFilterIterator) super.clone();
			DupFilterIterator clone = (DupFilterIterator) base.clone();
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

		public override bool Restartable
		{
			set
			{
			_isRestartable = value;
			_source.Restartable = value;
			}
		}

		public override void setMark()
		{
		_markedNode = _current;
			_markedLastNext = _lastNext; // Bugzilla 25924
		}

		public override void gotoMark()
		{
		_current = _markedNode;
			_lastNext = _markedLastNext; // Bugzilla 25924
		}

		public override DTMAxisIterator reset()
		{
		_current = 0;
		_lastNext = org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		return resetPosition();
		}
	}

}