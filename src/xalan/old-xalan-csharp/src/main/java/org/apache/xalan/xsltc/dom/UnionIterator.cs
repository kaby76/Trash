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
 * $Id: UnionIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// UnionIterator takes a set of NodeIterators and produces
	/// a merged NodeSet in document order with duplicates removed
	/// The individual iterators are supposed to generate nodes
	/// in document order
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class UnionIterator : MultiValuedNodeHeapIterator
	{
		/// <summary>
		/// wrapper for NodeIterators to support iterator
		/// comparison on the value of their next() method
		/// </summary>
		private readonly DOM _dom;

		private sealed class LookAheadIterator : MultiValuedNodeHeapIterator.HeapNode
		{
			private readonly UnionIterator outerInstance;

		public DTMAxisIterator iterator;

		public LookAheadIterator(UnionIterator outerInstance, DTMAxisIterator iterator) : base(outerInstance)
		{
			this.outerInstance = outerInstance;
			this.iterator = iterator;
		}

		public override int step()
		{
			_node = iterator.next();
			return _node;
		}

		public override HeapNode cloneHeapNode()
		{
				LookAheadIterator clone = (LookAheadIterator) base.cloneHeapNode();
				clone.iterator = iterator.cloneIterator();
			return clone;
		}

		public override void setMark()
		{
				base.setMark();
			iterator.setMark();
		}

		public override void gotoMark()
		{
				base.gotoMark();
			iterator.gotoMark();
		}

			public override bool isLessThan(HeapNode heapNode)
			{
				LookAheadIterator comparand = (LookAheadIterator) heapNode;
				return outerInstance._dom.lessThan(_node, heapNode._node);
			}

			public override HeapNode setStartNode(int node)
			{
				iterator.StartNode = node;
				return this;
			}

			public override HeapNode reset()
			{
				iterator.reset();
				return this;
			}
		} // end of LookAheadIterator

		public UnionIterator(DOM dom)
		{
		_dom = dom;
		}

		public UnionIterator addIterator(DTMAxisIterator iterator)
		{
			addHeapNode(new LookAheadIterator(this, iterator));
			return this;
		}
	}

}