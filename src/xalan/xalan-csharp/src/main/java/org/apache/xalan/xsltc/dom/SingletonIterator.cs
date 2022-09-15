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
 * $Id: SingletonIterator.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class SingletonIterator : DTMAxisIteratorBase
	{
		private int _node;
		private readonly bool _isConstant;

		public SingletonIterator() : this(int.MinValue, false)
		{
		}

		public SingletonIterator(int node) : this(node, false)
		{
		}

		public SingletonIterator(int node, bool constant)
		{
		_node = _startNode = node;
		_isConstant = constant;
		}

		/// <summary>
		/// Override the value of <tt>_node</tt> only when this
		/// object was constructed using the empty constructor.
		/// </summary>
		public override DTMAxisIterator setStartNode(int node)
		{
		if (_isConstant)
		{
			_node = _startNode;
			return resetPosition();
		}
		else if (_isRestartable)
		{
			if (_node <= 0)
			{
			_node = _startNode = node;
			}
			return resetPosition();
		}
		return this;
		}

		public override DTMAxisIterator reset()
		{
		if (_isConstant)
		{
			_node = _startNode;
			return resetPosition();
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _isRestartable;
			bool temp = _isRestartable;
			_isRestartable = true;
			StartNode = _startNode;
			_isRestartable = temp;
		}
		return this;
		}

		public override int next()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int result = _node;
		int result = _node;
		_node = DTMAxisIterator.END;
		return returnNode(result);
		}

		public override void setMark()
		{
		_markedNode = _node;
		}

		public override void gotoMark()
		{
		_node = _markedNode;
		}
	}

}