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
 * $Id: StepIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// A step iterator is used to evaluate expressions like "BOOK/TITLE". 
	/// A better name for this iterator would have been ParentIterator since 
	/// both "BOOK" and "TITLE" are steps in XPath lingo. Step iterators are 
	/// constructed from two other iterators which we are going to refer to 
	/// as "outer" and "inner". Every node from the outer iterator (the one 
	/// for BOOK in our example) is used to initialize the inner iterator. 
	/// After this initialization, every node from the inner iterator is 
	/// returned (in essence, implementing a "nested loop").
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author Morten Jorgensen
	/// </summary>
	public class StepIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// A reference to the "outer" iterator.
		/// </summary>
		protected internal DTMAxisIterator _source;

		/// <summary>
		/// A reference to the "inner" iterator.
		/// </summary>
		protected internal DTMAxisIterator _iterator;

		/// <summary>
		/// Temp variable to store a marked position.
		/// </summary>
		private int _pos = -1;

		public StepIterator(DTMAxisIterator source, DTMAxisIterator iterator)
		{
		_source = source;
		_iterator = iterator;
	// System.out.println("SI source = " + source + " this = " + this);
	// System.out.println("SI iterator = " + iterator + " this = " + this);
		}


		public override bool Restartable
		{
			set
			{
			_isRestartable = value;
			_source.Restartable = value;
			_iterator.Restartable = true; // must be restartable
			}
		}

		public override DTMAxisIterator cloneIterator()
		{
		_isRestartable = false;
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StepIterator clone = (StepIterator) super.clone();
			StepIterator clone = (StepIterator) base.clone();
			clone._source = _source.cloneIterator();
			clone._iterator = _iterator.cloneIterator();
			clone._iterator.Restartable = true; // must be restartable
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
			// Set start node for left-hand iterator...
			_source.StartNode = _startNode = node;

			// ... and get start node for right-hand iterator from left-hand,
			// with special case for //* path - see ParentLocationPath
			_iterator.StartNode = _includeSelf ? _startNode : _source.next();
			return resetPosition();
		}
		return this;
		}

		public override DTMAxisIterator reset()
		{
		_source.reset();
		// Special case for //* path - see ParentLocationPath
		_iterator.StartNode = _includeSelf ? _startNode : _source.next();
		return resetPosition();
		}

		public override int next()
		{
		for (int node;;)
		{
			// Try to get another node from the right-hand iterator
			if ((node = _iterator.next()) != END)
			{
			return returnNode(node);
			}
			// If not, get the next starting point from left-hand iterator...
			else if ((node = _source.next()) == END)
			{
			return END;
			}
			// ...and pass it on to the right-hand iterator
			else
			{
			_iterator.StartNode = node;
			}
		}
		}

		public override void setMark()
		{
		_source.setMark();
		_iterator.setMark();
		//_pos = _position;
		}

		public override void gotoMark()
		{
		_source.gotoMark();
		_iterator.gotoMark();
		//_position = _pos;
		}
	}

}