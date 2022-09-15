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
 * $Id: AbsoluteIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;

	/// <summary>
	/// Absolute iterators ignore the node that is passed to setStartNode(). 
	/// Instead, they always start from the root node. The node passed to 
	/// setStartNode() is not totally useless, though. It is needed to obtain the 
	/// DOM mask, i.e. the index into the MultiDOM table that corresponds to the 
	/// DOM "owning" the node. 
	/// 
	/// The DOM mask is cached, so successive calls to setStartNode() passing 
	/// nodes from other DOMs will have no effect (i.e. this iterator cannot 
	/// migrate between DOMs).
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class AbsoluteIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// Source for this iterator.
		/// </summary>
		private DTMAxisIterator _source;

		public AbsoluteIterator(DTMAxisIterator source)
		{
		_source = source;
	// System.out.println("AI source = " + source + " this = " + this);
		}

		public override bool Restartable
		{
			set
			{
			_isRestartable = value;
			_source.Restartable = value;
			}
		}

		public override DTMAxisIterator setStartNode(int node)
		{
		_startNode = DTMDefaultBase.ROOTNODE;
		if (_isRestartable)
		{
			_source.StartNode = _startNode;
			resetPosition();
		}
		return this;
		}

		public override int next()
		{
		return returnNode(_source.next());
		}

		public override DTMAxisIterator cloneIterator()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AbsoluteIterator clone = (AbsoluteIterator) super.clone();
			AbsoluteIterator clone = (AbsoluteIterator) base.clone();
			clone._source = _source.cloneIterator(); // resets source
			clone.resetPosition();
			clone._isRestartable = false;
			return clone;
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