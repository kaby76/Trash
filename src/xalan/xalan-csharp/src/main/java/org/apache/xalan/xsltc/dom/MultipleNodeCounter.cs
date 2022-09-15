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
 * $Id: MultipleNodeCounter.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{
	using DOM = org.apache.xalan.xsltc.DOM;
	using Translet = org.apache.xalan.xsltc.Translet;
	using IntegerArray = org.apache.xalan.xsltc.util.IntegerArray;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using Axis = org.apache.xml.dtm.Axis;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public abstract class MultipleNodeCounter : NodeCounter
	{
		private DTMAxisIterator _precSiblings = null;

		public MultipleNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
		}

		public override NodeCounter setStartNode(int node)
		{
		_node = node;
		_nodeType = _document.getExpandedTypeID(node);
		_precSiblings = _document.getAxisIterator(Axis.PRECEDINGSIBLING);
		return this;
		}

		public override string Counter
		{
			get
			{
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
    
			IntegerArray ancestors = new IntegerArray();
    
			// Gather all ancestors that do not match from pattern
			int next = _node;
			ancestors.add(next); // include self
			while ((next = _document.getParent(next)) > END && !matchesFrom(next))
			{
				ancestors.add(next);
			}
    
			// Create an array of counters
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int nAncestors = ancestors.cardinality();
			int nAncestors = ancestors.cardinality();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int[] counters = new int[nAncestors];
			int[] counters = new int[nAncestors];
			for (int i = 0; i < nAncestors; i++)
			{
				counters[i] = int.MinValue;
			}
    
			// Increment array of counters according to semantics
			for (int j = 0, i = nAncestors - 1; i >= 0 ; i--, j++)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int counter = counters[j];
				int counter = counters[j];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int ancestor = ancestors.at(i);
				int ancestor = ancestors.at(i);
    
				if (matchesCount(ancestor))
				{
				_precSiblings.StartNode = ancestor;
				while ((next = _precSiblings.next()) != END)
				{
					if (matchesCount(next))
					{
					counters[j] = (counters[j] == int.MinValue) ? 1 : counters[j] + 1;
					}
				}
				// Count the node itself
				counters[j] = counters[j] == int.MinValue ? 1 : counters[j] + 1;
				}
			}
			return formatNumbers(counters);
			}
		}

		public static NodeCounter getDefaultNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator)
		{
		return new DefaultMultipleNodeCounter(translet, document, iterator);
		}

		internal class DefaultMultipleNodeCounter : MultipleNodeCounter
		{
		public DefaultMultipleNodeCounter(Translet translet, DOM document, DTMAxisIterator iterator) : base(translet, document, iterator)
		{
		}
		}
	}

}