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
 * $Id: ExsltBase.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{

	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// The base class for some EXSLT extension classes.
	/// It contains common utility methods to be used by the sub-classes.
	/// </summary>
	public abstract class ExsltBase
	{
	  /// <summary>
	  /// Return the string value of a Node
	  /// </summary>
	  /// <param name="n"> The Node. </param>
	  /// <returns> The string value of the Node </returns>
	  protected internal static string ToString(Node n)
	  {
		if (n is DTMNodeProxy)
		{
		   return ((DTMNodeProxy)n).StringValue;
		}
		else
		{
		  string value = n.NodeValue;
		  if (string.ReferenceEquals(value, null))
		  {
			NodeList nodelist = n.ChildNodes;
			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < nodelist.Length; i++)
			{
			  Node childNode = nodelist.item(i);
			  buf.Append(ToString(childNode));
			}
			return buf.ToString();
		  }
		  else
		  {
			return value;
		  }
		}
	  }

	  /// <summary>
	  /// Convert the string value of a Node to a number.
	  /// Return NaN if the string is not a valid number.
	  /// </summary>
	  /// <param name="n"> The Node. </param>
	  /// <returns> The number value of the Node </returns>
	  protected internal static double toNumber(Node n)
	  {
		double d = 0.0;
		string str = ToString(n);
		try
		{
		  d = Convert.ToDouble(str);
		}
		catch (System.FormatException)
		{
		  d = Double.NaN;
		}
		return d;
	  }
	}

}