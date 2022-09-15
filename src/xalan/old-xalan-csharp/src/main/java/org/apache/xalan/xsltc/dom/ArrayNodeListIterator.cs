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
 * $Id: ArrayNodeListIterator.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;

	public class ArrayNodeListIterator : DTMAxisIterator
	{

		private int _pos = 0;

		private int _mark = 0;

		private int[] _nodes;

		private static readonly int[] EMPTY = new int[] { };

		public ArrayNodeListIterator(int[] nodes)
		{
		_nodes = nodes;
		}

		public virtual int next()
		{
		return _pos < _nodes.Length ? _nodes[_pos++] : org.apache.xml.dtm.DTMAxisIterator_Fields.END;
		}

		public virtual DTMAxisIterator reset()
		{
		_pos = 0;
		return this;
		}

		public virtual int Last
		{
			get
			{
			return _nodes.Length;
			}
		}

		public virtual int Position
		{
			get
			{
			return _pos;
			}
		}

		public virtual void setMark()
		{
		_mark = _pos;
		}

		public virtual void gotoMark()
		{
		_pos = _mark;
		}

		public virtual DTMAxisIterator setStartNode(int node)
		{
		if (node == org.apache.xml.dtm.DTMAxisIterator_Fields.END)
		{
			_nodes = EMPTY;
		}
		return this;
		}

		public virtual int StartNode
		{
			get
			{
			return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			}
		}

		public virtual bool Reverse
		{
			get
			{
			return false;
			}
		}

		public virtual DTMAxisIterator cloneIterator()
		{
		return new ArrayNodeListIterator(_nodes);
		}

		public virtual bool Restartable
		{
			set
			{
			}
		}

		public virtual int getNodeByPosition(int position)
		{
		return _nodes[position - 1];
		}

	}

}