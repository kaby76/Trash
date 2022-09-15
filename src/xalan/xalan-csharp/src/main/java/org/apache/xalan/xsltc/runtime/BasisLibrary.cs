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
 * $Id: BasisLibrary.java 1225582 2011-12-29 15:58:28Z mrglavas $
 */

namespace org.apache.xalan.xsltc.runtime
{

	using DOM = org.apache.xalan.xsltc.DOM;
	using Translet = org.apache.xalan.xsltc.Translet;
	using AbsoluteIterator = org.apache.xalan.xsltc.dom.AbsoluteIterator;
	using ArrayNodeListIterator = org.apache.xalan.xsltc.dom.ArrayNodeListIterator;
	using DOMAdapter = org.apache.xalan.xsltc.dom.DOMAdapter;
	using MultiDOM = org.apache.xalan.xsltc.dom.MultiDOM;
	using SingletonIterator = org.apache.xalan.xsltc.dom.SingletonIterator;
	using StepIterator = org.apache.xalan.xsltc.dom.StepIterator;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;
	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XML11Char = org.apache.xml.utils.XML11Char;
	using Attr = org.w3c.dom.Attr;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using NodeList = org.w3c.dom.NodeList;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Standard XSLT functions. All standard functions expect the current node 
	/// and the DOM as their last two arguments.
	/// </summary>
	public sealed class BasisLibrary
	{

		private const string EMPTYSTRING = "";

		/// <summary>
		/// Standard function count(node-set)
		/// </summary>
		public static int countF(DTMAxisIterator iterator)
		{
		return (iterator.Last);
		}

		/// <summary>
		/// Standard function position() </summary>
		/// @deprecated This method exists only for backwards compatibility with old
		///             translets.  New code should not reference it. 
		public static int positionF(DTMAxisIterator iterator)
		{
			return iterator.Reverse ? iterator.Last - iterator.Position + 1 : iterator.Position;
		}

		/// <summary>
		/// XSLT Standard function sum(node-set). 
		/// stringToDouble is inlined
		/// </summary>
		public static double sumF(DTMAxisIterator iterator, DOM dom)
		{
		try
		{
			double result = 0.0;
			int node;
			while ((node = iterator.next()) != DTMAxisIterator.END)
			{
			result += double.Parse(dom.getStringValueX(node));
			}
			return result;
		}
		catch (System.FormatException)
		{
			return Double.NaN;
		}
		}

		/// <summary>
		/// XSLT Standard function string()
		/// </summary>
		public static string stringF(int node, DOM dom)
		{
		return dom.getStringValueX(node);
		}

		/// <summary>
		/// XSLT Standard function string(value)
		/// </summary>
		public static string stringF(object obj, DOM dom)
		{
		if (obj is DTMAxisIterator)
		{
			return dom.getStringValueX(((DTMAxisIterator)obj).reset().next());
		}
		else if (obj is Node)
		{
			return dom.getStringValueX(((Node)obj).node);
		}
		else if (obj is DOM)
		{
			return ((DOM)obj).StringValue;
		}
		else
		{
			return obj.ToString();
		}
		}

		/// <summary>
		/// XSLT Standard function string(value)
		/// </summary>
		public static string stringF(object obj, int node, DOM dom)
		{
		if (obj is DTMAxisIterator)
		{
			return dom.getStringValueX(((DTMAxisIterator)obj).reset().next());
		}
		else if (obj is Node)
		{
			return dom.getStringValueX(((Node)obj).node);
		}
		else if (obj is DOM)
		{
			// When the first argument is a DOM we want the whole
			// DOM and not just a single node - that would not make sense.
			//return ((DOM)obj).getStringValueX(node);
			return ((DOM)obj).StringValue;
		}
		else if (obj is Double)
		{
			double? d = (double?)obj;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String result = d.toString();
			string result = d.ToString();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = result.length();
			int length = result.Length;
			if ((result[length - 2] == '.') && (result[length - 1] == '0'))
			{
			return result.Substring(0, length - 2);
			}
			else
			{
			return result;
			}
		}
		else
		{
			if (obj != null)
			{
			return obj.ToString();
			}
			else
			{
			return stringF(node, dom);
			}
		}
		}

		/// <summary>
		/// XSLT Standard function number()
		/// </summary>
		public static double numberF(int node, DOM dom)
		{
		return stringToReal(dom.getStringValueX(node));
		}

		/// <summary>
		/// XSLT Standard function number(value)
		/// </summary>
		public static double numberF(object obj, DOM dom)
		{
		if (obj is Double)
		{
			return ((double?) obj).Value;
		}
		else if (obj is Integer)
		{
			return ((int?) obj).Value;
		}
		else if (obj is Boolean)
		{
			return ((bool?) obj).Value ? 1.0 : 0.0;
		}
		else if (obj is string)
		{
			return stringToReal((string) obj);
		}
		else if (obj is DTMAxisIterator)
		{
			DTMAxisIterator iter = (DTMAxisIterator) obj;
			return stringToReal(dom.getStringValueX(iter.reset().next()));
		}
		else if (obj is Node)
		{
			return stringToReal(dom.getStringValueX(((Node) obj).node));
		}
		else if (obj is DOM)
		{
			return stringToReal(((DOM) obj).StringValue);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(INVALID_ARGUMENT_ERR, className, "number()");
			return 0.0;
		}
		}

		/// <summary>
		/// XSLT Standard function round()
		/// </summary>
		public static double roundF(double d)
		{
				return (d < -0.5 || d>0.0)?Math.Floor(d + 0.5):((d == 0.0)? d:(double.IsNaN(d)?Double.NaN:-0.0));
		}

		/// <summary>
		/// XSLT Standard function boolean()
		/// </summary>
		public static bool booleanF(object obj)
		{
		if (obj is Double)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = ((System.Nullable<double>) obj).doubleValue();
			double temp = ((double?) obj).Value;
			return temp != 0.0 && !double.IsNaN(temp);
		}
		else if (obj is Integer)
		{
			return ((int?) obj).Value != 0;
		}
		else if (obj is Boolean)
		{
			return ((bool?) obj).Value;
		}
		else if (obj is string)
		{
			return !((string) obj).Equals(EMPTYSTRING);
		}
		else if (obj is DTMAxisIterator)
		{
			DTMAxisIterator iter = (DTMAxisIterator) obj;
			return iter.reset().next() != DTMAxisIterator.END;
		}
		else if (obj is Node)
		{
			return true;
		}
		else if (obj is DOM)
		{
			string temp = ((DOM) obj).StringValue;
			return !temp.Equals(EMPTYSTRING);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(INVALID_ARGUMENT_ERR, className, "boolean()");
		}
		return false;
		}

		/// <summary>
		/// XSLT Standard function substring(). Must take a double because of
		/// conversions resulting into NaNs and rounding.
		/// </summary>
		public static string substringF(string value, double start)
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int strlen = value.length();
			int strlen = value.Length;
			int istart = (int)(long)Math.Round(start, MidpointRounding.AwayFromZero) - 1;

			if (double.IsNaN(start))
			{
				return (EMPTYSTRING);
			}
			if (istart > strlen)
			{
				return (EMPTYSTRING);
			}
			 if (istart < 1)
			 {
				 istart = 0;
			 }

			return value.Substring(istart);
		}
		catch (System.IndexOutOfRangeException)
		{
			runTimeError(RUN_TIME_INTERNAL_ERR, "substring()");
			return null;
		}
		}

		/// <summary>
		/// XSLT Standard function substring(). Must take a double because of
		/// conversions resulting into NaNs and rounding.
		/// </summary>
		public static string substringF(string value, double start, double length)
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int strlen = value.length();
			int strlen = value.Length;
			int istart = (int)(long)Math.Round(start, MidpointRounding.AwayFromZero) - 1;
			int isum = istart + (int)(long)Math.Round(length, MidpointRounding.AwayFromZero);

			if (double.IsInfinity(length))
			{
				isum = int.MaxValue;
			}

			if (double.IsNaN(start) || double.IsNaN(length))
			{
			return (EMPTYSTRING);
			}
			if (double.IsInfinity(start))
			{
				return (EMPTYSTRING);
			}
			if (istart > strlen)
			{
				return (EMPTYSTRING);
			}
			if (isum < 0)
			{
				return (EMPTYSTRING);
			}
			 if (istart < 0)
			 {
				 istart = 0;
			 }

			if (isum > strlen)
			{
			return value.Substring(istart);
			}
			else
			{
			return value.Substring(istart, isum - istart);
			}
		}
		catch (System.IndexOutOfRangeException)
		{
			runTimeError(RUN_TIME_INTERNAL_ERR, "substring()");
			return null;
		}
		}

		/// <summary>
		/// XSLT Standard function substring-after(). 
		/// </summary>
		public static string substring_afterF(string value, string substring)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = value.indexOf(substring);
		int index = value.IndexOf(substring, StringComparison.Ordinal);
		if (index >= 0)
		{
			return value.Substring(index + substring.Length);
		}
		else
		{
			return EMPTYSTRING;
		}
		}

		/// <summary>
		/// XSLT Standard function substring-before(). 
		/// </summary>
		public static string substring_beforeF(string value, string substring)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = value.indexOf(substring);
		int index = value.IndexOf(substring, StringComparison.Ordinal);
		if (index >= 0)
		{
			return value.Substring(0, index);
		}
		else
		{
			return EMPTYSTRING;
		}
		}

		/// <summary>
		/// XSLT Standard function translate(). 
		/// </summary>
		public static string translateF(string value, string from, string to)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int tol = to.length();
		int tol = to.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int froml = from.length();
		int froml = from.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int valuel = value.length();
		int valuel = value.Length;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer result = new StringBuffer();
		StringBuilder result = new StringBuilder();
		for (int j, i = 0; i < valuel; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = value.charAt(i);
			char ch = value[i];
			for (j = 0; j < froml; j++)
			{
			if (ch == from[j])
			{
				if (j < tol)
				{
				result.Append(to[j]);
				}
				break;
			}
			}
			if (j == froml)
			{
			result.Append(ch);
			}
		}
		return result.ToString();
		}

		/// <summary>
		/// XSLT Standard function normalize-space(). 
		/// </summary>
		public static string normalize_spaceF(int node, DOM dom)
		{
		return normalize_spaceF(dom.getStringValueX(node));
		}

		/// <summary>
		/// XSLT Standard function normalize-space(string). 
		/// </summary>
		public static string normalize_spaceF(string value)
		{
		int i = 0, n = value.Length;
		StringBuilder result = new StringBuilder();

		while (i < n && isWhiteSpace(value[i]))
		{
			i++;
		}

		while (true)
		{
			while (i < n && !isWhiteSpace(value[i]))
			{
			result.Append(value[i++]);
			}
			if (i == n)
			{
			break;
			}
			while (i < n && isWhiteSpace(value[i]))
			{
			i++;
			}
			if (i < n)
			{
			result.Append(' ');
			}
		}
		return result.ToString();
		}

		/// <summary>
		/// XSLT Standard function generate-id(). 
		/// </summary>
		public static string generate_idF(int node)
		{
		if (node > 0)
		{
			// Only generate ID if node exists
			return "N" + node;
		}
		else
		{
			// Otherwise return an empty string
			return EMPTYSTRING;
		}
		}

		/// <summary>
		/// utility function for calls to local-name(). 
		/// </summary>
		public static string getLocalName(string value)
		{
		int idx = value.LastIndexOf(':');
		if (idx >= 0)
		{
			value = value.Substring(idx + 1);
		}
		idx = value.LastIndexOf('@');
		if (idx >= 0)
		{
			value = value.Substring(idx + 1);
		}
		return (value);
		}

		/// <summary>
		/// External functions that cannot be resolved are replaced with a call
		/// to this method. This method will generate a runtime errors. A good
		/// stylesheet checks whether the function exists using conditional
		/// constructs, and never really tries to call it if it doesn't exist.
		/// But simple stylesheets may result in a call to this method.
		/// The compiler should generate a warning if it encounters a call to
		/// an unresolved external function.
		/// </summary>
		public static void unresolved_externalF(string name)
		{
		runTimeError(EXTERNAL_FUNC_ERR, name);
		}

		/// <summary>
		/// Utility function to throw a runtime error on the use of an extension 
		/// function when the secure processing feature is set to true.
		/// </summary>
		public static void unallowed_extension_functionF(string name)
		{
			runTimeError(UNALLOWED_EXTENSION_FUNCTION_ERR, name);
		}

		/// <summary>
		/// Utility function to throw a runtime error on the use of an extension 
		/// element when the secure processing feature is set to true.
		/// </summary>
		public static void unallowed_extension_elementF(string name)
		{
			runTimeError(UNALLOWED_EXTENSION_ELEMENT_ERR, name);
		}

		/// <summary>
		/// Utility function to throw a runtime error for an unsupported element.
		/// 
		/// This is only used in forward-compatibility mode, when the control flow
		/// cannot be determined. In 1.0 mode, the error message is emitted at 
		/// compile time.
		/// </summary>
		public static void unsupported_ElementF(string qname, bool isExtension)
		{
		if (isExtension)
		{
			runTimeError(UNSUPPORTED_EXT_ERR, qname);
		}
		else
		{
			runTimeError(UNSUPPORTED_XSL_ERR, qname);
		}
		}

		/// <summary>
		/// XSLT Standard function namespace-uri(node-set).
		/// </summary>
		public static string namespace_uriF(DTMAxisIterator iter, DOM dom)
		{
		return namespace_uriF(iter.next(), dom);
		}

		/// <summary>
		/// XSLT Standard function system-property(name)
		/// </summary>
		public static string system_propertyF(string name)
		{
		if (name.Equals("xsl:version"))
		{
			return ("1.0");
		}
		if (name.Equals("xsl:vendor"))
		{
			return ("Apache Software Foundation (Xalan XSLTC)");
		}
		if (name.Equals("xsl:vendor-url"))
		{
			return ("http://xml.apache.org/xalan-j");
		}

		runTimeError(INVALID_ARGUMENT_ERR, name, "system-property()");
		return (EMPTYSTRING);
		}

		/// <summary>
		/// XSLT Standard function namespace-uri(). 
		/// </summary>
		public static string namespace_uriF(int node, DOM dom)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = dom.getNodeName(node);
		string value = dom.getNodeName(node);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = value.lastIndexOf(':');
		int colon = value.LastIndexOf(':');
		if (colon >= 0)
		{
			return value.Substring(0, colon);
		}
		else
		{
			return EMPTYSTRING;
		}
		}

		/// <summary>
		/// Implements the object-type() extension function.
		/// </summary>
		/// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
		public static string objectTypeF(object obj)
		{
		  if (obj is string)
		  {
			return "string";
		  }
		  else if (obj is Boolean)
		  {
			return "boolean";
		  }
		  else if (obj is Number)
		  {
			return "number";
		  }
		  else if (obj is DOM)
		  {
			return "RTF";
		  }
		  else if (obj is DTMAxisIterator)
		  {
			return "node-set";
		  }
		  else
		  {
			return "unknown";
		  }
		}

		/// <summary>
		/// Implements the nodeset() extension function. 
		/// </summary>
		public static DTMAxisIterator nodesetF(object obj)
		{
		if (obj is DOM)
		{
		   //final DOMAdapter adapter = (DOMAdapter) obj;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.DOM dom = (org.apache.xalan.xsltc.DOM)obj;
		   DOM dom = (DOM)obj;
		   return new SingletonIterator(dom.Document, true);
		}
			else if (obj is DTMAxisIterator)
			{
		   return (DTMAxisIterator) obj;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, "node-set", className);
			return null;
			}
		}

		//-- Begin utility functions

		private static bool isWhiteSpace(char ch)
		{
		return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
		}

		private static bool compareStrings(string lstring, string rstring, int op, DOM dom)
		{
		switch (op)
		{
		case Operators.EQ:
			return lstring.Equals(rstring);

		case Operators.NE:
			return !lstring.Equals(rstring);

		case Operators.GT:
			return numberF(lstring, dom) > numberF(rstring, dom);

		case Operators.LT:
			return numberF(lstring, dom) < numberF(rstring, dom);

		case Operators.GE:
			return numberF(lstring, dom) >= numberF(rstring, dom);

		case Operators.LE:
			return numberF(lstring, dom) <= numberF(rstring, dom);

		default:
			runTimeError(RUN_TIME_INTERNAL_ERR, "compare()");
			return false;
		}
		}

		/// <summary>
		/// Utility function: node-set/node-set compare. 
		/// </summary>
		public static bool compare(DTMAxisIterator left, DTMAxisIterator right, int op, DOM dom)
		{
		int lnode;
		left.reset();

		while ((lnode = left.next()) != DTMAxisIterator.END)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String lvalue = dom.getStringValueX(lnode);
			string lvalue = dom.getStringValueX(lnode);

			int rnode;
			right.reset();
			while ((rnode = right.next()) != DTMAxisIterator.END)
			{
					// String value must be the same if both nodes are the same
					if (lnode == rnode)
					{
						if (op == Operators.EQ)
						{
							return true;
						}
						else if (op == Operators.NE)
						{
							continue;
						}
					}
			if (compareStrings(lvalue, dom.getStringValueX(rnode), op, dom))
			{
				return true;
			}
			}
		}
		return false;
		}

		public static bool compare(int node, DTMAxisIterator iterator, int op, DOM dom)
		{
		//iterator.reset();

		int rnode;
		string value;

		switch (op)
		{
		case Operators.EQ:
				rnode = iterator.next();
				if (rnode != DTMAxisIterator.END)
				{
				value = dom.getStringValueX(node);
					do
					{
				if (node == rnode || value.Equals(dom.getStringValueX(rnode)))
				{
						   return true;
				}
					} while ((rnode = iterator.next()) != DTMAxisIterator.END);
				}
			break;
		case Operators.NE:
				rnode = iterator.next();
				if (rnode != DTMAxisIterator.END)
				{
				value = dom.getStringValueX(node);
					do
					{
				if (node != rnode && !value.Equals(dom.getStringValueX(rnode)))
				{
							return true;
				}
					} while ((rnode = iterator.next()) != DTMAxisIterator.END);
				}
			break;
		case Operators.LT:
			// Assume we're comparing document order here
			while ((rnode = iterator.next()) != DTMAxisIterator.END)
			{
			if (rnode > node)
			{
				return true;
			}
			}
			break;
		case Operators.GT:
			// Assume we're comparing document order here
			while ((rnode = iterator.next()) != DTMAxisIterator.END)
			{
			if (rnode < node)
			{
				return true;
			}
			}
			break;
		}
		return (false);
		}

		/// <summary>
		/// Utility function: node-set/number compare.
		/// </summary>
		public static bool compare(DTMAxisIterator left, in double rnumber, in int op, DOM dom)
		{
		int node;
		//left.reset();

		switch (op)
		{
		case Operators.EQ:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) == rnumber)
			{
				return true;
			}
			}
			break;

		case Operators.NE:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) != rnumber)
			{
				return true;
			}
			}
			break;

		case Operators.GT:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) > rnumber)
			{
				return true;
			}
			}
			break;

		case Operators.LT:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) < rnumber)
			{
				return true;
			}
			}
			break;

		case Operators.GE:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) >= rnumber)
			{
				return true;
			}
			}
			break;

		case Operators.LE:
			while ((node = left.next()) != DTMAxisIterator.END)
			{
			if (numberF(dom.getStringValueX(node), dom) <= rnumber)
			{
				return true;
			}
			}
			break;

		default:
			runTimeError(RUN_TIME_INTERNAL_ERR, "compare()");
		break;
		}

		return false;
		}

		/// <summary>
		/// Utility function: node-set/string comparison. 
		/// </summary>
		public static bool compare(DTMAxisIterator left, in string rstring, int op, DOM dom)
		{
		int node;
		//left.reset();
		while ((node = left.next()) != DTMAxisIterator.END)
		{
			if (compareStrings(dom.getStringValueX(node), rstring, op, dom))
			{
			return true;
			}
		}
		return false;
		}


		public static bool compare(object left, object right, int op, DOM dom)
		{
		bool result = false;
		bool hasSimpleArgs = hasSimpleType(left) && hasSimpleType(right);

		if (op != Operators.EQ && op != Operators.NE)
		{
			// If node-boolean comparison -> convert node to boolean
			if (left is Node || right is Node)
			{
			if (left is Boolean)
			{
				right = new bool?(booleanF(right));
				hasSimpleArgs = true;
			}
			if (right is Boolean)
			{
				left = new bool?(booleanF(left));
				hasSimpleArgs = true;
			}
			}

			if (hasSimpleArgs)
			{
			switch (op)
			{
			case Operators.GT:
				return numberF(left, dom) > numberF(right, dom);

			case Operators.LT:
				return numberF(left, dom) < numberF(right, dom);

			case Operators.GE:
				return numberF(left, dom) >= numberF(right, dom);

			case Operators.LE:
				return numberF(left, dom) <= numberF(right, dom);

			default:
				runTimeError(RUN_TIME_INTERNAL_ERR, "compare()");
			break;
			}
			}
			// falls through
		}

		if (hasSimpleArgs)
		{
			if (left is bool? || right is Boolean)
			{
			result = booleanF(left) == booleanF(right);
			}
			else if (left is double? || right is double? || left is int? || right is Integer)
			{
			result = numberF(left, dom) == numberF(right, dom);
			}
			else
			{ // compare them as strings
			result = stringF(left, dom).Equals(stringF(right, dom));
			}

			if (op == Operators.NE)
			{
			result = !result;
			}
		}
		else
		{
			if (left is Node)
			{
			left = new SingletonIterator(((Node)left).node);
			}
			if (right is Node)
			{
			right = new SingletonIterator(((Node)right).node);
			}

			if (hasSimpleType(left) || left is DOM && right is DTMAxisIterator)
			{
			// swap operands and operator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object temp = right;
			object temp = right;
			right = left;
			left = temp;
					op = Operators.swapOp(op);
			}

			if (left is DOM)
			{
			if (right is Boolean)
			{
				result = ((bool?)right).Value;
				return result == (op == Operators.EQ);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String sleft = ((org.apache.xalan.xsltc.DOM)left).getStringValue();
			string sleft = ((DOM)left).StringValue;

			if (right is Number)
			{
				result = (double)((Number)right) == stringToReal(sleft);
			}
			else if (right is string)
			{
				result = sleft.Equals((string)right);
			}
			else if (right is DOM)
			{
				result = sleft.Equals(((DOM)right).StringValue);
			}

			if (op == Operators.NE)
			{
				result = !result;
			}
			return result;
			}

			// Next, node-set/t for t in {real, string, node-set, result-tree}

			DTMAxisIterator iter = ((DTMAxisIterator)left).reset();

			if (right is DTMAxisIterator)
			{
			result = compare(iter, (DTMAxisIterator)right, op, dom);
			}
			else if (right is string)
			{
			result = compare(iter, (string)right, op, dom);
			}
			else if (right is Number)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = ((Number)right).doubleValue();
			double temp = (double)((Number)right);
			result = compare(iter, temp, op, dom);
			}
			else if (right is Boolean)
			{
			bool temp = ((bool?)right).Value;
			result = (iter.reset().next() != DTMAxisIterator.END) == temp;
			}
			else if (right is DOM)
			{
			result = compare(iter, ((DOM)right).StringValue, op, dom);
			}
			else if (right == null)
			{
			return (false);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = right.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = right.GetType().FullName;
			runTimeError(INVALID_ARGUMENT_ERR, className, "compare()");
			}
		}
		return result;
		}

		/// <summary>
		/// Utility function: used to test context node's language
		/// </summary>
		public static bool testLanguage(string testLang, DOM dom, int node)
		{
		// language for context node (if any)
		string nodeLang = dom.getLanguage(node);
		if (string.ReferenceEquals(nodeLang, null))
		{
			return (false);
		}
		else
		{
			nodeLang = nodeLang.ToLower();
		}

		// compare context node's language agains test language
		testLang = testLang.ToLower();
		if (testLang.Length == 2)
		{
			return (nodeLang.StartsWith(testLang, StringComparison.Ordinal));
		}
		else
		{
			return (nodeLang.Equals(testLang));
		}
		}

		private static bool hasSimpleType(object obj)
		{
		return obj is bool? || obj is double? || obj is int? || obj is string || obj is Node || obj is DOM;
		}

		/// <summary>
		/// Utility function: used in StringType to convert a string to a real.
		/// </summary>
		public static double stringToReal(string s)
		{
		try
		{
			return Convert.ToDouble(s);
		}
		catch (System.FormatException)
		{
			return Double.NaN;
		}
		}

		/// <summary>
		/// Utility function: used in StringType to convert a string to an int.
		/// </summary>
		public static int stringToInt(string s)
		{
		try
		{
			return int.Parse(s);
		}
		catch (System.FormatException)
		{
			return (-1); // ???
		}
		}

		private const int DOUBLE_FRACTION_DIGITS = 340;
		private const double lowerBounds = 0.001;
		private const double upperBounds = 10000000;
		private static DecimalFormat defaultFormatter;
		private static string defaultPattern = "";

		static BasisLibrary()
		{
		NumberFormat f = NumberFormat.getInstance(Locale.getDefault());
		defaultFormatter = (f is DecimalFormat) ? (DecimalFormat) f : new DecimalFormat();
		// Set max fraction digits so that truncation does not occur. Setting 
			// the max to Integer.MAX_VALUE may cause problems with some JDK's.
		defaultFormatter.setMaximumFractionDigits(DOUBLE_FRACTION_DIGITS);
			defaultFormatter.setMinimumFractionDigits(0);
			defaultFormatter.setMinimumIntegerDigits(1);
			defaultFormatter.setGroupingUsed(false);
		string resource = "org.apache.xalan.xsltc.runtime.ErrorMessages";
		m_bundle = ResourceBundle.getBundle(resource);
		}

		/// <summary>
		/// Utility function: used in RealType to convert a real to a string.
		/// Removes the decimal if null.
		/// </summary>
		public static string realToString(double d)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = Math.abs(d);
		double m = Math.Abs(d);
		if ((m >= lowerBounds) && (m < upperBounds))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String result = System.Convert.ToString(d);
			string result = Convert.ToString(d);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = result.length();
			int length = result.Length;
			// Remove leading zeros.
			if ((result[length - 2] == '.') && (result[length - 1] == '0'))
			{
			return result.Substring(0, length - 2);
			}
			else
			{
			return result;
			}
		}
		else
		{
			if (double.IsNaN(d) || double.IsInfinity(d))
			{
			return (Convert.ToString(d));
			}
			return formatNumber(d, defaultPattern, defaultFormatter);
		}
		}

		/// <summary>
		/// Utility function: used in RealType to convert a real to an integer
		/// </summary>
		public static int realToInt(double d)
		{
		return (int)d;
		}

		/// <summary>
		/// Utility function: used to format/adjust  a double to a string. The 
		/// DecimalFormat object comes from the 'formatSymbols' hashtable in 
		/// AbstractTranslet.
		/// </summary>
		private static FieldPosition _fieldPosition = new FieldPosition(0);

		public static string formatNumber(double number, string pattern, DecimalFormat formatter)
		{
			// bugzilla fix 12813 
		if (formatter == null)
		{
			formatter = defaultFormatter;
		}
		try
		{
			StringBuilder result = new StringBuilder();
			if (!string.ReferenceEquals(pattern, defaultPattern))
			{
			formatter.applyLocalizedPattern(pattern);
			}
				formatter.format(number, result, _fieldPosition);
			return result.ToString();
		}
		catch (System.ArgumentException)
		{
			runTimeError(FORMAT_NUMBER_ERR, Convert.ToString(number), pattern);
			return (EMPTYSTRING);
		}
		}

		/// <summary>
		/// Utility function: used to convert references to node-sets. If the
		/// obj is an instanceof Node then create a singleton iterator.
		/// </summary>
		public static DTMAxisIterator referenceToNodeSet(object obj)
		{
		// Convert var/param -> node
		if (obj is Node)
		{
			return (new SingletonIterator(((Node)obj).node));
		}
		// Convert var/param -> node-set
		else if (obj is DTMAxisIterator)
		{
			return (((DTMAxisIterator)obj).cloneIterator().reset());
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, "node-set");
			return null;
		}
		}

		/// <summary>
		/// Utility function: used to convert reference to org.w3c.dom.NodeList.
		/// </summary>
		public static NodeList referenceToNodeList(object obj, DOM dom)
		{
			if (obj is Node || obj is DTMAxisIterator)
			{
				DTMAxisIterator iter = referenceToNodeSet(obj);
				return dom.makeNodeList(iter);
			}
			else if (obj is DOM)
			{
			  dom = (DOM)obj;
			  return dom.makeNodeList(DTMDefaultBase.ROOTNODE);
			}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, "org.w3c.dom.NodeList");
			return null;
		}
		}

		/// <summary>
		/// Utility function: used to convert reference to org.w3c.dom.Node.
		/// </summary>
		public static org.w3c.dom.Node referenceToNode(object obj, DOM dom)
		{
			if (obj is Node || obj is DTMAxisIterator)
			{
				DTMAxisIterator iter = referenceToNodeSet(obj);
				return dom.makeNode(iter);
			}
			else if (obj is DOM)
			{
			  dom = (DOM)obj;
			  DTMAxisIterator iter = dom.getChildren(DTMDefaultBase.ROOTNODE);
			  return dom.makeNode(iter);
			}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, "org.w3c.dom.Node");
			return null;
		}
		}

		/// <summary>
		/// Utility function: used to convert reference to long.
		/// </summary>
		public static long referenceToLong(object obj)
		{
			if (obj is Number)
			{
				return (long)((Number) obj); // handles Integer and Double
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, Long.TYPE);
			return 0;
			}
		}

		/// <summary>
		/// Utility function: used to convert reference to double.
		/// </summary>
		public static double referenceToDouble(object obj)
		{
			if (obj is Number)
			{
				return (double)((Number) obj); // handles Integer and Double
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, Double.TYPE);
			return 0;
			}
		}

		/// <summary>
		/// Utility function: used to convert reference to boolean.
		/// </summary>
		public static bool referenceToBoolean(object obj)
		{
			if (obj is Boolean)
			{
				return ((bool?) obj).Value;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, Boolean.TYPE);
			return false;
			}
		}

		/// <summary>
		/// Utility function: used to convert reference to String.
		/// </summary>
		public static string referenceToString(object obj, DOM dom)
		{
			if (obj is string)
			{
				return (string) obj;
			}
			else if (obj is DTMAxisIterator)
			{
			return dom.getStringValueX(((DTMAxisIterator)obj).reset().next());
			}
		else if (obj is Node)
		{
			return dom.getStringValueX(((Node)obj).node);
		}
		else if (obj is DOM)
		{
			return ((DOM) obj).StringValue;
		}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, className, typeof(string));
			return null;
			}
		}

		/// <summary>
		/// Utility function used to convert a w3c Node into an internal DOM iterator. 
		/// </summary>
		public static DTMAxisIterator node2Iterator(org.w3c.dom.Node node, Translet translet, DOM dom)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node inNode = node;
			org.w3c.dom.Node inNode = node;
			// Create a dummy NodeList which only contains the given node to make 
			// use of the nodeList2Iterator() interface.
			NodeList nodelist = new NodeListAnonymousInnerClass(inNode);

			return nodeList2Iterator(nodelist, translet, dom);
		}

		private class NodeListAnonymousInnerClass : NodeList
		{
			private org.w3c.dom.Node inNode;

			public NodeListAnonymousInnerClass(org.w3c.dom.Node inNode)
			{
				this.inNode = inNode;
			}

			public int Length
			{
				get
				{
					return 1;
				}
			}

			public org.w3c.dom.Node item(int index)
			{
				if (index == 0)
				{
					return inNode;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// In a perfect world, this would be the implementation for
		/// nodeList2Iterator. In reality, though, this causes a
		/// ClassCastException in getDTMHandleFromNode because SAXImpl is
		/// not an instance of DOM2DTM. So we use the more lengthy
		/// implementation below until this issue has been addressed.
		/// </summary>
		/// <seealso cref="org.apache.xml.dtm.ref.DTMManagerDefault.getDTMHandleFromNode"/>
		private static DTMAxisIterator nodeList2IteratorUsingHandleFromNode(NodeList nodeList, Translet translet, DOM dom)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = nodeList.getLength();
		int n = nodeList.getLength();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] dtmHandles = new int[n];
		int[] dtmHandles = new int[n];
		DTMManager dtmManager = null;
		if (dom is MultiDOM)
		{
			dtmManager = ((MultiDOM) dom).DTMManager;
		}
		for (int i = 0; i < n; ++i)
		{
			org.w3c.dom.Node node = nodeList.item(i);
			int handle;
			if (dtmManager != null)
			{
			handle = dtmManager.getDTMHandleFromNode(node);
			}
			else if (node is DTMNodeProxy && ((DTMNodeProxy) node).DTM == dom)
			{
			handle = ((DTMNodeProxy) node).DTMNodeNumber;
			}
			else
			{
			runTimeError(RUN_TIME_INTERNAL_ERR, "need MultiDOM");
			return null;
			}
			dtmHandles[i] = handle;
			Console.WriteLine("Node " + i + " has handle 0x" + Convert.ToString(handle, 16));
		}
		return new ArrayNodeListIterator(dtmHandles);
		}

		/// <summary>
		/// Utility function used to convert a w3c NodeList into a internal
		/// DOM iterator. 
		/// </summary>
		public static DTMAxisIterator nodeList2Iterator(NodeList nodeList, Translet translet, DOM dom)
		{
		// First pass: build w3c DOM for all nodes not proxied from our DOM.
		//
		// Notice: this looses some (esp. parent) context for these nodes,
		// so some way to wrap the original nodes inside a DTMAxisIterator
		// might be preferable in the long run.
		int n = 0; // allow for change in list length, just in case.
		Document doc = null;
		DTMManager dtmManager = null;
		int[] proxyNodes = new int[nodeList.getLength()];
		if (dom is MultiDOM)
		{
			dtmManager = ((MultiDOM) dom).DTMManager;
		}
		for (int i = 0; i < nodeList.getLength(); ++i)
		{
			org.w3c.dom.Node node = nodeList.item(i);
			if (node is DTMNodeProxy)
			{
			DTMNodeProxy proxy = (DTMNodeProxy)node;
			DTM nodeDTM = proxy.DTM;
			int handle = proxy.DTMNodeNumber;
			bool isOurDOM = (nodeDTM == dom);
			if (!isOurDOM && dtmManager != null)
			{
				try
				{
				isOurDOM = (nodeDTM == dtmManager.getDTM(handle));
				}
				catch (System.IndexOutOfRangeException)
				{
				// invalid node handle, so definitely not our doc
				}
			}
			if (isOurDOM)
			{
				proxyNodes[i] = handle;
				++n;
				continue;
			}
			}
			proxyNodes[i] = DTM.NULL;
			int nodeType = node.getNodeType();
			if (doc == null)
			{
			if (dom is MultiDOM == false)
			{
				runTimeError(RUN_TIME_INTERNAL_ERR, "need MultiDOM");
				return null;
			}
			try
			{
				AbstractTranslet at = (AbstractTranslet) translet;
				doc = at.newDocument("", "__top__");
			}
			catch (javax.xml.parsers.ParserConfigurationException e)
			{
				runTimeError(RUN_TIME_INTERNAL_ERR, e.Message);
				return null;
			}
			}
			// Use one dummy element as container for each node of the
			// list. That way, it is easier to detect resp. avoid
			// funny things which change the number of nodes,
			// e.g. auto-concatenation of text nodes.
			Element mid;
			switch (nodeType)
			{
			case org.w3c.dom.Node.ELEMENT_NODE:
			case org.w3c.dom.Node.TEXT_NODE:
			case org.w3c.dom.Node.CDATA_SECTION_NODE:
			case org.w3c.dom.Node.COMMENT_NODE:
			case org.w3c.dom.Node.ENTITY_REFERENCE_NODE:
			case org.w3c.dom.Node.PROCESSING_INSTRUCTION_NODE:
				mid = doc.createElementNS(null, "__dummy__");
				mid.appendChild(doc.importNode(node, true));
				doc.getDocumentElement().appendChild(mid);
				++n;
				break;
			case org.w3c.dom.Node.ATTRIBUTE_NODE:
				// The mid element also serves as a container for
				// attributes, avoiding problems with conflicting
				// attributes or node order.
				mid = doc.createElementNS(null, "__dummy__");
				mid.setAttributeNodeNS((Attr)doc.importNode(node, true));
				doc.getDocumentElement().appendChild(mid);
				++n;
				break;
			default:
				// Better play it safe for all types we aren't sure we know
				// how to deal with.
				runTimeError(RUN_TIME_INTERNAL_ERR, "Don't know how to convert node type " + nodeType);
			break;
			}
		}

			// w3cDOM -> DTM -> DOMImpl
		DTMAxisIterator iter = null, childIter = null, attrIter = null;
		if (doc != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.dom.MultiDOM multiDOM = (org.apache.xalan.xsltc.dom.MultiDOM) dom;
			MultiDOM multiDOM = (MultiDOM) dom;
			DOM idom = (DOM)dtmManager.getDTM(new DOMSource(doc), false, null, true, false);
			// Create DOMAdapter and register with MultiDOM
			DOMAdapter domAdapter = new DOMAdapter(idom, translet.NamesArray, translet.UrisArray, translet.TypesArray, translet.NamespaceArray);
				multiDOM.addDOMAdapter(domAdapter);

			DTMAxisIterator iter1 = idom.getAxisIterator(Axis.CHILD);
			DTMAxisIterator iter2 = idom.getAxisIterator(Axis.CHILD);
				iter = new AbsoluteIterator(new StepIterator(iter1, iter2));

			 iter.StartNode = DTMDefaultBase.ROOTNODE;

			childIter = idom.getAxisIterator(Axis.CHILD);
			attrIter = idom.getAxisIterator(Axis.ATTRIBUTE);
		}

		// Second pass: find DTM handles for every node in the list.
		int[] dtmHandles = new int[n];
		n = 0;
		for (int i = 0; i < nodeList.getLength(); ++i)
		{
			if (proxyNodes[i] != DTM.NULL)
			{
			dtmHandles[n++] = proxyNodes[i];
			continue;
			}
			org.w3c.dom.Node node = nodeList.item(i);
			DTMAxisIterator iter3 = null;
			int nodeType = node.getNodeType();
			switch (nodeType)
			{
			case org.w3c.dom.Node.ELEMENT_NODE:
			case org.w3c.dom.Node.TEXT_NODE:
			case org.w3c.dom.Node.CDATA_SECTION_NODE:
			case org.w3c.dom.Node.COMMENT_NODE:
			case org.w3c.dom.Node.ENTITY_REFERENCE_NODE:
			case org.w3c.dom.Node.PROCESSING_INSTRUCTION_NODE:
				iter3 = childIter;
				break;
			case org.w3c.dom.Node.ATTRIBUTE_NODE:
				iter3 = attrIter;
				break;
			default:
				// Should not happen, as first run should have got all these
				throw new InternalRuntimeError("Mismatched cases");
			}
			if (iter3 != null)
			{
			iter3.StartNode = iter.next();
			dtmHandles[n] = iter3.next();
			// For now, play it self and perform extra checks:
			if (dtmHandles[n] == DTMAxisIterator.END)
			{
				throw new InternalRuntimeError("Expected element missing at " + i);
			}
			if (iter3.next() != DTMAxisIterator.END)
			{
				throw new InternalRuntimeError("Too many elements at " + i);
			}
			++n;
			}
		}
		if (n != dtmHandles.Length)
		{
			throw new InternalRuntimeError("Nodes lost in second pass");
		}

		return new ArrayNodeListIterator(dtmHandles);
		}

		/// <summary>
		/// Utility function used to convert references to DOMs. 
		/// </summary>
		public static DOM referenceToResultTree(object obj)
		{
		try
		{
			return ((DOM) obj);
		}
		catch (System.ArgumentException)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = obj.getClass().getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = obj.GetType().FullName;
			runTimeError(DATA_CONVERSION_ERR, "reference", className);
			return null;
		}
		}

		/// <summary>
		/// Utility function: used with nth position filters to convert a sequence
		/// of nodes to just one single node (the one at position n).
		/// </summary>
		public static DTMAxisIterator getSingleNode(DTMAxisIterator iterator)
		{
		int node = iterator.next();
		return (new SingletonIterator(node));
		}

		/// <summary>
		/// Utility function: used in xsl:copy.
		/// </summary>
		private static char[] _characterArray = new char[32];

		public static void copy(object obj, SerializationHandler handler, int node, DOM dom)
		{
		try
		{
			if (obj is DTMAxisIterator)
			{
			DTMAxisIterator iter = (DTMAxisIterator) obj;
			dom.copy(iter.reset(), handler);
			}
			else if (obj is Node)
			{
			dom.copy(((Node) obj).node, handler);
			}
			else if (obj is DOM)
			{
			//((DOM)obj).copy(((org.apache.xml.dtm.ref.DTMDefaultBase)((DOMAdapter)obj).getDOMImpl()).getDocument(), handler);
			DOM newDom = (DOM)obj;
			newDom.copy(newDom.Document, handler);
			}
			else
			{
			string @string = obj.ToString(); // or call stringF()
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = string.length();
			int length = @string.Length;
			if (length > _characterArray.Length)
			{
				_characterArray = new char[length];
			}
			@string.CopyTo(0, _characterArray, 0, length - 0);
			handler.characters(_characterArray, 0, length);
			}
		}
		catch (SAXException)
		{
			runTimeError(RUN_TIME_COPY_ERR);
		}
		}

		/// <summary>
		/// Utility function to check if xsl:attribute has a valid qname
		/// This method should only be invoked if the name attribute is an AVT
		/// </summary>
		public static void checkAttribQName(string name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstOccur = name.indexOf(':');
			int firstOccur = name.IndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastOccur = name.lastIndexOf(':');
			int lastOccur = name.LastIndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = name.substring(lastOccur + 1);
			string localName = name.Substring(lastOccur + 1);

			if (firstOccur > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String newPrefix = name.substring(0, firstOccur);
				string newPrefix = name.Substring(0, firstOccur);

				if (firstOccur != lastOccur)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String oriPrefix = name.substring(firstOccur+1, lastOccur);
				   string oriPrefix = name.Substring(firstOccur + 1, lastOccur - (firstOccur + 1));
					if (!XML11Char.isXML11ValidNCName(oriPrefix))
					{
						// even though the orignal prefix is ignored, it should still get checked for valid NCName
						runTimeError(INVALID_QNAME_ERR,oriPrefix + ":" + localName);
					}
				}

				// prefix must be a valid NCName
				if (!XML11Char.isXML11ValidNCName(newPrefix))
				{
					runTimeError(INVALID_QNAME_ERR,newPrefix + ":" + localName);
				}
			}

			// local name must be a valid NCName and must not be XMLNS
			if ((!XML11Char.isXML11ValidNCName(localName)) || (localName.Equals(Constants.XMLNS_PREFIX)))
			{
				runTimeError(INVALID_QNAME_ERR,localName);
			}
		}

		/// <summary>
		/// Utility function to check if a name is a valid ncname
		/// This method should only be invoked if the attribute value is an AVT
		/// </summary>
		public static void checkNCName(string name)
		{
			if (!XML11Char.isXML11ValidNCName(name))
			{
				runTimeError(INVALID_NCNAME_ERR,name);
			}
		}

		/// <summary>
		/// Utility function to check if a name is a valid qname
		/// This method should only be invoked if the attribute value is an AVT
		/// </summary>
		public static void checkQName(string name)
		{
			if (!XML11Char.isXML11ValidQName(name))
			{
				runTimeError(INVALID_QNAME_ERR,name);
			}
		}

		/// <summary>
		/// Utility function for the implementation of xsl:element.
		/// </summary>
		public static string startXslElement(string qname, string @namespace, SerializationHandler handler, DOM dom, int node)
		{
			try
			{
				// Get prefix from qname
				string prefix;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = qname.indexOf(':');
				int index = qname.IndexOf(':');

				if (index > 0)
				{
					prefix = qname.Substring(0, index);

					// Handle case when prefix is not known at compile time
					if (string.ReferenceEquals(@namespace, null) || @namespace.Length == 0)
					{
						runTimeError(NAMESPACE_PREFIX_ERR,prefix);
					}

					handler.startElement(@namespace, qname.Substring(index + 1), qname);
					handler.namespaceAfterStartElement(prefix, @namespace);
				}
				else
				{
					// Need to generate a prefix?
					if (!string.ReferenceEquals(@namespace, null) && @namespace.Length > 0)
					{
						prefix = generatePrefix();
						qname = prefix + ':' + qname;
						handler.startElement(@namespace, qname, qname);
						handler.namespaceAfterStartElement(prefix, @namespace);
					}
					else
					{
						handler.startElement(null, null, qname);
					}
				}
			}
			catch (SAXException e)
			{
				throw new Exception(e.Message);
			}

			return qname;
		}

		/// <summary>
		/// <para>Look up the namespace for a lexical QName using the namespace
		/// declarations available at a particular location in the stylesheet.</para>
		/// <para>See <seealso cref="org.apache.xalan.xsltc.compiler.Stylesheet.compileStaticInitializer(org.apache.xalan.xsltc.compiler.util.ClassGenerator)"/>
		/// for more information about the <code>ancestorNodeIDs</code>,
		/// <code>prefixURIsIndex</code> and <code>prefixURIPairs</code arrays.</para>
		/// </summary>
		/// <param name="lexicalQName"> The QName as a <code>java.lang.String</code> </param>
		/// <param name="stylesheetNodeID"> An <code>int</code> representing the element in
		///                     the stylesheet relative to which the namespace of
		///                     the lexical QName is to be determined </param>
		/// <param name="ancestorNodeIDs"> An <code>int</code> array, indexed by stylesheet
		///                     node IDs, containing the ID of the nearest ancestor
		///                     node in the stylesheet that has namespace
		///                     declarations, or <code>-1</code> if there is no
		///                     such ancestor </param>
		/// <param name="prefixURIsIndex"> An <code>int</code> array, indexed by stylesheet
		///                     node IDs, containing the index into the
		///                     <code>prefixURIPairs</code> array of the first
		///                     prefix declared on that stylesheet node </param>
		/// <param name="prefixURIPairs"> A <code>java.lang.String</code> array that contains
		///                     pairs of </param>
		/// <param name="ignoreDefault"> A <code>boolean</code> indicating whether any
		///                     default namespace decarlation should be considered </param>
		/// <returns> The namespace of the lexical QName or a zero-length string if
		///         the QName is in no namespace or no namespace declaration for the
		///         prefix of the QName was found </returns>
		public static string lookupStylesheetQNameNamespace(string lexicalQName, int stylesheetNodeID, int[] ancestorNodeIDs, int[] prefixURIsIndex, string[] prefixURIPairs, bool ignoreDefault)
		{
			string prefix = getPrefix(lexicalQName);
			string uri = "";

			if (string.ReferenceEquals(prefix, null) && !ignoreDefault)
			{
				prefix = "";
			}

			if (!string.ReferenceEquals(prefix, null))
			{
				// Loop from current node in the stylesheet to its ancestors
				for (int currentNodeID = stylesheetNodeID; currentNodeID >= 0; currentNodeID = ancestorNodeIDs[currentNodeID])
				{
					// Look at all declarations on the current stylesheet node
					// The prefixURIsIndex is an array of indices into the
					// prefixURIPairs array that are stored in ascending order.
					// The declarations for a node I are in elements
					// prefixURIsIndex[I] to prefixURIsIndex[I+1]-1 (or 
					// prefixURIPairs.length-1 if I is the last node)
					int prefixStartIdx = prefixURIsIndex[currentNodeID];
					int prefixLimitIdx = (currentNodeID + 1 < prefixURIsIndex.Length) ? prefixURIsIndex[currentNodeID + 1] : prefixURIPairs.Length;

					for (int prefixIdx = prefixStartIdx; prefixIdx < prefixLimitIdx; prefixIdx = prefixIdx + 2)
					{
						// Did we find the declaration of our prefix
						if (prefix.Equals(prefixURIPairs[prefixIdx]))
						{
							uri = prefixURIPairs[prefixIdx + 1];
							goto nodeLoopBreak;
						}
					}
					nodeLoopContinue:;
				}
				nodeLoopBreak:;
			}

			return uri;
		}

		/// <summary>
		/// <para>Look up the namespace for a lexical QName using the namespace
		/// declarations available at a particular location in the stylesheet and
		/// return the expanded QName</para>
		/// <para>See <seealso cref="org.apache.xalan.xsltc.compiler.Stylesheet.compileStaticInitializer(org.apache.xalan.xsltc.compiler.util.ClassGenerator)"/>
		/// for more information about the <code>ancestorNodeIDs</code>,
		/// <code>prefixURIsIndex</code> and <code>prefixURIPairs</code arrays.</para>
		/// </summary>
		/// <param name="lexicalQName"> The QName as a <code>java.lang.String</code> </param>
		/// <param name="stylesheetNodeID"> An <code>int</code> representing the element in
		///                     the stylesheet relative to which the namespace of
		///                     the lexical QName is to be determined </param>
		/// <param name="ancestorNodeIDs"> An <code>int</code> array, indexed by stylesheet
		///                     node IDs, containing the ID of the nearest ancestor
		///                     node in the stylesheet that has namespace
		///                     declarations, or <code>-1</code> if there is no
		///                     such ancestor </param>
		/// <param name="prefixURIsIndex"> An <code>int</code> array, indexed by stylesheet
		///                     node IDs, containing the index into the
		///                     <code>prefixURIPairs</code> array of the first
		///                     prefix declared on that stylesheet node </param>
		/// <param name="prefixURIPairs"> A <code>java.lang.String</code> array that contains
		///                     pairs of </param>
		/// <param name="ignoreDefault"> A <code>boolean</code> indicating whether any
		///                     default namespace decarlation should be considered </param>
		/// <returns> The expanded QName in the form "uri:localName" or just
		///         "localName" if the QName is in no namespace or no namespace
		///         declaration for the prefix of the QName was found </returns>
		public static string expandStylesheetQNameRef(string lexicalQName, int stylesheetNodeID, int[] ancestorNodeIDs, int[] prefixURIsIndex, string[] prefixURIPairs, bool ignoreDefault)
		{
			string expandedQName;
			string prefix = getPrefix(lexicalQName);
			string localName = (!string.ReferenceEquals(prefix, null)) ? lexicalQName.Substring(prefix.Length + 1) : lexicalQName;
			string uri = lookupStylesheetQNameNamespace(lexicalQName, stylesheetNodeID, ancestorNodeIDs, prefixURIsIndex, prefixURIPairs, ignoreDefault);

			// Handle case when prefix is not resolved
			if (!string.ReferenceEquals(prefix, null) && prefix.Length != 0 && (string.ReferenceEquals(uri, null) || uri.Length == 0))
			{
				runTimeError(NAMESPACE_PREFIX_ERR, prefix);
			}

			if (uri.Length == 0)
			{
				expandedQName = localName;
			}
			else
			{
				expandedQName = uri + ':' + localName;
			}

			return expandedQName;
		}

		/// <summary>
		/// This function is used in the execution of xsl:element
		/// </summary>
		public static string getPrefix(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = qname.indexOf(':');
		int index = qname.IndexOf(':');
		return (index > 0) ? qname.Substring(0, index) : null;
		}

		/// <summary>
		/// This function is used in the execution of xsl:element
		/// </summary>
		private static int prefixIndex = 0; // not thread safe!!
		public static string generatePrefix()
		{
		return ("ns" + prefixIndex++);
		}

		public const string RUN_TIME_INTERNAL_ERR = "RUN_TIME_INTERNAL_ERR";
		public const string RUN_TIME_COPY_ERR = "RUN_TIME_COPY_ERR";
		public const string DATA_CONVERSION_ERR = "DATA_CONVERSION_ERR";
		public const string EXTERNAL_FUNC_ERR = "EXTERNAL_FUNC_ERR";
		public const string EQUALITY_EXPR_ERR = "EQUALITY_EXPR_ERR";
		public const string INVALID_ARGUMENT_ERR = "INVALID_ARGUMENT_ERR";
		public const string FORMAT_NUMBER_ERR = "FORMAT_NUMBER_ERR";
		public const string ITERATOR_CLONE_ERR = "ITERATOR_CLONE_ERR";
		public const string AXIS_SUPPORT_ERR = "AXIS_SUPPORT_ERR";
		public const string TYPED_AXIS_SUPPORT_ERR = "TYPED_AXIS_SUPPORT_ERR";
		public const string STRAY_ATTRIBUTE_ERR = "STRAY_ATTRIBUTE_ERR";
		public const string STRAY_NAMESPACE_ERR = "STRAY_NAMESPACE_ERR";
		public const string NAMESPACE_PREFIX_ERR = "NAMESPACE_PREFIX_ERR";
		public const string DOM_ADAPTER_INIT_ERR = "DOM_ADAPTER_INIT_ERR";
		public const string PARSER_DTD_SUPPORT_ERR = "PARSER_DTD_SUPPORT_ERR";
		public const string NAMESPACES_SUPPORT_ERR = "NAMESPACES_SUPPORT_ERR";
		public const string CANT_RESOLVE_RELATIVE_URI_ERR = "CANT_RESOLVE_RELATIVE_URI_ERR";
		public const string UNSUPPORTED_XSL_ERR = "UNSUPPORTED_XSL_ERR";
		public const string UNSUPPORTED_EXT_ERR = "UNSUPPORTED_EXT_ERR";
		public const string UNKNOWN_TRANSLET_VERSION_ERR = "UNKNOWN_TRANSLET_VERSION_ERR";
		public const string INVALID_QNAME_ERR = "INVALID_QNAME_ERR";
		public const string INVALID_NCNAME_ERR = "INVALID_NCNAME_ERR";
		public const string UNALLOWED_EXTENSION_FUNCTION_ERR = "UNALLOWED_EXTENSION_FUNCTION_ERR";
		public const string UNALLOWED_EXTENSION_ELEMENT_ERR = "UNALLOWED_EXTENSION_ELEMENT_ERR";

		// All error messages are localized and are stored in resource bundles.
		private static ResourceBundle m_bundle;

		public const string ERROR_MESSAGES_KEY = "error-messages";


		/// <summary>
		/// Print a run-time error message.
		/// </summary>
		public static void runTimeError(string code)
		{
		throw new Exception(m_bundle.getString(code));
		}

		public static void runTimeError(string code, object[] args)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String message = java.text.MessageFormat.format(m_bundle.getString(code), args);
		string message = MessageFormat.format(m_bundle.getString(code), args);
		throw new Exception(message);
		}

		public static void runTimeError(string code, object arg0)
		{
		runTimeError(code, new object[]{arg0});
		}

		public static void runTimeError(string code, object arg0, object arg1)
		{
		runTimeError(code, new object[]{arg0, arg1});
		}

		public static void consoleOutput(string msg)
		{
		Console.WriteLine(msg);
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
		/// Utility method to allow setting parameters of the form
		/// {namespaceuri}localName
		/// which get mapped to an instance variable in the class
		/// Hence  a parameter of the form "{http://foo.bar}xyz"
		/// will be replaced with the corresponding values  
		/// by the BasisLibrary's utility method mapQNametoJavaName
		/// and thus get mapped to legal java variable names 
		/// </summary>
		public static string mapQNameToJavaName(string @base)
		{
		   return replace(@base, ".-:/{}?#%*", new string[] {"$dot$", "$dash$", "$colon$", "$slash$", "", "$colon$", "$ques$", "$hash$", "$per$", "$aster$"});

		}

		//-- End utility functions
	}

}