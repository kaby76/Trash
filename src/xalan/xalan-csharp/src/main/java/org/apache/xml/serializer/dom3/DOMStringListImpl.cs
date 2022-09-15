using System.Collections;

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
 * $Id: DOMStringListImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */
namespace org.apache.xml.serializer.dom3
{

	//import org.apache.xerces.dom3.DOMStringList;
	using DOMStringList = org.w3c.dom.DOMStringList;

	/// <summary>
	/// This class implemets the DOM Level 3 Core interface DOMStringList.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal sealed class DOMStringListImpl : DOMStringList
	{

		//A collection of DOMString values
		private ArrayList fStrings;

		/// <summary>
		/// Construct an empty list of DOMStringListImpl
		/// </summary>
		internal DOMStringListImpl()
		{
			fStrings = new ArrayList();
		}

		/// <summary>
		/// Construct an empty list of DOMStringListImpl
		/// </summary>
		internal DOMStringListImpl(ArrayList @params)
		{
			fStrings = @params;
		}

		/// <summary>
		/// Construct an empty list of DOMStringListImpl
		/// </summary>
		internal DOMStringListImpl(string[] @params)
		{
			fStrings = new ArrayList();
			if (@params != null)
			{
				for (int i = 0; i < @params.Length; i++)
				{
					fStrings.Add(@params[i]);
				}
			}
		}

		/// <seealso cref="org.apache.xerces.dom3.DOMStringList.item(int)"/>
		public string item(int index)
		{
			try
			{
				return (string) fStrings[index];
			}
			catch (System.IndexOutOfRangeException)
			{
				return null;
			}
		}

		/// <seealso cref="org.apache.xerces.dom3.DOMStringList.getLength()"/>
		public int Length
		{
			get
			{
				return fStrings.Count;
			}
		}

		/// <seealso cref="org.apache.xerces.dom3.DOMStringList.contains(String)"/>
		public bool contains(string param)
		{
			return fStrings.Contains(param);
		}

		/// <summary>
		/// DOM Internal:
		/// Add a <code>DOMString</code> to the list.
		/// </summary>
		/// <param name="domString"> A string to add to the list </param>
		public void add(string param)
		{
			fStrings.Add(param);
		}

	}

}