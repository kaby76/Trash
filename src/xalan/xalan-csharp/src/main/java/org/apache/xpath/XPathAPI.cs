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
 * $Id: XPathAPI.java 524807 2007-04-02 15:51:43Z zongaro $
 */
namespace org.apache.xpath
{

	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using PrefixResolverDefault = org.apache.xml.utils.PrefixResolverDefault;
	using XObject = org.apache.xpath.objects.XObject;

	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// The methods in this class are convenience methods into the
	/// low-level XPath API.
	/// These functions tend to be a little slow, since a number of objects must be
	/// created for each evaluation.  A faster way is to precompile the
	/// XPaths using the low-level API, and then just use the XPaths
	/// over and over.
	/// 
	/// NOTE: In particular, each call to this method will create a new
	/// XPathContext, a new DTMManager... and thus a new DTM. That's very
	/// safe, since it guarantees that you're always processing against a
	/// fully up-to-date view of your document. But it's also portentially
	/// very expensive, since you're rebuilding the DTM every time. You should
	/// consider using an instance of CachedXPathAPI rather than these static
	/// methods.
	/// </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xpath">XPath Specification</a> "
	/// />
	public class XPathAPI
	{

	  /// <summary>
	  /// Use an XPath string to select a single node. XPath namespace
	  /// prefixes are resolved from the context node, which may not
	  /// be what you want (see the next method).
	  /// </summary>
	  /// <param name="contextNode"> The node to start searching from. </param>
	  /// <param name="str"> A valid XPath string. </param>
	  /// <returns> The first node found that matches the XPath, or null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.Node selectSingleNode(org.w3c.dom.Node contextNode, String str) throws javax.xml.transform.TransformerException
	  public static Node selectSingleNode(Node contextNode, string str)
	  {
		return selectSingleNode(contextNode, str, contextNode);
	  }

	  /// <summary>
	  /// Use an XPath string to select a single node.
	  /// XPath namespace prefixes are resolved from the namespaceNode.
	  /// </summary>
	  /// <param name="contextNode"> The node to start searching from. </param>
	  /// <param name="str"> A valid XPath string. </param>
	  /// <param name="namespaceNode"> The node from which prefixes in the XPath will be resolved to namespaces. </param>
	  /// <returns> The first node found that matches the XPath, or null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.Node selectSingleNode(org.w3c.dom.Node contextNode, String str, org.w3c.dom.Node namespaceNode) throws javax.xml.transform.TransformerException
	  public static Node selectSingleNode(Node contextNode, string str, Node namespaceNode)
	  {

		// Have the XObject return its result as a NodeSetDTM.
		NodeIterator nl = selectNodeIterator(contextNode, str, namespaceNode);

		// Return the first node, or null
		return nl.nextNode();
	  }

	  /// <summary>
	  ///  Use an XPath string to select a nodelist.
	  ///  XPath namespace prefixes are resolved from the contextNode.
	  /// </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <returns> A NodeIterator, should never be null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.traversal.NodeIterator selectNodeIterator(org.w3c.dom.Node contextNode, String str) throws javax.xml.transform.TransformerException
	  public static NodeIterator selectNodeIterator(Node contextNode, string str)
	  {
		return selectNodeIterator(contextNode, str, contextNode);
	  }

	  /// <summary>
	  ///  Use an XPath string to select a nodelist.
	  ///  XPath namespace prefixes are resolved from the namespaceNode.
	  /// </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <param name="namespaceNode"> The node from which prefixes in the XPath will be resolved to namespaces. </param>
	  ///  <returns> A NodeIterator, should never be null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.traversal.NodeIterator selectNodeIterator(org.w3c.dom.Node contextNode, String str, org.w3c.dom.Node namespaceNode) throws javax.xml.transform.TransformerException
	  public static NodeIterator selectNodeIterator(Node contextNode, string str, Node namespaceNode)
	  {

		// Execute the XPath, and have it return the result
		XObject list = eval(contextNode, str, namespaceNode);

		// Have the XObject return its result as a NodeSetDTM.                
		return list.nodeset();
	  }

	  /// <summary>
	  ///  Use an XPath string to select a nodelist.
	  ///  XPath namespace prefixes are resolved from the contextNode.
	  /// </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <returns> A NodeIterator, should never be null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.NodeList selectNodeList(org.w3c.dom.Node contextNode, String str) throws javax.xml.transform.TransformerException
	  public static NodeList selectNodeList(Node contextNode, string str)
	  {
		return selectNodeList(contextNode, str, contextNode);
	  }

	  /// <summary>
	  ///  Use an XPath string to select a nodelist.
	  ///  XPath namespace prefixes are resolved from the namespaceNode.
	  /// </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <param name="namespaceNode"> The node from which prefixes in the XPath will be resolved to namespaces. </param>
	  ///  <returns> A NodeIterator, should never be null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.NodeList selectNodeList(org.w3c.dom.Node contextNode, String str, org.w3c.dom.Node namespaceNode) throws javax.xml.transform.TransformerException
	  public static NodeList selectNodeList(Node contextNode, string str, Node namespaceNode)
	  {

		// Execute the XPath, and have it return the result
		XObject list = eval(contextNode, str, namespaceNode);

		// Return a NodeList.
		return list.nodelist();
	  }

	  /// <summary>
	  ///  Evaluate XPath string to an XObject.  Using this method,
	  ///  XPath namespace prefixes will be resolved from the namespaceNode. </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <returns> An XObject, which can be used to obtain a string, number, nodelist, etc, should never be null. </returns>
	  ///  <seealso cref="org.apache.xpath.objects.XObject"/>
	  ///  <seealso cref="org.apache.xpath.objects.XNull"/>
	  ///  <seealso cref="org.apache.xpath.objects.XBoolean"/>
	  ///  <seealso cref="org.apache.xpath.objects.XNumber"/>
	  ///  <seealso cref="org.apache.xpath.objects.XString"/>
	  ///  <seealso cref="org.apache.xpath.objects.XRTreeFrag"
	  ////>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject eval(org.w3c.dom.Node contextNode, String str) throws javax.xml.transform.TransformerException
	  public static XObject eval(Node contextNode, string str)
	  {
		return eval(contextNode, str, contextNode);
	  }

	  /// <summary>
	  ///  Evaluate XPath string to an XObject.
	  ///  XPath namespace prefixes are resolved from the namespaceNode.
	  ///  The implementation of this is a little slow, since it creates
	  ///  a number of objects each time it is called.  This could be optimized
	  ///  to keep the same objects around, but then thread-safety issues would arise.
	  /// </summary>
	  ///  <param name="contextNode"> The node to start searching from. </param>
	  ///  <param name="str"> A valid XPath string. </param>
	  ///  <param name="namespaceNode"> The node from which prefixes in the XPath will be resolved to namespaces. </param>
	  ///  <returns> An XObject, which can be used to obtain a string, number, nodelist, etc, should never be null. </returns>
	  ///  <seealso cref="org.apache.xpath.objects.XObject"/>
	  ///  <seealso cref="org.apache.xpath.objects.XNull"/>
	  ///  <seealso cref="org.apache.xpath.objects.XBoolean"/>
	  ///  <seealso cref="org.apache.xpath.objects.XNumber"/>
	  ///  <seealso cref="org.apache.xpath.objects.XString"/>
	  ///  <seealso cref="org.apache.xpath.objects.XRTreeFrag"
	  ////>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject eval(org.w3c.dom.Node contextNode, String str, org.w3c.dom.Node namespaceNode) throws javax.xml.transform.TransformerException
	  public static XObject eval(Node contextNode, string str, Node namespaceNode)
	  {

		// Since we don't have a XML Parser involved here, install some default support
		// for things like namespaces, etc.
		// (Changed from: XPathContext xpathSupport = new XPathContext();
		//    because XPathContext is weak in a number of areas... perhaps
		//    XPathContext should be done away with.)
		// Create an XPathContext that doesn't support pushing and popping of
		// variable resolution scopes.  Sufficient for simple XPath 1.0 expressions.
		XPathContext xpathSupport = new XPathContext(false);

		// Create an object to resolve namespace prefixes.
		// XPath namespaces are resolved from the input context node's document element
		// if it is a root node, or else the current context node (for lack of a better
		// resolution space, given the simplicity of this sample code).
		PrefixResolverDefault prefixResolver = new PrefixResolverDefault((namespaceNode.getNodeType() == Node.DOCUMENT_NODE) ? ((Document) namespaceNode).getDocumentElement() : namespaceNode);

		// Create the XPath object.
		XPath xpath = new XPath(str, null, prefixResolver, XPath.SELECT, null);

		// Execute the XPath, and have it return the result
		// return xpath.execute(xpathSupport, contextNode, prefixResolver);
		int ctxtNode = xpathSupport.getDTMHandleFromNode(contextNode);

		return xpath.execute(xpathSupport, ctxtNode, prefixResolver);
	  }

	  /// <summary>
	  ///   Evaluate XPath string to an XObject.
	  ///   XPath namespace prefixes are resolved from the namespaceNode.
	  ///   The implementation of this is a little slow, since it creates
	  ///   a number of objects each time it is called.  This could be optimized
	  ///   to keep the same objects around, but then thread-safety issues would arise.
	  /// </summary>
	  ///   <param name="contextNode"> The node to start searching from. </param>
	  ///   <param name="str"> A valid XPath string. </param>
	  ///   <param name="prefixResolver"> Will be called if the parser encounters namespace
	  ///                         prefixes, to resolve the prefixes to URLs. </param>
	  ///   <returns> An XObject, which can be used to obtain a string, number, nodelist, etc, should never be null. </returns>
	  ///   <seealso cref="org.apache.xpath.objects.XObject"/>
	  ///   <seealso cref="org.apache.xpath.objects.XNull"/>
	  ///   <seealso cref="org.apache.xpath.objects.XBoolean"/>
	  ///   <seealso cref="org.apache.xpath.objects.XNumber"/>
	  ///   <seealso cref="org.apache.xpath.objects.XString"/>
	  ///   <seealso cref="org.apache.xpath.objects.XRTreeFrag"
	  ////>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject eval(org.w3c.dom.Node contextNode, String str, org.apache.xml.utils.PrefixResolver prefixResolver) throws javax.xml.transform.TransformerException
	  public static XObject eval(Node contextNode, string str, PrefixResolver prefixResolver)
	  {

		// Since we don't have a XML Parser involved here, install some default support
		// for things like namespaces, etc.
		// (Changed from: XPathContext xpathSupport = new XPathContext();
		//    because XPathContext is weak in a number of areas... perhaps
		//    XPathContext should be done away with.)
		// Create the XPath object.
		XPath xpath = new XPath(str, null, prefixResolver, XPath.SELECT, null);

		// Create an XPathContext that doesn't support pushing and popping of
		// variable resolution scopes.  Sufficient for simple XPath 1.0 expressions.
		XPathContext xpathSupport = new XPathContext(false);

		// Execute the XPath, and have it return the result
		int ctxtNode = xpathSupport.getDTMHandleFromNode(contextNode);

		return xpath.execute(xpathSupport, ctxtNode, prefixResolver);
	  }
	}

}