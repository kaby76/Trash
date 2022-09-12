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
 * $Id: CachedNodeListIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;
	using IntegerArray = org.apache.xalan.xsltc.util.IntegerArray;

	/// <summary>
	/// CachedNodeListIterator is used for select expressions in a 
	/// variable or parameter. This iterator caches all nodes in an 
	/// IntegerArray. Its cloneIterator() method is overridden to 
	/// return an object of ClonedNodeListIterator.
	/// </summary>
	public sealed class CachedNodeListIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// Source for this iterator.
		/// </summary>
		private DTMAxisIterator _source;
		private IntegerArray _nodes = new IntegerArray();
		private int _numCachedNodes = 0;
		private int _index = 0;
		private bool _isEnded = false;

		public CachedNodeListIterator(DTMAxisIterator source)
		{
		_source = source;
		}

		public override bool Restartable
		{
			set
			{
			//_isRestartable = value;
			//_source.setRestartable(value);
			}
		}

		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isRestartable)
		{
			_startNode = node;
			_source.StartNode = node;
			resetPosition();

			_isRestartable = false;
		}
		return this;
		}

		public override int next()
		{
			return getNode(_index++);
		}

		public override int Position
		{
			get
			{
				return _index == 0 ? 1 : _index;
			}
		}

		public override int getNodeByPosition(int pos)
		{
			return getNode(pos);
		}

		public int getNode(int index)
		{
			if (index < _numCachedNodes)
			{
				return _nodes.at(index);
			}
			else if (!_isEnded)
			{
				int node = _source.next();
				if (node != org.apache.xml.dtm.DTMAxisIterator_Fields.END)
				{
					_nodes.add(node);
					_numCachedNodes++;
				}
				else
				{
					_isEnded = true;
				}
				return node;
			}
			else
			{
				return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			}
		}

		public override DTMAxisIterator cloneIterator()
		{
		ClonedNodeListIterator clone = new ClonedNodeListIterator(this);
		return clone;
		}

		public override DTMAxisIterator reset()
		{
			_index = 0;
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