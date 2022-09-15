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
 * $Id: SingleNodeCounter.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using Axis = org.apache.xml.dtm.Axis;


	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public abstract class SingleNodeCounter : NodeCounter
	{
		private static readonly int[] EmptyArray = new int[] { };
		internal DTMAxisIterator _countSiblings = null;

		public SingleNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
		}

		public override NodeCounter setStartNode(int node)
		{
		_node = node;
		_nodeType = _document.getExpandedTypeID(node);
		_countSiblings = _document.getAxisIterator(Axis.PRECEDINGSIBLING);
		return this;
		}

		public override string Counter
		{
			get
			{
			int result;
			if (_value != int.MinValue)
			{
						//See Errata E24
						if (_value == 0)
						{
							return "0";
						}
						else if (double.IsNaN(_value))
						{
							return "NaN";
						}
						else if (_value < 0 && double.IsInfinity(_value))
						{
							return "-Infinity";
						}
						else if (double.IsInfinity(_value))
						{
							return "Infinity";
						}
						else
						{
							result = (int) _value;
						}
			}
			else
			{
				int next = _node;
				result = 0;
				if (!matchesCount(next))
				{
				while ((next = _document.getParent(next)) > END)
				{
					if (matchesCount(next))
					{
					break; // found target
					}
					if (matchesFrom(next))
					{
					next = END;
					break; // no target found
					}
				}
				}
    
				if (next != END)
				{
				_countSiblings.StartNode = next;
				do
				{
					if (matchesCount(next))
					{
						result++;
					}
				} while ((next = _countSiblings.next()) != END);
				}
				else
				{
				// If no target found then pass the empty list
				return formatNumbers(EmptyArray);
				}
			}
			return formatNumbers(result);
			}
		}

		public static NodeCounter getDefaultNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator)
		{
		return new DefaultSingleNodeCounter(translet, document, iterator);
		}

		internal class DefaultSingleNodeCounter : SingleNodeCounter
		{
		public DefaultSingleNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
		}

		public override NodeCounter setStartNode(int node)
		{
			_node = node;
			_nodeType = _document.getExpandedTypeID(node);
			_countSiblings = _document.getTypedAxisIterator(Axis.PRECEDINGSIBLING, _document.getExpandedTypeID(node));
			return this;
		}

		public override string Counter
		{
			get
			{
				int result;
				if (_value != int.MinValue)
				{
						//See Errata E24
						if (_value == 0)
						{
							return "0";
						}
						else if (double.IsNaN(_value))
						{
							return "NaN";
						}
						else if (_value < 0 && double.IsInfinity(_value))
						{
							return "-Infinity";
						}
						else if (double.IsInfinity(_value))
						{
							return "Infinity";
						}
						else
						{
							result = (int) _value;
						}
				}
				else
				{
				int next;
				result = 1;
				_countSiblings.StartNode = _node;
				while ((next = _countSiblings.next()) != END)
				{
					result++;
				}
				}
				return formatNumbers(result);
			}
		}
		}
	}


}