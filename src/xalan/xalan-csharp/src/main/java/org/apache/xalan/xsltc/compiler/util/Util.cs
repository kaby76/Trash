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
 * $Id: Util.java 1225577 2011-12-29 15:54:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using Type = org.apache.bcel.generic.Type;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class Util
	{
		private static char filesep;

		static Util()
		{
		string temp = System.getProperty("file.separator", "/");
		filesep = temp[0];
		}

		public static string noExtName(string name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = name.lastIndexOf('.');
		int index = name.LastIndexOf('.');
		return name.Substring(0, index >= 0 ? index : name.Length);
		}

		/// <summary>
		/// Search for both slashes in order to support URLs and 
		/// files.
		/// </summary>
		public static string baseName(string name)
		{
		int index = name.LastIndexOf('\\');
		if (index < 0)
		{
			index = name.LastIndexOf('/');
		}

		if (index >= 0)
		{
			return name.Substring(index + 1);
		}
		else
		{
			int lastColonIndex = name.LastIndexOf(':');
			if (lastColonIndex > 0)
			{
				return name.Substring(lastColonIndex + 1);
			}
			else
			{
				return name;
			}
		}
		}

		/// <summary>
		/// Search for both slashes in order to support URLs and 
		/// files.
		/// </summary>
		public static string pathName(string name)
		{
		int index = name.LastIndexOf('/');
		if (index < 0)
		{
			index = name.LastIndexOf('\\');
		}
		return name.Substring(0, index + 1);
		}

		/// <summary>
		/// Replace all illegal Java chars by '_'.
		/// </summary>
		public static string toJavaName(string name)
		{
		if (name.Length > 0)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer result = new StringBuffer();
			StringBuilder result = new StringBuilder();

			char ch = name[0];
			result.Append(Character.isJavaIdentifierStart(ch) ? ch : '_');

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = name.length();
			int n = name.Length;
			for (int i = 1; i < n; i++)
			{
			ch = name[i];
			result.Append(Character.isJavaIdentifierPart(ch) ? ch : '_');
			}
			return result.ToString();
		}
		return name;
		}

		public static Type getJCRefType(string signature)
		{
		return Type.getType(signature);
		}

		public static string internalName(string cname)
		{
		return cname.Replace('.', filesep);
		}

		public static void println(string s)
		{
		Console.WriteLine(s);
		}

		public static void println(char ch)
		{
		Console.WriteLine(ch);
		}

		public static void TRACE1()
		{
		Console.WriteLine("TRACE1");
		}

		public static void TRACE2()
		{
		Console.WriteLine("TRACE2");
		}

		public static void TRACE3()
		{
		Console.WriteLine("TRACE3");
		}

		/// <summary>
		/// Replace a certain character in a string with a new substring.
		/// </summary>
		public static string replace(string @base, char ch, string str)
		{
		return (@base.IndexOf(ch) < 0) ? @base : replace(@base, ch.ToString(), new string[] {str});
		}

		public static string replace(string @base, string delim, string[] str)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = super.length();
		int len = @base.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer result = new StringBuffer();
		StringBuilder result = new StringBuilder();

		for (int i = 0; i < len; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = super.charAt(i);
			char ch = @base[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = delim.indexOf(ch);
			int k = delim.IndexOf(ch);

			if (k >= 0)
			{
			result.Append(str[k]);
			}
			else
			{
			result.Append(ch);
			}
		}
		return result.ToString();
		}

		/// <summary>
		/// Replace occurances of '.', '-', '/' and ':'
		/// </summary>
		public static string escape(string input)
		{
		return replace(input, ".-/:", new string[] {"$dot$", "$dash$", "$slash$", "$colon$"});
		}

		public static string getLocalName(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = qname.lastIndexOf(':');
		int index = qname.LastIndexOf(':');
		return (index > 0) ? qname.Substring(index + 1) : qname;
		}

		public static string getPrefix(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = qname.lastIndexOf(':');
		int index = qname.LastIndexOf(':');
		return (index > 0) ? qname.Substring(0, index) : Constants.EMPTYSTRING;
		}

		/// <summary>
		/// Checks if the string is a literal (i.e. not an AVT) or not.
		/// </summary>
		public static bool isLiteral(string str)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = str.length();
			int length = str.Length;
			for (int i = 0; i < length - 1; i++)
			{
				if (str[i] == '{' && str[i + 1] != '{')
				{
				return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if the string is valid list of qnames
		/// </summary>
		public static bool isValidQNames(string str)
		{
			if ((!string.ReferenceEquals(str, null)) && (!str.Equals(Constants.EMPTYSTRING)))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.StringTokenizer tokens = new java.util.StringTokenizer(str);
				StringTokenizer tokens = new StringTokenizer(str);
				while (tokens.hasMoreTokens())
				{
					if (!XML11Char.isXML11ValidQName(tokens.nextToken()))
					{
						return false;
					}
				}
			}
			return true;
		}

	}


}