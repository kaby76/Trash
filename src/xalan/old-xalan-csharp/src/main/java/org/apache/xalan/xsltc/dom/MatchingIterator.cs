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
 * $Id: MatchingIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// This is a special kind of iterator that takes a source iterator and a 
	/// node N. If initialized with a node M (the parent of N) it computes the 
	/// position of N amongst the children of M. This position can be obtained 
	/// by calling getPosition().
	/// It is an iterator even though next() will never be called. It is used to
	/// match patterns with a single predicate like:
	/// 
	///    BOOK[position() = last()]
	/// 
	/// In this example, the source iterator will return elements of type BOOK, 
	/// a call to position() will return the position of N. Notice that because 
	/// of the way the pattern matching is implemented, N will always be a node 
	/// in the source since (i) it is a BOOK or the test sequence would not be 
	/// considered and (ii) the source iterator is initialized with M which is 
	/// the parent of N. Also, and still in this example, a call to last() will 
	/// return the number of elements in the source (i.e. the number of BOOKs).
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class MatchingIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// A reference to a source iterator.
		/// </summary>
		private DTMAxisIterator _source;

		/// <summary>
		/// The node to match.
		/// </summary>
		private readonly int _match;

		public MatchingIterator(int match, DTMAxisIterator source)
		{
		_source = source;
		_match = match;
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
//ORIGINAL LINE: final MatchingIterator clone = (MatchingIterator) super.clone();
			MatchingIterator clone = (MatchingIterator) base.clone();
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

		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			// iterator is not a clone
			_source.StartNode = node;

			// Calculate the position of the node in the set
			_position = 1;
			while ((node = _source.next()) != org.apache.xml.dtm.DTMAxisIterator_Fields.END && node != _match)
			{
			_position++;
			}
		}
		return this;
		}

		public override DTMAxisIterator reset()
		{
		_source.reset();
		return resetPosition();
		}

		public override int next()
		{
		return _source.next();
		}

		public override int Last
		{
			get
			{
				if (_last == -1)
				{
					_last = _source.Last;
				}
				return _last;
			}
		}

		public override int Position
		{
			get
			{
			return _position;
			}
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