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
 * $Id: AnyNodeCounter.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{
	using DOM = org.apache.xalan.xsltc.DOM;
	using Translet = org.apache.xalan.xsltc.Translet;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public abstract class AnyNodeCounter : NodeCounter
	{
		public AnyNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
		}

		public override NodeCounter setStartNode(int node)
		{
		_node = node;
		_nodeType = _document.getExpandedTypeID(node);
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
						return formatNumbers((int)_value);
					}
			}
			else
			{
				int next = _node;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int root = _document.getDocument();
					int root = _document.Document;
				result = 0;
				while (next >= root && !matchesFrom(next))
				{
				if (matchesCount(next))
				{
					++result;
				}
				next--;
		//%HZ%:  Is this the best way of finding the root?  Is it better to check
		//%HZ%:  parent(next)?
				/*
				if (next == root) {
				    break;
						}
				else {
				    --next;		
						}
						*/
				}
			}
			return formatNumbers(result);
			}
		}

		public static NodeCounter getDefaultNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator)
		{
		return new DefaultAnyNodeCounter(translet, document, iterator);
		}

		internal class DefaultAnyNodeCounter : AnyNodeCounter
		{
		public DefaultAnyNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
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
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int ntype = _document.getExpandedTypeID(_node);
				int ntype = _document.getExpandedTypeID(_node);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int root = _document.getDocument();
						int root = _document.Document;
				while (next >= 0)
				{
					if (ntype == _document.getExpandedTypeID(next))
					{
					result++;
					}
		//%HZ%:  Is this the best way of finding the root?  Is it better to check
		//%HZ%:  parent(next)?
					if (next == root)
					{
						break;
					}
					else
					{
						--next;
					}
				}
				}
				return formatNumbers(result);
			}
		}
		}
	}

}