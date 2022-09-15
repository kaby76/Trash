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
// $Id: JAXPPrefixResolver.java 468655 2006-10-28 07:12:06Z minchau $

namespace org.apache.xpath.jaxp
{

	using Node = org.w3c.dom.Node;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;

	/// <summary>
	/// <meta name="usage" content="general"/>
	/// This class implements a Default PrefixResolver which
	/// can be used to perform prefix-to-namespace lookup
	/// for the XPath object.
	/// This class delegates the resolution to the passed NamespaceContext
	/// </summary>
	public class JAXPPrefixResolver : PrefixResolver
	{

		private NamespaceContext namespaceContext;


		public JAXPPrefixResolver(NamespaceContext nsContext)
		{
			this.namespaceContext = nsContext;
		}


		public virtual string getNamespaceForPrefix(string prefix)
		{
			return namespaceContext.getNamespaceURI(prefix);
		}

		/// <summary>
		/// Return the base identifier.
		/// </summary>
		/// <returns> null </returns>
		public virtual string BaseIdentifier
		{
			get
			{
				return null;
			}
		}

		/// <seealso cref= PrefixResolver#handlesNullPrefixes()  </seealso>
		public virtual bool handlesNullPrefixes()
		{
			return false;
		}


		/// <summary>
		/// The URI for the XML namespace.
		/// (Duplicate of that found in org.apache.xpath.XPathContext). 
		/// </summary>

		public const string S_XMLNAMESPACEURI = "http://www.w3.org/XML/1998/namespace";


		/// <summary>
		/// Given a prefix and a Context Node, get the corresponding namespace.
		/// Warning: This will not work correctly if namespaceContext
		/// is an attribute node. </summary>
		/// <param name="prefix"> Prefix to resolve. </param>
		/// <param name="namespaceContext"> Node from which to start searching for a
		/// xmlns attribute that binds a prefix to a namespace. </param>
		/// <returns> Namespace that prefix resolves to, or null if prefix
		/// is not bound. </returns>
		public virtual string getNamespaceForPrefix(string prefix, Node namespaceContext)
		{
			Node parent = namespaceContext;
			string @namespace = null;

			if (prefix.Equals("xml"))
			{
				@namespace = S_XMLNAMESPACEURI;
			}
			else
			{
				int type;

				while ((null != parent) && (null == @namespace) && (((type = parent.NodeType) == Node.ELEMENT_NODE) || (type == Node.ENTITY_REFERENCE_NODE)))
				{

					if (type == Node.ELEMENT_NODE)
					{
						NamedNodeMap nnm = parent.Attributes;

						for (int i = 0; i < nnm.Length; i++)
						{
							Node attr = nnm.item(i);
							string aname = attr.NodeName;
							bool isPrefix = aname.StartsWith("xmlns:", StringComparison.Ordinal);

							if (isPrefix || aname.Equals("xmlns"))
							{
								int index = aname.IndexOf(':');
								string p = isPrefix ?aname.Substring(index + 1) :"";

								if (p.Equals(prefix))
								{
									@namespace = attr.NodeValue;
									break;
								}
							}
						}
					}

					parent = parent.ParentNode;
				}
			}
			return @namespace;
		}

	}


}