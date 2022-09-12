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
 * $Id: ClonedNodeListIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// A ClonedNodeListIterator is returned by the cloneIterator() method
	/// of a CachedNodeListIterator. Its next() method retrieves the nodes from
	/// the cache of the CachedNodeListIterator.
	/// </summary>
	public sealed class ClonedNodeListIterator : DTMAxisIteratorBase
	{

		/// <summary>
		/// Source for this iterator.
		/// </summary>
		private CachedNodeListIterator _source;
		private int _index = 0;

		public ClonedNodeListIterator(CachedNodeListIterator source)
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
		return this;
		}

		public override int next()
		{
			return _source.getNode(_index++);
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
			return _source.getNode(pos);
		}

		public override DTMAxisIterator cloneIterator()
		{
		return _source.cloneIterator();
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