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
 * $Id: FilterIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// Similar to a CurrentNodeListIterator except that the filter has a 
	/// simpler interface (only needs the node, no position, last, etc.)  
	/// It takes a source iterator and a Filter object and returns nodes 
	/// from the source after filtering them by calling filter.test(node).
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class FilterIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// Reference to source iterator.
		/// </summary>
		private DTMAxisIterator _source;

		/// <summary>
		/// Reference to a filter object that to be applied to each node.
		/// </summary>
		private readonly DTMFilter _filter;

		/// <summary>
		/// A flag indicating if position is reversed.
		/// </summary>
		private readonly bool _isReverse;

		public FilterIterator(DTMAxisIterator source, DTMFilter filter)
		{
		_source = source;
	// System.out.println("FI souce = " + source + " this = " + this);
		_filter = filter;
		_isReverse = source.Reverse;
		}

		public override bool Reverse
		{
			get
			{
			return _isReverse;
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

		public override DTMAxisIterator cloneIterator()
		{

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FilterIterator clone = (FilterIterator) super.clone();
			FilterIterator clone = (FilterIterator) base.clone();
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
		_source.reset();
		return resetPosition();
		}

		public override int next()
		{
		int node;
		while ((node = _source.next()) != org.apache.xml.dtm.DTMAxisIterator_Fields.END)
		{
			if (_filter.acceptNode(node, org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL) == org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT)
			{
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
			return resetPosition();
		}
		return this;
		}

		public override void setMark()
		{
		_source.setMark();
		}

		public override void gotoMark()
		{
		_source.gotoMark();
		}

	}

}