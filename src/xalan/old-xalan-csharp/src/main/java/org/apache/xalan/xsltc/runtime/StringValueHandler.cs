using System;
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
 * $Id: StringValueHandler.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	using SAXException = org.xml.sax.SAXException;

	using EmptySerializer = org.apache.xml.serializer.EmptySerializer;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class StringValueHandler : EmptySerializer
	{

		private StringBuilder _buffer = new StringBuilder();
		private string _str = null;
		private const string EMPTY_STR = "";
		private bool m_escaping = false;
		private int _nestedLevel = 0;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int off, int len) throws org.xml.sax.SAXException
		public override void characters(char[] ch, int off, int len)
		{
		if (_nestedLevel > 0)
		{
			return;
		}

		if (!string.ReferenceEquals(_str, null))
		{
			_buffer.Append(_str);
			_str = null;
		}
		_buffer.Append(ch, off, len);
		}

		public string Value
		{
			get
			{
			if (_buffer.Length != 0)
			{
				string result = _buffer.ToString();
				_buffer.Length = 0;
				return result;
			}
			else
			{
				string result = _str;
				_str = null;
				return (!string.ReferenceEquals(result, null)) ? result : EMPTY_STR;
			}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String characters) throws org.xml.sax.SAXException
		public override void characters(string characters)
		{
		if (_nestedLevel > 0)
		{
			return;
		}

		if (string.ReferenceEquals(_str, null) && _buffer.Length == 0)
		{
			_str = characters;
		}
		else
		{
			if (!string.ReferenceEquals(_str, null))
			{
				_buffer.Append(_str);
				_str = null;
			}

			_buffer.Append(characters);
		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String qname) throws org.xml.sax.SAXException
		public override void startElement(string qname)
		{
			_nestedLevel++;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String qname) throws org.xml.sax.SAXException
		public override void endElement(string qname)
		{
			_nestedLevel--;
		}

		// Override the setEscaping method just to indicate that this class is
		// aware that that method might be called.
		public override bool setEscaping(bool @bool)
		{
			bool oldEscaping = m_escaping;
			m_escaping = @bool;

			return @bool;
		}

		/// <summary>
		/// The value of a PI must not contain the substring "?>". Should
		/// that substring be present, replace it by "? >". 
		/// </summary>
		public string ValueOfPI
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String value = getValue();
			string value = Value;
    
			if (value.IndexOf("?>", StringComparison.Ordinal) > 0)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = value.length();
				int n = value.Length;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final StringBuffer valueOfPI = new StringBuffer();
				StringBuilder valueOfPI = new StringBuilder();
    
				for (int i = 0; i < n;)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final char ch = value.charAt(i++);
				char ch = value[i++];
				if (ch == '?' && value[i] == '>')
				{
					valueOfPI.Append("? >");
					i++;
				}
				else
				{
					valueOfPI.Append(ch);
				}
				}
				return valueOfPI.ToString();
			}
			return value;
			}
		}
	}

}