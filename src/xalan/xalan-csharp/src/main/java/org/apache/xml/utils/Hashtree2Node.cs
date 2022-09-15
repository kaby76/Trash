using System;
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
 * $Id: Hashtree2Node.java 475902 2006-11-16 20:03:16Z minchau $
 */

namespace org.apache.xml.utils
{

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Simple static utility to convert Hashtable to a Node.  
	/// </summary>
	/// <seealso cref="org.apache.xalan.xslt.EnvironmentCheck"/>
	/// <seealso cref="org.apache.xalan.lib.Extensions"
	/// @author shane_curcuru@us.ibm.com
	/// @version $Id: Hashtree2Node.java 475902 2006-11-16 20:03:16Z minchau $
	/// @xsl.usage general/>
	public abstract class Hashtree2Node
	{

		/// <summary>
		/// Convert a Hashtable into a Node tree.  
		/// 
		/// <para>The hash may have either Hashtables as values (in which 
		/// case we recurse) or other values, in which case we print them 
		/// as &lt;item> elements, with a 'key' attribute with the value 
		/// of the key, and the element contents as the value.</para>
		/// 
		/// <para>If args are null we simply return without doing anything. 
		/// If we encounter an error, we will attempt to add an 'ERROR' 
		/// Element with exception info; if that doesn't work we simply 
		/// return without doing anything else byt printStackTrace().</para>
		/// </summary>
		/// <param name="hash"> to get info from (may have sub-hashtables) </param>
		/// <param name="name"> to use as parent element for appended node
		/// futurework could have namespace and prefix as well </param>
		/// <param name="container"> Node to append our report to </param>
		/// <param name="factory"> Document providing createElement, etc. services </param>
		public static void appendHashToNode(Hashtable hash, string name, Node container, Document factory)
		{
			// Required arguments must not be null
			if ((null == container) || (null == factory) || (null == hash))
			{
				return;
			}

			// name we will provide a default value for
			string elemName = null;
			if ((null == name) || ("".Equals(name)))
			{
				elemName = "appendHashToNode";
			}
			else
			{
				elemName = name;
			}

			try
			{
				Element hashNode = factory.createElement(elemName);
				container.appendChild(hashNode);

				System.Collections.IEnumerator keys = hash.Keys.GetEnumerator();
				System.Collections.IList v = new ArrayList();

				while (keys.MoveNext())
				{
					object key = keys.Current;
					string keyStr = key.ToString();
					object item = hash[key];

					if (item is Hashtable)
					{
						// Ensure a pre-order traversal; add this hashes 
						//  items before recursing to child hashes
						// Save name and hash in two steps
						v.Add(keyStr);
						v.Add((Hashtable) item);
					}
					else
					{
						try
						{
							// Add item to node
							Element node = factory.createElement("item");
							node.setAttribute("key", keyStr);
							node.appendChild(factory.createTextNode((string)item));
							hashNode.appendChild(node);
						}
						catch (Exception e)
						{
							Element node = factory.createElement("item");
							node.setAttribute("key", keyStr);
							node.appendChild(factory.createTextNode("ERROR: Reading " + key + " threw: " + e.ToString()));
							hashNode.appendChild(node);
						}
					}
				}

				// Now go back and do the saved hashes
				System.Collections.IEnumerator it = v.GetEnumerator();
				while (it.MoveNext())
				{
					// Retrieve name and hash in two steps
					string n = (string) it.Current;
					Hashtable h = (Hashtable) it.Current;

					appendHashToNode(h, n, hashNode, factory);
				}
			}
			catch (Exception e2)
			{
				// Ooops, just bail (suggestions for a safe thing 
				//  to do in this case appreciated)
				Console.WriteLine(e2.ToString());
				Console.Write(e2.StackTrace);
			}
		}
	}

}