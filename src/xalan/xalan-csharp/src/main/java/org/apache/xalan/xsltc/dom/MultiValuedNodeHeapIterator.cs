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
 * $Id: MultiValuedNodeHeapIterator.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// <para><code>MultiValuedNodeHeapIterator</code> takes a set of multi-valued
	/// heap nodes and produces a merged NodeSet in document order with duplicates
	/// removed.</para>
	/// <para>Each multi-valued heap node (which might be a
	/// <seealso cref="org.apache.xml.dtm.DTMAxisIterator"/>, but that's  not necessary)
	/// generates DTM node handles in document order.  The class
	/// maintains the multi-valued heap nodes in a heap, not surprisingly, sorted by
	/// the next DTM node handle available form the heap node.</para>
	/// <para>After a DTM node is pulled from the heap node that's at the top of the
	/// heap, the heap node is advanced to the next DTM node handle it makes
	/// available, and the heap nature of the heap is restored to ensure the next
	/// DTM node handle pulled is next in document order overall.
	/// 
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </para>
	/// </summary>
	public abstract class MultiValuedNodeHeapIterator : DTMAxisIteratorBase
	{
		/// <summary>
		/// wrapper for NodeIterators to support iterator
		/// comparison on the value of their next() method
		/// </summary>

		/// <summary>
		/// An abstract representation of a set of nodes that will be retrieved in
		/// document order.
		/// </summary>
		public abstract class HeapNode : ICloneable
		{
			private readonly MultiValuedNodeHeapIterator outerInstance;

			public HeapNode(MultiValuedNodeHeapIterator outerInstance)
			{
				this.outerInstance = outerInstance;
			}

		protected internal int _node, _markedNode;
		protected internal bool _isStartSet = false;

			/// <summary>
			/// Advance to the next node represented by this <seealso cref="HeapNode"/>
			/// </summary>
			/// <returns> the next DTM node. </returns>
		public abstract int step();


			/// <summary>
			/// Creates a deep copy of this <seealso cref="HeapNode"/>.  The clone is not
			/// reset from the current position of the original.
			/// </summary>
			/// <returns> the cloned heap node </returns>
		public virtual HeapNode cloneHeapNode()
		{
				HeapNode clone;

				try
				{
					clone = (HeapNode) base.clone();
				}
				catch (CloneNotSupportedException e)
				{
					BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
					return null;
				}

			clone._node = _node;
			clone._markedNode = _node;

			return clone;
		}

			/// <summary>
			/// Remembers the current node for the next call to <seealso cref="#gotoMark()"/>.
			/// </summary>
		public virtual void setMark()
		{
			_markedNode = _node;
		}

			/// <summary>
			/// Restores the current node remembered by <seealso cref="#setMark()"/>.
			/// </summary>
		public virtual void gotoMark()
		{
			_node = _markedNode;
		}

			/// <summary>
			/// Performs a comparison of the two heap nodes
			/// </summary>
			/// <param name="heapNode"> the heap node against which to compare </param>
			/// <returns> <code>true</code> if and only if the current node for this
			///         heap node is before the current node of the argument heap
			///         node in document order. </returns>
			public abstract bool isLessThan(HeapNode heapNode);

			/// <summary>
			/// Sets context with respect to which this heap node is evaluated.
			/// </summary>
			/// <param name="node"> The new context node </param>
			/// <returns> a <seealso cref="HeapNode"/> which may or may not be the same as
			///         this <code>HeapNode</code>. </returns>
			public abstract HeapNode setStartNode(int node);

			/// <summary>
			/// Reset the heap node back to its beginning.
			/// </summary>
			/// <returns> a <seealso cref="HeapNode"/> which may or may not be the same as
			///         this <code>HeapNode</code>. </returns>
			public abstract HeapNode reset();
		} // end of HeapNode

		private const int InitSize = 8;

		private int _heapSize = 0;
		private int _size = InitSize;
		private HeapNode[] _heap = new HeapNode[InitSize];
		private int _free = 0;

		// Last node returned by this MultiValuedNodeHeapIterator to the caller of
		// next; used to prune duplicates
		private int _returnedLast;

		// cached returned last for use in gotoMark
		private int _cachedReturnedLast = org.apache.xml.dtm.DTMAxisIterator_Fields.END;

		// cached heap size for use in gotoMark
		private int _cachedHeapSize;


		public override DTMAxisIterator cloneIterator()
		{
		_isRestartable = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HeapNode[] heapCopy = new HeapNode[_heap.length];
		HeapNode[] heapCopy = new HeapNode[_heap.Length];
		try
		{
			MultiValuedNodeHeapIterator clone = (MultiValuedNodeHeapIterator)base.clone();

				for (int i = 0; i < _free; i++)
				{
					heapCopy[i] = _heap[i].cloneHeapNode();
				}
			clone.Restartable = false;
			clone._heap = heapCopy;
			return clone.reset();
		}
		catch (CloneNotSupportedException e)
		{
			BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
			return null;
		}
		}

		protected internal virtual void addHeapNode(HeapNode node)
		{
		if (_free == _size)
		{
			HeapNode[] newArray = new HeapNode[_size *= 2];
			Array.Copy(_heap, 0, newArray, 0, _free);
			_heap = newArray;
		}
		_heapSize++;
		_heap[_free++] = node;
		}

		public override int next()
		{
		while (_heapSize > 0)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int smallest = _heap[0]._node;
			int smallest = _heap[0]._node;
			if (smallest == org.apache.xml.dtm.DTMAxisIterator_Fields.END)
			{ // iterator _heap[0] is done
			if (_heapSize > 1)
			{
				// Swap first and last (iterator must be restartable)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HeapNode temp = _heap[0];
				HeapNode temp = _heap[0];
				_heap[0] = _heap[--_heapSize];
				_heap[_heapSize] = temp;
			}
			else
			{
				return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			}
			}
			else if (smallest == _returnedLast)
			{ // duplicate
			_heap[0].step(); // value consumed
			}
			else
			{
			_heap[0].step(); // value consumed
			heapify(0);
			return returnNode(_returnedLast = smallest);
			}
			// fallthrough if not returned above
			heapify(0);
		}
		return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		}

		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			_startNode = node;
			for (int i = 0; i < _free; i++)
			{
				 if (!_heap[i]._isStartSet)
				 {
				   _heap[i].StartNode = node;
				   _heap[i].step(); // to get the first node
				   _heap[i]._isStartSet = true;
				 }
			}
			// build heap
			for (int i = (_heapSize = _free) / 2; i >= 0; i--)
			{
			heapify(i);
			}
			_returnedLast = org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			return resetPosition();
		}
		return this;
		}

		protected internal virtual void init()
		{
			for (int i = 0; i < _free; i++)
			{
				_heap[i] = null;
			}

			_heapSize = 0;
			_free = 0;
		}

		/* Build a heap in document order. put the smallest node on the top. 
		 * "smallest node" means the node before other nodes in document order
		 */
		private void heapify(int i)
		{
		for (int r, l, smallest;;)
		{
			r = (i + 1) << 1;
			l = r - 1;
			smallest = l < _heapSize && _heap[l].isLessThan(_heap[i]) ? l : i;
			if (r < _heapSize && _heap[r].isLessThan(_heap[smallest]))
			{
			smallest = r;
			}
			if (smallest != i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HeapNode temp = _heap[smallest];
			HeapNode temp = _heap[smallest];
			_heap[smallest] = _heap[i];
			_heap[i] = temp;
			i = smallest;
			}
			else
			{
			break;
			}
		}
		}

		public override void setMark()
		{
		for (int i = 0; i < _free; i++)
		{
			_heap[i].setMark();
		}
		_cachedReturnedLast = _returnedLast;
		_cachedHeapSize = _heapSize;
		}

		public override void gotoMark()
		{
		for (int i = 0; i < _free; i++)
		{
			_heap[i].gotoMark();
		}
		// rebuild heap after call last() function. fix for bug 20913
		for (int i = (_heapSize = _cachedHeapSize) / 2; i >= 0; i--)
		{
			heapify(i);
		}
			_returnedLast = _cachedReturnedLast;
		}

		public override DTMAxisIterator reset()
		{
		for (int i = 0; i < _free; i++)
		{
			_heap[i].reset();
			_heap[i].step();
		}

		// build heap
		for (int i = (_heapSize = _free) / 2; i >= 0; i--)
		{
			heapify(i);
		}

		_returnedLast = org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		return resetPosition();
		}

	}

}