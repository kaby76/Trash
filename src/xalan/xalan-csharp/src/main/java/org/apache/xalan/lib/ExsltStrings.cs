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
 * $Id: ExsltStrings.java 1225758 2011-12-30 05:44:27Z mrglavas $
 */
namespace org.apache.xalan.lib
{


	using NodeSet = org.apache.xpath.NodeSet;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Text = org.w3c.dom.Text;

	/// <summary>
	/// This class contains EXSLT strings extension functions.
	/// 
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:str="http://exslt.org/strings"
	/// </pre>
	/// The documentation for each function has been copied from the relevant
	/// EXSLT Implementer page.
	/// </summary>
	/// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"
	/// 
	/// @xsl.usage general/>
	public class ExsltStrings : ExsltBase
	{
	  /// <summary>
	  /// The str:align function aligns a string within another string. 
	  /// <para>
	  /// The first argument gives the target string to be aligned. The second argument gives 
	  /// the padding string within which it is to be aligned. 
	  /// </para>
	  /// <para>
	  /// If the target string is shorter than the padding string then a range of characters 
	  /// in the padding string are repaced with those in the target string. Which characters 
	  /// are replaced depends on the value of the third argument, which gives the type of 
	  /// alignment. It can be one of 'left', 'right' or 'center'. If no third argument is 
	  /// given or if it is not one of these values, then it defaults to left alignment. 
	  /// </para>
	  /// <para>
	  /// With left alignment, the range of characters replaced by the target string begins 
	  /// with the first character in the padding string. With right alignment, the range of 
	  /// characters replaced by the target string ends with the last character in the padding 
	  /// string. With center alignment, the range of characters replaced by the target string 
	  /// is in the middle of the padding string, such that either the number of unreplaced 
	  /// characters on either side of the range is the same or there is one less on the left 
	  /// than there is on the right. 
	  /// </para>
	  /// <para>
	  /// If the target string is longer than the padding string, then it is truncated to be 
	  /// the same length as the padding string and returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="targetStr"> The target string </param>
	  /// <param name="paddingStr"> The padding string </param>
	  /// <param name="type"> The type of alignment
	  /// </param>
	  /// <returns> The string after alignment </returns>
	  public static string align(string targetStr, string paddingStr, string type)
	  {
		if (targetStr.Length >= paddingStr.Length)
		{
		  return targetStr.Substring(0, paddingStr.Length);
		}

		if (type.Equals("right"))
		{
		  return paddingStr.Substring(0, paddingStr.Length - targetStr.Length) + targetStr;
		}
		else if (type.Equals("center"))
		{
		  int startIndex = (paddingStr.Length - targetStr.Length) / 2;
		  return paddingStr.Substring(0, startIndex) + targetStr + paddingStr.Substring(startIndex + targetStr.Length);
		}
		// Default is left
		else
		{
		  return targetStr + paddingStr.Substring(targetStr.Length);
		}
	  }

	  /// <summary>
	  /// See above
	  /// </summary>
	  public static string align(string targetStr, string paddingStr)
	  {
		return align(targetStr, paddingStr, "left");
	  }

	  /// <summary>
	  /// The str:concat function takes a node set and returns the concatenation of the 
	  /// string values of the nodes in that node set. If the node set is empty, it returns 
	  /// an empty string.
	  /// </summary>
	  /// <param name="nl"> A node set </param>
	  /// <returns> The concatenation of the string values of the nodes in that node set </returns>
	  public static string concat(NodeList nl)
	  {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node node = nl.item(i);
		  string value = toString(node);

		  if (!string.ReferenceEquals(value, null) && value.Length > 0)
		  {
			sb.Append(value);
		  }
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// The str:padding function creates a padding string of a certain length. 
	  /// The first argument gives the length of the padding string to be created. 
	  /// The second argument gives a string to be used to create the padding. This 
	  /// string is repeated as many times as is necessary to create a string of the 
	  /// length specified by the first argument; if the string is more than a character 
	  /// long, it may have to be truncated to produce the required length. If no second 
	  /// argument is specified, it defaults to a space (' '). If the second argument is 
	  /// an empty string, str:padding returns an empty string.
	  /// </summary>
	  /// <param name="length"> The length of the padding string to be created </param>
	  /// <param name="pattern"> The string to be used as pattern
	  /// </param>
	  /// <returns> A padding string of the given length </returns>
	  public static string padding(double length, string pattern)
	  {
		if (string.ReferenceEquals(pattern, null) || pattern.Length == 0)
		{
		  return "";
		}

		StringBuilder sb = new StringBuilder();
		int len = (int)length;
		int numAdded = 0;
		int index = 0;
		while (numAdded < len)
		{
		  if (index == pattern.Length)
		  {
			index = 0;
		  }

		  sb.Append(pattern[index]);
		  index++;
		  numAdded++;
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// See above
	  /// </summary>
	  public static string padding(double length)
	  {
		return padding(length, " ");
	  }

	  /// <summary>
	  /// The str:split function splits up a string and returns a node set of token 
	  /// elements, each containing one token from the string. 
	  /// <para>
	  /// The first argument is the string to be split. The second argument is a pattern 
	  /// string. The string given by the first argument is split at any occurrence of 
	  /// this pattern. For example: 
	  /// <pre>
	  /// str:split('a, simple, list', ', ') gives the node set consisting of: 
	  /// 
	  /// <token>a</token>
	  /// <token>simple</token>
	  /// <token>list</token>
	  /// </pre>
	  /// If the second argument is omitted, the default is the string '&#x20;' (i.e. a space).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="str"> The string to be split </param>
	  /// <param name="pattern"> The pattern
	  /// </param>
	  /// <returns> A node set of split tokens </returns>
	  public static NodeList split(string str, string pattern)
	  {


		NodeSet resultSet = new NodeSet();
		resultSet.ShouldCacheNodes = true;

		bool done = false;
		int fromIndex = 0;
		int matchIndex = 0;
		string token = null;

		while (!done && fromIndex < str.Length)
		{
		  matchIndex = str.IndexOf(pattern, fromIndex, StringComparison.Ordinal);
		  if (matchIndex >= 0)
		  {
		token = str.Substring(fromIndex, matchIndex - fromIndex);
		fromIndex = matchIndex + pattern.Length;
		  }
		  else
		  {
			done = true;
			token = str.Substring(fromIndex);
		  }

		  Document doc = DocumentHolder.m_doc;
		  lock (doc)
		  {
			Element element = doc.createElement("token");
			Text text = doc.createTextNode(token);
			element.appendChild(text);
			resultSet.addNode(element);
		  }
		}

		return resultSet;
	  }

	  /// <summary>
	  /// See above
	  /// </summary>
	  public static NodeList split(string str)
	  {
		return split(str, " ");
	  }

	  /// <summary>
	  /// The str:tokenize function splits up a string and returns a node set of token 
	  /// elements, each containing one token from the string. 
	  /// <para>
	  /// The first argument is the string to be tokenized. The second argument is a 
	  /// string consisting of a number of characters. Each character in this string is 
	  /// taken as a delimiting character. The string given by the first argument is split 
	  /// at any occurrence of any of these characters. For example: 
	  /// <pre>
	  /// str:tokenize('2001-06-03T11:40:23', '-T:') gives the node set consisting of: 
	  /// 
	  /// <token>2001</token>
	  /// <token>06</token>
	  /// <token>03</token>
	  /// <token>11</token>
	  /// <token>40</token>
	  /// <token>23</token>
	  /// </pre>
	  /// If the second argument is omitted, the default is the string '&#x9;&#xA;&#xD;&#x20;' 
	  /// (i.e. whitespace characters). 
	  /// </para>
	  /// <para>
	  /// If the second argument is an empty string, the function returns a set of token 
	  /// elements, each of which holds a single character.
	  /// </para>
	  /// <para>
	  /// Note: This one is different from the tokenize extension function in the Xalan
	  /// namespace. The one in Xalan returns a set of Text nodes, while this one wraps
	  /// the Text nodes inside the token Element nodes.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="toTokenize"> The string to be tokenized </param>
	  /// <param name="delims"> The delimiter string
	  /// </param>
	  /// <returns> A node set of split token elements </returns>
	  public static NodeList tokenize(string toTokenize, string delims)
	  {


		NodeSet resultSet = new NodeSet();

		if (!string.ReferenceEquals(delims, null) && delims.Length > 0)
		{
		  StringTokenizer lTokenizer = new StringTokenizer(toTokenize, delims);

		  Document doc = DocumentHolder.m_doc;
		  lock (doc)
		  {
			while (lTokenizer.hasMoreTokens())
			{
			  Element element = doc.createElement("token");
			  element.appendChild(doc.createTextNode(lTokenizer.nextToken()));
			  resultSet.addNode(element);
			}
		  }
		}
		// If the delimiter is an empty string, create one token Element for 
		// every single character.
		else
		{

		  Document doc = DocumentHolder.m_doc;
		  lock (doc)
		  {
			for (int i = 0; i < toTokenize.Length; i++)
			{
			  Element element = doc.createElement("token");
			  element.appendChild(doc.createTextNode(toTokenize.Substring(i, 1)));
			  resultSet.addNode(element);
			}
		  }
		}

		return resultSet;
	  }

	  /// <summary>
	  /// See above
	  /// </summary>
	  public static NodeList tokenize(string toTokenize)
	  {
		return tokenize(toTokenize, " \t\n\r");
	  }
		/// <summary>
		/// This class is not loaded until first referenced (see Java Language
		/// Specification by Gosling/Joy/Steele, section 12.4.1)
		/// 
		/// The static members are created when this class is first referenced, as a
		/// lazy initialization not needing checking against null or any
		/// synchronization.
		/// 
		/// </summary>
		private class DocumentHolder
		{
			// Reuse the Document object to reduce memory usage.
			internal static readonly Document m_doc;
			static DocumentHolder()
			{
				try
				{
					m_doc = DocumentBuilderFactory.newInstance().newDocumentBuilder().newDocument();
				}

				catch (ParserConfigurationException pce)
				{
					  throw new org.apache.xml.utils.WrappedRuntimeException(pce);
				}

			}
		}

	}

}