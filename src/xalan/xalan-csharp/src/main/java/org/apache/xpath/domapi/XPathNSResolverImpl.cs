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
 * $Id: XPathNSResolverImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xpath.domapi
{
	using PrefixResolverDefault = org.apache.xml.utils.PrefixResolverDefault;
	using Node = org.w3c.dom.Node;
	using XPathNSResolver = org.w3c.dom.xpath.XPathNSResolver;

	/// 
	/// <summary>
	/// The class provides an implementation XPathNSResolver according 
	/// to the DOM L3 XPath Specification, Working Group Note 26 February 2004.
	/// 
	/// <para>See also the <a href='http://www.w3.org/TR/2004/NOTE-DOM-Level-3-XPath-20040226'>Document Object Model (DOM) Level 3 XPath Specification</a>.</para>
	/// 
	/// <para>The <code>XPathNSResolver</code> interface permit <code>prefix</code> 
	/// strings in the expression to be properly bound to 
	/// <code>namespaceURI</code> strings. <code>XPathEvaluator</code> can 
	/// construct an implementation of <code>XPathNSResolver</code> from a node, 
	/// or the interface may be implemented by any application.</para>
	/// </summary>
	/// <seealso cref="org.w3c.dom.xpath.XPathNSResolver"
	/// @xsl.usage internal/>
	internal class XPathNSResolverImpl : PrefixResolverDefault, XPathNSResolver
	{

		/// <summary>
		/// Constructor for XPathNSResolverImpl. </summary>
		/// <param name="xpathExpressionContext"> </param>
		public XPathNSResolverImpl(Node xpathExpressionContext) : base(xpathExpressionContext)
		{
		}

		/// <seealso cref="org.w3c.dom.xpath.XPathNSResolver.lookupNamespaceURI(String)"/>
		public virtual string lookupNamespaceURI(string prefix)
		{
			return base.getNamespaceForPrefix(prefix);
		}

	}

}