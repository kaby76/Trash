using System;

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
 * $Id: NodeSortRecord.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{


	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using StringComparable = org.apache.xml.utils.StringComparable;

	/// <summary>
	/// Base class for sort records containing application specific sort keys 
	/// </summary>
	public abstract class NodeSortRecord
	{
		public const int COMPARE_STRING = 0;
		public const int COMPARE_NUMERIC = 1;

		public const int COMPARE_ASCENDING = 0;
		public const int COMPARE_DESCENDING = 1;

		/// <summary>
		/// A reference to a collator. May be updated by subclass if the stylesheet
		/// specifies a different language (will be updated iff _locale is updated). </summary>
		/// @deprecated This field continues to exist for binary compatibility.
		///             New code should not refer to it. 
		private static readonly Collator DEFAULT_COLLATOR = Collator.Instance;

		/// <summary>
		/// A reference to the first Collator </summary>
		/// @deprecated This field continues to exist for binary compatibility.
		///             New code should not refer to it. 
		protected internal Collator _collator = DEFAULT_COLLATOR;
		protected internal Collator[] _collators;

		/// <summary>
		/// A locale field that might be set by an instance of a subclass. </summary>
		/// @deprecated This field continues to exist for binary compatibility.
		///             New code should not refer to it. 
		protected internal Locale _locale;

		protected internal CollatorFactory _collatorFactory;

		protected internal SortSettings _settings;

		private DOM _dom = null;
		private int _node; // The position in the current iterator
		private int _last = 0; // Number of nodes in the current iterator
		private int _scanned = 0; // Number of key levels extracted from DOM

		private object[] _values; // Contains Comparable  objects

		/// <summary>
		/// This constructor is run by a call to ClassLoader in the
		/// makeNodeSortRecord method in the NodeSortRecordFactory class. Since we
		/// cannot pass any parameters to the constructor in that case we just set
		/// the default values here and wait for new values through initialize().
		/// </summary>
		public NodeSortRecord(int node)
		{
		_node = node;
		}

		public NodeSortRecord() : this(0)
		{
		}

		/// <summary>
		/// This method allows the caller to set the values that could not be passed
		/// to the default constructor.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void initialize(int node, int last, org.apache.xalan.xsltc.DOM dom, SortSettings settings) throws org.apache.xalan.xsltc.TransletException
		public void initialize(int node, int last, DOM dom, SortSettings settings)
		{
		_dom = dom;
		_node = node;
		_last = last;
			_settings = settings;

			int levels = settings.SortOrders.Length;
		_values = new object[levels];

		// -- W. Eliot Kimber (eliot@isogen.com)
			string colFactClassname = System.getProperty("org.apache.xalan.xsltc.COLLATOR_FACTORY");

			if (!string.ReferenceEquals(colFactClassname, null))
			{
				try
				{
					object candObj = ObjectFactory.findProviderClass(colFactClassname, ObjectFactory.findClassLoader(), true);
					_collatorFactory = (CollatorFactory)candObj;
				}
				catch (ClassNotFoundException e)
				{
					throw new TransletException(e);
				}
				Locale[] locales = settings.Locales;
				_collators = new Collator[levels];
				for (int i = 0; i < levels; i++)
				{
					_collators[i] = _collatorFactory.getCollator(locales[i]);
				}
				_collator = _collators[0];
			}
			else
			{
				_collators = settings.Collators;
				_collator = _collators[0];
			}
		}

		/// <summary>
		/// Returns the node for this sort object
		/// </summary>
		public int Node
		{
			get
			{
			return _node;
			}
		}

		/// 
		public int compareDocOrder(NodeSortRecord other)
		{
		return _node - other._node;
		}

		/// <summary>
		/// Get the string or numeric value of a specific level key for this sort
		/// element. The value is extracted from the DOM if it is not already in
		/// our sort key vector.
		/// </summary>
		private IComparable stringValue(int level)
		{
			// Get value from our array if possible
			if (_scanned <= level)
			{
				AbstractTranslet translet = _settings.Translet;
				Locale[] locales = _settings.Locales;
				string[] caseOrder = _settings.CaseOrders;

				// Get value from DOM if accessed for the first time
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String str = extractValueFromDOM(_dom, _node, level, translet, _last);
				string str = extractValueFromDOM(_dom, _node, level, translet, _last);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Comparable key = org.apache.xml.utils.StringComparable.getComparator(str, locales[level], _collators[level], caseOrder[level]);
				IComparable key = StringComparable.getComparator(str, locales[level], _collators[level], caseOrder[level]);
				_values[_scanned++] = key;
				return (key);
			}
			return ((IComparable)_values[level]);
		}

		private double? numericValue(int level)
		{
		// Get value from our vector if possible
		if (_scanned <= level)
		{
				AbstractTranslet translet = _settings.Translet;

			// Get value from DOM if accessed for the first time
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String str = extractValueFromDOM(_dom, _node, level, translet, _last);
			string str = extractValueFromDOM(_dom, _node, level, translet, _last);
			double? num;
			try
			{
			num = Convert.ToDouble(str);
			}
			// Treat number as NaN if it cannot be parsed as a double
			catch (System.FormatException)
			{
			num = new double?(double.NegativeInfinity);
			}
			_values[_scanned++] = num;
			return (num);
		}
		return ((double?)_values[level]);
		}

		/// <summary>
		/// Compare this sort element to another. The first level is checked first,
		/// and we proceed to the next level only if the first level keys are
		/// identical (and so the key values may not even be extracted from the DOM)
		/// 
		/// !!!!MUST OPTIMISE - THIS IS REALLY, REALLY SLOW!!!!
		/// </summary>
		public virtual int compareTo(NodeSortRecord other)
		{
		int cmp, level;
			int[] sortOrder = _settings.SortOrders;
			int levels = _settings.SortOrders.Length;
			int[] compareTypes = _settings.Types;

		for (level = 0; level < levels; level++)
		{
			// Compare the two nodes either as numeric or text values
			if (compareTypes[level] == COMPARE_NUMERIC)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Nullable<double> our = numericValue(level);
			double? our = numericValue(level);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Nullable<double> their = other.numericValue(level);
			double? their = other.numericValue(level);
			cmp = our.Value.CompareTo(their);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Comparable our = stringValue(level);
			IComparable our = stringValue(level);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Comparable their = other.stringValue(level);
			IComparable their = other.stringValue(level);
			cmp = our.CompareTo(their);
			}

			// Return inverse compare value if inverse sort order
			if (cmp != 0)
			{
			return sortOrder[level] == COMPARE_DESCENDING ? 0 - cmp : cmp;
			}
		}
		// Compare based on document order if all sort keys are equal
		return (_node - other._node);
		}

		/// <summary>
		/// Returns the array of Collators used for text comparisons in this object.
		/// May be overridden by inheriting classes
		/// </summary>
		public virtual Collator[] Collator
		{
			get
			{
			return _collators;
			}
		}

		/// <summary>
		/// Extract the sort value for a level of this key.
		/// </summary>
		public abstract string extractValueFromDOM(DOM dom, int current, int level, AbstractTranslet translet, int last);

	}

}