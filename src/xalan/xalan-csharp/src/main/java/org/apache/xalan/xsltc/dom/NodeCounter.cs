using System.Collections;
using System.Text;

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
 * $Id: NodeCounter.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DOM = org.apache.xalan.xsltc.DOM;
	using Translet = org.apache.xalan.xsltc.Translet;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using Axis = org.apache.xml.dtm.Axis;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public abstract class NodeCounter
	{
		public static readonly int END = DTM.NULL;

		protected internal int _node = END;
		protected internal int _nodeType = DOM.FIRST_TYPE - 1;
		protected internal double _value = int.MinValue;

		public readonly DOM _document;
		public readonly DTMAxisIterator _iterator;
		public readonly Translet _translet;

		protected internal string _format;
		protected internal string _lang;
		protected internal string _letterValue;
		protected internal string _groupSep;
		protected internal int _groupSize;

		private bool _separFirst = true;
		private bool _separLast = false;
		private ArrayList _separToks = new ArrayList();
		private ArrayList _formatToks = new ArrayList();
		private int _nSepars = 0;
		private int _nFormats = 0;

		private static readonly string[] Thousands = new string[] {"", "m", "mm", "mmm"};
		private static readonly string[] Hundreds = new string[] {"", "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm"};
		private static readonly string[] Tens = new string[] {"", "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc"};
		private static readonly string[] Ones = new string[] {"", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix"};

	  private StringBuilder _tempBuffer = new StringBuilder();

		protected internal NodeCounter(Translet translet, DOM document, DTMAxisIterator iterator)
		{
		_translet = translet;
		_document = document;
		_iterator = iterator;
		}

		/// <summary>
		/// Set the start node for this counter. The same <tt>NodeCounter</tt>
		/// object can be used multiple times by resetting the starting node.
		/// </summary>
		public abstract NodeCounter setStartNode(int node);

		/// <summary>
		/// If the user specified a value attribute, use this instead of 
		/// counting nodes.
		/// </summary>
		public virtual NodeCounter setValue(double value)
		{
		_value = value;
		return this;
		}

		/// <summary>
		/// Sets formatting fields before calling formatNumbers().
		/// </summary>
		protected internal virtual void setFormatting(string format, string lang, string letterValue, string groupSep, string groupSize)
		{
		_lang = lang;
		_groupSep = groupSep;
		_letterValue = letterValue;

		try
		{
			_groupSize = int.Parse(groupSize);
		}
		catch (System.FormatException)
		{
		   _groupSize = 0;
		}
		Tokens = format;

		}

	  // format == null assumed here 
	 private in string Tokens
	 {
		 set
		 {
			 if ((!string.ReferenceEquals(_format, null)) && (value.Equals(_format)))
			 { // has already been set
				return;
			 }
			 _format = value;
			 // reset
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int length = _format.length();
			 int length = _format.Length;
			 bool isFirst = true;
			 _separFirst = true;
			 _separLast = false;
			 _nSepars = 0;
			 _nFormats = 0;
			 _separToks.Clear();
			 _formatToks.Clear();
    
				 /* 
				  * Tokenize the value string into alphanumeric and non-alphanumeric
				  * tokens as described in M. Kay page 241.
				  */
				 for (int j = 0, i = 0; i < length;)
				 {
						 char c = value[i];
						 for (j = i; char.IsLetterOrDigit(c);)
						 {
							 if (++i == length)
							 {
								 break;
							 }
					 c = value[i];
						 }
						 if (i > j)
						 {
							 if (isFirst)
							 {
								 _separToks.Add(".");
								 isFirst = _separFirst = false;
							 }
							 _formatToks.Add(value.Substring(j, i - j));
						 }
    
						 if (i == length)
						 {
							 break;
						 }
    
						 c = value[i];
						 for (j = i; !char.IsLetterOrDigit(c);)
						 {
							 if (++i == length)
							 {
								 break;
							 }
							 c = value[i];
							 isFirst = false;
						 }
						 if (i > j)
						 {
							 _separToks.Add(value.Substring(j, i - j));
						 }
				 }
    
				 _nSepars = _separToks.Count;
				 _nFormats = _formatToks.Count;
				 if (_nSepars > _nFormats)
				 {
					 _separLast = true;
				 }
    
				 if (_separFirst)
				 {
					 _nSepars--;
				 }
				 if (_separLast)
				 {
					 _nSepars--;
				 }
				 if (_nSepars == 0)
				 {
					 _separToks.Insert(1, ".");
					 _nSepars++;
				 }
				 if (_separFirst)
				 {
					 _nSepars++;
				 }
    
		 }
	 }
		/// <summary>
		/// Sets formatting fields to their default values.
		/// </summary>
		public virtual NodeCounter setDefaultFormatting()
		{
		setFormatting("1", "en", "alphabetic", null, null);
		return this;
		}

		/// <summary>
		/// Returns the position of <tt>node</tt> according to the level and 
		/// the from and count patterns.
		/// </summary>
		public abstract string Counter {get;}

		/// <summary>
		/// Returns the position of <tt>node</tt> according to the level and 
		/// the from and count patterns. This position is converted into a
		/// string based on the arguments passed.
		/// </summary>
		public virtual string getCounter(string format, string lang, string letterValue, string groupSep, string groupSize)
		{
		setFormatting(format, lang, letterValue, groupSep, groupSize);
		return Counter;
		}

		/// <summary>
		/// Returns true if <tt>node</tt> matches the count pattern. By
		/// default a node matches the count patterns if it is of the 
		/// same type as the starting node.
		/// </summary>
		public virtual bool matchesCount(int node)
		{
		return _nodeType == _document.getExpandedTypeID(node);
		}

		/// <summary>
		/// Returns true if <tt>node</tt> matches the from pattern. By default, 
		/// no node matches the from pattern.
		/// </summary>
		public virtual bool matchesFrom(int node)
		{
		return false;
		}

		/// <summary>
		/// Format a single value according to the format parameters.
		/// </summary>
		protected internal virtual string formatNumbers(int value)
		{
		return formatNumbers(new int[] {value});
		}

		/// <summary>
		/// Format a sequence of values according to the format paramaters
		/// set by calling setFormatting().
		/// </summary>
		protected internal virtual string formatNumbers(int[] values)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nValues = values.length;
		int nValues = values.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = _format.length();
		int length = _format.Length;

		bool isEmpty = true;
		for (int i = 0; i < nValues; i++)
		{
			if (values[i] != int.MinValue)
			{
			isEmpty = false;
			}
		}
		if (isEmpty)
		{
			return ("");
		}

		// Format the output string using the values array and the fmt. tokens
		bool isFirst = true;
		int t = 0, n = 0, s = 1;
	  _tempBuffer.Length = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = _tempBuffer;
		StringBuilder buffer = _tempBuffer;

		// Append separation token before first digit/letter/numeral
		if (_separFirst)
		{
			buffer.Append((string)_separToks[0]);
		}

		// Append next digit/letter/numeral and separation token
		while (n < nValues)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int value = values[n];
			int value = values[n];
			if (value != int.MinValue)
			{
			if (!isFirst)
			{
				buffer.Append((string) _separToks[s++]);
			}
			formatValue(value, (string)_formatToks[t++], buffer);
			if (t == _nFormats)
			{
				t--;
			}
			if (s >= _nSepars)
			{
				s--;
			}
			isFirst = false;
			}
			n++;
		}

		// Append separation token after last digit/letter/numeral
		if (_separLast)
		{
			buffer.Append((string)_separToks[_separToks.Count - 1]);
		}
		return buffer.ToString();
		}

		/// <summary>
		/// Format a single value based on the appropriate formatting token. 
		/// This method is based on saxon (Michael Kay) and only implements
		/// lang="en".
		/// </summary>
		private void formatValue(int value, string format, StringBuilder buffer)
		{
			char c = format[0];

			if (char.IsDigit(c))
			{
				char zero = (char)(c - (int)char.GetNumericValue(c));

				StringBuilder temp = buffer;
				if (_groupSize > 0)
				{
					temp = new StringBuilder();
				}
				string s = "";
				int n = value;
				while (n > 0)
				{
					s = (char)((int) zero + (n % 10)) + s;
					n = n / 10;
				}

				for (int i = 0; i < format.Length - s.Length; i++)
				{
					temp.Append(zero);
				}
				temp.Append(s);

				if (_groupSize > 0)
				{
					for (int i = 0; i < temp.Length; i++)
					{
						if (i != 0 && ((temp.Length - i) % _groupSize) == 0)
						{
							buffer.Append(_groupSep);
						}
						buffer.Append(temp[i]);
					}
				}
			}
		else if (c == 'i' && !_letterValue.Equals("alphabetic"))
		{
				buffer.Append(romanValue(value));
		}
		else if (c == 'I' && !_letterValue.Equals("alphabetic"))
		{
				buffer.Append(romanValue(value).ToUpper());
		}
		else
		{
			int min = (int) c;
			int max = (int) c;

			// Special case for Greek alphabet 
			if (c >= (char)0x3b1 && c <= (char)0x3c9)
			{
			max = 0x3c9; // omega
			}
			else
			{
			// General case: search for end of group
			while (char.IsLetterOrDigit((char)(max + 1)))
			{
				max++;
			}
			}
				buffer.Append(alphaValue(value, min, max));
		}
		}

		private string alphaValue(int value, int min, int max)
		{
			if (value <= 0)
			{
			return "" + value;
			}

			int range = max - min + 1;
			char last = (char)(((value-1) % range) + min);
			if (value > range)
			{
				return alphaValue((value-1) / range, min, max) + last;
			}
		else
		{
				return "" + last;
		}
		}

		private string romanValue(int n)
		{
			if (n <= 0 || n > 4000)
			{
			return "" + n;
			}
			return Thousands[n / 1000] + Hundreds[(n / 100) % 10] + Tens[(n / 10) % 10] + Ones[n % 10];
		}

	}


}