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
 * $Id: XPathEvaluatorImpl.java 1225443 2011-12-29 05:44:18Z mrglavas $
 */

namespace org.apache.xpath.domapi
{

	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using XPath = org.apache.xpath.XPath;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XPATHMessages = org.apache.xpath.res.XPATHMessages;
	using DOMException = org.w3c.dom.DOMException;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using XPathEvaluator = org.w3c.dom.xpath.XPathEvaluator;
	using XPathException = org.w3c.dom.xpath.XPathException;
	using XPathExpression = org.w3c.dom.xpath.XPathExpression;
	using XPathNSResolver = org.w3c.dom.xpath.XPathNSResolver;

	/// 
	/// <summary>
	/// The class provides an implementation of XPathEvaluator according 
	/// to the DOM L3 XPath Specification, Working Group Note 26 February 2004.
	/// 
	/// <para>See also the <a href='http://www.w3.org/TR/2004/NOTE-DOM-Level-3-XPath-20040226'>Document Object Model (DOM) Level 3 XPath Specification</a>.</para>
	/// 
	/// </p>The evaluation of XPath expressions is provided by 
	/// <code>XPathEvaluator</code>, which will provide evaluation of XPath 1.0 
	/// expressions with no specialized extension functions or variables. It is 
	/// expected that the <code>XPathEvaluator</code> interface will be 
	/// implemented on the same object which implements the <code>Document</code> 
	/// interface in an implementation which supports the XPath DOM module. 
	/// <code>XPathEvaluator</code> implementations may be available from other 
	/// sources that may provide support for special extension functions or 
	/// variables which are not defined in this specification.</p>
	/// </summary>
	/// <seealso cref="org.w3c.dom.xpath.XPathEvaluator"
	/// 
	/// @xsl.usage internal/>
	public sealed class XPathEvaluatorImpl : XPathEvaluator
	{

		/// <summary>
		/// This prefix resolver is created whenever null is passed to the 
		/// evaluate method.  Its purpose is to satisfy the DOM L3 XPath API
		/// requirement that if a null prefix resolver is used, an exception 
		/// should only be thrown when an attempt is made to resolve a prefix.
		/// </summary>
		private class DummyPrefixResolver : PrefixResolver
		{

			/// <summary>
			/// Constructor for DummyPrefixResolver.
			/// </summary>
			internal DummyPrefixResolver()
			{
			}

			/// <exception cref="DOMException">
			///   NAMESPACE_ERR: Always throws this exceptionn
			/// </exception>
			/// <seealso cref="org.apache.xml.utils.PrefixResolver.getNamespaceForPrefix(String, Node)"/>
			public virtual string getNamespaceForPrefix(string prefix, Node context)
			{
				string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_RESOLVER, null);
				throw new DOMException(DOMException.NAMESPACE_ERR, fmsg); // Unable to resolve prefix with null prefix resolver.
			}

			/// <exception cref="DOMException">
			///   NAMESPACE_ERR: Always throws this exceptionn
			/// </exception>
			/// <seealso cref="org.apache.xml.utils.PrefixResolver.getNamespaceForPrefix(String)"/>
			public virtual string getNamespaceForPrefix(string prefix)
			{
				return getNamespaceForPrefix(prefix,null);
			}

			/// <seealso cref="org.apache.xml.utils.PrefixResolver.handlesNullPrefixes()"/>
			public virtual bool handlesNullPrefixes()
			{
				return false;
			}

			/// <seealso cref="org.apache.xml.utils.PrefixResolver.getBaseIdentifier()"/>
			public virtual string BaseIdentifier
			{
				get
				{
					return null;
				}
			}

		}

		/// <summary>
		/// The document to be searched to parallel the case where the XPathEvaluator
		/// is obtained by casting a Document.
		/// </summary>
		private readonly Document m_doc;

		/// <summary>
		/// Constructor for XPathEvaluatorImpl.
		/// </summary>
		/// <param name="doc"> The document to be searched, to parallel the case where''
		///            the XPathEvaluator is obtained by casting the document. </param>
		public XPathEvaluatorImpl(Document doc)
		{
			m_doc = doc;
		}

		/// <summary>
		/// Constructor in the case that the XPath expression can be evaluated
		/// without needing an XML document at all.
		/// 
		/// </summary>
		public XPathEvaluatorImpl()
		{
				m_doc = null;
		}

		/// <summary>
		/// Creates a parsed XPath expression with resolved namespaces. This is 
		/// useful when an expression will be reused in an application since it 
		/// makes it possible to compile the expression string into a more 
		/// efficient internal form and preresolve all namespace prefixes which 
		/// occur within the expression.
		/// </summary>
		/// <param name="expression"> The XPath expression string to be parsed. </param>
		/// <param name="resolver"> The <code>resolver</code> permits translation of 
		///   prefixes within the XPath expression into appropriate namespace URIs
		///   . If this is specified as <code>null</code>, any namespace prefix 
		///   within the expression will result in <code>DOMException</code> 
		///   being thrown with the code <code>NAMESPACE_ERR</code>. </param>
		/// <returns> The compiled form of the XPath expression. </returns>
		/// <exception cref="XPathException">
		///   INVALID_EXPRESSION_ERR: Raised if the expression is not legal 
		///   according to the rules of the <code>XPathEvaluator</code>i </exception>
		/// <exception cref="DOMException">
		///   NAMESPACE_ERR: Raised if the expression contains namespace prefixes 
		///   which cannot be resolved by the specified 
		///   <code>XPathNSResolver</code>.	
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathEvaluator.createExpression(String, XPathNSResolver)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.xpath.XPathExpression createExpression(String expression, org.w3c.dom.xpath.XPathNSResolver resolver) throws XPathException, org.w3c.dom.DOMException
		public XPathExpression createExpression(string expression, XPathNSResolver resolver)
		{

			try
			{

				// If the resolver is null, create a dummy prefix resolver
				XPath xpath = new XPath(expression,null, ((null == resolver) ? new DummyPrefixResolver() : ((PrefixResolver)resolver)), XPath.SELECT);

				return new XPathExpressionImpl(xpath, m_doc);

			}
			catch (TransformerException e)
			{
				// Need to pass back exception code DOMException.NAMESPACE_ERR also.
				// Error found in DOM Level 3 XPath Test Suite.
				if (e is XPathStylesheetDOM3Exception)
				{
					throw new DOMException(DOMException.NAMESPACE_ERR,e.getMessageAndLocation());
				}
				else
				{
					throw new XPathException(XPathException.INVALID_EXPRESSION_ERR,e.getMessageAndLocation());
				}

			}
		}

		/// <summary>
		/// Adapts any DOM node to resolve namespaces so that an XPath expression 
		/// can be easily evaluated relative to the context of the node where it 
		/// appeared within the document. This adapter works like the DOM Level 3 
		/// method <code>lookupNamespaceURI</code> on nodes in resolving the 
		/// namespaceURI from a given prefix using the current information available 
		/// in the node's hierarchy at the time lookupNamespaceURI is called, also 
		/// correctly resolving the implicit xml prefix.
		/// </summary>
		/// <param name="nodeResolver"> The node to be used as a context for namespace 
		///   resolution. </param>
		/// <returns> <code>XPathNSResolver</code> which resolves namespaces with 
		///   respect to the definitions in scope for a specified node.
		/// </returns>
		/// <seealso cref="org.w3c.dom.xpath.XPathEvaluator.createNSResolver(Node)"/>
		public XPathNSResolver createNSResolver(Node nodeResolver)
		{

			return new XPathNSResolverImpl((nodeResolver.getNodeType() == Node.DOCUMENT_NODE) ? ((Document) nodeResolver).getDocumentElement() : nodeResolver);
		}

		/// <summary>
		/// Evaluates an XPath expression string and returns a result of the 
		/// specified type if possible.
		/// </summary>
		/// <param name="expression"> The XPath expression string to be parsed and 
		///   evaluated. </param>
		/// <param name="contextNode"> The <code>context</code> is context node for the 
		///   evaluation of this XPath expression. If the XPathEvaluator was 
		///   obtained by casting the <code>Document</code> then this must be 
		///   owned by the same document and must be a <code>Document</code>, 
		///   <code>Element</code>, <code>Attribute</code>, <code>Text</code>, 
		///   <code>CDATASection</code>, <code>Comment</code>, 
		///   <code>ProcessingInstruction</code>, or <code>XPathNamespace</code> 
		///   node. If the context node is a <code>Text</code> or a 
		///   <code>CDATASection</code>, then the context is interpreted as the 
		///   whole logical text node as seen by XPath, unless the node is empty 
		///   in which case it may not serve as the XPath context. </param>
		/// <param name="resolver"> The <code>resolver</code> permits translation of 
		///   prefixes within the XPath expression into appropriate namespace URIs
		///   . If this is specified as <code>null</code>, any namespace prefix 
		///   within the expression will result in <code>DOMException</code> 
		///   being thrown with the code <code>NAMESPACE_ERR</code>. </param>
		/// <param name="type"> If a specific <code>type</code> is specified, then the 
		///   result will be coerced to return the specified type relying on 
		///   XPath type conversions and fail if the desired coercion is not 
		///   possible. This must be one of the type codes of 
		///   <code>XPathResult</code>. </param>
		/// <param name="result"> The <code>result</code> specifies a specific result 
		///   object which may be reused and returned by this method. If this is 
		///   specified as <code>null</code>or the implementation does not reuse 
		///   the specified result, a new result object will be constructed and 
		///   returned.For XPath 1.0 results, this object will be of type 
		///   <code>XPathResult</code>. </param>
		/// <returns> The result of the evaluation of the XPath expression.For XPath 
		///   1.0 results, this object will be of type <code>XPathResult</code>. </returns>
		/// <exception cref="XPathException">
		///   INVALID_EXPRESSION_ERR: Raised if the expression is not legal 
		///   according to the rules of the <code>XPathEvaluator</code>i
		///   <br>TYPE_ERR: Raised if the result cannot be converted to return the 
		///   specified type. </exception>
		/// <exception cref="DOMException">
		///   NAMESPACE_ERR: Raised if the expression contains namespace prefixes 
		///   which cannot be resolved by the specified 
		///   <code>XPathNSResolver</code>.
		///   <br>WRONG_DOCUMENT_ERR: The Node is from a document that is not 
		///   supported by this XPathEvaluator.
		///   <br>NOT_SUPPORTED_ERR: The Node is not a type permitted as an XPath 
		///   context node.
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathEvaluator.evaluate(String, Node, XPathNSResolver, short, XPathResult)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object evaluate(String expression, org.w3c.dom.Node contextNode, org.w3c.dom.xpath.XPathNSResolver resolver, short type, Object result) throws XPathException, org.w3c.dom.DOMException
		public object evaluate(string expression, Node contextNode, XPathNSResolver resolver, short type, object result)
		{

			XPathExpression xpathExpression = createExpression(expression, resolver);

			return xpathExpression.evaluate(contextNode, type, result);
		}

	}

}