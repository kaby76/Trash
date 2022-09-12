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
 * $Id: PrefixResolverDefault.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This class implements a generic PrefixResolver that
	/// can be used to perform prefix-to-namespace lookup
	/// for the XPath object.
	/// @xsl.usage general
	/// </summary>
	public class PrefixResolverDefault : PrefixResolver
	{

	  /// <summary>
	  /// The context to resolve the prefix from, if the context
	  /// is not given.
	  /// </summary>
	  internal Node m_context;

	  /// <summary>
	  /// Construct a PrefixResolverDefault object. </summary>
	  /// <param name="xpathExpressionContext"> The context from
	  /// which XPath expression prefixes will be resolved.
	  /// Warning: This will not work correctly if xpathExpressionContext
	  /// is an attribute node. </param>
	  public PrefixResolverDefault(Node xpathExpressionContext)
	  {
		m_context = xpathExpressionContext;
	  }

	  /// <summary>
	  /// Given a namespace, get the corrisponding prefix.  This assumes that
	  /// the PrevixResolver hold's it's own namespace context, or is a namespace
	  /// context itself. </summary>
	  /// <param name="prefix"> Prefix to resolve. </param>
	  /// <returns> Namespace that prefix resolves to, or null if prefix
	  /// is not bound. </returns>
	  public virtual string getNamespaceForPrefix(string prefix)
	  {
		return getNamespaceForPrefix(prefix, m_context);
	  }

	  /// <summary>
	  /// Given a namespace, get the corrisponding prefix.
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
		  @namespace = Constants.S_XMLNAMESPACEURI;
		}
		else
		{
		  int type;

		  while ((null != parent) && (null == @namespace) && (((type = parent.NodeType) == Node.ELEMENT_NODE) || (type == Node.ENTITY_REFERENCE_NODE)))
		  {
			if (type == Node.ELEMENT_NODE)
			{
					if (parent.NodeName.IndexOf(prefix + ":") == 0)
					{
							return parent.NamespaceURI;
					}
			  NamedNodeMap nnm = parent.Attributes;

			  for (int i = 0; i < nnm.Length; i++)
			  {
				Node attr = nnm.item(i);
				string aname = attr.NodeName;
				bool isPrefix = aname.StartsWith("xmlns:", StringComparison.Ordinal);

				if (isPrefix || aname.Equals("xmlns"))
				{
				  int index = aname.IndexOf(':');
				  string p = isPrefix ? aname.Substring(index + 1) : "";

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
		/// <seealso cref= PrefixResolver#handlesNullPrefixes() </seealso>
		public virtual bool handlesNullPrefixes()
		{
			return false;
		}

	}

}